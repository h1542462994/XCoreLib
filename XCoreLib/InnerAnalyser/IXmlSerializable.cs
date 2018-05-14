using System.Xml.Linq;

namespace XCore.InnerAnalyser
{
    /// <summary>
    /// 支持转换至<see cref="XDocument"/>或从<see cref="XDocument"/>转换为对象.
    /// </summary>
    public interface IXSerializable
    {
        void DeSerialize(XElement xElement);
        XElement Serialize();
    }
}
