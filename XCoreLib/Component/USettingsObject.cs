using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using XCore.InnerAnalyser;

namespace XCore.Component
{
    /// <summary>
    /// 轻量设置类,为可读写属性提供读取写入方法.
    /// 支持基础类型自定义类型及集合类型(仅支持<see cref="Array"/>,<see cref="List{T}"/>和<see cref="Dictionary{TKey, TValue}"/>).
    /// </summary>    
    public abstract class USettingsObject : USettingsBase,IXSerializable
    {
        protected override string Comment => "设置类1.0.4.0版本,基于USettingsObject.";
        public override void Load()
        {
            if (File.Exists(FileName))
            {
                XDocument xDocument;
                try
                {
                    xDocument = XDocument.Load(FileName);
                }
                catch (Exception)//>>说明文件被破坏
                {
                    File.Delete(FileName);
                    return;
                }
                ((IXSerializable)this).DeSerialize(xDocument.Root);
            }

        }
        public override void Save()
        {     
            XElement xElement = ((IXSerializable)this).Serialize();
            XDocument xDocument = new XDocument(new XComment(Comment), xElement);
            xDocument.Save(FileName);
        }
        
        /// <summary>
        /// 序列化方法.
        /// </summary>
        /// <returns></returns>
        XElement IXSerializable.Serialize()
        {
            XElement xElement = new XElement(this.GetType().ToString());
            //XDocument xDocument = CreateXml(this.GetType().ToString());
            foreach (var propertyInfo in GetType().GetProperties())
            {
                XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, this));
                if (element!=null)
                {
                    xElement.Add(element);
                }
            }
            return xElement;
        }
        void IXSerializable.DeSerialize(XElement xElement)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) == null &&propertyInfo.CanWrite && propertyInfo.CanRead)
                {

                    string elementName = propertyInfo.Name;
                    XmlElementAttribute attribute = (XmlElementAttribute)propertyInfo.GetCustomAttribute(typeof(XmlElementAttribute));
                    if (attribute != null)
                    {
                        elementName = attribute.ElementName;
                    }

                    XElement element = xElement.Element(elementName);
                    Type contentType = Type.GetType(element.Attribute("type").Value);
                    if (!IsBaseTypeOfType(contentType,propertyInfo.GetType()))
                    {
                        contentType = propertyInfo.GetType();
                    }
                    if (element != null)
                    {
                        object o = ToObject(element, contentType);
                        propertyInfo.SetValue(this, o);
                    }
                }
            }
        }
    }
}
