using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XCore.Component
{
    /// <summary>
    /// 控制<see cref="IXSettings.XSerialize(object, string, XSerializeOption, Predicate{string})"/>的操作.
    /// </summary>
    [Obsolete]
    public enum XSerializeOption
    {
        Serialize,
        DeSerialize
    }

    [Obsolete]
    public class XSerializer<T>
    {
        public XSerializer(T reference, string fileName, Predicate<string> condition = null)
        {
            Reference = reference;
            FileName = fileName;
            Condition = condition;
        }

        public T Reference { get; }
        public string FileName { get; set; }
        public Predicate<string> Condition { get; set; }

        /// <summary>
        /// 用序列化方法加载reference的成员.
        /// </summary>
        /// <param name="reference">引用.</param>
        /// <param name="fileName">xml文件名.</param>
        /// <param name="option">操作.</param>
        /// <param name="condition">判断条件,用以忽略不必要的属性.</param>
        internal static void XSerialize(object reference, string fileName, XSerializeOption option, Predicate<string> condition = null)
        {
            if (option == XSerializeOption.Serialize)
            {
                string comment = "设置类1.0.4.0版本,基于USettingsObject.";

                XElement xElement = new XElement(reference.GetType().ToString());
                foreach (var propertyInfo in reference.GetType().GetProperties())
                {
                    if (condition == null || condition(propertyInfo.Name))
                    {
                        XElement element = IXSettingsExtension.ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, reference));
                        if (element != null)
                        {
                            xElement.Add(element);
                        }
                    }
                }

                XDocument xDocument = new XDocument(new XComment(comment), xElement);
                xDocument.Save(fileName);
            }
            else
            {
                if (File.Exists(fileName))
                {
                    XDocument xDocument;
                    try
                    {
                        xDocument = XDocument.Load(fileName);
                    }
                    catch (Exception)//>>说明文件被破坏
                    {
                        File.Delete(fileName);
                        return;
                    }

                    foreach (var propertyInfo in reference.GetType().GetProperties())
                    {
                        if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) == null && propertyInfo.CanWrite && propertyInfo.CanRead)
                        {

                            string elementName = propertyInfo.Name;
                            XmlElementAttribute attribute = (XmlElementAttribute)propertyInfo.GetCustomAttribute(typeof(XmlElementAttribute));
                            if (attribute != null)
                            {
                                elementName = attribute.ElementName;
                            }

                            XElement element = xDocument.Root.Element(elementName);

                            if (element != null)
                            {
                                if (condition == null || condition(elementName))
                                {
                                    object o = IXSettingsExtension.ToObject(element, propertyInfo.PropertyType);
                                    propertyInfo.SetValue(reference, o);
                                }

                            }
                        }
                    }
                }
            }
        }

        public void DeSerialize()
        {
            XSerialize(Reference, FileName, XSerializeOption.DeSerialize, Condition);
        }

        public void Serialize()
        {
            XSerialize(Reference, FileName, XSerializeOption.Serialize, Condition);
        }
    }

}
