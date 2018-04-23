Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports DevExpress.Data.Filtering

Namespace ConsoleApplication17
	Friend Class Program
		Shared Sub Main(ByVal args() As String)

			XpoDefault.DataLayer = New SimpleDataLayer(New DevExpress.Xpo.DB.InMemoryDataStore())
			XpoDefault.Session = Nothing

			Using uow As New UnitOfWork()
				uow.ClearDatabase()
				Dim c1 As New TestClass(uow)
				c1.Name = "aaa"
				c1.Email = "e@mail.com"
				Dim c2 As New TestClass(uow)
				c2.Name = "bbb"
				c2.Email = "nobody@w3.com"
				c2.Master = c1
				uow.CommitChanges()
			End Using

			Dim ci As XPClassInfo = XpoDefault.DataLayer.Dictionary.GetClassInfo(GetType(TestClass))
			ci.CreateAliasedMember("DisplayName", GetType(String), "concat([Name],' (',[Email],')',iif([Master] is null,'',Concat(' managed by ',[Master].Name)))")

			Using uow As New UnitOfWork()
				Dim xpc As New XPCollection(Of TestClass)(uow, CriteriaOperator.Parse("Contains([DisplayName],'w3')"))
				System.Diagnostics.Debug.Assert(xpc.Count = 1)
				System.Diagnostics.Debug.Assert(xpc(0).Name = "bbb")
				Dim mi As XPMemberInfo = xpc(0).ClassInfo.FindMember("DisplayName")
				System.Diagnostics.Debug.Assert(mi IsNot Nothing AndAlso mi.IsReadOnly AndAlso mi.IsAliased)
				System.Diagnostics.Debug.Assert(Object.Equals(mi.GetValue(xpc(0)), "bbb (nobody@w3.com) managed by aaa"))
			End Using
		End Sub
	End Class

	Public Class TestClass
		Inherits XPObject
		Public Sub New(ByVal s As Session)
			MyBase.New(s)
		End Sub

		Public Property Name() As String
			Get
				Return GetPropertyValue(Of String)("Name")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Name", value)
			End Set
		End Property
		Public Property Email() As String
			Get
				Return GetPropertyValue(Of String)("Email")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Email", value)
			End Set
		End Property
		Public Property Master() As TestClass
			Get
				Return GetPropertyValue(Of TestClass)("Master")
			End Get
			Set(ByVal value As TestClass)
				SetPropertyValue(Of TestClass)("Master", value)
			End Set
		End Property
	End Class
End Namespace
