Imports CommandLine

Public Class CommandlineOptions

    ''' <summary>
    ''' Path to configuration CSV file
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("c", "configpath", Required:=True, HelpText:="Path to configuration CSV file")>
    Public Property ConfigPath As String

    ''' <summary>
    ''' Path to configuration CSV file
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("r", "indexCsvRegisteredBibles", Required:=True, HelpText:="Path to index CSV file of registered bibles")>
    Public Property IndexOfRegisteredBiblesPath As String

    ''' <summary>
    ''' Verbose output/logging
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("v", "verbose", HelpText:="Print additional logging output")>
    Public Property Verbose As Boolean

    ''' <summary>
    ''' The input directory for standard bibles
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("i", "inputDir", Required:=True, HelpText:="Path to input directory of registered bibles")>
    Public Property StandardBiblesDirectory As String

    ''' <summary>
    ''' The output directory for re-Jewished bibles
    ''' </summary>
    ''' <returns></returns>
    <CommandLine.Option("o", "outputDir", Required:=True, HelpText:="Path to output directory for re-Jewished bibles")>
    Public Property JewishBiblesDirectory As String

End Class
