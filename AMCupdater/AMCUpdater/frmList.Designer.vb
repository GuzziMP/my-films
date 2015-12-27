<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmList
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmList))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.txtSearchString = New System.Windows.Forms.TextBox
        Me.btnSearchAgain = New System.Windows.Forms.Button
        Me.txtTmpParserFilePath = New System.Windows.Forms.TextBox
        Me.btnSelectParserFile = New System.Windows.Forms.Button
        Me.btnDontAskAgain = New System.Windows.Forms.Button
        Me.chkDontAskAgain = New System.Windows.Forms.CheckBox
        Me.ButtonGrabberOptions = New System.Windows.Forms.Button
        Me.txtSearchintYear = New System.Windows.Forms.TextBox
        Me.txtSearchhintIMDB_Id = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnCancelFromDialog = New System.Windows.Forms.Button
        Me.txtTmpParserFilePathShort = New System.Windows.Forms.TextBox
        Me.btnSearchAgainWithIMDB_Id = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtSource = New System.Windows.Forms.TextBox
        Me.LabelSourcePath = New System.Windows.Forms.Label
        Me.lstOptionsExt = New System.Windows.Forms.DataGridView
        Me.Thumb = New System.Windows.Forms.DataGridViewImageColumn
        Me.Title = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Year = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Options = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.AKA = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Weblink = New System.Windows.Forms.DataGridViewLinkColumn
        Me.Distance = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.btnSearchGoogle = New System.Windows.Forms.Button
        Me.ToolTipImportDialog = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnRenameAndCancel = New System.Windows.Forms.Button
        Me.btnSearchAllPages = New System.Windows.Forms.Button
        Me.txtSourceFull = New System.Windows.Forms.TextBox
        Me.txtSourceFullAllPath = New System.Windows.Forms.TextBox
        Me.pbCoverPreview = New System.Windows.Forms.PictureBox
        CType(Me.lstOptionsExt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbCoverPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(617, 364)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(94, 23)
        Me.btnCancel.TabIndex = 25
        Me.btnCancel.Text = "Ignore"
        Me.ToolTipImportDialog.SetToolTip(Me.btnCancel, """Ignore"" does skip the current movie - thus not importing it." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Movie will be foun" & _
                "d again on a new scan next time.")
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(751, 364)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(93, 23)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "OK"
        Me.ToolTipImportDialog.SetToolTip(Me.btnOK, "Choose the selected data for the current movie.")
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'txtSearchString
        '
        Me.txtSearchString.Location = New System.Drawing.Point(96, 32)
        Me.txtSearchString.Name = "txtSearchString"
        Me.txtSearchString.Size = New System.Drawing.Size(316, 20)
        Me.txtSearchString.TabIndex = 6
        '
        'btnSearchAgain
        '
        Me.btnSearchAgain.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSearchAgain.Location = New System.Drawing.Point(26, 364)
        Me.btnSearchAgain.Name = "btnSearchAgain"
        Me.btnSearchAgain.Size = New System.Drawing.Size(94, 23)
        Me.btnSearchAgain.TabIndex = 19
        Me.btnSearchAgain.Text = "Search Again"
        Me.ToolTipImportDialog.SetToolTip(Me.btnSearchAgain, "Perform new search." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Will only be available, if settings have been changed," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "like" & _
                " grabber script or search title." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Note: By default, only the first page results " & _
                "are loaded.")
        Me.btnSearchAgain.UseVisualStyleBackColor = True
        '
        'txtTmpParserFilePath
        '
        Me.txtTmpParserFilePath.Location = New System.Drawing.Point(96, 59)
        Me.txtTmpParserFilePath.Name = "txtTmpParserFilePath"
        Me.txtTmpParserFilePath.Size = New System.Drawing.Size(272, 20)
        Me.txtTmpParserFilePath.TabIndex = 10
        Me.txtTmpParserFilePath.Visible = False
        '
        'btnSelectParserFile
        '
        Me.btnSelectParserFile.CausesValidation = False
        Me.btnSelectParserFile.Location = New System.Drawing.Point(255, 59)
        Me.btnSelectParserFile.Name = "btnSelectParserFile"
        Me.btnSelectParserFile.Size = New System.Drawing.Size(31, 20)
        Me.btnSelectParserFile.TabIndex = 14
        Me.btnSelectParserFile.Text = "..."
        Me.btnSelectParserFile.UseVisualStyleBackColor = True
        '
        'btnDontAskAgain
        '
        Me.btnDontAskAgain.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnDontAskAgain.Location = New System.Drawing.Point(511, 364)
        Me.btnDontAskAgain.Name = "btnDontAskAgain"
        Me.btnDontAskAgain.Size = New System.Drawing.Size(94, 23)
        Me.btnDontAskAgain.TabIndex = 24
        Me.btnDontAskAgain.Text = "Always Ignore"
        Me.ToolTipImportDialog.SetToolTip(Me.btnDontAskAgain, resources.GetString("btnDontAskAgain.ToolTip"))
        Me.btnDontAskAgain.UseVisualStyleBackColor = True
        '
        'chkDontAskAgain
        '
        Me.chkDontAskAgain.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkDontAskAgain.AutoSize = True
        Me.chkDontAskAgain.Location = New System.Drawing.Point(191, 369)
        Me.chkDontAskAgain.Name = "chkDontAskAgain"
        Me.chkDontAskAgain.Size = New System.Drawing.Size(15, 14)
        Me.chkDontAskAgain.TabIndex = 21
        Me.chkDontAskAgain.TabStop = False
        Me.chkDontAskAgain.UseVisualStyleBackColor = True
        Me.chkDontAskAgain.Visible = False
        '
        'ButtonGrabberOptions
        '
        Me.ButtonGrabberOptions.Location = New System.Drawing.Point(311, 59)
        Me.ButtonGrabberOptions.Name = "ButtonGrabberOptions"
        Me.ButtonGrabberOptions.Size = New System.Drawing.Size(101, 20)
        Me.ButtonGrabberOptions.TabIndex = 15
        Me.ButtonGrabberOptions.Text = "Grabber Options"
        Me.ToolTipImportDialog.SetToolTip(Me.ButtonGrabberOptions, "This starts the ""Grabber Interface"" " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "and allows you to either change user settin" & _
                "gs " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "or modify the grabber script.")
        Me.ButtonGrabberOptions.UseVisualStyleBackColor = True
        '
        'txtSearchintYear
        '
        Me.txtSearchintYear.Enabled = False
        Me.txtSearchintYear.Location = New System.Drawing.Point(638, 32)
        Me.txtSearchintYear.Name = "txtSearchintYear"
        Me.txtSearchintYear.Size = New System.Drawing.Size(44, 20)
        Me.txtSearchintYear.TabIndex = 8
        Me.txtSearchintYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtSearchhintIMDB_Id
        '
        Me.txtSearchhintIMDB_Id.Location = New System.Drawing.Point(766, 32)
        Me.txtSearchhintIMDB_Id.Name = "txtSearchhintIMDB_Id"
        Me.txtSearchhintIMDB_Id.Size = New System.Drawing.Size(78, 20)
        Me.txtSearchhintIMDB_Id.TabIndex = 10
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(571, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 26)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Searchhint " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Year"
        Me.ToolTipImportDialog.SetToolTip(Me.Label1, "If a year can be retrieved from the directory " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "or file name, the result is shown" & _
                " here.")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(703, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 26)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Searchhint " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "IMDB_Id"
        Me.ToolTipImportDialog.SetToolTip(Me.Label2, resources.GetString("Label2.ToolTip"))
        '
        'btnCancelFromDialog
        '
        Me.btnCancelFromDialog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCancelFromDialog.Location = New System.Drawing.Point(244, 364)
        Me.btnCancelFromDialog.Name = "btnCancelFromDialog"
        Me.btnCancelFromDialog.Size = New System.Drawing.Size(97, 23)
        Me.btnCancelFromDialog.TabIndex = 22
        Me.btnCancelFromDialog.Text = "Cancel Grabbing"
        Me.ToolTipImportDialog.SetToolTip(Me.btnCancelFromDialog, "Cancel current import.")
        Me.btnCancelFromDialog.UseVisualStyleBackColor = True
        '
        'txtTmpParserFilePathShort
        '
        Me.txtTmpParserFilePathShort.Enabled = False
        Me.txtTmpParserFilePathShort.Location = New System.Drawing.Point(96, 59)
        Me.txtTmpParserFilePathShort.Name = "txtTmpParserFilePathShort"
        Me.txtTmpParserFilePathShort.Size = New System.Drawing.Size(137, 20)
        Me.txtTmpParserFilePathShort.TabIndex = 13
        '
        'btnSearchAgainWithIMDB_Id
        '
        Me.btnSearchAgainWithIMDB_Id.Location = New System.Drawing.Point(706, 59)
        Me.btnSearchAgainWithIMDB_Id.Name = "btnSearchAgainWithIMDB_Id"
        Me.btnSearchAgainWithIMDB_Id.Size = New System.Drawing.Size(138, 20)
        Me.btnSearchAgainWithIMDB_Id.TabIndex = 17
        Me.btnSearchAgainWithIMDB_Id.Text = "Search IMDB"
        Me.ToolTipImportDialog.SetToolTip(Me.btnSearchAgainWithIMDB_Id, "Performs a search based on IMDB id entered above.")
        Me.btnSearchAgainWithIMDB_Id.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 62)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(75, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Grabber Script"
        Me.ToolTipImportDialog.SetToolTip(Me.Label3, "You can change the grabber script used for retrieving internet data." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Useful, if " & _
                "the default script does not find a result - so you can try " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "other internet site" & _
                "s to retrieve data.")
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 35)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(64, 13)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Search Title"
        Me.ToolTipImportDialog.SetToolTip(Me.Label4, resources.GetString("Label4.ToolTip"))
        '
        'txtSource
        '
        Me.txtSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSource.Enabled = False
        Me.txtSource.Location = New System.Drawing.Point(96, 6)
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(748, 20)
        Me.txtSource.TabIndex = 3
        '
        'LabelSourcePath
        '
        Me.LabelSourcePath.AutoSize = True
        Me.LabelSourcePath.Location = New System.Drawing.Point(15, 9)
        Me.LabelSourcePath.Name = "LabelSourcePath"
        Me.LabelSourcePath.Size = New System.Drawing.Size(73, 13)
        Me.LabelSourcePath.TabIndex = 0
        Me.LabelSourcePath.Text = "Media Source"
        Me.ToolTipImportDialog.SetToolTip(Me.LabelSourcePath, resources.GetString("LabelSourcePath.ToolTip"))
        '
        'lstOptionsExt
        '
        Me.lstOptionsExt.AllowUserToAddRows = False
        Me.lstOptionsExt.AllowUserToDeleteRows = False
        Me.lstOptionsExt.AllowUserToOrderColumns = True
        Me.lstOptionsExt.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstOptionsExt.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.lstOptionsExt.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstOptionsExt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.lstOptionsExt.BackgroundColor = System.Drawing.SystemColors.Control
        Me.lstOptionsExt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lstOptionsExt.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.lstOptionsExt.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.lstOptionsExt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.lstOptionsExt.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Thumb, Me.Title, Me.Year, Me.Options, Me.AKA, Me.ID, Me.Weblink, Me.Distance})
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.lstOptionsExt.DefaultCellStyle = DataGridViewCellStyle5
        Me.lstOptionsExt.Location = New System.Drawing.Point(18, 85)
        Me.lstOptionsExt.Margin = New System.Windows.Forms.Padding(1)
        Me.lstOptionsExt.MultiSelect = False
        Me.lstOptionsExt.Name = "lstOptionsExt"
        Me.lstOptionsExt.ReadOnly = True
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.lstOptionsExt.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.lstOptionsExt.RowHeadersVisible = False
        Me.lstOptionsExt.RowHeadersWidth = 11
        Me.lstOptionsExt.RowTemplate.Height = 24
        Me.lstOptionsExt.RowTemplate.ReadOnly = True
        Me.lstOptionsExt.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.lstOptionsExt.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.lstOptionsExt.Size = New System.Drawing.Size(828, 266)
        Me.lstOptionsExt.TabIndex = 18
        '
        'Thumb
        '
        Me.Thumb.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.NullValue = Nothing
        DataGridViewCellStyle2.Padding = New System.Windows.Forms.Padding(1)
        Me.Thumb.DefaultCellStyle = DataGridViewCellStyle2
        Me.Thumb.HeaderText = "Img"
        Me.Thumb.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom
        Me.Thumb.MinimumWidth = 28
        Me.Thumb.Name = "Thumb"
        Me.Thumb.ReadOnly = True
        Me.Thumb.Width = 28
        '
        'Title
        '
        Me.Title.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.Title.FillWeight = 377.7473!
        Me.Title.HeaderText = "Title"
        Me.Title.MinimumWidth = 150
        Me.Title.Name = "Title"
        Me.Title.ReadOnly = True
        Me.Title.Width = 150
        '
        'Year
        '
        Me.Year.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Year.DefaultCellStyle = DataGridViewCellStyle3
        Me.Year.HeaderText = "Year"
        Me.Year.Name = "Year"
        Me.Year.ReadOnly = True
        Me.Year.Width = 53
        '
        'Options
        '
        Me.Options.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.Options.HeaderText = "Options"
        Me.Options.Name = "Options"
        Me.Options.ReadOnly = True
        Me.Options.Width = 67
        '
        'AKA
        '
        Me.AKA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AKA.HeaderText = "AKA"
        Me.AKA.MinimumWidth = 50
        Me.AKA.Name = "AKA"
        Me.AKA.ReadOnly = True
        '
        'ID
        '
        Me.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.ID.HeaderText = "ID"
        Me.ID.Name = "ID"
        Me.ID.ReadOnly = True
        Me.ID.Width = 42
        '
        'Weblink
        '
        Me.Weblink.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Weblink.FillWeight = 50.0!
        Me.Weblink.HeaderText = "Weblink"
        Me.Weblink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
        Me.Weblink.MinimumWidth = 50
        Me.Weblink.Name = "Weblink"
        Me.Weblink.ReadOnly = True
        '
        'Distance
        '
        Me.Distance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Distance.DefaultCellStyle = DataGridViewCellStyle4
        Me.Distance.HeaderText = "Dist"
        Me.Distance.Name = "Distance"
        Me.Distance.ReadOnly = True
        Me.Distance.Width = 49
        '
        'btnSearchGoogle
        '
        Me.btnSearchGoogle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSearchGoogle.Location = New System.Drawing.Point(574, 59)
        Me.btnSearchGoogle.Name = "btnSearchGoogle"
        Me.btnSearchGoogle.Size = New System.Drawing.Size(108, 20)
        Me.btnSearchGoogle.TabIndex = 16
        Me.btnSearchGoogle.Text = "Search Internet"
        Me.ToolTipImportDialog.SetToolTip(Me.btnSearchGoogle, "Launches the web browser with search title as destination for a common lookup.")
        Me.btnSearchGoogle.UseVisualStyleBackColor = True
        '
        'ToolTipImportDialog
        '
        Me.ToolTipImportDialog.AutoPopDelay = 15000
        Me.ToolTipImportDialog.InitialDelay = 3000
        Me.ToolTipImportDialog.IsBalloon = True
        Me.ToolTipImportDialog.ReshowDelay = 100
        Me.ToolTipImportDialog.ToolTipTitle = "MyFilms AMCupdater Help ..."
        '
        'btnRenameAndCancel
        '
        Me.btnRenameAndCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRenameAndCancel.Location = New System.Drawing.Point(374, 364)
        Me.btnRenameAndCancel.Name = "btnRenameAndCancel"
        Me.btnRenameAndCancel.Size = New System.Drawing.Size(106, 23)
        Me.btnRenameAndCancel.TabIndex = 23
        Me.btnRenameAndCancel.Text = "rename and ignore"
        Me.ToolTipImportDialog.SetToolTip(Me.btnRenameAndCancel, resources.GetString("btnRenameAndCancel.ToolTip"))
        Me.btnRenameAndCancel.UseVisualStyleBackColor = True
        '
        'btnSearchAllPages
        '
        Me.btnSearchAllPages.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSearchAllPages.Enabled = False
        Me.btnSearchAllPages.Location = New System.Drawing.Point(126, 364)
        Me.btnSearchAllPages.Name = "btnSearchAllPages"
        Me.btnSearchAllPages.Size = New System.Drawing.Size(22, 23)
        Me.btnSearchAllPages.TabIndex = 20
        Me.btnSearchAllPages.Text = ">"
        Me.ToolTipImportDialog.SetToolTip(Me.btnSearchAllPages, "For grabbers supporting multiple pages, " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "you can show search results of all page" & _
                "s." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Note: " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "When using simple search expressions, this can result " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "in many resu" & _
                "lts and significant loading time." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.btnSearchAllPages.UseVisualStyleBackColor = True
        '
        'txtSourceFull
        '
        Me.txtSourceFull.Location = New System.Drawing.Point(501, 6)
        Me.txtSourceFull.Name = "txtSourceFull"
        Me.txtSourceFull.Size = New System.Drawing.Size(44, 20)
        Me.txtSourceFull.TabIndex = 5
        Me.txtSourceFull.Visible = False
        '
        'txtSourceFullAllPath
        '
        Me.txtSourceFullAllPath.Location = New System.Drawing.Point(374, 6)
        Me.txtSourceFullAllPath.Name = "txtSourceFullAllPath"
        Me.txtSourceFullAllPath.Size = New System.Drawing.Size(38, 20)
        Me.txtSourceFullAllPath.TabIndex = 4
        Me.txtSourceFullAllPath.Visible = False
        '
        'pbCoverPreview
        '
        Me.pbCoverPreview.ErrorImage = Global.AMCUpdater.My.Resources.Resources.film_reel_128x128
        Me.pbCoverPreview.Location = New System.Drawing.Point(18, 6)
        Me.pbCoverPreview.Margin = New System.Windows.Forms.Padding(2)
        Me.pbCoverPreview.Name = "pbCoverPreview"
        Me.pbCoverPreview.Size = New System.Drawing.Size(61, 73)
        Me.pbCoverPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbCoverPreview.TabIndex = 88
        Me.pbCoverPreview.TabStop = False
        Me.pbCoverPreview.Visible = False
        '
        'FrmList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(856, 396)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnSearchAllPages)
        Me.Controls.Add(Me.pbCoverPreview)
        Me.Controls.Add(Me.txtSourceFullAllPath)
        Me.Controls.Add(Me.txtSourceFull)
        Me.Controls.Add(Me.btnRenameAndCancel)
        Me.Controls.Add(Me.btnSearchGoogle)
        Me.Controls.Add(Me.lstOptionsExt)
        Me.Controls.Add(Me.LabelSourcePath)
        Me.Controls.Add(Me.txtSource)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btnSearchAgainWithIMDB_Id)
        Me.Controls.Add(Me.txtTmpParserFilePathShort)
        Me.Controls.Add(Me.btnCancelFromDialog)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtSearchhintIMDB_Id)
        Me.Controls.Add(Me.txtSearchintYear)
        Me.Controls.Add(Me.ButtonGrabberOptions)
        Me.Controls.Add(Me.chkDontAskAgain)
        Me.Controls.Add(Me.btnDontAskAgain)
        Me.Controls.Add(Me.btnSelectParserFile)
        Me.Controls.Add(Me.txtTmpParserFilePath)
        Me.Controls.Add(Me.btnSearchAgain)
        Me.Controls.Add(Me.txtSearchString)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.DoubleBuffered = True
        Me.KeyPreview = True
        Me.Name = "frmList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Please select the correct entry"
        Me.ToolTipImportDialog.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        CType(Me.lstOptionsExt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbCoverPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents txtSearchString As System.Windows.Forms.TextBox
    Friend WithEvents btnSearchAgain As System.Windows.Forms.Button
    Friend WithEvents txtTmpParserFilePath As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectParserFile As System.Windows.Forms.Button
    Friend WithEvents btnDontAskAgain As System.Windows.Forms.Button
    Friend WithEvents chkDontAskAgain As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonGrabberOptions As System.Windows.Forms.Button
    Friend WithEvents txtSearchintYear As System.Windows.Forms.TextBox
    Friend WithEvents txtSearchhintIMDB_Id As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnCancelFromDialog As System.Windows.Forms.Button
    Friend WithEvents txtTmpParserFilePathShort As System.Windows.Forms.TextBox
    Friend WithEvents btnSearchAgainWithIMDB_Id As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtSource As System.Windows.Forms.TextBox
    Friend WithEvents LabelSourcePath As System.Windows.Forms.Label
    Friend WithEvents lstOptionsExt As System.Windows.Forms.DataGridView
    Friend WithEvents btnSearchGoogle As System.Windows.Forms.Button
    Friend WithEvents ToolTipImportDialog As System.Windows.Forms.ToolTip
    Friend WithEvents btnRenameAndCancel As System.Windows.Forms.Button
    Friend WithEvents txtSourceFull As System.Windows.Forms.TextBox
    Friend WithEvents txtSourceFullAllPath As System.Windows.Forms.TextBox
    Friend WithEvents pbCoverPreview As System.Windows.Forms.PictureBox
    Friend WithEvents Thumb As System.Windows.Forms.DataGridViewImageColumn
    Friend WithEvents Title As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Year As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Options As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AKA As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Weblink As System.Windows.Forms.DataGridViewLinkColumn
    Friend WithEvents Distance As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents btnSearchAllPages As System.Windows.Forms.Button
End Class
