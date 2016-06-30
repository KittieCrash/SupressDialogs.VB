Imports System
Imports System.CodeDom.Compiler
Imports System.Configuration
Imports System.Runtime.CompilerServices

Namespace WebBrowserControlDialogs.Properties
	<GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"), CompilerGenerated()>
	Friend Class Settings
		Inherits ApplicationSettingsBase

		Private Shared defaultInstance As Settings = CType(SettingsBase.Synchronized(New Settings()), Settings)

		Public Shared ReadOnly Property [Default]() As Settings
			Get
				Return Settings.defaultInstance
			End Get
		End Property
	End Class
End Namespace
