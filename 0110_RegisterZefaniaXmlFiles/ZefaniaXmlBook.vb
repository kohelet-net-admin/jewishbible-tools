Public Class ZefaniaXmlBook

    Private ParentBible As ZefaniaXmlBible
    Private BookXmlNode As System.Xml.XmlNode

    Friend Sub New(bible As ZefaniaXmlBible, xmlNode As System.Xml.XmlNode)
        Me.ParentBible = bible
        Me.BookXmlNode = xmlNode
    End Sub

    Public ReadOnly Property BookName As String
        Get
            Throw New NotImplementedException
        End Get
    End Property


End Class
