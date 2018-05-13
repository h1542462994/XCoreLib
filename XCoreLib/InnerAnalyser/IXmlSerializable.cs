using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XCore.InnerAnalyser
{
    /// <summary>
    /// 支持转换至<see cref="XDocument"/>或从<see cref="XDocument"/>转换为对象.
    /// </summary>
    interface IXSerializable
    {
        void DeSerialize(XDocument xDocument);
        XDocument Serialize();
    }
}
