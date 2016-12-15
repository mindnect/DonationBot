using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Verse
{
	public static class XmlLoaderSimple
	{
		[DebuggerHidden]
		public static IEnumerable<KeyValuePair<string, string>> ValuesFromXmlFile(FileInfo file)
		{
			XmlLoaderSimple.<ValuesFromXmlFile>c__Iterator1EE <ValuesFromXmlFile>c__Iterator1EE = new XmlLoaderSimple.<ValuesFromXmlFile>c__Iterator1EE();
			<ValuesFromXmlFile>c__Iterator1EE.file = file;
			<ValuesFromXmlFile>c__Iterator1EE.<$>file = file;
			XmlLoaderSimple.<ValuesFromXmlFile>c__Iterator1EE expr_15 = <ValuesFromXmlFile>c__Iterator1EE;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
