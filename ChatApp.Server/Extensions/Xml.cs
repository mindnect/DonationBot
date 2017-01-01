using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ChatApp.Server.Extensions
{
    public static class Xml
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {OmitXmlDeclaration = true};
        private static readonly XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces(new[] {new XmlQualifiedName("", "")});


        public static string Serialize<T>(this T value)
        {
            if (value == null) return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, WriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, value, Namespaces);
                    return stringWriter.ToString();
                }
            }
        }

        public static T Deserialize<T>(string value) where T : class
        {
            if (string.IsNullOrEmpty(value)) return null;
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(value))
            {
                return (T) xmlSerializer.Deserialize(sr);
            }
        }
    }
}