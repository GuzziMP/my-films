Public Class FrmMovieUpdate

    Dim ImageViewer As New frmImageViewer()

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonOK.Click
        DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonCancel.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub PictureBoxOld_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles PictureBoxOld.Click
        Dim imagefile = PictureBoxOld.ImageLocation
        If Not System.IO.File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub

    Private Sub PictureBoxNew_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles PictureBoxNew.Click
        Dim imagefile = PictureBoxNew.ImageLocation
        If Not System.IO.File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub

    Private Sub ButtonSelectAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonSelectAll.Click
        For i As Integer = 0 To DgvUpdateMovie.RowCount - 1
            DgvUpdateMovie(0, i).Value = True
        Next
    End Sub

    Private Sub ButtonSelectNone_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonSelectNone.Click
        For i As Integer = 0 To DgvUpdateMovie.RowCount - 1
            DgvUpdateMovie(0, i).Value = False
        Next
    End Sub
    Private Sub ButtonSelectOnlyMissingData_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonSelectOnlyMissingData.Click
        For i As Integer = 0 To DgvUpdateMovie.RowCount - 1
            Dim itemName As String = DgvUpdateMovie(1, i).Value
            Dim itemValueOld As String = DgvUpdateMovie(2, i).Value
            Dim itemValueNew As String = DgvUpdateMovie(3, i).Value
            If String.IsNullOrEmpty(itemValueOld) And Not String.IsNullOrEmpty(itemValueNew) Then
                DgvUpdateMovie(0, i).Value = True
            Else
                DgvUpdateMovie(0, i).Value = False
            End If
        Next
    End Sub

    Private Sub ButtonSelectOnlyNonEmptyData_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles ButtonSelectOnlyNonEmptyData.Click
        For i As Integer = 0 To DgvUpdateMovie.RowCount - 1
            Dim itemName As String = DgvUpdateMovie(1, i).Value
            Dim itemValueOld As String = DgvUpdateMovie(2, i).Value
            Dim itemValueNew As String = DgvUpdateMovie(3, i).Value
            If Not String.IsNullOrEmpty(itemValueNew) Then
                DgvUpdateMovie(0, i).Value = True
            Else
                DgvUpdateMovie(0, i).Value = False
            End If
        Next
    End Sub
End Class