Imports KoheletNetwork

Public Class BibleProcessor

    ''' <summary>
    ''' Create level 100 bible: Jewish structure of bible
    ''' </summary>
    ''' <param name="args">Bible processing configuration</param>
    ''' <param name="options">Application's command line options</param>
    ''' <returns>The created bible document with level 100</returns>
    ''' <remarks>
    ''' Milestones:
    ''' <list type="number">
    ''' <item>Names and order of books as in Hebrew bible/Tenakh (e. g. last book of OT is 2. Chronicles, books of Torah called by their Hebrew names</item>
    ''' <item>Completeness of books as in Tenakh  (e. g. Maccabees), if necessary completion of a bible translation with parts of another alternative translation, which should be as close as possible to the origin translation style</item>
    ''' <item>Order of chapters And numbering of chapters And verses as in Tenakh (possibly differs e.g. at Joel 4, order of psalms in Russan language bibles, etc.)</item>
    ''' <item>Separation of Torah into Parashot, e.g.. by header according to scheme "Bereschit – In the beginning (%bookname 1, 1 - 5,45)"</item>
    ''' </list>
    ''' </remarks>
    Public Shared Function CreateLevel100Bible(args As Configuration.BibleToProcess, options As CommandlineOptions) As KoheletNetwork.ZefaniaXmlBible
        CompuMaster.Console.WriteLine("IN PROCESS: Level 100: Jewish structure of bible")
        'Load additional context data
        Dim BibleLanguageCode As String
        If args.Path.StartsWith(".\") OrElse args.Path.StartsWith("./") Then
            BibleLanguageCode = args.Path.Substring(2)
        Else
            BibleLanguageCode = args.Path.Substring(0)
        End If
        BibleLanguageCode = BibleLanguageCode.Substring(0, BibleLanguageCode.IndexOf("\"c))
        CompuMaster.Console.WriteLine("Bible language code: " & BibleLanguageCode)

        CompuMaster.Console.WriteLine("Loading ZefaniaXML")
        Dim Bible As New KoheletNetwork.ZefaniaXmlBible(System.IO.Path.Combine(System.Environment.CurrentDirectory, options.StandardBiblesDirectory, args.Path), options.XsdSchemaDirectory, LookupLanguageCodeFromRelativePath(args.Path))

        Dim DesiredBookOrderAndTranslation As List(Of Level100Support.BookTranslation) = Level100Support.GetLevel100BookOrderWithTranslations(BibleLanguageCode, options)
        CompuMaster.Console.WriteLine("MasterBookOrderAndTranslation contains " & DesiredBookOrderAndTranslation.Count & " books")
        CompuMaster.Console.WriteLine("MasterBookOrderAndTranslation 1st book name: " & DesiredBookOrderAndTranslation(0).BookName)

        'Prepare books collection 
        'Fill up missing books as far as possible
        Dim Bible2 As ZefaniaXmlBible = Nothing
        If args.PathAdditionalSource <> Nothing Then
            'Fill up missing books from additional source
            Bible2 = New ZefaniaXmlBible(System.IO.Path.Combine(System.Environment.CurrentDirectory, options.StandardBiblesDirectory, args.PathAdditionalSource), options.XsdSchemaDirectory, LookupLanguageCodeFromRelativePath(args.PathAdditionalSource))
            For MyAimedOrderCounter As Integer = DesiredBookOrderAndTranslation.Count - 1 To 0 Step -1
                'Find book match in current bible
                Dim MatchingBibleBookIndex As Integer = -1
                For MyBookCounter As Integer = 0 To Bible.Books.Count - 1
                    If DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID = Bible.Books(MyBookCounter).BookNumber Then
                        'Found the book reference for desired entry
                        MatchingBibleBookIndex = MyBookCounter
                        Exit For
                    End If
                Next
                If MatchingBibleBookIndex = -1 Then
                    'Remove entry of desired book without bible book match 
                    'Find book match in 2nd bible
                    Dim MatchingBible2BookIndex As Integer = -1
                    For MyBookCounter As Integer = 0 To Bible2.Books.Count - 1
                        If DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID = Bible2.Books(MyBookCounter).BookNumber Then
                            'Found the book reference for desired entry
                            MatchingBible2BookIndex = MyBookCounter
                            Exit For
                        End If
                    Next
                    If MatchingBible2BookIndex <> -1 Then
                        'Copy book from 2nd bible into 1st bible
                        If Bible.SchemaName = Bible2.SchemaName Then
                            Bible.AddBook(Bible2.Books(MatchingBible2BookIndex))
                        Else
                            Throw New NotImplementedException("Copying books between bibles of differing XML schemas not implemented, yet")
                        End If
                    End If
                End If
            Next
        End If
        'Clean up all desired books not available in bible collection
        For MyAimedOrderCounter As Integer = DesiredBookOrderAndTranslation.Count - 1 To 0 Step -1
            'Find book match in current bible
            Dim MatchingBibleBookIndex As Integer = -1
            For MyBookCounter As Integer = 0 To Bible.Books.Count - 1
                If DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID = Bible.Books(MyBookCounter).BookNumber Then
                    'Found the book reference for desired entry
                    MatchingBibleBookIndex = MyBookCounter
                    Exit For
                End If
            Next
            If MatchingBibleBookIndex = -1 Then
                'Remove entry of desired book without bible book match 
                DesiredBookOrderAndTranslation.RemoveAt(MyAimedOrderCounter)
            End If
        Next
        'Re-check again
        If DesiredBookOrderAndTranslation.Count <> Bible.Books.Count Then
            Throw New NotImplementedException("Bible contains more books than specified in master list. Desired books collection with " & DesiredBookOrderAndTranslation.Count & " books must match with books coolection of input bible with " & Bible.Books.Count & " books")
        End If
        CompuMaster.Console.WriteLine("Output books coolection will contain " & DesiredBookOrderAndTranslation.Count & " books")
        CompuMaster.Console.WriteLine("Output books coolection's 1st book name will be: " & DesiredBookOrderAndTranslation(0).BookName)

        'Reposition books
        For MyAimedOrderCounter As Integer = 0 To DesiredBookOrderAndTranslation.Count - 1
            'Find book match in current bible
            Dim MatchingBibleBookIndex As Integer = -1
            For MyBookCounter As Integer = 0 To Bible.Books.Count - 1
                If DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID = Bible.Books(MyBookCounter).BookNumber Then
                    'Found the book reference for desired entry
                    MatchingBibleBookIndex = MyBookCounter
                    Exit For
                End If
            Next
            If MatchingBibleBookIndex = -1 Then Throw New Exception("Bible book ID " & DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID & " (" & DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookShortName & ") not found in current bible")
            'Check and reposition book
            If MyAimedOrderCounter = MatchingBibleBookIndex Then
                'book is already at right position
            Else
                'book must be repositioned
                If MyAimedOrderCounter = DesiredBookOrderAndTranslation.Count - 1 Then
                    'Re-position as last book - should not be required after all other books have already been moved before the last remaining book - this one - the last one
                    Bible.Books(MatchingBibleBookIndex).MoveBookPositionInBible(Nothing)
                Else
                    'Re-position at given index
                    Bible.Books(MatchingBibleBookIndex).MoveBookPositionInBible(Bible.Books(MyAimedOrderCounter))
                End If
            End If
        Next
        'Check
        For MyAimedOrderCounter As Integer = 0 To DesiredBookOrderAndTranslation.Count - 1
            'Find book match in current bible
            Dim MatchingBibleBookIndex As Integer = -1
            For MyBookCounter As Integer = 0 To Bible.Books.Count - 1
                If DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookID = Bible.Books(MyBookCounter).BookNumber Then
                    'Found the book reference for desired entry
                    MatchingBibleBookIndex = MyBookCounter
                    Exit For
                End If
            Next
            If MyAimedOrderCounter <> MatchingBibleBookIndex Then
                Throw New Exception("Position of bible book doesn't match with desired book order after re-ordering process")
            End If
        Next

        'Rename books
        For MyAimedOrderCounter As Integer = 0 To DesiredBookOrderAndTranslation.Count - 1
            Bible.Books(MyAimedOrderCounter).BookName = DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookName
            Bible.Books(MyAimedOrderCounter).BookShortName = DesiredBookOrderAndTranslation(MyAimedOrderCounter).BookShortName
        Next
        CompuMaster.Console.WriteLine("Output books coolection contains " & Bible.Books.Count & " books")
        CompuMaster.Console.WriteLine("Output books coolection's 1st book name: " & Bible.Books(0).BookName)

        '<CAPTION vref="1" type="x-h2" count="146">Bereschit בראשית (Gen 1,1-6,8)</CAPTION>
        'Bible.Books(0).Chapters(0).Captions.Add(New ZefaniaXmlCaption()...)

        'Cache source bible(s) meta information
        Dim Bible1InfoCache As New ZefaniaXmlBibleInfoCache(Bible)
        Dim Bible2InfoCache As ZefaniaXmlBibleInfoCache = Nothing
        If Bible2 IsNot Nothing Then Bible2InfoCache = New ZefaniaXmlBibleInfoCache(Bible2)

        'Apply new bible meta information
        Bible.BibleInfoDate = Now.ToString("yyyy-MM-dd")
        Bible.BibleName = ReplaceWithBibleInfos(args.NewBibleName.Replace("{0}", Bible1InfoCache.BibleInfoTitle), Bible1InfoCache, Bible2InfoCache, 100)
        Bible.BibleInfoTitle = ReplaceWithBibleInfos(args.NewBibleInfoTitle.Replace("{NEWNAME}", Bible.BibleName).Replace("{0}", Bible1InfoCache.BibleInfoTitle), Bible1InfoCache, Bible2InfoCache, 100)
        Bible.BibleInfoIdentifier = ReplaceWithBibleInfos(args.NewBibleInfoIdentifier.Replace("{0}", Bible1InfoCache.BibleInfoIdentifier), Bible1InfoCache, Bible2InfoCache, 100)
        Bible.BibleInfoDescription = ReplaceWithBibleInfos(args.NewBibleInfoDescription.Replace("{0}", Bible1InfoCache.BibleInfoDescription), Bible1InfoCache, Bible2InfoCache, 100)
        Bible.BibleInfoPublisher = "Kohelet-Network"
        Bible.BibleInfoSource = "https://github.com/kohelet-net-admin/jewishbible/"
        Bible.BibleInfoFormat = "Zefania XML Bible Markup Language"

        'Save new bible
        Dim OutputFile As String = BibleOutputFilePath(args, options, 100, True)
        Bible.Save(OutputFile)
        ZefaniaStatistics.ExportBibleBookNames(OutputFile & ".books.csv", Bible)

        Return Bible
    End Function

    ''' <summary>
    ''' Create level 200 bible: Jewish names and terms
    ''' </summary>
    ''' <param name="args">Bible processing configuration</param>
    ''' <param name="options">Application's command line options</param>
    ''' <returns>The created bible document with level 200</returns>
    ''' <remarks>
    ''' Milestones:
    ''' <list type="number">
    ''' <item>Names of God stay in their Hebrew term (e.g. Elohim, JHWH instead of God, Lord) with origin/classic term of the Christian translation as foot note</item>
    ''' <item>Names of persons And locations stay in their Hebrew term (e.g. Jeschajahu statt Jesaja) with origin/classic term of the Christian translation as foot note</item>
    ''' <item>Fixed terms In their Hebrew version (e.g. “Talmidim” instead of “Disciples”) With origin/classic term of the Christian translation As foot note</item>
    ''' </list>
    ''' </remarks>
    Public Shared Function CreateLevel200Bible(level100Bible As KoheletNetwork.ZefaniaXmlBible, args As Configuration.BibleToProcess, options As CommandlineOptions) As KoheletNetwork.ZefaniaXmlBible
        CompuMaster.Console.WarnLine("NOT YET IMPLEMENTED: Level 200: Jewish names and terms")
        Return Nothing

        'Wording: אֱלֹהִים, אלהים
        'Start replacement wit BHS with Masoretics and missing NT
        'or start replacement with HEBM without Masoretics in whole bible, with NT, but maybe with continuous changes due to language evolution?
        'HEB\Modern Hebrew Bible\SF_2009-01-23_HEB_HEBM_(MODERN HEBREW BIBLE).xml
        'HEB\Biblia hebraica\SF_2009-01-20_HEB_BHS_(BIBLIA HEBRAICA).xml
    End Function

    ''' <summary>
    ''' Create level 300 bible: Theological and scientific correctness
    ''' </summary>
    ''' <param name="args">Bible processing configuration</param>
    ''' <param name="options">Application's command line options</param>
    ''' <returns>The created bible document with level 300</returns>
    ''' <remarks>
    ''' Milestones:
    ''' <list type="number">
    ''' <item>Theological corrections as by latest science knowledge (e.g. sometimes old Christian bible templates contain some incorrectnesses – partially already fixed in their newer revisions)</item>
    ''' <item>Theological corrections as by Messianic/Jewish view on scripture (e.g. “Shabbat” instead of “7th day”)</item>
    ''' </list>
    ''' </remarks>
    Public Shared Function CreateLevel300Bible(level200Bible As KoheletNetwork.ZefaniaXmlBible, args As Configuration.BibleToProcess, options As CommandlineOptions) As KoheletNetwork.ZefaniaXmlBible
        CompuMaster.Console.WarnLine("NOT YET IMPLEMENTED: Level 300: Theological and scientific correctness")
        Return Nothing
    End Function

    ''' <summary>
    ''' Update template text with placeholders from configuration data by bible meta information
    ''' </summary>
    ''' <param name="pattern"></param>
    ''' <param name="bible"></param>
    ''' <param name="bible2"></param>
    ''' <param name="level">100, 200 or 300</param>
    ''' <returns></returns>
    Private Shared Function ReplaceWithBibleInfos(pattern As String, bible As ZefaniaXmlBibleInfoCache, bible2 As ZefaniaXmlBibleInfoCache, level As Integer) As String
        'Arguments validation
        Select Case level
            Case 100, 200, 300
            Case Else
                Throw New ArgumentOutOfRangeException("Level must be 100, 200 or 300")
        End Select
        'Replace all variables in given pattern with bible meta data
        Dim Result As String = pattern.
            Replace("{LEVEL}", level).
            Replace("{TODAY}", Now.ToString("yyyy-MM-dd")).
            Replace("{NAME1}", bible.BibleName).
            Replace("{TITLE1}", bible.BibleInfoTitle).
            Replace("{IDENTIFIER1}", bible.BibleInfoIdentifier).
            Replace("{DESCRIPTION1}", bible.BibleInfoDescription).
            Replace("{PUBLISHER1}", bible.BibleInfoPublisher).
            Replace("{DATE1}", bible.BibleInfoDate)
        If bible2 IsNot Nothing Then
            Result = Result.
                Replace("{NAME2}", bible2.BibleName).
                Replace("{TITLE2}", bible2.BibleInfoTitle).
                Replace("{IDENTIFIER2}", bible2.BibleInfoIdentifier).
                Replace("{DESCRIPTION2}", bible2.BibleInfoDescription).
                Replace("{PUBLISHER2}", bible2.BibleInfoPublisher).
                Replace("{DATE2}", bible2.BibleInfoDate)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Output path for bible
    ''' </summary>
    ''' <param name="args"></param>
    ''' <param name="options"></param>
    ''' <param name="level"></param>
    ''' <returns></returns>
    Public Shared Function BibleOutputFilePath(args As Configuration.BibleToProcess, options As CommandlineOptions, level As Integer, ensureDirectoryExists As Boolean) As String
        Dim OutputFilePathPattern As String = args.NewPath  '"{DIRECTORY}{FILENAME} - Level {LEVEL}{.EXTENSION}"
        OutputFilePathPattern = OutputFilePathPattern.Replace("{DIRECTORY}", System.IO.Path.GetDirectoryName(args.Path))
        OutputFilePathPattern = OutputFilePathPattern.Replace("{FILENAME}", System.IO.Path.GetFileNameWithoutExtension(args.Path))
        OutputFilePathPattern = OutputFilePathPattern.Replace("{.EXTENSION}", System.IO.Path.GetExtension(args.Path))
        OutputFilePathPattern = OutputFilePathPattern.Replace("{LEVEL}", level)
        Dim Result As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, options.JewishBiblesDirectory, OutputFilePathPattern)
        If ensureDirectoryExists Then
            Dim DirName As String = System.IO.Path.GetDirectoryName(Result)
            If System.IO.Directory.Exists(DirName) = False Then
                System.IO.Directory.CreateDirectory(DirName)
            End If
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Lookup the language code from a relative path like '.\ENG\American Standard Version\...'
    ''' </summary>
    ''' <param name="relPath"></param>
    ''' <returns></returns>
    Private Shared Function LookupLanguageCodeFromRelativePath(relPath As String) As String
        Dim SplitUp As String() = relPath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
        If SplitUp(0) = "." Then
            Return SplitUp(1)
        Else
            Return SplitUp(0)
        End If
    End Function

End Class
