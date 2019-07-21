Option Strict On
Option Explicit On

Imports System.Xml

Public Class ZefaniaXmlBible

    ''' <summary>
    ''' The directory path where the XSD schema files for ZefaniaXml are stored
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property XsdDirectory As String
    Private XmlFilePath As String
    Private ReadOnly Property XmlDocument As XmlDocument

    Public Sub New(filePath As String, xsdDirectory As String)
        'Setup internal fields
        Me.XmlFilePath = filePath
        Me.XsdDirectory = xsdDirectory
        'Load XmlDocument
        Dim reader As XmlReader = Nothing
        reader = XmlReader.Create(filePath)
        Dim document As XmlDocument = New XmlDocument()
        document.Load(reader)
        XmlDocument = document
        reader.Close()
        reader.Dispose()
    End Sub


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

    Private Sub ValidateXmlData()
        If Me.SchemaName = "zef2005.xsd" Then
            Dim ZefaniaXmlSchema As System.Xml.Schema.XmlSchema
            ZefaniaXmlSchema = System.Xml.Schema.XmlSchema.Read(System.Xml.XmlReader.Create(System.IO.Path.Combine(Me.XsdDirectory, Me.SchemaName)), Nothing)
            Me.XmlDocument.Schemas.Add(ZefaniaXmlSchema)
            Me.XmlDocument.Validate(Nothing)
        ElseIf Me.SchemaName = "zef2014.xsd" Then
            Dim ZefaniaXmlSchema As System.Xml.Schema.XmlSchema
            ZefaniaXmlSchema = System.Xml.Schema.XmlSchema.Read(System.Xml.XmlReader.Create(System.IO.Path.Combine(Me.XsdDirectory, Me.SchemaName)), Nothing)
            Me.XmlDocument.Schemas.Add(ZefaniaXmlSchema)
            Me.XmlDocument.Validate(Nothing)
        ElseIf Me.SchemaName = "" Then 'empty schema
            Throw New ArgumentNullException("xsi:noNamespaceSchemaLocation", "Schema declaration required")
        Else 'unknown schema
            Throw New NotSupportedException("Schema not supported: " & Me.SchemaName)
        End If
    End Sub

    Public Function ValidateXml() As XPathValidation.ValidationResult
        Return XPathValidation.ValidateXml("http://www.bgfdb.de/zefaniaxml/2014/", System.IO.Path.Combine(Me.XsdDirectory, Me.SchemaName), Me.XmlFilePath)
    End Function

    Public Function IsValidXml() As Boolean
        Try
            Me.ValidateXmlData()
            Return True
        Catch ex As System.Xml.Schema.XmlSchemaValidationException
            Return False
        End Try
    End Function

    Public ReadOnly Property Books As List(Of ZefaniaXmlBook)
        Get
            Static Result As List(Of ZefaniaXmlBook)
            If Result Is Nothing Then
                Me.ValidateXmlData()
                Dim AllBooks As XmlNodeList = XmlDocument.SelectNodes("/XMLBIBLE").Item(0).SelectNodes("BIBLEBOOK")
                For Each BookNode As XmlNode In AllBooks
                    Result.Add(New ZefaniaXmlBook(Me, BookNode))
                Next
            End If
            Return Result
        End Get
    End Property


End Class
