Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers

Namespace DevExpress.Xpo.Metadata
	Public Class XPAliasedMemberInfo
		Inherits XPMemberInfo
		Private ReadOnly propertyName As String
		Private ReadOnly propertyType As Type
		Public Sub New(ByVal owner As XPClassInfo, ByVal propertyName As String, ByVal propertyType As Type, ByVal expression As String)
			MyBase.New(owner, True)
			If propertyName Is Nothing Then
				Throw New ArgumentNullException("propertyName")
			End If
			Me.propertyName = propertyName
			Me.propertyType = propertyType
			Owner.AddMember(Me)
			Me.AddAttribute(New PersistentAliasAttribute(expression))
		End Sub
		Public Overrides ReadOnly Property Name() As String
			Get
				Return propertyName
			End Get
		End Property
		Public Overrides ReadOnly Property IsPublic() As Boolean
			Get
				Return True
			End Get
		End Property
		Public Overrides ReadOnly Property MemberType() As Type
			Get
				Return propertyType
			End Get
		End Property
		Protected Overrides ReadOnly Property CanPersist() As Boolean
			Get
				Return False
			End Get
		End Property
		Public Overrides Function GetValue(ByVal theObject As Object) As Object
			Dim caseSensitive As Boolean = XpoDefault.DefaultCaseSensitive
			If TypeOf theObject Is DevExpress.Xpo.Helpers.ISessionProvider Then
				caseSensitive = (CType(theObject, DevExpress.Xpo.Helpers.ISessionProvider)).Session.CaseSensitive
			End If
			Dim persistentAliasAttribute As PersistentAliasAttribute = CType(Me.GetAttributeInfo(GetType(PersistentAliasAttribute)), PersistentAliasAttribute)
			Return New ExpressionEvaluator(Owner.GetEvaluatorContextDescriptor(), CriteriaOperator.Parse(persistentAliasAttribute.AliasExpression, New Object(){}), caseSensitive, Owner.Dictionary.CustomFunctionOperators).Evaluate(theObject)
		End Function
		Public Overrides Sub SetValue(ByVal theObject As Object, ByVal theValue As Object)
		End Sub
		Public Overrides Function GetModified(ByVal theObject As Object) As Boolean
			Return False
		End Function
		Public Overrides Function GetOldValue(ByVal theObject As Object) As Object
			Return GetValue(theObject)
		End Function
		Public Overrides Sub ResetModified(ByVal theObject As Object)
		End Sub
		Public Overrides Sub SetModified(ByVal theObject As Object, ByVal oldValue As Object)
		End Sub
	End Class

	#Region ".NET Framework 3+ Extender"
	Public Module XPAliasedMemberInfoExtender
		<System.Runtime.CompilerServices.Extension> _
		Public Function CreateAliasedMember(ByVal self As XPClassInfo, ByVal name As String, ByVal type As Type, ByVal expression As String) As XPAliasedMemberInfo
			Return XPAliasedMemberInfoExtender.CreateAliasedMember(self, name, type, expression, Nothing)
		End Function
		<System.Runtime.CompilerServices.Extension> _
		Public Function CreateAliasedMember(ByVal self As XPClassInfo, ByVal name As String, ByVal type As Type, ByVal expression As String, ByVal attrs() As Attribute) As XPAliasedMemberInfo
			Dim result As New XPAliasedMemberInfo(self, name, type, expression)
			If attrs IsNot Nothing Then
				For Each a As Attribute In attrs
					result.AddAttribute(a)
				Next a
			End If
			Return result
		End Function
	End Module
	#End Region
End Namespace
