using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using XCore.InnerAnalyser;

namespace XCore.Component.Linq
{

    public abstract class ElementTree : ICollection<ElementTree>, IXSerializable
    {
        protected ElementTree(string identiter,params ElementTree[] children)
        {
            foreach (var item in children)
            {
                Add(item);
            }
            Identiter = identiter;
        }
        [XmlIgnore]
        public ElementTree Parent { get; private set; } = null;
        [XmlIgnore]
        private List<ElementTree> Children { get; set; } = new List<ElementTree>();
        [XmlIgnore]
        public string Identiter { get; set; } 
        [XmlIgnore]
        public int Count => Children.Count;
        [XmlIgnore]
        bool ICollection<ElementTree>.IsReadOnly => false;
        public void Add(ElementTree item)
        {
            if (Contains(item.Identiter))
            {
                throw new ArgumentException();
            }
            else
            {
                item.Parent = this;
                Children.Add(item);
            }

        }
        public void Clear()
        {
            foreach (var item in Children)
            {
                item.Parent = null;
            }
            Children.Clear();
        }
        bool ICollection<ElementTree>.Contains(ElementTree item)
        {
            return Children.Contains(item);
        }
        void ICollection<ElementTree>.CopyTo(ElementTree[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(ElementTree item)
        {
            if (Children.Contains(item))
            {
                item.Parent = null;
                Children.Remove(item);
                return true;
            }
            else return false;
        }
        IEnumerator<ElementTree> IEnumerable<ElementTree>.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        bool Contains(string identiter)
        {
            var x = Children.Where((t) => t.Identiter == identiter);
            if (x.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected virtual ElementTree this[string identiter]
        {
            get
            {
                var x = Children.Where((t) => t.Identiter == identiter);
                if (x.Count() == 0)
                {
                    return null;
                }
                else
                {
                    return x.First();
                }
            }
            set
            {
                this[identiter] = value;
            }
        }
        /// <summary>
        /// 返回此节点对于整个tree的深度.
        /// </summary>
        public int Depth
        {
            get
            {
                ElementTree element = this;
                int i = 0;
                while (element.Parent != null)
                {
                    i++;
                    element = element.Parent;
                }
                return i;
            }
        }
        /// <summary>
        /// 返回整个tree的根.
        /// </summary>
        public ElementTree Root
        {
            get
            {
                ElementTree tree = this;
                while (tree.Parent != null)
                {
                    tree = tree.Parent;
                }
                return tree;
            }
        }


        public virtual string GetContentString()
        {
            return "ElementTree";
        }
        public override string ToString()
        {
            return ToString(Depth);
        }
        private string ToString(int depth)
        {
            int p = Depth - depth;
            string x = string.Format("{0}id:{1} {2}", new string(' ', 4 * p), Identiter, GetContentString());
            foreach (var item in Children)
            {
                x += "\n";
                x += item.ToString(depth);
            }
            return x;
        }
        public void SetFormerNodes(Action<ElementTree> tree)
        {
            ElementTree t = Parent;
            while (t != null)
            {
                tree.Invoke(t.Parent);
                t = t.Parent;
            }
        }

        void IXSerializable.DeSerialize(XElement xElement)
        {
            USettingsBase.XSerialize(this, ref xElement, XSerializeOption.DeSerialize);
        }
        XElement IXSerializable.Serialize()
        {
            XElement e = new XElement("content");
            USettingsBase.XSerialize(this, ref e, XSerializeOption.Serialize);
            return e;
        }

        public static T Load<T>(string fileName) where T:ElementTree
        {
            return ToElementTree<T>(XDocument.Load(fileName).Root);
        }
        public void Save(string fileName)
        {
            XDocument xDocument = new XDocument( new XElement( "tree", ToXElement(Root)));
            xDocument.Save(fileName);
        }

        public static XElement ToXElement(ElementTree tree)
        {
            var x = from item in tree select ToXElement(item);
            return new XElement("item",
                     new XAttribute("id", tree.Identiter),
                     new XElement("content", ((IXSerializable)tree).Serialize()),               
                     x.Count() != 0 ? x.ToArray() : null
                     );
        }
        public static T ToElementTree<T>(XElement element) where T : ElementTree
        {
            var x = from item in element.Elements() where item.Name == "item" select ToElementTree<T>(item);
            dynamic t = Activator.CreateInstance(typeof(T),element.Attribute("id").Value, x.ToArray());

            ((IXSerializable)t).DeSerialize(element.Element("content"));
            return t;
        }
    }
    public class StringTree : ElementTree
    {
        public StringTree(string identiter, params StringTree[] children) : base(identiter, children)
        {
        }
        public new StringTree this[string identiter] => (StringTree)base[identiter];

    }
}
