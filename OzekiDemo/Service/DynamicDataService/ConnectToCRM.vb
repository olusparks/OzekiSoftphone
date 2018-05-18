Imports System.Net.Http

Public Class ConnectToCRM
    Sub New()

    End Sub
    Function TheHttpClient(ByVal apiVersion As String) As HttpClient
        Dim config As Configuration = New Configuration("CrmOnline")
        Dim auth As Authentication = New Authentication(config)
        Dim url As String = config.ServiceUrl
        Dim accessToken As String = Authentication.AcquireToken().AccessToken
        Dim httpClient As HttpClient = New HttpClient()
        httpClient.BaseAddress = New Uri(url & apiVersion)
        httpClient.Timeout = New TimeSpan(0, 2, 0)
        httpClient.DefaultRequestHeaders.Authorization = New System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken)
        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0")
        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0")
        httpClient.DefaultRequestHeaders.Accept.Add(New System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))
        Return httpClient
    End Function
End Class
