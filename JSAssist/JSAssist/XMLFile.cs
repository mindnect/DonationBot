using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace JSAssist
{
	internal class XMLFile
	{
		protected string filename;

		protected XmlDocument document;

		public XMLFile(string filename)
		{
			this.filename = filename;
			this.LoadFile(filename);
		}

		public static bool GetAttributeBool(XmlNode node, string attributeName, bool defaultValue = false)
		{
			bool flag;
			string attributeString = XMLFile.GetAttributeString(node, attributeName, null);
			if (attributeString == null)
			{
				return defaultValue;
			}
			try
			{
				flag = bool.Parse(attributeString);
			}
			catch
			{
				flag = defaultValue;
			}
			return flag;
		}

		public static int GetAttributeInt(XmlNode node, string attributeName, int defaultValue = 0)
		{
			int num;
			string attributeString = XMLFile.GetAttributeString(node, attributeName, null);
			if (attributeString == null)
			{
				return defaultValue;
			}
			try
			{
				num = int.Parse(attributeString);
			}
			catch
			{
				num = defaultValue;
			}
			return num;
		}

		public static string GetAttributeString(XmlNode node, string attributeName, string defaultValue = null)
		{
			string value;
			IEnumerator enumerator = node.Attributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlAttribute current = (XmlAttribute)enumerator.Current;
					if (current.Name != attributeName)
					{
						continue;
					}
					value = current.Value;
					return value;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return value;
		}

		public static string GetComponentTypeFromNode(XmlNode node)
		{
			string value;
			IEnumerator enumerator = node.Attributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlAttribute current = (XmlAttribute)enumerator.Current;
					if (current.Name != "Type")
					{
						continue;
					}
					value = current.Value;
					return value;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return value;
		}

		public static bool GetValueBool(XmlNode node, string nodeName, bool defaultValue = false)
		{
			string valueString = XMLFile.GetValueString(node, nodeName, null);
			if (valueString == null)
			{
				return defaultValue;
			}
			bool flag = defaultValue;
			try
			{
				flag = bool.Parse(valueString);
			}
			catch
			{
			}
			return flag;
		}

		public static float GetValueFloat(XmlNode node, string nodeName, float defaultValue = 0f)
		{
			string valueString = XMLFile.GetValueString(node, nodeName, null);
			if (valueString == null)
			{
				return defaultValue;
			}
			float single = defaultValue;
			try
			{
				single = float.Parse(valueString);
			}
			catch
			{
			}
			return single;
		}

		public static int GetValueInt(XmlNode node, string nodeName, int defaultValue = 0)
		{
			string valueString = XMLFile.GetValueString(node, nodeName, null);
			if (valueString == null)
			{
				return defaultValue;
			}
			int num = defaultValue;
			try
			{
				num = int.Parse(valueString);
			}
			catch
			{
			}
			return num;
		}

		public static string GetValueString(XmlNode node, string nodeName, string defaultValue = null)
		{
			XmlNode xmlNodes = node.SelectSingleNode(nodeName);
			if (xmlNodes == null)
			{
				return defaultValue;
			}
			return xmlNodes.InnerText;
		}

		private bool LoadFile(string filename)
		{
			if (!File.Exists(filename))
			{
				File.Create(filename).Close();
				this.OnSetDefaultValue();
				this.SaveFile();
			}
			this.document = new XmlDocument();
			this.document.Load(filename);
			XmlNode xmlNodes = this.document.SelectSingleNode("JSAssist");
			if (xmlNodes == null)
			{
				return false;
			}
			return this.OnLoadFile(xmlNodes);
		}

		protected virtual bool OnLoadFile(XmlNode root)
		{
			return true;
		}

		protected virtual bool OnSaveFile(XmlTextWriter writer)
		{
			return true;
		}

		protected virtual void OnSetDefaultValue()
		{
		}

		public bool SaveFile()
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(this.filename, Encoding.UTF8);
			if (xmlTextWriter == null)
			{
				return false;
			}
			xmlTextWriter.WriteStartDocument(true);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = 2;
			xmlTextWriter.WriteStartElement("JSAssist");
			if (!this.OnSaveFile(xmlTextWriter))
			{
				return false;
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndDocument();
			xmlTextWriter.Close();
			return true;
		}

		public static void WriteElementValue(XmlWriter writer, string elementName, string value)
		{
			writer.WriteStartElement(elementName);
			writer.WriteValue(value);
			writer.WriteEndElement();
		}
	}
}