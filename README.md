XCoreLib(Assemby: type=neutrul,publicToken=null)
=================================================================
#声明
+ 此类库是开源的,但禁止用于商业目的,否则要追究法律责任.
#类层次结构
+ //developed by cht
	+ `namespace` XCore
		+`class` XmlBase:object 为基于<see cref="XDocument"/>操作的类提供基类.
		+ `namespace` Component
			+ `enum` ConvertType:int 
			+ `enum` XSerializeOption:int
			+ `class` USettingsBase:XmlBase
				+ `static method` ToElement() as Element;
				+ `static method` ToObject() as object;
				+ `static method` XSerialize();
			+ `class` USettingObject:USettingsBase
		+ `namespace` InnerAnalyser
			+ `interface` IXmlSerializable
				+ `method` Serialize() as XElement;
				+ `method` DeSerialzie(XElement);
			+ `static class` Extension

+ //developed by qht
	+ no record found!
#更新
+ 2018/5/17-2018/6/9
	+ (2018/5/17 1.0.3.0) 序列化和反序列化按照数据架构声明.
	+ (2018/5/21 1.0.4.0) 修复自定义类型在Type.GetType()下无法搜索的问题.