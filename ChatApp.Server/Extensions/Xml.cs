using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ChatApp.Server.Models;

namespace ChatApp.Server.Extensions
{
    public static class Xml
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {OmitXmlDeclaration = true};
        private static readonly XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces(new[] {new XmlQualifiedName("", "")});


        public static string Serialize(this Packet value)
        {
            if (value == null) return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(Packet));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, WriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, value, Namespaces);
                    return stringWriter.ToString();
                }
            }
        }

        public static Packet Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var xmlSerializer = new XmlSerializer(typeof(Packet));

            using (var sr = new StringReader(value))
            {
                return (Packet) xmlSerializer.Deserialize(sr);
            }
        }
    }
}