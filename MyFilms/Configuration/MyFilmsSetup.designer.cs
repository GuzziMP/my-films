using System.Windows.Forms;
using MyFilmsPlugin.DataBase;
using MyFilmsPlugin.Utils;

namespace MyFilmsPlugin.Configuration
{
  partial class MyFilmsSetup
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        public string PluginName()
        {
            return "MyFilms";
        }

        public string Description()
        {
            return "MyFilms Ant Movie Catalog";
        }

        public string Author()
        {
            return "Zebons (Mod by Guzzi)";
        }

        public void ShowPlugin()
        {
            ShowDialog();
        }
        public bool DefaultEnabled()
        {
            return false;
        }
        public bool CanEnable()
        {
            return true;
        }

        public bool HasSetup()
        {
            return true;
        }
        public int GetWindowId()
        {
            return 7986;
        }
        /// <summary>
        /// If the plugin should have its own button on the home screen then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true  : plugin needs its own button on home
        ///          false : plugin does not need its own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {

            strButtonText = GUILocalizeStrings.Get(50);
            if (strButtonText == "")
            {
                strButtonText = "MyFilms";
            }
            strButtonImage = "";
            strButtonImageFocus = "";
            strPictureImage = "hover_MyFilms.png";
            return true;
        }
        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label ownerLabel;
      System.Windows.Forms.Label mailLabel;
      System.Windows.Forms.Label siteLabel;
      System.Windows.Forms.Label descriptionLabel;
      System.Windows.Forms.Label indexLabel1;
      System.Windows.Forms.Label labelLabel;
      System.Windows.Forms.Label dBfieldLabel;
      System.Windows.Forms.Label valueLabel;
      System.Windows.Forms.Label filterLabel;
      System.Windows.Forms.Label sortFieldHierarchyLabel;
      System.Windows.Forms.Label sortDirectionHierarchyLabel;
      System.Windows.Forms.Label layoutHierarchyLabel;
      System.Windows.Forms.Label label68;
      System.Windows.Forms.Label label70;
      System.Windows.Forms.Label label69;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyFilmsSetup));
      System.Windows.Forms.Label label66;
      System.Windows.Forms.Label label65;
      this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.Config_Name = new System.Windows.Forms.ComboBox();
      this.textBoxPluginName = new System.Windows.Forms.TextBox();
      this.ButQuit = new System.Windows.Forms.Button();
      this.ButDelet = new System.Windows.Forms.Button();
      this.Config_Dflt = new System.Windows.Forms.CheckBox();
      this.Config_Menu = new System.Windows.Forms.CheckBox();
      this.label_VersionNumber = new System.Windows.Forms.Label();
      this.ButCopy = new System.Windows.Forms.Button();
      this.cbAllowRecentAddedAPI = new System.Windows.Forms.CheckBox();
      this.cbAllowTraktSync = new System.Windows.Forms.CheckBox();
      this.groupBox9 = new System.Windows.Forms.GroupBox();
      this.chkDVDprofilerMergeWithGenreField = new System.Windows.Forms.CheckBox();
      this.groupBox25 = new System.Windows.Forms.GroupBox();
      this.label37 = new System.Windows.Forms.Label();
      this.comboWOLtimeout = new System.Windows.Forms.ComboBox();
      this.buttonSendMagicPacket3 = new System.Windows.Forms.Button();
      this.buttonSendMagicPacket2 = new System.Windows.Forms.Button();
      this.label30 = new System.Windows.Forms.Label();
      this.buttonSendMagicPacket1 = new System.Windows.Forms.Button();
      this.buttonGetMACadresses = new System.Windows.Forms.Button();
      this.label_NAS_Server_3_MAC = new System.Windows.Forms.Label();
      this.NAS_MAC_3 = new System.Windows.Forms.TextBox();
      this.label_NAS_Server_3_Name = new System.Windows.Forms.Label();
      this.NAS_Name_3 = new System.Windows.Forms.TextBox();
      this.label_NAS_Server_2_MAC = new System.Windows.Forms.Label();
      this.NAS_MAC_2 = new System.Windows.Forms.TextBox();
      this.label_NAS_Server_2_Name = new System.Windows.Forms.Label();
      this.NAS_Name_2 = new System.Windows.Forms.TextBox();
      this.label_NAS_Server_1_MAC = new System.Windows.Forms.Label();
      this.NAS_MAC_1 = new System.Windows.Forms.TextBox();
      this.label_NAS_Server_1_Name = new System.Windows.Forms.Label();
      this.check_WOL_Userdialog = new System.Windows.Forms.CheckBox();
      this.check_WOL_enable = new System.Windows.Forms.CheckBox();
      this.NAS_Name_1 = new System.Windows.Forms.TextBox();
      this.label36 = new System.Windows.Forms.Label();
      this.SLogo_Type = new System.Windows.Forms.ComboBox();
      this.chkLogos = new System.Windows.Forms.CheckBox();
      this.LogoView = new System.Windows.Forms.ListView();
      this.LogoType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Field1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Op1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Value1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.And_Or = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Field2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Op2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Value2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Image = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.PathImage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.txtLogosPath = new System.Windows.Forms.TextBox();
      this.comboBoxLogoSpacing = new System.Windows.Forms.ComboBox();
      this.comboBoxLogoPresets = new System.Windows.Forms.ComboBox();
      this.btnLogoClearCache = new System.Windows.Forms.Button();
      this.Fanart = new System.Windows.Forms.GroupBox();
      this.buttonDefaultFanartReset = new System.Windows.Forms.Button();
      this.pictureBoxDefaultFanart = new System.Windows.Forms.PictureBox();
      this.chkFanartDefaultViewsUseRandom = new System.Windows.Forms.CheckBox();
      this.chkFanartDefaultViews = new System.Windows.Forms.CheckBox();
      this.chkDfltFanartImageAll = new System.Windows.Forms.CheckBox();
      this.label48 = new System.Windows.Forms.Label();
      this.chkDfltFanartImage = new System.Windows.Forms.CheckBox();
      this.lblResultingGroupViewsPathFanart = new System.Windows.Forms.Label();
      this.label40 = new System.Windows.Forms.Label();
      this.chkFanart = new System.Windows.Forms.CheckBox();
      this.btnFanart = new System.Windows.Forms.Button();
      this.MesFilmsFanart = new System.Windows.Forms.TextBox();
      this.labelFanart = new System.Windows.Forms.Label();
      this.chkDfltFanart = new System.Windows.Forms.CheckBox();
      this.groupBox22 = new System.Windows.Forms.GroupBox();
      this.chkShowIndexedImgInIndViews = new System.Windows.Forms.CheckBox();
      this.buttonDefaultViewImageReset = new System.Windows.Forms.Button();
      this.pictureBoxDefaultViewImage = new System.Windows.Forms.PictureBox();
      this.chkDfltViewsAll = new System.Windows.Forms.CheckBox();
      this.label47 = new System.Windows.Forms.Label();
      this.btnResetThumbs = new System.Windows.Forms.Button();
      this.chkViews = new System.Windows.Forms.CheckBox();
      this.btnViews = new System.Windows.Forms.Button();
      this.chkDfltViews = new System.Windows.Forms.CheckBox();
      this.label29 = new System.Windows.Forms.Label();
      this.MesFilmsViews = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btnResetThumbsFilms = new System.Windows.Forms.Button();
      this.buttonDefaultCoverReset = new System.Windows.Forms.Button();
      this.pictureBoxDefaultCover = new System.Windows.Forms.PictureBox();
      this.lblPicturePrefix = new System.Windows.Forms.Label();
      this.txtPicturePrefix = new System.Windows.Forms.TextBox();
      this.lblPictureHandling = new System.Windows.Forms.Label();
      this.cbPictureHandling = new System.Windows.Forms.ComboBox();
      this.label_DefaulCover = new System.Windows.Forms.Label();
      this.ButImg = new System.Windows.Forms.Button();
      this.MesFilmsImg = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox_ArtistImages = new System.Windows.Forms.GroupBox();
      this.chkPersonsEnableDownloads = new System.Windows.Forms.CheckBox();
      this.buttonDefaultPersonImageReset = new System.Windows.Forms.Button();
      this.pictureBoxDefaultPersonImage = new System.Windows.Forms.PictureBox();
      this.chkPersons = new System.Windows.Forms.CheckBox();
      this.btnResetThumbsArtist = new System.Windows.Forms.Button();
      this.chkDfltArtist = new System.Windows.Forms.CheckBox();
      this.ButImgArtist = new System.Windows.Forms.Button();
      this.MesFilmsImgArtist = new System.Windows.Forms.TextBox();
      this.label_DefaultArtistImage = new System.Windows.Forms.Label();
      this.label_ArtistImagePath = new System.Windows.Forms.Label();
      this.groupBox_AMCupdater = new System.Windows.Forms.GroupBox();
      this.chkAMC_ImportOnInternetFail = new System.Windows.Forms.CheckBox();
      this.labelAMCUstartdelay = new System.Windows.Forms.Label();
      this.numericUpDownAMCUscanStartDelay = new System.Windows.Forms.NumericUpDown();
      this.chkAMCUwatchScanFolders = new System.Windows.Forms.CheckBox();
      this.chkAMCUscanOnStartup = new System.Windows.Forms.CheckBox();
      this.groupBoxAMCUpdaterConfigFile = new System.Windows.Forms.GroupBox();
      this.txtAMCUpd_cnf_Display = new System.Windows.Forms.TextBox();
      this.btnAMCUpd_cnf = new System.Windows.Forms.Button();
      this.txtAMCUpd_cnf = new System.Windows.Forms.TextBox();
      this.groupBox_AMCupdaterScheduer = new System.Windows.Forms.GroupBox();
      this.scheduleAMCUpdater = new System.Windows.Forms.CheckBox();
      this.btnParameters = new System.Windows.Forms.Button();
      this.groupBox_AMCupdater_ExternalApplication = new System.Windows.Forms.GroupBox();
      this.label56 = new System.Windows.Forms.Label();
      this.AmcTitleSearchHandling = new System.Windows.Forms.ComboBox();
      this.btnCreateAMCDefaultConfig = new System.Windows.Forms.Button();
      this.lblAMCMovieScanPath = new System.Windows.Forms.Label();
      this.chkAMC_Purge_Missing_Files = new System.Windows.Forms.CheckBox();
      this.btnAMCMovieScanPathAdd = new System.Windows.Forms.Button();
      this.AMCMovieScanPath = new System.Windows.Forms.TextBox();
      this.btnCreateAMCDesktopIcon = new System.Windows.Forms.Button();
      this.chkAMCUpd = new System.Windows.Forms.CheckBox();
      this.groupBoxDeletionOptions = new System.Windows.Forms.GroupBox();
      this.cbSuppressAutomaticAction = new System.Windows.Forms.ComboBox();
      this.gpspfield = new System.Windows.Forms.GroupBox();
      this.label61 = new System.Windows.Forms.Label();
      this.chkSuppressPlayStopUpdateField = new System.Windows.Forms.CheckBox();
      this.txtSuppressPlayStopValueToUpdate = new System.Windows.Forms.TextBox();
      this.cbSuppressPlayStopFieldToUpdate = new System.Windows.Forms.ComboBox();
      this.lblUpdateValue = new System.Windows.Forms.Label();
      this.chkSuppressManual = new System.Windows.Forms.CheckBox();
      this.chkSuppressAutomatic = new System.Windows.Forms.CheckBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnWatchedImport = new System.Windows.Forms.Button();
      this.btnWatchedExport = new System.Windows.Forms.Button();
      this.lblUnwatchedItemsValue = new System.Windows.Forms.Label();
      this.textBoxGlobalUnwatchedOnlyValue = new System.Windows.Forms.TextBox();
      this.CheckWatchedPlayerStopped = new System.Windows.Forms.CheckBox();
      this.label19 = new System.Windows.Forms.Label();
      this.cbWatched = new System.Windows.Forms.ComboBox();
      this.CheckWatched = new System.Windows.Forms.CheckBox();
      this.btnMUSdeleteUserData = new System.Windows.Forms.Button();
      this.UserProfileName = new System.Windows.Forms.TextBox();
      this.groupBox_UserItemsDetails = new System.Windows.Forms.GroupBox();
      this.AntLabelDetails6 = new System.Windows.Forms.TextBox();
      this.AntItemDetails6 = new System.Windows.Forms.ComboBox();
      this.label_UserItemDetails6 = new System.Windows.Forms.Label();
      this.label_DetailsLabel = new System.Windows.Forms.Label();
      this.label_DetailsDBitem = new System.Windows.Forms.Label();
      this.label_UserItemDetails5 = new System.Windows.Forms.Label();
      this.label_UserItemDetails4 = new System.Windows.Forms.Label();
      this.label_UserItemDetails3 = new System.Windows.Forms.Label();
      this.label_UserItemDetails2 = new System.Windows.Forms.Label();
      this.label_UserItemDetails1 = new System.Windows.Forms.Label();
      this.AntLabelDetails5 = new System.Windows.Forms.TextBox();
      this.AntLabelDetails4 = new System.Windows.Forms.TextBox();
      this.AntLabelDetails3 = new System.Windows.Forms.TextBox();
      this.AntLabelDetails2 = new System.Windows.Forms.TextBox();
      this.AntLabelDetails1 = new System.Windows.Forms.TextBox();
      this.AntItemDetails5 = new System.Windows.Forms.ComboBox();
      this.AntItemDetails4 = new System.Windows.Forms.ComboBox();
      this.AntItemDetails3 = new System.Windows.Forms.ComboBox();
      this.AntItemDetails2 = new System.Windows.Forms.ComboBox();
      this.AntItemDetails1 = new System.Windows.Forms.ComboBox();
      this.groupBox_UserItemsMain = new System.Windows.Forms.GroupBox();
      this.label_UserItem5 = new System.Windows.Forms.Label();
      this.label_UserItem4 = new System.Windows.Forms.Label();
      this.label_UserItem3 = new System.Windows.Forms.Label();
      this.label_UserItem2 = new System.Windows.Forms.Label();
      this.label_UserItem1 = new System.Windows.Forms.Label();
      this.AntLabel5 = new System.Windows.Forms.TextBox();
      this.AntLabel4 = new System.Windows.Forms.TextBox();
      this.AntItem5 = new System.Windows.Forms.ComboBox();
      this.AntItem4 = new System.Windows.Forms.ComboBox();
      this.AntLabel3 = new System.Windows.Forms.TextBox();
      this.AntItem3 = new System.Windows.Forms.ComboBox();
      this.AntItem2 = new System.Windows.Forms.ComboBox();
      this.AntLabel2 = new System.Windows.Forms.TextBox();
      this.label_MainDBitem = new System.Windows.Forms.Label();
      this.AntItem1 = new System.Windows.Forms.ComboBox();
      this.labelMainLabel = new System.Windows.Forms.Label();
      this.AntLabel1 = new System.Windows.Forms.TextBox();
      this.groupBox_Separators = new System.Windows.Forms.GroupBox();
      this.RoleSeparator5 = new System.Windows.Forms.ComboBox();
      this.RoleSeparator4 = new System.Windows.Forms.ComboBox();
      this.ListSeparator5 = new System.Windows.Forms.ComboBox();
      this.ListSeparator4 = new System.Windows.Forms.ComboBox();
      this.ListSeparator3 = new System.Windows.Forms.ComboBox();
      this.ListSeparator2 = new System.Windows.Forms.ComboBox();
      this.RoleSeparator2 = new System.Windows.Forms.ComboBox();
      this.RoleSeparator3 = new System.Windows.Forms.ComboBox();
      this.label22 = new System.Windows.Forms.Label();
      this.RoleSeparator1 = new System.Windows.Forms.ComboBox();
      this.label24 = new System.Windows.Forms.Label();
      this.ListSeparator1 = new System.Windows.Forms.ComboBox();
      this.groupBox_DefaultView = new System.Windows.Forms.GroupBox();
      this.chkVirtualPathBrowsing = new System.Windows.Forms.CheckBox();
      this.chkReversePersonNames = new System.Windows.Forms.CheckBox();
      this.chkShowEmpty = new System.Windows.Forms.CheckBox();
      this.chkOnlyTitle = new System.Windows.Forms.CheckBox();
      this.groupBox7 = new System.Windows.Forms.GroupBox();
      this.Sort = new System.Windows.Forms.ComboBox();
      this.label32 = new System.Windows.Forms.Label();
      this.SortSens = new System.Windows.Forms.ComboBox();
      this.LayOut = new System.Windows.Forms.ComboBox();
      this.label14 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.label26 = new System.Windows.Forms.Label();
      this.LayoutInHierarchies = new System.Windows.Forms.ComboBox();
      this.label62 = new System.Windows.Forms.Label();
      this.SortSensInHierarchies = new System.Windows.Forms.ComboBox();
      this.SortInHierarchies = new System.Windows.Forms.ComboBox();
      this.label10 = new System.Windows.Forms.Label();
      this.chkGlobalAvailableOnly = new System.Windows.Forms.CheckBox();
      this.chkGlobalUnwatchedOnly = new System.Windows.Forms.CheckBox();
      this.AlwaysDefaultView = new System.Windows.Forms.CheckBox();
      this.View_Dflt_Item = new System.Windows.Forms.ComboBox();
      this.View_Dflt_Text = new System.Windows.Forms.TextBox();
      this.groupBoxView = new System.Windows.Forms.GroupBox();
      this.labelViewLabel = new System.Windows.Forms.Label();
      this.viewBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.mFview = new MyFilmsPlugin.DataBase.MFview();
      this.buttonResetImage = new System.Windows.Forms.Button();
      this.AntViewsImage = new System.Windows.Forms.PictureBox();
      this.lblAntViewIndex = new System.Windows.Forms.Label();
      this.bindingNavigatorViews = new System.Windows.Forms.BindingNavigator(this.components);
      this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
      this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
      this.toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripButtonAddDefaults = new System.Windows.Forms.ToolStripButton();
      this.dgViewsList = new System.Windows.Forms.DataGridView();
      this.labelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.viewEnabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.groupBoxSortAndLayoutForView = new System.Windows.Forms.GroupBox();
      this.AntViewSortType = new System.Windows.Forms.ComboBox();
      this.AntViewLayoutView = new System.Windows.Forms.ComboBox();
      this.AntViewSortOrder = new System.Windows.Forms.ComboBox();
      this.AntViewItem = new System.Windows.Forms.ComboBox();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.AntViewFilter = new System.Windows.Forms.TextBox();
      this.AntViewFilterEditButton = new System.Windows.Forms.Button();
      this.AntViewFilterSaveString = new System.Windows.Forms.TextBox();
      this.AntViewValue = new System.Windows.Forms.TextBox();
      this.AntViewIndex = new System.Windows.Forms.NumericUpDown();
      this.groupBox24 = new System.Windows.Forms.GroupBox();
      this.numUpDownTrailersInIntro = new System.Windows.Forms.NumericUpDown();
      this.cbCacheOnlineTrailer = new System.Windows.Forms.CheckBox();
      this.cbTrailerAutoregister = new System.Windows.Forms.CheckBox();
      this.labelTrailers = new System.Windows.Forms.Label();
      this.ShowTrailerWhenStartingMovie = new System.Windows.Forms.CheckBox();
      this.btnTrailer = new System.Windows.Forms.Button();
      this.PathStorageTrailer = new System.Windows.Forms.TextBox();
      this.label34 = new System.Windows.Forms.Label();
      this.AntStorageTrailer = new System.Windows.Forms.ComboBox();
      this.label35 = new System.Windows.Forms.Label();
      this.groupBox_AntSelectedEnreg = new System.Windows.Forms.GroupBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btnCustomConfigFilter = new System.Windows.Forms.Button();
      this.textBoxStrDfltSelect = new System.Windows.Forms.TextBox();
      this.AntFreetextFilterItem = new System.Windows.Forms.Label();
      this.AntFilterFreeText = new System.Windows.Forms.TextBox();
      this.AntFilterComb = new System.Windows.Forms.ComboBox();
      this.AntFilterSign2 = new System.Windows.Forms.ComboBox();
      this.AntFilterSign1 = new System.Windows.Forms.ComboBox();
      this.AntFilterItem2 = new System.Windows.Forms.ComboBox();
      this.AntFilterText2 = new System.Windows.Forms.TextBox();
      this.AntFilterItem1 = new System.Windows.Forms.ComboBox();
      this.AntFilterText1 = new System.Windows.Forms.TextBox();
      this.groupBox_PreLaunchingCommand = new System.Windows.Forms.GroupBox();
      this.label39 = new System.Windows.Forms.Label();
      this.label38 = new System.Windows.Forms.Label();
      this.CmdPar = new System.Windows.Forms.ComboBox();
      this.CmdExe = new System.Windows.Forms.TextBox();
      this.tbExternalPlayerStartParams = new System.Windows.Forms.TextBox();
      this.tbExternalPlayerPath = new System.Windows.Forms.TextBox();
      this.tbExternalPlayerExtensions = new System.Windows.Forms.TextBox();
      this.chkScanMediaOnStart = new System.Windows.Forms.CheckBox();
      this.SearchOnlyExactMatches = new System.Windows.Forms.CheckBox();
      this.PathStorage = new System.Windows.Forms.TextBox();
      this.SearchSubDirs = new System.Windows.Forms.CheckBox();
      this.SearchFileName = new System.Windows.Forms.CheckBox();
      this.ItemSearchFileName = new System.Windows.Forms.ComboBox();
      this.AntStorage = new System.Windows.Forms.ComboBox();
      this.AntIdentItem = new System.Windows.Forms.ComboBox();
      this.groupBox_Security = new System.Windows.Forms.GroupBox();
      this.label16 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.Rpt_Dwp = new System.Windows.Forms.TextBox();
      this.Dwp = new System.Windows.Forms.TextBox();
      this.CatalogType = new System.Windows.Forms.ComboBox();
      this.groupBox_TitleOrder = new System.Windows.Forms.GroupBox();
      this.label20 = new System.Windows.Forms.Label();
      this.AntSTitle = new System.Windows.Forms.ComboBox();
      this.label17 = new System.Windows.Forms.Label();
      this.TitleDelim = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.AntTitle2 = new System.Windows.Forms.ComboBox();
      this.AntTitle1 = new System.Windows.Forms.ComboBox();
      this.MesFilmsCat = new System.Windows.Forms.TextBox();
      this.butNew = new System.Windows.Forms.Button();
      this.groupBoxExtendedFieldHandling = new System.Windows.Forms.GroupBox();
      this.ECMergeDestinationFieldWriter = new System.Windows.Forms.ComboBox();
      this.ECMergeDestinationFieldCertification = new System.Windows.Forms.ComboBox();
      this.ECMergeDestinationFieldTags = new System.Windows.Forms.ComboBox();
      this.chkAddWriter = new System.Windows.Forms.CheckBox();
      this.ECMergeDestinationFieldTagline = new System.Windows.Forms.ComboBox();
      this.chkAddTagline = new System.Windows.Forms.CheckBox();
      this.chkAddCertification = new System.Windows.Forms.CheckBox();
      this.label54 = new System.Windows.Forms.Label();
      this.chkAddTags = new System.Windows.Forms.CheckBox();
      this.label53 = new System.Windows.Forms.Label();
      this.btnLaunchAMCglobal = new System.Windows.Forms.Button();
      this.SPicture = new System.Windows.Forms.PictureBox();
      this.groupBox_DVDprofiler = new System.Windows.Forms.GroupBox();
      this.groupBox13 = new System.Windows.Forms.GroupBox();
      this.chkDVDprofilerOnlyFile = new System.Windows.Forms.CheckBox();
      this.buttonDeleteTmpCatalog = new System.Windows.Forms.Button();
      this.buttonOpenTmpFileAMC = new System.Windows.Forms.Button();
      this.AMCConfigView = new System.Windows.Forms.ListView();
      this.Option = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.btnHyperLinkParamGen = new System.Windows.Forms.Button();
      this.groupBox_GrabberOptions = new System.Windows.Forms.GroupBox();
      this.lblFilterGrabberScripts = new System.Windows.Forms.Label();
      this.ItemSearchGrabberScriptsFilter = new System.Windows.Forms.TextBox();
      this.txtGrabberDisplay = new System.Windows.Forms.TextBox();
      this.chkGrabber_Always = new System.Windows.Forms.CheckBox();
      this.btnEditScript = new System.Windows.Forms.Button();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.cbGrabberOverrideGetRoles = new System.Windows.Forms.ComboBox();
      this.label55 = new System.Windows.Forms.Label();
      this.label51 = new System.Windows.Forms.Label();
      this.cbGrabberOverrideTitleLimit = new System.Windows.Forms.ComboBox();
      this.label50 = new System.Windows.Forms.Label();
      this.label49 = new System.Windows.Forms.Label();
      this.cbGrabberOverridePersonLimit = new System.Windows.Forms.ComboBox();
      this.cbGrabberOverrideLanguage = new System.Windows.Forms.ComboBox();
      this.lblSearchGrabberName = new System.Windows.Forms.Label();
      this.chkGrabber_ChooseScript = new System.Windows.Forms.CheckBox();
      this.ItemSearchGrabberName = new System.Windows.Forms.ComboBox();
      this.btnGrabber = new System.Windows.Forms.Button();
      this.txtGrabber = new System.Windows.Forms.TextBox();
      this.label27 = new System.Windows.Forms.Label();
      this.btnFirstTimeSetup = new System.Windows.Forms.Button();
      this.buttonWikiHelp = new System.Windows.Forms.Button();
      this.btnServerSync = new System.Windows.Forms.Button();
      this.groupBox_UpdateGrabberScripts = new System.Windows.Forms.GroupBox();
      this.button_DeleteBackupScripts = new System.Windows.Forms.Button();
      this.label_UpdateGrabberScriptsInstructions = new System.Windows.Forms.Label();
      this.textBoxUpdateGrabberScripts = new System.Windows.Forms.TextBox();
      this.progressBarUpdateGrabberScripts = new System.Windows.Forms.ProgressBar();
      this.buttonUpdateGrabberScripts = new System.Windows.Forms.Button();
      this.groupBoxMultiUserState = new System.Windows.Forms.GroupBox();
      this.Label_UserProfileName = new System.Windows.Forms.Label();
      this.chkEnhancedWatchedStatusHandling = new System.Windows.Forms.CheckBox();
      this.Config_EnablePreload = new System.Windows.Forms.CheckBox();
      this.btnTestMode = new System.Windows.Forms.Button();
      this.txtCountry = new System.Windows.Forms.TextBox();
      this.SOp1 = new System.Windows.Forms.ComboBox();
      this.Tab_Trakt = new System.Windows.Forms.TabPage();
      this.groupBoxExternal = new System.Windows.Forms.GroupBox();
      this.labelRecentlyAddedAPI = new System.Windows.Forms.Label();
      this.groupBoxTrakt = new System.Windows.Forms.GroupBox();
      this.linkLabelUsingTraktInMyFilmsWiki = new System.Windows.Forms.LinkLabel();
      this.linkLabelTraktWiki = new System.Windows.Forms.LinkLabel();
      this.labelTraktDescription = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.linkLabelTrakt = new System.Windows.Forms.LinkLabel();
      this.Tab_Display = new System.Windows.Forms.TabPage();
      this.Tab_Views = new System.Windows.Forms.TabPage();
      this.Tab_Trailer = new System.Windows.Forms.TabPage();
      this.Tab_General = new System.Windows.Forms.TabPage();
      this.lblYellowShowRequiredItems = new System.Windows.Forms.Label();
      this.groupBox_PlayMovieInfos = new System.Windows.Forms.GroupBox();
      this.butExternalPlayer = new System.Windows.Forms.Button();
      this.label23 = new System.Windows.Forms.Label();
      this.groupBoxMoviePathInfos = new System.Windows.Forms.GroupBox();
      this.label5 = new System.Windows.Forms.Label();
      this.butPath = new System.Windows.Forms.Button();
      this.AntIdentLabel = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.ButCat = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.ButSave = new System.Windows.Forms.Button();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.label21 = new System.Windows.Forms.Label();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.comboBox2 = new System.Windows.Forms.ComboBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.Tab_ExternalCatalogs = new System.Windows.Forms.TabPage();
      this.groupBoxAMCsettings = new System.Windows.Forms.GroupBox();
      this.AMCexePath = new System.Windows.Forms.TextBox();
      this.lblAMCpath = new System.Windows.Forms.Label();
      this.buttonAMCpathSearch = new System.Windows.Forms.Button();
      this.buttonOpenTmpFile = new System.Windows.Forms.Button();
      this.Tab_Network = new System.Windows.Forms.TabPage();
      this.Tab_Logos = new System.Windows.Forms.TabPage();
      this.lblCountry = new System.Windows.Forms.Label();
      this.btnUpdate = new System.Windows.Forms.Button();
      this.lblLogoPresets = new System.Windows.Forms.Label();
      this.lbl_LogoSpacing = new System.Windows.Forms.Label();
      this.lblSetupLogoRules = new System.Windows.Forms.Label();
      this.lblInfoLogosForAll = new System.Windows.Forms.Label();
      this.lblSelectLogoFile = new System.Windows.Forms.Label();
      this.btnLogosPath = new System.Windows.Forms.Button();
      this.SFilePicture = new System.Windows.Forms.TextBox();
      this.lblLogosPath = new System.Windows.Forms.Label();
      this.btnDown = new System.Windows.Forms.Button();
      this.btnUp = new System.Windows.Forms.Button();
      this.btnDel = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.SAnd_Or = new System.Windows.Forms.ComboBox();
      this.SValue2 = new System.Windows.Forms.ComboBox();
      this.SOp2 = new System.Windows.Forms.ComboBox();
      this.SField2 = new System.Windows.Forms.ComboBox();
      this.SValue1 = new System.Windows.Forms.ComboBox();
      this.SField1 = new System.Windows.Forms.ComboBox();
      this.Tab_Artwork = new System.Windows.Forms.TabPage();
      this.Tab_AMCupdater = new System.Windows.Forms.TabPage();
      this.Tab_Update = new System.Windows.Forms.TabPage();
      this.General = new System.Windows.Forms.TabControl();
      this.Tab_About = new System.Windows.Forms.TabPage();
      this.lblTmdbNote = new System.Windows.Forms.Label();
      this.pictureBoxTMDBlogo = new System.Windows.Forms.PictureBox();
      this.groupBoxSupportedCatalogs = new System.Windows.Forms.GroupBox();
      this.label41 = new System.Windows.Forms.Label();
      this.groupBoxAbout = new System.Windows.Forms.GroupBox();
      this.label28 = new System.Windows.Forms.Label();
      this.Tab_Other = new System.Windows.Forms.TabPage();
      this.personDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.dataGridViewCheckBoxColumn2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.dataGridViewCheckBoxColumn3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.dataGridViewCheckBoxColumn4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.dataGridViewTextBoxColumn18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.personBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.antMovieCatalog = new MyFilmsPlugin.DataBase.AntMovieCatalog();
      this.customFieldDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.customFieldBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.ownerTextBox = new System.Windows.Forms.TextBox();
      this.propertiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.mailTextBox = new System.Windows.Forms.TextBox();
      this.siteTextBox = new System.Windows.Forms.TextBox();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.lblAMCupdaterConfigPreview = new System.Windows.Forms.Label();
      this.Tab_OldStuff = new System.Windows.Forms.TabPage();
      this.btnAMCimport = new System.Windows.Forms.Button();
      this.groupBox8 = new System.Windows.Forms.GroupBox();
      this.label25 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.LayOutViewFilms = new System.Windows.Forms.ComboBox();
      this.SortSensViewFilms = new System.Windows.Forms.ComboBox();
      this.SortViewFilms = new System.Windows.Forms.ComboBox();
      this.textBoxNBconfigs = new System.Windows.Forms.TextBox();
      this.lblNbConfig = new System.Windows.Forms.Label();
      this.pictureBoxMyFilms = new System.Windows.Forms.PictureBox();
      this.customFieldsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.btnEditView = new System.Windows.Forms.Button();
      this.label52 = new System.Windows.Forms.Label();
      this.label33 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.indexNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.labelTextBox = new System.Windows.Forms.TextBox();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.filterTextBox = new System.Windows.Forms.TextBox();
      this.showEmptyCheckBox = new System.Windows.Forms.CheckBox();
      this.reverseNamesCheckBox = new System.Windows.Forms.CheckBox();
      this.sortOcurrenciesCheckBox = new System.Windows.Forms.CheckBox();
      this.sortDirectionViewComboBox = new System.Windows.Forms.ComboBox();
      this.layoutViewComboBox = new System.Windows.Forms.ComboBox();
      this.sortFieldHierarchyComboBox = new System.Windows.Forms.ComboBox();
      this.sortDirectionHierarchyComboBox = new System.Windows.Forms.ComboBox();
      this.layoutHierarchyComboBox = new System.Windows.Forms.ComboBox();
      this.sortFieldFilmsComboBox = new System.Windows.Forms.ComboBox();
      this.sortDirectionFilmsComboBox = new System.Windows.Forms.ComboBox();
      this.layoutFilmsComboBox = new System.Windows.Forms.ComboBox();
      ownerLabel = new System.Windows.Forms.Label();
      mailLabel = new System.Windows.Forms.Label();
      siteLabel = new System.Windows.Forms.Label();
      descriptionLabel = new System.Windows.Forms.Label();
      indexLabel1 = new System.Windows.Forms.Label();
      labelLabel = new System.Windows.Forms.Label();
      dBfieldLabel = new System.Windows.Forms.Label();
      valueLabel = new System.Windows.Forms.Label();
      filterLabel = new System.Windows.Forms.Label();
      sortFieldHierarchyLabel = new System.Windows.Forms.Label();
      sortDirectionHierarchyLabel = new System.Windows.Forms.Label();
      layoutHierarchyLabel = new System.Windows.Forms.Label();
      label68 = new System.Windows.Forms.Label();
      label70 = new System.Windows.Forms.Label();
      label69 = new System.Windows.Forms.Label();
      label66 = new System.Windows.Forms.Label();
      label65 = new System.Windows.Forms.Label();
      this.groupBox9.SuspendLayout();
      this.groupBox25.SuspendLayout();
      this.Fanart.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultFanart)).BeginInit();
      this.groupBox22.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultViewImage)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultCover)).BeginInit();
      this.groupBox_ArtistImages.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultPersonImage)).BeginInit();
      this.groupBox_AMCupdater.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAMCUscanStartDelay)).BeginInit();
      this.groupBoxAMCUpdaterConfigFile.SuspendLayout();
      this.groupBox_AMCupdaterScheduer.SuspendLayout();
      this.groupBox_AMCupdater_ExternalApplication.SuspendLayout();
      this.groupBoxDeletionOptions.SuspendLayout();
      this.gpspfield.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox_UserItemsDetails.SuspendLayout();
      this.groupBox_UserItemsMain.SuspendLayout();
      this.groupBox_Separators.SuspendLayout();
      this.groupBox_DefaultView.SuspendLayout();
      this.groupBox7.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.groupBoxView.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.viewBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mFview)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.AntViewsImage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorViews)).BeginInit();
      this.bindingNavigatorViews.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgViewsList)).BeginInit();
      this.groupBoxSortAndLayoutForView.SuspendLayout();
      this.groupBox5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.AntViewIndex)).BeginInit();
      this.groupBox24.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numUpDownTrailersInIntro)).BeginInit();
      this.groupBox_AntSelectedEnreg.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox_PreLaunchingCommand.SuspendLayout();
      this.groupBox_Security.SuspendLayout();
      this.groupBox_TitleOrder.SuspendLayout();
      this.groupBoxExtendedFieldHandling.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SPicture)).BeginInit();
      this.groupBox_DVDprofiler.SuspendLayout();
      this.groupBox13.SuspendLayout();
      this.groupBox_GrabberOptions.SuspendLayout();
      this.groupBox6.SuspendLayout();
      this.groupBox_UpdateGrabberScripts.SuspendLayout();
      this.groupBoxMultiUserState.SuspendLayout();
      this.Tab_Trakt.SuspendLayout();
      this.groupBoxExternal.SuspendLayout();
      this.groupBoxTrakt.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.Tab_Display.SuspendLayout();
      this.Tab_Views.SuspendLayout();
      this.Tab_Trailer.SuspendLayout();
      this.Tab_General.SuspendLayout();
      this.groupBox_PlayMovieInfos.SuspendLayout();
      this.groupBoxMoviePathInfos.SuspendLayout();
      this.Tab_ExternalCatalogs.SuspendLayout();
      this.groupBoxAMCsettings.SuspendLayout();
      this.Tab_Network.SuspendLayout();
      this.Tab_Logos.SuspendLayout();
      this.Tab_Artwork.SuspendLayout();
      this.Tab_AMCupdater.SuspendLayout();
      this.Tab_Update.SuspendLayout();
      this.General.SuspendLayout();
      this.Tab_About.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTMDBlogo)).BeginInit();
      this.groupBoxSupportedCatalogs.SuspendLayout();
      this.groupBoxAbout.SuspendLayout();
      this.Tab_Other.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.personDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.personBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.antMovieCatalog)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.propertiesBindingSource)).BeginInit();
      this.Tab_OldStuff.SuspendLayout();
      this.groupBox8.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMyFilms)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.indexNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // ownerLabel
      // 
      ownerLabel.AutoSize = true;
      ownerLabel.Location = new System.Drawing.Point(379, 12);
      ownerLabel.Name = "ownerLabel";
      ownerLabel.Size = new System.Drawing.Size(41, 13);
      ownerLabel.TabIndex = 2;
      ownerLabel.Text = "Owner:";
      // 
      // mailLabel
      // 
      mailLabel.AutoSize = true;
      mailLabel.Location = new System.Drawing.Point(379, 38);
      mailLabel.Name = "mailLabel";
      mailLabel.Size = new System.Drawing.Size(29, 13);
      mailLabel.TabIndex = 4;
      mailLabel.Text = "Mail:";
      // 
      // siteLabel
      // 
      siteLabel.AutoSize = true;
      siteLabel.Location = new System.Drawing.Point(379, 64);
      siteLabel.Name = "siteLabel";
      siteLabel.Size = new System.Drawing.Size(28, 13);
      siteLabel.TabIndex = 6;
      siteLabel.Text = "Site:";
      // 
      // descriptionLabel
      // 
      descriptionLabel.AutoSize = true;
      descriptionLabel.Location = new System.Drawing.Point(379, 90);
      descriptionLabel.Name = "descriptionLabel";
      descriptionLabel.Size = new System.Drawing.Size(38, 13);
      descriptionLabel.TabIndex = 8;
      descriptionLabel.Text = "Descr.";
      // 
      // indexLabel1
      // 
      indexLabel1.AutoSize = true;
      indexLabel1.Location = new System.Drawing.Point(178, 80);
      indexLabel1.Name = "indexLabel1";
      indexLabel1.Size = new System.Drawing.Size(36, 13);
      indexLabel1.TabIndex = 34;
      indexLabel1.Text = "Index:";
      // 
      // labelLabel
      // 
      labelLabel.AutoSize = true;
      labelLabel.Location = new System.Drawing.Point(8, 53);
      labelLabel.Name = "labelLabel";
      labelLabel.Size = new System.Drawing.Size(36, 13);
      labelLabel.TabIndex = 3;
      labelLabel.Text = "Label:";
      // 
      // dBfieldLabel
      // 
      dBfieldLabel.AutoSize = true;
      dBfieldLabel.Location = new System.Drawing.Point(8, 79);
      dBfieldLabel.Name = "dBfieldLabel";
      dBfieldLabel.Size = new System.Drawing.Size(44, 13);
      dBfieldLabel.TabIndex = 5;
      dBfieldLabel.Text = "DBfield:";
      // 
      // valueLabel
      // 
      valueLabel.AutoSize = true;
      valueLabel.Location = new System.Drawing.Point(265, 80);
      valueLabel.Name = "valueLabel";
      valueLabel.Size = new System.Drawing.Size(37, 13);
      valueLabel.TabIndex = 7;
      valueLabel.Text = "Value:";
      // 
      // filterLabel
      // 
      filterLabel.AutoSize = true;
      filterLabel.Location = new System.Drawing.Point(8, 106);
      filterLabel.Name = "filterLabel";
      filterLabel.Size = new System.Drawing.Size(32, 13);
      filterLabel.TabIndex = 9;
      filterLabel.Text = "Filter:";
      // 
      // sortFieldHierarchyLabel
      // 
      sortFieldHierarchyLabel.AutoSize = true;
      sortFieldHierarchyLabel.Location = new System.Drawing.Point(62, 140);
      sortFieldHierarchyLabel.Name = "sortFieldHierarchyLabel";
      sortFieldHierarchyLabel.Size = new System.Drawing.Size(51, 13);
      sortFieldHierarchyLabel.TabIndex = 23;
      sortFieldHierarchyLabel.Text = "Sort Field";
      // 
      // sortDirectionHierarchyLabel
      // 
      sortDirectionHierarchyLabel.AutoSize = true;
      sortDirectionHierarchyLabel.Location = new System.Drawing.Point(175, 140);
      sortDirectionHierarchyLabel.Name = "sortDirectionHierarchyLabel";
      sortDirectionHierarchyLabel.Size = new System.Drawing.Size(71, 13);
      sortDirectionHierarchyLabel.TabIndex = 25;
      sortDirectionHierarchyLabel.Text = "Sort Direction";
      // 
      // layoutHierarchyLabel
      // 
      layoutHierarchyLabel.AutoSize = true;
      layoutHierarchyLabel.Location = new System.Drawing.Point(273, 140);
      layoutHierarchyLabel.Name = "layoutHierarchyLabel";
      layoutHierarchyLabel.Size = new System.Drawing.Size(39, 13);
      layoutHierarchyLabel.TabIndex = 27;
      layoutHierarchyLabel.Text = "Layout";
      // 
      // label68
      // 
      label68.AutoSize = true;
      label68.Location = new System.Drawing.Point(16, 16);
      label68.Name = "label68";
      label68.Size = new System.Drawing.Size(53, 13);
      label68.TabIndex = 0;
      label68.Text = "Sort Type";
      this.ToolTip1.SetToolTip(label68, "In Views, you can either sort by Nyme or Count.\r\nBy Name will sort by the DB fiel" +
        "d selected.");
      // 
      // label70
      // 
      label70.AutoSize = true;
      label70.Location = new System.Drawing.Point(202, 16);
      label70.Name = "label70";
      label70.Size = new System.Drawing.Size(39, 13);
      label70.TabIndex = 3;
      label70.Text = "Layout";
      // 
      // label69
      // 
      label69.AutoSize = true;
      label69.Location = new System.Drawing.Point(125, 16);
      label69.Name = "label69";
      label69.Size = new System.Drawing.Size(26, 13);
      label69.TabIndex = 1;
      label69.Text = "Sort";
      this.ToolTip1.SetToolTip(label69, resources.GetString("label69.ToolTip"));
      // 
      // label66
      // 
      label66.AutoSize = true;
      label66.Location = new System.Drawing.Point(183, 104);
      label66.Name = "label66";
      label66.Size = new System.Drawing.Size(34, 13);
      label66.TabIndex = 8;
      label66.Text = "Value";
      this.ToolTip1.SetToolTip(label66, resources.GetString("label66.ToolTip"));
      // 
      // label65
      // 
      label65.AutoSize = true;
      label65.Location = new System.Drawing.Point(183, 65);
      label65.Name = "label65";
      label65.Size = new System.Drawing.Size(47, 13);
      label65.TabIndex = 4;
      label65.Text = "DB Field";
      this.ToolTip1.SetToolTip(label65, "DB Item to view is setting the information that will be shoen, if you select this" +
        " view.");
      // 
      // ToolTip1
      // 
      this.ToolTip1.AutoPopDelay = 15000;
      this.ToolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.ToolTip1.InitialDelay = 500;
      this.ToolTip1.IsBalloon = true;
      this.ToolTip1.ReshowDelay = 100;
      this.ToolTip1.ToolTipTitle = "MyFilms Help ...";
      // 
      // Config_Name
      // 
      this.Config_Name.BackColor = System.Drawing.SystemColors.Info;
      this.Config_Name.FormattingEnabled = true;
      this.Config_Name.Location = new System.Drawing.Point(189, 42);
      this.Config_Name.Name = "Config_Name";
      this.Config_Name.Size = new System.Drawing.Size(172, 21);
      this.Config_Name.Sorted = true;
      this.Config_Name.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.Config_Name, resources.GetString("Config_Name.ToolTip"));
      this.Config_Name.SelectedIndexChanged += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
      this.Config_Name.TextChanged += new System.EventHandler(this.Config_Name_TextChanged);
      this.Config_Name.Leave += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
      // 
      // textBoxPluginName
      // 
      this.textBoxPluginName.Location = new System.Drawing.Point(189, 14);
      this.textBoxPluginName.Name = "textBoxPluginName";
      this.textBoxPluginName.Size = new System.Drawing.Size(172, 20);
      this.textBoxPluginName.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.textBoxPluginName, "Name of the plugin displayed in MP.\r\nBy default Films, but you can choose a bette" +
        "r name");
      // 
      // ButQuit
      // 
      this.ButQuit.Location = new System.Drawing.Point(692, 466);
      this.ButQuit.Name = "ButQuit";
      this.ButQuit.Size = new System.Drawing.Size(64, 31);
      this.ButQuit.TabIndex = 19;
      this.ButQuit.Text = "Quit";
      this.ToolTip1.SetToolTip(this.ButQuit, "No comments...");
      this.ButQuit.UseVisualStyleBackColor = true;
      this.ButQuit.Click += new System.EventHandler(this.ButQuit_Click);
      // 
      // ButDelet
      // 
      this.ButDelet.Location = new System.Drawing.Point(622, 466);
      this.ButDelet.Name = "ButDelet";
      this.ButDelet.Size = new System.Drawing.Size(64, 31);
      this.ButDelet.TabIndex = 18;
      this.ButDelet.Text = "Delete";
      this.ToolTip1.SetToolTip(this.ButDelet, "Delete all information for that configuration.");
      this.ButDelet.UseVisualStyleBackColor = true;
      this.ButDelet.Click += new System.EventHandler(this.ButDelet_Click);
      // 
      // Config_Dflt
      // 
      this.Config_Dflt.AutoSize = true;
      this.Config_Dflt.Location = new System.Drawing.Point(481, 17);
      this.Config_Dflt.Name = "Config_Dflt";
      this.Config_Dflt.Size = new System.Drawing.Size(93, 17);
      this.Config_Dflt.TabIndex = 7;
      this.Config_Dflt.Text = "Default Config";
      this.ToolTip1.SetToolTip(this.Config_Dflt, resources.GetString("Config_Dflt.ToolTip"));
      this.Config_Dflt.UseVisualStyleBackColor = true;
      // 
      // Config_Menu
      // 
      this.Config_Menu.AutoSize = true;
      this.Config_Menu.Location = new System.Drawing.Point(481, 45);
      this.Config_Menu.Name = "Config_Menu";
      this.Config_Menu.Size = new System.Drawing.Size(159, 17);
      this.Config_Menu.TabIndex = 8;
      this.Config_Menu.Text = "Always Display Config Menu";
      this.ToolTip1.SetToolTip(this.Config_Menu, resources.GetString("Config_Menu.ToolTip"));
      this.Config_Menu.UseVisualStyleBackColor = true;
      // 
      // label_VersionNumber
      // 
      this.label_VersionNumber.AutoSize = true;
      this.label_VersionNumber.ForeColor = System.Drawing.SystemColors.HotTrack;
      this.label_VersionNumber.Location = new System.Drawing.Point(652, 18);
      this.label_VersionNumber.Name = "label_VersionNumber";
      this.label_VersionNumber.Size = new System.Drawing.Size(89, 13);
      this.label_VersionNumber.TabIndex = 9;
      this.label_VersionNumber.Text = "Version x.x.x.xxxx";
      this.ToolTip1.SetToolTip(this.label_VersionNumber, "MyFilms Version Number");
      // 
      // ButCopy
      // 
      this.ButCopy.Location = new System.Drawing.Point(552, 466);
      this.ButCopy.Name = "ButCopy";
      this.ButCopy.Size = new System.Drawing.Size(64, 31);
      this.ButCopy.TabIndex = 17;
      this.ButCopy.Text = "Copy";
      this.ToolTip1.SetToolTip(this.ButCopy, "Creates a copy of the current selected configuration.\r\nCan be used e.g. if you wa" +
        "nt to have more than one \r\nconfiguration available with different settings or fi" +
        "lters, \r\nbut based on same movie DB.");
      this.ButCopy.UseVisualStyleBackColor = true;
      this.ButCopy.Click += new System.EventHandler(this.ButCopy_Click);
      // 
      // cbAllowRecentAddedAPI
      // 
      this.cbAllowRecentAddedAPI.AutoSize = true;
      this.cbAllowRecentAddedAPI.Location = new System.Drawing.Point(15, 21);
      this.cbAllowRecentAddedAPI.Name = "cbAllowRecentAddedAPI";
      this.cbAllowRecentAddedAPI.Size = new System.Drawing.Size(179, 17);
      this.cbAllowRecentAddedAPI.TabIndex = 0;
      this.cbAllowRecentAddedAPI.Text = "Enable \"Recently added Media\"";
      this.ToolTip1.SetToolTip(this.cbAllowRecentAddedAPI, "If checked, this config\'s media will be included when external plugins \r\nlike \"re" +
        "cently added media handler\" ask for \"recenty added\" movies.\r\n\r\nYou can enable th" +
        "is for multiple configs.");
      this.cbAllowRecentAddedAPI.UseVisualStyleBackColor = true;
      // 
      // cbAllowTraktSync
      // 
      this.cbAllowTraktSync.AutoSize = true;
      this.cbAllowTraktSync.Location = new System.Drawing.Point(15, 22);
      this.cbAllowTraktSync.Name = "cbAllowTraktSync";
      this.cbAllowTraktSync.Size = new System.Drawing.Size(242, 17);
      this.cbAllowTraktSync.TabIndex = 0;
      this.cbAllowTraktSync.Text = "Enable TRAKT Synchronisation for this config";
      this.ToolTip1.SetToolTip(this.cbAllowTraktSync, resources.GetString("cbAllowTraktSync.ToolTip"));
      this.cbAllowTraktSync.UseVisualStyleBackColor = true;
      // 
      // groupBox9
      // 
      this.groupBox9.Controls.Add(this.chkDVDprofilerMergeWithGenreField);
      this.groupBox9.Location = new System.Drawing.Point(13, 86);
      this.groupBox9.Name = "groupBox9";
      this.groupBox9.Size = new System.Drawing.Size(307, 59);
      this.groupBox9.TabIndex = 1;
      this.groupBox9.TabStop = false;
      this.groupBox9.Text = "Tags ...";
      this.ToolTip1.SetToolTip(this.groupBox9, "To merge the DVD Profiler Tags field data with the My Films category field, enabl" +
        "e this option. \r\nAlternatively you may use the general External Catalogs options" +
        " to add Tags to another field.");
      // 
      // chkDVDprofilerMergeWithGenreField
      // 
      this.chkDVDprofilerMergeWithGenreField.AutoSize = true;
      this.chkDVDprofilerMergeWithGenreField.Location = new System.Drawing.Point(23, 24);
      this.chkDVDprofilerMergeWithGenreField.Name = "chkDVDprofilerMergeWithGenreField";
      this.chkDVDprofilerMergeWithGenreField.Size = new System.Drawing.Size(151, 17);
      this.chkDVDprofilerMergeWithGenreField.TabIndex = 0;
      this.chkDVDprofilerMergeWithGenreField.Text = "Merge With Category Field";
      this.chkDVDprofilerMergeWithGenreField.UseVisualStyleBackColor = true;
      // 
      // groupBox25
      // 
      this.groupBox25.Controls.Add(this.label37);
      this.groupBox25.Controls.Add(this.comboWOLtimeout);
      this.groupBox25.Controls.Add(this.buttonSendMagicPacket3);
      this.groupBox25.Controls.Add(this.buttonSendMagicPacket2);
      this.groupBox25.Controls.Add(this.label30);
      this.groupBox25.Controls.Add(this.buttonSendMagicPacket1);
      this.groupBox25.Controls.Add(this.buttonGetMACadresses);
      this.groupBox25.Controls.Add(this.label_NAS_Server_3_MAC);
      this.groupBox25.Controls.Add(this.NAS_MAC_3);
      this.groupBox25.Controls.Add(this.label_NAS_Server_3_Name);
      this.groupBox25.Controls.Add(this.NAS_Name_3);
      this.groupBox25.Controls.Add(this.label_NAS_Server_2_MAC);
      this.groupBox25.Controls.Add(this.NAS_MAC_2);
      this.groupBox25.Controls.Add(this.label_NAS_Server_2_Name);
      this.groupBox25.Controls.Add(this.NAS_Name_2);
      this.groupBox25.Controls.Add(this.label_NAS_Server_1_MAC);
      this.groupBox25.Controls.Add(this.NAS_MAC_1);
      this.groupBox25.Controls.Add(this.label_NAS_Server_1_Name);
      this.groupBox25.Controls.Add(this.check_WOL_Userdialog);
      this.groupBox25.Controls.Add(this.check_WOL_enable);
      this.groupBox25.Controls.Add(this.NAS_Name_1);
      this.groupBox25.Controls.Add(this.label36);
      this.groupBox25.Location = new System.Drawing.Point(22, 50);
      this.groupBox25.Name = "groupBox25";
      this.groupBox25.Size = new System.Drawing.Size(705, 234);
      this.groupBox25.TabIndex = 0;
      this.groupBox25.TabStop = false;
      this.groupBox25.Text = "WakeOnLAN Options to Support NAS Storage";
      this.ToolTip1.SetToolTip(this.groupBox25, "If enabled you can start NAS when launching movies");
      // 
      // label37
      // 
      this.label37.AutoSize = true;
      this.label37.Location = new System.Drawing.Point(414, 60);
      this.label37.Name = "label37";
      this.label37.Size = new System.Drawing.Size(165, 13);
      this.label37.TabIndex = 2;
      this.label37.Text = "Timeout for NAS Server WakeUp";
      // 
      // comboWOLtimeout
      // 
      this.comboWOLtimeout.FormattingEnabled = true;
      this.comboWOLtimeout.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "20",
            "30",
            "60"});
      this.comboWOLtimeout.Location = new System.Drawing.Point(603, 57);
      this.comboWOLtimeout.Name = "comboWOLtimeout";
      this.comboWOLtimeout.Size = new System.Drawing.Size(81, 21);
      this.comboWOLtimeout.TabIndex = 3;
      // 
      // buttonSendMagicPacket3
      // 
      this.buttonSendMagicPacket3.Location = new System.Drawing.Point(603, 200);
      this.buttonSendMagicPacket3.Name = "buttonSendMagicPacket3";
      this.buttonSendMagicPacket3.Size = new System.Drawing.Size(81, 23);
      this.buttonSendMagicPacket3.TabIndex = 0;
      this.buttonSendMagicPacket3.Text = "Start Server 3";
      this.buttonSendMagicPacket3.UseVisualStyleBackColor = true;
      this.buttonSendMagicPacket3.Click += new System.EventHandler(this.buttonSendMagicPacket3_Click);
      // 
      // buttonSendMagicPacket2
      // 
      this.buttonSendMagicPacket2.Location = new System.Drawing.Point(603, 174);
      this.buttonSendMagicPacket2.Name = "buttonSendMagicPacket2";
      this.buttonSendMagicPacket2.Size = new System.Drawing.Size(81, 23);
      this.buttonSendMagicPacket2.TabIndex = 17;
      this.buttonSendMagicPacket2.Text = "Start Server 2";
      this.buttonSendMagicPacket2.UseVisualStyleBackColor = true;
      this.buttonSendMagicPacket2.Click += new System.EventHandler(this.buttonSendMagicPacket2_Click);
      // 
      // label30
      // 
      this.label30.AutoSize = true;
      this.label30.Location = new System.Drawing.Point(414, 94);
      this.label30.Name = "label30";
      this.label30.Size = new System.Drawing.Size(272, 13);
      this.label30.TabIndex = 5;
      this.label30.Text = "Auto-MAC-discover only works if NAS Storage is running";
      // 
      // buttonSendMagicPacket1
      // 
      this.buttonSendMagicPacket1.Location = new System.Drawing.Point(603, 150);
      this.buttonSendMagicPacket1.Name = "buttonSendMagicPacket1";
      this.buttonSendMagicPacket1.Size = new System.Drawing.Size(81, 23);
      this.buttonSendMagicPacket1.TabIndex = 12;
      this.buttonSendMagicPacket1.Text = "Start Server 1";
      this.buttonSendMagicPacket1.UseVisualStyleBackColor = true;
      this.buttonSendMagicPacket1.Click += new System.EventHandler(this.buttonSendMagicPacket1_Click);
      // 
      // buttonGetMACadresses
      // 
      this.buttonGetMACadresses.Location = new System.Drawing.Point(417, 110);
      this.buttonGetMACadresses.Name = "buttonGetMACadresses";
      this.buttonGetMACadresses.Size = new System.Drawing.Size(160, 28);
      this.buttonGetMACadresses.TabIndex = 6;
      this.buttonGetMACadresses.Text = "Try getting MAC addresses";
      this.buttonGetMACadresses.UseVisualStyleBackColor = true;
      this.buttonGetMACadresses.Click += new System.EventHandler(this.buttonGetMACadresses_Click);
      // 
      // label_NAS_Server_3_MAC
      // 
      this.label_NAS_Server_3_MAC.AutoSize = true;
      this.label_NAS_Server_3_MAC.Location = new System.Drawing.Point(292, 205);
      this.label_NAS_Server_3_MAC.Name = "label_NAS_Server_3_MAC";
      this.label_NAS_Server_3_MAC.Size = new System.Drawing.Size(119, 13);
      this.label_NAS_Server_3_MAC.TabIndex = 20;
      this.label_NAS_Server_3_MAC.Text = "Server 3 - MAC address";
      // 
      // NAS_MAC_3
      // 
      this.NAS_MAC_3.Enabled = false;
      this.NAS_MAC_3.Location = new System.Drawing.Point(417, 202);
      this.NAS_MAC_3.Name = "NAS_MAC_3";
      this.NAS_MAC_3.Size = new System.Drawing.Size(160, 20);
      this.NAS_MAC_3.TabIndex = 21;
      // 
      // label_NAS_Server_3_Name
      // 
      this.label_NAS_Server_3_Name.AutoSize = true;
      this.label_NAS_Server_3_Name.Location = new System.Drawing.Point(24, 205);
      this.label_NAS_Server_3_Name.Name = "label_NAS_Server_3_Name";
      this.label_NAS_Server_3_Name.Size = new System.Drawing.Size(84, 13);
      this.label_NAS_Server_3_Name.TabIndex = 18;
      this.label_NAS_Server_3_Name.Text = "Server 3 - Name";
      // 
      // NAS_Name_3
      // 
      this.NAS_Name_3.Enabled = false;
      this.NAS_Name_3.Location = new System.Drawing.Point(114, 202);
      this.NAS_Name_3.Name = "NAS_Name_3";
      this.NAS_Name_3.Size = new System.Drawing.Size(160, 20);
      this.NAS_Name_3.TabIndex = 19;
      this.NAS_Name_3.TextChanged += new System.EventHandler(this.NAS_Name_3_TextChanged);
      // 
      // label_NAS_Server_2_MAC
      // 
      this.label_NAS_Server_2_MAC.AutoSize = true;
      this.label_NAS_Server_2_MAC.Location = new System.Drawing.Point(292, 179);
      this.label_NAS_Server_2_MAC.Name = "label_NAS_Server_2_MAC";
      this.label_NAS_Server_2_MAC.Size = new System.Drawing.Size(119, 13);
      this.label_NAS_Server_2_MAC.TabIndex = 15;
      this.label_NAS_Server_2_MAC.Text = "Server 2 - MAC address";
      // 
      // NAS_MAC_2
      // 
      this.NAS_MAC_2.Enabled = false;
      this.NAS_MAC_2.Location = new System.Drawing.Point(417, 176);
      this.NAS_MAC_2.Name = "NAS_MAC_2";
      this.NAS_MAC_2.Size = new System.Drawing.Size(160, 20);
      this.NAS_MAC_2.TabIndex = 16;
      // 
      // label_NAS_Server_2_Name
      // 
      this.label_NAS_Server_2_Name.AutoSize = true;
      this.label_NAS_Server_2_Name.Location = new System.Drawing.Point(24, 179);
      this.label_NAS_Server_2_Name.Name = "label_NAS_Server_2_Name";
      this.label_NAS_Server_2_Name.Size = new System.Drawing.Size(84, 13);
      this.label_NAS_Server_2_Name.TabIndex = 13;
      this.label_NAS_Server_2_Name.Text = "Server 2 - Name";
      // 
      // NAS_Name_2
      // 
      this.NAS_Name_2.Enabled = false;
      this.NAS_Name_2.Location = new System.Drawing.Point(114, 176);
      this.NAS_Name_2.Name = "NAS_Name_2";
      this.NAS_Name_2.Size = new System.Drawing.Size(160, 20);
      this.NAS_Name_2.TabIndex = 14;
      this.NAS_Name_2.TextChanged += new System.EventHandler(this.NAS_Name_2_TextChanged);
      // 
      // label_NAS_Server_1_MAC
      // 
      this.label_NAS_Server_1_MAC.AutoSize = true;
      this.label_NAS_Server_1_MAC.Location = new System.Drawing.Point(292, 153);
      this.label_NAS_Server_1_MAC.Name = "label_NAS_Server_1_MAC";
      this.label_NAS_Server_1_MAC.Size = new System.Drawing.Size(119, 13);
      this.label_NAS_Server_1_MAC.TabIndex = 10;
      this.label_NAS_Server_1_MAC.Text = "Server 1 - MAC address";
      // 
      // NAS_MAC_1
      // 
      this.NAS_MAC_1.Enabled = false;
      this.NAS_MAC_1.Location = new System.Drawing.Point(417, 150);
      this.NAS_MAC_1.Name = "NAS_MAC_1";
      this.NAS_MAC_1.Size = new System.Drawing.Size(160, 20);
      this.NAS_MAC_1.TabIndex = 11;
      // 
      // label_NAS_Server_1_Name
      // 
      this.label_NAS_Server_1_Name.AutoSize = true;
      this.label_NAS_Server_1_Name.Location = new System.Drawing.Point(24, 153);
      this.label_NAS_Server_1_Name.Name = "label_NAS_Server_1_Name";
      this.label_NAS_Server_1_Name.Size = new System.Drawing.Size(84, 13);
      this.label_NAS_Server_1_Name.TabIndex = 8;
      this.label_NAS_Server_1_Name.Text = "Server 1 - Name";
      // 
      // check_WOL_Userdialog
      // 
      this.check_WOL_Userdialog.AutoSize = true;
      this.check_WOL_Userdialog.Enabled = false;
      this.check_WOL_Userdialog.Location = new System.Drawing.Point(27, 59);
      this.check_WOL_Userdialog.Name = "check_WOL_Userdialog";
      this.check_WOL_Userdialog.Size = new System.Drawing.Size(346, 17);
      this.check_WOL_Userdialog.TabIndex = 1;
      this.check_WOL_Userdialog.Text = "Enable WakeOnLAN User Dialog (Autoinvoke WOL when disabled)";
      this.ToolTip1.SetToolTip(this.check_WOL_Userdialog, "Enable WakeOnLAN User Dialog when launching a movie");
      this.check_WOL_Userdialog.UseVisualStyleBackColor = true;
      // 
      // check_WOL_enable
      // 
      this.check_WOL_enable.AutoSize = true;
      this.check_WOL_enable.Location = new System.Drawing.Point(27, 36);
      this.check_WOL_enable.Name = "check_WOL_enable";
      this.check_WOL_enable.Size = new System.Drawing.Size(126, 17);
      this.check_WOL_enable.TabIndex = 0;
      this.check_WOL_enable.Text = "Enable WakeOnLAN";
      this.ToolTip1.SetToolTip(this.check_WOL_enable, "Select this option if you want MyFilms to automatically \r\nsend WOL magic packets " +
        "to start NAS server from S3.\r\n");
      this.check_WOL_enable.UseVisualStyleBackColor = true;
      this.check_WOL_enable.CheckedChanged += new System.EventHandler(this.check_WOL_enable_CheckedChanged);
      // 
      // NAS_Name_1
      // 
      this.NAS_Name_1.Enabled = false;
      this.NAS_Name_1.Location = new System.Drawing.Point(114, 150);
      this.NAS_Name_1.Name = "NAS_Name_1";
      this.NAS_Name_1.Size = new System.Drawing.Size(160, 20);
      this.NAS_Name_1.TabIndex = 9;
      this.NAS_Name_1.TextChanged += new System.EventHandler(this.NAS_1_Name_TextChanged);
      // 
      // label36
      // 
      this.label36.AutoSize = true;
      this.label36.Location = new System.Drawing.Point(24, 118);
      this.label36.Name = "label36";
      this.label36.Size = new System.Drawing.Size(235, 13);
      this.label36.TabIndex = 7;
      this.label36.Text = "Supported Servers (make sure, it supports WOL)";
      this.ToolTip1.SetToolTip(this.label36, "MyFilms will autodetect server name from file path, \r\nif UNC notation is used and" +
        " server is defined here.");
      // 
      // SLogo_Type
      // 
      this.SLogo_Type.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
      this.SLogo_Type.Enabled = false;
      this.SLogo_Type.FormattingEnabled = true;
      this.SLogo_Type.Items.AddRange(new object[] {
            "ID2001",
            "ID2002",
            "ID2003"});
      this.SLogo_Type.Location = new System.Drawing.Point(9, 289);
      this.SLogo_Type.Name = "SLogo_Type";
      this.SLogo_Type.Size = new System.Drawing.Size(102, 21);
      this.SLogo_Type.TabIndex = 16;
      this.ToolTip1.SetToolTip(this.SLogo_Type, resources.GetString("SLogo_Type.ToolTip"));
      // 
      // chkLogos
      // 
      this.chkLogos.AutoSize = true;
      this.chkLogos.Location = new System.Drawing.Point(9, 10);
      this.chkLogos.Name = "chkLogos";
      this.chkLogos.Size = new System.Drawing.Size(91, 17);
      this.chkLogos.TabIndex = 0;
      this.chkLogos.Text = "Enable Logos";
      this.ToolTip1.SetToolTip(this.chkLogos, resources.GetString("chkLogos.ToolTip"));
      this.chkLogos.UseVisualStyleBackColor = true;
      this.chkLogos.CheckedChanged += new System.EventHandler(this.chkLogos_CheckedChanged);
      // 
      // LogoView
      // 
      this.LogoView.AllowColumnReorder = true;
      this.LogoView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LogoType,
            this.Field1,
            this.Op1,
            this.Value1,
            this.And_Or,
            this.Field2,
            this.Op2,
            this.Value2,
            this.Image,
            this.PathImage});
      this.LogoView.Enabled = false;
      this.LogoView.FullRowSelect = true;
      this.LogoView.GridLines = true;
      this.LogoView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.LogoView.Location = new System.Drawing.Point(7, 59);
      this.LogoView.MultiSelect = false;
      this.LogoView.Name = "LogoView";
      this.LogoView.Size = new System.Drawing.Size(734, 170);
      this.LogoView.TabIndex = 9;
      this.ToolTip1.SetToolTip(this.LogoView, resources.GetString("LogoView.ToolTip"));
      this.LogoView.UseCompatibleStateImageBehavior = false;
      this.LogoView.View = System.Windows.Forms.View.Details;
      this.LogoView.SelectedIndexChanged += new System.EventHandler(this.LogoView_SelectedIndexChanged);
      // 
      // LogoType
      // 
      this.LogoType.Text = "Type Logo";
      this.LogoType.Width = 66;
      // 
      // Field1
      // 
      this.Field1.Text = "Field";
      this.Field1.Width = 56;
      // 
      // Op1
      // 
      this.Op1.Text = "Operator";
      this.Op1.Width = 58;
      // 
      // Value1
      // 
      this.Value1.Text = "Value";
      this.Value1.Width = 73;
      // 
      // And_Or
      // 
      this.And_Or.Text = "And/Or";
      this.And_Or.Width = 51;
      // 
      // Field2
      // 
      this.Field2.Text = "Field";
      // 
      // Op2
      // 
      this.Op2.Text = "Operator";
      this.Op2.Width = 58;
      // 
      // Value2
      // 
      this.Value2.Text = "Value";
      this.Value2.Width = 68;
      // 
      // Image
      // 
      this.Image.Text = "Picture";
      this.Image.Width = 83;
      // 
      // PathImage
      // 
      this.PathImage.Text = "Image Path (autosearch)";
      this.PathImage.Width = 155;
      // 
      // txtLogosPath
      // 
      this.txtLogosPath.Enabled = false;
      this.txtLogosPath.Location = new System.Drawing.Point(220, 33);
      this.txtLogosPath.Name = "txtLogosPath";
      this.txtLogosPath.Size = new System.Drawing.Size(469, 20);
      this.txtLogosPath.TabIndex = 7;
      this.ToolTip1.SetToolTip(this.txtLogosPath, resources.GetString("txtLogosPath.ToolTip"));
      this.txtLogosPath.TextChanged += new System.EventHandler(this.txtLogosPath_TextChanged);
      // 
      // comboBoxLogoSpacing
      // 
      this.comboBoxLogoSpacing.FormattingEnabled = true;
      this.comboBoxLogoSpacing.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "25"});
      this.comboBoxLogoSpacing.Location = new System.Drawing.Point(644, 8);
      this.comboBoxLogoSpacing.Name = "comboBoxLogoSpacing";
      this.comboBoxLogoSpacing.Size = new System.Drawing.Size(45, 21);
      this.comboBoxLogoSpacing.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.comboBoxLogoSpacing, "You can choose the distance between the generated logos in \"pixel\".\r\nDefault is \"" +
        "1\".");
      // 
      // comboBoxLogoPresets
      // 
      this.comboBoxLogoPresets.FormattingEnabled = true;
      this.comboBoxLogoPresets.Items.AddRange(new object[] {
            "Use Logos of currently selected skin",
            "Use MyFilms Logo Pack",
            "Define your path to logo image files"});
      this.comboBoxLogoPresets.Location = new System.Drawing.Point(220, 8);
      this.comboBoxLogoPresets.Name = "comboBoxLogoPresets";
      this.comboBoxLogoPresets.Size = new System.Drawing.Size(211, 21);
      this.comboBoxLogoPresets.TabIndex = 2;
      this.ToolTip1.SetToolTip(this.comboBoxLogoPresets, resources.GetString("comboBoxLogoPresets.ToolTip"));
      this.comboBoxLogoPresets.SelectedIndexChanged += new System.EventHandler(this.comboBoxLogoPresets_SelectedIndexChanged);
      // 
      // btnLogoClearCache
      // 
      this.btnLogoClearCache.Location = new System.Drawing.Point(461, 8);
      this.btnLogoClearCache.Name = "btnLogoClearCache";
      this.btnLogoClearCache.Size = new System.Drawing.Size(116, 21);
      this.btnLogoClearCache.TabIndex = 3;
      this.btnLogoClearCache.Text = "Clear Logo Cache";
      this.ToolTip1.SetToolTip(this.btnLogoClearCache, "If you have redefined logo parameters like spacing, you should clear the logo cac" +
        "he,\r\nso the logos will be rebuild with the new parameters");
      this.btnLogoClearCache.UseVisualStyleBackColor = true;
      this.btnLogoClearCache.Click += new System.EventHandler(this.btnLogoClearCache_Click);
      // 
      // Fanart
      // 
      this.Fanart.Controls.Add(this.buttonDefaultFanartReset);
      this.Fanart.Controls.Add(this.pictureBoxDefaultFanart);
      this.Fanart.Controls.Add(this.chkFanartDefaultViewsUseRandom);
      this.Fanart.Controls.Add(this.chkFanartDefaultViews);
      this.Fanart.Controls.Add(this.chkDfltFanartImageAll);
      this.Fanart.Controls.Add(this.label48);
      this.Fanart.Controls.Add(this.chkDfltFanartImage);
      this.Fanart.Controls.Add(this.lblResultingGroupViewsPathFanart);
      this.Fanart.Controls.Add(this.label40);
      this.Fanart.Controls.Add(this.chkFanart);
      this.Fanart.Controls.Add(this.btnFanart);
      this.Fanart.Controls.Add(this.MesFilmsFanart);
      this.Fanart.Controls.Add(this.labelFanart);
      this.Fanart.Controls.Add(this.chkDfltFanart);
      this.Fanart.Location = new System.Drawing.Point(6, 230);
      this.Fanart.Name = "Fanart";
      this.Fanart.Size = new System.Drawing.Size(735, 121);
      this.Fanart.TabIndex = 3;
      this.Fanart.TabStop = false;
      this.Fanart.Text = "Fanart ...";
      this.ToolTip1.SetToolTip(this.Fanart, resources.GetString("Fanart.ToolTip"));
      // 
      // buttonDefaultFanartReset
      // 
      this.buttonDefaultFanartReset.Location = new System.Drawing.Point(711, 11);
      this.buttonDefaultFanartReset.Name = "buttonDefaultFanartReset";
      this.buttonDefaultFanartReset.Size = new System.Drawing.Size(18, 18);
      this.buttonDefaultFanartReset.TabIndex = 9;
      this.buttonDefaultFanartReset.Text = "X";
      this.ToolTip1.SetToolTip(this.buttonDefaultFanartReset, "Click to reset image.\r\n(Image itself will not be deleted.)");
      this.buttonDefaultFanartReset.UseVisualStyleBackColor = true;
      // 
      // pictureBoxDefaultFanart
      // 
      this.pictureBoxDefaultFanart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBoxDefaultFanart.Location = new System.Drawing.Point(625, 47);
      this.pictureBoxDefaultFanart.Name = "pictureBoxDefaultFanart";
      this.pictureBoxDefaultFanart.Size = new System.Drawing.Size(90, 52);
      this.pictureBoxDefaultFanart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBoxDefaultFanart.TabIndex = 82;
      this.pictureBoxDefaultFanart.TabStop = false;
      this.ToolTip1.SetToolTip(this.pictureBoxDefaultFanart, "Click to select default fanart image.");
      this.pictureBoxDefaultFanart.Click += new System.EventHandler(this.pictureBoxDefaultFanart_Click);
      // 
      // chkFanartDefaultViewsUseRandom
      // 
      this.chkFanartDefaultViewsUseRandom.AutoSize = true;
      this.chkFanartDefaultViewsUseRandom.Location = new System.Drawing.Point(37, 36);
      this.chkFanartDefaultViewsUseRandom.Name = "chkFanartDefaultViewsUseRandom";
      this.chkFanartDefaultViewsUseRandom.Size = new System.Drawing.Size(116, 17);
      this.chkFanartDefaultViewsUseRandom.TabIndex = 1;
      this.chkFanartDefaultViewsUseRandom.Text = "Use random Fanart";
      this.ToolTip1.SetToolTip(this.chkFanartDefaultViewsUseRandom, "If selected, MyFilms will show random fanart from the movies contained in the sel" +
        "ected group.");
      this.chkFanartDefaultViewsUseRandom.UseVisualStyleBackColor = true;
      // 
      // chkFanartDefaultViews
      // 
      this.chkFanartDefaultViews.AutoSize = true;
      this.chkFanartDefaultViews.Location = new System.Drawing.Point(37, 99);
      this.chkFanartDefaultViews.Name = "chkFanartDefaultViews";
      this.chkFanartDefaultViews.Size = new System.Drawing.Size(161, 17);
      this.chkFanartDefaultViews.TabIndex = 4;
      this.chkFanartDefaultViews.Text = "Use Fanart for Default Views";
      this.ToolTip1.SetToolTip(this.chkFanartDefaultViews, resources.GetString("chkFanartDefaultViews.ToolTip"));
      this.chkFanartDefaultViews.UseVisualStyleBackColor = true;
      // 
      // chkDfltFanartImageAll
      // 
      this.chkDfltFanartImageAll.AutoSize = true;
      this.chkDfltFanartImageAll.Location = new System.Drawing.Point(229, 99);
      this.chkDfltFanartImageAll.Name = "chkDfltFanartImageAll";
      this.chkDfltFanartImageAll.Size = new System.Drawing.Size(104, 17);
      this.chkDfltFanartImageAll.TabIndex = 10;
      this.chkDfltFanartImageAll.Text = "Use for all Views";
      this.ToolTip1.SetToolTip(this.chkDfltFanartImageAll, "Use default fanart image for all group views like persons, date added, etc.");
      this.chkDfltFanartImageAll.UseVisualStyleBackColor = true;
      // 
      // label48
      // 
      this.label48.AutoSize = true;
      this.label48.Location = new System.Drawing.Point(622, 11);
      this.label48.Name = "label48";
      this.label48.Size = new System.Drawing.Size(41, 26);
      this.label48.TabIndex = 8;
      this.label48.Text = "Default\r\nFanart";
      // 
      // chkDfltFanartImage
      // 
      this.chkDfltFanartImage.AutoSize = true;
      this.chkDfltFanartImage.Location = new System.Drawing.Point(37, 57);
      this.chkDfltFanartImage.Name = "chkDfltFanartImage";
      this.chkDfltFanartImage.Size = new System.Drawing.Size(199, 17);
      this.chkDfltFanartImage.TabIndex = 2;
      this.chkDfltFanartImage.Text = "Use Default Image for missing Fanart";
      this.ToolTip1.SetToolTip(this.chkDfltFanartImage, "Use default fanart image if no fanart found for movies and group views like year," +
        " genre, country.");
      this.chkDfltFanartImage.UseVisualStyleBackColor = true;
      this.chkDfltFanartImage.CheckedChanged += new System.EventHandler(this.chkDfltFanartImage_CheckedChanged);
      // 
      // lblResultingGroupViewsPathFanart
      // 
      this.lblResultingGroupViewsPathFanart.AutoSize = true;
      this.lblResultingGroupViewsPathFanart.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.lblResultingGroupViewsPathFanart.Location = new System.Drawing.Point(452, 101);
      this.lblResultingGroupViewsPathFanart.Name = "lblResultingGroupViewsPathFanart";
      this.lblResultingGroupViewsPathFanart.Size = new System.Drawing.Size(105, 13);
      this.lblResultingGroupViewsPathFanart.TabIndex = 12;
      this.lblResultingGroupViewsPathFanart.Text = "<Views Fanart Path>";
      this.ToolTip1.SetToolTip(this.lblResultingGroupViewsPathFanart, "To use Fanart for \"Views\" put fanart in the subfolder for year, category, country" +
        ", \r\ne.g. <fanartfolder>\\_view\\year\\2010");
      // 
      // label40
      // 
      this.label40.AutoSize = true;
      this.label40.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.label40.Location = new System.Drawing.Point(352, 101);
      this.label40.Name = "label40";
      this.label40.Size = new System.Drawing.Size(93, 13);
      this.label40.TabIndex = 11;
      this.label40.Text = "Views Fanart Path";
      // 
      // chkFanart
      // 
      this.chkFanart.AutoSize = true;
      this.chkFanart.Location = new System.Drawing.Point(18, 15);
      this.chkFanart.Name = "chkFanart";
      this.chkFanart.Size = new System.Drawing.Size(92, 17);
      this.chkFanart.TabIndex = 0;
      this.chkFanart.Text = "Enable Fanart";
      this.chkFanart.UseVisualStyleBackColor = true;
      this.chkFanart.CheckedChanged += new System.EventHandler(this.chkFanart_CheckedChanged);
      // 
      // btnFanart
      // 
      this.btnFanart.Enabled = false;
      this.btnFanart.Location = new System.Drawing.Point(569, 15);
      this.btnFanart.Name = "btnFanart";
      this.btnFanart.Size = new System.Drawing.Size(32, 20);
      this.btnFanart.TabIndex = 7;
      this.btnFanart.Text = "...";
      this.btnFanart.UseVisualStyleBackColor = true;
      this.btnFanart.Click += new System.EventHandler(this.btnFanart_Click);
      // 
      // MesFilmsFanart
      // 
      this.MesFilmsFanart.Enabled = false;
      this.MesFilmsFanart.Location = new System.Drawing.Point(455, 15);
      this.MesFilmsFanart.Name = "MesFilmsFanart";
      this.MesFilmsFanart.Size = new System.Drawing.Size(108, 20);
      this.MesFilmsFanart.TabIndex = 6;
      // 
      // labelFanart
      // 
      this.labelFanart.AutoSize = true;
      this.labelFanart.Location = new System.Drawing.Point(383, 19);
      this.labelFanart.Name = "labelFanart";
      this.labelFanart.Size = new System.Drawing.Size(62, 13);
      this.labelFanart.TabIndex = 5;
      this.labelFanart.Text = "Fanart Path";
      // 
      // chkDfltFanart
      // 
      this.chkDfltFanart.AutoSize = true;
      this.chkDfltFanart.Enabled = false;
      this.chkDfltFanart.Location = new System.Drawing.Point(37, 78);
      this.chkDfltFanart.Name = "chkDfltFanart";
      this.chkDfltFanart.Size = new System.Drawing.Size(193, 17);
      this.chkDfltFanart.TabIndex = 3;
      this.chkDfltFanart.Text = "Use Movie Cover for missing Fanart";
      this.chkDfltFanart.UseVisualStyleBackColor = true;
      this.chkDfltFanart.CheckedChanged += new System.EventHandler(this.chkDfltFanart_CheckedChanged);
      // 
      // groupBox22
      // 
      this.groupBox22.Controls.Add(this.chkShowIndexedImgInIndViews);
      this.groupBox22.Controls.Add(this.buttonDefaultViewImageReset);
      this.groupBox22.Controls.Add(this.pictureBoxDefaultViewImage);
      this.groupBox22.Controls.Add(this.chkDfltViewsAll);
      this.groupBox22.Controls.Add(this.label47);
      this.groupBox22.Controls.Add(this.btnResetThumbs);
      this.groupBox22.Controls.Add(this.chkViews);
      this.groupBox22.Controls.Add(this.btnViews);
      this.groupBox22.Controls.Add(this.chkDfltViews);
      this.groupBox22.Controls.Add(this.label29);
      this.groupBox22.Controls.Add(this.MesFilmsViews);
      this.groupBox22.Location = new System.Drawing.Point(6, 78);
      this.groupBox22.Name = "groupBox22";
      this.groupBox22.Size = new System.Drawing.Size(735, 78);
      this.groupBox22.TabIndex = 1;
      this.groupBox22.TabStop = false;
      this.groupBox22.Text = "Views ...";
      this.ToolTip1.SetToolTip(this.groupBox22, resources.GetString("groupBox22.ToolTip"));
      // 
      // chkShowIndexedImgInIndViews
      // 
      this.chkShowIndexedImgInIndViews.AutoSize = true;
      this.chkShowIndexedImgInIndViews.Location = new System.Drawing.Point(37, 55);
      this.chkShowIndexedImgInIndViews.Name = "chkShowIndexedImgInIndViews";
      this.chkShowIndexedImgInIndViews.Size = new System.Drawing.Size(115, 17);
      this.chkShowIndexedImgInIndViews.TabIndex = 3;
      this.chkShowIndexedImgInIndViews.Text = "Use Index Thumbs";
      this.ToolTip1.SetToolTip(this.chkShowIndexedImgInIndViews, "If checked, MyFilms will show \"Index Thumbs\" (\"A\", \"B\", etc.) in all indexed view" +
        "s.\r\nMake sure, your skin does support them.");
      this.chkShowIndexedImgInIndViews.UseVisualStyleBackColor = true;
      // 
      // buttonDefaultViewImageReset
      // 
      this.buttonDefaultViewImageReset.Location = new System.Drawing.Point(711, 16);
      this.buttonDefaultViewImageReset.Name = "buttonDefaultViewImageReset";
      this.buttonDefaultViewImageReset.Size = new System.Drawing.Size(18, 18);
      this.buttonDefaultViewImageReset.TabIndex = 8;
      this.buttonDefaultViewImageReset.Text = "X";
      this.ToolTip1.SetToolTip(this.buttonDefaultViewImageReset, "Click to reset image.\r\n(Image itself will not be deleted.)");
      this.buttonDefaultViewImageReset.UseVisualStyleBackColor = true;
      this.buttonDefaultViewImageReset.Click += new System.EventHandler(this.buttonDefaultViewImageReset_Click);
      // 
      // pictureBoxDefaultViewImage
      // 
      this.pictureBoxDefaultViewImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBoxDefaultViewImage.Location = new System.Drawing.Point(672, 16);
      this.pictureBoxDefaultViewImage.Name = "pictureBoxDefaultViewImage";
      this.pictureBoxDefaultViewImage.Size = new System.Drawing.Size(33, 50);
      this.pictureBoxDefaultViewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBoxDefaultViewImage.TabIndex = 81;
      this.pictureBoxDefaultViewImage.TabStop = false;
      this.ToolTip1.SetToolTip(this.pictureBoxDefaultViewImage, resources.GetString("pictureBoxDefaultViewImage.ToolTip"));
      this.pictureBoxDefaultViewImage.Click += new System.EventHandler(this.pictureBoxDefaultViewImage_Click);
      // 
      // chkDfltViewsAll
      // 
      this.chkDfltViewsAll.AutoSize = true;
      this.chkDfltViewsAll.Location = new System.Drawing.Point(229, 16);
      this.chkDfltViewsAll.Name = "chkDfltViewsAll";
      this.chkDfltViewsAll.Size = new System.Drawing.Size(104, 17);
      this.chkDfltViewsAll.TabIndex = 1;
      this.chkDfltViewsAll.Text = "Use for all Views";
      this.ToolTip1.SetToolTip(this.chkDfltViewsAll, "If unchecked, group thumbs will only be used for genre, year and country.\r\nIf che" +
        "cked, it will be used for all group views except \r\nthe person views (actor, dire" +
        "ctor, producer, borrower).");
      this.chkDfltViewsAll.UseVisualStyleBackColor = true;
      // 
      // label47
      // 
      this.label47.AutoSize = true;
      this.label47.Location = new System.Drawing.Point(622, 16);
      this.label47.Name = "label47";
      this.label47.Size = new System.Drawing.Size(44, 26);
      this.label47.TabIndex = 7;
      this.label47.Text = "Default \r\nImage";
      // 
      // btnResetThumbs
      // 
      this.btnResetThumbs.Location = new System.Drawing.Point(473, 39);
      this.btnResetThumbs.Name = "btnResetThumbs";
      this.btnResetThumbs.Size = new System.Drawing.Size(77, 20);
      this.btnResetThumbs.TabIndex = 9;
      this.btnResetThumbs.Text = "Reset Cache";
      this.ToolTip1.SetToolTip(this.btnResetThumbs, resources.GetString("btnResetThumbs.ToolTip"));
      this.btnResetThumbs.UseVisualStyleBackColor = true;
      this.btnResetThumbs.Click += new System.EventHandler(this.btnResetThumbs_Click);
      // 
      // chkViews
      // 
      this.chkViews.AutoSize = true;
      this.chkViews.Location = new System.Drawing.Point(18, 15);
      this.chkViews.Name = "chkViews";
      this.chkViews.Size = new System.Drawing.Size(142, 17);
      this.chkViews.TabIndex = 0;
      this.chkViews.Text = "Enable Images for Views";
      this.chkViews.UseVisualStyleBackColor = true;
      // 
      // btnViews
      // 
      this.btnViews.Location = new System.Drawing.Point(569, 13);
      this.btnViews.Name = "btnViews";
      this.btnViews.Size = new System.Drawing.Size(32, 20);
      this.btnViews.TabIndex = 6;
      this.btnViews.Text = "...";
      this.btnViews.UseVisualStyleBackColor = true;
      this.btnViews.Click += new System.EventHandler(this.btnViews_Click);
      // 
      // chkDfltViews
      // 
      this.chkDfltViews.AutoSize = true;
      this.chkDfltViews.Location = new System.Drawing.Point(37, 35);
      this.chkDfltViews.Name = "chkDfltViews";
      this.chkDfltViews.Size = new System.Drawing.Size(200, 17);
      this.chkDfltViews.TabIndex = 2;
      this.chkDfltViews.Text = "Use default image for missing thumbs";
      this.chkDfltViews.UseVisualStyleBackColor = true;
      // 
      // label29
      // 
      this.label29.AutoSize = true;
      this.label29.Location = new System.Drawing.Point(353, 17);
      this.label29.Name = "label29";
      this.label29.Size = new System.Drawing.Size(97, 13);
      this.label29.TabIndex = 4;
      this.label29.Text = "Views Images Path";
      // 
      // MesFilmsViews
      // 
      this.MesFilmsViews.Location = new System.Drawing.Point(455, 13);
      this.MesFilmsViews.Name = "MesFilmsViews";
      this.MesFilmsViews.Size = new System.Drawing.Size(108, 20);
      this.MesFilmsViews.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.MesFilmsViews, resources.GetString("MesFilmsViews.ToolTip"));
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btnResetThumbsFilms);
      this.groupBox1.Controls.Add(this.buttonDefaultCoverReset);
      this.groupBox1.Controls.Add(this.pictureBoxDefaultCover);
      this.groupBox1.Controls.Add(this.lblPicturePrefix);
      this.groupBox1.Controls.Add(this.txtPicturePrefix);
      this.groupBox1.Controls.Add(this.lblPictureHandling);
      this.groupBox1.Controls.Add(this.cbPictureHandling);
      this.groupBox1.Controls.Add(this.label_DefaulCover);
      this.groupBox1.Controls.Add(this.ButImg);
      this.groupBox1.Controls.Add(this.MesFilmsImg);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Location = new System.Drawing.Point(6, 7);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(735, 67);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Films ...";
      this.ToolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
      // 
      // btnResetThumbsFilms
      // 
      this.btnResetThumbsFilms.Location = new System.Drawing.Point(473, 39);
      this.btnResetThumbsFilms.Name = "btnResetThumbsFilms";
      this.btnResetThumbsFilms.Size = new System.Drawing.Size(77, 20);
      this.btnResetThumbsFilms.TabIndex = 9;
      this.btnResetThumbsFilms.Text = "Reset Cache";
      this.btnResetThumbsFilms.UseVisualStyleBackColor = true;
      this.btnResetThumbsFilms.Click += new System.EventHandler(this.btnResetThumbsFilms_Click);
      // 
      // buttonDefaultCoverReset
      // 
      this.buttonDefaultCoverReset.Location = new System.Drawing.Point(711, 11);
      this.buttonDefaultCoverReset.Name = "buttonDefaultCoverReset";
      this.buttonDefaultCoverReset.Size = new System.Drawing.Size(18, 18);
      this.buttonDefaultCoverReset.TabIndex = 8;
      this.buttonDefaultCoverReset.Text = "X";
      this.ToolTip1.SetToolTip(this.buttonDefaultCoverReset, "Click to reset image.\r\n(Image itself will not be deleted.)");
      this.buttonDefaultCoverReset.UseVisualStyleBackColor = true;
      this.buttonDefaultCoverReset.Click += new System.EventHandler(this.buttonDefaultCoverReset_Click_1);
      // 
      // pictureBoxDefaultCover
      // 
      this.pictureBoxDefaultCover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBoxDefaultCover.Location = new System.Drawing.Point(672, 11);
      this.pictureBoxDefaultCover.Name = "pictureBoxDefaultCover";
      this.pictureBoxDefaultCover.Size = new System.Drawing.Size(33, 50);
      this.pictureBoxDefaultCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBoxDefaultCover.TabIndex = 99;
      this.pictureBoxDefaultCover.TabStop = false;
      this.ToolTip1.SetToolTip(this.pictureBoxDefaultCover, resources.GetString("pictureBoxDefaultCover.ToolTip"));
      this.pictureBoxDefaultCover.Click += new System.EventHandler(this.pictureBoxDefaultCover_Click);
      // 
      // lblPicturePrefix
      // 
      this.lblPicturePrefix.AutoSize = true;
      this.lblPicturePrefix.Location = new System.Drawing.Point(15, 16);
      this.lblPicturePrefix.Name = "lblPicturePrefix";
      this.lblPicturePrefix.Size = new System.Drawing.Size(69, 13);
      this.lblPicturePrefix.TabIndex = 0;
      this.lblPicturePrefix.Text = "Picture Prefix";
      // 
      // txtPicturePrefix
      // 
      this.txtPicturePrefix.Location = new System.Drawing.Point(106, 13);
      this.txtPicturePrefix.Name = "txtPicturePrefix";
      this.txtPicturePrefix.Size = new System.Drawing.Size(122, 20);
      this.txtPicturePrefix.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.txtPicturePrefix, resources.GetString("txtPicturePrefix.ToolTip"));
      // 
      // lblPictureHandling
      // 
      this.lblPictureHandling.AutoSize = true;
      this.lblPictureHandling.Location = new System.Drawing.Point(15, 42);
      this.lblPictureHandling.Name = "lblPictureHandling";
      this.lblPictureHandling.Size = new System.Drawing.Size(85, 13);
      this.lblPictureHandling.TabIndex = 2;
      this.lblPictureHandling.Text = "Picture Handling";
      // 
      // cbPictureHandling
      // 
      this.cbPictureHandling.BackColor = System.Drawing.SystemColors.Info;
      this.cbPictureHandling.FormattingEnabled = true;
      this.cbPictureHandling.Items.AddRange(new object[] {
            "Full Path",
            "Relative Path",
            "Use Folder.jpg"});
      this.cbPictureHandling.Location = new System.Drawing.Point(106, 40);
      this.cbPictureHandling.Name = "cbPictureHandling";
      this.cbPictureHandling.Size = new System.Drawing.Size(122, 21);
      this.cbPictureHandling.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.cbPictureHandling, resources.GetString("cbPictureHandling.ToolTip"));
      this.cbPictureHandling.SelectedIndexChanged += new System.EventHandler(this.cbPictureHandling_SelectedIndexChanged);
      // 
      // label_DefaulCover
      // 
      this.label_DefaulCover.AutoSize = true;
      this.label_DefaulCover.Location = new System.Drawing.Point(622, 11);
      this.label_DefaulCover.Name = "label_DefaulCover";
      this.label_DefaulCover.Size = new System.Drawing.Size(44, 26);
      this.label_DefaulCover.TabIndex = 7;
      this.label_DefaulCover.Text = "Default \r\nCover";
      // 
      // ButImg
      // 
      this.ButImg.Location = new System.Drawing.Point(569, 14);
      this.ButImg.Name = "ButImg";
      this.ButImg.Size = new System.Drawing.Size(32, 20);
      this.ButImg.TabIndex = 6;
      this.ButImg.Text = "...";
      this.ButImg.UseVisualStyleBackColor = true;
      this.ButImg.Click += new System.EventHandler(this.ButImg_Click);
      // 
      // MesFilmsImg
      // 
      this.MesFilmsImg.Location = new System.Drawing.Point(455, 14);
      this.MesFilmsImg.Name = "MesFilmsImg";
      this.MesFilmsImg.Size = new System.Drawing.Size(108, 20);
      this.MesFilmsImg.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.MesFilmsImg, resources.GetString("MesFilmsImg.ToolTip"));
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(352, 18);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(97, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Cover Images Path";
      // 
      // groupBox_ArtistImages
      // 
      this.groupBox_ArtistImages.Controls.Add(this.chkPersonsEnableDownloads);
      this.groupBox_ArtistImages.Controls.Add(this.buttonDefaultPersonImageReset);
      this.groupBox_ArtistImages.Controls.Add(this.pictureBoxDefaultPersonImage);
      this.groupBox_ArtistImages.Controls.Add(this.chkPersons);
      this.groupBox_ArtistImages.Controls.Add(this.btnResetThumbsArtist);
      this.groupBox_ArtistImages.Controls.Add(this.chkDfltArtist);
      this.groupBox_ArtistImages.Controls.Add(this.ButImgArtist);
      this.groupBox_ArtistImages.Controls.Add(this.MesFilmsImgArtist);
      this.groupBox_ArtistImages.Controls.Add(this.label_DefaultArtistImage);
      this.groupBox_ArtistImages.Controls.Add(this.label_ArtistImagePath);
      this.groupBox_ArtistImages.Location = new System.Drawing.Point(6, 158);
      this.groupBox_ArtistImages.Name = "groupBox_ArtistImages";
      this.groupBox_ArtistImages.Size = new System.Drawing.Size(735, 69);
      this.groupBox_ArtistImages.TabIndex = 2;
      this.groupBox_ArtistImages.TabStop = false;
      this.groupBox_ArtistImages.Text = "Persons Views ...";
      this.ToolTip1.SetToolTip(this.groupBox_ArtistImages, resources.GetString("groupBox_ArtistImages.ToolTip"));
      // 
      // chkPersonsEnableDownloads
      // 
      this.chkPersonsEnableDownloads.AutoSize = true;
      this.chkPersonsEnableDownloads.Location = new System.Drawing.Point(229, 10);
      this.chkPersonsEnableDownloads.Name = "chkPersonsEnableDownloads";
      this.chkPersonsEnableDownloads.Size = new System.Drawing.Size(79, 30);
      this.chkPersonsEnableDownloads.TabIndex = 1;
      this.chkPersonsEnableDownloads.Text = "Enable \r\nDownloads";
      this.ToolTip1.SetToolTip(this.chkPersonsEnableDownloads, "When enabled, MyFilms will automatically try to download\r\nperson images in Detail" +
        "s View.");
      this.chkPersonsEnableDownloads.UseVisualStyleBackColor = true;
      // 
      // buttonDefaultPersonImageReset
      // 
      this.buttonDefaultPersonImageReset.Location = new System.Drawing.Point(711, 12);
      this.buttonDefaultPersonImageReset.Name = "buttonDefaultPersonImageReset";
      this.buttonDefaultPersonImageReset.Size = new System.Drawing.Size(18, 18);
      this.buttonDefaultPersonImageReset.TabIndex = 7;
      this.buttonDefaultPersonImageReset.Text = "X";
      this.ToolTip1.SetToolTip(this.buttonDefaultPersonImageReset, "Click to reset image.\r\n(Image itself will not be deleted.)");
      this.buttonDefaultPersonImageReset.UseVisualStyleBackColor = true;
      this.buttonDefaultPersonImageReset.Click += new System.EventHandler(this.buttonDefaultPersonImageReset_Click);
      // 
      // pictureBoxDefaultPersonImage
      // 
      this.pictureBoxDefaultPersonImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBoxDefaultPersonImage.Location = new System.Drawing.Point(672, 12);
      this.pictureBoxDefaultPersonImage.Name = "pictureBoxDefaultPersonImage";
      this.pictureBoxDefaultPersonImage.Size = new System.Drawing.Size(33, 50);
      this.pictureBoxDefaultPersonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBoxDefaultPersonImage.TabIndex = 101;
      this.pictureBoxDefaultPersonImage.TabStop = false;
      this.ToolTip1.SetToolTip(this.pictureBoxDefaultPersonImage, resources.GetString("pictureBoxDefaultPersonImage.ToolTip"));
      this.pictureBoxDefaultPersonImage.Click += new System.EventHandler(this.pictureBoxDefaultPersonImage_Click);
      // 
      // chkPersons
      // 
      this.chkPersons.AutoSize = true;
      this.chkPersons.Location = new System.Drawing.Point(18, 15);
      this.chkPersons.Name = "chkPersons";
      this.chkPersons.Size = new System.Drawing.Size(152, 17);
      this.chkPersons.TabIndex = 0;
      this.chkPersons.Text = "Enable Images for Persons";
      this.chkPersons.UseVisualStyleBackColor = true;
      // 
      // btnResetThumbsArtist
      // 
      this.btnResetThumbsArtist.Location = new System.Drawing.Point(473, 40);
      this.btnResetThumbsArtist.Name = "btnResetThumbsArtist";
      this.btnResetThumbsArtist.Size = new System.Drawing.Size(77, 20);
      this.btnResetThumbsArtist.TabIndex = 8;
      this.btnResetThumbsArtist.Text = "Reset Cache";
      this.ToolTip1.SetToolTip(this.btnResetThumbsArtist, resources.GetString("btnResetThumbsArtist.ToolTip"));
      this.btnResetThumbsArtist.UseVisualStyleBackColor = true;
      this.btnResetThumbsArtist.Click += new System.EventHandler(this.btnResetThumbsArtist_Click);
      // 
      // chkDfltArtist
      // 
      this.chkDfltArtist.AutoSize = true;
      this.chkDfltArtist.Location = new System.Drawing.Point(37, 38);
      this.chkDfltArtist.Name = "chkDfltArtist";
      this.chkDfltArtist.Size = new System.Drawing.Size(200, 17);
      this.chkDfltArtist.TabIndex = 2;
      this.chkDfltArtist.Text = "Use default image for missing thumbs";
      this.chkDfltArtist.UseVisualStyleBackColor = true;
      // 
      // ButImgArtist
      // 
      this.ButImgArtist.Location = new System.Drawing.Point(569, 13);
      this.ButImgArtist.Name = "ButImgArtist";
      this.ButImgArtist.Size = new System.Drawing.Size(32, 20);
      this.ButImgArtist.TabIndex = 5;
      this.ButImgArtist.Text = "...";
      this.ButImgArtist.UseVisualStyleBackColor = true;
      this.ButImgArtist.Click += new System.EventHandler(this.ButImgArtist_Click);
      // 
      // MesFilmsImgArtist
      // 
      this.MesFilmsImgArtist.Location = new System.Drawing.Point(455, 13);
      this.MesFilmsImgArtist.Name = "MesFilmsImgArtist";
      this.MesFilmsImgArtist.Size = new System.Drawing.Size(108, 20);
      this.MesFilmsImgArtist.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.MesFilmsImgArtist, resources.GetString("MesFilmsImgArtist.ToolTip"));
      // 
      // label_DefaultArtistImage
      // 
      this.label_DefaultArtistImage.AutoSize = true;
      this.label_DefaultArtistImage.Location = new System.Drawing.Point(622, 12);
      this.label_DefaultArtistImage.Name = "label_DefaultArtistImage";
      this.label_DefaultArtistImage.Size = new System.Drawing.Size(41, 26);
      this.label_DefaultArtistImage.TabIndex = 6;
      this.label_DefaultArtistImage.Text = "Default\r\nImage";
      // 
      // label_ArtistImagePath
      // 
      this.label_ArtistImagePath.AutoSize = true;
      this.label_ArtistImagePath.Location = new System.Drawing.Point(347, 16);
      this.label_ArtistImagePath.Name = "label_ArtistImagePath";
      this.label_ArtistImagePath.Size = new System.Drawing.Size(102, 13);
      this.label_ArtistImagePath.TabIndex = 3;
      this.label_ArtistImagePath.Text = "Person Images Path";
      // 
      // groupBox_AMCupdater
      // 
      this.groupBox_AMCupdater.BackColor = System.Drawing.Color.Transparent;
      this.groupBox_AMCupdater.Controls.Add(this.chkAMC_ImportOnInternetFail);
      this.groupBox_AMCupdater.Controls.Add(this.labelAMCUstartdelay);
      this.groupBox_AMCupdater.Controls.Add(this.numericUpDownAMCUscanStartDelay);
      this.groupBox_AMCupdater.Controls.Add(this.chkAMCUwatchScanFolders);
      this.groupBox_AMCupdater.Controls.Add(this.chkAMCUscanOnStartup);
      this.groupBox_AMCupdater.Controls.Add(this.groupBoxAMCUpdaterConfigFile);
      this.groupBox_AMCupdater.Controls.Add(this.groupBox_AMCupdaterScheduer);
      this.groupBox_AMCupdater.Controls.Add(this.groupBox_AMCupdater_ExternalApplication);
      this.groupBox_AMCupdater.Controls.Add(this.chkAMCUpd);
      this.groupBox_AMCupdater.Location = new System.Drawing.Point(6, 9);
      this.groupBox_AMCupdater.Name = "groupBox_AMCupdater";
      this.groupBox_AMCupdater.Size = new System.Drawing.Size(735, 251);
      this.groupBox_AMCupdater.TabIndex = 0;
      this.groupBox_AMCupdater.TabStop = false;
      this.groupBox_AMCupdater.Text = "AMCUpdater Options (for global internet update functions)";
      this.ToolTip1.SetToolTip(this.groupBox_AMCupdater, resources.GetString("groupBox_AMCupdater.ToolTip"));
      // 
      // chkAMC_ImportOnInternetFail
      // 
      this.chkAMC_ImportOnInternetFail.AutoSize = true;
      this.chkAMC_ImportOnInternetFail.Location = new System.Drawing.Point(62, 59);
      this.chkAMC_ImportOnInternetFail.Name = "chkAMC_ImportOnInternetFail";
      this.chkAMC_ImportOnInternetFail.Size = new System.Drawing.Size(134, 17);
      this.chkAMC_ImportOnInternetFail.TabIndex = 8;
      this.chkAMC_ImportOnInternetFail.Text = "Import if no auto match";
      this.ToolTip1.SetToolTip(this.chkAMC_ImportOnInternetFail, resources.GetString("chkAMC_ImportOnInternetFail.ToolTip"));
      this.chkAMC_ImportOnInternetFail.UseVisualStyleBackColor = true;
      // 
      // labelAMCUstartdelay
      // 
      this.labelAMCUstartdelay.AutoSize = true;
      this.labelAMCUstartdelay.Location = new System.Drawing.Point(221, 83);
      this.labelAMCUstartdelay.Name = "labelAMCUstartdelay";
      this.labelAMCUstartdelay.Size = new System.Drawing.Size(59, 13);
      this.labelAMCUstartdelay.TabIndex = 7;
      this.labelAMCUstartdelay.Text = "Start Delay";
      // 
      // numericUpDownAMCUscanStartDelay
      // 
      this.numericUpDownAMCUscanStartDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numericUpDownAMCUscanStartDelay.Location = new System.Drawing.Point(286, 81);
      this.numericUpDownAMCUscanStartDelay.Name = "numericUpDownAMCUscanStartDelay";
      this.numericUpDownAMCUscanStartDelay.Size = new System.Drawing.Size(53, 20);
      this.numericUpDownAMCUscanStartDelay.TabIndex = 6;
      this.ToolTip1.SetToolTip(this.numericUpDownAMCUscanStartDelay, "It is recommended to delay the Scan for new movies.\r\nSet the number of seconds.");
      this.numericUpDownAMCUscanStartDelay.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
      // 
      // chkAMCUwatchScanFolders
      // 
      this.chkAMCUwatchScanFolders.AutoSize = true;
      this.chkAMCUwatchScanFolders.Location = new System.Drawing.Point(62, 105);
      this.chkAMCUwatchScanFolders.Name = "chkAMCUwatchScanFolders";
      this.chkAMCUwatchScanFolders.Size = new System.Drawing.Size(122, 17);
      this.chkAMCUwatchScanFolders.TabIndex = 5;
      this.chkAMCUwatchScanFolders.Text = "Watch Scan Path(s)";
      this.ToolTip1.SetToolTip(this.chkAMCUwatchScanFolders, resources.GetString("chkAMCUwatchScanFolders.ToolTip"));
      this.chkAMCUwatchScanFolders.UseVisualStyleBackColor = true;
      // 
      // chkAMCUscanOnStartup
      // 
      this.chkAMCUscanOnStartup.AutoSize = true;
      this.chkAMCUscanOnStartup.Location = new System.Drawing.Point(62, 82);
      this.chkAMCUscanOnStartup.Name = "chkAMCUscanOnStartup";
      this.chkAMCUscanOnStartup.Size = new System.Drawing.Size(103, 17);
      this.chkAMCUscanOnStartup.TabIndex = 4;
      this.chkAMCUscanOnStartup.Text = "Scan on Startup";
      this.ToolTip1.SetToolTip(this.chkAMCUscanOnStartup, resources.GetString("chkAMCUscanOnStartup.ToolTip"));
      this.chkAMCUscanOnStartup.UseVisualStyleBackColor = true;
      // 
      // groupBoxAMCUpdaterConfigFile
      // 
      this.groupBoxAMCUpdaterConfigFile.Controls.Add(this.txtAMCUpd_cnf_Display);
      this.groupBoxAMCUpdaterConfigFile.Controls.Add(this.btnAMCUpd_cnf);
      this.groupBoxAMCUpdaterConfigFile.Controls.Add(this.txtAMCUpd_cnf);
      this.groupBoxAMCUpdaterConfigFile.Location = new System.Drawing.Point(388, 13);
      this.groupBoxAMCUpdaterConfigFile.Name = "groupBoxAMCUpdaterConfigFile";
      this.groupBoxAMCUpdaterConfigFile.Size = new System.Drawing.Size(332, 53);
      this.groupBoxAMCUpdaterConfigFile.TabIndex = 1;
      this.groupBoxAMCUpdaterConfigFile.TabStop = false;
      this.groupBoxAMCUpdaterConfigFile.Text = "AMCU Config File ...";
      // 
      // txtAMCUpd_cnf_Display
      // 
      this.txtAMCUpd_cnf_Display.Enabled = false;
      this.txtAMCUpd_cnf_Display.Location = new System.Drawing.Point(17, 19);
      this.txtAMCUpd_cnf_Display.Name = "txtAMCUpd_cnf_Display";
      this.txtAMCUpd_cnf_Display.Size = new System.Drawing.Size(255, 20);
      this.txtAMCUpd_cnf_Display.TabIndex = 0;
      // 
      // btnAMCUpd_cnf
      // 
      this.btnAMCUpd_cnf.Location = new System.Drawing.Point(278, 19);
      this.btnAMCUpd_cnf.Name = "btnAMCUpd_cnf";
      this.btnAMCUpd_cnf.Size = new System.Drawing.Size(32, 20);
      this.btnAMCUpd_cnf.TabIndex = 1;
      this.btnAMCUpd_cnf.Text = "...";
      this.btnAMCUpd_cnf.UseVisualStyleBackColor = true;
      this.btnAMCUpd_cnf.Click += new System.EventHandler(this.btnAMCUpd_cnf_Click);
      // 
      // txtAMCUpd_cnf
      // 
      this.txtAMCUpd_cnf.Location = new System.Drawing.Point(18, 19);
      this.txtAMCUpd_cnf.Name = "txtAMCUpd_cnf";
      this.txtAMCUpd_cnf.Size = new System.Drawing.Size(168, 20);
      this.txtAMCUpd_cnf.TabIndex = 57;
      this.ToolTip1.SetToolTip(this.txtAMCUpd_cnf, resources.GetString("txtAMCUpd_cnf.ToolTip"));
      this.txtAMCUpd_cnf.Visible = false;
      this.txtAMCUpd_cnf.TextChanged += new System.EventHandler(this.txtAMCUpd_cnf_TextChanged);
      // 
      // groupBox_AMCupdaterScheduer
      // 
      this.groupBox_AMCupdaterScheduer.Controls.Add(this.scheduleAMCUpdater);
      this.groupBox_AMCupdaterScheduer.Controls.Add(this.btnParameters);
      this.groupBox_AMCupdaterScheduer.Location = new System.Drawing.Point(388, 71);
      this.groupBox_AMCupdaterScheduer.Name = "groupBox_AMCupdaterScheduer";
      this.groupBox_AMCupdaterScheduer.Size = new System.Drawing.Size(332, 52);
      this.groupBox_AMCupdaterScheduer.TabIndex = 2;
      this.groupBox_AMCupdaterScheduer.TabStop = false;
      this.groupBox_AMCupdaterScheduer.Text = "Scheduler for AMCupdater";
      this.ToolTip1.SetToolTip(this.groupBox_AMCupdaterScheduer, "The scheduled task will use the AMCU Config file you configured \r\nor alternativel" +
        "y fallback to the one autocreated in Mediaportal data directory.");
      // 
      // scheduleAMCUpdater
      // 
      this.scheduleAMCUpdater.AutoSize = true;
      this.scheduleAMCUpdater.Location = new System.Drawing.Point(32, 21);
      this.scheduleAMCUpdater.Name = "scheduleAMCUpdater";
      this.scheduleAMCUpdater.Size = new System.Drawing.Size(71, 17);
      this.scheduleAMCUpdater.TabIndex = 0;
      this.scheduleAMCUpdater.Text = "Schedule";
      this.ToolTip1.SetToolTip(this.scheduleAMCUpdater, resources.GetString("scheduleAMCUpdater.ToolTip"));
      this.scheduleAMCUpdater.UseVisualStyleBackColor = true;
      this.scheduleAMCUpdater.CheckedChanged += new System.EventHandler(this.scheduleAMCUpdater_CheckedChanged);
      // 
      // btnParameters
      // 
      this.btnParameters.Location = new System.Drawing.Point(167, 16);
      this.btnParameters.Name = "btnParameters";
      this.btnParameters.Size = new System.Drawing.Size(143, 24);
      this.btnParameters.TabIndex = 0;
      this.btnParameters.Text = "Task Parameters";
      this.btnParameters.UseVisualStyleBackColor = true;
      this.btnParameters.Click += new System.EventHandler(this.btnParameters_Click);
      // 
      // groupBox_AMCupdater_ExternalApplication
      // 
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.label56);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.AmcTitleSearchHandling);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnCreateAMCDefaultConfig);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.lblAMCMovieScanPath);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.chkAMC_Purge_Missing_Files);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnAMCMovieScanPathAdd);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.AMCMovieScanPath);
      this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnCreateAMCDesktopIcon);
      this.groupBox_AMCupdater_ExternalApplication.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.groupBox_AMCupdater_ExternalApplication.Location = new System.Drawing.Point(12, 137);
      this.groupBox_AMCupdater_ExternalApplication.Name = "groupBox_AMCupdater_ExternalApplication";
      this.groupBox_AMCupdater_ExternalApplication.Size = new System.Drawing.Size(709, 105);
      this.groupBox_AMCupdater_ExternalApplication.TabIndex = 3;
      this.groupBox_AMCupdater_ExternalApplication.TabStop = false;
      this.groupBox_AMCupdater_ExternalApplication.Text = "AMC Updater external application";
      // 
      // label56
      // 
      this.label56.AutoSize = true;
      this.label56.Location = new System.Drawing.Point(10, 51);
      this.label56.Name = "label56";
      this.label56.Size = new System.Drawing.Size(112, 13);
      this.label56.TabIndex = 3;
      this.label56.Text = "Title & Search Handling";
      // 
      // AmcTitleSearchHandling
      // 
      this.AmcTitleSearchHandling.FormattingEnabled = true;
      this.AmcTitleSearchHandling.Items.AddRange(new object[] {
            "File Name",
            "Folder Name",
            "Relative Name",
            "File Name + Internet Lookup",
            "Folder Name + Internet Lookup",
            "Relative Name + Internet Lookup"});
      this.AmcTitleSearchHandling.Location = new System.Drawing.Point(139, 48);
      this.AmcTitleSearchHandling.Name = "AmcTitleSearchHandling";
      this.AmcTitleSearchHandling.Size = new System.Drawing.Size(188, 21);
      this.AmcTitleSearchHandling.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.AmcTitleSearchHandling, resources.GetString("AmcTitleSearchHandling.ToolTip"));
      // 
      // btnCreateAMCDefaultConfig
      // 
      this.btnCreateAMCDefaultConfig.AllowDrop = true;
      this.btnCreateAMCDefaultConfig.Location = new System.Drawing.Point(544, 55);
      this.btnCreateAMCDefaultConfig.Name = "btnCreateAMCDefaultConfig";
      this.btnCreateAMCDefaultConfig.Size = new System.Drawing.Size(143, 36);
      this.btnCreateAMCDefaultConfig.TabIndex = 7;
      this.btnCreateAMCDefaultConfig.Text = "(re)Create AMC Updater \r\nDefault Settings";
      this.ToolTip1.SetToolTip(this.btnCreateAMCDefaultConfig, resources.GetString("btnCreateAMCDefaultConfig.ToolTip"));
      this.btnCreateAMCDefaultConfig.UseVisualStyleBackColor = true;
      this.btnCreateAMCDefaultConfig.Click += new System.EventHandler(this.btnCreateAMCDefaultConfig_Click);
      // 
      // lblAMCMovieScanPath
      // 
      this.lblAMCMovieScanPath.AutoSize = true;
      this.lblAMCMovieScanPath.Location = new System.Drawing.Point(10, 24);
      this.lblAMCMovieScanPath.Name = "lblAMCMovieScanPath";
      this.lblAMCMovieScanPath.Size = new System.Drawing.Size(100, 13);
      this.lblAMCMovieScanPath.TabIndex = 0;
      this.lblAMCMovieScanPath.Text = "Movie Scan Path(s)";
      // 
      // chkAMC_Purge_Missing_Files
      // 
      this.chkAMC_Purge_Missing_Files.AutoSize = true;
      this.chkAMC_Purge_Missing_Files.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.chkAMC_Purge_Missing_Files.ForeColor = System.Drawing.Color.Red;
      this.chkAMC_Purge_Missing_Files.Location = new System.Drawing.Point(139, 75);
      this.chkAMC_Purge_Missing_Files.Name = "chkAMC_Purge_Missing_Files";
      this.chkAMC_Purge_Missing_Files.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.chkAMC_Purge_Missing_Files.Size = new System.Drawing.Size(135, 17);
      this.chkAMC_Purge_Missing_Files.TabIndex = 5;
      this.chkAMC_Purge_Missing_Files.Text = "Purge Orphan Records";
      this.ToolTip1.SetToolTip(this.chkAMC_Purge_Missing_Files, resources.GetString("chkAMC_Purge_Missing_Files.ToolTip"));
      this.chkAMC_Purge_Missing_Files.UseVisualStyleBackColor = true;
      this.chkAMC_Purge_Missing_Files.CheckedChanged += new System.EventHandler(this.chkAMC_Purge_Missing_Files_CheckedChanged);
      // 
      // btnAMCMovieScanPathAdd
      // 
      this.btnAMCMovieScanPathAdd.Location = new System.Drawing.Point(655, 21);
      this.btnAMCMovieScanPathAdd.Name = "btnAMCMovieScanPathAdd";
      this.btnAMCMovieScanPathAdd.Size = new System.Drawing.Size(32, 20);
      this.btnAMCMovieScanPathAdd.TabIndex = 2;
      this.btnAMCMovieScanPathAdd.Text = "...";
      this.btnAMCMovieScanPathAdd.UseVisualStyleBackColor = true;
      this.btnAMCMovieScanPathAdd.Click += new System.EventHandler(this.btnAMCMovieScanPathAdd_Click);
      // 
      // AMCMovieScanPath
      // 
      this.AMCMovieScanPath.Location = new System.Drawing.Point(139, 21);
      this.AMCMovieScanPath.Name = "AMCMovieScanPath";
      this.AMCMovieScanPath.Size = new System.Drawing.Size(510, 20);
      this.AMCMovieScanPath.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.AMCMovieScanPath, "Enter the path(s) to your media files.\r\nMore than one path, separated by semicolo" +
        "n, are allowed.");
      // 
      // btnCreateAMCDesktopIcon
      // 
      this.btnCreateAMCDesktopIcon.Location = new System.Drawing.Point(387, 55);
      this.btnCreateAMCDesktopIcon.Name = "btnCreateAMCDesktopIcon";
      this.btnCreateAMCDesktopIcon.Size = new System.Drawing.Size(143, 36);
      this.btnCreateAMCDesktopIcon.TabIndex = 6;
      this.btnCreateAMCDesktopIcon.Text = "Create Desktop Icon ";
      this.ToolTip1.SetToolTip(this.btnCreateAMCDesktopIcon, "By creating a desktop icon you can launch AMC Updater directly \r\nfrom Windows wit" +
        "hout first starting MyFilms setup.");
      this.btnCreateAMCDesktopIcon.UseVisualStyleBackColor = true;
      this.btnCreateAMCDesktopIcon.Click += new System.EventHandler(this.btnCreateAMCDesktopIcon_Click);
      // 
      // chkAMCUpd
      // 
      this.chkAMCUpd.AutoSize = true;
      this.chkAMCUpd.BackColor = System.Drawing.Color.Transparent;
      this.chkAMCUpd.Location = new System.Drawing.Point(24, 31);
      this.chkAMCUpd.Name = "chkAMCUpd";
      this.chkAMCUpd.Size = new System.Drawing.Size(123, 17);
      this.chkAMCUpd.TabIndex = 0;
      this.chkAMCUpd.Text = "Enable AMCUpdater";
      this.ToolTip1.SetToolTip(this.chkAMCUpd, resources.GetString("chkAMCUpd.ToolTip"));
      this.chkAMCUpd.UseVisualStyleBackColor = false;
      this.chkAMCUpd.CheckedChanged += new System.EventHandler(this.chkAMCUpd_CheckedChanged);
      // 
      // groupBoxDeletionOptions
      // 
      this.groupBoxDeletionOptions.Controls.Add(this.cbSuppressAutomaticAction);
      this.groupBoxDeletionOptions.Controls.Add(this.gpspfield);
      this.groupBoxDeletionOptions.Controls.Add(this.chkSuppressManual);
      this.groupBoxDeletionOptions.Controls.Add(this.chkSuppressAutomatic);
      this.groupBoxDeletionOptions.Location = new System.Drawing.Point(354, 14);
      this.groupBoxDeletionOptions.Name = "groupBoxDeletionOptions";
      this.groupBoxDeletionOptions.Size = new System.Drawing.Size(387, 189);
      this.groupBoxDeletionOptions.TabIndex = 2;
      this.groupBoxDeletionOptions.TabStop = false;
      this.groupBoxDeletionOptions.Text = "Update and Deletion Options ...";
      this.ToolTip1.SetToolTip(this.groupBoxDeletionOptions, resources.GetString("groupBoxDeletionOptions.ToolTip"));
      // 
      // cbSuppressAutomaticAction
      // 
      this.cbSuppressAutomaticAction.Enabled = false;
      this.cbSuppressAutomaticAction.FormattingEnabled = true;
      this.cbSuppressAutomaticAction.Items.AddRange(new object[] {
            "Delete DB Entry only",
            "Delete both DB entry and Movie file",
            "Update DB - Keep movie file",
            "Update DB entry - Delete Movie File"});
      this.cbSuppressAutomaticAction.Location = new System.Drawing.Point(174, 40);
      this.cbSuppressAutomaticAction.Name = "cbSuppressAutomaticAction";
      this.cbSuppressAutomaticAction.Size = new System.Drawing.Size(181, 21);
      this.cbSuppressAutomaticAction.TabIndex = 2;
      this.cbSuppressAutomaticAction.SelectedIndexChanged += new System.EventHandler(this.cbSuppress_SelectedIndexChanged);
      // 
      // gpspfield
      // 
      this.gpspfield.Controls.Add(this.label61);
      this.gpspfield.Controls.Add(this.chkSuppressPlayStopUpdateField);
      this.gpspfield.Controls.Add(this.txtSuppressPlayStopValueToUpdate);
      this.gpspfield.Controls.Add(this.cbSuppressPlayStopFieldToUpdate);
      this.gpspfield.Controls.Add(this.lblUpdateValue);
      this.gpspfield.Enabled = false;
      this.gpspfield.Location = new System.Drawing.Point(20, 78);
      this.gpspfield.Name = "gpspfield";
      this.gpspfield.Size = new System.Drawing.Size(335, 93);
      this.gpspfield.TabIndex = 3;
      this.gpspfield.TabStop = false;
      this.gpspfield.Text = "Player finished Update Action";
      this.ToolTip1.SetToolTip(this.gpspfield, resources.GetString("gpspfield.ToolTip"));
      // 
      // label61
      // 
      this.label61.AutoSize = true;
      this.label61.Location = new System.Drawing.Point(141, 42);
      this.label61.Name = "label61";
      this.label61.Size = new System.Drawing.Size(72, 13);
      this.label61.TabIndex = 3;
      this.label61.Text = "Update Value";
      // 
      // chkSuppressPlayStopUpdateField
      // 
      this.chkSuppressPlayStopUpdateField.AutoSize = true;
      this.chkSuppressPlayStopUpdateField.Enabled = false;
      this.chkSuppressPlayStopUpdateField.Location = new System.Drawing.Point(13, 22);
      this.chkSuppressPlayStopUpdateField.Name = "chkSuppressPlayStopUpdateField";
      this.chkSuppressPlayStopUpdateField.Size = new System.Drawing.Size(184, 17);
      this.chkSuppressPlayStopUpdateField.TabIndex = 0;
      this.chkSuppressPlayStopUpdateField.Text = "Update Field when player finishes";
      this.ToolTip1.SetToolTip(this.chkSuppressPlayStopUpdateField, "You can update a DB field with a specified value after a movie is watched.");
      this.chkSuppressPlayStopUpdateField.UseVisualStyleBackColor = true;
      this.chkSuppressPlayStopUpdateField.CheckedChanged += new System.EventHandler(this.chksupplaystop_CheckedChanged);
      // 
      // txtSuppressPlayStopValueToUpdate
      // 
      this.txtSuppressPlayStopValueToUpdate.Location = new System.Drawing.Point(142, 60);
      this.txtSuppressPlayStopValueToUpdate.Name = "txtSuppressPlayStopValueToUpdate";
      this.txtSuppressPlayStopValueToUpdate.Size = new System.Drawing.Size(115, 20);
      this.txtSuppressPlayStopValueToUpdate.TabIndex = 4;
      // 
      // cbSuppressPlayStopFieldToUpdate
      // 
      this.cbSuppressPlayStopFieldToUpdate.FormattingEnabled = true;
      this.cbSuppressPlayStopFieldToUpdate.Location = new System.Drawing.Point(13, 60);
      this.cbSuppressPlayStopFieldToUpdate.Name = "cbSuppressPlayStopFieldToUpdate";
      this.cbSuppressPlayStopFieldToUpdate.Size = new System.Drawing.Size(123, 21);
      this.cbSuppressPlayStopFieldToUpdate.Sorted = true;
      this.cbSuppressPlayStopFieldToUpdate.TabIndex = 2;
      this.cbSuppressPlayStopFieldToUpdate.SelectedIndexChanged += new System.EventHandler(this.cbfdupdate_SelectedIndexChanged);
      // 
      // lblUpdateValue
      // 
      this.lblUpdateValue.AutoSize = true;
      this.lblUpdateValue.Location = new System.Drawing.Point(12, 43);
      this.lblUpdateValue.Name = "lblUpdateValue";
      this.lblUpdateValue.Size = new System.Drawing.Size(93, 13);
      this.lblUpdateValue.TabIndex = 1;
      this.lblUpdateValue.Text = "DB Item to update";
      // 
      // chkSuppressManual
      // 
      this.chkSuppressManual.AutoSize = true;
      this.chkSuppressManual.Location = new System.Drawing.Point(20, 19);
      this.chkSuppressManual.Name = "chkSuppressManual";
      this.chkSuppressManual.Size = new System.Drawing.Size(136, 17);
      this.chkSuppressManual.TabIndex = 0;
      this.chkSuppressManual.Text = "Enable manual deletion";
      this.ToolTip1.SetToolTip(this.chkSuppressManual, "Enables an entry in context menu to manually delete DB record and/or media files." +
        "\r\nNote: Enabling allows the user to delete files physically - don\'t enable for c" +
        "onfig used by your kids ;-)");
      this.chkSuppressManual.UseVisualStyleBackColor = true;
      this.chkSuppressManual.CheckedChanged += new System.EventHandler(this.chkSuppressManual_CheckedChanged);
      // 
      // chkSuppressAutomatic
      // 
      this.chkSuppressAutomatic.AutoSize = true;
      this.chkSuppressAutomatic.Location = new System.Drawing.Point(20, 42);
      this.chkSuppressAutomatic.Name = "chkSuppressAutomatic";
      this.chkSuppressAutomatic.Size = new System.Drawing.Size(148, 17);
      this.chkSuppressAutomatic.TabIndex = 1;
      this.chkSuppressAutomatic.Text = "Enable automatic deletion";
      this.ToolTip1.SetToolTip(this.chkSuppressAutomatic, resources.GetString("chkSuppressAutomatic.ToolTip"));
      this.chkSuppressAutomatic.UseVisualStyleBackColor = true;
      this.chkSuppressAutomatic.CheckedChanged += new System.EventHandler(this.chkSuppress_CheckedChanged);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnWatchedImport);
      this.groupBox2.Controls.Add(this.btnWatchedExport);
      this.groupBox2.Controls.Add(this.lblUnwatchedItemsValue);
      this.groupBox2.Controls.Add(this.textBoxGlobalUnwatchedOnlyValue);
      this.groupBox2.Controls.Add(this.CheckWatchedPlayerStopped);
      this.groupBox2.Controls.Add(this.label19);
      this.groupBox2.Controls.Add(this.cbWatched);
      this.groupBox2.Controls.Add(this.CheckWatched);
      this.groupBox2.Location = new System.Drawing.Point(6, 14);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(338, 110);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Watched-Status Handling";
      this.ToolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
      // 
      // btnWatchedImport
      // 
      this.btnWatchedImport.Location = new System.Drawing.Point(255, 33);
      this.btnWatchedImport.Name = "btnWatchedImport";
      this.btnWatchedImport.Size = new System.Drawing.Size(75, 21);
      this.btnWatchedImport.TabIndex = 3;
      this.btnWatchedImport.Text = "import";
      this.ToolTip1.SetToolTip(this.btnWatchedImport, resources.GetString("btnWatchedImport.ToolTip"));
      this.btnWatchedImport.UseVisualStyleBackColor = true;
      this.btnWatchedImport.Click += new System.EventHandler(this.btnWatchedImport_Click);
      // 
      // btnWatchedExport
      // 
      this.btnWatchedExport.Location = new System.Drawing.Point(255, 11);
      this.btnWatchedExport.Name = "btnWatchedExport";
      this.btnWatchedExport.Size = new System.Drawing.Size(75, 21);
      this.btnWatchedExport.TabIndex = 1;
      this.btnWatchedExport.Text = "export";
      this.ToolTip1.SetToolTip(this.btnWatchedExport, resources.GetString("btnWatchedExport.ToolTip"));
      this.btnWatchedExport.UseVisualStyleBackColor = true;
      this.btnWatchedExport.Click += new System.EventHandler(this.btnWatchedExport_Click);
      // 
      // lblUnwatchedItemsValue
      // 
      this.lblUnwatchedItemsValue.AutoSize = true;
      this.lblUnwatchedItemsValue.Location = new System.Drawing.Point(165, 62);
      this.lblUnwatchedItemsValue.Name = "lblUnwatchedItemsValue";
      this.lblUnwatchedItemsValue.Size = new System.Drawing.Size(165, 13);
      this.lblUnwatchedItemsValue.TabIndex = 0;
      this.lblUnwatchedItemsValue.Text = "Value to identify unwatched items";
      // 
      // textBoxGlobalUnwatchedOnlyValue
      // 
      this.textBoxGlobalUnwatchedOnlyValue.Location = new System.Drawing.Point(168, 80);
      this.textBoxGlobalUnwatchedOnlyValue.Name = "textBoxGlobalUnwatchedOnlyValue";
      this.textBoxGlobalUnwatchedOnlyValue.Size = new System.Drawing.Size(162, 20);
      this.textBoxGlobalUnwatchedOnlyValue.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.textBoxGlobalUnwatchedOnlyValue, resources.GetString("textBoxGlobalUnwatchedOnlyValue.ToolTip"));
      // 
      // CheckWatchedPlayerStopped
      // 
      this.CheckWatchedPlayerStopped.AutoSize = true;
      this.CheckWatchedPlayerStopped.Location = new System.Drawing.Point(10, 37);
      this.CheckWatchedPlayerStopped.Name = "CheckWatchedPlayerStopped";
      this.CheckWatchedPlayerStopped.Size = new System.Drawing.Size(238, 17);
      this.CheckWatchedPlayerStopped.TabIndex = 2;
      this.CheckWatchedPlayerStopped.Text = "Update when playback is stopped or finished";
      this.ToolTip1.SetToolTip(this.CheckWatchedPlayerStopped, "Watched status (by updating the field configured for Watched) will be set if:\r\n- " +
        "movie ended\r\n- movie was stopped at a playtime >= 80%");
      this.CheckWatchedPlayerStopped.UseVisualStyleBackColor = true;
      this.CheckWatchedPlayerStopped.CheckedChanged += new System.EventHandler(this.CheckWatchedPlayerStopped_CheckedChanged);
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(8, 62);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(150, 13);
      this.label19.TabIndex = 4;
      this.label19.Text = "Field used for Watched Status";
      // 
      // cbWatched
      // 
      this.cbWatched.FormattingEnabled = true;
      this.cbWatched.Location = new System.Drawing.Point(10, 79);
      this.cbWatched.Name = "cbWatched";
      this.cbWatched.Size = new System.Drawing.Size(123, 21);
      this.cbWatched.Sorted = true;
      this.cbWatched.TabIndex = 6;
      this.ToolTip1.SetToolTip(this.cbWatched, resources.GetString("cbWatched.ToolTip"));
      // 
      // CheckWatched
      // 
      this.CheckWatched.AutoSize = true;
      this.CheckWatched.Location = new System.Drawing.Point(10, 17);
      this.CheckWatched.Name = "CheckWatched";
      this.CheckWatched.Size = new System.Drawing.Size(178, 17);
      this.CheckWatched.TabIndex = 0;
      this.CheckWatched.Text = "Update when movie is launched";
      this.ToolTip1.SetToolTip(this.CheckWatched, "Watched status (by updating the field configured for Watched) will be set if:\r\n- " +
        "movie is started");
      this.CheckWatched.UseVisualStyleBackColor = true;
      this.CheckWatched.CheckedChanged += new System.EventHandler(this.CheckWatched_CheckedChanged);
      // 
      // btnMUSdeleteUserData
      // 
      this.btnMUSdeleteUserData.Location = new System.Drawing.Point(226, 11);
      this.btnMUSdeleteUserData.Name = "btnMUSdeleteUserData";
      this.btnMUSdeleteUserData.Size = new System.Drawing.Size(104, 21);
      this.btnMUSdeleteUserData.TabIndex = 1;
      this.btnMUSdeleteUserData.Text = "Delete User Data";
      this.ToolTip1.SetToolTip(this.btnMUSdeleteUserData, resources.GetString("btnMUSdeleteUserData.ToolTip"));
      this.btnMUSdeleteUserData.UseVisualStyleBackColor = true;
      this.btnMUSdeleteUserData.Visible = false;
      this.btnMUSdeleteUserData.Click += new System.EventHandler(this.button1_Click);
      // 
      // UserProfileName
      // 
      this.UserProfileName.Location = new System.Drawing.Point(168, 34);
      this.UserProfileName.Name = "UserProfileName";
      this.UserProfileName.Size = new System.Drawing.Size(162, 20);
      this.UserProfileName.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.UserProfileName, resources.GetString("UserProfileName.ToolTip"));
      // 
      // groupBox_UserItemsDetails
      // 
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails6);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails6);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails6);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_DetailsLabel);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_DetailsDBitem);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails5);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails4);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails3);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails2);
      this.groupBox_UserItemsDetails.Controls.Add(this.label_UserItemDetails1);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails5);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails4);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails3);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails2);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntLabelDetails1);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails5);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails4);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails3);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails2);
      this.groupBox_UserItemsDetails.Controls.Add(this.AntItemDetails1);
      this.groupBox_UserItemsDetails.Location = new System.Drawing.Point(387, 52);
      this.groupBox_UserItemsDetails.Name = "groupBox_UserItemsDetails";
      this.groupBox_UserItemsDetails.Size = new System.Drawing.Size(324, 221);
      this.groupBox_UserItemsDetails.TabIndex = 1;
      this.groupBox_UserItemsDetails.TabStop = false;
      this.groupBox_UserItemsDetails.Text = "Custom Display Items for Details Screen";
      this.ToolTip1.SetToolTip(this.groupBox_UserItemsDetails, resources.GetString("groupBox_UserItemsDetails.ToolTip"));
      // 
      // AntLabelDetails6
      // 
      this.AntLabelDetails6.Location = new System.Drawing.Point(165, 177);
      this.AntLabelDetails6.Name = "AntLabelDetails6";
      this.AntLabelDetails6.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails6.TabIndex = 0;
      // 
      // AntItemDetails6
      // 
      this.AntItemDetails6.FormattingEnabled = true;
      this.AntItemDetails6.Location = new System.Drawing.Point(45, 176);
      this.AntItemDetails6.Name = "AntItemDetails6";
      this.AntItemDetails6.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails6.Sorted = true;
      this.AntItemDetails6.TabIndex = 18;
      // 
      // label_UserItemDetails6
      // 
      this.label_UserItemDetails6.AutoSize = true;
      this.label_UserItemDetails6.Location = new System.Drawing.Point(20, 179);
      this.label_UserItemDetails6.Name = "label_UserItemDetails6";
      this.label_UserItemDetails6.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails6.TabIndex = 17;
      this.label_UserItemDetails6.Text = "6";
      // 
      // label_DetailsLabel
      // 
      this.label_DetailsLabel.AutoSize = true;
      this.label_DetailsLabel.Location = new System.Drawing.Point(172, 23);
      this.label_DetailsLabel.Name = "label_DetailsLabel";
      this.label_DetailsLabel.Size = new System.Drawing.Size(33, 13);
      this.label_DetailsLabel.TabIndex = 1;
      this.label_DetailsLabel.Text = "Label";
      // 
      // label_DetailsDBitem
      // 
      this.label_DetailsDBitem.AutoSize = true;
      this.label_DetailsDBitem.Location = new System.Drawing.Point(43, 23);
      this.label_DetailsDBitem.Name = "label_DetailsDBitem";
      this.label_DetailsDBitem.Size = new System.Drawing.Size(94, 13);
      this.label_DetailsDBitem.TabIndex = 0;
      this.label_DetailsDBitem.Text = "DB Item to Display";
      // 
      // label_UserItemDetails5
      // 
      this.label_UserItemDetails5.AutoSize = true;
      this.label_UserItemDetails5.Location = new System.Drawing.Point(20, 152);
      this.label_UserItemDetails5.Name = "label_UserItemDetails5";
      this.label_UserItemDetails5.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails5.TabIndex = 14;
      this.label_UserItemDetails5.Text = "5";
      // 
      // label_UserItemDetails4
      // 
      this.label_UserItemDetails4.AutoSize = true;
      this.label_UserItemDetails4.Location = new System.Drawing.Point(20, 125);
      this.label_UserItemDetails4.Name = "label_UserItemDetails4";
      this.label_UserItemDetails4.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails4.TabIndex = 11;
      this.label_UserItemDetails4.Text = "4";
      // 
      // label_UserItemDetails3
      // 
      this.label_UserItemDetails3.AutoSize = true;
      this.label_UserItemDetails3.Location = new System.Drawing.Point(20, 98);
      this.label_UserItemDetails3.Name = "label_UserItemDetails3";
      this.label_UserItemDetails3.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails3.TabIndex = 8;
      this.label_UserItemDetails3.Text = "3";
      // 
      // label_UserItemDetails2
      // 
      this.label_UserItemDetails2.AutoSize = true;
      this.label_UserItemDetails2.Location = new System.Drawing.Point(20, 71);
      this.label_UserItemDetails2.Name = "label_UserItemDetails2";
      this.label_UserItemDetails2.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails2.TabIndex = 5;
      this.label_UserItemDetails2.Text = "2";
      // 
      // label_UserItemDetails1
      // 
      this.label_UserItemDetails1.AutoSize = true;
      this.label_UserItemDetails1.Location = new System.Drawing.Point(20, 42);
      this.label_UserItemDetails1.Name = "label_UserItemDetails1";
      this.label_UserItemDetails1.Size = new System.Drawing.Size(13, 13);
      this.label_UserItemDetails1.TabIndex = 2;
      this.label_UserItemDetails1.Text = "1";
      // 
      // AntLabelDetails5
      // 
      this.AntLabelDetails5.Location = new System.Drawing.Point(165, 149);
      this.AntLabelDetails5.Name = "AntLabelDetails5";
      this.AntLabelDetails5.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails5.TabIndex = 16;
      // 
      // AntLabelDetails4
      // 
      this.AntLabelDetails4.Location = new System.Drawing.Point(165, 122);
      this.AntLabelDetails4.Name = "AntLabelDetails4";
      this.AntLabelDetails4.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails4.TabIndex = 13;
      // 
      // AntLabelDetails3
      // 
      this.AntLabelDetails3.Location = new System.Drawing.Point(165, 95);
      this.AntLabelDetails3.Name = "AntLabelDetails3";
      this.AntLabelDetails3.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails3.TabIndex = 10;
      // 
      // AntLabelDetails2
      // 
      this.AntLabelDetails2.Location = new System.Drawing.Point(165, 68);
      this.AntLabelDetails2.Name = "AntLabelDetails2";
      this.AntLabelDetails2.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails2.TabIndex = 7;
      // 
      // AntLabelDetails1
      // 
      this.AntLabelDetails1.Location = new System.Drawing.Point(165, 39);
      this.AntLabelDetails1.Name = "AntLabelDetails1";
      this.AntLabelDetails1.Size = new System.Drawing.Size(105, 20);
      this.AntLabelDetails1.TabIndex = 4;
      // 
      // AntItemDetails5
      // 
      this.AntItemDetails5.FormattingEnabled = true;
      this.AntItemDetails5.Location = new System.Drawing.Point(45, 149);
      this.AntItemDetails5.Name = "AntItemDetails5";
      this.AntItemDetails5.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails5.Sorted = true;
      this.AntItemDetails5.TabIndex = 15;
      // 
      // AntItemDetails4
      // 
      this.AntItemDetails4.FormattingEnabled = true;
      this.AntItemDetails4.Location = new System.Drawing.Point(45, 122);
      this.AntItemDetails4.Name = "AntItemDetails4";
      this.AntItemDetails4.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails4.Sorted = true;
      this.AntItemDetails4.TabIndex = 12;
      // 
      // AntItemDetails3
      // 
      this.AntItemDetails3.FormattingEnabled = true;
      this.AntItemDetails3.Location = new System.Drawing.Point(45, 95);
      this.AntItemDetails3.Name = "AntItemDetails3";
      this.AntItemDetails3.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails3.Sorted = true;
      this.AntItemDetails3.TabIndex = 9;
      // 
      // AntItemDetails2
      // 
      this.AntItemDetails2.FormattingEnabled = true;
      this.AntItemDetails2.Location = new System.Drawing.Point(45, 68);
      this.AntItemDetails2.Name = "AntItemDetails2";
      this.AntItemDetails2.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails2.Sorted = true;
      this.AntItemDetails2.TabIndex = 6;
      // 
      // AntItemDetails1
      // 
      this.AntItemDetails1.FormattingEnabled = true;
      this.AntItemDetails1.Location = new System.Drawing.Point(45, 39);
      this.AntItemDetails1.Name = "AntItemDetails1";
      this.AntItemDetails1.Size = new System.Drawing.Size(101, 21);
      this.AntItemDetails1.Sorted = true;
      this.AntItemDetails1.TabIndex = 3;
      // 
      // groupBox_UserItemsMain
      // 
      this.groupBox_UserItemsMain.Controls.Add(this.label_UserItem5);
      this.groupBox_UserItemsMain.Controls.Add(this.label_UserItem4);
      this.groupBox_UserItemsMain.Controls.Add(this.label_UserItem3);
      this.groupBox_UserItemsMain.Controls.Add(this.label_UserItem2);
      this.groupBox_UserItemsMain.Controls.Add(this.label_UserItem1);
      this.groupBox_UserItemsMain.Controls.Add(this.AntLabel5);
      this.groupBox_UserItemsMain.Controls.Add(this.AntLabel4);
      this.groupBox_UserItemsMain.Controls.Add(this.AntItem5);
      this.groupBox_UserItemsMain.Controls.Add(this.AntItem4);
      this.groupBox_UserItemsMain.Controls.Add(this.AntLabel3);
      this.groupBox_UserItemsMain.Controls.Add(this.AntItem3);
      this.groupBox_UserItemsMain.Controls.Add(this.AntItem2);
      this.groupBox_UserItemsMain.Controls.Add(this.AntLabel2);
      this.groupBox_UserItemsMain.Controls.Add(this.label_MainDBitem);
      this.groupBox_UserItemsMain.Controls.Add(this.AntItem1);
      this.groupBox_UserItemsMain.Controls.Add(this.labelMainLabel);
      this.groupBox_UserItemsMain.Controls.Add(this.AntLabel1);
      this.groupBox_UserItemsMain.Location = new System.Drawing.Point(21, 52);
      this.groupBox_UserItemsMain.Name = "groupBox_UserItemsMain";
      this.groupBox_UserItemsMain.Size = new System.Drawing.Size(324, 221);
      this.groupBox_UserItemsMain.TabIndex = 0;
      this.groupBox_UserItemsMain.TabStop = false;
      this.groupBox_UserItemsMain.Text = "Custom Display Items for Main Screen";
      this.ToolTip1.SetToolTip(this.groupBox_UserItemsMain, resources.GetString("groupBox_UserItemsMain.ToolTip"));
      // 
      // label_UserItem5
      // 
      this.label_UserItem5.AutoSize = true;
      this.label_UserItem5.Location = new System.Drawing.Point(20, 152);
      this.label_UserItem5.Name = "label_UserItem5";
      this.label_UserItem5.Size = new System.Drawing.Size(13, 13);
      this.label_UserItem5.TabIndex = 14;
      this.label_UserItem5.Text = "5";
      // 
      // label_UserItem4
      // 
      this.label_UserItem4.AutoSize = true;
      this.label_UserItem4.Location = new System.Drawing.Point(20, 125);
      this.label_UserItem4.Name = "label_UserItem4";
      this.label_UserItem4.Size = new System.Drawing.Size(13, 13);
      this.label_UserItem4.TabIndex = 11;
      this.label_UserItem4.Text = "4";
      // 
      // label_UserItem3
      // 
      this.label_UserItem3.AutoSize = true;
      this.label_UserItem3.Location = new System.Drawing.Point(20, 98);
      this.label_UserItem3.Name = "label_UserItem3";
      this.label_UserItem3.Size = new System.Drawing.Size(13, 13);
      this.label_UserItem3.TabIndex = 8;
      this.label_UserItem3.Text = "3";
      // 
      // label_UserItem2
      // 
      this.label_UserItem2.AutoSize = true;
      this.label_UserItem2.Location = new System.Drawing.Point(20, 71);
      this.label_UserItem2.Name = "label_UserItem2";
      this.label_UserItem2.Size = new System.Drawing.Size(13, 13);
      this.label_UserItem2.TabIndex = 5;
      this.label_UserItem2.Text = "2";
      // 
      // label_UserItem1
      // 
      this.label_UserItem1.AutoSize = true;
      this.label_UserItem1.Location = new System.Drawing.Point(20, 44);
      this.label_UserItem1.Name = "label_UserItem1";
      this.label_UserItem1.Size = new System.Drawing.Size(13, 13);
      this.label_UserItem1.TabIndex = 2;
      this.label_UserItem1.Text = "1";
      // 
      // AntLabel5
      // 
      this.AntLabel5.Location = new System.Drawing.Point(165, 149);
      this.AntLabel5.Name = "AntLabel5";
      this.AntLabel5.Size = new System.Drawing.Size(105, 20);
      this.AntLabel5.TabIndex = 0;
      // 
      // AntLabel4
      // 
      this.AntLabel4.Location = new System.Drawing.Point(165, 122);
      this.AntLabel4.Name = "AntLabel4";
      this.AntLabel4.Size = new System.Drawing.Size(105, 20);
      this.AntLabel4.TabIndex = 13;
      // 
      // AntItem5
      // 
      this.AntItem5.FormattingEnabled = true;
      this.AntItem5.Location = new System.Drawing.Point(45, 149);
      this.AntItem5.Name = "AntItem5";
      this.AntItem5.Size = new System.Drawing.Size(101, 21);
      this.AntItem5.Sorted = true;
      this.AntItem5.TabIndex = 15;
      // 
      // AntItem4
      // 
      this.AntItem4.FormattingEnabled = true;
      this.AntItem4.Location = new System.Drawing.Point(45, 122);
      this.AntItem4.Name = "AntItem4";
      this.AntItem4.Size = new System.Drawing.Size(101, 21);
      this.AntItem4.Sorted = true;
      this.AntItem4.TabIndex = 12;
      // 
      // AntLabel3
      // 
      this.AntLabel3.Location = new System.Drawing.Point(165, 95);
      this.AntLabel3.Name = "AntLabel3";
      this.AntLabel3.Size = new System.Drawing.Size(105, 20);
      this.AntLabel3.TabIndex = 10;
      // 
      // AntItem3
      // 
      this.AntItem3.FormattingEnabled = true;
      this.AntItem3.Location = new System.Drawing.Point(45, 95);
      this.AntItem3.Name = "AntItem3";
      this.AntItem3.Size = new System.Drawing.Size(101, 21);
      this.AntItem3.Sorted = true;
      this.AntItem3.TabIndex = 9;
      // 
      // AntItem2
      // 
      this.AntItem2.FormattingEnabled = true;
      this.AntItem2.Location = new System.Drawing.Point(45, 68);
      this.AntItem2.Name = "AntItem2";
      this.AntItem2.Size = new System.Drawing.Size(101, 21);
      this.AntItem2.Sorted = true;
      this.AntItem2.TabIndex = 6;
      // 
      // AntLabel2
      // 
      this.AntLabel2.Location = new System.Drawing.Point(165, 68);
      this.AntLabel2.Name = "AntLabel2";
      this.AntLabel2.Size = new System.Drawing.Size(105, 20);
      this.AntLabel2.TabIndex = 7;
      // 
      // label_MainDBitem
      // 
      this.label_MainDBitem.AutoSize = true;
      this.label_MainDBitem.Location = new System.Drawing.Point(43, 25);
      this.label_MainDBitem.Name = "label_MainDBitem";
      this.label_MainDBitem.Size = new System.Drawing.Size(94, 13);
      this.label_MainDBitem.TabIndex = 0;
      this.label_MainDBitem.Text = "DB Item to Display";
      // 
      // AntItem1
      // 
      this.AntItem1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.AntItem1.FormattingEnabled = true;
      this.AntItem1.Location = new System.Drawing.Point(45, 41);
      this.AntItem1.Name = "AntItem1";
      this.AntItem1.Size = new System.Drawing.Size(101, 21);
      this.AntItem1.Sorted = true;
      this.AntItem1.TabIndex = 3;
      // 
      // labelMainLabel
      // 
      this.labelMainLabel.AutoSize = true;
      this.labelMainLabel.Location = new System.Drawing.Point(172, 25);
      this.labelMainLabel.Name = "labelMainLabel";
      this.labelMainLabel.Size = new System.Drawing.Size(33, 13);
      this.labelMainLabel.TabIndex = 1;
      this.labelMainLabel.Text = "Label";
      // 
      // AntLabel1
      // 
      this.AntLabel1.Location = new System.Drawing.Point(165, 41);
      this.AntLabel1.Name = "AntLabel1";
      this.AntLabel1.Size = new System.Drawing.Size(105, 20);
      this.AntLabel1.TabIndex = 4;
      // 
      // groupBox_Separators
      // 
      this.groupBox_Separators.Controls.Add(this.RoleSeparator5);
      this.groupBox_Separators.Controls.Add(this.RoleSeparator4);
      this.groupBox_Separators.Controls.Add(this.ListSeparator5);
      this.groupBox_Separators.Controls.Add(this.ListSeparator4);
      this.groupBox_Separators.Controls.Add(this.ListSeparator3);
      this.groupBox_Separators.Controls.Add(this.ListSeparator2);
      this.groupBox_Separators.Controls.Add(this.RoleSeparator2);
      this.groupBox_Separators.Controls.Add(this.RoleSeparator3);
      this.groupBox_Separators.Controls.Add(this.label22);
      this.groupBox_Separators.Controls.Add(this.RoleSeparator1);
      this.groupBox_Separators.Controls.Add(this.label24);
      this.groupBox_Separators.Controls.Add(this.ListSeparator1);
      this.groupBox_Separators.Location = new System.Drawing.Point(6, 273);
      this.groupBox_Separators.Name = "groupBox_Separators";
      this.groupBox_Separators.Size = new System.Drawing.Size(467, 74);
      this.groupBox_Separators.TabIndex = 1;
      this.groupBox_Separators.TabStop = false;
      this.groupBox_Separators.Text = "Separators";
      this.ToolTip1.SetToolTip(this.groupBox_Separators, resources.GetString("groupBox_Separators.ToolTip"));
      // 
      // RoleSeparator5
      // 
      this.RoleSeparator5.FormattingEnabled = true;
      this.RoleSeparator5.Items.AddRange(new object[] {
            " as ",
            "(",
            "....",
            ")"});
      this.RoleSeparator5.Location = new System.Drawing.Point(396, 46);
      this.RoleSeparator5.Name = "RoleSeparator5";
      this.RoleSeparator5.Size = new System.Drawing.Size(48, 21);
      this.RoleSeparator5.TabIndex = 11;
      this.RoleSeparator5.Text = "List";
      // 
      // RoleSeparator4
      // 
      this.RoleSeparator4.FormattingEnabled = true;
      this.RoleSeparator4.Items.AddRange(new object[] {
            " as ",
            "(",
            "....",
            ")"});
      this.RoleSeparator4.Location = new System.Drawing.Point(342, 46);
      this.RoleSeparator4.Name = "RoleSeparator4";
      this.RoleSeparator4.Size = new System.Drawing.Size(48, 21);
      this.RoleSeparator4.TabIndex = 10;
      this.RoleSeparator4.Text = "List";
      // 
      // ListSeparator5
      // 
      this.ListSeparator5.FormattingEnabled = true;
      this.ListSeparator5.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
      this.ListSeparator5.Location = new System.Drawing.Point(396, 19);
      this.ListSeparator5.Name = "ListSeparator5";
      this.ListSeparator5.Size = new System.Drawing.Size(48, 21);
      this.ListSeparator5.TabIndex = 5;
      this.ListSeparator5.Text = "List";
      // 
      // ListSeparator4
      // 
      this.ListSeparator4.FormattingEnabled = true;
      this.ListSeparator4.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
      this.ListSeparator4.Location = new System.Drawing.Point(342, 19);
      this.ListSeparator4.Name = "ListSeparator4";
      this.ListSeparator4.Size = new System.Drawing.Size(48, 21);
      this.ListSeparator4.TabIndex = 4;
      this.ListSeparator4.Text = "List";
      // 
      // ListSeparator3
      // 
      this.ListSeparator3.FormattingEnabled = true;
      this.ListSeparator3.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
      this.ListSeparator3.Location = new System.Drawing.Point(288, 19);
      this.ListSeparator3.Name = "ListSeparator3";
      this.ListSeparator3.Size = new System.Drawing.Size(48, 21);
      this.ListSeparator3.TabIndex = 3;
      this.ListSeparator3.Text = "List";
      // 
      // ListSeparator2
      // 
      this.ListSeparator2.FormattingEnabled = true;
      this.ListSeparator2.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
      this.ListSeparator2.Location = new System.Drawing.Point(234, 19);
      this.ListSeparator2.Name = "ListSeparator2";
      this.ListSeparator2.Size = new System.Drawing.Size(48, 21);
      this.ListSeparator2.TabIndex = 2;
      this.ListSeparator2.Text = "List";
      // 
      // RoleSeparator2
      // 
      this.RoleSeparator2.FormattingEnabled = true;
      this.RoleSeparator2.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
      this.RoleSeparator2.Location = new System.Drawing.Point(234, 46);
      this.RoleSeparator2.Name = "RoleSeparator2";
      this.RoleSeparator2.Size = new System.Drawing.Size(48, 21);
      this.RoleSeparator2.Sorted = true;
      this.RoleSeparator2.TabIndex = 8;
      this.RoleSeparator2.Text = "List";
      // 
      // RoleSeparator3
      // 
      this.RoleSeparator3.FormattingEnabled = true;
      this.RoleSeparator3.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
      this.RoleSeparator3.Location = new System.Drawing.Point(288, 46);
      this.RoleSeparator3.Name = "RoleSeparator3";
      this.RoleSeparator3.Size = new System.Drawing.Size(48, 21);
      this.RoleSeparator3.Sorted = true;
      this.RoleSeparator3.TabIndex = 9;
      this.RoleSeparator3.Text = "List";
      // 
      // label22
      // 
      this.label22.AutoSize = true;
      this.label22.Location = new System.Drawing.Point(14, 49);
      this.label22.Name = "label22";
      this.label22.Size = new System.Drawing.Size(107, 13);
      this.label22.TabIndex = 6;
      this.label22.Text = "Role Text Separators";
      this.ToolTip1.SetToolTip(this.label22, "Separators to remove \"roles\" from person names for Actors, Producers, etc.");
      // 
      // RoleSeparator1
      // 
      this.RoleSeparator1.FormattingEnabled = true;
      this.RoleSeparator1.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
      this.RoleSeparator1.Location = new System.Drawing.Point(180, 46);
      this.RoleSeparator1.Name = "RoleSeparator1";
      this.RoleSeparator1.Size = new System.Drawing.Size(48, 21);
      this.RoleSeparator1.Sorted = true;
      this.RoleSeparator1.TabIndex = 7;
      this.RoleSeparator1.Text = "List";
      // 
      // label24
      // 
      this.label24.AutoSize = true;
      this.label24.Location = new System.Drawing.Point(14, 22);
      this.label24.Name = "label24";
      this.label24.Size = new System.Drawing.Size(72, 13);
      this.label24.TabIndex = 0;
      this.label24.Text = "List Separator";
      this.ToolTip1.SetToolTip(this.label24, "Separators used for splitting splittable fields, like Genre, Country, Actors, etc" +
        ".");
      // 
      // ListSeparator1
      // 
      this.ListSeparator1.FormattingEnabled = true;
      this.ListSeparator1.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
      this.ListSeparator1.Location = new System.Drawing.Point(180, 19);
      this.ListSeparator1.Name = "ListSeparator1";
      this.ListSeparator1.Size = new System.Drawing.Size(48, 21);
      this.ListSeparator1.TabIndex = 1;
      this.ListSeparator1.Text = "List";
      // 
      // groupBox_DefaultView
      // 
      this.groupBox_DefaultView.Controls.Add(this.chkVirtualPathBrowsing);
      this.groupBox_DefaultView.Controls.Add(this.chkReversePersonNames);
      this.groupBox_DefaultView.Controls.Add(this.chkShowEmpty);
      this.groupBox_DefaultView.Controls.Add(this.chkOnlyTitle);
      this.groupBox_DefaultView.Controls.Add(this.groupBox7);
      this.groupBox_DefaultView.Controls.Add(this.groupBox4);
      this.groupBox_DefaultView.Controls.Add(this.label10);
      this.groupBox_DefaultView.Controls.Add(this.chkGlobalAvailableOnly);
      this.groupBox_DefaultView.Controls.Add(this.chkGlobalUnwatchedOnly);
      this.groupBox_DefaultView.Controls.Add(this.AlwaysDefaultView);
      this.groupBox_DefaultView.Controls.Add(this.View_Dflt_Item);
      this.groupBox_DefaultView.Controls.Add(this.View_Dflt_Text);
      this.groupBox_DefaultView.Location = new System.Drawing.Point(480, 5);
      this.groupBox_DefaultView.Name = "groupBox_DefaultView";
      this.groupBox_DefaultView.Size = new System.Drawing.Size(258, 342);
      this.groupBox_DefaultView.TabIndex = 2;
      this.groupBox_DefaultView.TabStop = false;
      this.groupBox_DefaultView.Text = "Start Settings ...";
      this.ToolTip1.SetToolTip(this.groupBox_DefaultView, resources.GetString("groupBox_DefaultView.ToolTip"));
      // 
      // chkVirtualPathBrowsing
      // 
      this.chkVirtualPathBrowsing.AutoSize = true;
      this.chkVirtualPathBrowsing.Location = new System.Drawing.Point(158, 298);
      this.chkVirtualPathBrowsing.Name = "chkVirtualPathBrowsing";
      this.chkVirtualPathBrowsing.Size = new System.Drawing.Size(91, 17);
      this.chkVirtualPathBrowsing.TabIndex = 0;
      this.chkVirtualPathBrowsing.Text = "Virtual Pathes";
      this.ToolTip1.SetToolTip(this.chkVirtualPathBrowsing, "If enabled, your mediapathes will be used for browsing film lists.\r\nThis allows n" +
        "avigtion similar to browse the filesystem, though it is still based \r\non DB cont" +
        "ent  (media path).");
      this.chkVirtualPathBrowsing.UseVisualStyleBackColor = true;
      // 
      // chkReversePersonNames
      // 
      this.chkReversePersonNames.AutoSize = true;
      this.chkReversePersonNames.Location = new System.Drawing.Point(67, 319);
      this.chkReversePersonNames.Name = "chkReversePersonNames";
      this.chkReversePersonNames.Size = new System.Drawing.Size(138, 17);
      this.chkReversePersonNames.TabIndex = 11;
      this.chkReversePersonNames.Text = "Reverse Person Names";
      this.ToolTip1.SetToolTip(this.chkReversePersonNames, resources.GetString("chkReversePersonNames.ToolTip"));
      this.chkReversePersonNames.UseVisualStyleBackColor = true;
      // 
      // chkShowEmpty
      // 
      this.chkShowEmpty.AutoSize = true;
      this.chkShowEmpty.Location = new System.Drawing.Point(67, 298);
      this.chkShowEmpty.Name = "chkShowEmpty";
      this.chkShowEmpty.Size = new System.Drawing.Size(85, 17);
      this.chkShowEmpty.TabIndex = 10;
      this.chkShowEmpty.Text = "Show Empty";
      this.ToolTip1.SetToolTip(this.chkShowEmpty, resources.GetString("chkShowEmpty.ToolTip"));
      this.chkShowEmpty.UseVisualStyleBackColor = true;
      // 
      // chkOnlyTitle
      // 
      this.chkOnlyTitle.AutoSize = true;
      this.chkOnlyTitle.Location = new System.Drawing.Point(67, 229);
      this.chkOnlyTitle.Name = "chkOnlyTitle";
      this.chkOnlyTitle.Size = new System.Drawing.Size(167, 17);
      this.chkOnlyTitle.TabIndex = 7;
      this.chkOnlyTitle.Text = "Only Movie Title in List Layout";
      this.ToolTip1.SetToolTip(this.chkOnlyTitle, "Depending on the skin, there is normally displayed additional information like \"y" +
        "ear\"\r\nor \"counts\".\r\nBy enabling that option, only the title itself will be displ" +
        "ayed in list layouts.");
      this.chkOnlyTitle.UseVisualStyleBackColor = true;
      // 
      // groupBox7
      // 
      this.groupBox7.Controls.Add(this.Sort);
      this.groupBox7.Controls.Add(this.label32);
      this.groupBox7.Controls.Add(this.SortSens);
      this.groupBox7.Controls.Add(this.LayOut);
      this.groupBox7.Controls.Add(this.label14);
      this.groupBox7.Location = new System.Drawing.Point(6, 72);
      this.groupBox7.Name = "groupBox7";
      this.groupBox7.Size = new System.Drawing.Size(246, 73);
      this.groupBox7.TabIndex = 5;
      this.groupBox7.TabStop = false;
      this.groupBox7.Text = "Movies Sort and Layout ...";
      // 
      // Sort
      // 
      this.Sort.FormattingEnabled = true;
      this.Sort.Location = new System.Drawing.Point(61, 19);
      this.Sort.Name = "Sort";
      this.Sort.Size = new System.Drawing.Size(104, 21);
      this.Sort.TabIndex = 1;
      // 
      // label32
      // 
      this.label32.AutoSize = true;
      this.label32.Location = new System.Drawing.Point(6, 22);
      this.label32.Name = "label32";
      this.label32.Size = new System.Drawing.Size(51, 13);
      this.label32.TabIndex = 0;
      this.label32.Text = "Sort Field";
      // 
      // SortSens
      // 
      this.SortSens.FormattingEnabled = true;
      this.SortSens.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
      this.SortSens.Location = new System.Drawing.Point(171, 19);
      this.SortSens.Name = "SortSens";
      this.SortSens.Size = new System.Drawing.Size(55, 21);
      this.SortSens.TabIndex = 2;
      this.SortSens.Text = "ASC";
      // 
      // LayOut
      // 
      this.LayOut.FormattingEnabled = true;
      this.LayOut.Items.AddRange(new object[] {
            "List",
            "Big Icon List",
            "Small Icons",
            "Large Icons",
            "Filmstrip",
            "Cover Flow"});
      this.LayOut.Location = new System.Drawing.Point(61, 46);
      this.LayOut.Name = "LayOut";
      this.LayOut.Size = new System.Drawing.Size(104, 21);
      this.LayOut.TabIndex = 4;
      this.LayOut.Text = "List";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(6, 49);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(39, 13);
      this.label14.TabIndex = 3;
      this.label14.Text = "Layout";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.label26);
      this.groupBox4.Controls.Add(this.LayoutInHierarchies);
      this.groupBox4.Controls.Add(this.label62);
      this.groupBox4.Controls.Add(this.SortSensInHierarchies);
      this.groupBox4.Controls.Add(this.SortInHierarchies);
      this.groupBox4.Location = new System.Drawing.Point(6, 151);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(246, 71);
      this.groupBox4.TabIndex = 6;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Movie Group (Hierarchy) Sort and Layout ...";
      this.ToolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
      // 
      // label26
      // 
      this.label26.AutoSize = true;
      this.label26.Location = new System.Drawing.Point(6, 48);
      this.label26.Name = "label26";
      this.label26.Size = new System.Drawing.Size(39, 13);
      this.label26.TabIndex = 3;
      this.label26.Text = "Layout";
      // 
      // LayoutInHierarchies
      // 
      this.LayoutInHierarchies.FormattingEnabled = true;
      this.LayoutInHierarchies.Items.AddRange(new object[] {
            "List",
            "Big Icon List",
            "Small Icons",
            "Large Icons",
            "Filmstrip",
            "Cover Flow"});
      this.LayoutInHierarchies.Location = new System.Drawing.Point(61, 45);
      this.LayoutInHierarchies.Name = "LayoutInHierarchies";
      this.LayoutInHierarchies.Size = new System.Drawing.Size(104, 21);
      this.LayoutInHierarchies.TabIndex = 4;
      this.LayoutInHierarchies.Text = "List";
      // 
      // label62
      // 
      this.label62.AutoSize = true;
      this.label62.Location = new System.Drawing.Point(6, 22);
      this.label62.Name = "label62";
      this.label62.Size = new System.Drawing.Size(51, 13);
      this.label62.TabIndex = 0;
      this.label62.Text = "Sort Field";
      this.ToolTip1.SetToolTip(this.label62, "Select an optional field to be used for hierarchy sort order.");
      // 
      // SortSensInHierarchies
      // 
      this.SortSensInHierarchies.FormattingEnabled = true;
      this.SortSensInHierarchies.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
      this.SortSensInHierarchies.Location = new System.Drawing.Point(171, 19);
      this.SortSensInHierarchies.Name = "SortSensInHierarchies";
      this.SortSensInHierarchies.Size = new System.Drawing.Size(55, 21);
      this.SortSensInHierarchies.TabIndex = 2;
      this.SortSensInHierarchies.Text = "ASC";
      this.ToolTip1.SetToolTip(this.SortSensInHierarchies, "Select, if hierarchy sort order should be ascending or descending.");
      // 
      // SortInHierarchies
      // 
      this.SortInHierarchies.FormattingEnabled = true;
      this.SortInHierarchies.Location = new System.Drawing.Point(61, 19);
      this.SortInHierarchies.Name = "SortInHierarchies";
      this.SortInHierarchies.Size = new System.Drawing.Size(104, 21);
      this.SortInHierarchies.Sorted = true;
      this.SortInHierarchies.TabIndex = 1;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(12, 21);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(30, 13);
      this.label10.TabIndex = 0;
      this.label10.Text = "View";
      // 
      // chkGlobalAvailableOnly
      // 
      this.chkGlobalAvailableOnly.AutoSize = true;
      this.chkGlobalAvailableOnly.Location = new System.Drawing.Point(67, 275);
      this.chkGlobalAvailableOnly.Name = "chkGlobalAvailableOnly";
      this.chkGlobalAvailableOnly.Size = new System.Drawing.Size(156, 17);
      this.chkGlobalAvailableOnly.TabIndex = 9;
      this.chkGlobalAvailableOnly.Text = "Show only available movies";
      this.ToolTip1.SetToolTip(this.chkGlobalAvailableOnly, resources.GetString("chkGlobalAvailableOnly.ToolTip"));
      this.chkGlobalAvailableOnly.UseVisualStyleBackColor = true;
      this.chkGlobalAvailableOnly.CheckedChanged += new System.EventHandler(this.chkGlobalAvailableOnly_CheckedChanged);
      // 
      // chkGlobalUnwatchedOnly
      // 
      this.chkGlobalUnwatchedOnly.AutoSize = true;
      this.chkGlobalUnwatchedOnly.Location = new System.Drawing.Point(67, 252);
      this.chkGlobalUnwatchedOnly.Name = "chkGlobalUnwatchedOnly";
      this.chkGlobalUnwatchedOnly.Size = new System.Drawing.Size(167, 17);
      this.chkGlobalUnwatchedOnly.TabIndex = 8;
      this.chkGlobalUnwatchedOnly.Text = "Show only unwatched movies";
      this.ToolTip1.SetToolTip(this.chkGlobalUnwatchedOnly, resources.GetString("chkGlobalUnwatchedOnly.ToolTip"));
      this.chkGlobalUnwatchedOnly.UseVisualStyleBackColor = true;
      // 
      // AlwaysDefaultView
      // 
      this.AlwaysDefaultView.AutoSize = true;
      this.AlwaysDefaultView.Location = new System.Drawing.Point(67, 50);
      this.AlwaysDefaultView.Name = "AlwaysDefaultView";
      this.AlwaysDefaultView.Size = new System.Drawing.Size(145, 17);
      this.AlwaysDefaultView.TabIndex = 4;
      this.AlwaysDefaultView.Text = "Always use Start Settings";
      this.ToolTip1.SetToolTip(this.AlwaysDefaultView, resources.GetString("AlwaysDefaultView.ToolTip"));
      this.AlwaysDefaultView.UseVisualStyleBackColor = true;
      // 
      // View_Dflt_Item
      // 
      this.View_Dflt_Item.FormattingEnabled = true;
      this.View_Dflt_Item.Location = new System.Drawing.Point(67, 18);
      this.View_Dflt_Item.Name = "View_Dflt_Item";
      this.View_Dflt_Item.Size = new System.Drawing.Size(104, 21);
      this.View_Dflt_Item.TabIndex = 1;
      this.View_Dflt_Item.SelectedIndexChanged += new System.EventHandler(this.View_Dflt_Item_SelectedIndexChanged);
      // 
      // View_Dflt_Text
      // 
      this.View_Dflt_Text.Enabled = false;
      this.View_Dflt_Text.Location = new System.Drawing.Point(177, 18);
      this.View_Dflt_Text.Name = "View_Dflt_Text";
      this.View_Dflt_Text.Size = new System.Drawing.Size(55, 20);
      this.View_Dflt_Text.TabIndex = 2;
      // 
      // groupBoxView
      // 
      this.groupBoxView.Controls.Add(this.labelViewLabel);
      this.groupBoxView.Controls.Add(this.buttonResetImage);
      this.groupBoxView.Controls.Add(this.AntViewsImage);
      this.groupBoxView.Controls.Add(this.lblAntViewIndex);
      this.groupBoxView.Controls.Add(this.bindingNavigatorViews);
      this.groupBoxView.Controls.Add(this.dgViewsList);
      this.groupBoxView.Controls.Add(this.groupBoxSortAndLayoutForView);
      this.groupBoxView.Controls.Add(this.AntViewItem);
      this.groupBoxView.Controls.Add(this.groupBox5);
      this.groupBoxView.Controls.Add(this.AntViewValue);
      this.groupBoxView.Controls.Add(label66);
      this.groupBoxView.Controls.Add(this.AntViewIndex);
      this.groupBoxView.Controls.Add(label65);
      this.groupBoxView.Location = new System.Drawing.Point(6, 5);
      this.groupBoxView.Name = "groupBoxView";
      this.groupBoxView.Size = new System.Drawing.Size(467, 266);
      this.groupBoxView.TabIndex = 0;
      this.groupBoxView.TabStop = false;
      this.groupBoxView.Text = "Custom View Editor ...";
      this.ToolTip1.SetToolTip(this.groupBoxView, resources.GetString("groupBoxView.ToolTip"));
      // 
      // labelViewLabel
      // 
      this.labelViewLabel.AutoSize = true;
      this.labelViewLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "Label", true));
      this.labelViewLabel.ForeColor = System.Drawing.SystemColors.AppWorkspace;
      this.labelViewLabel.Location = new System.Drawing.Point(179, 48);
      this.labelViewLabel.Name = "labelViewLabel";
      this.labelViewLabel.Size = new System.Drawing.Size(99, 13);
      this.labelViewLabel.TabIndex = 2;
      this.labelViewLabel.Text = "Custom View Name";
      // 
      // viewBindingSource
      // 
      this.viewBindingSource.DataMember = "View";
      this.viewBindingSource.DataSource = this.mFview;
      // 
      // mFview
      // 
      this.mFview.DataSetName = "MFview";
      this.mFview.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // buttonResetImage
      // 
      this.buttonResetImage.Location = new System.Drawing.Point(371, 45);
      this.buttonResetImage.Name = "buttonResetImage";
      this.buttonResetImage.Size = new System.Drawing.Size(18, 18);
      this.buttonResetImage.TabIndex = 3;
      this.buttonResetImage.Text = "X";
      this.ToolTip1.SetToolTip(this.buttonResetImage, "Click to reset image.\r\n(Image itself will not be deleted.)");
      this.buttonResetImage.UseVisualStyleBackColor = true;
      this.buttonResetImage.Click += new System.EventHandler(this.buttonResetImage_Click);
      // 
      // AntViewsImage
      // 
      this.AntViewsImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.AntViewsImage.DataBindings.Add(new System.Windows.Forms.Binding("ImageLocation", this.viewBindingSource, "ImagePath", true));
      this.AntViewsImage.Location = new System.Drawing.Point(395, 45);
      this.AntViewsImage.Name = "AntViewsImage";
      this.AntViewsImage.Size = new System.Drawing.Size(66, 99);
      this.AntViewsImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.AntViewsImage.TabIndex = 88;
      this.AntViewsImage.TabStop = false;
      this.ToolTip1.SetToolTip(this.AntViewsImage, resources.GetString("AntViewsImage.ToolTip"));
      this.AntViewsImage.Click += new System.EventHandler(this.AntViewsImage_Click);
      // 
      // lblAntViewIndex
      // 
      this.lblAntViewIndex.AutoSize = true;
      this.lblAntViewIndex.Location = new System.Drawing.Point(291, 65);
      this.lblAntViewIndex.Name = "lblAntViewIndex";
      this.lblAntViewIndex.Size = new System.Drawing.Size(33, 13);
      this.lblAntViewIndex.TabIndex = 5;
      this.lblAntViewIndex.Text = "Index";
      this.ToolTip1.SetToolTip(this.lblAntViewIndex, "You can define number of chars to be viewd as indexed group\r\nfor this Item, e.g.\r" +
        "\n\"1\" results in getting a view of first letter.");
      // 
      // bindingNavigatorViews
      // 
      this.bindingNavigatorViews.AddNewItem = null;
      this.bindingNavigatorViews.BindingSource = this.viewBindingSource;
      this.bindingNavigatorViews.CountItem = this.toolStripLabel1;
      this.bindingNavigatorViews.DeleteItem = this.toolStripButtonDelete;
      this.bindingNavigatorViews.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.bindingNavigatorViews.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSeparator1,
            this.toolStripTextBox1,
            this.toolStripLabel1,
            this.toolStripSeparator2,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripSeparator3,
            this.toolStripButtonAdd,
            this.toolStripButtonDelete,
            this.toolStripSeparator4,
            this.toolStripButtonMoveUp,
            this.toolStripButtonMoveDown,
            this.toolStripSeparator5,
            this.toolStripButtonAddDefaults});
      this.bindingNavigatorViews.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
      this.bindingNavigatorViews.Location = new System.Drawing.Point(3, 16);
      this.bindingNavigatorViews.MoveFirstItem = this.toolStripButton3;
      this.bindingNavigatorViews.MoveLastItem = this.toolStripButton6;
      this.bindingNavigatorViews.MoveNextItem = this.toolStripButton5;
      this.bindingNavigatorViews.MovePreviousItem = this.toolStripButton4;
      this.bindingNavigatorViews.Name = "bindingNavigatorViews";
      this.bindingNavigatorViews.PositionItem = this.toolStripTextBox1;
      this.bindingNavigatorViews.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.bindingNavigatorViews.Size = new System.Drawing.Size(461, 23);
      this.bindingNavigatorViews.TabIndex = 0;
      this.bindingNavigatorViews.Text = "bindingNavigatorViews";
      // 
      // toolStripLabel1
      // 
      this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 1);
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new System.Drawing.Size(44, 15);
      this.toolStripLabel1.Text = "von {0}";
      this.toolStripLabel1.ToolTipText = "Total number of Items.";
      // 
      // toolStripButtonDelete
      // 
      this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButtonDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelete.Image")));
      this.toolStripButtonDelete.Name = "toolStripButtonDelete";
      this.toolStripButtonDelete.RightToLeftAutoMirrorImage = true;
      this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 20);
      this.toolStripButtonDelete.Text = "Delete";
      this.toolStripButtonDelete.ToolTipText = "Delete current Item";
      // 
      // toolStripButton3
      // 
      this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.RightToLeftAutoMirrorImage = true;
      this.toolStripButton3.Size = new System.Drawing.Size(23, 20);
      this.toolStripButton3.Text = "Select First";
      this.toolStripButton3.ToolTipText = "Select First";
      // 
      // toolStripButton4
      // 
      this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.RightToLeftAutoMirrorImage = true;
      this.toolStripButton4.Size = new System.Drawing.Size(23, 20);
      this.toolStripButton4.Text = "Choose previous";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
      // 
      // toolStripTextBox1
      // 
      this.toolStripTextBox1.AccessibleName = "Position";
      this.toolStripTextBox1.AutoSize = false;
      this.toolStripTextBox1.Name = "toolStripTextBox1";
      this.toolStripTextBox1.Size = new System.Drawing.Size(50, 21);
      this.toolStripTextBox1.Text = "0";
      this.toolStripTextBox1.ToolTipText = "Current Item";
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
      // 
      // toolStripButton5
      // 
      this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.RightToLeftAutoMirrorImage = true;
      this.toolStripButton5.Size = new System.Drawing.Size(23, 20);
      this.toolStripButton5.Text = "Choose next";
      // 
      // toolStripButton6
      // 
      this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.RightToLeftAutoMirrorImage = true;
      this.toolStripButton6.Size = new System.Drawing.Size(23, 20);
      this.toolStripButton6.Text = "Select Last";
      this.toolStripButton6.ToolTipText = "Select Last";
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
      // 
      // toolStripButtonAdd
      // 
      this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd.Image")));
      this.toolStripButtonAdd.Name = "toolStripButtonAdd";
      this.toolStripButtonAdd.RightToLeftAutoMirrorImage = true;
      this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 20);
      this.toolStripButtonAdd.Text = "Add New";
      this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
      // 
      // toolStripButtonMoveUp
      // 
      this.toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButtonMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveUp.Image")));
      this.toolStripButtonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
      this.toolStripButtonMoveUp.Size = new System.Drawing.Size(23, 20);
      this.toolStripButtonMoveUp.Text = "up";
      this.toolStripButtonMoveUp.ToolTipText = "Move Item up";
      this.toolStripButtonMoveUp.Click += new System.EventHandler(this.toolStripButtonMoveUp_Click);
      // 
      // toolStripButtonMoveDown
      // 
      this.toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButtonMoveDown.Image = global::MyFilmsPlugin.Properties.Resources.arrow_down;
      this.toolStripButtonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
      this.toolStripButtonMoveDown.Size = new System.Drawing.Size(23, 20);
      this.toolStripButtonMoveDown.Text = "down";
      this.toolStripButtonMoveDown.ToolTipText = "Move Item down";
      this.toolStripButtonMoveDown.Click += new System.EventHandler(this.toolStripButtonMoveDown_Click);
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
      // 
      // toolStripButtonAddDefaults
      // 
      this.toolStripButtonAddDefaults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripButtonAddDefaults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.toolStripButtonAddDefaults.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddDefaults.Image")));
      this.toolStripButtonAddDefaults.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonAddDefaults.Name = "toolStripButtonAddDefaults";
      this.toolStripButtonAddDefaults.Size = new System.Drawing.Size(69, 17);
      this.toolStripButtonAddDefaults.Text = "add defaults";
      this.toolStripButtonAddDefaults.ToolTipText = "Add default Views:\r\nThis will add the default Views to the Custom Views.\r\nExistin" +
    "g Views will NOT be removed - default views will be added to the end of the list" +
    ".";
      this.toolStripButtonAddDefaults.Click += new System.EventHandler(this.toolStripButtonAddDefaults_Click);
      // 
      // dgViewsList
      // 
      this.dgViewsList.AllowUserToAddRows = false;
      this.dgViewsList.AllowUserToDeleteRows = false;
      this.dgViewsList.AllowUserToResizeRows = false;
      this.dgViewsList.AutoGenerateColumns = false;
      this.dgViewsList.BackgroundColor = System.Drawing.SystemColors.Menu;
      this.dgViewsList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.dgViewsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgViewsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.labelDataGridViewTextBoxColumn,
            this.viewEnabledDataGridViewCheckBoxColumn});
      this.dgViewsList.DataSource = this.viewBindingSource;
      this.dgViewsList.GridColor = System.Drawing.SystemColors.ControlDarkDark;
      this.dgViewsList.Location = new System.Drawing.Point(6, 44);
      this.dgViewsList.MultiSelect = false;
      this.dgViewsList.Name = "dgViewsList";
      this.dgViewsList.RowHeadersVisible = false;
      this.dgViewsList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgViewsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgViewsList.Size = new System.Drawing.Size(158, 216);
      this.dgViewsList.TabIndex = 1;
      this.dgViewsList.Leave += new System.EventHandler(this.dgViewsList_Leave);
      // 
      // labelDataGridViewTextBoxColumn
      // 
      this.labelDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.labelDataGridViewTextBoxColumn.DataPropertyName = "Label";
      this.labelDataGridViewTextBoxColumn.HeaderText = "Label";
      this.labelDataGridViewTextBoxColumn.Name = "labelDataGridViewTextBoxColumn";
      // 
      // viewEnabledDataGridViewCheckBoxColumn
      // 
      this.viewEnabledDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.viewEnabledDataGridViewCheckBoxColumn.DataPropertyName = "ViewEnabled";
      this.viewEnabledDataGridViewCheckBoxColumn.HeaderText = "on";
      this.viewEnabledDataGridViewCheckBoxColumn.Name = "viewEnabledDataGridViewCheckBoxColumn";
      this.viewEnabledDataGridViewCheckBoxColumn.Width = 25;
      // 
      // groupBoxSortAndLayoutForView
      // 
      this.groupBoxSortAndLayoutForView.Controls.Add(label68);
      this.groupBoxSortAndLayoutForView.Controls.Add(this.AntViewSortType);
      this.groupBoxSortAndLayoutForView.Controls.Add(label70);
      this.groupBoxSortAndLayoutForView.Controls.Add(label69);
      this.groupBoxSortAndLayoutForView.Controls.Add(this.AntViewLayoutView);
      this.groupBoxSortAndLayoutForView.Controls.Add(this.AntViewSortOrder);
      this.groupBoxSortAndLayoutForView.Location = new System.Drawing.Point(170, 196);
      this.groupBoxSortAndLayoutForView.Name = "groupBoxSortAndLayoutForView";
      this.groupBoxSortAndLayoutForView.Size = new System.Drawing.Size(291, 64);
      this.groupBoxSortAndLayoutForView.TabIndex = 11;
      this.groupBoxSortAndLayoutForView.TabStop = false;
      this.groupBoxSortAndLayoutForView.Text = "Sort and Layouts for View";
      // 
      // AntViewSortType
      // 
      this.AntViewSortType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "SortFieldViewType", true));
      this.AntViewSortType.FormattingEnabled = true;
      this.AntViewSortType.Items.AddRange(new object[] {
            "Count",
            "Name"});
      this.AntViewSortType.Location = new System.Drawing.Point(10, 34);
      this.AntViewSortType.Name = "AntViewSortType";
      this.AntViewSortType.Size = new System.Drawing.Size(104, 21);
      this.AntViewSortType.Sorted = true;
      this.AntViewSortType.TabIndex = 4;
      // 
      // AntViewLayoutView
      // 
      this.AntViewLayoutView.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "LayoutView", true));
      this.AntViewLayoutView.FormattingEnabled = true;
      this.AntViewLayoutView.Items.AddRange(new object[] {
            "List",
            "Big Icon List",
            "Small Icons",
            "Large Icons",
            "Filmstrip",
            "Cover Flow"});
      this.AntViewLayoutView.Location = new System.Drawing.Point(192, 34);
      this.AntViewLayoutView.Name = "AntViewLayoutView";
      this.AntViewLayoutView.Size = new System.Drawing.Size(90, 21);
      this.AntViewLayoutView.TabIndex = 0;
      // 
      // AntViewSortOrder
      // 
      this.AntViewSortOrder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "SortDirectionView", true));
      this.AntViewSortOrder.FormattingEnabled = true;
      this.AntViewSortOrder.Items.AddRange(new object[] {
            " ASC",
            " DESC"});
      this.AntViewSortOrder.Location = new System.Drawing.Point(120, 34);
      this.AntViewSortOrder.Name = "AntViewSortOrder";
      this.AntViewSortOrder.Size = new System.Drawing.Size(59, 21);
      this.AntViewSortOrder.TabIndex = 5;
      // 
      // AntViewItem
      // 
      this.AntViewItem.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "DBfield", true));
      this.AntViewItem.FormattingEnabled = true;
      this.AntViewItem.Location = new System.Drawing.Point(180, 80);
      this.AntViewItem.Name = "AntViewItem";
      this.AntViewItem.Size = new System.Drawing.Size(104, 21);
      this.AntViewItem.Sorted = true;
      this.AntViewItem.TabIndex = 6;
      this.ToolTip1.SetToolTip(this.AntViewItem, "Select the field you want to use for this view.\r\nThe label is the name the specif" +
        "ic view and will be shown in MyFilms, e.g. \"My Highrated Films\". It can be edite" +
        "d in the View list.\r\n");
      this.AntViewItem.SelectedIndexChanged += new System.EventHandler(this.AntViewItem_SelectedIndexChanged);
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.AntViewFilter);
      this.groupBox5.Controls.Add(this.AntViewFilterEditButton);
      this.groupBox5.Controls.Add(this.AntViewFilterSaveString);
      this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox5.Location = new System.Drawing.Point(170, 145);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(291, 46);
      this.groupBox5.TabIndex = 10;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Filter Expression ...";
      this.ToolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
      // 
      // AntViewFilter
      // 
      this.AntViewFilter.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "Filter", true));
      this.AntViewFilter.Location = new System.Drawing.Point(10, 16);
      this.AntViewFilter.Name = "AntViewFilter";
      this.AntViewFilter.Size = new System.Drawing.Size(243, 20);
      this.AntViewFilter.TabIndex = 0;
      this.AntViewFilter.TextChanged += new System.EventHandler(this.AntViewFilter_TextChanged);
      // 
      // AntViewFilterEditButton
      // 
      this.AntViewFilterEditButton.Location = new System.Drawing.Point(259, 16);
      this.AntViewFilterEditButton.Name = "AntViewFilterEditButton";
      this.AntViewFilterEditButton.Size = new System.Drawing.Size(23, 20);
      this.AntViewFilterEditButton.TabIndex = 1;
      this.AntViewFilterEditButton.Text = "...";
      this.AntViewFilterEditButton.UseVisualStyleBackColor = true;
      this.AntViewFilterEditButton.Click += new System.EventHandler(this.AntViewFilterEditButton_Click);
      // 
      // AntViewFilterSaveString
      // 
      this.AntViewFilterSaveString.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "FilterSaveString", true));
      this.AntViewFilterSaveString.Location = new System.Drawing.Point(128, 16);
      this.AntViewFilterSaveString.Name = "AntViewFilterSaveString";
      this.AntViewFilterSaveString.Size = new System.Drawing.Size(42, 20);
      this.AntViewFilterSaveString.TabIndex = 89;
      this.AntViewFilterSaveString.TextChanged += new System.EventHandler(this.AntViewFilterSaveString_TextChanged);
      // 
      // AntViewValue
      // 
      this.AntViewValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewBindingSource, "Value", true));
      this.AntViewValue.Location = new System.Drawing.Point(180, 120);
      this.AntViewValue.Name = "AntViewValue";
      this.AntViewValue.Size = new System.Drawing.Size(149, 20);
      this.AntViewValue.TabIndex = 9;
      this.AntViewValue.TextChanged += new System.EventHandler(this.AntViewValue_TextChanged);
      // 
      // AntViewIndex
      // 
      this.AntViewIndex.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.viewBindingSource, "Index", true));
      this.AntViewIndex.Location = new System.Drawing.Point(290, 81);
      this.AntViewIndex.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.AntViewIndex.Name = "AntViewIndex";
      this.AntViewIndex.Size = new System.Drawing.Size(39, 20);
      this.AntViewIndex.TabIndex = 7;
      this.ToolTip1.SetToolTip(this.AntViewIndex, "You can view normally (\"0\") or indexed. Indexed means there will be a grouping by" +
        " the first x characters (1 or 2).");
      // 
      // groupBox24
      // 
      this.groupBox24.Controls.Add(this.numUpDownTrailersInIntro);
      this.groupBox24.Controls.Add(this.cbCacheOnlineTrailer);
      this.groupBox24.Controls.Add(this.cbTrailerAutoregister);
      this.groupBox24.Controls.Add(this.labelTrailers);
      this.groupBox24.Controls.Add(this.ShowTrailerWhenStartingMovie);
      this.groupBox24.Controls.Add(this.btnTrailer);
      this.groupBox24.Controls.Add(this.PathStorageTrailer);
      this.groupBox24.Controls.Add(this.label34);
      this.groupBox24.Controls.Add(this.AntStorageTrailer);
      this.groupBox24.Controls.Add(this.label35);
      this.groupBox24.Location = new System.Drawing.Point(23, 37);
      this.groupBox24.Name = "groupBox24";
      this.groupBox24.Size = new System.Drawing.Size(681, 266);
      this.groupBox24.TabIndex = 0;
      this.groupBox24.TabStop = false;
      this.groupBox24.Text = "DB Item for storing trailerinfo (borrower recommended)";
      this.ToolTip1.SetToolTip(this.groupBox24, resources.GetString("groupBox24.ToolTip"));
      // 
      // numUpDownTrailersInIntro
      // 
      this.numUpDownTrailersInIntro.Location = new System.Drawing.Point(283, 223);
      this.numUpDownTrailersInIntro.Name = "numUpDownTrailersInIntro";
      this.numUpDownTrailersInIntro.Size = new System.Drawing.Size(39, 20);
      this.numUpDownTrailersInIntro.TabIndex = 10;
      this.ToolTip1.SetToolTip(this.numUpDownTrailersInIntro, "You can set the number of trailers that will be played before the main movie star" +
        "ts.\r\nSet to \'0\' to disable trailer intro.");
      // 
      // cbCacheOnlineTrailer
      // 
      this.cbCacheOnlineTrailer.AutoSize = true;
      this.cbCacheOnlineTrailer.Location = new System.Drawing.Point(27, 189);
      this.cbCacheOnlineTrailer.Name = "cbCacheOnlineTrailer";
      this.cbCacheOnlineTrailer.Size = new System.Drawing.Size(182, 17);
      this.cbCacheOnlineTrailer.TabIndex = 9;
      this.cbCacheOnlineTrailer.Text = "Cache Trailers for Online Content";
      this.ToolTip1.SetToolTip(this.cbCacheOnlineTrailer, resources.GetString("cbCacheOnlineTrailer.ToolTip"));
      this.cbCacheOnlineTrailer.UseVisualStyleBackColor = true;
      // 
      // cbTrailerAutoregister
      // 
      this.cbTrailerAutoregister.AutoSize = true;
      this.cbTrailerAutoregister.Location = new System.Drawing.Point(27, 155);
      this.cbTrailerAutoregister.Name = "cbTrailerAutoregister";
      this.cbTrailerAutoregister.Size = new System.Drawing.Size(114, 17);
      this.cbTrailerAutoregister.TabIndex = 8;
      this.cbTrailerAutoregister.Text = "Autoregister Trailer";
      this.ToolTip1.SetToolTip(this.cbTrailerAutoregister, resources.GetString("cbTrailerAutoregister.ToolTip"));
      this.cbTrailerAutoregister.UseVisualStyleBackColor = true;
      // 
      // labelTrailers
      // 
      this.labelTrailers.AutoSize = true;
      this.labelTrailers.Location = new System.Drawing.Point(24, 27);
      this.labelTrailers.Name = "labelTrailers";
      this.labelTrailers.Size = new System.Drawing.Size(379, 13);
      this.labelTrailers.TabIndex = 0;
      this.labelTrailers.Text = "To enable Trailer support, you must select a DB Item/field for storing Trailer in" +
    "fo";
      // 
      // ShowTrailerWhenStartingMovie
      // 
      this.ShowTrailerWhenStartingMovie.AutoSize = true;
      this.ShowTrailerWhenStartingMovie.Location = new System.Drawing.Point(27, 224);
      this.ShowTrailerWhenStartingMovie.Name = "ShowTrailerWhenStartingMovie";
      this.ShowTrailerWhenStartingMovie.Size = new System.Drawing.Size(228, 17);
      this.ShowTrailerWhenStartingMovie.TabIndex = 0;
      this.ShowTrailerWhenStartingMovie.Text = "Show Trailer as Movie Intro (Cinema mode)";
      this.ToolTip1.SetToolTip(this.ShowTrailerWhenStartingMovie, "-- Coming in future version --\r\nIf checked, a trailer from same category as movie" +
        " \r\nwill be played before starting the movie itself.");
      this.ShowTrailerWhenStartingMovie.UseVisualStyleBackColor = true;
      // 
      // btnTrailer
      // 
      this.btnTrailer.Location = new System.Drawing.Point(634, 92);
      this.btnTrailer.Name = "btnTrailer";
      this.btnTrailer.Size = new System.Drawing.Size(32, 20);
      this.btnTrailer.TabIndex = 5;
      this.btnTrailer.Text = "...";
      this.btnTrailer.UseVisualStyleBackColor = true;
      this.btnTrailer.Click += new System.EventHandler(this.btnTrailer_Click);
      // 
      // PathStorageTrailer
      // 
      this.PathStorageTrailer.Location = new System.Drawing.Point(190, 92);
      this.PathStorageTrailer.Name = "PathStorageTrailer";
      this.PathStorageTrailer.Size = new System.Drawing.Size(428, 20);
      this.PathStorageTrailer.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.PathStorageTrailer, resources.GetString("PathStorageTrailer.ToolTip"));
      // 
      // label34
      // 
      this.label34.AutoSize = true;
      this.label34.Location = new System.Drawing.Point(24, 95);
      this.label34.Name = "label34";
      this.label34.Size = new System.Drawing.Size(142, 13);
      this.label34.TabIndex = 3;
      this.label34.Text = "Extended Trailer Searchpath";
      // 
      // AntStorageTrailer
      // 
      this.AntStorageTrailer.Location = new System.Drawing.Point(190, 52);
      this.AntStorageTrailer.Name = "AntStorageTrailer";
      this.AntStorageTrailer.Size = new System.Drawing.Size(185, 21);
      this.AntStorageTrailer.Sorted = true;
      this.AntStorageTrailer.TabIndex = 2;
      this.ToolTip1.SetToolTip(this.AntStorageTrailer, resources.GetString("AntStorageTrailer.ToolTip"));
      // 
      // label35
      // 
      this.label35.AutoSize = true;
      this.label35.Location = new System.Drawing.Point(24, 55);
      this.label35.Name = "label35";
      this.label35.Size = new System.Drawing.Size(147, 13);
      this.label35.TabIndex = 1;
      this.label35.Text = "DB Item for storing Trailer Info";
      // 
      // groupBox_AntSelectedEnreg
      // 
      this.groupBox_AntSelectedEnreg.Controls.Add(this.groupBox3);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFreetextFilterItem);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterFreeText);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterComb);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText1);
      this.groupBox_AntSelectedEnreg.Location = new System.Drawing.Point(451, 44);
      this.groupBox_AntSelectedEnreg.Name = "groupBox_AntSelectedEnreg";
      this.groupBox_AntSelectedEnreg.Size = new System.Drawing.Size(286, 101);
      this.groupBox_AntSelectedEnreg.TabIndex = 79;
      this.groupBox_AntSelectedEnreg.TabStop = false;
      this.groupBox_AntSelectedEnreg.Text = "Custom Config Filters";
      this.ToolTip1.SetToolTip(this.groupBox_AntSelectedEnreg, resources.GetString("groupBox_AntSelectedEnreg.ToolTip"));
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btnCustomConfigFilter);
      this.groupBox3.Controls.Add(this.textBoxStrDfltSelect);
      this.groupBox3.Location = new System.Drawing.Point(0, 0);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(286, 101);
      this.groupBox3.TabIndex = 0;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Custom Config Filter";
      this.ToolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
      // 
      // btnCustomConfigFilter
      // 
      this.btnCustomConfigFilter.Location = new System.Drawing.Point(9, 19);
      this.btnCustomConfigFilter.Name = "btnCustomConfigFilter";
      this.btnCustomConfigFilter.Size = new System.Drawing.Size(42, 23);
      this.btnCustomConfigFilter.TabIndex = 0;
      this.btnCustomConfigFilter.Text = "Edit";
      this.btnCustomConfigFilter.UseVisualStyleBackColor = true;
      this.btnCustomConfigFilter.Click += new System.EventHandler(this.btnCustomConfigFilter_Click);
      // 
      // textBoxStrDfltSelect
      // 
      this.textBoxStrDfltSelect.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.textBoxStrDfltSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxStrDfltSelect.Enabled = false;
      this.textBoxStrDfltSelect.Location = new System.Drawing.Point(60, 19);
      this.textBoxStrDfltSelect.Multiline = true;
      this.textBoxStrDfltSelect.Name = "textBoxStrDfltSelect";
      this.textBoxStrDfltSelect.ReadOnly = true;
      this.textBoxStrDfltSelect.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
      this.textBoxStrDfltSelect.Size = new System.Drawing.Size(216, 74);
      this.textBoxStrDfltSelect.TabIndex = 1;
      this.textBoxStrDfltSelect.TabStop = false;
      // 
      // AntFreetextFilterItem
      // 
      this.AntFreetextFilterItem.AutoSize = true;
      this.AntFreetextFilterItem.Location = new System.Drawing.Point(4, 76);
      this.AntFreetextFilterItem.Name = "AntFreetextFilterItem";
      this.AntFreetextFilterItem.Size = new System.Drawing.Size(50, 13);
      this.AntFreetextFilterItem.TabIndex = 7;
      this.AntFreetextFilterItem.Text = "FreeFilter";
      // 
      // AntFilterFreeText
      // 
      this.AntFilterFreeText.Location = new System.Drawing.Point(55, 73);
      this.AntFilterFreeText.Name = "AntFilterFreeText";
      this.AntFilterFreeText.Size = new System.Drawing.Size(220, 20);
      this.AntFilterFreeText.TabIndex = 8;
      this.ToolTip1.SetToolTip(this.AntFilterFreeText, "This field allows you to define freetext conditions for your DB restrictions.\r\nIt" +
        " could be used e.g. to restrict the config to certain actors:\r\n(Actors like \'*Wi" +
        "llis*\' OR Actors like \'*Celentano*\')");
      // 
      // AntFilterComb
      // 
      this.AntFilterComb.DisplayMember = "or";
      this.AntFilterComb.FormattingEnabled = true;
      this.AntFilterComb.ItemHeight = 13;
      this.AntFilterComb.Items.AddRange(new object[] {
            "or",
            "and"});
      this.AntFilterComb.Location = new System.Drawing.Point(6, 34);
      this.AntFilterComb.Name = "AntFilterComb";
      this.AntFilterComb.Size = new System.Drawing.Size(43, 21);
      this.AntFilterComb.TabIndex = 3;
      // 
      // AntFilterSign2
      // 
      this.AntFilterSign2.DisplayMember = "#";
      this.AntFilterSign2.FormattingEnabled = true;
      this.AntFilterSign2.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like",
            "in",
            "not in",
            "like in"});
      this.AntFilterSign2.Location = new System.Drawing.Point(122, 46);
      this.AntFilterSign2.Name = "AntFilterSign2";
      this.AntFilterSign2.Size = new System.Drawing.Size(60, 21);
      this.AntFilterSign2.TabIndex = 5;
      // 
      // AntFilterSign1
      // 
      this.AntFilterSign1.DisplayMember = "#";
      this.AntFilterSign1.FormattingEnabled = true;
      this.AntFilterSign1.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like",
            "in",
            "not in",
            "like in"});
      this.AntFilterSign1.Location = new System.Drawing.Point(122, 19);
      this.AntFilterSign1.Name = "AntFilterSign1";
      this.AntFilterSign1.Size = new System.Drawing.Size(60, 21);
      this.AntFilterSign1.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.AntFilterSign1, resources.GetString("AntFilterSign1.ToolTip"));
      // 
      // AntFilterItem2
      // 
      this.AntFilterItem2.FormattingEnabled = true;
      this.AntFilterItem2.Location = new System.Drawing.Point(55, 46);
      this.AntFilterItem2.Name = "AntFilterItem2";
      this.AntFilterItem2.Size = new System.Drawing.Size(61, 21);
      this.AntFilterItem2.Sorted = true;
      this.AntFilterItem2.TabIndex = 4;
      // 
      // AntFilterText2
      // 
      this.AntFilterText2.Location = new System.Drawing.Point(188, 46);
      this.AntFilterText2.Name = "AntFilterText2";
      this.AntFilterText2.Size = new System.Drawing.Size(87, 20);
      this.AntFilterText2.TabIndex = 6;
      // 
      // AntFilterItem1
      // 
      this.AntFilterItem1.FormattingEnabled = true;
      this.AntFilterItem1.Location = new System.Drawing.Point(55, 19);
      this.AntFilterItem1.Name = "AntFilterItem1";
      this.AntFilterItem1.Size = new System.Drawing.Size(61, 21);
      this.AntFilterItem1.Sorted = true;
      this.AntFilterItem1.TabIndex = 19;
      // 
      // AntFilterText1
      // 
      this.AntFilterText1.Location = new System.Drawing.Point(188, 19);
      this.AntFilterText1.Name = "AntFilterText1";
      this.AntFilterText1.Size = new System.Drawing.Size(87, 20);
      this.AntFilterText1.TabIndex = 2;
      // 
      // groupBox_PreLaunchingCommand
      // 
      this.groupBox_PreLaunchingCommand.Controls.Add(this.label39);
      this.groupBox_PreLaunchingCommand.Controls.Add(this.label38);
      this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdPar);
      this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdExe);
      this.groupBox_PreLaunchingCommand.Location = new System.Drawing.Point(451, 234);
      this.groupBox_PreLaunchingCommand.Name = "groupBox_PreLaunchingCommand";
      this.groupBox_PreLaunchingCommand.Size = new System.Drawing.Size(286, 76);
      this.groupBox_PreLaunchingCommand.TabIndex = 8;
      this.groupBox_PreLaunchingCommand.TabStop = false;
      this.groupBox_PreLaunchingCommand.Text = "Pre-Launching Command";
      this.ToolTip1.SetToolTip(this.groupBox_PreLaunchingCommand, resources.GetString("groupBox_PreLaunchingCommand.ToolTip"));
      // 
      // label39
      // 
      this.label39.AutoSize = true;
      this.label39.Location = new System.Drawing.Point(12, 51);
      this.label39.Name = "label39";
      this.label39.Size = new System.Drawing.Size(55, 13);
      this.label39.TabIndex = 2;
      this.label39.Text = "Parameter";
      // 
      // label38
      // 
      this.label38.AutoSize = true;
      this.label38.Location = new System.Drawing.Point(12, 22);
      this.label38.Name = "label38";
      this.label38.Size = new System.Drawing.Size(77, 13);
      this.label38.TabIndex = 0;
      this.label38.Text = "Command Line";
      // 
      // CmdPar
      // 
      this.CmdPar.FormattingEnabled = true;
      this.CmdPar.Location = new System.Drawing.Point(93, 48);
      this.CmdPar.Name = "CmdPar";
      this.CmdPar.Size = new System.Drawing.Size(140, 21);
      this.CmdPar.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.CmdPar, "The value of the  chosen DB-field will be used as commandline option for the appl" +
        "ication laiunched");
      // 
      // CmdExe
      // 
      this.CmdExe.Location = new System.Drawing.Point(93, 19);
      this.CmdExe.Name = "CmdExe";
      this.CmdExe.Size = new System.Drawing.Size(140, 20);
      this.CmdExe.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.CmdExe, "Enter the commandline to an application you want to launch before the movie is st" +
        "arted in internal player");
      // 
      // tbExternalPlayerStartParams
      // 
      this.tbExternalPlayerStartParams.Location = new System.Drawing.Point(302, 169);
      this.tbExternalPlayerStartParams.Name = "tbExternalPlayerStartParams";
      this.tbExternalPlayerStartParams.Size = new System.Drawing.Size(65, 20);
      this.tbExternalPlayerStartParams.TabIndex = 6;
      this.ToolTip1.SetToolTip(this.tbExternalPlayerStartParams, resources.GetString("tbExternalPlayerStartParams.ToolTip"));
      // 
      // tbExternalPlayerPath
      // 
      this.tbExternalPlayerPath.Location = new System.Drawing.Point(172, 169);
      this.tbExternalPlayerPath.Name = "tbExternalPlayerPath";
      this.tbExternalPlayerPath.Size = new System.Drawing.Size(89, 20);
      this.tbExternalPlayerPath.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.tbExternalPlayerPath, "Path to external player.\r\nLeave empty, if you prefer using internal Player.\r\nIf y" +
        "ou have a valid player defined, MyFilms will offer you \r\na playback option in th" +
        "e context menu.");
      // 
      // tbExternalPlayerExtensions
      // 
      this.tbExternalPlayerExtensions.Location = new System.Drawing.Point(373, 169);
      this.tbExternalPlayerExtensions.Name = "tbExternalPlayerExtensions";
      this.tbExternalPlayerExtensions.Size = new System.Drawing.Size(53, 20);
      this.tbExternalPlayerExtensions.TabIndex = 7;
      this.ToolTip1.SetToolTip(this.tbExternalPlayerExtensions, resources.GetString("tbExternalPlayerExtensions.ToolTip"));
      // 
      // chkScanMediaOnStart
      // 
      this.chkScanMediaOnStart.AutoSize = true;
      this.chkScanMediaOnStart.Location = new System.Drawing.Point(302, 20);
      this.chkScanMediaOnStart.Name = "chkScanMediaOnStart";
      this.chkScanMediaOnStart.Size = new System.Drawing.Size(120, 17);
      this.chkScanMediaOnStart.TabIndex = 10;
      this.chkScanMediaOnStart.Text = "Scan media on start";
      this.ToolTip1.SetToolTip(this.chkScanMediaOnStart, resources.GetString("chkScanMediaOnStart.ToolTip"));
      this.chkScanMediaOnStart.UseVisualStyleBackColor = true;
      // 
      // SearchOnlyExactMatches
      // 
      this.SearchOnlyExactMatches.AutoSize = true;
      this.SearchOnlyExactMatches.Location = new System.Drawing.Point(296, 40);
      this.SearchOnlyExactMatches.Name = "SearchOnlyExactMatches";
      this.SearchOnlyExactMatches.Size = new System.Drawing.Size(79, 30);
      this.SearchOnlyExactMatches.TabIndex = 5;
      this.SearchOnlyExactMatches.Text = "Only exact \r\nMatches";
      this.ToolTip1.SetToolTip(this.SearchOnlyExactMatches, resources.GetString("SearchOnlyExactMatches.ToolTip"));
      this.SearchOnlyExactMatches.UseVisualStyleBackColor = true;
      // 
      // PathStorage
      // 
      this.PathStorage.Location = new System.Drawing.Point(166, 14);
      this.PathStorage.Name = "PathStorage";
      this.PathStorage.Size = new System.Drawing.Size(209, 20);
      this.PathStorage.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.PathStorage, resources.GetString("PathStorage.ToolTip"));
      // 
      // SearchSubDirs
      // 
      this.SearchSubDirs.AutoSize = true;
      this.SearchSubDirs.Location = new System.Drawing.Point(21, 68);
      this.SearchSubDirs.Name = "SearchSubDirs";
      this.SearchSubDirs.Size = new System.Drawing.Size(130, 17);
      this.SearchSubDirs.TabIndex = 6;
      this.SearchSubDirs.Text = "Search in Sub Folders";
      this.ToolTip1.SetToolTip(this.SearchSubDirs, "Select this option if you want the search for the movie files \r\nto include sub-fo" +
        "lders of the defined path.\r\n");
      this.SearchSubDirs.UseVisualStyleBackColor = true;
      // 
      // SearchFileName
      // 
      this.SearchFileName.AutoSize = true;
      this.SearchFileName.Location = new System.Drawing.Point(9, 45);
      this.SearchFileName.Name = "SearchFileName";
      this.SearchFileName.Size = new System.Drawing.Size(144, 17);
      this.SearchFileName.TabIndex = 3;
      this.SearchFileName.Text = "Search by Movie\'s Name";
      this.ToolTip1.SetToolTip(this.SearchFileName, "Select this option if you want to search the file in cases it\'s not found \r\nwith " +
        "the \'Item for Storage Info\' field. \r\nThe movie name will be used for file search" +
        ".");
      this.SearchFileName.UseVisualStyleBackColor = true;
      // 
      // ItemSearchFileName
      // 
      this.ItemSearchFileName.FormattingEnabled = true;
      this.ItemSearchFileName.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
      this.ItemSearchFileName.Location = new System.Drawing.Point(166, 43);
      this.ItemSearchFileName.Name = "ItemSearchFileName";
      this.ItemSearchFileName.Size = new System.Drawing.Size(124, 21);
      this.ItemSearchFileName.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.ItemSearchFileName, resources.GetString("ItemSearchFileName.ToolTip"));
      // 
      // AntStorage
      // 
      this.AntStorage.BackColor = System.Drawing.SystemColors.Info;
      this.AntStorage.FormattingEnabled = true;
      this.AntStorage.Location = new System.Drawing.Point(172, 16);
      this.AntStorage.Name = "AntStorage";
      this.AntStorage.Size = new System.Drawing.Size(124, 21);
      this.AntStorage.Sorted = true;
      this.AntStorage.TabIndex = 9;
      this.ToolTip1.SetToolTip(this.AntStorage, resources.GetString("AntStorage.ToolTip"));
      // 
      // AntIdentItem
      // 
      this.AntIdentItem.FormattingEnabled = true;
      this.AntIdentItem.Location = new System.Drawing.Point(172, 140);
      this.AntIdentItem.Name = "AntIdentItem";
      this.AntIdentItem.Size = new System.Drawing.Size(124, 21);
      this.AntIdentItem.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.AntIdentItem, resources.GetString("AntIdentItem.ToolTip"));
      // 
      // groupBox_Security
      // 
      this.groupBox_Security.Controls.Add(this.label16);
      this.groupBox_Security.Controls.Add(this.label15);
      this.groupBox_Security.Controls.Add(this.Rpt_Dwp);
      this.groupBox_Security.Controls.Add(this.Dwp);
      this.groupBox_Security.Location = new System.Drawing.Point(451, 151);
      this.groupBox_Security.Name = "groupBox_Security";
      this.groupBox_Security.Size = new System.Drawing.Size(286, 77);
      this.groupBox_Security.TabIndex = 7;
      this.groupBox_Security.TabStop = false;
      this.groupBox_Security.Text = "Security - Password to protect the config / DB";
      this.ToolTip1.SetToolTip(this.groupBox_Security, resources.GetString("groupBox_Security.ToolTip"));
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(12, 50);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(42, 13);
      this.label16.TabIndex = 2;
      this.label16.Text = "Repeat";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(12, 24);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(53, 13);
      this.label15.TabIndex = 0;
      this.label15.Text = "Password";
      // 
      // Rpt_Dwp
      // 
      this.Rpt_Dwp.Location = new System.Drawing.Point(93, 47);
      this.Rpt_Dwp.Name = "Rpt_Dwp";
      this.Rpt_Dwp.Size = new System.Drawing.Size(140, 20);
      this.Rpt_Dwp.TabIndex = 3;
      this.Rpt_Dwp.UseSystemPasswordChar = true;
      // 
      // Dwp
      // 
      this.Dwp.Location = new System.Drawing.Point(93, 21);
      this.Dwp.Name = "Dwp";
      this.Dwp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.Dwp.Size = new System.Drawing.Size(140, 20);
      this.Dwp.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.Dwp, "Enter a password here if you wish to restrict access (from children, for example)" +
        " \r\nto this particular configuration.  \r\nThe password must be repeated in the sec" +
        "ond field.\r\n");
      this.Dwp.UseSystemPasswordChar = true;
      // 
      // CatalogType
      // 
      this.CatalogType.BackColor = System.Drawing.SystemColors.Info;
      this.CatalogType.FormattingEnabled = true;
      this.CatalogType.Items.AddRange(new object[] {
            "Ant Movie Catalog (V3.5.1.2)",
            "DVD Profiler (V3.7.2)",
            "Movie Collector (V7.1.4)",
            "MyMovies (V3.18)",
            "Eax Movie Catalog (2.5.0)",
            "Eax Movie Catalog (3.0.9 b5)",
            "PVD - Personal Video Database (0.9.9.21)",
            "eXtreme Movie Manager (V7.1.1.1)",
            "XBMC (V10.0)",
            "MovingPicturesXML (V1.2 process plugin)",
            "Ant Movie Catalog Xtended (V4.1)",
            "XBMC nfo reader",
            "Mediabrowser 3"});
      this.CatalogType.Location = new System.Drawing.Point(533, 14);
      this.CatalogType.Name = "CatalogType";
      this.CatalogType.Size = new System.Drawing.Size(194, 21);
      this.CatalogType.TabIndex = 4;
      this.ToolTip1.SetToolTip(this.CatalogType, resources.GetString("CatalogType.ToolTip"));
      this.CatalogType.SelectedIndexChanged += new System.EventHandler(this.CatalogType_SelectedIndexChanged);
      // 
      // groupBox_TitleOrder
      // 
      this.groupBox_TitleOrder.Controls.Add(this.label20);
      this.groupBox_TitleOrder.Controls.Add(this.AntSTitle);
      this.groupBox_TitleOrder.Controls.Add(this.label17);
      this.groupBox_TitleOrder.Controls.Add(this.TitleDelim);
      this.groupBox_TitleOrder.Controls.Add(this.label9);
      this.groupBox_TitleOrder.Controls.Add(this.label8);
      this.groupBox_TitleOrder.Controls.Add(this.AntTitle2);
      this.groupBox_TitleOrder.Controls.Add(this.AntTitle1);
      this.groupBox_TitleOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox_TitleOrder.Location = new System.Drawing.Point(7, 44);
      this.groupBox_TitleOrder.Name = "groupBox_TitleOrder";
      this.groupBox_TitleOrder.Size = new System.Drawing.Size(438, 101);
      this.groupBox_TitleOrder.TabIndex = 5;
      this.groupBox_TitleOrder.TabStop = false;
      this.groupBox_TitleOrder.Text = "Title Order";
      this.ToolTip1.SetToolTip(this.groupBox_TitleOrder, resources.GetString("groupBox_TitleOrder.ToolTip"));
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Location = new System.Drawing.Point(24, 73);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(46, 13);
      this.label20.TabIndex = 4;
      this.label20.Text = "SortTitle";
      // 
      // AntSTitle
      // 
      this.AntSTitle.FormattingEnabled = true;
      this.AntSTitle.Location = new System.Drawing.Point(172, 70);
      this.AntSTitle.Name = "AntSTitle";
      this.AntSTitle.Size = new System.Drawing.Size(124, 21);
      this.AntSTitle.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.AntSTitle, "Select the ANT database field that you want to be used in the ‘Sort by name’ fiel" +
        "d in the MediaPortal menu.");
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(309, 13);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(84, 26);
      this.label17.TabIndex = 6;
      this.label17.Text = "Group Hierarchy\r\nSeparator";
      this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.ToolTip1.SetToolTip(this.label17, resources.GetString("label17.ToolTip"));
      // 
      // TitleDelim
      // 
      this.TitleDelim.Location = new System.Drawing.Point(399, 17);
      this.TitleDelim.MaxLength = 1;
      this.TitleDelim.Name = "TitleDelim";
      this.TitleDelim.Size = new System.Drawing.Size(20, 20);
      this.TitleDelim.TabIndex = 7;
      this.ToolTip1.SetToolTip(this.TitleDelim, resources.GetString("TitleDelim.ToolTip"));
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(24, 47);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(78, 13);
      this.label9.TabIndex = 2;
      this.label9.Text = "SecondaryTitle";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(24, 22);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(62, 13);
      this.label8.TabIndex = 0;
      this.label8.Text = "Master Title";
      // 
      // AntTitle2
      // 
      this.AntTitle2.FormattingEnabled = true;
      this.AntTitle2.Location = new System.Drawing.Point(172, 43);
      this.AntTitle2.Name = "AntTitle2";
      this.AntTitle2.Size = new System.Drawing.Size(124, 21);
      this.AntTitle2.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.AntTitle2, "Select the ANT database field that you want to be displayed as the ‘Alternate Tit" +
        "le’.");
      // 
      // AntTitle1
      // 
      this.AntTitle1.BackColor = System.Drawing.SystemColors.Info;
      this.AntTitle1.FormattingEnabled = true;
      this.AntTitle1.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
      this.AntTitle1.Location = new System.Drawing.Point(172, 16);
      this.AntTitle1.Name = "AntTitle1";
      this.AntTitle1.Size = new System.Drawing.Size(124, 21);
      this.AntTitle1.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.AntTitle1, resources.GetString("AntTitle1.ToolTip"));
      // 
      // MesFilmsCat
      // 
      this.MesFilmsCat.BackColor = System.Drawing.SystemColors.Info;
      this.MesFilmsCat.Location = new System.Drawing.Point(116, 15);
      this.MesFilmsCat.Name = "MesFilmsCat";
      this.MesFilmsCat.Size = new System.Drawing.Size(272, 20);
      this.MesFilmsCat.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.MesFilmsCat, "Enter the full path and name of your AMC xml database file.\r\nYou can use the brow" +
        "se button to find the file");
      this.MesFilmsCat.TextChanged += new System.EventHandler(this.MesFilmsCat_TextChanged);
      this.MesFilmsCat.Leave += new System.EventHandler(this.MesFilmsCat_Leave);
      // 
      // butNew
      // 
      this.butNew.Location = new System.Drawing.Point(412, 466);
      this.butNew.Name = "butNew";
      this.butNew.Size = new System.Drawing.Size(64, 31);
      this.butNew.TabIndex = 15;
      this.butNew.Text = " New ...";
      this.ToolTip1.SetToolTip(this.butNew, "To create a new MyFilms configuration.");
      this.butNew.UseVisualStyleBackColor = true;
      this.butNew.Click += new System.EventHandler(this.butNew_Click);
      // 
      // groupBoxExtendedFieldHandling
      // 
      this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldWriter);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldCertification);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldTags);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddWriter);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldTagline);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddTagline);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddCertification);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.label54);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddTags);
      this.groupBoxExtendedFieldHandling.Controls.Add(this.label53);
      this.groupBoxExtendedFieldHandling.Location = new System.Drawing.Point(19, 17);
      this.groupBoxExtendedFieldHandling.Name = "groupBoxExtendedFieldHandling";
      this.groupBoxExtendedFieldHandling.Size = new System.Drawing.Size(355, 197);
      this.groupBoxExtendedFieldHandling.TabIndex = 0;
      this.groupBoxExtendedFieldHandling.TabStop = false;
      this.groupBoxExtendedFieldHandling.Text = "External Catalog Import Options ...";
      this.ToolTip1.SetToolTip(this.groupBoxExtendedFieldHandling, resources.GetString("groupBoxExtendedFieldHandling.ToolTip"));
      // 
      // ECMergeDestinationFieldWriter
      // 
      this.ECMergeDestinationFieldWriter.FormattingEnabled = true;
      this.ECMergeDestinationFieldWriter.Items.AddRange(new object[] {
            "Producer",
            "Director",
            "Borrower"});
      this.ECMergeDestinationFieldWriter.Location = new System.Drawing.Point(192, 138);
      this.ECMergeDestinationFieldWriter.Name = "ECMergeDestinationFieldWriter";
      this.ECMergeDestinationFieldWriter.Size = new System.Drawing.Size(134, 21);
      this.ECMergeDestinationFieldWriter.TabIndex = 9;
      this.ECMergeDestinationFieldWriter.Visible = false;
      // 
      // ECMergeDestinationFieldCertification
      // 
      this.ECMergeDestinationFieldCertification.FormattingEnabled = true;
      this.ECMergeDestinationFieldCertification.Items.AddRange(new object[] {
            "Description",
            "Comments"});
      this.ECMergeDestinationFieldCertification.Location = new System.Drawing.Point(192, 108);
      this.ECMergeDestinationFieldCertification.Name = "ECMergeDestinationFieldCertification";
      this.ECMergeDestinationFieldCertification.Size = new System.Drawing.Size(134, 21);
      this.ECMergeDestinationFieldCertification.TabIndex = 7;
      // 
      // ECMergeDestinationFieldTags
      // 
      this.ECMergeDestinationFieldTags.FormattingEnabled = true;
      this.ECMergeDestinationFieldTags.Items.AddRange(new object[] {
            "Description",
            "Comments"});
      this.ECMergeDestinationFieldTags.Location = new System.Drawing.Point(192, 78);
      this.ECMergeDestinationFieldTags.Name = "ECMergeDestinationFieldTags";
      this.ECMergeDestinationFieldTags.Size = new System.Drawing.Size(134, 21);
      this.ECMergeDestinationFieldTags.TabIndex = 5;
      // 
      // chkAddWriter
      // 
      this.chkAddWriter.AutoSize = true;
      this.chkAddWriter.Location = new System.Drawing.Point(39, 140);
      this.chkAddWriter.Name = "chkAddWriter";
      this.chkAddWriter.Size = new System.Drawing.Size(88, 17);
      this.chkAddWriter.TabIndex = 8;
      this.chkAddWriter.Text = "Add Writer ...";
      this.chkAddWriter.UseVisualStyleBackColor = true;
      this.chkAddWriter.Visible = false;
      // 
      // ECMergeDestinationFieldTagline
      // 
      this.ECMergeDestinationFieldTagline.FormattingEnabled = true;
      this.ECMergeDestinationFieldTagline.Items.AddRange(new object[] {
            "Description",
            "Comments"});
      this.ECMergeDestinationFieldTagline.Location = new System.Drawing.Point(192, 49);
      this.ECMergeDestinationFieldTagline.Name = "ECMergeDestinationFieldTagline";
      this.ECMergeDestinationFieldTagline.Size = new System.Drawing.Size(134, 21);
      this.ECMergeDestinationFieldTagline.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.ECMergeDestinationFieldTagline, "Choose a MyFilms field, where you want to add data from \r\nexternal catalog fields" +
        ".");
      // 
      // chkAddTagline
      // 
      this.chkAddTagline.AutoSize = true;
      this.chkAddTagline.Location = new System.Drawing.Point(39, 51);
      this.chkAddTagline.Name = "chkAddTagline";
      this.chkAddTagline.Size = new System.Drawing.Size(95, 17);
      this.chkAddTagline.TabIndex = 2;
      this.chkAddTagline.Text = "Add Tagline ...";
      this.chkAddTagline.UseVisualStyleBackColor = true;
      // 
      // chkAddCertification
      // 
      this.chkAddCertification.AutoSize = true;
      this.chkAddCertification.Location = new System.Drawing.Point(39, 110);
      this.chkAddCertification.Name = "chkAddCertification";
      this.chkAddCertification.Size = new System.Drawing.Size(115, 17);
      this.chkAddCertification.TabIndex = 6;
      this.chkAddCertification.Text = "Add Certification ...";
      this.chkAddCertification.UseVisualStyleBackColor = true;
      // 
      // label54
      // 
      this.label54.AutoSize = true;
      this.label54.Location = new System.Drawing.Point(194, 21);
      this.label54.Name = "label54";
      this.label54.Size = new System.Drawing.Size(125, 13);
      this.label54.TabIndex = 1;
      this.label54.Text = "MyFilms Destination Field";
      // 
      // chkAddTags
      // 
      this.chkAddTags.AutoSize = true;
      this.chkAddTags.Location = new System.Drawing.Point(39, 80);
      this.chkAddTags.Name = "chkAddTags";
      this.chkAddTags.Size = new System.Drawing.Size(84, 17);
      this.chkAddTags.TabIndex = 4;
      this.chkAddTags.Text = "Add Tags ...";
      this.chkAddTags.UseVisualStyleBackColor = true;
      // 
      // label53
      // 
      this.label53.AutoSize = true;
      this.label53.Location = new System.Drawing.Point(16, 21);
      this.label53.Name = "label53";
      this.label53.Size = new System.Drawing.Size(146, 13);
      this.label53.TabIndex = 0;
      this.label53.Text = "External Catalog Source Field";
      // 
      // btnLaunchAMCglobal
      // 
      this.btnLaunchAMCglobal.Image = ((System.Drawing.Image)(resources.GetObject("btnLaunchAMCglobal.Image")));
      this.btnLaunchAMCglobal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnLaunchAMCglobal.Location = new System.Drawing.Point(99, 466);
      this.btnLaunchAMCglobal.Name = "btnLaunchAMCglobal";
      this.btnLaunchAMCglobal.Size = new System.Drawing.Size(72, 31);
      this.btnLaunchAMCglobal.TabIndex = 13;
      this.btnLaunchAMCglobal.Text = "Import ";
      this.btnLaunchAMCglobal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.ToolTip1.SetToolTip(this.btnLaunchAMCglobal, resources.GetString("btnLaunchAMCglobal.ToolTip"));
      this.btnLaunchAMCglobal.UseVisualStyleBackColor = true;
      this.btnLaunchAMCglobal.Click += new System.EventHandler(this.btnLaunchAMCglobal_Click);
      // 
      // SPicture
      // 
      this.SPicture.BackColor = System.Drawing.Color.SteelBlue;
      this.SPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.SPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.SPicture.Enabled = false;
      this.SPicture.Location = new System.Drawing.Point(498, 272);
      this.SPicture.Name = "SPicture";
      this.SPicture.Size = new System.Drawing.Size(79, 58);
      this.SPicture.TabIndex = 85;
      this.SPicture.TabStop = false;
      this.ToolTip1.SetToolTip(this.SPicture, "Click here to open file dialog and select image for logo");
      this.SPicture.Click += new System.EventHandler(this.pictureBox_Click);
      // 
      // groupBox_DVDprofiler
      // 
      this.groupBox_DVDprofiler.Controls.Add(this.groupBox13);
      this.groupBox_DVDprofiler.Controls.Add(this.groupBox9);
      this.groupBox_DVDprofiler.Location = new System.Drawing.Point(392, 17);
      this.groupBox_DVDprofiler.Name = "groupBox_DVDprofiler";
      this.groupBox_DVDprofiler.Size = new System.Drawing.Size(335, 197);
      this.groupBox_DVDprofiler.TabIndex = 1;
      this.groupBox_DVDprofiler.TabStop = false;
      this.groupBox_DVDprofiler.Text = "DVDProfiler";
      this.ToolTip1.SetToolTip(this.groupBox_DVDprofiler, "Those setting are kept to keep backward compatibility to older MyFilms versions.\r" +
        "\nThey only apply, if the catalog type you selected is \"DVDProfiler\".");
      // 
      // groupBox13
      // 
      this.groupBox13.Controls.Add(this.chkDVDprofilerOnlyFile);
      this.groupBox13.Location = new System.Drawing.Point(13, 19);
      this.groupBox13.Name = "groupBox13";
      this.groupBox13.Size = new System.Drawing.Size(307, 49);
      this.groupBox13.TabIndex = 0;
      this.groupBox13.TabStop = false;
      this.groupBox13.Text = "Map Notes field to Source";
      // 
      // chkDVDprofilerOnlyFile
      // 
      this.chkDVDprofilerOnlyFile.AutoSize = true;
      this.chkDVDprofilerOnlyFile.Location = new System.Drawing.Point(24, 21);
      this.chkDVDprofilerOnlyFile.Name = "chkDVDprofilerOnlyFile";
      this.chkDVDprofilerOnlyFile.Size = new System.Drawing.Size(221, 17);
      this.chkDVDprofilerOnlyFile.TabIndex = 0;
      this.chkDVDprofilerOnlyFile.Text = "Notes Field contains only Movie Filename";
      this.chkDVDprofilerOnlyFile.UseVisualStyleBackColor = true;
      // 
      // buttonDeleteTmpCatalog
      // 
      this.buttonDeleteTmpCatalog.Location = new System.Drawing.Point(29, 302);
      this.buttonDeleteTmpCatalog.Name = "buttonDeleteTmpCatalog";
      this.buttonDeleteTmpCatalog.Size = new System.Drawing.Size(152, 27);
      this.buttonDeleteTmpCatalog.TabIndex = 4;
      this.buttonDeleteTmpCatalog.Text = "Delete tmp Catalog";
      this.ToolTip1.SetToolTip(this.buttonDeleteTmpCatalog, resources.GetString("buttonDeleteTmpCatalog.ToolTip"));
      this.buttonDeleteTmpCatalog.UseVisualStyleBackColor = true;
      this.buttonDeleteTmpCatalog.Click += new System.EventHandler(this.buttonDeleteTmpCatalog_Click);
      // 
      // buttonOpenTmpFileAMC
      // 
      this.buttonOpenTmpFileAMC.Location = new System.Drawing.Point(29, 236);
      this.buttonOpenTmpFileAMC.Name = "buttonOpenTmpFileAMC";
      this.buttonOpenTmpFileAMC.Size = new System.Drawing.Size(152, 27);
      this.buttonOpenTmpFileAMC.TabIndex = 2;
      this.buttonOpenTmpFileAMC.Text = "Open tmp Catalog in AMC";
      this.ToolTip1.SetToolTip(this.buttonOpenTmpFileAMC, resources.GetString("buttonOpenTmpFileAMC.ToolTip"));
      this.buttonOpenTmpFileAMC.UseVisualStyleBackColor = true;
      this.buttonOpenTmpFileAMC.Click += new System.EventHandler(this.buttonOpenTmpFileAMC_Click);
      // 
      // AMCConfigView
      // 
      this.AMCConfigView.AllowColumnReorder = true;
      this.AMCConfigView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Option,
            this.Value});
      this.AMCConfigView.FullRowSelect = true;
      this.AMCConfigView.GridLines = true;
      this.AMCConfigView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.AMCConfigView.Location = new System.Drawing.Point(6, 23);
      this.AMCConfigView.MultiSelect = false;
      this.AMCConfigView.Name = "AMCConfigView";
      this.AMCConfigView.Size = new System.Drawing.Size(317, 86);
      this.AMCConfigView.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.AMCConfigView, resources.GetString("AMCConfigView.ToolTip"));
      this.AMCConfigView.UseCompatibleStateImageBehavior = false;
      this.AMCConfigView.View = System.Windows.Forms.View.Details;
      // 
      // Option
      // 
      this.Option.Text = "Option";
      this.Option.Width = 82;
      // 
      // Value
      // 
      this.Value.Text = "Value";
      this.Value.Width = 515;
      // 
      // btnHyperLinkParamGen
      // 
      this.btnHyperLinkParamGen.Location = new System.Drawing.Point(668, 40);
      this.btnHyperLinkParamGen.Name = "btnHyperLinkParamGen";
      this.btnHyperLinkParamGen.Size = new System.Drawing.Size(75, 23);
      this.btnHyperLinkParamGen.TabIndex = 10;
      this.btnHyperLinkParamGen.Text = "StartParams";
      this.ToolTip1.SetToolTip(this.btnHyperLinkParamGen, resources.GetString("btnHyperLinkParamGen.ToolTip"));
      this.btnHyperLinkParamGen.UseVisualStyleBackColor = true;
      this.btnHyperLinkParamGen.Click += new System.EventHandler(this.btnHyperLinkParamGen_Click);
      // 
      // groupBox_GrabberOptions
      // 
      this.groupBox_GrabberOptions.Controls.Add(this.lblFilterGrabberScripts);
      this.groupBox_GrabberOptions.Controls.Add(this.ItemSearchGrabberScriptsFilter);
      this.groupBox_GrabberOptions.Controls.Add(this.txtGrabberDisplay);
      this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_Always);
      this.groupBox_GrabberOptions.Controls.Add(this.btnEditScript);
      this.groupBox_GrabberOptions.Controls.Add(this.groupBox6);
      this.groupBox_GrabberOptions.Controls.Add(this.lblSearchGrabberName);
      this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_ChooseScript);
      this.groupBox_GrabberOptions.Controls.Add(this.ItemSearchGrabberName);
      this.groupBox_GrabberOptions.Controls.Add(this.btnGrabber);
      this.groupBox_GrabberOptions.Controls.Add(this.txtGrabber);
      this.groupBox_GrabberOptions.Controls.Add(this.label27);
      this.groupBox_GrabberOptions.Location = new System.Drawing.Point(6, 211);
      this.groupBox_GrabberOptions.Name = "groupBox_GrabberOptions";
      this.groupBox_GrabberOptions.Size = new System.Drawing.Size(735, 133);
      this.groupBox_GrabberOptions.TabIndex = 3;
      this.groupBox_GrabberOptions.TabStop = false;
      this.groupBox_GrabberOptions.Text = "MyFilms Internal Internet Grabbing";
      this.ToolTip1.SetToolTip(this.groupBox_GrabberOptions, resources.GetString("groupBox_GrabberOptions.ToolTip"));
      // 
      // lblFilterGrabberScripts
      // 
      this.lblFilterGrabberScripts.AutoSize = true;
      this.lblFilterGrabberScripts.Location = new System.Drawing.Point(16, 51);
      this.lblFilterGrabberScripts.Name = "lblFilterGrabberScripts";
      this.lblFilterGrabberScripts.Size = new System.Drawing.Size(105, 13);
      this.lblFilterGrabberScripts.TabIndex = 4;
      this.lblFilterGrabberScripts.Text = "Filter Grabber Scripts";
      // 
      // ItemSearchGrabberScriptsFilter
      // 
      this.ItemSearchGrabberScriptsFilter.Location = new System.Drawing.Point(145, 48);
      this.ItemSearchGrabberScriptsFilter.Name = "ItemSearchGrabberScriptsFilter";
      this.ItemSearchGrabberScriptsFilter.Size = new System.Drawing.Size(138, 20);
      this.ItemSearchGrabberScriptsFilter.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.ItemSearchGrabberScriptsFilter, resources.GetString("ItemSearchGrabberScriptsFilter.ToolTip"));
      // 
      // txtGrabberDisplay
      // 
      this.txtGrabberDisplay.Location = new System.Drawing.Point(145, 22);
      this.txtGrabberDisplay.Name = "txtGrabberDisplay";
      this.txtGrabberDisplay.ReadOnly = true;
      this.txtGrabberDisplay.Size = new System.Drawing.Size(100, 20);
      this.txtGrabberDisplay.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.txtGrabberDisplay, resources.GetString("txtGrabberDisplay.ToolTip"));
      // 
      // chkGrabber_Always
      // 
      this.chkGrabber_Always.AutoSize = true;
      this.chkGrabber_Always.Location = new System.Drawing.Point(19, 74);
      this.chkGrabber_Always.Name = "chkGrabber_Always";
      this.chkGrabber_Always.Size = new System.Drawing.Size(188, 17);
      this.chkGrabber_Always.TabIndex = 6;
      this.chkGrabber_Always.Text = "try to find best match automatically";
      this.ToolTip1.SetToolTip(this.chkGrabber_Always, resources.GetString("chkGrabber_Always.ToolTip"));
      this.chkGrabber_Always.UseVisualStyleBackColor = true;
      // 
      // btnEditScript
      // 
      this.btnEditScript.Location = new System.Drawing.Point(289, 22);
      this.btnEditScript.Name = "btnEditScript";
      this.btnEditScript.Size = new System.Drawing.Size(95, 20);
      this.btnEditScript.TabIndex = 3;
      this.btnEditScript.Text = "Grabber Options";
      this.ToolTip1.SetToolTip(this.btnEditScript, resources.GetString("btnEditScript.ToolTip"));
      this.btnEditScript.UseVisualStyleBackColor = true;
      this.btnEditScript.Click += new System.EventHandler(this.btnEditScript_Click);
      // 
      // groupBox6
      // 
      this.groupBox6.Controls.Add(this.cbGrabberOverrideGetRoles);
      this.groupBox6.Controls.Add(this.label55);
      this.groupBox6.Controls.Add(this.label51);
      this.groupBox6.Controls.Add(this.cbGrabberOverrideTitleLimit);
      this.groupBox6.Controls.Add(this.label50);
      this.groupBox6.Controls.Add(this.label49);
      this.groupBox6.Controls.Add(this.cbGrabberOverridePersonLimit);
      this.groupBox6.Controls.Add(this.cbGrabberOverrideLanguage);
      this.groupBox6.Location = new System.Drawing.Point(398, 15);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(330, 112);
      this.groupBox6.TabIndex = 10;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "MyFilms Grabber Override Options ...";
      this.ToolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
      // 
      // cbGrabberOverrideGetRoles
      // 
      this.cbGrabberOverrideGetRoles.FormattingEnabled = true;
      this.cbGrabberOverrideGetRoles.Items.AddRange(new object[] {
            "true",
            "false"});
      this.cbGrabberOverrideGetRoles.Location = new System.Drawing.Point(276, 21);
      this.cbGrabberOverrideGetRoles.Name = "cbGrabberOverrideGetRoles";
      this.cbGrabberOverrideGetRoles.Size = new System.Drawing.Size(45, 21);
      this.cbGrabberOverrideGetRoles.TabIndex = 3;
      this.ToolTip1.SetToolTip(this.cbGrabberOverrideGetRoles, "If checked, add Roles to actor infos.\r\nDepends on the grabber script supporting t" +
        "hat option.");
      // 
      // label55
      // 
      this.label55.AutoSize = true;
      this.label55.Location = new System.Drawing.Point(191, 27);
      this.label55.Name = "label55";
      this.label55.Size = new System.Drawing.Size(82, 13);
      this.label55.TabIndex = 2;
      this.label55.Text = "Get Actor Roles";
      // 
      // label51
      // 
      this.label51.AutoSize = true;
      this.label51.Location = new System.Drawing.Point(191, 79);
      this.label51.Name = "label51";
      this.label51.Size = new System.Drawing.Size(66, 13);
      this.label51.TabIndex = 6;
      this.label51.Text = "Limit # Titles";
      // 
      // cbGrabberOverrideTitleLimit
      // 
      this.cbGrabberOverrideTitleLimit.FormattingEnabled = true;
      this.cbGrabberOverrideTitleLimit.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "10",
            "15",
            "20",
            "999"});
      this.cbGrabberOverrideTitleLimit.Location = new System.Drawing.Point(276, 76);
      this.cbGrabberOverrideTitleLimit.Name = "cbGrabberOverrideTitleLimit";
      this.cbGrabberOverrideTitleLimit.Size = new System.Drawing.Size(45, 21);
      this.cbGrabberOverrideTitleLimit.TabIndex = 7;
      this.ToolTip1.SetToolTip(this.cbGrabberOverrideTitleLimit, "Limits the number of translated titles grabbed.\r\nYou may also set a preferred lan" +
        "guage/country \r\nfor grabbing in the override options.");
      // 
      // label50
      // 
      this.label50.AutoSize = true;
      this.label50.Location = new System.Drawing.Point(12, 28);
      this.label50.Name = "label50";
      this.label50.Size = new System.Drawing.Size(161, 13);
      this.label50.TabIndex = 0;
      this.label50.Text = "Preferred Languages / Countries";
      // 
      // label49
      // 
      this.label49.AutoSize = true;
      this.label49.Location = new System.Drawing.Point(191, 52);
      this.label49.Name = "label49";
      this.label49.Size = new System.Drawing.Size(79, 13);
      this.label49.TabIndex = 4;
      this.label49.Text = "Limit # Persons";
      // 
      // cbGrabberOverridePersonLimit
      // 
      this.cbGrabberOverridePersonLimit.FormattingEnabled = true;
      this.cbGrabberOverridePersonLimit.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "10",
            "15",
            "20",
            "999"});
      this.cbGrabberOverridePersonLimit.Location = new System.Drawing.Point(276, 48);
      this.cbGrabberOverridePersonLimit.Name = "cbGrabberOverridePersonLimit";
      this.cbGrabberOverridePersonLimit.Size = new System.Drawing.Size(45, 21);
      this.cbGrabberOverridePersonLimit.TabIndex = 5;
      this.ToolTip1.SetToolTip(this.cbGrabberOverridePersonLimit, "Limits the number of person names grabbed.\r\nThis settings applies to all fields t" +
        "hat grab person names, \r\nlike actors, producers, writers, directors.");
      // 
      // cbGrabberOverrideLanguage
      // 
      this.cbGrabberOverrideLanguage.FormattingEnabled = true;
      this.cbGrabberOverrideLanguage.Items.AddRange(new object[] {
            "Argentina",
            "Australia",
            "Austria",
            "Belgium",
            "Brazil",
            "Canada",
            "Chile",
            "Croatia",
            "Czech Republic",
            "Denmark",
            "Estonia",
            "Finland",
            "France",
            "Germany",
            "Greece",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Ireland",
            "Israel",
            "Italy",
            "Japan",
            "Malaysia",
            "Mexico",
            "Netherlands",
            "New Zealand",
            "Norway",
            "Peru",
            "Philippines",
            "Poland",
            "Portugal",
            "Romania",
            "Russia",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "South Africa",
            "South Korea",
            "Spain",
            "Sweden",
            "Switzerland",
            "Turkey",
            "UK",
            "Uruguay",
            "USA"});
      this.cbGrabberOverrideLanguage.Location = new System.Drawing.Point(11, 49);
      this.cbGrabberOverrideLanguage.Name = "cbGrabberOverrideLanguage";
      this.cbGrabberOverrideLanguage.Size = new System.Drawing.Size(167, 21);
      this.cbGrabberOverrideLanguage.Sorted = true;
      this.cbGrabberOverrideLanguage.TabIndex = 1;
      this.ToolTip1.SetToolTip(this.cbGrabberOverrideLanguage, resources.GetString("cbGrabberOverrideLanguage.ToolTip"));
      // 
      // lblSearchGrabberName
      // 
      this.lblSearchGrabberName.AutoSize = true;
      this.lblSearchGrabberName.Location = new System.Drawing.Point(16, 105);
      this.lblSearchGrabberName.Name = "lblSearchGrabberName";
      this.lblSearchGrabberName.Size = new System.Drawing.Size(124, 13);
      this.lblSearchGrabberName.TabIndex = 8;
      this.lblSearchGrabberName.Text = "Title for Internet Updates";
      // 
      // chkGrabber_ChooseScript
      // 
      this.chkGrabber_ChooseScript.AutoSize = true;
      this.chkGrabber_ChooseScript.Location = new System.Drawing.Point(226, 74);
      this.chkGrabber_ChooseScript.Name = "chkGrabber_ChooseScript";
      this.chkGrabber_ChooseScript.Size = new System.Drawing.Size(158, 17);
      this.chkGrabber_ChooseScript.TabIndex = 7;
      this.chkGrabber_ChooseScript.Text = "don\'t use default script (ask)";
      this.ToolTip1.SetToolTip(this.chkGrabber_ChooseScript, resources.GetString("chkGrabber_ChooseScript.ToolTip"));
      this.chkGrabber_ChooseScript.UseVisualStyleBackColor = true;
      // 
      // ItemSearchGrabberName
      // 
      this.ItemSearchGrabberName.FormattingEnabled = true;
      this.ItemSearchGrabberName.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
      this.ItemSearchGrabberName.Location = new System.Drawing.Point(145, 102);
      this.ItemSearchGrabberName.Name = "ItemSearchGrabberName";
      this.ItemSearchGrabberName.Size = new System.Drawing.Size(138, 21);
      this.ItemSearchGrabberName.TabIndex = 9;
      this.ToolTip1.SetToolTip(this.ItemSearchGrabberName, "The title that you want to use for internet data searches.\r\nMyFilms will offer yo" +
        "u the other titles for a \"re\"search too.");
      // 
      // btnGrabber
      // 
      this.btnGrabber.Location = new System.Drawing.Point(251, 22);
      this.btnGrabber.Name = "btnGrabber";
      this.btnGrabber.Size = new System.Drawing.Size(32, 20);
      this.btnGrabber.TabIndex = 2;
      this.btnGrabber.Text = "...";
      this.ToolTip1.SetToolTip(this.btnGrabber, "Browse and choose the script you want MyFilms \r\nto use as default.");
      this.btnGrabber.UseVisualStyleBackColor = true;
      this.btnGrabber.Click += new System.EventHandler(this.btnGrabber_Click);
      // 
      // txtGrabber
      // 
      this.txtGrabber.Location = new System.Drawing.Point(145, 22);
      this.txtGrabber.Name = "txtGrabber";
      this.txtGrabber.Size = new System.Drawing.Size(68, 20);
      this.txtGrabber.TabIndex = 54;
      this.ToolTip1.SetToolTip(this.txtGrabber, resources.GetString("txtGrabber.ToolTip"));
      this.txtGrabber.Visible = false;
      this.txtGrabber.TextChanged += new System.EventHandler(this.txtGrabber_TextChanged);
      // 
      // label27
      // 
      this.label27.AutoSize = true;
      this.label27.Location = new System.Drawing.Point(16, 25);
      this.label27.Name = "label27";
      this.label27.Size = new System.Drawing.Size(112, 13);
      this.label27.TabIndex = 0;
      this.label27.Text = "Default Grabber Script";
      // 
      // btnFirstTimeSetup
      // 
      this.btnFirstTimeSetup.Location = new System.Drawing.Point(195, 466);
      this.btnFirstTimeSetup.Name = "btnFirstTimeSetup";
      this.btnFirstTimeSetup.Size = new System.Drawing.Size(107, 31);
      this.btnFirstTimeSetup.TabIndex = 14;
      this.btnFirstTimeSetup.Text = " Setup Wizard ...";
      this.ToolTip1.SetToolTip(this.btnFirstTimeSetup, resources.GetString("btnFirstTimeSetup.ToolTip"));
      this.btnFirstTimeSetup.UseVisualStyleBackColor = true;
      this.btnFirstTimeSetup.Click += new System.EventHandler(this.btnFirstTimeSetup_Click);
      // 
      // buttonWikiHelp
      // 
      this.buttonWikiHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonWikiHelp.ForeColor = System.Drawing.SystemColors.HotTrack;
      this.buttonWikiHelp.Location = new System.Drawing.Point(18, 466);
      this.buttonWikiHelp.Name = "buttonWikiHelp";
      this.buttonWikiHelp.Size = new System.Drawing.Size(64, 31);
      this.buttonWikiHelp.TabIndex = 12;
      this.buttonWikiHelp.Text = "Wiki Help";
      this.ToolTip1.SetToolTip(this.buttonWikiHelp, "Opens the MyFilms Wiki in your browser...");
      this.buttonWikiHelp.UseVisualStyleBackColor = true;
      this.buttonWikiHelp.Click += new System.EventHandler(this.buttonWikiHelp_Click);
      // 
      // btnServerSync
      // 
      this.btnServerSync.Location = new System.Drawing.Point(376, 12);
      this.btnServerSync.Name = "btnServerSync";
      this.btnServerSync.Size = new System.Drawing.Size(84, 23);
      this.btnServerSync.TabIndex = 4;
      this.btnServerSync.Text = "Central Config";
      this.ToolTip1.SetToolTip(this.btnServerSync, resources.GetString("btnServerSync.ToolTip"));
      this.btnServerSync.UseVisualStyleBackColor = true;
      this.btnServerSync.Click += new System.EventHandler(this.btnFirstTimeSetupSyncClient_Click);
      // 
      // groupBox_UpdateGrabberScripts
      // 
      this.groupBox_UpdateGrabberScripts.Controls.Add(this.button_DeleteBackupScripts);
      this.groupBox_UpdateGrabberScripts.Controls.Add(this.label_UpdateGrabberScriptsInstructions);
      this.groupBox_UpdateGrabberScripts.Controls.Add(this.textBoxUpdateGrabberScripts);
      this.groupBox_UpdateGrabberScripts.Controls.Add(this.progressBarUpdateGrabberScripts);
      this.groupBox_UpdateGrabberScripts.Controls.Add(this.buttonUpdateGrabberScripts);
      this.groupBox_UpdateGrabberScripts.Location = new System.Drawing.Point(6, 263);
      this.groupBox_UpdateGrabberScripts.Name = "groupBox_UpdateGrabberScripts";
      this.groupBox_UpdateGrabberScripts.Size = new System.Drawing.Size(735, 85);
      this.groupBox_UpdateGrabberScripts.TabIndex = 1;
      this.groupBox_UpdateGrabberScripts.TabStop = false;
      this.groupBox_UpdateGrabberScripts.Text = "Update Grabber Scripts ...";
      this.ToolTip1.SetToolTip(this.groupBox_UpdateGrabberScripts, resources.GetString("groupBox_UpdateGrabberScripts.ToolTip"));
      // 
      // button_DeleteBackupScripts
      // 
      this.button_DeleteBackupScripts.Location = new System.Drawing.Point(555, 13);
      this.button_DeleteBackupScripts.Name = "button_DeleteBackupScripts";
      this.button_DeleteBackupScripts.Size = new System.Drawing.Size(144, 25);
      this.button_DeleteBackupScripts.TabIndex = 2;
      this.button_DeleteBackupScripts.Text = "Delete Script Backups";
      this.button_DeleteBackupScripts.UseVisualStyleBackColor = true;
      this.button_DeleteBackupScripts.Click += new System.EventHandler(this.button_DeleteBackupScripts_Click);
      // 
      // label_UpdateGrabberScriptsInstructions
      // 
      this.label_UpdateGrabberScriptsInstructions.AutoSize = true;
      this.label_UpdateGrabberScriptsInstructions.Location = new System.Drawing.Point(21, 32);
      this.label_UpdateGrabberScriptsInstructions.Name = "label_UpdateGrabberScriptsInstructions";
      this.label_UpdateGrabberScriptsInstructions.Size = new System.Drawing.Size(267, 26);
      this.label_UpdateGrabberScriptsInstructions.TabIndex = 0;
      this.label_UpdateGrabberScriptsInstructions.Text = "Update will download missing grabber scripts \r\nor update existing ones, if newer " +
    "versions are available.";
      // 
      // textBoxUpdateGrabberScripts
      // 
      this.textBoxUpdateGrabberScripts.Enabled = false;
      this.textBoxUpdateGrabberScripts.Location = new System.Drawing.Point(399, 42);
      this.textBoxUpdateGrabberScripts.Name = "textBoxUpdateGrabberScripts";
      this.textBoxUpdateGrabberScripts.Size = new System.Drawing.Size(300, 20);
      this.textBoxUpdateGrabberScripts.TabIndex = 3;
      // 
      // progressBarUpdateGrabberScripts
      // 
      this.progressBarUpdateGrabberScripts.Location = new System.Drawing.Point(399, 66);
      this.progressBarUpdateGrabberScripts.Name = "progressBarUpdateGrabberScripts";
      this.progressBarUpdateGrabberScripts.Size = new System.Drawing.Size(300, 13);
      this.progressBarUpdateGrabberScripts.TabIndex = 4;
      // 
      // buttonUpdateGrabberScripts
      // 
      this.buttonUpdateGrabberScripts.Location = new System.Drawing.Point(399, 13);
      this.buttonUpdateGrabberScripts.Name = "buttonUpdateGrabberScripts";
      this.buttonUpdateGrabberScripts.Size = new System.Drawing.Size(143, 25);
      this.buttonUpdateGrabberScripts.TabIndex = 1;
      this.buttonUpdateGrabberScripts.Text = "Update Grabber Scripts";
      this.buttonUpdateGrabberScripts.UseVisualStyleBackColor = true;
      this.buttonUpdateGrabberScripts.Click += new System.EventHandler(this.buttonUpdateGrabberScripts_Click);
      // 
      // groupBoxMultiUserState
      // 
      this.groupBoxMultiUserState.Controls.Add(this.btnMUSdeleteUserData);
      this.groupBoxMultiUserState.Controls.Add(this.UserProfileName);
      this.groupBoxMultiUserState.Controls.Add(this.Label_UserProfileName);
      this.groupBoxMultiUserState.Controls.Add(this.chkEnhancedWatchedStatusHandling);
      this.groupBoxMultiUserState.Location = new System.Drawing.Point(7, 131);
      this.groupBoxMultiUserState.Name = "groupBoxMultiUserState";
      this.groupBoxMultiUserState.Size = new System.Drawing.Size(338, 72);
      this.groupBoxMultiUserState.TabIndex = 1;
      this.groupBoxMultiUserState.TabStop = false;
      this.groupBoxMultiUserState.Text = "Multi User States (MUS)";
      this.ToolTip1.SetToolTip(this.groupBoxMultiUserState, resources.GetString("groupBoxMultiUserState.ToolTip"));
      // 
      // Label_UserProfileName
      // 
      this.Label_UserProfileName.AutoSize = true;
      this.Label_UserProfileName.Location = new System.Drawing.Point(28, 37);
      this.Label_UserProfileName.Name = "Label_UserProfileName";
      this.Label_UserProfileName.Size = new System.Drawing.Size(125, 13);
      this.Label_UserProfileName.TabIndex = 2;
      this.Label_UserProfileName.Text = "Active User Profile Name";
      // 
      // chkEnhancedWatchedStatusHandling
      // 
      this.chkEnhancedWatchedStatusHandling.AutoSize = true;
      this.chkEnhancedWatchedStatusHandling.Enabled = false;
      this.chkEnhancedWatchedStatusHandling.Location = new System.Drawing.Point(11, 17);
      this.chkEnhancedWatchedStatusHandling.Name = "chkEnhancedWatchedStatusHandling";
      this.chkEnhancedWatchedStatusHandling.Size = new System.Drawing.Size(112, 17);
      this.chkEnhancedWatchedStatusHandling.TabIndex = 0;
      this.chkEnhancedWatchedStatusHandling.Text = "MUS autoenabled";
      this.chkEnhancedWatchedStatusHandling.UseVisualStyleBackColor = true;
      // 
      // Config_EnablePreload
      // 
      this.Config_EnablePreload.AutoSize = true;
      this.Config_EnablePreload.Location = new System.Drawing.Point(578, 17);
      this.Config_EnablePreload.Name = "Config_EnablePreload";
      this.Config_EnablePreload.Size = new System.Drawing.Size(62, 17);
      this.Config_EnablePreload.TabIndex = 76;
      this.Config_EnablePreload.Text = "Preload";
      this.ToolTip1.SetToolTip(this.Config_EnablePreload, resources.GetString("Config_EnablePreload.ToolTip"));
      this.Config_EnablePreload.UseVisualStyleBackColor = true;
      // 
      // btnTestMode
      // 
      this.btnTestMode.Location = new System.Drawing.Point(320, 466);
      this.btnTestMode.Name = "btnTestMode";
      this.btnTestMode.Size = new System.Drawing.Size(75, 31);
      this.btnTestMode.TabIndex = 77;
      this.btnTestMode.Text = "Test Mode";
      this.ToolTip1.SetToolTip(this.btnTestMode, resources.GetString("btnTestMode.ToolTip"));
      this.btnTestMode.UseVisualStyleBackColor = true;
      this.btnTestMode.Click += new System.EventHandler(this.btnTestMode_Click);
      // 
      // txtCountry
      // 
      this.txtCountry.Location = new System.Drawing.Point(75, 33);
      this.txtCountry.Name = "txtCountry";
      this.txtCountry.Size = new System.Drawing.Size(29, 20);
      this.txtCountry.TabIndex = 86;
      this.ToolTip1.SetToolTip(this.txtCountry, resources.GetString("txtCountry.ToolTip"));
      // 
      // SOp1
      // 
      this.SOp1.Enabled = false;
      this.SOp1.FormattingEnabled = true;
      this.SOp1.Items.AddRange(new object[] {
            "equal",
            "not equal",
            "contains",
            "not contains",
            "greater",
            "lower",
            "filled",
            "not filled",
            "regex",
            "value"});
      this.SOp1.Location = new System.Drawing.Point(291, 276);
      this.SOp1.Name = "SOp1";
      this.SOp1.Size = new System.Drawing.Size(74, 21);
      this.SOp1.TabIndex = 14;
      this.ToolTip1.SetToolTip(this.SOp1, resources.GetString("SOp1.ToolTip"));
      this.SOp1.SelectedIndexChanged += new System.EventHandler(this.SOp1_SelectedIndexChanged);
      // 
      // Tab_Trakt
      // 
      this.Tab_Trakt.Controls.Add(this.groupBoxExternal);
      this.Tab_Trakt.Controls.Add(this.groupBoxTrakt);
      this.Tab_Trakt.Location = new System.Drawing.Point(4, 22);
      this.Tab_Trakt.Name = "Tab_Trakt";
      this.Tab_Trakt.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Trakt.Size = new System.Drawing.Size(747, 354);
      this.Tab_Trakt.TabIndex = 10;
      this.Tab_Trakt.Text = "Trakt & Latest Media";
      this.Tab_Trakt.ToolTipText = "Setup for Trakt user settings";
      this.Tab_Trakt.UseVisualStyleBackColor = true;
      this.Tab_Trakt.Visible = false;
      // 
      // groupBoxExternal
      // 
      this.groupBoxExternal.Controls.Add(this.labelRecentlyAddedAPI);
      this.groupBoxExternal.Controls.Add(this.cbAllowRecentAddedAPI);
      this.groupBoxExternal.Location = new System.Drawing.Point(19, 244);
      this.groupBoxExternal.Name = "groupBoxExternal";
      this.groupBoxExternal.Size = new System.Drawing.Size(698, 89);
      this.groupBoxExternal.TabIndex = 1;
      this.groupBoxExternal.TabStop = false;
      this.groupBoxExternal.Text = "Recentrly Added Media ...";
      // 
      // labelRecentlyAddedAPI
      // 
      this.labelRecentlyAddedAPI.AutoSize = true;
      this.labelRecentlyAddedAPI.Location = new System.Drawing.Point(21, 47);
      this.labelRecentlyAddedAPI.Name = "labelRecentlyAddedAPI";
      this.labelRecentlyAddedAPI.Size = new System.Drawing.Size(279, 26);
      this.labelRecentlyAddedAPI.TabIndex = 1;
      this.labelRecentlyAddedAPI.Text = "You can enable or disable per config, if you want to \r\ninclude the catalog conten" +
    "t to the recently added media...";
      // 
      // groupBoxTrakt
      // 
      this.groupBoxTrakt.Controls.Add(this.linkLabelUsingTraktInMyFilmsWiki);
      this.groupBoxTrakt.Controls.Add(this.linkLabelTraktWiki);
      this.groupBoxTrakt.Controls.Add(this.labelTraktDescription);
      this.groupBoxTrakt.Controls.Add(this.cbAllowTraktSync);
      this.groupBoxTrakt.Controls.Add(this.pictureBox1);
      this.groupBoxTrakt.Controls.Add(this.linkLabelTrakt);
      this.groupBoxTrakt.Location = new System.Drawing.Point(19, 15);
      this.groupBoxTrakt.Name = "groupBoxTrakt";
      this.groupBoxTrakt.Size = new System.Drawing.Size(698, 223);
      this.groupBoxTrakt.TabIndex = 0;
      this.groupBoxTrakt.TabStop = false;
      this.groupBoxTrakt.Text = "Trakt ...";
      // 
      // linkLabelUsingTraktInMyFilmsWiki
      // 
      this.linkLabelUsingTraktInMyFilmsWiki.AutoSize = true;
      this.linkLabelUsingTraktInMyFilmsWiki.Location = new System.Drawing.Point(518, 133);
      this.linkLabelUsingTraktInMyFilmsWiki.Name = "linkLabelUsingTraktInMyFilmsWiki";
      this.linkLabelUsingTraktInMyFilmsWiki.Size = new System.Drawing.Size(113, 13);
      this.linkLabelUsingTraktInMyFilmsWiki.TabIndex = 3;
      this.linkLabelUsingTraktInMyFilmsWiki.TabStop = true;
      this.linkLabelUsingTraktInMyFilmsWiki.Text = "Using Trakt in MyFilms";
      this.linkLabelUsingTraktInMyFilmsWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelUsingTraktInMyFilmsWiki_LinkClicked);
      // 
      // linkLabelTraktWiki
      // 
      this.linkLabelTraktWiki.AutoSize = true;
      this.linkLabelTraktWiki.Location = new System.Drawing.Point(518, 169);
      this.linkLabelTraktWiki.Name = "linkLabelTraktWiki";
      this.linkLabelTraktWiki.Size = new System.Drawing.Size(56, 13);
      this.linkLabelTraktWiki.TabIndex = 4;
      this.linkLabelTraktWiki.TabStop = true;
      this.linkLabelTraktWiki.Text = "Trakt Wiki";
      this.linkLabelTraktWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTraktWiki_LinkClicked);
      // 
      // labelTraktDescription
      // 
      this.labelTraktDescription.AutoSize = true;
      this.labelTraktDescription.Location = new System.Drawing.Point(21, 54);
      this.labelTraktDescription.Name = "labelTraktDescription";
      this.labelTraktDescription.Size = new System.Drawing.Size(407, 195);
      this.labelTraktDescription.TabIndex = 1;
      this.labelTraktDescription.Text = resources.GetString("labelTraktDescription.Text");
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::MyFilmsPlugin.Properties.Resources.trakt;
      this.pictureBox1.Location = new System.Drawing.Point(521, 17);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(52, 50);
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // linkLabelTrakt
      // 
      this.linkLabelTrakt.AutoSize = true;
      this.linkLabelTrakt.Location = new System.Drawing.Point(518, 98);
      this.linkLabelTrakt.Name = "linkLabelTrakt";
      this.linkLabelTrakt.Size = new System.Drawing.Size(74, 13);
      this.linkLabelTrakt.TabIndex = 2;
      this.linkLabelTrakt.TabStop = true;
      this.linkLabelTrakt.Text = "Trakt Website";
      this.linkLabelTrakt.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTrakt_LinkClicked);
      // 
      // Tab_Display
      // 
      this.Tab_Display.AutoScroll = true;
      this.Tab_Display.Controls.Add(this.groupBox_UserItemsDetails);
      this.Tab_Display.Controls.Add(this.groupBox_UserItemsMain);
      this.Tab_Display.Location = new System.Drawing.Point(4, 22);
      this.Tab_Display.Name = "Tab_Display";
      this.Tab_Display.Size = new System.Drawing.Size(747, 354);
      this.Tab_Display.TabIndex = 9;
      this.Tab_Display.Text = "Display";
      this.Tab_Display.ToolTipText = "Setup for user defined display items";
      this.Tab_Display.UseVisualStyleBackColor = true;
      // 
      // Tab_Views
      // 
      this.Tab_Views.Controls.Add(this.groupBox_Separators);
      this.Tab_Views.Controls.Add(this.groupBox_DefaultView);
      this.Tab_Views.Controls.Add(this.groupBoxView);
      this.Tab_Views.Location = new System.Drawing.Point(4, 22);
      this.Tab_Views.Name = "Tab_Views";
      this.Tab_Views.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Views.Size = new System.Drawing.Size(747, 354);
      this.Tab_Views.TabIndex = 1;
      this.Tab_Views.Text = "Views";
      this.Tab_Views.ToolTipText = "Setup for userdefined views and filters";
      this.Tab_Views.UseVisualStyleBackColor = true;
      // 
      // Tab_Trailer
      // 
      this.Tab_Trailer.Controls.Add(this.groupBox24);
      this.Tab_Trailer.Location = new System.Drawing.Point(4, 22);
      this.Tab_Trailer.Name = "Tab_Trailer";
      this.Tab_Trailer.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Trailer.Size = new System.Drawing.Size(747, 354);
      this.Tab_Trailer.TabIndex = 7;
      this.Tab_Trailer.Text = "Trailer";
      this.Tab_Trailer.ToolTipText = "Setup for trailer options like search paths and options for behaviour in GUI";
      this.Tab_Trailer.UseVisualStyleBackColor = true;
      // 
      // Tab_General
      // 
      this.Tab_General.Controls.Add(this.groupBox_AntSelectedEnreg);
      this.Tab_General.Controls.Add(this.groupBox_PreLaunchingCommand);
      this.Tab_General.Controls.Add(this.lblYellowShowRequiredItems);
      this.Tab_General.Controls.Add(this.groupBox_PlayMovieInfos);
      this.Tab_General.Controls.Add(this.groupBox_Security);
      this.Tab_General.Controls.Add(this.label13);
      this.Tab_General.Controls.Add(this.CatalogType);
      this.Tab_General.Controls.Add(this.groupBox_TitleOrder);
      this.Tab_General.Controls.Add(this.ButCat);
      this.Tab_General.Controls.Add(this.MesFilmsCat);
      this.Tab_General.Controls.Add(this.label1);
      this.Tab_General.Location = new System.Drawing.Point(4, 22);
      this.Tab_General.Name = "Tab_General";
      this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_General.Size = new System.Drawing.Size(747, 354);
      this.Tab_General.TabIndex = 0;
      this.Tab_General.Text = "General";
      this.Tab_General.ToolTipText = "Basic configuration like selecting movie catalog, titles to display, etc.";
      this.Tab_General.UseVisualStyleBackColor = true;
      // 
      // lblYellowShowRequiredItems
      // 
      this.lblYellowShowRequiredItems.AutoSize = true;
      this.lblYellowShowRequiredItems.BackColor = System.Drawing.SystemColors.Info;
      this.lblYellowShowRequiredItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblYellowShowRequiredItems.Location = new System.Drawing.Point(466, 323);
      this.lblYellowShowRequiredItems.Name = "lblYellowShowRequiredItems";
      this.lblYellowShowRequiredItems.Size = new System.Drawing.Size(245, 15);
      this.lblYellowShowRequiredItems.TabIndex = 9;
      this.lblYellowShowRequiredItems.Text = "Yellow fields are mandatory for the plugin to work !";
      this.lblYellowShowRequiredItems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // groupBox_PlayMovieInfos
      // 
      this.groupBox_PlayMovieInfos.Controls.Add(this.tbExternalPlayerStartParams);
      this.groupBox_PlayMovieInfos.Controls.Add(this.butExternalPlayer);
      this.groupBox_PlayMovieInfos.Controls.Add(this.tbExternalPlayerPath);
      this.groupBox_PlayMovieInfos.Controls.Add(this.tbExternalPlayerExtensions);
      this.groupBox_PlayMovieInfos.Controls.Add(this.chkScanMediaOnStart);
      this.groupBox_PlayMovieInfos.Controls.Add(this.label23);
      this.groupBox_PlayMovieInfos.Controls.Add(this.groupBoxMoviePathInfos);
      this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentLabel);
      this.groupBox_PlayMovieInfos.Controls.Add(this.label6);
      this.groupBox_PlayMovieInfos.Controls.Add(this.AntStorage);
      this.groupBox_PlayMovieInfos.Controls.Add(this.label4);
      this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentItem);
      this.groupBox_PlayMovieInfos.Location = new System.Drawing.Point(7, 151);
      this.groupBox_PlayMovieInfos.Name = "groupBox_PlayMovieInfos";
      this.groupBox_PlayMovieInfos.Size = new System.Drawing.Size(438, 197);
      this.groupBox_PlayMovieInfos.TabIndex = 6;
      this.groupBox_PlayMovieInfos.TabStop = false;
      this.groupBox_PlayMovieInfos.Text = "Movie Playback Path Configuration";
      // 
      // butExternalPlayer
      // 
      this.butExternalPlayer.Location = new System.Drawing.Point(267, 169);
      this.butExternalPlayer.Name = "butExternalPlayer";
      this.butExternalPlayer.Size = new System.Drawing.Size(29, 20);
      this.butExternalPlayer.TabIndex = 5;
      this.butExternalPlayer.Text = "...";
      this.butExternalPlayer.UseVisualStyleBackColor = true;
      this.butExternalPlayer.Click += new System.EventHandler(this.butExternalPlayer_Click);
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(12, 172);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(77, 13);
      this.label23.TabIndex = 3;
      this.label23.Text = "External Player";
      // 
      // groupBoxMoviePathInfos
      // 
      this.groupBoxMoviePathInfos.Controls.Add(this.SearchOnlyExactMatches);
      this.groupBoxMoviePathInfos.Controls.Add(this.label5);
      this.groupBoxMoviePathInfos.Controls.Add(this.PathStorage);
      this.groupBoxMoviePathInfos.Controls.Add(this.SearchSubDirs);
      this.groupBoxMoviePathInfos.Controls.Add(this.butPath);
      this.groupBoxMoviePathInfos.Controls.Add(this.SearchFileName);
      this.groupBoxMoviePathInfos.Controls.Add(this.ItemSearchFileName);
      this.groupBoxMoviePathInfos.Location = new System.Drawing.Point(6, 41);
      this.groupBoxMoviePathInfos.Name = "groupBoxMoviePathInfos";
      this.groupBoxMoviePathInfos.Size = new System.Drawing.Size(420, 92);
      this.groupBoxMoviePathInfos.TabIndex = 11;
      this.groupBoxMoviePathInfos.TabStop = false;
      this.groupBoxMoviePathInfos.Text = "Playback Search Configuration";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 17);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(145, 13);
      this.label5.TabIndex = 0;
      this.label5.Text = "Paths for Movies File Search ";
      // 
      // butPath
      // 
      this.butPath.Location = new System.Drawing.Point(384, 14);
      this.butPath.Name = "butPath";
      this.butPath.Size = new System.Drawing.Size(29, 20);
      this.butPath.TabIndex = 2;
      this.butPath.Text = "...";
      this.butPath.UseVisualStyleBackColor = true;
      this.butPath.Click += new System.EventHandler(this.butPath_Click);
      // 
      // AntIdentLabel
      // 
      this.AntIdentLabel.Location = new System.Drawing.Point(302, 140);
      this.AntIdentLabel.Name = "AntIdentLabel";
      this.AntIdentLabel.Size = new System.Drawing.Size(124, 20);
      this.AntIdentLabel.TabIndex = 2;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(11, 18);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(152, 13);
      this.label6.TabIndex = 8;
      this.label6.Text = "DB Field for Movie Source Info";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 144);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(132, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Offline Media Identification";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(457, 17);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(70, 13);
      this.label13.TabIndex = 3;
      this.label13.Text = "Catalog Type";
      // 
      // ButCat
      // 
      this.ButCat.Location = new System.Drawing.Point(397, 15);
      this.ButCat.Name = "ButCat";
      this.ButCat.Size = new System.Drawing.Size(29, 20);
      this.ButCat.TabIndex = 2;
      this.ButCat.Text = "...";
      this.ButCat.UseVisualStyleBackColor = true;
      this.ButCat.Click += new System.EventHandler(this.ButCat_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(17, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(93, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Catalog File (XML)";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(80, 17);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(97, 13);
      this.label11.TabIndex = 0;
      this.label11.Text = "Menu Plugin Name";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(80, 45);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(100, 13);
      this.label12.TabIndex = 2;
      this.label12.Text = "Configuration Name";
      // 
      // ButSave
      // 
      this.ButSave.Location = new System.Drawing.Point(482, 466);
      this.ButSave.Name = "ButSave";
      this.ButSave.Size = new System.Drawing.Size(64, 31);
      this.ButSave.TabIndex = 16;
      this.ButSave.Text = "Save";
      this.ButSave.UseVisualStyleBackColor = true;
      this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(76, 71);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(253, 17);
      this.checkBox1.TabIndex = 75;
      this.checkBox1.Text = "Display Always that view when using  this config";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // label21
      // 
      this.label21.AutoSize = true;
      this.label21.Location = new System.Drawing.Point(82, 47);
      this.label21.Name = "label21";
      this.label21.Size = new System.Drawing.Size(78, 13);
      this.label21.TabIndex = 67;
      this.label21.Text = "Default LayOut";
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "List",
            "Small Icons",
            "Large Icons",
            "Filmstrip"});
      this.comboBox1.Location = new System.Drawing.Point(166, 44);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(112, 21);
      this.comboBox1.TabIndex = 66;
      this.comboBox1.Text = "List";
      // 
      // comboBox2
      // 
      this.comboBox2.FormattingEnabled = true;
      this.comboBox2.Location = new System.Drawing.Point(10, 18);
      this.comboBox2.Name = "comboBox2";
      this.comboBox2.Size = new System.Drawing.Size(146, 21);
      this.comboBox2.TabIndex = 16;
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(166, 18);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(170, 20);
      this.textBox2.TabIndex = 17;
      // 
      // Tab_ExternalCatalogs
      // 
      this.Tab_ExternalCatalogs.Controls.Add(this.groupBoxAMCsettings);
      this.Tab_ExternalCatalogs.Controls.Add(this.buttonOpenTmpFileAMC);
      this.Tab_ExternalCatalogs.Controls.Add(this.buttonOpenTmpFile);
      this.Tab_ExternalCatalogs.Controls.Add(this.buttonDeleteTmpCatalog);
      this.Tab_ExternalCatalogs.Controls.Add(this.groupBoxExtendedFieldHandling);
      this.Tab_ExternalCatalogs.Controls.Add(this.groupBox_DVDprofiler);
      this.Tab_ExternalCatalogs.Location = new System.Drawing.Point(4, 22);
      this.Tab_ExternalCatalogs.Name = "Tab_ExternalCatalogs";
      this.Tab_ExternalCatalogs.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_ExternalCatalogs.Size = new System.Drawing.Size(747, 354);
      this.Tab_ExternalCatalogs.TabIndex = 2;
      this.Tab_ExternalCatalogs.Text = "External Catalogs";
      this.Tab_ExternalCatalogs.ToolTipText = "Options for external catalogs (only applies if not using ANT movie catalog)";
      this.Tab_ExternalCatalogs.UseVisualStyleBackColor = true;
      // 
      // groupBoxAMCsettings
      // 
      this.groupBoxAMCsettings.Controls.Add(this.AMCexePath);
      this.groupBoxAMCsettings.Controls.Add(this.lblAMCpath);
      this.groupBoxAMCsettings.Controls.Add(this.buttonAMCpathSearch);
      this.groupBoxAMCsettings.Location = new System.Drawing.Point(392, 236);
      this.groupBoxAMCsettings.Name = "groupBoxAMCsettings";
      this.groupBoxAMCsettings.Size = new System.Drawing.Size(335, 93);
      this.groupBoxAMCsettings.TabIndex = 5;
      this.groupBoxAMCsettings.TabStop = false;
      this.groupBoxAMCsettings.Text = "Ant Movie Catalog (AMC)";
      // 
      // AMCexePath
      // 
      this.AMCexePath.Location = new System.Drawing.Point(14, 52);
      this.AMCexePath.Name = "AMCexePath";
      this.AMCexePath.Size = new System.Drawing.Size(261, 20);
      this.AMCexePath.TabIndex = 1;
      // 
      // lblAMCpath
      // 
      this.lblAMCpath.AutoSize = true;
      this.lblAMCpath.Location = new System.Drawing.Point(11, 33);
      this.lblAMCpath.Name = "lblAMCpath";
      this.lblAMCpath.Size = new System.Drawing.Size(213, 13);
      this.lblAMCpath.TabIndex = 0;
      this.lblAMCpath.Text = "Path to AMC Ant Movie Catalog Executable";
      // 
      // buttonAMCpathSearch
      // 
      this.buttonAMCpathSearch.Location = new System.Drawing.Point(281, 52);
      this.buttonAMCpathSearch.Name = "buttonAMCpathSearch";
      this.buttonAMCpathSearch.Size = new System.Drawing.Size(36, 20);
      this.buttonAMCpathSearch.TabIndex = 2;
      this.buttonAMCpathSearch.Text = "...";
      this.buttonAMCpathSearch.UseVisualStyleBackColor = true;
      this.buttonAMCpathSearch.Click += new System.EventHandler(this.buttonAMCpathSearch_Click);
      // 
      // buttonOpenTmpFile
      // 
      this.buttonOpenTmpFile.Location = new System.Drawing.Point(29, 269);
      this.buttonOpenTmpFile.Name = "buttonOpenTmpFile";
      this.buttonOpenTmpFile.Size = new System.Drawing.Size(152, 27);
      this.buttonOpenTmpFile.TabIndex = 3;
      this.buttonOpenTmpFile.Text = "Open tmp Catalog in Editor";
      this.buttonOpenTmpFile.UseVisualStyleBackColor = true;
      this.buttonOpenTmpFile.Click += new System.EventHandler(this.buttonOpenTmpFile_Click);
      // 
      // Tab_Network
      // 
      this.Tab_Network.Controls.Add(this.groupBox25);
      this.Tab_Network.Location = new System.Drawing.Point(4, 22);
      this.Tab_Network.Name = "Tab_Network";
      this.Tab_Network.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Network.Size = new System.Drawing.Size(747, 354);
      this.Tab_Network.TabIndex = 8;
      this.Tab_Network.Text = "Network";
      this.Tab_Network.ToolTipText = "Setup for WakeOnLan features";
      this.Tab_Network.UseVisualStyleBackColor = true;
      // 
      // Tab_Logos
      // 
      this.Tab_Logos.Controls.Add(this.lblCountry);
      this.Tab_Logos.Controls.Add(this.txtCountry);
      this.Tab_Logos.Controls.Add(this.btnUpdate);
      this.Tab_Logos.Controls.Add(this.btnLogoClearCache);
      this.Tab_Logos.Controls.Add(this.lblLogoPresets);
      this.Tab_Logos.Controls.Add(this.lbl_LogoSpacing);
      this.Tab_Logos.Controls.Add(this.comboBoxLogoPresets);
      this.Tab_Logos.Controls.Add(this.comboBoxLogoSpacing);
      this.Tab_Logos.Controls.Add(this.lblSetupLogoRules);
      this.Tab_Logos.Controls.Add(this.lblInfoLogosForAll);
      this.Tab_Logos.Controls.Add(this.lblSelectLogoFile);
      this.Tab_Logos.Controls.Add(this.btnLogosPath);
      this.Tab_Logos.Controls.Add(this.txtLogosPath);
      this.Tab_Logos.Controls.Add(this.SFilePicture);
      this.Tab_Logos.Controls.Add(this.lblLogosPath);
      this.Tab_Logos.Controls.Add(this.LogoView);
      this.Tab_Logos.Controls.Add(this.btnDown);
      this.Tab_Logos.Controls.Add(this.btnUp);
      this.Tab_Logos.Controls.Add(this.btnDel);
      this.Tab_Logos.Controls.Add(this.btnAdd);
      this.Tab_Logos.Controls.Add(this.SAnd_Or);
      this.Tab_Logos.Controls.Add(this.SValue2);
      this.Tab_Logos.Controls.Add(this.SOp2);
      this.Tab_Logos.Controls.Add(this.SField2);
      this.Tab_Logos.Controls.Add(this.SValue1);
      this.Tab_Logos.Controls.Add(this.SOp1);
      this.Tab_Logos.Controls.Add(this.SField1);
      this.Tab_Logos.Controls.Add(this.chkLogos);
      this.Tab_Logos.Controls.Add(this.SLogo_Type);
      this.Tab_Logos.Controls.Add(this.SPicture);
      this.Tab_Logos.Location = new System.Drawing.Point(4, 22);
      this.Tab_Logos.Name = "Tab_Logos";
      this.Tab_Logos.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Logos.Size = new System.Drawing.Size(747, 354);
      this.Tab_Logos.TabIndex = 6;
      this.Tab_Logos.Text = "Logos";
      this.Tab_Logos.ToolTipText = "Setup for logos";
      this.Tab_Logos.UseVisualStyleBackColor = true;
      // 
      // lblCountry
      // 
      this.lblCountry.AutoSize = true;
      this.lblCountry.Location = new System.Drawing.Point(26, 36);
      this.lblCountry.Name = "lblCountry";
      this.lblCountry.Size = new System.Drawing.Size(43, 13);
      this.lblCountry.TabIndex = 87;
      this.lblCountry.Text = "Country";
      // 
      // btnUpdate
      // 
      this.btnUpdate.Location = new System.Drawing.Point(583, 331);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(144, 21);
      this.btnUpdate.TabIndex = 26;
      this.btnUpdate.Text = "Update Logo Rule";
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      // 
      // lblLogoPresets
      // 
      this.lblLogoPresets.AutoSize = true;
      this.lblLogoPresets.Location = new System.Drawing.Point(110, 11);
      this.lblLogoPresets.Name = "lblLogoPresets";
      this.lblLogoPresets.Size = new System.Drawing.Size(42, 13);
      this.lblLogoPresets.TabIndex = 1;
      this.lblLogoPresets.Text = "Presets";
      // 
      // lbl_LogoSpacing
      // 
      this.lbl_LogoSpacing.AutoSize = true;
      this.lbl_LogoSpacing.Location = new System.Drawing.Point(592, 11);
      this.lbl_LogoSpacing.Name = "lbl_LogoSpacing";
      this.lbl_LogoSpacing.Size = new System.Drawing.Size(46, 13);
      this.lbl_LogoSpacing.TabIndex = 4;
      this.lbl_LogoSpacing.Text = "Spacing";
      // 
      // lblSetupLogoRules
      // 
      this.lblSetupLogoRules.AutoSize = true;
      this.lblSetupLogoRules.Location = new System.Drawing.Point(6, 253);
      this.lblSetupLogoRules.Name = "lblSetupLogoRules";
      this.lblSetupLogoRules.Size = new System.Drawing.Size(333, 13);
      this.lblSetupLogoRules.TabIndex = 11;
      this.lblSetupLogoRules.Text = "Set conditional Logo rules, select Logo Image and add rule to ruleset:";
      // 
      // lblInfoLogosForAll
      // 
      this.lblInfoLogosForAll.AutoSize = true;
      this.lblInfoLogosForAll.BackColor = System.Drawing.SystemColors.Control;
      this.lblInfoLogosForAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.lblInfoLogosForAll.ForeColor = System.Drawing.SystemColors.ControlText;
      this.lblInfoLogosForAll.Location = new System.Drawing.Point(6, 334);
      this.lblInfoLogosForAll.Name = "lblInfoLogosForAll";
      this.lblInfoLogosForAll.Size = new System.Drawing.Size(297, 13);
      this.lblInfoLogosForAll.TabIndex = 21;
      this.lblInfoLogosForAll.Text = "(The Logo Configuration is used for all MyFilms configurations)";
      // 
      // lblSelectLogoFile
      // 
      this.lblSelectLogoFile.AutoSize = true;
      this.lblSelectLogoFile.Location = new System.Drawing.Point(486, 253);
      this.lblSelectLogoFile.Name = "lblSelectLogoFile";
      this.lblSelectLogoFile.Size = new System.Drawing.Size(100, 13);
      this.lblSelectLogoFile.TabIndex = 12;
      this.lblSelectLogoFile.Text = "Click to select Logo";
      // 
      // btnLogosPath
      // 
      this.btnLogosPath.Enabled = false;
      this.btnLogosPath.Location = new System.Drawing.Point(695, 33);
      this.btnLogosPath.Name = "btnLogosPath";
      this.btnLogosPath.Size = new System.Drawing.Size(32, 20);
      this.btnLogosPath.TabIndex = 8;
      this.btnLogosPath.Text = "...";
      this.btnLogosPath.UseVisualStyleBackColor = true;
      this.btnLogosPath.Click += new System.EventHandler(this.btnLogosPath_Click);
      // 
      // SFilePicture
      // 
      this.SFilePicture.BackColor = System.Drawing.SystemColors.Control;
      this.SFilePicture.Location = new System.Drawing.Point(7, 230);
      this.SFilePicture.Name = "SFilePicture";
      this.SFilePicture.Size = new System.Drawing.Size(734, 20);
      this.SFilePicture.TabIndex = 10;
      // 
      // lblLogosPath
      // 
      this.lblLogosPath.AutoSize = true;
      this.lblLogosPath.Location = new System.Drawing.Point(110, 36);
      this.lblLogosPath.Name = "lblLogosPath";
      this.lblLogosPath.Size = new System.Drawing.Size(106, 13);
      this.lblLogosPath.TabIndex = 6;
      this.lblLogosPath.Text = "Searchpath to Logos";
      // 
      // btnDown
      // 
      this.btnDown.Enabled = false;
      this.btnDown.Location = new System.Drawing.Point(583, 303);
      this.btnDown.Name = "btnDown";
      this.btnDown.Size = new System.Drawing.Size(64, 21);
      this.btnDown.TabIndex = 24;
      this.btnDown.Text = "Down";
      this.btnDown.UseVisualStyleBackColor = true;
      this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
      // 
      // btnUp
      // 
      this.btnUp.Enabled = false;
      this.btnUp.Location = new System.Drawing.Point(583, 276);
      this.btnUp.Name = "btnUp";
      this.btnUp.Size = new System.Drawing.Size(64, 21);
      this.btnUp.TabIndex = 22;
      this.btnUp.Text = "Up";
      this.btnUp.UseVisualStyleBackColor = true;
      this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
      // 
      // btnDel
      // 
      this.btnDel.Enabled = false;
      this.btnDel.Location = new System.Drawing.Point(663, 303);
      this.btnDel.Name = "btnDel";
      this.btnDel.Size = new System.Drawing.Size(64, 21);
      this.btnDel.TabIndex = 25;
      this.btnDel.Text = "Delete";
      this.btnDel.UseVisualStyleBackColor = true;
      this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.Enabled = false;
      this.btnAdd.Location = new System.Drawing.Point(663, 276);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(64, 21);
      this.btnAdd.TabIndex = 23;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // SAnd_Or
      // 
      this.SAnd_Or.Enabled = false;
      this.SAnd_Or.FormattingEnabled = true;
      this.SAnd_Or.Items.AddRange(new object[] {
            "And",
            "Or",
            ""});
      this.SAnd_Or.Location = new System.Drawing.Point(117, 289);
      this.SAnd_Or.Name = "SAnd_Or";
      this.SAnd_Or.Size = new System.Drawing.Size(54, 21);
      this.SAnd_Or.TabIndex = 17;
      // 
      // SValue2
      // 
      this.SValue2.Enabled = false;
      this.SValue2.FormattingEnabled = true;
      this.SValue2.Location = new System.Drawing.Point(371, 303);
      this.SValue2.Name = "SValue2";
      this.SValue2.Size = new System.Drawing.Size(110, 21);
      this.SValue2.TabIndex = 20;
      // 
      // SOp2
      // 
      this.SOp2.Enabled = false;
      this.SOp2.FormattingEnabled = true;
      this.SOp2.Items.AddRange(new object[] {
            "equal",
            "not equal",
            "contains",
            "not contains",
            "greater",
            "lower",
            "filled",
            "not filled",
            "regex"});
      this.SOp2.Location = new System.Drawing.Point(291, 303);
      this.SOp2.Name = "SOp2";
      this.SOp2.Size = new System.Drawing.Size(74, 21);
      this.SOp2.TabIndex = 19;
      this.SOp2.SelectedIndexChanged += new System.EventHandler(this.SOp2_SelectedIndexChanged);
      // 
      // SField2
      // 
      this.SField2.Enabled = false;
      this.SField2.FormattingEnabled = true;
      this.SField2.Location = new System.Drawing.Point(175, 303);
      this.SField2.Name = "SField2";
      this.SField2.Size = new System.Drawing.Size(110, 21);
      this.SField2.Sorted = true;
      this.SField2.TabIndex = 18;
      this.SField2.SelectedIndexChanged += new System.EventHandler(this.SField2_SelectedIndexChanged);
      // 
      // SValue1
      // 
      this.SValue1.Enabled = false;
      this.SValue1.FormattingEnabled = true;
      this.SValue1.Location = new System.Drawing.Point(371, 276);
      this.SValue1.Name = "SValue1";
      this.SValue1.Size = new System.Drawing.Size(110, 21);
      this.SValue1.TabIndex = 15;
      // 
      // SField1
      // 
      this.SField1.Enabled = false;
      this.SField1.FormattingEnabled = true;
      this.SField1.Location = new System.Drawing.Point(175, 276);
      this.SField1.Name = "SField1";
      this.SField1.Size = new System.Drawing.Size(110, 21);
      this.SField1.Sorted = true;
      this.SField1.TabIndex = 13;
      this.SField1.SelectedIndexChanged += new System.EventHandler(this.SField1_SelectedIndexChanged);
      // 
      // Tab_Artwork
      // 
      this.Tab_Artwork.Controls.Add(this.groupBox_ArtistImages);
      this.Tab_Artwork.Controls.Add(this.groupBox1);
      this.Tab_Artwork.Controls.Add(this.groupBox22);
      this.Tab_Artwork.Controls.Add(this.Fanart);
      this.Tab_Artwork.Location = new System.Drawing.Point(4, 22);
      this.Tab_Artwork.Name = "Tab_Artwork";
      this.Tab_Artwork.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Artwork.Size = new System.Drawing.Size(747, 354);
      this.Tab_Artwork.TabIndex = 5;
      this.Tab_Artwork.Text = "Artwork";
      this.Tab_Artwork.ToolTipText = "Setup for artwork, like pathes to coverart, person images etc.";
      this.Tab_Artwork.UseVisualStyleBackColor = true;
      // 
      // Tab_AMCupdater
      // 
      this.Tab_AMCupdater.Controls.Add(this.groupBox_UpdateGrabberScripts);
      this.Tab_AMCupdater.Controls.Add(this.groupBox_AMCupdater);
      this.Tab_AMCupdater.Location = new System.Drawing.Point(4, 22);
      this.Tab_AMCupdater.Name = "Tab_AMCupdater";
      this.Tab_AMCupdater.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_AMCupdater.Size = new System.Drawing.Size(747, 354);
      this.Tab_AMCupdater.TabIndex = 4;
      this.Tab_AMCupdater.Text = "Import";
      this.Tab_AMCupdater.ToolTipText = "Setup for internet updates and basic configuration for AMC Updater";
      this.Tab_AMCupdater.UseVisualStyleBackColor = true;
      // 
      // Tab_Update
      // 
      this.Tab_Update.Controls.Add(this.groupBoxMultiUserState);
      this.Tab_Update.Controls.Add(this.groupBox_GrabberOptions);
      this.Tab_Update.Controls.Add(this.groupBox2);
      this.Tab_Update.Controls.Add(this.groupBoxDeletionOptions);
      this.Tab_Update.Location = new System.Drawing.Point(4, 22);
      this.Tab_Update.Name = "Tab_Update";
      this.Tab_Update.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Update.Size = new System.Drawing.Size(747, 354);
      this.Tab_Update.TabIndex = 3;
      this.Tab_Update.Text = "Update";
      this.Tab_Update.ToolTipText = "Setup for update options (updates to values from GUI)";
      this.Tab_Update.UseVisualStyleBackColor = true;
      // 
      // General
      // 
      this.General.Controls.Add(this.Tab_General);
      this.General.Controls.Add(this.Tab_Display);
      this.General.Controls.Add(this.Tab_Views);
      this.General.Controls.Add(this.Tab_Trailer);
      this.General.Controls.Add(this.Tab_Update);
      this.General.Controls.Add(this.Tab_AMCupdater);
      this.General.Controls.Add(this.Tab_Artwork);
      this.General.Controls.Add(this.Tab_Logos);
      this.General.Controls.Add(this.Tab_Network);
      this.General.Controls.Add(this.Tab_ExternalCatalogs);
      this.General.Controls.Add(this.Tab_Trakt);
      this.General.Controls.Add(this.Tab_About);
      this.General.Controls.Add(this.Tab_Other);
      this.General.Controls.Add(this.Tab_OldStuff);
      this.General.Location = new System.Drawing.Point(12, 78);
      this.General.Name = "General";
      this.General.SelectedIndex = 0;
      this.General.ShowToolTips = true;
      this.General.Size = new System.Drawing.Size(755, 380);
      this.General.TabIndex = 11;
      this.General.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.General_Selected);
      // 
      // Tab_About
      // 
      this.Tab_About.Controls.Add(this.lblTmdbNote);
      this.Tab_About.Controls.Add(this.pictureBoxTMDBlogo);
      this.Tab_About.Controls.Add(this.groupBoxSupportedCatalogs);
      this.Tab_About.Controls.Add(this.groupBoxAbout);
      this.Tab_About.Location = new System.Drawing.Point(4, 22);
      this.Tab_About.Name = "Tab_About";
      this.Tab_About.Size = new System.Drawing.Size(747, 354);
      this.Tab_About.TabIndex = 13;
      this.Tab_About.Text = "About";
      this.Tab_About.ToolTipText = "About Info for MyFilms";
      this.Tab_About.UseVisualStyleBackColor = true;
      // 
      // lblTmdbNote
      // 
      this.lblTmdbNote.AutoSize = true;
      this.lblTmdbNote.ForeColor = System.Drawing.SystemColors.GrayText;
      this.lblTmdbNote.Location = new System.Drawing.Point(498, 256);
      this.lblTmdbNote.Name = "lblTmdbNote";
      this.lblTmdbNote.Size = new System.Drawing.Size(199, 26);
      this.lblTmdbNote.TabIndex = 79;
      this.lblTmdbNote.Text = "This product uses the TMDb API \r\nbut is not endorsed or certified by TMDb.";
      this.lblTmdbNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // pictureBoxTMDBlogo
      // 
      this.pictureBoxTMDBlogo.Image = global::MyFilmsPlugin.Properties.Resources.var_1_0_PoweredByTMDB_Blk_Antitled;
      this.pictureBoxTMDBlogo.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxTMDBlogo.InitialImage")));
      this.pictureBoxTMDBlogo.Location = new System.Drawing.Point(481, 178);
      this.pictureBoxTMDBlogo.Name = "pictureBoxTMDBlogo";
      this.pictureBoxTMDBlogo.Size = new System.Drawing.Size(231, 67);
      this.pictureBoxTMDBlogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBoxTMDBlogo.TabIndex = 78;
      this.pictureBoxTMDBlogo.TabStop = false;
      // 
      // groupBoxSupportedCatalogs
      // 
      this.groupBoxSupportedCatalogs.Controls.Add(this.label41);
      this.groupBoxSupportedCatalogs.Location = new System.Drawing.Point(30, 171);
      this.groupBoxSupportedCatalogs.Name = "groupBoxSupportedCatalogs";
      this.groupBoxSupportedCatalogs.Size = new System.Drawing.Size(414, 180);
      this.groupBoxSupportedCatalogs.TabIndex = 1;
      this.groupBoxSupportedCatalogs.TabStop = false;
      this.groupBoxSupportedCatalogs.Text = "Credits ...";
      // 
      // label41
      // 
      this.label41.AutoSize = true;
      this.label41.Location = new System.Drawing.Point(27, 18);
      this.label41.Name = "label41";
      this.label41.Size = new System.Drawing.Size(315, 143);
      this.label41.TabIndex = 0;
      this.label41.Text = resources.GetString("label41.Text");
      this.label41.Click += new System.EventHandler(this.label41_Click);
      // 
      // groupBoxAbout
      // 
      this.groupBoxAbout.Controls.Add(this.label28);
      this.groupBoxAbout.Location = new System.Drawing.Point(30, 13);
      this.groupBoxAbout.Name = "groupBoxAbout";
      this.groupBoxAbout.Size = new System.Drawing.Size(682, 152);
      this.groupBoxAbout.TabIndex = 0;
      this.groupBoxAbout.TabStop = false;
      this.groupBoxAbout.Text = "About MyFilms ...";
      // 
      // label28
      // 
      this.label28.AutoSize = true;
      this.label28.Location = new System.Drawing.Point(27, 25);
      this.label28.Name = "label28";
      this.label28.Size = new System.Drawing.Size(584, 117);
      this.label28.TabIndex = 0;
      this.label28.Text = resources.GetString("label28.Text");
      this.label28.Click += new System.EventHandler(this.label28_Click);
      // 
      // Tab_Other
      // 
      this.Tab_Other.AutoScroll = true;
      this.Tab_Other.Controls.Add(this.personDataGridView);
      this.Tab_Other.Controls.Add(this.customFieldDataGridView);
      this.Tab_Other.Controls.Add(ownerLabel);
      this.Tab_Other.Controls.Add(this.ownerTextBox);
      this.Tab_Other.Controls.Add(mailLabel);
      this.Tab_Other.Controls.Add(this.mailTextBox);
      this.Tab_Other.Controls.Add(siteLabel);
      this.Tab_Other.Controls.Add(this.siteTextBox);
      this.Tab_Other.Controls.Add(descriptionLabel);
      this.Tab_Other.Controls.Add(this.descriptionTextBox);
      this.Tab_Other.Controls.Add(this.lblAMCupdaterConfigPreview);
      this.Tab_Other.Controls.Add(this.AMCConfigView);
      this.Tab_Other.Location = new System.Drawing.Point(4, 22);
      this.Tab_Other.Name = "Tab_Other";
      this.Tab_Other.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_Other.Size = new System.Drawing.Size(747, 354);
      this.Tab_Other.TabIndex = 11;
      this.Tab_Other.Text = "Other";
      this.Tab_Other.ToolTipText = "Other NonPublic Content";
      this.Tab_Other.UseVisualStyleBackColor = true;
      // 
      // personDataGridView
      // 
      this.personDataGridView.AutoGenerateColumns = false;
      this.personDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.personDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.personDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn17,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewCheckBoxColumn2,
            this.dataGridViewCheckBoxColumn3,
            this.dataGridViewCheckBoxColumn4,
            this.dataGridViewTextBoxColumn18,
            this.dataGridViewTextBoxColumn19});
      this.personDataGridView.DataSource = this.personBindingSource;
      this.personDataGridView.Location = new System.Drawing.Point(6, 233);
      this.personDataGridView.Name = "personDataGridView";
      this.personDataGridView.RowHeadersVisible = false;
      this.personDataGridView.RowTemplate.Height = 18;
      this.personDataGridView.Size = new System.Drawing.Size(735, 115);
      this.personDataGridView.TabIndex = 12;
      // 
      // dataGridViewTextBoxColumn9
      // 
      this.dataGridViewTextBoxColumn9.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn9.HeaderText = "Name";
      this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
      this.dataGridViewTextBoxColumn9.Width = 60;
      // 
      // dataGridViewTextBoxColumn10
      // 
      this.dataGridViewTextBoxColumn10.DataPropertyName = "AlternateName";
      this.dataGridViewTextBoxColumn10.HeaderText = "AlternateName";
      this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
      this.dataGridViewTextBoxColumn10.Width = 102;
      // 
      // dataGridViewTextBoxColumn11
      // 
      this.dataGridViewTextBoxColumn11.DataPropertyName = "Born";
      this.dataGridViewTextBoxColumn11.HeaderText = "Born";
      this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
      this.dataGridViewTextBoxColumn11.Width = 54;
      // 
      // dataGridViewTextBoxColumn12
      // 
      this.dataGridViewTextBoxColumn12.DataPropertyName = "BirthPlace";
      this.dataGridViewTextBoxColumn12.HeaderText = "BirthPlace";
      this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
      this.dataGridViewTextBoxColumn12.Width = 80;
      // 
      // dataGridViewTextBoxColumn13
      // 
      this.dataGridViewTextBoxColumn13.DataPropertyName = "MiniBiography";
      this.dataGridViewTextBoxColumn13.HeaderText = "MiniBiography";
      this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
      this.dataGridViewTextBoxColumn13.Width = 98;
      // 
      // dataGridViewTextBoxColumn14
      // 
      this.dataGridViewTextBoxColumn14.DataPropertyName = "Biography";
      this.dataGridViewTextBoxColumn14.HeaderText = "Biography";
      this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
      this.dataGridViewTextBoxColumn14.Width = 79;
      // 
      // dataGridViewTextBoxColumn15
      // 
      this.dataGridViewTextBoxColumn15.DataPropertyName = "URL";
      this.dataGridViewTextBoxColumn15.HeaderText = "URL";
      this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
      this.dataGridViewTextBoxColumn15.Width = 54;
      // 
      // dataGridViewTextBoxColumn16
      // 
      this.dataGridViewTextBoxColumn16.DataPropertyName = "IMDB_Id";
      this.dataGridViewTextBoxColumn16.HeaderText = "IMDB_Id";
      this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
      this.dataGridViewTextBoxColumn16.Width = 74;
      // 
      // dataGridViewTextBoxColumn17
      // 
      this.dataGridViewTextBoxColumn17.DataPropertyName = "TMDB_Id";
      this.dataGridViewTextBoxColumn17.HeaderText = "TMDB_Id";
      this.dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
      this.dataGridViewTextBoxColumn17.Width = 78;
      // 
      // dataGridViewCheckBoxColumn1
      // 
      this.dataGridViewCheckBoxColumn1.DataPropertyName = "IsActor";
      this.dataGridViewCheckBoxColumn1.HeaderText = "IsActor";
      this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
      this.dataGridViewCheckBoxColumn1.Width = 46;
      // 
      // dataGridViewCheckBoxColumn2
      // 
      this.dataGridViewCheckBoxColumn2.DataPropertyName = "IsProducer";
      this.dataGridViewCheckBoxColumn2.HeaderText = "IsProducer";
      this.dataGridViewCheckBoxColumn2.Name = "dataGridViewCheckBoxColumn2";
      this.dataGridViewCheckBoxColumn2.Width = 64;
      // 
      // dataGridViewCheckBoxColumn3
      // 
      this.dataGridViewCheckBoxColumn3.DataPropertyName = "IsDirector";
      this.dataGridViewCheckBoxColumn3.HeaderText = "IsDirector";
      this.dataGridViewCheckBoxColumn3.Name = "dataGridViewCheckBoxColumn3";
      this.dataGridViewCheckBoxColumn3.Width = 58;
      // 
      // dataGridViewCheckBoxColumn4
      // 
      this.dataGridViewCheckBoxColumn4.DataPropertyName = "IsWriter";
      this.dataGridViewCheckBoxColumn4.HeaderText = "IsWriter";
      this.dataGridViewCheckBoxColumn4.Name = "dataGridViewCheckBoxColumn4";
      this.dataGridViewCheckBoxColumn4.Width = 49;
      // 
      // dataGridViewTextBoxColumn18
      // 
      this.dataGridViewTextBoxColumn18.DataPropertyName = "Photos";
      this.dataGridViewTextBoxColumn18.HeaderText = "Photos";
      this.dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
      this.dataGridViewTextBoxColumn18.Width = 65;
      // 
      // dataGridViewTextBoxColumn19
      // 
      this.dataGridViewTextBoxColumn19.DataPropertyName = "Picture";
      this.dataGridViewTextBoxColumn19.HeaderText = "Picture";
      this.dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
      this.dataGridViewTextBoxColumn19.Width = 65;
      // 
      // personBindingSource
      // 
      this.personBindingSource.DataMember = "Person";
      this.personBindingSource.DataSource = this.antMovieCatalog;
      // 
      // antMovieCatalog
      // 
      this.antMovieCatalog.DataSetName = "AntMovieCatalog";
      this.antMovieCatalog.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // customFieldDataGridView
      // 
      this.customFieldDataGridView.AutoGenerateColumns = false;
      this.customFieldDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.customFieldDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.customFieldDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
      this.customFieldDataGridView.DataSource = this.customFieldBindingSource;
      this.customFieldDataGridView.Location = new System.Drawing.Point(6, 115);
      this.customFieldDataGridView.Name = "customFieldDataGridView";
      this.customFieldDataGridView.RowHeadersVisible = false;
      this.customFieldDataGridView.RowTemplate.Height = 18;
      this.customFieldDataGridView.Size = new System.Drawing.Size(735, 112);
      this.customFieldDataGridView.TabIndex = 11;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Tag";
      this.dataGridViewTextBoxColumn1.HeaderText = "Tag";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Width = 51;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Type";
      this.dataGridViewTextBoxColumn2.HeaderText = "Type";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.Width = 56;
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn3.HeaderText = "Name";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.Width = 60;
      // 
      // dataGridViewTextBoxColumn4
      // 
      this.dataGridViewTextBoxColumn4.DataPropertyName = "DefaultValue";
      this.dataGridViewTextBoxColumn4.HeaderText = "DefaultValue";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.Width = 93;
      // 
      // dataGridViewTextBoxColumn5
      // 
      this.dataGridViewTextBoxColumn5.DataPropertyName = "MultiValues";
      this.dataGridViewTextBoxColumn5.HeaderText = "MultiValues";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.Width = 86;
      // 
      // dataGridViewTextBoxColumn6
      // 
      this.dataGridViewTextBoxColumn6.DataPropertyName = "ExcludedInScripts";
      this.dataGridViewTextBoxColumn6.HeaderText = "ExcludedInScripts";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.Width = 117;
      // 
      // dataGridViewTextBoxColumn7
      // 
      this.dataGridViewTextBoxColumn7.DataPropertyName = "GUIProperties";
      this.dataGridViewTextBoxColumn7.HeaderText = "GUIProperties";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.Width = 98;
      // 
      // dataGridViewTextBoxColumn8
      // 
      this.dataGridViewTextBoxColumn8.DataPropertyName = "OtherProperties";
      this.dataGridViewTextBoxColumn8.HeaderText = "OtherProperties";
      this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
      this.dataGridViewTextBoxColumn8.Width = 105;
      // 
      // customFieldBindingSource
      // 
      this.customFieldBindingSource.DataMember = "CustomField";
      this.customFieldBindingSource.DataSource = this.antMovieCatalog;
      // 
      // ownerTextBox
      // 
      this.ownerTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.propertiesBindingSource, "Owner", true));
      this.ownerTextBox.Location = new System.Drawing.Point(426, 9);
      this.ownerTextBox.Name = "ownerTextBox";
      this.ownerTextBox.Size = new System.Drawing.Size(134, 20);
      this.ownerTextBox.TabIndex = 3;
      // 
      // propertiesBindingSource
      // 
      this.propertiesBindingSource.DataMember = "Properties";
      this.propertiesBindingSource.DataSource = this.antMovieCatalog;
      // 
      // mailTextBox
      // 
      this.mailTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.propertiesBindingSource, "Mail", true));
      this.mailTextBox.Location = new System.Drawing.Point(426, 35);
      this.mailTextBox.Name = "mailTextBox";
      this.mailTextBox.Size = new System.Drawing.Size(134, 20);
      this.mailTextBox.TabIndex = 5;
      // 
      // siteTextBox
      // 
      this.siteTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.propertiesBindingSource, "Site", true));
      this.siteTextBox.Location = new System.Drawing.Point(426, 61);
      this.siteTextBox.Name = "siteTextBox";
      this.siteTextBox.Size = new System.Drawing.Size(134, 20);
      this.siteTextBox.TabIndex = 7;
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.propertiesBindingSource, "Description", true));
      this.descriptionTextBox.Location = new System.Drawing.Point(426, 87);
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.Size = new System.Drawing.Size(134, 20);
      this.descriptionTextBox.TabIndex = 9;
      // 
      // lblAMCupdaterConfigPreview
      // 
      this.lblAMCupdaterConfigPreview.AutoSize = true;
      this.lblAMCupdaterConfigPreview.Location = new System.Drawing.Point(6, 7);
      this.lblAMCupdaterConfigPreview.Name = "lblAMCupdaterConfigPreview";
      this.lblAMCupdaterConfigPreview.Size = new System.Drawing.Size(225, 13);
      this.lblAMCupdaterConfigPreview.TabIndex = 0;
      this.lblAMCupdaterConfigPreview.Text = "Preview of current AMC Updater Configuration";
      // 
      // Tab_OldStuff
      // 
      this.Tab_OldStuff.Controls.Add(this.btnAMCimport);
      this.Tab_OldStuff.Controls.Add(this.groupBox8);
      this.Tab_OldStuff.Location = new System.Drawing.Point(4, 22);
      this.Tab_OldStuff.Name = "Tab_OldStuff";
      this.Tab_OldStuff.Padding = new System.Windows.Forms.Padding(3);
      this.Tab_OldStuff.Size = new System.Drawing.Size(747, 354);
      this.Tab_OldStuff.TabIndex = 12;
      this.Tab_OldStuff.Text = "Old Stuff";
      this.Tab_OldStuff.ToolTipText = "Temorary placed elements";
      this.Tab_OldStuff.UseVisualStyleBackColor = true;
      // 
      // btnAMCimport
      // 
      this.btnAMCimport.Location = new System.Drawing.Point(37, 27);
      this.btnAMCimport.Name = "btnAMCimport";
      this.btnAMCimport.Size = new System.Drawing.Size(101, 46);
      this.btnAMCimport.TabIndex = 1;
      this.btnAMCimport.Text = "Import Movies";
      this.btnAMCimport.UseVisualStyleBackColor = true;
      this.btnAMCimport.Click += new System.EventHandler(this.btnAMCimport_Click);
      // 
      // groupBox8
      // 
      this.groupBox8.Controls.Add(this.label25);
      this.groupBox8.Controls.Add(this.label7);
      this.groupBox8.Controls.Add(this.label3);
      this.groupBox8.Controls.Add(this.LayOutViewFilms);
      this.groupBox8.Controls.Add(this.SortSensViewFilms);
      this.groupBox8.Controls.Add(this.SortViewFilms);
      this.groupBox8.Location = new System.Drawing.Point(436, 6);
      this.groupBox8.Name = "groupBox8";
      this.groupBox8.Size = new System.Drawing.Size(291, 66);
      this.groupBox8.TabIndex = 0;
      this.groupBox8.TabStop = false;
      this.groupBox8.Text = "Sort and Layouts for Films (Custom View)";
      // 
      // label25
      // 
      this.label25.AutoSize = true;
      this.label25.Location = new System.Drawing.Point(202, 18);
      this.label25.Name = "label25";
      this.label25.Size = new System.Drawing.Size(39, 13);
      this.label25.TabIndex = 2;
      this.label25.Text = "Layout";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(125, 18);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(26, 13);
      this.label7.TabIndex = 1;
      this.label7.Text = "Sort";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(51, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Sort Field";
      // 
      // LayOutViewFilms
      // 
      this.LayOutViewFilms.FormattingEnabled = true;
      this.LayOutViewFilms.Items.AddRange(new object[] {
            "List",
            "Big Icon List",
            "Small Icons",
            "Large Icons",
            "Filmstrip",
            "Cover Flow"});
      this.LayOutViewFilms.Location = new System.Drawing.Point(192, 35);
      this.LayOutViewFilms.Name = "LayOutViewFilms";
      this.LayOutViewFilms.Size = new System.Drawing.Size(90, 21);
      this.LayOutViewFilms.TabIndex = 5;
      // 
      // SortSensViewFilms
      // 
      this.SortSensViewFilms.FormattingEnabled = true;
      this.SortSensViewFilms.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
      this.SortSensViewFilms.Location = new System.Drawing.Point(120, 35);
      this.SortSensViewFilms.Name = "SortSensViewFilms";
      this.SortSensViewFilms.Size = new System.Drawing.Size(59, 21);
      this.SortSensViewFilms.TabIndex = 4;
      // 
      // SortViewFilms
      // 
      this.SortViewFilms.FormattingEnabled = true;
      this.SortViewFilms.Location = new System.Drawing.Point(10, 35);
      this.SortViewFilms.Name = "SortViewFilms";
      this.SortViewFilms.Size = new System.Drawing.Size(104, 21);
      this.SortViewFilms.TabIndex = 3;
      // 
      // textBoxNBconfigs
      // 
      this.textBoxNBconfigs.Enabled = false;
      this.textBoxNBconfigs.Location = new System.Drawing.Point(421, 42);
      this.textBoxNBconfigs.Name = "textBoxNBconfigs";
      this.textBoxNBconfigs.Size = new System.Drawing.Size(28, 20);
      this.textBoxNBconfigs.TabIndex = 6;
      this.textBoxNBconfigs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // lblNbConfig
      // 
      this.lblNbConfig.AutoSize = true;
      this.lblNbConfig.Enabled = false;
      this.lblNbConfig.Location = new System.Drawing.Point(373, 45);
      this.lblNbConfig.Name = "lblNbConfig";
      this.lblNbConfig.Size = new System.Drawing.Size(42, 13);
      this.lblNbConfig.TabIndex = 5;
      this.lblNbConfig.Text = "Configs";
      // 
      // pictureBoxMyFilms
      // 
      this.pictureBoxMyFilms.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxMyFilms.Image")));
      this.pictureBoxMyFilms.Location = new System.Drawing.Point(12, 7);
      this.pictureBoxMyFilms.Name = "pictureBoxMyFilms";
      this.pictureBoxMyFilms.Size = new System.Drawing.Size(62, 65);
      this.pictureBoxMyFilms.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBoxMyFilms.TabIndex = 75;
      this.pictureBoxMyFilms.TabStop = false;
      // 
      // customFieldsBindingSource
      // 
      this.customFieldsBindingSource.DataMember = "CustomFields";
      this.customFieldsBindingSource.DataSource = this.antMovieCatalog;
      // 
      // btnEditView
      // 
      this.btnEditView.Location = new System.Drawing.Point(373, 103);
      this.btnEditView.Name = "btnEditView";
      this.btnEditView.Size = new System.Drawing.Size(45, 20);
      this.btnEditView.TabIndex = 39;
      this.btnEditView.Text = "edit";
      this.btnEditView.UseVisualStyleBackColor = true;
      // 
      // label52
      // 
      this.label52.AutoSize = true;
      this.label52.Location = new System.Drawing.Point(8, 216);
      this.label52.Name = "label52";
      this.label52.Size = new System.Drawing.Size(30, 13);
      this.label52.TabIndex = 38;
      this.label52.Text = "Films";
      // 
      // label33
      // 
      this.label33.AutoSize = true;
      this.label33.Location = new System.Drawing.Point(8, 190);
      this.label33.Name = "label33";
      this.label33.Size = new System.Drawing.Size(52, 13);
      this.label33.TabIndex = 37;
      this.label33.Text = "Hierarchy";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(8, 166);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(30, 13);
      this.label18.TabIndex = 36;
      this.label18.Text = "View";
      // 
      // indexNumericUpDown
      // 
      this.indexNumericUpDown.Location = new System.Drawing.Point(220, 78);
      this.indexNumericUpDown.Name = "indexNumericUpDown";
      this.indexNumericUpDown.Size = new System.Drawing.Size(39, 20);
      this.indexNumericUpDown.TabIndex = 35;
      // 
      // labelTextBox
      // 
      this.labelTextBox.Location = new System.Drawing.Point(65, 50);
      this.labelTextBox.Name = "labelTextBox";
      this.labelTextBox.Size = new System.Drawing.Size(107, 20);
      this.labelTextBox.TabIndex = 4;
      // 
      // valueTextBox
      // 
      this.valueTextBox.Location = new System.Drawing.Point(304, 77);
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.Size = new System.Drawing.Size(114, 20);
      this.valueTextBox.TabIndex = 8;
      // 
      // filterTextBox
      // 
      this.filterTextBox.Location = new System.Drawing.Point(65, 103);
      this.filterTextBox.Name = "filterTextBox";
      this.filterTextBox.Size = new System.Drawing.Size(299, 20);
      this.filterTextBox.TabIndex = 10;
      // 
      // showEmptyCheckBox
      // 
      this.showEmptyCheckBox.Location = new System.Drawing.Point(333, 166);
      this.showEmptyCheckBox.Name = "showEmptyCheckBox";
      this.showEmptyCheckBox.Size = new System.Drawing.Size(121, 24);
      this.showEmptyCheckBox.TabIndex = 14;
      this.showEmptyCheckBox.Text = "Show Empty";
      this.showEmptyCheckBox.UseVisualStyleBackColor = true;
      // 
      // reverseNamesCheckBox
      // 
      this.reverseNamesCheckBox.Location = new System.Drawing.Point(333, 187);
      this.reverseNamesCheckBox.Name = "reverseNamesCheckBox";
      this.reverseNamesCheckBox.Size = new System.Drawing.Size(121, 24);
      this.reverseNamesCheckBox.TabIndex = 16;
      this.reverseNamesCheckBox.Text = "Reverse Names";
      this.reverseNamesCheckBox.UseVisualStyleBackColor = true;
      // 
      // sortOcurrenciesCheckBox
      // 
      this.sortOcurrenciesCheckBox.Location = new System.Drawing.Point(333, 208);
      this.sortOcurrenciesCheckBox.Name = "sortOcurrenciesCheckBox";
      this.sortOcurrenciesCheckBox.Size = new System.Drawing.Size(121, 24);
      this.sortOcurrenciesCheckBox.TabIndex = 18;
      this.sortOcurrenciesCheckBox.Text = "Sort Occurenvies";
      this.sortOcurrenciesCheckBox.UseVisualStyleBackColor = true;
      // 
      // sortDirectionViewComboBox
      // 
      this.sortDirectionViewComboBox.FormattingEnabled = true;
      this.sortDirectionViewComboBox.Location = new System.Drawing.Point(178, 161);
      this.sortDirectionViewComboBox.Name = "sortDirectionViewComboBox";
      this.sortDirectionViewComboBox.Size = new System.Drawing.Size(63, 21);
      this.sortDirectionViewComboBox.TabIndex = 20;
      // 
      // layoutViewComboBox
      // 
      this.layoutViewComboBox.FormattingEnabled = true;
      this.layoutViewComboBox.Location = new System.Drawing.Point(247, 161);
      this.layoutViewComboBox.Name = "layoutViewComboBox";
      this.layoutViewComboBox.Size = new System.Drawing.Size(69, 21);
      this.layoutViewComboBox.TabIndex = 22;
      // 
      // sortFieldHierarchyComboBox
      // 
      this.sortFieldHierarchyComboBox.FormattingEnabled = true;
      this.sortFieldHierarchyComboBox.Location = new System.Drawing.Point(65, 187);
      this.sortFieldHierarchyComboBox.Name = "sortFieldHierarchyComboBox";
      this.sortFieldHierarchyComboBox.Size = new System.Drawing.Size(107, 21);
      this.sortFieldHierarchyComboBox.TabIndex = 24;
      // 
      // sortDirectionHierarchyComboBox
      // 
      this.sortDirectionHierarchyComboBox.FormattingEnabled = true;
      this.sortDirectionHierarchyComboBox.Location = new System.Drawing.Point(178, 187);
      this.sortDirectionHierarchyComboBox.Name = "sortDirectionHierarchyComboBox";
      this.sortDirectionHierarchyComboBox.Size = new System.Drawing.Size(63, 21);
      this.sortDirectionHierarchyComboBox.TabIndex = 26;
      // 
      // layoutHierarchyComboBox
      // 
      this.layoutHierarchyComboBox.FormattingEnabled = true;
      this.layoutHierarchyComboBox.Location = new System.Drawing.Point(247, 187);
      this.layoutHierarchyComboBox.Name = "layoutHierarchyComboBox";
      this.layoutHierarchyComboBox.Size = new System.Drawing.Size(69, 21);
      this.layoutHierarchyComboBox.TabIndex = 28;
      // 
      // sortFieldFilmsComboBox
      // 
      this.sortFieldFilmsComboBox.FormattingEnabled = true;
      this.sortFieldFilmsComboBox.Location = new System.Drawing.Point(65, 214);
      this.sortFieldFilmsComboBox.Name = "sortFieldFilmsComboBox";
      this.sortFieldFilmsComboBox.Size = new System.Drawing.Size(107, 21);
      this.sortFieldFilmsComboBox.TabIndex = 30;
      // 
      // sortDirectionFilmsComboBox
      // 
      this.sortDirectionFilmsComboBox.FormattingEnabled = true;
      this.sortDirectionFilmsComboBox.Location = new System.Drawing.Point(178, 213);
      this.sortDirectionFilmsComboBox.Name = "sortDirectionFilmsComboBox";
      this.sortDirectionFilmsComboBox.Size = new System.Drawing.Size(63, 21);
      this.sortDirectionFilmsComboBox.TabIndex = 32;
      // 
      // layoutFilmsComboBox
      // 
      this.layoutFilmsComboBox.FormattingEnabled = true;
      this.layoutFilmsComboBox.Location = new System.Drawing.Point(246, 213);
      this.layoutFilmsComboBox.Name = "layoutFilmsComboBox";
      this.layoutFilmsComboBox.Size = new System.Drawing.Size(70, 21);
      this.layoutFilmsComboBox.TabIndex = 34;
      // 
      // MyFilmsSetup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(779, 506);
      this.Controls.Add(this.btnTestMode);
      this.Controls.Add(this.Config_EnablePreload);
      this.Controls.Add(this.btnServerSync);
      this.Controls.Add(this.buttonWikiHelp);
      this.Controls.Add(this.btnLaunchAMCglobal);
      this.Controls.Add(this.btnFirstTimeSetup);
      this.Controls.Add(this.btnHyperLinkParamGen);
      this.Controls.Add(this.lblNbConfig);
      this.Controls.Add(this.textBoxNBconfigs);
      this.Controls.Add(this.butNew);
      this.Controls.Add(this.ButCopy);
      this.Controls.Add(this.pictureBoxMyFilms);
      this.Controls.Add(this.Config_Menu);
      this.Controls.Add(this.ButSave);
      this.Controls.Add(this.Config_Dflt);
      this.Controls.Add(this.ButDelet);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.Config_Name);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.textBoxPluginName);
      this.Controls.Add(this.label_VersionNumber);
      this.Controls.Add(this.General);
      this.Controls.Add(this.ButQuit);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.HelpButton = true;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MyFilmsSetup";
      this.Text = "MyFilms Setup";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MesFilmsSetup_Quit);
      this.Load += new System.EventHandler(this.MesFilmsSetup_Load);
      this.groupBox9.ResumeLayout(false);
      this.groupBox9.PerformLayout();
      this.groupBox25.ResumeLayout(false);
      this.groupBox25.PerformLayout();
      this.Fanart.ResumeLayout(false);
      this.Fanart.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultFanart)).EndInit();
      this.groupBox22.ResumeLayout(false);
      this.groupBox22.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultViewImage)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultCover)).EndInit();
      this.groupBox_ArtistImages.ResumeLayout(false);
      this.groupBox_ArtistImages.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefaultPersonImage)).EndInit();
      this.groupBox_AMCupdater.ResumeLayout(false);
      this.groupBox_AMCupdater.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAMCUscanStartDelay)).EndInit();
      this.groupBoxAMCUpdaterConfigFile.ResumeLayout(false);
      this.groupBoxAMCUpdaterConfigFile.PerformLayout();
      this.groupBox_AMCupdaterScheduer.ResumeLayout(false);
      this.groupBox_AMCupdaterScheduer.PerformLayout();
      this.groupBox_AMCupdater_ExternalApplication.ResumeLayout(false);
      this.groupBox_AMCupdater_ExternalApplication.PerformLayout();
      this.groupBoxDeletionOptions.ResumeLayout(false);
      this.groupBoxDeletionOptions.PerformLayout();
      this.gpspfield.ResumeLayout(false);
      this.gpspfield.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox_UserItemsDetails.ResumeLayout(false);
      this.groupBox_UserItemsDetails.PerformLayout();
      this.groupBox_UserItemsMain.ResumeLayout(false);
      this.groupBox_UserItemsMain.PerformLayout();
      this.groupBox_Separators.ResumeLayout(false);
      this.groupBox_Separators.PerformLayout();
      this.groupBox_DefaultView.ResumeLayout(false);
      this.groupBox_DefaultView.PerformLayout();
      this.groupBox7.ResumeLayout(false);
      this.groupBox7.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.groupBoxView.ResumeLayout(false);
      this.groupBoxView.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.viewBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mFview)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.AntViewsImage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorViews)).EndInit();
      this.bindingNavigatorViews.ResumeLayout(false);
      this.bindingNavigatorViews.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgViewsList)).EndInit();
      this.groupBoxSortAndLayoutForView.ResumeLayout(false);
      this.groupBoxSortAndLayoutForView.PerformLayout();
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.AntViewIndex)).EndInit();
      this.groupBox24.ResumeLayout(false);
      this.groupBox24.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numUpDownTrailersInIntro)).EndInit();
      this.groupBox_AntSelectedEnreg.ResumeLayout(false);
      this.groupBox_AntSelectedEnreg.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox_PreLaunchingCommand.ResumeLayout(false);
      this.groupBox_PreLaunchingCommand.PerformLayout();
      this.groupBox_Security.ResumeLayout(false);
      this.groupBox_Security.PerformLayout();
      this.groupBox_TitleOrder.ResumeLayout(false);
      this.groupBox_TitleOrder.PerformLayout();
      this.groupBoxExtendedFieldHandling.ResumeLayout(false);
      this.groupBoxExtendedFieldHandling.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SPicture)).EndInit();
      this.groupBox_DVDprofiler.ResumeLayout(false);
      this.groupBox13.ResumeLayout(false);
      this.groupBox13.PerformLayout();
      this.groupBox_GrabberOptions.ResumeLayout(false);
      this.groupBox_GrabberOptions.PerformLayout();
      this.groupBox6.ResumeLayout(false);
      this.groupBox6.PerformLayout();
      this.groupBox_UpdateGrabberScripts.ResumeLayout(false);
      this.groupBox_UpdateGrabberScripts.PerformLayout();
      this.groupBoxMultiUserState.ResumeLayout(false);
      this.groupBoxMultiUserState.PerformLayout();
      this.Tab_Trakt.ResumeLayout(false);
      this.groupBoxExternal.ResumeLayout(false);
      this.groupBoxExternal.PerformLayout();
      this.groupBoxTrakt.ResumeLayout(false);
      this.groupBoxTrakt.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.Tab_Display.ResumeLayout(false);
      this.Tab_Views.ResumeLayout(false);
      this.Tab_Trailer.ResumeLayout(false);
      this.Tab_General.ResumeLayout(false);
      this.Tab_General.PerformLayout();
      this.groupBox_PlayMovieInfos.ResumeLayout(false);
      this.groupBox_PlayMovieInfos.PerformLayout();
      this.groupBoxMoviePathInfos.ResumeLayout(false);
      this.groupBoxMoviePathInfos.PerformLayout();
      this.Tab_ExternalCatalogs.ResumeLayout(false);
      this.groupBoxAMCsettings.ResumeLayout(false);
      this.groupBoxAMCsettings.PerformLayout();
      this.Tab_Network.ResumeLayout(false);
      this.Tab_Logos.ResumeLayout(false);
      this.Tab_Logos.PerformLayout();
      this.Tab_Artwork.ResumeLayout(false);
      this.Tab_AMCupdater.ResumeLayout(false);
      this.Tab_Update.ResumeLayout(false);
      this.General.ResumeLayout(false);
      this.Tab_About.ResumeLayout(false);
      this.Tab_About.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTMDBlogo)).EndInit();
      this.groupBoxSupportedCatalogs.ResumeLayout(false);
      this.groupBoxSupportedCatalogs.PerformLayout();
      this.groupBoxAbout.ResumeLayout(false);
      this.groupBoxAbout.PerformLayout();
      this.Tab_Other.ResumeLayout(false);
      this.Tab_Other.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.personDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.personBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.antMovieCatalog)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.propertiesBindingSource)).EndInit();
      this.Tab_OldStuff.ResumeLayout(false);
      this.groupBox8.ResumeLayout(false);
      this.groupBox8.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMyFilms)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.customFieldsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.indexNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private Label label_VersionNumber;
        private TextBox textBoxPluginName;
        private Label label11;
        private ComboBox Config_Name;
        private Label label12;
        private Button ButDelet;
        private Button ButQuit;
        //private ComboBox AntFilterMinRating;
        private Button ButSave;
        private CheckBox Config_Dflt;
        private CheckBox Config_Menu;
        private CheckBox checkBox1;
        private Label label21;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private TextBox textBox2;
        private ToolTip ToolTip1;
        private PictureBox pictureBoxMyFilms;
        private Button ButCopy;
        private Button btnLaunchAMCglobal;
        private TabPage Tab_Trakt;
        private TabPage Tab_ExternalCatalogs;
        private GroupBox groupBox9;
        private CheckBox chkDVDprofilerMergeWithGenreField;
        private GroupBox groupBox_DVDprofiler;
        private GroupBox groupBox13;
        private CheckBox chkDVDprofilerOnlyFile;
        private TabPage Tab_Network;
        private GroupBox groupBox25;
        private Label label37;
        private ComboBox comboWOLtimeout;
        private Button buttonSendMagicPacket3;
        private Button buttonSendMagicPacket2;
        private Label label30;
        private Button buttonSendMagicPacket1;
        private Button buttonGetMACadresses;
        private Label label_NAS_Server_3_MAC;
        private TextBox NAS_MAC_3;
        private Label label_NAS_Server_3_Name;
        private TextBox NAS_Name_3;
        private Label label_NAS_Server_2_MAC;
        private TextBox NAS_MAC_2;
        private Label label_NAS_Server_2_Name;
        private TextBox NAS_Name_2;
        private Label label_NAS_Server_1_MAC;
        private TextBox NAS_MAC_1;
        private Label label_NAS_Server_1_Name;
        private CheckBox check_WOL_Userdialog;
        private CheckBox check_WOL_enable;
        private TextBox NAS_Name_1;
        private Label label36;
        private TabPage Tab_Logos;
        private Button btnUpdate;
        private Button btnLogoClearCache;
        private Label lblLogoPresets;
        private Label lbl_LogoSpacing;
        private ComboBox comboBoxLogoPresets;
        private ComboBox comboBoxLogoSpacing;
        private Label lblSetupLogoRules;
        private Label lblInfoLogosForAll;
        private Label lblSelectLogoFile;
        private Button btnLogosPath;
        private TextBox txtLogosPath;
        private TextBox SFilePicture;
        private Label lblLogosPath;
        private ListView LogoView;
        private ColumnHeader LogoType;
        private ColumnHeader Field1;
        private ColumnHeader Op1;
        private ColumnHeader Value1;
        private ColumnHeader And_Or;
        private ColumnHeader Field2;
        private ColumnHeader Op2;
        private ColumnHeader Value2;
        private ColumnHeader Image;
        private ColumnHeader PathImage;
        private Button btnDown;
        private Button btnUp;
        private Button btnDel;
        private Button btnAdd;
        private ComboBox SAnd_Or;
        private ComboBox SValue2;
        private ComboBox SOp2;
        private ComboBox SField2;
        private ComboBox SValue1;
        private ComboBox SOp1;
        private ComboBox SField1;
        private CheckBox chkLogos;
        private ComboBox SLogo_Type;
        private PictureBox SPicture;
        private TabPage Tab_Artwork;
        private GroupBox groupBox_ArtistImages;
        private Button btnResetThumbsArtist;
        private CheckBox chkDfltArtist;
        private Button ButImgArtist;
        private TextBox MesFilmsImgArtist;
        private Label label_DefaultArtistImage;
        private Label label_ArtistImagePath;
        private GroupBox groupBox1;
        private Label label_DefaulCover;
        private Button ButImg;
        private TextBox MesFilmsImg;
        private Label label2;
        private GroupBox groupBox22;
        private Button btnResetThumbs;
        private CheckBox chkViews;
        private Button btnViews;
        private CheckBox chkDfltViews;
        private Label label29;
        private TextBox MesFilmsViews;
        private GroupBox Fanart;
        private Label label40;
        private CheckBox chkFanart;
        private Button btnFanart;
        private TextBox MesFilmsFanart;
        private Label labelFanart;
        private CheckBox chkDfltFanart;
        private TabPage Tab_AMCupdater;
        private GroupBox groupBox_AMCupdater;
        private GroupBox groupBox_AMCupdaterScheduer;
        private CheckBox scheduleAMCUpdater;
        private Button btnParameters;
        private TextBox txtAMCUpd_cnf;
        private Button btnAMCUpd_cnf;
        private GroupBox groupBox_AMCupdater_ExternalApplication;
        private Label lblAMCMovieScanPath;
        private CheckBox chkAMC_Purge_Missing_Files;
        private Button btnAMCMovieScanPathAdd;
        private TextBox AMCMovieScanPath;
        private Button btnCreateAMCDesktopIcon;
        private Button btnCreateAMCDefaultConfig;
        private CheckBox chkAMCUpd;
        private TabPage Tab_Update;
        private GroupBox groupBox2;
        private Label lblUnwatchedItemsValue;
        private TextBox textBoxGlobalUnwatchedOnlyValue;
        private CheckBox CheckWatchedPlayerStopped;
        private Label Label_UserProfileName;
        private TextBox UserProfileName;
        private Label label19;
        private ComboBox cbWatched;
        private CheckBox chkEnhancedWatchedStatusHandling;
        private CheckBox CheckWatched;
        private GroupBox groupBoxDeletionOptions;
        private CheckBox chkSuppressAutomatic;
        private GroupBox gpspfield;
        private CheckBox chkSuppressPlayStopUpdateField;
        private TextBox txtSuppressPlayStopValueToUpdate;
        private Label lblUpdateValue;
        private ComboBox cbSuppressPlayStopFieldToUpdate;
        private TabPage Tab_Display;
        private TabPage Tab_Views;
        private GroupBox groupBox_DefaultView;
        private CheckBox chkGlobalUnwatchedOnly;
        private ComboBox SortSens;
        private Label label32;
        private ComboBox Sort;
        private CheckBox AlwaysDefaultView;
        private Label label14;
        private ComboBox LayOut;
        private ComboBox View_Dflt_Item;
        private TabPage Tab_Trailer;
        private GroupBox groupBox24;
        private Label labelTrailers;
        private CheckBox ShowTrailerWhenStartingMovie;
        private Button btnTrailer;
        private TextBox PathStorageTrailer;
        private Label label34;
        private ComboBox AntStorageTrailer;
        private Label label35;
        private TabPage Tab_General;
        private Label lblYellowShowRequiredItems;
        private GroupBox groupBox_Security;
        private Label label16;
        private Label label15;
        private TextBox Rpt_Dwp;
        private TextBox Dwp;
        private GroupBox groupBox_PlayMovieInfos;
        private GroupBox groupBoxMoviePathInfos;
        private Label label5;
        private TextBox PathStorage;
        private CheckBox SearchSubDirs;
        private Button butPath;
        private CheckBox SearchFileName;
        private ComboBox ItemSearchFileName;
        private TextBox AntIdentLabel;
        private Label label6;
        private ComboBox AntStorage;
        private Label label4;
        private ComboBox AntIdentItem;
        private Label label13;
        private ComboBox CatalogType;
        private GroupBox groupBox_TitleOrder;
        private Label label20;
        private ComboBox AntSTitle;
        private Label label17;
        private TextBox TitleDelim;
        private Label label9;
        private Label label8;
        private ComboBox AntTitle2;
        private ComboBox AntTitle1;
        private Button ButCat;
        private TextBox MesFilmsCat;
        private Label label1;
        private TabControl General;
        private Button btnFirstTimeSetup;
        private Label lblResultingGroupViewsPathFanart;
        private Button butNew;
        private Label label47;
        private Label label48;
        private CheckBox chkDfltFanartImage;
        private CheckBox chkPersons;
        private TextBox textBoxNBconfigs;
        private Label lblNbConfig;
        private CheckBox chkDfltViewsAll;
        private CheckBox chkDfltFanartImageAll;
        private CheckBox chkFanartDefaultViews;
        private Label lblPicturePrefix;
        private TextBox txtPicturePrefix;
        private Label lblPictureHandling;
        private ComboBox cbPictureHandling;
        private GroupBox groupBoxExtendedFieldHandling;
        private CheckBox chkAddTagline;
        private CheckBox chkAddCertification;
        private ComboBox ECMergeDestinationFieldTagline;
        private CheckBox chkAddTags;
        private Label label53;
        private Label label54;
        private ComboBox ECMergeDestinationFieldCertification;
        private ComboBox ECMergeDestinationFieldTags;
        private ComboBox ECMergeDestinationFieldWriter;
        private CheckBox chkAddWriter;
        private Button buttonDeleteTmpCatalog;
        private Button buttonOpenTmpFile;
        private Button buttonOpenTmpFileAMC;
        private GroupBox groupBoxAMCsettings;
        private TextBox AMCexePath;
        private Label lblAMCpath;
        private Button buttonAMCpathSearch;
        private CheckBox chkScanMediaOnStart;
        private CheckBox cbAllowTraktSync;
        private CheckBox SearchOnlyExactMatches;
        private ComboBox AmcTitleSearchHandling;
        private Label label56;
        private TextBox txtAMCUpd_cnf_Display;
        private LinkLabel linkLabelTrakt;
        private TabPage Tab_Other;
        private Label lblAMCupdaterConfigPreview;
        private ListView AMCConfigView;
        private ColumnHeader Option;
        private ColumnHeader Value;
        private Label labelTraktDescription;
        private PictureBox pictureBox1;
        private CheckBox chkGlobalAvailableOnly;
        private Button btnWatchedImport;
        private Button btnWatchedExport;
        private CheckBox chkFanartDefaultViewsUseRandom;
        private GroupBox groupBoxTrakt;
        private LinkLabel linkLabelTraktWiki;
        private LinkLabel linkLabelUsingTraktInMyFilmsWiki;
        private CheckBox chkSuppressManual;
        private Label label61;
        private Button btnHyperLinkParamGen;
        private GroupBox groupBoxExternal;
        private Label labelRecentlyAddedAPI;
        private CheckBox cbAllowRecentAddedAPI;
        private AntMovieCatalog antMovieCatalog;
        private BindingSource propertiesBindingSource;
        private TextBox ownerTextBox;
        private TextBox mailTextBox;
        private TextBox siteTextBox;
        private TextBox descriptionTextBox;
        private BindingSource customFieldBindingSource;
        private BindingSource customFieldsBindingSource;
        private DataGridView customFieldDataGridView;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridView personDataGridView;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn2;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn3;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private BindingSource personBindingSource;
        private Button btnEditView;
        private Label label52;
        private Label label33;
        private Label label18;
        private NumericUpDown indexNumericUpDown;
        private TextBox labelTextBox;
        private TextBox valueTextBox;
        private TextBox filterTextBox;
        private CheckBox showEmptyCheckBox;
        private CheckBox reverseNamesCheckBox;
        private CheckBox sortOcurrenciesCheckBox;
        private ComboBox sortDirectionViewComboBox;
        private ComboBox layoutViewComboBox;
        private ComboBox sortFieldHierarchyComboBox;
        private ComboBox sortDirectionHierarchyComboBox;
        private ComboBox layoutHierarchyComboBox;
        private ComboBox sortFieldFilmsComboBox;
        private ComboBox sortDirectionFilmsComboBox;
        private ComboBox layoutFilmsComboBox;
        private BindingSource viewBindingSource;
        private MFview mFview;
        private TabPage Tab_OldStuff;
        private GroupBox groupBoxView;
        private BindingNavigator bindingNavigatorViews;
        private ToolStripButton toolStripButtonAdd;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton toolStripButtonDelete;
        private ToolStripButton toolStripButton3;
        private ToolStripButton toolStripButton4;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButton5;
        private ToolStripButton toolStripButton6;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton toolStripButtonMoveUp;
        private ToolStripButton toolStripButtonMoveDown;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton toolStripButtonAddDefaults;
        private DataGridView dgViewsList;
        private DataGridViewTextBoxColumn labelDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn viewEnabledDataGridViewCheckBoxColumn;
        private GroupBox groupBoxSortAndLayoutForView;
        private ComboBox AntViewSortType;
        private ComboBox AntViewLayoutView;
        private ComboBox AntViewSortOrder;
        private ComboBox AntViewItem;
        private GroupBox groupBox5;
        private TextBox AntViewFilter;
        private Button AntViewFilterEditButton;
        private TextBox AntViewValue;
        private NumericUpDown AntViewIndex;
        private Label label10;
        private GroupBox groupBox4;
        private ComboBox LayoutInHierarchies;
        private Label label62;
        private ComboBox SortSensInHierarchies;
        private ComboBox SortInHierarchies;
        private GroupBox groupBox7;
        private Label label26;
        private Label lblAntViewIndex;
        private GroupBox groupBox_Separators;
        private ComboBox RoleSeparator5;
        private ComboBox RoleSeparator4;
        private ComboBox ListSeparator5;
        private ComboBox ListSeparator4;
        private ComboBox ListSeparator3;
        private ComboBox ListSeparator2;
        private ComboBox RoleSeparator2;
        private ComboBox RoleSeparator3;
        private Label label22;
        private ComboBox RoleSeparator1;
        private Label label24;
        private ComboBox ListSeparator1;
        private CheckBox chkOnlyTitle;
        private GroupBox groupBox_GrabberOptions;
        private Label lblFilterGrabberScripts;
        private TextBox ItemSearchGrabberScriptsFilter;
        private TextBox txtGrabberDisplay;
        private CheckBox chkGrabber_Always;
        private Button btnEditScript;
        private GroupBox groupBox6;
        private ComboBox cbGrabberOverrideGetRoles;
        private Label label55;
        private Label label51;
        private ComboBox cbGrabberOverrideTitleLimit;
        private Label label50;
        private Label label49;
        private ComboBox cbGrabberOverridePersonLimit;
        private ComboBox cbGrabberOverrideLanguage;
        private Label lblSearchGrabberName;
        private CheckBox chkGrabber_ChooseScript;
        private ComboBox ItemSearchGrabberName;
        private Button btnGrabber;
        private TextBox txtGrabber;
        private Label label27;
        private Button buttonWikiHelp;
        private GroupBox groupBox3;
        private TextBox textBoxStrDfltSelect;
        private Button btnCustomConfigFilter;
        private GroupBox groupBox_PreLaunchingCommand;
        private Label label39;
        private Label label38;
        private ComboBox CmdPar;
        private TextBox CmdExe;
        private GroupBox groupBox_AntSelectedEnreg;
        private Label AntFreetextFilterItem;
        private TextBox AntFilterFreeText;
        private ComboBox AntFilterComb;
        private ComboBox AntFilterSign2;
        private ComboBox AntFilterSign1;
        private ComboBox AntFilterItem2;
        private TextBox AntFilterText2;
        private ComboBox AntFilterItem1;
        private TextBox AntFilterText1;
        private GroupBox groupBox_UserItemsMain;
        private TextBox AntLabel5;
        private TextBox AntLabel4;
        private ComboBox AntItem5;
        private ComboBox AntItem4;
        private TextBox AntLabel3;
        private ComboBox AntItem3;
        private ComboBox AntItem2;
        private TextBox AntLabel2;
        private Label label_MainDBitem;
        private ComboBox AntItem1;
        private Label labelMainLabel;
        private TextBox AntLabel1;
        private TextBox View_Dflt_Text;
        private CheckBox chkShowEmpty;
        private ComboBox cbSuppressAutomaticAction;
        private PictureBox AntViewsImage;
        private Button buttonResetImage;
        private Label labelViewLabel;
        private PictureBox pictureBoxDefaultFanart;
        private PictureBox pictureBoxDefaultViewImage;
        private PictureBox pictureBoxDefaultCover;
        private PictureBox pictureBoxDefaultPersonImage;
        private Button buttonDefaultCoverReset;
        private Button buttonDefaultFanartReset;
        private Button buttonDefaultViewImageReset;
        private Button buttonDefaultPersonImageReset;
        private CheckBox chkShowIndexedImgInIndViews;
        private GroupBox groupBoxAMCUpdaterConfigFile;
        private Button btnServerSync;
        private Button btnResetThumbsFilms;
        private CheckBox cbTrailerAutoregister;
        private CheckBox chkReversePersonNames;
        private Label label23;
        private Button butExternalPlayer;
        private TextBox tbExternalPlayerExtensions;
        private TextBox tbExternalPlayerPath;
        private TextBox tbExternalPlayerStartParams;
        private CheckBox chkPersonsEnableDownloads;
        private GroupBox groupBox_UserItemsDetails;
        private Label label_DetailsLabel;
        private Label label_DetailsDBitem;
        private Label label_UserItemDetails5;
        private Label label_UserItemDetails4;
        private Label label_UserItemDetails3;
        private Label label_UserItemDetails2;
        private Label label_UserItemDetails1;
        private TextBox AntLabelDetails5;
        private TextBox AntLabelDetails4;
        private TextBox AntLabelDetails3;
        private TextBox AntLabelDetails2;
        private TextBox AntLabelDetails1;
        private ComboBox AntItemDetails5;
        private ComboBox AntItemDetails4;
        private ComboBox AntItemDetails3;
        private ComboBox AntItemDetails2;
        private ComboBox AntItemDetails1;
        private Label label_UserItem5;
        private Label label_UserItem4;
        private Label label_UserItem3;
        private Label label_UserItem2;
        private Label label_UserItem1;
        private TextBox AntLabelDetails6;
        private ComboBox AntItemDetails6;
        private Label label_UserItemDetails6;
        private GroupBox groupBox_UpdateGrabberScripts;
        private TextBox textBoxUpdateGrabberScripts;
        private ProgressBar progressBarUpdateGrabberScripts;
        private Button buttonUpdateGrabberScripts;
        private Label label_UpdateGrabberScriptsInstructions;
        private Button button_DeleteBackupScripts;
        private GroupBox groupBox8;
        private Label label25;
        private Label label7;
        private Label label3;
        private ComboBox LayOutViewFilms;
        private ComboBox SortSensViewFilms;
        private ComboBox SortViewFilms;
        private CheckBox chkVirtualPathBrowsing;
        private Button btnMUSdeleteUserData;
        private GroupBox groupBoxMultiUserState;
        private TabPage Tab_About;
        private GroupBox groupBoxSupportedCatalogs;
        private Label label41;
        private GroupBox groupBoxAbout;
        private Label label28;
        private CheckBox cbCacheOnlineTrailer;
        private CheckBox Config_EnablePreload;
        private Button btnAMCimport;
        private NumericUpDown numUpDownTrailersInIntro;
        private CheckBox chkAMC_ImportOnInternetFail;
        private CheckBox chkAMCUwatchScanFolders;
        private CheckBox chkAMCUscanOnStartup;
        private NumericUpDown numericUpDownAMCUscanStartDelay;
        private Label labelAMCUstartdelay;
        private TextBox AntViewFilterSaveString;
        private Button btnTestMode;
        private Label lblCountry;
        private TextBox txtCountry;
        private PictureBox pictureBoxTMDBlogo;
        private Label lblTmdbNote;
    }
}