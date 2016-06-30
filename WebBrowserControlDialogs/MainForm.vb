Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace WebBrowserControlDialogs
	Public Class MainForm
		Inherits Form

		Private components As IContainer = Nothing

		Private splitContainer1 As SplitContainer

		Private webBrowser1 As WebBrowser

		Private btnTestNoSecurityAlertDialog As Button

		Private btnTestNoCredentialsDialog As Button

        Public Sub New()
            Me.InitializeComponent()
            AddHandler WindowsInterop.SecurityAlertDialogWillBeShown, AddressOf Me.WindowsInterop_SecurityAlertDialogWillBeShown
            'AddHandler WindowsInterop.ConnectToDialogWillBeShown, AddressOf Me.WindowsInterop_ConnectToDialogWillBeShown
            AddHandler Me.webBrowser1.DocumentCompleted, AddressOf Me.webBrowser1_DocumentCompleted
            Me.navigateTo("https://www.github.com/")
        End Sub

        Private Sub btnTestNoSecurityAlertDialog_Click(sender As Object, e As EventArgs)
            Dim text As String = "https://expired.badssl.com/"
            If String.IsNullOrEmpty(text) Then
                MessageBox.Show(Me, "Please provide a Url in the Source Code", "btnTestNoSecurityAlertDialog_Click(): No URL Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            Else
                Me.navigateTo(text)
			End If
		End Sub

		Private Function WindowsInterop_SecurityAlertDialogWillBeShown(blnIsSSLDialog As Boolean) As Boolean
			Return True
		End Function

		Private Function WindowsInterop_ConnectToDialogWillBeShown(ByRef sUsername As String, ByRef sPassword As String) As Boolean
			sUsername = ""
			sPassword = ""
			Return True
		End Function

        Private Sub webBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
            Me.btnTestNoSecurityAlertDialog.Enabled = True
            Me.Cursor = Cursors.[Default]
        End Sub

        Private Sub navigateTo(sUrl As String)
            Me.Cursor = Cursors.WaitCursor
            Me.btnTestNoSecurityAlertDialog.Enabled = False
			Me.webBrowser1.Navigate(sUrl)
		End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
			Me.splitContainer1 = New SplitContainer()
            Me.webBrowser1 = New WebBrowser()
            Me.btnTestNoSecurityAlertDialog = New Button()
			Me.splitContainer1.Panel1.SuspendLayout()
			Me.splitContainer1.Panel2.SuspendLayout()
			Me.splitContainer1.SuspendLayout()
            MyBase.SuspendLayout()
            Me.splitContainer1.Dock = DockStyle.Fill
			Me.splitContainer1.FixedPanel = FixedPanel.Panel2
			Me.splitContainer1.IsSplitterFixed = True
			Me.splitContainer1.Location = New Point(0, 0)
			Me.splitContainer1.Name = "splitContainer1"
			Me.splitContainer1.Orientation = Orientation.Horizontal
			Me.splitContainer1.Panel1.Controls.Add(Me.webBrowser1)
			Me.splitContainer1.Panel2.Controls.Add(Me.btnTestNoCredentialsDialog)
			Me.splitContainer1.Panel2.Controls.Add(Me.btnTestNoSecurityAlertDialog)
			Me.splitContainer1.Size = New Size(875, 613)
			Me.splitContainer1.SplitterDistance = 562
			Me.splitContainer1.TabIndex = 0
			Me.webBrowser1.Dock = DockStyle.Fill
			Me.webBrowser1.Location = New Point(0, 0)
			Me.webBrowser1.MinimumSize = New Size(20, 20)
			Me.webBrowser1.Name = "webBrowser1"
			Me.webBrowser1.Size = New Size(875, 562)
			Me.webBrowser1.TabIndex = 0
            Me.btnTestNoSecurityAlertDialog.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
            Me.btnTestNoSecurityAlertDialog.Location = New Point(709, 12)
            Me.btnTestNoSecurityAlertDialog.Name = "btnTestNoSecurityAlertDialog"
            Me.btnTestNoSecurityAlertDialog.Size = New Size(154, 23)
            Me.btnTestNoSecurityAlertDialog.TabIndex = 3
            Me.btnTestNoSecurityAlertDialog.Text = "Test no 'Security Alert' Dialog"
            Me.btnTestNoSecurityAlertDialog.UseVisualStyleBackColor = True
            AddHandler Me.btnTestNoSecurityAlertDialog.Click, AddressOf Me.btnTestNoSecurityAlertDialog_Click
            MyBase.AutoScaleDimensions = New SizeF(6F, 13F)
			MyBase.AutoScaleMode = AutoScaleMode.Font
			MyBase.ClientSize = New Size(875, 613)
			MyBase.Controls.Add(Me.splitContainer1)
			MyBase.Name = "MainForm"
			Me.Text = "WebBrowserControlDialogs Demo"
			Me.splitContainer1.Panel1.ResumeLayout(False)
			Me.splitContainer1.Panel2.ResumeLayout(False)
			Me.splitContainer1.ResumeLayout(False)
			MyBase.ResumeLayout(False)
		End Sub
	End Class
End Namespace
