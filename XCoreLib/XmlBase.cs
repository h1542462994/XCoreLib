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
    public abstract class XmlBase
    {
        /// <summary>
        /// 注释,用于生成<see cref="XComment"/>.
        /// </summary>
        [XmlIgnore]
        protected abstract string Comment { get; }
        [XmlIgnore]
        public string Folder { get; protected set; }
        [XmlIgnore]
        public string RootName { get;protected set; }
        [XmlIgnore]
        public string FileName => Folder + RootName + ".xml";

        public abstract void Load();
        public abstract void Save();
        public async Task LoadAsync()
        {
            await Task.Run(()=>Load());
        }
        public async Task SaveAsync()
        {
            await Task.Run(() => Save());
        }

        protected XDocument CreateXml()
        {
            return new XDocument(
                new XComment(Comment),
                new XElement(RootName)
                );
        }
    }
}
