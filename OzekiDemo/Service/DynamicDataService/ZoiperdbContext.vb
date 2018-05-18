﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection

'Namespace DynamicDataService

<Global.System.Data.Linq.Mapping.DatabaseAttribute(Name:="C:\Users\haliaeetusvocifer\Documents\Visual Studio 2015\Projects\DynamicDataServi" &
        "ce\Zoiperdb.sdf")>
    Partial Public Class ZoiperdbContext
        Inherits System.Data.Linq.DataContext

        Private Shared mappingSource As System.Data.Linq.Mapping.MappingSource = New AttributeMappingSource()

#Region "Extensibility Method Definitions"
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub InsertTblPhoneContact(instance As TblPhoneContact)
        End Sub
        Partial Private Sub UpdateTblPhoneContact(instance As TblPhoneContact)
        End Sub
        Partial Private Sub DeleteTblPhoneContact(instance As TblPhoneContact)
        End Sub
        Partial Private Sub InsertTblZoiperSetting(instance As TblZoiperSetting)
        End Sub
        Partial Private Sub UpdateTblZoiperSetting(instance As TblZoiperSetting)
        End Sub
        Partial Private Sub DeleteTblZoiperSetting(instance As TblZoiperSetting)
        End Sub
#End Region

        Public Sub New(ByVal connection As String)
            MyBase.New(connection, mappingSource)
            OnCreated
        End Sub

        Public Sub New(ByVal connection As System.Data.IDbConnection)
            MyBase.New(connection, mappingSource)
            OnCreated
        End Sub

        Public Sub New(ByVal connection As String, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
            MyBase.New(connection, mappingSource)
            OnCreated
        End Sub

        Public Sub New(ByVal connection As System.Data.IDbConnection, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
            MyBase.New(connection, mappingSource)
            OnCreated
        End Sub

        Public ReadOnly Property TblPhoneContact() As System.Data.Linq.Table(Of TblPhoneContact)
            Get
                Return Me.GetTable(Of TblPhoneContact)
            End Get
        End Property

        Public ReadOnly Property TblZoiperSetting() As System.Data.Linq.Table(Of TblZoiperSetting)
            Get
                Return Me.GetTable(Of TblZoiperSetting)
            End Get
        End Property
    End Class

    <Global.System.Data.Linq.Mapping.TableAttribute()>
    Partial Public Class TblPhoneContact
        Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged

        Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)

        Private _ContactName As String

        Private _Organization As String

        Private _Telephone As String

        Private _Url As String

        Private _Id As Integer

        Private _EntryDate As System.Nullable(Of Date)

#Region "Extensibility Method Definitions"
        Partial Private Sub OnLoaded()
        End Sub
        Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
        End Sub
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub OnContactNameChanging(value As String)
        End Sub
        Partial Private Sub OnContactNameChanged()
        End Sub
        Partial Private Sub OnOrganizationChanging(value As String)
        End Sub
        Partial Private Sub OnOrganizationChanged()
        End Sub
        Partial Private Sub OnTelephoneChanging(value As String)
        End Sub
        Partial Private Sub OnTelephoneChanged()
        End Sub
        Partial Private Sub OnUrlChanging(value As String)
        End Sub
        Partial Private Sub OnUrlChanged()
        End Sub
        Partial Private Sub OnIdChanging(value As Integer)
        End Sub
        Partial Private Sub OnIdChanged()
        End Sub
        Partial Private Sub OnEntryDateChanging(value As System.Nullable(Of Date))
        End Sub
        Partial Private Sub OnEntryDateChanged()
        End Sub
#End Region

        Public Sub New()
            MyBase.New
            OnCreated
        End Sub

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_ContactName", DbType:="NVarChar(100)")>
        Public Property ContactName() As String
            Get
                Return Me._ContactName
            End Get
            Set
                If (String.Equals(Me._ContactName, value) = False) Then
                    Me.OnContactNameChanging(value)
                    Me.SendPropertyChanging
                    Me._ContactName = value
                    Me.SendPropertyChanged("ContactName")
                    Me.OnContactNameChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Organization", DbType:="NVarChar(100)")>
        Public Property Organization() As String
            Get
                Return Me._Organization
            End Get
            Set
                If (String.Equals(Me._Organization, value) = False) Then
                    Me.OnOrganizationChanging(value)
                    Me.SendPropertyChanging
                    Me._Organization = value
                    Me.SendPropertyChanged("Organization")
                    Me.OnOrganizationChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Telephone", DbType:="NVarChar(15)")>
        Public Property Telephone() As String
            Get
                Return Me._Telephone
            End Get
            Set
                If (String.Equals(Me._Telephone, value) = False) Then
                    Me.OnTelephoneChanging(value)
                    Me.SendPropertyChanging
                    Me._Telephone = value
                    Me.SendPropertyChanged("Telephone")
                    Me.OnTelephoneChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Url", DbType:="NVarChar(250)")>
        Public Property Url() As String
            Get
                Return Me._Url
            End Get
            Set
                If (String.Equals(Me._Url, value) = False) Then
                    Me.OnUrlChanging(value)
                    Me.SendPropertyChanging
                    Me._Url = value
                    Me.SendPropertyChanged("Url")
                    Me.OnUrlChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Id", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=True, IsDbGenerated:=True)>
        Public Property Id() As Integer
            Get
                Return Me._Id
            End Get
            Set
                If ((Me._Id = value) _
                            = False) Then
                    Me.OnIdChanging(value)
                    Me.SendPropertyChanging
                    Me._Id = value
                    Me.SendPropertyChanged("Id")
                    Me.OnIdChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_EntryDate", DbType:="DateTime")>
        Public Property EntryDate() As System.Nullable(Of Date)
            Get
                Return Me._EntryDate
            End Get
            Set
                If (Me._EntryDate.Equals(value) = False) Then
                    Me.OnEntryDateChanging(value)
                    Me.SendPropertyChanging
                    Me._EntryDate = value
                    Me.SendPropertyChanged("EntryDate")
                    Me.OnEntryDateChanged
                End If
            End Set
        End Property

        Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging

        Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub SendPropertyChanging()
            If ((Me.PropertyChangingEvent Is Nothing) _
                        = False) Then
                RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
            End If
        End Sub

        Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
            If ((Me.PropertyChangedEvent Is Nothing) _
                        = False) Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class

    <Global.System.Data.Linq.Mapping.TableAttribute()>
    Partial Public Class TblZoiperSetting
        Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged

        Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)

        Private _Id As Integer

        Private _Domain As String

        Private _Username As String

        Private _CallerId As String

        Private _Password As String

        Private _ServiceDuration As Integer

        Private _IsRestart As System.Nullable(Of Integer)

#Region "Extensibility Method Definitions"
        Partial Private Sub OnLoaded()
        End Sub
        Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
        End Sub
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub OnIdChanging(value As Integer)
        End Sub
        Partial Private Sub OnIdChanged()
        End Sub
        Partial Private Sub OnDomainChanging(value As String)
        End Sub
        Partial Private Sub OnDomainChanged()
        End Sub
        Partial Private Sub OnUsernameChanging(value As String)
        End Sub
        Partial Private Sub OnUsernameChanged()
        End Sub
        Partial Private Sub OnCallerIdChanging(value As String)
        End Sub
        Partial Private Sub OnCallerIdChanged()
        End Sub
        Partial Private Sub OnPasswordChanging(value As String)
        End Sub
        Partial Private Sub OnPasswordChanged()
        End Sub
        Partial Private Sub OnServiceDurationChanging(value As Integer)
        End Sub
        Partial Private Sub OnServiceDurationChanged()
        End Sub
        Partial Private Sub OnIsRestartChanging(value As System.Nullable(Of Integer))
        End Sub
        Partial Private Sub OnIsRestartChanged()
        End Sub
#End Region

        Public Sub New()
            MyBase.New
            OnCreated
        End Sub

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Id", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=True, IsDbGenerated:=True)>
        Public Property Id() As Integer
            Get
                Return Me._Id
            End Get
            Set
                If ((Me._Id = value) _
                            = False) Then
                    Me.OnIdChanging(value)
                    Me.SendPropertyChanging
                    Me._Id = value
                    Me.SendPropertyChanged("Id")
                    Me.OnIdChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Domain", DbType:="NVarChar(100) NOT NULL", CanBeNull:=False)>
        Public Property Domain() As String
            Get
                Return Me._Domain
            End Get
            Set
                If (String.Equals(Me._Domain, value) = False) Then
                    Me.OnDomainChanging(value)
                    Me.SendPropertyChanging
                    Me._Domain = value
                    Me.SendPropertyChanged("Domain")
                    Me.OnDomainChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Username", DbType:="NVarChar(100) NOT NULL", CanBeNull:=False)>
        Public Property Username() As String
            Get
                Return Me._Username
            End Get
            Set
                If (String.Equals(Me._Username, value) = False) Then
                    Me.OnUsernameChanging(value)
                    Me.SendPropertyChanging
                    Me._Username = value
                    Me.SendPropertyChanged("Username")
                    Me.OnUsernameChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_CallerId", DbType:="NVarChar(100) NOT NULL", CanBeNull:=False)>
        Public Property CallerId() As String
            Get
                Return Me._CallerId
            End Get
            Set
                If (String.Equals(Me._CallerId, value) = False) Then
                    Me.OnCallerIdChanging(value)
                    Me.SendPropertyChanging
                    Me._CallerId = value
                    Me.SendPropertyChanged("CallerId")
                    Me.OnCallerIdChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Password", DbType:="NVarChar(100) NOT NULL", CanBeNull:=False)>
        Public Property Password() As String
            Get
                Return Me._Password
            End Get
            Set
                If (String.Equals(Me._Password, value) = False) Then
                    Me.OnPasswordChanging(value)
                    Me.SendPropertyChanging
                    Me._Password = value
                    Me.SendPropertyChanged("Password")
                    Me.OnPasswordChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_ServiceDuration", DbType:="Int NOT NULL")>
        Public Property ServiceDuration() As Integer
            Get
                Return Me._ServiceDuration
            End Get
            Set
                If ((Me._ServiceDuration = value) _
                            = False) Then
                    Me.OnServiceDurationChanging(value)
                    Me.SendPropertyChanging
                    Me._ServiceDuration = value
                    Me.SendPropertyChanged("ServiceDuration")
                    Me.OnServiceDurationChanged
                End If
            End Set
        End Property

        <Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_IsRestart", DbType:="Int")>
        Public Property IsRestart() As System.Nullable(Of Integer)
            Get
                Return Me._IsRestart
            End Get
            Set
                If (Me._IsRestart.Equals(value) = False) Then
                    Me.OnIsRestartChanging(value)
                    Me.SendPropertyChanging
                    Me._IsRestart = value
                    Me.SendPropertyChanged("IsRestart")
                    Me.OnIsRestartChanged
                End If
            End Set
        End Property

        Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging

        Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub SendPropertyChanging()
            If ((Me.PropertyChangingEvent Is Nothing) _
                        = False) Then
                RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
            End If
        End Sub

        Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
            If ((Me.PropertyChangedEvent Is Nothing) _
                        = False) Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
'End Namespace
