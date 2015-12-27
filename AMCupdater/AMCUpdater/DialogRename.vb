Imports System.Windows.Forms

Public Class DialogRename
    Private Sub DialogRename_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        AcceptButton = OK_Button
        CancelButton = Cancel_Button
        TextBoxDirectoryNameNew.Text = TextBoxDirectoryNameCurrent.Text
        TextBoxFileNameNew.Text = TextBoxFileNameCurrent.Text
        If TextBoxDirectoryNameCurrent.Text.Length = 0 Then
            TextBoxDirectoryNameCurrent.Enabled = False
            TextBoxDirectoryNameNew.Enabled = False
        End If
        TextBoxResultingPath.Text = TextBoxStartPathCurrent.Text + "\" + TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
        TextBoxResultingPathShort.Text = TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
        OK_Button.Enabled = False

        If TextBoxAllPathWithMultiFiles.Text.Length > 0 Then ' If Me.TextBoxAllPathWithMultiFiles.Text.Contains(";") Then
            GroupBoxRenameFile.Enabled = False
            LabelWarningMultiPartFiles.Visible = True
        Else
            GroupBoxRenameFile.Enabled = True
            LabelWarningMultiPartFiles.Visible = False
        End If
    End Sub
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles OK_Button.Click
        DialogResult = Windows.Forms.DialogResult.OK
        Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles Cancel_Button.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
        Close()
    End Sub

    Private Sub TextBoxDirectoryNameNew_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TextBoxDirectoryNameNew.TextChanged
        ' check valid directory name
        If FilenameIsOK(TextBoxDirectoryNameNew.Text) = True And TextBoxDirectoryNameNew.Text <> TextBoxDirectoryNameCurrent.Text Then
            OK_Button.Enabled = True
        Else
            OK_Button.Enabled = False
        End If
        TextBoxResultingPath.Text = TextBoxStartPathCurrent.Text + "\" + TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
        TextBoxResultingPathShort.Text = TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
    End Sub

    Private Sub TextBoxFileNameNew_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TextBoxFileNameNew.TextChanged
        ' check valid file name
        If FilenameIsOK(TextBoxFileNameNew.Text) = True And TextBoxFileNameNew.Text <> TextBoxFileNameCurrent.Text Then
            OK_Button.Enabled = True
        Else
            OK_Button.Enabled = False
        End If
        TextBoxResultingPath.Text = TextBoxStartPathCurrent.Text + "\" + TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
        TextBoxResultingPathShort.Text = TextBoxDirectoryNameNew.Text + "\" + TextBoxFileNameNew.Text
    End Sub
    Public Shared Function FilenameIsOK(ByVal fileNameAndPath As String) As Boolean
        If fileNameAndPath Is Nothing Or fileNameAndPath.Length = 0 Then
            Return False
        End If
        Try
            Dim fileName = Path.GetFileName(fileNameAndPath)
            Dim directory = Path.GetDirectoryName(fileNameAndPath)
            If Path.GetInvalidFileNameChars().Any(Function(c) fileName.Contains(c)) Then
                Return False
            End If
            Return Path.GetInvalidPathChars().All(Function(c) Not directory.Contains(c))
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
End Class
