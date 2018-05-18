Imports DynamicDataService.DsTableAdapters
Imports DynamicDataService.Ds
Imports CRMContact.DataOP
Imports CRMContact.DataObjects

Public Class Notifier
    Dim iCRMop As CRMContact.DataOP.DataOP
    Public ICRMobj As DbContact
    Sub New()

    End Sub
    Public Enum AccountNotificationWhen
        mBeforeExpiration = 1
        mAfterExpiration = 2
        'mBeforeActivation = 3
        mAfterActivation = 4
    End Enum
    Public Enum AccountNotificationPeriod
        mDay = 1
        mWeek = 2
        mMonth = 3
        mYear = 4
    End Enum

    Public Class ContactList

        Private newContactName As String
        Public Property ContactName() As String
            Get
                Return newContactName
            End Get
            Set(ByVal value As String)
                newContactName = value
            End Set
        End Property
        Private newTelephone As String
        Public Property Telephone() As String
            Get
                Return newTelephone
            End Get
            Set(ByVal value As String)
                newTelephone = value
            End Set
        End Property

    End Class




    Shared Function GetLastNotifiedDate(ByVal SubID As Integer) As DateTime





        Dim iClient As New Tbl_SubscriptionProfileTableAdapter
        Dim Db As Tbl_SubscriptionProfileDataTable = iClient.GetSubscriptionID(SubID)
        Dim LastDate As Date = DateTime.Parse("2000-01-01")

        If Db.Rows.Count > 0 Then
            Dim Ir As Tbl_SubscriptionProfileRow = Db.Rows(0)
            If Ir.IsLastDateNotifiedNull = False Then
                LastDate = Ir.LastDateNotified
            End If
        End If
        Return LastDate
    End Function
    Shared Function Insert2SMSCue(ByVal iTel As String, ByVal EntryDate As Date, ByVal msg As String, ByVal SubID As Integer, ByVal StrOnlineID As String) As Integer


        Dim iSuccess As Integer = 0
        If CType(My.Settings.ProjectStateID, Integer) = 0 Then  '0 Means Test
            Dim iClient As New Tbl_SMSCue_TestTableAdapter
            iSuccess = iClient.Insert(iTel, msg)
        ElseIf CType(My.Settings.ProjectStateID, Integer) = 1 Then ' 1 means Live
            Dim iClient As New Tbl_SMSCueTableAdapter
            iSuccess = iClient.Insert(iTel, msg, False, EntryDate)
        End If



        Dim iSubs As New Tbl_SubscriptionProfileTableAdapter
        iSubs.UpdateLastDateNotified(EntryDate, SubID)    'Update with SubscriptionID which is the major Update neccessary

        iSubs.UpdateLastDateNotifiedON(EntryDate, StrOnlineID) 'Update with OnlineID so that there wont be a null LastDatemodified

        ' Begin to update by Number to avoid Shareholder receiving multiple SMS bcos of one phone tied to many account

        Dim iExp As New Qry_MaccessExpiryTableAdapter
        Dim Db As Qry_MaccessExpiryDataTable = iExp.GetByPhone(iTel)
        For Each ir As Qry_MaccessExpiryRow In Db.Rows
            iSubs.UpdateLastDateNotified(EntryDate, ir.SubscriptionID)    'Update with SubscriptionID which is the major Update neccessary

            iSubs.UpdateLastDateNotifiedON(EntryDate, ir.OnlineID) 'Update with OnlineID so that there wont be a null LastDatemodified

        Next



        Return iSuccess
    End Function
End Class
