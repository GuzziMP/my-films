<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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

    <STAThread()> _
    Private Sub Main()

    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim OwnerLabel As System.Windows.Forms.Label
        Dim MailLabel As System.Windows.Forms.Label
        Dim SiteLabel As System.Windows.Forms.Label
        Dim DescriptionLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.Import_Movies = New System.Windows.Forms.TabPage()
        Me.btnOpenLog = New System.Windows.Forms.Button()
        Me.Label_VersionNumber = New System.Windows.Forms.Label()
        Me.LinkLabelMyFilmsWiki = New System.Windows.Forms.LinkLabel()
        Me.Label88 = New System.Windows.Forms.Label()
        Me.GroupBox27 = New System.Windows.Forms.GroupBox()
        Me.chkDontAskInteractive = New System.Windows.Forms.CheckBox()
        Me.chkImportOnInternetFailInGuiMode = New System.Windows.Forms.CheckBox()
        Me.cbInternetLookupAlwaysPrompt = New System.Windows.Forms.CheckBox()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.chkImportOnInternetFail = New System.Windows.Forms.CheckBox()
        Me.lblInternetLookupCaseExplanation = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.cbInternetLookupBehaviour = New System.Windows.Forms.ComboBox()
        Me.GroupBox23 = New System.Windows.Forms.GroupBox()
        Me.btnImportFromDrive = New System.Windows.Forms.Button()
        Me.txtParserFilePathDisplay = New System.Windows.Forms.TextBox()
        Me.cbTitleHandling = New System.Windows.Forms.ComboBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnSelectParserFile = New System.Windows.Forms.Button()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.txtParserFilePath = New System.Windows.Forms.TextBox()
        Me.GroupBox17 = New System.Windows.Forms.GroupBox()
        Me.chkPurgeMissingAlways = New System.Windows.Forms.CheckBox()
        Me.chkRescanMovedFiles = New System.Windows.Forms.CheckBox()
        Me.chkProhibitInternetLookup = New System.Windows.Forms.CheckBox()
        Me.chkPurgeMissing = New System.Windows.Forms.CheckBox()
        Me.chkOverwriteXML = New System.Windows.Forms.CheckBox()
        Me.chkBackupXMLFirst = New System.Windows.Forms.CheckBox()
        Me.ToolStripProgressMessage = New System.Windows.Forms.Label()
        Me.btnShowHideLog = New System.Windows.Forms.Button()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.BtnImportWatcher = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btnParseXML = New System.Windows.Forms.Button()
        Me.btnFindOrphans = New System.Windows.Forms.Button()
        Me.btnCancelProcessing = New System.Windows.Forms.Button()
        Me.btnUpdateXML = New System.Windows.Forms.Button()
        Me.btnProcessMovieList = New System.Windows.Forms.Button()
        Me.btnJustDoIt = New System.Windows.Forms.Button()
        Me.Update_Movies = New System.Windows.Forms.TabPage()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.GroupBox32 = New System.Windows.Forms.GroupBox()
        Me.btnManualCancel = New System.Windows.Forms.Button()
        Me.btnManualDoTest = New System.Windows.Forms.Button()
        Me.btnManualApplyChanges = New System.Windows.Forms.Button()
        Me.grpManualInternetLookupSettings = New System.Windows.Forms.GroupBox()
        Me.cbManualInternetLookupAlwaysPrompt = New System.Windows.Forms.CheckBox()
        Me.chkManualDontAskInteractive = New System.Windows.Forms.CheckBox()
        Me.txtManualInternetParserPathDisplay = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.btnManualSelectInternetParserPath = New System.Windows.Forms.Button()
        Me.cbManualInternetLookupBehaviour = New System.Windows.Forms.ComboBox()
        Me.txtManualInternetParserPath = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.btnShowHideLogTest = New System.Windows.Forms.Button()
        Me.grpManualUpdatesParameters = New System.Windows.Forms.GroupBox()
        Me.cbSkipExcludedMovieFiles = New System.Windows.Forms.CheckBox()
        Me.cbManualParameterAndOr = New System.Windows.Forms.ComboBox()
        Me.lblManualParametersValue2 = New System.Windows.Forms.Label()
        Me.lblManualParametersOperator2 = New System.Windows.Forms.Label()
        Me.txtManualParameterValue2 = New System.Windows.Forms.TextBox()
        Me.cbManualParameterOperator2 = New System.Windows.Forms.ComboBox()
        Me.lblManualParametersField2 = New System.Windows.Forms.Label()
        Me.cbManualParameterFieldList2 = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.chkManualParametersUpdateAll = New System.Windows.Forms.CheckBox()
        Me.lblManualParametersValue1 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtManualParameterValue1 = New System.Windows.Forms.TextBox()
        Me.cbManualParameterOperator1 = New System.Windows.Forms.ComboBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.cbManualParameterFieldList1 = New System.Windows.Forms.ComboBox()
        Me.GroupBox10 = New System.Windows.Forms.GroupBox()
        Me.chkManualNfoFilesOnlyAddMissing = New System.Windows.Forms.CheckBox()
        Me.cbManualNfoFileHandling = New System.Windows.Forms.ComboBox()
        Me.lblManualNfoFileHandling = New System.Windows.Forms.Label()
        Me.lblManualSelectFieldDestination = New System.Windows.Forms.Label()
        Me.cbManualSelectFieldDestination = New System.Windows.Forms.ComboBox()
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData = New System.Windows.Forms.CheckBox()
        Me.chkManualUpdateRecordsOnlyMissingData = New System.Windows.Forms.CheckBox()
        Me.txtManualOldValue = New System.Windows.Forms.TextBox()
        Me.lblManualEnterOldValue = New System.Windows.Forms.Label()
        Me.chkManualMissingFanartDownload = New System.Windows.Forms.CheckBox()
        Me.lblManualDatabaseFieldsPrompt = New System.Windows.Forms.Label()
        Me.lblManualSelectField = New System.Windows.Forms.Label()
        Me.cbManualSelectField = New System.Windows.Forms.ComboBox()
        Me.txtManualNewValue = New System.Windows.Forms.TextBox()
        Me.lblManualEnterNewValue = New System.Windows.Forms.Label()
        Me.cbManualSelectOperation = New System.Windows.Forms.ComboBox()
        Me.DatabaseFields = New System.Windows.Forms.TabPage()
        Me.GroupBox_FanartHandling = New System.Windows.Forms.GroupBox()
        Me.chkLoadPersonImagesWithFanart = New System.Windows.Forms.CheckBox()
        Me.chkUseGrabberForFanart = New System.Windows.Forms.CheckBox()
        Me.cbFanartLimitResolutionMin = New System.Windows.Forms.ComboBox()
        Me.lblFanartLimits = New System.Windows.Forms.Label()
        Me.cbFanartLimitResolutionMax = New System.Windows.Forms.ComboBox()
        Me.cbNumFanartLimitNumber = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox_TitleHandling = New System.Windows.Forms.GroupBox()
        Me.Label101 = New System.Windows.Forms.Label()
        Me.cbEditionNameAppliesTo = New System.Windows.Forms.ComboBox()
        Me.txtSeriesNameIdentifier = New System.Windows.Forms.TextBox()
        Me.Label106 = New System.Windows.Forms.Label()
        Me.Label89 = New System.Windows.Forms.Label()
        Me.txtGroupNameIdentifier = New System.Windows.Forms.TextBox()
        Me.cbGroupNameAppliesTo = New System.Windows.Forms.ComboBox()
        Me.chkFolderNameIsGroupName = New System.Windows.Forms.CheckBox()
        Me.lblGroupNameAppliesTo = New System.Windows.Forms.Label()
        Me.cbMasterTitle = New System.Windows.Forms.ComboBox()
        Me.lblMasterTitle = New System.Windows.Forms.Label()
        Me.GroupBox_StorageFieldHandling = New System.Windows.Forms.GroupBox()
        Me.chkShortNames = New System.Windows.Forms.CheckBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.txtDefaultSourceField = New System.Windows.Forms.TextBox()
        Me.GroupBox_MediaLabelFieldHandling = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.chkReadDVDLabel = New System.Windows.Forms.CheckBox()
        Me.txtMediaLabel = New System.Windows.Forms.TextBox()
        Me.GroupBox_OtherFieldHandling = New System.Windows.Forms.GroupBox()
        Me.chkUseInternetDataForLanguagesField = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtMediaType = New System.Windows.Forms.TextBox()
        Me.lblCheckFieldHandling = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.cbDateHandling = New System.Windows.Forms.ComboBox()
        Me.cbCheckHandling = New System.Windows.Forms.ComboBox()
        Me.GroupBox_PictureHandling = New System.Windows.Forms.GroupBox()
        Me.txtPictureFilenameSuffix = New System.Windows.Forms.TextBox()
        Me.lblPicturePrefix = New System.Windows.Forms.Label()
        Me.lblPictureHandling = New System.Windows.Forms.Label()
        Me.cbPictureHandling = New System.Windows.Forms.ComboBox()
        Me.txtPictureFilenamePrefix = New System.Windows.Forms.TextBox()
        Me.lblInternetLookupRequired = New System.Windows.Forms.Label()
        Me.GroupBox16 = New System.Windows.Forms.GroupBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.btnDBFieldsSelectNoMedia = New System.Windows.Forms.Button()
        Me.btnDBFieldsSelectNoInternet = New System.Windows.Forms.Button()
        Me.btnDBFieldsSelectNone = New System.Windows.Forms.Button()
        Me.btnDBFieldsSelectAllMedia = New System.Windows.Forms.Button()
        Me.btnDBFieldsSelectAllInternet = New System.Windows.Forms.Button()
        Me.btnDBFieldsSelectAll = New System.Windows.Forms.Button()
        Me.cbDatabaseFields = New System.Windows.Forms.CheckedListBox()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.ScanFilters = New System.Windows.Forms.TabPage()
        Me.GroupBox28 = New System.Windows.Forms.GroupBox()
        Me.dgEditionStrings = New System.Windows.Forms.DataGridView()
        Me.EditorSearchExpression = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EditorReplacementString = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupBox24 = New System.Windows.Forms.GroupBox()
        Me.txtOverridePath = New System.Windows.Forms.TextBox()
        Me.lblOverridePath = New System.Windows.Forms.Label()
        Me.btnSelectMovieFolder = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtMovieFolder = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.txtRegExSearchMultiPart = New System.Windows.Forms.TextBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.GroupBox20 = New System.Windows.Forms.GroupBox()
        Me.Label83 = New System.Windows.Forms.Label()
        Me.dgFilterStrings = New System.Windows.Forms.DataGridView()
        Me.GroupBox19 = New System.Windows.Forms.GroupBox()
        Me.dgExcludedFolderStrings = New System.Windows.Forms.DataGridView()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.dgExcludedFileStrings = New System.Windows.Forms.DataGridView()
        Me.Options = New System.Windows.Forms.TabPage()
        Me.GroupBox26 = New System.Windows.Forms.GroupBox()
        Me.Label87 = New System.Windows.Forms.Label()
        Me.Label86 = New System.Windows.Forms.Label()
        Me.Label85 = New System.Windows.Forms.Label()
        Me.Label84 = New System.Windows.Forms.Label()
        Me.chkGrabberOverrideTitleLimit = New System.Windows.Forms.ComboBox()
        Me.chkGrabberOverridePersonLimit = New System.Windows.Forms.ComboBox()
        Me.chkGrabberOverrideGetRoles = New System.Windows.Forms.ComboBox()
        Me.chkGrabberOverrideLanguage = New System.Windows.Forms.ComboBox()
        Me.GroupBox11 = New System.Windows.Forms.GroupBox()
        Me.btnExcludeFileDelete = New System.Windows.Forms.Button()
        Me.btnExcludeFileShow = New System.Windows.Forms.Button()
        Me.txtExcludeFilePath = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnSelectExcludeFile = New System.Windows.Forms.Button()
        Me.txtConfigFilePath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnSelectConfigFile = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.chkParseTrailers = New System.Windows.Forms.CheckBox()
        Me.GroupBox25 = New System.Windows.Forms.GroupBox()
        Me.chkCheckDVDFolders = New System.Windows.Forms.CheckBox()
        Me.chkParseSubtitleFiles = New System.Windows.Forms.CheckBox()
        Me.chkParsePlaylistFiles = New System.Windows.Forms.CheckBox()
        Me.txtTrailerIentificationStrings = New System.Windows.Forms.TextBox()
        Me.Label79 = New System.Windows.Forms.Label()
        Me.txtDefaultFileTypesNonMedia = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtDefaultFileTypes = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.btnSelectPersonArtworkFolder = New System.Windows.Forms.Button()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.txtPersonArtworkFolder = New System.Windows.Forms.TextBox()
        Me.btnSelectFanartFolder = New System.Windows.Forms.Button()
        Me.txtFanartFolder = New System.Windows.Forms.TextBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.GroupBox13 = New System.Windows.Forms.GroupBox()
        Me.cbLogLevel = New System.Windows.Forms.ComboBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.GroupBox12 = New System.Windows.Forms.GroupBox()
        Me.chkExecuteOnlyForOrphans = New System.Windows.Forms.CheckBox()
        Me.btnExecuteProgramSelectPath = New System.Windows.Forms.Button()
        Me.txtExecuteProgramPath = New System.Windows.Forms.TextBox()
        Me.chkExecuteProgram = New System.Windows.Forms.CheckBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Mediainfo = New System.Windows.Forms.TabPage()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.GroupBox18 = New System.Windows.Forms.GroupBox()
        Me.txtSampleTextLanguageList = New System.Windows.Forms.TextBox()
        Me.txtSampleTextCodecList = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.txtSampleFileLength = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtSampleFileSize = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label100 = New System.Windows.Forms.Label()
        Me.txtSampleAudioChannelCount = New System.Windows.Forms.TextBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.txtSampleAudioLanguageList = New System.Windows.Forms.TextBox()
        Me.txtSampleAudioStreamList = New System.Windows.Forms.TextBox()
        Me.txtSampleAudioStreamCount = New System.Windows.Forms.TextBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.txtSampleAudioBitrate = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtSampleAudioCodec = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.btnTestAnalyse = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtSampleVideo3D = New System.Windows.Forms.TextBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.txtSampleAspectRatio = New System.Windows.Forms.TextBox()
        Me.Label107 = New System.Windows.Forms.Label()
        Me.txtSampleVideoResolution = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtSampleVideoFramerate = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtSampleVideoBitrate = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtSampleVideoCodec = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnGetSampleFile = New System.Windows.Forms.Button()
        Me.txtSampleFile = New System.Windows.Forms.TextBox()
        Me.ViewCollection = New System.Windows.Forms.TabPage()
        Me.GroupBoxMovieDetails = New System.Windows.Forms.GroupBox()
        Me.TextBox40 = New System.Windows.Forms.TextBox()
        Me.MovieBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TextBox39 = New System.Windows.Forms.TextBox()
        Me.TextBox38 = New System.Windows.Forms.TextBox()
        Me.TextBox37 = New System.Windows.Forms.TextBox()
        Me.Label103 = New System.Windows.Forms.Label()
        Me.Label102 = New System.Windows.Forms.Label()
        Me.TextBox33 = New System.Windows.Forms.TextBox()
        Me.TextBox32 = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TextBox31 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label82 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.TextBox30 = New System.Windows.Forms.TextBox()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.TextBox29 = New System.Windows.Forms.TextBox()
        Me.GroupBox14 = New System.Windows.Forms.GroupBox()
        Me.TextBox36 = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.TextBox7 = New System.Windows.Forms.TextBox()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label78 = New System.Windows.Forms.Label()
        Me.TextBox28 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.TextBox27 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.TextBox24 = New System.Windows.Forms.TextBox()
        Me.TextBox9 = New System.Windows.Forms.TextBox()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.TextBox23 = New System.Windows.Forms.TextBox()
        Me.TextBox8 = New System.Windows.Forms.TextBox()
        Me.GroupBox22 = New System.Windows.Forms.GroupBox()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.TextBox25 = New System.Windows.Forms.TextBox()
        Me.TextBox26 = New System.Windows.Forms.TextBox()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.GroupBox21 = New System.Windows.Forms.GroupBox()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.TextBox22 = New System.Windows.Forms.TextBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.TextBox21 = New System.Windows.Forms.TextBox()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.Label70 = New System.Windows.Forms.Label()
        Me.TextBox19 = New System.Windows.Forms.TextBox()
        Me.TextBox20 = New System.Windows.Forms.TextBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.GroupBox15 = New System.Windows.Forms.GroupBox()
        Me.Label68 = New System.Windows.Forms.Label()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.TextBox17 = New System.Windows.Forms.TextBox()
        Me.TextBox18 = New System.Windows.Forms.TextBox()
        Me.TextBox10 = New System.Windows.Forms.TextBox()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.TextBox16 = New System.Windows.Forms.TextBox()
        Me.TextBox11 = New System.Windows.Forms.TextBox()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.TextBox14 = New System.Windows.Forms.TextBox()
        Me.TextBox13 = New System.Windows.Forms.TextBox()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.TextBox15 = New System.Windows.Forms.TextBox()
        Me.TextBox12 = New System.Windows.Forms.TextBox()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.XionPanel1 = New XionControls.XionPanel()
        Me.DataGridViewMovie = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Category = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Country = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Rating = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MovieBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.VidéoBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorUpdate = New System.Windows.Forms.ToolStripButton()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.ViewPersons = New System.Windows.Forms.TabPage()
        Me.GroupBoxPersonInfo = New System.Windows.Forms.GroupBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.TextBox34 = New System.Windows.Forms.TextBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Label104 = New System.Windows.Forms.Label()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.TextBox35 = New System.Windows.Forms.TextBox()
        Me.Label105 = New System.Windows.Forms.Label()
        Me.CheckBoxWriter = New System.Windows.Forms.CheckBox()
        Me.PersonBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CheckBoxDirector = New System.Windows.Forms.CheckBox()
        Me.CheckBoxProducer = New System.Windows.Forms.CheckBox()
        Me.CheckBoxActor = New System.Windows.Forms.CheckBox()
        Me.Label99 = New System.Windows.Forms.Label()
        Me.Label98 = New System.Windows.Forms.Label()
        Me.Label97 = New System.Windows.Forms.Label()
        Me.Label96 = New System.Windows.Forms.Label()
        Me.ViewPersons_Photos = New System.Windows.Forms.TextBox()
        Me.ViewPersons_URL = New System.Windows.Forms.TextBox()
        Me.ViewPersons_TMDB_Id = New System.Windows.Forms.TextBox()
        Me.ViewPersons_IMDB_Id = New System.Windows.Forms.TextBox()
        Me.Label95 = New System.Windows.Forms.Label()
        Me.Label94 = New System.Windows.Forms.Label()
        Me.Label93 = New System.Windows.Forms.Label()
        Me.ViewPersons_Birthplace = New System.Windows.Forms.TextBox()
        Me.ViewPersons_Birthday = New System.Windows.Forms.TextBox()
        Me.ViewPersons_MiniBio = New System.Windows.Forms.RichTextBox()
        Me.ViewPersons_Name = New System.Windows.Forms.TextBox()
        Me.ViewPersons_Biography = New System.Windows.Forms.RichTextBox()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.Label91 = New System.Windows.Forms.Label()
        Me.Label92 = New System.Windows.Forms.Label()
        Me.Label90 = New System.Windows.Forms.Label()
        Me.ViewPersons_OtherName = New System.Windows.Forms.TextBox()
        Me.XionPanelPerson = New XionControls.XionPanel()
        Me.DataGridViewPerson = New System.Windows.Forms.DataGridView()
        Me.NameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AlternateNameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BornDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BirthPlaceDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MiniBiographyDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BiographyDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.URLDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IMDBIdDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TMDBIdDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PictureDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PhotosDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IsActor = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.IsProducer = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.IsDirector = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.IsWriter = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.PersonBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItemPerson = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorCountItem1 = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorDeleteItemPerson = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveFirstItem1 = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem1 = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem1 = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem1 = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem1 = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SpeichernToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButtonAddMissingPersons = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonGrabPersons = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripTextBoxSearch = New System.Windows.Forms.ToolStripTextBox()
        Me.ViewCatalog = New System.Windows.Forms.TabPage()
        Me.UserDataGridView = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn15 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn16 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn17 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UserBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.AntMovieCatalog = New AMCUpdater.AntMovieCatalog()
        Me.CustomFieldDataGridView = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn12 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn13 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CustomFieldBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.OwnerTextBox = New System.Windows.Forms.TextBox()
        Me.PropertiesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.MailTextBox = New System.Windows.Forms.TextBox()
        Me.SiteTextBox = New System.Windows.Forms.TextBox()
        Me.DescriptionTextBox = New System.Windows.Forms.TextBox()
        Me.mnuFile = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMenuItemFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadConfigurationFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveConfigFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveConfigFileAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemScanPath = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemsScanFilter = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.InternetLinksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MyFilmsWikiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AntMovieCatalogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MediaInfodllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemDebug = New System.Windows.Forms.ToolStripMenuItem()
        Me.XMLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MediaFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NonMediaFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrailerFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OrphanMediaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OrphanNonMediaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OrphanTrailerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MultiPartFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OrphanAntToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MultiPartProcessedFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AntFieldsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NodesToProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ListMediaInfoParamsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripFixedText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.epInteractive = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.epManualUpdater = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.epOptions = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ListVideos = New System.Windows.Forms.DataGridView()
        Me.NumberDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OriginalTitleDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TranslatedTitleDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.YearDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DateAddedDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ImageListViewPersons = New System.Windows.Forms.ImageList(Me.components)
        OwnerLabel = New System.Windows.Forms.Label()
        MailLabel = New System.Windows.Forms.Label()
        SiteLabel = New System.Windows.Forms.Label()
        DescriptionLabel = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout
        Me.Import_Movies.SuspendLayout
        Me.GroupBox27.SuspendLayout
        Me.GroupBox9.SuspendLayout
        Me.GroupBox23.SuspendLayout
        Me.GroupBox17.SuspendLayout
        Me.GroupBox8.SuspendLayout
        Me.Update_Movies.SuspendLayout
        Me.GroupBox32.SuspendLayout
        Me.grpManualInternetLookupSettings.SuspendLayout
        Me.grpManualUpdatesParameters.SuspendLayout
        Me.GroupBox10.SuspendLayout
        Me.DatabaseFields.SuspendLayout
        Me.GroupBox_FanartHandling.SuspendLayout
        CType(Me.cbNumFanartLimitNumber,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GroupBox_TitleHandling.SuspendLayout
        Me.GroupBox_StorageFieldHandling.SuspendLayout
        Me.GroupBox_MediaLabelFieldHandling.SuspendLayout
        Me.GroupBox_OtherFieldHandling.SuspendLayout
        Me.GroupBox_PictureHandling.SuspendLayout
        Me.GroupBox16.SuspendLayout
        Me.ScanFilters.SuspendLayout
        Me.GroupBox28.SuspendLayout
        CType(Me.dgEditionStrings,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GroupBox24.SuspendLayout
        Me.GroupBox6.SuspendLayout
        Me.GroupBox20.SuspendLayout
        CType(Me.dgFilterStrings,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GroupBox19.SuspendLayout
        CType(Me.dgExcludedFolderStrings,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.dgExcludedFileStrings,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Options.SuspendLayout
        Me.GroupBox26.SuspendLayout
        Me.GroupBox11.SuspendLayout
        Me.GroupBox4.SuspendLayout
        Me.GroupBox25.SuspendLayout
        Me.GroupBox7.SuspendLayout
        Me.GroupBox13.SuspendLayout
        Me.GroupBox12.SuspendLayout
        Me.Mediainfo.SuspendLayout
        Me.GroupBox18.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.GroupBox2.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.ViewCollection.SuspendLayout
        Me.GroupBoxMovieDetails.SuspendLayout
        CType(Me.MovieBindingSource,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GroupBox14.SuspendLayout
        Me.GroupBox22.SuspendLayout
        Me.GroupBox21.SuspendLayout
        Me.GroupBox15.SuspendLayout
        Me.XionPanel1.SuspendLayout
        CType(Me.DataGridViewMovie,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.MovieBindingNavigator,System.ComponentModel.ISupportInitialize).BeginInit
        Me.MovieBindingNavigator.SuspendLayout
        Me.ViewPersons.SuspendLayout
        Me.GroupBoxPersonInfo.SuspendLayout
        Me.GroupBox5.SuspendLayout
        CType(Me.PersonBindingSource,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PictureBox2,System.ComponentModel.ISupportInitialize).BeginInit
        Me.XionPanelPerson.SuspendLayout
        CType(Me.DataGridViewPerson,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PersonBindingNavigator,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PersonBindingNavigator.SuspendLayout
        Me.ViewCatalog.SuspendLayout
        CType(Me.UserDataGridView,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.UserBindingSource,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.AntMovieCatalog,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CustomFieldDataGridView,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CustomFieldBindingSource,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PropertiesBindingSource,System.ComponentModel.ISupportInitialize).BeginInit
        Me.mnuFile.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.epInteractive,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.epManualUpdater,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.epOptions,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ListVideos,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'OwnerLabel
        '
        OwnerLabel.AutoSize = true
        OwnerLabel.Location = New System.Drawing.Point(8, 13)
        OwnerLabel.Name = "OwnerLabel"
        OwnerLabel.Size = New System.Drawing.Size(41, 13)
        OwnerLabel.TabIndex = 0
        OwnerLabel.Text = "Owner:"
        '
        'MailLabel
        '
        MailLabel.AutoSize = true
        MailLabel.Location = New System.Drawing.Point(8, 39)
        MailLabel.Name = "MailLabel"
        MailLabel.Size = New System.Drawing.Size(29, 13)
        MailLabel.TabIndex = 4
        MailLabel.Text = "Mail:"
        '
        'SiteLabel
        '
        SiteLabel.AutoSize = true
        SiteLabel.Location = New System.Drawing.Point(303, 13)
        SiteLabel.Name = "SiteLabel"
        SiteLabel.Size = New System.Drawing.Size(28, 13)
        SiteLabel.TabIndex = 2
        SiteLabel.Text = "Site:"
        '
        'DescriptionLabel
        '
        DescriptionLabel.AutoSize = true
        DescriptionLabel.Location = New System.Drawing.Point(303, 39)
        DescriptionLabel.Name = "DescriptionLabel"
        DescriptionLabel.Size = New System.Drawing.Size(63, 13)
        DescriptionLabel.TabIndex = 6
        DescriptionLabel.Text = "Description:"
        '
        'FolderBrowserDialog1
        '
        Me.FolderBrowserDialog1.ShowNewFolderButton = false
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.Import_Movies)
        Me.TabControl1.Controls.Add(Me.Update_Movies)
        Me.TabControl1.Controls.Add(Me.DatabaseFields)
        Me.TabControl1.Controls.Add(Me.ScanFilters)
        Me.TabControl1.Controls.Add(Me.Options)
        Me.TabControl1.Controls.Add(Me.Mediainfo)
        Me.TabControl1.Controls.Add(Me.ViewCollection)
        Me.TabControl1.Controls.Add(Me.ViewPersons)
        Me.TabControl1.Controls.Add(Me.ViewCatalog)
        Me.TabControl1.Location = New System.Drawing.Point(0, 27)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(608, 606)
        Me.TabControl1.TabIndex = 1
        '
        'Import_Movies
        '
        Me.Import_Movies.Controls.Add(Me.btnOpenLog)
        Me.Import_Movies.Controls.Add(Me.Label_VersionNumber)
        Me.Import_Movies.Controls.Add(Me.LinkLabelMyFilmsWiki)
        Me.Import_Movies.Controls.Add(Me.Label88)
        Me.Import_Movies.Controls.Add(Me.GroupBox27)
        Me.Import_Movies.Controls.Add(Me.GroupBox23)
        Me.Import_Movies.Controls.Add(Me.GroupBox17)
        Me.Import_Movies.Controls.Add(Me.ToolStripProgressMessage)
        Me.Import_Movies.Controls.Add(Me.btnShowHideLog)
        Me.Import_Movies.Controls.Add(Me.GroupBox8)
        Me.Import_Movies.Location = New System.Drawing.Point(4, 22)
        Me.Import_Movies.Name = "Import_Movies"
        Me.Import_Movies.Padding = New System.Windows.Forms.Padding(3)
        Me.Import_Movies.Size = New System.Drawing.Size(600, 580)
        Me.Import_Movies.TabIndex = 0
        Me.Import_Movies.Text = "Import Movies"
        Me.Import_Movies.UseVisualStyleBackColor = true
        '
        'btnOpenLog
        '
        Me.btnOpenLog.Location = New System.Drawing.Point(410, 549)
        Me.btnOpenLog.Name = "btnOpenLog"
        Me.btnOpenLog.Size = New System.Drawing.Size(75, 23)
        Me.btnOpenLog.TabIndex = 8
        Me.btnOpenLog.Text = "Open Log"
        Me.btnOpenLog.UseVisualStyleBackColor = true
        '
        'Label_VersionNumber
        '
        Me.Label_VersionNumber.AutoSize = true
        Me.Label_VersionNumber.Location = New System.Drawing.Point(23, 14)
        Me.Label_VersionNumber.Name = "Label_VersionNumber"
        Me.Label_VersionNumber.Size = New System.Drawing.Size(71, 13)
        Me.Label_VersionNumber.TabIndex = 0
        Me.Label_VersionNumber.Text = "V0.0.0.00000"
        Me.Label_VersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LinkLabelMyFilmsWiki
        '
        Me.LinkLabelMyFilmsWiki.AutoSize = true
        Me.LinkLabelMyFilmsWiki.Location = New System.Drawing.Point(111, 14)
        Me.LinkLabelMyFilmsWiki.Name = "LinkLabelMyFilmsWiki"
        Me.LinkLabelMyFilmsWiki.Size = New System.Drawing.Size(68, 13)
        Me.LinkLabelMyFilmsWiki.TabIndex = 1
        Me.LinkLabelMyFilmsWiki.TabStop = true
        Me.LinkLabelMyFilmsWiki.Text = "MyFilms Wiki"
        '
        'Label88
        '
        Me.Label88.Location = New System.Drawing.Point(19, 56)
        Me.Label88.Name = "Label88"
        Me.Label88.Size = New System.Drawing.Size(552, 60)
        Me.Label88.TabIndex = 2
        Me.Label88.Text = resources.GetString("Label88.Text")
        '
        'GroupBox27
        '
        Me.GroupBox27.Controls.Add(Me.chkDontAskInteractive)
        Me.GroupBox27.Controls.Add(Me.chkImportOnInternetFailInGuiMode)
        Me.GroupBox27.Controls.Add(Me.cbInternetLookupAlwaysPrompt)
        Me.GroupBox27.Controls.Add(Me.GroupBox9)
        Me.GroupBox27.Controls.Add(Me.lblInternetLookupCaseExplanation)
        Me.GroupBox27.Controls.Add(Me.Label24)
        Me.GroupBox27.Controls.Add(Me.cbInternetLookupBehaviour)
        Me.GroupBox27.Location = New System.Drawing.Point(11, 218)
        Me.GroupBox27.Name = "GroupBox27"
        Me.GroupBox27.Size = New System.Drawing.Size(572, 126)
        Me.GroupBox27.TabIndex = 4
        Me.GroupBox27.TabStop = false
        Me.GroupBox27.Text = "Import Options ..."
        '
        'chkDontAskInteractive
        '
        Me.chkDontAskInteractive.AutoSize = true
        Me.chkDontAskInteractive.Location = New System.Drawing.Point(406, 80)
        Me.chkDontAskInteractive.Name = "chkDontAskInteractive"
        Me.chkDontAskInteractive.Size = New System.Drawing.Size(129, 17)
        Me.chkDontAskInteractive.TabIndex = 2
        Me.chkDontAskInteractive.Text = "Don't ask, if no match"
        Me.ToolTip1.SetToolTip(Me.chkDontAskInteractive, resources.GetString("chkDontAskInteractive.ToolTip"))
        Me.chkDontAskInteractive.UseVisualStyleBackColor = true
        '
        'chkImportOnInternetFailInGuiMode
        '
        Me.chkImportOnInternetFailInGuiMode.AutoSize = true
        Me.chkImportOnInternetFailInGuiMode.Location = New System.Drawing.Point(406, 102)
        Me.chkImportOnInternetFailInGuiMode.Name = "chkImportOnInternetFailInGuiMode"
        Me.chkImportOnInternetFailInGuiMode.Size = New System.Drawing.Size(131, 17)
        Me.chkImportOnInternetFailInGuiMode.TabIndex = 4
        Me.chkImportOnInternetFailInGuiMode.Text = "Import if no automatch"
        Me.ToolTip1.SetToolTip(Me.chkImportOnInternetFailInGuiMode, resources.GetString("chkImportOnInternetFailInGuiMode.ToolTip"))
        Me.chkImportOnInternetFailInGuiMode.UseVisualStyleBackColor = true
        '
        'cbInternetLookupAlwaysPrompt
        '
        Me.cbInternetLookupAlwaysPrompt.AutoSize = true
        Me.cbInternetLookupAlwaysPrompt.Location = New System.Drawing.Point(406, 59)
        Me.cbInternetLookupAlwaysPrompt.Name = "cbInternetLookupAlwaysPrompt"
        Me.cbInternetLookupAlwaysPrompt.Size = New System.Drawing.Size(93, 17)
        Me.cbInternetLookupAlwaysPrompt.TabIndex = 5
        Me.cbInternetLookupAlwaysPrompt.TabStop = false
        Me.cbInternetLookupAlwaysPrompt.Text = "always prompt"
        Me.cbInternetLookupAlwaysPrompt.UseVisualStyleBackColor = true
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.chkImportOnInternetFail)
        Me.GroupBox9.Location = New System.Drawing.Point(392, 9)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(174, 44)
        Me.GroupBox9.TabIndex = 10
        Me.GroupBox9.TabStop = false
        Me.GroupBox9.Text = "Import behaviour in MyFilms ..."
        Me.ToolTip1.SetToolTip(Me.GroupBox9, "This is the import behaviour used when you run AMCU in MyFilms plugin "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"via Globa"& _ 
        "l Updates > Import/Update all movies from the My Films main menu.")
        '
        'chkImportOnInternetFail
        '
        Me.chkImportOnInternetFail.AutoSize = true
        Me.chkImportOnInternetFail.Location = New System.Drawing.Point(14, 17)
        Me.chkImportOnInternetFail.Name = "chkImportOnInternetFail"
        Me.chkImportOnInternetFail.Size = New System.Drawing.Size(134, 17)
        Me.chkImportOnInternetFail.TabIndex = 3
        Me.chkImportOnInternetFail.Text = "Import if no auto match"
        Me.ToolTip1.SetToolTip(Me.chkImportOnInternetFail, resources.GetString("chkImportOnInternetFail.ToolTip"))
        Me.chkImportOnInternetFail.UseVisualStyleBackColor = true
        '
        'lblInternetLookupCaseExplanation
        '
        Me.lblInternetLookupCaseExplanation.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lblInternetLookupCaseExplanation.Location = New System.Drawing.Point(12, 56)
        Me.lblInternetLookupCaseExplanation.Name = "lblInternetLookupCaseExplanation"
        Me.lblInternetLookupCaseExplanation.Size = New System.Drawing.Size(548, 63)
        Me.lblInternetLookupCaseExplanation.TabIndex = 6
        Me.lblInternetLookupCaseExplanation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(14, 26)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(87, 13)
        Me.Label24.TabIndex = 0
        Me.Label24.Text = "Import Behaviour"
        '
        'cbInternetLookupBehaviour
        '
        Me.cbInternetLookupBehaviour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbInternetLookupBehaviour.FormattingEnabled = true
        Me.cbInternetLookupBehaviour.Items.AddRange(New Object() {"Auto match only", "Auto match & media only if no match", "Auto match & ask if no match", "Manual match (always ask)"})
        Me.cbInternetLookupBehaviour.Location = New System.Drawing.Point(116, 23)
        Me.cbInternetLookupBehaviour.Name = "cbInternetLookupBehaviour"
        Me.cbInternetLookupBehaviour.Size = New System.Drawing.Size(239, 21)
        Me.cbInternetLookupBehaviour.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cbInternetLookupBehaviour, resources.GetString("cbInternetLookupBehaviour.ToolTip"))
        '
        'GroupBox23
        '
        Me.GroupBox23.Controls.Add(Me.btnImportFromDrive)
        Me.GroupBox23.Controls.Add(Me.txtParserFilePathDisplay)
        Me.GroupBox23.Controls.Add(Me.cbTitleHandling)
        Me.GroupBox23.Controls.Add(Me.Label43)
        Me.GroupBox23.Controls.Add(Me.Button1)
        Me.GroupBox23.Controls.Add(Me.btnSelectParserFile)
        Me.GroupBox23.Controls.Add(Me.Label23)
        Me.GroupBox23.Controls.Add(Me.txtParserFilePath)
        Me.GroupBox23.Location = New System.Drawing.Point(11, 353)
        Me.GroupBox23.Name = "GroupBox23"
        Me.GroupBox23.Size = New System.Drawing.Size(572, 79)
        Me.GroupBox23.TabIndex = 5
        Me.GroupBox23.TabStop = false
        Me.GroupBox23.Text = "Internet Grabber Options ..."
        '
        'btnImportFromDrive
        '
        Me.btnImportFromDrive.Location = New System.Drawing.Point(500, 35)
        Me.btnImportFromDrive.Name = "btnImportFromDrive"
        Me.btnImportFromDrive.Size = New System.Drawing.Size(60, 38)
        Me.btnImportFromDrive.TabIndex = 6
        Me.btnImportFromDrive.Text = "Import Disc"
        Me.btnImportFromDrive.UseVisualStyleBackColor = true
        Me.btnImportFromDrive.Visible = false
        '
        'txtParserFilePathDisplay
        '
        Me.txtParserFilePathDisplay.Enabled = false
        Me.txtParserFilePathDisplay.Location = New System.Drawing.Point(116, 18)
        Me.txtParserFilePathDisplay.Name = "txtParserFilePathDisplay"
        Me.txtParserFilePathDisplay.Size = New System.Drawing.Size(135, 20)
        Me.txtParserFilePathDisplay.TabIndex = 1
        '
        'cbTitleHandling
        '
        Me.cbTitleHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbTitleHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbTitleHandling.FormattingEnabled = true
        Me.cbTitleHandling.Items.AddRange(New Object() {"File Name", "Folder Name", "Relative Name", "File Name + Internet Lookup", "Folder Name + Internet Lookup", "Relative Name + Internet Lookup"})
        Me.cbTitleHandling.Location = New System.Drawing.Point(116, 44)
        Me.cbTitleHandling.Name = "cbTitleHandling"
        Me.cbTitleHandling.Size = New System.Drawing.Size(198, 21)
        Me.cbTitleHandling.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cbTitleHandling, resources.GetString("cbTitleHandling.ToolTip"))
        '
        'Label43
        '
        Me.Label43.AutoSize = true
        Me.Label43.Location = New System.Drawing.Point(11, 41)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(86, 26)
        Me.Label43.TabIndex = 4
        Me.Label43.Text = "Title and "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Search Handling"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(364, 17)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(116, 20)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Grabber Options"
        Me.Button1.UseVisualStyleBackColor = true
        '
        'btnSelectParserFile
        '
        Me.btnSelectParserFile.CausesValidation = false
        Me.btnSelectParserFile.Location = New System.Drawing.Point(282, 18)
        Me.btnSelectParserFile.Name = "btnSelectParserFile"
        Me.btnSelectParserFile.Size = New System.Drawing.Size(32, 20)
        Me.btnSelectParserFile.TabIndex = 2
        Me.btnSelectParserFile.Text = "..."
        Me.btnSelectParserFile.UseVisualStyleBackColor = true
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Location = New System.Drawing.Point(11, 20)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(73, 13)
        Me.Label23.TabIndex = 0
        Me.Label23.Text = "Grabber script"
        '
        'txtParserFilePath
        '
        Me.txtParserFilePath.Location = New System.Drawing.Point(116, 18)
        Me.txtParserFilePath.Name = "txtParserFilePath"
        Me.txtParserFilePath.Size = New System.Drawing.Size(198, 20)
        Me.txtParserFilePath.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtParserFilePath, resources.GetString("txtParserFilePath.ToolTip"))
        Me.txtParserFilePath.Visible = false
        '
        'GroupBox17
        '
        Me.GroupBox17.Controls.Add(Me.chkPurgeMissingAlways)
        Me.GroupBox17.Controls.Add(Me.chkRescanMovedFiles)
        Me.GroupBox17.Controls.Add(Me.chkProhibitInternetLookup)
        Me.GroupBox17.Controls.Add(Me.chkPurgeMissing)
        Me.GroupBox17.Controls.Add(Me.chkOverwriteXML)
        Me.GroupBox17.Controls.Add(Me.chkBackupXMLFirst)
        Me.GroupBox17.Location = New System.Drawing.Point(11, 137)
        Me.GroupBox17.Name = "GroupBox17"
        Me.GroupBox17.Size = New System.Drawing.Size(572, 67)
        Me.GroupBox17.TabIndex = 3
        Me.GroupBox17.TabStop = false
        Me.GroupBox17.Text = "Options for File Handling ..."
        '
        'chkPurgeMissingAlways
        '
        Me.chkPurgeMissingAlways.AutoSize = true
        Me.chkPurgeMissingAlways.Location = New System.Drawing.Point(180, 42)
        Me.chkPurgeMissingAlways.Name = "chkPurgeMissingAlways"
        Me.chkPurgeMissingAlways.Size = New System.Drawing.Size(220, 17)
        Me.chkPurgeMissingAlways.TabIndex = 4
        Me.chkPurgeMissingAlways.Text = "Purge Orphans when Source unavailable"
        Me.ToolTip1.SetToolTip(Me.chkPurgeMissingAlways, resources.GetString("chkPurgeMissingAlways.ToolTip"))
        Me.chkPurgeMissingAlways.UseVisualStyleBackColor = true
        '
        'chkRescanMovedFiles
        '
        Me.chkRescanMovedFiles.AutoSize = true
        Me.chkRescanMovedFiles.Location = New System.Drawing.Point(406, 42)
        Me.chkRescanMovedFiles.Name = "chkRescanMovedFiles"
        Me.chkRescanMovedFiles.Size = New System.Drawing.Size(128, 17)
        Me.chkRescanMovedFiles.TabIndex = 5
        Me.chkRescanMovedFiles.Text = "Re-Scan Moved Files"
        Me.ToolTip1.SetToolTip(Me.chkRescanMovedFiles, "Forces a refresh of file-based and online data for a file when it is found to hav"& _ 
        "e moved.")
        Me.chkRescanMovedFiles.UseVisualStyleBackColor = true
        '
        'chkProhibitInternetLookup
        '
        Me.chkProhibitInternetLookup.AutoSize = true
        Me.chkProhibitInternetLookup.Location = New System.Drawing.Point(406, 19)
        Me.chkProhibitInternetLookup.Name = "chkProhibitInternetLookup"
        Me.chkProhibitInternetLookup.Size = New System.Drawing.Size(139, 17)
        Me.chkProhibitInternetLookup.TabIndex = 2
        Me.chkProhibitInternetLookup.Text = "Prohibit Internet Lookup"
        Me.ToolTip1.SetToolTip(Me.chkProhibitInternetLookup, "Global override option to disable all Internet-based functionality.")
        Me.chkProhibitInternetLookup.UseVisualStyleBackColor = true
        '
        'chkPurgeMissing
        '
        Me.chkPurgeMissing.AutoSize = true
        Me.chkPurgeMissing.Location = New System.Drawing.Point(180, 19)
        Me.chkPurgeMissing.Name = "chkPurgeMissing"
        Me.chkPurgeMissing.Size = New System.Drawing.Size(135, 17)
        Me.chkPurgeMissing.TabIndex = 1
        Me.chkPurgeMissing.Text = "Purge Orphan Records"
        Me.ToolTip1.SetToolTip(Me.chkPurgeMissing, resources.GetString("chkPurgeMissing.ToolTip"))
        Me.chkPurgeMissing.UseVisualStyleBackColor = true
        '
        'chkOverwriteXML
        '
        Me.chkOverwriteXML.AutoSize = true
        Me.chkOverwriteXML.Location = New System.Drawing.Point(12, 19)
        Me.chkOverwriteXML.Name = "chkOverwriteXML"
        Me.chkOverwriteXML.Size = New System.Drawing.Size(115, 17)
        Me.chkOverwriteXML.TabIndex = 0
        Me.chkOverwriteXML.Text = "Overwrite XML File"
        Me.ToolTip1.SetToolTip(Me.chkOverwriteXML, "Directly over-write the given Ant Movie Catalog database file.")
        Me.chkOverwriteXML.UseVisualStyleBackColor = true
        '
        'chkBackupXMLFirst
        '
        Me.chkBackupXMLFirst.AutoSize = true
        Me.chkBackupXMLFirst.Checked = true
        Me.chkBackupXMLFirst.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkBackupXMLFirst.Enabled = false
        Me.chkBackupXMLFirst.Location = New System.Drawing.Point(12, 42)
        Me.chkBackupXMLFirst.Name = "chkBackupXMLFirst"
        Me.chkBackupXMLFirst.Size = New System.Drawing.Size(107, 17)
        Me.chkBackupXMLFirst.TabIndex = 3
        Me.chkBackupXMLFirst.Text = "Backup XML File"
        Me.ToolTip1.SetToolTip(Me.chkBackupXMLFirst, "Back up the Ant Movie Catalog database file before modifying it.")
        Me.chkBackupXMLFirst.UseVisualStyleBackColor = true
        '
        'ToolStripProgressMessage
        '
        Me.ToolStripProgressMessage.AutoSize = true
        Me.ToolStripProgressMessage.Location = New System.Drawing.Point(16, 553)
        Me.ToolStripProgressMessage.Name = "ToolStripProgressMessage"
        Me.ToolStripProgressMessage.Size = New System.Drawing.Size(35, 13)
        Me.ToolStripProgressMessage.TabIndex = 7
        Me.ToolStripProgressMessage.Text = "status"
        Me.ToolStripProgressMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnShowHideLog
        '
        Me.btnShowHideLog.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnShowHideLog.Location = New System.Drawing.Point(491, 549)
        Me.btnShowHideLog.Name = "btnShowHideLog"
        Me.btnShowHideLog.Size = New System.Drawing.Size(92, 22)
        Me.btnShowHideLog.TabIndex = 9
        Me.btnShowHideLog.Text = "Show Log >>"
        Me.btnShowHideLog.UseVisualStyleBackColor = true
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.BtnImportWatcher)
        Me.GroupBox8.Controls.Add(Me.Label16)
        Me.GroupBox8.Controls.Add(Me.btnParseXML)
        Me.GroupBox8.Controls.Add(Me.btnFindOrphans)
        Me.GroupBox8.Controls.Add(Me.btnCancelProcessing)
        Me.GroupBox8.Controls.Add(Me.btnUpdateXML)
        Me.GroupBox8.Controls.Add(Me.btnProcessMovieList)
        Me.GroupBox8.Controls.Add(Me.btnJustDoIt)
        Me.GroupBox8.Location = New System.Drawing.Point(11, 438)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(572, 105)
        Me.GroupBox8.TabIndex = 6
        Me.GroupBox8.TabStop = false
        Me.GroupBox8.Text = "Begin Import ..."
        '
        'BtnImportWatcher
        '
        Me.BtnImportWatcher.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtnImportWatcher.Location = New System.Drawing.Point(500, 32)
        Me.BtnImportWatcher.Name = "BtnImportWatcher"
        Me.BtnImportWatcher.Size = New System.Drawing.Size(60, 57)
        Me.BtnImportWatcher.TabIndex = 7
        Me.BtnImportWatcher.Text = "Start Import Watcher"
        Me.BtnImportWatcher.UseVisualStyleBackColor = true
        '
        'Label16
        '
        Me.Label16.AutoSize = true
        Me.Label16.Location = New System.Drawing.Point(286, 16)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(178, 13)
        Me.Label16.TabIndex = 2
        Me.Label16.Text = "Or...  Run process steps individually:"
        '
        'btnParseXML
        '
        Me.btnParseXML.Location = New System.Drawing.Point(261, 36)
        Me.btnParseXML.Name = "btnParseXML"
        Me.btnParseXML.Size = New System.Drawing.Size(106, 23)
        Me.btnParseXML.TabIndex = 3
        Me.btnParseXML.Text = "1 - Scan Movie DB"
        Me.btnParseXML.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnParseXML.UseVisualStyleBackColor = true
        '
        'btnFindOrphans
        '
        Me.btnFindOrphans.Enabled = false
        Me.btnFindOrphans.Location = New System.Drawing.Point(261, 66)
        Me.btnFindOrphans.Name = "btnFindOrphans"
        Me.btnFindOrphans.Size = New System.Drawing.Size(106, 23)
        Me.btnFindOrphans.TabIndex = 5
        Me.btnFindOrphans.Text = "3 - Find Orphans"
        Me.btnFindOrphans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFindOrphans.UseVisualStyleBackColor = true
        '
        'btnCancelProcessing
        '
        Me.btnCancelProcessing.Enabled = false
        Me.btnCancelProcessing.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnCancelProcessing.Image = CType(resources.GetObject("btnCancelProcessing.Image"),System.Drawing.Image)
        Me.btnCancelProcessing.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnCancelProcessing.Location = New System.Drawing.Point(137, 32)
        Me.btnCancelProcessing.Name = "btnCancelProcessing"
        Me.btnCancelProcessing.Size = New System.Drawing.Size(99, 57)
        Me.btnCancelProcessing.TabIndex = 1
        Me.btnCancelProcessing.Text = "Cancel"
        Me.btnCancelProcessing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancelProcessing.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnCancelProcessing.UseVisualStyleBackColor = true
        '
        'btnUpdateXML
        '
        Me.btnUpdateXML.Enabled = false
        Me.btnUpdateXML.Location = New System.Drawing.Point(373, 66)
        Me.btnUpdateXML.Name = "btnUpdateXML"
        Me.btnUpdateXML.Size = New System.Drawing.Size(117, 23)
        Me.btnUpdateXML.TabIndex = 6
        Me.btnUpdateXML.Text = "4 - Update Movie DB"
        Me.btnUpdateXML.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnUpdateXML.UseVisualStyleBackColor = true
        '
        'btnProcessMovieList
        '
        Me.btnProcessMovieList.Enabled = false
        Me.btnProcessMovieList.Location = New System.Drawing.Point(373, 36)
        Me.btnProcessMovieList.Name = "btnProcessMovieList"
        Me.btnProcessMovieList.Size = New System.Drawing.Size(117, 23)
        Me.btnProcessMovieList.TabIndex = 4
        Me.btnProcessMovieList.Text = "2 - Scan Movie Files"
        Me.btnProcessMovieList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnProcessMovieList.UseVisualStyleBackColor = true
        '
        'btnJustDoIt
        '
        Me.btnJustDoIt.Image = CType(resources.GetObject("btnJustDoIt.Image"),System.Drawing.Image)
        Me.btnJustDoIt.Location = New System.Drawing.Point(13, 32)
        Me.btnJustDoIt.Name = "btnJustDoIt"
        Me.btnJustDoIt.Size = New System.Drawing.Size(103, 57)
        Me.btnJustDoIt.TabIndex = 0
        Me.btnJustDoIt.Text = "Begin Import"
        Me.btnJustDoIt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnJustDoIt.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnJustDoIt.UseVisualStyleBackColor = true
        '
        'Update_Movies
        '
        Me.Update_Movies.Controls.Add(Me.Label20)
        Me.Update_Movies.Controls.Add(Me.GroupBox32)
        Me.Update_Movies.Controls.Add(Me.grpManualInternetLookupSettings)
        Me.Update_Movies.Controls.Add(Me.btnShowHideLogTest)
        Me.Update_Movies.Controls.Add(Me.grpManualUpdatesParameters)
        Me.Update_Movies.Controls.Add(Me.GroupBox10)
        Me.Update_Movies.Location = New System.Drawing.Point(4, 22)
        Me.Update_Movies.Name = "Update_Movies"
        Me.Update_Movies.Size = New System.Drawing.Size(600, 580)
        Me.Update_Movies.TabIndex = 3
        Me.Update_Movies.Text = "Update Movies"
        Me.Update_Movies.UseVisualStyleBackColor = true
        '
        'Label20
        '
        Me.Label20.Location = New System.Drawing.Point(18, 37)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(552, 60)
        Me.Label20.TabIndex = 0
        Me.Label20.Text = resources.GetString("Label20.Text")
        '
        'GroupBox32
        '
        Me.GroupBox32.Controls.Add(Me.btnManualCancel)
        Me.GroupBox32.Controls.Add(Me.btnManualDoTest)
        Me.GroupBox32.Controls.Add(Me.btnManualApplyChanges)
        Me.GroupBox32.Location = New System.Drawing.Point(10, 484)
        Me.GroupBox32.Name = "GroupBox32"
        Me.GroupBox32.Size = New System.Drawing.Size(398, 87)
        Me.GroupBox32.TabIndex = 4
        Me.GroupBox32.TabStop = false
        Me.GroupBox32.Text = "Begin Update ..."
        '
        'btnManualCancel
        '
        Me.btnManualCancel.Enabled = false
        Me.btnManualCancel.Image = CType(resources.GetObject("btnManualCancel.Image"),System.Drawing.Image)
        Me.btnManualCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnManualCancel.Location = New System.Drawing.Point(263, 21)
        Me.btnManualCancel.Name = "btnManualCancel"
        Me.btnManualCancel.Size = New System.Drawing.Size(110, 57)
        Me.btnManualCancel.TabIndex = 2
        Me.btnManualCancel.Text = "Cancel"
        Me.btnManualCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualCancel.UseVisualStyleBackColor = true
        '
        'btnManualDoTest
        '
        Me.btnManualDoTest.Image = CType(resources.GetObject("btnManualDoTest.Image"),System.Drawing.Image)
        Me.btnManualDoTest.ImageAlign = System.Drawing.ContentAlignment.BottomRight
        Me.btnManualDoTest.Location = New System.Drawing.Point(8, 21)
        Me.btnManualDoTest.Name = "btnManualDoTest"
        Me.btnManualDoTest.Size = New System.Drawing.Size(110, 57)
        Me.btnManualDoTest.TabIndex = 0
        Me.btnManualDoTest.Text = "Test "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Params"
        Me.btnManualDoTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualDoTest.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualDoTest.UseVisualStyleBackColor = true
        '
        'btnManualApplyChanges
        '
        Me.btnManualApplyChanges.Enabled = false
        Me.btnManualApplyChanges.Image = CType(resources.GetObject("btnManualApplyChanges.Image"),System.Drawing.Image)
        Me.btnManualApplyChanges.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnManualApplyChanges.Location = New System.Drawing.Point(124, 21)
        Me.btnManualApplyChanges.Name = "btnManualApplyChanges"
        Me.btnManualApplyChanges.Size = New System.Drawing.Size(110, 57)
        Me.btnManualApplyChanges.TabIndex = 1
        Me.btnManualApplyChanges.Text = "Apply Changes"
        Me.btnManualApplyChanges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualApplyChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualApplyChanges.UseVisualStyleBackColor = true
        '
        'grpManualInternetLookupSettings
        '
        Me.grpManualInternetLookupSettings.Controls.Add(Me.cbManualInternetLookupAlwaysPrompt)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.chkManualDontAskInteractive)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.txtManualInternetParserPathDisplay)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Button2)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Label42)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.btnManualSelectInternetParserPath)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.cbManualInternetLookupBehaviour)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.txtManualInternetParserPath)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Label41)
        Me.grpManualInternetLookupSettings.Location = New System.Drawing.Point(10, 371)
        Me.grpManualInternetLookupSettings.Name = "grpManualInternetLookupSettings"
        Me.grpManualInternetLookupSettings.Size = New System.Drawing.Size(573, 107)
        Me.grpManualInternetLookupSettings.TabIndex = 3
        Me.grpManualInternetLookupSettings.TabStop = false
        Me.grpManualInternetLookupSettings.Text = "Internet Grabber Options ..."
        '
        'cbManualInternetLookupAlwaysPrompt
        '
        Me.cbManualInternetLookupAlwaysPrompt.AutoSize = true
        Me.cbManualInternetLookupAlwaysPrompt.Location = New System.Drawing.Point(343, 65)
        Me.cbManualInternetLookupAlwaysPrompt.Name = "cbManualInternetLookupAlwaysPrompt"
        Me.cbManualInternetLookupAlwaysPrompt.Size = New System.Drawing.Size(93, 17)
        Me.cbManualInternetLookupAlwaysPrompt.TabIndex = 47
        Me.cbManualInternetLookupAlwaysPrompt.Text = "always prompt"
        Me.cbManualInternetLookupAlwaysPrompt.UseVisualStyleBackColor = true
        '
        'chkManualDontAskInteractive
        '
        Me.chkManualDontAskInteractive.AutoSize = true
        Me.chkManualDontAskInteractive.Location = New System.Drawing.Point(465, 58)
        Me.chkManualDontAskInteractive.Name = "chkManualDontAskInteractive"
        Me.chkManualDontAskInteractive.Size = New System.Drawing.Size(78, 30)
        Me.chkManualDontAskInteractive.TabIndex = 6
        Me.chkManualDontAskInteractive.Text = "Don't ask,"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"if no match"
        Me.ToolTip1.SetToolTip(Me.chkManualDontAskInteractive, resources.GetString("chkManualDontAskInteractive.ToolTip"))
        Me.chkManualDontAskInteractive.UseVisualStyleBackColor = true
        '
        'txtManualInternetParserPathDisplay
        '
        Me.txtManualInternetParserPathDisplay.Enabled = false
        Me.txtManualInternetParserPathDisplay.Location = New System.Drawing.Point(107, 35)
        Me.txtManualInternetParserPathDisplay.Name = "txtManualInternetParserPathDisplay"
        Me.txtManualInternetParserPathDisplay.Size = New System.Drawing.Size(166, 20)
        Me.txtManualInternetParserPathDisplay.TabIndex = 1
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(342, 34)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(94, 21)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Grabber Options"
        Me.Button2.UseVisualStyleBackColor = true
        '
        'Label42
        '
        Me.Label42.AutoSize = true
        Me.Label42.Location = New System.Drawing.Point(8, 66)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(93, 13)
        Me.Label42.TabIndex = 4
        Me.Label42.Text = "Update Behaviour"
        '
        'btnManualSelectInternetParserPath
        '
        Me.btnManualSelectInternetParserPath.CausesValidation = false
        Me.btnManualSelectInternetParserPath.Location = New System.Drawing.Point(290, 35)
        Me.btnManualSelectInternetParserPath.Name = "btnManualSelectInternetParserPath"
        Me.btnManualSelectInternetParserPath.Size = New System.Drawing.Size(36, 20)
        Me.btnManualSelectInternetParserPath.TabIndex = 2
        Me.btnManualSelectInternetParserPath.Text = "..."
        Me.btnManualSelectInternetParserPath.UseVisualStyleBackColor = true
        '
        'cbManualInternetLookupBehaviour
        '
        Me.cbManualInternetLookupBehaviour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualInternetLookupBehaviour.FormattingEnabled = true
        Me.cbManualInternetLookupBehaviour.Items.AddRange(New Object() {"Auto match only", "Auto match & ask if no match", "Manual match (always ask)"})
        Me.cbManualInternetLookupBehaviour.Location = New System.Drawing.Point(107, 63)
        Me.cbManualInternetLookupBehaviour.Name = "cbManualInternetLookupBehaviour"
        Me.cbManualInternetLookupBehaviour.Size = New System.Drawing.Size(219, 21)
        Me.cbManualInternetLookupBehaviour.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cbManualInternetLookupBehaviour, resources.GetString("cbManualInternetLookupBehaviour.ToolTip"))
        '
        'txtManualInternetParserPath
        '
        Me.txtManualInternetParserPath.Location = New System.Drawing.Point(107, 35)
        Me.txtManualInternetParserPath.Name = "txtManualInternetParserPath"
        Me.txtManualInternetParserPath.Size = New System.Drawing.Size(219, 20)
        Me.txtManualInternetParserPath.TabIndex = 46
        Me.txtManualInternetParserPath.Visible = false
        '
        'Label41
        '
        Me.Label41.AutoSize = true
        Me.Label41.Location = New System.Drawing.Point(8, 38)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(75, 13)
        Me.Label41.TabIndex = 0
        Me.Label41.Text = "Grabber Script"
        '
        'btnShowHideLogTest
        '
        Me.btnShowHideLogTest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnShowHideLogTest.Location = New System.Drawing.Point(489, 547)
        Me.btnShowHideLogTest.Name = "btnShowHideLogTest"
        Me.btnShowHideLogTest.Size = New System.Drawing.Size(94, 22)
        Me.btnShowHideLogTest.TabIndex = 5
        Me.btnShowHideLogTest.Text = "Show Log >>"
        Me.btnShowHideLogTest.UseVisualStyleBackColor = true
        '
        'grpManualUpdatesParameters
        '
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbSkipExcludedMovieFiles)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterAndOr)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersValue2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersOperator2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.txtManualParameterValue2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterOperator2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersField2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterFieldList2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label21)
        Me.grpManualUpdatesParameters.Controls.Add(Me.chkManualParametersUpdateAll)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersValue1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label19)
        Me.grpManualUpdatesParameters.Controls.Add(Me.txtManualParameterValue1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterOperator1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label18)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterFieldList1)
        Me.grpManualUpdatesParameters.Location = New System.Drawing.Point(10, 236)
        Me.grpManualUpdatesParameters.Name = "grpManualUpdatesParameters"
        Me.grpManualUpdatesParameters.Size = New System.Drawing.Size(573, 129)
        Me.grpManualUpdatesParameters.TabIndex = 2
        Me.grpManualUpdatesParameters.TabStop = false
        Me.grpManualUpdatesParameters.Text = "Parameters ..."
        Me.grpManualUpdatesParameters.Visible = false
        '
        'cbSkipExcludedMovieFiles
        '
        Me.cbSkipExcludedMovieFiles.AutoSize = true
        Me.cbSkipExcludedMovieFiles.Location = New System.Drawing.Point(341, 102)
        Me.cbSkipExcludedMovieFiles.Name = "cbSkipExcludedMovieFiles"
        Me.cbSkipExcludedMovieFiles.Size = New System.Drawing.Size(202, 17)
        Me.cbSkipExcludedMovieFiles.TabIndex = 15
        Me.cbSkipExcludedMovieFiles.Text = "Skip Movies in 'Excluded Movies File'"
        Me.ToolTip1.SetToolTip(Me.cbSkipExcludedMovieFiles, "If checked, files contained in "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"'Excluded Movie Files' will be skipped.")
        Me.cbSkipExcludedMovieFiles.UseVisualStyleBackColor = true
        Me.cbSkipExcludedMovieFiles.Visible = false
        '
        'cbManualParameterAndOr
        '
        Me.cbManualParameterAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterAndOr.FormattingEnabled = true
        Me.cbManualParameterAndOr.Items.AddRange(New Object() {"and", "or", " "})
        Me.cbManualParameterAndOr.Location = New System.Drawing.Point(6, 49)
        Me.cbManualParameterAndOr.Name = "cbManualParameterAndOr"
        Me.cbManualParameterAndOr.Size = New System.Drawing.Size(53, 21)
        Me.cbManualParameterAndOr.TabIndex = 6
        '
        'lblManualParametersValue2
        '
        Me.lblManualParametersValue2.AutoSize = true
        Me.lblManualParametersValue2.Location = New System.Drawing.Point(339, 57)
        Me.lblManualParametersValue2.Name = "lblManualParametersValue2"
        Me.lblManualParametersValue2.Size = New System.Drawing.Size(34, 13)
        Me.lblManualParametersValue2.TabIndex = 11
        Me.lblManualParametersValue2.Text = "Value"
        Me.lblManualParametersValue2.Visible = false
        '
        'lblManualParametersOperator2
        '
        Me.lblManualParametersOperator2.AutoSize = true
        Me.lblManualParametersOperator2.Location = New System.Drawing.Point(219, 58)
        Me.lblManualParametersOperator2.Name = "lblManualParametersOperator2"
        Me.lblManualParametersOperator2.Size = New System.Drawing.Size(48, 13)
        Me.lblManualParametersOperator2.TabIndex = 9
        Me.lblManualParametersOperator2.Text = "Operator"
        Me.lblManualParametersOperator2.Visible = false
        '
        'txtManualParameterValue2
        '
        Me.txtManualParameterValue2.Location = New System.Drawing.Point(341, 75)
        Me.txtManualParameterValue2.Name = "txtManualParameterValue2"
        Me.txtManualParameterValue2.Size = New System.Drawing.Size(207, 20)
        Me.txtManualParameterValue2.TabIndex = 12
        Me.txtManualParameterValue2.Visible = false
        Me.txtManualParameterValue2.WordWrap = false
        '
        'cbManualParameterOperator2
        '
        Me.cbManualParameterOperator2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterOperator2.FormattingEnabled = true
        Me.cbManualParameterOperator2.Items.AddRange(New Object() {"=", "!=", "LIKE", "NOT LIKE", "EXISTS", "NOT EXISTS", ">", "<", ">Num", "<Num"})
        Me.cbManualParameterOperator2.Location = New System.Drawing.Point(215, 74)
        Me.cbManualParameterOperator2.Name = "cbManualParameterOperator2"
        Me.cbManualParameterOperator2.Size = New System.Drawing.Size(111, 21)
        Me.cbManualParameterOperator2.TabIndex = 10
        Me.cbManualParameterOperator2.Visible = false
        '
        'lblManualParametersField2
        '
        Me.lblManualParametersField2.AutoSize = true
        Me.lblManualParametersField2.Location = New System.Drawing.Point(68, 58)
        Me.lblManualParametersField2.Name = "lblManualParametersField2"
        Me.lblManualParametersField2.Size = New System.Drawing.Size(47, 13)
        Me.lblManualParametersField2.TabIndex = 7
        Me.lblManualParametersField2.Text = "DB Field"
        Me.lblManualParametersField2.Visible = false
        '
        'cbManualParameterFieldList2
        '
        Me.cbManualParameterFieldList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterFieldList2.FormattingEnabled = true
        Me.cbManualParameterFieldList2.Location = New System.Drawing.Point(68, 74)
        Me.cbManualParameterFieldList2.Name = "cbManualParameterFieldList2"
        Me.cbManualParameterFieldList2.Size = New System.Drawing.Size(132, 21)
        Me.cbManualParameterFieldList2.TabIndex = 8
        Me.cbManualParameterFieldList2.Visible = false
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(18, 102)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(25, 13)
        Me.Label21.TabIndex = 13
        Me.Label21.Text = "or..."
        '
        'chkManualParametersUpdateAll
        '
        Me.chkManualParametersUpdateAll.AutoSize = true
        Me.chkManualParametersUpdateAll.Location = New System.Drawing.Point(49, 101)
        Me.chkManualParametersUpdateAll.Name = "chkManualParametersUpdateAll"
        Me.chkManualParametersUpdateAll.Size = New System.Drawing.Size(115, 17)
        Me.chkManualParametersUpdateAll.TabIndex = 14
        Me.chkManualParametersUpdateAll.Text = "Update all records."
        Me.chkManualParametersUpdateAll.UseVisualStyleBackColor = true
        '
        'lblManualParametersValue1
        '
        Me.lblManualParametersValue1.AutoSize = true
        Me.lblManualParametersValue1.Location = New System.Drawing.Point(339, 15)
        Me.lblManualParametersValue1.Name = "lblManualParametersValue1"
        Me.lblManualParametersValue1.Size = New System.Drawing.Size(34, 13)
        Me.lblManualParametersValue1.TabIndex = 4
        Me.lblManualParametersValue1.Text = "Value"
        Me.lblManualParametersValue1.Visible = false
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(219, 16)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(48, 13)
        Me.Label19.TabIndex = 2
        Me.Label19.Text = "Operator"
        '
        'txtManualParameterValue1
        '
        Me.txtManualParameterValue1.Location = New System.Drawing.Point(341, 32)
        Me.txtManualParameterValue1.Name = "txtManualParameterValue1"
        Me.txtManualParameterValue1.Size = New System.Drawing.Size(207, 20)
        Me.txtManualParameterValue1.TabIndex = 5
        Me.txtManualParameterValue1.Visible = false
        '
        'cbManualParameterOperator1
        '
        Me.cbManualParameterOperator1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterOperator1.FormattingEnabled = true
        Me.cbManualParameterOperator1.Items.AddRange(New Object() {"=", "!=", "LIKE", "NOT LIKE", "EXISTS", "NOT EXISTS", ">", "<", ">Num", "<Num"})
        Me.cbManualParameterOperator1.Location = New System.Drawing.Point(215, 32)
        Me.cbManualParameterOperator1.Name = "cbManualParameterOperator1"
        Me.cbManualParameterOperator1.Size = New System.Drawing.Size(111, 21)
        Me.cbManualParameterOperator1.TabIndex = 3
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(68, 16)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(47, 13)
        Me.Label18.TabIndex = 0
        Me.Label18.Text = "DB Field"
        '
        'cbManualParameterFieldList1
        '
        Me.cbManualParameterFieldList1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterFieldList1.FormattingEnabled = true
        Me.cbManualParameterFieldList1.Location = New System.Drawing.Point(68, 32)
        Me.cbManualParameterFieldList1.Name = "cbManualParameterFieldList1"
        Me.cbManualParameterFieldList1.Size = New System.Drawing.Size(132, 21)
        Me.cbManualParameterFieldList1.TabIndex = 1
        '
        'GroupBox10
        '
        Me.GroupBox10.Controls.Add(Me.chkManualNfoFilesOnlyAddMissing)
        Me.GroupBox10.Controls.Add(Me.cbManualNfoFileHandling)
        Me.GroupBox10.Controls.Add(Me.lblManualNfoFileHandling)
        Me.GroupBox10.Controls.Add(Me.lblManualSelectFieldDestination)
        Me.GroupBox10.Controls.Add(Me.cbManualSelectFieldDestination)
        Me.GroupBox10.Controls.Add(Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData)
        Me.GroupBox10.Controls.Add(Me.chkManualUpdateRecordsOnlyMissingData)
        Me.GroupBox10.Controls.Add(Me.txtManualOldValue)
        Me.GroupBox10.Controls.Add(Me.lblManualEnterOldValue)
        Me.GroupBox10.Controls.Add(Me.chkManualMissingFanartDownload)
        Me.GroupBox10.Controls.Add(Me.lblManualDatabaseFieldsPrompt)
        Me.GroupBox10.Controls.Add(Me.lblManualSelectField)
        Me.GroupBox10.Controls.Add(Me.cbManualSelectField)
        Me.GroupBox10.Controls.Add(Me.txtManualNewValue)
        Me.GroupBox10.Controls.Add(Me.lblManualEnterNewValue)
        Me.GroupBox10.Controls.Add(Me.cbManualSelectOperation)
        Me.GroupBox10.Location = New System.Drawing.Point(10, 117)
        Me.GroupBox10.Name = "GroupBox10"
        Me.GroupBox10.Size = New System.Drawing.Size(573, 113)
        Me.GroupBox10.TabIndex = 1
        Me.GroupBox10.TabStop = false
        Me.GroupBox10.Text = "Operation ..."
        '
        'chkManualNfoFilesOnlyAddMissing
        '
        Me.chkManualNfoFilesOnlyAddMissing.AutoSize = true
        Me.chkManualNfoFilesOnlyAddMissing.Location = New System.Drawing.Point(49, 80)
        Me.chkManualNfoFilesOnlyAddMissing.Name = "chkManualNfoFilesOnlyAddMissing"
        Me.chkManualNfoFilesOnlyAddMissing.Size = New System.Drawing.Size(127, 17)
        Me.chkManualNfoFilesOnlyAddMissing.TabIndex = 7
        Me.chkManualNfoFilesOnlyAddMissing.Text = "Don't overrite existing"
        Me.chkManualNfoFilesOnlyAddMissing.UseVisualStyleBackColor = true
        Me.chkManualNfoFilesOnlyAddMissing.Visible = false
        '
        'cbManualNfoFileHandling
        '
        Me.cbManualNfoFileHandling.FormattingEnabled = true
        Me.cbManualNfoFileHandling.Items.AddRange(New Object() {"MyFilms.nfo", "Movie Name", "Both"})
        Me.cbManualNfoFileHandling.Location = New System.Drawing.Point(342, 19)
        Me.cbManualNfoFileHandling.Name = "cbManualNfoFileHandling"
        Me.cbManualNfoFileHandling.Size = New System.Drawing.Size(206, 21)
        Me.cbManualNfoFileHandling.TabIndex = 3
        Me.cbManualNfoFileHandling.Visible = false
        '
        'lblManualNfoFileHandling
        '
        Me.lblManualNfoFileHandling.AutoSize = true
        Me.lblManualNfoFileHandling.Location = New System.Drawing.Point(234, 24)
        Me.lblManualNfoFileHandling.Name = "lblManualNfoFileHandling"
        Me.lblManualNfoFileHandling.Size = New System.Drawing.Size(88, 13)
        Me.lblManualNfoFileHandling.TabIndex = 1
        Me.lblManualNfoFileHandling.Text = "NFO file handling"
        Me.lblManualNfoFileHandling.Visible = false
        '
        'lblManualSelectFieldDestination
        '
        Me.lblManualSelectFieldDestination.AutoSize = true
        Me.lblManualSelectFieldDestination.Location = New System.Drawing.Point(222, 71)
        Me.lblManualSelectFieldDestination.Name = "lblManualSelectFieldDestination"
        Me.lblManualSelectFieldDestination.Size = New System.Drawing.Size(103, 13)
        Me.lblManualSelectFieldDestination.TabIndex = 15
        Me.lblManualSelectFieldDestination.Text = "DB Field Destination"
        Me.lblManualSelectFieldDestination.Visible = false
        '
        'cbManualSelectFieldDestination
        '
        Me.cbManualSelectFieldDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualSelectFieldDestination.FormattingEnabled = true
        Me.cbManualSelectFieldDestination.Location = New System.Drawing.Point(342, 69)
        Me.cbManualSelectFieldDestination.Name = "cbManualSelectFieldDestination"
        Me.cbManualSelectFieldDestination.Size = New System.Drawing.Size(206, 21)
        Me.cbManualSelectFieldDestination.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.cbManualSelectFieldDestination, "Destination field for ""Copy Value"" operation")
        Me.cbManualSelectFieldDestination.Visible = false
        '
        'chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData
        '
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.AutoSize = true
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Location = New System.Drawing.Point(49, 90)
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Name = "chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData"
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Size = New System.Drawing.Size(181, 17)
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.TabIndex = 8
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Text = "Only update with non-empty data"
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.UseVisualStyleBackColor = true
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = false
        '
        'chkManualUpdateRecordsOnlyMissingData
        '
        Me.chkManualUpdateRecordsOnlyMissingData.AutoSize = true
        Me.chkManualUpdateRecordsOnlyMissingData.Location = New System.Drawing.Point(49, 73)
        Me.chkManualUpdateRecordsOnlyMissingData.Name = "chkManualUpdateRecordsOnlyMissingData"
        Me.chkManualUpdateRecordsOnlyMissingData.Size = New System.Drawing.Size(129, 17)
        Me.chkManualUpdateRecordsOnlyMissingData.TabIndex = 6
        Me.chkManualUpdateRecordsOnlyMissingData.Text = "Only add missing data"
        Me.ToolTip1.SetToolTip(Me.chkManualUpdateRecordsOnlyMissingData, "If checked, instead of owerwriting all fields selected on ""Database Fields"" tab w"& _ 
        "ith the new grabbed data,"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"only fields that are empty are filled with data.")
        Me.chkManualUpdateRecordsOnlyMissingData.UseVisualStyleBackColor = true
        Me.chkManualUpdateRecordsOnlyMissingData.Visible = false
        '
        'txtManualOldValue
        '
        Me.txtManualOldValue.Location = New System.Drawing.Point(342, 53)
        Me.txtManualOldValue.Name = "txtManualOldValue"
        Me.txtManualOldValue.Size = New System.Drawing.Size(206, 20)
        Me.txtManualOldValue.TabIndex = 14
        Me.txtManualOldValue.Visible = false
        '
        'lblManualEnterOldValue
        '
        Me.lblManualEnterOldValue.AutoSize = true
        Me.lblManualEnterOldValue.Location = New System.Drawing.Point(251, 56)
        Me.lblManualEnterOldValue.Name = "lblManualEnterOldValue"
        Me.lblManualEnterOldValue.Size = New System.Drawing.Size(53, 13)
        Me.lblManualEnterOldValue.TabIndex = 13
        Me.lblManualEnterOldValue.Text = "Old Value"
        Me.lblManualEnterOldValue.Visible = false
        '
        'chkManualMissingFanartDownload
        '
        Me.chkManualMissingFanartDownload.AutoSize = true
        Me.chkManualMissingFanartDownload.Checked = true
        Me.chkManualMissingFanartDownload.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkManualMissingFanartDownload.Location = New System.Drawing.Point(279, 23)
        Me.chkManualMissingFanartDownload.Name = "chkManualMissingFanartDownload"
        Me.chkManualMissingFanartDownload.Size = New System.Drawing.Size(218, 17)
        Me.chkManualMissingFanartDownload.TabIndex = 2
        Me.chkManualMissingFanartDownload.Text = "Download only for Movies missing Fanart"
        Me.chkManualMissingFanartDownload.UseVisualStyleBackColor = true
        Me.chkManualMissingFanartDownload.Visible = false
        '
        'lblManualDatabaseFieldsPrompt
        '
        Me.lblManualDatabaseFieldsPrompt.AutoSize = true
        Me.lblManualDatabaseFieldsPrompt.Location = New System.Drawing.Point(5, 44)
        Me.lblManualDatabaseFieldsPrompt.Name = "lblManualDatabaseFieldsPrompt"
        Me.lblManualDatabaseFieldsPrompt.Size = New System.Drawing.Size(205, 26)
        Me.lblManualDatabaseFieldsPrompt.TabIndex = 4
        Me.lblManualDatabaseFieldsPrompt.Text = "Note : Ensure the fields you want updated"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"are selected on the Database Fields ta"& _ 
    "b."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)
        Me.lblManualDatabaseFieldsPrompt.Visible = false
        '
        'lblManualSelectField
        '
        Me.lblManualSelectField.AutoSize = true
        Me.lblManualSelectField.Location = New System.Drawing.Point(278, 24)
        Me.lblManualSelectField.Name = "lblManualSelectField"
        Me.lblManualSelectField.Size = New System.Drawing.Size(47, 13)
        Me.lblManualSelectField.TabIndex = 4
        Me.lblManualSelectField.Text = "DB Field"
        Me.lblManualSelectField.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblManualSelectField.Visible = false
        '
        'cbManualSelectField
        '
        Me.cbManualSelectField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualSelectField.FormattingEnabled = true
        Me.cbManualSelectField.Location = New System.Drawing.Point(342, 19)
        Me.cbManualSelectField.Name = "cbManualSelectField"
        Me.cbManualSelectField.Size = New System.Drawing.Size(206, 21)
        Me.cbManualSelectField.TabIndex = 1
        Me.cbManualSelectField.Visible = false
        '
        'txtManualNewValue
        '
        Me.txtManualNewValue.Location = New System.Drawing.Point(342, 81)
        Me.txtManualNewValue.Name = "txtManualNewValue"
        Me.txtManualNewValue.Size = New System.Drawing.Size(206, 20)
        Me.txtManualNewValue.TabIndex = 2
        Me.txtManualNewValue.Visible = false
        '
        'lblManualEnterNewValue
        '
        Me.lblManualEnterNewValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lblManualEnterNewValue.AutoSize = true
        Me.lblManualEnterNewValue.Location = New System.Drawing.Point(252, 84)
        Me.lblManualEnterNewValue.Name = "lblManualEnterNewValue"
        Me.lblManualEnterNewValue.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblManualEnterNewValue.Size = New System.Drawing.Size(65, 13)
        Me.lblManualEnterNewValue.TabIndex = 1
        Me.lblManualEnterNewValue.Text = "New Value :"
        Me.lblManualEnterNewValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblManualEnterNewValue.Visible = false
        '
        'cbManualSelectOperation
        '
        Me.cbManualSelectOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualSelectOperation.FormattingEnabled = true
        Me.cbManualSelectOperation.Items.AddRange(New Object() {"Update Value", "Update Value - Replace String", "Update Value - Add String", "Update Value - Insert String", "Update Value - Remove String", "Delete Value", "Copy Value", "Update Record", "Delete Record", "Download Fanart", "Update NFO File", "Delete NFO File", "Add IMDB to File"})
        Me.cbManualSelectOperation.Location = New System.Drawing.Point(8, 19)
        Me.cbManualSelectOperation.Name = "cbManualSelectOperation"
        Me.cbManualSelectOperation.Size = New System.Drawing.Size(195, 21)
        Me.cbManualSelectOperation.TabIndex = 0
        '
        'DatabaseFields
        '
        Me.DatabaseFields.Controls.Add(Me.GroupBox_FanartHandling)
        Me.DatabaseFields.Controls.Add(Me.GroupBox_TitleHandling)
        Me.DatabaseFields.Controls.Add(Me.GroupBox_StorageFieldHandling)
        Me.DatabaseFields.Controls.Add(Me.GroupBox_MediaLabelFieldHandling)
        Me.DatabaseFields.Controls.Add(Me.GroupBox_OtherFieldHandling)
        Me.DatabaseFields.Controls.Add(Me.GroupBox_PictureHandling)
        Me.DatabaseFields.Controls.Add(Me.lblInternetLookupRequired)
        Me.DatabaseFields.Controls.Add(Me.GroupBox16)
        Me.DatabaseFields.Controls.Add(Me.cbDatabaseFields)
        Me.DatabaseFields.Controls.Add(Me.Label34)
        Me.DatabaseFields.Location = New System.Drawing.Point(4, 22)
        Me.DatabaseFields.Name = "DatabaseFields"
        Me.DatabaseFields.Size = New System.Drawing.Size(600, 580)
        Me.DatabaseFields.TabIndex = 4
        Me.DatabaseFields.Text = "Database Fields"
        Me.DatabaseFields.UseVisualStyleBackColor = true
        '
        'GroupBox_FanartHandling
        '
        Me.GroupBox_FanartHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_FanartHandling.Controls.Add(Me.chkLoadPersonImagesWithFanart)
        Me.GroupBox_FanartHandling.Controls.Add(Me.chkUseGrabberForFanart)
        Me.GroupBox_FanartHandling.Controls.Add(Me.cbFanartLimitResolutionMin)
        Me.GroupBox_FanartHandling.Controls.Add(Me.lblFanartLimits)
        Me.GroupBox_FanartHandling.Controls.Add(Me.cbFanartLimitResolutionMax)
        Me.GroupBox_FanartHandling.Controls.Add(Me.cbNumFanartLimitNumber)
        Me.GroupBox_FanartHandling.Location = New System.Drawing.Point(239, 515)
        Me.GroupBox_FanartHandling.Name = "GroupBox_FanartHandling"
        Me.GroupBox_FanartHandling.Size = New System.Drawing.Size(353, 62)
        Me.GroupBox_FanartHandling.TabIndex = 8
        Me.GroupBox_FanartHandling.TabStop = false
        Me.GroupBox_FanartHandling.Text = "Fanart Handling ..."
        '
        'chkLoadPersonImagesWithFanart
        '
        Me.chkLoadPersonImagesWithFanart.AutoSize = true
        Me.chkLoadPersonImagesWithFanart.Location = New System.Drawing.Point(21, 37)
        Me.chkLoadPersonImagesWithFanart.Name = "chkLoadPersonImagesWithFanart"
        Me.chkLoadPersonImagesWithFanart.Size = New System.Drawing.Size(123, 17)
        Me.chkLoadPersonImagesWithFanart.TabIndex = 1
        Me.chkLoadPersonImagesWithFanart.Text = "Load Person Images"
        Me.ToolTip1.SetToolTip(Me.chkLoadPersonImagesWithFanart, resources.GetString("chkLoadPersonImagesWithFanart.ToolTip"))
        Me.chkLoadPersonImagesWithFanart.UseVisualStyleBackColor = true
        '
        'chkUseGrabberForFanart
        '
        Me.chkUseGrabberForFanart.AutoSize = true
        Me.chkUseGrabberForFanart.Location = New System.Drawing.Point(21, 17)
        Me.chkUseGrabberForFanart.Name = "chkUseGrabberForFanart"
        Me.chkUseGrabberForFanart.Size = New System.Drawing.Size(121, 17)
        Me.chkUseGrabberForFanart.TabIndex = 0
        Me.chkUseGrabberForFanart.Text = "Use script for Fanart"
        Me.ToolTip1.SetToolTip(Me.chkUseGrabberForFanart, resources.GetString("chkUseGrabberForFanart.ToolTip"))
        Me.chkUseGrabberForFanart.UseVisualStyleBackColor = true
        '
        'cbFanartLimitResolutionMin
        '
        Me.cbFanartLimitResolutionMin.FormattingEnabled = true
        Me.cbFanartLimitResolutionMin.Location = New System.Drawing.Point(164, 34)
        Me.cbFanartLimitResolutionMin.Name = "cbFanartLimitResolutionMin"
        Me.cbFanartLimitResolutionMin.Size = New System.Drawing.Size(78, 21)
        Me.cbFanartLimitResolutionMin.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.cbFanartLimitResolutionMin, "Sets minimum resolution for fanart to be allowed to download.")
        '
        'lblFanartLimits
        '
        Me.lblFanartLimits.AutoSize = true
        Me.lblFanartLimits.Location = New System.Drawing.Point(166, 13)
        Me.lblFanartLimits.Name = "lblFanartLimits"
        Me.lblFanartLimits.Size = New System.Drawing.Size(66, 13)
        Me.lblFanartLimits.TabIndex = 2
        Me.lblFanartLimits.Text = "Fanart Limits"
        '
        'cbFanartLimitResolutionMax
        '
        Me.cbFanartLimitResolutionMax.FormattingEnabled = true
        Me.cbFanartLimitResolutionMax.Location = New System.Drawing.Point(248, 34)
        Me.cbFanartLimitResolutionMax.Name = "cbFanartLimitResolutionMax"
        Me.cbFanartLimitResolutionMax.Size = New System.Drawing.Size(90, 21)
        Me.cbFanartLimitResolutionMax.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cbFanartLimitResolutionMax, "Sets maximum resolution for fanart to be allowed to download.")
        '
        'cbNumFanartLimitNumber
        '
        Me.cbNumFanartLimitNumber.Location = New System.Drawing.Point(248, 11)
        Me.cbNumFanartLimitNumber.Name = "cbNumFanartLimitNumber"
        Me.cbNumFanartLimitNumber.Size = New System.Drawing.Size(44, 20)
        Me.cbNumFanartLimitNumber.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.cbNumFanartLimitNumber, "Limits the number of downloaded fanarts."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"""0"" = all available.")
        '
        'GroupBox_TitleHandling
        '
        Me.GroupBox_TitleHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_TitleHandling.Controls.Add(Me.Label101)
        Me.GroupBox_TitleHandling.Controls.Add(Me.cbEditionNameAppliesTo)
        Me.GroupBox_TitleHandling.Controls.Add(Me.txtSeriesNameIdentifier)
        Me.GroupBox_TitleHandling.Controls.Add(Me.Label106)
        Me.GroupBox_TitleHandling.Controls.Add(Me.Label89)
        Me.GroupBox_TitleHandling.Controls.Add(Me.txtGroupNameIdentifier)
        Me.GroupBox_TitleHandling.Controls.Add(Me.cbGroupNameAppliesTo)
        Me.GroupBox_TitleHandling.Controls.Add(Me.chkFolderNameIsGroupName)
        Me.GroupBox_TitleHandling.Controls.Add(Me.lblGroupNameAppliesTo)
        Me.GroupBox_TitleHandling.Controls.Add(Me.cbMasterTitle)
        Me.GroupBox_TitleHandling.Controls.Add(Me.lblMasterTitle)
        Me.GroupBox_TitleHandling.Location = New System.Drawing.Point(239, 148)
        Me.GroupBox_TitleHandling.Name = "GroupBox_TitleHandling"
        Me.GroupBox_TitleHandling.Size = New System.Drawing.Size(353, 136)
        Me.GroupBox_TitleHandling.TabIndex = 4
        Me.GroupBox_TitleHandling.TabStop = false
        Me.GroupBox_TitleHandling.Text = "Title Handling ..."
        '
        'Label101
        '
        Me.Label101.AutoSize = true
        Me.Label101.Location = New System.Drawing.Point(170, 38)
        Me.Label101.Name = "Label101"
        Me.Label101.Size = New System.Drawing.Size(36, 13)
        Me.Label101.TabIndex = 3
        Me.Label101.Text = "Series"
        '
        'cbEditionNameAppliesTo
        '
        Me.cbEditionNameAppliesTo.FormattingEnabled = true
        Me.cbEditionNameAppliesTo.Items.AddRange(New Object() {"", "Original Title", "Translated Title", "Both Titles"})
        Me.cbEditionNameAppliesTo.Location = New System.Drawing.Point(164, 84)
        Me.cbEditionNameAppliesTo.Name = "cbEditionNameAppliesTo"
        Me.cbEditionNameAppliesTo.Size = New System.Drawing.Size(174, 21)
        Me.cbEditionNameAppliesTo.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.cbEditionNameAppliesTo, resources.GetString("cbEditionNameAppliesTo.ToolTip"))
        '
        'txtSeriesNameIdentifier
        '
        Me.txtSeriesNameIdentifier.Location = New System.Drawing.Point(229, 35)
        Me.txtSeriesNameIdentifier.Name = "txtSeriesNameIdentifier"
        Me.txtSeriesNameIdentifier.Size = New System.Drawing.Size(109, 20)
        Me.txtSeriesNameIdentifier.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtSeriesNameIdentifier, resources.GetString("txtSeriesNameIdentifier.ToolTip"))
        '
        'Label106
        '
        Me.Label106.AutoSize = true
        Me.Label106.Location = New System.Drawing.Point(18, 87)
        Me.Label106.Name = "Label106"
        Me.Label106.Size = New System.Drawing.Size(127, 13)
        Me.Label106.TabIndex = 7
        Me.Label106.Text = "Add Edition Name(s) to ..."
        '
        'Label89
        '
        Me.Label89.AutoSize = true
        Me.Label89.Location = New System.Drawing.Point(170, 14)
        Me.Label89.Name = "Label89"
        Me.Label89.Size = New System.Drawing.Size(36, 13)
        Me.Label89.TabIndex = 1
        Me.Label89.Text = "Group"
        '
        'txtGroupNameIdentifier
        '
        Me.txtGroupNameIdentifier.Location = New System.Drawing.Point(229, 11)
        Me.txtGroupNameIdentifier.Name = "txtGroupNameIdentifier"
        Me.txtGroupNameIdentifier.Size = New System.Drawing.Size(109, 20)
        Me.txtGroupNameIdentifier.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtGroupNameIdentifier, resources.GetString("txtGroupNameIdentifier.ToolTip"))
        '
        'cbGroupNameAppliesTo
        '
        Me.cbGroupNameAppliesTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbGroupNameAppliesTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbGroupNameAppliesTo.FormattingEnabled = true
        Me.cbGroupNameAppliesTo.Items.AddRange(New Object() {"Original Title", "Translated Title", "Both Titles"})
        Me.cbGroupNameAppliesTo.Location = New System.Drawing.Point(164, 59)
        Me.cbGroupNameAppliesTo.Name = "cbGroupNameAppliesTo"
        Me.cbGroupNameAppliesTo.Size = New System.Drawing.Size(174, 21)
        Me.cbGroupNameAppliesTo.TabIndex = 6
        '
        'chkFolderNameIsGroupName
        '
        Me.chkFolderNameIsGroupName.AutoSize = true
        Me.chkFolderNameIsGroupName.Location = New System.Drawing.Point(21, 17)
        Me.chkFolderNameIsGroupName.Name = "chkFolderNameIsGroupName"
        Me.chkFolderNameIsGroupName.Size = New System.Drawing.Size(112, 30)
        Me.chkFolderNameIsGroupName.TabIndex = 0
        Me.chkFolderNameIsGroupName.Text = "Folder Name "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"as Group Identifier"
        Me.ToolTip1.SetToolTip(Me.chkFolderNameIsGroupName, resources.GetString("chkFolderNameIsGroupName.ToolTip"))
        Me.chkFolderNameIsGroupName.UseVisualStyleBackColor = true
        '
        'lblGroupNameAppliesTo
        '
        Me.lblGroupNameAppliesTo.AutoSize = true
        Me.lblGroupNameAppliesTo.Location = New System.Drawing.Point(18, 62)
        Me.lblGroupNameAppliesTo.Name = "lblGroupNameAppliesTo"
        Me.lblGroupNameAppliesTo.Size = New System.Drawing.Size(113, 13)
        Me.lblGroupNameAppliesTo.TabIndex = 5
        Me.lblGroupNameAppliesTo.Text = "Add Group Name to ..."
        '
        'cbMasterTitle
        '
        Me.cbMasterTitle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbMasterTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMasterTitle.FormattingEnabled = true
        Me.cbMasterTitle.Items.AddRange(New Object() {"TranslatedTitle", "OriginalTitle", "FormattedTitle"})
        Me.cbMasterTitle.Location = New System.Drawing.Point(164, 109)
        Me.cbMasterTitle.Name = "cbMasterTitle"
        Me.cbMasterTitle.Size = New System.Drawing.Size(174, 21)
        Me.cbMasterTitle.TabIndex = 10
        Me.ToolTip1.SetToolTip(Me.cbMasterTitle, "Select the title that is used for internet data search."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"It is used for both inte"& _ 
        "rnet data (e.g. update records) and"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"fanart search and creation.")
        '
        'lblMasterTitle
        '
        Me.lblMasterTitle.AutoSize = true
        Me.lblMasterTitle.Location = New System.Drawing.Point(18, 112)
        Me.lblMasterTitle.Name = "lblMasterTitle"
        Me.lblMasterTitle.Size = New System.Drawing.Size(62, 13)
        Me.lblMasterTitle.TabIndex = 9
        Me.lblMasterTitle.Text = "Master Title"
        '
        'GroupBox_StorageFieldHandling
        '
        Me.GroupBox_StorageFieldHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_StorageFieldHandling.Controls.Add(Me.chkShortNames)
        Me.GroupBox_StorageFieldHandling.Controls.Add(Me.Label26)
        Me.GroupBox_StorageFieldHandling.Controls.Add(Me.txtDefaultSourceField)
        Me.GroupBox_StorageFieldHandling.Location = New System.Drawing.Point(239, 102)
        Me.GroupBox_StorageFieldHandling.Name = "GroupBox_StorageFieldHandling"
        Me.GroupBox_StorageFieldHandling.Size = New System.Drawing.Size(353, 43)
        Me.GroupBox_StorageFieldHandling.TabIndex = 3
        Me.GroupBox_StorageFieldHandling.TabStop = false
        Me.GroupBox_StorageFieldHandling.Text = "Storage Field Handling ..."
        '
        'chkShortNames
        '
        Me.chkShortNames.AutoSize = true
        Me.chkShortNames.Location = New System.Drawing.Point(21, 17)
        Me.chkShortNames.Name = "chkShortNames"
        Me.chkShortNames.Size = New System.Drawing.Size(78, 17)
        Me.chkShortNames.TabIndex = 0
        Me.chkShortNames.Text = "Name Only"
        Me.ToolTip1.SetToolTip(Me.chkShortNames, "If checked, only file name will be stored to your DB "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"without path information."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"You have to make sure, your MyFilms settings support searching"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"for matching fi"& _ 
        "les in your setup.")
        Me.chkShortNames.UseVisualStyleBackColor = true
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(158, 12)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(65, 26)
        Me.Label26.TabIndex = 1
        Me.Label26.Text = "Item for File "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Info Storage"
        '
        'txtDefaultSourceField
        '
        Me.txtDefaultSourceField.Location = New System.Drawing.Point(229, 14)
        Me.txtDefaultSourceField.Name = "txtDefaultSourceField"
        Me.txtDefaultSourceField.Size = New System.Drawing.Size(109, 20)
        Me.txtDefaultSourceField.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtDefaultSourceField, "This is the Ant Movie Catalog field which references the location of your moviel "& _ 
        "files."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"The default is 'Source' - do not change this unless you are sure you nee"& _ 
        "d to!")
        '
        'GroupBox_MediaLabelFieldHandling
        '
        Me.GroupBox_MediaLabelFieldHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_MediaLabelFieldHandling.Controls.Add(Me.Label13)
        Me.GroupBox_MediaLabelFieldHandling.Controls.Add(Me.chkReadDVDLabel)
        Me.GroupBox_MediaLabelFieldHandling.Controls.Add(Me.txtMediaLabel)
        Me.GroupBox_MediaLabelFieldHandling.Location = New System.Drawing.Point(239, 287)
        Me.GroupBox_MediaLabelFieldHandling.Name = "GroupBox_MediaLabelFieldHandling"
        Me.GroupBox_MediaLabelFieldHandling.Size = New System.Drawing.Size(353, 59)
        Me.GroupBox_MediaLabelFieldHandling.TabIndex = 5
        Me.GroupBox_MediaLabelFieldHandling.TabStop = false
        Me.GroupBox_MediaLabelFieldHandling.Text = "Media Label Field Handling ..."
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(18, 16)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(65, 13)
        Me.Label13.TabIndex = 0
        Me.Label13.Text = "Media Label"
        '
        'chkReadDVDLabel
        '
        Me.chkReadDVDLabel.AutoSize = true
        Me.chkReadDVDLabel.Location = New System.Drawing.Point(21, 37)
        Me.chkReadDVDLabel.Name = "chkReadDVDLabel"
        Me.chkReadDVDLabel.Size = New System.Drawing.Size(327, 17)
        Me.chkReadDVDLabel.TabIndex = 2
        Me.chkReadDVDLabel.Text = "Try to read DVD Disk Label, Volume Name or UNC server name"
        Me.ToolTip1.SetToolTip(Me.chkReadDVDLabel, resources.GetString("chkReadDVDLabel.ToolTip"))
        Me.chkReadDVDLabel.UseVisualStyleBackColor = true
        '
        'txtMediaLabel
        '
        Me.txtMediaLabel.Location = New System.Drawing.Point(164, 13)
        Me.txtMediaLabel.Name = "txtMediaLabel"
        Me.txtMediaLabel.Size = New System.Drawing.Size(174, 20)
        Me.txtMediaLabel.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtMediaLabel, "Sets a value to be stored in the Ant Movie Database under the 'Media Label' field"& _ 
        ".")
        '
        'GroupBox_OtherFieldHandling
        '
        Me.GroupBox_OtherFieldHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.chkUseInternetDataForLanguagesField)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.Label12)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.txtMediaType)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.lblCheckFieldHandling)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.Label30)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.cbDateHandling)
        Me.GroupBox_OtherFieldHandling.Controls.Add(Me.cbCheckHandling)
        Me.GroupBox_OtherFieldHandling.Location = New System.Drawing.Point(239, 349)
        Me.GroupBox_OtherFieldHandling.Name = "GroupBox_OtherFieldHandling"
        Me.GroupBox_OtherFieldHandling.Size = New System.Drawing.Size(353, 93)
        Me.GroupBox_OtherFieldHandling.TabIndex = 6
        Me.GroupBox_OtherFieldHandling.TabStop = false
        Me.GroupBox_OtherFieldHandling.Text = "Other Field Handling Options ..."
        '
        'chkUseInternetDataForLanguagesField
        '
        Me.chkUseInternetDataForLanguagesField.AutoSize = true
        Me.chkUseInternetDataForLanguagesField.Location = New System.Drawing.Point(21, 58)
        Me.chkUseInternetDataForLanguagesField.Name = "chkUseInternetDataForLanguagesField"
        Me.chkUseInternetDataForLanguagesField.Size = New System.Drawing.Size(119, 30)
        Me.chkUseInternetDataForLanguagesField.TabIndex = 4
        Me.chkUseInternetDataForLanguagesField.Text = "Use Internet Data "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"for Languages Field"
        Me.ToolTip1.SetToolTip(Me.chkUseInternetDataForLanguagesField, "If checked, the internet data from the grabber will be used to populate the ""Lang"& _ 
        "uages"" field."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"If unchecked, mediainfo audio stream languages list will be used."& _ 
        "")
        Me.chkUseInternetDataForLanguagesField.UseVisualStyleBackColor = true
        '
        'Label12
        '
        Me.Label12.AutoSize = true
        Me.Label12.Location = New System.Drawing.Point(170, 68)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(63, 13)
        Me.Label12.TabIndex = 5
        Me.Label12.Text = "Media Type"
        Me.ToolTip1.SetToolTip(Me.Label12, "You can enter a value that will be imported "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"into the ""MediaType"" field."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"This a"& _ 
        "cts as a ""preset"".")
        '
        'txtMediaType
        '
        Me.txtMediaType.Location = New System.Drawing.Point(248, 65)
        Me.txtMediaType.Name = "txtMediaType"
        Me.txtMediaType.Size = New System.Drawing.Size(90, 20)
        Me.txtMediaType.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtMediaType, "Sets a value to be stored in the Ant Movie Database under the 'Media Type' field."& _ 
        "")
        '
        'lblCheckFieldHandling
        '
        Me.lblCheckFieldHandling.AutoSize = true
        Me.lblCheckFieldHandling.Location = New System.Drawing.Point(18, 42)
        Me.lblCheckFieldHandling.Name = "lblCheckFieldHandling"
        Me.lblCheckFieldHandling.Size = New System.Drawing.Size(120, 13)
        Me.lblCheckFieldHandling.TabIndex = 2
        Me.lblCheckFieldHandling.Text = "Checked Field Handling"
        '
        'Label30
        '
        Me.Label30.AutoSize = true
        Me.Label30.Location = New System.Drawing.Point(18, 17)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(100, 13)
        Me.Label30.TabIndex = 0
        Me.Label30.Text = "Date Field Handling"
        '
        'cbDateHandling
        '
        Me.cbDateHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbDateHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDateHandling.FormattingEnabled = true
        Me.cbDateHandling.Items.AddRange(New Object() {"File Created Date", "File Modified Date", "Current System Date", "No Date"})
        Me.cbDateHandling.Location = New System.Drawing.Point(164, 14)
        Me.cbDateHandling.Name = "cbDateHandling"
        Me.cbDateHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbDateHandling.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cbDateHandling, "When using ""file creation date"", the result is depending on your setting "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"for ""T"& _ 
        "itle Search Handling"" - it will be the file date or "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"the directory date, depend"& _ 
        "ing on your choice.")
        '
        'cbCheckHandling
        '
        Me.cbCheckHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbCheckHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbCheckHandling.FormattingEnabled = true
        Me.cbCheckHandling.Items.AddRange(New Object() {"Checked", "Unchecked"})
        Me.cbCheckHandling.Location = New System.Drawing.Point(164, 39)
        Me.cbCheckHandling.Name = "cbCheckHandling"
        Me.cbCheckHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbCheckHandling.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.cbCheckHandling, "The Ant Movie Catalog database includes a boolean 'Checked' field."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"If you select"& _ 
        " this option in the Database Fields list you can choose "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"whether to set it to '"& _ 
        "True' or 'False'.")
        '
        'GroupBox_PictureHandling
        '
        Me.GroupBox_PictureHandling.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox_PictureHandling.Controls.Add(Me.txtPictureFilenameSuffix)
        Me.GroupBox_PictureHandling.Controls.Add(Me.lblPicturePrefix)
        Me.GroupBox_PictureHandling.Controls.Add(Me.lblPictureHandling)
        Me.GroupBox_PictureHandling.Controls.Add(Me.cbPictureHandling)
        Me.GroupBox_PictureHandling.Controls.Add(Me.txtPictureFilenamePrefix)
        Me.GroupBox_PictureHandling.Location = New System.Drawing.Point(239, 445)
        Me.GroupBox_PictureHandling.Name = "GroupBox_PictureHandling"
        Me.GroupBox_PictureHandling.Size = New System.Drawing.Size(353, 67)
        Me.GroupBox_PictureHandling.TabIndex = 7
        Me.GroupBox_PictureHandling.TabStop = false
        Me.GroupBox_PictureHandling.Text = "Cover Handling ..."
        '
        'txtPictureFilenameSuffix
        '
        Me.txtPictureFilenameSuffix.Location = New System.Drawing.Point(248, 40)
        Me.txtPictureFilenameSuffix.Name = "txtPictureFilenameSuffix"
        Me.txtPictureFilenameSuffix.Size = New System.Drawing.Size(90, 20)
        Me.txtPictureFilenameSuffix.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtPictureFilenameSuffix, resources.GetString("txtPictureFilenameSuffix.ToolTip"))
        '
        'lblPicturePrefix
        '
        Me.lblPicturePrefix.AutoSize = true
        Me.lblPicturePrefix.Location = New System.Drawing.Point(18, 42)
        Me.lblPicturePrefix.Name = "lblPicturePrefix"
        Me.lblPicturePrefix.Size = New System.Drawing.Size(133, 13)
        Me.lblPicturePrefix.TabIndex = 2
        Me.lblPicturePrefix.Text = "Prefix/Suffix to Covername"
        '
        'lblPictureHandling
        '
        Me.lblPictureHandling.AutoSize = true
        Me.lblPictureHandling.Location = New System.Drawing.Point(18, 17)
        Me.lblPictureHandling.Name = "lblPictureHandling"
        Me.lblPictureHandling.Size = New System.Drawing.Size(85, 13)
        Me.lblPictureHandling.TabIndex = 0
        Me.lblPictureHandling.Text = "Picture Handling"
        '
        'cbPictureHandling
        '
        Me.cbPictureHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbPictureHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPictureHandling.FormattingEnabled = true
        Me.cbPictureHandling.Items.AddRange(New Object() {"Full Path", "Full Path & Create Moviethumb", "Relative Path", "Relative Path & Create Moviethumb", "Use Folder.jpg", "Create Moviethumb"})
        Me.cbPictureHandling.Location = New System.Drawing.Point(164, 13)
        Me.cbPictureHandling.Name = "cbPictureHandling"
        Me.cbPictureHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbPictureHandling.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cbPictureHandling, resources.GetString("cbPictureHandling.ToolTip"))
        '
        'txtPictureFilenamePrefix
        '
        Me.txtPictureFilenamePrefix.Location = New System.Drawing.Point(164, 40)
        Me.txtPictureFilenamePrefix.Name = "txtPictureFilenamePrefix"
        Me.txtPictureFilenamePrefix.Size = New System.Drawing.Size(78, 20)
        Me.txtPictureFilenamePrefix.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtPictureFilenamePrefix, resources.GetString("txtPictureFilenamePrefix.ToolTip"))
        '
        'lblInternetLookupRequired
        '
        Me.lblInternetLookupRequired.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.lblInternetLookupRequired.AutoSize = true
        Me.lblInternetLookupRequired.ForeColor = System.Drawing.Color.Red
        Me.lblInternetLookupRequired.Location = New System.Drawing.Point(8, 557)
        Me.lblInternetLookupRequired.Name = "lblInternetLookupRequired"
        Me.lblInternetLookupRequired.Size = New System.Drawing.Size(227, 13)
        Me.lblInternetLookupRequired.TabIndex = 9
        Me.lblInternetLookupRequired.Text = "-- Internet lookup required with these settings --"
        Me.lblInternetLookupRequired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox16
        '
        Me.GroupBox16.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox16.Controls.Add(Me.Label33)
        Me.GroupBox16.Controls.Add(Me.Label32)
        Me.GroupBox16.Controls.Add(Me.Label31)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNoMedia)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNoInternet)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNone)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAllMedia)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAllInternet)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAll)
        Me.GroupBox16.Location = New System.Drawing.Point(239, 7)
        Me.GroupBox16.Name = "GroupBox16"
        Me.GroupBox16.Size = New System.Drawing.Size(353, 93)
        Me.GroupBox16.TabIndex = 2
        Me.GroupBox16.TabStop = false
        Me.GroupBox16.Text = "Quick Select ..."
        '
        'Label33
        '
        Me.Label33.AutoSize = true
        Me.Label33.Location = New System.Drawing.Point(245, 16)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(93, 13)
        Me.Label33.TabIndex = 6
        Me.Label33.Text = "Only Internet Data"
        '
        'Label32
        '
        Me.Label32.AutoSize = true
        Me.Label32.Location = New System.Drawing.Point(138, 16)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(86, 13)
        Me.Label32.TabIndex = 3
        Me.Label32.Text = "Only Media Data"
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(44, 16)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(45, 13)
        Me.Label31.TabIndex = 0
        Me.Label31.Text = "All items"
        '
        'btnDBFieldsSelectNoMedia
        '
        Me.btnDBFieldsSelectNoMedia.Location = New System.Drawing.Point(141, 61)
        Me.btnDBFieldsSelectNoMedia.Name = "btnDBFieldsSelectNoMedia"
        Me.btnDBFieldsSelectNoMedia.Size = New System.Drawing.Size(89, 23)
        Me.btnDBFieldsSelectNoMedia.TabIndex = 5
        Me.btnDBFieldsSelectNoMedia.Text = "Select None"
        Me.btnDBFieldsSelectNoMedia.UseVisualStyleBackColor = true
        '
        'btnDBFieldsSelectNoInternet
        '
        Me.btnDBFieldsSelectNoInternet.Location = New System.Drawing.Point(248, 61)
        Me.btnDBFieldsSelectNoInternet.Name = "btnDBFieldsSelectNoInternet"
        Me.btnDBFieldsSelectNoInternet.Size = New System.Drawing.Size(90, 23)
        Me.btnDBFieldsSelectNoInternet.TabIndex = 8
        Me.btnDBFieldsSelectNoInternet.Text = "Select None"
        Me.btnDBFieldsSelectNoInternet.UseVisualStyleBackColor = true
        '
        'btnDBFieldsSelectNone
        '
        Me.btnDBFieldsSelectNone.Location = New System.Drawing.Point(21, 61)
        Me.btnDBFieldsSelectNone.Name = "btnDBFieldsSelectNone"
        Me.btnDBFieldsSelectNone.Size = New System.Drawing.Size(89, 23)
        Me.btnDBFieldsSelectNone.TabIndex = 2
        Me.btnDBFieldsSelectNone.Text = "Select None"
        Me.btnDBFieldsSelectNone.UseVisualStyleBackColor = true
        '
        'btnDBFieldsSelectAllMedia
        '
        Me.btnDBFieldsSelectAllMedia.Location = New System.Drawing.Point(141, 32)
        Me.btnDBFieldsSelectAllMedia.Name = "btnDBFieldsSelectAllMedia"
        Me.btnDBFieldsSelectAllMedia.Size = New System.Drawing.Size(89, 23)
        Me.btnDBFieldsSelectAllMedia.TabIndex = 4
        Me.btnDBFieldsSelectAllMedia.Text = "Select All"
        Me.btnDBFieldsSelectAllMedia.UseVisualStyleBackColor = true
        '
        'btnDBFieldsSelectAllInternet
        '
        Me.btnDBFieldsSelectAllInternet.Location = New System.Drawing.Point(248, 32)
        Me.btnDBFieldsSelectAllInternet.Name = "btnDBFieldsSelectAllInternet"
        Me.btnDBFieldsSelectAllInternet.Size = New System.Drawing.Size(90, 23)
        Me.btnDBFieldsSelectAllInternet.TabIndex = 7
        Me.btnDBFieldsSelectAllInternet.Text = "Select All"
        Me.btnDBFieldsSelectAllInternet.UseVisualStyleBackColor = true
        '
        'btnDBFieldsSelectAll
        '
        Me.btnDBFieldsSelectAll.Location = New System.Drawing.Point(21, 32)
        Me.btnDBFieldsSelectAll.Name = "btnDBFieldsSelectAll"
        Me.btnDBFieldsSelectAll.Size = New System.Drawing.Size(89, 23)
        Me.btnDBFieldsSelectAll.TabIndex = 1
        Me.btnDBFieldsSelectAll.Text = "Select All"
        Me.btnDBFieldsSelectAll.UseVisualStyleBackColor = true
        '
        'cbDatabaseFields
        '
        Me.cbDatabaseFields.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.cbDatabaseFields.CheckOnClick = true
        Me.cbDatabaseFields.ColumnWidth = 140
        Me.cbDatabaseFields.FormattingEnabled = true
        Me.cbDatabaseFields.Location = New System.Drawing.Point(11, 24)
        Me.cbDatabaseFields.MultiColumn = true
        Me.cbDatabaseFields.Name = "cbDatabaseFields"
        Me.cbDatabaseFields.Size = New System.Drawing.Size(218, 529)
        Me.cbDatabaseFields.TabIndex = 1
        '
        'Label34
        '
        Me.Label34.AutoSize = true
        Me.Label34.Location = New System.Drawing.Point(8, 7)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(225, 13)
        Me.Label34.TabIndex = 0
        Me.Label34.Text = "Select database fields to be imported/updated"
        '
        'ScanFilters
        '
        Me.ScanFilters.Controls.Add(Me.GroupBox28)
        Me.ScanFilters.Controls.Add(Me.GroupBox24)
        Me.ScanFilters.Controls.Add(Me.GroupBox6)
        Me.ScanFilters.Controls.Add(Me.Label46)
        Me.ScanFilters.Controls.Add(Me.GroupBox20)
        Me.ScanFilters.Controls.Add(Me.GroupBox19)
        Me.ScanFilters.Location = New System.Drawing.Point(4, 22)
        Me.ScanFilters.Name = "ScanFilters"
        Me.ScanFilters.Size = New System.Drawing.Size(600, 580)
        Me.ScanFilters.TabIndex = 5
        Me.ScanFilters.Text = "Scan Path and Filters"
        Me.ScanFilters.UseVisualStyleBackColor = true
        '
        'GroupBox28
        '
        Me.GroupBox28.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox28.Controls.Add(Me.dgEditionStrings)
        Me.GroupBox28.Location = New System.Drawing.Point(8, 375)
        Me.GroupBox28.Name = "GroupBox28"
        Me.GroupBox28.Size = New System.Drawing.Size(584, 87)
        Me.GroupBox28.TabIndex = 4
        Me.GroupBox28.TabStop = false
        Me.GroupBox28.Text = "Edition Detection ..."
        Me.ToolTip1.SetToolTip(Me.GroupBox28, resources.GetString("GroupBox28.ToolTip"))
        '
        'dgEditionStrings
        '
        Me.dgEditionStrings.AllowUserToResizeRows = false
        Me.dgEditionStrings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.dgEditionStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgEditionStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgEditionStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgEditionStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgEditionStrings.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.EditorSearchExpression, Me.EditorReplacementString})
        Me.dgEditionStrings.Location = New System.Drawing.Point(14, 19)
        Me.dgEditionStrings.Name = "dgEditionStrings"
        Me.dgEditionStrings.RowHeadersWidth = 25
        Me.dgEditionStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgEditionStrings.RowTemplate.Height = 18
        Me.dgEditionStrings.Size = New System.Drawing.Size(546, 62)
        Me.dgEditionStrings.TabIndex = 0
        '
        'EditorSearchExpression
        '
        Me.EditorSearchExpression.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.EditorSearchExpression.HeaderText = "Search Expression"
        Me.EditorSearchExpression.Name = "EditorSearchExpression"
        '
        'EditorReplacementString
        '
        Me.EditorReplacementString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.EditorReplacementString.HeaderText = "Replacement Expression"
        Me.EditorReplacementString.Name = "EditorReplacementString"
        '
        'GroupBox24
        '
        Me.GroupBox24.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox24.Controls.Add(Me.txtOverridePath)
        Me.GroupBox24.Controls.Add(Me.lblOverridePath)
        Me.GroupBox24.Controls.Add(Me.btnSelectMovieFolder)
        Me.GroupBox24.Controls.Add(Me.Label1)
        Me.GroupBox24.Controls.Add(Me.txtMovieFolder)
        Me.GroupBox24.Controls.Add(Me.Label17)
        Me.GroupBox24.Location = New System.Drawing.Point(8, 13)
        Me.GroupBox24.Name = "GroupBox24"
        Me.GroupBox24.Size = New System.Drawing.Size(584, 88)
        Me.GroupBox24.TabIndex = 0
        Me.GroupBox24.TabStop = false
        Me.GroupBox24.Text = "Movie Scan Folder(s) ..."
        '
        'txtOverridePath
        '
        Me.txtOverridePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtOverridePath.Location = New System.Drawing.Point(103, 60)
        Me.txtOverridePath.Name = "txtOverridePath"
        Me.txtOverridePath.Size = New System.Drawing.Size(410, 20)
        Me.txtOverridePath.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.txtOverridePath, resources.GetString("txtOverridePath.ToolTip"))
        '
        'lblOverridePath
        '
        Me.lblOverridePath.AutoSize = true
        Me.lblOverridePath.Location = New System.Drawing.Point(11, 63)
        Me.lblOverridePath.Name = "lblOverridePath"
        Me.lblOverridePath.Size = New System.Drawing.Size(71, 13)
        Me.lblOverridePath.TabIndex = 4
        Me.lblOverridePath.Text = "Override path"
        '
        'btnSelectMovieFolder
        '
        Me.btnSelectMovieFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSelectMovieFolder.CausesValidation = false
        Me.btnSelectMovieFolder.Location = New System.Drawing.Point(528, 34)
        Me.btnSelectMovieFolder.Name = "btnSelectMovieFolder"
        Me.btnSelectMovieFolder.Size = New System.Drawing.Size(32, 20)
        Me.btnSelectMovieFolder.TabIndex = 3
        Me.btnSelectMovieFolder.Text = "..."
        Me.btnSelectMovieFolder.UseVisualStyleBackColor = true
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(11, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(130, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Path to Movies Folder(s)  :"
        '
        'txtMovieFolder
        '
        Me.txtMovieFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtMovieFolder.Location = New System.Drawing.Point(12, 34)
        Me.txtMovieFolder.Name = "txtMovieFolder"
        Me.txtMovieFolder.Size = New System.Drawing.Size(501, 20)
        Me.txtMovieFolder.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtMovieFolder, "Enter the paths (local or UNC) to the folders you want to scan."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Separate multipl"& _ 
        "e folders with semi-colons.")
        '
        'Label17
        '
        Me.Label17.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label17.AutoSize = true
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label17.Location = New System.Drawing.Point(311, 19)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(202, 12)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "Note : Separate movie folders with semi-colons."
        '
        'GroupBox6
        '
        Me.GroupBox6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox6.Controls.Add(Me.Label25)
        Me.GroupBox6.Controls.Add(Me.txtRegExSearchMultiPart)
        Me.GroupBox6.Location = New System.Drawing.Point(8, 162)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(584, 63)
        Me.GroupBox6.TabIndex = 2
        Me.GroupBox6.TabStop = false
        Me.GroupBox6.Text = "Multi Part Files Detection  ..."
        '
        'Label25
        '
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(20, 16)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(210, 13)
        Me.Label25.TabIndex = 0
        Me.Label25.Text = "RegEx Expression to detect multi-part files :"
        '
        'txtRegExSearchMultiPart
        '
        Me.txtRegExSearchMultiPart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtRegExSearchMultiPart.Location = New System.Drawing.Point(14, 32)
        Me.txtRegExSearchMultiPart.Name = "txtRegExSearchMultiPart"
        Me.txtRegExSearchMultiPart.Size = New System.Drawing.Size(546, 20)
        Me.txtRegExSearchMultiPart.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtRegExSearchMultiPart, "AMCUpdater uses this Regular Expression to detect multi-part movies."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Example: 'm"& _ 
        "oviename - CD1.avi;'moviename - CD2.avi '")
        '
        'Label46
        '
        Me.Label46.Location = New System.Drawing.Point(8, 115)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(584, 44)
        Me.Label46.TabIndex = 1
        Me.Label46.Text = resources.GetString("Label46.Text")
        '
        'GroupBox20
        '
        Me.GroupBox20.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox20.Controls.Add(Me.Label83)
        Me.GroupBox20.Controls.Add(Me.dgFilterStrings)
        Me.GroupBox20.Location = New System.Drawing.Point(8, 231)
        Me.GroupBox20.Name = "GroupBox20"
        Me.GroupBox20.Size = New System.Drawing.Size(584, 138)
        Me.GroupBox20.TabIndex = 3
        Me.GroupBox20.TabStop = false
        Me.GroupBox20.Text = "Strip Characters From Title ..."
        Me.ToolTip1.SetToolTip(Me.GroupBox20, "You can define expressions to clean up the (media) names to get a clean title for"& _ 
        " internet searches."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Both normal and RegEx search expressi0ons are allowed.")
        '
        'Label83
        '
        Me.Label83.AutoSize = true
        Me.Label83.Location = New System.Drawing.Point(20, 19)
        Me.Label83.Name = "Label83"
        Me.Label83.Size = New System.Drawing.Size(233, 13)
        Me.Label83.TabIndex = 0
        Me.Label83.Text = "Cleanup TITLEs based on keyword and RegEx:"
        '
        'dgFilterStrings
        '
        Me.dgFilterStrings.AllowUserToResizeRows = false
        Me.dgFilterStrings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.dgFilterStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgFilterStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgFilterStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgFilterStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgFilterStrings.ColumnHeadersVisible = false
        Me.dgFilterStrings.Location = New System.Drawing.Point(14, 35)
        Me.dgFilterStrings.Name = "dgFilterStrings"
        Me.dgFilterStrings.RowHeadersWidth = 25
        Me.dgFilterStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgFilterStrings.RowTemplate.Height = 18
        Me.dgFilterStrings.Size = New System.Drawing.Size(546, 95)
        Me.dgFilterStrings.TabIndex = 1
        '
        'GroupBox19
        '
        Me.GroupBox19.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox19.Controls.Add(Me.dgExcludedFolderStrings)
        Me.GroupBox19.Controls.Add(Me.Label44)
        Me.GroupBox19.Controls.Add(Me.Label45)
        Me.GroupBox19.Controls.Add(Me.dgExcludedFileStrings)
        Me.GroupBox19.Location = New System.Drawing.Point(8, 468)
        Me.GroupBox19.Name = "GroupBox19"
        Me.GroupBox19.Size = New System.Drawing.Size(584, 106)
        Me.GroupBox19.TabIndex = 5
        Me.GroupBox19.TabStop = false
        Me.GroupBox19.Text = "File and Folder Exclusions ..."
        '
        'dgExcludedFolderStrings
        '
        Me.dgExcludedFolderStrings.AllowUserToResizeRows = false
        Me.dgExcludedFolderStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgExcludedFolderStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgExcludedFolderStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgExcludedFolderStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgExcludedFolderStrings.ColumnHeadersVisible = false
        Me.dgExcludedFolderStrings.Location = New System.Drawing.Point(14, 34)
        Me.dgExcludedFolderStrings.Name = "dgExcludedFolderStrings"
        Me.dgExcludedFolderStrings.RowHeadersWidth = 25
        Me.dgExcludedFolderStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgExcludedFolderStrings.Size = New System.Drawing.Size(259, 62)
        Me.dgExcludedFolderStrings.TabIndex = 1
        '
        'Label44
        '
        Me.Label44.AutoSize = true
        Me.Label44.Location = New System.Drawing.Point(20, 18)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(194, 13)
        Me.Label44.TabIndex = 0
        Me.Label44.Text = "Exclude FOLDERs based on keywords:"
        '
        'Label45
        '
        Me.Label45.AutoSize = true
        Me.Label45.Location = New System.Drawing.Point(307, 18)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(173, 13)
        Me.Label45.TabIndex = 2
        Me.Label45.Text = "Exclude FILEs based on keywords:"
        '
        'dgExcludedFileStrings
        '
        Me.dgExcludedFileStrings.AllowUserToResizeRows = false
        Me.dgExcludedFileStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgExcludedFileStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgExcludedFileStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgExcludedFileStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgExcludedFileStrings.ColumnHeadersVisible = false
        Me.dgExcludedFileStrings.Location = New System.Drawing.Point(299, 34)
        Me.dgExcludedFileStrings.Name = "dgExcludedFileStrings"
        Me.dgExcludedFileStrings.RowHeadersWidth = 25
        Me.dgExcludedFileStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgExcludedFileStrings.Size = New System.Drawing.Size(261, 62)
        Me.dgExcludedFileStrings.TabIndex = 3
        '
        'Options
        '
        Me.Options.Controls.Add(Me.GroupBox26)
        Me.Options.Controls.Add(Me.GroupBox11)
        Me.Options.Controls.Add(Me.GroupBox4)
        Me.Options.Controls.Add(Me.GroupBox7)
        Me.Options.Controls.Add(Me.GroupBox13)
        Me.Options.Controls.Add(Me.GroupBox12)
        Me.Options.Location = New System.Drawing.Point(4, 22)
        Me.Options.Name = "Options"
        Me.Options.Padding = New System.Windows.Forms.Padding(3)
        Me.Options.Size = New System.Drawing.Size(600, 580)
        Me.Options.TabIndex = 1
        Me.Options.Text = "Options"
        Me.Options.UseVisualStyleBackColor = true
        '
        'GroupBox26
        '
        Me.GroupBox26.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox26.Controls.Add(Me.Label87)
        Me.GroupBox26.Controls.Add(Me.Label86)
        Me.GroupBox26.Controls.Add(Me.Label85)
        Me.GroupBox26.Controls.Add(Me.Label84)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideTitleLimit)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverridePersonLimit)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideGetRoles)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideLanguage)
        Me.GroupBox26.Location = New System.Drawing.Point(13, 368)
        Me.GroupBox26.Name = "GroupBox26"
        Me.GroupBox26.Size = New System.Drawing.Size(575, 86)
        Me.GroupBox26.TabIndex = 3
        Me.GroupBox26.TabStop = false
        Me.GroupBox26.Text = "Internet Grabber - Override Options ..."
        Me.ToolTip1.SetToolTip(Me.GroupBox26, resources.GetString("GroupBox26.ToolTip"))
        '
        'Label87
        '
        Me.Label87.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label87.AutoSize = true
        Me.Label87.Location = New System.Drawing.Point(339, 62)
        Me.Label87.Name = "Label87"
        Me.Label87.Size = New System.Drawing.Size(66, 13)
        Me.Label87.TabIndex = 6
        Me.Label87.Text = "Limit # Titles"
        '
        'Label86
        '
        Me.Label86.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label86.AutoSize = true
        Me.Label86.Location = New System.Drawing.Point(339, 38)
        Me.Label86.Name = "Label86"
        Me.Label86.Size = New System.Drawing.Size(79, 13)
        Me.Label86.TabIndex = 4
        Me.Label86.Text = "Limit # Persons"
        '
        'Label85
        '
        Me.Label85.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label85.AutoSize = true
        Me.Label85.Location = New System.Drawing.Point(339, 14)
        Me.Label85.Name = "Label85"
        Me.Label85.Size = New System.Drawing.Size(82, 13)
        Me.Label85.TabIndex = 2
        Me.Label85.Text = "Get Actor Roles"
        '
        'Label84
        '
        Me.Label84.AutoSize = true
        Me.Label84.Location = New System.Drawing.Point(12, 27)
        Me.Label84.Name = "Label84"
        Me.Label84.Size = New System.Drawing.Size(68, 39)
        Me.Label84.TabIndex = 0
        Me.Label84.Text = "Preferred "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Languages /"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Countries"
        '
        'chkGrabberOverrideTitleLimit
        '
        Me.chkGrabberOverrideTitleLimit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.chkGrabberOverrideTitleLimit.FormattingEnabled = true
        Me.chkGrabberOverrideTitleLimit.Items.AddRange(New Object() {"0", "1", "2", "3", "4", "5", "10", "15", "20", "999"})
        Me.chkGrabberOverrideTitleLimit.Location = New System.Drawing.Point(436, 59)
        Me.chkGrabberOverrideTitleLimit.Name = "chkGrabberOverrideTitleLimit"
        Me.chkGrabberOverrideTitleLimit.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverrideTitleLimit.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideTitleLimit, "Limits the number of translated titles grabbed."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"You may also set a preferred lan"& _ 
        "guage/country "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"for grabbing in the override options.")
        '
        'chkGrabberOverridePersonLimit
        '
        Me.chkGrabberOverridePersonLimit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.chkGrabberOverridePersonLimit.FormattingEnabled = true
        Me.chkGrabberOverridePersonLimit.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "10", "15", "20", "999"})
        Me.chkGrabberOverridePersonLimit.Location = New System.Drawing.Point(436, 35)
        Me.chkGrabberOverridePersonLimit.Name = "chkGrabberOverridePersonLimit"
        Me.chkGrabberOverridePersonLimit.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverridePersonLimit.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverridePersonLimit, "Limits the number of person names grabbed."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"This settings applies to all fields t"& _ 
        "hat grab person names, "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"like actors, producers, writers, directors.")
        '
        'chkGrabberOverrideGetRoles
        '
        Me.chkGrabberOverrideGetRoles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.chkGrabberOverrideGetRoles.FormattingEnabled = true
        Me.chkGrabberOverrideGetRoles.Items.AddRange(New Object() {"true", "false"})
        Me.chkGrabberOverrideGetRoles.Location = New System.Drawing.Point(436, 11)
        Me.chkGrabberOverrideGetRoles.Name = "chkGrabberOverrideGetRoles"
        Me.chkGrabberOverrideGetRoles.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverrideGetRoles.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideGetRoles, "If checked, add Roles to actor infos."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Depends on the grabber script supporting t"& _ 
        "hat option.")
        '
        'chkGrabberOverrideLanguage
        '
        Me.chkGrabberOverrideLanguage.FormattingEnabled = true
        Me.chkGrabberOverrideLanguage.Items.AddRange(New Object() {"Argentina", "Australia", "Austria", "Belgium", "Brazil", "Canada", "Chile", "China", "Croatia", "Czech Republic", "Denmark", "Estonia", "Finland", "France", "Germany", "Greece", "Hong Kong", "Hungary", "Iceland", "India", "Ireland", "Israel", "Italy", "Japan", "Malaysia", "Mexico", "Netherlands", "New Zealand", "Norway", "Peru", "Philippines", "Poland", "Portugal", "Romania", "Russia", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sweden", "Switzerland", "Turkey", "UK", "Uruguay", "USA"})
        Me.chkGrabberOverrideLanguage.Location = New System.Drawing.Point(112, 35)
        Me.chkGrabberOverrideLanguage.Name = "chkGrabberOverrideLanguage"
        Me.chkGrabberOverrideLanguage.Size = New System.Drawing.Size(196, 21)
        Me.chkGrabberOverrideLanguage.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideLanguage, resources.GetString("chkGrabberOverrideLanguage.ToolTip"))
        '
        'GroupBox11
        '
        Me.GroupBox11.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox11.Controls.Add(Me.btnExcludeFileDelete)
        Me.GroupBox11.Controls.Add(Me.btnExcludeFileShow)
        Me.GroupBox11.Controls.Add(Me.txtExcludeFilePath)
        Me.GroupBox11.Controls.Add(Me.Label22)
        Me.GroupBox11.Controls.Add(Me.btnSelectExcludeFile)
        Me.GroupBox11.Controls.Add(Me.txtConfigFilePath)
        Me.GroupBox11.Controls.Add(Me.Label2)
        Me.GroupBox11.Controls.Add(Me.btnSelectConfigFile)
        Me.GroupBox11.Location = New System.Drawing.Point(13, 6)
        Me.GroupBox11.Name = "GroupBox11"
        Me.GroupBox11.Size = New System.Drawing.Size(575, 102)
        Me.GroupBox11.TabIndex = 0
        Me.GroupBox11.TabStop = false
        Me.GroupBox11.Text = "AMC Folders and Files ..."
        '
        'btnExcludeFileDelete
        '
        Me.btnExcludeFileDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnExcludeFileDelete.Location = New System.Drawing.Point(339, 72)
        Me.btnExcludeFileDelete.Name = "btnExcludeFileDelete"
        Me.btnExcludeFileDelete.Size = New System.Drawing.Size(56, 20)
        Me.btnExcludeFileDelete.TabIndex = 5
        Me.btnExcludeFileDelete.Text = "reset"
        Me.ToolTip1.SetToolTip(Me.btnExcludeFileDelete, resources.GetString("btnExcludeFileDelete.ToolTip"))
        Me.btnExcludeFileDelete.UseVisualStyleBackColor = true
        '
        'btnExcludeFileShow
        '
        Me.btnExcludeFileShow.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnExcludeFileShow.Location = New System.Drawing.Point(415, 72)
        Me.btnExcludeFileShow.Name = "btnExcludeFileShow"
        Me.btnExcludeFileShow.Size = New System.Drawing.Size(56, 20)
        Me.btnExcludeFileShow.TabIndex = 6
        Me.btnExcludeFileShow.Text = "show"
        Me.btnExcludeFileShow.UseVisualStyleBackColor = true
        '
        'txtExcludeFilePath
        '
        Me.txtExcludeFilePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtExcludeFilePath.Location = New System.Drawing.Point(12, 72)
        Me.txtExcludeFilePath.Name = "txtExcludeFilePath"
        Me.txtExcludeFilePath.Size = New System.Drawing.Size(296, 20)
        Me.txtExcludeFilePath.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtExcludeFilePath, resources.GetString("txtExcludeFilePath.ToolTip"))
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(12, 56)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(173, 13)
        Me.Label22.TabIndex = 3
        Me.Label22.Text = "Path to Excluded Movies File (.txt) :"
        '
        'btnSelectExcludeFile
        '
        Me.btnSelectExcludeFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSelectExcludeFile.CausesValidation = false
        Me.btnSelectExcludeFile.Location = New System.Drawing.Point(521, 72)
        Me.btnSelectExcludeFile.Name = "btnSelectExcludeFile"
        Me.btnSelectExcludeFile.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectExcludeFile.TabIndex = 7
        Me.btnSelectExcludeFile.Text = "..."
        Me.btnSelectExcludeFile.UseVisualStyleBackColor = true
        '
        'txtConfigFilePath
        '
        Me.txtConfigFilePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtConfigFilePath.Location = New System.Drawing.Point(12, 32)
        Me.txtConfigFilePath.Name = "txtConfigFilePath"
        Me.txtConfigFilePath.Size = New System.Drawing.Size(501, 20)
        Me.txtConfigFilePath.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtConfigFilePath, "Enter the location of your Ant Movie Database file."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"This must be saved in .xml f"& _ 
        "ormat."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"If you do not have one, enter the desired location and a new database fi"& _ 
        "le will be created for you.")
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(12, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Path to AMC Database File (.xml) :"
        '
        'btnSelectConfigFile
        '
        Me.btnSelectConfigFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSelectConfigFile.CausesValidation = false
        Me.btnSelectConfigFile.Location = New System.Drawing.Point(521, 32)
        Me.btnSelectConfigFile.Name = "btnSelectConfigFile"
        Me.btnSelectConfigFile.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectConfigFile.TabIndex = 2
        Me.btnSelectConfigFile.Text = "..."
        Me.btnSelectConfigFile.UseVisualStyleBackColor = true
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.Controls.Add(Me.chkParseTrailers)
        Me.GroupBox4.Controls.Add(Me.GroupBox25)
        Me.GroupBox4.Controls.Add(Me.txtTrailerIentificationStrings)
        Me.GroupBox4.Controls.Add(Me.Label79)
        Me.GroupBox4.Controls.Add(Me.txtDefaultFileTypesNonMedia)
        Me.GroupBox4.Controls.Add(Me.Label14)
        Me.GroupBox4.Controls.Add(Me.txtDefaultFileTypes)
        Me.GroupBox4.Controls.Add(Me.Label15)
        Me.GroupBox4.Location = New System.Drawing.Point(13, 224)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(575, 138)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = false
        Me.GroupBox4.Text = "File Type Identification ..."
        '
        'chkParseTrailers
        '
        Me.chkParseTrailers.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.chkParseTrailers.AutoSize = true
        Me.chkParseTrailers.Enabled = false
        Me.chkParseTrailers.Location = New System.Drawing.Point(377, 113)
        Me.chkParseTrailers.Name = "chkParseTrailers"
        Me.chkParseTrailers.Size = New System.Drawing.Size(134, 17)
        Me.chkParseTrailers.TabIndex = 7
        Me.chkParseTrailers.Text = "Check for Trailers (wip)"
        Me.chkParseTrailers.UseVisualStyleBackColor = true
        Me.chkParseTrailers.Visible = false
        '
        'GroupBox25
        '
        Me.GroupBox25.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox25.Controls.Add(Me.chkCheckDVDFolders)
        Me.GroupBox25.Controls.Add(Me.chkParseSubtitleFiles)
        Me.GroupBox25.Controls.Add(Me.chkParsePlaylistFiles)
        Me.GroupBox25.Location = New System.Drawing.Point(367, 16)
        Me.GroupBox25.Name = "GroupBox25"
        Me.GroupBox25.Size = New System.Drawing.Size(187, 91)
        Me.GroupBox25.TabIndex = 6
        Me.GroupBox25.TabStop = false
        Me.GroupBox25.Text = "Media Scan Options ..."
        '
        'chkCheckDVDFolders
        '
        Me.chkCheckDVDFolders.AutoSize = true
        Me.chkCheckDVDFolders.Location = New System.Drawing.Point(10, 23)
        Me.chkCheckDVDFolders.Name = "chkCheckDVDFolders"
        Me.chkCheckDVDFolders.Size = New System.Drawing.Size(158, 17)
        Me.chkCheckDVDFolders.TabIndex = 0
        Me.chkCheckDVDFolders.Text = "Check for DVD / BR folders"
        Me.ToolTip1.SetToolTip(Me.chkCheckDVDFolders, "Enable this option to search for DVD rips."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"The system looks for a VIDEO_TS.IFO f"& _ 
        "ile and attempts to work out"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"the movie title from the folder structure.")
        Me.chkCheckDVDFolders.UseVisualStyleBackColor = true
        '
        'chkParseSubtitleFiles
        '
        Me.chkParseSubtitleFiles.AutoSize = true
        Me.chkParseSubtitleFiles.Location = New System.Drawing.Point(10, 46)
        Me.chkParseSubtitleFiles.Name = "chkParseSubtitleFiles"
        Me.chkParseSubtitleFiles.Size = New System.Drawing.Size(145, 17)
        Me.chkParseSubtitleFiles.TabIndex = 1
        Me.chkParseSubtitleFiles.Text = "Try to Parse Subtitle Files"
        Me.ToolTip1.SetToolTip(Me.chkParseSubtitleFiles, resources.GetString("chkParseSubtitleFiles.ToolTip"))
        Me.chkParseSubtitleFiles.UseVisualStyleBackColor = true
        '
        'chkParsePlaylistFiles
        '
        Me.chkParsePlaylistFiles.AutoSize = true
        Me.chkParsePlaylistFiles.Enabled = false
        Me.chkParsePlaylistFiles.Location = New System.Drawing.Point(10, 69)
        Me.chkParsePlaylistFiles.Name = "chkParsePlaylistFiles"
        Me.chkParsePlaylistFiles.Size = New System.Drawing.Size(156, 17)
        Me.chkParsePlaylistFiles.TabIndex = 2
        Me.chkParsePlaylistFiles.Text = "Check for Playlist Files (wip)"
        Me.chkParsePlaylistFiles.UseVisualStyleBackColor = true
        '
        'txtTrailerIentificationStrings
        '
        Me.txtTrailerIentificationStrings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtTrailerIentificationStrings.Location = New System.Drawing.Point(12, 111)
        Me.txtTrailerIentificationStrings.Name = "txtTrailerIentificationStrings"
        Me.txtTrailerIentificationStrings.Size = New System.Drawing.Size(296, 20)
        Me.txtTrailerIentificationStrings.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.txtTrailerIentificationStrings, resources.GetString("txtTrailerIentificationStrings.ToolTip"))
        '
        'Label79
        '
        Me.Label79.AutoSize = true
        Me.Label79.Location = New System.Drawing.Point(9, 94)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(183, 13)
        Me.Label79.TabIndex = 4
        Me.Label79.Text = "Accepted Trailer Identification Strings"
        '
        'txtDefaultFileTypesNonMedia
        '
        Me.txtDefaultFileTypesNonMedia.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtDefaultFileTypesNonMedia.Location = New System.Drawing.Point(12, 71)
        Me.txtDefaultFileTypesNonMedia.Name = "txtDefaultFileTypesNonMedia"
        Me.txtDefaultFileTypesNonMedia.Size = New System.Drawing.Size(296, 20)
        Me.txtDefaultFileTypesNonMedia.TabIndex = 3
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(9, 55)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(285, 13)
        Me.Label14.TabIndex = 2
        Me.Label14.Text = "Accepted non-Mediainfo Files (import without media details)"
        '
        'txtDefaultFileTypes
        '
        Me.txtDefaultFileTypes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtDefaultFileTypes.Location = New System.Drawing.Point(12, 32)
        Me.txtDefaultFileTypes.Name = "txtDefaultFileTypes"
        Me.txtDefaultFileTypes.Size = New System.Drawing.Size(296, 20)
        Me.txtDefaultFileTypes.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.AutoSize = true
        Me.Label15.Location = New System.Drawing.Point(9, 16)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(274, 13)
        Me.Label15.TabIndex = 0
        Me.Label15.Text = "Accepted Mediainfo Files. Separate List with semi-colons"
        Me.ToolTip1.SetToolTip(Me.Label15, "Accepted Media Files."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Separate List with semi-colons, e.g. wmv,avi,mpg")
        '
        'GroupBox7
        '
        Me.GroupBox7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox7.Controls.Add(Me.btnSelectPersonArtworkFolder)
        Me.GroupBox7.Controls.Add(Me.Label28)
        Me.GroupBox7.Controls.Add(Me.txtPersonArtworkFolder)
        Me.GroupBox7.Controls.Add(Me.btnSelectFanartFolder)
        Me.GroupBox7.Controls.Add(Me.txtFanartFolder)
        Me.GroupBox7.Controls.Add(Me.Label48)
        Me.GroupBox7.Location = New System.Drawing.Point(13, 111)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(575, 107)
        Me.GroupBox7.TabIndex = 1
        Me.GroupBox7.TabStop = false
        Me.GroupBox7.Text = "Artwork Folders ..."
        '
        'btnSelectPersonArtworkFolder
        '
        Me.btnSelectPersonArtworkFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSelectPersonArtworkFolder.Location = New System.Drawing.Point(521, 74)
        Me.btnSelectPersonArtworkFolder.Name = "btnSelectPersonArtworkFolder"
        Me.btnSelectPersonArtworkFolder.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectPersonArtworkFolder.TabIndex = 5
        Me.btnSelectPersonArtworkFolder.Text = "..."
        Me.btnSelectPersonArtworkFolder.UseVisualStyleBackColor = true
        '
        'Label28
        '
        Me.Label28.AutoSize = true
        Me.Label28.Location = New System.Drawing.Point(10, 58)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(117, 13)
        Me.Label28.TabIndex = 3
        Me.Label28.Text = "Path to Person Images:"
        '
        'txtPersonArtworkFolder
        '
        Me.txtPersonArtworkFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtPersonArtworkFolder.Location = New System.Drawing.Point(10, 74)
        Me.txtPersonArtworkFolder.Name = "txtPersonArtworkFolder"
        Me.txtPersonArtworkFolder.Size = New System.Drawing.Size(503, 20)
        Me.txtPersonArtworkFolder.TabIndex = 4
        '
        'btnSelectFanartFolder
        '
        Me.btnSelectFanartFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSelectFanartFolder.CausesValidation = false
        Me.btnSelectFanartFolder.Location = New System.Drawing.Point(521, 33)
        Me.btnSelectFanartFolder.Name = "btnSelectFanartFolder"
        Me.btnSelectFanartFolder.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectFanartFolder.TabIndex = 2
        Me.btnSelectFanartFolder.Text = "..."
        Me.btnSelectFanartFolder.UseVisualStyleBackColor = true
        '
        'txtFanartFolder
        '
        Me.txtFanartFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtFanartFolder.Location = New System.Drawing.Point(10, 33)
        Me.txtFanartFolder.Name = "txtFanartFolder"
        Me.txtFanartFolder.Size = New System.Drawing.Size(503, 20)
        Me.txtFanartFolder.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtFanartFolder, "Enter the paths (local or UNC) to the folders you want to scan."&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Separate multipl"& _ 
        "e folders with semi-colons.")
        '
        'Label48
        '
        Me.Label48.AutoSize = true
        Me.Label48.Location = New System.Drawing.Point(10, 16)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(80, 13)
        Me.Label48.TabIndex = 0
        Me.Label48.Text = "Path to Fanart :"
        '
        'GroupBox13
        '
        Me.GroupBox13.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox13.Controls.Add(Me.cbLogLevel)
        Me.GroupBox13.Controls.Add(Me.Label29)
        Me.GroupBox13.Location = New System.Drawing.Point(13, 534)
        Me.GroupBox13.Name = "GroupBox13"
        Me.GroupBox13.Size = New System.Drawing.Size(575, 40)
        Me.GroupBox13.TabIndex = 5
        Me.GroupBox13.TabStop = false
        Me.GroupBox13.Text = "Other"
        '
        'cbLogLevel
        '
        Me.cbLogLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLogLevel.FormattingEnabled = true
        Me.cbLogLevel.Items.AddRange(New Object() {"All Events with Grabbing", "All Events", "Major Events", "Errors Only"})
        Me.cbLogLevel.Location = New System.Drawing.Point(112, 13)
        Me.cbLogLevel.Name = "cbLogLevel"
        Me.cbLogLevel.Size = New System.Drawing.Size(196, 21)
        Me.cbLogLevel.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cbLogLevel, "The logging level option controls what information is saved to the logfile")
        '
        'Label29
        '
        Me.Label29.AutoSize = true
        Me.Label29.Location = New System.Drawing.Point(9, 16)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(80, 13)
        Me.Label29.TabIndex = 0
        Me.Label29.Text = "Logging Level :"
        '
        'GroupBox12
        '
        Me.GroupBox12.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBox12.Controls.Add(Me.chkExecuteOnlyForOrphans)
        Me.GroupBox12.Controls.Add(Me.btnExecuteProgramSelectPath)
        Me.GroupBox12.Controls.Add(Me.txtExecuteProgramPath)
        Me.GroupBox12.Controls.Add(Me.chkExecuteProgram)
        Me.GroupBox12.Controls.Add(Me.Label27)
        Me.GroupBox12.Location = New System.Drawing.Point(13, 460)
        Me.GroupBox12.Name = "GroupBox12"
        Me.GroupBox12.Size = New System.Drawing.Size(575, 68)
        Me.GroupBox12.TabIndex = 4
        Me.GroupBox12.TabStop = false
        Me.GroupBox12.Text = "Post Processing"
        '
        'chkExecuteOnlyForOrphans
        '
        Me.chkExecuteOnlyForOrphans.AutoSize = true
        Me.chkExecuteOnlyForOrphans.Location = New System.Drawing.Point(377, 16)
        Me.chkExecuteOnlyForOrphans.Name = "chkExecuteOnlyForOrphans"
        Me.chkExecuteOnlyForOrphans.Size = New System.Drawing.Size(136, 17)
        Me.chkExecuteOnlyForOrphans.TabIndex = 4
        Me.chkExecuteOnlyForOrphans.Text = "Only if changes applied"
        Me.chkExecuteOnlyForOrphans.UseVisualStyleBackColor = true
        '
        'btnExecuteProgramSelectPath
        '
        Me.btnExecuteProgramSelectPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnExecuteProgramSelectPath.Location = New System.Drawing.Point(521, 39)
        Me.btnExecuteProgramSelectPath.Name = "btnExecuteProgramSelectPath"
        Me.btnExecuteProgramSelectPath.Size = New System.Drawing.Size(37, 20)
        Me.btnExecuteProgramSelectPath.TabIndex = 3
        Me.btnExecuteProgramSelectPath.Text = "..."
        Me.btnExecuteProgramSelectPath.UseVisualStyleBackColor = true
        '
        'txtExecuteProgramPath
        '
        Me.txtExecuteProgramPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtExecuteProgramPath.Location = New System.Drawing.Point(112, 39)
        Me.txtExecuteProgramPath.Name = "txtExecuteProgramPath"
        Me.txtExecuteProgramPath.Size = New System.Drawing.Size(401, 20)
        Me.txtExecuteProgramPath.TabIndex = 2
        '
        'chkExecuteProgram
        '
        Me.chkExecuteProgram.AutoSize = true
        Me.chkExecuteProgram.Location = New System.Drawing.Point(12, 16)
        Me.chkExecuteProgram.Name = "chkExecuteProgram"
        Me.chkExecuteProgram.Size = New System.Drawing.Size(217, 17)
        Me.chkExecuteProgram.TabIndex = 0
        Me.chkExecuteProgram.Text = "Launch a file after processing completed"
        Me.chkExecuteProgram.UseVisualStyleBackColor = true
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(10, 42)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(96, 13)
        Me.Label27.TabIndex = 1
        Me.Label27.Text = "Path to executable"
        '
        'Mediainfo
        '
        Me.Mediainfo.Controls.Add(Me.Label47)
        Me.Mediainfo.Controls.Add(Me.GroupBox18)
        Me.Mediainfo.Controls.Add(Me.GroupBox3)
        Me.Mediainfo.Controls.Add(Me.GroupBox2)
        Me.Mediainfo.Controls.Add(Me.btnTestAnalyse)
        Me.Mediainfo.Controls.Add(Me.GroupBox1)
        Me.Mediainfo.Controls.Add(Me.Label3)
        Me.Mediainfo.Controls.Add(Me.btnGetSampleFile)
        Me.Mediainfo.Controls.Add(Me.txtSampleFile)
        Me.Mediainfo.Location = New System.Drawing.Point(4, 22)
        Me.Mediainfo.Name = "Mediainfo"
        Me.Mediainfo.Padding = New System.Windows.Forms.Padding(3)
        Me.Mediainfo.Size = New System.Drawing.Size(600, 580)
        Me.Mediainfo.TabIndex = 2
        Me.Mediainfo.Text = "Mediainfo"
        Me.Mediainfo.UseVisualStyleBackColor = true
        '
        'Label47
        '
        Me.Label47.Location = New System.Drawing.Point(19, 12)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(495, 43)
        Me.Label47.TabIndex = 0
        Me.Label47.Text = "This page allows you to test that the MediaInfo dll is able to read your movie fi"& _ 
    "le correctly.  Relevant information from the file will be displayed below if pos"& _ 
    "sible."
        '
        'GroupBox18
        '
        Me.GroupBox18.Controls.Add(Me.txtSampleTextLanguageList)
        Me.GroupBox18.Controls.Add(Me.txtSampleTextCodecList)
        Me.GroupBox18.Controls.Add(Me.Label39)
        Me.GroupBox18.Controls.Add(Me.Label38)
        Me.GroupBox18.Location = New System.Drawing.Point(15, 398)
        Me.GroupBox18.Name = "GroupBox18"
        Me.GroupBox18.Size = New System.Drawing.Size(522, 99)
        Me.GroupBox18.TabIndex = 7
        Me.GroupBox18.TabStop = false
        Me.GroupBox18.Text = "Subtitles ..."
        '
        'txtSampleTextLanguageList
        '
        Me.txtSampleTextLanguageList.Location = New System.Drawing.Point(87, 45)
        Me.txtSampleTextLanguageList.Name = "txtSampleTextLanguageList"
        Me.txtSampleTextLanguageList.ReadOnly = true
        Me.txtSampleTextLanguageList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleTextLanguageList.TabIndex = 3
        Me.txtSampleTextLanguageList.TabStop = false
        '
        'txtSampleTextCodecList
        '
        Me.txtSampleTextCodecList.Location = New System.Drawing.Point(87, 19)
        Me.txtSampleTextCodecList.Name = "txtSampleTextCodecList"
        Me.txtSampleTextCodecList.ReadOnly = true
        Me.txtSampleTextCodecList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleTextCodecList.TabIndex = 1
        Me.txtSampleTextCodecList.TabStop = false
        '
        'Label39
        '
        Me.Label39.AutoSize = true
        Me.Label39.Location = New System.Drawing.Point(18, 22)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(63, 13)
        Me.Label39.TabIndex = 0
        Me.Label39.Text = "Codec List :"
        '
        'Label38
        '
        Me.Label38.AutoSize = true
        Me.Label38.Location = New System.Drawing.Point(15, 48)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(66, 13)
        Me.Label38.TabIndex = 2
        Me.Label38.Text = "Languages :"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.txtSampleFileLength)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.txtSampleFileSize)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Location = New System.Drawing.Point(15, 503)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(522, 58)
        Me.GroupBox3.TabIndex = 8
        Me.GroupBox3.TabStop = false
        Me.GroupBox3.Text = "File ..."
        '
        'txtSampleFileLength
        '
        Me.txtSampleFileLength.Location = New System.Drawing.Point(327, 22)
        Me.txtSampleFileLength.Name = "txtSampleFileLength"
        Me.txtSampleFileLength.ReadOnly = true
        Me.txtSampleFileLength.Size = New System.Drawing.Size(157, 20)
        Me.txtSampleFileLength.TabIndex = 3
        Me.txtSampleFileLength.TabStop = false
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(275, 25)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(46, 13)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "Length :"
        '
        'txtSampleFileSize
        '
        Me.txtSampleFileSize.Location = New System.Drawing.Point(87, 22)
        Me.txtSampleFileSize.Name = "txtSampleFileSize"
        Me.txtSampleFileSize.ReadOnly = true
        Me.txtSampleFileSize.Size = New System.Drawing.Size(139, 20)
        Me.txtSampleFileSize.TabIndex = 1
        Me.txtSampleFileSize.TabStop = false
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(47, 25)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(33, 13)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Size :"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label100)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioChannelCount)
        Me.GroupBox2.Controls.Add(Me.Label37)
        Me.GroupBox2.Controls.Add(Me.Label36)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioLanguageList)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioStreamList)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioStreamCount)
        Me.GroupBox2.Controls.Add(Me.Label35)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioBitrate)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioCodec)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Location = New System.Drawing.Point(15, 238)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(522, 154)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Audio ..."
        '
        'Label100
        '
        Me.Label100.AutoSize = true
        Me.Label100.Location = New System.Drawing.Point(275, 22)
        Me.Label100.Name = "Label100"
        Me.Label100.Size = New System.Drawing.Size(54, 13)
        Me.Label100.TabIndex = 2
        Me.Label100.Text = "Channels:"
        '
        'txtSampleAudioChannelCount
        '
        Me.txtSampleAudioChannelCount.Location = New System.Drawing.Point(359, 19)
        Me.txtSampleAudioChannelCount.Name = "txtSampleAudioChannelCount"
        Me.txtSampleAudioChannelCount.ReadOnly = true
        Me.txtSampleAudioChannelCount.Size = New System.Drawing.Size(125, 20)
        Me.txtSampleAudioChannelCount.TabIndex = 3
        Me.txtSampleAudioChannelCount.TabStop = false
        '
        'Label37
        '
        Me.Label37.AutoSize = true
        Me.Label37.Location = New System.Drawing.Point(19, 125)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(66, 13)
        Me.Label37.TabIndex = 10
        Me.Label37.Text = "Languages :"
        '
        'Label36
        '
        Me.Label36.AutoSize = true
        Me.Label36.Location = New System.Drawing.Point(30, 99)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(51, 13)
        Me.Label36.TabIndex = 8
        Me.Label36.Text = "Streams :"
        '
        'txtSampleAudioLanguageList
        '
        Me.txtSampleAudioLanguageList.Location = New System.Drawing.Point(87, 122)
        Me.txtSampleAudioLanguageList.Name = "txtSampleAudioLanguageList"
        Me.txtSampleAudioLanguageList.ReadOnly = true
        Me.txtSampleAudioLanguageList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleAudioLanguageList.TabIndex = 11
        Me.txtSampleAudioLanguageList.TabStop = false
        '
        'txtSampleAudioStreamList
        '
        Me.txtSampleAudioStreamList.Location = New System.Drawing.Point(87, 96)
        Me.txtSampleAudioStreamList.Name = "txtSampleAudioStreamList"
        Me.txtSampleAudioStreamList.ReadOnly = true
        Me.txtSampleAudioStreamList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleAudioStreamList.TabIndex = 9
        Me.txtSampleAudioStreamList.TabStop = false
        '
        'txtSampleAudioStreamCount
        '
        Me.txtSampleAudioStreamCount.Location = New System.Drawing.Point(359, 48)
        Me.txtSampleAudioStreamCount.Name = "txtSampleAudioStreamCount"
        Me.txtSampleAudioStreamCount.ReadOnly = true
        Me.txtSampleAudioStreamCount.Size = New System.Drawing.Size(125, 20)
        Me.txtSampleAudioStreamCount.TabIndex = 7
        Me.txtSampleAudioStreamCount.TabStop = false
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(275, 51)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(51, 13)
        Me.Label35.TabIndex = 6
        Me.Label35.Text = "Streams :"
        '
        'txtSampleAudioBitrate
        '
        Me.txtSampleAudioBitrate.Location = New System.Drawing.Point(86, 48)
        Me.txtSampleAudioBitrate.Name = "txtSampleAudioBitrate"
        Me.txtSampleAudioBitrate.ReadOnly = true
        Me.txtSampleAudioBitrate.Size = New System.Drawing.Size(157, 20)
        Me.txtSampleAudioBitrate.TabIndex = 5
        Me.txtSampleAudioBitrate.TabStop = false
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(36, 51)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(43, 13)
        Me.Label10.TabIndex = 4
        Me.Label10.Text = "Bitrate :"
        '
        'txtSampleAudioCodec
        '
        Me.txtSampleAudioCodec.Location = New System.Drawing.Point(87, 19)
        Me.txtSampleAudioCodec.Name = "txtSampleAudioCodec"
        Me.txtSampleAudioCodec.ReadOnly = true
        Me.txtSampleAudioCodec.Size = New System.Drawing.Size(156, 20)
        Me.txtSampleAudioCodec.TabIndex = 1
        Me.txtSampleAudioCodec.TabStop = false
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(37, 22)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(44, 13)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "Codec :"
        '
        'btnTestAnalyse
        '
        Me.btnTestAnalyse.Location = New System.Drawing.Point(413, 105)
        Me.btnTestAnalyse.Name = "btnTestAnalyse"
        Me.btnTestAnalyse.Size = New System.Drawing.Size(124, 31)
        Me.btnTestAnalyse.TabIndex = 4
        Me.btnTestAnalyse.Text = "Analyse"
        Me.btnTestAnalyse.UseVisualStyleBackColor = true
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtSampleVideo3D)
        Me.GroupBox1.Controls.Add(Me.Label40)
        Me.GroupBox1.Controls.Add(Me.txtSampleAspectRatio)
        Me.GroupBox1.Controls.Add(Me.Label107)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoResolution)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoFramerate)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoBitrate)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoCodec)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Location = New System.Drawing.Point(15, 151)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(522, 81)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Video ..."
        '
        'txtSampleVideo3D
        '
        Me.txtSampleVideo3D.Location = New System.Drawing.Point(252, 22)
        Me.txtSampleVideo3D.Name = "txtSampleVideo3D"
        Me.txtSampleVideo3D.ReadOnly = true
        Me.txtSampleVideo3D.Size = New System.Drawing.Size(68, 20)
        Me.txtSampleVideo3D.TabIndex = 12
        Me.txtSampleVideo3D.TabStop = false
        '
        'Label40
        '
        Me.Label40.AutoSize = true
        Me.Label40.Location = New System.Drawing.Point(182, 25)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(24, 13)
        Me.Label40.TabIndex = 10
        Me.Label40.Text = "3D:"
        '
        'txtSampleAspectRatio
        '
        Me.txtSampleAspectRatio.BackColor = System.Drawing.SystemColors.Control
        Me.txtSampleAspectRatio.Location = New System.Drawing.Point(400, 48)
        Me.txtSampleAspectRatio.Name = "txtSampleAspectRatio"
        Me.txtSampleAspectRatio.Size = New System.Drawing.Size(84, 20)
        Me.txtSampleAspectRatio.TabIndex = 9
        Me.txtSampleAspectRatio.TabStop = false
        '
        'Label107
        '
        Me.Label107.AutoSize = true
        Me.Label107.Location = New System.Drawing.Point(326, 51)
        Me.Label107.Name = "Label107"
        Me.Label107.Size = New System.Drawing.Size(68, 13)
        Me.Label107.TabIndex = 8
        Me.Label107.Text = "AspectRatio:"
        '
        'txtSampleVideoResolution
        '
        Me.txtSampleVideoResolution.Location = New System.Drawing.Point(400, 22)
        Me.txtSampleVideoResolution.Name = "txtSampleVideoResolution"
        Me.txtSampleVideoResolution.ReadOnly = true
        Me.txtSampleVideoResolution.Size = New System.Drawing.Size(84, 20)
        Me.txtSampleVideoResolution.TabIndex = 3
        Me.txtSampleVideoResolution.TabStop = false
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(331, 25)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(63, 13)
        Me.Label7.TabIndex = 2
        Me.Label7.Text = "Resolution :"
        '
        'txtSampleVideoFramerate
        '
        Me.txtSampleVideoFramerate.Location = New System.Drawing.Point(252, 48)
        Me.txtSampleVideoFramerate.Name = "txtSampleVideoFramerate"
        Me.txtSampleVideoFramerate.ReadOnly = true
        Me.txtSampleVideoFramerate.Size = New System.Drawing.Size(68, 20)
        Me.txtSampleVideoFramerate.TabIndex = 7
        Me.txtSampleVideoFramerate.TabStop = false
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(182, 51)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "FrameRate :"
        '
        'txtSampleVideoBitrate
        '
        Me.txtSampleVideoBitrate.Location = New System.Drawing.Point(86, 48)
        Me.txtSampleVideoBitrate.Name = "txtSampleVideoBitrate"
        Me.txtSampleVideoBitrate.ReadOnly = true
        Me.txtSampleVideoBitrate.Size = New System.Drawing.Size(88, 20)
        Me.txtSampleVideoBitrate.TabIndex = 5
        Me.txtSampleVideoBitrate.TabStop = false
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(37, 51)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Bitrate :"
        '
        'txtSampleVideoCodec
        '
        Me.txtSampleVideoCodec.Location = New System.Drawing.Point(87, 22)
        Me.txtSampleVideoCodec.Name = "txtSampleVideoCodec"
        Me.txtSampleVideoCodec.ReadOnly = true
        Me.txtSampleVideoCodec.Size = New System.Drawing.Size(87, 20)
        Me.txtSampleVideoCodec.TabIndex = 1
        Me.txtSampleVideoCodec.TabStop = false
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(36, 25)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(44, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Codec :"
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(20, 55)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(126, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Select sample movie file :"
        '
        'btnGetSampleFile
        '
        Me.btnGetSampleFile.Location = New System.Drawing.Point(499, 70)
        Me.btnGetSampleFile.Name = "btnGetSampleFile"
        Me.btnGetSampleFile.Size = New System.Drawing.Size(38, 20)
        Me.btnGetSampleFile.TabIndex = 3
        Me.btnGetSampleFile.Text = "..."
        Me.btnGetSampleFile.UseVisualStyleBackColor = true
        '
        'txtSampleFile
        '
        Me.txtSampleFile.Location = New System.Drawing.Point(22, 70)
        Me.txtSampleFile.Name = "txtSampleFile"
        Me.txtSampleFile.Size = New System.Drawing.Size(471, 20)
        Me.txtSampleFile.TabIndex = 2
        '
        'ViewCollection
        '
        Me.ViewCollection.Controls.Add(Me.GroupBoxMovieDetails)
        Me.ViewCollection.Controls.Add(Me.XionPanel1)
        Me.ViewCollection.Controls.Add(Me.MovieBindingNavigator)
        Me.ViewCollection.Controls.Add(Me.Label59)
        Me.ViewCollection.Location = New System.Drawing.Point(4, 22)
        Me.ViewCollection.Name = "ViewCollection"
        Me.ViewCollection.Padding = New System.Windows.Forms.Padding(3)
        Me.ViewCollection.Size = New System.Drawing.Size(600, 580)
        Me.ViewCollection.TabIndex = 6
        Me.ViewCollection.Text = "View Movies"
        Me.ViewCollection.UseVisualStyleBackColor = true
        '
        'GroupBoxMovieDetails
        '
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox40)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox39)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox38)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox37)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label103)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label102)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox33)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox32)
        Me.GroupBoxMovieDetails.Controls.Add(Me.PictureBox1)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox31)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox2)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label82)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label49)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox30)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox5)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label81)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label50)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label80)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox29)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox14)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label78)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox28)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox3)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label77)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label54)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox27)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox4)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label74)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label55)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox24)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox9)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label73)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label57)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox23)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox8)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox22)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label56)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox21)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label58)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox15)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox10)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label66)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label60)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox16)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox11)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label64)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label61)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox14)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox13)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label65)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label63)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox15)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox12)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label62)
        Me.GroupBoxMovieDetails.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupBoxMovieDetails.Location = New System.Drawing.Point(87, 28)
        Me.GroupBoxMovieDetails.Name = "GroupBoxMovieDetails"
        Me.GroupBoxMovieDetails.Size = New System.Drawing.Size(510, 549)
        Me.GroupBoxMovieDetails.TabIndex = 1
        Me.GroupBoxMovieDetails.TabStop = false
        '
        'TextBox40
        '
        Me.TextBox40.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Comments", true))
        Me.TextBox40.Location = New System.Drawing.Point(187, 430)
        Me.TextBox40.Multiline = true
        Me.TextBox40.Name = "TextBox40"
        Me.TextBox40.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox40.Size = New System.Drawing.Size(310, 56)
        Me.TextBox40.TabIndex = 55
        '
        'MovieBindingSource
        '
        Me.MovieBindingSource.DataSource = GetType(AMCUpdater.AntMovieCatalog.MovieDataTable)
        '
        'TextBox39
        '
        Me.TextBox39.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Description", true))
        Me.TextBox39.Location = New System.Drawing.Point(187, 329)
        Me.TextBox39.Multiline = true
        Me.TextBox39.Name = "TextBox39"
        Me.TextBox39.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox39.Size = New System.Drawing.Size(310, 93)
        Me.TextBox39.TabIndex = 54
        '
        'TextBox38
        '
        Me.TextBox38.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Actors", true))
        Me.TextBox38.Location = New System.Drawing.Point(227, 266)
        Me.TextBox38.Multiline = true
        Me.TextBox38.Name = "TextBox38"
        Me.TextBox38.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox38.Size = New System.Drawing.Size(270, 57)
        Me.TextBox38.TabIndex = 53
        '
        'TextBox37
        '
        Me.TextBox37.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "IMDB_Id", true))
        Me.TextBox37.Location = New System.Drawing.Point(241, 20)
        Me.TextBox37.Name = "TextBox37"
        Me.TextBox37.Size = New System.Drawing.Size(63, 20)
        Me.TextBox37.TabIndex = 2
        '
        'Label103
        '
        Me.Label103.AutoSize = true
        Me.Label103.Location = New System.Drawing.Point(326, 242)
        Me.Label103.Name = "Label103"
        Me.Label103.Size = New System.Drawing.Size(31, 13)
        Me.Label103.TabIndex = 32
        Me.Label103.Text = "Tags"
        '
        'Label102
        '
        Me.Label102.AutoSize = true
        Me.Label102.Location = New System.Drawing.Point(184, 242)
        Me.Label102.Name = "Label102"
        Me.Label102.Size = New System.Drawing.Size(37, 13)
        Me.Label102.TabIndex = 30
        Me.Label102.Text = "Studio"
        '
        'TextBox33
        '
        Me.TextBox33.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Studio", true))
        Me.TextBox33.Location = New System.Drawing.Point(227, 239)
        Me.TextBox33.Name = "TextBox33"
        Me.TextBox33.Size = New System.Drawing.Size(93, 20)
        Me.TextBox33.TabIndex = 31
        '
        'TextBox32
        '
        Me.TextBox32.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Tags", true))
        Me.TextBox32.Location = New System.Drawing.Point(376, 239)
        Me.TextBox32.Name = "TextBox32"
        Me.TextBox32.Size = New System.Drawing.Size(121, 20)
        Me.TextBox32.TabIndex = 33
        '
        'PictureBox1
        '
        Me.PictureBox1.DataBindings.Add(New System.Windows.Forms.Binding("ImageLocation", Me.MovieBindingSource, "Picture", true))
        Me.PictureBox1.Location = New System.Drawing.Point(6, 11)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(158, 222)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = false
        Me.ToolTip1.SetToolTip(Me.PictureBox1, "Klick to get bigger image ...")
        '
        'TextBox31
        '
        Me.TextBox31.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "TagLine", true))
        Me.TextBox31.Location = New System.Drawing.Point(227, 213)
        Me.TextBox31.Name = "TextBox31"
        Me.TextBox31.Size = New System.Drawing.Size(93, 20)
        Me.TextBox31.TabIndex = 27
        '
        'TextBox2
        '
        Me.TextBox2.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Number", true))
        Me.TextBox2.Location = New System.Drawing.Point(202, 20)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(33, 20)
        Me.TextBox2.TabIndex = 1
        '
        'Label82
        '
        Me.Label82.AutoSize = true
        Me.Label82.Location = New System.Drawing.Point(184, 216)
        Me.Label82.Name = "Label82"
        Me.Label82.Size = New System.Drawing.Size(42, 13)
        Me.Label82.TabIndex = 26
        Me.Label82.Text = "Tagline"
        '
        'Label49
        '
        Me.Label49.AutoSize = true
        Me.Label49.Location = New System.Drawing.Point(172, 23)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(24, 13)
        Me.Label49.TabIndex = 0
        Me.Label49.Text = "Nbr"
        '
        'TextBox30
        '
        Me.TextBox30.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Certification", true))
        Me.TextBox30.Location = New System.Drawing.Point(227, 187)
        Me.TextBox30.Name = "TextBox30"
        Me.TextBox30.Size = New System.Drawing.Size(93, 20)
        Me.TextBox30.TabIndex = 23
        '
        'TextBox5
        '
        Me.TextBox5.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Rating", true))
        Me.TextBox5.Location = New System.Drawing.Point(352, 20)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(35, 20)
        Me.TextBox5.TabIndex = 4
        '
        'Label81
        '
        Me.Label81.AutoSize = true
        Me.Label81.Location = New System.Drawing.Point(184, 190)
        Me.Label81.Name = "Label81"
        Me.Label81.Size = New System.Drawing.Size(26, 13)
        Me.Label81.TabIndex = 22
        Me.Label81.Text = "Cert"
        '
        'Label50
        '
        Me.Label50.AutoSize = true
        Me.Label50.Location = New System.Drawing.Point(311, 23)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(38, 13)
        Me.Label50.TabIndex = 3
        Me.Label50.Text = "Rating"
        '
        'Label80
        '
        Me.Label80.AutoSize = true
        Me.Label80.Location = New System.Drawing.Point(329, 216)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(35, 13)
        Me.Label80.TabIndex = 28
        Me.Label80.Text = "Writer"
        '
        'TextBox29
        '
        Me.TextBox29.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Writer", true))
        Me.TextBox29.Location = New System.Drawing.Point(376, 213)
        Me.TextBox29.Name = "TextBox29"
        Me.TextBox29.Size = New System.Drawing.Size(121, 20)
        Me.TextBox29.TabIndex = 29
        '
        'GroupBox14
        '
        Me.GroupBox14.Controls.Add(Me.TextBox36)
        Me.GroupBox14.Controls.Add(Me.Label53)
        Me.GroupBox14.Controls.Add(Me.Label52)
        Me.GroupBox14.Controls.Add(Me.Label51)
        Me.GroupBox14.Controls.Add(Me.TextBox7)
        Me.GroupBox14.Controls.Add(Me.TextBox6)
        Me.GroupBox14.Controls.Add(Me.TextBox1)
        Me.GroupBox14.Location = New System.Drawing.Point(172, 43)
        Me.GroupBox14.Name = "GroupBox14"
        Me.GroupBox14.Size = New System.Drawing.Size(236, 86)
        Me.GroupBox14.TabIndex = 7
        Me.GroupBox14.TabStop = false
        Me.GroupBox14.Text = "Titles"
        Me.ToolTip1.SetToolTip(Me.GroupBox14, "Shows all3 titles and the edition field")
        '
        'TextBox36
        '
        Me.TextBox36.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Edition", true))
        Me.TextBox36.Location = New System.Drawing.Point(154, 60)
        Me.TextBox36.Name = "TextBox36"
        Me.TextBox36.Size = New System.Drawing.Size(76, 20)
        Me.TextBox36.TabIndex = 6
        '
        'Label53
        '
        Me.Label53.AutoSize = true
        Me.Label53.Location = New System.Drawing.Point(6, 63)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(54, 13)
        Me.Label53.TabIndex = 4
        Me.Label53.Text = "Formatted"
        '
        'Label52
        '
        Me.Label52.AutoSize = true
        Me.Label52.Location = New System.Drawing.Point(6, 39)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(57, 13)
        Me.Label52.TabIndex = 2
        Me.Label52.Text = "Translated"
        '
        'Label51
        '
        Me.Label51.AutoSize = true
        Me.Label51.Location = New System.Drawing.Point(7, 15)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(42, 13)
        Me.Label51.TabIndex = 0
        Me.Label51.Text = "Original"
        '
        'TextBox7
        '
        Me.TextBox7.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "FormattedTitle", true))
        Me.TextBox7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox7.Location = New System.Drawing.Point(65, 60)
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.Size = New System.Drawing.Size(83, 20)
        Me.TextBox7.TabIndex = 5
        '
        'TextBox6
        '
        Me.TextBox6.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "TranslatedTitle", true))
        Me.TextBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox6.Location = New System.Drawing.Point(65, 36)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(165, 20)
        Me.TextBox6.TabIndex = 3
        '
        'TextBox1
        '
        Me.TextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "OriginalTitle", true))
        Me.TextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox1.Location = New System.Drawing.Point(65, 12)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(165, 20)
        Me.TextBox1.TabIndex = 1
        '
        'Label78
        '
        Me.Label78.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label78.AutoSize = true
        Me.Label78.Location = New System.Drawing.Point(179, 523)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(33, 13)
        Me.Label78.TabIndex = 42
        Me.Label78.Text = "Disks"
        '
        'TextBox28
        '
        Me.TextBox28.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox28.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Disks", true))
        Me.TextBox28.Location = New System.Drawing.Point(215, 520)
        Me.TextBox28.Name = "TextBox28"
        Me.TextBox28.Size = New System.Drawing.Size(36, 20)
        Me.TextBox28.TabIndex = 43
        '
        'TextBox3
        '
        Me.TextBox3.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Date", true))
        Me.TextBox3.Location = New System.Drawing.Point(430, 20)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(67, 20)
        Me.TextBox3.TabIndex = 6
        '
        'Label77
        '
        Me.Label77.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label77.AutoSize = true
        Me.Label77.Location = New System.Drawing.Point(179, 497)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(27, 13)
        Me.Label77.TabIndex = 38
        Me.Label77.Text = "Size"
        '
        'Label54
        '
        Me.Label54.AutoSize = true
        Me.Label54.Location = New System.Drawing.Point(394, 23)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(30, 13)
        Me.Label54.TabIndex = 5
        Me.Label54.Text = "Date"
        '
        'TextBox27
        '
        Me.TextBox27.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox27.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Size", true))
        Me.TextBox27.Location = New System.Drawing.Point(215, 494)
        Me.TextBox27.Name = "TextBox27"
        Me.TextBox27.Size = New System.Drawing.Size(36, 20)
        Me.TextBox27.TabIndex = 39
        '
        'TextBox4
        '
        Me.TextBox4.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Year", true))
        Me.TextBox4.Location = New System.Drawing.Point(457, 53)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(40, 20)
        Me.TextBox4.TabIndex = 9
        '
        'Label74
        '
        Me.Label74.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label74.AutoSize = true
        Me.Label74.Location = New System.Drawing.Point(10, 523)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(47, 13)
        Me.Label74.TabIndex = 51
        Me.Label74.Text = "Subtitles"
        Me.Label74.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label55
        '
        Me.Label55.AutoSize = true
        Me.Label55.Location = New System.Drawing.Point(413, 56)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(29, 13)
        Me.Label55.TabIndex = 8
        Me.Label55.Text = "Year"
        '
        'TextBox24
        '
        Me.TextBox24.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox24.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Subtitles", true))
        Me.TextBox24.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox24.Location = New System.Drawing.Point(70, 520)
        Me.TextBox24.Name = "TextBox24"
        Me.TextBox24.Size = New System.Drawing.Size(93, 20)
        Me.TextBox24.TabIndex = 52
        '
        'TextBox9
        '
        Me.TextBox9.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Checked", true))
        Me.TextBox9.Location = New System.Drawing.Point(457, 79)
        Me.TextBox9.Name = "TextBox9"
        Me.TextBox9.Size = New System.Drawing.Size(40, 20)
        Me.TextBox9.TabIndex = 11
        '
        'Label73
        '
        Me.Label73.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label73.AutoSize = true
        Me.Label73.Location = New System.Drawing.Point(7, 497)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(60, 13)
        Me.Label73.TabIndex = 49
        Me.Label73.Text = "Languages"
        Me.Label73.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label57
        '
        Me.Label57.AutoSize = true
        Me.Label57.Location = New System.Drawing.Point(413, 82)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(32, 13)
        Me.Label57.TabIndex = 10
        Me.Label57.Text = "Chkd"
        Me.Label57.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox23
        '
        Me.TextBox23.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox23.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Languages", true))
        Me.TextBox23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox23.Location = New System.Drawing.Point(70, 494)
        Me.TextBox23.Name = "TextBox23"
        Me.TextBox23.Size = New System.Drawing.Size(93, 20)
        Me.TextBox23.TabIndex = 50
        '
        'TextBox8
        '
        Me.TextBox8.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Length", true))
        Me.TextBox8.Location = New System.Drawing.Point(457, 105)
        Me.TextBox8.Name = "TextBox8"
        Me.TextBox8.Size = New System.Drawing.Size(40, 20)
        Me.TextBox8.TabIndex = 13
        '
        'GroupBox22
        '
        Me.GroupBox22.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.GroupBox22.Controls.Add(Me.Label75)
        Me.GroupBox22.Controls.Add(Me.Label76)
        Me.GroupBox22.Controls.Add(Me.TextBox25)
        Me.GroupBox22.Controls.Add(Me.TextBox26)
        Me.GroupBox22.Location = New System.Drawing.Point(11, 424)
        Me.GroupBox22.Name = "GroupBox22"
        Me.GroupBox22.Size = New System.Drawing.Size(152, 62)
        Me.GroupBox22.TabIndex = 48
        Me.GroupBox22.TabStop = false
        Me.GroupBox22.Text = "Audio"
        '
        'Label75
        '
        Me.Label75.AutoSize = true
        Me.Label75.Location = New System.Drawing.Point(9, 39)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(37, 13)
        Me.Label75.TabIndex = 2
        Me.Label75.Text = "Bitrate"
        Me.Label75.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label76
        '
        Me.Label76.AutoSize = true
        Me.Label76.Location = New System.Drawing.Point(6, 16)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(39, 13)
        Me.Label76.TabIndex = 0
        Me.Label76.Text = "Format"
        Me.Label76.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox25
        '
        Me.TextBox25.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "AudioFormat", true))
        Me.TextBox25.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox25.Location = New System.Drawing.Point(59, 13)
        Me.TextBox25.Name = "TextBox25"
        Me.TextBox25.Size = New System.Drawing.Size(86, 20)
        Me.TextBox25.TabIndex = 1
        '
        'TextBox26
        '
        Me.TextBox26.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "AudioBitrate", true))
        Me.TextBox26.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox26.Location = New System.Drawing.Point(59, 36)
        Me.TextBox26.Name = "TextBox26"
        Me.TextBox26.Size = New System.Drawing.Size(86, 20)
        Me.TextBox26.TabIndex = 3
        '
        'Label56
        '
        Me.Label56.AutoSize = true
        Me.Label56.Location = New System.Drawing.Point(411, 108)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(40, 13)
        Me.Label56.TabIndex = 12
        Me.Label56.Text = "Length"
        Me.Label56.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'GroupBox21
        '
        Me.GroupBox21.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.GroupBox21.Controls.Add(Me.Label72)
        Me.GroupBox21.Controls.Add(Me.TextBox22)
        Me.GroupBox21.Controls.Add(Me.Label71)
        Me.GroupBox21.Controls.Add(Me.TextBox21)
        Me.GroupBox21.Controls.Add(Me.Label69)
        Me.GroupBox21.Controls.Add(Me.Label70)
        Me.GroupBox21.Controls.Add(Me.TextBox19)
        Me.GroupBox21.Controls.Add(Me.TextBox20)
        Me.GroupBox21.Location = New System.Drawing.Point(11, 316)
        Me.GroupBox21.Name = "GroupBox21"
        Me.GroupBox21.Size = New System.Drawing.Size(152, 106)
        Me.GroupBox21.TabIndex = 47
        Me.GroupBox21.TabStop = false
        Me.GroupBox21.Text = "Video"
        '
        'Label72
        '
        Me.Label72.AutoSize = true
        Me.Label72.Location = New System.Drawing.Point(3, 84)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(54, 13)
        Me.Label72.TabIndex = 6
        Me.Label72.Text = "Framerate"
        Me.Label72.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox22
        '
        Me.TextBox22.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Framerate", true))
        Me.TextBox22.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox22.Location = New System.Drawing.Point(59, 82)
        Me.TextBox22.Name = "TextBox22"
        Me.TextBox22.Size = New System.Drawing.Size(86, 20)
        Me.TextBox22.TabIndex = 7
        '
        'Label71
        '
        Me.Label71.AutoSize = true
        Me.Label71.Location = New System.Drawing.Point(1, 62)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(57, 13)
        Me.Label71.TabIndex = 4
        Me.Label71.Text = "Resolution"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox21
        '
        Me.TextBox21.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Resolution", true))
        Me.TextBox21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox21.Location = New System.Drawing.Point(59, 59)
        Me.TextBox21.Name = "TextBox21"
        Me.TextBox21.Size = New System.Drawing.Size(86, 20)
        Me.TextBox21.TabIndex = 5
        '
        'Label69
        '
        Me.Label69.AutoSize = true
        Me.Label69.Location = New System.Drawing.Point(9, 39)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(37, 13)
        Me.Label69.TabIndex = 2
        Me.Label69.Text = "Bitrate"
        Me.Label69.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label70
        '
        Me.Label70.AutoSize = true
        Me.Label70.Location = New System.Drawing.Point(6, 16)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(39, 13)
        Me.Label70.TabIndex = 0
        Me.Label70.Text = "Format"
        Me.Label70.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox19
        '
        Me.TextBox19.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "VideoFormat", true))
        Me.TextBox19.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox19.Location = New System.Drawing.Point(59, 13)
        Me.TextBox19.Name = "TextBox19"
        Me.TextBox19.Size = New System.Drawing.Size(86, 20)
        Me.TextBox19.TabIndex = 1
        '
        'TextBox20
        '
        Me.TextBox20.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "VideoBitrate", true))
        Me.TextBox20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox20.Location = New System.Drawing.Point(59, 36)
        Me.TextBox20.Name = "TextBox20"
        Me.TextBox20.Size = New System.Drawing.Size(86, 20)
        Me.TextBox20.TabIndex = 3
        '
        'Label58
        '
        Me.Label58.AutoSize = true
        Me.Label58.Location = New System.Drawing.Point(184, 266)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(37, 13)
        Me.Label58.TabIndex = 34
        Me.Label58.Text = "Actors"
        '
        'GroupBox15
        '
        Me.GroupBox15.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.GroupBox15.Controls.Add(Me.Label68)
        Me.GroupBox15.Controls.Add(Me.Label67)
        Me.GroupBox15.Controls.Add(Me.TextBox17)
        Me.GroupBox15.Controls.Add(Me.TextBox18)
        Me.GroupBox15.Location = New System.Drawing.Point(10, 251)
        Me.GroupBox15.Name = "GroupBox15"
        Me.GroupBox15.Size = New System.Drawing.Size(153, 59)
        Me.GroupBox15.TabIndex = 46
        Me.GroupBox15.TabStop = false
        Me.GroupBox15.Text = "Media"
        '
        'Label68
        '
        Me.Label68.AutoSize = true
        Me.Label68.Location = New System.Drawing.Point(9, 39)
        Me.Label68.Name = "Label68"
        Me.Label68.Size = New System.Drawing.Size(31, 13)
        Me.Label68.TabIndex = 2
        Me.Label68.Text = "Type"
        '
        'Label67
        '
        Me.Label67.AutoSize = true
        Me.Label67.Location = New System.Drawing.Point(9, 15)
        Me.Label67.Name = "Label67"
        Me.Label67.Size = New System.Drawing.Size(33, 13)
        Me.Label67.TabIndex = 0
        Me.Label67.Text = "Label"
        '
        'TextBox17
        '
        Me.TextBox17.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "MediaLabel", true))
        Me.TextBox17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox17.Location = New System.Drawing.Point(53, 12)
        Me.TextBox17.Name = "TextBox17"
        Me.TextBox17.Size = New System.Drawing.Size(93, 20)
        Me.TextBox17.TabIndex = 1
        '
        'TextBox18
        '
        Me.TextBox18.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "MediaType", true))
        Me.TextBox18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox18.Location = New System.Drawing.Point(53, 36)
        Me.TextBox18.Name = "TextBox18"
        Me.TextBox18.Size = New System.Drawing.Size(93, 20)
        Me.TextBox18.TabIndex = 3
        '
        'TextBox10
        '
        Me.TextBox10.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Director", true))
        Me.TextBox10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox10.Location = New System.Drawing.Point(376, 135)
        Me.TextBox10.Name = "TextBox10"
        Me.TextBox10.Size = New System.Drawing.Size(121, 20)
        Me.TextBox10.TabIndex = 17
        '
        'Label66
        '
        Me.Label66.AutoSize = true
        Me.Label66.Location = New System.Drawing.Point(326, 190)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(49, 13)
        Me.Label66.TabIndex = 24
        Me.Label66.Text = "Borrower"
        '
        'Label60
        '
        Me.Label60.AutoSize = true
        Me.Label60.Location = New System.Drawing.Point(326, 138)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(44, 13)
        Me.Label60.TabIndex = 16
        Me.Label60.Text = "Director"
        '
        'TextBox16
        '
        Me.TextBox16.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Borrower", true))
        Me.TextBox16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox16.Location = New System.Drawing.Point(376, 187)
        Me.TextBox16.Name = "TextBox16"
        Me.TextBox16.Size = New System.Drawing.Size(121, 20)
        Me.TextBox16.TabIndex = 25
        '
        'TextBox11
        '
        Me.TextBox11.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Producer", true))
        Me.TextBox11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox11.Location = New System.Drawing.Point(376, 161)
        Me.TextBox11.Name = "TextBox11"
        Me.TextBox11.Size = New System.Drawing.Size(121, 20)
        Me.TextBox11.TabIndex = 21
        '
        'Label64
        '
        Me.Label64.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label64.AutoSize = true
        Me.Label64.Location = New System.Drawing.Point(264, 523)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(29, 13)
        Me.Label64.TabIndex = 44
        Me.Label64.Text = "URL"
        '
        'Label61
        '
        Me.Label61.AutoSize = true
        Me.Label61.Location = New System.Drawing.Point(326, 164)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(50, 13)
        Me.Label61.TabIndex = 20
        Me.Label61.Text = "Producer"
        '
        'TextBox14
        '
        Me.TextBox14.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox14.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "URL", true))
        Me.TextBox14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox14.Location = New System.Drawing.Point(299, 520)
        Me.TextBox14.Name = "TextBox14"
        Me.TextBox14.Size = New System.Drawing.Size(198, 20)
        Me.TextBox14.TabIndex = 45
        '
        'TextBox13
        '
        Me.TextBox13.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Country", true))
        Me.TextBox13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox13.Location = New System.Drawing.Point(227, 161)
        Me.TextBox13.Name = "TextBox13"
        Me.TextBox13.Size = New System.Drawing.Size(93, 20)
        Me.TextBox13.TabIndex = 19
        '
        'Label65
        '
        Me.Label65.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label65.AutoSize = true
        Me.Label65.Location = New System.Drawing.Point(254, 497)
        Me.Label65.Name = "Label65"
        Me.Label65.Size = New System.Drawing.Size(41, 13)
        Me.Label65.TabIndex = 40
        Me.Label65.Text = "Source"
        '
        'Label63
        '
        Me.Label63.AutoSize = true
        Me.Label63.Location = New System.Drawing.Point(172, 164)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(43, 13)
        Me.Label63.TabIndex = 18
        Me.Label63.Text = "Country"
        '
        'TextBox15
        '
        Me.TextBox15.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TextBox15.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Source", true))
        Me.TextBox15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox15.Location = New System.Drawing.Point(299, 494)
        Me.TextBox15.Name = "TextBox15"
        Me.TextBox15.Size = New System.Drawing.Size(198, 20)
        Me.TextBox15.TabIndex = 41
        '
        'TextBox12
        '
        Me.TextBox12.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.MovieBindingSource, "Category", true))
        Me.TextBox12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.TextBox12.Location = New System.Drawing.Point(227, 135)
        Me.TextBox12.Name = "TextBox12"
        Me.TextBox12.Size = New System.Drawing.Size(93, 20)
        Me.TextBox12.TabIndex = 15
        '
        'Label62
        '
        Me.Label62.AutoSize = true
        Me.Label62.Location = New System.Drawing.Point(172, 138)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(49, 13)
        Me.Label62.TabIndex = 14
        Me.Label62.Text = "Category"
        '
        'XionPanel1
        '
        Me.XionPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.XionPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.XionPanel1.Controls.Add(Me.DataGridViewMovie)
        Me.XionPanel1.Location = New System.Drawing.Point(3, 28)
        Me.XionPanel1.Movable = false
        Me.XionPanel1.Name = "XionPanel1"
        Me.XionPanel1.Orientation = XionControls.XionPanel.PanelOrientation.Horizontal
        Me.XionPanel1.Padding = New System.Windows.Forms.Padding(0, 22, 0, 0)
        Me.XionPanel1.Sizable = true
        Me.XionPanel1.Size = New System.Drawing.Size(84, 549)
        Me.XionPanel1.State = XionControls.XionPanel.PanelState.Expand
        Me.XionPanel1.TabIndex = 0
        Me.XionPanel1.Text = "Movie List"
        Me.XionPanel1.Title = "Movie List"
        Me.XionPanel1.TitleBackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.XionPanel1.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.XionPanel1.TitleHeight = 22
        '
        'DataGridViewMovie
        '
        Me.DataGridViewMovie.AutoGenerateColumns = false
        Me.DataGridViewMovie.ColumnHeadersHeight = 21
        Me.DataGridViewMovie.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridViewMovie.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn5, Me.Category, Me.Country, Me.Rating})
        Me.DataGridViewMovie.DataSource = Me.MovieBindingSource
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewMovie.DefaultCellStyle = DataGridViewCellStyle11
        Me.DataGridViewMovie.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewMovie.Location = New System.Drawing.Point(0, 22)
        Me.DataGridViewMovie.Name = "DataGridViewMovie"
        Me.DataGridViewMovie.ReadOnly = true
        Me.DataGridViewMovie.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridViewMovie.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.DataGridViewMovie.RowHeadersVisible = false
        Me.DataGridViewMovie.RowHeadersWidth = 20
        Me.DataGridViewMovie.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewMovie.RowTemplate.Height = 16
        Me.DataGridViewMovie.Size = New System.Drawing.Size(80, 523)
        Me.DataGridViewMovie.TabIndex = 0
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "Number"
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn1.DefaultCellStyle = DataGridViewCellStyle8
        Me.DataGridViewTextBoxColumn1.HeaderText = "N°"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = true
        Me.DataGridViewTextBoxColumn1.Width = 35
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Year"
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn4.DefaultCellStyle = DataGridViewCellStyle9
        Me.DataGridViewTextBoxColumn4.HeaderText = "Year"
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        Me.DataGridViewTextBoxColumn4.ReadOnly = true
        Me.DataGridViewTextBoxColumn4.Width = 35
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "OriginalTitle"
        Me.DataGridViewTextBoxColumn2.HeaderText = "Original Title"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = true
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "TranslatedTitle"
        Me.DataGridViewTextBoxColumn3.HeaderText = "Translated Title"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = true
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.DataPropertyName = "DateAdded"
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn5.DefaultCellStyle = DataGridViewCellStyle10
        Me.DataGridViewTextBoxColumn5.HeaderText = "Date Added"
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        Me.DataGridViewTextBoxColumn5.ReadOnly = true
        Me.DataGridViewTextBoxColumn5.Width = 66
        '
        'Category
        '
        Me.Category.DataPropertyName = "Category"
        Me.Category.HeaderText = "Category"
        Me.Category.Name = "Category"
        Me.Category.ReadOnly = true
        Me.Category.Width = 110
        '
        'Country
        '
        Me.Country.DataPropertyName = "Country"
        Me.Country.HeaderText = "Country"
        Me.Country.Name = "Country"
        Me.Country.ReadOnly = true
        Me.Country.Width = 70
        '
        'Rating
        '
        Me.Rating.DataPropertyName = "Rating"
        Me.Rating.HeaderText = "Rating"
        Me.Rating.Name = "Rating"
        Me.Rating.ReadOnly = true
        Me.Rating.Width = 50
        '
        'MovieBindingNavigator
        '
        Me.MovieBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.MovieBindingNavigator.BindingSource = Me.MovieBindingSource
        Me.MovieBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.MovieBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.MovieBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.ToolStripSeparator3, Me.VidéoBindingNavigatorSaveItem, Me.ToolStripSeparator6, Me.BindingNavigatorUpdate})
        Me.MovieBindingNavigator.Location = New System.Drawing.Point(3, 3)
        Me.MovieBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.MovieBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.MovieBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.MovieBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.MovieBindingNavigator.Name = "MovieBindingNavigator"
        Me.MovieBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.MovieBindingNavigator.Size = New System.Drawing.Size(594, 25)
        Me.MovieBindingNavigator.TabIndex = 0
        Me.MovieBindingNavigator.Text = "BindingNavigator1"
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorAddNewItem.Enabled = false
        Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorAddNewItem.Text = "Add Entry"
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(44, 22)
        Me.BindingNavigatorCountItem.Text = "von {0}"
        Me.BindingNavigatorCountItem.ToolTipText = "Total Number of entries"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.CheckOnClick = true
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorDeleteItem.Text = "Delete Entry"
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveFirstItem.Text = "Move First"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMovePreviousItem.Text = "Move Previous"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorPositionItem
        '
        Me.BindingNavigatorPositionItem.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem.AutoSize = false
        Me.BindingNavigatorPositionItem.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(70, 18)
        Me.BindingNavigatorPositionItem.Text = "0"
        Me.BindingNavigatorPositionItem.ToolTipText = "Current Position"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveNextItem.Text = "Move Next"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveLastItem.Text = "Move Last"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'VidéoBindingNavigatorSaveItem
        '
        Me.VidéoBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.VidéoBindingNavigatorSaveItem.Image = CType(resources.GetObject("VidéoBindingNavigatorSaveItem.Image"),System.Drawing.Image)
        Me.VidéoBindingNavigatorSaveItem.Name = "VidéoBindingNavigatorSaveItem"
        Me.VidéoBindingNavigatorSaveItem.Size = New System.Drawing.Size(23, 22)
        Me.VidéoBindingNavigatorSaveItem.Text = "Save Datas"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorUpdate
        '
        Me.BindingNavigatorUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.BindingNavigatorUpdate.Image = CType(resources.GetObject("BindingNavigatorUpdate.Image"),System.Drawing.Image)
        Me.BindingNavigatorUpdate.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BindingNavigatorUpdate.Name = "BindingNavigatorUpdate"
        Me.BindingNavigatorUpdate.Size = New System.Drawing.Size(85, 22)
        Me.BindingNavigatorUpdate.Text = "Update Movie"
        '
        'Label59
        '
        Me.Label59.AutoSize = true
        Me.Label59.Location = New System.Drawing.Point(-55, 223)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(42, 13)
        Me.Label59.TabIndex = 28
        Me.Label59.Text = "Original"
        '
        'ViewPersons
        '
        Me.ViewPersons.Controls.Add(Me.GroupBoxPersonInfo)
        Me.ViewPersons.Controls.Add(Me.XionPanelPerson)
        Me.ViewPersons.Controls.Add(Me.PersonBindingNavigator)
        Me.ViewPersons.Location = New System.Drawing.Point(4, 22)
        Me.ViewPersons.Name = "ViewPersons"
        Me.ViewPersons.Size = New System.Drawing.Size(600, 580)
        Me.ViewPersons.TabIndex = 7
        Me.ViewPersons.Text = "View Persons"
        Me.ViewPersons.UseVisualStyleBackColor = true
        '
        'GroupBoxPersonInfo
        '
        Me.GroupBoxPersonInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GroupBoxPersonInfo.Controls.Add(Me.GroupBox5)
        Me.GroupBoxPersonInfo.Controls.Add(Me.CheckBoxWriter)
        Me.GroupBoxPersonInfo.Controls.Add(Me.CheckBoxDirector)
        Me.GroupBoxPersonInfo.Controls.Add(Me.CheckBoxProducer)
        Me.GroupBoxPersonInfo.Controls.Add(Me.CheckBoxActor)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label99)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label98)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label97)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label96)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_Photos)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_URL)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_TMDB_Id)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_IMDB_Id)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label95)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label94)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label93)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_Birthplace)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_Birthday)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_MiniBio)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_Name)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_Biography)
        Me.GroupBoxPersonInfo.Controls.Add(Me.PictureBox2)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label91)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label92)
        Me.GroupBoxPersonInfo.Controls.Add(Me.Label90)
        Me.GroupBoxPersonInfo.Controls.Add(Me.ViewPersons_OtherName)
        Me.GroupBoxPersonInfo.Location = New System.Drawing.Point(206, 31)
        Me.GroupBoxPersonInfo.Name = "GroupBoxPersonInfo"
        Me.GroupBoxPersonInfo.Size = New System.Drawing.Size(394, 546)
        Me.GroupBoxPersonInfo.TabIndex = 2
        Me.GroupBoxPersonInfo.TabStop = false
        Me.GroupBoxPersonInfo.Text = "Person Info ..."
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.CheckBox1)
        Me.GroupBox5.Controls.Add(Me.TextBox34)
        Me.GroupBox5.Controls.Add(Me.Button3)
        Me.GroupBox5.Controls.Add(Me.Label104)
        Me.GroupBox5.Controls.Add(Me.Button4)
        Me.GroupBox5.Controls.Add(Me.ComboBox1)
        Me.GroupBox5.Controls.Add(Me.TextBox35)
        Me.GroupBox5.Controls.Add(Me.Label105)
        Me.GroupBox5.Location = New System.Drawing.Point(9, 462)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(379, 79)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = false
        Me.GroupBox5.Text = "Internet Grabber Options ..."
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = true
        Me.CheckBox1.Location = New System.Drawing.Point(268, 49)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(71, 17)
        Me.CheckBox1.TabIndex = 6
        Me.CheckBox1.Text = "Don't ask"
        Me.CheckBox1.UseVisualStyleBackColor = true
        '
        'TextBox34
        '
        Me.TextBox34.Enabled = false
        Me.TextBox34.Location = New System.Drawing.Point(97, 19)
        Me.TextBox34.Name = "TextBox34"
        Me.TextBox34.Size = New System.Drawing.Size(111, 20)
        Me.TextBox34.TabIndex = 1
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(268, 18)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(94, 21)
        Me.Button3.TabIndex = 3
        Me.Button3.Text = "Grabber Options"
        Me.Button3.UseVisualStyleBackColor = true
        '
        'Label104
        '
        Me.Label104.AutoSize = true
        Me.Label104.Location = New System.Drawing.Point(6, 44)
        Me.Label104.Name = "Label104"
        Me.Label104.Size = New System.Drawing.Size(85, 26)
        Me.Label104.TabIndex = 4
        Me.Label104.Text = "Internet Lookup "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Behaviour"
        '
        'Button4
        '
        Me.Button4.CausesValidation = false
        Me.Button4.Location = New System.Drawing.Point(217, 19)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(36, 20)
        Me.Button4.TabIndex = 2
        Me.Button4.Text = "..."
        Me.Button4.UseVisualStyleBackColor = true
        '
        'ComboBox1
        '
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.FormattingEnabled = true
        Me.ComboBox1.Location = New System.Drawing.Point(97, 47)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(156, 21)
        Me.ComboBox1.TabIndex = 5
        '
        'TextBox35
        '
        Me.TextBox35.Location = New System.Drawing.Point(97, 19)
        Me.TextBox35.Name = "TextBox35"
        Me.TextBox35.Size = New System.Drawing.Size(156, 20)
        Me.TextBox35.TabIndex = 46
        Me.TextBox35.Visible = false
        '
        'Label105
        '
        Me.Label105.AutoSize = true
        Me.Label105.Location = New System.Drawing.Point(6, 22)
        Me.Label105.Name = "Label105"
        Me.Label105.Size = New System.Drawing.Size(75, 13)
        Me.Label105.TabIndex = 0
        Me.Label105.Text = "Grabber Script"
        '
        'CheckBoxWriter
        '
        Me.CheckBoxWriter.AutoSize = true
        Me.CheckBoxWriter.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.PersonBindingSource, "IsWriter", true))
        Me.CheckBoxWriter.Location = New System.Drawing.Point(248, 218)
        Me.CheckBoxWriter.Name = "CheckBoxWriter"
        Me.CheckBoxWriter.Size = New System.Drawing.Size(54, 17)
        Me.CheckBoxWriter.TabIndex = 12
        Me.CheckBoxWriter.Text = "Writer"
        Me.CheckBoxWriter.UseVisualStyleBackColor = true
        '
        'PersonBindingSource
        '
        Me.PersonBindingSource.DataSource = GetType(AMCUpdater.AntMovieCatalog.PersonDataTable)
        '
        'CheckBoxDirector
        '
        Me.CheckBoxDirector.AutoSize = true
        Me.CheckBoxDirector.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.PersonBindingSource, "IsDirector", true))
        Me.CheckBoxDirector.Location = New System.Drawing.Point(248, 195)
        Me.CheckBoxDirector.Name = "CheckBoxDirector"
        Me.CheckBoxDirector.Size = New System.Drawing.Size(63, 17)
        Me.CheckBoxDirector.TabIndex = 11
        Me.CheckBoxDirector.Text = "Director"
        Me.CheckBoxDirector.UseVisualStyleBackColor = true
        '
        'CheckBoxProducer
        '
        Me.CheckBoxProducer.AutoSize = true
        Me.CheckBoxProducer.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.PersonBindingSource, "IsProducer", true))
        Me.CheckBoxProducer.Location = New System.Drawing.Point(248, 172)
        Me.CheckBoxProducer.Name = "CheckBoxProducer"
        Me.CheckBoxProducer.Size = New System.Drawing.Size(69, 17)
        Me.CheckBoxProducer.TabIndex = 9
        Me.CheckBoxProducer.Text = "Producer"
        Me.CheckBoxProducer.UseVisualStyleBackColor = true
        '
        'CheckBoxActor
        '
        Me.CheckBoxActor.AutoSize = true
        Me.CheckBoxActor.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.PersonBindingSource, "IsActor", true))
        Me.CheckBoxActor.Location = New System.Drawing.Point(248, 149)
        Me.CheckBoxActor.Name = "CheckBoxActor"
        Me.CheckBoxActor.Size = New System.Drawing.Size(51, 17)
        Me.CheckBoxActor.TabIndex = 8
        Me.CheckBoxActor.Text = "Actor"
        Me.CheckBoxActor.UseVisualStyleBackColor = true
        '
        'Label99
        '
        Me.Label99.AutoSize = true
        Me.Label99.Location = New System.Drawing.Point(6, 387)
        Me.Label99.Name = "Label99"
        Me.Label99.Size = New System.Drawing.Size(29, 13)
        Me.Label99.TabIndex = 17
        Me.Label99.Text = "URL"
        '
        'Label98
        '
        Me.Label98.AutoSize = true
        Me.Label98.Location = New System.Drawing.Point(68, 414)
        Me.Label98.Name = "Label98"
        Me.Label98.Size = New System.Drawing.Size(49, 13)
        Me.Label98.TabIndex = 19
        Me.Label98.Text = "IMDB_Id"
        '
        'Label97
        '
        Me.Label97.AutoSize = true
        Me.Label97.Location = New System.Drawing.Point(229, 413)
        Me.Label97.Name = "Label97"
        Me.Label97.Size = New System.Drawing.Size(53, 13)
        Me.Label97.TabIndex = 21
        Me.Label97.Text = "TMDB_Id"
        '
        'Label96
        '
        Me.Label96.AutoSize = true
        Me.Label96.Location = New System.Drawing.Point(6, 439)
        Me.Label96.Name = "Label96"
        Me.Label96.Size = New System.Drawing.Size(40, 13)
        Me.Label96.TabIndex = 23
        Me.Label96.Text = "Photos"
        '
        'ViewPersons_Photos
        '
        Me.ViewPersons_Photos.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "Photos", true))
        Me.ViewPersons_Photos.Location = New System.Drawing.Point(66, 436)
        Me.ViewPersons_Photos.Name = "ViewPersons_Photos"
        Me.ViewPersons_Photos.Size = New System.Drawing.Size(322, 20)
        Me.ViewPersons_Photos.TabIndex = 24
        '
        'ViewPersons_URL
        '
        Me.ViewPersons_URL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "URL", true))
        Me.ViewPersons_URL.Location = New System.Drawing.Point(66, 384)
        Me.ViewPersons_URL.Name = "ViewPersons_URL"
        Me.ViewPersons_URL.Size = New System.Drawing.Size(322, 20)
        Me.ViewPersons_URL.TabIndex = 18
        '
        'ViewPersons_TMDB_Id
        '
        Me.ViewPersons_TMDB_Id.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "TMDB_Id", true))
        Me.ViewPersons_TMDB_Id.Location = New System.Drawing.Point(288, 411)
        Me.ViewPersons_TMDB_Id.Name = "ViewPersons_TMDB_Id"
        Me.ViewPersons_TMDB_Id.Size = New System.Drawing.Size(100, 20)
        Me.ViewPersons_TMDB_Id.TabIndex = 22
        '
        'ViewPersons_IMDB_Id
        '
        Me.ViewPersons_IMDB_Id.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "IMDB_Id", true))
        Me.ViewPersons_IMDB_Id.Location = New System.Drawing.Point(123, 410)
        Me.ViewPersons_IMDB_Id.Name = "ViewPersons_IMDB_Id"
        Me.ViewPersons_IMDB_Id.Size = New System.Drawing.Size(100, 20)
        Me.ViewPersons_IMDB_Id.TabIndex = 20
        '
        'Label95
        '
        Me.Label95.AutoSize = true
        Me.Label95.Location = New System.Drawing.Point(6, 245)
        Me.Label95.Name = "Label95"
        Me.Label95.Size = New System.Drawing.Size(54, 26)
        Me.Label95.TabIndex = 13
        Me.Label95.Text = "Mini "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Biography"
        '
        'Label94
        '
        Me.Label94.AutoSize = true
        Me.Label94.Location = New System.Drawing.Point(184, 126)
        Me.Label94.Name = "Label94"
        Me.Label94.Size = New System.Drawing.Size(58, 13)
        Me.Label94.TabIndex = 6
        Me.Label94.Text = "Birth Place"
        '
        'Label93
        '
        Me.Label93.AutoSize = true
        Me.Label93.Location = New System.Drawing.Point(184, 100)
        Me.Label93.Name = "Label93"
        Me.Label93.Size = New System.Drawing.Size(54, 13)
        Me.Label93.TabIndex = 4
        Me.Label93.Text = "Birth Date"
        '
        'ViewPersons_Birthplace
        '
        Me.ViewPersons_Birthplace.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "BirthPlace", true))
        Me.ViewPersons_Birthplace.Location = New System.Drawing.Point(248, 123)
        Me.ViewPersons_Birthplace.Name = "ViewPersons_Birthplace"
        Me.ViewPersons_Birthplace.Size = New System.Drawing.Size(138, 20)
        Me.ViewPersons_Birthplace.TabIndex = 7
        '
        'ViewPersons_Birthday
        '
        Me.ViewPersons_Birthday.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "Born", true))
        Me.ViewPersons_Birthday.Location = New System.Drawing.Point(248, 97)
        Me.ViewPersons_Birthday.Name = "ViewPersons_Birthday"
        Me.ViewPersons_Birthday.Size = New System.Drawing.Size(100, 20)
        Me.ViewPersons_Birthday.TabIndex = 5
        '
        'ViewPersons_MiniBio
        '
        Me.ViewPersons_MiniBio.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "MiniBiography", true))
        Me.ViewPersons_MiniBio.Location = New System.Drawing.Point(66, 245)
        Me.ViewPersons_MiniBio.Name = "ViewPersons_MiniBio"
        Me.ViewPersons_MiniBio.Size = New System.Drawing.Size(320, 44)
        Me.ViewPersons_MiniBio.TabIndex = 14
        '
        'ViewPersons_Name
        '
        Me.ViewPersons_Name.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "Name", true))
        Me.ViewPersons_Name.Location = New System.Drawing.Point(248, 24)
        Me.ViewPersons_Name.Name = "ViewPersons_Name"
        Me.ViewPersons_Name.Size = New System.Drawing.Size(138, 20)
        Me.ViewPersons_Name.TabIndex = 1
        '
        'ViewPersons_Biography
        '
        Me.ViewPersons_Biography.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "Biography", true))
        Me.ViewPersons_Biography.Location = New System.Drawing.Point(66, 298)
        Me.ViewPersons_Biography.Name = "ViewPersons_Biography"
        Me.ViewPersons_Biography.Size = New System.Drawing.Size(320, 80)
        Me.ViewPersons_Biography.TabIndex = 16
        '
        'PictureBox2
        '
        Me.PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox2.DataBindings.Add(New System.Windows.Forms.Binding("ImageLocation", Me.PersonBindingSource, "Picture", true))
        Me.PictureBox2.Location = New System.Drawing.Point(9, 19)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(166, 220)
        Me.PictureBox2.TabIndex = 1
        Me.PictureBox2.TabStop = false
        '
        'Label91
        '
        Me.Label91.AutoSize = true
        Me.Label91.Location = New System.Drawing.Point(6, 298)
        Me.Label91.Name = "Label91"
        Me.Label91.Size = New System.Drawing.Size(54, 13)
        Me.Label91.TabIndex = 15
        Me.Label91.Text = "Biography"
        '
        'Label92
        '
        Me.Label92.AutoSize = true
        Me.Label92.Location = New System.Drawing.Point(184, 62)
        Me.Label92.Name = "Label92"
        Me.Label92.Size = New System.Drawing.Size(52, 26)
        Me.Label92.TabIndex = 2
        Me.Label92.Text = "Alternate "&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"Name"
        '
        'Label90
        '
        Me.Label90.AutoSize = true
        Me.Label90.Location = New System.Drawing.Point(184, 27)
        Me.Label90.Name = "Label90"
        Me.Label90.Size = New System.Drawing.Size(35, 13)
        Me.Label90.TabIndex = 0
        Me.Label90.Text = "Name"
        '
        'ViewPersons_OtherName
        '
        Me.ViewPersons_OtherName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PersonBindingSource, "AlternateName", true))
        Me.ViewPersons_OtherName.Location = New System.Drawing.Point(248, 66)
        Me.ViewPersons_OtherName.Name = "ViewPersons_OtherName"
        Me.ViewPersons_OtherName.Size = New System.Drawing.Size(138, 20)
        Me.ViewPersons_OtherName.TabIndex = 3
        '
        'XionPanelPerson
        '
        Me.XionPanelPerson.Controls.Add(Me.DataGridViewPerson)
        Me.XionPanelPerson.Location = New System.Drawing.Point(3, 31)
        Me.XionPanelPerson.Movable = true
        Me.XionPanelPerson.Name = "XionPanelPerson"
        Me.XionPanelPerson.Orientation = XionControls.XionPanel.PanelOrientation.Horizontal
        Me.XionPanelPerson.Padding = New System.Windows.Forms.Padding(0, 22, 0, 0)
        Me.XionPanelPerson.Sizable = true
        Me.XionPanelPerson.Size = New System.Drawing.Size(197, 546)
        Me.XionPanelPerson.State = XionControls.XionPanel.PanelState.Expand
        Me.XionPanelPerson.TabIndex = 1
        Me.XionPanelPerson.Text = "Person List"
        Me.XionPanelPerson.Title = "Person List"
        Me.XionPanelPerson.TitleBackColor = System.Drawing.SystemColors.Control
        Me.XionPanelPerson.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.XionPanelPerson.TitleHeight = 22
        '
        'DataGridViewPerson
        '
        Me.DataGridViewPerson.AllowUserToOrderColumns = true
        Me.DataGridViewPerson.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.DataGridViewPerson.AutoGenerateColumns = false
        Me.DataGridViewPerson.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridViewPerson.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DataGridViewPerson.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewPerson.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NameDataGridViewTextBoxColumn, Me.AlternateNameDataGridViewTextBoxColumn, Me.BornDataGridViewTextBoxColumn, Me.BirthPlaceDataGridViewTextBoxColumn, Me.MiniBiographyDataGridViewTextBoxColumn, Me.BiographyDataGridViewTextBoxColumn, Me.URLDataGridViewTextBoxColumn, Me.IMDBIdDataGridViewTextBoxColumn, Me.TMDBIdDataGridViewTextBoxColumn, Me.PictureDataGridViewTextBoxColumn, Me.PhotosDataGridViewTextBoxColumn, Me.IsActor, Me.IsProducer, Me.IsDirector, Me.IsWriter})
        Me.DataGridViewPerson.DataSource = Me.PersonBindingSource
        Me.DataGridViewPerson.Location = New System.Drawing.Point(0, 22)
        Me.DataGridViewPerson.MultiSelect = false
        Me.DataGridViewPerson.Name = "DataGridViewPerson"
        Me.DataGridViewPerson.RowHeadersVisible = false
        Me.DataGridViewPerson.RowHeadersWidth = 20
        Me.DataGridViewPerson.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewPerson.RowTemplate.Height = 16
        Me.DataGridViewPerson.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridViewPerson.Size = New System.Drawing.Size(375, 524)
        Me.DataGridViewPerson.TabIndex = 0
        '
        'NameDataGridViewTextBoxColumn
        '
        Me.NameDataGridViewTextBoxColumn.DataPropertyName = "Name"
        Me.NameDataGridViewTextBoxColumn.HeaderText = "Name"
        Me.NameDataGridViewTextBoxColumn.Name = "NameDataGridViewTextBoxColumn"
        Me.NameDataGridViewTextBoxColumn.Width = 60
        '
        'AlternateNameDataGridViewTextBoxColumn
        '
        Me.AlternateNameDataGridViewTextBoxColumn.DataPropertyName = "AlternateName"
        Me.AlternateNameDataGridViewTextBoxColumn.HeaderText = "AlternateName"
        Me.AlternateNameDataGridViewTextBoxColumn.Name = "AlternateNameDataGridViewTextBoxColumn"
        Me.AlternateNameDataGridViewTextBoxColumn.Width = 102
        '
        'BornDataGridViewTextBoxColumn
        '
        Me.BornDataGridViewTextBoxColumn.DataPropertyName = "Born"
        Me.BornDataGridViewTextBoxColumn.HeaderText = "Born"
        Me.BornDataGridViewTextBoxColumn.Name = "BornDataGridViewTextBoxColumn"
        Me.BornDataGridViewTextBoxColumn.Width = 54
        '
        'BirthPlaceDataGridViewTextBoxColumn
        '
        Me.BirthPlaceDataGridViewTextBoxColumn.DataPropertyName = "BirthPlace"
        Me.BirthPlaceDataGridViewTextBoxColumn.HeaderText = "BirthPlace"
        Me.BirthPlaceDataGridViewTextBoxColumn.Name = "BirthPlaceDataGridViewTextBoxColumn"
        Me.BirthPlaceDataGridViewTextBoxColumn.Width = 80
        '
        'MiniBiographyDataGridViewTextBoxColumn
        '
        Me.MiniBiographyDataGridViewTextBoxColumn.DataPropertyName = "MiniBiography"
        Me.MiniBiographyDataGridViewTextBoxColumn.HeaderText = "MiniBiography"
        Me.MiniBiographyDataGridViewTextBoxColumn.Name = "MiniBiographyDataGridViewTextBoxColumn"
        Me.MiniBiographyDataGridViewTextBoxColumn.Width = 98
        '
        'BiographyDataGridViewTextBoxColumn
        '
        Me.BiographyDataGridViewTextBoxColumn.DataPropertyName = "Biography"
        Me.BiographyDataGridViewTextBoxColumn.HeaderText = "Biography"
        Me.BiographyDataGridViewTextBoxColumn.Name = "BiographyDataGridViewTextBoxColumn"
        Me.BiographyDataGridViewTextBoxColumn.Width = 79
        '
        'URLDataGridViewTextBoxColumn
        '
        Me.URLDataGridViewTextBoxColumn.DataPropertyName = "URL"
        Me.URLDataGridViewTextBoxColumn.HeaderText = "URL"
        Me.URLDataGridViewTextBoxColumn.Name = "URLDataGridViewTextBoxColumn"
        Me.URLDataGridViewTextBoxColumn.Width = 54
        '
        'IMDBIdDataGridViewTextBoxColumn
        '
        Me.IMDBIdDataGridViewTextBoxColumn.DataPropertyName = "IMDB_Id"
        Me.IMDBIdDataGridViewTextBoxColumn.HeaderText = "IMDB_Id"
        Me.IMDBIdDataGridViewTextBoxColumn.Name = "IMDBIdDataGridViewTextBoxColumn"
        Me.IMDBIdDataGridViewTextBoxColumn.Width = 74
        '
        'TMDBIdDataGridViewTextBoxColumn
        '
        Me.TMDBIdDataGridViewTextBoxColumn.DataPropertyName = "TMDB_Id"
        Me.TMDBIdDataGridViewTextBoxColumn.HeaderText = "TMDB_Id"
        Me.TMDBIdDataGridViewTextBoxColumn.Name = "TMDBIdDataGridViewTextBoxColumn"
        Me.TMDBIdDataGridViewTextBoxColumn.Width = 78
        '
        'PictureDataGridViewTextBoxColumn
        '
        Me.PictureDataGridViewTextBoxColumn.DataPropertyName = "Picture"
        Me.PictureDataGridViewTextBoxColumn.HeaderText = "Picture"
        Me.PictureDataGridViewTextBoxColumn.Name = "PictureDataGridViewTextBoxColumn"
        Me.PictureDataGridViewTextBoxColumn.Width = 65
        '
        'PhotosDataGridViewTextBoxColumn
        '
        Me.PhotosDataGridViewTextBoxColumn.DataPropertyName = "Photos"
        Me.PhotosDataGridViewTextBoxColumn.HeaderText = "Photos"
        Me.PhotosDataGridViewTextBoxColumn.Name = "PhotosDataGridViewTextBoxColumn"
        Me.PhotosDataGridViewTextBoxColumn.Width = 65
        '
        'IsActor
        '
        Me.IsActor.DataPropertyName = "IsActor"
        Me.IsActor.HeaderText = "IsActor"
        Me.IsActor.Name = "IsActor"
        Me.IsActor.Width = 46
        '
        'IsProducer
        '
        Me.IsProducer.DataPropertyName = "IsProducer"
        Me.IsProducer.HeaderText = "IsProducer"
        Me.IsProducer.Name = "IsProducer"
        Me.IsProducer.Width = 64
        '
        'IsDirector
        '
        Me.IsDirector.DataPropertyName = "IsDirector"
        Me.IsDirector.HeaderText = "IsDirector"
        Me.IsDirector.Name = "IsDirector"
        Me.IsDirector.Width = 58
        '
        'IsWriter
        '
        Me.IsWriter.DataPropertyName = "IsWriter"
        Me.IsWriter.HeaderText = "IsWriter"
        Me.IsWriter.Name = "IsWriter"
        Me.IsWriter.Width = 49
        '
        'PersonBindingNavigator
        '
        Me.PersonBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItemPerson
        Me.PersonBindingNavigator.BindingSource = Me.PersonBindingSource
        Me.PersonBindingNavigator.CountItem = Me.BindingNavigatorCountItem1
        Me.PersonBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItemPerson
        Me.PersonBindingNavigator.Dock = System.Windows.Forms.DockStyle.None
        Me.PersonBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem1, Me.BindingNavigatorMovePreviousItem1, Me.BindingNavigatorSeparator3, Me.BindingNavigatorPositionItem1, Me.BindingNavigatorCountItem1, Me.BindingNavigatorSeparator4, Me.BindingNavigatorMoveNextItem1, Me.BindingNavigatorMoveLastItem1, Me.BindingNavigatorSeparator5, Me.BindingNavigatorAddNewItemPerson, Me.BindingNavigatorDeleteItemPerson, Me.ToolStripSeparator2, Me.SpeichernToolStripButton, Me.ToolStripSeparator4, Me.ToolStripButtonAddMissingPersons, Me.ToolStripButtonGrabPersons, Me.ToolStripSeparator5, Me.ToolStripTextBoxSearch})
        Me.PersonBindingNavigator.Location = New System.Drawing.Point(0, 3)
        Me.PersonBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem1
        Me.PersonBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem1
        Me.PersonBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem1
        Me.PersonBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem1
        Me.PersonBindingNavigator.Name = "PersonBindingNavigator"
        Me.PersonBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem1
        Me.PersonBindingNavigator.Size = New System.Drawing.Size(581, 29)
        Me.PersonBindingNavigator.TabIndex = 0
        Me.PersonBindingNavigator.Text = "PersonBindingNavigator"
        '
        'BindingNavigatorAddNewItemPerson
        '
        Me.BindingNavigatorAddNewItemPerson.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorAddNewItemPerson.Image = CType(resources.GetObject("BindingNavigatorAddNewItemPerson.Image"),System.Drawing.Image)
        Me.BindingNavigatorAddNewItemPerson.Name = "BindingNavigatorAddNewItemPerson"
        Me.BindingNavigatorAddNewItemPerson.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorAddNewItemPerson.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorAddNewItemPerson.Text = "Add Entry"
        '
        'BindingNavigatorCountItem1
        '
        Me.BindingNavigatorCountItem1.Name = "BindingNavigatorCountItem1"
        Me.BindingNavigatorCountItem1.Size = New System.Drawing.Size(44, 26)
        Me.BindingNavigatorCountItem1.Text = "von {0}"
        Me.BindingNavigatorCountItem1.ToolTipText = "Total Number of Entries."
        '
        'BindingNavigatorDeleteItemPerson
        '
        Me.BindingNavigatorDeleteItemPerson.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItemPerson.Image = CType(resources.GetObject("BindingNavigatorDeleteItemPerson.Image"),System.Drawing.Image)
        Me.BindingNavigatorDeleteItemPerson.Name = "BindingNavigatorDeleteItemPerson"
        Me.BindingNavigatorDeleteItemPerson.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorDeleteItemPerson.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorDeleteItemPerson.Text = "Delete Entry"
        '
        'BindingNavigatorMoveFirstItem1
        '
        Me.BindingNavigatorMoveFirstItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem1.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem1.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem1.Name = "BindingNavigatorMoveFirstItem1"
        Me.BindingNavigatorMoveFirstItem1.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveFirstItem1.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorMoveFirstItem1.Text = "Move First"
        '
        'BindingNavigatorMovePreviousItem1
        '
        Me.BindingNavigatorMovePreviousItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem1.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem1.Image"),System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem1.Name = "BindingNavigatorMovePreviousItem1"
        Me.BindingNavigatorMovePreviousItem1.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMovePreviousItem1.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorMovePreviousItem1.Text = "Move Previous"
        '
        'BindingNavigatorSeparator3
        '
        Me.BindingNavigatorSeparator3.Name = "BindingNavigatorSeparator3"
        Me.BindingNavigatorSeparator3.Size = New System.Drawing.Size(6, 29)
        '
        'BindingNavigatorPositionItem1
        '
        Me.BindingNavigatorPositionItem1.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem1.AutoSize = false
        Me.BindingNavigatorPositionItem1.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.BindingNavigatorPositionItem1.Name = "BindingNavigatorPositionItem1"
        Me.BindingNavigatorPositionItem1.Size = New System.Drawing.Size(50, 18)
        Me.BindingNavigatorPositionItem1.Text = "0"
        Me.BindingNavigatorPositionItem1.ToolTipText = "Current Position"
        '
        'BindingNavigatorSeparator4
        '
        Me.BindingNavigatorSeparator4.Name = "BindingNavigatorSeparator4"
        Me.BindingNavigatorSeparator4.Size = New System.Drawing.Size(6, 29)
        '
        'BindingNavigatorMoveNextItem1
        '
        Me.BindingNavigatorMoveNextItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem1.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem1.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem1.Name = "BindingNavigatorMoveNextItem1"
        Me.BindingNavigatorMoveNextItem1.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveNextItem1.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorMoveNextItem1.Text = "Move Next"
        '
        'BindingNavigatorMoveLastItem1
        '
        Me.BindingNavigatorMoveLastItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem1.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem1.Image"),System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem1.Name = "BindingNavigatorMoveLastItem1"
        Me.BindingNavigatorMoveLastItem1.RightToLeftAutoMirrorImage = true
        Me.BindingNavigatorMoveLastItem1.Size = New System.Drawing.Size(23, 26)
        Me.BindingNavigatorMoveLastItem1.Text = "Move Last"
        '
        'BindingNavigatorSeparator5
        '
        Me.BindingNavigatorSeparator5.Name = "BindingNavigatorSeparator5"
        Me.BindingNavigatorSeparator5.Size = New System.Drawing.Size(6, 29)
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 29)
        '
        'SpeichernToolStripButton
        '
        Me.SpeichernToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.SpeichernToolStripButton.Image = CType(resources.GetObject("SpeichernToolStripButton.Image"),System.Drawing.Image)
        Me.SpeichernToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SpeichernToolStripButton.Name = "SpeichernToolStripButton"
        Me.SpeichernToolStripButton.Size = New System.Drawing.Size(23, 26)
        Me.SpeichernToolStripButton.Text = "&Save Datas"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 29)
        '
        'ToolStripButtonAddMissingPersons
        '
        Me.ToolStripButtonAddMissingPersons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripButtonAddMissingPersons.Image = CType(resources.GetObject("ToolStripButtonAddMissingPersons.Image"),System.Drawing.Image)
        Me.ToolStripButtonAddMissingPersons.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonAddMissingPersons.Name = "ToolStripButtonAddMissingPersons"
        Me.ToolStripButtonAddMissingPersons.Size = New System.Drawing.Size(121, 26)
        Me.ToolStripButtonAddMissingPersons.Text = "Add missing Persons"
        '
        'ToolStripButtonGrabPersons
        '
        Me.ToolStripButtonGrabPersons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripButtonGrabPersons.Image = CType(resources.GetObject("ToolStripButtonGrabPersons.Image"),System.Drawing.Image)
        Me.ToolStripButtonGrabPersons.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonGrabPersons.Name = "ToolStripButtonGrabPersons"
        Me.ToolStripButtonGrabPersons.Size = New System.Drawing.Size(63, 26)
        Me.ToolStripButtonGrabPersons.Text = "Grab Data"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 29)
        '
        'ToolStripTextBoxSearch
        '
        Me.ToolStripTextBoxSearch.AccessibleName = "PersonSearchName"
        Me.ToolStripTextBoxSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.ToolStripTextBoxSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList
        Me.ToolStripTextBoxSearch.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.ToolStripTextBoxSearch.Name = "ToolStripTextBoxSearch"
        Me.ToolStripTextBoxSearch.Size = New System.Drawing.Size(90, 23)
        '
        'ViewCatalog
        '
        Me.ViewCatalog.AutoScroll = true
        Me.ViewCatalog.Controls.Add(Me.UserDataGridView)
        Me.ViewCatalog.Controls.Add(Me.CustomFieldDataGridView)
        Me.ViewCatalog.Controls.Add(OwnerLabel)
        Me.ViewCatalog.Controls.Add(Me.OwnerTextBox)
        Me.ViewCatalog.Controls.Add(MailLabel)
        Me.ViewCatalog.Controls.Add(Me.MailTextBox)
        Me.ViewCatalog.Controls.Add(SiteLabel)
        Me.ViewCatalog.Controls.Add(Me.SiteTextBox)
        Me.ViewCatalog.Controls.Add(DescriptionLabel)
        Me.ViewCatalog.Controls.Add(Me.DescriptionTextBox)
        Me.ViewCatalog.Location = New System.Drawing.Point(4, 22)
        Me.ViewCatalog.Name = "ViewCatalog"
        Me.ViewCatalog.Size = New System.Drawing.Size(600, 580)
        Me.ViewCatalog.TabIndex = 8
        Me.ViewCatalog.Text = "View Catalog"
        Me.ViewCatalog.UseVisualStyleBackColor = true
        '
        'UserDataGridView
        '
        Me.UserDataGridView.AutoGenerateColumns = false
        Me.UserDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.UserDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.UserDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn14, Me.DataGridViewTextBoxColumn15, Me.DataGridViewTextBoxColumn16, Me.DataGridViewTextBoxColumn17})
        Me.UserDataGridView.DataSource = Me.UserBindingSource
        Me.UserDataGridView.Location = New System.Drawing.Point(11, 80)
        Me.UserDataGridView.Name = "UserDataGridView"
        Me.UserDataGridView.RowTemplate.Height = 18
        Me.UserDataGridView.Size = New System.Drawing.Size(581, 145)
        Me.UserDataGridView.TabIndex = 8
        '
        'DataGridViewTextBoxColumn14
        '
        Me.DataGridViewTextBoxColumn14.DataPropertyName = "Name"
        Me.DataGridViewTextBoxColumn14.HeaderText = "Name"
        Me.DataGridViewTextBoxColumn14.Name = "DataGridViewTextBoxColumn14"
        Me.DataGridViewTextBoxColumn14.Width = 60
        '
        'DataGridViewTextBoxColumn15
        '
        Me.DataGridViewTextBoxColumn15.DataPropertyName = "Alias"
        Me.DataGridViewTextBoxColumn15.HeaderText = "Alias"
        Me.DataGridViewTextBoxColumn15.Name = "DataGridViewTextBoxColumn15"
        Me.DataGridViewTextBoxColumn15.Width = 54
        '
        'DataGridViewTextBoxColumn16
        '
        Me.DataGridViewTextBoxColumn16.DataPropertyName = "TraktName"
        Me.DataGridViewTextBoxColumn16.HeaderText = "TraktName"
        Me.DataGridViewTextBoxColumn16.Name = "DataGridViewTextBoxColumn16"
        Me.DataGridViewTextBoxColumn16.Width = 85
        '
        'DataGridViewTextBoxColumn17
        '
        Me.DataGridViewTextBoxColumn17.DataPropertyName = "Age"
        Me.DataGridViewTextBoxColumn17.HeaderText = "Age"
        Me.DataGridViewTextBoxColumn17.Name = "DataGridViewTextBoxColumn17"
        Me.DataGridViewTextBoxColumn17.Width = 51
        '
        'UserBindingSource
        '
        Me.UserBindingSource.DataMember = "User"
        Me.UserBindingSource.DataSource = Me.AntMovieCatalog
        '
        'AntMovieCatalog
        '
        Me.AntMovieCatalog.Locale = New System.Globalization.CultureInfo("")
        Me.AntMovieCatalog.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'CustomFieldDataGridView
        '
        Me.CustomFieldDataGridView.AutoGenerateColumns = false
        Me.CustomFieldDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.CustomFieldDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.CustomFieldDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn7, Me.DataGridViewTextBoxColumn8, Me.DataGridViewTextBoxColumn9, Me.DataGridViewTextBoxColumn10, Me.DataGridViewTextBoxColumn11, Me.DataGridViewTextBoxColumn12, Me.DataGridViewTextBoxColumn13})
        Me.CustomFieldDataGridView.DataSource = Me.CustomFieldBindingSource
        Me.CustomFieldDataGridView.Location = New System.Drawing.Point(11, 253)
        Me.CustomFieldDataGridView.Name = "CustomFieldDataGridView"
        Me.CustomFieldDataGridView.RowTemplate.Height = 18
        Me.CustomFieldDataGridView.Size = New System.Drawing.Size(581, 306)
        Me.CustomFieldDataGridView.TabIndex = 9
        '
        'DataGridViewTextBoxColumn6
        '
        Me.DataGridViewTextBoxColumn6.DataPropertyName = "Tag"
        Me.DataGridViewTextBoxColumn6.HeaderText = "Tag"
        Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
        Me.DataGridViewTextBoxColumn6.Width = 51
        '
        'DataGridViewTextBoxColumn7
        '
        Me.DataGridViewTextBoxColumn7.DataPropertyName = "Name"
        Me.DataGridViewTextBoxColumn7.HeaderText = "Name"
        Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
        Me.DataGridViewTextBoxColumn7.Width = 60
        '
        'DataGridViewTextBoxColumn8
        '
        Me.DataGridViewTextBoxColumn8.DataPropertyName = "Type"
        Me.DataGridViewTextBoxColumn8.HeaderText = "Type"
        Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
        Me.DataGridViewTextBoxColumn8.Width = 56
        '
        'DataGridViewTextBoxColumn9
        '
        Me.DataGridViewTextBoxColumn9.DataPropertyName = "DefaultValue"
        Me.DataGridViewTextBoxColumn9.HeaderText = "DefaultValue"
        Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
        Me.DataGridViewTextBoxColumn9.Width = 93
        '
        'DataGridViewTextBoxColumn10
        '
        Me.DataGridViewTextBoxColumn10.DataPropertyName = "MultiValues"
        Me.DataGridViewTextBoxColumn10.HeaderText = "MultiValues"
        Me.DataGridViewTextBoxColumn10.Name = "DataGridViewTextBoxColumn10"
        Me.DataGridViewTextBoxColumn10.Width = 86
        '
        'DataGridViewTextBoxColumn11
        '
        Me.DataGridViewTextBoxColumn11.DataPropertyName = "ExcludedInScripts"
        Me.DataGridViewTextBoxColumn11.HeaderText = "ExcludedInScripts"
        Me.DataGridViewTextBoxColumn11.Name = "DataGridViewTextBoxColumn11"
        Me.DataGridViewTextBoxColumn11.Width = 117
        '
        'DataGridViewTextBoxColumn12
        '
        Me.DataGridViewTextBoxColumn12.DataPropertyName = "GUIProperties"
        Me.DataGridViewTextBoxColumn12.HeaderText = "GUIProperties"
        Me.DataGridViewTextBoxColumn12.Name = "DataGridViewTextBoxColumn12"
        Me.DataGridViewTextBoxColumn12.Width = 98
        '
        'DataGridViewTextBoxColumn13
        '
        Me.DataGridViewTextBoxColumn13.DataPropertyName = "OtherProperties"
        Me.DataGridViewTextBoxColumn13.HeaderText = "OtherProperties"
        Me.DataGridViewTextBoxColumn13.Name = "DataGridViewTextBoxColumn13"
        Me.DataGridViewTextBoxColumn13.Width = 105
        '
        'CustomFieldBindingSource
        '
        Me.CustomFieldBindingSource.DataMember = "CustomField"
        Me.CustomFieldBindingSource.DataSource = Me.AntMovieCatalog
        '
        'OwnerTextBox
        '
        Me.OwnerTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PropertiesBindingSource, "Owner", true))
        Me.OwnerTextBox.Location = New System.Drawing.Point(77, 10)
        Me.OwnerTextBox.Name = "OwnerTextBox"
        Me.OwnerTextBox.Size = New System.Drawing.Size(220, 20)
        Me.OwnerTextBox.TabIndex = 1
        '
        'PropertiesBindingSource
        '
        Me.PropertiesBindingSource.DataMember = "Properties"
        Me.PropertiesBindingSource.DataSource = Me.AntMovieCatalog
        '
        'MailTextBox
        '
        Me.MailTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PropertiesBindingSource, "Mail", true))
        Me.MailTextBox.Location = New System.Drawing.Point(77, 36)
        Me.MailTextBox.Name = "MailTextBox"
        Me.MailTextBox.Size = New System.Drawing.Size(220, 20)
        Me.MailTextBox.TabIndex = 5
        '
        'SiteTextBox
        '
        Me.SiteTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PropertiesBindingSource, "Site", true))
        Me.SiteTextBox.Location = New System.Drawing.Point(372, 10)
        Me.SiteTextBox.Name = "SiteTextBox"
        Me.SiteTextBox.Size = New System.Drawing.Size(220, 20)
        Me.SiteTextBox.TabIndex = 3
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.PropertiesBindingSource, "Description", true))
        Me.DescriptionTextBox.Location = New System.Drawing.Point(372, 36)
        Me.DescriptionTextBox.Name = "DescriptionTextBox"
        Me.DescriptionTextBox.Size = New System.Drawing.Size(220, 20)
        Me.DescriptionTextBox.TabIndex = 7
        '
        'mnuFile
        '
        Me.mnuFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemFile, Me.ToolStripMenuItemOptions, Me.ToolStripMenuItemHelp, Me.ToolStripMenuItemDebug})
        Me.mnuFile.Location = New System.Drawing.Point(0, 0)
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(608, 24)
        Me.mnuFile.TabIndex = 0
        Me.mnuFile.Text = "File"
        '
        'ToolStripMenuItemFile
        '
        Me.ToolStripMenuItemFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadConfigurationFileToolStripMenuItem, Me.SaveConfigFileToolStripMenuItem, Me.SaveConfigFileAsToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile"
        Me.ToolStripMenuItemFile.Size = New System.Drawing.Size(37, 20)
        Me.ToolStripMenuItemFile.Text = "File"
        '
        'LoadConfigurationFileToolStripMenuItem
        '
        Me.LoadConfigurationFileToolStripMenuItem.Name = "LoadConfigurationFileToolStripMenuItem"
        Me.LoadConfigurationFileToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.LoadConfigurationFileToolStripMenuItem.Text = "Load config File"
        '
        'SaveConfigFileToolStripMenuItem
        '
        Me.SaveConfigFileToolStripMenuItem.Name = "SaveConfigFileToolStripMenuItem"
        Me.SaveConfigFileToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.SaveConfigFileToolStripMenuItem.Text = "Save config file"
        '
        'SaveConfigFileAsToolStripMenuItem
        '
        Me.SaveConfigFileAsToolStripMenuItem.Name = "SaveConfigFileAsToolStripMenuItem"
        Me.SaveConfigFileAsToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.SaveConfigFileAsToolStripMenuItem.Text = "Save config File as ..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(179, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ToolStripMenuItemOptions
        '
        Me.ToolStripMenuItemOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemScanPath, Me.ToolStripMenuItemsScanFilter})
        Me.ToolStripMenuItemOptions.Name = "ToolStripMenuItemOptions"
        Me.ToolStripMenuItemOptions.Size = New System.Drawing.Size(61, 20)
        Me.ToolStripMenuItemOptions.Text = "Options"
        '
        'ToolStripMenuItemScanPath
        '
        Me.ToolStripMenuItemScanPath.Name = "ToolStripMenuItemScanPath"
        Me.ToolStripMenuItemScanPath.Size = New System.Drawing.Size(140, 22)
        Me.ToolStripMenuItemScanPath.Text = "Scan Path ..."
        '
        'ToolStripMenuItemsScanFilter
        '
        Me.ToolStripMenuItemsScanFilter.Name = "ToolStripMenuItemsScanFilter"
        Me.ToolStripMenuItemsScanFilter.Size = New System.Drawing.Size(140, 22)
        Me.ToolStripMenuItemsScanFilter.Text = "Scan Filter ..."
        '
        'ToolStripMenuItemHelp
        '
        Me.ToolStripMenuItemHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InternetLinksToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp"
        Me.ToolStripMenuItemHelp.Size = New System.Drawing.Size(44, 20)
        Me.ToolStripMenuItemHelp.Text = "Help"
        '
        'InternetLinksToolStripMenuItem
        '
        Me.InternetLinksToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MyFilmsWikiToolStripMenuItem, Me.AntMovieCatalogToolStripMenuItem, Me.MediaInfodllToolStripMenuItem})
        Me.InternetLinksToolStripMenuItem.Name = "InternetLinksToolStripMenuItem"
        Me.InternetLinksToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.InternetLinksToolStripMenuItem.Text = "Internet Links"
        '
        'MyFilmsWikiToolStripMenuItem
        '
        Me.MyFilmsWikiToolStripMenuItem.Name = "MyFilmsWikiToolStripMenuItem"
        Me.MyFilmsWikiToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.MyFilmsWikiToolStripMenuItem.Text = "MyFilms Wiki"
        '
        'AntMovieCatalogToolStripMenuItem
        '
        Me.AntMovieCatalogToolStripMenuItem.Name = "AntMovieCatalogToolStripMenuItem"
        Me.AntMovieCatalogToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.AntMovieCatalogToolStripMenuItem.Text = "Ant Movie Catalog"
        '
        'MediaInfodllToolStripMenuItem
        '
        Me.MediaInfodllToolStripMenuItem.Name = "MediaInfodllToolStripMenuItem"
        Me.MediaInfodllToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.MediaInfodllToolStripMenuItem.Text = "MediaInfo.dll"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'ToolStripMenuItemDebug
        '
        Me.ToolStripMenuItemDebug.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XMLToolStripMenuItem, Me.MediaFileToolStripMenuItem, Me.NonMediaFilesToolStripMenuItem, Me.TrailerFilesToolStripMenuItem, Me.OrphanMediaToolStripMenuItem, Me.OrphanNonMediaToolStripMenuItem, Me.OrphanTrailerToolStripMenuItem, Me.MultiPartFilesToolStripMenuItem, Me.OrphanAntToolStripMenuItem, Me.MultiPartProcessedFilesToolStripMenuItem, Me.AntFieldsToolStripMenuItem, Me.NodesToProcessToolStripMenuItem, Me.ListMediaInfoParamsToolStripMenuItem})
        Me.ToolStripMenuItemDebug.Name = "ToolStripMenuItemDebug"
        Me.ToolStripMenuItemDebug.Size = New System.Drawing.Size(54, 20)
        Me.ToolStripMenuItemDebug.Text = "Debug"
        '
        'XMLToolStripMenuItem
        '
        Me.XMLToolStripMenuItem.Name = "XMLToolStripMenuItem"
        Me.XMLToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.XMLToolStripMenuItem.Text = "XML"
        '
        'MediaFileToolStripMenuItem
        '
        Me.MediaFileToolStripMenuItem.Name = "MediaFileToolStripMenuItem"
        Me.MediaFileToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.MediaFileToolStripMenuItem.Text = "Media Files"
        '
        'NonMediaFilesToolStripMenuItem
        '
        Me.NonMediaFilesToolStripMenuItem.Name = "NonMediaFilesToolStripMenuItem"
        Me.NonMediaFilesToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.NonMediaFilesToolStripMenuItem.Text = "Non Media Files"
        '
        'TrailerFilesToolStripMenuItem
        '
        Me.TrailerFilesToolStripMenuItem.Name = "TrailerFilesToolStripMenuItem"
        Me.TrailerFilesToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.TrailerFilesToolStripMenuItem.Text = "Trailer Media Files"
        '
        'OrphanMediaToolStripMenuItem
        '
        Me.OrphanMediaToolStripMenuItem.Name = "OrphanMediaToolStripMenuItem"
        Me.OrphanMediaToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.OrphanMediaToolStripMenuItem.Text = "Orphan Media"
        '
        'OrphanNonMediaToolStripMenuItem
        '
        Me.OrphanNonMediaToolStripMenuItem.Name = "OrphanNonMediaToolStripMenuItem"
        Me.OrphanNonMediaToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.OrphanNonMediaToolStripMenuItem.Text = "Orphan Non Media"
        '
        'OrphanTrailerToolStripMenuItem
        '
        Me.OrphanTrailerToolStripMenuItem.Name = "OrphanTrailerToolStripMenuItem"
        Me.OrphanTrailerToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.OrphanTrailerToolStripMenuItem.Text = "Orphan Trailer"
        '
        'MultiPartFilesToolStripMenuItem
        '
        Me.MultiPartFilesToolStripMenuItem.Name = "MultiPartFilesToolStripMenuItem"
        Me.MultiPartFilesToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.MultiPartFilesToolStripMenuItem.Text = "Multi-Part Files"
        '
        'OrphanAntToolStripMenuItem
        '
        Me.OrphanAntToolStripMenuItem.Name = "OrphanAntToolStripMenuItem"
        Me.OrphanAntToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.OrphanAntToolStripMenuItem.Text = "Orphan Ant"
        '
        'MultiPartProcessedFilesToolStripMenuItem
        '
        Me.MultiPartProcessedFilesToolStripMenuItem.Name = "MultiPartProcessedFilesToolStripMenuItem"
        Me.MultiPartProcessedFilesToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.MultiPartProcessedFilesToolStripMenuItem.Text = "MultiPartProcessedFiles"
        '
        'AntFieldsToolStripMenuItem
        '
        Me.AntFieldsToolStripMenuItem.Name = "AntFieldsToolStripMenuItem"
        Me.AntFieldsToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.AntFieldsToolStripMenuItem.Text = "AntFields"
        '
        'NodesToProcessToolStripMenuItem
        '
        Me.NodesToProcessToolStripMenuItem.Name = "NodesToProcessToolStripMenuItem"
        Me.NodesToProcessToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.NodesToProcessToolStripMenuItem.Text = "NodesToProcess"
        '
        'ListMediaInfoParamsToolStripMenuItem
        '
        Me.ListMediaInfoParamsToolStripMenuItem.Name = "ListMediaInfoParamsToolStripMenuItem"
        Me.ListMediaInfoParamsToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.ListMediaInfoParamsToolStripMenuItem.Text = "List MediaInfo Params"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripFixedText, Me.ToolStripStatusLabel, Me.ToolStripProgressBar})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 636)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(608, 22)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripFixedText
        '
        Me.ToolStripFixedText.Name = "ToolStripFixedText"
        Me.ToolStripFixedText.Size = New System.Drawing.Size(48, 17)
        Me.ToolStripFixedText.Text = "Status : "
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(0, 17)
        '
        'ToolStripProgressBar
        '
        Me.ToolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgressBar.Name = "ToolStripProgressBar"
        Me.ToolStripProgressBar.Size = New System.Drawing.Size(200, 16)
        '
        'epInteractive
        '
        Me.epInteractive.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epInteractive.ContainerControl = Me
        '
        'epManualUpdater
        '
        Me.epManualUpdater.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epManualUpdater.ContainerControl = Me
        '
        'epOptions
        '
        Me.epOptions.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epOptions.ContainerControl = Me
        '
        'ToolTip1
        '
        Me.ToolTip1.AutoPopDelay = 15000
        Me.ToolTip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer))
        Me.ToolTip1.InitialDelay = 1000
        Me.ToolTip1.IsBalloon = true
        Me.ToolTip1.ReshowDelay = 250
        Me.ToolTip1.ToolTipTitle = "MyFilms AMCupdater Help ..."
        '
        'ListVideos
        '
        Me.ListVideos.AutoGenerateColumns = false
        Me.ListVideos.ColumnHeadersHeight = 21
        Me.ListVideos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ListVideos.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NumberDataGridViewTextBoxColumn, Me.OriginalTitleDataGridViewTextBoxColumn, Me.TranslatedTitleDataGridViewTextBoxColumn, Me.YearDataGridViewTextBoxColumn, Me.DateAddedDataGridViewTextBoxColumn})
        Me.ListVideos.DataSource = Me.MovieBindingSource
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ListVideos.DefaultCellStyle = DataGridViewCellStyle13
        Me.ListVideos.Location = New System.Drawing.Point(-2, 28)
        Me.ListVideos.Name = "ListVideos"
        Me.ListVideos.ReadOnly = true
        Me.ListVideos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ListVideos.RowHeadersDefaultCellStyle = DataGridViewCellStyle14
        Me.ListVideos.RowHeadersVisible = false
        Me.ListVideos.RowHeadersWidth = 20
        Me.ListVideos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.ListVideos.RowTemplate.Height = 16
        Me.ListVideos.Size = New System.Drawing.Size(184, 441)
        Me.ListVideos.TabIndex = 1
        '
        'NumberDataGridViewTextBoxColumn
        '
        Me.NumberDataGridViewTextBoxColumn.DataPropertyName = "Number"
        Me.NumberDataGridViewTextBoxColumn.HeaderText = "N°"
        Me.NumberDataGridViewTextBoxColumn.Name = "NumberDataGridViewTextBoxColumn"
        Me.NumberDataGridViewTextBoxColumn.ReadOnly = true
        Me.NumberDataGridViewTextBoxColumn.Width = 35
        '
        'OriginalTitleDataGridViewTextBoxColumn
        '
        Me.OriginalTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.OriginalTitleDataGridViewTextBoxColumn.DataPropertyName = "OriginalTitle"
        Me.OriginalTitleDataGridViewTextBoxColumn.HeaderText = "Original Title"
        Me.OriginalTitleDataGridViewTextBoxColumn.Name = "OriginalTitleDataGridViewTextBoxColumn"
        Me.OriginalTitleDataGridViewTextBoxColumn.ReadOnly = true
        Me.OriginalTitleDataGridViewTextBoxColumn.Width = 90
        '
        'TranslatedTitleDataGridViewTextBoxColumn
        '
        Me.TranslatedTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.TranslatedTitleDataGridViewTextBoxColumn.DataPropertyName = "TranslatedTitle"
        Me.TranslatedTitleDataGridViewTextBoxColumn.HeaderText = "Translated Title"
        Me.TranslatedTitleDataGridViewTextBoxColumn.Name = "TranslatedTitleDataGridViewTextBoxColumn"
        Me.TranslatedTitleDataGridViewTextBoxColumn.ReadOnly = true
        Me.TranslatedTitleDataGridViewTextBoxColumn.Width = 105
        '
        'YearDataGridViewTextBoxColumn
        '
        Me.YearDataGridViewTextBoxColumn.DataPropertyName = "Year"
        Me.YearDataGridViewTextBoxColumn.HeaderText = "Year"
        Me.YearDataGridViewTextBoxColumn.Name = "YearDataGridViewTextBoxColumn"
        Me.YearDataGridViewTextBoxColumn.ReadOnly = true
        Me.YearDataGridViewTextBoxColumn.Width = 35
        '
        'DateAddedDataGridViewTextBoxColumn
        '
        Me.DateAddedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.DateAddedDataGridViewTextBoxColumn.DataPropertyName = "DateAdded"
        Me.DateAddedDataGridViewTextBoxColumn.HeaderText = "Date Added"
        Me.DateAddedDataGridViewTextBoxColumn.Name = "DateAddedDataGridViewTextBoxColumn"
        Me.DateAddedDataGridViewTextBoxColumn.ReadOnly = true
        Me.DateAddedDataGridViewTextBoxColumn.Width = 89
        '
        'ImageListViewPersons
        '
        Me.ImageListViewPersons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageListViewPersons.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageListViewPersons.TransparentColor = System.Drawing.Color.Transparent
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(608, 658)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.mnuFile)
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MainMenuStrip = Me.mnuFile
        Me.Name = "Form1"
        Me.Text = "Ant Movie Catalog Auto-Updater"
        Me.TabControl1.ResumeLayout(false)
        Me.Import_Movies.ResumeLayout(false)
        Me.Import_Movies.PerformLayout
        Me.GroupBox27.ResumeLayout(false)
        Me.GroupBox27.PerformLayout
        Me.GroupBox9.ResumeLayout(false)
        Me.GroupBox9.PerformLayout
        Me.GroupBox23.ResumeLayout(false)
        Me.GroupBox23.PerformLayout
        Me.GroupBox17.ResumeLayout(false)
        Me.GroupBox17.PerformLayout
        Me.GroupBox8.ResumeLayout(false)
        Me.GroupBox8.PerformLayout
        Me.Update_Movies.ResumeLayout(false)
        Me.GroupBox32.ResumeLayout(false)
        Me.grpManualInternetLookupSettings.ResumeLayout(false)
        Me.grpManualInternetLookupSettings.PerformLayout
        Me.grpManualUpdatesParameters.ResumeLayout(false)
        Me.grpManualUpdatesParameters.PerformLayout
        Me.GroupBox10.ResumeLayout(false)
        Me.GroupBox10.PerformLayout
        Me.DatabaseFields.ResumeLayout(false)
        Me.DatabaseFields.PerformLayout
        Me.GroupBox_FanartHandling.ResumeLayout(false)
        Me.GroupBox_FanartHandling.PerformLayout
        CType(Me.cbNumFanartLimitNumber,System.ComponentModel.ISupportInitialize).EndInit
        Me.GroupBox_TitleHandling.ResumeLayout(false)
        Me.GroupBox_TitleHandling.PerformLayout
        Me.GroupBox_StorageFieldHandling.ResumeLayout(false)
        Me.GroupBox_StorageFieldHandling.PerformLayout
        Me.GroupBox_MediaLabelFieldHandling.ResumeLayout(false)
        Me.GroupBox_MediaLabelFieldHandling.PerformLayout
        Me.GroupBox_OtherFieldHandling.ResumeLayout(false)
        Me.GroupBox_OtherFieldHandling.PerformLayout
        Me.GroupBox_PictureHandling.ResumeLayout(false)
        Me.GroupBox_PictureHandling.PerformLayout
        Me.GroupBox16.ResumeLayout(false)
        Me.GroupBox16.PerformLayout
        Me.ScanFilters.ResumeLayout(false)
        Me.GroupBox28.ResumeLayout(false)
        CType(Me.dgEditionStrings,System.ComponentModel.ISupportInitialize).EndInit
        Me.GroupBox24.ResumeLayout(false)
        Me.GroupBox24.PerformLayout
        Me.GroupBox6.ResumeLayout(false)
        Me.GroupBox6.PerformLayout
        Me.GroupBox20.ResumeLayout(false)
        Me.GroupBox20.PerformLayout
        CType(Me.dgFilterStrings,System.ComponentModel.ISupportInitialize).EndInit
        Me.GroupBox19.ResumeLayout(false)
        Me.GroupBox19.PerformLayout
        CType(Me.dgExcludedFolderStrings,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.dgExcludedFileStrings,System.ComponentModel.ISupportInitialize).EndInit
        Me.Options.ResumeLayout(false)
        Me.GroupBox26.ResumeLayout(false)
        Me.GroupBox26.PerformLayout
        Me.GroupBox11.ResumeLayout(false)
        Me.GroupBox11.PerformLayout
        Me.GroupBox4.ResumeLayout(false)
        Me.GroupBox4.PerformLayout
        Me.GroupBox25.ResumeLayout(false)
        Me.GroupBox25.PerformLayout
        Me.GroupBox7.ResumeLayout(false)
        Me.GroupBox7.PerformLayout
        Me.GroupBox13.ResumeLayout(false)
        Me.GroupBox13.PerformLayout
        Me.GroupBox12.ResumeLayout(false)
        Me.GroupBox12.PerformLayout
        Me.Mediainfo.ResumeLayout(false)
        Me.Mediainfo.PerformLayout
        Me.GroupBox18.ResumeLayout(false)
        Me.GroupBox18.PerformLayout
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox3.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.ViewCollection.ResumeLayout(false)
        Me.ViewCollection.PerformLayout
        Me.GroupBoxMovieDetails.ResumeLayout(false)
        Me.GroupBoxMovieDetails.PerformLayout
        CType(Me.MovieBindingSource,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.GroupBox14.ResumeLayout(false)
        Me.GroupBox14.PerformLayout
        Me.GroupBox22.ResumeLayout(false)
        Me.GroupBox22.PerformLayout
        Me.GroupBox21.ResumeLayout(false)
        Me.GroupBox21.PerformLayout
        Me.GroupBox15.ResumeLayout(false)
        Me.GroupBox15.PerformLayout
        Me.XionPanel1.ResumeLayout(false)
        CType(Me.DataGridViewMovie,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.MovieBindingNavigator,System.ComponentModel.ISupportInitialize).EndInit
        Me.MovieBindingNavigator.ResumeLayout(false)
        Me.MovieBindingNavigator.PerformLayout
        Me.ViewPersons.ResumeLayout(false)
        Me.ViewPersons.PerformLayout
        Me.GroupBoxPersonInfo.ResumeLayout(false)
        Me.GroupBoxPersonInfo.PerformLayout
        Me.GroupBox5.ResumeLayout(false)
        Me.GroupBox5.PerformLayout
        CType(Me.PersonBindingSource,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PictureBox2,System.ComponentModel.ISupportInitialize).EndInit
        Me.XionPanelPerson.ResumeLayout(false)
        CType(Me.DataGridViewPerson,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PersonBindingNavigator,System.ComponentModel.ISupportInitialize).EndInit
        Me.PersonBindingNavigator.ResumeLayout(false)
        Me.PersonBindingNavigator.PerformLayout
        Me.ViewCatalog.ResumeLayout(false)
        Me.ViewCatalog.PerformLayout
        CType(Me.UserDataGridView,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.UserBindingSource,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.AntMovieCatalog,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CustomFieldDataGridView,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CustomFieldBindingSource,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PropertiesBindingSource,System.ComponentModel.ISupportInitialize).EndInit
        Me.mnuFile.ResumeLayout(false)
        Me.mnuFile.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.epInteractive,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.epManualUpdater,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.epOptions,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ListVideos,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents Import_Movies As System.Windows.Forms.TabPage
    Friend WithEvents Options As System.Windows.Forms.TabPage
    Friend WithEvents btnProcessMovieList As System.Windows.Forms.Button
    Friend WithEvents btnParseXML As System.Windows.Forms.Button
    Friend WithEvents Mediainfo As System.Windows.Forms.TabPage
    Friend WithEvents btnGetSampleFile As System.Windows.Forms.Button
    Friend WithEvents txtSampleFile As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnTestAnalyse As System.Windows.Forms.Button
    Friend WithEvents txtSampleVideoCodec As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleVideoResolution As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtSampleVideoFramerate As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtSampleVideoBitrate As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleAudioBitrate As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioCodec As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleFileLength As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtSampleFileSize As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents btnFindOrphans As System.Windows.Forms.Button
    Friend WithEvents btnUpdateXML As System.Windows.Forms.Button
    Friend WithEvents btnJustDoIt As System.Windows.Forms.Button
    Friend WithEvents mnuFile As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItemFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InternetLinksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AntMovieCatalogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MediaInfodllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCancelProcessing As System.Windows.Forms.Button
    Friend WithEvents btnShowHideLog As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents ToolStripFixedText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents Update_Movies As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox10 As System.Windows.Forms.GroupBox
    Friend WithEvents cbManualSelectOperation As System.Windows.Forms.ComboBox
    Friend WithEvents btnManualApplyChanges As System.Windows.Forms.Button
    Friend WithEvents btnManualDoTest As System.Windows.Forms.Button
    Friend WithEvents grpManualUpdatesParameters As System.Windows.Forms.GroupBox
    Friend WithEvents txtManualNewValue As System.Windows.Forms.TextBox
    Friend WithEvents lblManualEnterNewValue As System.Windows.Forms.Label
    Friend WithEvents lblManualSelectField As System.Windows.Forms.Label
    Friend WithEvents cbManualSelectField As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualParametersValue1 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txtManualParameterValue1 As System.Windows.Forms.TextBox
    Friend WithEvents cbManualParameterOperator1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents cbManualParameterFieldList1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents chkManualParametersUpdateAll As System.Windows.Forms.CheckBox
    Friend WithEvents ToolStripMenuItemDebug As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents XMLToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MediaFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrailerFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NonMediaFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanMediaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanNonMediaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MultiPartFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanAntToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MultiPartProcessedFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AntFieldsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NodesToProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnShowHideLogTest As System.Windows.Forms.Button
    Friend WithEvents epInteractive As System.Windows.Forms.ErrorProvider
    Friend WithEvents epManualUpdater As System.Windows.Forms.ErrorProvider
    Friend WithEvents LoadConfigurationFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveConfigFileAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox12 As System.Windows.Forms.GroupBox
    Friend WithEvents chkExecuteOnlyForOrphans As System.Windows.Forms.CheckBox
    Friend WithEvents btnExecuteProgramSelectPath As System.Windows.Forms.Button
    Friend WithEvents txtExecuteProgramPath As System.Windows.Forms.TextBox
    Friend WithEvents chkExecuteProgram As System.Windows.Forms.CheckBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents GroupBox13 As System.Windows.Forms.GroupBox
    Friend WithEvents cbLogLevel As System.Windows.Forms.ComboBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents DatabaseFields As System.Windows.Forms.TabPage
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNoInternet As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAllInternet As System.Windows.Forms.Button
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNoMedia As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAllMedia As System.Windows.Forms.Button
    Friend WithEvents GroupBox16 As System.Windows.Forms.GroupBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNone As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAll As System.Windows.Forms.Button
    Friend WithEvents cbDatabaseFields As System.Windows.Forms.CheckedListBox
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents cbDateHandling As System.Windows.Forms.ComboBox
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioStreamCount As System.Windows.Forms.TextBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioLanguageList As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleAudioStreamList As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox18 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleTextLanguageList As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleTextCodecList As System.Windows.Forms.TextBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ListMediaInfoParamsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblManualParametersValue2 As System.Windows.Forms.Label
    Friend WithEvents lblManualParametersOperator2 As System.Windows.Forms.Label
    Friend WithEvents txtManualParameterValue2 As System.Windows.Forms.TextBox
    Friend WithEvents cbManualParameterOperator2 As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualParametersField2 As System.Windows.Forms.Label
    Friend WithEvents cbManualParameterFieldList2 As System.Windows.Forms.ComboBox
    Friend WithEvents cbManualParameterAndOr As System.Windows.Forms.ComboBox
    Friend WithEvents grpManualInternetLookupSettings As System.Windows.Forms.GroupBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents btnManualSelectInternetParserPath As System.Windows.Forms.Button
    Friend WithEvents cbManualInternetLookupBehaviour As System.Windows.Forms.ComboBox
    Friend WithEvents txtManualInternetParserPath As System.Windows.Forms.TextBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents lblManualDatabaseFieldsPrompt As System.Windows.Forms.Label
    Friend WithEvents btnManualCancel As System.Windows.Forms.Button
    Friend WithEvents lblInternetLookupRequired As System.Windows.Forms.Label
    Friend WithEvents ScanFilters As System.Windows.Forms.TabPage
    Friend WithEvents dgExcludedFileStrings As System.Windows.Forms.DataGridView
    Friend WithEvents dgExcludedFolderStrings As System.Windows.Forms.DataGridView
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents dgFilterStrings As System.Windows.Forms.DataGridView
    Friend WithEvents epOptions As System.Windows.Forms.ErrorProvider
    Friend WithEvents cbCheckHandling As System.Windows.Forms.ComboBox
    Friend WithEvents lblCheckFieldHandling As System.Windows.Forms.Label
    Friend WithEvents GroupBox19 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents chkFolderNameIsGroupName As System.Windows.Forms.CheckBox
    Friend WithEvents cbGroupNameAppliesTo As System.Windows.Forms.ComboBox
    Friend WithEvents lblGroupNameAppliesTo As System.Windows.Forms.Label
    Friend WithEvents cbPictureHandling As System.Windows.Forms.ComboBox
    Friend WithEvents lblPictureHandling As System.Windows.Forms.Label
    Friend WithEvents txtPictureFilenamePrefix As System.Windows.Forms.TextBox
    Friend WithEvents lblPicturePrefix As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents chkManualMissingFanartDownload As System.Windows.Forms.CheckBox
    Friend WithEvents ViewCollection As System.Windows.Forms.TabPage
    Friend WithEvents AntMovieCatalogAMC As AMCUpdater.AntMovieCatalog
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents MovieBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents VidéoBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents MovieBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents GroupBox14 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBox7 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents TextBox8 As System.Windows.Forms.TextBox
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents TextBox9 As System.Windows.Forms.TextBox
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents ListVideos As System.Windows.Forms.DataGridView
    Friend WithEvents NumberDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OriginalTitleDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TranslatedTitleDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents YearDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DateAddedDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents TextBox11 As System.Windows.Forms.TextBox
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents TextBox10 As System.Windows.Forms.TextBox
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents TextBox14 As System.Windows.Forms.TextBox
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents TextBox15 As System.Windows.Forms.TextBox
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents TextBox12 As System.Windows.Forms.TextBox
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents TextBox13 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox15 As System.Windows.Forms.GroupBox
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents TextBox18 As System.Windows.Forms.TextBox
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents TextBox17 As System.Windows.Forms.TextBox
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents TextBox16 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox21 As System.Windows.Forms.GroupBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents TextBox19 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox20 As System.Windows.Forms.TextBox
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents TextBox22 As System.Windows.Forms.TextBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents TextBox21 As System.Windows.Forms.TextBox
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents TextBox24 As System.Windows.Forms.TextBox
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents TextBox23 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox22 As System.Windows.Forms.GroupBox
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents TextBox25 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox26 As System.Windows.Forms.TextBox
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents TextBox28 As System.Windows.Forms.TextBox
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents TextBox27 As System.Windows.Forms.TextBox
    Friend WithEvents XionPanel1 As XionControls.XionPanel
    Friend WithEvents DataGridViewMovie As System.Windows.Forms.DataGridView
    Friend WithEvents cbMasterTitle As System.Windows.Forms.ComboBox
    Friend WithEvents lblMasterTitle As System.Windows.Forms.Label
    Friend WithEvents SaveConfigFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripProgressMessage As System.Windows.Forms.Label
    Friend WithEvents txtManualOldValue As System.Windows.Forms.TextBox
    Friend WithEvents lblManualEnterOldValue As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents TextBox29 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox31 As System.Windows.Forms.TextBox
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents TextBox30 As System.Windows.Forms.TextBox
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TrailerFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanTrailerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MyFilmsWikiToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
    Friend WithEvents chkDontAskInteractive As System.Windows.Forms.CheckBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnSelectParserFile As System.Windows.Forms.Button
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents txtParserFilePath As System.Windows.Forms.TextBox
    Friend WithEvents cbInternetLookupBehaviour As System.Windows.Forms.ComboBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents GroupBox17 As System.Windows.Forms.GroupBox
    Friend WithEvents chkRescanMovedFiles As System.Windows.Forms.CheckBox
    Friend WithEvents chkProhibitInternetLookup As System.Windows.Forms.CheckBox
    Friend WithEvents chkImportOnInternetFail As System.Windows.Forms.CheckBox
    Friend WithEvents chkPurgeMissing As System.Windows.Forms.CheckBox
    Friend WithEvents chkOverwriteXML As System.Windows.Forms.CheckBox
    Friend WithEvents chkBackupXMLFirst As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSelectFanartFolder As System.Windows.Forms.Button
    Friend WithEvents txtFanartFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents txtRegExSearchMultiPart As System.Windows.Forms.TextBox
    Friend WithEvents txtPersonArtworkFolder As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectPersonArtworkFolder As System.Windows.Forms.Button
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents chkParseSubtitleFiles As System.Windows.Forms.CheckBox
    Friend WithEvents txtTrailerIentificationStrings As System.Windows.Forms.TextBox
    Friend WithEvents chkParsePlaylistFiles As System.Windows.Forms.CheckBox
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents chkCheckDVDFolders As System.Windows.Forms.CheckBox
    Friend WithEvents txtDefaultFileTypesNonMedia As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtDefaultFileTypes As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents GroupBox25 As System.Windows.Forms.GroupBox
    Friend WithEvents chkParseTrailers As System.Windows.Forms.CheckBox
    Friend WithEvents cbTitleHandling As System.Windows.Forms.ComboBox
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents chkImportOnInternetFailInGuiMode As System.Windows.Forms.CheckBox
    Friend WithEvents txtParserFilePathDisplay As System.Windows.Forms.TextBox
    Friend WithEvents txtManualInternetParserPathDisplay As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox27 As System.Windows.Forms.GroupBox
    Friend WithEvents chkManualUpdateRecordsOnlyMissingData As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxMovieDetails As System.Windows.Forms.GroupBox
    Friend WithEvents Category As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Country As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Rating As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData As System.Windows.Forms.CheckBox
    Friend WithEvents chkManualDontAskInteractive As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox24 As System.Windows.Forms.GroupBox
    Friend WithEvents txtOverridePath As System.Windows.Forms.TextBox
    Friend WithEvents lblOverridePath As System.Windows.Forms.Label
    Friend WithEvents btnSelectMovieFolder As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtMovieFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents GroupBox_OtherFieldHandling As System.Windows.Forms.GroupBox
    Friend WithEvents chkShortNames As System.Windows.Forms.CheckBox
    Friend WithEvents chkReadDVDLabel As System.Windows.Forms.CheckBox
    Friend WithEvents txtMediaLabel As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtMediaType As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtDefaultSourceField As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents GroupBox_MediaLabelFieldHandling As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox_TitleHandling As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox_PictureHandling As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox_StorageFieldHandling As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox32 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox11 As System.Windows.Forms.GroupBox
    Friend WithEvents btnExcludeFileDelete As System.Windows.Forms.Button
    Friend WithEvents btnExcludeFileShow As System.Windows.Forms.Button
    Friend WithEvents txtExcludeFilePath As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnSelectExcludeFile As System.Windows.Forms.Button
    Friend WithEvents txtConfigFilePath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label88 As System.Windows.Forms.Label
    Friend WithEvents GroupBox26 As System.Windows.Forms.GroupBox
    Friend WithEvents Label87 As System.Windows.Forms.Label
    Friend WithEvents Label86 As System.Windows.Forms.Label
    Friend WithEvents Label85 As System.Windows.Forms.Label
    Friend WithEvents Label84 As System.Windows.Forms.Label
    Friend WithEvents chkGrabberOverrideTitleLimit As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverridePersonLimit As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverrideGetRoles As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverrideLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents Label_VersionNumber As System.Windows.Forms.Label
    Friend WithEvents LinkLabelMyFilmsWiki As System.Windows.Forms.LinkLabel
    Friend WithEvents cbManualSelectFieldDestination As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualSelectFieldDestination As System.Windows.Forms.Label
    Friend WithEvents cbFanartLimitResolutionMax As System.Windows.Forms.ComboBox
    Friend WithEvents cbNumFanartLimitNumber As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblFanartLimits As System.Windows.Forms.Label
    Friend WithEvents cbFanartLimitResolutionMin As System.Windows.Forms.ComboBox
    Friend WithEvents txtGroupNameIdentifier As System.Windows.Forms.TextBox
    Friend WithEvents Label89 As System.Windows.Forms.Label
    Friend WithEvents ViewPersons As System.Windows.Forms.TabPage
    Friend WithEvents PersonBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItemPerson As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem1 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItemPerson As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem1 As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents PersonBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents AntMovieCatalog As AntMovieCatalog
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents XionPanelPerson As XionControls.XionPanel
    Friend WithEvents DataGridViewPerson As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ViewPersons_Biography As System.Windows.Forms.RichTextBox
    Friend WithEvents Label91 As System.Windows.Forms.Label
    Friend WithEvents Label90 As System.Windows.Forms.Label
    Friend WithEvents ViewPersons_Name As System.Windows.Forms.TextBox
    Friend WithEvents ContentsIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SpeichernToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripButtonAddMissingPersons As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButtonGrabPersons As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripTextBoxSearch As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents Label92 As System.Windows.Forms.Label
    Friend WithEvents ViewPersons_OtherName As System.Windows.Forms.TextBox
    Friend WithEvents GroupBoxPersonInfo As System.Windows.Forms.GroupBox
    Friend WithEvents Label95 As System.Windows.Forms.Label
    Friend WithEvents Label94 As System.Windows.Forms.Label
    Friend WithEvents Label93 As System.Windows.Forms.Label
    Friend WithEvents ViewPersons_Birthplace As System.Windows.Forms.TextBox
    Friend WithEvents ViewPersons_Birthday As System.Windows.Forms.TextBox
    Friend WithEvents ViewPersons_MiniBio As System.Windows.Forms.RichTextBox
    Friend WithEvents ImageListViewPersons As System.Windows.Forms.ImageList
    Friend WithEvents Label99 As System.Windows.Forms.Label
    Friend WithEvents Label98 As System.Windows.Forms.Label
    Friend WithEvents Label97 As System.Windows.Forms.Label
    Friend WithEvents Label96 As System.Windows.Forms.Label
    Friend WithEvents ViewPersons_Photos As System.Windows.Forms.TextBox
    Friend WithEvents ViewPersons_URL As System.Windows.Forms.TextBox
    Friend WithEvents ViewPersons_TMDB_Id As System.Windows.Forms.TextBox
    Friend WithEvents ViewPersons_IMDB_Id As System.Windows.Forms.TextBox
    Friend WithEvents ToolStripMenuItemOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemScanPath As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemsScanFilter As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckBoxWriter As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxDirector As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxProducer As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxActor As System.Windows.Forms.CheckBox
    Friend WithEvents PersonIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NameDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AlternateNameDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BornDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BirthPlaceDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MiniBiographyDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BiographyDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents URLDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IMDBIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TMDBIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PictureDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PhotosDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IsActor As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents IsProducer As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents IsDirector As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents IsWriter As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ViewCatalog As System.Windows.Forms.TabPage
    Friend WithEvents PropertiesIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CustomPropertiesDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label103 As System.Windows.Forms.Label
    Friend WithEvents Label102 As System.Windows.Forms.Label
    Friend WithEvents TextBox33 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox32 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox34 As System.Windows.Forms.TextBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Label104 As System.Windows.Forms.Label
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox35 As System.Windows.Forms.TextBox
    Friend WithEvents Label105 As System.Windows.Forms.Label
    Friend WithEvents Label106 As System.Windows.Forms.Label
    Friend WithEvents cbEditionNameAppliesTo As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox28 As System.Windows.Forms.GroupBox
    Friend WithEvents dgEditionStrings As System.Windows.Forms.DataGridView
    Friend WithEvents EditorSearchExpression As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents EditorReplacementString As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cbManualNfoFileHandling As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualNfoFileHandling As System.Windows.Forms.Label
    Friend WithEvents chkManualNfoFilesOnlyAddMissing As System.Windows.Forms.CheckBox
    Friend WithEvents OwnerTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PropertiesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents MailTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SiteTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DescriptionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents CustomFieldDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn10 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn11 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn12 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn13 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CustomFieldBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents UserDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn14 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn15 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn16 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn17 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UserBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Label100 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioChannelCount As System.Windows.Forms.TextBox
    Friend WithEvents BtnImportWatcher As System.Windows.Forms.Button
    Friend WithEvents btnOpenLog As System.Windows.Forms.Button
    Friend WithEvents chkPurgeMissingAlways As System.Windows.Forms.CheckBox
    Friend WithEvents txtSeriesNameIdentifier As System.Windows.Forms.TextBox
    Friend WithEvents Label101 As System.Windows.Forms.Label
    Friend WithEvents GroupBox_FanartHandling As System.Windows.Forms.GroupBox
    Friend WithEvents txtPictureFilenameSuffix As System.Windows.Forms.TextBox
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorUpdate As System.Windows.Forms.ToolStripButton
    Friend WithEvents TextBox36 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox37 As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleAspectRatio As System.Windows.Forms.TextBox
    Friend WithEvents Label107 As System.Windows.Forms.Label
    Friend WithEvents chkUseInternetDataForLanguagesField As System.Windows.Forms.CheckBox
    Friend WithEvents chkUseGrabberForFanart As System.Windows.Forms.CheckBox
    Friend WithEvents chkLoadPersonImagesWithFanart As System.Windows.Forms.CheckBox
    Friend WithEvents btnSelectConfigFile As System.Windows.Forms.Button
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents cbSkipExcludedMovieFiles As System.Windows.Forms.CheckBox
    Friend WithEvents cbInternetLookupAlwaysPrompt As System.Windows.Forms.CheckBox
    Friend WithEvents lblInternetLookupCaseExplanation As System.Windows.Forms.Label
    Friend WithEvents GroupBox9 As System.Windows.Forms.GroupBox
    Friend WithEvents cbManualInternetLookupAlwaysPrompt As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox38 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox39 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox40 As System.Windows.Forms.TextBox
    Friend WithEvents btnImportFromDrive As System.Windows.Forms.Button
    Friend WithEvents txtSampleVideo3D As System.Windows.Forms.TextBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
End Class
