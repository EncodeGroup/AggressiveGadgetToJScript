// Based on https://github.com/med0x2e/GadgetToJScript/blob/master/GadgetToJScript/_SurrogateSelector.cs
using System;
using System.Runtime.Serialization;

namespace GadgetToJScript
{
	class SurrogateSelector : System.Runtime.Serialization.SurrogateSelector
	{
		public override ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			selector = this;

			if (!type.IsSerializable)
			{
				Type t = Type.GetType("System.Workflow.ComponentModel.Serialization.ActivitySurrogateSelector+ObjectSurrogate, System.Workflow.ComponentModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				return (ISerializationSurrogate)Activator.CreateInstance(t);
			}

			return base.GetSurrogate(type, context, out selector);
		}
	}
}
