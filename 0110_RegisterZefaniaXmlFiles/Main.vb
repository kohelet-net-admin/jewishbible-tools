Option Explicit On
Option Strict On

Imports CompuMaster

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
        Dim XsdDirectory As String = System.IO.Path.Combine(baseDir, "..\Schema")
        Dim XsdSchemaFile As String = System.IO.Path.Combine(XsdDirectory, "zef2014.xsd")
        Dim XmlIndexDetailsCsvFile As String = System.IO.Path.Combine(baseDir, "index.csv")
        Dim XmlIndexDetailsXlsxFile As String = System.IO.Path.Combine(baseDir, "index.xlsx")
        Dim LogFileTxt As String = System.IO.Path.Combine(baseDir, "log.0110_RegisterZefaniaXmlFiles.txt")
        Dim LogFileHtml As String = System.IO.Path.Combine(baseDir, "log.0110_RegisterZefaniaXmlFiles.html")
        'Try
        '    Console.SetWindowSize(Console.WindowWidth * 2.5, Console.WindowHeight * 2.5)
        'Catch ex As Exception
        '    Console.ForegroundColor = ConsoleColor.Red
        '    Console.WriteLine(ex.Message)
        '    Console.ResetColor()
        'End Try
        'Console.InputEncoding = System.Text.Encoding.UTF8
        'Console.OutputEncoding = System.Text.Encoding.UTF8
        Dim LastFileInProcess As String = Nothing
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
                LastFileInProcess = XmlFilePath
                Dim FullXmlFilePath As String = System.IO.Path.Combine(baseDir, XmlFilePath)
                Dim FullXmlErrorLogPath As String = FullXmlFilePath & ".errors.log"
                If System.IO.File.Exists(FullXmlErrorLogPath) Then System.IO.File.Delete(FullXmlErrorLogPath)
                If System.IO.File.Exists(FullXmlFilePath) = False Then
                    Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("MISSING XML FILE: " & FullXmlFilePath)
                    Console.ForegroundColor = ForegroundDefaulColor
                    XmlFile("Warnings") = "Missing XML file"
                Else
                    Console.WriteLine("USING XML FILE: " & XmlFilePath)
                    Dim ZefaniaData As New ZefaniaXmlBible(FullXmlFilePath, XsdDirectory)
                    XmlFile("ZefaniaXmlSchemaVersion") = ZefaniaData.SchemaName
                    XmlFile("ZefaniaBibleName") = ZefaniaData.BibleName
                    XmlFile("ZefaniaBibleStatus") = ZefaniaData.BibleStatus
                    XmlFile("ZefaniaBibleVersion") = ZefaniaData.BibleVersion
                    XmlFile("ZefaniaBibleRevision") = ZefaniaData.BibleRevision
                    XmlFile("ZefaniaBibleType") = ZefaniaData.BibleType
                    XmlFile("ZefaniaBibleInfoTitle") = ZefaniaData.BibleInfoTitle
                    XmlFile("ZefaniaBibleInfoIdentifier") = ZefaniaData.BibleInfoIdentifier
                    If CompuMaster.Data.Utils.NoDBNull(XmlFile("MD5-Hash"), "") = "" Then XmlFile("MD5-Hash") = ZefaniaXmlValidation.MD5FileHash(FullXmlFilePath)
                    'XmlFile("IsValidXml") = System.Enum.GetName(GetType(ZefaniaXmlValidation.ValidationResult), ZefaniaXmlValidation.ValidateXml("http://www.bgfdb.de/zefaniaxml/2014/", XsdSchemaFile, FullXmlFilePath))
                    XmlFile("IsValidXml") = System.Enum.GetName(GetType(ZefaniaXmlBible.ValidationResult), ZefaniaData.IsValidXml)
                    'Provide file with collected errors regarding bible file data
                    ZefaniaData.ValidateDeeply()
                    If ZefaniaData.ValidationErrors.Count > 0 Then
                        Dim sb As New System.Text.StringBuilder
                        For MyCounter As Integer = 0 To ZefaniaData.ValidationErrors.Count - 1
                            sb.AppendLine(ZefaniaData.ValidationErrors(MyCounter).ToString)
                        Next
                        System.IO.File.WriteAllText(FullXmlErrorLogPath, sb.ToString, System.Text.Encoding.UTF8)
                        If sb.Length > 254 Then
                            XmlFile("Warnings") = sb.ToString(0, 251) & "..."
                        Else
                            XmlFile("Warnings") = sb.ToString
                        End If
                    End If
                    'Provide books listing for each bible
                    Try
                        ExportBibleBookNames(FullXmlFilePath & ".books.csv", ZefaniaData)
                    Catch ex As Exception
                        'Console.WarnLine("WARNING: " & ex.Message)
                        Console.WarnLine("WARNING: " & ex.ToString)
                    End Try
                    ''Check and compare validation results
                    'Dim ZefaniaXmlValidationValidateXmlErrorStatus = (ZefaniaXmlValidation.ValidateXml("http://www.bgfdb.de/zefaniaxml/2014/", XsdSchemaFile, FullXmlFilePath) = ZefaniaXmlValidation.ValidationResult.HasErrors)
                    'If ZefaniaXmlValidationValidateXmlErrorStatus Then Console.Beep()
                    'Dim ZefaniaDataIsValidXmlErrorStatus = Not ZefaniaData.IsValidXml
                    'If ZefaniaXmlValidationValidateXmlErrorStatus Or ZefaniaDataIsValidXmlErrorStatus Or
                    '                        ZefaniaXmlValidationValidateXmlErrorStatus <> ZefaniaDataIsValidXmlErrorStatus Then
                    '    Console.WarnLine("Declared ZefaniaXml schema name: " & ZefaniaData.SchemaName)
                    '    Console.WarnLine("Result of ZefaniaXmlValidation.ValidateXml error status: " & ZefaniaXmlValidationValidateXmlErrorStatus)
                    '    Console.WarnLine("Result of ZefaniaData.IsValidXml error status: " & ZefaniaDataIsValidXmlErrorStatus)
                    '    For MyCounter As Integer = 0 To ZefaniaXmlBible.ValidationXmlErrors.Count - 1
                    '        Console.WarnLine("- " & ZefaniaXmlBible.ValidationXmlErrors(MyCounter).Message)
                    '    Next
                    'End If
                End If
                'Zef1014Deserializer.Main(FullXmlFilePath)
            Next
            CompuMaster.Data.Csv.WriteDataTableToCsvFile(XmlIndexDetailsCsvFile, xmlFiles, True, System.Globalization.CultureInfo.InvariantCulture, "UTF-8", ",", """"c)
            CompuMaster.Data.XlsEpplus.WriteDataTableToXlsFileAndFirstSheet(XmlIndexDetailsXlsxFile, xmlFiles)
            CompuMaster.Console.SaveHtmlLog(LogFileHtml)
            CompuMaster.Console.SavePlainTextLog(LogFileTxt)
        Catch ex As Exception
            Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
            If LastFileInProcess <> Nothing Then Console.WriteLine("Last file in process: " & LastFileInProcess)
            'Console.WarnLine("CRITICAL ERROR: " & ex.Message)
            Console.WarnLine("CRITICAL ERROR: " & ex.ToString)
            If System.Environment.GetCommandLineArgs(0).Contains(".vshost") Then Console.ReadLine()
            System.Environment.Exit(3)
        End Try
    End Sub

    Public Sub EnsureAvailableStringColumn(table As DataTable, columnName As String)
        If table.Columns.Contains(columnName) = False Then
            table.Columns.Add(columnName, GetType(String))
        End If
    End Sub

    Public Sub ExportBibleBookNames(exportCsvFilePath As String, bible As ZefaniaXmlBible)
        Dim Result As New DataTable("BibleBooks")
        Result.Columns.Add("Index", GetType(Integer))
        Result.Columns.Add("BookNumber", GetType(Integer))
        Result.Columns.Add("BookName", GetType(String))
        Result.Columns.Add("BookShortName", GetType(String))
        For MyCounter As Integer = 0 To bible.Books.Count - 1
            Dim Row As DataRow = Result.NewRow
            Row(0) = MyCounter
            Row(1) = bible.Books(MyCounter).BookNumber
            Row(2) = bible.Books(MyCounter).BookName
            Row(3) = bible.Books(MyCounter).BookShortName
            Result.Rows.Add(Row)
        Next
        'Result = CompuMaster.Data.DataTables.CreateDataTableClone(Result, "", "BookNumber") 
        CompuMaster.Data.Csv.WriteDataTableToCsvFile(exportCsvFilePath, Result, True, System.Globalization.CultureInfo.InvariantCulture)
    End Sub

End Module