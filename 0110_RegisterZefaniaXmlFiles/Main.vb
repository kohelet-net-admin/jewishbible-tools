Module Module1

    Sub Main()

        Dim baseDir As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, "..\..\..\..\Zefania-Xml-Bibles\Bibles")
        Dim XmlIndexFile As String = System.IO.Path.Combine(baseDir, "index.txt")
        Dim XsdSchemaFile As String = System.IO.Path.Combine(baseDir, "..\Schema\zef2014.xsd")
        Console.SetWindowSize(Console.WindowWidth * 2.5, Console.WindowHeight * 2.5)
        Console.InputEncoding = System.Text.Encoding.UTF8
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Try
            If System.IO.File.Exists(XmlIndexFile) = False Then
                Console.WriteLine("MISSING XML INDEX FILE: " & New IO.FileInfo(XmlIndexFile).FullName)
                System.Environment.Exit(1)
            End If
            Dim xmlFiles As String() = System.IO.File.ReadAllLines(XmlIndexFile)
            For Each XmlFile As String In xmlFiles
                Dim FullXmlFilePath As String = System.IO.Path.Combine(baseDir, XmlFile)
                If System.IO.File.Exists(FullXmlFilePath) = False Then
                    Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("MISSING XML FILE: " & FullXmlFilePath)
                    Console.ForegroundColor = ConsoleColor.Black
                    System.Environment.Exit(2)
                Else
                    Console.WriteLine("USING XML FILE: " & XmlFile)
                    Console.WriteLine("Validation Result: " & System.Enum.GetName(GetType(XPathValidation.ValidationResult), XPathValidation.ValidateXml(XsdSchemaFile, FullXmlFilePath)))
                    Console.WriteLine()
                End If
            Next
        Catch ex As Exception
            Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Warning {0}", ex.Message)
            Console.ForegroundColor = ConsoleColor.Black
            System.Environment.Exit(3)
        End Try
    End Sub

End Module
