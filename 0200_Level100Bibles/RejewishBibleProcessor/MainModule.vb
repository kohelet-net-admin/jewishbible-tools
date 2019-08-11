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
            Dim ConfigPath As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.ConfigPath)
            If System.IO.File.Exists(ConfigPath) = False Then Throw New System.IO.FileNotFoundException("File not found: " & ConfigPath)
            Dim ConfigData As System.Data.DataTable = CompuMaster.Data.Csv.ReadDataTableFromCsvFile(ConfigPath, True, "UTF-8", ","c, """"c)
            CompuMaster.Console.WriteLine("Reading configuration file completed")
            CompuMaster.Console.WriteLine("Reading index of registered ZefaniaXml bibles at " & options.IndexOfRegisteredBiblesPath)
            Dim IndexOfRegisteredBiblesPath As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.ConfigPath)
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
            Dim ProcessorConfig As New Config(ConfigData)

            'BiblesToProcess
            For MyCounter As Integer = 0 To ProcessorConfig.BiblesToProcess.Count - 1
                If TerminateProcessImmediately Then Exit For
                CompuMaster.Console.WriteLine()
                If IsMatchAlwaysIgnorePath(ProcessorConfig.BiblesToProcess(MyCounter).Path, ProcessorConfig) = False Then
                    CompuMaster.Console.WriteLine("Bible in process: " & ProcessorConfig.BiblesToProcess(MyCounter).Path)
                    Dim Bible As KoheletNetwork.ZefaniaXmlBible
                    Bible = BibleProcessor.CreateLevel100Bible(ProcessorConfig.BiblesToProcess(MyCounter), options)

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

    Private Function IsMatchAlwaysIgnorePath(biblePath As String, processorConfig As Config) As Boolean
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

End Module
