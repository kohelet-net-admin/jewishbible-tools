Public Class CsvEditor

    Public PathOfLoadedData As String = Nothing

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If Me.CsvOpenFileDialog.InitialDirectory = Nothing Then Me.CsvOpenFileDialog.InitialDirectory = System.Environment.CurrentDirectory
        Dim Result As DialogResult = Me.CsvOpenFileDialog.ShowDialog
        If Result = DialogResult.OK Then
            Dim CsvPath As String = Nothing
            CsvPath = Me.CsvOpenFileDialog.FileName
            LoadCsvFile(CsvPath)
        End If
    End Sub

    ''' <summary>
    ''' Load a CSV file into the data grid
    ''' </summary>
    ''' <param name="csvPath"></param>
    Private Sub LoadCsvFile(csvPath As String)
        If System.IO.File.Exists(csvPath) = False Then
            MsgBox("File not found: " & csvPath, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
            Return
        End If
        Try
            Me.CsvDataGridView.DataSource = CompuMaster.Data.Csv.ReadDataTableFromCsvFile(csvPath, True, CompuMaster.Data.Csv.ReadLineEncodings.RowBreakCrLfOrLf_CellLineBreakCr, CompuMaster.Data.Csv.ReadLineEncodingAutoConversion.NoAutoConversion, "UTF-8", ","c, """"c, False, False)
            PathOfLoadedData = csvPath
            Me.SaveToolStripMenuItem.Enabled = True
            Me.SaveasToolStripMenuItem.Enabled = True
            Me.CsvDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            Me.CsvDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells)
        Catch ex As Exception
            Me.CsvDataGridView.DataSource = Nothing
            PathOfLoadedData = Nothing
            MsgBox(ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
        End Try
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If PathOfLoadedData = Nothing Then
            SaveasToolStripMenuItem_Click(sender, e)
        Else
            Try
                'Save current sheet
                Dim dt As System.Data.DataTable = CType(Me.CsvDataGridView.DataSource, System.Data.DataTable)
                CompuMaster.Data.Csv.WriteDataTableToCsvFile(PathOfLoadedData, dt, True, CompuMaster.Data.Csv.WriteLineEncodings.RowBreakCrLf_CellLineBreakSpaceChar, "UTF-8", ","c, """"c, "."c)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
            End Try
        End If
    End Sub

    Private Sub SaveasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveasToolStripMenuItem.Click
        SaveCurrentCsvFileAs()
    End Sub

    ''' <summary>
    ''' Save the file at a new location
    ''' </summary>
    ''' <returns>True if saved successfully, False if user aborted or error</returns>
    Private Function SaveCurrentCsvFileAs() As Boolean
        Me.CsvOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Me.PathOfLoadedData)
        Dim Result As DialogResult = Me.CsvSaveFileDialog.ShowDialog
        If Result = DialogResult.OK Then
            Dim NewCsvPath As String
            NewCsvPath = Me.CsvSaveFileDialog.FileName
            Try
                'Save current sheet at another path
                Dim dt As System.Data.DataTable = CType(Me.CsvDataGridView.DataSource, System.Data.DataTable)
                CompuMaster.Data.Csv.WriteDataTableToCsvFile(NewCsvPath, dt, True, CompuMaster.Data.Csv.WriteLineEncodings.RowBreakCrLf_CellLineBreakSpaceChar, "UTF-8", ","c, """"c, "."c)
                PathOfLoadedData = NewCsvPath
                Return True
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If Me.CsvDataGridView.DataSource IsNot Nothing AndAlso CType(Me.CsvDataGridView.DataSource, System.Data.DataTable).GetChanges.Rows.Count > 0 Then
            'Changes found - save file?
            Select Case MsgBox("Quit with saving changes?", MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question)
                Case MsgBoxResult.Yes
                    If SaveCurrentCsvFileAs() Then System.Environment.Exit(0)
                Case MsgBoxResult.No
                    System.Environment.Exit(0)
            End Select
        Else
            'No changes found - simply quit
            System.Environment.Exit(0)
        End If
    End Sub

    Private Sub CsvEditor_Load(sender As Object, e As EventArgs) Handles Me.Load
        If System.Environment.GetCommandLineArgs().Length > 1 Then
            Dim FilePath As String = System.Environment.GetCommandLineArgs()(1)
            LoadCsvFile(System.IO.Path.Combine(System.Environment.CurrentDirectory, FilePath))
        End If
    End Sub
End Class
