## AggressiveGadgetToJScript
We created this aggressor script in order to automate the generation of payloads using the GadgetToJScript technique.

For the purposes of this release, we used a common injector that implements the QueueUserAPC injection method and injects to notepad.exe.

Feel free to use your own templates.

Additionally, the shellcode generated is compressed before being used in the injector template.

---

### Usage
* Install mono framework: `apt install mono-complete`.
* Set the path variables inside `GadgetToJScript.cna`:
	* `$toolpath` is the absolute path of the installation directory.
	* `$outpath` is the directory used to output all generated artifacts.
	* `$python3` is the absolute path of python3 binary.
	* `$gzip` is the absolute path of gzip binary.
	* `$mcs` is the absolute path of mcs binary.
* Load cna into CobaltStrike.
* A new menu `CustomPayloads` will appear. Generate the payload choosing listener, staged / stageless, architecture. Payload will be stored in your defined `$outpath`.
* Due to `ConfigurationManager.AppSettings` being readonly in Mono (https://github.com/mono/mono/issues/11751), we have to copy the generated EXE file into a windows box and execute it. 
* The final GadgetToJscript payload (.js) will be generated. Currently using the reg-free template from GadgetToJscript. 

### Configuration:
```
$toolpath = "/opt/cobaltstrike/custom/AggressiveGadgetToJScript";
$outpath = "/tmp/payloads";
$python3 = "/usr/bin/python3";
$gzip = "/usr/bin/gzip";
$mcs = "/usr/bin/mcs";
```
---

### Caveats
* Payload generated (.js) gets flagged by AV. Consider obfuscating `/Templates/GadgetToJScript.js`. As a PoC we opted using a powerful royal-like technique directly stolen from Caesar!
* Shellcode can also be encrypted by placing an encrypt function in Helper.py and a decrypt function in `/Templates/Injector.cs`
* Injection method can be replaced in `/Templates/Injector.cs`. Just make sure to place it in the constructor of the class.

---

### Authors

* [@eksperience](https://github.com/eksperience)

* [@leftp](https://github.com/leftp)

---

### Credits

This tool is based on:

* Original code of GadgetToJScript from @med0x2e - https://github.com/med0x2e/GadgetToJScript

* Sample Injector used from @pwndizzle - https://github.com/pwndizzle/c-sharp-memory-injection/blob/master/apc-injection-new-process.cs
