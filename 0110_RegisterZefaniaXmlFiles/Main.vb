﻿Option Explicit On
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
        Dim XsdSchemaFile As String = System.IO.Path.Combine(baseDir, "..\Schema\zef2014.xsd")
        Dim XmlIndexDetailsCsvFile As String = System.IO.Path.Combine(baseDir, "index.csv")
        Dim XmlIndexDetailsXlsxFile As String = System.IO.Path.Combine(baseDir, "index.xlsx")
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
                If System.IO.File.Exists(FullXmlFilePath) = False Then
                    Dim ForegroundDefaulColor As ConsoleColor = Console.ForegroundColor
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("MISSING XML FILE: " & FullXmlFilePath)
                    Console.ForegroundColor = ForegroundDefaulColor
                    XmlFile("Warnings") = "Missing XML file"
                Else
                    Console.WriteLine("USING XML FILE: " & XmlFilePath)
                    Dim ZefaniaData As New ZefaniaXmlBible(FullXmlFilePath, My.Application.Info.DirectoryPath)
                    XmlFile("ZefaniaXmlSchemaVersion") = ZefaniaData.SchemaName
                    XmlFile("ZefaniaBibleName") = ZefaniaData.BibleName
                    XmlFile("ZefaniaBibleStatus") = ZefaniaData.BibleStatus
                    XmlFile("ZefaniaBibleVersion") = ZefaniaData.BibleVersion
                    XmlFile("ZefaniaBibleRevision") = ZefaniaData.BibleRevision
                    XmlFile("ZefaniaBibleType") = ZefaniaData.BibleType
                    XmlFile("ZefaniaBibleInfoTitle") = ZefaniaData.BibleInfoTitle
                    XmlFile("ZefaniaBibleInfoIdentifier") = ZefaniaData.BibleInfoIdentifier
                    If CompuMaster.Data.Utils.NoDBNull(XmlFile("MD5-Hash"), "") = "" Then XmlFile("MD5-Hash") = ZefaniaXmlValidation.MD5FileHash(FullXmlFilePath)
                    XmlFile("IsValidXml") = System.Enum.GetName(GetType(ZefaniaXmlValidation.ValidationResult), ZefaniaXmlValidation.ValidateXml("http://www.bgfdb.de/zefaniaxml/2014/", XsdSchemaFile, FullXmlFilePath))
                    Try
                        ExportBibleBookNames(FullXmlFilePath & ".books.csv", ZefaniaData)
                    Catch ex As Exception
                        'Console.WarnLine("WARNING: " & ex.Message)
                        Console.WarnLine("WARNING: " & ex.ToString)
                    End Try
                End If
                'Zef1014Deserializer.Main(FullXmlFilePath)
            Next
            CompuMaster.Data.Csv.WriteDataTableToCsvFile(XmlIndexDetailsCsvFile, xmlFiles, True, System.Globalization.CultureInfo.InvariantCulture, "UTF-8", ",", """"c)
            CompuMaster.Data.XlsEpplus.WriteDataTableToXlsFileAndFirstSheet(XmlIndexDetailsXlsxFile, xmlFiles)
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
        Result = CompuMaster.Data.DataTables.CreateDataTableClone(Result, "", "BookNumber")
        CompuMaster.Data.Csv.WriteDataTableToCsvFile(exportCsvFilePath, Result, True, System.Globalization.CultureInfo.InvariantCulture)
    End Sub

End Module