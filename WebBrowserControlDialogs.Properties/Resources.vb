Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Resources
Imports System.Runtime.CompilerServices

Namespace WebBrowserControlDialogs.Properties
	<GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode(), CompilerGenerated()>
	Friend Class Resources
		Private Shared resourceMan As ResourceManager

		Private Shared resourceCulture As CultureInfo

		<EditorBrowsable(EditorBrowsableState.Advanced)>
		Friend Shared ReadOnly Property ResourceManager() As ResourceManager
			Get
				Dim flag As Boolean = Resources.resourceMan Is Nothing
				If flag Then
                    Dim resourceManagerRenamed As ResourceManager = New ResourceManager("WebBrowserControlDialogs.Properties.Resources", GetType(Resources).Assembly)
                    Resources.resourceMan = resourceManagerRenamed
                End If
				Return Resources.resourceMan
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Advanced)>
		Friend Shared Property Culture() As CultureInfo
			Get
				Return Resources.resourceCulture
			End Get
			Set(value As CultureInfo)
				Resources.resourceCulture = value
			End Set
		End Property

		Friend Sub New()
		End Sub
	End Class
End Namespace
