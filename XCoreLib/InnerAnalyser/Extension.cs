using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XCore.InnerAnalyser
{
    public static class Extension
    {
        public static IXmlSerializable AsIXmlSerializable<T>(this List<T> list)
        {
            throw new Exception();
        }
        public static string GetAssemblyQualifiedName(this Type type)
        {
            if (type.Assembly == System.Reflection.Assembly.GetAssembly(typeof(int)))
            {
                return type.ToString();
            }
            else
            {
                return type.AssemblyQualifiedName;
            }
        }
    }
    internal class XmlSerializableList<T> : IXmlSerializable
    {
        public List<T> list;

        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("List");

        }
    }
}
