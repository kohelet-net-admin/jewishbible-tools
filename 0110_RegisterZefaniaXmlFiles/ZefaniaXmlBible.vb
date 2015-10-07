Option Strict On
Option Explicit On

Imports System.Xml

Public Class ZefaniaXmlBible

    Public Sub New(filePath As String)
        Dim reader As XmlReader = Nothing
        reader = XmlReader.Create(filePath)
        Dim document As XmlDocument = New XmlDocument()
        document.Load(reader)
        XmlDocument = document
        reader.Close()
        reader.Dispose()
    End Sub

    Private ReadOnly Property XmlDocument As XmlDocument

    Public ReadOnly Property SchemaName As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("xsi:noNamespaceSchemaLocation").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleName As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("biblename").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleRevision As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("revision").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleStatus As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("status").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleVersion As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("version").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleType As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).Attributes("type").Value
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleInfoTitle As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).SelectNodes("INFORMATION").Item(0).SelectNodes("title").Item(0).InnerText
            Catch
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property BibleInfoIdentifier As String
        Get
            Try
                Return XmlDocument.SelectNodes("/XMLBIBLE").Item(0).SelectNodes("INFORMATION").Item(0).SelectNodes("identifier").Item(0).InnerText
            Catch
                Return Nothing
            End Try
        End Get
    End Property

End Class
