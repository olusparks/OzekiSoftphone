Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks


Public Class Contact

    Public Property FullName As String
    Public Property Address As String
    Public Property MobilePhone As String
    Public Property BusinessPhone As String
    Public Property Company As String
    Public Property ContactGuid As String

    Public Sub New()

    End Sub
End Class




Public Class DbContact
        Inherits Contact

    Public Property Url As String
    Public Property Phone As String

    Public Sub New()
        End Sub

        Public Sub New(ByVal fullname As String, ByVal phone As String, ByVal company As String, ByVal url As String)
            Me.FullName = fullname
            Me.Phone = phone
            Me.Company = company
            Me.Url = url
        End Sub

        Public Sub InsertIntoCE_DB(ByVal contact As List(Of DbContact))
        End Sub
    End Class


