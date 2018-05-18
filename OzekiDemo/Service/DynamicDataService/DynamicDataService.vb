Imports System.Runtime.InteropServices
Imports DynamicDataService.DsTableAdapters
Imports DynamicDataService.Ds
Imports DynamicDataService.Notifier
Imports CRMContact.DataOP
Imports CRMContact.DataObjects

Public Class DynamicDataService
    ' Declare Auto Function SetServiceStatus Lib "advapi32.dll" (ByVal handle As IntPtr, ByRef serviceStatus As ServiceStatus) As Boolean
    Dim mTimer As System.Timers.Timer = Nothing
    Dim rCounter As Long = 0
    Dim CRM As New DataOP
    Public Enum ServiceState
        SERVICE_STOPPED = 1
        SERVICE_START_PENDING = 2
        SERVICE_STOP_PENDING = 3
        SERVICE_RUNNING = 4
        SERVICE_CONTINUE_PENDING = 5
        SERVICE_PAUSE_PENDING = 6
        SERVICE_PAUSED = 7
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Structure ServiceStatus
        Public dwServiceType As Long
        Public dwCurrentState As ServiceState
        Public dwControlsAccepted As Long
        Public dwWin32ExitCode As Long
        Public dwServiceSpecificExitCode As Long
        Public dwCheckPoint As Long
        Public dwWaitHint As Long
    End Structure
    Protected Overrides Sub OnStart(ByVal args() As String)

        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.

        Application.WriteEventToLog("Entering OnStart Mode: " & Now.ToString)
        mTimer = New System.Timers.Timer()
        ' mTimer.Interval = 60000 ' 60 seconds
        Dim mSec As Integer = 60000 ' 1 Minutes
        Dim mInterval As Integer = 1
        mInterval = IIf(CRM.GetServiceDuration = 0, 1, CRM.GetServiceDuration) ' IIf(String.IsNullOrEmpty(My.Settings.IntervalinMinutes) = True, 1, CType(My.Settings.IntervalinMinutes, Integer))
        mTimer.Interval = mInterval * mSec
        '   AddHandler mTimer.Elapsed, AddressOf Me.DoNotification
        AddHandler mTimer.Elapsed, AddressOf Me.LoadCRMdata
        mTimer.Start()
        Application.WriteEventToLog("About to Begin Operation : " + Now.ToString())
    End Sub
    Protected Overrides Sub OnContinue()
        Application.WriteEventToLog("In OnContinue.")
    End Sub

    Protected Overrides Sub OnStop()
        Application.WriteEventToLog("Service has  Stopped : " + Now.ToString())
    End Sub
    Sub LoadCRMdata()
        Try
            Dim iC As New List(Of DbContact)

            iC = CRM.GetContacts
            Application.WriteEventToLog(iC.Count.ToString & " Records fetched from Dynamic CRM")
            If iC.Count > 1 Then
                If CRM.IdentityReset = True Then
                    Application.WriteEventToLog("Identity Reset Successfull")
                    CRM.InsertContactData(iC)
                End If
            End If
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString + " Async at " & Now.ToString())
        End Try
    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Me.DataLog = New System.Diagnostics.EventLog
        'If Not System.Diagnostics.EventLog.SourceExists("CRMcontact") Then
        '    System.Diagnostics.EventLog.CreateEventSource("CRMcontact", "CRMcontactLog")
        'End If
        'DataLog.Source = "CRMcontact"
        'DataLog.Log = "CRMcontactLog"

        Application.CreateEventLog()

    End Sub
    Private Sub DoNotification()
        ' TODO: Insert monitoring activities here.
        Try
            mTimer.Stop()
            rCounter = 0
            Application.WriteEventToLog("Entering Notification engine : " + Now.ToString())
            '   RunNotifier()
            Application.WriteEventToLog("End of Notification engine : " + Now.ToString())
        Catch ex As Exception
            Application.WriteEventToLog("Error occurred : " + ex.ToString())
        Finally
            mTimer.Start()
        End Try

    End Sub
    Private Shared Sub WriteEventLogEntry(ByVal message As String)
        ' Create an instance of EventLog
        Dim eventLog As New System.Diagnostics.EventLog()

        ' Check if the event source exists. If not create it.
        If Not System.Diagnostics.EventLog.SourceExists("CRMcontact") Then
            System.Diagnostics.EventLog.CreateEventSource("CRMcontact", "Application")
        End If

        ' Set the source name for writing log entries.
        eventLog.Source = "CRMcontact"
        ' Create an event ID to add to the event log
        Dim eventID As Integer = 8
        ' Write an entry to the event log.
        eventLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information, eventID)

        ' Close the Event Log
        eventLog.Close()
    End Sub




End Class
