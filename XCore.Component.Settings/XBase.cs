using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XCore
{
    /// <summary>
    /// 为基于<see cref="XDocument"/>操作的类提供基类.
    /// </summary>
    public interface IXmlFile
    {
        /// <summary>
        /// Xml文档的注释
        /// </summary>
        string _Comment { get; }
        /// <summary>
        /// Xml所存的文件夹
        /// </summary>
        string _Folder { get;}
        /// <summary>
        /// Xml文件的名称
        /// </summary>
        string _DisplayName { get;}
    }

    public static class IXmlFileExtension
    {
        public static string _FileName(this IXmlFile obj)
        {
            return obj._Folder + obj._DisplayName + ".xml";
        }
        public static XDocument CreateNew(this IXmlFile obj, string root)
        {
            return new XDocument(
                new XComment(obj._Comment),
                new XElement(root)
                );
        }
    }
}
