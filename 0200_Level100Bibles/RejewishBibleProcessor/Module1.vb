Imports CommandLine

Module Module1

    Function Main(ByVal args As String()) As Integer
        Return CommandLine.Parser.Default.ParseArguments(Of CommandlineOptions)(args) _
        .MapResult(
             (Function(opts As CommandlineOptions) RunAndReturnExitCode(opts)),
             (Function(errs As IEnumerable(Of [Error])) 1)
        )
    End Function

    Public Function RunAndReturnExitCode(options As CommandlineOptions) As Integer
        Return 2
    End Function

End Module
