using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XCore;
using XCore.Component;

namespace XCoreLibTest
{
    class Program
    {
        static Example example = new Example(AppDomain.CurrentDomain.BaseDirectory, "settings");
        static readonly XSerializer<Example> xSerializer = new XSerializer<Example>(example, AppDomain.CurrentDomain.BaseDirectory + "1.xml");
        static void Main(string[] args)
        {
            while (true)
            {
                string t = Console.ReadLine();
   
                switch (t)
                {
                    case "save":
                        example.Save();
                        break;
                    case "load":
                        example.Load();
                        break;
                    case "add":
                        example.A += (int) Math.IEEERemainder( DateTime.Now.Millisecond,55);
                        for (int i = 0; i < example.B.Length; i++)
                        {
                            example.B[i]++;
                        }
                        break;
                    case "print":
                        Console.WriteLine(example);
                        break;
                }
            }

        }
    }

    class Example : USettingsObject
    {
        public Example(string folder, string rootName)
        {
            Folder = folder;
            DisplayName = rootName;
        }

        public int A { get; set; } = 1234;
        public int[] B { get; set; } = new int[] { 1, 4, 5, 7 };
        public List<byte> C { get; set; } = new List<byte> { 1, 2, 4, 8 };
        public MSC MSC { get; set; } = new MSC { A = 12, B = true };
        public Dictionary<string, int> DicLS { get; set; } = new Dictionary<string, int>()
        {
            { "123",124}
        };
        public object PX { get; set; } = 12;

        public override string ToString()
        {
            return string.Format("A:{0};\nB:{1},\nMSC:{2}",A,string.Join(";",B),MSC);
        } 
    }

    struct MSC
    {
        public int A { get; set; }
        public bool B { get; set; }
        public override string ToString()
        {
            return A + " " + B;
        }
    }
}
