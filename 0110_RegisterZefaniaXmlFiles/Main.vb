Option Explicit On
'Option Strict On

Module MainModule

    Sub Main()
        'Zef1014Deserializer.MainZefaniaXmlTest()
        'Return

        Dim commandLineArgs As String() = System.Environment.GetCommandLineArgs
        Dim workingDir As String = "."
        If commandLineArgs.Length = 2 Then
            workingDir = commandLineArgs(1)
        End If
        Dim baseDir As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, workingDir)
        'Dim XmlIndexFile As String = System.IO.Path.Combine(baseDir, "index.txt")
        Dim XmlIndexCsvFile As String = System.IO.Path.Combine(baseDir, "downloads.csv")
        Dim XsdSchemaFile As String = System.IO.Path.Combine(baseDir, "..\Schema\zef2014.xsd")
        Dim XmlIndexDetailsCsvFile As String = System.IO.Path.Combine(baseDir, "index.csv")
        Dim XmlIndexDetailsXlsxFile As String = System.IO.Path.Combine(baseDir, "index.xlsx")
        Try
            Console.SetWindowSize(Console.WindowWidth * 2.5, Console.WindowHeight * 2.5)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(ex.Message)
            Console.ResetColor()
        End Try
        Console.InputEncoding = System.Text.Encoding.UTF8
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Try
            If System.IO.File.Exists(XmlIndexCsvFile) = False Then
                Console.WriteLine("MISSING XML INDEX FILE: " & New IO.FileInfo(XmlIndexCsvFile).FullName)
                System.Environment.Exit(1)
            End If
            Dim xmlFiles As System.Data.DataTable = CompuMaster.Data.Csv.ReadDataTableFromCsvFile(XmlIndexCsvFile, True, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, """"c, False, False)
            'Add required columns if required
            EnsureAvailableStringColumn(xmlFiles, "IsValidXml")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaXmlSchemaVersion")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleName")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleStatus")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleVersion")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleRevision")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleType")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleInfoTitle")
            EnsureAvailableStringColumn(xmlFiles, "ZefaniaBibleInfoIdentifier")
            EnsureAvailableStringColumn(xmlFiles, "MD5-Hash")
            EnsureAvailableStringColumn(xmlFiles, "Warnings")
            'Dim xmlFiles As String() = System.IO.File.ReadAllLines(XmlIndexFile)
            For Each XmlFile As DataRow In xmlFiles.Rows
                Dim XmlFilePath As String = CType(XmlFile("FilePath"), String)
                Dim FullXmlFilePath As String = System.IO.Path.Combine(baseDir, XmlFilePath)
                If System.IO.File.Exists(FullXmlFilePath) = False Then
                    Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("MISSING XML FILE: " & FullXmlFilePath)
                    Console.ForegroundColor = ForegroundDefaulColor
                    XmlFile("Warnings") = "Missing XML file"
                Else
                    Console.WriteLine("USING XML FILE: " & XmlFilePath)
                    Dim ZefaniaData As New ZefaniaXmlBible(FullXmlFilePath, baseDir)
                    XmlFile("ZefaniaXmlSchemaVersion") = ZefaniaData.SchemaName
                    XmlFile("ZefaniaBibleName") = ZefaniaData.BibleName
                    XmlFile("ZefaniaBibleStatus") = ZefaniaData.BibleStatus
                    XmlFile("ZefaniaBibleVersion") = ZefaniaData.BibleVersion
                    XmlFile("ZefaniaBibleRevision") = ZefaniaData.BibleRevision
                    XmlFile("ZefaniaBibleType") = ZefaniaData.BibleType
                    XmlFile("ZefaniaBibleInfoTitle") = ZefaniaData.BibleInfoTitle
                    XmlFile("ZefaniaBibleInfoIdentifier") = ZefaniaData.BibleInfoIdentifier
                    If CompuMaster.Data.Utils.NoDBNull(XmlFile("MD5-Hash"), "") = "" Then XmlFile("MD5-Hash") = MD5FileHash(FullXmlFilePath)
                    XmlFile("IsValidXml") = System.Enum.GetName(GetType(XPathValidation.ValidationResult), XPathValidation.ValidateXml("http://www.bgfdb.de/zefaniaxml/2014/", XsdSchemaFile, FullXmlFilePath))
                End If
                'Zef1014Deserializer.Main(FullXmlFilePath)
            Next
            CompuMaster.Data.Csv.WriteDataTableToCsvFile(XmlIndexDetailsCsvFile, xmlFiles, True, System.Globalization.CultureInfo.InvariantCulture, "UTF-8", ",", """"c)
            CompuMaster.Data.XlsEpplus.WriteDataTableToXlsFileAndFirstSheet(XmlIndexDetailsXlsxFile, xmlFiles)
        Catch ex As Exception
            Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red
            'Console.WriteLine("Warning {0}", ex.Message)
            Console.WriteLine("Warning {0}", ex.ToString)
            Console.ForegroundColor = ConsoleColor.Black
            If System.Environment.GetCommandLineArgs(0).Contains(".vshost") Then Console.ReadLine()
            System.Environment.Exit(3)
        End Try
    End Sub

    Public Sub EnsureAvailableStringColumn(table As DataTable, columnName As String)
        If table.Columns.Contains(columnName) = False Then
            table.Columns.Add(columnName, GetType(String))
        End If
    End Sub

    Public Function MD5FileHash(ByVal sFile As String) As String
        Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim Hash As Byte()
        Dim Result As String = ""
        Dim Tmp As String = ""

        Dim FN As New System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 8192)
        MD5.ComputeHash(FN)
        FN.Close()

        Hash = MD5.Hash
        For i As Integer = 0 To Hash.Length - 1
            Tmp = Hex(Hash(i))
            If Len(Tmp) = 1 Then Tmp = "0" & Tmp
            Result += Tmp
        Next
        Return Result
    End Function


End Module