Imports System
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.XPath

Class XPathValidation

    Public Enum ValidationResult As Byte
        NoWarnings = 0
        HasWarnings = 1
        HasErrors = 2
    End Enum

    Shared Function ValidateXml(schemaFile As String, xmlFile As String) As ValidationResult
        Try
            Dim settings As XmlReaderSettings = New XmlReaderSettings()
            settings.Schemas.Add("http://www.bgfdb.de/zefaniaxml/2014/", schemaFile)
            settings.ValidationType = ValidationType.Schema

            Dim reader As XmlReader = XmlReader.Create(xmlFile, settings)
            Dim document As XmlDocument = New XmlDocument()
            document.Load(reader)

            Dim eventHandler As ValidationEventHandler = New ValidationEventHandler(AddressOf ValidationEventHandler)
            document.Validate(eventHandler)
        Catch ex As Exception
            ValidationErrors.Add(ex)
            Return ValidationResult.HasErrors
        End Try
        If ValidationErrors.Count > 0 Then
            Return ValidationResult.HasErrors
        ElseIf ValidationWarnings.Count > 0 Then
            Return ValidationResult.HasWarnings
        Else
            Return ValidationResult.NoWarnings
        End If
    End Function

    Public Shared ReadOnly Property ValidationWarnings As New List(Of Exception)
    Public Shared ReadOnly Property ValidationErrors As New List(Of Exception)

    Private Shared Sub ValidationEventHandler(ByVal sender As Object, ByVal e As ValidationEventArgs)
        Select Case e.Severity
            Case XmlSeverityType.Error
                ValidationErrors.Add(e.Exception)
            Case XmlSeverityType.Warning
                ValidationWarnings.Add(e.Exception)
        End Select

    End Sub

End Class