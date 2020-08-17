App.Utils
===================================
1.8.9
+ JsonHelper.AsJObject
+ JsonHelper.AddProperty
+ StringHelper.AddQueryString
* Fix StringHelper.TrimEnd
+ TAttribute - ParamAttribute - UIAttribute
+ UIAttribute support globalizatin, Add AppCoreConfig.Instance
+ Fix GetAttribute&lt;T&gt;()
+ Parse<T>()
+ To<T>()
+ ToChinaNumber
+ Net.Ping



1.9.0
+ Asp.ToVirtualPath()
+ IO.ToRelativePath()
+ ReflectionHelper.GetItemType()
+ GetDescription() support DisplayNameAttribute
+ GetDescription() -> GetTitle()
+ Each、Each2
* GetPropertyValue support subproperty, eg user.Parent.Name
+ Asp.GetHandler()加上缓存处理
+ IO.GetDict()
+ Convertor.Merge(List, List)
+ Asp.GetParam()
+ DateTimeHelper.ToFriendlyText()
+ IO.CombinePath(), CombineWebPath()


2.0.0
* ParseJson(txt, ignoreException) can ignore exception
* ToJson() only export name and assembly when export Type to json.
* StringHelper.TrimStartTo()
* SimplyReflectionApi GetPropertyValue->GetValue, GetPropertyInfo->GetProperty
* ReflectionHelper.GetTypeValues -> GetEnumString
+ ReflectionHelper.IsCollection()
* ReflectionHelper.GetName(), GetProperty(), GetTitle(), GetValue(), SetValue()


Target
- App.Utils 编译 Asp.net core 版本的代码
- HttpApi 编译 Asp.net core 版本的代码

