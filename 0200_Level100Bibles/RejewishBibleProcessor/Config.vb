Public Class Config

    Public Sub New(configData As System.Data.DataTable)

        ReadConfigStringValues(configData, "AlwaysIgnorePath", "Path", Me.AlwaysIgnorePath)
        ReadConfigBiblesToProcess(configData, Me.BiblesToProcess)

    End Sub

    Public ReadOnly Property AlwaysIgnorePath As New List(Of String)
    Public ReadOnly Property BiblesToProcess As New List(Of BibleToProcess)


    Private Sub ReadConfigStringValues(configData As System.Data.DataTable, key As String, valueColumnName As String, outputList As List(Of String))
        Dim FilteredConfig As System.Data.DataTable
        FilteredConfig = CompuMaster.Data.DataTables.CreateDataTableClone(configData, "Key='" & key.Replace("'", "''") & "'")
        For MyCounter As Integer = 0 To FilteredConfig.Rows.Count - 1
            outputList.Add(CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)(valueColumnName)))
        Next
    End Sub

    Private Sub ReadConfigBiblesToProcess(configData As System.Data.DataTable, outputList As List(Of BibleToProcess))
        Dim FilteredConfig As System.Data.DataTable
        FilteredConfig = CompuMaster.Data.DataTables.CreateDataTableClone(configData, "Key='BibleToProcess'")
        For MyCounter As Integer = 0 To FilteredConfig.Rows.Count - 1
            Dim NewItem As New BibleToProcess
            NewItem.Path = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("Path"))
            NewItem.PathAdditionalSource = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("PathAdditionalSource"))
            NewItem.NewPath = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("NewPath"))
            NewItem.NewBibleName = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("NewBibleName"))
            NewItem.NewBibleInfoTitle = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("NewBibleInfoTitle"))
            NewItem.NewBibleInfoIdentifier = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("NewBibleInfoIdentifier"))
            NewItem.NewBibleInfoDescription = CompuMaster.Data.Utils.NoDBNull(Of String)(FilteredConfig(MyCounter)("NewBibleInfoDescription"))
            outputList.Add(NewItem)
        Next
    End Sub

    Public Class BibleToProcess
        Public Property Path As String
        Public Property PathAdditionalSource As String
        Public Property NewPath As String
        Public Property NewBibleName As String
        Public Property NewBibleInfoTitle As String
        Public Property NewBibleInfoIdentifier As String
        Public Property NewBibleInfoDescription As String
    End Class

End Class
