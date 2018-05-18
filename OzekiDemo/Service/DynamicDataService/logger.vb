
Imports System.Security.Cryptography
    Imports System.Text
    Imports System.IO.IsolatedStorage
    Imports System.IO

Public Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Public Shared Function CreateEventLog() As Boolean
        Dim iSuccess As Boolean = False
        Try

            If EventLog.SourceExists("CRMcontact") Then
                iSuccess = False
            Else
                EventLog.CreateEventSource("CRMcontact", "CRMcontactLog")
                Dim iLog As EventLog = GetEventLog()
                iLog.MaximumKilobytes = 2048
                iLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 7)
                'The source is created.  Exit the application to allow it to be registered.
                iSuccess = True
            End If
        Catch ex As Exception
            iSuccess = False
            WriteEventToApplication(ex.ToString)

        End Try
        Return iSuccess

    End Function

    Shared Function GetEventLog() As EventLog

        ' Create an EventLog instance and assign its source.
        Dim myLog As New EventLog()
        myLog.Source = "CRMcontact"
        Return (myLog)

    End Function


    Public Shared Sub WriteErrorToLog(ByVal Exception As System.Exception)
        Try
            Dim iLog As EventLog = GetEventLog()
            iLog.WriteEntry(Exception.ToString, EventLogEntryType.Error)
        Catch ex As Exception
            WriteEventToApplication(ex.ToString)
        End Try
    End Sub

    Public Shared Sub WriteErrorToLog(ByVal ErrorMessage As String)
        Try
            Dim iLog As EventLog = GetEventLog()
            iLog.WriteEntry(ErrorMessage, EventLogEntryType.Error)
        Catch ex As Exception
            WriteEventToApplication(ex.ToString)
        End Try
    End Sub

    Public Shared Sub WriteEventToLog(ByVal Message As String)
        Try
            Dim iLog As EventLog = GetEventLog()
            iLog.WriteEntry(Message, EventLogEntryType.Information)
        Catch ex As Exception
            WriteEventToApplication(ex.ToString)
        End Try
    End Sub

    Public Shared Sub WriteWarningToLog(ByVal Exception As System.Exception)
        Try
            Dim iLog As EventLog = GetEventLog()
            iLog.WriteEntry(Exception.ToString, EventLogEntryType.Warning)
        Catch ex As Exception
            WriteEventToApplication(ex.ToString)
        End Try
    End Sub

    Public Shared Sub WriteWarningToLog(ByVal ErrorMessage As String)
        Try
            Dim iLog As EventLog = GetEventLog()
            iLog.WriteEntry(ErrorMessage, EventLogEntryType.Warning)
        Catch ex As Exception
            WriteEventToApplication(ex.ToString)
        End Try
    End Sub


    Public Shared Sub WriteEventToApplication(ByVal Message As String)
        Try
            WriteEventLogEntry(Message)
        Catch ex As Exception
            MsgBox(ex.ToString)
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
        eventLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.[Error], eventID)

        ' Close the Event Log
        eventLog.Close()
    End Sub


End Class



