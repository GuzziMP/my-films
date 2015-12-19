#region GNU license
// MyFilms - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

namespace MyFilmsPlugin.MyFilms.Configuration
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Reflection;
  using System.Threading;
  using System.Windows.Forms;
  using System.Windows.Data;
  using System.Xml;

  using Grabber;

  using IWshRuntimeLibrary;

  using MediaPortal.Configuration;

  using MyFilmsPlugin.Configuration;
  using MyFilmsPlugin.DataBase;
  using MyFilmsPlugin.MyFilms.CatalogConverter;
  using MyFilmsPlugin.MyFilms.MyFilmsGUI;
  using MyFilmsPlugin.MyFilms.Utils;

  using TaskScheduler;

  using AntMovieCatalog = MyFilmsPlugin.DataBase.AntMovieCatalog;

  public partial class MyFilmsSetup : Form
  {
    //private WshShellClass WshShell; // Added for creating Desktop icon via wsh
    XmlConfig XmlConfig = new XmlConfig();  // XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")); // XmlConfig XmlConfig = new XmlConfig();
    //XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")); //Guzzi

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    static ScheduledTasks st = null;

    private OpenFileDialog openFileDialog1;
    private FolderBrowserDialog folderBrowserDialog1;
    private int MesFilms_nb_config = 0;
    private string MyFilms_PluginMode = String.Empty; // "normal" for standard operations, "test" for extended functionalities, to be set manually in MyFilms.XML
    private string StrDfltSelect = string.Empty;
    private AntMovieCatalog mydivx = new AntMovieCatalog();
    private MFview MyCustomViews = new MFview();

    private Crypto crypto = new Crypto();
    public int selected_Logo_Item;
    public bool load = true;
    public DataSet AMCdsSettings = new DataSet();
    TabPage tabPageSave = null; // Zwischenspeicher für TabPage
    private bool StoreFullLogoPath = false;

    private bool WizardActive = false; // Status of running new config wizard (to control check behaviour)
    private bool RunWizardAfterInstall = false; // Will only be set true after first blank install, when no MyFilms.xml config is present to launch Wizard.
    private bool NewConfigButton = false; // Will avid that catalogselectedindex-changed will be run on "New!" config...
    private string ActiveLogoPath = String.Empty;

    public MyFilmsSetup()
    {
      InitializeComponent();
      System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
      label_VersionNumber.Text = "Version " + asm.GetName().Version;
    }

    private void MesFilmsSetup_Load(object sender, EventArgs e)
    {
      XmlConfig = new XmlConfig(); // clear cache
      Config_Name.Items.Clear(); // clear entries in config dropdown befoer loading/adding new ones ...
      Refresh_Items(true);
      if (!System.IO.File.Exists(Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml"))
        RunWizardAfterInstall = true;
      textBoxPluginName.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginName", "Films");
      MyFilms_PluginMode = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginMode", "normal"); // Read Plugin Start Mode to diable/enable normal vs. testfeatures
      LogMyFilms.Info("MyFilms Setup ********** OperationsMode (PluginMode): '" + MyFilms_PluginMode + "' **********");

      #region hide elements from GUI for test mode
      if (MyFilms_PluginMode == "normal" || MyFilms_PluginMode == "test") // 'extended' or dev settings
      {
        //hide a tab by removing it from the TabPages collection
        if (General.TabPages.Count > 13)
        {
          tabPageSave = General.TabPages[13];
          General.TabPages.Remove(tabPageSave); // Disable "Old Stuff" Tab, as it has stuff not for public
        }
        if (General.TabPages.Count > 12)
        {
          tabPageSave = General.TabPages[12];
          General.TabPages.Remove(tabPageSave); // Disable "Other" Tab, as it has stuff not for public
        }

        ShowTrailerWhenStartingMovie.Visible = false;
        numUpDownTrailersInIntro.Visible = false;

      }
      
      if (MyFilms_PluginMode == "normal")
      {
        //this.tabPageSave = General.TabPages[11];
        //this.General.TabPages.Remove(tabPageSave); // Disable "About" Tab

        // cbTrailerAutoregister.Visible = false;
        // cbCacheOnlineTrailer.Visible = false; // local caching for trailersof TMDB online content

        buttonOpenTmpFile.Visible = false; // disable button to open tmp catalog in editor on EC tab
        buttonDeleteTmpCatalog.Visible = false; // disable button to delete tmp catalog on EC tab
        groupBoxAMCsettings.Visible = false; // disable groupbox with setting for AMC exe path
        buttonOpenTmpFileAMC.Visible = false; // disable Launch Button to start AMC with Catalogs externally
        // chkVirtualPathBrowsing.Visible = false; // disable global option to use virtual path browsing
        
        // Remove unused Catalog types -- also changes index, so doesn't work with existing code !
        //CatalogType.Items.Remove("MovingPicturesXML (V1.2 process plugin)");
        //CatalogType.Items.Remove("Ant Movie Catalog Xtended (V4.1)");
        CatalogType.Items.Remove("Mediabrowser 3");
        CatalogType.Items.Remove("XBMC nfo reader");
        //CatalogType.Items.Remove(CatalogType.Items[7]); // MF internal DB
        //CatalogType.Items.RemoveAt(8); // XBMC nfo reader (deparate files)
        //CatalogType.Items.Add("test");

        //Config_EnablePreload.Visible = false; // precaching of config and DB on Init phase
        //butNew.Visible = false; // disable "new" button for normal mode for V6.0.1

        //// autostart settings for AMCupdater background Launcher:
        //chkAMCUscanOnStartup.Visible = false;
        //chkAMCUwatchScanFolders.Visible = false;
        //labelAMCUstartdelay.Visible = false;
        //numericUpDownAMCUscanStartDelay.Visible = false;
        btnTestMode.ResetBackColor();
      }
      else if (MyFilms_PluginMode == "test")
      {

        btnTestMode.BackColor = System.Drawing.Color.OrangeRed;
      }
      else // 'extended' or dev settings
      {
        //  //show a tab by adding it to the TabPages collection
        //  if (this.tabPageSave != null)
        //  {
        //    int loc = General.SelectedIndex;
        //    this.General.TabPages.Insert(loc, this.tabPageSave);
        //  }
      }
      #endregion

      //// load about labels
      //LogMyFilms.Info("MyFilms.Init() started on '" + System.Environment.MachineName + "'.");
      //LogMyFilms.Info("MyFilms     Version: 'V" + MyFilmsSettings.Version + "', BuildDate: '" + MyFilmsSettings.BuildDate + "'");
      //LogMyFilms.Info("MediaPortal Version: 'V" + MyFilmsSettings.MPVersion + "',    BuildDate: '" + MyFilmsSettings.MPBuildDate + "'");
      //LogMyFilms.Info("MyFilms Skin Interface Version: 'V" + MyFilms.SkinInterfaceVersionMajor + "." + MyFilms.SkinInterfaceVersionMinor + "'");

      LogMyFilms.Info("MyFilms Setup: Started with version '" + label_VersionNumber.Text + "'");

      MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
      //for (int i = 0; i < (int)MesFilms_nb_config; i++)
      //{
      //  // Config_Name.Items.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, ""));
      //  XmlConfig.RemoveEntry("MyFilms", "MyFilms", "ConfigName" + i);
      //}
      if (MesFilms_nb_config > 0)
      {
        Config_Menu.Checked = (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false));
      }
      for (int i = 0; i < MesFilms_nb_config; i++)
      {
        Config_Name.Items.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
      }

      var ds = new AntMovieCatalog();
      AntStorage.Items.Add("(none)");
      AntStorageTrailer.Items.Add("(none)");
      AntTitle2.Items.Add("(none)");
      Sort.Items.Add("(none)");
      SortInHierarchies.Items.Add("(none)");
      AntIdentItem.Items.Add("(none)");
      AntFilterItem1.Items.Add("(none)");
      AntFilterItem2.Items.Add("(none)");
      //AntViewItem.Items.Add("(none)"); // removed, as we do not want to have "none" views defined - they can be disabled instead, as they are handled dynamic
      AntItem1.Items.Add("(none)");
      AntItem2.Items.Add("(none)");
      AntItem3.Items.Add("(none)");
      AntItem4.Items.Add("(none)");
      AntItem5.Items.Add("(none)");
      AntItemDetails1.Items.Add("(none)");
      AntItemDetails2.Items.Add("(none)");
      AntItemDetails3.Items.Add("(none)");
      AntItemDetails4.Items.Add("(none)");
      AntItemDetails5.Items.Add("(none)");
      AntItemDetails6.Items.Add("(none)");
      CmdPar.Items.Add("(none)");
      CatalogType.SelectedIndex = 0;
      #region add dropdown content for AMC3
      foreach (DataColumn dc in ds.Movie.Columns)
      {
        if ((dc.ColumnName != "Contents_Id" && dc.ColumnName != "Movie_Id" && dc.ColumnName != "IsOnline" && dc.ColumnName != "IsOnlineTrailer" &&
             dc.ColumnName != "LastPosition" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "MultiUserState" &&
             dc.ColumnName != "AlternateTitles" && dc.ColumnName != "VirtualPathTitle" && dc.ColumnName != "CollectionImage")
            && (CatalogType.SelectedIndex != 0 ||
            (dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" && dc.ColumnName != "Watched" && dc.ColumnName != "Certification" &&
             dc.ColumnName != "Writer" && dc.ColumnName != "SourceTrailer" && dc.ColumnName != "TagLine" && dc.ColumnName != "Tags" &&
             dc.ColumnName != "RatingUser" && dc.ColumnName != "Studio" && dc.ColumnName != "IMDB_Rank" && dc.ColumnName != "Edition" &&
             dc.ColumnName != "Aspectratio" && dc.ColumnName != "CategoryTrakt" && dc.ColumnName != "Favorite" &&
             dc.ColumnName != "CustomField1" && dc.ColumnName != "CustomField2" && dc.ColumnName != "CustomField3" && dc.ColumnName != "DateWatched" &&
             dc.ColumnName != "DateReleased" && dc.ColumnName != "Budget" && dc.ColumnName != "Composer" && dc.ColumnName != "S3D" && dc.ColumnName != "Collection" && dc.ColumnName != "CollectionDetails" && dc.ColumnName != "Votes"))
          )
        {
          if (dc.ColumnName == "MediaLabel" || dc.ColumnName == "MediaType" || dc.ColumnName == "Source" || (dc.ColumnName == "SourceTrailer" && CatalogType.SelectedIndex == 10) ||
              dc.ColumnName == "URL" || dc.ColumnName == "Comments" || dc.ColumnName == "Borrower" ||
              dc.ColumnName == "Languages" || dc.ColumnName == "Subtitles")
          {
            AntStorage.Items.Add(dc.ColumnName);
            AntStorageTrailer.Items.Add(dc.ColumnName);
          }

          //if (dc.ColumnName == "OriginalTitle" || dc.ColumnName == "TranslatedTitle" || dc.ColumnName == "FormattedTitle" || dc.ColumnName == "Year" ||
          //    dc.ColumnName == "Date" || dc.ColumnName == "Rating")
          //{
          //  Sort.Items.Add(dc.ColumnName);
          //  SortInHierarchies.Items.Add(dc.ColumnName);
          //}

          if (dc.ColumnName == "TranslatedTitle" || dc.ColumnName == "OriginalTitle" || dc.ColumnName == "FormattedTitle") // ToDo: Add "VirtualPathTitle" to selection
          {
            //AntTitle1.Items.Add(dc.ColumnName); // Fields already added in Controls definition
            AntTitle2.Items.Add(dc.ColumnName);
            AntSTitle.Items.Add(dc.ColumnName);
          }
          if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded")
          {
            AntFilterItem1.Items.Add(dc.ColumnName);
            AntFilterItem2.Items.Add(dc.ColumnName);
            AntItem1.Items.Add(dc.ColumnName);
            AntItem2.Items.Add(dc.ColumnName);
            AntItem3.Items.Add(dc.ColumnName);
            AntItem4.Items.Add(dc.ColumnName);
            AntItem5.Items.Add(dc.ColumnName);
            AntItemDetails1.Items.Add(dc.ColumnName);
            AntItemDetails2.Items.Add(dc.ColumnName);
            AntItemDetails3.Items.Add(dc.ColumnName);
            AntItemDetails4.Items.Add(dc.ColumnName);
            AntItemDetails5.Items.Add(dc.ColumnName);
            AntItemDetails6.Items.Add(dc.ColumnName);
          }
          if (dc.ColumnName != "OriginalTitle" && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "IndexedTitle" && dc.ColumnName != "AlternateTitles" &&
              dc.ColumnName != "Comments" && dc.ColumnName != "Description" &&
              dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && dc.ColumnName != "Rating" &&
              dc.ColumnName != "URL" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "DateWatched" && dc.ColumnName != "DateReleased")
          {
            SField1.Items.Add(dc.ColumnName);
            SField2.Items.Add(dc.ColumnName);
          }
          if (dc.ColumnName != "Description" && dc.ColumnName != "Comments") //  && dc.ColumnName != "Number" && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle"  && dc.ColumnName != "AlternateTitles" && dc.ColumnName != "DateWatched"
          {
            AntViewItem.Items.Add(dc.ColumnName);
          }
          if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
              dc.ColumnName != "Year" && dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && // disabled for Doug testing
              dc.ColumnName != "Length" && dc.ColumnName != "Rating" &&
              dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle" && dc.ColumnName != "AlternateTitles" && 
              dc.ColumnName != "DateWatched" && dc.ColumnName != "DateReleased" && dc.ColumnName != "S3D")
          {
            AntIdentItem.Items.Add(dc.ColumnName);
          }
          if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle" && dc.ColumnName != "AlternateTitles" && 
              dc.ColumnName != "DateWatched" && dc.ColumnName != "DateReleased" && dc.ColumnName != "S3D")
          {
            cbSuppressPlayStopFieldToUpdate.Items.Add(dc.ColumnName);
            cbWatched.Items.Add(dc.ColumnName);
            CmdPar.Items.Add(dc.ColumnName);
          }
        }
      }
      #endregion
      AntViewText_Change();
      AntSort_Change();
      Config_Name.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
      chkLogos.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Logos", true); // Changed default to "true" - hope, it works even without fiull working config ....

      // Show number of configs available ...
      textBoxNBconfigs.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", string.Empty);

      st = new ScheduledTasks();
      Task t = null;
      string name = string.Empty;
      if (MesFilmsCat.Text.LastIndexOf("\\") > 0)
      {
        name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
        name = name.Substring(0, name.Length - 4);
      }
      try { t = st.OpenTask("MyFilms_AMCUpdater_" + name); }
      catch (ArgumentException) { }
      scheduleAMCUpdater.Checked = (t != null);
      load = false;
      // Now start SetupWizard, if it's a clean new install
      if (RunWizardAfterInstall)
      {
        WizardActive = true;
        Thread.Sleep(100);
        newCatalogWizard();
        RunWizardAfterInstall = false;
        WizardActive = false;
      }
      else
      {
        LoadCentralConfigSetupAndUpdateVisibility();  // check, if remote server sync is enabled
      }
    }


    private void LoadCentralConfigSetupAndUpdateVisibility()
    {
      var myFilmsServer = new XmlConfig();
      string centralConfigDir = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", "");
      bool syncFromServerOnStartup = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
      if (syncFromServerOnStartup && centralConfigDir.Length > 0)
      {
        SetLocalConfigVisibility(false);
      }
      else
      {
        SetLocalConfigVisibility(true);
      }
    }

    private void SetLocalConfigVisibility(bool visibility)
    {
      if (visibility)
        btnServerSync.ResetBackColor();
      else
        btnServerSync.BackColor = System.Drawing.Color.DarkSeaGreen;
      textBoxPluginName.Enabled = visibility;
      Config_Menu.Enabled = visibility;
      Config_Name.Enabled = visibility;
      Config_Dflt.Enabled = visibility;
      General.Enabled = visibility;
      btnLaunchAMCglobal.Enabled = visibility;
      btnFirstTimeSetup.Enabled = visibility;
      butNew.Enabled = visibility;
      ButSave.Enabled = visibility;
      ButCopy.Enabled = visibility;
      ButDelet.Enabled = visibility;
    }

    private void ButCat_Click(object sender, System.EventArgs e)
    {
      if (!string.IsNullOrEmpty(MesFilmsCat.Text))
        openFileDialog1.FileName = MesFilmsCat.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        //if (System.IO.Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath) + @"\Catalog\"))
        //  openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath) + @"\Catalog\";
        //else
        // openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\";
        openFileDialog1.InitialDirectory = "";
      }

      if (MesFilmsCat.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "xml";
      openFileDialog1.Filter = "XML Files|*.xml";
      openFileDialog1.Title = "Select Movie Catalog File (xml)";
      openFileDialog1.CheckFileExists = false;
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        Control_Database(openFileDialog1.FileName);
      }
    }
    private void Control_Database(string filename)
    {
      if (!System.IO.File.Exists(filename))
      {
        //Ant Movie Catalog (V3.5.1.2)
        //DVD Profiler
        //Movie Collector
        //MyMovies
        //Eax Movie Catalog
        //eXtreme Movie Manager (V7.1.0.2)
        //XBMC (V10.0)
        //MyFilms DB (ANT with extended Database Fields)
        if (IsAMCcatalogType(CatalogType.SelectedIndex) || CatalogType.SelectedIndex == 11) // AMC, AMCextended or XBMC NFO reader
        {
          if (MessageBox.Show("That File doesn't exists, do you want to create it ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            var destXml = new XmlTextWriter(filename, System.Text.Encoding.Default);
            destXml.Formatting = Formatting.Indented;
            destXml.WriteStartDocument();
            destXml.WriteStartElement("AntMovieCatalog");
            destXml.WriteStartElement("Catalog");
            destXml.WriteElementString("Properties", string.Empty);
            destXml.WriteStartElement("Contents");
            destXml.Close();
          }
          else
          {
            MessageBox.Show("You have to select a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            MesFilmsCat.Focus();
            return;
          }
        }
        else
        {

          MessageBox.Show("You have to selected a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          MesFilmsCat.Focus();
          return;

        }
      }
      MesFilmsCat.Text = filename;
      if (MesFilmsImg.Text.Length == 0)
        MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf(@"\"));

    }

    private void ButImg_Click(object sender, EventArgs e)
    {
      this.folderBrowserDialog1.SelectedPath = !String.IsNullOrEmpty(this.MesFilmsImg.Text) ? this.MesFilmsImg.Text : String.Empty;
      folderBrowserDialog1.Description = "Select Cover Images Path";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        MesFilmsImg.Text = folderBrowserDialog1.SelectedPath;
        if (!string.IsNullOrEmpty(MesFilmsCat.Text))
        {
          string catalogDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
          if (MesFilmsImg.Text == catalogDirectory && IsAMCcatalogType(CatalogType.SelectedIndex))  // only if not external catalog
            cbPictureHandling.Text = "Relative Path";
          else
            cbPictureHandling.Text = "Full Path"; // set as default, unless other is possible
          if (MesFilmsImg.Text.StartsWith(catalogDirectory) && MesFilmsImg.Text != catalogDirectory && IsAMCcatalogType(CatalogType.SelectedIndex)) // only if not external catalog
          {
            MesFilmsImg.Text = catalogDirectory;
            cbPictureHandling.Text = "Relative Path";
            if (string.IsNullOrEmpty(txtPicturePrefix.Text))
            {
              txtPicturePrefix.Text = "";
              txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\";
            }
            else if (!txtPicturePrefix.Text.Contains("\\"))
            {
              txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\" + txtPicturePrefix.Text;
            }
            else
            {
              string imgPrefixFilenameOnly = txtPicturePrefix.Text.Substring(txtPicturePrefix.Text.LastIndexOf("\\") + 1);
              txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\" + imgPrefixFilenameOnly;
            }
          }
          else if (txtPicturePrefix.Text.Contains("\\"))
          {
            txtPicturePrefix.Text = txtPicturePrefix.Text.Substring(txtPicturePrefix.Text.LastIndexOf("\\") + 1);
          }
        }
      }
    }
    // Display any warnings or errors.

    private void ButSave_Click(object sender, EventArgs e)
    {
      // update AMCupdater config with latest MyFilms settings
      // Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file // reading already done in "load MF settings)
      CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
      Save_XML_AMCconfig(Config_Name.Text); // save new config

      // Save MF config
      Save_Config();
      // XmlSettings.SaveCache();
      Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults
    }

    private void Save_Config()
    {
      #region sanity checks
      if (Config_Name.Text.Length == 0)
      {
        MessageBox.Show("The Configuration's Name is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        Config_Name.Focus();
        return;
      }

      foreach (MFview.ViewRow viewRow in Enumerable.Where(this.MyCustomViews.View, viewRow => string.IsNullOrEmpty(viewRow.DBfield)))
      {
        MessageBox.Show("No DB field is set for Custom View '" + viewRow.Label + "' - this is required - delete that view or add missing config!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        return;
      }

      if (Config_Dflt.Checked && Config_Menu.Checked)
      {
        MessageBox.Show("Option 'Always Display Configuration Menu' not possible with a Default Configuration defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        Config_Menu.Focus();
        return;
      }
      //if (textBoxPluginName.Text.Length == 0)
      //{
      //    MessageBox.Show("The Plugin's Name is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //    textBoxPluginName.Focus();
      //    return;
      //}
      if (AntTitle1.Text.Length == 0)
      {
        MessageBox.Show("The Master Title is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntTitle1.Focus();
        return;
      }
      if ((SearchFileName.Checked) && ItemSearchFileName.Text.Length == 0)
      {
        MessageBox.Show("The Field used for searching by Movie's File Name is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        ItemSearchFileName.Focus();
        return;
      }
      if (!string.IsNullOrEmpty(txtGrabber.Text) && ItemSearchGrabberName.Text.Length == 0)
      {
        MessageBox.Show("The Field used for searching internet data by Movie's Name is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        General.SelectedIndex = 5;
        ItemSearchGrabberName.Focus();
        return;
      }
      if (!string.IsNullOrEmpty(txtGrabber.Text) && string.IsNullOrEmpty(cbPictureHandling.Text))
      {
        MessageBox.Show("The Field used for selecting if cover image name to be stored with relative or absolute path is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        General.SelectedIndex = 6;
        cbPictureHandling.Focus();
        return;
      }

      if (View_Dflt_Item.Text.Length == 0) View_Dflt_Item.Text = "(none)";

      //if (AntFilterItem1.Text.Length == 0) AntFilterItem1.Text = "(none)";
      //if (AntFilterItem1.Text != "(none)" && AntFilterSign1.Text.Length == 0)
      //{
      //    MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //    AntFilterSign1.Focus();
      //    return;
      //}
      //if (AntFilterItem1.Text != "(none)" && AntFilterText1.Text.Length == 0)
      //{
      //    MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //    AntFilterText1.Focus();
      //    return;
      //}
      //if (AntFilterItem2.Text.Length == 0)
      //    AntFilterItem2.Text = "(none)";
      //if (AntFilterItem2.Text != "(none)" && AntFilterSign2.Text.Length == 0)
      //{
      //    MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //    AntFilterSign2.Focus();
      //    return;
      //}
      //if (AntFilterItem2.Text != "(none)" && AntFilterText2.Text.Length == 0)
      //{
      //    MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //    AntFilterText2.Focus();
      //    return;
      //}
      //if (AntFilterItem1.Text != "(none)" && AntFilterItem2.Text != "(none)")
      //{
      //    if (AntFilterComb.Text.Length == 0)
      //    {
      //        MessageBox.Show("Must be 'or' or 'and' for filter combination", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //        AntFilterComb.Focus();
      //        return;
      //    }
      //}
      //if (AntFilterItem1.Text == "DateAdded")
      //{
      //    try
      //    {
      //        DateTime wdate = Convert.ToDateTime(AntFilterText1.Text);
      //    }
      //    catch
      //    {
      //        MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //        AntFilterText1.Focus();
      //    }
      //}
      //if (AntFilterItem2.Text == "DateAdded")
      //{
      //    try
      //    {
      //        DateTime wdate = Convert.ToDateTime(AntFilterText2.Text);
      //    }
      //    catch
      //    {
      //        MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //        AntFilterText2.Focus();
      //    }
      //}
      if (Dwp.Text.Length > 0)
        if (Dwp.Text != Rpt_Dwp.Text)
        {
          MessageBox.Show("The two Passwords must be identical !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          Dwp.Clear();
          Rpt_Dwp.Focus();
          Dwp.Focus();
          return;
        }
      if (chkAMCUpd.Checked)
        if (txtGrabber.Text.Length == 0)
        {
          MessageBox.Show("Grabber Config File Name is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          General.SelectedIndex = 4;
          txtGrabber.Focus();
          return;
        }
      if (chkAMCUpd.Checked)
      {
        if (txtAMCUpd_cnf.Text.Length == 0)
        {
          MessageBox.Show("AMCUpdater Config File Name is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          General.SelectedIndex = 5;
          txtAMCUpd_cnf.Focus();
          return;
        }
        else if (!System.IO.File.Exists(txtAMCUpd_cnf.Text))
        {
          if (MessageBox.Show("Your AMCUpdater config file does not exist - do you want to create a default one? \n configured file: '" + txtAMCUpd_cnf.Text + "' \n (If no, you can't save your current config without disabling AMCUpdater)", "Control Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            Read_XML_AMCconfig(Config_Name.Text);
          }
          else
          {
            General.SelectedIndex = 5;
            txtAMCUpd_cnf.Focus();
            return;
          }
        }
      }
      if (chkSuppressPlayStopUpdateField.Checked && !chkSuppressAutomatic.Checked)
      {
        MessageBox.Show("Suppress action must be enabled for that choice !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        chkSuppressPlayStopUpdateField.Focus();
        return;
      }
      if (chkSuppressAutomatic.Checked)
        if ((cbSuppressAutomaticAction.SelectedIndex == 2 || cbSuppressAutomaticAction.SelectedIndex == 3) && (cbSuppressPlayStopFieldToUpdate.Text.Length == 0 || txtSuppressPlayStopValueToUpdate.Text.Length == 0))
        {
          MessageBox.Show("For updating entry, field and value are mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          cbSuppressPlayStopFieldToUpdate.Focus();
          return;
        }
      if ((chkFanart.Checked) && (MesFilmsFanart.Text.Length == 0))
      {
        MessageBox.Show("Fanart path must be fill for using !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        MesFilmsFanart.Focus();
        return;
      }
      //if ((chkEnhancedWatchedStatusHandling.Checked) && (UserProfileName.Text.Length == 0))
      //{
      //  MessageBox.Show("'Active User Profile Name' must be filled (e.g. for using user rating and Trakt syncing)' !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //  General.SelectedIndex = 4;
      //  UserProfileName.Focus();
      //  return;
      //}
      //if ((chkEnhancedWatchedStatusHandling.Checked) && (cbWatched.Text.ToLower() == "checked"))
      //{
      //  MessageBox.Show("You have enabled 'Enhanced Watched Status Handling' and use 'Checked' field for watched status !\n This is NOT compatible with AMC and it is recommended to use a field that can store text like e.g. 'MediaLabel' !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      //  General.SelectedIndex = 4;
      //  cbWatched.Focus();
      //  // return;
      //}
      if (chkGlobalAvailableOnly.Checked && !chkScanMediaOnStart.Checked)
      {
        MessageBox.Show("You have enabled the global filter to only see available movies.\n As you don't have 'scan media on start' enabled, you won't get the filtered view until you made a manual availability scan via global options !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      if (chkSuppressAutomatic.Checked && cbSuppressAutomaticAction.SelectedIndex == -1)
      {
        General.SelectedIndex = 4;
        cbSuppressAutomaticAction.Focus();
        MessageBox.Show("You have enabled the automatic deletion after a movie is watched - select the proper action !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
      }
      #endregion

      Selected_Enreg_TextChanged();
      if (View_Dflt_Item.Text.Length == 0) View_Dflt_Item.Text = "(none)";

      #region Delete temporary catalog file
      string destFile = string.Empty;
      switch (CatalogType.SelectedIndex)
      {
        case 0: // ANT Movie Catalog
        case 10:// Starter Settings ANT extended DB
          break;
        case 1: //DVD Profiler
        case 2: // Movie Collector V7.1.4
        case 3: // MyMovies
        case 4: // EAX Movie Catalog 2.5.0
        case 5: // EAX Movie Catalog 3.0.9 (beta5)
        case 6: // PVD PersonalVideoDatabase V0.9.9.21
        case 7: // eXtreme Movie Manager
        case 8: // XBMC fulldb export (all movies in one DB)
        case 9: // MovingPicturesXML V1.2
          destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
          break;
        case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
          destFile = MesFilmsCat.Text;
          break;
      }
      if (destFile.Length > 0)
        if (System.IO.File.Exists(destFile))
        {
          try
          {
            System.IO.File.Delete(destFile);
          }
          catch (Exception) { }
          LogMyFilms.Debug("MyFilmsSetup: Automatically deleted tmp catalog (Save action): '" + destFile + "'");
        }
      #endregion

      string verifyResult = "";

      if (Verify_Config(ref verifyResult) == false) // check if config successful and if config should be saved - Also (re) imports the external catalog data
        return;

      if (AntTitle2.Text.Length == 0) AntTitle2.Text = "(none)";

      string wDfltSort = string.Empty;
      switch (Sort.Text.ToLower())
      {
        case "(none)":
          break;
        case "title":
          if (AntSTitle.Text != "(none)" && AntSTitle.Text.Length > 0)
            wDfltSort = AntSTitle.Text;
          else
            wDfltSort = AntTitle1.Text;
          break;
        case "dateadded":
          wDfltSort = "Date";
          break;
        default:
          wDfltSort = Sort.Text; //Guzzi: Added to not reset mapped settings other than dropdown names
          break;

      }

      string wDfltSortInHierarchies = string.Empty;
      switch (SortInHierarchies.Text.ToLower())
      {
        case "(none)":
          break;
        case "title":
          if (AntSTitle.Text != "(none)" && AntSTitle.Text.Length > 0)
            wDfltSortInHierarchies = AntSTitle.Text;
          else
            wDfltSortInHierarchies = AntTitle1.Text;
          break;
        case "dateadded":
          wDfltSortInHierarchies = "Date";
          break;
        default:
          wDfltSortInHierarchies = SortInHierarchies.Text; //Guzzi: Added to not reset mapped settings other than dropdown names
          break;
      }

      if (System.IO.File.Exists(XmlConfig.EntireFilenameConfig("MyFilms"))) System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilms"), XmlConfig.EntireFilenameConfig("MyFilms") + ".bak", true); // backup the XML Config before writing

      if (Config_Dflt.Checked)
        XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", Config_Name.Text);
      else
      {
        if (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "") == Config_Name.Text)
          XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
      }
      if (Config_Menu.Checked) XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", "");

      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", Config_Menu.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginMode", MyFilms_PluginMode);
      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PreCaching", Config_EnablePreload.Checked);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Logos", chkLogos.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CatalogType", CatalogType.SelectedIndex.ToString());
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntCatalogExecutable", AMCexePath.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntCatalog", MesFilmsCat.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntPicture", MesFilmsImg.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ArtistPicturePath", MesFilmsImgArtist.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ArtistDflt", chkDfltArtist.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntStorage", AntStorage.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntStorageTrailer", AntStorageTrailer.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "PathStorage", PathStorage.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "PathStorageTrailer", PathStorageTrailer.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntIdentItem", AntIdentItem.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntIdentLabel", AntIdentLabel.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntTitle1", AntTitle1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntTitle2", AntTitle2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntSTitle", AntSTitle.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterItem1", AntFilterItem1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterSign1", AntFilterSign1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterText1", AntFilterText1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterItem2", AntFilterItem2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterSign2", AntFilterSign2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterText2", AntFilterText2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterFreeText", AntFilterFreeText.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntFilterComb", AntFilterComb.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrDfltSelect", StrDfltSelect);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntViewTotalCount", MyCustomViews.View.Count);
      int index = 1;
      foreach (MFview.ViewRow viewRow in MyCustomViews.View) // for (int i = 1; i < 6; i++)
      {
        SaveView(index, viewRow);
        index++;
      }
      for (int i = index; i < index + 5; i++)
      {
        RemoveView(i);  // cleanup config file by removing unused view entries
      }

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItem1", AntItem1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabel1", AntLabel1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItem2", AntItem2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabel2", AntLabel2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItem3", AntItem3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabel3", AntLabel3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItem4", AntItem4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabel4", AntLabel4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItem5", AntItem5.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabel5", AntLabel5.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails1", AntItemDetails1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails1", AntLabelDetails1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails2", AntItemDetails2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails2", AntLabelDetails2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails3", AntItemDetails3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails3", AntLabelDetails3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails4", AntItemDetails4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails4", AntLabelDetails4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails5", AntItemDetails5.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails5", AntLabelDetails5.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails6", AntItemDetails6.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails6", AntLabelDetails6.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewDfltItem", View_Dflt_Item.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewDfltText", View_Dflt_Text.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "WOL-Enable", check_WOL_enable.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "WOLtimeout", comboWOLtimeout.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "WOL-Userdialog", check_WOL_Userdialog.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-1", NAS_Name_1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-1", NAS_MAC_1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-2", NAS_Name_2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-2", NAS_MAC_2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-3", NAS_Name_3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-3", NAS_MAC_3.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrSelect", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ListSeparator1", ListSeparator1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ListSeparator2", ListSeparator2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ListSeparator3", ListSeparator3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ListSeparator4", ListSeparator4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ListSeparator5", ListSeparator5.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator1", RoleSeparator1.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator2", RoleSeparator2.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator3", RoleSeparator3.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator4", RoleSeparator4.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator5", RoleSeparator5.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Selection", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "IndexItem", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "TitleDelim", TitleDelim.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSort", wDfltSort);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortSens", SortSens.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortInHierarchies", wDfltSortInHierarchies); // InHierarchies
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortSensInHierarchies", SortSensInHierarchies.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltLayOut", GetLayoutFromName(LayOut.Text));
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AntDfltLayOutInHierarchies", GetLayoutFromName(LayoutInHierarchies.Text));
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrSort", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrSortSens", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrSortInHierarchies", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "StrSortSensInHierarchies", "");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "LayOut", 0);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "LayOutInHierarchies", 0);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Dwp", crypto.Crypter(Dwp.Text));
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SearchFileName", SearchFileName.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SearchSubDirs", SearchSubDirs.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SearchOnlyExactMatches", SearchOnlyExactMatches.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "TrailerAutoregister", cbTrailerAutoregister.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CacheOnlineTrailer", cbCacheOnlineTrailer.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CheckWatched", CheckWatched.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CheckWatchedPlayerStopped", CheckWatchedPlayerStopped.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AlwaysDefaultView", AlwaysDefaultView.Checked);
      //XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "UseListviewForGroups", chkUseListviewForGroups.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GlobalAvailableOnly", chkGlobalAvailableOnly.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GlobalUnwatchedOnly", chkGlobalUnwatchedOnly.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GlobalUnwatchedOnlyValue", textBoxGlobalUnwatchedOnlyValue.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "EnhancedWatchedStatusHandling", chkEnhancedWatchedStatusHandling.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "UserProfileName", UserProfileName.Text);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CheckMediaOnStart", chkScanMediaOnStart.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ShowEmpty", chkShowEmpty.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ReversePersonNames", chkReversePersonNames.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "VirtualPathBrowsing", chkVirtualPathBrowsing.Checked);

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AllowTraktSync", cbAllowTraktSync.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AllowRecentAddedAPI", cbAllowRecentAddedAPI.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "OnlyTitleList", chkOnlyTitle.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ItemSearchFileName", ItemSearchFileName.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ItemSearchGrabberName", ItemSearchGrabberName.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ItemSearchGrabberScriptsFilter", ItemSearchGrabberScriptsFilter.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideLanguage", cbGrabberOverrideLanguage.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GrabberOverridePersonLimit", cbGrabberOverridePersonLimit.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideTitleLimit", cbGrabberOverrideTitleLimit.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideGetRoles", cbGrabberOverrideGetRoles.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "PictureHandling", cbPictureHandling.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "DefaultCover", pictureBoxDefaultCover.ImageLocation);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "DefaultCoverArtist", pictureBoxDefaultPersonImage.ImageLocation);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "DefaultCoverViews", pictureBoxDefaultViewImage.ImageLocation);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "DefaultFanartImage", pictureBoxDefaultFanart.ImageLocation);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CmdExe", CmdExe.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "CmdPar", CmdPar.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Grabber_cnf", txtGrabber.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "PicturePrefix", txtPicturePrefix.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Grabber_Always", chkGrabber_Always.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Grabber_ChooseScript", chkGrabber_ChooseScript.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AMCUpd", chkAMCUpd.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AMCUscanOnStartup", chkAMCUscanOnStartup.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AMCUwatchScanFolders", chkAMCUwatchScanFolders.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AMCUscanStartDelay", numericUpDownAMCUscanStartDelay.Value);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "AMCUpd_cnf", txtAMCUpd_cnf.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Fanart", chkFanart.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartDefaultViews", chkFanartDefaultViews.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartDefaultViewsUseRandom", chkFanartDefaultViewsUseRandom.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartDflt", chkDfltFanart.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartDfltImage", chkDfltFanartImage.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartDfltImageAll", chkDfltFanartImageAll.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "FanartPicture", MesFilmsFanart.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewsPicture", MesFilmsViews.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Views", chkViews.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Persons", chkPersons.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "PersonsEnableDownloads", chkPersonsEnableDownloads.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewsDflt", chkDfltViews.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewsDfltAll", chkDfltViewsAll.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ViewsShowIndexedImages", chkShowIndexedImgInIndViews.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "LastID", "7986");
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "Suppress", chkSuppressAutomatic.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressManual", chkSuppressManual.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressPlayed", chkSuppressPlayStopUpdateField.Checked);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "WatchedField", cbWatched.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressField", cbSuppressPlayStopFieldToUpdate.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressValue", txtSuppressPlayStopValueToUpdate.Text);
      switch (cbSuppressAutomaticAction.SelectedIndex)
      {
        case 0:
          XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressType", "1"); // delete DB entry only
          break;
        case 1:
          XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressType", "2");
          break;
        case 2:
          XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressType", "3");
          break;
        case 3:
          XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "SuppressType", "4");
          break;
      }

      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerPath", tbExternalPlayerPath.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerStartParams", tbExternalPlayerStartParams.Text);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerExtensions", tbExternalPlayerExtensions.Text);

      CleanupOldEntries(); // this can be removed in later versions - only to clean up config files from unused entries ...

      if (!IsAMCcatalogType(CatalogType.SelectedIndex)) // common external catalog options
      {
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddTagline", chkAddTagline.Checked);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddTags", chkAddTags.Checked);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddCertification", chkAddCertification.Checked);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddWriter", chkAddWriter.Checked);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationTagline", ECMergeDestinationFieldTagline.Text);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationTags", ECMergeDestinationFieldTags.Text);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationCertification", ECMergeDestinationFieldCertification.Text);
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationWriter", ECMergeDestinationFieldWriter.Text);
      }
      if (CatalogType.SelectedIndex == 1)
      {
        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "OnlyFile", chkDVDprofilerOnlyFile.Checked);
        if (this.chkDVDprofilerMergeWithGenreField.Checked)
          XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, "DVDPTagField", "Category");
        else
          XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "DVDPTagField");
      }

      string wConfigName = Config_Name.Text;
      Config_Name.Items.Remove(Config_Name.Text);
      Config_Name.Items.Add(wConfigName);
      Config_Name.Text = wConfigName;
      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
      if (chkLogos.Checked) // Save Logoconfig ...
      {
        Save_XML_Logos();
      }
      //if (chkAMCUpd.Checked) Save_XML_AMCconfig(currentconfig);
      textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
      MessageBox.Show("Configuration saved !\n\n" + verifyResult, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveView(int index, MFview.ViewRow viewRow)
    {
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewText{0}", index), viewRow.Label);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewEnabled{0}", index), viewRow.ViewEnabled);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewImagePath{0}", index), viewRow.ImagePath);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewItem{0}", index), viewRow.DBfield);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewValue{0}", index), viewRow.Value);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewFilter{0}", index), viewRow.Filter);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewFilterSaveString{0}", index), viewRow.FilterSaveString);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewIndex{0}", index), viewRow.Index);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewSortFieldViewType{0}", index), viewRow.SortFieldViewType);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewSortDirectionView{0}", index), viewRow.SortDirectionView);
      XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewLayoutView{0}", index), this.GetLayoutFromName(viewRow.LayoutView));
    }

    private void RemoveView(int index)
    {
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewItem{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewEnabled{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewImagePath{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewText{0}", index));
      // XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewLabel2{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewValue{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewFilter{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewFilterSaveString{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewIndex{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewSortFieldViewType{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewSortDirectionView{0}", index));
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Format("AntViewLayoutView{0}", index));
    }

    private void CleanupOldEntries() // this can be removed in later versions - only to clean up config files from unused entries ...
    {
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntSearchItem1");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntSearchText1");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntSearchItem2");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntSearchText2");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdItem1");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdText1");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdDflT1");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdItem2");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdText2");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntUpdDflT2");

      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "CurrentSortMethod");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "CurrentSortMethodInHierarchies");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntDfltSortMethodInHierarchies");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "AntDfltSortMethod");
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "SearchSubDirsTrailer");

      // XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, "EnhancedWatchedStatusHandling"); // ToDo: can be removed in later release, when we don't support nonEnhanced/AMC3 anymore
    }

    internal int GetLayoutFromName(string layoutname)
    {
      int wLayout = 0;
      if (layoutname == "List") wLayout = 0;
      if (layoutname == "Big Icon List") wLayout = 1;
      if (layoutname == "Small Icons") wLayout = 2;
      if (layoutname == "Large Icons") wLayout = 3;
      if (layoutname == "Filmstrip") wLayout = 4;
      if (layoutname == "Cover Flow") wLayout = 5;
      return wLayout;
    }

    internal string GetLayoutFromInt(int layoutnumber)
    {
      string layoutname = "List";
      if (layoutnumber == 0) layoutname = "List";
      if (layoutnumber == 1) layoutname = "Big Icon List";
      if (layoutnumber == 2) layoutname = "Small Icons";
      if (layoutnumber == 3) layoutname = "Large Icons";
      if (layoutnumber == 4) layoutname = "Filmstrip";
      if (layoutnumber == 5) layoutname = "Cover Flow";
      return layoutname;
    }

    private void Save_XML_Logos()
    {
      string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
      //if (System.IO.File.Exists(wfile + "_" + Config_Name.Text + ".xml"))
      //  System.IO.File.Copy(wfile + "_" + Config_Name.Text + ".xml", wfile + "_" + Config_Name.Text + ".xml.sav", true);
      if (System.IO.File.Exists(wfile + ".xml"))
        System.IO.File.Copy(wfile + ".xml", wfile + ".xml.sav", true);
      //try
      //{
      //    System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsLogos"), wfile + "_" + Config_Name.Text + ".xml", true);
      //    //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + Config_Name.Text; // commented, as we always want to write to the original file !
      //}
      //catch
      //{
      //    wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
      //}
      wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
      //using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
      //{
      //  logoPath = MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin") + @"\Media\Logos"; // Get current path to logos in skindirectory
      //}
      //if (txtLogosPath.Text.Length > 0)
      XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogoPresets", comboBoxLogoPresets.Text); // Added Presets to Logo Config
      XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogosPath", txtLogosPath.Text);
      XmlConfig.WriteXmlConfig(wfile, "ID0000", "Spacing", comboBoxLogoSpacing.Text); // Added Spacing to Logo Config
      XmlConfig.WriteXmlConfig(wfile, "ID0000", "Country", txtCountry.Text); // Added Country to Logo Config

      int iID2001 = 0;
      int iID2002 = 0;
      int iID2003 = 0;
      for (int i = 0; i < (int)LogoView.Items.Count; i++)
      {
        string wline = LogoView.Items[i].SubItems[1].Text + ";";
        wline = wline + LogoView.Items[i].SubItems[2].Text.ToLower() + ";";
        wline = wline + LogoView.Items[i].SubItems[3].Text + ";";
        wline = wline + LogoView.Items[i].SubItems[4].Text.ToLower() + ";";
        wline = wline + LogoView.Items[i].SubItems[5].Text + ";";
        wline = wline + LogoView.Items[i].SubItems[6].Text.ToLower() + ";";
        wline = wline + LogoView.Items[i].SubItems[7].Text + ";";
        if ((LogoView.Items[i].SubItems[9].Text.Length > 0) && (StoreFullLogoPath))
          wline = wline + LogoView.Items[i].SubItems[9].Text + "\\" + LogoView.Items[i].SubItems[8].Text;
        else
          wline = wline + LogoView.Items[i].SubItems[8].Text;
        switch (LogoView.Items[i].SubItems[0].Text)
        {
          case "ID2001":
            XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2001, wline);
            iID2001++;
            break;
          case "ID2002":
            XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2002, wline);
            iID2002++;
            break;
          case "ID2003":
            XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2003, wline);
            iID2003++;
            break;
        }
      }
    }

    private void butPath_Click(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(PathStorage.Text))
      {
        this.folderBrowserDialog1.SelectedPath = this.PathStorage.Text.Contains(";") ? this.PathStorage.Text.Substring(this.PathStorage.Text.LastIndexOf(";") + 1).Trim() : this.PathStorage.Text;
        if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
      }
      else
        folderBrowserDialog1.SelectedPath = String.Empty;
      folderBrowserDialog1.Description = "Path for Movies File Search";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != this.folderBrowserDialog1.SelectedPath.Length - 1)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

        if (PathStorage.Text.Length == 0)
          PathStorage.Text = folderBrowserDialog1.SelectedPath;
        else
          PathStorage.Text = PathStorage.Text + ";" + folderBrowserDialog1.SelectedPath;
        if (AMCMovieScanPath.Text.Length == 0)
          AMCMovieScanPath.Text = PathStorage.Text;
      }
    }

    public void ButQuit_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void Config_Name_SelectedIndexChanged(object sender, EventArgs e)
    {
      Config_Name_Load();
      BindSources();
    }

    private void BindSources()
    {
      propertiesBindingSource.DataSource = mydivx.Properties;
      propertiesBindingSource.ResumeBinding();
      customFieldsBindingSource.DataSource = mydivx.CustomFieldsProperties;
      customFieldsBindingSource.ResumeBinding();
      customFieldBindingSource.DataSource = mydivx.CustomField;
      customFieldBindingSource.ResumeBinding();
      personBindingSource.DataSource = mydivx.Person;
      personBindingSource.ResumeBinding();

      // views
      viewBindingSource.DataSource = this.MyCustomViews;
      viewBindingSource.ResumeBinding();
    }

    private void Config_Name_Load()
    {
      Refresh_Tabs(true); // enable Tabs
      Refresh_Items(false);
      CatalogType.SelectedIndex = Convert.ToInt16(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CatalogType", "0"));
      chkEnhancedWatchedStatusHandling.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "EnhancedWatchedStatusHandling", false) || CatalogType.SelectedIndex == 10;
      UserProfileName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UserProfileName", ""); // MyFilms.DefaultUsername
      MesFilmsCat.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntCatalog", "");
      AMCexePath.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntCatalogExecutable", "");
      MesFilmsImg.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntPicture", "");
      MesFilmsImgArtist.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ArtistPicturePath", "");
      MesFilmsFanart.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartPicture", "");
      lblResultingGroupViewsPathFanart.Text = MesFilmsFanart.Text + "\\_View\\";
      MesFilmsViews.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewsPicture", "");
      chkDfltViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewsDflt", false);
      chkDfltViewsAll.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewsDfltAll", false);
      chkShowIndexedImgInIndViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewsShowIndexedImages", false);
      chkDfltArtist.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ArtistDflt", false);
      chkViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Views", false);
      chkPersons.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Persons", false);
      chkPersonsEnableDownloads.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "PersonsEnableDownloads", false);
      AntStorage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntStorage", "");
      AntStorageTrailer.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntStorageTrailer", "");
      PathStorage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "PathStorage", "");
      PathStorageTrailer.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "PathStorageTrailer", "");
      AntIdentItem.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntIdentItem", "");
      AntIdentLabel.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntIdentLabel", "");
      AntTitle1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntTitle1", "");
      AntTitle2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntTitle2", "");
      AntSTitle.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntSTitle", "");
      if (AntSTitle.Text == "") AntSTitle.Text = AntTitle1.Text;

      #region default values for film lists and groups
      Sort.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSort", "");
      SortInHierarchies.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortInHierarchies", "");
      SortSens.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortSens", " ASC");
      SortSensInHierarchies.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltStrSortSensInHierarchies", " ASC");
      LayOut.Text = GetLayoutFromInt(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltLayOut", 0));
      LayoutInHierarchies.Text = GetLayoutFromInt(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntDfltLayOutInHierarchies", 0));
      #endregion

      AntFilterItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterItem1", "");
      AntFilterSign1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterSign1", "#");
      AntFilterText1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterText1", "");
      AntFilterItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterItem2", "");
      AntFilterSign2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterSign2", "#");
      AntFilterText2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterText2", "");
      AntFilterFreeText.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterFreeText", "");
      AntFilterComb.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntFilterComb", "and");

      StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "StrDfltSelect", "");
      textBoxStrDfltSelect.Text = StrDfltSelect;

      MyCustomViews.View.Clear();
      int iCustomViews = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntViewTotalCount", -1);
      if (iCustomViews == -1) // Customviews not yet present ... ToDo: Remove in later Version - only "migration code"
      {
        AddOldHardcodedViews();
      }
      int index = 1;
      while (true) // for (int i = 1; i < iCustomViews + 1; i++)
      {
        if (string.IsNullOrEmpty(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewText{0}", index), string.Empty))) break; // stop loading, if no View name is given
        MFview.ViewRow view = this.MyCustomViews.View.NewViewRow();
        view.DBfield = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewItem{0}", index), string.Empty);
        view.ViewEnabled = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewEnabled{0}", index), true);
        view.ImagePath = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewImagePath{0}", index), string.Empty);
        view.Label = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewText{0}", index), string.Empty);
        // if (view.Label.Length == 0) view.Label = view.DBfield;
        view.Value = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewValue{0}", index), string.Empty);
        view.Filter = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewFilter{0}", index), string.Empty);
        view.FilterSaveString = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewFilterSaveString{0}", index), string.Empty);
        view.Index = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewIndex{0}", index), 0);
        view.SortFieldViewType = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewSortFieldViewType{0}", index), "Name");
        view.SortDirectionView = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewSortDirectionView{0}", index), " ASC");
        view.SortDirectionView = view.SortDirectionView.Contains("ASC") ? " ASC" : " DESC";
        view.LayoutView = this.GetLayoutFromInt(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, string.Format("AntViewLayoutView{0}", index), 0));
        this.MyCustomViews.View.AddViewRow(view);
        index++;
      }
      if (iCustomViews == -1) // Customviews not yet present ... ToDo: Remove in later Version - only "migration code"
      {
        UpdateOldHardcodedViews();
      }

      check_WOL_enable.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOL-Enable", false);
      comboWOLtimeout.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOLtimeout", "15");
      check_WOL_Userdialog.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOL-Userdialog", false);
      NAS_Name_1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-1", string.Empty);
      NAS_MAC_1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-1", string.Empty);
      NAS_Name_2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-2", string.Empty);
      NAS_MAC_2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-2", string.Empty);
      NAS_Name_3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-Name-3", string.Empty);
      NAS_MAC_3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "NAS-MAC-3", string.Empty);

      AntItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItem1", "Category");
      AntLabel1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabel1", BaseMesFilms.TranslateColumn(AntItem1.Text.Trim()));
      AntItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItem2", "Country");
      AntLabel2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabel2", BaseMesFilms.TranslateColumn(AntItem2.Text.Trim()));
      AntItem3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItem3", "Director");
      AntLabel3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabel3", BaseMesFilms.TranslateColumn(AntItem3.Text.Trim()));
      AntItem4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItem4", "Languages");
      AntLabel4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabel4", BaseMesFilms.TranslateColumn(AntItem4.Text.Trim()));
      AntItem5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItem5", "Certification");
      AntLabel5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabel5", BaseMesFilms.TranslateColumn(AntItem5.Text.Trim()));

      AntItemDetails1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails1", "Category");
      AntLabelDetails1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails1", BaseMesFilms.TranslateColumn(AntItemDetails1.Text.Trim()));
      AntItemDetails2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails2", "Country");
      AntLabelDetails2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails2", BaseMesFilms.TranslateColumn(AntItemDetails2.Text.Trim()));
      AntItemDetails3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails3", "Director");
      AntLabelDetails3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails3", BaseMesFilms.TranslateColumn(AntItemDetails3.Text.Trim()));
      AntItemDetails4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails4", "Producer");
      AntLabelDetails4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails4", BaseMesFilms.TranslateColumn(AntItemDetails4.Text.Trim()));
      AntItemDetails5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails5", "Writer");
      AntLabelDetails5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails5", BaseMesFilms.TranslateColumn(AntItemDetails5.Text.Trim()));
      AntItemDetails6.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntItemDetails6", "Certification");
      AntLabelDetails6.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntLabelDetails6", BaseMesFilms.TranslateColumn(AntItemDetails6.Text.Trim()));

      ListSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ListSeparator1", ",");
      ListSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ListSeparator2", ";");
      ListSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ListSeparator3", "|");
      ListSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ListSeparator4", "/");
      ListSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ListSeparator5", "");
      RoleSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator1", "(");
      RoleSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator2", ")");
      RoleSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator3", " as ");
      RoleSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator4", "");
      RoleSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "RoleSeparator5", "");
      CmdPar.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CmdPar", "(none)");
      CmdExe.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CmdExe", string.Empty);
      TitleDelim.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "TitleDelim", "\\");
      chkDfltFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartDflt", false);
      chkDfltFanartImage.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartDfltImage", false);
      chkDfltFanartImageAll.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartDfltImageAll", false);
      chkFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Fanart", false);
      chkFanartDefaultViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartDefaultViews", false);
      chkFanartDefaultViewsUseRandom.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "FanartDefaultViewsUseRandom", false);
      txtGrabber.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Grabber_cnf", string.Empty);
      txtPicturePrefix.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "PicturePrefix", string.Empty);
      chkGrabber_Always.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Grabber_Always", false);
      chkAMCUpd.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AMCUpd", false);
      chkAMCUscanOnStartup.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AMCUscanOnStartup", false);
      chkAMCUwatchScanFolders.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AMCUwatchScanFolders", false);
      numericUpDownAMCUscanStartDelay.Value = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AMCUscanStartDelay", 60);
      chkGrabber_ChooseScript.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Grabber_ChooseScript", false);
      txtAMCUpd_cnf.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AMCUpd_cnf", string.Empty);
      chkSuppressAutomatic.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Suppress", false);
      chkSuppressManual.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SuppressManual", false);
      chkSuppressPlayStopUpdateField.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SuppressPlayed", false);
      cbWatched.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WatchedField", "Checked");
      cbSuppressPlayStopFieldToUpdate.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SuppressField", string.Empty);
      txtSuppressPlayStopValueToUpdate.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SuppressValue", string.Empty);
      chkLogos.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Logos", true);
      string wsuppressType = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SuppressType", "1");
      switch (wsuppressType)
      {
        case "1":
          cbSuppressAutomaticAction.SelectedIndex = 0;
          break;
        case "2":
          cbSuppressAutomaticAction.SelectedIndex = 1;
          break;
        case "3":
          cbSuppressAutomaticAction.SelectedIndex = 2;
          break;
        default:
          cbSuppressAutomaticAction.SelectedIndex = 3;
          break;
      }
      Dwp.Text = crypto.Decrypter(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Dwp", string.Empty));
      Rpt_Dwp.Text = Dwp.Text;
      SearchFileName.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchFileName", false);
      SearchSubDirs.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchSubDirs", false);
      SearchOnlyExactMatches.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchOnlyExactMatches", false);
      cbTrailerAutoregister.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "TrailerAutoregister", false);
      cbCacheOnlineTrailer.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CacheOnlineTrailer", false);
      check_WOL_enable.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOL-Enable", false);
      comboWOLtimeout.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOLtimeout", "15");
      check_WOL_Userdialog.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "WOL-Userdialog", false);

      chkDVDprofilerMergeWithGenreField.Checked = false;
      chkDVDprofilerMergeWithGenreField.Checked = XmlConfig.ReadXmlConfig("MyFilms", this.Config_Name.Text, "DVDPTagField", "") == "Category";
      CheckWatched.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CheckWatched", false);
      CheckWatchedPlayerStopped.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CheckWatchedPlayerStopped", false);
      AlwaysDefaultView.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AlwaysDefaultView", false);
      chkGlobalAvailableOnly.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GlobalAvailableOnly", false);
      chkGlobalUnwatchedOnly.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GlobalUnwatchedOnly", false);
      textBoxGlobalUnwatchedOnlyValue.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GlobalUnwatchedOnlyValue", "false");

      chkScanMediaOnStart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "CheckMediaOnStart", false);
      cbAllowTraktSync.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AllowTraktSync", false);
      cbAllowRecentAddedAPI.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AllowRecentAddedAPI", false);
      chkOnlyTitle.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "OnlyTitleList", false);
      chkShowEmpty.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ShowEmpty", false);
      chkReversePersonNames.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ReversePersonNames", false);
      chkVirtualPathBrowsing.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "VirtualPathBrowsing", false);

      tbExternalPlayerPath.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerPath", "");
      tbExternalPlayerStartParams.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerStartParams", "");
      tbExternalPlayerExtensions.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ExternalPlayerExtensions", "");

      // common external catalog options
      chkAddTagline.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddTagline", false);
      chkAddTags.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddTags", false);
      chkAddCertification.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddCertification", false);
      chkAddWriter.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddWriter", false);
      ECMergeDestinationFieldTagline.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationTagline", "");
      ECMergeDestinationFieldTags.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationTags", "");
      ECMergeDestinationFieldCertification.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationCertification", "");
      ECMergeDestinationFieldWriter.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ECoptionAddDestinationWriter", "");
      chkDVDprofilerOnlyFile.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "OnlyFile", false);
      ItemSearchFileName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ItemSearchFileName", "");
      ItemSearchGrabberName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ItemSearchGrabberName", "");
      ItemSearchGrabberScriptsFilter.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ItemSearchGrabberScriptsFilter", "");
      cbGrabberOverrideLanguage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideLanguage", string.Empty);
      cbGrabberOverridePersonLimit.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GrabberOverridePersonLimit", string.Empty);
      cbGrabberOverrideTitleLimit.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideTitleLimit", string.Empty);
      cbGrabberOverrideGetRoles.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "GrabberOverrideGetRoles", string.Empty);

      cbPictureHandling.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "PictureHandling", "");
      pictureBoxDefaultCover.ImageLocation = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "DefaultCover", "");
      pictureBoxDefaultPersonImage.ImageLocation = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "DefaultCoverArtist", "");
      pictureBoxDefaultViewImage.ImageLocation = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "DefaultCoverViews", "");
      pictureBoxDefaultFanart.ImageLocation = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "DefaultFanartImage", "");
      View_Dflt_Item.Items.Remove(View_Dflt_Item.Text);
      View_Dflt_Item.Items.Add(View_Dflt_Item.Text);
      View_Dflt_Item.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewDfltItem", "(none)");
      View_Dflt_Text.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "ViewDfltText", "");
      Config_Dflt.Checked = (Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
      Config_EnablePreload.Checked = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PreCaching", false);

      AntViewText_Change();
      AntSort_Change();

      chkLogos.Enabled = (Config_Name.Text.Length > 0);

      if (chkAMCUpd.Checked)
      {
        groupBoxAMCUpdaterConfigFile.Enabled = true;
        groupBox_AMCupdater_ExternalApplication.Enabled = true;
        groupBox_AMCupdaterScheduer.Enabled = true;
      }
      else
      {
        groupBoxAMCUpdaterConfigFile.Enabled = false;
        groupBox_AMCupdater_ExternalApplication.Enabled = false;
        groupBox_AMCupdaterScheduer.Enabled = false;
      }

      Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file // Added by Guzzi to load or initialize the AMCupdater Default configuration and create default configfiles, if necessary.

      textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
    }

    private void AddRemoveExtendedfields(bool isXtended)
    {
      var ds = new AntMovieCatalog();
      foreach (DataColumn dc in ds.Movie.Columns)
      {
        if (IsExtendedField(dc.ColumnName))
        {
          string tmpValue;
          if (dc.ColumnName == "MediaLabel" || dc.ColumnName == "MediaType" || dc.ColumnName == "Source" || (dc.ColumnName == "SourceTrailer" && CatalogType.SelectedIndex == 10) ||
              dc.ColumnName == "URL" || dc.ColumnName == "Comments" || dc.ColumnName == "Borrower" ||
              dc.ColumnName == "Languages" || dc.ColumnName == "Subtitles")
          {
            tmpValue = AntStorage.Text;
            if (AntStorage.Items.Contains(dc.ColumnName)) AntStorage.Items.Remove(dc.ColumnName);
            if (isXtended) AntStorage.Items.Add(dc.ColumnName);
            AntStorage.Text = tmpValue;
            tmpValue = AntStorageTrailer.Text;
            if (AntStorageTrailer.Items.Contains(dc.ColumnName)) AntStorageTrailer.Items.Remove(dc.ColumnName);
            if (isXtended) AntStorageTrailer.Items.Add(dc.ColumnName);
            AntStorageTrailer.Text = tmpValue;
          }

          if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded") // added "DatedAdded" to remove filter
          {
            tmpValue = AntFilterItem1.Text;
            if (AntFilterItem1.Items.Contains(dc.ColumnName)) AntFilterItem1.Items.Remove(dc.ColumnName);
            if (isXtended) AntFilterItem1.Items.Add(dc.ColumnName);
            AntFilterItem1.Text = tmpValue;
            tmpValue = AntFilterItem2.Text;
            if (AntFilterItem2.Items.Contains(dc.ColumnName)) AntFilterItem2.Items.Remove(dc.ColumnName);
            if (isXtended) AntFilterItem2.Items.Add(dc.ColumnName);
            AntFilterItem2.Text = tmpValue;
            tmpValue = AntItem1.Text;
            if (AntItem1.Items.Contains(dc.ColumnName)) AntItem1.Items.Remove(dc.ColumnName);
            if (isXtended) AntItem1.Items.Add(dc.ColumnName);
            AntItem1.Text = tmpValue;
            tmpValue = AntItem2.Text;
            if (AntItem2.Items.Contains(dc.ColumnName)) AntItem2.Items.Remove(dc.ColumnName);
            if (isXtended) AntItem2.Items.Add(dc.ColumnName);
            AntItem2.Text = tmpValue;
            tmpValue = AntItem3.Text;
            if (AntItem3.Items.Contains(dc.ColumnName)) AntItem3.Items.Remove(dc.ColumnName);
            if (isXtended) AntItem3.Items.Add(dc.ColumnName);
            AntItem3.Text = tmpValue;
            tmpValue = AntItem4.Text;
            if (AntItem4.Items.Contains(dc.ColumnName)) AntItem4.Items.Remove(dc.ColumnName);
            if (isXtended) AntItem4.Items.Add(dc.ColumnName);
            AntItem4.Text = tmpValue;
            tmpValue = AntItem5.Text;
            if (AntItem5.Items.Contains(dc.ColumnName)) AntItem5.Items.Remove(dc.ColumnName);
            if (isXtended) AntItem5.Items.Add(dc.ColumnName);
            AntItem5.Text = tmpValue;
            tmpValue = AntItemDetails1.Text;
            if (AntItemDetails1.Items.Contains(dc.ColumnName)) AntItemDetails1.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails1.Items.Add(dc.ColumnName);
            AntItemDetails1.Text = tmpValue;
            tmpValue = AntItemDetails2.Text;
            if (AntItemDetails2.Items.Contains(dc.ColumnName)) AntItemDetails2.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails2.Items.Add(dc.ColumnName);
            AntItemDetails2.Text = tmpValue;
            tmpValue = AntItemDetails3.Text;
            if (AntItemDetails3.Items.Contains(dc.ColumnName)) AntItemDetails3.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails3.Items.Add(dc.ColumnName);
            AntItemDetails3.Text = tmpValue;
            tmpValue = AntItemDetails4.Text;
            if (AntItemDetails4.Items.Contains(dc.ColumnName)) AntItemDetails4.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails4.Items.Add(dc.ColumnName);
            AntItemDetails4.Text = tmpValue;
            tmpValue = AntItemDetails5.Text;
            if (AntItemDetails5.Items.Contains(dc.ColumnName)) AntItemDetails5.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails5.Items.Add(dc.ColumnName);
            AntItemDetails5.Text = tmpValue;
            tmpValue = AntItemDetails6.Text;
            if (AntItemDetails6.Items.Contains(dc.ColumnName)) AntItemDetails6.Items.Remove(dc.ColumnName);
            if (isXtended) AntItemDetails6.Items.Add(dc.ColumnName);
            AntItemDetails6.Text = tmpValue;
          }
          if (dc.ColumnName != "OriginalTitle" && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "IndexedTitle" &&
              dc.ColumnName != "Comments" && dc.ColumnName != "Description" && dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && dc.ColumnName != "Rating" &&
              dc.ColumnName != "URL" && dc.ColumnName != "AgeAdded"  && dc.ColumnName != "RecentlyAdded")
          {
            tmpValue = SField1.Text;
            if (SField1.Items.Contains(dc.ColumnName)) SField1.Items.Remove(dc.ColumnName);
            if (isXtended) SField1.Items.Add(dc.ColumnName);
            SField1.Text = tmpValue;

            tmpValue = SField2.Text;
            if (SField2.Items.Contains(dc.ColumnName)) SField2.Items.Remove(dc.ColumnName);
            if (isXtended) SField2.Items.Add(dc.ColumnName);
            SField2.Text = tmpValue;
          }

          if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
              dc.ColumnName != "Description" && dc.ColumnName != "Comments" && dc.ColumnName != "Number")
          {
            tmpValue = AntViewItem.Text;
            if (AntViewItem.Items.Contains(dc.ColumnName)) AntViewItem.Items.Remove(dc.ColumnName);
            if (isXtended) AntViewItem.Items.Add(dc.ColumnName);
            AntViewItem.Text = tmpValue;
          }

          if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
              dc.ColumnName != "Year" && dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && // disabled for Doug testing
              dc.ColumnName != "Length" && dc.ColumnName != "Rating" &&
              dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
          {
            tmpValue = AntIdentItem.Text;
            if (AntIdentItem.Items.Contains(dc.ColumnName)) AntIdentItem.Items.Remove(dc.ColumnName);
            if (isXtended) AntIdentItem.Items.Add(dc.ColumnName);
            AntIdentItem.Text = tmpValue;
          }

          if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
          {
            tmpValue = cbSuppressPlayStopFieldToUpdate.Text;
            if (cbSuppressPlayStopFieldToUpdate.Items.Contains(dc.ColumnName)) cbSuppressPlayStopFieldToUpdate.Items.Remove(dc.ColumnName);
            if (isXtended) cbSuppressPlayStopFieldToUpdate.Items.Add(dc.ColumnName);
            cbSuppressPlayStopFieldToUpdate.Text = tmpValue;

            tmpValue = cbWatched.Text;
            if (cbWatched.Items.Contains(dc.ColumnName)) cbWatched.Items.Remove(dc.ColumnName);
            if (isXtended) cbWatched.Items.Add(dc.ColumnName);
            cbWatched.Text = tmpValue;

            tmpValue = CmdPar.Text;
            if (CmdPar.Items.Contains(dc.ColumnName)) CmdPar.Items.Remove(dc.ColumnName);
            if (isXtended) CmdPar.Items.Add(dc.ColumnName);
            CmdPar.Text = tmpValue;
          }
        }
      }
    }

    private void AddItem(IList list, Type type, string valueMember, string displayMember, string displayText)
    {
      //Creates an instance of the specified type using the constructor that best matches the specified parameters.

      Object obj = Activator.CreateInstance(type);

      // Gets the Display Property Information
      PropertyInfo displayProperty = type.GetProperty(displayMember);

      // Sets the required text into the display property
      displayProperty.SetValue(obj, displayText, null);

      // Gets the Value Property Information
      PropertyInfo valueProperty = type.GetProperty(valueMember);

      // Sets the required value into the value property
      valueProperty.SetValue(obj, -1, null);

      // Insert the new object on the list
      list.Insert(0, obj);
    }

    private bool IsExtendedField(string DbField)
    {
      if (DbField == "IMDB_Id" || DbField == "TMDB_Id" || DbField == "Watched" || DbField == "Certification" ||
           DbField == "Writer" || DbField == "SourceTrailer" || DbField == "TagLine" || DbField == "Tags" ||
           DbField == "RatingUser" || DbField == "Studio" || DbField == "IMDB_Rank" || DbField == "Edition" ||
           DbField == "Aspectratio" || DbField == "CategoryTrakt" || DbField == "Favorite" ||
           DbField == "CustomField1" || DbField == "CustomField2" || DbField == "CustomField3")
        return true;
      else
        return false;
    }

    private void Refresh_Tabs(bool enable)
    {
      if (!enable)
      {
        Tab_Trailer.Enabled = false;
        Tab_Logos.Enabled = false;
        Tab_Views.Enabled = false;
        Tab_Display.Enabled = false;
        Tab_Update.Enabled = false;
        Tab_AMCupdater.Enabled = false;
        Tab_Artwork.Enabled = false;
        Tab_ExternalCatalogs.Enabled = false;
        Tab_Network.Enabled = false;
        Tab_Trakt.Enabled = false;
        MesFilmsCat.Enabled = false;
        CatalogType.Enabled = false;
        ButCat.Enabled = false;
        groupBox_TitleOrder.Enabled = false;
        groupBox_Security.Enabled = false;
        groupBox_PlayMovieInfos.Enabled = false;
        groupBox_PreLaunchingCommand.Enabled = false;

      }
      else
      {
        Tab_Trailer.Enabled = true;
        Tab_Logos.Enabled = true;
        Tab_Views.Enabled = true;
        Tab_Display.Enabled = true;
        Tab_Update.Enabled = true;
        Tab_AMCupdater.Enabled = true;
        Tab_Artwork.Enabled = true;
        Tab_ExternalCatalogs.Enabled = true;
        Tab_Network.Enabled = true;
        Tab_Trakt.Enabled = true;
        MesFilmsCat.Enabled = true;
        CatalogType.Enabled = true;
        ButCat.Enabled = true;
        groupBox_TitleOrder.Enabled = true;
        groupBox_Security.Enabled = true;
        groupBox_PlayMovieInfos.Enabled = true;
        groupBox_PreLaunchingCommand.Enabled = true;
      }
    }

    private void Refresh_Items(bool all)
    {
      CatalogType.SelectedIndex = 0;
      StrDfltSelect = "";
      //mydivx.Clear();
      View_Dflt_Item.Items.Clear();
      View_Dflt_Item.Items.Add("(none)");
      View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(1079819)); //Views Menu
      //View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(342)); // Films
      //View_Dflt_Item.Items.Add("Year");
      //View_Dflt_Item.Items.Add("Category");
      //View_Dflt_Item.Items.Add("Country");
      if (Config_Name.Text.Length == 0)
      {
        //btnFirstTimeSetup.Enabled = true;
        Tab_Trailer.Enabled = false;
        Tab_Logos.Enabled = false;
        Tab_Views.Enabled = false;
        Tab_Display.Enabled = false;
        Tab_Update.Enabled = false;
        Tab_AMCupdater.Enabled = false;
        Tab_Artwork.Enabled = false;
        Tab_ExternalCatalogs.Enabled = false;
        Tab_Network.Enabled = false;
        Tab_Trakt.Enabled = false;
        MesFilmsCat.Enabled = false;
        CatalogType.Enabled = false;
        ButCat.Enabled = false;
        groupBox_TitleOrder.Enabled = false;
        groupBox_Security.Enabled = false;
        groupBox_PlayMovieInfos.Enabled = false;
        groupBox_PreLaunchingCommand.Enabled = false;

      }
      else
      {
        // btnFirstTimeSetup.Enabled = false;
        Tab_Trailer.Enabled = true;
        Tab_Logos.Enabled = true;
        Tab_Views.Enabled = true;
        Tab_Display.Enabled = true;
        Tab_Update.Enabled = true;
        Tab_AMCupdater.Enabled = true;
        Tab_Artwork.Enabled = true;
        Tab_ExternalCatalogs.Enabled = true;
        Tab_Network.Enabled = true;
        Tab_Trakt.Enabled = true;
        MesFilmsCat.Enabled = true;
        CatalogType.Enabled = true;
        ButCat.Enabled = true;
        groupBox_TitleOrder.Enabled = true;
        groupBox_Security.Enabled = true;
        groupBox_PlayMovieInfos.Enabled = true;
        groupBox_PreLaunchingCommand.Enabled = true;
      }

      if (!all) return;

      MyCustomViews.View.Clear();
      Config_Dflt.Checked = false;
      MesFilmsCat.ResetText();
      MesFilmsImg.ResetText();
      MesFilmsImgArtist.ResetText();
      MesFilmsFanart.ResetText();
      MesFilmsViews.ResetText();
      AntStorage.ResetText();
      AntStorageTrailer.ResetText();
      PathStorage.ResetText();
      PathStorageTrailer.ResetText();
      AntIdentItem.ResetText();
      AntIdentLabel.ResetText();
      AntTitle1.ResetText();
      AntTitle2.ResetText();
      AntSTitle.ResetText();
      Sort.ResetText();
      SortInHierarchies.ResetText();
      AntFilterItem1.ResetText();
      AntFilterItem2.ResetText();
      AntFilterText1.ResetText();
      AntFilterText2.ResetText();
      AntFilterFreeText.ResetText();
      AntFilterSign1.ResetText();
      AntFilterSign2.ResetText();
      AntFilterComb.ResetText();
      AntViewItem.ResetText();
      AntLabel1.ResetText();
      AntLabel2.ResetText();
      AntLabel3.ResetText();
      AntLabel4.ResetText();
      AntLabel5.ResetText();
      AntItem1.ResetText();
      AntItem2.ResetText();
      AntItem3.ResetText();
      AntItem4.ResetText();
      AntItem5.ResetText();
      AntItemDetails1.ResetText();
      AntItemDetails2.ResetText();
      AntItemDetails3.ResetText();
      AntItemDetails4.ResetText();
      AntItemDetails5.ResetText();
      AntItemDetails6.ResetText();
      TitleDelim.ResetText();
      LayOut.ResetText();
      LayoutInHierarchies.ResetText();
      ListSeparator1.ResetText();
      ListSeparator2.ResetText();
      ListSeparator3.ResetText();
      ListSeparator4.ResetText();
      ListSeparator5.ResetText();
      RoleSeparator1.ResetText();
      RoleSeparator2.ResetText();
      RoleSeparator3.ResetText();
      RoleSeparator4.ResetText();
      RoleSeparator5.ResetText();
      View_Dflt_Item.ResetText();
      View_Dflt_Text.ResetText();
      pictureBoxDefaultCover.ImageLocation = "";
      pictureBoxDefaultPersonImage.ImageLocation = "";
      pictureBoxDefaultViewImage.ImageLocation = "";
      pictureBoxDefaultFanart.ImageLocation = "";
      Dwp.ResetText();
      Rpt_Dwp.ResetText();
      CmdExe.ResetText();
      CmdPar.ResetText();
      ItemSearchFileName.ResetText();
      ItemSearchGrabberName.ResetText();
      ItemSearchGrabberScriptsFilter.ResetText();
      cbPictureHandling.ResetText();
      SearchFileName.Checked = false;
      chkSuppressAutomatic.Checked = false;
      chkSuppressManual.Checked = false;
      chkSuppressPlayStopUpdateField.Checked = false;
      cbSuppressPlayStopFieldToUpdate.ResetText();
      cbWatched.ResetText();
      txtSuppressPlayStopValueToUpdate.ResetText();
      cbSuppressAutomaticAction.ResetText();
      SearchSubDirs.Checked = false;
      SearchOnlyExactMatches.Checked = false;
      cbTrailerAutoregister.Checked = false;
      cbCacheOnlineTrailer.Checked = false;
      check_WOL_enable.Checked = false;
      comboWOLtimeout.ResetText();
      check_WOL_Userdialog.Checked = false;
      CheckWatched.Checked = false;
      CheckWatchedPlayerStopped.Checked = false;
      NAS_Name_1.ResetText();
      NAS_Name_2.ResetText();
      NAS_Name_3.ResetText();
      NAS_MAC_1.ResetText();
      NAS_MAC_2.ResetText();
      NAS_MAC_3.ResetText();
      chkAddTagline.Checked = false;
      chkAddTags.Checked = false;
      chkAddCertification.Checked = false;
      chkAddWriter.Checked = false;
      ECMergeDestinationFieldTagline.Text = string.Empty;
      ECMergeDestinationFieldTags.Text = string.Empty;
      ECMergeDestinationFieldCertification.Text = string.Empty;
      ECMergeDestinationFieldWriter.Text = string.Empty;
      cbGrabberOverrideLanguage.Text = string.Empty;
      cbGrabberOverrideTitleLimit.Text = string.Empty;
      cbGrabberOverridePersonLimit.Text = string.Empty;
      cbGrabberOverrideGetRoles.Text = string.Empty;
      chkDVDprofilerMergeWithGenreField.Checked = false;
      chkDVDprofilerOnlyFile.Checked = false;
      AlwaysDefaultView.Checked = false;
      chkGlobalAvailableOnly.Checked = false;
      chkGlobalUnwatchedOnly.Checked = false;
      chkScanMediaOnStart.Checked = false;
      chkShowEmpty.Checked = false;
      cbAllowTraktSync.Checked = false;
      cbAllowRecentAddedAPI.Checked = false;
      textBoxGlobalUnwatchedOnlyValue.Text = "false";
      // chkEnhancedWatchedStatusHandling.Checked = false;
      UserProfileName.Text = string.Empty;
      chkOnlyTitle.Checked = false;
      txtGrabber.ResetText();
      txtPicturePrefix.ResetText();
      chkGrabber_Always.Checked = false;
      chkGrabber_ChooseScript.Checked = false;
      chkAMCUpd.Checked = false;
      chkAMCUscanOnStartup.Checked = false;
      chkAMCUwatchScanFolders.Checked = false;
      numericUpDownAMCUscanStartDelay.Value = 60;
      txtAMCUpd_cnf.ResetText();
      chkDfltFanart.Checked = false;
      chkDfltFanartImage.Checked = false;
      chkDfltFanartImageAll.Checked = false;
      chkFanart.Checked = false;
      chkFanartDefaultViews.Checked = false;
      chkFanartDefaultViewsUseRandom.Checked = false;
      chkDfltViews.Checked = false;
      chkShowIndexedImgInIndViews.Checked = false;
      chkDfltViewsAll.Checked = false;
      chkDfltArtist.Checked = false;
      chkViews.Checked = false;
      chkPersons.Checked = false;
      chkPersonsEnableDownloads.Checked = false;
      chkAMC_Purge_Missing_Files.Checked = false;
      chkAMC_ImportOnInternetFail.Checked = false;
      AMCMovieScanPath.ResetText();
      AmcTitleSearchHandling.Text = string.Empty;
      //btnCreateAMCDesktopIcon.Enabled = false;
      //btnCreateAMCDefaultConfig.Enabled = false;
      //comboBoxLogoSpacing.ResetText();
      tbExternalPlayerPath.Text = string.Empty;
      tbExternalPlayerStartParams.Text = string.Empty;
      tbExternalPlayerExtensions.Text = string.Empty;
    }

    private void ButDelet_Click(object sender, EventArgs e)
    {
      DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this configuration? \n\nYour corresponding AMCupdater configuration and \nyour desktop icon will also be deleted!", "Warning", MessageBoxButtons.YesNo,
      MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        string DeleteName = Config_Name.Text;
        Remove_Config();
        if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "")) XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
        if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", "")) XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Current_Config");
        Config_Name.Items.Remove(Config_Name.Text);
        Refresh_Items(true);
        Config_Name.ResetText();
        textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
        // Remove desktop Icon and AMCupdater config
        string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string linkName = "AMC-Updater '" + DeleteName + "'";
        string shortcutFile = deskDir + @"\AMC-Updater (" + DeleteName + ")" + ".lnk";
        if (System.IO.File.Exists(deskDir + "\\" + linkName + ".url")) try { System.IO.File.Delete(deskDir + "\\" + linkName + ".url"); }
          catch { }
        if (System.IO.File.Exists(shortcutFile)) try { System.IO.File.Delete(shortcutFile); }
          catch { }
        string amCupdaterConfigFile = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + DeleteName + ".xml";
        if (System.IO.File.Exists(amCupdaterConfigFile)) try { System.IO.File.Delete(amCupdaterConfigFile); }
          catch { }
      }
    }

    private void ButCopy_Click(object sender, EventArgs e)
    {
      MyFilmsInputBox input = new MyFilmsInputBox();
      input.Text = "MyFilms - Copy Config";
      input.CatalogTypeSelectedIndex = CatalogType.SelectedIndex; // preset to currently chosen catalog type 
      input.CatalogType = CatalogType.Text; // preset to currently chosen catalog name
      input.Country = cbGrabberOverrideLanguage.Text; // to preset country to alredy existing setting ...
      input.ShowOnlyName = true; // to disable selection of country and year
      input.ShowDialog(this);
      string newCatalogType = input.CatalogType;
      int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;
      string newConfig_Name = input.ConfigName;
      if (newConfig_Name == Config_Name.Text)
      {
        MessageBox.Show("New Config Name must be different from the existing one !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
      {
        Config_Name.Text = newConfig_Name;
        MessageBox.Show("Created a copy of current Configuration !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        CatalogType.SelectedIndex = newCatalogSelectedIndex; // set to selected catalog type 
        Save_Config();
        // Create matching AMCupdater config
        Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
        CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
        Save_XML_AMCconfig(Config_Name.Text); // save new config
        Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults

        Config_Name.Focus();
        textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
      }
    }

    private void Remove_Config()
    {
      XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Empty);
    }

    private void MesFilmsSetup_Quit(object sender, FormClosedEventArgs e)
    {
      if (Config_Name.Items.Count == 0)
      {
        DialogResult dialogResult = MessageBox.Show("No Configuration defined; the plugin'll not work !", "Information", MessageBoxButtons.OKCancel,
        MessageBoxIcon.Warning);
        if (dialogResult == DialogResult.Cancel)
          return;
        else
        {
          XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
          if (textBoxPluginName.Text == "")
            textBoxPluginName.Text = "MyFilms"; // Make sure, a plugin name is given - assign default, if user didn't choose any!
          XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginName", textBoxPluginName.Text);
          XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
          XmlConfig.Save();
          LogMyFilms.Debug("(Setup) - Quit - created default empty config!");
          mydivx.Dispose();
          Close();
        }
      }
      MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
      for (int i = 0; i < (int)MesFilms_nb_config; i++)
      {
        XmlConfig.RemoveEntry("MyFilms", "MyFilms", "ConfigName" + i);
      }
      for (int i = 0; i < (int)Config_Name.Items.Count; i++)
      {
        XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, Config_Name.Items[i].ToString());
        if (Config_Name.Items.Count == 1)
        {
          XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", Config_Name.Items[i].ToString());
          XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", Config_Name.Items[i].ToString());
        }
      }
      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
      XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginName", textBoxPluginName.Text);
      XmlConfig.Save();
      LogMyFilms.Debug("(Setup) - Quit - saved base config!");
      Close();
    }

    private void ButQuit_Click(object sender, FormClosingEventArgs e)
    {
      MesFilmsSetup_Quit();
    }

    private static void MesFilmsSetup_Quit()
    {
    }

    private void Config_Name_Control(object sender, EventArgs e)
    {
      if (Config_Name.Text.Length > 0) return;
      MessageBox.Show("Give the Configuration's Name first !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      Config_Name.Focus();
    }
    private bool Verify_Config(ref string verifyResult)
    {
      if (MesFilmsCat.Text.Length > 0)
      {
        try
        {
          mydivx.Clear();
        }
        catch { }

        try
        {
          bool valid = BaseMesFilms.IsValidDb(MesFilmsCat.Text);
          if (!valid)
          {
            MessageBox.Show("Your XML file is using invalid format (AMC4.2), make sure your DB is in compatible format!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return false;
          }
        }
        catch (Exception)
        {
        }

        mydivx = ReadXml();
        if (mydivx != null)
        {
          DataRow[] movies = mydivx.Movie.Select(StrDfltSelect + AntTitle1.Text + " not like ''");
          if (mydivx.Movie.Count > 0)
          {
            if (movies.Length > 0)
              verifyResult = "Your XML file is valid with " + mydivx.Movie.Count + " Movies in your database and " + movies.Length + " Movies to display with your 'User defined Config Filter' configuration";
            else MessageBox.Show("Your XML file is valid with 0 Movie in your database but no Movie to display, you have to change the 'User defined Config Filter' or fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            if (IsAMC4AndHasInvalidCustomFields(mydivx)) MessageBox.Show("Your XML file seems to have CustomFields defined that are NOT supported by MyFilms ! Their content will get lost, if you continue!", "MyFilms Compatibility Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          }
          else if (!WizardActive)
          {
            if (MessageBox.Show("There is no Movie to display with that file ! Do you Want to continue ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.No)
            {
              MesFilmsCat.Focus();
              return false;
            }
            MessageBox.Show("You have to fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
        }
      }
      else
      {
        MessageBox.Show("Please give the XML file's name first");
        MesFilmsCat.Focus();
        return false;
      }
      return true; // save config in calling routine
    }

    private bool IsAMC4AndHasInvalidCustomFields(AntMovieCatalog myCatalog)
    {
      AntMovieCatalog.CustomFieldsPropertiesRow[] cfpCollection = myCatalog.Catalog[0].GetCustomFieldsPropertiesRows();
      if (cfpCollection.Length == 0) return false;
      var customFieldTagList = BaseMesFilms.CustomFieldDefinitions.Select(stringse => stringse[0]).ToList();
      var cFields = new ArrayList();
      AntMovieCatalog.CustomFieldRow[] cfrCollection = myCatalog.CustomFieldsProperties[0].GetCustomFieldRows();
      return cfrCollection.Any(fieldRow => !customFieldTagList.Contains(fieldRow.Tag));
    }

    private void Selected_Enreg_TextChanged()
    {
      StrDfltSelect = "";
      string wAntFilterSign;
      wAntFilterSign = this.AntFilterSign1.Text == "#" ? "<>" : this.AntFilterSign1.Text;
      if ((AntFilterItem1.Text.Length > 0) && AntFilterItem1.Text != "(none)")
        if (AntFilterItem1.Text == "DateAdded")
          switch (this.AntFilterSign1.Text)
          {
            case "not like":
            case "#":
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(this.AntFilterText1.Text) + "# or " + this.AntFilterItem1.Text + " is null) ";
              break;
            case "not in":
            case "in":
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " (" + this.DBitemList(this.AntFilterText1.Text, true) + ")) ";
              break;
            case "like in":
              this.StrDfltSelect = "(" + this.TransformedLikeIn(this.AntFilterItem1.Text, this.AntFilterText1.Text, true) + ") ";
              break;
            default:
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(this.AntFilterText1.Text) + "# ) ";
              break;
          }
        else
          switch (this.AntFilterSign1.Text)
          {
            case "not like":
            case "#":
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " '" + this.AntFilterText1.Text + "' or " + this.AntFilterItem1.Text + " is null) ";
              break;
            case "not in":
            case "in":
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " (" + this.DBitemList(this.AntFilterText1.Text, false) + ")) ";
              break;
            case "like in":
              this.StrDfltSelect = "(" + this.TransformedLikeIn(this.AntFilterItem1.Text, this.AntFilterText1.Text, false) + ") ";
              break;
            default:
              this.StrDfltSelect = "(" + this.AntFilterItem1.Text + " " + wAntFilterSign + " '" + this.AntFilterText1.Text + "') ";
              break;
          }
      if ((AntFilterComb.Text == "or") && (StrDfltSelect.Length > 0))
        StrDfltSelect = StrDfltSelect + " OR ";
      else
        if (StrDfltSelect.Length > 0)
          StrDfltSelect = StrDfltSelect + " AND ";
      wAntFilterSign = this.AntFilterSign2.Text == "#" ? "<>" : this.AntFilterSign2.Text;
      if ((AntFilterItem2.Text.Length > 0) && AntFilterItem2.Text != "(none)")
        if (AntFilterItem2.Text == "DateAdded")
          switch (this.AntFilterSign2.Text)
          {
            case "not like":
            case "#":
              this.StrDfltSelect = "(" + this.StrDfltSelect + "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(this.AntFilterText2.Text) + "# or " + this.AntFilterItem2.Text + " is null)) AND ";
              break;
            case "not in":
            case "in":
              this.StrDfltSelect = "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " (" + this.DBitemList(this.AntFilterText2.Text, true) + ")) AND ";
              break;
            case "like in":
              this.StrDfltSelect = "(" + this.TransformedLikeIn(this.AntFilterItem2.Text, this.AntFilterText2.Text, true) + ") AND ";
              break;
            default:
              this.StrDfltSelect = "(" + this.StrDfltSelect + "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(this.AntFilterText2.Text) + "# )) AND ";
              break;
          }
        else
          switch (this.AntFilterSign2.Text)
          {
            case "not like":
            case "#":
              this.StrDfltSelect = "(" + this.StrDfltSelect + "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " '" + this.AntFilterText2.Text + "' or " + this.AntFilterItem2.Text + " is null)) AND ";
              break;
            case "not in":
            case "in":
              this.StrDfltSelect = "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " (" + this.DBitemList(this.AntFilterText2.Text, false) + ")) AND ";
              break;
            case "like in":
              this.StrDfltSelect = "(" + this.TransformedLikeIn(this.AntFilterItem2.Text, this.AntFilterText2.Text, true) + ") AND ";
              break;
            default:
              this.StrDfltSelect = "(" + this.StrDfltSelect + "(" + this.AntFilterItem2.Text + " " + wAntFilterSign + " '" + this.AntFilterText2.Text + "' )) AND ";
              break;
          }
      if (!string.IsNullOrEmpty(AntFilterFreeText.Text))
        StrDfltSelect = StrDfltSelect + AntFilterFreeText.Text + " AND ";
      //Selected_Enreg.Text = StrDfltSelect + AntTitle1.Text + " not like ''";
      //LogMyFilms.Debug("MyFilms (Build Selected Enreg) - Selected_Enreg: '" + Selected_Enreg.Text + "'");
    }

    private string DBitemList(string inputstring, bool isdate)
    {
      string returnValue = "";
      string[] split = inputstring.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (returnValue.Length > 0) returnValue += ", ";
        if (isdate)
          returnValue += "#" + Convert.ToDateTime(s) + "#";
        else
          returnValue += "'" + s + "'";
      }
      return returnValue;
    }

    private string TransformedLikeIn(string dbfield, string inputstring, bool isdate)
    {
      string returnValue = "";
      string[] split = inputstring.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (returnValue.Length > 0) returnValue += " OR ";
        if (isdate)
          returnValue += "(" + dbfield + " like #" + Convert.ToDateTime(s) + "#)";
        else
          returnValue += "(" + dbfield + " like '*" + s + "*')";
      }
      return returnValue;
    }

    private void Selected_Enreg_Changed(object sender, EventArgs e)
    {
      Selected_Enreg_TextChanged();
    }

    private bool IsAMCcatalogType(int selectedindex)
    {
      return (selectedindex == 0 || selectedindex == 10);
    }

    private void CatalogType_SelectedIndexChanged(object sender, EventArgs e)
    {
      AddRemoveExtendedfields(CatalogType.SelectedIndex > 0);
      if (!IsAMCcatalogType(CatalogType.SelectedIndex)) // all presets for "Non-ANT-MC-Catalogs/External Catalogs"
      {
        if (!NewConfigButton)
        {
          chkFanart.Checked = true;
          chkDfltFanart.Checked = false;
          chkDfltFanartImage.Checked = true;
          chkDfltFanartImageAll.Checked = true;
          chkPersons.Checked = true; // enable person thumbs
          Tab_AMCupdater.Enabled = false;
          Tab_Update.Enabled = false;
          if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "en" || AntTitle1.Text == "OriginalTitle")
          {
            //if (string.IsNullOrEmpty(AntTitle1.Text))
            AntTitle1.Text = "OriginalTitle";
            AntTitle2.Text = "TranslatedTitle";
            ItemSearchFileName.Text = "OriginalTitle";
            AntSTitle.Text = "OriginalTitle";
          }
          else
          {
            AntTitle1.Text = "TranslatedTitle";
            AntTitle2.Text = "OriginalTitle";
            ItemSearchFileName.Text = "TranslatedTitle";
            AntSTitle.Text = "TranslatedTitle";
          }
          //if (string.IsNullOrEmpty(AntSTitle.Text)) AntSTitle.Text = "FormattedTitle";
          TitleDelim.Text = "\\";
          SearchFileName.Checked = true;

          //Presets for useritems for main screen:
          AntItem1.Text = "Category";
          AntLabel1.Text = BaseMesFilms.TranslateColumn(AntItem1.Text.Trim());
          AntItem2.Text = "Country";
          AntLabel2.Text = BaseMesFilms.TranslateColumn(AntItem2.Text.Trim());
          AntItem3.Text = "Director";
          AntLabel3.Text = BaseMesFilms.TranslateColumn(AntItem3.Text.Trim());
          AntItem4.Text = "Languages";
          AntLabel4.Text = BaseMesFilms.TranslateColumn(AntItem4.Text.Trim());
          AntItem5.Text = "Certification";
          AntLabel5.Text = BaseMesFilms.TranslateColumn(AntItem5.Text.Trim());

          //Presets for useritems for details screen:
          AntItemDetails1.Text = "Category";
          AntLabelDetails1.Text = BaseMesFilms.TranslateColumn(AntItemDetails1.Text.Trim());
          AntItemDetails2.Text = "Country";
          AntLabelDetails2.Text = BaseMesFilms.TranslateColumn(AntItemDetails2.Text.Trim());
          AntItemDetails3.Text = "Director";
          AntLabelDetails3.Text = BaseMesFilms.TranslateColumn(AntItemDetails3.Text.Trim());
          AntItemDetails4.Text = "Producer";
          AntLabelDetails4.Text = BaseMesFilms.TranslateColumn(AntItemDetails4.Text.Trim());
          AntItemDetails5.Text = "Writer";
          AntLabelDetails5.Text = BaseMesFilms.TranslateColumn(AntItemDetails5.Text.Trim());
          AntItemDetails6.Text = "Certification";
          AntLabelDetails6.Text = BaseMesFilms.TranslateColumn(AntItemDetails6.Text.Trim());
        }
        Tab_ExternalCatalogs.Enabled = true;
        txtPicturePrefix.Text = "";
        groupBoxExtendedFieldHandling.Enabled = true;
      }
      groupBox_DVDprofiler.Enabled = false; // deaktivates DVDprofiler options as default...
      switch (CatalogType.SelectedIndex)
      #region catalog specific settings
      //0	Ant Movie Catalog (V3.5.1.2)
      //1	DVD Profiler (V3.7.2)
      //2	Movie Collector (V7.1.4)
      //3	MyMovies (V3.18)
      //4	Eax Movie Catalog (2.5.0)
      //5	Eax Movie Catalog (3.0.9 b5)
      //6	PVD - Personal Video Database (0.9.9.21)
      //7	eXtreme Movie Manager (V7.1.1.1)
      //8	XBMC (V10.0)
      //9	MovingPicturesXML (V1.2 process plugin)
      //10	MyFilms extended Database
      //11	XBMC nfo reader          
      {
        case 0:
          groupBoxExtendedFieldHandling.Enabled = false;
          Tab_AMCupdater.Enabled = true;
          Tab_Update.Enabled = true;
          break;
        case 1: // DVDprofiler
          groupBox_DVDprofiler.Enabled = true;
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = ""; // Disable Trailers ...
          if (MesFilmsCat.Text.Length > 0)
          {
            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Images"; // cover path
            //MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Persons"; // person thumb path
            //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Fanart"; // fanart path
          }
          cbWatched.Text = "Checked";
          break;
        case 3: // MyMovies
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "SourceTrailer";
          if (MesFilmsCat.Text.Length > 0)
          {
            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // cover path
            MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // person thumb path
            //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // fanart path
          }
          chkAddTagline.Checked = true;
          ECMergeDestinationFieldTagline.Text = "Description";
          break;
        case 4: // EAX MC 2.5.0
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "SourceTrailer";
          if (MesFilmsCat.Text.Length > 0)
          {
            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures"; // cover path
            MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\NamePictures"; // person thumb path
            // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Thumbnails"; // fanart path
          }
          break;

        case 5: // EAX MC 3.0.9
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "SourceTrailer";
          if (MesFilmsCat.Text.Length > 0)
          {
            //if (MesFilmsImg.Text.Length == 0)
            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures"; // cover path
            MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\NamePictures"; // person thumb path
            // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Thumbnails"; // fanart path
            //a.	Cover image folder = D:My Documents\Data\Eax Movie Catalog \Pictures
            //b.	Person Thumbs = D:My Documents\Data\Eax Movie Catalog\NamePictures
            //c.	Fanart = D:My Documents\Eax Movie Catalog\Thumbnails – depends on the script i.e. grabs photos, but TMDB scrip grabs fanart but if users have fanart in EAX this folder is where it will be stored.
          }
          cbWatched.Text = "Checked";
          break;

        case 6: // PVD Personal Video Database V0.9.9.21
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "Borrower";
          if (MesFilmsCat.Text.Length > 0)
          {
            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
            MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Photos"; // person thumb path
            // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Screenshots"; // fanart path
            // o	Posters - D:\My Documents\Personal Video Database\TEST\Posters – movie covers used in PVD DB - but setting to DB path required !
            //o	D:\My Documents\Personal Video Database\TEST\Photos – actor/person images
            //o	D:\My Documents\Personal Video Database\TEST\Screenshots – could be video thumbnails, fanart, manually saved thumbs
            //o	D:\My Documents\Personal Video Database\TEST\images – PVD copies covers to this folder when you export to xml and these are the filenames exported for cover images, i.e.:
            //	<poster>images/image1_66_-1x-1.jpg</poster>
          }
          cbWatched.Text = "Checked";
          chkAddTagline.Checked = true;
          ECMergeDestinationFieldTagline.Text = "Description";
          break;

        case 7: // XMM Extreme Movie Manager
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "SourceTrailer";
          if (MesFilmsCat.Text.Length > 0)
          {
            //<XMM_Movie_Database>
            //<DBName='Test.mdb'>
            //<DBpath='D:\My Documents\eXtreme Movie Manager 7\Databases\Test.mdb'>
            //Then 
            //Covers path = D:\My Documents\eXtreme Movie Manager 7\Databases\Test_cover
            //Persons Thumbs = D:\My Documents\eXtreme Movie Manager 7\Databases\Test_photos
            string DBname = string.Empty;
            string DBpath = string.Empty;
            string XMMthumbpath = string.Empty;
            try
            {
              // try to get path infos from export file
              XmlDocument doc = new XmlDocument();
              doc.Load(MesFilmsCat.Text);
              //XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/XMM_Movie_Database/Movie");
              //foreach (XmlNode nodeDVD in dvdList)
              //{ }
              XmlNode nodeDBname = doc.DocumentElement.SelectSingleNodeFast("/XMM_Movie_Database/DBName");
              XmlNode nodeDBpath = doc.DocumentElement.SelectSingleNodeFast("/XMM_Movie_Database/DBpath");
              if (nodeDBname != null && nodeDBname.InnerText.Length > 0)
                DBname = nodeDBname.InnerText.Replace(char.ConvertFromUtf32(160), " ");
              if (nodeDBpath != null && nodeDBpath.InnerText.Length > 0)
                DBpath = nodeDBpath.InnerText.Replace(char.ConvertFromUtf32(160), " ");
              if (!string.IsNullOrEmpty(DBname) && !string.IsNullOrEmpty(DBpath))
                XMMthumbpath = DBpath.Substring(0, DBpath.LastIndexOf("\\") + 1) + DBname.Substring(0, DBname.IndexOf("."));
            }
            catch (Exception) { }

            // C:\WinApps\Video\eXtreme Movie Manager 7\Databases\<DB-name>_cover etc.
            //string strDatadirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
            //string[] directories = System.IO.Directory.GetDirectories(strDatadirectory);
            //string lastDirectory = string.Empty;
            //foreach (string directory in directories)
            //{
            //  if (string.Compare(directory, lastDirectory) >= 1)
            //  {
            //    lastDirectory = directory;
            //  }
            //}
            //string strDbName = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
            //strDbName = strDbName.Substring(0, strDbName.LastIndexOf(".")); // results in filename only without extension
            //MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_cover"; // cover path
            //MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_photos"; // person thumb path
            //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_thumbs"; // fanart path - better use _cover, as otherwise only small thumbs !
            //a.	…eXtreme Movie Manager 7\Databases\Test_cover – for movie covers
            //b.	… eXtreme Movie Manager 7\Databases\Test_photos – for person thumbs – filename format is Steve-Martin_143135.jpg
            //c.	… eXtreme Movie Manager 7\Databases\Test_thumbs – for covers and thumbnails – this is where fanart might be stored as #-[movietitle]_fanart.jpg
            if (!string.IsNullOrEmpty(XMMthumbpath))
            {
              MesFilmsImg.Text = XMMthumbpath + "_cover"; // cover path
              MesFilmsImgArtist.Text = XMMthumbpath + "_photos"; // person thumb path
              // MesFilmsFanart.Text = XMMthumbpath + "_thumbs"; // fanart path - better use _cover, as otherwise only small thumbs !
              MesFilmsFanart.Text = XMMthumbpath + "_cover";
            }
            else
            {
              MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // cover path
              MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // person thumb path
              //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // fanart path
            }
            chkAddTagline.Checked = true;
            ECMergeDestinationFieldTagline.Text = "Description";
          }
          break;

        case 9: // MovingPicturesXML V1.2
          AntStorage.Text = "Source";
          AntStorageTrailer.Text = "SourceTrailer";
          if (MesFilmsCat.Text.Length > 0)
          {
            MesFilmsImg.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MovingPictures\Covers\FullSize"; // Covers path
            MesFilmsImgArtist.Text = ""; // person thumb path
            // Did Work! MesFilmsFanart.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MovingPictures\Backdrops\FullSize"; // fanart path
          }
          cbWatched.Text = "Checked";
          break;

        case 10: // MyFilms DB (currently same as ANT Movie Catalog, but possible to extend DB fields in the future
          break;

        default:
          if (string.IsNullOrEmpty(AntStorage.Text))
            AntStorage.Text = "Source";
          if (string.IsNullOrEmpty(AntStorageTrailer.Text))
            AntStorageTrailer.Text = "Borrower";
          break;
      }
      #endregion

      chkEnhancedWatchedStatusHandling.Checked = (CatalogType.SelectedIndex != 0); // autoset this by catalog type
      groupBoxMultiUserState.Enabled = (CatalogType.SelectedIndex != 0);
    }

    private void AddDefaultViews()
    {
      MFview.ViewRow newRow = null;

      //Films (mastertitle)
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = AntTitle1.Text;
      newRow.Label = GUILocalizeStrings.Get(342); // videos
      newRow.Value = "*";
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Films.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Year
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Year";
      newRow.SortDirectionView = " DESC";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Year.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Category
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Category";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Category.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Country
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Country";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Country.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //RecentlyAdded
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "RecentlyAdded";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\RecentlyAdded.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Indexed title view
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = AntTitle1.Text;
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.Index = 1;
      newRow.Value = "*";
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\TitlesIndex.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Box Sets view (mastertitle)
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = AntTitle1.Text;
      newRow.Label = "Box Sets";
      newRow.Value = "*";
      newRow.Filter = @"(" + AntTitle1.Text + @" like '*\*') ";
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Films.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Actors
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Actors";
      newRow.Index = 1;
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\PersonsIndex.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Producer
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Producer";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Persons.jpg";
      MyCustomViews.View.Rows.Add(newRow);
    }

    private void AddOldHardcodedViews()
    {
      MFview.ViewRow newRow = MyCustomViews.View.NewViewRow();

      //Films (mastertitle)
      newRow.DBfield = AntTitle1.Text;
      newRow.Label = GUILocalizeStrings.Get(342); // videos
      newRow.Value = "*";
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Films.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //year
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Year";
      newRow.SortDirectionView = " DESC";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Year.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Category
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Category";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Category.jpg";
      MyCustomViews.View.Rows.Add(newRow);
      //Country
      newRow = MyCustomViews.View.NewViewRow();
      newRow.DBfield = "Country";
      newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
      newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Country.jpg";
      MyCustomViews.View.Rows.Add(newRow);
    }

    private void UpdateOldHardcodedViews()
    {
      // upgrade old custom views to new image locations ...
      foreach (MFview.ViewRow customView in this.MyCustomViews.View.Rows.Cast<MFview.ViewRow>().Where(customView => customView.DBfield == "Producer"))
      {
        customView.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Persons.jpg";
      }
      pictureBoxDefaultCover.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Cover2.jpg"; //DefaultCover.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Cover.jpg";
      pictureBoxDefaultPersonImage.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Persons2.jpg";
      pictureBoxDefaultViewImage.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Views.jpg";
      pictureBoxDefaultFanart.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Fanart.jpg";
    }

    private void btnGrabber_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtGrabber.Text))
        openFileDialog1.FileName = txtGrabber.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts);
      }

      if (txtGrabber.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = txtGrabber.Text.Substring(0, txtGrabber.Text.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "xml";
      openFileDialog1.Filter = "XML Files|*.xml";
      openFileDialog1.Title = "Select Default Internet Grabber Script (xml file)";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        txtGrabber.Text = openFileDialog1.FileName;
        txtGrabberDisplay.Text = !string.IsNullOrEmpty(txtGrabber.Text) ? Path.GetFileName(txtGrabber.Text) : string.Empty;
      }
    }

    //private void btnAMCUpd_exe_Click(object sender, EventArgs e)
    //{
    //    openFileDialog1.RestoreDirectory = true;
    //    openFileDialog1.DefaultExt = "AMCUpdater.exe";
    //    openFileDialog1.Filter = "exe Files|AMCUpdater.exe";
    //    openFileDialog1.Title = "Find AMCUpdater program";
    //    if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
    //        txtAMCUpd_exe.Text = openFileDialog1.FileName;
    //}

    private void btnAMCUpd_cnf_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtAMCUpd_cnf.Text))
        openFileDialog1.FileName = txtAMCUpd_cnf.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString();
      }

      if (txtAMCUpd_cnf.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = txtAMCUpd_cnf.Text.Substring(0, txtAMCUpd_cnf.Text.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "xml";
      openFileDialog1.Filter = "XML AMCUpdater Config Files|*.xml";
      openFileDialog1.Title = "Select AMCUPdater Config file (xml file)";
      openFileDialog1.CheckFileExists = false;
      openFileDialog1.CheckPathExists = false;
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (!System.IO.File.Exists(openFileDialog1.FileName))
        {
          if (MessageBox.Show("That AMCUpdater config file doesn't exists, do you want to create one with default values ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            try
            {
              if (!Directory.Exists(Path.GetDirectoryName(openFileDialog1.FileName)))
              {
                Directory.CreateDirectory(Path.GetDirectoryName(openFileDialog1.FileName));
              }
              txtAMCUpd_cnf.Text = openFileDialog1.FileName;
              if (!System.IO.File.Exists(txtAMCUpd_cnf.Text))
              {
                Read_XML_AMCconfig(Config_Name.Text); // create a new default config at the specified location
              }
            }
            catch (Exception ex)
            {
              openFileDialog1.CheckFileExists = true;
              openFileDialog1.CheckPathExists = true;
              MessageBox.Show("File or directory could not be created ! - Message: '" + ex.Message + "'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
          }
        }
        else
        {
          txtAMCUpd_cnf.Text = openFileDialog1.FileName;
        }
      }
      openFileDialog1.CheckFileExists = true;
      openFileDialog1.CheckPathExists = true;
    }

    private void chkAMCUpd_CheckedChanged(object sender, EventArgs e)
    {
      if (chkAMCUpd.Checked)
      {
        groupBoxAMCUpdaterConfigFile.Enabled = true;
        groupBox_AMCupdater_ExternalApplication.Enabled = true;
        groupBox_AMCupdaterScheduer.Enabled = true;
        chkAMCUscanOnStartup.Enabled = true;
        chkAMCUwatchScanFolders.Enabled = true;
        labelAMCUstartdelay.Enabled = true;
        numericUpDownAMCUscanStartDelay.Enabled = true;
        chkAMC_ImportOnInternetFail.Enabled = true;
      }
      else
      {
        groupBoxAMCUpdaterConfigFile.Enabled = false;
        groupBox_AMCupdater_ExternalApplication.Enabled = false;
        groupBox_AMCupdaterScheduer.Enabled = false;
        chkAMCUscanOnStartup.Enabled = false;
        chkAMCUwatchScanFolders.Enabled = false;
        labelAMCUstartdelay.Enabled = false;
        numericUpDownAMCUscanStartDelay.Enabled = false;
        chkAMC_ImportOnInternetFail.Enabled = false;
      }
    }

    private void btnFanart_Click(object sender, EventArgs e)
    {
      folderBrowserDialog1.SelectedPath = !String.IsNullOrEmpty(MesFilmsFanart.Text) ? MesFilmsFanart.Text : String.Empty;

      folderBrowserDialog1.Description = "Select Fanart Backdrop Path";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        MesFilmsFanart.Text = folderBrowserDialog1.SelectedPath;
        lblResultingGroupViewsPathFanart.Text = MesFilmsFanart.Text + "\\_View\\";
      }
    }

    private void chkFanart_CheckedChanged(object sender, EventArgs e)
    {
      if (chkFanart.Checked)
      {
        MesFilmsFanart.Enabled = true;
        pictureBoxDefaultFanart.Enabled = true;
        btnFanart.Enabled = true;
        buttonDefaultFanartReset.Enabled = true;
        chkDfltFanart.Enabled = true;
        chkDfltFanartImage.Enabled = true;
        chkDfltFanartImageAll.Enabled = true;
        chkFanartDefaultViews.Enabled = true;
        chkFanartDefaultViewsUseRandom.Enabled = true;
      }
      else
      {
        MesFilmsFanart.Enabled = false;
        pictureBoxDefaultFanart.Enabled = false;
        btnFanart.Enabled = false;
        buttonDefaultFanartReset.Enabled = false;
        chkDfltFanart.Enabled = false;
        chkDfltFanartImage.Enabled = false;
        chkDfltFanartImageAll.Enabled = false;
        chkFanartDefaultViews.Enabled = false;
        chkFanartDefaultViewsUseRandom.Enabled = false;
      }
    }


    private void pictureBox_Click(object sender, EventArgs e)
    {

      openFileDialog1.FileName = !string.IsNullOrEmpty(SFilePicture.Text) ? SFilePicture.Text : String.Empty;
      if (SFilePicture.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = SFilePicture.Text.Substring(0, SFilePicture.Text.LastIndexOf("\\") + 1);
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "png";
      openFileDialog1.Filter = "PNG Files|*.png";
      openFileDialog1.Title = "Find Logos files (png file)";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (MediaPortal.Util.Utils.IsPicture(openFileDialog1.FileName))
        {
          SPicture.BackgroundImage = ImageFast.FastFromFile(openFileDialog1.FileName);
          SPicture.BackgroundImageLayout = ImageLayout.Stretch;
          SFilePicture.Text = openFileDialog1.FileName;
        }
        else
        {
          MessageBox.Show("File choosen isn't a Picture !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          SPicture.Focus();
        }
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (SLogo_Type.Text.Length == 0)
      {
        MessageBox.Show("Logo Type must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        SLogo_Type.Focus();
        return;
      }
      if ((SField1.Text.Length == 0) || (SOp1.Text.Length == 0) || (SValue1.Text.Length == 0))
      {
        if ((SField1.Text.Length == 0 || SOp1.Text.Length == 0) && (SOp1.Text != "filled" && SOp1.Text != "not filled"))
        {
          MessageBox.Show("The three Fields for comparison must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          SField1.Focus();
          return;
        }
      }
      if ((SAnd_Or.Text.Length > 0) && ((SField2.Text.Length == 0) || (SOp2.Text.Length == 0) || (SValue2.Text.Length == 0)))
      {
        if ((SField2.Text.Length == 0 || SOp2.Text.Length == 0) && (SOp2.Text != "filled" && SOp2.Text != "not filled"))
        {
          MessageBox.Show("The three Fields for comparison must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          SField2.Focus();
          return;
        }
      }
      if ((SAnd_Or.Text.Length == 0) && ((SField2.Text.Length > 0) || (SOp2.Text.Length > 0) || (SValue2.Text.Length > 0)))
      {
        MessageBox.Show("The Operator 'AND' or 'OR' must be defined for Two conditions !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        SAnd_Or.Focus();
        return;
      }
      if (SPicture.BackgroundImage == null)
      {
        MessageBox.Show("Logo must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        SPicture.Focus();
        return;
      }
      if (selected_Logo_Item > -1)
      {
        //Edit_Item(selected_Logo_Item);
        selected_Logo_Item = -1;
      }
      //else
      //{
      LogoView.Items.Add(SLogo_Type.Text);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField1.Text);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SOp1.Text.ToLower());
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SValue1.Text.ToLower());
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SAnd_Or.Text.ToLower());
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField2.Text);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField2.Text.ToLower());
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SValue2.Text.ToLower());
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetFileName(SFilePicture.Text));
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(SFilePicture.Text));
      //}
      SField1.Text = string.Empty;
      SOp1.Text = string.Empty;
      SValue1.Text = string.Empty;
      SAnd_Or.Text = string.Empty;
      SField2.Text = string.Empty;
      SOp2.Text = string.Empty;
      SValue2.Text = string.Empty;
      SFilePicture.Text = string.Empty;
      SPicture.BackgroundImage = null;
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      if (LogoView.SelectedItems.Count != 0)
      {
        DialogResult rc = MessageBox.Show("Focused Item'll be remove, do you confirm ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (rc == DialogResult.Yes)
        {
          LogoView.SelectedItems[0].Remove();
          selected_Logo_Item = -1;
        }
      }
      else
      {
        MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      if (selected_Logo_Item != -1)
      {
        //Select_Item(LogoView.SelectedItems[0].Index, false);
        //Move_Item(LogoView.SelectedItems[0].Index, LogoView.SelectedItems[0].Index); // this is a move to "same position"
        Edit_Item(selected_Logo_Item);
        Select_Item(selected_Logo_Item, true);
        Save_XML_Logos();
      }
      else
      {
        MessageBox.Show("Please first select an Item for updating !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      }
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      if (LogoView.SelectedItems.Count != 0)
      {
        if (LogoView.SelectedItems[0].Index == 0)
          return;
        Select_Item(LogoView.SelectedItems[0].Index, false);
        Move_Item(LogoView.SelectedItems[0].Index - 1, LogoView.SelectedItems[0].Index);
        Edit_Item(LogoView.SelectedItems[0].Index - 1);
        LogoView.Items[LogoView.SelectedItems[0].Index - 1].Selected = true;
      }
      else
      {
        MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      }

    }
    private void btnDown_Click(object sender, EventArgs e)
    {
      if (LogoView.SelectedItems.Count != 0)
      {
        if (LogoView.SelectedItems[0].Index == LogoView.Items.Count - 1)
          return;
        Select_Item(LogoView.SelectedItems[0].Index, false);
        Move_Item(LogoView.SelectedItems[0].Index + 1, LogoView.SelectedItems[0].Index);
        Edit_Item(LogoView.SelectedItems[0].Index + 1);
      }
      else
      {
        MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        return;
      }
      LogoView.Items[LogoView.SelectedItems[0].Index + 1].Selected = true;
    }
    private void LogoView_SelectedIndexChanged(object sender, EventArgs e)
    {
      //if (LogoView.Items.Count != 0)
      if (LogoView.SelectedItems.Count != 0)
        if ((LogoView.SelectedItems[0].Index > -1) && (LogoView.SelectedItems[0].Index < LogoView.Items.Count))
          Select_Item(LogoView.SelectedItems[0].Index, true);
    }

    private void Select_Item(int select_item, bool select)
    {
      SLogo_Type.Text = LogoView.Items[select_item].SubItems[0].Text;
      SField1.Text = LogoView.Items[select_item].SubItems[1].Text;
      SOp1.Text = LogoView.Items[select_item].SubItems[2].Text;
      SValue1.Text = LogoView.Items[select_item].SubItems[3].Text;
      SAnd_Or.Text = LogoView.Items[select_item].SubItems[4].Text;
      SField2.Text = LogoView.Items[select_item].SubItems[5].Text;
      SOp2.Text = LogoView.Items[select_item].SubItems[6].Text;
      SValue2.Text = LogoView.Items[select_item].SubItems[7].Text;

      // Search logo file according settings, rule and active skin
      //textBoxActiveLogoPath.Text = "";
      //if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(LogoView.Items[select_item].SubItems[7].Text)))
      //  textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(LogoView.Items[select_item].SubItems[7].Text);
      //else if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ActiveLogoPath + @"\" + LogoView.Items[select_item].SubItems[7].Text))) // Check if Logofile exists in default media directory of current skin and add it, if found
      //  textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(ActiveLogoPath + @"\" + LogoView.Items[select_item].SubItems[7].Text);
      //else if (!LogoView.Items[select_item].SubItems[7].Text.Contains("\\")) // Check, if logo file is present in subdirectories of logo directory of current skin - only if not already full path defined !
      //  {
      //    string[] filePathsLogoSearch = System.IO.Directory.GetFiles(ActiveLogoPath + @"\", LogoView.Items[select_item].SubItems[7].Text, System.IO.SearchOption.AllDirectories);
      //    if (filePathsLogoSearch.Length > 0)
      //    {
      //      textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(filePathsLogoSearch[0]);
      //    }
      //  }
      //else textBoxActiveLogoPath.Text = String.Empty;
      //if (textBoxActiveLogoPath.Text.Length > 0)
      //  LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(ActiveLogoPath);
      //  else
      //  LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(string.Empty); // Add empty field, if logofile istn't found anywhere


      string wfile = string.Empty;
      if (LogoView.Items[select_item].SubItems[9].Text.Length > 0)
        wfile = LogoView.Items[select_item].SubItems[9].Text + "\\" + LogoView.Items[select_item].SubItems[8].Text;
      else
        wfile = LogoView.Items[select_item].SubItems[8].Text;
      SFilePicture.Text = wfile;
      if (System.IO.File.Exists(wfile))
      {
        SPicture.BackgroundImage = ImageFast.FastFromFile(SFilePicture.Text);
      }
      else
      {
        if (SPicture.BackgroundImage != null)
        {
          SPicture.BackgroundImage.Dispose();
          SPicture.BackgroundImage = null;
        }
      }
      if (select)
      {
        selected_Logo_Item = select_item;
      }
    }
    private void Move_Item(int select_item, int dest_item)
    {
      LogoView.Items[dest_item].SubItems[0].Text = LogoView.Items[select_item].SubItems[0].Text;
      LogoView.Items[dest_item].SubItems[1].Text = LogoView.Items[select_item].SubItems[1].Text;
      LogoView.Items[dest_item].SubItems[2].Text = LogoView.Items[select_item].SubItems[2].Text;
      LogoView.Items[dest_item].SubItems[3].Text = LogoView.Items[select_item].SubItems[3].Text;
      LogoView.Items[dest_item].SubItems[4].Text = LogoView.Items[select_item].SubItems[4].Text;
      LogoView.Items[dest_item].SubItems[5].Text = LogoView.Items[select_item].SubItems[5].Text;
      LogoView.Items[dest_item].SubItems[6].Text = LogoView.Items[select_item].SubItems[6].Text;
      LogoView.Items[dest_item].SubItems[7].Text = LogoView.Items[select_item].SubItems[7].Text;
      LogoView.Items[dest_item].SubItems[8].Text = LogoView.Items[select_item].SubItems[8].Text;
      LogoView.Items[dest_item].SubItems[9].Text = LogoView.Items[select_item].SubItems[9].Text;
    }
    private void Edit_Item(int dest_item)
    {
      LogoView.Items[dest_item].SubItems[0].Text = SLogo_Type.Text;
      LogoView.Items[dest_item].SubItems[1].Text = SField1.Text;
      LogoView.Items[dest_item].SubItems[2].Text = SOp1.Text.ToLower();
      LogoView.Items[dest_item].SubItems[3].Text = SValue1.Text.ToLower();
      LogoView.Items[dest_item].SubItems[4].Text = SAnd_Or.Text.ToLower();
      LogoView.Items[dest_item].SubItems[5].Text = SField2.Text;
      LogoView.Items[dest_item].SubItems[6].Text = SOp2.Text.ToLower();
      LogoView.Items[dest_item].SubItems[7].Text = SValue2.Text.ToLower();

      LogoView.Items[dest_item].SubItems[8].Text = System.IO.Path.GetFileName(SFilePicture.Text);
      LogoView.Items[dest_item].SubItems[9].Text = System.IO.Path.GetDirectoryName(SFilePicture.Text);
      selected_Logo_Item = dest_item;
      LogoView.Items[dest_item].Checked = true;
      SField1.Text = string.Empty;
      SOp1.Text = string.Empty;
      SValue1.Text = string.Empty;
      SAnd_Or.Text = string.Empty;
      SField2.Text = string.Empty;
      SOp2.Text = string.Empty;
      SValue2.Text = string.Empty;
      SFilePicture.Text = string.Empty;
      SPicture.BackgroundImage = null;
      LogoView.Focus();
    }

    private void SField1_SelectedIndexChanged(object sender, EventArgs e)
    {
      Update_Svalue(SField1, SOp1, ref SValue1);
    }
    private void SField2_SelectedIndexChanged(object sender, EventArgs e)
    {
      Update_Svalue(SField2, SOp2, ref SValue2);
    }
    private void Update_Svalue(ComboBox sField, ComboBox sOp, ref ComboBox sValue)
    {
      string WSelect = string.Empty;
      switch (sOp.Text.ToLower())
      {
        case "equal":
          WSelect = sField.Text + " = " + sValue.Text;
          break;
        case "not equal":
          WSelect = sField.Text + " # " + sValue.Text;
          break;
        case "contains":
          WSelect = sField.Text + " like '*" + sValue.Text + "*'";
          break;
        case "not contains":
          WSelect = sField.Text + " not like '*" + sValue.Text + "*'";
          break;
        case "greater":
          WSelect = sField.Text + " greater than '*" + sValue.Text + "*'";
          break;
        case "lower":
          WSelect = sField.Text + " lower than '*" + sValue.Text + "*'";
          break;
      }
      //if (WSelect.Length > 0)
      //    WSelect = " and " + WSelect;
      if (mydivx.Contents.Count == 0 && Config_Name.Text.Length > 0)
      {
        mydivx.Clear();
        if (!string.IsNullOrEmpty(MesFilmsCat.Text)) // only read, if catalog file is defined !
          mydivx = ReadXml();
        LogoView.Items.Clear();
        selected_Logo_Item = -1;
        //Read_XML_Logos(Config_Name.Text);
        Read_XML_Logos();
      }
      DataRow[] movies = mydivx.Tables["Movie"].Select(sField.Text + " is not null", sField.Text + " ASC");
      string wsfield = null;
      sValue.Items.Clear();
      foreach (DataRow enr in movies)
      {
        if (enr[sField.Text].ToString().ToLower() != wsfield)
        {
          wsfield = enr[sField.Text].ToString().ToLower();
          sValue.Items.Add(wsfield);
        }
      }
    }
    public AntMovieCatalog ReadXml()
    {
      var reader = new XmlTextReader(MesFilmsCat.Text);
      try
      {
        while (reader.Read())
        {
        }
        if (reader != null)
        {
          reader.Close();
          string destFile = "";
          // ec options
          string destinationTagline = "";
          string destinationTags = "";
          string destinationCertification = "";
          string destinationWriter = "";

          if (!IsAMCcatalogType(CatalogType.SelectedIndex))
          {
            destinationTagline = chkAddTagline.Checked ? ECMergeDestinationFieldTagline.Text : "";
            destinationTags = chkAddTags.Checked ? ECMergeDestinationFieldTags.Text : "";
            destinationCertification = chkAddCertification.Checked ? ECMergeDestinationFieldCertification.Text : "";
            destinationWriter = chkAddWriter.Checked ? ECMergeDestinationFieldWriter.Text : "";
          }

          switch (CatalogType.SelectedIndex)
          {
            case 0: // ANT Movie Catalog
            case 10: // Starter Settings ANT DB
              mydivx.ReadXml(MesFilmsCat.Text);
              break;
            case 1: //DVD Profiler
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var cc1 = new DvdProfiler("Category");
              mydivx.ReadXml(cc1.ConvertProfiler(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, "Category", chkDVDprofilerOnlyFile.Checked));
              break;
            case 2: // Movie Collector V7.1.4
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var cc2 = new MovieCollector();
              mydivx.ReadXml(cc2.ConvertMovieCollector(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
              break;
            case 3: // MyMovies
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var mm = new MyMovies();
              mydivx.ReadXml(mm.ConvertMyMovies(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked));
              break;
            case 4: // EAX Movie Catalog 2.5.0
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var emc = new EaxMovieCatalog();
              mydivx.ReadXml(emc.ConvertEaxMovieCatalog(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
              break;
            case 5: // EAX Movie Catalog 3.0.9 (beta5)
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var emc3 = new EaxMovieCatalog3();
              mydivx.ReadXml(emc3.ConvertEaxMovieCatalog3(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
              break;
            case 6: // PVD PersonalVideoDatabase V0.9.9.21
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var pvd = new PersonalVideoDatabase();
              mydivx.ReadXml(pvd.ConvertPersonalVideoDatabase(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text, this.chkAddTagline.Checked));
              break;
            case 7: //eXtreme Movie Manager
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var xmm = new XMM();
              mydivx.ReadXml(xmm.ConvertXMM(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked));
              break;
            case 8: // XBMC fulldb export (all movies in one DB)
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if (System.IO.File.Exists(destFile))
              {
                FileInfo fInfo = new FileInfo(destFile);
                long size = fInfo.Length;
                if (size == 0)
                  System.IO.File.Delete(destFile);
              }
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var xdb = new XbmcDb();
              mydivx.ReadXml(xdb.ConvertXbmcDb(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, AntStorage.Text, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
              break;
            case 9: // MovingPicturesXML
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var mopi = new MovingPicturesXML();
              mydivx.ReadXml(mopi.ConvertMovingPicturesXML(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, chkDVDprofilerOnlyFile.Checked));
              break;
            case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
              destFile = MesFilmsCat.Text;
              if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
              {
                mydivx.ReadXml(destFile);
                break;
              }
              var nfo = new XbmcNfo();
              mydivx.ReadXml(nfo.ConvertXbmcNfo(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter, AntStorage.Text, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
              break;
            //case 12: // Mediabrowser 3 server import
            //  destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
            //  if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
            //  {
            //    mydivx.ReadXml(destFile);
            //    break;
            //  }
            //  var mb3 = new Mediabrowser();
            //  mydivx.ReadXml(mb3.ConvertMediabrowser(MesFilmsCat.Text, MesFilmsImg.Text, destinationTagline, destinationTags, destinationCertification, destinationWriter));
            //  break;
          }

        }
      }
      catch (Exception ex)
      {
        int pos = reader.LinePosition;
        reader.MoveToNextAttribute();
        MessageBox.Show("Invalid Character for Movie Number " + reader.Value + " at position " + pos + ", number of records read : " + mydivx.Movie.Count + ". You have to correct the Movie's information with your movie catalog software !. Exception Message : " + ex.Message, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        return mydivx;
      }
      return mydivx;
    }


    private bool WriteXml(AntMovieCatalog dataset, string Catalog)
    {
      if (dataset == null || string.IsNullOrEmpty(Catalog)) return false;
      if (!IsAMCcatalogType(CatalogType.SelectedIndex)) return false; // only write AMC type catalogs

      try
      {
        using (var fs = new FileStream(Catalog, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
        {
          LogMyFilms.Debug("Commit()- opening '" + Catalog + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
          fs.SetLength(0);
          using (var myXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
          {
            LogMyFilms.Debug("Commit()- writing '" + Catalog + "' as MyXmlTextWriter in FileStream");
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
            myXmlTextWriter.WriteStartDocument();
            dataset.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema);
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
          }
          fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
          LogMyFilms.Debug("Commit()- closing '" + Catalog + "' FileStream and releasing file lock");
        }
      }
      catch (Exception saveexeption)
      {
        LogMyFilms.Debug("Commit()- exception while trying to save data in '" + Catalog + "' - exception: " + saveexeption.Message + ", stacktrace: " + saveexeption.StackTrace);
        return false;
      }
      return true;
    }

    private void General_Selected(object sender, TabControlCancelEventArgs e)
    {
      if (General.SelectedTab.Text == "Logos")
      {
        if (LogoView.Items.Count == 0)
        {
          if (MesFilmsCat.Text.Length == 0)
          {
            MessageBox.Show("You must first define a valid database configuration !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            MesFilmsCat.Focus();
            e.Cancel = true;
            return;
          }
          mydivx.Clear();
          mydivx = ReadXml();
          //Read_XML_Logos(Config_Name.Text);
          Read_XML_Logos();
        }
      }
    }

    private void Read_Default_XML_AMCconfig()
    {
      string AMCDefaultconfigFile = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + ".xml";
      if (!System.IO.File.Exists(AMCDefaultconfigFile))
      {
        MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        return;
      }

      //DataSet ds = new DataSet();
      AMCdsSettings.Clear();

      FileStream fs = new FileStream(AMCDefaultconfigFile, FileMode.Open, FileAccess.Read);
      StreamReader sr = new StreamReader(fs);
      AMCdsSettings.ReadXml(sr, XmlReadMode.InferSchema);
      AMCdsSettings.CaseSensitive = false;
      sr.Close();
      fs.Close();

      if (AMCdsSettings.Tables.Count == 1)
        AMCdsSettings.Tables[0].PrimaryKey = new DataColumn[] { AMCdsSettings.Tables[0].Columns["Option"] };

      // load dataset for GUI display
      AMCConfigView.Items.Clear();
      int i = 0;
      foreach (DataRow dr in AMCdsSettings.Tables[0].Rows)
      {
        AMCConfigView.Items.Add(dr[1].ToString());
        AMCConfigView.Items[i].SubItems.Add(dr[0].ToString());
        i = i + 1;
      }
      // set MF GUI to values from AMC file, if values are present
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Movie_Scan_Path")))
        AMCMovieScanPath.Text = this.AMCGetAttribute("Movie_Scan_Path");
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Purge_Missing_Files")))
        chkAMC_Purge_Missing_Files.Checked = AMCGetAttribute("Purge_Missing_Files").ToLower() == "true";
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Import_File_On_Internet_Lookup_Failure")))
        chkAMC_ImportOnInternetFail.Checked = AMCGetAttribute("Import_File_On_Internet_Lookup_Failure").ToLower() == "true";
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Movie_Title_Handling")))
        AmcTitleSearchHandling.Text = this.AMCGetAttribute("Movie_Title_Handling");
    }

    private void Read_XML_AMCconfig(string currentconfig)
    {
      if (currentconfig.Length == 0) // Do not process, if no valid config is selected !
        return;

      string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings";
      string AMCconfigFile = (txtAMCUpd_cnf.Text.Length > 0) ? txtAMCUpd_cnf.Text : wfiledefault + "_" + currentconfig + ".xml";

      if (!Directory.Exists(Path.GetDirectoryName(AMCconfigFile)))
      {
        try
        { Directory.CreateDirectory(Path.GetDirectoryName(AMCconfigFile)); }
        catch { }
      }

      if (!System.IO.File.Exists(AMCconfigFile)) // if (!System.IO.File.Exists(wfiledefault + "_" + currentconfig + ".xml"))
      {
        try
        {
          if (System.IO.File.Exists(wfiledefault + ".xml"))
          {
            System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsAMCSettings"), AMCconfigFile, true);
          }
          else
          {
            MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
          }

        }
        catch
        {
          MessageBox.Show("The AMCUpdater config file could not be found! \n(" + AMCconfigFile + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
      }

      //DataSet ds = new DataSet();
      AMCdsSettings.Clear();

      var fs = new FileStream(AMCconfigFile, FileMode.Open, FileAccess.Read);
      var sr = new StreamReader(fs);
      AMCdsSettings.ReadXml(sr, XmlReadMode.InferSchema);
      AMCdsSettings.CaseSensitive = false;
      sr.Close();
      fs.Close();

      if (AMCdsSettings.Tables.Count == 1)
        AMCdsSettings.Tables[0].PrimaryKey = new DataColumn[] { AMCdsSettings.Tables[0].Columns["Option"] };

      // load dataset for GUI display
      AMCConfigView.Items.Clear();
      int i = 0;
      foreach (DataRow dr in AMCdsSettings.Tables[0].Rows)
      {
        AMCConfigView.Items.Add(dr[1].ToString());
        AMCConfigView.Items[i].SubItems.Add(dr[0].ToString());
        i = i + 1;
      }
      // set MF GUI to values from AMC file, if values are present
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Movie_Scan_Path")))
        AMCMovieScanPath.Text = this.AMCGetAttribute("Movie_Scan_Path");
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Purge_Missing_Files")))
        chkAMC_Purge_Missing_Files.Checked = AMCGetAttribute("Purge_Missing_Files").ToLower() == "true";
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Import_File_On_Internet_Lookup_Failure")))
        chkAMC_ImportOnInternetFail.Checked = AMCGetAttribute("Import_File_On_Internet_Lookup_Failure").ToLower() == "true";
      if (!string.IsNullOrEmpty(this.AMCGetAttribute("Movie_Title_Handling")))
        AmcTitleSearchHandling.Text = this.AMCGetAttribute("Movie_Title_Handling");

      //  if (i > 0)
      //  {
      //    lblAMCupdaterConfigPreview.Visible = true;
      //    btnCreateAMCDefaultConfig.Enabled = true;
      //    btnCreateAMCDesktopIcon.Enabled = true;


      //    if (AMCMovieScanPath.Text.Length != 0 || !initialconfig)
      //    {
      //      AMCConfigView.Visible = true;
      //      AMCMovieScanPath.Visible = false;
      //      lblAMCMovieScanPath.Visible = false;
      //      btnAMCMovieScanPathAdd.Visible = false;
      //      chkAMC_Purge_Missing_Files.Visible = false;
      //      txtAMCUpd_cnf.Text = AMCconfigFile;
      //      txtAMCUpd_cnf.Enabled = false;
      //    }
      //    else
      //    {
      //      AMCConfigView.Visible = false;
      //      AMCMovieScanPath.Visible = true;
      //      lblAMCMovieScanPath.Visible = true;
      //      btnAMCMovieScanPathAdd.Visible = true;
      //      chkAMC_Purge_Missing_Files.Visible = true;
      //    }
      //  }
      //  else
      //  {
      //    lblAMCupdaterConfigPreview.Visible = false;
      //    btnCreateAMCDefaultConfig.Enabled = true;
      //    btnCreateAMCDesktopIcon.Enabled = false;
      //    btnLaunchAMCupdater.Enabled = false;
      //    AMCConfigView.Visible = false;
      //  }
    }

    private void Save_XML_AMCconfig(string currentconfig) //Save AMC configuration to file (before launching AMCupdater with it)
    {
      string amCconfigFile = (txtAMCUpd_cnf.Text.Length > 0) ? txtAMCUpd_cnf.Text : Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + currentconfig + ".xml";
      if (System.IO.File.Exists(amCconfigFile))
        System.IO.File.Delete(amCconfigFile);
      try
      {
        AMCdsSettings.WriteXml(amCconfigFile);
        LogMyFilms.Debug("AMCupdater Settings saved to file: '" + amCconfigFile + "'");
      }
      catch { }
    }

    private void AMCSetAttribute(string OptionName, string OptionValue)
    {
      DataRow row = null;
      if (AMCdsSettings.Tables.Count > 0)
        row = AMCdsSettings.Tables[0].Rows.Find(OptionName);
      if (row != null)
        row["Value"] = OptionValue;
      else
        //AMCdsSettings.Tables[0].Rows.Add();
        AMCdsSettings.Tables[0].Rows.Add(OptionValue, OptionName);
    }

    private string AMCGetAttribute(string OptionName)
    {
      DataRow row = null;
      if (AMCdsSettings.Tables.Count > 0)
        row = AMCdsSettings.Tables[0].Rows.Find(OptionName);
      return row != null ? row["Value"].ToString() : string.Empty;
    }

    //private void Read_XML_Logos(string currentconfig)
    private void Read_XML_Logos()
    {
      //LogoView.Clear();
      string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
      //if (!System.IO.File.Exists(wfile + "_" + currentconfig + ".xml") && currentconfig.Length > 0)
      //{
      //    try
      //    {
      //        System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsLogos"), wfile + "_" + currentconfig + ".xml", true);
      //        //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;
      //    }
      //    catch
      //    {
      //        wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
      //    }
      //}
      //else
      //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;

      wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
      //txtLogosPath.Text = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID0000", "LogosPath", Config.GetDirectoryInfo(Config.Dir.Thumbs).ToString() + "\\MyFilms_Logos");
      txtLogosPath.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogosPath", "");
      comboBoxLogoSpacing.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "Spacing", "1");
      txtCountry.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "Country", "");
      selected_Logo_Item = -1;
      comboBoxLogoPresets.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogoPresets", "Use Logos of currently selected skin");
      LogosPresetSelect();
      Read_XML_Logos_Details(wfile);
    }

    private void Read_XML_Logos_Details(string wfile)
    {
      LogoView.Items.Clear();
      //LogoView.SelectedItems[0].Remove();
      selected_Logo_Item = -1;
      //string logoConfigPathSkin;
      string skinLogoPath;
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
      {
        skinLogoPath = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "DefaultWide") + @"\Media\Logos";
        // Get current path to logos in skindirectory
        //logoConfigPathSkin = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin"); // Get current path to active skin directory
      }
      ActiveLogoPath = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogosPath", "");
      //Recreate the path to make it OS independant...
      if (ActiveLogoPath.Length < 1) // Fall back to default skin logos !
      {
        ActiveLogoPath = skinLogoPath;
      }
      else
      {
        if (ActiveLogoPath.ToLower().Contains(@"Team Mediaportal\Mediaportal".ToLower()))
        {
          int pos = ActiveLogoPath.ToLower().LastIndexOf(@"Team Mediaportal\Mediaportal".ToLower());
          ActiveLogoPath = ActiveLogoPath.Substring(pos + @"Team Mediaportal\Mediaportal".Length);
          ActiveLogoPath = Config.GetDirectoryInfo(Config.Dir.Config) + ActiveLogoPath;
        }
      }
      if (ActiveLogoPath.LastIndexOf("\\") != ActiveLogoPath.Length - 1) ActiveLogoPath = ActiveLogoPath + "\\";
      LogMyFilms.Debug("Logo path for reading logos        : '" + ActiveLogoPath + "'");

      int i = 0;
      do
      {
        string wline = XmlConfig.ReadXmlConfig(wfile, "ID2001", "ID2001_" + i, null);
        if (wline == null) break;
        string[] wtab = wline.Split(new Char[] { ';' });
        Charge_LogosView(ref wtab, i, "ID2001", ActiveLogoPath);
        i++;
      }
      while (true);
      i = 0;
      do
      {
        string wline = XmlConfig.ReadXmlConfig(wfile, "ID2002", "ID2002_" + i, null);
        if (wline == null) break;
        string[] wtab = wline.Split(new Char[] { ';' });
        Charge_LogosView(ref wtab, i, "ID2002", ActiveLogoPath);
        i++;
      }
      while (true);
      i = 0;
      do
      {
        string wline = XmlConfig.ReadXmlConfig(wfile, "ID2003", "ID2003_" + i, null);
        if (wline == null) break;
        string[] wtab = wline.Split(new Char[] { ';' });
        Charge_LogosView(ref wtab, i, "ID2003", ActiveLogoPath);
        i++;
      }
      while (true);
      i = 0;
    }

    private void Charge_LogosView(ref string[] wtab, int i, string typelogo, string logopath)
    {
      LogoView.Items.Add(typelogo);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[0]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[1]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[2]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[3]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[4]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[5]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[6]);
      LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetFileName(wtab[7]));
      if (System.IO.File.Exists(wtab[7]))
        LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(wtab[7]));
      else
        if (System.IO.File.Exists(logopath + wtab[7])) // Check if Logofile exists in default media directory of current skin and add it, if found
          LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(logopath + wtab[7]));
        else
          if (!wtab[7].Contains("\\")) // Check, if logo file is present in subdirectories of logo directory of current skin - only if not already full path defined !
          {
            string[] filePathsLogoSearch = Directory.GetFiles(logopath, wtab[7], SearchOption.AllDirectories);
            LogoView.Items[this.LogoView.Items.Count - 1].SubItems.Add(filePathsLogoSearch.Length > 0 ? Path.GetDirectoryName(filePathsLogoSearch[0]) : "");
          }
          else
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(wtab[7]));
    }
    private void chkLogos_CheckedChanged(object sender, EventArgs e)
    {
      if (chkLogos.Checked)
      {
        LogoView.Enabled = true;
        SLogo_Type.Enabled = true;
        SField1.Enabled = true;
        SOp1.Enabled = true;
        SValue1.Enabled = true;
        SField2.Enabled = true;
        SOp2.Enabled = true;
        SValue2.Enabled = true;
        SAnd_Or.Enabled = true;
        SPicture.Enabled = true;
        btnAdd.Enabled = true;
        btnDel.Enabled = true;
        btnUp.Enabled = true;
        btnDown.Enabled = true;
        btnUpdate.Enabled = true;
        txtLogosPath.Enabled = true;
        btnLogosPath.Enabled = true;
        comboBoxLogoSpacing.Enabled = true;
        btnLogoClearCache.Enabled = true;
        txtCountry.Enabled = true;
        comboBoxLogoPresets.Enabled = true;
        if (LogoView.Items.Count == 0)
          Read_XML_Logos(); // Old: //Read_XML_Logos(Config_Name.Text);
      }
      else
      {
        LogoView.Enabled = false;
        SLogo_Type.Enabled = false;
        SField1.Enabled = false;
        SOp1.Enabled = false;
        SValue1.Enabled = false;
        SField2.Enabled = false;
        SOp2.Enabled = false;
        SValue2.Enabled = false;
        SAnd_Or.Enabled = false;
        SPicture.Enabled = false;
        btnAdd.Enabled = false;
        btnDel.Enabled = false;
        btnUp.Enabled = false;
        btnDown.Enabled = false;
        btnUpdate.Enabled = false;
        txtLogosPath.Enabled = false;
        btnLogosPath.Enabled = false;
        comboBoxLogoSpacing.Enabled = false;
        btnLogoClearCache.Enabled = false;
        txtCountry.Enabled = false;
        comboBoxLogoPresets.Enabled = false;
      }
    }

    private void chkSuppress_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSuppressManual.Checked) chkSuppressManual.Checked = false;

      if (chkSuppressAutomatic.Checked)
      {
        cbSuppressAutomaticAction.Enabled = true;
        gpspfield.Enabled = true;
        chkSuppressPlayStopUpdateField.Enabled = true;
      }
      else
      {
        cbSuppressAutomaticAction.Enabled = false;
        chkSuppressPlayStopUpdateField.Checked = false;
        gpspfield.Enabled = false;
        chkSuppressPlayStopUpdateField.Enabled = false;
      }

    }

    private void chkSuppressManual_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSuppressAutomatic.Checked) chkSuppressAutomatic.Checked = false;
    }

    private void cbSuppress_SelectedIndexChanged(object sender, EventArgs e)
    {
      if ((cbSuppressAutomaticAction.SelectedIndex == 2) || (cbSuppressAutomaticAction.SelectedIndex == 3))
        gpspfield.Enabled = true;
      else
        gpspfield.Enabled = false;
    }

    private void cbfdupdate_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbSuppressPlayStopFieldToUpdate.SelectedItem.ToString() == "Checked" && !(load))
      {
        General.SelectTab(4);
        cbSuppressPlayStopFieldToUpdate.Focus();
        MessageBox.Show("Be carefull, if you use the field 'Checked' for deleted movies, you cann't get any difference between deleted and launching movies !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void chksupplaystop_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSuppressPlayStopUpdateField.Checked && !(load))
      {
        General.SelectTab(4);
        chkSuppressPlayStopUpdateField.Focus();
        MessageBox.Show("Be carefull, that deletion action'll be done each time ended watching movie !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void MesFilmsCat_Leave(object sender, EventArgs e)
    {
      if (MesFilmsCat.Text.Length > 0)
        Control_Database(MesFilmsCat.Text);
    }

    private void btnLogosPath_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtLogosPath.Text))
        openFileDialog1.FileName = txtLogosPath.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString();
      }

      if (txtLogosPath.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = txtLogosPath.Text.Substring(0, txtLogosPath.Text.LastIndexOf("\\") + 1);

      folderBrowserDialog1.Description = "Select Search Path to Logos";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        txtLogosPath.Text = folderBrowserDialog1.SelectedPath;
        LogosPresetSelect();
        //Read_XML_Logos(Config_Name.Text);
        string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
        wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
        Save_XML_Logos();
        Read_XML_Logos_Details(wfile);
        LogoClearCache(false);
        SFilePicture.Text = String.Empty;
        if (SPicture.BackgroundImage != null)
        {
          SPicture.BackgroundImage.Dispose();
          SPicture.BackgroundImage = null;
        }
        selected_Logo_Item = -1;
      }
    }

    private void SOp1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (SOp1.Text == "filled" || SOp1.Text == "not filled")
      {
        SValue1.ResetText();
        SValue1.Enabled = false;
      }
      else
        SValue1.Enabled = true;
    }

    private void SOp2_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (SOp2.Text == "filled" || SOp2.Text == "not filled")
      {
        SValue2.ResetText();
        SValue2.Enabled = false;
      }
      else
        SValue2.Enabled = true;
    }

    private void MesFilmsCat_TextChanged(object sender, EventArgs e)
    {
      if (CatalogType.SelectedIndex == 4 || CatalogType.SelectedIndex == 5) // EAX Movie Catalog 2.5.0 or 3.0.9
        if (MesFilmsImg.Text.Length == 0)
          MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures";
    }

    private void AntViewText_Change()
    {
      View_Dflt_Item.Items.Clear();
      View_Dflt_Item.Items.Add("(none)");
      View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(1079819)); // Views Menu - will be used when "none"
      //View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(342)); //Films
      //View_Dflt_Item.Items.Add("Year");
      //View_Dflt_Item.Items.Add("Category");
      //View_Dflt_Item.Items.Add("Country");
      //View_Dflt_Item.Items.Add("RecentlyAdded");

      foreach (MFview.ViewRow viewRow in Enumerable.Where(this.MyCustomViews.View, viewRow => !string.IsNullOrEmpty(viewRow.Label) && !string.IsNullOrEmpty(viewRow.DBfield)))
      {
        this.View_Dflt_Item.Items.Add(viewRow.Label);
      }

      //if (this.AntStorage.Text.Length != 0 && !(AntStorage.Text == "(none)")) View_Dflt_Item.Items.Add("Storage");

      if (!View_Dflt_Item.Items.Contains(View_Dflt_Item.Text))
      {
        View_Dflt_Item.Text = "(none)";
        View_Dflt_Text.Text = string.Empty;
      }
    }

    private void AntSort_Change()
    {
      Sort.Items.Clear();
      SortInHierarchies.Items.Clear();
      if (AntSTitle.Text.Length > 0 && AntSTitle.Text != "(none)")
      {
        Sort.Items.Add(AntSTitle.Text);
        SortInHierarchies.Items.Add(AntSTitle.Text);
      }
      else
      {
        Sort.Items.Add(AntTitle1);
        SortInHierarchies.Items.Add(AntTitle1);
      }
      //Sort.Items.Add("Title");
      //SortInHierarchies.Items.Add("Title");
      Sort.Items.Add("Year");
      Sort.Items.Add("Date");
      Sort.Items.Add("Rating");
      Sort.Items.Add("Number");
      SortInHierarchies.Items.Add("Year");
      SortInHierarchies.Items.Add("Date");
      SortInHierarchies.Items.Add("Rating");

      if (!Sort.Items.Contains(Sort.Text)) Sort.Text = "(none)";
      if (!SortInHierarchies.Items.Contains(Sort.Text)) SortInHierarchies.Text = "(none)";
    }

    private void View_Dflt_Item_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (View_Dflt_Item.Text == "(none)" || View_Dflt_Item.Text == GUILocalizeStrings.Get(1079819)) // Views Menu
      {
        View_Dflt_Text.Visible = false;
        //lbl_View_Dflt_Text.Visible = false;
        View_Dflt_Text.Clear();
        return;
      }
      else
      {
        View_Dflt_Text.Visible = true;
        //lbl_View_Dflt_Text.Visible = true;
      }

      foreach (MFview.ViewRow viewRow in Enumerable.Where(this.MyCustomViews.View, viewRow => this.View_Dflt_Item.Text == viewRow.Label))
      {
        this.View_Dflt_Text.Text = (viewRow.Value.Length > 0) ? viewRow.Value : "";
        this.View_Dflt_Text.Enabled = false;
        return;
      }
      View_Dflt_Text.Enabled = true;
    }

    public Task CreateTask(string name)
    {
      Task t;
      try
      {
        t = st.CreateTask(name);
      }
      catch (ArgumentException)
      {
        return null;
      }
      string amCupdaterExecutable = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe";
      t.ApplicationName = amCupdaterExecutable;
      t.Parameters = txtAMCUpd_cnf.Text;
      t.Comment = "Updating the database for the MP plugin MyFilms";
      t.Creator = "MP-Plugin MyFilms";
      t.WorkingDirectory = amCupdaterExecutable.Substring(0, amCupdaterExecutable.LastIndexOf("\\"));
      t.SetAccountInformation(Environment.UserName, (string)null);
      t.Flags = TaskFlags.RunOnlyIfLoggedOn;
      t.IdleWaitDeadlineMinutes = 20;
      t.IdleWaitMinutes = 10;
      t.MaxRunTime = new TimeSpan(1, 0, 0);
      t.Priority = System.Diagnostics.ProcessPriorityClass.High;
      t.Triggers.Add(new RunOnceTrigger(DateTime.Now + TimeSpan.FromDays(1.0) + TimeSpan.FromMinutes(3.0)));
      t.Triggers.Add(new DailyTrigger(8, 30, 1));
      t.Triggers.Add(new WeeklyTrigger(6, 0, DaysOfTheWeek.Sunday));
      t.Triggers.Add(new MonthlyDOWTrigger(8, 0, DaysOfTheWeek.Monday | DaysOfTheWeek.Thursday, WhichWeek.FirstWeek | WhichWeek.ThirdWeek));
      int[] days = { 1, 8, 15, 22, 29 };
      t.Triggers.Add(new MonthlyTrigger(9, 0, days, MonthsOfTheYear.July));
      t.Triggers.Add(new OnIdleTrigger());
      t.Triggers.Add(new OnLogonTrigger());
      t.Triggers.Add(new OnSystemStartTrigger());
      return t;
    }

    private void btnParameters_Click(object sender, EventArgs e)
    {
      st = new ScheduledTasks();
      Task t;
      string name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
      name = name.Substring(0, name.Length - 4);
      try
      {
        t = st.OpenTask("MyFilms_AMCUpdater_" + name);
      }
      catch (ArgumentException)
      {
        t = CreateTask("MyFilms_AMCUpdater_" + name);
      }
      if (t != null)
      {
        bool OK = t.DisplayPropertySheet();
        if (OK)
        {
          t.Save();
          MessageBox.Show("Scheduled Task saved !");
        }
      }
    }

    private void scheduleAMCUpdater_CheckedChanged(object sender, EventArgs e)
    {
      if (load)
        return;
      st = new ScheduledTasks();
      Task t = null;
      string name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
      name = name.Substring(0, name.Length - 4);
      if (scheduleAMCUpdater.Checked)
      {
        btnParameters.Enabled = true;
        try
        {
          t = st.OpenTask("MyFilms_AMCUpdater_" + name);
        }
        catch (ArgumentException)
        {
        }
        if (t == null)
        {
          t = CreateTask("MyFilms_AMCUpdater_" + name);
        }
        if (t != null)
        {
          bool OK = t.DisplayPropertySheet();
          if (OK)
          {
            t.Save();
            MessageBox.Show("Scheduled Task saved !");
          }
        }
      }
      else
      {
        btnParameters.Enabled = false;
        try
        {
          t = st.OpenTask("MyFilms_AMCUpdater_" + name);
          bool OK = st.DeleteTask("MyFilms_AMCUpdater_" + name);
          if (OK)
            MessageBox.Show("Scheduled Task deleted !");
        }
        catch (ArgumentException)
        {
        }
      }
    }

    private void btnViews_Click(object sender, EventArgs e)
    {
      folderBrowserDialog1.SelectedPath = !String.IsNullOrEmpty(MesFilmsViews.Text) ? MesFilmsViews.Text : String.Empty;
      folderBrowserDialog1.Description = "Select Group Views Picture Path";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        MesFilmsViews.Text = folderBrowserDialog1.SelectedPath;
      }
    }

    private void btnResetThumbs_Click(object sender, EventArgs e)
    {
      DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated Thumbs?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        foreach (string wfile in System.IO.Directory.GetFiles(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views", "*.*", SearchOption.AllDirectories))
        {
          try
          {
            if (wfile != pictureBoxDefaultCover.ImageLocation && wfile.Substring(wfile.LastIndexOf("\\")).ToLower() != "default.jpg")
              System.IO.File.Delete(wfile);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Setup: Error deleting file '" + wfile + "' - Exception: " + ex);
          }
        }
      }
    }

    private void btnTrailer_Click(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(PathStorageTrailer.Text))
      {
        folderBrowserDialog1.SelectedPath = PathStorageTrailer.Text.Contains(";") ? PathStorageTrailer.Text.Substring(PathStorageTrailer.Text.LastIndexOf(";") + 1).Trim() : PathStorageTrailer.Text;
        if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
      }
      else
        folderBrowserDialog1.SelectedPath = String.Empty;

      folderBrowserDialog1.Description = "Select Extended Trailer Searchpath";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != this.folderBrowserDialog1.SelectedPath.Length - 1)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

        if (PathStorageTrailer.Text.Length == 0)
          PathStorageTrailer.Text = folderBrowserDialog1.SelectedPath;
        else
          PathStorageTrailer.Text = PathStorageTrailer.Text + ";" + folderBrowserDialog1.SelectedPath;
      }
    }

    private void check_WOL_enable_CheckedChanged(object sender, EventArgs e)
    {
      if (check_WOL_enable.Checked)
      {
        check_WOL_Userdialog.Enabled = true;
        NAS_Name_1.Enabled = true;
        NAS_Name_2.Enabled = true;
        NAS_Name_3.Enabled = true;
        NAS_MAC_1.Enabled = true;
        NAS_MAC_2.Enabled = true;
        NAS_MAC_3.Enabled = true;
      }
      else
      {
        check_WOL_Userdialog.Enabled = false;
        NAS_Name_1.Enabled = false;
        NAS_Name_2.Enabled = false;
        NAS_Name_3.Enabled = false;
        NAS_MAC_1.Enabled = false;
        NAS_MAC_2.Enabled = false;
        NAS_MAC_3.Enabled = false;
      }

    }

    private void ButImgArtist_Click(object sender, EventArgs e)
    {
      folderBrowserDialog1.SelectedPath = !String.IsNullOrEmpty(MesFilmsImgArtist.Text) ? MesFilmsImgArtist.Text : String.Empty;
      folderBrowserDialog1.Description = "Select Person Images Path";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        MesFilmsImgArtist.Text = folderBrowserDialog1.SelectedPath;
      }

    }

    private void btnResetThumbsArtist_Click(object sender, EventArgs e)
    {
      DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated artist thumbs?", "Information", MessageBoxButtons.YesNo,
      MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        foreach (string wfile in System.IO.Directory.GetFiles(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons"))
        {
          try
          {
            if (wfile != pictureBoxDefaultPersonImage.ImageLocation)
              System.IO.File.Delete(wfile);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Setup: Error deleting file '" + wfile + "' - Exception: " + ex);
          }
        }
      }
    }

    private void buttonGetMACadresses_Click(object sender, EventArgs e)
    {
      var wakeOnLanManager = new WakeOnLanManager();
      String macAddress;
      byte[] hwAddress;
      IPAddress ipAddress = null;

      if (!IPAddress.TryParse(NAS_Name_1.Text, out ipAddress))
      {
        try
        {
          IPAddress[] ips;
          ips = Dns.GetHostAddresses(NAS_Name_1.Text);
          foreach (IPAddress ip in ips)
          {
            LogMyFilms.Debug("    {0}", ip);
          }
          //Use first valid IP address
          ipAddress = ips[0];
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
        }
      }

      //Check for valid IP address
      if (ipAddress != null)
      {
        //Update the MAC address if possible
        hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

        if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
        {
          LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                    , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

          //Store MAC address
          macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

          LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
          NAS_MAC_1.Text = macAddress;

        }
      }


      ipAddress = null;
      if (!IPAddress.TryParse(NAS_Name_2.Text, out ipAddress))
      {
        //Get IP address of the TV server
        try
        {
          IPAddress[] ips;
          ips = Dns.GetHostAddresses(NAS_Name_2.Text);
          foreach (IPAddress ip in ips)
          {
            LogMyFilms.Debug("    {0}", ip);
          }
          //Use first valid IP address
          ipAddress = ips[0];
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
        }
      }

      //Check for valid IP address
      if (ipAddress != null)
      {
        //Update the MAC address if possible
        hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

        if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
        {
          LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                    , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

          //Store MAC address
          macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

          LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
          NAS_MAC_2.Text = macAddress;

        }
      }

      ipAddress = null;
      if (!IPAddress.TryParse(NAS_Name_3.Text, out ipAddress))
      {
        //Get IP address of the TV server
        try
        {
          IPAddress[] ips;
          ips = Dns.GetHostAddresses(NAS_Name_3.Text);
          foreach (IPAddress ip in ips)
          {
            LogMyFilms.Debug("    {0}", ip);
          }
          //Use first valid IP address
          ipAddress = ips[0];
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
        }
      }

      //Check for valid IP address
      if (ipAddress != null)
      {
        //Update the MAC address if possible
        hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

        if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
        {
          LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                    , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

          //Store MAC address
          macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

          LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
          NAS_MAC_3.Text = macAddress;

        }
      }
    }

    private void buttonSendMagicPacket1_Click(object sender, EventArgs e)
    {
      var wakeOnLanManager = new WakeOnLanManager();
      const int intTimeOut = 30; //Timeout für WOL
      String macAddress;
      byte[] hwAddress;

      macAddress = NAS_MAC_1.Text;
      if (macAddress.Length > 1)
      {
        try
        {
          hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

          //Finally, start up the TV server
          LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

          if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_1.Text, intTimeOut))
            LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
          else
            LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
        }
      }
      else
      {
        MessageBox.Show("No MAC address available for '" + NAS_Name_1 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void buttonSendMagicPacket2_Click(object sender, EventArgs e)
    {
      var wakeOnLanManager = new WakeOnLanManager();
      const int intTimeOut = 30; //Timeout für WOL
      String macAddress;
      byte[] hwAddress;

      macAddress = NAS_MAC_2.Text;
      if (macAddress.Length > 1)
      {
        try
        {
          hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

          //Finally, start up the TV server
          LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

          if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_2.Text, intTimeOut))
            LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
          else
            LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
        }
      }
      else
      {
        MessageBox.Show("No MAC address available for '" + NAS_Name_2 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void buttonSendMagicPacket3_Click(object sender, EventArgs e)
    {
      var wakeOnLanManager = new WakeOnLanManager();
      const int intTimeOut = 30; //Timeout für WOL
      String macAddress;
      byte[] hwAddress;

      macAddress = NAS_MAC_3.Text;
      if (macAddress.Length > 1)
      {
        try
        {
          hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

          //Finally, start up the TV server
          LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

          if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_3.Text, intTimeOut))
            LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
          else
            LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
        }
      }
      else
      {
        MessageBox.Show("No MAC address available for '" + NAS_Name_3 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void NAS_1_Name_TextChanged(object sender, EventArgs e)
    {
      if (NAS_Name_1.Text.Length == 0)
        NAS_MAC_1.Text = "";
    }

    private void NAS_Name_2_TextChanged(object sender, EventArgs e)
    {
      if (NAS_Name_2.Text.Length == 0) NAS_MAC_2.Text = "";
    }

    private void NAS_Name_3_TextChanged(object sender, EventArgs e)
    {
      if (NAS_Name_3.Text.Length == 0) NAS_MAC_3.Text = "";
    }

    private void btnLaunchAMCglobal_Click(object sender, EventArgs e)
    {
      if (IsAMCcatalogType(CatalogType.SelectedIndex)) // can be "10" = "MyFilms DB", if standalone with extended features/DB-fields is supported...
        launchAMCmanager();
      else
        MessageBox.Show("Cannot launch AMC Updater !  \n AMC Updater is not supported for external catalogs other than AMC  !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void launchAMCmanager()
    {
      string configParam = "";
      if (!string.IsNullOrEmpty(txtAMCUpd_cnf.Text))
      {
        if (!System.IO.File.Exists(txtAMCUpd_cnf.Text))
        {
          MessageBox.Show("Cannot launch AMC Updater !  \n The config file you have set cannot be found ! Please correct your settings.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }
        else
          configParam = txtAMCUpd_cnf.Text;
      }
      else
      {
        if (!System.IO.File.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml"))
        {
          MessageBox.Show("Cannot launch AMC Updater !  \n You first have to create a config for AMCupdater (create Default Config on AMC Updater Tab) !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }
        else
          configParam = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml";
      }

      using (var p = new Process())
      {
        var psi = new ProcessStartInfo();
        psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCupdater.exe";
        psi.UseShellExecute = false; // try to avoid UAC popup ...
        psi.WindowStyle = ProcessWindowStyle.Normal;
        psi.Arguments = "\"" + configParam + "\"" + " " + "LogDirectory" + " " + "GUI";

        //psi.Arguments = " \"" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\" \"GUI\"";
        psi.ErrorDialog = true;
        if (OSInfo.OSInfo.VistaOrLater())
        {
          psi.Verb = "runas";
        }

        p.StartInfo = psi;
        LogMyFilms.Debug("MyFilmsSetup: Launch AMCupdater from PluginSetup with argument: {0}", string.Empty);
        try
        {
          p.Start();
          //p.WaitForExit();
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex.ToString());
        }
        LogMyFilms.Debug("MyFilmsSetup: Launch AMCupdater from PluginSetup done");
      }

    }

    private void btnCreateAMCDesktopIcon_Click(object sender, EventArgs e)
    {
      if (Config_Name.Text.Length > 0)
      {
        CreateAMCDesktopIcon();
      }
      else
      {
        MessageBox.Show("You need an active Configuration Name first !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private void CreateAMCDesktopIcon()
    {
      string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
      string linkName = "AMC-Updater '" + Config_Name.Text + "'";
      //string commandLine = "\"" + Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCupdater.exe""" + " \"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log) + "\" \"GUI\"";
      //string argument = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log) + "\" \"GUI";

      string configParam = "";
      if (!string.IsNullOrEmpty(txtAMCUpd_cnf.Text))
      {
        if (!System.IO.File.Exists(txtAMCUpd_cnf.Text))
        {
          MessageBox.Show("Config file not found !  \n The config file you have set cannot be found ! Please correct your settings.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }
        else
          configParam = txtAMCUpd_cnf.Text;
      }
      else
      {
        if (!System.IO.File.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml"))
        {
          MessageBox.Show("Cannot launch AMC Updater !  \n You first have to create a config for AMCupdater (create Default Config on AMC Updater Tab) !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }
        else
          configParam = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml";
      }


      string shortcutFile = deskDir + @"\AMC-Updater (" + Config_Name.Text + ")" + ".lnk";
      string soureFile = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCupdater.exe";
      string description = "AMC: '" + Config_Name.Text + "'";

      string arguments = "\"" + configParam + "\" " + "LogDirectory" + " " + "GUI";

      //string arguments = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\"" + " " + "\"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\"" + " " + "\"" + "GUI\"";
      string hotKey = String.Empty;
      string workingDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts);

      if (System.IO.File.Exists(deskDir + "\\" + linkName + ".url"))
        try { System.IO.File.Delete(deskDir + "\\" + linkName + ".url"); }
        catch { }

      try
      {
        CreateShortcut(soureFile, shortcutFile, description, arguments, hotKey, workingDirectory);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }

      LogMyFilms.Debug("Setup - Successfully created Desktop Icon for '" + linkName + "'");
      if (!WizardActive)
        MessageBox.Show("Successfully created Desktop Icon for " + linkName + " \n Linked Config File: " + configParam, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Create Windows Shorcut
    /// </summary>
    /// <param name="SourceFile">A file you want to make shortcut to</param>
    /// <param name="ShortcutFile">Path and shorcut file name including file extension (.lnk)</param>
    /// <param name="Description">Shortcut description</param>
    /// <param name="Arguments">Command line arguments</param>
    /// <param name="HotKey">Shortcut hot key as a string, for example "Ctrl+F"</param>
    /// <param name="WorkingDirectory">"Start in" shorcut parameter</param>
    public void CreateShortcut(string SourceFile, string ShortcutFile, string Description, string Arguments, string HotKey, string WorkingDirectory)
    {
      // Check necessary parameters first:
      if (String.IsNullOrEmpty(SourceFile))
        throw new ArgumentNullException("SourceFile");
      if (String.IsNullOrEmpty(ShortcutFile))
        throw new ArgumentNullException("ShortcutFile");

      // Create WshShellClass instance:
      var wshShell = new WshShellClass();

      // Create shortcut object:
      IWshRuntimeLibrary.IWshShortcut shorcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(ShortcutFile);

      // Assign shortcut properties:
      shorcut.TargetPath = SourceFile;
      shorcut.Description = Description;
      if (!String.IsNullOrEmpty(Arguments))
        shorcut.Arguments = Arguments;
      if (!String.IsNullOrEmpty(HotKey))
        shorcut.Hotkey = HotKey;
      if (!String.IsNullOrEmpty(WorkingDirectory))
        shorcut.WorkingDirectory = WorkingDirectory;

      // Save the shortcut:
      shorcut.Save();
    }

    private void btnAMCMovieScanPathAdd_Click(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(AMCMovieScanPath.Text))
      {
        folderBrowserDialog1.SelectedPath = AMCMovieScanPath.Text.Contains(";") ? AMCMovieScanPath.Text.Substring(AMCMovieScanPath.Text.LastIndexOf(";") + 1).Trim() : AMCMovieScanPath.Text;
        if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
      }
      else
        folderBrowserDialog1.SelectedPath = String.Empty;

      folderBrowserDialog1.Description = "Scan Path(es)for Movies Search";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != folderBrowserDialog1.SelectedPath.Length - 1)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

        if (AMCMovieScanPath.Text.Length == 0)
          AMCMovieScanPath.Text = folderBrowserDialog1.SelectedPath;
        else
          AMCMovieScanPath.Text = AMCMovieScanPath.Text + ";" + folderBrowserDialog1.SelectedPath;
      }
    }

    private void btnCreateAMCDefaultConfig_Click(object sender, EventArgs e)
    {

      if (AMCMovieScanPath.Text.Length == 0)
      {
        MessageBox.Show("You first have to define the Search path for your movies to create or sync a config !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AMCMovieScanPath.Focus();
        return;
      }
      if (AmcTitleSearchHandling.Text.Length == 0)
      {
        MessageBox.Show("You first have to define the Title and Search Handling for AMCupdater to create or sync a config !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AmcTitleSearchHandling.Focus();
        return;
      }
      string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings";
      if (System.IO.File.Exists(wfiledefault + ".xml"))
      {
        if (System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
        {
          DialogResult dialogResult = MessageBox.Show("Are you sure you want to (re)create your AMC Updater settings based on current MyFilms config? You might loose AMC Updater customizations you made!", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (dialogResult != DialogResult.Yes)
          {
            return;
          }
          else
            System.IO.File.Copy(wfiledefault + "_" + Config_Name.Text + ".xml", wfiledefault + "_" + Config_Name.Text + ".xml.sav", true);
        }
        System.IO.File.Delete(wfiledefault + "_" + Config_Name.Text + ".xml");
        Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
        CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
        Save_XML_AMCconfig(Config_Name.Text); // save new config
        Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults

        if (AMCMovieScanPath.Text.Length != 0)
        {
          MessageBox.Show("Successfully created an AMCupdater default config with your settings ! \nIf you change your MyFilms settings, recreate the AMCupdater config to get config updated!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          MessageBox.Show("You have not set up the movie scan path for AMCupdater. \n Change setting and recreate AMCupdater config !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

      }
      else
        MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
    }

    private void CreateMyFilmsDefaultsForAMCconfig(string currentconfig)
    {
      // Set Parameters from MyFilms configuration
      AMCSetAttribute("Ant_Database_Source_Field", AntStorage.Text);
      AMCSetAttribute("Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCExcludedMoviesFile.txt");

      if (txtGrabber.Text.Length != 0)
        AMCSetAttribute("Internet_Parser_Path", txtGrabber.Text);
      else
        AMCSetAttribute("Internet_Parser_Path", MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.xml");
      AMCSetAttribute("Manual_Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCExcludedMoviesFile.txt");
      AMCSetAttribute("Manual_Internet_Parser_Path", txtGrabber.Text);
      AMCSetAttribute("Manual_XML_File", MesFilmsCat.Text);
      AMCSetAttribute("Master_Title", AntTitle1.Text); // Check, if this should be replaced by ItemSearchGrabber...
      AMCSetAttribute("Movie_Fanart_Path", MesFilmsFanart.Text);
      AMCSetAttribute("Movie_PersonArtwork_Path", MesFilmsImgArtist.Text);
      AMCSetAttribute("Grabber_Override_Language", cbGrabberOverrideLanguage.Text);
      AMCSetAttribute("Grabber_Override_PersonLimit", cbGrabberOverridePersonLimit.Text);
      AMCSetAttribute("Grabber_Override_TitleLimit", cbGrabberOverrideTitleLimit.Text);
      AMCSetAttribute("Grabber_Override_GetRoles", cbGrabberOverrideGetRoles.Text);

      AMCSetAttribute("XML_File", MesFilmsCat.Text);

      // Set specific userdefined Parameters from MyFilms GUI configuration
      AMCSetAttribute("Movie_Scan_Path", AMCMovieScanPath.Text);
      AMCSetAttribute("Purge_Missing_Files", chkAMC_Purge_Missing_Files.Checked ? "true" : "false");
      AMCSetAttribute("LogDirectory", Config.GetDirectoryInfo(Config.Dir.Config) + @"\log");
      AMCSetAttribute("Movie_Title_Handling", AmcTitleSearchHandling.Text);

      if (!string.IsNullOrEmpty(cbPictureHandling.Text))
      {
        switch (cbPictureHandling.Text)
        {
          case "Relative Path":
            AMCSetAttribute("Use_Folder_Dot_Jpg", "False");
            AMCSetAttribute("Store_Image_With_Relative_Path", "True");
            AMCSetAttribute("Image_Download_Filename_Prefix", txtPicturePrefix.Text); // just prefix
            break;
          case "Full Path":
            AMCSetAttribute("Use_Folder_Dot_Jpg", "False");
            AMCSetAttribute("Store_Image_With_Relative_Path", "False");
            AMCSetAttribute("Image_Download_Filename_Prefix", MesFilmsImg.Text + "\\" + txtPicturePrefix); // Full path including "\\" excluding picturefilename 
            break;
          case "Use Folder.jpg":
            AMCSetAttribute("Use_Folder_Dot_Jpg", "True");
            AMCSetAttribute("Store_Image_With_Relative_Path", "False");
            AMCSetAttribute("Image_Download_Filename_Prefix", ""); // just prefix
            break;
          default:
            AMCSetAttribute("Use_Folder_Dot_Jpg", "False");
            AMCSetAttribute("Store_Image_With_Relative_Path", "False");
            AMCSetAttribute("Image_Download_Filename_Prefix", MesFilmsImg.Text + "\\" + txtPicturePrefix); // Full path including "\\" excluding picturefilename 
            break;
        }
      }

      AMCSetAttribute("Import_File_On_Internet_Lookup_Failure", chkAMC_ImportOnInternetFail.Checked ? "true" : "false");

      // Those Parameters already set via MyFilmsAMCSettings.xml (Defaultconfigfile):
      //Movie_Title_Handling	Default	Folder Name + Internet Lookup
      //Option	Status / Source	DefaultValue
      //Ant_Media_Label	Default	HDD
      //Ant_Media_Type	Default	File
      //Backup_XML_First	Default	True
      //Check_Field_Handling	Default	True
      //Database_Fields_To_Import	Default	#################
      //Date_Handling	Default	Current System Date
      //DVD_Drive_Letter	Default	
      //Excluded_File_Strings	Default	
      //Excluded_Folder_Strings	Default	
      //Execute_Only_For_Orphans	Default	True
      //Execute_Program	Default	False
      //Execute_Program_Path	Default	
      //File_Types_Media	Default	avi;mpg;divx;mpeg;wmv;mkv
      //File_Types_Non_Media	Default	iso;img
      //Filter_Strings	Default	\([0-9][0-9][0-9][0-9]\)
      //Folder_Name_Is_Group_Name	Default	False
      //Group_Name_Applies_To	Default	Both Titles
      //Internet_Lookup_Always_Prompt	Default	False
      //Log_Level	Default	All Events
      //Manual_Internet_Lookup_Always_Prompt	Default	True
      //Override_Path	Default	
      //Overwrite_XML_File	Default	True
      //Parse_Playlist_Files	Default	False
      //Parse_Subtitle_Files	Default	True
      //Prohibit_Internet_Lookup	Default	False
      //Read_DVD_Label	Default	False
      //RegEx_Check_For_MultiPart_Files	Default	[-|_]cd[0-9]|[-|_]disk[0-9]|[0-9]of[0-9]
      //Rescan_Moved_Files	Default	False
      //Scan_For_DVD_Folders	Default	True
      //Store_Image_With_Relative_Path	Default	True
      //Store_Short_Names_Only	Default	False
      //Use_Folder_Dot_Jpg	Default	False
      //Use_Page_Grabber	Default	False
      //Use_XBMC_nfo	Default	False
      //LogDirectory Default

    }

    private void comboBoxLogoPresets_SelectedIndexChanged(object sender, EventArgs e)
    {
      LogosPresetSelect();
      string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
      wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
      //XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogoPresets", comboBoxLogoPresets.Text); // Added Presets to Logo Config
      //XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogosPath", txtLogosPath.Text);
      Save_XML_Logos(); // Write Logos with new pathes for Images !
      Read_XML_Logos_Details(wfile);
      LogoClearCache(false);
      SFilePicture.Text = String.Empty;
      if (SPicture.BackgroundImage != null)
      {
        SPicture.BackgroundImage.Dispose();
        SPicture.BackgroundImage = null;
      }
      selected_Logo_Item = -1;
    }

    private void LogosPresetSelect()
    {
      //"Use Logos of currently selected skin",
      //"Use MyFilms Logo Pack",
      //"Define your path to logo image files"});
      switch (this.comboBoxLogoPresets.Text)
      {
        case "Define your path to logo image files":
          this.StoreFullLogoPath = false; // Changed on request of Dadeo
          this.txtLogosPath.Visible = true;
          this.txtLogosPath.Enabled = true;
          this.btnLogosPath.Visible = true;
          this.lblLogosPath.Visible = true;
          break;
        case "Use Logos of currently selected skin":
          this.txtLogosPath.Text = string.Empty;
          this.StoreFullLogoPath = false;
          this.txtLogosPath.Visible = true;
          this.txtLogosPath.Enabled = false;
          this.btnLogosPath.Visible = false;
          this.lblLogosPath.Visible = false;
          break;
        case "Use MyFilms Logo Pack":
          this.txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultLogos";
          this.StoreFullLogoPath = false;
          this.txtLogosPath.Visible = true;
          this.txtLogosPath.Enabled = false;
          this.btnLogosPath.Visible = false;
          this.lblLogosPath.Visible = false;
          break;
        case "Use Blue3Wide logos":
          this.txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\defaultwide\media\logos";
          this.StoreFullLogoPath = false;
          this.txtLogosPath.Visible = true;
          this.txtLogosPath.Enabled = false;
          this.btnLogosPath.Visible = false;
          this.lblLogosPath.Visible = false;
          break;
        default:
          this.txtLogosPath.Text = string.Empty;
          this.StoreFullLogoPath = false;
          this.txtLogosPath.Visible = false;
          this.btnLogosPath.Visible = false;
          this.lblLogosPath.Visible = false;
          break;
      }
    }

    private void btnLogoClearCache_Click(object sender, EventArgs e)
    {
      LogoClearCache(true);
    }

    private void LogoClearCache(bool showmessage)
    {
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos"))
        Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos");
      int i = 0;
      foreach (string sFile in System.IO.Directory.GetFiles(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos"))
      {
        if (sFile.ToUpper().EndsWith(".PNG"))
          System.IO.File.Delete(sFile);
        i = i + 1;
      }
      if (showmessage)
      {
        if (i == 0)
          MessageBox.Show(
            "No cached logo files to delete, your logo cache is already empty !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
          MessageBox.Show("Successfully cleared " + i.ToString() + " cached files in your logo cache directory! Be aware browsing your movies might be slower when rebuilding the logos in cache.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void btnFirstTimeSetup_Click(object sender, EventArgs e)
    {
      WizardActive = true;
      newCatalogWizard();
      WizardActive = false;
    }

    private void newCatalogWizard()
    {
      bool newCatalog = true;
      // MyFilms.SetupType setupType = MyFilms.SetupType.Local; // set local install as default

      // load central server config, if it already exists
      XmlConfig myFilmsServer = new XmlConfig();
      string myFilmsCentralConfigDir = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", "");
      bool syncFromServerOnStartup = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
      if (Config_Name.Text.Length != 0 || RunWizardAfterInstall)
      {
        if (MessageBox.Show("Do you want to create a new MyFilms Configuration ? \n\nThis wizard helps you to setup a new configuration with default settings. \nIf you select 'yes', enter a name for the configuration.\nIf you select 'no' you can relaunch the wizard later with the 'Setup Wizard' button.", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
          return;
      }

      #region ask for setup type - local, central master, central client
      MyFilmsInputBox input = new MyFilmsInputBox
        {
          // preset to central config client only, if central config is available
          SetupType = (!syncFromServerOnStartup || myFilmsCentralConfigDir.Length == 0) ? (int)MyFilms.SetupType.Local : (int)MyFilms.SetupType.CentralClient,
          Text = "MyFilms - Setup Wizard",
          UseNfoGrabber = false,
          CatalogTypeSelectedIndex = 10,
          CatalogType = "Ant Movie Catalog Xtended (V4.1)",
          Country = "USA (Full Detail Grabbing)",
          ShowOnlyName =  false,
          TestMode = true // always use feature, as it is moved to normal mode - before: TestMode = (MyFilms_PluginMode != "normal")
        };
      input.ShowDialog();
      #endregion

      if (input.DialogResult != DialogResult.OK) return; // stop, of user did not confirm new configuration

      if (input.SetupType == (int)MyFilms.SetupType.CentralClient)
      {
        #region setup for network client

        #region choose server location
        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog
          {
            Description = "Choose the network folder where you have stored your central MyFilms config before ...",
            ShowNewFolderButton = false
          };
        if (!String.IsNullOrEmpty(myFilmsCentralConfigDir))
        {
          folderBrowserDialog1.SelectedPath = myFilmsCentralConfigDir;
          if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
            folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
        }
        else
          folderBrowserDialog1.SelectedPath = String.Empty;
        if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
        {
          if (folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != folderBrowserDialog1.SelectedPath.Length - 1) folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";
          myFilmsCentralConfigDir = folderBrowserDialog1.SelectedPath;
        }
        else return;
        #endregion

        #region load server config for client
        string serverConfigFile = myFilmsCentralConfigDir + @"\MyFilms.xml";
        string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
        if (!Directory.Exists(myFilmsCentralConfigDir))
        {
          MessageBox.Show("Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        if (!System.IO.File.Exists(serverConfigFile))
        {
          MessageBox.Show("Remote MyFilms.xml not found - cannot continue !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        if (MessageBox.Show("Are you sure you want to copy remote config to local config ?\n\nIf you select 'yes', your local config file will be overwritten, MyFilms setup will reload and you loose your local configuration.\n(A backup will be autocreated)",
            "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
          return;

        if (System.IO.File.Exists(localConfigFile))
        {
          try
          {
            System.IO.File.Copy(localConfigFile, localConfigFile + "_" + System.DateTime.Now.ToString("yyyy-mm-dd hh_mm"), true);
          }
          catch (Exception ex)
          {
            //MessageBox.Show("Error: " + ex.StackTrace, "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("Cannot write to local directory - cannot continue ! \n(" + ex.Message + ")", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

        }
        try
        {
          System.IO.File.Copy(serverConfigFile, localConfigFile, true);
          
          #region save central server config locally
          myFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", myFilmsCentralConfigDir);
          myFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
          myFilmsServer.Save();
          #endregion

          MessageBox.Show("Successfully copied remote config to local directory !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
          if (MessageBox.Show("MyFilms Setup will now reload the updated local MyFilms.xml config file !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
            MesFilmsSetup_Load(null, null); // reload setup config
          // this.DialogResult = DialogResult.OK;
          // this.Close(); // close the setup app
        }
        catch (Exception)
        {
          MessageBox.Show("Cannot copy to local directory - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        // need to reload config here?
        #endregion
        return;
      }

      if (input.SetupType == (int)MyFilms.SetupType.CentralMaster)
      {
        #region ask the user for the network location for the myfilms server directory

        #region choose server share location
        MessageBox.Show("Next, you need to choose the network path for your central MyFilms files.\nUse UNC path and make sure, it can be accessed from all your clients with the same name.\nAlso make sure, you have write access to the directory.\n\nExample: \n\\\\MyServer\\Mediashare\\MyFilms\n", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog
          {
            Description = "Select the network path for the central files (MyFilms root dir) \nThis will be used for folder presets and central config file. \nMake sure, you use UNC path that is acessible from all your clients ...",
            ShowNewFolderButton = true
          };
        if (!String.IsNullOrEmpty(myFilmsCentralConfigDir))
        {
          folderBrowserDialog1.SelectedPath = myFilmsCentralConfigDir;
          if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
            folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
        }
        else
          folderBrowserDialog1.SelectedPath = String.Empty;
        if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
        {
          if (folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != folderBrowserDialog1.SelectedPath.Length - 1)
            folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";
          myFilmsCentralConfigDir = folderBrowserDialog1.SelectedPath;
        }
        #endregion

        if (!System.IO.Directory.Exists(myFilmsCentralConfigDir))
        {
          MessageBox.Show("Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        else
        {
          #region save central server config locally
          myFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", myFilmsCentralConfigDir);
          myFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
          myFilmsServer.Save();
          #endregion
        }

        #endregion
      }

      #region preset the configuration !!!

      string newConfigName = input.ConfigName;
      string newCatalogType = input.CatalogType;
      string newCountry = input.Country;
      int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;
      bool useNfoGrabber = input.UseNfoGrabber;
      if (!(newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 10)) // if not Ant Movie Catalog, do NOT use nfo grabber anyway
        useNfoGrabber = false;

      if (string.IsNullOrEmpty(newConfigName))
      {
        MessageBox.Show("New Config Name must not be empty ! No Config created !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }
      if (newConfigName == Config_Name.Text)
      {
        MessageBox.Show("Config Name must be different from existing ones ! No Config created !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      // Set Configuration Name & initialize new current config
      Config_Name.Text = newConfigName;
      Refresh_Items(true); // Reset all
      // Refresh_Tabs(true); // enable Tabs - autocontrolled by the changes handlers ...

      CatalogType.SelectedIndex = 0; // can be "10" = "AMC 4.1 extended DB"

      // Ask user to select existing or create new catalog...
      bool useExistingCatalog = true;

      if (newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 10) // Configuration.CatalogType.AntMovieCatalog3
      {
        //if (MyFilms_PluginMode != "normal" || MyFilms_PluginMode == "normal") // added to only allow new catalogs in test mode // edit: reenabled for normal mode
        //{
        //  useExistingCatalog = MessageBox.Show("Do you want to use an existing catalog? \n\nIf you select 'yes', you will be asked to select the path to your existing catalog file.\n If you select 'no' you will create a new empty catalog.", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        //}
        //else
        //  MessageBox.Show("Please select the path to your existing catalog file !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
        useExistingCatalog = MessageBox.Show("Do you want to use an existing catalog? \n\nIf you select 'yes', you will be asked to select the path to your existing catalog file.\n If you select 'no' you will create a new empty catalog.", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
      }
      else
      {
        MessageBox.Show("Please select the path to your existing catalog file. \n (You have to export your movie collection to xml format in your catalog manager first to use it in myfilms.)", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }

      string catalogDirectory = "";
      switch (input.SetupType)
      {
        case (int)MyFilms.SetupType.Local:
          catalogDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath) + @"\Catalog";
          if (!Directory.Exists(catalogDirectory))
          {
            try { Directory.CreateDirectory(catalogDirectory); }
            catch (Exception ex)
            {
              MessageBox.Show("The directory could not be created. \n\nError: " + ex.Message, "MyFilms Configuration Wizard - Abort !", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
          }
          break;
        case (int)MyFilms.SetupType.CentralMaster:
          catalogDirectory = Path.Combine(myFilmsCentralConfigDir, "Catalog");
          if (!Directory.Exists(catalogDirectory))
          {
            try { Directory.CreateDirectory(catalogDirectory); }
            catch (Exception ex)
            {
              MessageBox.Show("The directory could not be created. \n\nError: " + ex.Message, "MyFilms Configuration Wizard - Abort !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
          break;
        case (int)MyFilms.SetupType.CentralClient:
          break;
        default:
          break;
      }
      
      if (useExistingCatalog)
      {
        // Ask User for existing database file
        newCatalog = false;
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = Directory.Exists(catalogDirectory) ? catalogDirectory : "";
        if (!string.IsNullOrEmpty(MesFilmsCat.Text) && MesFilmsCat.Text.Contains("\\"))
          openFileDialog1.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);
        openFileDialog1.RestoreDirectory = true;
        openFileDialog1.DefaultExt = "xml";
        openFileDialog1.Filter = "XML Files|*.xml";
        openFileDialog1.Title = "Select Movie Catalog File (xml)";
        openFileDialog1.CheckFileExists = false;

        if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        {
          Control_Database(openFileDialog1.FileName);
          // check, if it is invalid AMC4.2 catalog - if yes, abort !!!
          if (!BaseMesFilms.IsValidDb(openFileDialog1.FileName))
          {
            Config_Name.Text = "";
            Refresh_Items(true); // Reset all
            MessageBox.Show("The selected catalog is not compatible with MyFilms (AMC4.2 format) \n\n Please use a compatible DB format!", "MyFilms Configuration Wizard - Abort !", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            return;
          }
        }
      }
      else
      {
        string catalogName = catalogDirectory + @"\" + Config_Name.Text + ".xml";
        if (!System.IO.File.Exists(catalogName))
        {
          XmlTextWriter destXml = new XmlTextWriter(catalogName, System.Text.Encoding.Default)
            {
              Formatting = Formatting.Indented
            };
          destXml.WriteStartDocument();
          destXml.WriteStartElement("AntMovieCatalog");
          destXml.WriteStartElement("Catalog");
          destXml.WriteElementString("Properties", string.Empty);
          destXml.WriteStartElement("Contents");
          destXml.Close();
        }
        MesFilmsCat.Text = catalogName;
        if (this.MesFilmsCat.Text.Length > 0 && this.MesFilmsImg.Text.Length == 0)
        {
          MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
        }
        newCatalog = true;
      }

      // Set values and Presets ...
      AntStorage.Text = "Source";
      cbPictureHandling.Text = "Relative Path"; // set option for picture path handling (grabber)

      switch (newCatalogSelectedIndex)
      {
        case 0:
          AntStorageTrailer.Text = "Borrower";
          break;
        default:
          AntStorageTrailer.Text = "SourceTrailer";
          break;
      }

      if (MessageBox.Show("Do you want to use Original Title as Master Title ? \n(If you select no, Translated Title will be used)", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        AntTitle1.Text = "OriginalTitle";
        AntTitle2.Text = "TranslatedTitle";
        ItemSearchFileName.Text = "OriginalTitle";
        ItemSearchGrabberName.Text = "OriginalTitle";
      }
      else
      {
        AntTitle1.Text = "TranslatedTitle";
        AntTitle2.Text = "OriginalTitle";
        ItemSearchFileName.Text = "TranslatedTitle";
        ItemSearchGrabberName.Text = "TranslatedTitle";
      }

      AntSTitle.Text = "FormattedTitle";
      TitleDelim.Text = "\\";

      SearchFileName.Checked = false;

      cbPictureHandling.Text = "Relative Path";

      #region preset separators
      ListSeparator1.Text = ",";
      ListSeparator2.Text = ";";
      ListSeparator3.Text = "|";
      ListSeparator4.Text = "/";
      ListSeparator5.Text = string.Empty;
      RoleSeparator1.Text = "(";
      RoleSeparator2.Text = ")";
      RoleSeparator3.Text = " as ";
      RoleSeparator4.Text = string.Empty;
      RoleSeparator5.Text = string.Empty;
      #endregion

      chkFanart.Checked = true;
      chkFanartDefaultViews.Checked = false;
      chkFanartDefaultViewsUseRandom.Checked = true;
      chkDfltFanart.Checked = true;
      chkDfltFanartImage.Checked = true;
      chkDfltFanartImageAll.Checked = true;

      Sort.Text = (AntSTitle.Text == "") ? AntTitle1.Text : AntSTitle.Text;
      SortInHierarchies.Text = (AntSTitle.Text == "") ? AntTitle1.Text : AntSTitle.Text;
      SortSens.Text = " ASC";
      SortSensInHierarchies.Text = " ASC";
      LayoutInHierarchies.Text = "List";
      LayOut.Text = "List";
      View_Dflt_Item.Text = GUILocalizeStrings.Get(1079819);


      string fanartDirectory = "";
      switch (input.SetupType)
      {
        case (int)MyFilms.SetupType.Local:
          fanartDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\Fanart";
          break;
        case (int)MyFilms.SetupType.CentralMaster:
          fanartDirectory = Path.Combine(myFilmsCentralConfigDir, "Fanart");
          break;
      }
      if (!Directory.Exists(fanartDirectory)) { try { Directory.CreateDirectory(fanartDirectory); } catch { } }
      MesFilmsFanart.Text = fanartDirectory;

      string artistImagesDirectory = "";
      switch (input.SetupType)
      {
        case (int)MyFilms.SetupType.Local:
          artistImagesDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\PersonImages";
          break;
        case (int)MyFilms.SetupType.CentralMaster:
          artistImagesDirectory = Path.Combine(myFilmsCentralConfigDir, "PersonImages");
          break;
      }
      if (!Directory.Exists(artistImagesDirectory)) { try { Directory.CreateDirectory(artistImagesDirectory); } catch { } }
      MesFilmsImgArtist.Text = artistImagesDirectory;
      pictureBoxDefaultCover.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Cover2.jpg"; //DefaultCover.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Cover.jpg";

      pictureBoxDefaultPersonImage.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Persons2.jpg";
      pictureBoxDefaultViewImage.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Views.jpg";
      pictureBoxDefaultFanart.ImageLocation = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Fanart.jpg";
      chkDfltArtist.Checked = true; // Use default person cover if missing artwork...

      string viewImagesDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ViewImages);
      switch (input.SetupType)
      {
        case (int)MyFilms.SetupType.Local:
          viewImagesDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ViewImages);
          if (!Directory.Exists(viewImagesDirectory)) { try { Directory.CreateDirectory(viewImagesDirectory); } catch { } }
          break;
        case (int)MyFilms.SetupType.CentralMaster:
          viewImagesDirectory = Path.Combine(myFilmsCentralConfigDir, "ViewImages");
          try
          {
            if (!Directory.Exists(viewImagesDirectory)) { try { Directory.CreateDirectory(viewImagesDirectory); } catch { } }
            // copy local viewimages to server
            string localViewImagesDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ViewImages);
            Helper.CopyFolder(localViewImagesDirectory, viewImagesDirectory, false); // copy files only, if missing in destination dir
          }
          catch (Exception) { }
          break;
      }

      MesFilmsViews.Text = viewImagesDirectory;
      chkViews.Checked = true; // Use Thumbs for views
      chkPersons.Checked = true; // Use Thumbs for persons views
      chkPersonsEnableDownloads.Checked = true; // Use background worker image downloads in details screen to automatically load person images
      chkDfltViews.Checked = true; // Use default cover for missing thumbs
      chkShowIndexedImgInIndViews.Checked = true; // activate indexed Images for in
      chkDfltViewsAll.Checked = true; // Use group view thumbs for all group views

      #region Logos
      chkLogos.Checked = true;
      comboBoxLogoSpacing.Text = "2";
      comboBoxLogoPresets.Text = "Use Logos of currently selected skin";
      #endregion

      //GrabberConfig
      //txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.xml";
      chkGrabber_ChooseScript.Checked = true; //Don't use default script (ask)

      CheckWatchedPlayerStopped.Checked = true; // set watched status handling

      #region set user items for main and details
      switch (newCatalogSelectedIndex) // if (newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 10) 
      {
        case 0: // AMC3
          AntItem1.Text = "Category";
          AntLabel1.Text = BaseMesFilms.TranslateColumn(AntItem1.Text.Trim());
          AntItem2.Text = "Country";
          AntLabel2.Text = BaseMesFilms.TranslateColumn(AntItem2.Text.Trim());
          AntItem3.Text = "Director";
          AntLabel3.Text = BaseMesFilms.TranslateColumn(AntItem3.Text.Trim());
          AntItem4.Text = "Languages";
          AntLabel4.Text = BaseMesFilms.TranslateColumn(AntItem4.Text.Trim());
          AntItem5.Text = "Date";
          AntLabel5.Text = BaseMesFilms.TranslateColumn(AntItem5.Text.Trim());

          AntItemDetails1.Text = "Category";
          AntLabelDetails1.Text = BaseMesFilms.TranslateColumn(AntItemDetails1.Text.Trim());
          AntItemDetails2.Text = "Country";
          AntLabelDetails2.Text = BaseMesFilms.TranslateColumn(AntItemDetails2.Text.Trim());
          AntItemDetails3.Text = "Director";
          AntLabelDetails3.Text = BaseMesFilms.TranslateColumn(AntItemDetails3.Text.Trim());
          AntItemDetails4.Text = "Producer";
          AntLabelDetails4.Text = BaseMesFilms.TranslateColumn(AntItemDetails4.Text.Trim());
          AntItemDetails5.Text = "Languages";
          AntLabelDetails5.Text = BaseMesFilms.TranslateColumn(AntItemDetails5.Text.Trim());
          AntItemDetails6.Text = "Date";
          AntLabelDetails6.Text = BaseMesFilms.TranslateColumn(AntItemDetails6.Text.Trim());
          break;
        default:
          AntItem1.Text = "Category";
          AntLabel1.Text = BaseMesFilms.TranslateColumn(AntItem1.Text.Trim());
          AntItem2.Text = "Country";
          AntLabel2.Text = BaseMesFilms.TranslateColumn(AntItem2.Text.Trim());
          AntItem3.Text = "Director";
          AntLabel3.Text = BaseMesFilms.TranslateColumn(AntItem3.Text.Trim());
          AntItem4.Text = "Certification";
          AntLabel4.Text = BaseMesFilms.TranslateColumn(AntItem4.Text.Trim());
          AntItem5.Text = "Languages";
          AntLabel5.Text = BaseMesFilms.TranslateColumn(AntItem5.Text.Trim());

          AntItemDetails1.Text = "Category";
          AntLabelDetails1.Text = BaseMesFilms.TranslateColumn(AntItemDetails1.Text.Trim());
          AntItemDetails2.Text = "Country";
          AntLabelDetails2.Text = BaseMesFilms.TranslateColumn(AntItemDetails2.Text.Trim());
          AntItemDetails3.Text = "Director";
          AntLabelDetails3.Text = BaseMesFilms.TranslateColumn(AntItemDetails3.Text.Trim());
          AntItemDetails4.Text = "Producer";
          AntLabelDetails4.Text = BaseMesFilms.TranslateColumn(AntItemDetails4.Text.Trim());
          AntItemDetails5.Text = "Writer";
          AntLabelDetails5.Text = BaseMesFilms.TranslateColumn(AntItemDetails5.Text.Trim());
          AntItemDetails6.Text = "Certification";
          AntLabelDetails6.Text = BaseMesFilms.TranslateColumn(AntItemDetails6.Text.Trim());
          break;
      }
      #endregion

      #region add default Views
      MyCustomViews.View.Clear();
      AddDefaultViews();
      #endregion

      #region Set watched field
      switch (newCatalogSelectedIndex)
      {
        case 0:
          cbWatched.Text = "Checked";
          break;
        case 10:
          cbWatched.Text = "Checked"; // cbWatched.Text = "Watched"; cahnged bck to "Checked" to better support upgrade scenarios
          break;
        default:
          cbWatched.Text = "Checked";
          break;
      }
      #endregion

      // chkSuppress.Checked = false;

      #region Now ask user for his movie directory...
      switch (newCatalogSelectedIndex)
      {
        case 0:
        case 10:
          MessageBox.Show("Now choose the folder containing your movies.", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);

          string moviepath = "";
          switch (input.SetupType)
          {
            case (int)MyFilms.SetupType.Local:
              moviepath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath) + @"\SampleMovies";
              break;
            case (int)MyFilms.SetupType.CentralMaster:
              moviepath = Path.Combine(myFilmsCentralConfigDir, "Movies");
              break;
          }
          if (!Directory.Exists(moviepath)) { try { Directory.CreateDirectory(moviepath); } catch { } }

          if (System.IO.Directory.Exists(moviepath))
            folderBrowserDialog1.SelectedPath = moviepath;

          if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
          {
            if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") !=
                this.folderBrowserDialog1.SelectedPath.Length - 1) folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

            if (PathStorage.Text.Length == 0)
              PathStorage.Text = folderBrowserDialog1.SelectedPath;
            else PathStorage.Text = PathStorage.Text + ";" + folderBrowserDialog1.SelectedPath;
          }
          //MessageBox.Show("Successfully created a new Configuration ! You may now run AMCupdater to populate or update your catalog.", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          break;
        default:
          PathStorage.Text = "";
          break;
      }
      #endregion

      #region set grabber defaults
      txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.xml";
      cbGrabberOverrideLanguage.Text = newCountry; // Create country specific settings
      cbGrabberOverrideGetRoles.Text = "true";
      cbGrabberOverridePersonLimit.Text = "10";
      cbGrabberOverrideTitleLimit.Text = "0";
      ItemSearchGrabberScriptsFilter.Text = "";
      #endregion

      switch (newCountry)
      {
        #region set country specific settings
        // to allow skinners use IMDB-Full and get all properties loaded !
        case "USA (Full Detail Grabbing)":
        case "SampleMovies":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB-Full.xml";
          cbGrabberOverrideGetRoles.Text = "true";
          cbGrabberOverridePersonLimit.Text = "10";
          cbGrabberOverrideTitleLimit.Text = "0";
          ItemSearchGrabberScriptsFilter.Text = "en, all";
          cbGrabberOverrideLanguage.Text = "USA"; // preset for sample movies for skinners
          break;

        case "Austria":
        case "Germany":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.DE-OFDB.xml";
          cbGrabberOverrideGetRoles.Text = "";
          cbGrabberOverridePersonLimit.Text = "";
          cbGrabberOverrideTitleLimit.Text = "";
          ItemSearchGrabberScriptsFilter.Text = "de, all";
          break;
        case "China":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\TMDB-ZH.xml";
          break;
        case "Canada":
        case "UK":
        case "USA":
        case "Australia":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.xml";
          cbGrabberOverrideGetRoles.Text = "true";
          cbGrabberOverridePersonLimit.Text = "10";
          cbGrabberOverrideTitleLimit.Text = "0";
          ItemSearchGrabberScriptsFilter.Text = "en, all";
          break;
        case "Argentina":
        case "Peru":
        case "Spain":
        case "Chile":
        case "Portugal":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.ES.xml";
          ItemSearchGrabberScriptsFilter.Text = "es, all";
          break;
        case "France":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.FR.xml";
          ItemSearchGrabberScriptsFilter.Text = "fr, all";
          break;
        case "Hungary":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\Port.hu-IMDB.xml";
          ItemSearchGrabberScriptsFilter.Text = "hu, all";
          break;
        case "Italy":
          txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB.IT.xml";
          ItemSearchGrabberScriptsFilter.Text = "it, all";
          break;

        #region No country specific settings:
        //Belgium
        //Brazil
        //Croatia
        //Czech Republic
        //Denmark
        //Estonia
        //Finland
        //Greece
        //Hong Kong
        //Iceland
        //India
        //Ireland
        //Israel
        //Japan
        //Malaysia
        //Mexico
        //Netherlands
        //New Zealand
        //Norway
        //Philippines
        //Poland
        //Romania
        //Russia
        //Singapore
        //Slovakia
        //Slovenia
        //South Africa
        //South Korea
        //Sweden
        //Switzerland
        //Turkey
        //Uruguay
        #endregion

        #endregion
      }

      if (useNfoGrabber) // set to nfo grabber, if selected in setup
      {
        txtGrabber.Text = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\NFO.xml";
        ItemSearchGrabberScriptsFilter.Text = "all";
      }

      #region AMCUpdater settings
      chkAMCUpd.Checked = true; // Use AMCupdater
      chkAMCUscanOnStartup.Checked = false;
      chkAMCUwatchScanFolders.Checked = false;
      numericUpDownAMCUscanStartDelay.Value = 90;

      AMCMovieScanPath.Text = PathStorage.Text;
      AmcTitleSearchHandling.Text = "Folder Name + Internet Lookup"; // set this as default
      chkAMC_Purge_Missing_Files.Checked = false;
      chkAMC_ImportOnInternetFail.Checked = true;
      txtPicturePrefix.Text = @"pictures\"; // txtPicturePrefix.Text = ""; // "Config_Name.Text + "_"; // Use configname as default prefix
      txtAMCUpd_cnf.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml";
      // Make controls visible:
      //txtAMCUpd_cnf.Enabled = true;
      //btnAMCUpd_cnf.Enabled = true;
      //scheduleAMCUpdater.Enabled = true;
      //btnParameters.Enabled = true;
      ////btnLaunchAMCupdater.Enabled = true;
      //btnAMCMovieScanPathAdd.Enabled = true;
      //chkAMC_Purge_Missing_Files.Enabled = true;
      //btnCreateAMCDefaultConfig.Enabled = true;
      //btnCreateAMCDesktopIcon.Enabled = true;
      //AMCMovieScanPath.Enabled = true;
      #endregion

      #region Create config file for AMCupdater
      string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings";
      if (System.IO.File.Exists(wfiledefault + ".xml"))
      {
        if (System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
        {
          System.IO.File.Copy(wfiledefault + "_" + Config_Name.Text + ".xml", wfiledefault + "_" + Config_Name.Text + ".xml.sav", true);
        }
        System.IO.File.Delete(wfiledefault + "_" + Config_Name.Text + ".xml");

        Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
        CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
        Save_XML_AMCconfig(Config_Name.Text); // save new config
        Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults
      }
      #endregion

      #region Create Desktop Icon for AMCupdater with config created...
      if (newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 10) // only for AMC configs
        CreateAMCDesktopIcon();
      #endregion

      // Change Config to selected catalog type
      CatalogType.SelectedIndex = newCatalogSelectedIndex;
      // Set as default config, if it's the only config - but this would always have this config set as default, even if adding more configs....
      //if (Config_Name.Items.Count == 1) 
      //  Config_Dflt.Checked = true;

      Save_Config();
      //Config_Name.Focus();
      switch (newCatalogSelectedIndex)
      {
        case 0:
          MessageBox.Show("Successfully created a new Configuration with default settings ! \n\nPlease review your settings in MyFilms and AMC Updater to match your personal needs. \n You may run AMCupdater to populate or update your catalog.", "MyFilms Configuration Wizard - Finished !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          break;
        default:
          MessageBox.Show("Successfully created a new Configuration for '" + CatalogType.Text + "' with default settings ! \n\nPlease verify the settings to artwork pathes to match your personal needs.", "MyFilms Configuration Wizard - Finished !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          General.SelectedIndex = 6;
          MesFilmsImg.Focus(); // Set focus to cover path                
          break;
      }

      #endregion

      if (input.SetupType == (int)MyFilms.SetupType.CentralMaster)
      {
        #region upload to network if "central master" setup was chosen

        #region upload server config for client
        string serverConfigFile = myFilmsCentralConfigDir + @"\MyFilms.xml";
        string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
        if (!System.IO.Directory.Exists(myFilmsCentralConfigDir))
        {
          MessageBox.Show("Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        if (!System.IO.File.Exists(localConfigFile))
        {
          MessageBox.Show("Local MyFilms.xml not found - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        if (System.IO.File.Exists(serverConfigFile))
        {
          try
          {
            string backupfile = serverConfigFile.Replace(".xml", " - " + DateTime.Now.ToString("u").Replace(":", "-") + ".xml").Replace("/", "-");
            System.IO.File.Copy(serverConfigFile, backupfile, true);
          }
          catch (Exception)
          {
            MessageBox.Show("Cannot write to Server directory - Missing access rights? - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
        }
        try
        {
          System.IO.File.Copy(localConfigFile, serverConfigFile, true);
          MessageBox.Show("Successfully copied local config to remote directory !\nYou can use the 'Central Client' setup to install your clients using this config now.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception)
        {
          MessageBox.Show("Cannot write to Server directory - Missing access rights? - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #endregion
      }

      switch (input.SetupType)
      {
        case (int)MyFilms.SetupType.Local:
        case (int)MyFilms.SetupType.CentralMaster:
          if (newCatalog && IsAMCcatalogType(CatalogType.SelectedIndex))
          {
            launchAMCmanager();
          }
          break;
        case (int)MyFilms.SetupType.CentralClient:
          break;
        default:
          break;
      }

    }

    private void butNew_Click(object sender, EventArgs e)
    {
      MyFilmsInputBox input = new MyFilmsInputBox
        {
          Text = "MyFilms - New Catalog",
          CatalogTypeSelectedIndex = 0,
          CatalogType = "Ant Movie Catalog (V3.5.1.2)"
        };
      input.ShowDialog(this);
      string newConfig_Name = input.ConfigName;
      string newCatalogType = input.CatalogType;
      int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;

      if (string.IsNullOrEmpty(newConfig_Name))
      {
        MessageBox.Show("New Config Name must not be empty !", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
      {
        if (newConfig_Name == Config_Name.Text)
        {
          MessageBox.Show("New Config Name must be different from the existing one !", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        else
        {
          Read_XML_AMCconfig(newConfig_Name);
          NewConfigButton = true;
          Refresh_Items(true); // Reset all
          Refresh_Tabs(true); // enable Tabs
          Config_Name.Text = newConfig_Name;
          //Config_Name_Load();
          MessageBox.Show("Created a new Configuration ! \n You must do proper setup to use it.", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
          CatalogType.SelectedIndex = newCatalogSelectedIndex; // set selected CatalogType
          //Config_Name.Focus();
          MesFilmsCat.Focus(); // change focus away from config to initialize ...
          textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
          NewConfigButton = true;
        }
      }
    }

    private void chkAMC_Purge_Missing_Files_CheckedChanged(object sender, EventArgs e)
    {
      if (General.SelectedIndex == 5 && chkAMC_Purge_Missing_Files.Checked && !WizardActive && AMCGetAttribute("Purge_Missing_Files") != "true") // if the user is on the tab, show a security warning
      {
        if (MessageBox.Show("Are you sure, you want to purge records from your DB \n where media files are not accessible during AMC Updater scans ?", "Control Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
          chkAMC_Purge_Missing_Files.Checked = false;
        }
      }
    }

    private void txtLogosPath_TextChanged(object sender, EventArgs e)
    {

    }

    private void CheckWatched_CheckedChanged(object sender, EventArgs e)
    {
      if (CheckWatched.Checked)
        CheckWatchedPlayerStopped.Checked = false;
    }

    private void CheckWatchedPlayerStopped_CheckedChanged(object sender, EventArgs e)
    {
      if (CheckWatchedPlayerStopped.Checked)
        CheckWatched.Checked = false;
    }

    private void chkDfltFanartImage_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDfltFanartImage.Checked)
      {
        chkDfltFanart.Checked = false;
        chkDfltFanartImageAll.Enabled = true;
      }
      else
      {
        chkDfltFanartImageAll.Enabled = false;
      }
    }

    private void chkDfltFanart_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDfltFanart.Checked)
        chkDfltFanartImage.Checked = false;
    }

    private void cbPictureHandling_SelectedIndexChanged(object sender, EventArgs e)
    {
      // options:
      // cbPictureHandling = "Full Path"
      // cbPictureHandling = "Relative Path"
      // cbPictureHandling = "Use Folder.jpg"
    }

    private void btnEditScript_Click(object sender, EventArgs e)
    {
      using (var p = new Process())
      {
        var psi = new ProcessStartInfo();
        psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\MyFilms_Grabber_Interface.exe";
        psi.UseShellExecute = true;
        psi.WindowStyle = ProcessWindowStyle.Normal;
        psi.Arguments = "\"" + txtGrabber.Text + "\"";
        psi.ErrorDialog = true;
        if (OSInfo.OSInfo.VistaOrLater())
        {
          psi.Verb = "runas";
        }

        p.StartInfo = psi;
        LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup with command param '" + psi.Arguments + "'");
        try
        {
          p.Start();
          //p.WaitForExit();
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex.ToString());
        }
        LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup done");
      }

    }

    private void buttonDeleteTmpCatalog_Click(object sender, EventArgs e)
    {
      string destFile = string.Empty;
      switch (CatalogType.SelectedIndex)
      {
        case 0: // ANT Movie Catalog
        case 10:// ANT Movie Catalog extnded V4.1
          break;
        case 1: // DVD Profiler
        case 2: // Movie Collector V7.1.4
        case 3: // MyMovies
        case 4: // EAX Movie Catalog 2.5.0
        case 5: // EAX Movie Catalog 3.0.9 (beta5)
        case 6: // PVD PersonalVideoDatabase V0.9.9.21
        case 7: // eXtreme Movie Manager
        case 8: // XBMC fulldb export (all movies in one DB)
        case 9: // MovingPicturesXML V1.2
          destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
          break;
        case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
          destFile = MesFilmsCat.Text;
          break;
      }
      if (destFile.Length > 0)
        if (System.IO.File.Exists(destFile))
        {
          DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the tmp cache file for this config? \n\n(File will be recreated on next 'save' action.)", "Warning", MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);
          if (dialogResult == DialogResult.Yes)
          {
            System.IO.File.Delete(destFile);
            LogMyFilms.Debug("MyFilmsSetup: Manually deleted tmp catalog: '" + destFile + "'");
            MessageBox.Show("Deleted tmp imported data: '" + destFile + "' \n MyFilms will reimport data on next launch or when saving the config.", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
        }
    }

    private void buttonOpenTmpFile_Click(object sender, EventArgs e)
    {
      string destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
      if (!System.IO.File.Exists(destFile))
      {
        MessageBox.Show("The file '" + destFile + "' does not exist!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      using (var p = new Process())
      {
        var psi = new ProcessStartInfo();
        psi.FileName = "notepad.exe";
        psi.UseShellExecute = true;
        psi.WindowStyle = ProcessWindowStyle.Normal;
        psi.Arguments = "\"" + MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml" + "\"";
        psi.ErrorDialog = true;
        if (OSInfo.OSInfo.VistaOrLater())
        {
          psi.Verb = "runas";
        }

        p.StartInfo = psi;
        try
        {
          p.Start();
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex.ToString());
        }
      }

    }

    private void buttonOpenTmpFileAMC_Click(object sender, EventArgs e)
    {
      string destFile = string.Empty;
      destFile = this.IsAMCcatalogType(this.CatalogType.SelectedIndex)
                   ? this.MesFilmsCat.Text
                   : this.MesFilmsCat.Text.Substring(0, this.MesFilmsCat.Text.Length - 4) + "_tmp.xml";
      if (!System.IO.File.Exists(destFile))
      {
        MessageBox.Show("The file '" + destFile + "' does not exist!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      if (string.IsNullOrEmpty(AMCexePath.Text))
      {
        MessageBox.Show("Unable to launch Ant Movie Catalog ! \nPlease set the path accordingly first!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      if (IsAMCcatalogType(CatalogType.SelectedIndex)) // show warning when launching AMC with EC tmp file !
      {
        MessageBox.Show("Info: You are launching AMC with a temporary cache catalog file !\nPlease be aware, that changes in MC will be overwritten on next export of your external catalog !", "AMC Launcher Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      //System.Diagnostics.Process.Start(AMCexePath.Text); // More detailed startup below ...
      using (var p = new Process())
      {
        var psi = new ProcessStartInfo();
        psi.FileName = AMCexePath.Text;
        psi.UseShellExecute = true;
        psi.WindowStyle = ProcessWindowStyle.Normal;
        psi.Arguments = "\"" + destFile + "\"";
        psi.ErrorDialog = true;
        if (OSInfo.OSInfo.VistaOrLater())
        {
          psi.Verb = "runas";
        }

        p.StartInfo = psi;
        try
        {
          p.Start();
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex.ToString());
        }
      }
    }

    private void buttonAMCpathSearch_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(AMCexePath.Text))
        openFileDialog1.FileName = AMCexePath.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
      }
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "exe";
      openFileDialog1.Filter = "exe Files|*.exe";
      openFileDialog1.Title = "Select AMC Movie Catalog Executable (.exe)";
      openFileDialog1.CheckFileExists = false;
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        AMCexePath.Text = openFileDialog1.FileName;
      }
    }

    private void txtGrabber_TextChanged(object sender, EventArgs e)
    {
      txtGrabberDisplay.Text = !string.IsNullOrEmpty(txtGrabber.Text) ? Path.GetFileName(txtGrabber.Text) : string.Empty;
    }

    private void txtAMCUpd_cnf_TextChanged(object sender, EventArgs e)
    {
      txtAMCUpd_cnf_Display.Text = !string.IsNullOrEmpty(txtAMCUpd_cnf.Text) ? Path.GetFileName(txtAMCUpd_cnf.Text) : string.Empty;
    }

    private void linkLabelTrakt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://trakt.tv/");
    }

    private void btnWatchedExport_Click(object sender, EventArgs e)
    {
      SaveFileDialog fd = new SaveFileDialog();
      fd.Title = "Select Backup file for watched Status (.watched)";
      fd.Filter = "Exported Watched Info (*.watched)|*.watched";
      fd.DefaultExt = ".watched";
      if (MesFilmsCat.Text.Contains("\\"))
        fd.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);
      // fd.RestoreDirectory = true;
      fd.FileName = Path.GetFileNameWithoutExtension(MesFilmsCat.Text) + ".watched";

      if (fd.ShowDialog() == DialogResult.OK)
      {
        var w = new StreamWriter(fd.FileName);

        if (MesFilmsCat.Text.Length > 0)
        {
          try { mydivx.Clear(); }
          catch { }
          mydivx = ReadXml();
          if (mydivx != null)
          {
            string expression = StrDfltSelect + AntTitle1.Text + " not like ''";
            DataRow[] movies = mydivx.Movie.Select(expression);
            if (mydivx.Movie.Count > 0)
            {
              foreach (string val in movies.Select(movie => movie["Number"] + " || " + movie[this.cbWatched.Text] + " || " + movie["OriginalTitle"] + " || " + movie["Year"]))
              {
                try
                {
                  w.WriteLine((string)val);
                }
                catch (IOException exception)
                {
                  LogMyFilms.Debug("Watched info NOT exported!  Error: " + exception);
                  MessageBox.Show("Watched info NOT exported!  Error: " + exception, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  return;
                }
              }
            }
            else
            {
              MessageBox.Show("Watched info NOT exported!  No Movies found in Catalog !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          }
          else
            MessageBox.Show("Watched info NOT exported!  Catalog cannot be read !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
          LogMyFilms.Debug("Movie Data cannot be read - Watched info NOT exported!");
          MessageBox.Show("Movie Data cannot be read - Watched info NOT exported!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        }
        w.Close();
        LogMyFilms.Debug("Watched info succesfully exported!");
        MessageBox.Show("Watched info succesfully exported !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

    }

    private void btnWatchedImport_Click(object sender, EventArgs e)
    {
      OpenFileDialog fd = new OpenFileDialog();
      fd.Title = "Select Backup file for watched Status (.watched)";
      fd.Filter = "Exported Watched Info (*.watched)|*.watched";
      fd.DefaultExt = ".watched";
      if (MesFilmsCat.Text.Contains("\\"))
        fd.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);
      // fd.RestoreDirectory = true;
      fd.FileName = System.IO.Path.GetFileNameWithoutExtension(MesFilmsCat.Text) + ".watched";

      if (fd.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fd.FileName))
      {
        string line = string.Empty;
        DataRow[] movies = null;

        // try reading catalog
        if (MesFilmsCat.Text.Length > 0)
        {
          try { mydivx.Clear(); }
          catch { }
          mydivx = ReadXml();
          if (mydivx != null)
          {
            string expression = StrDfltSelect + AntTitle1.Text + " not like ''";
            movies = mydivx.Movie.Select(expression);
            if (mydivx.Movie.Count == 0)
            {
              MessageBox.Show("Watched info NOT imported!  No Movies found in Catalog !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          }
          else
          {
            MessageBox.Show("Watched info NOT imported!  Catalog cannot be read !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
          }
        }
        else
        {
          LogMyFilms.Debug("Movie Data cannot be read - Watched info NOT imported!");
          MessageBox.Show("Movie Data cannot be read - Watched info NOT imported!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        }

        // set unwatched for all
        foreach (DataRow movie in movies)
        {
          //movie[cbWatched.Text] = "";
          movie[cbWatched.Text] = textBoxGlobalUnwatchedOnlyValue.Text;
        }
        // now set watched for all in file
        var r = new StreamReader(fd.FileName);
        while ((line = r.ReadLine()) != null)
        {
          // ToDo: Set watched per line to DB here
          string tmp = line;
          var number = tmp.Substring(0, tmp.IndexOf("||")).Trim();
          tmp = tmp.Substring(tmp.IndexOf("||") + 2).Trim();
          var watched = tmp.Substring(0, tmp.IndexOf("||")).Trim();
          tmp = tmp.Substring(tmp.IndexOf("||") + 2).Trim();
          var otitle = tmp.Substring(0, tmp.IndexOf("||")).Trim();
          tmp = tmp.Substring(tmp.IndexOf("||") + 2).Trim();
          var year = tmp.Trim();

          foreach (DataRow movie in movies.Where(movie => movie["Number"].ToString() == number && movie["OriginalTitle"].ToString() == otitle && movie["Year"].ToString() == year))
          {
            movie[this.cbWatched.Text] = watched;
          }
        }
        r.Close();

        // now save dataset to catalog file
        bool writesuccess = WriteXml(mydivx, MesFilmsCat.Text);
        if (writesuccess)
        {
          LogMyFilms.Debug("Watched info succesfully imported!");
          MessageBox.Show("Watched info succesfully imported !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          LogMyFilms.Debug("Watched info NOT succesfully imported!");
          MessageBox.Show("Watched info NOT succesfully imported !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void linkLabelTraktWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("https://github.com/Technicolour/Trakt-for-Mediaportal/wiki/");
    }

    private void linkLabelUsingTraktInMyFilmsWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films/3_Using_My_Films/Trakt/");
    }


    private void chkGlobalAvailableOnly_CheckedChanged(object sender, EventArgs e)
    {
      if (chkGlobalAvailableOnly.Checked && !chkScanMediaOnStart.Checked)
      {
        chkGlobalAvailableOnly.ForeColor = System.Drawing.Color.IndianRed;
      }
      else
      {
        chkGlobalAvailableOnly.ResetForeColor();
      }
      //if (chkGlobalAvailableOnly.Checked && !chkScanMediaOnStart.Checked)
      //{
      //  // MessageBox.Show("If you don't have 'scan media on start' enabled, \nyou won't get the filtered view until you made a manual scan via options !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      //  // chkGlobalAvailableOnly.Checked = false;
      //}
    }

    private void btnHyperLinkParamGen_Click(object sender, EventArgs e)
    {
      var input = new HyperLinkParamGenerator();
      input.Text = "MyFilms - Hyperlink Start Parameter Creator";
      input.ShowDialog(this);
    }


    #region Tab Enable/Disable Helper
    private void HideTabPage(TabPage tp)
    {
      if (General.TabPages.Contains(tp))
        General.TabPages.Remove(tp);
    }

    private void ShowTabPage(TabPage tp)
    {
      ShowTabPage(tp, General.TabPages.Count);
    }

    private void ShowTabPage(TabPage tp, int index)
    {
      if (General.TabPages.Contains(tp)) return;
      InsertTabPage(tp, index);
    }

    private void InsertTabPage(TabPage tabpage, int index)
    {
      if (index < 0 || index > General.TabCount)
        throw new ArgumentException("Index out of Range.");
      General.TabPages.Add(tabpage);
      if (index < General.TabCount - 1)
        do
        {
          SwapTabPages(tabpage, (General.TabPages[General.TabPages.IndexOf(tabpage) - 1]));
        }
        while (General.TabPages.IndexOf(tabpage) != index);
      General.SelectedTab = tabpage;
    }

    private void SwapTabPages(TabPage tp1, TabPage tp2)
    {
      if (General.TabPages.Contains(tp1) == false || General.TabPages.Contains(tp2) == false)
        throw new ArgumentException("TabPages must be in the TabControls TabPageCollection.");

      int Index1 = General.TabPages.IndexOf(tp1);
      int Index2 = General.TabPages.IndexOf(tp2);
      General.TabPages[Index1] = tp2;
      General.TabPages[Index2] = tp1;

      //Uncomment the following section to overcome bugs in the Compact Framework
      //tabControl1.SelectedIndex = tabControl1.SelectedIndex; 
      //string tp1Text, tp2Text;
      //tp1Text = tp1.Text;
      //tp2Text = tp2.Text;
      //tp1.Text=tp2Text;
      //tp2.Text=tp1Text;
    }
    #endregion

    //private void button_GrabberScriptUpdate_Click(object sender, EventArgs e)
    //{
    //  Grabber.Updater.UpdateScripts(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts));
    //}

    private void Config_Name_TextChanged(object sender, EventArgs e)
    {
      Config_Name.Text = StringExtensions.XmlCharacterWhitelist(Config_Name.Text).Replace(@"'", "");
    }

    private void AntViewItem_SelectedIndexChanged(object sender, EventArgs e)
    {
      AntViewText_Change();
      // if (MyCustomViews.View.Rows.Count == 0) return;
      // if (viewBindingSource.List.Count == 0) return;
      // viewBindingSource.ResetBindings(false);
      if (AntViewItem.Text.Length == 0)
      {
        AntViewItem.Text = "(none)";

        MyCustomViews.View[viewBindingSource.Position].ViewEnabled = false;
      }

      if (AntViewItem.Text != "(none)" && string.IsNullOrEmpty(MyCustomViews.View[viewBindingSource.Position].Label))
      {
        MessageBox.Show("The View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        dgViewsList.Focus();
      }
      if (AntViewItem.Text == "OriginalTitle" || AntViewItem.Text == "TranslatedTitle" || AntViewItem.Text == "FormattedTitle")
      {
        if (AntViewValue.Text == "")
          AntViewValue.Text = "*";
      }
      else
      {
        if (AntViewValue.Text == "*")
          AntViewValue.Text = "";
      }
    }

    private void buttonUpdateGrabberScripts_Click(object sender, EventArgs e)
    {
      var updateThread = new Thread(delegate(object obj)
      {
        try
        {
          string versionfile = Config.GetFolder(Config.Dir.Config) + @"\MyFilmsScriptVersions.xml";
          if (System.IO.File.Exists(versionfile)) System.IO.File.Delete(versionfile);
          var url = new Uri("http://my-films.googlecode.com/svn/trunk/Installer" + @"/updateScriptVersions.xml");

          if (DownloadFile(url, versionfile)) // DownloadFileAsync(url, versionfile);
          {
            List<string> updatedscripts = UpdateScriptFiles(versionfile);
            textBoxUpdateGrabberScripts.Text = "";
            if (updatedscripts.Count > 0)
            {
              string updatelist = updatedscripts.Aggregate("Grabber Scripts updated: \n", (current, updatedscript) => current + (updatedscript + "\n"));
              MessageBox.Show(updatelist);
            }
            else MessageBox.Show("No Updates available !");
            //try { System.IO.File.Delete(versionfile); } // remove temp download file
            //catch { }
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show("Error updating script files: '" + ex.Message + "'\n\n" + ex.StackTrace);
        }
      })
      {
        IsBackground = true,
        Name = "Check for Updates"
      };
      updateThread.Start();
    }

    private bool DownloadFile(Uri url, string file)
    {
      var webClient = new WebClient();
      try
      {
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        if (!System.IO.File.Exists(file))
        {
          LogMyFilms.Debug("Downloading file from: {0}", url);
          webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
          webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
          webClient.DownloadFile(url, file);
        }
        return true;
      }
      catch (Exception)
      {
        LogMyFilms.Error("Download failed from '{0}' to '{1}'", url, file);
        try { if (System.IO.File.Exists(file)) System.IO.File.Delete(file); }
        catch { }
        return false;
      }
    }

    private void DownloadFileAsync(Uri url, string file)
    {
      var webClient = new WebClient();
      webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
      webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
      webClient.DownloadFileAsync(url, file); // load threaded, but file isn't there for proceeding ...
    }

    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      progressBarUpdateGrabberScripts.Value = e.ProgressPercentage;
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
      //FileInfo versionfile = new FileInfo(file);
      //if (versionfile.Length == 0)
      //{
      //  System.IO.File.Delete(file);
      //}
      textBoxUpdateGrabberScripts.Text = "";
      progressBarUpdateGrabberScripts.Value = 100;
      // MessageBox.Show("Download(s) completed!");
    }

    public List<string> UpdateScriptFiles(string versionfile)
    {
      var updatedscripts = new List<string>();
      var dirsInf = new DirectoryInfo(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts));
      var sfiles = dirsInf.GetFileSystemInfos();
      var existingGrabberScriptFiles = new List<string>();

      foreach (FileSystemInfo sfi in sfiles.Where(sfi => sfi.Extension.ToLower() == ".xml"))
      {
        try // do try-catch, if there is old or invalid greabber script files in directory !
        {
          var script = new GrabberScript(sfi.FullName);
          script.Load(sfi.FullName);
          existingGrabberScriptFiles.Add(Path.GetFileName(sfi.FullName).ToLower());
        }
        catch (Exception ex)
        {
          MessageBox.Show("Skip check for invalid Grabber Script '" + sfi.Name + "' - Error: '" + ex.Message + "'");
        }
      }

      try
      {
        var doc = new XmlDocument();
        doc.Load(versionfile);
        string urlbase = doc.DocumentElement.SelectSingleNode("/grabberscripts/urlbase").InnerText;
        XmlNodeList grabberupdates = doc.DocumentElement.SelectNodes("/grabberscripts/scripts");
        if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\temp\")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\temp\");
        progressBarUpdateGrabberScripts.Maximum = grabberupdates.Count;
        progressBarUpdateGrabberScripts.Value = 0;
        foreach (XmlNode grabberupdate in grabberupdates)
        {
          if (progressBarUpdateGrabberScripts.Value < progressBarUpdateGrabberScripts.Maximum) progressBarUpdateGrabberScripts.Value += 1;
          string name = grabberupdate.SelectSingleNodeFast("name").InnerText;
          Version version = new Version(grabberupdate.SelectSingleNodeFast("version").InnerText);
          string url = urlbase + name;
          string localfilename = Path.Combine(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts), name);
          string backupfile = Path.Combine(Path.GetDirectoryName(localfilename) + @"\backup\", (Path.GetFileNameWithoutExtension(localfilename) + " - " + DateTime.Now.ToString("u") + ".xml").Replace(":", "-").Replace("/", "-"));
          string tempFile = Path.Combine(Path.GetDirectoryName(localfilename) + @"\temp\", Path.GetFileName(localfilename)); // string tempFile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";
          if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);
          Thread.Sleep(25);

          textBoxUpdateGrabberScripts.Text = "Checking update for '" + name + "'";

          // check, if script missing - then load it
          if (!existingGrabberScriptFiles.Contains(name.ToLower()))
          {
            if (DownloadFile(new Uri(url), tempFile)) // DownloadFile(new Uri(url), localfilename); // "http://my-films.googlecode.com/svn/trunk/Installer/Config/scripts/MyFilms/";
            {
              System.IO.File.Copy(tempFile, localfilename, true);
              System.IO.File.Delete(tempFile);
              Thread.Sleep(25);
            }
            GrabberScript newScript = new GrabberScript(localfilename);
            newScript.Load(localfilename);
            updatedscripts.Add(newScript.DBName + " (Language(s) = '" + newScript.Language + "', Version = '" + newScript.Version + "')");
          }
          else
          {
            Version curVersion = null;
            foreach (GrabberScript currentScript in from existingGrabberScript in existingGrabberScriptFiles where existingGrabberScript.ToLower() == Path.GetFileName(localfilename).ToLower() select new GrabberScript(localfilename))
            {
              currentScript.Load(localfilename);
              curVersion = new Version(currentScript.Version);
            }

            if (curVersion != null && curVersion.CompareTo(version) < 0) // compare the versions
            {
              textBoxUpdateGrabberScripts.Text = "Updating '" + name + "'";
              if (DownloadFile(new Uri(url), tempFile)) // DownloadFile(new Uri(url), localfilename); // "http://my-films.googlecode.com/svn/trunk/Installer/Config/scripts/MyFilms/";
              {
                // backup existing script
                if (!Directory.Exists(Path.GetDirectoryName(backupfile))) Directory.CreateDirectory(Path.GetDirectoryName(backupfile));
                System.IO.File.Copy(localfilename, backupfile, true);
                System.IO.File.Delete(localfilename);
                Thread.Sleep(25);
                System.IO.File.Copy(tempFile, localfilename, true);
                System.IO.File.Delete(tempFile);
                Thread.Sleep(25);
                GrabberScript newScript = new GrabberScript(localfilename);
                newScript.Load(localfilename);
                updatedscripts.Add(newScript.DBName + " (Language(s) = '" + newScript.Language + "', Version = '" + newScript.Version + "')");
              }
            }
          }
          // delete temp file
          if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);
          Thread.Sleep(25);
        }
        // remove temp directory
        try
        {
          if (System.IO.Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\temp\")) System.IO.Directory.Delete(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\temp\", true);
        }
        catch (Exception) { }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error updating script files: '" + ex.Message + "'\n" + ex.StackTrace);
        return updatedscripts;
      }

      #region old method
      //Version newVersion = null;
      //string name = "";
      //string url = "";
      //XmlTextReader reader;

      //try
      //{
      //  string xmlURL = "http://domain/app_version.xml";
      //  reader = new XmlTextReader(xmlURL);
      //  reader.MoveToContent();
      //  string elementName = "";
      //  if ((reader.NodeType == XmlNodeType.Element) &&
      //      (reader.Name == "GrabberScript"))
      //  {
      //    while (reader.Read())
      //    {
      //      // when we find an element node,
      //      // we remember its name
      //      if (reader.NodeType == XmlNodeType.Element)
      //        elementName = reader.Name;
      //      else
      //      {
      //        // for text nodes...
      //        if ((reader.NodeType == XmlNodeType.Text) &&
      //            (reader.HasValue))
      //        {
      //          // we check what the name of the node was
      //          switch (elementName)
      //          {
      //            case "version":
      //              // thats why we keep the version info
      //              // in xxx.xxx.xxx.xxx format
      //              // the Version class does the
      //              // parsing for us
      //              newVersion = new Version(reader.Value);
      //              break;
      //            case "name":
      //              name = reader.Value;
      //              break;
      //            case "url":
      //              url = reader.Value;
      //              break;
      //          }
      //        }
      //      }
      //    }
      //  }
      //}
      //catch (Exception)
      //{
      //}
      //finally
      //{
      //  //if (null != reader) reader.Close();
      //}
      #endregion
      return updatedscripts;
    }

    private void button_DeleteBackupScripts_Click(object sender, EventArgs e)
    {
      string backupdir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\backup\";
      try
      {
        if (Directory.Exists(backupdir))
        {
          DirectoryInfo dirsInf = new DirectoryInfo(backupdir);
          FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
          foreach (FileSystemInfo sfi in sfiles.Where(sfi => sfi.Extension.ToLower() == ".xml"))
          {
            System.IO.File.Delete(sfi.FullName);
          }
          Directory.Delete(backupdir, true);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error deleting script backups: " + ex.Message);
      }
      MessageBox.Show("Successfully deleted all script backups !");
    }

    private void toolStripButtonAddDefaults_Click(object sender, EventArgs e)
    {
      this.AddDefaultViews();
    }

    private void AntViewFilterEditButton_Click(object sender, EventArgs e)
    {
      var filterEditor = new FilterEditor();
      filterEditor.Text = "MyFilms - View Filter Editor ('" + AntViewFilter.Text + "')";
      filterEditor.MasterTitle = AntTitle1.Text;
      filterEditor.ExtendedFields = (this.CatalogType.SelectedIndex != 0);

      // preset the values
      string[] filterSettings = AntViewFilterSaveString.Text.Split('|');
      if (filterSettings.Length > 8)
      {
        filterEditor.FilterItem1 = filterSettings[0];
        filterEditor.FilterSign1 = filterSettings[1];
        filterEditor.FilterText1 = filterSettings[2];
        filterEditor.FilterItem2 = filterSettings[3];
        filterEditor.FilterSign2 = filterSettings[4];
        filterEditor.FilterText2 = filterSettings[5];
        filterEditor.FilterFreeText = filterSettings[6];
        filterEditor.FilterComb = filterSettings[7];
        filterEditor.ConfigString = filterSettings[8];
      }
      
      filterEditor.ShowDialog(this);
      if (filterEditor.DialogResult == System.Windows.Forms.DialogResult.OK)
      {
        // AntViewFilter.Focus();
        AntViewFilter.Text = filterEditor.ConfigString;
        string s = "";
        s += filterEditor.FilterItem1;
        s += "|" + filterEditor.FilterSign1;
        s += "|" + filterEditor.FilterText1;
        s += "|" + filterEditor.FilterItem2;
        s += "|" + filterEditor.FilterSign2;
        s += "|" + filterEditor.FilterText2;
        s += "|" + filterEditor.FilterFreeText;
        s += "|" + filterEditor.FilterComb;
        s += "|" + filterEditor.ConfigString;
        AntViewFilterSaveString.Text = s;
        viewBindingSource.EndEdit();
      }
      else
        MessageBox.Show("Filter Editor cancelled !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void AntViewFilter_TextChanged(object sender, EventArgs e)
    {
      // AntViewFilterEditButton.Focus();
    }

    private void toolStripButtonAdd_Click(object sender, EventArgs e)
    {
      MFview.ViewRow newRow = this.MyCustomViews.View.NewViewRow();
      newRow.Label = "New View";
      this.MyCustomViews.View.Rows.Add(newRow);
      // bindingNavigatorSaveItem.PerformClick();
      //viewBindingSourceAddNewItem.PerformClick();
      viewBindingSource.ResetBindings(false);
      viewBindingSource.Position = viewBindingSource.Count - 1;
    }

    private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
    {
      //this.viewBindingSource.MoveUp();
      int position = viewBindingSource.Position;
      if (position == 0) return;  // already at top

      viewBindingSource.RaiseListChangedEvents = false;

      //MFview current = (MFview)viewBindingSource.Current;
      //viewBindingSource.Remove(current);

      DataRow selectedRow = this.MyCustomViews.View.Rows[position];
      DataRow newRow = this.MyCustomViews.View.NewRow();
      newRow.ItemArray = selectedRow.ItemArray; // copy data
      this.MyCustomViews.View.Rows.Remove(selectedRow);
      this.MyCustomViews.View.Rows.InsertAt(newRow, position - 1);

      //viewBindingSource.Insert(position - 1, current);
      viewBindingSource.Position = position - 1;

      viewBindingSource.RaiseListChangedEvents = true;
      viewBindingSource.ResetBindings(false);
    }

    private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
    {
      //this.viewBindingSource.MoveDown();
      int position = viewBindingSource.Position;
      if (position == viewBindingSource.Count - 1) return;  // already at bottom

      viewBindingSource.RaiseListChangedEvents = false;

      //MFview current = (MFview)viewBindingSource.Current;
      //viewBindingSource.Remove(current);


      DataRow selectedRow = this.MyCustomViews.View.Rows[position];
      DataRow newRow = this.MyCustomViews.View.NewRow();
      newRow.ItemArray = selectedRow.ItemArray; // copy data
      this.MyCustomViews.View.Rows.Remove(selectedRow);
      this.MyCustomViews.View.Rows.InsertAt(newRow, position + 1);

      //viewBindingSource.Insert(position + 1, current);
      viewBindingSource.Position = position + 1;

      viewBindingSource.RaiseListChangedEvents = true;
      viewBindingSource.ResetBindings(false);
    }

    private void dgViewsList_Leave(object sender, EventArgs e)
    {
      AntViewText_Change();
    }

    private void AntViewValue_TextChanged(object sender, EventArgs e)
    {
      groupBoxSortAndLayoutForView.Visible = string.IsNullOrEmpty(AntViewValue.Text);
    }

    private void buttonWikiHelp_Click(object sender, EventArgs e)
    {
      Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films");
    }

    private void btnCustomConfigFilter_Click(object sender, EventArgs e)
    {
      var filterEditor = new FilterEditor();
      filterEditor.Text = "MyFilms - View Filter Editor ('" + AntViewFilter.Text + "')";
      filterEditor.MasterTitle = AntTitle1.Text;
      filterEditor.ExtendedFields = (this.CatalogType.SelectedIndex != 0);
      filterEditor.FilterItem1 = AntFilterItem1.Text;
      filterEditor.FilterItem2 = AntFilterItem2.Text;
      filterEditor.FilterSign1 = AntFilterSign1.Text;
      filterEditor.FilterSign2 = AntFilterSign2.Text;
      filterEditor.FilterText1 = AntFilterText1.Text;
      filterEditor.FilterText2 = AntFilterText2.Text;
      filterEditor.FilterFreeText = AntFilterFreeText.Text;
      filterEditor.FilterComb = AntFilterComb.Text;
      filterEditor.ShowDialog(this);
      if (filterEditor.DialogResult == DialogResult.OK)
      {
        StrDfltSelect = filterEditor.StrDfltSelect;
        textBoxStrDfltSelect.Text = filterEditor.StrDfltSelect;
        AntFilterItem1.Text = filterEditor.FilterItem1;
        AntFilterItem2.Text = filterEditor.FilterItem2;
        AntFilterSign1.Text = filterEditor.FilterSign1;
        AntFilterSign2.Text = filterEditor.FilterSign2;
        AntFilterText1.Text = filterEditor.FilterText1;
        AntFilterText2.Text = filterEditor.FilterText2;
        AntFilterFreeText.Text = filterEditor.FilterFreeText;
        AntFilterComb.Text = filterEditor.FilterComb;
      }
      else MessageBox.Show("Filter Editor cancelled !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void AntViewsImage_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(AntViewsImage.ImageLocation))
        openFileDialog1.FileName = AntViewsImage.ImageLocation;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages);
      }

      if (null != AntViewsImage.ImageLocation && AntViewsImage.ImageLocation.Contains("\\"))
        openFileDialog1.InitialDirectory = AntViewsImage.ImageLocation.Substring(0, AntViewsImage.ImageLocation.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "jpg";
      openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
      openFileDialog1.Title = "Select Views Image";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        AntViewsImage.ImageLocation = openFileDialog1.FileName;
        viewBindingSource.EndEdit();
        //buttonResetImage.Focus(); // change focus to update binding source ! Otherwise content gets lost, if save immediately
      }
    }

    private void buttonResetImage_Click(object sender, EventArgs e)
    {
      AntViewsImage.ImageLocation = "";
      viewBindingSource.EndEdit();
    }

    private void pictureBoxDefaultCover_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(pictureBoxDefaultCover.ImageLocation))
        openFileDialog1.FileName = pictureBoxDefaultCover.ImageLocation;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages);
      }

      if (pictureBoxDefaultCover.ImageLocation.Contains("\\"))
        openFileDialog1.InitialDirectory = pictureBoxDefaultCover.ImageLocation.Substring(0, pictureBoxDefaultCover.ImageLocation.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "jpg";
      openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
      openFileDialog1.Title = "Select Default Movie Cover";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        pictureBoxDefaultCover.ImageLocation = openFileDialog1.FileName;
    }

    private void buttonDefaultCoverReset_Click_1(object sender, EventArgs e)
    {
      pictureBoxDefaultCover.ImageLocation = "";
    }

    private void buttonDefaultViewImageReset_Click(object sender, EventArgs e)
    {
      pictureBoxDefaultViewImage.ImageLocation = "";
    }

    private void pictureBoxDefaultViewImage_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(pictureBoxDefaultViewImage.ImageLocation))
        openFileDialog1.FileName = pictureBoxDefaultViewImage.ImageLocation;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages);
      }

      if (pictureBoxDefaultViewImage.ImageLocation.Contains("\\"))
        openFileDialog1.InitialDirectory = pictureBoxDefaultViewImage.ImageLocation.Substring(0, pictureBoxDefaultViewImage.ImageLocation.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "jpg";
      openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
      openFileDialog1.Title = "Select Default Group Views Cover Image";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        pictureBoxDefaultViewImage.ImageLocation = openFileDialog1.FileName;
    }

    private void pictureBoxDefaultPersonImage_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(pictureBoxDefaultPersonImage.ImageLocation))
        openFileDialog1.FileName = pictureBoxDefaultPersonImage.ImageLocation;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages);
      }

      if (pictureBoxDefaultPersonImage.ImageLocation.Contains("\\"))
        openFileDialog1.InitialDirectory = pictureBoxDefaultPersonImage.ImageLocation.Substring(0, pictureBoxDefaultPersonImage.ImageLocation.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "jpg";
      openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
      openFileDialog1.Title = "Select Default Person Cover Image";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        pictureBoxDefaultPersonImage.ImageLocation = openFileDialog1.FileName;
    }

    private void buttonDefaultPersonImageReset_Click(object sender, EventArgs e)
    {
      pictureBoxDefaultPersonImage.ImageLocation = "";
    }

    private void pictureBoxDefaultFanart_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(pictureBoxDefaultFanart.ImageLocation))
        openFileDialog1.FileName = pictureBoxDefaultFanart.ImageLocation;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages);
      }

      if (pictureBoxDefaultFanart.ImageLocation.Contains("\\"))
        openFileDialog1.InitialDirectory = pictureBoxDefaultFanart.ImageLocation.Substring(0, pictureBoxDefaultFanart.ImageLocation.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "jpg";
      openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
      openFileDialog1.Title = "Select Default Fanart Image";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        pictureBoxDefaultFanart.ImageLocation = openFileDialog1.FileName;
    }

    private void btnFirstTimeSetupSyncClient_Click(object sender, EventArgs e)
    {
      var syncSetup = new CentralConfigSetup();
      if (syncSetup.ShowDialog(this) == DialogResult.OK)
        MesFilmsSetup_Load(null, null); // reload setup config
      else
        LoadCentralConfigSetupAndUpdateVisibility();
    }

    private void btnResetThumbsFilms_Click(object sender, EventArgs e)
    {
      DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated Film Cover Thumbs?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        foreach (string wfile in Directory.GetFiles(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies", "*.*", SearchOption.AllDirectories))
        {
          try
          {
            System.IO.File.Delete(wfile);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Setup: Error deleting file '" + wfile + "' - Exception: " + ex);
          }
        }
      }
    }

    private void butExternalPlayer_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(tbExternalPlayerPath.Text))
        openFileDialog1.FileName = tbExternalPlayerPath.Text;
      else
      {
        openFileDialog1.FileName = String.Empty;
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Base).ToString();
      }

      if (tbExternalPlayerPath.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = tbExternalPlayerPath.Text.Substring(0, tbExternalPlayerPath.Text.LastIndexOf("\\") + 1);

      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.DefaultExt = "exe";
      openFileDialog1.Filter = "External Player Files|*.exe";
      openFileDialog1.Title = "Select External Player Executable";
      if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        tbExternalPlayerPath.Text = openFileDialog1.FileName;
    }

    private void button1_Click(object sender, EventArgs e)
    {

    }

    private void label41_Click(object sender, EventArgs e)
    {

    }

    private void btnAMCimport_Click(object sender, EventArgs e)
    {
    //  try
    //  {
    //    LogMyFilms.Info("ACMupdater - try loading CurrentSettings");
    //    Module1.CurrentSettings.LoadUserSettings(txtAMCUpd_cnf.Text);

    //    var f = new FileInfo(AMCUpdater.Module1.CurrentSettings.XML_File);
    //    if (!f.Exists)
    //    {
    //      LogMyFilms.Info(((@"XML File '"  + (AMCUpdater.Module1.CurrentSettings.XML_File + @"' Not Found."))));
    //      var destXml = new XmlTextWriter(AMCUpdater.Module1.CurrentSettings.XML_File, System.Text.Encoding.Default);
    //      destXml.Formatting = Formatting.Indented;
    //      destXml.WriteStartDocument(false);
    //      destXml.WriteStartElement("AntMovieCatalog");
    //      destXml.WriteStartElement("Catalog");
    //      destXml.WriteElementString("Properties", "");
    //      destXml.WriteStartElement("Contents");
    //      destXml.WriteEndElement();
    //      destXml.WriteEndElement();
    //      destXml.WriteEndElement();
    //      destXml.Close();
    //      LogMyFilms.Info("Creating XML File - FilePath : " + Module1.CurrentSettings.XML_File);
    //    }
    //    var ant = new AntProcessor();
    //    ant.InteractiveMode = false;
    //    ant.ProcessXML("");
    //    ant.ProcessMovieFolder();
    //    ant.ProcessOrphanFiles();
    //    if (ant.CountOrphanFiles > 0 || ant.CountOrphanRecords > 0)
    //    {
    //      ant.UpdateXMLFile();
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    LogMyFilms.Error("Error: " + ex.Message);
    //  }
    //  finally
    //  {
    //    LogMyFilms.Info("Processing Complete.");
    //  }
    }

    private void AntViewFilterSaveString_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnTestMode_Click(object sender, EventArgs e)
    {
      MyFilms_PluginMode = MyFilms_PluginMode == "normal" ? "test" : "normal";

      if (MyFilms_PluginMode == "normal")
      {
        btnTestMode.ResetBackColor();
      }
      else
      {
        btnTestMode.BackColor = System.Drawing.Color.OrangeRed;
      }
    }

    private void label28_Click(object sender, EventArgs e)
    {

    }
  }

  public static class BindingSourceExtension
  {
    public static void MoveUp(this BindingSource aBindingSource)
    {
      int position = aBindingSource.Position;
      if (position == 0) return;  // already at top

      aBindingSource.RaiseListChangedEvents = false;

      object current = aBindingSource.Current;
      aBindingSource.Remove(current);

      position--;

      aBindingSource.Insert(position, current);
      aBindingSource.Position = position;

      aBindingSource.RaiseListChangedEvents = true;
      aBindingSource.ResetBindings(false);
    }

    public static void MoveDown(this BindingSource aBindingSource)
    {
      int position = aBindingSource.Position;
      if (position == aBindingSource.Count - 1) return;  // already at bottom

      aBindingSource.RaiseListChangedEvents = false;

      object current = aBindingSource.Current;
      aBindingSource.Remove(current);

      position++;

      aBindingSource.Insert(position, current);
      aBindingSource.Position = position;

      aBindingSource.RaiseListChangedEvents = true;
      aBindingSource.ResetBindings(false);
    }

  }
}
