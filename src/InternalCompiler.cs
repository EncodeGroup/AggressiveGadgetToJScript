//Based on https://github.com/med0x2e/GadgetToJScript/blob/master/GadgetToJScript/TestAssemblyLoader.cs
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace GadgetToJScript
{
	class InternalCompiler
	{
		public static Assembly compile(string source)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters();
			parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add("System.Core.dll");
			parameters.ReferencedAssemblies.Add("System.IO.Compression.dll");
			parameters.ReferencedAssemblies.Add("System.Management.dll");
			CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);

			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder();

				foreach (CompilerError error in results.Errors)
				{
					sb.AppendLine(string.Format("Error ({0}): {1}: {2}", error.ErrorNumber, error.ErrorText, error.Line));
				}

				throw new InvalidOperationException(sb.ToString());
			}

			Assembly compiled = results.CompiledAssembly;
			return compiled;
		}
	}
}
