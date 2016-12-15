using MSHTML;
using System;
using System.Windows.Forms;

namespace JSAssist
{
	internal class ChatBrowserBase : ChatBase
	{
		protected WebBrowser browser;

		public ChatBrowserBase()
		{
		}

		public void BindBrowser(WebBrowser browser)
		{
			this.browser = browser;
		}

		public void SetURL(string url)
		{
			this.browser.Navigate(url);
			this.document = (IHTMLDocument2)this.browser.Document.DomDocument;
		}
	}
}