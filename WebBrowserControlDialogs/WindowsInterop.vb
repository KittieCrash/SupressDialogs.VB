Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace WebBrowserControlDialogs
	Friend Module WindowsInterop
		Private Enum HookType
			WH_JOURNALRECORD
			WH_JOURNALPLAYBACK
			WH_KEYBOARD
			WH_GETMESSAGE
			WH_CALLWNDPROC
			WH_CBT
			WH_SYSMSGFILTER
			WH_MOUSE
			WH_HARDWARE
			WH_DEBUG
			WH_SHELL
			WH_FOREGROUNDIDLE
			WH_CALLWNDPROCRET
			WH_KEYBOARD_LL
			WH_MOUSE_LL
		End Enum

		Private Structure CWPRETSTRUCT
			Public lResult As IntPtr

			Public lParam As IntPtr

			Public wParam As IntPtr

			Public message As UInteger

			Public hwnd As IntPtr
		End Structure

		Private Delegate Function HookProcedureDelegate(iCode As Integer, pWParam As IntPtr, pLParam As IntPtr) As Integer

		Private Delegate Function EnumerateWindowDelegate(pHwnd As IntPtr, pParam As IntPtr) As Boolean

		Private Const WM_COMMAND As Integer = 273

		Private Const WM_INITDIALOG As Integer = 272

		Private _pWH_CALLWNDPROCRET As IntPtr = IntPtr.Zero

		Private _WH_CALLWNDPROCRET_PROC As WindowsInterop.HookProcedureDelegate = AddressOf WindowsInterop.WH_CALLWNDPROCRET_PROC

        Friend Event SecurityAlertDialogWillBeShown As GenericDelegate(Of Boolean, Boolean)


        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function SetWindowsHookEx(hooktype As WindowsInterop.HookType, callback As WindowsInterop.HookProcedureDelegate, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function UnhookWindowsHookEx(hhk As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function GetWindowTextLength(hWnd As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function GetWindowText(hWnd As IntPtr, text As StringBuilder, maxLength As Integer) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function SetWindowText(hWnd As IntPtr, lpString As String) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function GetClassName(hWnd As IntPtr, lpClassName As StringBuilder, nMaxCount As Integer) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function EnumChildWindows(hWndParent As IntPtr, callback As EnumerateWindowDelegate, data As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function SendMessage(hWnd As IntPtr, Msg As UInteger, wParam As IntPtr, lParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
        Private Function GetDlgCtrlID(hwndCtl As IntPtr) As Integer
        End Function

        Friend Sub Hook()
			Dim flag As Boolean = WindowsInterop._pWH_CALLWNDPROCRET = IntPtr.Zero
			If flag Then
                WindowsInterop._pWH_CALLWNDPROCRET = SetWindowsHookEx(HookType.WH_CALLWNDPROCRET, WindowsInterop._WH_CALLWNDPROCRET_PROC, IntPtr.Zero, CUInt(AppDomain.GetCurrentThreadId()))
            End If
		End Sub

		Friend Sub Unhook()
			Dim flag As Boolean = WindowsInterop._pWH_CALLWNDPROCRET <> IntPtr.Zero
			If flag Then
				WindowsInterop.UnhookWindowsHookEx(WindowsInterop._pWH_CALLWNDPROCRET)
			End If
		End Sub

        ' Hook procedure called by the OS when a message has been processed by the target Window
        Private Function WH_CALLWNDPROCRET_PROC(iCode As Integer, pWParam As IntPtr, pLParam As IntPtr) As Integer
            Dim result As Integer
            If iCode < 0 Then
                Return CallNextHookEx(WindowsInterop._pWH_CALLWNDPROCRET, iCode, pWParam, pLParam)
            Else
                Dim cWPRETSTRUCT As CWPRETSTRUCT = CType(Marshal.PtrToStructure(pLParam, GetType(CWPRETSTRUCT)), CWPRETSTRUCT)
                If cWPRETSTRUCT.message = WM_INITDIALOG Then
                    ' A dialog was initialized, find out what type it was via its Caption Text
                    Dim windowTextLength As Integer = GetWindowTextLength(cWPRETSTRUCT.hwnd)
                    Dim stringBuilder As StringBuilder = New StringBuilder(windowTextLength + 1)

                    GetWindowText(cWPRETSTRUCT.hwnd, stringBuilder, stringBuilder.Capacity)
                    If StringConstants.DialogCaptionSecurityAlert.Equals(stringBuilder.ToString(), StringComparison.InvariantCultureIgnoreCase) Then
                        '**********************************************************************************
                        ' A "Security Aleart" dialog was initialized, so now we need:
                        ' 
                        ' 1) To know what type it is - e.g. is it an SSL related on or switching between
                        '    secure/non-secure modes?
                        ' 2) A handle to the dialog's 'Yes' button so a user-click can be simulated on it
                        '**********************************************************************************
                        Dim blnIsSslDialog As Boolean = True
                        Dim pYesButtonHwnd As IntPtr = IntPtr.Zero

                        ' Iterate over the chlid controls on the dialog and inspect their text
                        For Each pChildOfDialog As IntPtr In WindowsInterop.listChildWindows(cWPRETSTRUCT.hwnd)
                            If GetWindowTextLength(pChildOfDialog) > 0 Then
                                Dim sbProbe As StringBuilder = New StringBuilder(windowTextLength + 1)
                                GetWindowText(pChildOfDialog, sbProbe, sbProbe.Capacity)
                                Dim secureToInsecure As Boolean = StringConstants.DialogTextSecureToNonSecureWarning.Equals(sbProbe.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                Dim insecureToSecure As Boolean = StringConstants.DialogTextNonSecureToSecureWarning.Equals(sbProbe.ToString(), StringComparison.InvariantCultureIgnoreCase)

                                If secureToInsecure OrElse insecureToSecure Then
                                    ' Text says something about toggling between secure/insecure; not of interest to us
                                    blnIsSslDialog = False
                                End If

                                If StringConstants.ButtonTextYes.Equals(sbProbe.ToString(), StringComparison.InvariantCultureIgnoreCase) Then
                                    ' Dialog has a 'Yes' button! Let's cache a pointer to it!
                                    pYesButtonHwnd = pChildOfDialog
                                End If
                            End If
                        Next

                        '********************************************************************************************************
                        ' At this point, C# project verifies that SecurityAlertDialogWillBeShown is not Null.
                        ' We don't seem to have this ability in VB.NET; all we can do  with the event object is raise events.
                        ' We will have to trust that the SecurityAlertDialogWillBeShown event has been created before this call
                        '********************************************************************************************************                        
                        If blnIsSslDialog Then
                            RaiseEvent SecurityAlertDialogWillBeShown(blnIsSslDialog)
                            ' Check if the Dialog is SSL and that we have a 'Yes' button
                            If blnIsSslDialog AndAlso pYesButtonHwnd <> IntPtr.Zero Then
                                ' Inject a message into the dialog's message-pump that says the 'Yes' button was 'Pressed'
                                Dim dlgCtrlID As Integer = GetDlgCtrlID(pYesButtonHwnd)
                                SendMessage(cWPRETSTRUCT.hwnd, CType(273, UInteger), New IntPtr(dlgCtrlID), pYesButtonHwnd)

                                ' Returning at this point blocks any further processing of the WM_INITDIALOG message,
                                ' by all processes. This is CRUCIAL to ensuring the dialog never appears on the screen
                                Return 1
                            End If
                        End If
                    End If
                End If
                result = WindowsInterop.CallNextHookEx(WindowsInterop._pWH_CALLWNDPROCRET, iCode, pWParam, pLParam)
            End If
            Return result
        End Function

        ' Populate a list of all the child windows of a given parent window
        ' Calls Win32 EnumChildWindows()
        Private Function listChildWindows(p As IntPtr) As List(Of IntPtr)
            Dim lstChildWindows As List(Of IntPtr) = New List(Of IntPtr)()
            Dim gchChildWindows As GCHandle = GCHandle.Alloc(lstChildWindows)
            Try
                EnumChildWindows(p, AddressOf WindowsInterop.enumWindowsCallback, GCHandle.ToIntPtr(gchChildWindows))
            Finally
                Dim isAllocated As Boolean = gchChildWindows.IsAllocated
                If isAllocated Then
                    gchChildWindows.Free()
                End If
            End Try
            Return lstChildWindows
        End Function

        ' Callback method called when EnumChildWindows is enumerating over windows...
        ' Called by Win32 EnumChildWindows()
        Private Function enumWindowsCallback(hwnd As IntPtr, p As IntPtr) As Boolean
            Dim lstChildWindows As List(Of IntPtr) = TryCast(GCHandle.FromIntPtr(p).Target, List(Of IntPtr))
            If lstChildWindows Is Nothing Then
                Throw New InvalidCastException("GCHandle target is not expected type")
            End If
            lstChildWindows.Add(hwnd)
            Return True
		End Function
	End Module
End Namespace
