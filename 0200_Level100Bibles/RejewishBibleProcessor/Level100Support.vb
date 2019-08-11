Imports KoheletNetwork.RejewishBibleProcessor

Public Class Level100Support

    Public Class BookTranslation
        Public Property BookID As Integer
        Public Property BookName As String
        Public Property BookShortName As String
    End Class

    Public Shared Function GetLevel100BookOrderWithTranslations(languageCode As String, options As CommandlineOptions) As List(Of BookTranslation)
        If languageCode = Nothing Then Throw New ArgumentNullException("languageCode")
        Dim DesiredBookOrderWithTranslations As List(Of BookTranslation) = GetLevel100BookOrderWithFallbackTranslations(options)
        Dim Translations As List(Of BookTranslation) = GetLevel100BookTranslations(languageCode, options)
        For MyMasterCounter As Integer = 0 To DesiredBookOrderWithTranslations.Count - 1
            For MyTranslationCounter As Integer = 0 To Translations.Count - 1
                If DesiredBookOrderWithTranslations(MyMasterCounter).BookID = Translations(MyTranslationCounter).BookID Then
                    DesiredBookOrderWithTranslations(MyMasterCounter).BookName = Translations(MyTranslationCounter).BookName
                    DesiredBookOrderWithTranslations(MyMasterCounter).BookShortName = Translations(MyTranslationCounter).BookShortName
                End If
            Next
        Next
        Return DesiredBookOrderWithTranslations
    End Function

    Private Shared Function GetLevel100BookOrderWithFallbackTranslations(options As CommandlineOptions) As List(Of BookTranslation)
        Dim Result As New List(Of BookTranslation)
        Dim Config As System.Data.DataTable = Configuration.LoadConfig("JewishBookOrder", options, True)
        For MyCounter As Integer = 0 To Config.Rows.Count - 1
            Dim NewItem As New BookTranslation
            NewItem.BookID = CType(Config(MyCounter)("BookNumber"), Integer)
            NewItem.BookName = CType(Config(MyCounter)("BookName"), String)
            NewItem.BookShortName = CType(Config(MyCounter)("BookShortName"), String)
            Result.Add(NewItem)
        Next
        Return Result
    End Function

    Private Shared Function GetLevel100BookTranslations(languageCode As String, options As CommandlineOptions) As List(Of BookTranslation)
        Dim Result As New List(Of BookTranslation)
        Dim Config As System.Data.DataTable = Configuration.LoadConfig("JewishBookOrder_" & languageCode, options, False)
        If Config IsNot Nothing Then
            For MyCounter As Integer = 0 To Config.Rows.Count - 1
                Dim NewItem As New BookTranslation
                NewItem.BookID = CType(Config(MyCounter)("BookNumber"), Integer)
                NewItem.BookName = CType(Config(MyCounter)("BookName"), String)
                NewItem.BookShortName = CType(Config(MyCounter)("BookShortName"), String)
                Result.Add(NewItem)
            Next
        End If
        Return Result
    End Function

End Class
