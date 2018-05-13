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
    /// 支持基础类型及集合类型(仅支持<see cref="Array"/>和<see cref="List{T}"/>).
    /// </summary>    
    public abstract class USettingsObject : XmlBase,IXSerializable
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
            else if (type.BaseType == typeof(Array) || type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return ConvertType.Collection;
            }
            else
            {
                return ConvertType.User;
            }
        }

        private static XElement ToXElement(object value, string name = "add")
        {
            Type type = value.GetType();
            ConvertType t = GetConvertType(type);
            if (t == ConvertType.Convert)
            {
                return new XElement(name, new XAttribute("type", type), value);
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
                return new XElement(name, new XAttribute("type", type), result);
            }
            else if (t == ConvertType.Collection)
            {
                List<XElement> elements = new List<XElement>();
                foreach (var item in (IEnumerable)value)
                {
                    elements.Add(ToXElement(item));
                }
                return new XElement(name, new XAttribute("type", type), elements.ToArray());
            }
            else
            {
                throw new ArgumentException();
            }
        }
        private static object ToObject(XElement element, Type type)
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
                    listtype.MakeGenericType(membertype);
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
            else
            {
                throw new ArgumentException();
            }
        }
        XDocument IXSerializable.Serialize()
        {
            XDocument xDocument = CreateXml();
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) == null && propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    xDocument.Root.Add(ToXElement(propertyInfo.GetValue(this), propertyInfo.Name));
                }
            }
            return xDocument;
        }
        void IXSerializable.DeSerialize(XDocument xDocument)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(XmlIgnoreAttribute)) != null || propertyInfo.CanWrite)
                {
                    XElement element = xDocument.Root.Element(propertyInfo.Name);
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
