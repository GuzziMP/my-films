Imports System.Windows.Forms
Imports Grabber
Imports MediaPortal.Configuration
Imports Grabber.Util

Public Class FrmList

    Dim DialogRename As New DialogRename()
    Private SearchTextChanged As Boolean = False

    Private Sub lstOptionsExt_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lstOptionsExt.DoubleClick
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnSearchAgain_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearchAgain.Click
        Dim distance As String
        btnSearchAgain.Enabled = False
        lstOptionsExt.Rows.Clear()
        'lstOptionsExt.Rows.Add(New String() {Nothing, "... now searching for results ...", "", "", "", "", "", ""})
        'Thread.Sleep(5)
        If txtSearchString.Text <> "" Then
            Dim gb As Grabber.GrabberUrlClass = New GrabberUrlClass
            wurl.Clear()
            wurl = gb.ReturnURL(txtSearchString.Text, txtTmpParserFilePath.Text, -1, True, String.Empty) 'wurl = Gb.ReturnURL(txtSearchString.Text, txtTmpParserFilePath.Text, -1, CurrentSettings.Internet_Lookup_Always_Prompt)
            lstOptionsExt.Rows.Clear()
            If (wurl.Count > 0) Then
                For i As Integer = 0 To wurl.Count - 1
                    If FuzziDistance(txtSearchString.Text, wurl.Item(i).Title.ToString) = Integer.MaxValue Then
                        distance = ""
                    Else
                        distance = FuzziDistance(txtSearchString.Text, wurl.Item(i).Title.ToString).ToString
                    End If
                    Dim image As System.Drawing.Image = GrabUtil.GetImageFromUrl(wurl.Item(i).Thumb)
                    lstOptionsExt.Rows.Add(New Object() {image, wurl.Item(i).Title, wurl.Item(i).Year, wurl.Item(i).Options, wurl.Item(i).Akas, wurl.Item(i).ID, wurl.Item(i).URL, distance})
                Next
                lstOptionsExt.SelectionMode = Windows.Forms.DataGridViewSelectionMode.FullRowSelect
                lstOptionsExt.Rows(0).Selected = True
                btnOK.Enabled = True
                btnSearchAllPages.Enabled = True
            Else
                lstOptionsExt.Rows.Add(New Object() {Nothing, "No results found !", "", "", "", "", "", ""})
                btnOK.Enabled = False
                btnSearchAllPages.Enabled = False
            End If
            SearchTextChanged = False
        Else
            MsgBox("You must enter some text in the search box!", MsgBoxStyle.OkOnly)
        End If
    End Sub

    Function FuzziDistance(ByVal searchtitle As String, ByVal movietitle As String) As Integer
        Return AdvancedStringComparer.Levenshtein(searchtitle, movietitle)
    End Function

    Private Sub btnSearchAgainWithIMDB_Id_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchAgainWithIMDB_Id.Click
        If txtSearchhintIMDB_Id.Text <> "" And txtSearchhintIMDB_Id.Text.StartsWith("tt") Then
            Dim gb As Grabber.GrabberUrlClass = New Grabber.GrabberUrlClass
            'Dim wurl As ArrayList
            lstOptionsExt.Rows.Clear()
            lstOptionsExt.Rows.Add(New Object() {Nothing, "... now searching for results ...", "", "", "", "", "", ""})
            wurl.Clear()
            wurl = gb.ReturnURL(txtSearchhintIMDB_Id.Text, txtTmpParserFilePath.Text, 1, True, String.Empty) 'wurl = Gb.ReturnURL(txtSearchhintIMDB_Id.Text, txtTmpParserFilePath.Text, 1, CurrentSettings.Internet_Lookup_Always_Prompt)
            lstOptionsExt.Rows.Clear()
            If (wurl.Count > 0) Then
                For i As Integer = 0 To wurl.Count - 1
                    Dim image As System.Drawing.Image = GrabUtil.GetImageFromUrl(wurl.Item(i).Thumb)
                    ' Image smallImage = image.GetThumbnailImage(20, 30, null, IntPtr.Zero);
                    lstOptionsExt.Rows.Add(New Object() {image, wurl.Item(i).Title, wurl.Item(i).Year, wurl.Item(i).Options, wurl.Item(i).Akas, wurl.Item(i).ID, wurl.Item(i).URL, ""})
                Next
                lstOptionsExt.SelectionMode = Windows.Forms.DataGridViewSelectionMode.FullRowSelect
                lstOptionsExt.Rows(0).Selected = True
                btnOK.Enabled = True
            Else
                lstOptionsExt.Rows.Add(New Object() {Nothing, "No results found !", "", "", "", "", "", ""})
                btnOK.Enabled = False
            End If
            SearchTextChanged = False
        Else
            MsgBox("There must be a valid IMDB-Id (ttxxxxxxx) present!", MsgBoxStyle.OkOnly)
        End If
    End Sub
    Private Sub btnDontAskAgain_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnDontAskAgain.Click
        'Add to xml file to ignore in future scans:
        chkDontAskAgain.Checked() = True
        DialogResult = Windows.Forms.DialogResult.Ignore
    End Sub

    Private Sub btnSelectParserFile_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSelectParserFile.Click
        Dim OpenParserFileDialog = New Windows.Forms.OpenFileDialog
        Try
            With OpenParserFileDialog
                If Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\scripts\MyFilms") Then
                    .InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\scripts\MyFilms"
                Else
                    .InitialDirectory = Environment.SpecialFolder.Desktop
                End If
                .FileName = ""
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtTmpParserFilePath.Text = .FileName
                        txtTmpParserFilePathShort.Text = .FileName.ToString.Substring(.FileName.ToString.LastIndexOf("\") + 1)
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Text)
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCancel.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub txtSearchString_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearchString.KeyPress
        If e.KeyChar = Chr(13) Then
            If SearchTextChanged = True Then
                'User has changed the text - default to Search Again:
                e.Handled = True
                btnSearchAgain_Click(Me, EventArgs.Empty)
            Else
                'Current results match search text - return OK if an item is selected:
                e.Handled = True
                If lstOptionsExt.Rows.Count > 0 Then
                    If lstOptionsExt("Title", 0).Value.ToString() <> "Movie not found..." Then
                        Me.DialogResult = Windows.Forms.DialogResult.OK
                    End If
                End If
            End If
        Else
            SearchTextChanged = True
        End If
    End Sub

    Private Sub frmList_KeyDown(ByVal sender As Object, ByVal e As Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        'Console.WriteLine(e.KeyCode.ToString)
        If e.KeyCode = Windows.Forms.Keys.Up Or e.KeyCode = Windows.Forms.Keys.Down Then
            Dim rowSelected As Int32 = lstOptionsExt.Rows.GetFirstRow(DataGridViewElementStates.Selected)
            If e.KeyCode = Windows.Forms.Keys.Up Then
                If rowSelected > 0 Then
                    lstOptionsExt.Rows(rowSelected).Selected = False
                    lstOptionsExt.Rows(rowSelected - 1).Selected = True
                End If
            ElseIf e.KeyCode = Windows.Forms.Keys.Down Then
                If rowSelected < lstOptionsExt.Rows.Count - 1 Then
                    lstOptionsExt.Rows(rowSelected).Selected = False
                    lstOptionsExt.Rows(rowSelected + 1).Selected = True
                End If
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub frmList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtSearchString.Focus()
        Me.btnSearchAgain.Enabled = False
        Me.btnSearchAllPages.Enabled = True
    End Sub

    Private Sub ButtonGrabberOptions_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonGrabberOptions.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtTmpParserFilePath.Text & """"
            psi.ErrorDialog = True
            If (OSInfo.OSInfo.VistaOrLater()) Then
                psi.Verb = "runas"
            End If
            p.StartInfo = psi
            Try
                p.Start()
            Catch
            End Try
        End Using

    End Sub

    Private Sub btnCancelFromDialog_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCancelFromDialog.Click
        'Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.DialogResult = Windows.Forms.DialogResult.Abort
        'Dim AntProc As New AntProcessor
        'AntProc.bgwFolderScanUpdate_Cancel()
        'bgwFolderScanUpdate.CancelAsync()
        'Form1.btnCancelProcessing.Enabled = False
    End Sub

    Private Sub txtSearchhintIMDB_Id_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles txtSearchhintIMDB_Id.TextChanged
        If txtSearchhintIMDB_Id.Text <> "" Then
            btnSearchAgainWithIMDB_Id.Enabled = True
        Else
            btnSearchAgainWithIMDB_Id.Enabled = False
        End If
    End Sub

    Private Sub txtSearchString_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles txtSearchString.TextChanged
        btnSearchAgain.Enabled = True
        btnSearchAllPages.Enabled = True
    End Sub

    Private Sub txtTmpParserFilePathShort_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles txtTmpParserFilePathShort.TextChanged
        btnSearchAgain.Enabled = True
        btnSearchAllPages.Enabled = True
    End Sub

    Private Sub lstOptionsExt_CellContentClick(ByVal sender As System.Object, ByVal e As Windows.Forms.DataGridViewCellEventArgs) Handles lstOptionsExt.CellContentClick
        Try
            If (e.ColumnIndex = Me.lstOptionsExt.Columns("Weblink").Index) Then
                Dim Filepath As String = lstOptionsExt("Weblink", e.RowIndex).Value.ToString()
                'WebBrowserPreview.Url = New System.Uri(Filepath)
                'WebBrowserPreview.Refresh()

                If Not Filepath.ToLower().Contains(".nfo") Then
                    System.Diagnostics.Process.Start(Filepath)
                Else
                    Using p = New Process
                        Dim psi As New ProcessStartInfo
                        psi.FileName = "notepad.exe"
                        psi.UseShellExecute = True
                        psi.WindowStyle = ProcessWindowStyle.Normal
                        psi.Arguments = """" + Filepath + """"
                        psi.ErrorDialog = True
                        If (OSInfo.OSInfo.VistaOrLater()) Then
                            psi.Verb = "runas"
                        End If
                        p.StartInfo = psi
                        Try
                            p.Start()
                        Catch
                        End Try
                    End Using
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub lstOptionsExt_CellMouseEnter(ByVal sender As System.Object, ByVal e As Windows.Forms.DataGridViewCellEventArgs) Handles lstOptionsExt.CellMouseEnter
        Try
            'e.RowIndex, e.ColumnIndex
            If lstOptionsExt("Thumb", e.RowIndex).Value IsNot Nothing Then
                Label3.Visible = False
                Label4.Visible = False
                LabelSourcePath.Visible = False
                pbCoverPreview.Visible = True
                pbCoverPreview.Image = lstOptionsExt("Thumb", e.RowIndex).Value
            End If
            If e.ColumnIndex = lstOptionsExt.Columns("AKA").Index And Me.lstOptionsExt("AKA", e.RowIndex).Value IsNot Nothing Then
                Dim TooltipMovieInfo As String = ""
                Dim AkaTitles As String() = lstOptionsExt("AKA", e.RowIndex).Value.Split(New Char() {"|"}, StringSplitOptions.RemoveEmptyEntries)
                For Each AkaTitle As String In AkaTitles
                    If TooltipMovieInfo.Length > 0 Then
                        TooltipMovieInfo = TooltipMovieInfo + vbCrLf
                    End If
                    TooltipMovieInfo = TooltipMovieInfo + AkaTitle.Trim
                Next
                lstOptionsExt("AKA", e.RowIndex).ToolTipText = TooltipMovieInfo
                'ToolTipImportDialog.ToolTipTitle = "Movie Details ..."
                'ToolTipImportDialog.Show(TooltipMovieInfo, lstOptionsExt.Columns("AKA"))
            Else
                lstOptionsExt("AKA", e.RowIndex).ToolTipText = ""
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub lstOptionsExt_CellMouseLeave(ByVal sender As System.Object, ByVal e As Windows.Forms.DataGridViewCellEventArgs) Handles lstOptionsExt.CellMouseLeave
        Label3.Visible = True
        Label4.Visible = True
        LabelSourcePath.Visible = True
        pbCoverPreview.Visible = False
        pbCoverPreview.Image = Nothing
        'Me.ToolTipImportDialog.Hide(Me.lstOptionsExt.Columns("AKA"))
    End Sub

    Private Sub btnSearchGoogle_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearchGoogle.Click
        Dim t As String = txtSearchString.Text & " (" & txtSearchintYear.Text & ")"
        Try
            'WebBrowserPreview.Url = New System.Uri(txtSearchintYear.Text)
            'WebBrowserPreview.Refresh()
            Process.Start("http://google.com/search?q=" + t)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnRenameAndCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnRenameAndCancel.Click
        ' launches Rename Dialog and "ignores" file for later import after rescanning
        Dim currentPathAllFiles As String = ""
        Dim currentPathFull As String = ""
        Dim currentStartPath As String = ""
        Dim currentDirectoryName As String = ""
        Dim currentFileName As String = ""

        currentPathAllFiles = txtSourceFullAllPath.Text
        If currentPathAllFiles.Contains(";") Then
            currentPathFull = currentPathAllFiles.Substring(0, currentPathAllFiles.IndexOf(";")).Trim
        Else
            currentPathFull = currentPathAllFiles.Trim
        End If
        If currentPathFull.Length = 0 Then
            currentPathFull = txtSourceFull.Text
        End If
        If currentPathFull.Contains("\") = True Then
            currentFileName = currentPathFull.Substring(currentPathFull.LastIndexOf("\") + 1)
            currentDirectoryName = currentPathFull.Substring(0, currentPathFull.LastIndexOf("\"))
            currentStartPath = currentDirectoryName
            If currentDirectoryName.Contains("\") = True Then
                currentStartPath = currentDirectoryName.Substring(0, currentDirectoryName.LastIndexOf("\"))
                currentDirectoryName = currentDirectoryName.Substring(currentDirectoryName.LastIndexOf("\") + 1)
            End If
        End If
        If (currentPathFull.Length <> (currentStartPath + "\" + currentDirectoryName + "\" + currentFileName).Length) Then
            MsgBox("ErrorEvent getting directory and file information !" + Chr(13) + "'" + currentPathFull + "'" + Chr(13) + "'" + currentStartPath + "\" + currentDirectoryName + "\" + currentFileName + "'", MsgBoxStyle.Exclamation, Me.Text)
            Return
        End If
        If Not System.IO.File.Exists(currentPathFull) Then
            MsgBox("Rename not possible - File does not exist !", MsgBoxStyle.Exclamation, Me.Text)
            Return
        End If
        Try
            With DialogRename
                .TextBoxAllPathWithMultiFiles.Text = currentPathAllFiles
                .TextBoxStartPathCurrent.Text = currentPathFull
                .TextBoxStartPathCurrent.Text = currentStartPath
                .TextBoxDirectoryNameCurrent.Text = currentDirectoryName
                .TextBoxFileNameCurrent.Text = currentFileName
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        If .TextBoxFileNameCurrent.Text <> .TextBoxFileNameNew.Text And .TextBoxFileNameNew.Text.Length > 0 Then
                            My.Computer.FileSystem.RenameFile(currentPathFull, .TextBoxFileNameNew.Text)
                        End If

                        If .TextBoxDirectoryNameCurrent.Text <> .TextBoxDirectoryNameNew.Text And .TextBoxDirectoryNameNew.Text.Length > 0 Then
                            My.Computer.FileSystem.RenameDirectory(currentStartPath + "\" + currentDirectoryName, .TextBoxDirectoryNameNew.Text)
                        End If
                        MsgBox("Rename Successful!" + Chr(13) + Chr(13) + "Old name = '...\" + .TextBoxDirectoryNameCurrent.Text + "\" + .TextBoxFileNameCurrent.Text + "'" + Chr(13) + "New name = '...\" + .TextBoxDirectoryNameNew.Text + "\" + .TextBoxFileNameNew.Text + "'", MsgBoxStyle.Information)
                        Me.DialogResult = Windows.Forms.DialogResult.Cancel ' to cancel import in List Selection !
                        Return
                    Catch renameException As Exception
                        LogEvent("RENAME : successful! Old name = '...\" + .TextBoxDirectoryNameCurrent.Text + "\" + .TextBoxFileNameCurrent.Text + "', New name = '" + .TextBoxDirectoryNameNew.Text + "\" + .TextBoxFileNameNew.Text + "'", EventLogLevel.ErrorEvent)
                        MsgBox("Rename not successful! - ErrorEvent: " + renameException.Message, MsgBoxStyle.Exclamation)
                    End Try
                End If
            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub LabelSourcePath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelSourcePath.Click
        Try
            Dim info As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(txtSourceFull.Text)
            Call Shell("explorer /select," & info.FullName, AppWinStyle.NormalFocus)
            'Process.Start("explorer.exe", info.DirectoryName)
        Catch ex As Exception
            MsgBox("Path '" & txtSourceFull.Text & "' not found! - ErrorEvent: " + ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub btnSearchNextPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchAllPages.Click
        Dim distance As String
        Dim resultfound As Boolean = False
        Dim SearchPage As Integer = -1 ' results in starting with page = 0
        Dim pageurl As New ArrayList

        btnSearchAgain.Enabled = False
        btnSearchAllPages.Enabled = False
        If txtSearchString.Text <> "" Then
            lstOptionsExt.Rows.Clear()
            wurl.Clear()
            Dim gb As Grabber.GrabberUrlClass = New Grabber.GrabberUrlClass
            While SearchPage < 25
                SearchPage += 1
                pageurl.Clear()
                pageurl = gb.ReturnURL(txtSearchString.Text, txtTmpParserFilePath.Text, SearchPage, True, String.Empty) 'pageurl = Gb.ReturnURL(txtSearchString.Text, txtTmpParserFilePath.Text, SearchPage, CurrentSettings.Internet_Lookup_Always_Prompt)
                If (pageurl.Count > 0) Then
                    wurl.AddRange(pageurl)
                ElseIf (SearchPage > 1) Then ' If no results AND page is 2 or bigger, stop loop
                    Exit While
                End If
            End While
            If (wurl.Count > 0) Then
                For i As Integer = 0 To wurl.Count - 1
                    If FuzziDistance(txtSearchString.Text, wurl.Item(i).Title.ToString) = Integer.MaxValue Then
                        distance = ""
                    Else
                        distance = FuzziDistance(txtSearchString.Text, wurl.Item(i).Title.ToString).ToString
                    End If
                    Dim image As System.Drawing.Image = GrabUtil.GetImageFromUrl(wurl.Item(i).Thumb)
                    lstOptionsExt.Rows.Add(New Object() {image, wurl.Item(i).Title, wurl.Item(i).Year, wurl.Item(i).Options, wurl.Item(i).Akas, wurl.Item(i).ID, wurl.Item(i).URL, distance})
                Next
                resultfound = True
            End If
            If resultfound = True Then
                lstOptionsExt.SelectionMode = Windows.Forms.DataGridViewSelectionMode.FullRowSelect
                lstOptionsExt.Rows(0).Selected = True
                btnOK.Enabled = True
            Else
                lstOptionsExt.Rows.Add(New Object() {Nothing, "No results found !", "", "", "", "", "", ""})
                btnOK.Enabled = False
            End If
            SearchTextChanged = False
        Else
            MsgBox("You must enter some text in the search box!", MsgBoxStyle.OkOnly)
        End If
    End Sub
End Class