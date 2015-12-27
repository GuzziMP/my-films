Public Class FrmImageViewer
    Sub main()

    End Sub

    Private Sub PictureBoxImageViewer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBoxImageViewer.Click
        Me.Close()
    End Sub

    Private Sub frmImageViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Me.AddOwnedForm(FrmImageViewer)
        If My.Settings.ImageFormLocation.X > 0 And My.Settings.ImageFormLocation.Y > 0 And (My.Settings.ImageFormLocation.X + My.Settings.ImageFormSize.Width) < System.Windows.Forms.SystemInformation.VirtualScreen.Width And (My.Settings.ImageFormLocation.Y + My.Settings.ImageFormSize.Height) < Windows.Forms.SystemInformation.VirtualScreen.Height Then
            Me.Location = My.Settings.ImageFormLocation
        End If
        If My.Settings.ImageFormSize.Height > 0 And My.Settings.ImageFormSize.Width > 0 Then
            Me.Size = My.Settings.ImageFormSize
        End If
    End Sub

    Private Sub frmImageViewer_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Leave
        Try
            'Set my user settings to save size and position
            My.Settings.ImageFormSize = Me.Size
            My.Settings.ImageFormLocation = Me.Location
            My.Settings.Save() 'Save the user settings so next time the window will be the same size and location
        Catch ex As Exception
            MsgBox("There was a problem saving your settings.", MsgBoxStyle.Critical, "Save ErrorEvent...")
        End Try
    End Sub

    Private Sub frmImageViewer_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            'Set my user settings to save size and position
            My.Settings.ImageFormSize = Me.Size
            My.Settings.ImageFormLocation = Me.Location
            My.Settings.Save() 'Save the user settings so next time the window will be the same size and location
        Catch ex As Exception
            MsgBox("There was a problem saving your settings.", MsgBoxStyle.Critical, "Save ErrorEvent...")
        End Try
    End Sub

    Private Sub frmImageViewer_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Enter
        'Me.AddOwnedForm(FrmImageViewer)
        If My.Settings.ImageFormLocation.X > 0 And My.Settings.ImageFormLocation.Y > 0 And (My.Settings.ImageFormLocation.X + My.Settings.ImageFormSize.Width) < System.Windows.Forms.SystemInformation.VirtualScreen.Width And (My.Settings.ImageFormLocation.Y + My.Settings.ImageFormSize.Height) < Windows.Forms.SystemInformation.VirtualScreen.Height Then
            Me.Location = My.Settings.ImageFormLocation
        End If
        If My.Settings.ImageFormSize.Height > 0 And My.Settings.ImageFormSize.Width > 0 Then
            Me.Size = My.Settings.ImageFormSize
        End If
    End Sub
End Class