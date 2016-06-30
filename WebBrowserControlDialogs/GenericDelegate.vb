Imports System

Namespace WebBrowserControlDialogs
    Friend Delegate Sub GenericDelegate(Of T1, T2)(param As T1)
    Friend Delegate Sub GenericDelegate(Of T1, T2, T3)(ByRef param1 As T1, ByRef param2 As T2)
End Namespace
