using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using mshtml;

namespace ChatApp.Extension
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

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? (value == null ? defaultValue : value) : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? (value == null ? defaultValueProvider() : value) : defaultValueProvider();
        }


        public static IHTMLElement FindElementWithClassName(DispHTMLDocument document, string targetClassName)
        {
            foreach (IHTMLElement child in document.body.children)
            {
                if (child.className == targetClassName)
                {
                    return child;
                }
            }
            return null;
        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if ((obj == null) || (obj == DBNull.Value))
            {
                return default(T); // returns the default value for the type
            }
            return (T) obj;
        }

        public static void SetElemenentAsParsed(IHTMLElement elem)
        {
            elem.setAttribute("parsed", true);
        }

        public static bool GetIsParsedOfElement(IHTMLElement elem)
        {
            var value = elem.getAttribute("parsed");
            return ConvertFromDBVal<bool>(value);
        }
    }
}