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
    /// 支持基础类型自定义类型及集合类型(仅支持<see cref="Array"/>和<see cref="List{T}"/>).
    /// </summary>    
    public abstract class USettingsObject : XmlBase, IXSerializable
    {
        protected override string Comment => "null";
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
                ((IXSerializable)this).DeSerialize(xDocument);
            }

        }
        public override void Save()
        {
            XDocument xDocument = ((IXSerializable)this).Serialize();
            xDocument.Save(FileName);
        }

        public static ConvertType GetConvertType(Type type)
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
                {typeof(DateTime),ConvertType.Convert },
                {typeof(System.Windows.Point),ConvertType.Transfer },
                {typeof(System.Windows.Size),ConvertType.Transfer },
                {typeof(System.Windows.Media.Color) ,ConvertType.Transfer}
            };
            if (dic.TryGetValue(type, out ConvertType t))
            {
                return t;
            }
            else if (type.BaseType == typeof(Array) ||(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                return ConvertType.Collection;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition()== typeof(Dictionary<,>))
            {
                return ConvertType.Dictionary;
            }
            else
            {
                return ConvertType.User;
            }
        }


        /// <summary>
        ///将对象转化为等价<see cref="XElement"/>.
        /// </summary>
        /// <param name="inobj">传入值,支持属性和单元值.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static XElement ToXElement(object inobj, string name = "add")
        {
            string elementName = name;
            Type type;
            object value;
            if (inobj is KeyValuePair<PropertyInfo,object> pair)
            {
                PropertyInfo propertyInfo = pair.Key;
                if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) == null && propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    elementName = propertyInfo.Name;
                    XmlElementAttribute attribute =         (XmlElementAttribute)propertyInfo.GetCustomAttribute(typeof(XmlElementAttribute));
                    if (attribute != null)
                    {
                        elementName = attribute.ElementName;
                    }
                    value = propertyInfo.GetValue(pair.Value);
                    type = propertyInfo.PropertyType;
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
                return new XElement(elementName, new XAttribute("type", type), value);
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
                return new XElement(elementName, new XAttribute("type", type), result);
            }
            else if (t == ConvertType.Collection)
            {
                List<XElement> elements = new List<XElement>();
                foreach (var item in (IEnumerable)value)
                {
                    elements.Add(ToXElement(item));
                }
                return new XElement(elementName, new XAttribute("type", type), elements.ToArray());
            }
            else if (t == ConvertType.Dictionary)
            {
                List<XElement> elements = new List<XElement>();
                foreach (dynamic item in (IEnumerable)value)
                {
                    XElement eKey = ToXElement(item.Key,"Key");
                    XElement eValue = ToXElement(item.Value,"Value");
                    XElement eKeyValuePair = new XElement("add", new XAttribute("type", item.GetType()), eKey, eValue);
                    elements.Add(eKeyValuePair);
                }

                return new XElement(elementName, new XAttribute("type", type), elements.ToArray());
            }
            else
            {
                List<XElement> elements = new List<XElement>();
                foreach (var propertyInfo in value.GetType().GetProperties())
                {
                    XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, value));
                    if (element!=null)
                    {
                        elements.Add(element);
                    }
                }
                return new XElement(elementName, new XAttribute("type", type), elements.ToArray());
            }
        }
        private static dynamic ToObject(XElement element, Type type)
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
                    Type membertype = type.GetMethod("Find").ReturnType;
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
            else if (t== ConvertType.Dictionary)
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

                    if (element!=null)
                    {
                        object o = ToObject(e, propertyInfo.PropertyType);
                        propertyInfo.SetValue(result, o);
                    }

                }

                return result;
            }
        }
        /// <summary>
        /// 序列化方法.
        /// </summary>
        /// <returns></returns>
        XDocument IXSerializable.Serialize()
        {
            XDocument xDocument = CreateXml(this.GetType().ToString());
            foreach (var propertyInfo in GetType().GetProperties())
            {
                XElement element = ToXElement(new KeyValuePair<PropertyInfo, object>(propertyInfo, this));
                if (element!=null)
                {
                    xDocument.Root.Add(element);
                }
            }
            return xDocument;
        }
        void IXSerializable.DeSerialize(XDocument xDocument)
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

                    XElement element = xDocument.Root.Element(elementName);

                    if (element != null)
                    {
                        object o = ToObject(element, propertyInfo.PropertyType);
                        propertyInfo.SetValue(this, o);
                    }
                }
            }
        }
    }
}
