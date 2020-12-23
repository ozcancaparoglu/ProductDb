using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PMS.LogoService.Helper
{
    public class XmlSerializerHelper
    {
        public static string ParseToXML<T>(T t) where T : class
        {
            try
            {
                var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;
                var serializer = new XmlSerializer(t.GetType());

                using (var stream = new System.IO.StringWriter())
                {
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.Serialize(writer, t, emptyNamespaces);
                        return stream.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ICollection<T> ParseXMLToObject<T>(string XML, string startFromNode) where T : class
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(XML);
                var collection = new List<T>();

                XmlNodeList nodeList = xmlDoc.SelectNodes(startFromNode);

                for (int i = 0; i < nodeList.Count; i++)
                    collection.Add(ConvertNode<T>(nodeList[i]));

                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static T ConvertNode<T>(XmlNode node) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(node.OuterXml))
            {
                return (T)ser.Deserialize(sr);
            }
        }

    }
}
