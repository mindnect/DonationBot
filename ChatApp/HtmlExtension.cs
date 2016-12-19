using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ChatApp
{
    public static class HtmlExtension
    {
        public static IEnumerable<HtmlAttribute> GetChildsAttributes(this HtmlNode _this, string name)
        {
            var ret = _this.ChildNodes.SelectMany(child => child.ChildAttributes(name));
            return ret;
        }

        public static IEnumerable<HtmlAttribute> GetRecursiveAttributes(this HtmlNode _this, string name)
        {
            foreach (var childAttribute in _this.ChildAttributes(name))
                yield return childAttribute;

            foreach (var ret in _this.ChildNodes.SelectMany(child => child.GetRecursiveAttributes(name)))
                yield return ret;
        }

        //public static string Get
    }
}