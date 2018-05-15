using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Component.Editor
{
    public class UType
    {
        public UType(string baseFrom)
        {
            BaseFrom = baseFrom;
        }

        public string BaseFrom { get; set; }
        public Type Type{
            get
            {
                return Type.GetType(BaseFrom);
            }
        }
          
        public ConvertType ConvertType {
            get
            {
                return USettingsBase.GetConvertType(Type);
            }
        }
    }
}
