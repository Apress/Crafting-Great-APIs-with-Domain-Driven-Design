# XML from XSD

Requirements
* Windows
* .NET Framework 4.8 (not .NET or .NET Core)

```powershell
cd .\LibrarySchemaTester\Example.Library.Book.Dto.Xml\
&'C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\xsd.exe' ../../book.xsd /classes /namespace:Example.Library.Book.Dto.Xml
```


