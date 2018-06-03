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
    /// 设置类基础类,提供必须功能.
    /// 并提供静态序列化方法.
    /// </summary>
    public abstract class USettingsBase : XmlBase
    {
        /// <summary>
        ///将对象转化为等价<see cref="XElement"/>.
        /// </summary>
        /// <param name="inobj">传入值,支持属性和单元值.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static XElement ToXElement(object inobj, string name = "add")
        {
            string elementName = name;
            Type type;
            object value;
            if (inobj is KeyValuePair<PropertyInfo, object> pair)
            {
                PropertyInfo propertyInfo = pair.Key;
                if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) == null && propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    elementName = propertyInfo.Name;
                    XmlElementAttribute attribute = (XmlElementAttribute)propertyInfo.GetCustomAttribute(typeof(XmlElementAttribute));
                    if (attribute != null)
                    {
                        elementName = attribute.ElementName;
                    }
                    value = propertyInfo.GetValue(pair.Value);
                    type = value.GetType();
                }
                else
                {
                    return null;
                }


            }
            else
            {
                value = inobj;
                type = inobj.GetType();
            }
            ConvertType t = GetConvertType(type);

            if (t == ConvertType.Convert)
            {
                return new XElement(elementName, new XAttribute("type", type.GetAssemblyQualifiedName()), value);
            }
            else if (t == ConvertType.Transfer)
            {
                string result = string.Empty;
                if (value is System.Windows.Size size)
                {
                    result = size.Width + "," + size.Height;
                }
                else if (value is System.Windows.Point point)
                {
                    result = point.X + "," + point.Y;
                }
                else if (value is System.Windows.Media.Color color)
                {
                    result = color.A + "," + color.R + "," + color.G + "," + color.B;
                }
                else if (value is DateTime time)
                {
                    result = time.ToString();
                }
                return new XElement(elementName, new XAttribute("type", type.GetAssemblyQualifiedName()), result);
            }
            else if (t == ConvertType.Collection)
            {
                List<XElement> elements = new List<XElement>();
                foreach (var item in (IEnumerable)value)
                {
                    elements.Add(ToXElement(item));
                }
                return new XElement(elementName, new XAttribute("type", type.GetAssemblyQualifiedName()), elements.ToArray());
            }
            else if (t == ConvertType.Dictionary)
            {
                List<XElement> elements = new List<XElement>();
                foreach (dynamic item in (IEnumerable)value)
                {
                    XElement eKey = ToXElement(item.Key, "Key");
                    XElement eValue = ToXElement(item.Value, "Value");
                    XElement eKeyValuePair = new XElement("add", new XAttribute("type", ((Type)item.GetType()).GetAssemblyQualifiedName()), eKey, eValue);
                    elements.Add(eKeyValuePair);
                }

                return new XElement(elementName, new XAttribute("type", type.GetAssemblyQualifiedName()), elements.ToArray());
            }
            else
            {
                List<XElement> elements = new List<XElement>();
                foreach (var propertyInfo in value.GetType().GetProperties())
                {
                    XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, value));
                    if (element != null)
                    {
                        elements.Add(element);
                    }
                }
                return new XElement(elementName, new XAttribute("type", type.GetAssemblyQualifiedName()), elements.ToArray());
            }
        }
        /// <summary>
        /// 将<see cref="XElement"/>转化为等价<see cref="object"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static dynamic ToObject(XElement element, Type type)
        {
            ConvertType t = GetConvertType(type);
            if (t == ConvertType.Convert)
            {
                return Convert.ChangeType(element.Value, type);
            }
            else if (t == ConvertType.Transfer)
            {
                string s = element.Value;
                object o = null;
                if (type == typeof(System.Windows.Size))
                {
                    o = System.Windows.Size.Parse(s);
                }
                else if (type == typeof(System.Windows.Point))
                {
                    o = System.Windows.Point.Parse(s);
                }
                else if (type == typeof(System.Windows.Media.Color))
                {
                    string[] x = s.Split(',');
                    o = System.Windows.Media.Color.FromArgb(byte.Parse(x[0]), byte.Parse(x[1]), byte.Parse(x[2]), byte.Parse(x[3]));
                }
                else if (type == typeof(DateTime))
                {
                    o = DateTime.Parse(s);
                }
                return o;
            }
            else if (t == ConvertType.Collection)
            {
                dynamic result = null;
                if (type.IsArray)
                {
                    Type membertype = type.GetElementType();
                    Type listtype = typeof(List<>);
                    listtype = listtype.MakeGenericType(membertype);
                    result = Activator.CreateInstance(listtype);
                    foreach (var item in element.Elements())
                    {
                        result.Add(ToObject(item, membertype));
                    }
                    return result.ToArray();
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type membertype = type.GenericTypeArguments[0];
                    result = Activator.CreateInstance(type);
                    foreach (var item in element.Elements())
                    {
                        result.Add(ToObject(item, membertype));
                    }
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else if (t == ConvertType.Dictionary)
            {
                Type keyType = type.GenericTypeArguments[0];
                Type valueType = type.GenericTypeArguments[1];
                Type keyValuePairType = (typeof(KeyValuePair<,>)).MakeGenericType(keyType, valueType);
                dynamic dic = Activator.CreateInstance(type);
                foreach (var item in element.Elements())
                {
                    dynamic key = ToObject(item.Element("Key"), keyType);
                    dynamic value = ToObject(item.Element("Value"), valueType);
                    dic.Add(key, value);
                }

                return dic;
            }
            else
            {
                object result = Activator.CreateInstance(type);

                foreach (var propertyInfo in type.GetProperties())
                {

                    string elementName = propertyInfo.Name;
                    XmlElementAttribute attribute = (XmlElementAttribute)propertyInfo.GetCustomAttribute(typeof(XmlElementAttribute));
                    if (attribute != null)
                    {
                        elementName = attribute.ElementName;
                    }

                    XElement e = element.Element(elementName);
                    Type contentType = Type.GetType(e.Attribute("type").Value);

                    object o = ToObject(e, contentType);
                    propertyInfo.SetValue(result, o);

                }

                return result;
            }
        }
        internal static ConvertType GetConvertType(Type type)
        {
            Dictionary<Type, ConvertType> dic = new Dictionary<Type, ConvertType>()
            {
                {typeof(byte),ConvertType.Convert },
                { typeof(sbyte),ConvertType.Convert},
                {typeof( short),ConvertType.Convert },
                {typeof(int),ConvertType.Convert },
                { typeof(long),ConvertType.Convert},
                {typeof(ushort),ConvertType.Convert },
                {typeof(uint),ConvertType.Convert },
                {typeof(ulong),ConvertType.Convert },
                {typeof(float),ConvertType.Convert },
                {typeof(double),ConvertType.Convert },
                {typeof(char),ConvertType.Convert },
                {typeof(string),ConvertType.Convert },
                {typeof(decimal),ConvertType.Convert },
                {typeof(bool),ConvertType.Convert },
                {typeof(DateTime),ConvertType.Transfer },
                {typeof(System.Windows.Point),ConvertType.Transfer },
                {typeof(System.Windows.Size),ConvertType.Transfer },
                {typeof(System.Windows.Media.Color) ,ConvertType.Transfer}
            };
            if (dic.TryGetValue(type, out ConvertType t))
            {
                return t;
            }
            else if (type.BaseType == typeof(Array) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                return ConvertType.Collection;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return ConvertType.Dictionary;
            }
            else
            {
                return ConvertType.User;
            }
        }

        [Obsolete("", true)]
        public static object LoadObject(string fileName, Type type)
        {
            throw new NotImplementedException();
        }
        [Obsolete("", true)]
        public static void SaveObject(object obj, string fileName, Type type)
        {
            throw new NotImplementedException();
        }
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
                        XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, reference));
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
                                    object o = ToObject(element, propertyInfo.PropertyType);
                                    propertyInfo.SetValue(reference, o);
                                }

                            }
                        }
                    }
                }
            }
        }
        internal static void XSerialize(object reference, ref XElement xDocument, XSerializeOption option, Predicate<string> condition = null)
        {
            if (option == XSerializeOption.Serialize)
            {
                XElement xElement = new XElement(reference.GetType().ToString());
                foreach (var propertyInfo in reference.GetType().GetProperties())
                {
                    if (condition == null || condition(propertyInfo.Name))
                    {
                        XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, reference));
                        if (element != null)
                        {
                            xElement.Add(element);
                        }
                    }
                }
                xDocument = xElement;
            }
            else
            {
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

                        XElement element = xDocument. Element(elementName);

                        if (element != null)
                        {
                            if (condition == null || condition(elementName))
                            {
                                object o = ToObject(element, propertyInfo.PropertyType);
                                propertyInfo.SetValue(reference, o);
                            }

                        }
                    }
                }
            }
        }
    }

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

        public void Serialize() => USettingsBase.XSerialize((object)Reference, FileName, XSerializeOption.Serialize, Condition);
        public void DeSerialize() => USettingsBase.XSerialize(Reference, FileName, XSerializeOption.DeSerialize, Condition);
    }
}
