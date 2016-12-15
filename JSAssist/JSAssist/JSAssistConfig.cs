using JSAssist.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace JSAssist
{
	internal class JSAssistConfig : XMLFile
	{
		public int build;

		public bool skipPatchNote;

		public bool chatTwitchEnable;

		public string chatTwitchID;

		public bool chatYoutubeEnable;

		public string chatYoutubeID;

		public bool chatTVPotEnable;

		public bool widgetChatEnable;

		public List<WidgetChatPreset> listChatPreset = new List<WidgetChatPreset>();

		public JSAssistConfig(string filename) : base(filename)
		{
		}

		protected override bool OnLoadFile(XmlNode root)
		{
			if (!base.OnLoadFile(root))
			{
				return false;
			}
			XmlNode xmlNodes = root.SelectSingleNode("ApplicationInfo");
			if (xmlNodes == null)
			{
				return false;
			}
			this.build = XMLFile.GetAttributeInt(xmlNodes, "version", Program.currentVersion);
			this.skipPatchNote = XMLFile.GetAttributeBool(xmlNodes, "skip_patch_note", false);
			XmlNode xmlNodes1 = root.SelectSingleNode("ChatIntegration");
			if (xmlNodes1 == null)
			{
				return false;
			}
			XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode("Twitch");
			if (xmlNodes2 == null)
			{
				return false;
			}
			this.chatTwitchEnable = XMLFile.GetAttributeBool(xmlNodes2, "enable", false);
			this.chatTwitchID = XMLFile.GetAttributeString(xmlNodes2, "id", "");
			XmlNode xmlNodes3 = xmlNodes1.SelectSingleNode("Youtube");
			if (xmlNodes3 == null)
			{
				return false;
			}
			this.chatYoutubeEnable = XMLFile.GetAttributeBool(xmlNodes3, "enable", false);
			this.chatYoutubeID = XMLFile.GetAttributeString(xmlNodes3, "id", "");
			XmlNode xmlNodes4 = xmlNodes1.SelectSingleNode("TVPot");
			if (xmlNodes4 == null)
			{
				return false;
			}
			this.chatTVPotEnable = XMLFile.GetAttributeBool(xmlNodes4, "enable", false);
			XmlNode xmlNodes5 = root.SelectSingleNode("Widgets");
			if (xmlNodes5 == null)
			{
				return false;
			}
			XmlNode xmlNodes6 = xmlNodes5.SelectSingleNode("ChatWidget");
			if (xmlNodes6 == null)
			{
				return false;
			}
			this.widgetChatEnable = XMLFile.GetAttributeBool(xmlNodes6, "enable", false);
			foreach (XmlNode xmlNodes7 in xmlNodes6.SelectNodes("Preset"))
			{
				WidgetChatPreset widgetChatPreset = new WidgetChatPreset();
				widgetChatPreset.LoadFromXML(xmlNodes7);
				this.listChatPreset.Add(widgetChatPreset);
			}
			if (this.listChatPreset.Count == 0)
			{
				WidgetChatPreset widgetChatPreset1 = new WidgetChatPreset();
				this.listChatPreset.Add(widgetChatPreset1);
			}
			return true;
		}

		protected override bool OnSaveFile(XmlTextWriter writer)
		{
			if (!base.OnSaveFile(writer))
			{
				return false;
			}
			writer.WriteStartElement("ApplicationInfo");
			writer.WriteAttributeString("version", this.build.ToString());
			writer.WriteAttributeString("skip_patch_note", this.skipPatchNote.ToString());
			writer.WriteEndElement();
			writer.WriteStartElement("ChatIntegration");
			writer.WriteStartElement("Twitch");
			writer.WriteAttributeString("enable", this.chatTwitchEnable.ToString());
			writer.WriteAttributeString("id", this.chatTwitchID);
			writer.WriteEndElement();
			writer.WriteStartElement("Youtube");
			writer.WriteAttributeString("enable", this.chatYoutubeEnable.ToString());
			writer.WriteAttributeString("id", this.chatYoutubeID);
			writer.WriteEndElement();
			writer.WriteStartElement("TVPot");
			writer.WriteAttributeString("enable", this.chatTVPotEnable.ToString());
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("Widgets");
			writer.WriteStartElement("ChatWidget");
			writer.WriteAttributeString("enable", this.widgetChatEnable.ToString());
			foreach (WidgetChatPreset widgetChatPreset in this.listChatPreset)
			{
				widgetChatPreset.SaveToXML(writer);
			}
			writer.WriteEndElement();
			return true;
		}

		protected override void OnSetDefaultValue()
		{
			base.OnSetDefaultValue();
			this.build = Program.currentVersion;
			this.skipPatchNote = false;
			this.chatTwitchEnable = false;
			this.chatTwitchID = "";
			this.chatYoutubeEnable = false;
			this.chatYoutubeID = "";
			this.chatTVPotEnable = false;
			this.widgetChatEnable = true;
		}
	}
}