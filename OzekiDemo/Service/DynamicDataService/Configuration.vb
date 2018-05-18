Imports System.Configuration
Imports System.Security

Public Class Configuration

    Private _serviceUrl As String

    Public Property ServiceUrl As String
        Get
            Return _serviceUrl
        End Get

        Set(ByVal value As String)
            _serviceUrl = value
        End Set
    End Property

    Private _username As String

    Public Property Username As String
        Get
            Return _username
        End Get

        Set(ByVal value As String)
            _username = value
        End Set
    End Property

    Private _password As SecureString

    Public Property Password As SecureString
        Get
            Return _password
        End Get

        Set(ByVal value As SecureString)
            _password = value
        End Set
    End Property

    Private _domain As String

    Public Property Domain As String
        Get
            Return _domain
        End Get

        Set(ByVal value As String)
            _domain = value
        End Set
    End Property

    Private _clientId As String

    Public Property ClientId As String
        Get
            Return _clientId
        End Get

        Set(ByVal value As String)
            _clientId = value
        End Set
    End Property

    Private _redirectUrl As String

    Public Property RedirectUrl As String
        Get
            Return _redirectUrl
        End Get

        Set(ByVal value As String)
            _redirectUrl = value
        End Set
    End Property

    Private _name As String

    Public Property Name As String
        Get
            Return _name
        End Get

        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Sub New(ByVal connectionStringName As String)
        Name = connectionStringName
        GetConnectionStringValues(Name)
    End Sub

    Public Sub GetConnectionStringValues(ByVal connectionName As String)
        Try
            Dim config = ConfigurationManager.ConnectionStrings
            Dim connection As ConnectionStringSettings
            Dim appSettings = ConfigurationManager.AppSettings
            connection = config(connectionName)
            Name = connectionName
            Application.WriteEventToLog("Connection=" & connection.ToString)
            If connection IsNot Nothing Then
                Dim parameters = connection.ConnectionString.Split(";"c)
                For Each parameter As String In parameters
                    Dim trimmedParamters = parameter.Trim()
                    If trimmedParamters.StartsWith("Url=") Then
                        ServiceUrl = parameter.Replace("Url=", String.Empty).TrimStart(" "c)
                    End If

                    If trimmedParamters.StartsWith("Username=") Then
                        Username = parameters(1).Replace("Username=", String.Empty).TrimStart(" "c)
                    End If

                    If trimmedParamters.StartsWith("Password=") Then
                        Dim passwrd = parameters(2).Replace("Password=", String.Empty).TrimStart(" "c)
                        Password = New SecureString()
                        For Each c As Char In passwrd
                            Password.AppendChar(c)
                        Next
                    End If

                    If trimmedParamters.StartsWith("Domain=") Then
                        Domain = parameter.Replace("Domain=", String.Empty).TrimStart(" "c)
                    End If
                Next
            Else
                Application.WriteErrorToLog("The specified connection string could not be found.")
            End If

            RedirectUrl = My.Settings.RedirectURL
            ClientId = My.Settings.ClientID
        Catch e As Exception
            Application.WriteErrorToLog(e.ToString & " at GetConnectionStringValues")
        End Try
    End Sub
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by Refactoring Essentials.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================

