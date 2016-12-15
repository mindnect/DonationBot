using JSAssist;
using System;
using System.Xml;

namespace JSAssist.Widget
{
	internal class WidgetChatPreset
	{
		public string presetName = "default";

		public string theme = "default";

		public bool platformIcon = true;

		public string platform = "all";

		public string animation = "fade";

		public float chatFade = 10f;

		public string font = "Jeju Gothic";

		public float fontUsernameSize = 14f;

		public string fontUsernameColor = "255, 255, 255";

		public float fontChatSize = 16f;

		public string fontChatColor = "255, 255, 255";

		public string backgroundColor = "255, 255, 255";

		public int backgroundAlpha;

		public string chatBackgroundColor = "255, 255, 255";

		public int chatBackgroundAlpha = 25;

		public WidgetChatPreset()
		{
		}

		public void CopyFrom(WidgetChatPreset preset)
		{
			this.theme = preset.theme;
			this.platformIcon = preset.platformIcon;
			this.platform = preset.platform;
			this.animation = preset.animation;
			this.chatFade = preset.chatFade;
			this.font = preset.font;
			this.fontUsernameSize = preset.fontUsernameSize;
			this.fontUsernameColor = preset.fontUsernameColor;
			this.fontChatSize = preset.fontChatSize;
			this.fontChatColor = preset.fontChatColor;
			this.backgroundColor = preset.backgroundColor;
			this.backgroundAlpha = preset.backgroundAlpha;
			this.chatBackgroundColor = preset.chatBackgroundColor;
			this.chatBackgroundAlpha = preset.chatBackgroundAlpha;
		}

		private string GetJsonElement(string element, string value, bool isEnd = false)
		{
			return string.Format("\"{0}\" : {1}{2}", element, value, (isEnd ? "" : ", "));
		}

		private string GetJsonElementString(string element, string value, bool isEnd = false)
		{
			return string.Format("\"{0}\" : \"{1}\"{2}", element, value, (isEnd ? "" : ", "));
		}

		public void LoadFromXML(XmlNode root)
		{
			this.presetName = XMLFile.GetAttributeString(root, "name", "default");
			this.theme = XMLFile.GetValueString(root, "Theme", "default");
			this.platformIcon = XMLFile.GetValueBool(root, "PlatformIcon", true);
			this.platform = XMLFile.GetValueString(root, "Platform", "all");
			this.animation = XMLFile.GetValueString(root, "Animation", "fade");
			this.chatFade = XMLFile.GetValueFloat(root, "ChatFade", 10f);
			this.font = XMLFile.GetValueString(root, "Font", "Jeju Gothic");
			this.fontUsernameSize = XMLFile.GetValueFloat(root, "FontUsernameSize", 14f);
			this.fontUsernameColor = XMLFile.GetValueString(root, "FontUsernameColor", "255, 255, 255");
			this.fontChatSize = XMLFile.GetValueFloat(root, "FontChatSize", 16f);
			this.fontChatColor = XMLFile.GetValueString(root, "FontChatColor", "255, 255, 255");
			this.backgroundColor = XMLFile.GetValueString(root, "BackgroundColor", "255, 255, 255");
			this.backgroundAlpha = XMLFile.GetValueInt(root, "BackgroundAlpha", 0);
			this.chatBackgroundColor = XMLFile.GetValueString(root, "ChatBackgroundColor", "255, 255, 255");
			this.chatBackgroundAlpha = XMLFile.GetValueInt(root, "ChatBackgroundAlpha", 25);
		}

		public void SaveToXML(XmlWriter writer)
		{
			writer.WriteStartElement("Preset");
			writer.WriteAttributeString("name", this.presetName);
			XMLFile.WriteElementValue(writer, "Theme", this.theme);
			XMLFile.WriteElementValue(writer, "PlatformIcon", this.platformIcon.ToString());
			XMLFile.WriteElementValue(writer, "Platform", this.platform);
			XMLFile.WriteElementValue(writer, "Animation", this.animation);
			XMLFile.WriteElementValue(writer, "ChatFade", this.chatFade.ToString());
			XMLFile.WriteElementValue(writer, "Font", this.font);
			XMLFile.WriteElementValue(writer, "FontUsernameSize", this.fontUsernameSize.ToString());
			XMLFile.WriteElementValue(writer, "FontUsernameColor", this.fontUsernameColor);
			XMLFile.WriteElementValue(writer, "FontChatSize", this.fontChatSize.ToString());
			XMLFile.WriteElementValue(writer, "FontChatColor", this.fontChatColor);
			XMLFile.WriteElementValue(writer, "BackgroundColor", this.backgroundColor);
			XMLFile.WriteElementValue(writer, "BackgroundAlpha", this.backgroundAlpha.ToString());
			XMLFile.WriteElementValue(writer, "ChatBackgroundColor", this.chatBackgroundColor);
			XMLFile.WriteElementValue(writer, "ChatBackgroundAlpha", this.chatBackgroundAlpha.ToString());
			writer.WriteEndElement();
		}

		public string ToJson()
		{
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("{", this.GetJsonElementString("type", "config", false)), this.GetJsonElementString("presetName", this.presetName, false)), this.GetJsonElementString("theme", this.theme, false)), this.GetJsonElement("platformIcon", this.platformIcon.ToString().ToLower(), false)), this.GetJsonElementString("platform", this.platform, false)), this.GetJsonElementString("animation", this.animation, false)), this.GetJsonElement("chatFade", this.chatFade.ToString(), false)), this.GetJsonElementString("font", this.font, false)), this.GetJsonElement("fontUsernameSize", this.fontUsernameSize.ToString(), false)), this.GetJsonElementString("fontUsernameColor", this.fontUsernameColor, false)), this.GetJsonElement("fontChatSize", this.fontChatSize.ToString(), false)), this.GetJsonElementString("fontChatColor", this.fontChatColor, false)), this.GetJsonElementString("backgroundColor", this.backgroundColor, false)), this.GetJsonElement("backgroundAlpha", this.backgroundAlpha.ToString(), false)), this.GetJsonElementString("chatBackgroundColor", this.chatBackgroundColor, false)), this.GetJsonElement("chatBackgroundAlpha", this.chatBackgroundAlpha.ToString(), true)), "}");
		}
	}
}