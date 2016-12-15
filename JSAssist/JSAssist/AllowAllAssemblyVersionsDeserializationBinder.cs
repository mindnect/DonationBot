using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace JSAssist
{
	internal sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
	{
		public AllowAllAssemblyVersionsDeserializationBinder()
		{
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			return Type.GetType(string.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
		}
	}
}