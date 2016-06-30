'******************************************************************************************
' VB.NET Proof of Concept by Derek Kelly                                                         
'
' This application is intended for demonstration of features only.
' In no circumstances should this project be built and ran in a production environment.
' *****************************************************************************************

Imports System
Imports System.Windows.Forms

Namespace WebBrowserControlDialogs
	Friend Module Program
        <STAThread()>
        Public Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            WindowsInterop.Hook()
            Application.Run(New MainForm())
            WindowsInterop.Unhook()
        End Sub
    End Module
End Namespace
