using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore
{
    public static class MyTypeExtension

    {
        public static string GetAssemblyQualifiedName(this Type type)
        {
            if (type.IsArray)
            {
                Type membertype = type.GetElementType();
                return "System.Array[" + GetAssemblyQualifiedName(membertype) + "]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type membertype = type.GenericTypeArguments[0];
                return "System.Collections.Generic.List`1[" + GetAssemblyQualifiedName(membertype) + "]";
            }
            else
            {
                if (type.Assembly == System.Reflection.Assembly.GetAssembly(typeof(int)))
                {
                    return type.ToString();
                }
                else
                {
                    return type.AssemblyQualifiedName;
                }
            }

        }
        public static Type GetAssemblyQualifiedType(string value)
        {
            string[] v = new string[] { "System.Collections.Generic.List`1[", "System.Array[" };
            if (value.StartsWith(v[0]))
            {
                Type type = typeof(List<>);
                return type.MakeGenericType(GetAssemblyQualifiedType(value.Substring(v[0].Length, value.Length - v[0].Length - 1)));
            }
            else if (value.StartsWith(v[1]))
            {
                Type m = GetAssemblyQualifiedType(value.Substring(v[1].Length, value.Length - v[1].Length - 1));

                return m.MakeArrayType();
            }
            else
            {
                return Type.GetType(value);
            }
        }

    }
}
