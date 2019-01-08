using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XCore
{
    public interface IFileEntity
    {
        string _Folder { get; }
        string _DisplayName { get; }
    }

    /// <summary>
    /// 为基于<see cref="XDocument"/>操作的类提供基类.
    /// </summary>
    public interface IXmlFileEntity:IFileEntity
    {
        string _Comment { get; }
    }

    public static class IFileEntityExtension
    {
        public static string _FileName(this IFileEntity obj,string extension)
        {
            return obj._Folder + obj._DisplayName + "." + extension;
        }
        public static XDocument CreateNew(this IXmlFileEntity obj, string root)
        {
            return new XDocument(
                new XComment(obj._Comment),
                new XElement(root)
                );
        }
    }
}
