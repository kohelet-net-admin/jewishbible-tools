Imports KoheletNetwork

Public Class BibleProcessor

    Public Shared Function CreateLevel100Bible(args As Config.BibleToProcess, options As CommandlineOptions) As KoheletNetwork.ZefaniaXmlBible
        CompuMaster.Console.WriteLine("Loading ZefaniaXML")
        Dim Bible As New KoheletNetwork.ZefaniaXmlBible(System.IO.Path.Combine(options.StandardBiblesDirectory, args.Path), options.XsdSchemaDirectory)
        'TODO
        Return Bible
    End Function

End Class
