# How to dynamically create a read-only calculated (persistent alias) property


<p>When building and extending persistent classes at runtime, it is often desirable to create a non-persistent calculated property. Currently available <a href="https://documentation.devexpress.com/#CoreLibraries/DevExpressXpoMetadataXPClassInfo_CreateMembertopic">XPClassInfo.CreateMember</a> method overrides are only capable of creating writable <a href="https://documentation.devexpress.com/CoreLibraries/clsDevExpressXpoMetadataXPCustomMemberInfotopic.aspx">XPCustomMemberInfo</a> instances and the XPCustomMemberInfo.ReadOnly property cannot be changed.</p>
<p>This example demonstrates how to create a custom XPAliasedMemberInfo class inherited directly from <a href="https://documentation.devexpress.com/CoreLibraries/clsDevExpressXpoMetadataXPMemberInfotopic.aspx">XPMemberInfo</a>. In this class constructor, we add <a href="https://documentation.devexpress.com/CoreLibraries/clsDevExpressXpoPersistentAliasAttributetopic.aspx">PersistentAliasAttribute</a> with a specified expression, and override the GetValue method to evaluate the alias expression in the same manner as it is done in the XPBaseObject class. The read-only behavior is achieved by passing an argument to the base class constructor.</p>
<p>Also, an extender is implemented to provide additional methods for XPClassInfo class.</p>
<p> </p>

<br/>


