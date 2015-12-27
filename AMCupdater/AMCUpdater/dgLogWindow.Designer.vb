<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DgLogWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtLogWindow = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'txtLogWindow
        '
        Me.txtLogWindow.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLogWindow.BackColor = System.Drawing.SystemColors.Window
        Me.txtLogWindow.Location = New System.Drawing.Point(12, 12)
        Me.txtLogWindow.Multiline = True
        Me.txtLogWindow.Name = "txtLogWindow"
        Me.txtLogWindow.ReadOnly = True
        Me.txtLogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLogWindow.Size = New System.Drawing.Size(492, 138)
        Me.txtLogWindow.TabIndex = 1
        Me.txtLogWindow.WordWrap = False
        '
        'DgLogWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(516, 162)
        Me.Controls.Add(Me.txtLogWindow)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dgLogWindow"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "AMCUpdater Log Window"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtLogWindow As System.Windows.Forms.TextBox

End Class
