using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XCore;

namespace XCoreLibTest
{
    class Program
    {
        static Example example = new Example(AppDomain.CurrentDomain.BaseDirectory, "settings");
        static void Main(string[] args)
        {
            while (true)
            {
                string t = Console.ReadLine();
   
                switch (t)
                {
                    case "save":
                        example.Save();
                        Console.WriteLine(example.A);
                        break;
                    case "load":
                        example.Load();
                        Console.WriteLine(example.A);
                        break;
                    case "add":
                        example.A++;
                        Console.WriteLine(example.A);
                        break;
                    default:
                        break;
                }
            }

        }
    }

    class Example : XCore.Component.USettingsObject
    {
        public Example(string folder, string rootName)
        {
            Folder = folder;
            RootName = rootName;
        }

        public int A { get; set; } = 1234;
        public int[] B { get; set; } = new int[] { 1, 4, 5, 7 };
        public List<byte> C { get; set; } = new List<byte> { 1, 2, 4, 8 };
        
    }
}
