Imports System.IO
Imports System.Xml.Serialization

Public Class Zef1014Deserializer

    Public Shared Sub MainZefaniaXmlTest()
        Dim t As New Zef1014Deserializer()
        t.ReadPO("elb1905.xml")
    End Sub 'Main

    Public Shared Sub Main(file As String)
        Dim t As New Zef1014Deserializer()
        t.ReadPO(file)
    End Sub 'Main

    Protected Sub ReadPO(filename As String)
        ' Create an instance of the XmlSerializer class;
        ' specify the type of object to be deserialized.
        Dim serializer As New XmlSerializer(GetType(ZefaniaXml2005.XMLBIBLE))
        '' If the XML document has been altered with unknown
        '' nodes or attributes, handle them with the
        '' UnknownNode and UnknownAttribute events.
        'AddHandler serializer.UnknownNode, AddressOf serializer_UnknownNode
        'AddHandler serializer.UnknownAttribute, AddressOf serializer_UnknownAttribute
        ''AddHandler serializer.UnknownElement, AddressOf serializer_UnknownElement
        ''AddHandler serializer.UnreferencedObject, AddressOf serializer_UnreferencedObject

        ' A FileStream is needed to read the XML document.
        Dim fs As New FileStream(filename, FileMode.Open)
        ' Declare an object variable of the type to be deserialized.
        Dim po As ZefaniaXml2005.XMLBIBLE
        ' Use the Deserialize method to restore the object's state with
        ' data from the XML document. 
        po = CType(serializer.Deserialize(fs), ZefaniaXml2005.XMLBIBLE)
        ' Read the order date.
        Console.WriteLine(("Lang: " & po.biblename))


    End Sub 'ReadPO

    Private Sub serializer_UnknownNode(sender As Object, e As XmlNodeEventArgs)
        Console.WriteLine(("Unknown Node:" & e.Name & ControlChars.Tab & e.Text))
    End Sub 'serializer_UnknownNode


    Private Sub serializer_UnknownAttribute(sender As Object, e As XmlAttributeEventArgs)
        Dim attr As System.Xml.XmlAttribute = e.Attr
        Console.WriteLine(("Unknown attribute " & attr.Name & "='" & attr.Value & "'"))
    End Sub 'serializer_UnknownAttribute

    Private Sub serializer_UnknownElement(sender As Object, e As XmlElementEventArgs)
        Dim elem As System.Xml.XmlElement = e.Element
        Console.WriteLine(("Unknown element " & elem.Name & "='" & elem.Value & "'"))
    End Sub 'serializer_UnknownAttribute

    Private Sub serializer_UnreferencedObject(sender As Object, e As UnreferencedObjectEventArgs)
        Dim id As String = e.UnreferencedId
        Dim obj As Object = e.UnreferencedObject
        Console.WriteLine(("Unreferenced object " & id))
    End Sub 'serializer_UnknownAttribute

End Class
