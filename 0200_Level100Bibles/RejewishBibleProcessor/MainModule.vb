Imports CommandLine

Module MainModule

    Function Main(ByVal args As String()) As Integer
        AddHandler System.Console.CancelKeyPress, AddressOf ConsoleAppControlCKeyHandler
        Return CommandLine.Parser.Default.ParseArguments(Of CommandlineOptions)(args) _
        .MapResult(
             (Function(opts As CommandlineOptions) RunAndReturnExitCode(opts)),
             (Function(errs As IEnumerable(Of [Error])) 1)
        )
    End Function

    Public Function RunAndReturnExitCode(options As CommandlineOptions) As Integer
        Try
            CompuMaster.Console.WriteLine("Reading configuration file " & options.ConfigPath)
            Dim ConfigData As System.Data.DataTable = Configuration.LoadConfig("Bibles.config", options, True)
            CompuMaster.Console.WriteLine("Reading configuration file completed")
            CompuMaster.Console.WriteLine("Reading index of registered ZefaniaXml bibles at " & options.IndexOfRegisteredBiblesPath)
            Dim IndexOfRegisteredBiblesPath As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.StandardBiblesDirectory, "index.csv")
            If System.IO.File.Exists(IndexOfRegisteredBiblesPath) = False Then Throw New System.IO.FileNotFoundException("File not found: " & IndexOfRegisteredBiblesPath)
            Dim IndexOfRegisteredBibles As System.Data.DataTable = CompuMaster.Data.Csv.ReadDataTableFromCsvFile(IndexOfRegisteredBiblesPath, True, "UTF-8", ","c, """"c)
            CompuMaster.Console.WriteLine("Reading index of registered ZefaniaXml bibles completed")
            Dim InputBaseDir As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.StandardBiblesDirectory)
            If System.IO.Directory.Exists(InputBaseDir) = False Then Throw New System.IO.DirectoryNotFoundException("Directory not found: " & InputBaseDir)
            Dim OutputBaseDir As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.JewishBiblesDirectory)
            If System.IO.Directory.Exists(InputBaseDir) = False Then Throw New System.IO.DirectoryNotFoundException("Directory not found: " & OutputBaseDir)
            Dim LogFileTxt As String = System.IO.Path.Combine(options.StandardBiblesDirectory, "log.0200_BibleProcessor.txt")
            Dim LogFileHtml As String = System.IO.Path.Combine(options.StandardBiblesDirectory, "log.0200_BibleProcessor.html")
            Dim XsdDirectory As String = System.IO.Path.Combine(options.XsdSchemaDirectory)
            If System.IO.Directory.Exists(XsdDirectory) = False Then Throw New System.IO.DirectoryNotFoundException("Directory not found: " & XsdDirectory)
            Dim ProcessorConfig As New Configuration(ConfigData)

            'Cleanup of output files
            CompuMaster.Console.WriteLine("Clean-up (.xml, .csv, .log) at output directory " & OutputBaseDir)
            CleanupOutputDirectory(OutputBaseDir, False)

            'BiblesToProcess
            For MyCounter As Integer = 0 To ProcessorConfig.BiblesToProcess.Count - 1
                If TerminateProcessImmediately Then Exit For
                If IsMatchAlwaysIgnorePath(ProcessorConfig.BiblesToProcess(MyCounter).Path, ProcessorConfig) = False Then
                    Try
                        CompuMaster.Console.WriteLine()
                        CompuMaster.Console.WriteLine("Bible in process: " & ProcessorConfig.BiblesToProcess(MyCounter).Path)
                        Dim Bible As KoheletNetwork.ZefaniaXmlBible
                        Bible = BibleProcessor.CreateLevel100Bible(ProcessorConfig.BiblesToProcess(MyCounter), options)
                    Catch ex As Exception
                        If options.Verbose Then
                            CompuMaster.Console.WarnLine("ERROR: " & ex.ToString)
                        Else
                            CompuMaster.Console.WarnLine("ERROR: " & ex.Message)
                        End If
                    End Try
                End If
            Next

            If TerminateProcessImmediately = False Then
                'CompuMaster.Data.Csv.WriteDataTableToCsvFile(XmlIndexDetailsCsvFile, xmlFiles, True, System.Globalization.CultureInfo.InvariantCulture, "UTF-8", ",", """"c)
                'CompuMaster.Data.XlsEpplus.WriteDataTableToXlsFileAndFirstSheet(XmlIndexDetailsXlsxFile, xmlFiles)
            End If
            CompuMaster.Console.SaveHtmlLog(LogFileHtml)
            CompuMaster.Console.SavePlainTextLog(LogFileTxt)

            'End program with success
            Return 0
        Catch ex As Exception
            If options.Verbose Then
                CompuMaster.Console.WarnLine("CRITICAL ERROR: " & ex.ToString)
            Else
                CompuMaster.Console.WarnLine("CRITICAL ERROR: " & ex.Message)
            End If
            Return 2
        End Try
    End Function

    Private Function IsMatchAlwaysIgnorePath(biblePath As String, processorConfig As Configuration) As Boolean
        For MyIgnoreRuleCounter As Integer = 0 To processorConfig.AlwaysIgnorePath.Count - 1
            Dim CurrentCheckRule As String = processorConfig.AlwaysIgnorePath(MyIgnoreRuleCounter)
            If FilterUtils.IsFilterMatch(biblePath, CurrentCheckRule, FilterUtils.CaseSensitivity.Windows) Then
                Return True
            ElseIf FilterUtils.IsFilterMatch(biblePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), CurrentCheckRule, FilterUtils.CaseSensitivity.Windows) Then
                Return True
            ElseIf FilterUtils.IsFilterMatch(biblePath.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar), CurrentCheckRule, FilterUtils.CaseSensitivity.Windows) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private TerminateProcessImmediately As Boolean = False

    Sub ConsoleAppControlCKeyHandler(sender As Object, args As ConsoleCancelEventArgs)
        TerminateProcessImmediately = True
        args.Cancel = True
    End Sub

    Public Sub CleanupOutputDirectory(directory As String, removeIfEmpty As Boolean)
        For Each fileName As String In System.IO.Directory.EnumerateFiles(directory)
            Select Case System.IO.Path.GetExtension(fileName).ToLowerInvariant
                Case ".xml", ".csv", ".log"
                    System.IO.File.Delete(fileName)
            End Select
        Next
        For Each dirName As String In System.IO.Directory.EnumerateDirectories(directory)
            If System.IO.Path.GetFileName(dirName).StartsWith(".") = False Then
                CompuMaster.Console.WriteLine("CLEANUP: " & dirName)
                CleanupOutputDirectory(dirName, True)
            End If
        Next
        Dim DirIsEmpty As Boolean = True
        For Each fsItemName As String In System.IO.Directory.EnumerateFileSystemEntries(directory)
            CompuMaster.Console.WriteLine("FOUND: " & fsItemName)
            DirIsEmpty = False
        Next
        If DirIsEmpty Then
            System.IO.Directory.Delete(directory, False)
        End If
    End Sub

End Module
