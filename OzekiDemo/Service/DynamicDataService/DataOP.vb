Imports System.Net.Http
Imports NLog
Imports System.Reflection
Imports System.IO
Imports Newtonsoft.Json
Imports DynamicDataService
Imports System.Text
Imports Newtonsoft.Json.Linq

'Namespace DynamicDataService
Public Class DataOP
    Shared httpClient As HttpClient

    Private Shared Property contactProperties As String() = {"fullname", "mobilephone", "telephone1", "contactid"}

    Shared accountProperties As String() = {"name"}

    Private Shared Property contactList As List(Of DbContact)

    'Private Shared Property contact As DataObjects.Contact

    Private Shared Property dbContact As DbContact

    Private Shared Property contactUrl As String = String.Empty

    Private Shared logger As Logger = LogManager.GetCurrentClassLogger()

    Private Shared connectionString As String = String.Empty

    Private Shared dbcontext As ZoiperdbContext

    Sub New()

    End Sub
#Region "CRM OPERATION"
    Function GetContacts() As List(Of DbContact)

        Try
            Application.WriteEventToLog("Entering GetContact Method")

            Dim apiVersion As String = "api/data/v9.0"
            Dim IClient As New ConnectToCRM
            httpClient = IClient.TheHttpClient(apiVersion)
            Dim expand As String = "&$expand=parentcustomerid_account($select=" & String.Join(",", accountProperties) & ")"
            Dim queryOperation As String = "?$select=" & String.Join(",", contactProperties) & expand
            Application.WriteEventToLog(httpClient.BaseAddress.ToString & "/contacts" & queryOperation)
            Dim retrieveResponse As HttpResponseMessage = httpClient.GetAsync(httpClient.BaseAddress.ToString & "/contacts" & queryOperation).Result
            If retrieveResponse.StatusCode = System.Net.HttpStatusCode.OK Then
                ' logger.Info("retrieveResponse is OK")
                Application.WriteEventToLog("retrieveResponse is OK")
                ' LibraryLog.WriteErrorLog("retrieveResponse is OK")
                Dim retrievedContacts = JsonConvert.DeserializeObject(retrieveResponse.Content.ReadAsStringAsync().Result)
                ' logger.Info("Looping through values")
                Application.WriteEventToLog("Looping through values")
                '  LibraryLog.WriteErrorLog("Looping through values")
                contactList = New List(Of DbContact)

                For Each item As JToken In retrievedContacts.SelectToken("value")
                    Dim cont As Contact = New Contact()

                    cont.FullName = item.SelectToken("fullname")
                    cont.MobilePhone = item.SelectToken("mobilephone")
                    cont.BusinessPhone = item.SelectToken("telephone1")
                    cont.ContactGuid = item.SelectToken("contactid")
                    cont.Company = item.SelectToken("parentcustomerid_account").SelectToken("name")
                    Application.WriteEventToLog("After logging value to Nlog")
                    Application.WriteEventToLog("Inside the loop")
                    '  Dim mString As String = String.Format("fullname={0};mobilephone={1};contactID={2};bussinessphone={3}", item.SelectToken("fullname"), item.SelectToken("mobilephone"), generateContactUrl(cont.ContactGuid), item.SelectToken("telephone1"))
                    ' Application.WriteEventToLog(mString)
                    If Not String.IsNullOrEmpty(cont.MobilePhone) Then
                        dbContact = New DbContact(cont.FullName, cont.MobilePhone, cont.Company, generateContactUrl(cont.ContactGuid))
                        ' logger.Info("Add to contactlist")
                        ' logger.Info("Mobile phone Is Not empty: Contact Added")
                        contactList.Add(dbContact)
                        '   LibraryLog.WriteErrorLog("Mobile phone is not empty: Contact Added " & dbContact.Url)
                        ' Application.WriteEventToLog("Mobile Null Check")
                    End If

                    If Not String.IsNullOrEmpty(cont.BusinessPhone) Then
                        dbContact = New DbContact(cont.FullName, cont.BusinessPhone, cont.Company, generateContactUrl(cont.ContactGuid))
                        ' logger.Info("Add to contactlist")
                        ' logger.Info("Business phone is not empty: Contact Added")
                        contactList.Add(dbContact)
                        '  LibraryLog.WriteErrorLog("Business phone is not empty: Contact Added: " & dbContact.Url)
                        ' Application.WriteEventToLog("Business Phone Null Check")
                    End If
                Next
            End If
            Application.WriteEventToLog("Loop is over")
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at GetContact" & Now)
        End Try
        Return contactList
    End Function
#End Region


#Region "DB OPERATION"
    Sub InsertContactData(ByVal contact As List(Of DbContact))
        connectionString = GetAppPath("\Zoiperdb.sdf")
        dbcontext = New ZoiperdbContext(connectionString)
        Dim Counter As Integer = 0
        Try
            For Each item In contact
                Dim tblContact As TblPhoneContact = New TblPhoneContact() With {.ContactName = item.FullName, .Organization = item.Company, .Telephone = item.Phone, .Url = item.Url, .EntryDate = Now}
                dbcontext.TblPhoneContact.InsertOnSubmit(tblContact)
                dbcontext.SubmitChanges()
                Counter = Counter + 1
            Next
            Application.WriteEventToLog("No of Contact feteched: " & Counter.ToString)
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at InsertContactData" & Now)
        End Try
    End Sub

    Function GetContactByPhone(ByVal phoneNumber As String) As DbContact
        connectionString = GetAppPath("\Zoiperdb.sdf")
        dbcontext = New ZoiperdbContext(connectionString)
        Dim contact As DbContact = New DbContact()
        Try
            Dim tblContact As TblPhoneContact = dbcontext.TblPhoneContact.Where(Function(x) x.Telephone = phoneNumber).FirstOrDefault()
            contact = New DbContact(tblContact.ContactName, tblContact.Telephone, tblContact.Organization, tblContact.Url)
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at GetContactByPhone" & Now)
        End Try

        Return contact
    End Function

    Function SaveSettings(ByVal settings As Settings) As Boolean
        Dim iSuccess As Boolean = False
        Try
            If settings IsNot Nothing Then
                connectionString = GetAppPath("\Zoiperdb.sdf")
                dbcontext = New ZoiperdbContext(connectionString)
                Dim zoiperSetting As TblZoiperSetting = New TblZoiperSetting() With {
                    .Domain = settings.Domain,
                    .Username = settings.Username,
                    .CallerId = settings.CallerId,
                    .ServiceDuration = settings.ServiceDuration,
                    .IsRestart = Convert.ToInt32(settings.Restart),
                    .Password = Hash(settings.Password)}
                dbcontext.TblZoiperSetting.InsertOnSubmit(zoiperSetting)
                dbcontext.SubmitChanges()
                iSuccess = True
            End If
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at SaveSettings" & Now)
            iSuccess = False
        End Try
        Return iSuccess
    End Function

    Function GetServiceDuration() As Integer
        connectionString = GetAppPath("\Zoiperdb.sdf")
        dbcontext = New ZoiperdbContext(connectionString)
        Dim duration As Integer = 0
        Try
            Dim setting As TblZoiperSetting = dbcontext.TblZoiperSetting.FirstOrDefault()
            duration = setting.ServiceDuration
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at GetServiceDuration" & Now)
            duration = Convert.ToInt32(False)
        End Try
        Return duration
    End Function

    Function IdentityReset() As Boolean
        Dim strsql As String = String.Empty
        Dim iSuccess As Boolean = False
        'Droping the constraint,
        strsql = "ALTER TABLE [TblPhoneContact] DROP CONSTRAINT [PK__TblPhoneContact__0000000000000019]"
        iSuccess = Sqloperation(strsql)
        'dropping the column
        If iSuccess = True Then  'drop column after successfully droping constrainst
            strsql = "ALTER TABLE [TblPhoneContact] DROP COLUMN [Id]"
            iSuccess = Sqloperation(strsql)
            If iSuccess = True Then 'Delete table content after successfully droping  column and the constrainst
                strsql = "DELETE from TblPhoneContact"
                iSuccess = Sqloperation(strsql)
                If iSuccess = True Then ' if delete was successful, then add identity ID column and the Primary key Constrsaint
                    strsql = "ALTER TABLE [TblPhoneContact] ADD [Id] int IDENTITY (1, 1)  Not NULL"
                    iSuccess = Sqloperation(strsql)
                    strsql = "ALTER TABLE [TblPhoneContact] ADD CONSTRAINT [PK__TblPhoneContact__0000000000000019] PRIMARY KEY ([Id])"
                    iSuccess = Sqloperation(strsql)
                End If
            End If
        End If
        Return iSuccess
    End Function


    Function Sqloperation(query As String) As Boolean
        connectionString = GetAppPath("\Zoiperdb.sdf")
        Dim iSuccess As Boolean = False
        '  Dim query As String = "DELETE from TblPhoneContact"
        Try
            dbcontext = New ZoiperdbContext(connectionString)
            If dbcontext.DatabaseExists() Then
                dbcontext.ExecuteCommand(query)
                iSuccess = True
            End If
        Catch ex As Exception
            Application.WriteEventToLog(ex.ToString & " error at Sql Operation" & Now)
            iSuccess = False
        End Try
        Return iSuccess
    End Function
#End Region


#Region "HELPER METHODS"
    Public Shared Function Hash(ByVal password As String) As String
        Dim bytes = New UTF8Encoding().GetBytes(password)
        Dim hashBytes As Byte()
        Using algorithm = New System.Security.Cryptography.SHA512Managed()
            hashBytes = algorithm.ComputeHash(bytes)
        End Using

        Return Convert.ToBase64String(hashBytes)
    End Function

    Private Function generateContactUrl(ByVal contactGuid As String) As String
        Return String.Format("https://genericinsurancedemo.crm4.dynamics.com/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{0}%7D", contactGuid)
    End Function

    Function GetPath(ByVal imagePath As String) As String
        Dim exeFile As String = New System.Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath
        Dim Dir As String = Path.GetDirectoryName(exeFile)
        Dim dbPath As String = Path.GetFullPath(System.IO.Path.Combine(Dir, imagePath))
        Return dbPath
    End Function

    Function GetAppPath(ByVal dbPath As String) As String
        Dim Dir2 As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        dbPath = Dir2 & dbPath
        Return dbPath
    End Function
#End Region


    'Function BasicRetrieve(ByVal httpClient As HttpClient, ByVal phoneNumber As String) As DataObjects.Contact
    '    Dim contactProperties As String() = {"fullname", "jobtitle", "annualincome", "telephone1", "mobilephone"}
    '    Dim filter As String = "&$filter=telephone1 eq '814'"
    '    Dim queryOperation As String = "?$select=" & String.Join(",", contactProperties) & filter
    '    Dim retrieveResponse As HttpResponseMessage = httpClient.GetAsync(httpClient.BaseAddress & "/contacts" & queryOperation).Result
    '    If retrieveResponse.StatusCode = HttpStatusCode.OK Then
    '        Dim retrievedContact As Dynamic = JsonConvert.DeserializeObject(retrieveResponse.Content.ReadAsStringAsync().Result)
    '        Dim contact As DataObjects.Contact = New DataObjects.Contact()
    '        For Each item In retrievedContact.value
    '            contact.FullName = item.fullname.Value
    '        Next

    '        Return contact
    '    Else
    '        Throw New CrmHttpException(retrieveResponse.Content)
    '    End If
    'End Function

    'Function RetrieveContact(ByVal phoneNumber As String) As DataObjects.Contact
    '    Dim cred As ClientCredentials = getCredentials()
    '    Uri = New Uri(urlName)
    '    Dim contact As DataObjects.Contact = New DataObjects.Contact()
    '    Using __InlineAssignHelper(_serviceProxy, New OrganizationServiceProxy(Uri, homeRealmUri, cred, deviceCredentials))
    '        _serviceProxy.EnableProxyTypes()
    '        _service = CType(_serviceProxy, IOrganizationService)
    '        Using context As AbisoyeServiceContext = New AbisoyeServiceContext(_service)
    '            Dim queryContact = (From c In context.ContactSet Where c.Telephone1 = phoneNumber OrElse c.MobilePhone = phoneNumber Select New With {Key .fullname = c.FullName, Key .email = c.EMailAddress1, Key .jobtitle = c.JobTitle, Key .mobilephone = c.MobilePhone, Key .businessphone = c.Telephone1, Key .contactGuid = c.ContactId.Value}).FirstOrDefault()
    '            contact.FullName = queryContact.fullname
    '            contact.MobilePhone = queryContact.mobilephone
    '            contact.BusinessPhone = queryContact.businessphone
    '        End Using
    '    End Using

    '    Return contact
    'End Function

    'Private Function getCredentials() As ClientCredentials
    '    Dim cred As ClientCredentials = New ClientCredentials()
    '    cred.UserName.UserName = "abisoyes@abisoyeltd.onmicrosoft.com"
    '    cred.UserName.Password = "Adejumoke1!"
    '    Return cred
    'End Function

    <Obsolete("Please refactor code that uses this function, it is a simple work-around to simulate inline assignment in VB!")>
    Private Shared Function __InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

End Class
'End Namespace

