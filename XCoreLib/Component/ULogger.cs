using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XCore.Component
{
    public static class ULogger
    {
        [XmlIgnore]
        public static string FileName { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "ULogger.xml";
        public static void Write(ULoggerInfo info)
        {
            XDocument xDocument = null;
            try
            {
                if (File.Exists(FileName))
                {
                    xDocument = XDocument.Load(FileName);
                }
                else
                {
                    xDocument = new XDocument(new XComment("ULogger"), new XElement("ULogger"));
                }
            }
            catch (Exception)
            {
                File.Delete(FileName);
                xDocument = new XDocument(new XComment("ULogger"), new XElement("ULogger"));
            }
            XElement element;
            var xElement = from item in xDocument.Root.Elements() where item.Name == "logger" && item.Attribute("date").Value == DateTime.Now.Date.ToString() select item;
            if (xElement.Count()== 0)
            {
                element = new XElement("logger", new XAttribute("date", DateTime.Now.Date.ToString()));
                xDocument.Root.Add(element);
            }
            else
            {
                element = xElement.First();
            }
            element.Add(USettingsBase.ToXElement(info, info.GetType().ToString()));
            xDocument.Save(FileName);
        }
    }
}
