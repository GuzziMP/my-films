<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMovieUpdate
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMovieUpdate))
        Me.DgvUpdateMovie = New System.Windows.Forms.DataGridView()
        Me.AntMovieCatalog = New AMCUpdater.AntMovieCatalog()
        Me.MovieBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.PictureBoxOld = New System.Windows.Forms.PictureBox()
        Me.PictureBoxNew = New System.Windows.Forms.PictureBox()
        Me.ButtonSelectOnlyMissingData = New System.Windows.Forms.Button()
        Me.ButtonSelectOnlyNonEmptyData = New System.Windows.Forms.Button()
        Me.ButtonSelectAll = New System.Windows.Forms.Button()
        Me.ButtonSelectNone = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cbUpdate = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.tbItem = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tbCurrent = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tbNew = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DgvUpdateMovie, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MovieBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxOld, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxNew, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DgvUpdateMovie
        '
        Me.DgvUpdateMovie.AllowUserToAddRows = False
        Me.DgvUpdateMovie.AllowUserToDeleteRows = False
        Me.DgvUpdateMovie.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvUpdateMovie.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DgvUpdateMovie.BackgroundColor = System.Drawing.SystemColors.Control
        Me.DgvUpdateMovie.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DgvUpdateMovie.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvUpdateMovie.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cbUpdate, Me.tbItem, Me.tbCurrent, Me.tbNew})
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DgvUpdateMovie.DefaultCellStyle = DataGridViewCellStyle5
        Me.DgvUpdateMovie.Location = New System.Drawing.Point(3, 4)
        Me.DgvUpdateMovie.Name = "DgvUpdateMovie"
        Me.DgvUpdateMovie.RowHeadersVisible = False
        Me.DgvUpdateMovie.Size = New System.Drawing.Size(904, 433)
        Me.DgvUpdateMovie.TabIndex = 0
        '
        'AntMovieCatalog
        '
        Me.AntMovieCatalog.DataSetName = "AntMovieCatalog"
        Me.AntMovieCatalog.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'MovieBindingSource
        '
        Me.MovieBindingSource.DataMember = "Movie"
        Me.MovieBindingSource.DataSource = Me.AntMovieCatalog
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(782, 549)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(86, 33)
        Me.ButtonOK.TabIndex = 3
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(782, 473)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(86, 23)
        Me.ButtonCancel.TabIndex = 2
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'PictureBoxOld
        '
        Me.PictureBoxOld.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxOld.DataBindings.Add(New System.Windows.Forms.Binding("Image", Me.MovieBindingSource, "Picture", True))
        Me.PictureBoxOld.Location = New System.Drawing.Point(12, 450)
        Me.PictureBoxOld.Name = "PictureBoxOld"
        Me.PictureBoxOld.Size = New System.Drawing.Size(97, 141)
        Me.PictureBoxOld.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBoxOld.TabIndex = 3
        Me.PictureBoxOld.TabStop = False
        '
        'PictureBoxNew
        '
        Me.PictureBoxNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxNew.Location = New System.Drawing.Point(148, 450)
        Me.PictureBoxNew.Name = "PictureBoxNew"
        Me.PictureBoxNew.Size = New System.Drawing.Size(97, 141)
        Me.PictureBoxNew.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBoxNew.TabIndex = 4
        Me.PictureBoxNew.TabStop = False
        '
        'ButtonSelectOnlyMissingData
        '
        Me.ButtonSelectOnlyMissingData.Location = New System.Drawing.Point(20, 65)
        Me.ButtonSelectOnlyMissingData.Name = "ButtonSelectOnlyMissingData"
        Me.ButtonSelectOnlyMissingData.Size = New System.Drawing.Size(159, 23)
        Me.ButtonSelectOnlyMissingData.TabIndex = 2
        Me.ButtonSelectOnlyMissingData.Text = "Select only missing data"
        Me.ButtonSelectOnlyMissingData.UseVisualStyleBackColor = True
        '
        'ButtonSelectOnlyNonEmptyData
        '
        Me.ButtonSelectOnlyNonEmptyData.Location = New System.Drawing.Point(20, 104)
        Me.ButtonSelectOnlyNonEmptyData.Name = "ButtonSelectOnlyNonEmptyData"
        Me.ButtonSelectOnlyNonEmptyData.Size = New System.Drawing.Size(159, 23)
        Me.ButtonSelectOnlyNonEmptyData.TabIndex = 3
        Me.ButtonSelectOnlyNonEmptyData.Text = "Select only nonempty data"
        Me.ButtonSelectOnlyNonEmptyData.UseVisualStyleBackColor = True
        '
        'ButtonSelectAll
        '
        Me.ButtonSelectAll.Location = New System.Drawing.Point(20, 23)
        Me.ButtonSelectAll.Name = "ButtonSelectAll"
        Me.ButtonSelectAll.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSelectAll.TabIndex = 0
        Me.ButtonSelectAll.Text = "Select all"
        Me.ButtonSelectAll.UseVisualStyleBackColor = True
        '
        'ButtonSelectNone
        '
        Me.ButtonSelectNone.Location = New System.Drawing.Point(104, 23)
        Me.ButtonSelectNone.Name = "ButtonSelectNone"
        Me.ButtonSelectNone.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSelectNone.TabIndex = 1
        Me.ButtonSelectNone.Text = "Select none"
        Me.ButtonSelectNone.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.ButtonSelectAll)
        Me.GroupBox1.Controls.Add(Me.ButtonSelectOnlyMissingData)
        Me.GroupBox1.Controls.Add(Me.ButtonSelectOnlyNonEmptyData)
        Me.GroupBox1.Controls.Add(Me.ButtonSelectNone)
        Me.GroupBox1.Location = New System.Drawing.Point(544, 450)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(201, 141)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Field Selection ..."
        '
        'cbUpdate
        '
        Me.cbUpdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle1.NullValue = False
        Me.cbUpdate.DefaultCellStyle = DataGridViewCellStyle1
        Me.cbUpdate.HeaderText = "Update"
        Me.cbUpdate.Name = "cbUpdate"
        Me.cbUpdate.Width = 48
        '
        'tbItem
        '
        Me.tbItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        Me.tbItem.DefaultCellStyle = DataGridViewCellStyle2
        Me.tbItem.HeaderText = "Item"
        Me.tbItem.Name = "tbItem"
        Me.tbItem.Width = 52
        '
        'tbCurrent
        '
        Me.tbCurrent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        Me.tbCurrent.DefaultCellStyle = DataGridViewCellStyle3
        Me.tbCurrent.HeaderText = "Current Value"
        Me.tbCurrent.Name = "tbCurrent"
        '
        'tbNew
        '
        Me.tbNew.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        Me.tbNew.DefaultCellStyle = DataGridViewCellStyle4
        Me.tbNew.HeaderText = "New Value"
        Me.tbNew.Name = "tbNew"
        '
        'FrmMovieUpdate
        '
        Me.AcceptButton = Me.ButtonOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(908, 603)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBoxNew)
        Me.Controls.Add(Me.PictureBoxOld)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.DgvUpdateMovie)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMovieUpdate"
        Me.Text = "Movie Update"
        CType(Me.DgvUpdateMovie, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MovieBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxOld, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxNew, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DgvUpdateMovie As System.Windows.Forms.DataGridView
    Friend WithEvents AntMovieCatalog As AMCUpdater.AntMovieCatalog
    Friend WithEvents MovieBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents PictureBoxOld As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBoxNew As System.Windows.Forms.PictureBox
    Friend WithEvents ButtonSelectOnlyMissingData As System.Windows.Forms.Button
    Friend WithEvents ButtonSelectOnlyNonEmptyData As System.Windows.Forms.Button
    Friend WithEvents ButtonSelectAll As System.Windows.Forms.Button
    Friend WithEvents ButtonSelectNone As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cbUpdate As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents tbItem As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tbCurrent As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tbNew As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
