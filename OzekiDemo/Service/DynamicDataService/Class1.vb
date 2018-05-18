

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports ADALV3_For_CRM
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows

Imports System.Reflection
Imports System.IO
Imports System.Security
Imports DynamicDataService
Imports NLog


Class DataOP


    Shared httpClient As HttpClient

    Private Shared Property contactProperties As String() = {"fullname", "mobilephone", "telephone1", "contactid"}

    Shared accountProperties As String() = {"name"}

    Private Shared Property contactList As List(Of DbContact)

    'Private Shared Property contact As DataObjects.Contact

    Private Shared Property dbContact As DbContact

    Private Shared Property contactUrl As String = String.Empty

    Private Shared logger As Logger = LogManager.GetCurrentClassLogger()

    Private Shared connectionString As String = String.Empty

    'Private Shared dbcontext As ZoiperdbContext

    Function GetContacts() As List(Of DbContact)
        Try

            Dim apiVersion As String = "api/data/v9.0"
            Dim IClient As New ConnectToCRM
            httpClient = IClient.TheHttpClient(apiVersion)
            Dim expand As String = "&$expand=parentcustomerid_account($select=" & String.Join(",", accountProperties) & ")"
            Dim queryOperation As String = "?$select=" & String.Join(",", contactProperties) & expand

            Dim retrieveResponse As HttpResponseMessage = httpClient.GetAsync(httpClient.BaseAddress.ToString & "/contacts" & queryOperation).Result

            If retrieveResponse.StatusCode = System.Net.HttpStatusCode.OK Then

                logger.Info("retrieveResponse is OK")
                ' LibraryLog.WriteErrorLog("retrieveResponse is OK")
                Dim retrievedContacts As Object = JsonConvert.DeserializeObject(retrieveResponse.Content.ReadAsStringAsync().Result)

                logger.Info("Looping through values")
                '  LibraryLog.WriteErrorLog("Looping through values")
                contactList = New List(Of DbContact)

                For Each item In retrievedContacts.value

                    Dim cont As Contact = New Contact()
                    cont.FullName = item.fullname.Value
                    cont.MobilePhone = item.mobilephone.Value
                    cont.BusinessPhone = item.telephone1.Value
                    cont.Company = If(item.parentcustomerid_account IsNot Nothing, item.parentcustomerid_account.name.Value, "Nil")
                    cont.ContactGuid = item.contactid.Value

                    logger.Info("Inside the loop")
                    logger.Info($"FullName {item.fullname.Value}:  contactid {generateContactUrl(item.contactid.Value)}")
                    logger.Info("After logging value to Nlog")

                    If Not String.IsNullOrEmpty(cont.MobilePhone) Then
                        dbContact = New DbContact(cont.FullName, cont.MobilePhone, cont.Company, generateContactUrl(cont.ContactGuid))
                        logger.Info("Add to contactlist")
                        logger.Info("Mobile phone is not empty: Contact Added")
                        contactList.Add(dbContact)
                        '   LibraryLog.WriteErrorLog("Mobile phone is not empty: Contact Added " & dbContact.Url)
                        logger.Info("After Addition to contactlist")
                    End If

                    If Not String.IsNullOrEmpty(cont.BusinessPhone) Then
                        dbContact = New DbContact(cont.FullName, cont.BusinessPhone, cont.Company, generateContactUrl(cont.ContactGuid))
                        logger.Info("Add to contactlist")
                        logger.Info("Business phone is not empty: Contact Added")
                        contactList.Add(dbContact)
                        '  LibraryLog.WriteErrorLog("Business phone is not empty: Contact Added: " & dbContact.Url)
                        logger.Info("After Addition to contactlist")
                    End If

                Next

            End If

            '   LibraryLog.WriteErrorLog("Loop is over")
            logger.Info("Loop is over")
            Return contactList
        Catch ex As Exception
            logger.[Error](ex, ex.Message)
            ' LibraryLog.WriteErrorLog(ex)
            Return contactList
        End Try
    End Function

    Private Function generateContactUrl(ByVal contactGuid As String) As String
        Return $"https://abisoyeltd.crm4.dynamics.com/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{contactGuid}%7D"
    End Function

    Sub InsertContactData(ByVal contact As List(Of DbContact))
        connectionString = GetAppPath("\Zoiperdb.sdf")
        dbcontext = New ZoiperdbContext(connectionString)
        Try
            For Each item In contact
                Dim tblContact As TblPhoneContact = New TblPhoneContact() With {.ContactName = item.FullName, .Organization = item.Company, .Telephone = item.Phone, .Url = item.Url, .EntryDate = DateTime.Now}
                dbcontext.TblPhoneContact.InsertOnSubmit(tblContact)
                dbcontext.SubmitChanges()
            Next
        Catch ex As Exception
            LibraryLog.WriteErrorLog(ex.Message)
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
            LibraryLog.WriteErrorLog(ex.Message)
        End Try

        Return contact
    End Function

    Function GetPath(ByVal imagePath As String) As String
        Dim exeFile As String = New System.Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath
        Dim Dir As String = path.GetDirectoryName(exeFile)
        Dim path As String = path.GetFullPath(path.Combine(Dir, imagePath))
        Return path
    End Function

    Function GetAppPath(ByVal dbPath As String) As String
        Dim Dir2 As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim path2 As String = Dir2 & dbPath
        Return path2
    End Function

    Function SaveSettings(ByVal settings As Settings) As Boolean
        Try
            If settings IsNot Nothing Then
                connectionString = GetAppPath("\Zoiperdb.sdf")
                dbcontext = New ZoiperdbContext(connectionString)
                Dim zoiperSetting As TblZoiperSetting = New TblZoiperSetting() With {.Domain = settings.Domain, .Username = settings.Username, .CallerId = settings.CallerId, .ServiceDuration = settings.ServiceDuration, .IsRestart = Convert.ToInt32(settings.Restart), .Password = SharedFunctions.Hash(settings.Password)}
                dbcontext.TblZoiperSetting.InsertOnSubmit(zoiperSetting)
                dbcontext.SubmitChanges()
            End If

            Return True
        Catch ex As Exception
            LibraryLog.WriteErrorLog(ex)
            Return False
        End Try
    End Function

    Function GetServiceDuration() As Integer
        connectionString = GetAppPath("\Zoiperdb.sdf")
        dbcontext = New ZoiperdbContext(connectionString)
        Try
            Dim setting As TblZoiperSetting = dbcontext.TblZoiperSetting.FirstOrDefault()
            Return setting.ServiceDuration
        Catch ex As Exception
            logger.[Error](ex, ex?.Message)
            LibraryLog.WriteErrorLog(ex)
            Return Convert.ToInt32(False)
        End Try
    End Function

    Function TruncateTable() As Boolean
        connectionString = GetAppPath("\Zoiperdb.sdf")
        Dim query As String = "DELETE from TblPhoneContact"
        Try
            dbcontext = New ZoiperdbContext(connectionString)
            Dim result As Boolean = True
            If dbcontext.DatabaseExists() Then
                dbcontext.ExecuteCommand(query)
            End If

            Return result
        Catch ex As Exception
            LibraryLog.WriteErrorLog(ex)
            Return False
        End Try
    End Function

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


    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by Refactoring Essentials.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

End Class
