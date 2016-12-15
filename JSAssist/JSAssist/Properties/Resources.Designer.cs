using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace JSAssist.Properties
{
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Resources
	{
		private static System.Resources.ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		internal static Bitmap chat_setting_twitch
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("chat_setting_twitch", Resources.resourceCulture);
			}
		}

		internal static Bitmap chat_setting_youtube_1
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("chat_setting_youtube_1", Resources.resourceCulture);
			}
		}

		internal static Bitmap chat_setting_youtube_2
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("chat_setting_youtube_2", Resources.resourceCulture);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		internal static Bitmap donate
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("donate", Resources.resourceCulture);
			}
		}

		internal static Icon icon_jsa_core
		{
			get
			{
				return (Icon)Resources.ResourceManager.GetObject("icon_jsa_core", Resources.resourceCulture);
			}
		}

		internal static Bitmap logo
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("logo", Resources.resourceCulture);
			}
		}

		internal static Bitmap logo_core
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("logo_core", Resources.resourceCulture);
			}
		}

		internal static Bitmap mail
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("mail", Resources.resourceCulture);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new System.Resources.ResourceManager("JSAssist.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		internal Resources()
		{
		}
	}
}