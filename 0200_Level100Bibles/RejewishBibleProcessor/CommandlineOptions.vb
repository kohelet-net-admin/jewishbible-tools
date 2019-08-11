Imports CommandLine

Public Class CommandlineOptions

    ''' <summary>
    ''' Path to configuration CSV file
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("c", "configpath", Required:=True, HelpText:="Path to configuration CSV file")>
    Public Property ConfigPath As String

End Class
