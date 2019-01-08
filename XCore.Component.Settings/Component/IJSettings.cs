using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XCore.Component
{
    /// <summary>
    /// Json设置类，依赖<see cref="Newtonsoft.Json.JsonConvert"/>
    /// </summary>
    public interface IJSettings:IFileEntity
    {
        void OnSettingsInitialized();
    }

    public static class JSettingsExtension
    {
        public static void Load(this IJSettings obj)
        {
            if (File.Exists(obj._FileName("json")))
            {
                string jsonstring = null;
                try
                {
                    jsonstring = File.ReadAllText(obj._FileName("json"));
                }
                catch (Exception)
                {
                    return;
                }

                JObject jObject = JsonConvert.DeserializeObject<JObject>(jsonstring);

                foreach (var item in obj.GetType().GetProperties())
                {
                    if (item.CanWriteAndRead() && item.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                    {
                        try
                        {
                            item.SetValue(jObject[item.Name], obj);
                            
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            obj.OnSettingsInitialized();
        }
        public static void Save(this IJSettings obj)
        {
            if (!Directory.Exists(Path.GetDirectoryName(obj._FileName("json"))))
            {
                Directory.CreateDirectory(obj._DisplayName);
            }

            string jsonstring = JsonConvert.SerializeObject(obj, Formatting.Indented);

            File.WriteAllText(obj._FileName("json"), jsonstring);
        }
    }
}
