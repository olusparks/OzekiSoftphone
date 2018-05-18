Imports System.Net.Http
Imports Microsoft.IdentityModel.Clients.ActiveDirectory
Imports System.Threading.Tasks


Public Class Authentication


    Private Shared _config As Configuration = Nothing

    Private Shared _clientHandler As HttpMessageHandler = Nothing

    Private Shared _context As AuthenticationContext = Nothing

    Private Shared _authority As String = Nothing

    Public Property ClientHandler As HttpMessageHandler
        Get
            Return _clientHandler
        End Get

        Set(ByVal value As HttpMessageHandler)
            _clientHandler = value
        End Set
    End Property

    Public Property Context As AuthenticationContext
        Get
            Return _context
        End Get

        Set(ByVal value As AuthenticationContext)
            _context = value
        End Set
    End Property

    Public Property Authority As String
        Get
            If _authority IsNot Nothing Then
                _authority = DiscoverAuthority(_config.ServiceUrl)
            End If

            Return _authority
        End Get

        Set(ByVal value As String)
            _authority = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal config As Configuration)
        _config = config
    End Sub

    Public Shared Function DiscoverAuthority(ByVal serviceUrl As String) As String
        Try
            Dim ap As AuthenticationParameters = AuthenticationParameters.CreateFromResourceUrlAsync(New Uri(serviceUrl & "api/data/")).Result
            Return ap.Authority
        Catch e As HttpRequestException
            Throw New Exception("An HTTP request exception occurred during authority discovery.", e)
        End Try
    End Function

    Public Shared Async Function DiscoverAuthorityAsync(ByVal serviceUrl As String) As Threading.Tasks.Task(Of String)
        Try
            Dim ap As AuthenticationParameters = Await AuthenticationParameters.CreateFromResourceUrlAsync(New Uri(serviceUrl & "api/data/"))
            Return ap.Authority
        Catch e As HttpRequestException
            Throw New Exception("An HTTP request exception occurred during authority discovery.", e)
        End Try
    End Function

    Public Shared Async Function AcquireTokenAsync() As Task(Of AuthenticationResult)
        Try
            If _config IsNot Nothing AndAlso (_config.Username IsNot Nothing AndAlso _config.Password IsNot Nothing) Then
                Dim userCred As UserPasswordCredential = New UserPasswordCredential(_config.Username, _config.Password)
                _context = New AuthenticationContext(DiscoverAuthorityAsync(_config.ServiceUrl).Result)
                Return Await _context.AcquireTokenAsync(_config.ServiceUrl, _config.ClientId, userCred)
            End If
        Catch e As Exception
            Throw New Exception("Authentication failed. Verify the configuration values are correct.", e)
        End Try

        Return Nothing
    End Function

    Public Shared Function AcquireToken() As AuthenticationResult
        Try
            If _config IsNot Nothing AndAlso (_config.Username IsNot Nothing AndAlso _config.Password IsNot Nothing) Then
                Dim userCred As UserPasswordCredential = New UserPasswordCredential(_config.Username, _config.Password)
                _context = New AuthenticationContext(DiscoverAuthority(_config.ServiceUrl))
                Return _context.AcquireTokenAsync(_config.ServiceUrl, _config.ClientId, userCred).Result
            End If
        Catch e As Exception
            Throw New Exception("Authentication failed. Verify the configuration values are correct.", e)
        End Try

        Return Nothing
    End Function
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by Refactoring Essentials.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================

