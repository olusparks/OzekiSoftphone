Imports System.ServiceProcess
Imports System.Runtime.Remoting.Contexts

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DynamicDataService
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New mAccessMessageService, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New DynamicDataService}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        '
        'DynamicDataService
        '
        Me.CanPauseAndContinue = True
        Me.CanShutdown = True
        Me.ServiceName = "DynamicDataService"

    End Sub
    'Shared Sub Main(ByVal cmdArgs() As String)
    '    Dim ServicesToRun() As System.ServiceProcess.ServiceBase
    '    ServicesToRun = New System.ServiceProcess.ServiceBase() {New mAccessMessageService(cmdArgs)}
    '    System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    'End Sub
    Public Sub New(ByVal cmdArgs() As String)
        InitializeComponent()
        'Dim eventSourceName As String = "mAccess"
        'Dim logName As String = "mAccessLog"
        'If (cmdArgs.Count() > 0) Then
        '    eventSourceName = cmdArgs(0)
        'End If
        'If (cmdArgs.Count() > 1) Then
        '    logName = cmdArgs(1)
        'End If

        'If (Not System.Diagnostics.EventLog.SourceExists(eventSourceName)) Then
        '    System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName)
        'End If

    End Sub

End Class
