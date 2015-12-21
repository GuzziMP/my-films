using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using Grabber;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

using Grabber.Data;

using MediaPortal.Configuration;

using GrabberScript = Grabber.Data.GrabberScript;


namespace Grabber_Interface
{
  public partial class GrabConfig : Form
  {
    private System.Resources.ResourceManager RM = new System.Resources.ResourceManager("Localisation.Form1", Assembly.GetExecutingAssembly());
    private CultureInfo EnglishCulture = new CultureInfo("en-US");
    private CultureInfo FrenchCulture = new CultureInfo("fr-FR");

    static OpenFileDialog openFileDialog1 = new OpenFileDialog();
    static SaveFileDialog saveFileDialog1 = new SaveFileDialog();

    // the search detail items, we use
    private List<string> searchUrlItemsList = new List<string> { "List", "Title", "Year", "Options", "ID", "Link", "Director", "Akas", "Thumb" };

    private int GLiSearch = 0;
    private int GLiSearchD = 0;
    private int GLiSearchMatches = 0; // Added for search match highlighting
    //block auto text changed.
    private bool GLbBlock = false;
    //block auto selection changed in body
    private bool GLbBlockSelect = false;
    private string Body = string.Empty;
    private string BodyStripped = string.Empty; // added to hold stripped search page

    // vars for details
    private string BodyDetail = string.Empty;
    private string BodyDetail2 = string.Empty;
    private string BodyLinkImg = string.Empty;
    private string BodyLinkGeneric1 = string.Empty;
    private string BodyLinkGeneric2 = string.Empty;
    private string BodyLinkPersons = string.Empty;
    private string BodyLinkTitles = string.Empty;
    private string BodyLinkCertification = string.Empty;
    private string BodyLinkComment = string.Empty;
    private string BodyLinkSyn = string.Empty;
    private string BodyLinkMultiPosters = string.Empty;
    private string BodyLinkPhotos = string.Empty;
    private string BodyLinkPersonImages = string.Empty;
    private string BodyLinkMultiFanart = string.Empty;
    private string BodyLinkTrailer = string.Empty;
    private string BodyLinkDetailsPath = string.Empty;

    private string URLBodyDetail = string.Empty;
    private string URLBodyDetail2 = string.Empty;
    private string URLBodyLinkImg = string.Empty;
    private string URLBodyLinkGeneric1 = string.Empty;
    private string URLBodyLinkGeneric2 = string.Empty;
    private string URLBodyLinkPersons = string.Empty;
    private string URLBodyLinkTitles = string.Empty;
    private string URLBodyLinkCertification = string.Empty;
    private string URLBodyLinkComment = string.Empty;
    private string URLBodyLinkSyn = string.Empty;
    private string URLBodyLinkMultiPosters = string.Empty;
    private string URLBodyLinkPhotos = string.Empty;
    private string URLBodyLinkPersonImages = string.Empty;
    private string URLBodyLinkMultiFanart = string.Empty;
    private string URLBodyLinkTrailer = string.Empty;
    private string URLBodyLinkDetailsPath = string.Empty;

    private string TimeBodyDetail = string.Empty;
    private string TimeBodyDetail2 = string.Empty;
    private string TimeBodyLinkImg = string.Empty;
    private string TimeBodyLinkGeneric1 = string.Empty;
    private string TimeBodyLinkGeneric2 = string.Empty;
    private string TimeBodyLinkPersons = string.Empty;
    private string TimeBodyLinkTitles = string.Empty;
    private string TimeBodyLinkCertification = string.Empty;
    private string TimeBodyLinkComment = string.Empty;
    private string TimeBodyLinkSyn = string.Empty;
    private string TimeBodyLinkMultiPosters = string.Empty;
    private string TimeBodyLinkPhotos = string.Empty;
    private string TimeBodyLinkPersonImages = string.Empty;
    private string TimeBodyLinkMultiFanart = string.Empty;
    private string TimeBodyLinkTrailer = string.Empty;

    private bool ExpertModeOn = true; // to toggle GUI for simplification

    private XmlConf xmlConf;
    private GrabberScript grabberscript;
    private OutputData outputdata = new OutputData();
    private ArrayList listUrl = new ArrayList();
    private CookieContainer cookie = new CookieContainer();
    //private List<LinkPage> LinkPages = new List<LinkPage>();
    //private List<GrabElement> GrabElements = new List<GrabElement>();

    //TabPage tabPageSaveMovie = null;
    //TabPage tabPageSaveDetails = null;

    private string[] Fields = new string[40]; // List to hold all possible grab fields ...

    public GrabConfig(string[] args)
    {
      System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
      InitializeComponent();

      InitMappingTable(); // load Mappingtable with values and other initialization

      if (CultureInfo.InstalledUICulture.ToString() == FrenchCulture.ToString())
        radioButtonFR.Checked = true;
      else
        radioButtonEN.Checked = true;
      // tabPageSearchPage.Enabled = false;
      // tabPageDetailPage.Enabled = false;
      ChangeVisibility(true);

      Assembly asm = Assembly.GetExecutingAssembly();
      Version_Label.Text = "V" + asm.GetName().Version;

      // Test if input arguments were supplied:
      if (args.Length > 0)
      {
        // ExpertModeOn = false; // Disabled due to google request z3us -> show always expert mode
        // ChangeVisibility(false);
        ResetFormControlValues(this);
        if (File.Exists(args[0]))
        {
          textConfig.Text = args[0]; // load command line file
          LoadXml();
          //button_Load_Click(this, e);
        }
      }
    }

    private void button_Browse_Click(object sender, EventArgs e)
    {
      if (Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\"))
      {
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\";
        // openFileDialog1.FileName = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\*.xml";
        // openFileDialog1.FileName = @"*.xml";
        openFileDialog1.RestoreDirectory = false;
      }
      else
      {
        openFileDialog1.RestoreDirectory = true;
      }
      openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
      openFileDialog1.Title = "Find Internet Grabber Script file (xml file)";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        ResetFormControlValues(this);
        textConfig.Text = openFileDialog1.FileName;
        bool loadSuccess = LoadXml();
        if (loadSuccess) button_Load_Click(this, e);
      }
    }

    private void comboSearch_SelectedIndexChanged(object sender, EventArgs e)
    {
      GLbBlock = true;
      buttonPrevParam1.Visible = true;
      label_SearchMatches_Starttext.Text = "";
      label_SearchMatches_Endtext.Text = "";
      textboxSearchAkasRegex.Clear();
      textboxSearchAkasRegex.Visible = false;
      button_Preview.Enabled = true;
      labelSearchAkasRegex.Visible = false;

      foreach (GrabberScript.SearchDetailsRow searchDetailsRow in grabberscript.URLSearch[0].GetSearchDetailsRows().Where(searchDetailsRow => searchDetailsRow.Name == GetSysNameFromDisplay(cb_Parameter.Text)))
      {
        labelSearchAkasRegex.Visible = true;
        textboxSearchAkasRegex.Visible = true;
        TextKeyStart.Text = searchDetailsRow.Start;
        TextKeyStop.Text = searchDetailsRow.End;
        textReplace.Text = searchDetailsRow.Param1;
        textReplaceWith.Text = searchDetailsRow.Param2;
        textboxSearchAkasRegex.Text = searchDetailsRow.RegExp;
        break;
      }

      switch (cb_Parameter.SelectedIndex)
      {
        case 0: // Start - End
          buttonPrevParam1.Visible = false;
          break;
        case 1: // Title
          break;
        case 2: // Year
          break;
        case 3: // Director
          break;
        case 4: // details page URL
          break;
        case 5: // ID (e.g. IMDB_Id)
          break;
        case 6: // Options (e.g. Groups like "Video, Kino, TV, Series, etc.)
          break;
        case 7: // Akas (other title infos)
          labelSearchAkasRegex.Visible = true;
          textboxSearchAkasRegex.Visible = true;
          break;
        case 8: // Thumb
          break;

        default:
          TextKeyStart.Text = "";
          TextKeyStop.Text = "";
          textReplace.Text = "";
          textReplaceWith.Text = "";
          textboxSearchAkasRegex.Text = "";
          break;

      }

      if (cb_Parameter.SelectedIndex > 0)
      {
        textBody.Text = BodyStripped;
        textReplace.Visible = true;
        textReplaceWith.Visible = true;
        //btReset.Visible = true;
        labelReplace.Visible = true;
        labelReplaceWith.Visible = true;
      }
      else
      {
        textBody.Text = Body;
        textReplace.Text = "";
        textReplaceWith.Text = "";
        textReplace.Visible = false;
        textReplaceWith.Visible = false;
        labelReplace.Visible = false;
        labelReplaceWith.Visible = false;
        //btReset.Visible = false;
      }

      GLbBlock = false;
    }

    private string GetSysNameFromDisplay(string displayname)
    {
      if (displayname == "(Start-End Result List)") return "List";
      if (displayname == "URL Detail Page") return "DetailsURL";
      if (displayname == "AKAs") return "Akas";
      if (displayname == "Image") return "Thumb";
      return displayname;
    }
    
    private void button_Load_Click(object sender, EventArgs e)
    {
      if (!TextURL.Text.Contains("#Search#") && !TextURL.Text.Contains("#Filename#") && !TextSearch.Text.Contains("\\"))
      {
        MessageBox.Show("Please, replace search keyword by #Search# in URL or use a media path for local grabbing in Testpage !", "Error");
        return;
      }

      if (TextSearch.Text.Length == 0)
      {
        MessageBox.Show("Please, insert search keyword !", "Error");
        return;
      }
      if (TextURL.Text.Contains("#Page#") && (textPage.Text.Length == 0))
      {
        MessageBox.Show("Please, give the page number to load !", "Error");
        return;
      }

      dataGridViewSearchResults.Rows.Clear();
      if (TextURL.Text.Length > 0)
      {
        string absoluteUri;
        string strSearch;
        string starttext = "";
        string endtext = "";
        int iStart = -1;
        int iEnd = -1;
        int iLength = 0;

        // enable preview button
        button_Preview.Enabled = true;
        // reset preview cover
        pictureBoxPreviewCollection.ImageLocation = "";
        pictureBoxPreviewCover.ImageLocation = "";

        //if (TextURL.Text.StartsWith("http://") == false && !TextSearch.Text.Contains("\\"))
        //  TextURL.Text = "http://" + TextURL.Text;

        strSearch = TextSearch.Text;
        strSearch = GrabUtil.CleanupSearch(strSearch, textSearchCleanup.Text); // cleanup search expression
        if (!strSearch.Contains("\\"))
          strSearch = GrabUtil.EncodeSearch(strSearch, textEncoding.Text);

        string wurl = TextURL.Text.Replace("#Search#", strSearch);
        wurl = wurl.Replace("#Page#", textPage.Text);

        if (wurl.StartsWith("http://") == false && !TextSearch.Text.Contains("\\"))
          wurl = "http://" + wurl;

        Body = GrabUtil.GetPage(wurl, textEncoding.Text, out absoluteUri, cookie, textHeaders.Text, textAccept.Text, textUserAgent.Text);

        //1 resultat -> redirection automatique vers la fiche
        if (!wurl.Equals(absoluteUri))
        {
          Body = "";
          textBody.Text = "";
          listUrl.Clear();
          listUrl.Add(new Grabber_URLClass.IMDBUrl(absoluteUri, TextSearch.Text + " (AutoRedirect)", null, null));

          dataGridViewSearchResults.Rows.Clear();
          while (dataGridViewSearchResults.Rows.Count > 0)
          {
            dataGridViewSearchResults.Rows.RemoveAt(0);
          }
          for (int i = 0; i < 1; i++) // only add 1 line ...
          {
            Grabber_URLClass.IMDBUrl singleUrl = (Grabber_URLClass.IMDBUrl)listUrl[i];
            Image image = GrabUtil.GetImageFromUrl(singleUrl.Thumb); // Image image = Image.FromFile(wurl.Thumb); // Image smallImage = image.GetThumbnailImage(20, 30, null, IntPtr.Zero);
            dataGridViewSearchResults.Rows.Add(new object[] { (i + 1).ToString(), image, singleUrl.Title, singleUrl.Year, singleUrl.Options, singleUrl.ID, singleUrl.URL, singleUrl.Director, singleUrl.Akas });

            //row.Cells[0].Value = i;
            //row.Cells[1].Value = image;
            //row.Cells[2].Value = singleUrl.Title;
            //row.Cells[3].Value = singleUrl.Year;
            //row.Cells[4].Value = singleUrl.Options;
            //row.Cells[5].Value = singleUrl.ID;
            //row.Cells[6].Value = singleUrl.URL;
            //row.Cells[7].Value = singleUrl.Director;
            //row.Cells[8].Value = singleUrl.Akas;

            //i = dataGridViewSearchResults.Rows.Add(row); // add row for config

            //dataGridViewSearchResults.Rows[i].Cells[0].Value = i;
            //dataGridViewSearchResults.Rows[i].Cells[1].Value = image;
            //dataGridViewSearchResults.Rows[i].Cells[2].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).Title;
            //dataGridViewSearchResults.Rows[i].Cells[3].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).Year;
            //dataGridViewSearchResults.Rows[i].Cells[4].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).Options;
            //dataGridViewSearchResults.Rows[i].Cells[5].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).ID;
            //dataGridViewSearchResults.Rows[i].Cells[6].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).URL;
            //dataGridViewSearchResults.Rows[i].Cells[7].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).Director;
            //dataGridViewSearchResults.Rows[i].Cells[8].Value = ((Grabber_URLClass.IMDBUrl)listUrl[0]).Akas;
          }

          if (dataGridViewSearchResults.Rows.Count > 0)
          {
            // dataGridViewSearchResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // dataGridViewSearchResults.Rows[0].Selected = true; //set first line as selected
          }
        }

        if (textRedir.Text.Length > 0)
          Body = GrabUtil.GetPage(textRedir.Text, textEncoding.Text, out absoluteUri, cookie, textHeaders.Text, textAccept.Text, textUserAgent.Text);

        // now stripping the search page
        BodyStripped = Body;
        textBody.Text = Body;
        foreach (GrabberScript.SearchDetailsRow searchDetailsRow in grabberscript.URLSearch[0].GetSearchDetailsRows())
        {
          if (searchDetailsRow.Name == "List")
          {
            starttext = searchDetailsRow.Start;
            endtext = searchDetailsRow.End;
          }
        }

        if (Body.Length > 0 && (starttext.Length > 0 || endtext.Length > 0))
        {
          iStart = 0; iEnd = -1; iLength = 0;
          if (starttext.Length > 0)
          {
            iStart = GrabUtil.FindPosition(Body, starttext, iStart, ref iLength, true, true);
            if (iStart <= 0) { iStart = 0; labelSearchPosition.Text = ""; }
            else { labelSearchPosition.Text = iStart.ToString(); }
          }
          if (endtext.Length > 0)
          {
            iEnd = GrabUtil.FindPosition(Body, endtext, iStart, ref iLength, true, false);
            if (iEnd <= 0) iEnd = Body.Length;
          }

          if (iStart == -1)
            iStart = iEnd;
          if (iEnd == -1)
            iEnd = iStart;
          if ((iEnd == -1) && (iStart == -1))
            iStart = iEnd = 0;

          CountSearchMatches(starttext, endtext);
          BodyStripped = Body.Substring(iStart, iEnd - iStart);
          textBody.Text = BodyStripped; // initial view is stripped, as it's more interesting for script programmer ...
        }
        // CountSearchMatches(starttext, endtext);
      }
    }

    public bool LoadXml()
    {
      grabberscript = new GrabberScript();
      string strTemp;
      string[] split;
      List<string> fields;

      try
      {
        using (var fs = new FileStream(textConfig.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          grabberscript = new GrabberScript();
          //// synchronize dataset with hierarchical XMLdoc
          //xmlDoc = new XmlDataDocument(data);
          //xmlDoc.Load(fs);
          grabberscript.ReadXml(fs);
          fs.Close();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Config not loaded !\n\nError: " + ex.Message, "Info");
        return false;
      }

      try
      {
        string test = grabberscript.UserOptions[0].PreferredLanguage; // this one does not exist in old scripts !
      }
      catch (Exception ex)
      {
        MessageBox.Show("Script is not compatible!\n\nYou might try to convert it to current format.\n\nError: " + ex.Message, "Info");
        return false;
      }
      
      #region init GUI with settings and URLsearch options

      //textName.Text = grabberscript.Settings[0].DBName;
      //textVersion.Text = !grabberscript.Settings[0].IsVersionNull() ? grabberscript.Settings[0].Version : "";
      //textURLPrefix.Text = grabberscript.Settings[0].URLPrefix;
      //textLanguage.Text = !grabberscript.Settings[0].IsLanguageNull() ? grabberscript.Settings[0].Language : "";
      //textType.Text = !grabberscript.Settings[0].IsTypeNull() ? grabberscript.Settings[0].Type : "";
      //textSearchCleanup.Text = !grabberscript.Settings[0].IsSearchCleanupNull() ? grabberscript.Settings[0].SearchCleanup : "";
      //textEncoding.Text = !grabberscript.Settings[0].IsEncodingNull() ? grabberscript.Settings[0].Encoding : "";
      //textAccept.Text = !grabberscript.Settings[0].IsAcceptNull() ? grabberscript.Settings[0].Accept : "";
      //textUserAgent.Text = !grabberscript.Settings[0].IsUseragentNull() ? grabberscript.Settings[0].Useragent : "";
      //textHeaders.Text = !grabberscript.Settings[0].IsHeadersNull() ? grabberscript.Settings[0].Headers : "";
      //cbFileBasedReader.Checked = !grabberscript.Settings[0].IsFileBasedReaderNull() && grabberscript.Settings[0].FileBasedReader;

      textName.Text = grabberscript.Settings[0].DBName;
      textVersion.Text = grabberscript.Settings[0].Version;
      textURLPrefix.Text = grabberscript.Settings[0].URLPrefix;
      textLanguage.Text = grabberscript.Settings[0].Language;
      textType.Text = grabberscript.Settings[0].Type;
      textSearchCleanup.Text = grabberscript.Settings[0].SearchCleanup;
      textEncoding.Text = grabberscript.Settings[0].Encoding;
      textAccept.Text = grabberscript.Settings[0].Accept;
      textUserAgent.Text = grabberscript.Settings[0].Useragent;
      textHeaders.Text = grabberscript.Settings[0].Headers;
      cbFileBasedReader.Checked = grabberscript.Settings[0].FileBasedReader;

      TextURL.Text = grabberscript.URLSearch[0].URL;
      textRedir.Text = grabberscript.URLSearch[0].Redirection;
      textStartPage.Text = grabberscript.URLSearch[0].StartPage;
      textNextPage.Text = grabberscript.URLSearch[0].NextPage;
      textStepPage.Text = grabberscript.URLSearch[0].StepPage;

      textPage.Text = textStartPage.Text;

      foreach (GrabberScript.SearchDetailsRow searchRow in grabberscript.URLSearch[0].GetSearchDetailsRows())
      {
        // nothing to do here, only will be used when user selects the field in GUI
        switch (searchRow.Name)
        {
          case "List":
            //textStartPage.Text = searchRow.Start;
            //ToDo: Init values, if necessary
            
            break;
        }
      }
      #endregion

      #region Load User Settings
      cbMaxActors.Text = grabberscript.UserOptions[0].MaxActors;
      cbMaxActors.Enabled = !string.IsNullOrEmpty(cbMaxActors.Text);
      chkGrabActorRoles.Checked = grabberscript.UserOptions[0].GrabActorRoles;

      // ToDo: once we load the data, the check can be reenabled
      //string strGrabActorRoles = "";
      //string strGrabActorRegex = "";
      //try
      //{
      //  strGrabActorRoles = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value;
      //  strGrabActorRegex = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value;
      //}
      //catch
      //{
      //  chkGrabActorRoles.Checked = false;
      //  chkGrabActorRoles.Enabled = false;
      //}
      //if (string.IsNullOrEmpty(strGrabActorRoles) || string.IsNullOrEmpty(strGrabActorRegex))
      //  chkGrabActorRoles.Enabled = false;
      //else chkGrabActorRoles.Enabled = true;

      cbMaxProducers.Text = grabberscript.UserOptions[0].MaxProducers;
      cbMaxProducers.Enabled = !string.IsNullOrEmpty(this.cbMaxProducers.Text);

      cbMaxDirectors.Text = grabberscript.UserOptions[0].MaxDirectors;
      cbMaxDirectors.Enabled = !string.IsNullOrEmpty(this.cbMaxDirectors.Text);

      cbMaxWriters.Text = grabberscript.UserOptions[0].MaxWriters;
      cbMaxWriters.Enabled = !string.IsNullOrEmpty(this.cbMaxWriters.Text);

      cbTtitlePreferredLanguage.Text = grabberscript.UserOptions[0].TitleCountry;
      //if (string.IsNullOrEmpty(cbTtitlePreferredLanguage.Text)) cbTtitlePreferredLanguage.Enabled = false;
      //else cbTtitlePreferredLanguage.Enabled = true;
      cbTtitleMaxTitles.Text = grabberscript.UserOptions[0].MaxTitles;
      cbTtitleMaxTitles.Enabled = !string.IsNullOrEmpty(cbTtitleMaxTitles.Text);
      cbCertificationPreferredLanguage.Text = grabberscript.UserOptions[0].CertificationCountry;
      //if (string.IsNullOrEmpty(cbCertificationPreferredLanguage.Text)) cbCertificationPreferredLanguage.Enabled = false;
      //else cbCertificationPreferredLanguage.Enabled = true;

      // Add Dropdownentries to User Options
      cbTtitlePreferredLanguage.Items.Clear();
      split = grabberscript.UserOptions[0].TitleCountryAll.ToString().Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbTtitlePreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbTtitlePreferredLanguage.Items.Add(strDroptext.Trim());
      }
      cbTtitlePreferredLanguage.Enabled = cbTtitlePreferredLanguage.Items.Count > 0;

      cbCertificationPreferredLanguage.Items.Clear();
      try
      {
        strTemp = grabberscript.UserOptions[0].CertificationCountry;
      }
      catch
      {
        strTemp = string.Empty;
      }
      split = strTemp.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbCertificationPreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbCertificationPreferredLanguage.Items.Add(strDroptext.Trim());
      }
      cbCertificationPreferredLanguage.Enabled = cbCertificationPreferredLanguage.Items.Count > 0;

      cbAllowMideaInfo.Checked = grabberscript.UserOptions[0].GetMediaInfo;

      cbGoogleTranslateLanguage.Text = grabberscript.UserOptions[0].TranslationLanguage;

      #endregion

      #region Load Mapping to mapping matrix
      fields = Grabber.Grabber_URLClass.FieldList();
      for (int i = 0; i < grabberscript.MappingRules[0].GetMappingRuleRows().Length; i++)
      {
        var mapRuleRow = grabberscript.MappingRules[0].GetMappingRuleRows()[i];
        dataGridViewMapping.Rows[i].Cells[1].Value = mapRuleRow.InputField;
        dataGridViewMapping.Rows[i].Cells[2].Value = mapRuleRow.OutputField;
        dataGridViewMapping.Rows[i].Cells[3].Value = mapRuleRow.Replace;
        dataGridViewMapping.Rows[i].Cells[4].Value = mapRuleRow.AddStart;
        dataGridViewMapping.Rows[i].Cells[5].Value = mapRuleRow.AddEnd;
        dataGridViewMapping.Rows[i].Cells[6].Value = mapRuleRow.MergePreferInput;
        dataGridViewMapping.Rows[i].Cells[7].Value = mapRuleRow.MergePreferOutput;
        // dataGridViewMapping.Rows[i].Cells[8].Value = mapRuleRow.ApplyReplacements;
      }
      #endregion

      return true;
    }

    public void SaveXml(string File)
    {
      #region old save method - DISABLED
      //xmlConf.find(xmlConf.listGen, TagName.DBName)._Value = textName.Text;
      //xmlConf.find(xmlConf.listGen, TagName.URLPrefix)._Value = textURLPrefix.Text;
      //try { xmlConf.find(xmlConf.listGen, TagName.Encoding)._Value = textEncoding.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.Language)._Value = textLanguage.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.Version)._Value = textVersion.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.Type)._Value = textType.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.SearchCleanup)._Value = textSearchCleanup.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.Accept)._Value = textAccept.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.UserAgent)._Value = textUserAgent.Text; } catch (Exception) { }
      //try { xmlConf.find(xmlConf.listGen, TagName.Headers)._Value = textHeaders.Text; } catch (Exception) { }

      //try { xmlConf.find(this.xmlConf.listGen, TagName.FileBasedReader)._Value = cbFileBasedReader.Checked ? "true" : "false"; } catch (Exception) { }

      //xmlConf.find(xmlConf.listSearch, TagName.URL)._Value = TextURL.Text;
      //xmlConf.find(xmlConf.listSearch, TagName.URL)._Param1 = textRedir.Text;
      //xmlConf.find(xmlConf.listSearch, TagName.KeyNextPage)._Value = textNextPage.Text;
      //xmlConf.find(xmlConf.listSearch, TagName.KeyStartPage)._Value = textStartPage.Text;
      //xmlConf.find(xmlConf.listSearch, TagName.KeyStepPage)._Value = textStepPage.Text;


      //XmlTextWriter tw = new XmlTextWriter(File, UTF8Encoding.UTF8);
      //tw.Formatting = Formatting.Indented;
      //tw.WriteStartDocument(true);
      //tw.WriteStartElement("Profile");
      //tw.WriteStartElement("Section");

      //for (int i = 0; i < xmlConf.listGen.Count; i++)
      //{
      //  tw.WriteStartElement(xmlConf.listGen[i]._Tag);
      //  tw.WriteString(xmlConf.listGen[i]._Value);
      //  tw.WriteEndElement();
      //}

      //tw.WriteStartElement("URLSearch");

      //for (int i = 0; i < xmlConf.listSearch.Count; i++)
      //{
      //  tw.WriteStartElement(xmlConf.listSearch[i]._Tag);
      //  if (xmlConf.listSearch[i]._Tag.StartsWith("KeyStart") || xmlConf.listSearch[i]._Tag.Equals("URL"))
      //  {
      //    tw.WriteAttributeString("Param1", XmlConvert.EncodeName(xmlConf.listSearch[i]._Param1));
      //    tw.WriteAttributeString("Param2", XmlConvert.EncodeName(xmlConf.listSearch[i]._Param2));
      //  }
      //  tw.WriteString(XmlConvert.EncodeName(xmlConf.listSearch[i]._Value));
      //  tw.WriteEndElement();

      //}

      //tw.WriteEndElement();
      //tw.WriteStartElement("Details");

      //for (int i = 0; i < xmlConf.listDetail.Count; i++)
      //{
      //  tw.WriteStartElement(xmlConf.listDetail[i]._Tag);
      //  if (xmlConf.listDetail[i]._Tag.StartsWith("KeyStart"))
      //  {
      //    tw.WriteAttributeString("Param1", XmlConvert.EncodeName(xmlConf.listDetail[i]._Param1));
      //    tw.WriteAttributeString("Param2", XmlConvert.EncodeName(xmlConf.listDetail[i]._Param2));
      //  }
      //  tw.WriteString(XmlConvert.EncodeName(xmlConf.listDetail[i]._Value));
      //  tw.WriteEndElement();
      //}
      //tw.WriteEndElement();

      //// Write Mapping Infos
      //tw.WriteStartElement("Mapping");
      //for (int i = 0; i < dataGridViewMapping.Rows.Count; i++)
      //{
      //  tw.WriteStartElement("Field_" + i.ToString()); // fieldnumer
      //  string val1 = string.Empty, val2 = string.Empty, val3 = string.Empty, val4 = string.Empty, val5 = string.Empty, val6 = string.Empty, val7 = string.Empty;
      //  if (dataGridViewMapping.Rows[i].Cells[1].Value != null) val1 = dataGridViewMapping.Rows[i].Cells[1].Value.ToString(); // source
      //  if (dataGridViewMapping.Rows[i].Cells[2].Value != null) val2 = dataGridViewMapping.Rows[i].Cells[2].Value.ToString(); // destination
      //  if (dataGridViewMapping.Rows[i].Cells[3].Value != null) val3 = dataGridViewMapping.Rows[i].Cells[3].Value.ToString(); // replace
      //  if (dataGridViewMapping.Rows[i].Cells[4].Value != null) val4 = dataGridViewMapping.Rows[i].Cells[4].Value.ToString(); // add before
      //  if (dataGridViewMapping.Rows[i].Cells[5].Value != null) val5 = dataGridViewMapping.Rows[i].Cells[5].Value.ToString(); // add after
      //  if (dataGridViewMapping.Rows[i].Cells[6].Value != null) val6 = dataGridViewMapping.Rows[i].Cells[6].Value.ToString(); // merge prefer source
      //  if (dataGridViewMapping.Rows[i].Cells[7].Value != null) val7 = dataGridViewMapping.Rows[i].Cells[7].Value.ToString(); // merge prefer destination

      //  tw.WriteAttributeString("source", XmlConvert.EncodeName(val1));
      //  tw.WriteAttributeString("destination", XmlConvert.EncodeName(val2));
      //  tw.WriteAttributeString("replace", XmlConvert.EncodeName(val3));
      //  tw.WriteAttributeString("addstart", XmlConvert.EncodeName(val4));
      //  tw.WriteAttributeString("addend", XmlConvert.EncodeName(val5));
      //  tw.WriteAttributeString("mergeprefersource", XmlConvert.EncodeName(val6));
      //  tw.WriteAttributeString("mergepreferdestination", XmlConvert.EncodeName(val7));
      //  tw.WriteEndElement();
      //}

      //tw.WriteEndElement();
      //tw.WriteEndElement();
      //tw.WriteEndDocument();
      //// on ferme ensuite le fichier xml
      //tw.Flush();
      //// pour finir on va vider le buffer , et on ferme le fichier
      //tw.Close();
      #endregion old save method

      #region new method:
      //ToDo: Change filename to original one
      grabberscript.Settings[0].DBName = textName.Text;
      grabberscript.Settings[0].Version = textVersion.Text;
      grabberscript.Settings[0].URLPrefix = textURLPrefix.Text;
      grabberscript.Settings[0].Language = textLanguage.Text;
      grabberscript.Settings[0].Type = textType.Text;
      grabberscript.Settings[0].SearchCleanup = textSearchCleanup.Text;
      grabberscript.Settings[0].Encoding = textEncoding.Text;
      grabberscript.Settings[0].Accept = textAccept.Text;
      grabberscript.Settings[0].Useragent = textUserAgent.Text;
      grabberscript.Settings[0].Headers = textHeaders.Text;
      grabberscript.Settings[0].FileBasedReader = cbFileBasedReader.Checked;

      grabberscript.URLSearch[0].URL = TextURL.Text;
      grabberscript.URLSearch[0].Redirection = textRedir.Text;
      grabberscript.URLSearch[0].StartPage = textStartPage.Text;
      grabberscript.URLSearch[0].NextPage = textNextPage.Text;
      grabberscript.URLSearch[0].StepPage = textStepPage.Text;

      // ToDo: Add missing Useroptions
      grabberscript.UserOptions[0].GetMediaInfo = cbAllowMideaInfo.Checked;
      grabberscript.UserOptions[0].TranslationLanguage = cbGoogleTranslateLanguage.Text;


      try
      {
        using (var fs = new FileStream(File, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
          fs.SetLength(0); // do not append, owerwrite !
          // XmlTextWriter tw = new XmlTextWriter(File, UTF8Encoding.UTF8);
          using (var myXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.UTF8))
          {
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
            myXmlTextWriter.WriteStartDocument();
            grabberscript.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema); myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
          }
          //xmlDoc.Save(fs);
          fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error saving script !\n\nError: " + ex.Message, "Info");
      }
      #endregion

    }

    private void ResetFormControlValues(Control parent)
    {
      cb_ParamDetail.SelectedIndex = -1;
      cb_Parameter.SelectedIndex = -1;

      foreach (Control c in parent.Controls)
      {
        if (c.Controls.Count > 0)
        {
          ResetFormControlValues(c);
        }
        else
        {
          switch (c.GetType().ToString())
          {
            case "System.Windows.Forms.ComboBox":
              ((ComboBox)c).SelectedIndex = -1;
              break;
            case "System.Windows.Forms.TextBox":
              if (c.Name.ToString() != "TextSearch" && c.Name != "textPage")
                ((TextBox)c).Text = "";
              break;
            case "System.Windows.Forms.RichTextBox":
              ((RichTextBox)c).Text = "";
              break;
            case "System.Windows.Forms.ListBox":
              ((ListBox)c).Items.Clear();
              break;
          }
        }
      }

      GLiSearchMatches = 0;
      GLiSearch = 0;
      GLiSearchD = 0;
      Body = string.Empty;
      BodyDetail = string.Empty;

      xmlConf = null;
      listUrl = new ArrayList();
      pictureBoxPreviewCollection.ImageLocation = "";
      pictureBoxPreviewCover.ImageLocation = "";

    }

    private void button_Preview_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      if (cbFileBasedReader.Checked && !TextSearch.Text.Contains("\\"))
      {
        MessageBox.Show("You have to select a movie FILE, when File Based Reader is selected !", "Error");
        return;
      }
      else
      {
        // We need to save the config, as the grabber engine loads it from disk !
        SaveXml(textConfig.Text + ".tmp");
        Load_Preview(true); // always ask - gives all matching results! - was false in earlier versions ...
      }
    }

    private void Load_Preview(bool AlwaysAsk)
    {
      // dataGridViewSearchResults.Rows.Clear();
      while (dataGridViewSearchResults.Rows.Count > 0)
      {
        dataGridViewSearchResults.Rows.RemoveAt(0);
      }
      button_GoDetailPage.Enabled = false;
      button_Preview.Enabled = false;

      var Grab = new Grabber_URLClass();
      int pageNumber = -1;
      if (!string.IsNullOrEmpty(textPage.Text))
        pageNumber = Convert.ToInt16(textPage.Text);
      try
      {
        listUrl = Grab.ReturnURL(TextSearch.Text, textConfig.Text + ".tmp", pageNumber, AlwaysAsk, string.Empty);
      }
      catch (Exception ex)
      {
        DialogResult dlgResult = DialogResult.None;
        button_Preview.Enabled = true;
        dlgResult = MessageBox.Show("Grabber ERROR - check your definitions! \n\nException Message: " + ex.Message + "\nStacktrace: " + ex.StackTrace, "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK) { }
      }

      for (int i = 0; i < listUrl.Count; i++)
      {
        //DataGridViewRow row = new DataGridViewRow();
        //row.Cells[0].Value = i;
        //row.Cells[1].Value = image;
        //row.Cells[2].Value = wurl.Title;
        //row.Cells[3].Value = wurl.Year;
        //row.Cells[4].Value = wurl.Options;
        //row.Cells[5].Value = wurl.ID;
        //row.Cells[6].Value = wurl.URL;
        //row.Cells[7].Value = wurl.Director;
        //row.Cells[8].Value = wurl.Akas;
        //i = dataGridViewSearchResults.Rows.Add(row); // add row for config

        var wurl = (Grabber_URLClass.IMDBUrl)listUrl[i];
        Image image = GrabUtil.GetImageFromUrl(wurl.Thumb); // Image image = Image.FromFile(wurl.Thumb); // Image smallImage = image.GetThumbnailImage(20, 30, null, IntPtr.Zero);
        dataGridViewSearchResults.Rows.Add(new object[] { (i + 1).ToString(), image, wurl.Title, wurl.Year, wurl.Options, wurl.ID, wurl.URL, wurl.Director, wurl.Akas });


        //dataGridViewSearchResults.Rows[i].Cells[0].Value = i;
        //dataGridViewSearchResults.Rows[i].Cells[1].Style.NullValue = null;
        //dataGridViewSearchResults.Rows[i].Cells[1].Value = image;
        //dataGridViewSearchResults.Rows[i].Cells[2].Value = wurl.Title;
        //dataGridViewSearchResults.Rows[i].Cells[3].Value = wurl.Year;
        //dataGridViewSearchResults.Rows[i].Cells[4].Value = wurl.Options;
        //dataGridViewSearchResults.Rows[i].Cells[5].Value = wurl.ID;
        //dataGridViewSearchResults.Rows[i].Cells[6].Value = wurl.URL;
        //dataGridViewSearchResults.Rows[i].Cells[7].Value = wurl.Director;
        //dataGridViewSearchResults.Rows[i].Cells[8].Value = wurl.Akas;
      }
      if (dataGridViewSearchResults.Rows.Count > 0)
      {
        dataGridViewSearchResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewSearchResults.Rows[0].Selected = true; //set first line as selected
        button_GoDetailPage.Enabled = true;
        button_Preview.Enabled = true;
      }
    }

    private void button_Find_Click(object sender, EventArgs e)
    {
      try
      {
        int iLength = 0;
        int i = GrabUtil.FindPosition(textBody.Text, textBox5.Text, GLiSearch, ref iLength, true, false, cbIgnoreCase.Checked);
        //int i = textBody.Find(textBox5.Text, GLiSearch, RichTextBoxFinds.None);
        if (i > 0)
        {
          textBody.Select(i, iLength);
          // textBody.Select(i, textBox5.Text.Length);
          GLiSearch = i + iLength;
        }
        else
          GLiSearch = 0;
      }
      catch (Exception)
      {
        GLiSearch = 0;
      }
    }

    private void textBody_Click(object sender, EventArgs e)
    {
      GLiSearch = ((RichTextBox)sender).SelectionStart;
      GLiSearchMatches = ((RichTextBox)sender).SelectionStart;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      DialogResult dlgResult = DialogResult.None;

      if (textConfig.Text.Length > 0)
      {
        dlgResult = MessageBox.Show("Save current configuration ?", "Save", MessageBoxButtons.YesNoCancel);
        if (dlgResult == DialogResult.Yes)
        {
          SaveXml(textConfig.Text);
          ResetFormControlValues(this);
        }
        if (dlgResult == DialogResult.No)
          ResetFormControlValues(this);

      }

      if ((textConfig.Text.Length == 0) && (dlgResult != DialogResult.Cancel))
      {
        saveFileDialog1.RestoreDirectory = true;
        saveFileDialog1.Filter = "Load xml (*.xml)|*.xml";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          LoadXml();
        }
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        SaveXml(textConfig.Text);
        MessageBox.Show("Config saved !", "Info");
      }
    }

    private void button_SaveAs_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        if (Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms"))
        {
          if (!Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\myfilms\user"))
          {
            try { Directory.CreateDirectory(Config.Dir.Config + @"\scripts\myfilms\user"); }
            catch (Exception) { }
          }
          saveFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms";
        }
        else
        {
          saveFileDialog1.RestoreDirectory = true;
        }
        saveFileDialog1.FileName = textConfig.Text;
        saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
        saveFileDialog1.Title = "Save Internet Grabber Script file (xml file)";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          SaveXml(textConfig.Text);
          MessageBox.Show("Config saved !", "Info");
        }
      }
    }

    private void TextKeyStart_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      foreach (GrabberScript.SearchDetailsRow searchDetailsRow in grabberscript.URLSearch[0].GetSearchDetailsRows().Where(searchDetailsRow => searchDetailsRow.Name == GetSysNameFromDisplay(cb_Parameter.Text)))
      {
        searchDetailsRow.Start = TextKeyStart.Text;
        break;
      }

      textBody.Text = cb_Parameter.SelectedIndex > 0 ? BodyStripped : Body;
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, false);

    }

    private void TextKeyStop_TextChanged(object sender, EventArgs e)
    {
      foreach (GrabberScript.SearchDetailsRow searchDetailsRow in grabberscript.URLSearch[0].GetSearchDetailsRows().Where(searchDetailsRow => searchDetailsRow.Name == GetSysNameFromDisplay(cb_Parameter.Text)))
      {
        searchDetailsRow.End = TextKeyStop.Text;
        break;
      }

      // change text, if neccessary and redo selection
      textBody.Text = cb_Parameter.SelectedIndex > 0 ? BodyStripped : Body;  // && TextKeyStop.Text.Length > 0
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, false);
    }

    private void textboxSearchAkasRegex_TextChanged(object sender, EventArgs e)
    {
      foreach (GrabberScript.SearchDetailsRow searchDetailsRow in grabberscript.URLSearch[0].GetSearchDetailsRows().Where(searchDetailsRow => searchDetailsRow.Name == GetSysNameFromDisplay(cb_Parameter.Text)))
      {
        searchDetailsRow.RegExp = textboxSearchAkasRegex.Text;
        break;
      }
    }

    private void textBody_SelectionChanged(object sender, EventArgs e)
    {
      if (GLbBlockSelect == true)
        return;

      int nb = 0;
      int i = 0;
      int iLength = 0;
      // i = textBody.Find(textBody.SelectedText, 0, RichTextBoxFinds.NoHighlight);
      i = GrabUtil.FindPosition(textBody.Text, textBody.SelectedText, i, ref iLength, true, false);
      while (i > 0)
      {
        nb++;
        // i = textBody.Find(textBody.SelectedText, i + textBody.SelectedText.Length, RichTextBoxFinds.NoHighlight);
        i = GrabUtil.FindPosition(textBody.Text, textBody.SelectedText, i + iLength, ref iLength, true, false);
      }
      lblResultsFound.Text = nb + " match(es) found";
    }

    private void textBody_NewSelection(string starttext, string endtext, bool manualselect)
    {
      // If you have at least the key to start, we cut strBody
      if (textBody.Text.Length > 0 && (starttext.Length > 0 || endtext.Length > 0))
      {
        GLbBlockSelect = true;

        int iStart = 0;
        int iEnd = 0;
        int iLength = 0;

        if (manualselect)
          iStart = GLiSearchMatches;


        if (starttext.Length > 0)
        {
          iStart = GrabUtil.FindPosition(textBody.Text, starttext, iStart, ref iLength, true, true);
          if (iStart <= 0) { iStart = 0; labelSearchPosition.Text = ""; }
          else { labelSearchPosition.Text = iStart.ToString(); }
        }
        if (endtext.Length > 0)
        {
          iEnd = GrabUtil.FindPosition(textBody.Text, endtext, iStart, ref iLength, true, false);
          if (iEnd <= 0) iEnd = textBody.Text.Length;
        }

        if (iStart == -1)
          iStart = iEnd;
        if (iEnd == -1)
          iEnd = iStart;
        if ((iEnd == -1) && (iStart == -1))
          iStart = iEnd = 0;

        if (manualselect)
          GLiSearchMatches = iEnd;

        CountSearchMatches(starttext, endtext);
        textBody.Select(iStart, iEnd - iStart);

        GLbBlockSelect = false;
        textBody_SelectionChanged(null, null);
      }
    }

    private void CountSearchMatches(string starttext, string endtext)
    {
      int nb = 0;
      int i = 0;
      int iLength = 0;
      bool bregexs = false;
      bool bregexe = false;
      if (starttext.StartsWith("#REGEX#")) bregexs = true;
      if (endtext.StartsWith("#REGEX#")) bregexe = true;

      if (bregexs)
        i = GrabUtil.FindRegEx(textBody.Text, starttext, i, ref iLength, true) + i;
      else
        i = textBody.Text.IndexOf(starttext, i);
      // i = textBody.Find(starttext, 0, RichTextBoxFinds.NoHighlight);
      while (i > 0)
      {
        nb++;
        //i = textBody.Find(starttext, i + starttext.Length, RichTextBoxFinds.NoHighlight);
        if (bregexs)
        {
          i = GrabUtil.FindRegEx(textBody.Text, starttext, i + starttext.Length, ref iLength, true) + i;
          if (iLength == 0)
            i = 0;
          else
            i += iLength;
        }
        else
          i = textBody.Text.IndexOf(starttext, i + starttext.Length);
      }
      label_SearchMatches_Starttext.Text = nb.ToString();

      nb = 0;
      i = 0;
      iLength = 0;
      if (bregexe)
        i = GrabUtil.FindRegEx(textBody.Text, endtext, i, ref iLength, true) + i;
      else
        i = textBody.Text.IndexOf(endtext, i);
      //i = textBody.Find(endtext, 0, RichTextBoxFinds.NoHighlight);
      while (i > 0)
      {
        nb++;
        //i = textBody.Find(endtext, i + endtext.Length, RichTextBoxFinds.NoHighlight);
        if (bregexe)
        {
          i = GrabUtil.FindRegEx(textBody.Text, endtext, i + endtext.Length, ref iLength, true) + i;
          if (iLength == 0)
            i = 0;
          else
            i += iLength;
        }
        else
          i = textBody.Text.IndexOf(endtext, i + endtext.Length);
      }
      label_SearchMatches_Endtext.Text = nb.ToString();
    }

    private void GrabConfig_FormClosing(object sender, FormClosingEventArgs e)
    {
      DialogResult dlgResult = DialogResult.None;

      if (textConfig.Text.Length > 0)
      {
        dlgResult = MessageBox.Show("Save current configuration ?", "Save", MessageBoxButtons.YesNoCancel);
        if (dlgResult == DialogResult.Yes)
        {
          SaveXml(textConfig.Text);

        }
        if (dlgResult == DialogResult.Cancel)
          e.Cancel = true;
      }
    }

    /*
     *
     * DETAIL PAGE
     * 
     */

    private void ButtonLoad_Click(object sender, EventArgs e)
    {
      string absoluteUri;
      int iStart;
      int iEnd;
      string strStart = string.Empty;
      string strEnd = string.Empty;
      string strParam1 = string.Empty;
      string strParam2 = string.Empty;
      string strIndex = string.Empty;
      string strPage = string.Empty;
      string strEncoding = string.Empty;
      string strActivePage = string.Empty;

      URLBodyDetail = string.Empty;
      URLBodyDetail2 = string.Empty;
      URLBodyLinkImg = string.Empty;
      URLBodyLinkGeneric1 = string.Empty;
      URLBodyLinkGeneric2 = string.Empty;
      URLBodyLinkPersons = string.Empty;
      URLBodyLinkTitles = string.Empty;
      URLBodyLinkCertification = string.Empty;
      URLBodyLinkComment = string.Empty;
      URLBodyLinkSyn = string.Empty;
      URLBodyLinkMultiPosters = string.Empty;
      URLBodyLinkPhotos = string.Empty;
      URLBodyLinkPersonImages = string.Empty;
      URLBodyLinkMultiFanart = string.Empty;
      URLBodyLinkTrailer = string.Empty;

      TimeBodyDetail = string.Empty;
      TimeBodyDetail2 = string.Empty;
      TimeBodyLinkImg = string.Empty;
      TimeBodyLinkGeneric1 = string.Empty;
      TimeBodyLinkGeneric2 = string.Empty;
      TimeBodyLinkPersons = string.Empty;
      TimeBodyLinkTitles = string.Empty;
      TimeBodyLinkCertification = string.Empty;
      TimeBodyLinkComment = string.Empty;
      TimeBodyLinkSyn = string.Empty;
      TimeBodyLinkMultiPosters = string.Empty;
      TimeBodyLinkPhotos = string.Empty;
      TimeBodyLinkPersonImages = string.Empty;
      TimeBodyLinkMultiFanart = string.Empty;
      TimeBodyLinkTrailer = string.Empty;

      Stopwatch watch = new Stopwatch();

      if (TextURLDetail.Text.Length > 0)
      {
        #region Load the DetailsPath
        if (TextURLDetail.Text.Length > 0)
        {
          URLBodyLinkDetailsPath = TextURLDetail.Text;
          if (TextURLDetail.Text.ToLower().StartsWith("http"))
          {
            BodyLinkDetailsPath = "<url>" + TextURLDetail.Text + "</url>";
            if (TextURLDetail.Text.LastIndexOf("/", StringComparison.Ordinal) > 0)
            {
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<baseurl>" + TextURLDetail.Text.Substring(0, TextURLDetail.Text.LastIndexOf("/", StringComparison.Ordinal)) + "</baseurl>";
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<pageurl>" + TextURLDetail.Text.Substring(TextURLDetail.Text.LastIndexOf("/", StringComparison.Ordinal) + 1) + "</pageurl>";
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<replacement>" + TextURLDetail.Text.Substring(0, TextURLDetail.Text.LastIndexOf("/", StringComparison.Ordinal)) + "%replacement%" + TextURLDetail.Text.Substring(TextURLDetail.Text.LastIndexOf("/", StringComparison.Ordinal) + 1) + "</replacement>";
            }
          }
          else
          {
            string strURL = TextURLDetail.Text;
            if (File.Exists(strURL))
            {
              string MovieDirectory = Path.GetDirectoryName(strURL);
              string MovieFilename = Path.GetFileNameWithoutExtension(strURL);
              // Set DetailsPath
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<directory>" + MovieDirectory + "</directory>";
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<filename>" + MovieFilename + "</filename>";
              if (MovieDirectory != null)
              {
                string[] files = Directory.GetFiles(MovieDirectory, "*", SearchOption.AllDirectories);

                //foreach (string extension in files.Select(file => Path.GetExtension(file)).Distinct().ToList())
                //{
                //  BodyLinkDetailsPath += Environment.NewLine;
                //  BodyLinkDetailsPath += "<" + extension + "-files>";
                //  foreach (string file in files.Where(file => file.EndsWith("." + extension)).ToList())
                //  {
                //    BodyLinkDetailsPath += Environment.NewLine;
                //    BodyLinkDetailsPath += "<" + extension + ">" + file + "</" + extension + ">";
                //  }
                //  BodyLinkDetailsPath += Environment.NewLine;
                //  BodyLinkDetailsPath += "</" + extension + "-files>";
                //}

                BodyLinkDetailsPath += Environment.NewLine;
                BodyLinkDetailsPath += "<jpg-files>";
                foreach (string file in files.Where(file => file.EndsWith(".jpg")).ToList())
                {
                  BodyLinkDetailsPath += Environment.NewLine;
                  BodyLinkDetailsPath += "<jpg>" + file + "</jpg>";
                }
                BodyLinkDetailsPath += Environment.NewLine;
                BodyLinkDetailsPath += "</jpg-files>";

                BodyLinkDetailsPath += Environment.NewLine;
                BodyLinkDetailsPath += "<other-files>";
                foreach (string file in files.Where(file => !file.EndsWith(".jpg")).ToList())
                {
                  BodyLinkDetailsPath += Environment.NewLine;
                  BodyLinkDetailsPath += "<other>" + file + "</other>";
                }
                BodyLinkDetailsPath += Environment.NewLine;
                BodyLinkDetailsPath += "</other-files>";
              }
            }
          }
        }
        else
          BodyLinkDetailsPath = "";
        #endregion

        #region Load basic page
        watch.Reset();
        watch.Start();
        textPreview.ResetText();
        URLBodyDetail = TextURLDetail.Text;
        BodyDetail = TextURLDetail.Text.ToLower().StartsWith("http")
                            ? GrabUtil.GetPage(TextURLDetail.Text, textEncoding.Text, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text)
                            : GrabUtil.GetFileContent(TextURLDetail.Text, textEncoding.Text);


        if (grabberscript.SearchDetails[0].Start.Length > 0)
        {
          iStart = BodyDetail.IndexOf(grabberscript.SearchDetails[0].Start);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            iEnd = grabberscript.SearchDetails[0].End != "" ? BodyDetail.IndexOf(grabberscript.SearchDetails[0].End, iStart) : BodyDetail.Length;

            if (iEnd == -1)
              iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length;
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
            textBodyDetail.Text = BodyDetail;
          }
          else
            textBodyDetail.Text = BodyDetail;
        }
        else
          textBodyDetail.Text = BodyDetail;

        watch.Stop();
        TimeBodyDetail = " (" + (watch.ElapsedMilliseconds) + " ms)";
        #endregion
      }

      #region load all other data pages ...

      //#region Test if there is a page for Secondary Details (like OFDB GW) and load page in BodyDetails2
      //try
      //{
      //  watch.Reset();
      //  watch.Start();
      //  strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value;
      //  strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value;
      //  strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1;
      //  strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2;
      //  strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value;
      //  strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value;
      //  try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingDetails2)._Value; }
      //  catch (Exception) { strPage = ""; }

      //  strActivePage = this.LoadPage(strPage);
      //  if (strStart.Length > 0)
      //  {
      //    string strTemp = string.Empty;
      //    if (strParam1.Length > 0 && strParam2.Length > 0)
      //      strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //    else
      //      strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //    URLBodyDetail2 = strTemp;
      //    BodyDetail2 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //  }
      //  else
      //    BodyDetail2 = "";
      //  watch.Stop();
      //  TimeBodyDetail2 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //}
      //catch
      //{
      //  BodyDetail2 = "";
      //}
      //#endregion

      //#region Test if there is a page for Generic 1 Page
      //try
      //{
      //  watch.Reset();
      //  watch.Start();
      //  strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value;
      //  strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value;
      //  strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1;
      //  strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2;
      //  strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value;
      //  strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value;
      //  try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric1)._Value; }
      //  catch (Exception) { strPage = ""; }

      //  strActivePage = this.LoadPage(strPage);
      //  if (strStart.Length > 0)
      //  {
      //    string strTemp = string.Empty;
      //    if (strParam1.Length > 0 && strParam2.Length > 0)
      //      strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //    else
      //      strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //    URLBodyLinkGeneric1 = strTemp;
      //    BodyLinkGeneric1 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //  }
      //  else
      //    BodyLinkGeneric1 = "";
      //  watch.Stop();
      //  TimeBodyLinkGeneric1 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //}
      //catch
      //{
      //  BodyLinkGeneric1 = "";
      //}
      //#endregion

      //#region Test if there is a page for Generic 2 Page
      //try
      //{
      //  watch.Reset();
      //  watch.Start();
      //  strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value;
      //  strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value;
      //  strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1;
      //  strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2;
      //  strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value;
      //  strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value;
      //  try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric2)._Value; }
      //  catch (Exception) { strEncoding = ""; }

      //  strActivePage = this.LoadPage(strPage);
      //  if (strStart.Length > 0)
      //  {
      //    string strTemp = string.Empty;
      //    if (strParam1.Length > 0 && strParam2.Length > 0)
      //      strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //    else
      //      strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //    URLBodyLinkGeneric2 = strTemp;
      //    BodyLinkGeneric2 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //  }
      //  else
      //    BodyLinkGeneric2 = "";
      //  watch.Stop();
      //  TimeBodyLinkGeneric2 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //}
      //catch
      //{
      //  BodyLinkGeneric2 = "";
      //}
      //#endregion

      //#region Test if there is a redirection page for Covers and load page in BodyLinkImg
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkImg)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkImg = strTemp;
      //  BodyLinkImg = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkImg = "";
      //watch.Stop();
      //TimeBodyLinkImg = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Persons and load page in BodyLinkPersons
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersons)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkPersons = strTemp;
      //  BodyLinkPersons = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkPersons = "";
      //watch.Stop();
      //TimeBodyLinkPersons = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Titles and load page in BodyLinkTitles
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTitles)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkTitles = strTemp;
      //  BodyLinkTitles = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkTitles = "";
      //watch.Stop();
      //TimeBodyLinkTitles = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Certification and load page in BodyLinkCertification
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkCertification)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkCertification = strTemp;
      //  BodyLinkCertification = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkCertification = "";
      //watch.Stop();
      //TimeBodyLinkCertification = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Synopsis/Description and load page in BodyLinkSyn
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkSyn)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkSyn = strTemp;
      //  BodyLinkSyn = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkSyn = "";
      //watch.Stop();
      //TimeBodyLinkSyn = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Comment and load page in BodyLinkComment
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkComment)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkComment = strTemp;
      //  BodyLinkComment = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkComment = "";
      //watch.Stop();
      //TimeBodyLinkComment = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for MultiPosters and load page in BodyLinkMultiPosters
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiPosters)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiPosters)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkMultiPosters = strTemp;
      //  BodyLinkMultiPosters = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkMultiPosters = "";
      //watch.Stop();
      //TimeBodyLinkMultiPosters = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Photos and load page in BodyLinkPhotos
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPhotos)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPhotos)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkPhotos = strTemp;
      //  BodyLinkPhotos = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkPhotos = "";
      //watch.Stop();
      //TimeBodyLinkPhotos = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for PersonImages and load page in BodyLinkPersonImages
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersonImages)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersonImages)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkPersonImages = strTemp;
      //  BodyLinkPersonImages = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkPersonImages = "";
      //watch.Stop();
      //TimeBodyLinkPersonImages = " (" + (watch.ElapsedMilliseconds) + " ms)";
      //#endregion

      //#region Test if there is a redirection page for MultiFanart and load page in BodyLinkMultiFanart
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiFanart)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiFanart)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkMultiFanart = strTemp;
      //  BodyLinkMultiFanart = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkMultiFanart = "";
      //watch.Stop();
      //TimeBodyLinkMultiFanart = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion

      //#region Test if there is a redirection page for Trailer and load page in BodyLinkTrailer
      //watch.Reset();
      //watch.Start();
      //strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Value;
      //strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTrailer)._Value;
      //strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param1;
      //strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param2;
      //strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerIndex)._Value;
      //strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerPage)._Value;
      //try { strEncoding = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTrailer)._Value; }
      //catch (Exception) { strEncoding = ""; }

      //strActivePage = this.LoadPage(strPage);
      //if (strStart.Length > 0)
      //{
      //  string strTemp = string.Empty;
      //  if (strParam1.Length > 0 && strParam2.Length > 0)
      //    strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
      //  else
      //    strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
      //  URLBodyLinkTrailer = strTemp;
      //  BodyLinkTrailer = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncoding)) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      //}
      //else
      //  BodyLinkTrailer = "";
      //watch.Stop();
      //TimeBodyLinkTrailer = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      //#endregion
      
      #endregion
    }

    private string LoadPage(string page)
    {
      string strActivePage = string.Empty;
      switch (page)
      {
        case "":
          strActivePage = BodyDetail;
          textURLPreview.Text = URLBodyDetail;
          break;
        case "URL Gateway":
          strActivePage = BodyDetail2;
          textURLPreview.Text = URLBodyDetail2;
          break;
        case "URL Redirection Generic 1":
          strActivePage = BodyLinkGeneric1;
          textURLPreview.Text = URLBodyLinkGeneric1;
          break;
        case "URL Redirection Generic 2":
          strActivePage = BodyLinkGeneric2;
          textURLPreview.Text = URLBodyLinkGeneric2;
          break;
        case "URL Redirection Cover":
          strActivePage = BodyLinkImg;
          textURLPreview.Text = URLBodyLinkImg;
          break;
        case "URL Redirection Persons":
          strActivePage = BodyLinkPersons;
          textURLPreview.Text = URLBodyLinkPersons;
          break;
        case "URL Redirection Title":
          strActivePage = BodyLinkTitles;
          textURLPreview.Text = URLBodyLinkTitles;
          break;
        case "URL Redirection Certification":
          strActivePage = BodyLinkCertification;
          textURLPreview.Text = URLBodyLinkCertification;
          break;
        case "URL Redirection Comment":
          strActivePage = BodyLinkComment;
          textURLPreview.Text = URLBodyLinkComment;
          break;
        case "URL Redirection Description":
          strActivePage = BodyLinkSyn;
          textURLPreview.Text = URLBodyLinkSyn;
          break;
        case "URL Redirection Multi Posters":
          strActivePage = BodyLinkMultiPosters;
          textURLPreview.Text = URLBodyLinkMultiPosters;
          break;
        case "URL Redirection Photos":
          strActivePage = BodyLinkPhotos;
          textURLPreview.Text = URLBodyLinkPhotos;
          break;
        case "URL Redirection PersonImages":
          strActivePage = BodyLinkPersonImages;
          textURLPreview.Text = URLBodyLinkPersonImages;
          break;
        case "URL Redirection Multi Fanart":
          strActivePage = BodyLinkMultiFanart;
          textURLPreview.Text = URLBodyLinkMultiFanart;
          break;
        case "URL Redirection Trailer":
          strActivePage = BodyLinkTrailer;
          textURLPreview.Text = URLBodyLinkTrailer;
          break;
        case "DetailsPath":
          strActivePage = BodyLinkDetailsPath;
          textURLPreview.Text = URLBodyLinkDetailsPath;
          break;

        default:
          strActivePage = "";
          textURLPreview.Text = "";
          lblResult.Text = "Sub URL";
          break;
      }
      if (!string.IsNullOrEmpty(strActivePage))
        lblResult.Text = strActivePage.Length.ToString();
      return strActivePage;
    }

    private void textBodyDetail_NewSelection(string starttext, string endtext, int bodystart, string param1)
    {
      if (textBodyDetail.Text.Length > 0 && starttext.Length > 0 && endtext.Length > 0)
      {
        GLbBlockSelect = true;

        int iStart = 0;
        int iEnd = 0;
        int iLength = 0;

        string strTemp = String.Empty;
        // HtmlUtil htmlUtil = new HtmlUtil(); // in MP Core.dll
        bool bregexs = false;
        bool bregexe = false;
        if (starttext.StartsWith("#REGEX#"))
          bregexs = true;
        if (endtext.StartsWith("#REGEX#"))
          bregexe = true;

        if (starttext != "" && endtext != "")
        {
          iLength = starttext.Length;
          if (param1.StartsWith("#REVERSE#"))
          {
            iStart = bregexs ? GrabUtil.FindRegEx(textBodyDetail.Text, starttext, iStart, ref iLength, false) : textBodyDetail.Text.LastIndexOf(starttext);
          }
          else if (bregexs) iStart = GrabUtil.FindRegEx(textBodyDetail.Text, starttext, iStart, ref iLength, true);
          else iStart = textBodyDetail.Text.IndexOf(starttext);

          if (iStart > 0)
          {
            if (param1.StartsWith("#REVERSE#"))
            {
              iLength = endtext.Length;
              if (bregexe) iEnd = GrabUtil.FindRegEx(textBodyDetail.Text, endtext, iStart, ref iLength, false) + iStart;
              else iEnd = textBodyDetail.Text.LastIndexOf(endtext, iStart);
            }
            else
            {
              iStart += iLength;
              if (bregexe) iEnd = GrabUtil.FindRegEx(textBodyDetail.Text, endtext, iStart, ref iLength, true) + iStart;
              else iEnd = textBodyDetail.Text.IndexOf(endtext, iStart);
            }
          }
        }
        // Old method (not using regex)
        //try
        //{
        //  iStart = textBodyDetail.Text.IndexOf(starttext, bodystart) + starttext.Length;
        //  iEnd = textBodyDetail.Find(endtext, iStart, RichTextBoxFinds.None);
        //}
        //catch
        //{
        //  MessageBox.Show("Cannot find searchtext with given parameter, please change !", "Error");
        //}
        if (iStart == -1)
          iStart = 0;
        if (iEnd == -1)
          iEnd = 0;

        textBodyDetail.Select(iStart, iEnd - iStart);

        //if (textDReplace.Text.Length > 0 && textDReplaceWith.Text.Length > 0)
        //{
        //    textBodyDetail.SelectedText = textBodyDetail.SelectedText.Replace(textDReplace.Text, textDReplaceWith.Text);

        //    iStart = textBodyDetail.Text.IndexOf(starttext, bodystart) + starttext.Length;
        //    iEnd = textBodyDetail.Find(endtext, iStart, RichTextBoxFinds.None);
        //    if (iStart == -1)
        //        iStart = 0;
        //    if (iEnd == -1)
        //        iEnd = 0;

        //    textBodyDetail.Select(iStart, iEnd - iStart);
        //}

        GLbBlockSelect = false;
        textBodyDetail_SelectionChanged(null, null);
      }
    }

    private void cbParamDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
      GLbBlock = true;

      textComplement.Clear();
      textMaxItems.Clear();
      textLanguages.Clear();
      textLanguagesAll.Clear();
      lblComplement.Visible = false;
      lblMaxItems.Visible = false;
      lblLanguages.Visible = false;
      lblLanguagesAll.Visible = false;
      textComplement.Visible = false;
      textMaxItems.Visible = false;
      textLanguages.Visible = false;
      textLanguagesAll.Visible = false;
      chkActorRoles.Visible = false;
      chkActorRoles.Enabled = false;
      buttonPrevParamDetail.Visible = true;
      lblResult.Text = "Sub URL";
      //lblComplement.Text = "Complement";
      lblEncodingSubPage.Visible = false;
      EncodingSubPage.Visible = false;
      EncodingSubPage.Text = "";
      if (!textBodyDetail.Text.Equals(BodyDetail))
        textBodyDetail.Text = BodyDetail;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0: // Start/end Page
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value;
          buttonPrevParamDetail.Visible = false;
          break;
        case 1: // Original Title
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyOTitlePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndOTitle)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyOTitleIndex)._Value;
          break;
        case 2: // Translated Title
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitlePage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkTitles)) textBodyDetail.Text = BodyLinkTitles;
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguageAll)._Value; }
          catch { textLanguagesAll.Text = string.Empty; }
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTTitle)._Value;
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleIndex)._Value;
          break;
        case 3: // Coverimage
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyImgPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkImg)) textBodyDetail.Text = BodyLinkImg;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyImgIndex)._Value;
          break;
        case 4: // Rating 1
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRatePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRateIndex)._Value;
          break;
        case 5: // Rating 2
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate2)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Index)._Value;
          break;
        case 6: // Director
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealisePage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value;
          break;
        case 7: // Producer
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value;
          break;
        case 8: // Writer
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndWriter)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterIndex)._Value;
          break;
        case 9: // Actors (Credits)
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCredits)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          string strActorRoles = string.Empty;
          try
          {
            strActorRoles = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value;
            this.chkActorRoles.Checked = strActorRoles == "true";
          }
          catch { chkActorRoles.Checked = false; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsIndex)._Value;
          break;
        case 10: // Country
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCountry)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryIndex)._Value;
          break;
        case 11: // Category
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenrePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGenre)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenreRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenreIndex)._Value;
          break;
        case 12: // Year
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyYearPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndYear)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyYearIndex)._Value;
          break;
        case 13: // IMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Id)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdIndex)._Value;
          break;
        case 14: // Description
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeySynPage)._Value;
          //if (!textBodyDetail.Text.Equals(BodyLinkSyn)) textBodyDetail.Text = BodyLinkSyn;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value;
          break;
        case 15: // Comment
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkComment)) textBodyDetail.Text = BodyLinkComment;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndComment)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentIndex)._Value;
          break;
        case 16: // Language
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLanguagePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLanguage)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageIndex)._Value;
          break;
        case 17: // Tagline
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTaglinePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTagline)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTaglineIndex)._Value;
          break;
        case 18: // Certification
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkCertification)) textBodyDetail.Text = BodyLinkCertification;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCertification)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguageAll)._Value; }
          catch { textLanguagesAll.Text = string.Empty; }
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationIndex)._Value;
          break;
        case 19: // Studio
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStudioPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndStudio)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStudioRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStudioIndex)._Value;
          break;
        case 20: // Edition
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEditionPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndEdition)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEditionIndex)._Value;
          break;
        case 21: // IMDB_Rank
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Rank)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankIndex)._Value;
          break;
        case 22: // TMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTMDB_Id)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdIndex)._Value;
          break;
        case 23: // Generic Field 1
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric1)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1RegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Language)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Index)._Value;
          break;
        case 24: // Generic Field 2
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric2)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2RegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Language)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Index)._Value;
          break;
        case 25: // Generic Field 3
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric3)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3RegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Language)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Index)._Value;
          break;
        case 26: // Link Secondary Details Base Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingDetails2)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 27: // Link Generic 1
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric1)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 28: // Link Generic 2
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric2)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 29: // Link Coverart-Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkImg)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 30: // Link Persons Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersons)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 31: // Link Titles-Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTitles)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 32: // Link Certification-Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkCertification)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 33: // Link Comment-Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkComment)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 34: // Link Synopsis/Description-Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkSyn)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 35: // Link MultiPosters - Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiPosters)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiPosters)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 36: // Link Photos - Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPhotos)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPhotos)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 37: // Link PersonImages - Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersonImages)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersonImages)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 38: // Link MultiFanart - Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiFanart)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiFanart)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 39: // Link Trailer - Secondary Page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTrailer)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerIndex)._Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTrailer)._Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 40: // MultiPosters
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersPage)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndMultiPosters)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersIndex)._Value;
          break;
        case 41: // Photos
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosPage)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndPhotos)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosIndex)._Value;
          break;
        case 42: // PersonImages
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesPage)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndPersonImages)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesIndex)._Value;
          break;
        case 43: // MultiFanart
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartPage)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndMultiFanart)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartIndex)._Value;
          break;
        case 44: // Trailer
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerPage)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTrailer)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerRegExp)._Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerIndex)._Value;
          break;
        case 45: // Runtime
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRuntimePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRuntime)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRuntimeIndex)._Value;
          break;
        case 46: // Collection
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCollection)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionIndex)._Value;
          break;

        case 47: // Collection Image URL
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionImageURLPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCollectionImageURL)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionImageURLIndex)._Value;
          break;

        default:
          URLpage.Text = "";
          textDReplace.Text = "";
          textDReplaceWith.Text = "";
          TextKeyStartD.Text = "";
          TextKeyStopD.Text = "";
          Index.Text = "";
          break;

      }

      if (lblComplement.Visible == true)
      {
        chkActorRoles.Visible = true;
        chkActorRoles.Enabled = true;
      }

      if (cb_ParamDetail.SelectedIndex > 0)
      {
        textDReplace.Visible = true;
        textDReplaceWith.Visible = true;
        labelDReplace.Visible = true;
        labelDReplaceWith.Visible = true;
        //btResetDetail.Visible = true;
      }
      else
      {
        textDReplace.Text = "";
        textDReplaceWith.Text = "";
        textDReplace.Visible = false;
        textDReplaceWith.Visible = false;
        labelDReplace.Visible = false;
        labelDReplaceWith.Visible = false;
        //btResetDetail.Visible = false;
      }

      // load selected Page into webpage window
      textBodyDetail.Text = this.LoadPage(URLpage.Text);

      // Mark Selection, if it's valid
      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1

      GLbBlock = false;
    }

    private void textKeyStartD_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      int iStart;
      int iEnd;
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value = TextKeyStartD.Text;
          if (TextKeyStartD.Text.Length > 0)
          {
            iStart = BodyDetail.IndexOf(TextKeyStartD.Text);
            //If the key was found early
            if (iStart > 0)
            {
              //If a key purpose has been set, it is used if no one takes the rest of the body
              iEnd = TextKeyStopD.Text != "" ? BodyDetail.IndexOf(TextKeyStopD.Text, iStart) : BodyDetail.Length;

              if (iEnd == -1)
                iEnd = BodyDetail.Length;

              //Cutting the Body
              iStart += TextKeyStartD.Text.Length;
              textBodyDetail.Text = BodyDetail.Substring(iStart, iEnd - iStart);

            }
            else
              textBodyDetail.Text = BodyDetail;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value = TextKeyStartD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value = TextKeyStartD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value = TextKeyStartD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value = TextKeyStartD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value = TextKeyStartD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value = TextKeyStartD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value = TextKeyStartD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value = TextKeyStartD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value = TextKeyStartD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value = TextKeyStartD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value = TextKeyStartD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value = TextKeyStartD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value = TextKeyStartD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value = TextKeyStartD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value = TextKeyStartD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value = TextKeyStartD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value = TextKeyStartD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value = TextKeyStartD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value = TextKeyStartD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value = TextKeyStartD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value = TextKeyStartD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Value = TextKeyStartD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value = TextKeyStartD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value = TextKeyStartD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value = TextKeyStartD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value = TextKeyStartD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value = TextKeyStartD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value = TextKeyStartD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value = TextKeyStartD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value = TextKeyStartD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value = TextKeyStartD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value = TextKeyStartD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value = TextKeyStartD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value = TextKeyStartD.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Value = TextKeyStartD.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Value = TextKeyStartD.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Value = TextKeyStartD.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Value = TextKeyStartD.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Value = TextKeyStartD.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Value = TextKeyStartD.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Value = TextKeyStartD.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Value = TextKeyStartD.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Value = TextKeyStartD.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Value = TextKeyStartD.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Value = TextKeyStartD.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Value = TextKeyStartD.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Value = TextKeyStartD.Text;
          break;

        default:
          TextKeyStartD.Text = "";
          break;
      }

      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1
    }

    private void TextKeyStopD_TextChanged(object sender, EventArgs e)
    {
      int iStart;
      int iEnd;
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value = TextKeyStartD.Text;
          if (TextKeyStartD.Text.Length > 0)
          {
            iStart = BodyDetail.IndexOf(TextKeyStartD.Text);
            //Si la clé de début a été trouvé
            if (iStart > 0)
            {
              //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
              iEnd = TextKeyStopD.Text != "" ? BodyDetail.IndexOf(TextKeyStopD.Text, iStart) : BodyDetail.Length;

              if (iEnd == -1)
                iEnd = BodyDetail.Length;

              //Découpage du body
              iStart += TextKeyStartD.Text.Length;
              BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
              textBodyDetail.Text = BodyDetail;

            }
            else
              textBodyDetail.Text = BodyDetail;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndOTitle)._Value = TextKeyStopD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTTitle)._Value = TextKeyStopD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndImg)._Value = TextKeyStopD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate)._Value = TextKeyStopD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate2)._Value = TextKeyStopD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value = TextKeyStopD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value = TextKeyStopD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndWriter)._Value = TextKeyStopD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCredits)._Value = TextKeyStopD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCountry)._Value = TextKeyStopD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGenre)._Value = TextKeyStopD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndYear)._Value = TextKeyStopD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Id)._Value = TextKeyStopD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value = TextKeyStopD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndComment)._Value = TextKeyStopD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLanguage)._Value = TextKeyStopD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTagline)._Value = TextKeyStopD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCertification)._Value = TextKeyStopD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndStudio)._Value = TextKeyStopD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndEdition)._Value = TextKeyStopD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Rank)._Value = TextKeyStopD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTMDB_Id)._Value = TextKeyStopD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric1)._Value = TextKeyStopD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric2)._Value = TextKeyStopD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric3)._Value = TextKeyStopD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value = TextKeyStopD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value = TextKeyStopD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value = TextKeyStopD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value = TextKeyStopD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value = TextKeyStopD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value = TextKeyStopD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value = TextKeyStopD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value = TextKeyStopD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value = TextKeyStopD.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiPosters)._Value = TextKeyStopD.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPhotos)._Value = TextKeyStopD.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersonImages)._Value = TextKeyStopD.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkMultiFanart)._Value = TextKeyStopD.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTrailer)._Value = TextKeyStopD.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndMultiPosters)._Value = TextKeyStopD.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndPhotos)._Value = TextKeyStopD.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndPersonImages)._Value = TextKeyStopD.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndMultiFanart)._Value = TextKeyStopD.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTrailer)._Value = TextKeyStopD.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRuntime)._Value = TextKeyStopD.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCollection)._Value = TextKeyStopD.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCollectionImageURL)._Value = TextKeyStopD.Text;
          break;
        default:
          TextKeyStopD.Text = "";
          break;
      }

      if (cb_ParamDetail.SelectedIndex > 0)
      {
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text);
      }
    }

    private void buttonFind_Click(object sender, EventArgs e)
    {
      int iLength = 0;
      int i = GrabUtil.FindPosition(textBodyDetail.Text, textFind.Text, GLiSearchD, ref iLength, true, false, cbIgnoreCaseDetails.Checked);
      // int i = textBodyDetail.Find(textFind.Text, GLiSearchD, RichTextBoxFinds.None);
      if (i > 0)
      {
        textBodyDetail.Select(i, iLength);
        // textBodyDetail.Select(i, textFind.Text.Length);
        GLiSearchD = i + iLength;
      }
      else
        GLiSearchD = 0;
    }

    private void textBodyDetail_Click(object sender, EventArgs e)
    {
      GLiSearchD = ((RichTextBox)sender).SelectionStart;
    }

    private void textBodyDetail_SelectionChanged(object sender, EventArgs e)
    {
      if (GLbBlockSelect)
        return;

      if (textBodyDetail.SelectedText.Trim().Length > 0)
      {
        int nb = 0;
        int i = 0;
        int iLength = 0;
        // i = textBodyDetail.Find(textBodyDetail.SelectedText, 0, RichTextBoxFinds.NoHighlight);
        i = GrabUtil.FindPosition(textBodyDetail.Text, textBodyDetail.SelectedText, i, ref iLength, true, false);
        while (i > 0)
        {
          nb++;
          // i = textBodyDetail.Find(textBodyDetail.SelectedText, i + textBodyDetail.SelectedText.Length, RichTextBoxFinds.NoHighlight);
          i = GrabUtil.FindPosition(textBodyDetail.Text, textBodyDetail.SelectedText, i + iLength, ref iLength, true, false);
        }
        label10.Text = nb + " match found";
      }
    }

    private void dataGridViewSearchResults_SelectionChanged(object sender, EventArgs e)
    {
      int rowSelected = dataGridViewSearchResults.Rows.GetFirstRow(DataGridViewElementStates.Selected);
      if (rowSelected == -1)
        return;
      if (dataGridViewSearchResults["ResultColumn2", rowSelected].Value == null)
        return;
      if (rowSelected >= 0 && dataGridViewSearchResults["ResultColumn2", rowSelected].Value.ToString() == "+++")
        button_GoDetailPage.Text = "Display Next Page";
      else if (rowSelected >= 0 && dataGridViewSearchResults["ResultColumn2", rowSelected].Value.ToString() == "---")
        button_GoDetailPage.Text = "Display Previous Page";
      else
        button_GoDetailPage.Text = "Use with Detail Page";
      button_GoDetailPage.Enabled = rowSelected >= 0;
      //this.dataGridViewSearchResults.Rows[rowSelected].Selected = false;
      //this.dataGridViewSearchResults.Rows[rowSelected - 1].Selected = true;
    }

    private void dataGridViewSearchResults_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      button_GoDetailPage_Click(sender, e);
    }

    private void button_GoDetailPage_Click(object sender, EventArgs e)
    {
      int rowSelected = dataGridViewSearchResults.Rows.GetFirstRow(DataGridViewElementStates.Selected);
      if (rowSelected >= 0)
      {
        switch (dataGridViewSearchResults["ResultColumn2", rowSelected].Value.ToString())
        {
          case "+++":
            {
              textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) + Convert.ToInt16(textStepPage.Text));
              Grabber_URLClass.IMDBUrl wurl;
              wurl = (Grabber_URLClass.IMDBUrl)listUrl[rowSelected];
              Load_Preview(true); // always ask - was false in earlier versions ...
              button_GoDetailPage.Enabled = false;
            }
            break;
          case "---":
            {
              this.textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) - Convert.ToInt16(textStepPage.Text));
              Grabber_URLClass.IMDBUrl wurl;
              wurl = (Grabber_URLClass.IMDBUrl)listUrl[rowSelected];
              Load_Preview(true); // always ask - gives all results - was "false" in earlier versions
              button_GoDetailPage.Enabled = false;
            }
            break;
          default:
            {
              File.Delete(textConfig.Text + ".tmp");
              Grabber_URLClass.IMDBUrl wurl;
              wurl = (Grabber_URLClass.IMDBUrl)listUrl[rowSelected];
              TextURLDetail.Text = wurl.URL;
              EventArgs ea = new EventArgs();
              ButtonLoad_Click(Button_Load_URL, ea);
              tabControl1.SelectTab(2);
            }
            break;
        }
      }
    }

    private void buttonPreview_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }

      Stopwatch watch = new Stopwatch();
      string totalruntime = string.Empty;
      watch.Reset();
      watch.Start();
      textPreview.Clear();
      pictureBoxPreviewCollection.ImageLocation = "";
      pictureBoxPreviewCover.ImageLocation = "";
      labelImageSize.Text = "";

      SaveXml(textConfig.Text + ".tmp");
      Grabber_URLClass grab = new Grabber_URLClass();
      string[] Result = new string[80];

      try // http://akas.imdb.com/title/tt0133093/
      {
        Result = grab.GetDetail(TextURLDetail.Text, Environment.GetEnvironmentVariable("TEMP"), textConfig.Text + ".tmp", true, string.Empty, string.Empty, string.Empty, string.Empty, null);
      }
      catch (Exception ex)
      {
        DialogResult dlgResult = DialogResult.None;
        dlgResult = MessageBox.Show("An error ocurred - check your definitions!\n Exception: " + ex + ", Stacktrace: " + ex.StackTrace, "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK)
        {
        }
      }
      watch.Stop();
      totalruntime = "Total Runtime: " + watch.ElapsedMilliseconds + " ms.";

      string mapped;
      for (int i = 0; i < Result.Length; i++)
      {
        textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
        mapped = i > 39 ? " (mapped)" : "";

        switch (i)
        {
          case 0:
            textPreview.SelectedText += "(" + i + ") " + "Original Title" + mapped + Environment.NewLine;
            break;
          case 40:
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
            textPreview.SelectedText += "MAPPED OUTPUT FIELDS:" + Environment.NewLine;
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
            textPreview.SelectedText += "(" + i + ") " + "Original Title" + mapped + Environment.NewLine;
            break;
          case 1:
          case 41:
            textPreview.SelectedText += "(" + i + ") " + "Translated Title" + mapped + Environment.NewLine;
            break;
          case 2:
          case 42:
            textPreview.SelectedText += "(" + i + ") " + "Cover" + mapped + Environment.NewLine;
            if (i > 39) // only show image once !
            {
              try
              {
                pictureBoxPreviewCover.ImageLocation = Result[i];

                // Create new FileInfo object and get the Length.
                FileInfo f = new FileInfo(Result[i]);
                //long s1 = f.Length;
                //labelImageSize.Text = s1.ToString();
                labelImageSize.Text = ByteString(f.Length);
              }
              catch (Exception)
              {
                labelImageSize.Text = "n/a";
                //MessageBox.Show("An error ocurred in image preview - check your config.\n" + ex.Message + "\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
              }
            }
            break;
          case 3:
          case 43:
            textPreview.SelectedText += "(" + i + ") " + "Description" + mapped + Environment.NewLine;
            break;
          case 4:
          case 44:
            textPreview.SelectedText += "(" + i + ") " + "Rating" + mapped + Environment.NewLine;
            break;
          case 5:
          case 45:
            textPreview.SelectedText += "(" + i + ") " + "Actors" + mapped + Environment.NewLine;
            break;
          case 6:
          case 46:
            textPreview.SelectedText += "(" + i + ") " + "Director" + mapped + Environment.NewLine;
            break;
          case 7:
          case 47:
            textPreview.SelectedText += "(" + i + ") " + "Producer" + mapped + Environment.NewLine;
            break;
          case 8:
          case 48:
            textPreview.SelectedText += "(" + i + ") " + "Year" + mapped + Environment.NewLine;
            break;
          case 9:
          case 49:
            textPreview.SelectedText += "(" + i + ") " + "Country" + mapped + Environment.NewLine;
            break;
          case 10:
          case 50:
            textPreview.SelectedText += "(" + i + ") " + "Category" + mapped + Environment.NewLine;
            break;
          case 11:
          case 51:
            textPreview.SelectedText += "(" + i + ") " + "URL" + mapped + Environment.NewLine;
            break;
          case 12:
          case 52:
            textPreview.SelectedText += "(" + i + ") " + "Image" + mapped + Environment.NewLine;
            break;
          case 13:
          case 53:
            textPreview.SelectedText += "(" + i + ") " + "Writer" + mapped + Environment.NewLine;
            break;
          case 14:
          case 54:
            textPreview.SelectedText += "(" + i + ") " + "Comment" + mapped + Environment.NewLine;
            break;
          case 15:
          case 55:
            textPreview.SelectedText += "(" + i + ") " + "Language" + mapped + Environment.NewLine;
            break;
          case 16:
          case 56:
            textPreview.SelectedText += "(" + i + ") " + "Tagline" + mapped + Environment.NewLine;
            break;
          case 17:
          case 57:
            textPreview.SelectedText += "(" + i + ") " + "Certification" + mapped + Environment.NewLine;
            break;
          case 18:
          case 58:
            textPreview.SelectedText += "(" + i + ") " + "IMDB_Id" + mapped + Environment.NewLine;
            break;
          case 19:
          case 59:
            textPreview.SelectedText += "(" + i + ") " + "IMDB_Rank" + mapped + Environment.NewLine;
            break;
          case 20:
          case 60:
            textPreview.SelectedText += "(" + i + ") " + "Studio" + mapped + Environment.NewLine;
            break;
          case 21:
          case 61:
            textPreview.SelectedText += "(" + i + ") " + "Edition" + mapped + Environment.NewLine;
            break;
          case 22:
          case 62:
            textPreview.SelectedText += "(" + i + ") " + "Fanart" + mapped + Environment.NewLine;
            break;
          case 23:
          case 63:
            textPreview.SelectedText += "(" + i + ") " + "Generic 1" + mapped + Environment.NewLine;
            break;
          case 24:
          case 64:
            textPreview.SelectedText += "(" + i + ") " + "Generic 2" + mapped + Environment.NewLine;
            break;
          case 25:
          case 65:
            textPreview.SelectedText += "(" + i + ") " + "Generic 3" + mapped + Environment.NewLine;
            break;
          case 26:
          case 66:
            textPreview.SelectedText += "(" + i + ") " + "Names: Countries for 'Translated Title'" + mapped + Environment.NewLine;
            break;
          case 27:
          case 67:
            textPreview.SelectedText += "(" + i + ") " + "Values: Countries for 'Translated Title'" + mapped + Environment.NewLine;
            break;
          case 28:
          case 68:
            textPreview.SelectedText += "(" + i + ") " + "Names: Countries for 'Certification'" + mapped + Environment.NewLine;
            break;
          case 29:
          case 69:
            textPreview.SelectedText += "(" + i + ") " + "Values: Countries for 'Certification'" + mapped + Environment.NewLine;
            break;
          case 30:
          case 70:
            textPreview.SelectedText += "(" + i + ") " + "Values: MultiPosters'" + mapped + Environment.NewLine;
            break;
          case 31:
          case 71:
            textPreview.SelectedText += "(" + i + ") " + "Values: Photos'" + mapped + Environment.NewLine;
            break;
          case 32:
          case 72:
            textPreview.SelectedText += "(" + i + ") " + "Values: PersonImages'" + mapped + Environment.NewLine;
            break;
          case 33:
          case 73:
            textPreview.SelectedText += "(" + i + ") " + "Values: MultiFanart'" + mapped + Environment.NewLine;
            break;
          case 34:
          case 74:
            textPreview.SelectedText += "(" + i + ") " + "Values: Trailer'" + mapped + Environment.NewLine;
            break;
          case 35:
          case 75:
            textPreview.SelectedText += "(" + i + ") " + "TMDB_Id'" + mapped + Environment.NewLine;
            break;
          case 36:
          case 76:
            textPreview.SelectedText += "(" + i + ") " + "Runtime'" + mapped + Environment.NewLine;
            break;
          case 37:
          case 77:
            textPreview.SelectedText += "(" + i + ") " + "Collection'" + mapped + Environment.NewLine;
            break;
          case 38:
          case 78:
            textPreview.SelectedText += "(" + i + ") " + "CollectionImageURL'" + mapped + Environment.NewLine;
            try
            {
              pictureBoxPreviewCollection.ImageLocation = Path.Combine(Path.GetDirectoryName(Result[i]), Path.GetFileName(Result[i]));
              //FileInfo f = new FileInfo(Result[i]);
              //labelImageSize.Text = this.ByteString(f.Length);
            }
            catch (Exception)
            {
              // labelImageSize.Text = "n/a";
            }
            break;
          case 39:
          case 79:
            textPreview.SelectedText += "(" + i + ") " + "Picture URL'" + mapped + Environment.NewLine;
            break;
          default:
            textPreview.SelectedText += "(" + (i) + ") " + "Mapping Output Field '" + (i - 40) + "'" + mapped + Environment.NewLine;
            break;
        }
        if (i <= 80) // Changed to support new fields...
          textPreview.AppendText(Result[i] + Environment.NewLine);
      }
      // List of Grab Pages used for Grabber results:
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
      textPreview.SelectedText += "*** Infos about used Grab Pages ***" + Environment.NewLine;
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("Base Page:" + TimeBodyDetail + Environment.NewLine);
      textPreview.AppendText(URLBodyDetail + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Gateway:" + TimeBodyDetail2 + Environment.NewLine);
      textPreview.AppendText(URLBodyDetail2 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Generic 1:" + TimeBodyLinkGeneric1 + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkGeneric1 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Generic 2:" + TimeBodyLinkGeneric2 + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkGeneric2 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Cover:" + TimeBodyLinkImg + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkImg + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Persons:" + TimeBodyLinkPersons + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkPersons + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Title:" + TimeBodyLinkTitles + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkTitles + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Certification:" + TimeBodyLinkCertification + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkCertification + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Comment:" + TimeBodyLinkComment + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkComment + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Description:" + TimeBodyLinkSyn + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkSyn + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection MultiPosters:" + TimeBodyLinkMultiPosters + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkMultiPosters + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Photos:" + TimeBodyLinkPhotos + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkPhotos + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection PersonImages:" + TimeBodyLinkPersonImages + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkPersonImages + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection MultiFanart:" + TimeBodyLinkMultiFanart + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkMultiFanart + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Trailer:" + TimeBodyLinkTrailer + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkTrailer + Environment.NewLine);

      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
      textPreview.SelectedText += totalruntime + Environment.NewLine;

      File.Delete(textConfig.Text + ".tmp");
    }

    private void textComplement_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 4:
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value = textComplement.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleRegExp)._Value = textComplement.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseRegExp)._Value = textComplement.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductRegExp)._Value = textComplement.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterRegExp)._Value = textComplement.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value = textComplement.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryRegExp)._Value = textComplement.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenreRegExp)._Value = textComplement.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentRegExp)._Value = textComplement.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageRegExp)._Value = textComplement.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationRegExp)._Value = textComplement.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStudioRegExp)._Value = textComplement.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1RegExp)._Value = textComplement.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2RegExp)._Value = textComplement.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3RegExp)._Value = textComplement.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersRegExp)._Value = textComplement.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosRegExp)._Value = textComplement.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesRegExp)._Value = textComplement.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartRegExp)._Value = textComplement.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerRegExp)._Value = textComplement.Text;
          break;

        default:
          break;
      }
    }

    private void textMaxItems_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value = textMaxItems.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value = textMaxItems.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value = textMaxItems.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value = textMaxItems.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value = textMaxItems.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value = textMaxItems.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2MaxItems)._Value = textMaxItems.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3MaxItems)._Value = textMaxItems.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersMaxItems)._Value = textMaxItems.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosMaxItems)._Value = textMaxItems.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesMaxItems)._Value = textMaxItems.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartMaxItems)._Value = textMaxItems.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerMaxItems)._Value = textMaxItems.Text;
          break;

        default:
          break;
      }
    }

    private void chkACTORROLES_CheckedChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 9:
          this.xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = chkActorRoles.Checked ? "true" : "false";
          break;
        default:
          break;
      }
    }

    private void textLanguages_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value = textLanguages.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value = textLanguages.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Language)._Value = textLanguages.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Language)._Value = textLanguages.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Language)._Value = textLanguages.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersLanguage)._Value = textLanguages.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosLanguage)._Value = textLanguages.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesLanguage)._Value = textLanguages.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartLanguage)._Value = textLanguages.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerLanguage)._Value = textLanguages.Text;
          break;

        default:
          break;
      }
    }

    private void textLanguagesAll_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguageAll)._Value = textLanguagesAll.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguageAll)._Value = textLanguagesAll.Text;
          break;
        default:
          break;
      }
    }

    private void radioButtonFR_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = FrenchCulture;
      ApplyCulture(FrenchCulture);
    }

    private void ApplyCulture(CultureInfo culture)
    {
      // Applies culture to current Thread.
      System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

      // Create a resource manager for this Form and determine its fields via reflection.
      ComponentResourceManager resources = new ComponentResourceManager(this.GetType());
      FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Instance |
          BindingFlags.DeclaredOnly | BindingFlags.NonPublic);

      // Call SuspendLayout for Form and all fields derived from Control, so assignment of 
      //   localized text doesn't change layout immediately.
      this.SuspendLayout();
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.IsSubclassOf(typeof(Control)))
        {
          fieldInfos[index].FieldType.InvokeMember("SuspendLayout",
              BindingFlags.InvokeMethod, null,
              fieldInfos[index].GetValue(this), null);
        }
      }

      // If available, assign localized text to Form and fields with Text property.
      String text = resources.GetString("$this.Text");
      if (text != null)
        this.Text = text;
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.GetProperty("Text", typeof(String)) != null)
        {
          text = resources.GetString(fieldInfos[index].Name + ".Text");
          if (text != null)
          {
            fieldInfos[index].FieldType.InvokeMember("Text",
                BindingFlags.SetProperty, null,
                fieldInfos[index].GetValue(this), new object[] { text });
          }
        }
      }

      // Call ResumeLayout for Form and all fields derived from Control to resume layout logic.
      // Call PerformLayout, so layout changes due to assignment of localized text are performed.
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.IsSubclassOf(typeof(Control)))
        {
          fieldInfos[index].FieldType.InvokeMember("ResumeLayout",
                  BindingFlags.InvokeMethod, null,
                  fieldInfos[index].GetValue(this), new object[] { false });
        }
      }
      this.ResumeLayout(true);

    }

    private void radioButtonEN_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = EnglishCulture;
      ApplyCulture(EnglishCulture);
    }

    private void textDReplace_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param1 = textDReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param1 = textDReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param1 = textDReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param1 = textDReplace.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param1 = textDReplace.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1 = textDReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1 = textDReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param1 = textDReplace.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param1 = textDReplace.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param1 = textDReplace.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param1 = textDReplace.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param1 = textDReplace.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param1 = textDReplace.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1 = textDReplace.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param1 = textDReplace.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param1 = textDReplace.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param1 = textDReplace.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param1 = textDReplace.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param1 = textDReplace.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param1 = textDReplace.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param1 = textDReplace.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param1 = textDReplace.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param1 = textDReplace.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param1 = textDReplace.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param1 = textDReplace.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1 = textDReplace.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1 = textDReplace.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1 = textDReplace.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1 = textDReplace.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1 = textDReplace.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1 = textDReplace.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1 = textDReplace.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1 = textDReplace.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1 = textDReplace.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param1 = textDReplace.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param1 = textDReplace.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param1 = textDReplace.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param1 = textDReplace.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param1 = textDReplace.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Param1 = textDReplace.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Param1 = textDReplace.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Param1 = textDReplace.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Param1 = textDReplace.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Param1 = textDReplace.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Param1 = textDReplace.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Param1 = textDReplace.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Param1 = textDReplace.Text;
          break;
        default:
          break;

      }
      //if (cbParamDetail.SelectedIndex > 0 && textDReplaceWith.Text.Length > 0)
      //    textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));

    }

    private void textDReplaceWith_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param2 = textDReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param2 = textDReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param2 = textDReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param2 = textDReplaceWith.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param2 = textDReplaceWith.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2 = textDReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2 = textDReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param2 = textDReplaceWith.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param2 = textDReplaceWith.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param2 = textDReplaceWith.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param2 = textDReplaceWith.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param2 = textDReplaceWith.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param2 = textDReplaceWith.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2 = textDReplaceWith.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param2 = textDReplaceWith.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param2 = textDReplaceWith.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param2 = textDReplaceWith.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param2 = textDReplaceWith.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param2 = textDReplaceWith.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param2 = textDReplaceWith.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param2 = textDReplaceWith.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param2 = textDReplaceWith.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param2 = textDReplaceWith.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param2 = textDReplaceWith.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param2 = textDReplaceWith.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2 = textDReplaceWith.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2 = textDReplaceWith.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2 = textDReplaceWith.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2 = textDReplaceWith.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2 = textDReplaceWith.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2 = textDReplaceWith.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2 = textDReplaceWith.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2 = textDReplaceWith.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2 = textDReplaceWith.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiPosters)._Param2 = textDReplaceWith.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPhotos)._Param2 = textDReplaceWith.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersonImages)._Param2 = textDReplaceWith.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkMultiFanart)._Param2 = textDReplaceWith.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTrailer)._Param2 = textDReplaceWith.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiPosters)._Param2 = textDReplaceWith.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPhotos)._Param2 = textDReplaceWith.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartPersonImages)._Param2 = textDReplaceWith.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartMultiFanart)._Param2 = textDReplaceWith.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTrailer)._Param2 = textDReplaceWith.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Param2 = textDReplaceWith.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Param2 = textDReplaceWith.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Param2 = textDReplaceWith.Text;
          break;

        default:
          break;
      }
      //if (cbParamDetail.SelectedIndex > 0)
      //    textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));
    }

    private void textReplace_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_Parameter.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param1 = textReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param1 = textReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param1 = textReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param1 = textReplace.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartID)._Param1 = textReplace.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartOptions)._Param1 = textReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartAkas)._Param1 = textReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartThumb)._Param1 = textReplace.Text;
          break;
        default:
          break;

      }
      //if (comboBox1.SelectedIndex > 0 && textReplaceWith.Text.Length > 0)
      //    textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);

    }

    private void textReplaceWith_TextChanged(object sender, EventArgs e)
    {
      switch (cb_Parameter.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param2 = textReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param2 = textReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param2 = textReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param2 = textReplaceWith.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartID)._Param2 = textReplaceWith.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartOptions)._Param2 = textReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartAkas)._Param2 = textReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartThumb)._Param2 = textReplaceWith.Text;
          break;
        default:
          break;

      }
      //if (comboBox1.SelectedIndex > 0)
      //    textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);
    }
    
    private int ExtractBody(string Body, string ParamStart)
    {
      string strStart = string.Empty;
      switch (ParamStart)
      {
        case "Original Title":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value;
          break;
        case "Translated Title":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value;
          break;
        case "URL cover":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value;
          break;
        case "Rate 1":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value;
          break;
        case "Rate 2":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value;
          break;
        case "Synopsys":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value;
          break;
        case "Director":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value;
          break;
        case "Producer":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value;
          break;
        case "Actors":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value;
          break;
        case "Country":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value;
          break;
        case "Category":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value;
          break;
        case "Year":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value;
          break;
        // New Added
        case "Comment":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value;
          break;
        case "Language":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value;
          break;
        case "Tagline":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value;
          break;
        case "Certification":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value;
          break;
        case "Writer":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value;
          break;
        case "Studio":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value;
          break;
        case "Edition":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value;
          break;
        case "IMDB_Rank":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value;
          break;
        case "IMDB_Id":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value;
          break;
        case "TMDB_Id":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Value;
          break;
        case "Runtime":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRuntime)._Value;
          break;
        case "Collection":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollection)._Value;
          break;
        case "CollectionImageURL":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCollectionImageURL)._Value;
          break;
        case "Generic Field 1":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value;
          break;
        case "Generic Field 2":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value;
          break;
        case "Generic Field 3":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value;
          break;
        //case "TitlesURL":
        //  strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
        //  break;
        default:
          break;
      }
      return strStart.Length > 0 ? Body.IndexOf(strStart) : 0;
    }

    private void Index_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyOTitleIndex)._Value = Index.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleIndex)._Value = Index.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyImgIndex)._Value = Index.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRateIndex)._Value = Index.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Index)._Value = Index.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value = Index.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value = Index.Text;
          break;
        case 8: // writer
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterIndex)._Value = Index.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsIndex)._Value = Index.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryIndex)._Value = Index.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenreIndex)._Value = Index.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyYearIndex)._Value = Index.Text;
          break;
        case 13: // added for IMDB_Id
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdIndex)._Value = Index.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value = Index.Text;
          break;
        case 15: // added for comments
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentIndex)._Value = Index.Text;
          break;
        case 16: // added for languages
          xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageIndex)._Value = Index.Text;
          break;
        case 17: // added for tagline
          xmlConf.find(xmlConf.listDetail, TagName.KeyTaglineIndex)._Value = Index.Text;
          break;
        case 18: // added for certification 
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationIndex)._Value = Index.Text;
          break;
        case 19: // added for Studio
          xmlConf.find(xmlConf.listDetail, TagName.KeyStudioIndex)._Value = Index.Text;
          break;
        case 20: // added for Edition
          xmlConf.find(xmlConf.listDetail, TagName.KeyEditionIndex)._Value = Index.Text;
          break;
        case 21: // added for IMDB_Rank
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankIndex)._Value = Index.Text;
          break;
        case 22: // added for TMDB_Id
          xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdIndex)._Value = Index.Text;
          break;
        case 23: // added for Generic Field 1
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Index)._Value = Index.Text;
          break;
        case 24: // added for Generic Field 2
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Index)._Value = Index.Text;
          break;
        case 25: // added for Generic Field 3
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Index)._Value = Index.Text;
          break;
        case 26: // added for details base page 
          xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value = Index.Text;
          break;
        case 27: // added for secondary Generic 1 linkpage
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value = Index.Text;
          break;
        case 28: // added for secondary Generic 2 linkpage
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value = Index.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value = Index.Text;
          break;
        case 30: // added for secondary persons page
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value = Index.Text;
          break;
        case 31: // added for secondary titles page 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value = Index.Text;
          break;
        case 32: // added for secondary certification 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value = Index.Text;
          break;
        case 33: // added for secondary comment 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value = Index.Text;
          break;
        case 34: // added for secondary Synopsis/description 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value = Index.Text;
          break;

        case 35: // added for secondary MultiPosters
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersIndex)._Value = Index.Text;
          break;
        case 36: // added for secondary Photos
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosIndex)._Value = Index.Text;
          break;
        case 37: // added for secondary PersonImages
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesIndex)._Value = Index.Text;
          break;
        case 38: // added for secondary MultiFanart
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartIndex)._Value = Index.Text;
          break;
        case 39: // added for secondary Trailer
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerIndex)._Value = Index.Text;
          break;

        case 40: // added for MultiPosters
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersIndex)._Value = Index.Text;
          break;
        case 41: // added for Photos
          xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosIndex)._Value = Index.Text;
          break;
        case 42: // added for PersonImages
          xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesIndex)._Value = Index.Text;
          break;
        case 43: // added for MultiFanart
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartIndex)._Value = Index.Text;
          break;
        case 44: // added for Trailer
          xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerIndex)._Value = Index.Text;
          break;
        case 45: // added for Runtime (by web)
          xmlConf.find(xmlConf.listDetail, TagName.KeyRuntimeIndex)._Value = Index.Text;
          break;
        case 46: // added for Collection
          xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionIndex)._Value = Index.Text;
          break;
        case 47: // added for Collection Image URL
          xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionImageURLIndex)._Value = Index.Text;
          break;
        default:
          break;
      }
    }

    private void URLpage_SelectedIndexChanged(object sender, EventArgs e)
    {
      //Base
      //URL Gateway
      //URL Redirection Generic1
      //URL Redirection Generic2
      //URL Redirection Cover
      //URL Redirection Persons
      //URL Redirection Title
      //URL Redirection Certification
      //URL Redirection Comment
      //URL Redirection Description
      //NFOpath

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyOTitlePage)._Value = URLpage.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitlePage)._Value = URLpage.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyImgPage)._Value = URLpage.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRatePage)._Value = URLpage.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Page)._Value = URLpage.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealisePage)._Value = URLpage.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductPage)._Value = URLpage.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterPage)._Value = URLpage.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsPage)._Value = URLpage.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryPage)._Value = URLpage.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenrePage)._Value = URLpage.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyYearPage)._Value = URLpage.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdPage)._Value = URLpage.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeySynPage)._Value = URLpage.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentPage)._Value = URLpage.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLanguagePage)._Value = URLpage.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTaglinePage)._Value = URLpage.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationPage)._Value = URLpage.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStudioPage)._Value = URLpage.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEditionPage)._Value = URLpage.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankPage)._Value = URLpage.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdPage)._Value = URLpage.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Page)._Value = URLpage.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Page)._Value = URLpage.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Page)._Value = URLpage.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value = URLpage.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value = URLpage.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value = URLpage.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value = URLpage.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value = URLpage.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value = URLpage.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value = URLpage.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value = URLpage.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value = URLpage.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiPostersPage)._Value = URLpage.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPhotosPage)._Value = URLpage.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonImagesPage)._Value = URLpage.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkMultiFanartPage)._Value = URLpage.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTrailerPage)._Value = URLpage.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiPostersPage)._Value = URLpage.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPhotosPage)._Value = URLpage.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.listDetail, TagName.KeyPersonImagesPage)._Value = URLpage.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.listDetail, TagName.KeyMultiFanartPage)._Value = URLpage.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTrailerPage)._Value = URLpage.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRuntimePage)._Value = URLpage.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionPage)._Value = URLpage.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCollectionImageURLPage)._Value = URLpage.Text;
          break;

        default:
          break;
      }

      // if (!GLbBlock)
      textBodyDetail.Text = LoadPage(URLpage.Text);

      // Mark Selection, if valid
      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1

    }

    //private void btReset_Click(object sender, EventArgs e)
    //{
    //    textReplace.Text = "";
    //    textReplaceWith.Text = "";
    //    button_Load_Click(null, null);
    //    TextKeyStop_TextChanged(null, null);
    //}

    //private void btResetDetail_Click(object sender, EventArgs e)
    //{
    //    textDReplace.Text = "";
    //    textDReplaceWith.Text = "";
    //    ButtonLoad_Click(null, null);
    //    TextKeyStopD_TextChanged(null, null);
    //}

    private void buttonPrevParamDetail_Click(object sender, EventArgs e)
    {
      pictureBoxPreviewCover.ImageLocation = ""; // clear picture
      labelImageSize.Text = "";

      if (TextKeyStartD.Text.Length > 0 && TextKeyStopD.Text.Length > 0)
      {
        string allNames;
        string allRoles;

        string find = textDReplace.Text.Length > 0 ? GrabUtil.FindWithAction(this.textBodyDetail.Text, this.TextKeyStartD.Text, this.TextKeyStopD.Text, this.textDReplace.Text, this.textDReplaceWith.Text, this.textComplement.Text, this.textMaxItems.Text, this.textLanguages.Text, out allNames, out allRoles, this.chkActorRoles.Checked) : GrabUtil.Find(this.textBodyDetail.Text, this.TextKeyStartD.Text, this.TextKeyStopD.Text);

        MessageBox.Show(find, "Preview", MessageBoxButtons.OK);

        if (find.StartsWith("http"))
          textURLPreview.Text = find; // load Parameter in Sub URL field (to allow web launching etc.

        if (find.EndsWith("jpg") || find.EndsWith("png"))
        {
          try
          {
            pictureBoxPreviewCover.ImageLocation = find;
          }
          catch { }
        }
        try
        {
          // Create new FileInfo object and get the Length.
          FileInfo f = new FileInfo(find);
          labelImageSize.Text = ByteString(f.Length);
        }
        catch
        {
          try
          {
            string strTemp = Environment.GetEnvironmentVariable("TEMP") + @"\MFgrabpreview.jpg";
            try { File.Delete(strTemp); }
            catch (Exception) { }
            GrabUtil.DownloadImage(find, strTemp);
            FileInfo f = new FileInfo(strTemp);
            labelImageSize.Text = ByteString(f.Length);
            pictureBoxPreviewCover.ImageLocation = strTemp;
            //try { System.IO.File.Delete(strTemp); }
            //catch (Exception) { }
          }
          catch (Exception) { }
        }
      }

    }

    private string ByteString(long bytes)
    {
      double s = bytes;
      string[] format = new string[]
                  {
                      "{0} bytes", "{0} KB",  
                      "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB"
                  };

      int i = 0;

      while (i < format.Length && s >= 1024)
      {
        s = (long)(100 * s / 1024) / 100.0;
        i++;
      }
      return string.Format(format[i], s);
    }

    private void buttonPrevParam1_Click(object sender, EventArgs e)
    {
      if (TextKeyStart.Text.Length > 0 && TextKeyStop.Text.Length > 0)
      {
        string find;
        if (textReplace.Text.Length > 0 && textReplaceWith.Text.Length > 0)
          find = GrabUtil.FindWithAction(textBody.Text, TextKeyStart.Text, TextKeyStop.Text, textReplace.Text, textReplaceWith.Text);
        else
          find = GrabUtil.Find(textBody.Text, TextKeyStart.Text, TextKeyStop.Text);
        MessageBox.Show(find, "Preview", MessageBoxButtons.OK);
      }
    }

    private void btnLoadPreview_Click(object sender, EventArgs e)
    {
      string absoluteUri;
      int iStart;
      int iEnd;

      if (textURLPreview.Text.Length > 0)
      {

        textPreview.ResetText();

        BodyDetail = GrabUtil.GetPage(textURLPreview.Text, textEncoding.Text, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);

        if (xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length > 0)
        {
          iStart = BodyDetail.IndexOf(xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            iEnd = this.xmlConf.find(this.xmlConf.listDetail, TagName.KeyEndBody)._Value != "" ? this.BodyDetail.IndexOf(this.xmlConf.find(this.xmlConf.listDetail, TagName.KeyEndBody)._Value, iStart) : this.BodyDetail.Length;

            if (iEnd == -1) iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length;
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
            textBodyDetail.Text = BodyDetail;

          }
          else textBodyDetail.Text = BodyDetail;
        }
        else textBodyDetail.Text = BodyDetail;

      }
    }

    private void pictureBoxPreviewCover_Click(object sender, EventArgs e)
    {
      pictureBoxPreviewCover.ImageLocation = "";
      labelImageSize.Text = "";
    }

    private void textConfig_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        tabPageSearchPage.Enabled = false;
        tabPageDetailPage.Enabled = false;
      }
      else if (ExpertModeOn)
      {
        tabPageSearchPage.Enabled = true;
        tabPageDetailPage.Enabled = true;
      }
    }

    private void button_Load_File_Click(object sender, EventArgs e)
    {
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.Filter = "Web Files (*.htm)|*.htm";
      openFileDialog1.Title = "Load HTML File";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        TextURLDetail.Text = openFileDialog1.FileName;
        button_Load_Click(this, e);
      }

    }

    ///<summary>
    /// Liefert den Inhalt der Datei zurück.
    ///</summary>
    ///<param name="sFilename">Dateipfad</param>
    public string ReadFile(String sFilename)
    {
      string sContent = "";

      if (File.Exists(sFilename))
      {
        StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
        sContent = myFile.ReadToEnd();
        myFile.Close();
      }
      return sContent;
    }

    private void linkLabelMFwiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films");
    }

    private void buttonExpertMode_Click(object sender, EventArgs e)
    {
      if (ExpertModeOn)
      {
        ChangeVisibility(false);
        ExpertModeOn = false;
      }
      else
      {
        ChangeVisibility(true);
        ExpertModeOn = true;
      }
    }
    
    private void ChangeVisibility(bool visibleForExpert)
    {
      if (visibleForExpert == false)
      {
        buttonExpertMode.Text = "ExpertMode";
        textURLPrefix.Enabled = false;
        textURLPrefix.Visible = false;
        label2.Visible = false;
        textName.Enabled = false;
        textLanguage.Enabled = false;
        textVersion.Enabled = false;
        textType.Enabled = false;
        tabPageSearchPage.Enabled = false;
        tabPageDetailPage.Enabled = false;
        tabPageSearchPage.Visible = false;
        tabPageDetailPage.Visible = false;
        //tabPageSaveDetails = tabControl1.TabPages[2];
        //tabControl1.TabPages.Remove(tabPageSaveDetails);
        //tabPageSaveMovie = tabControl1.TabPages[1];
        //tabControl1.TabPages.Remove(tabPageSaveMovie);
      }
      else
      {
        buttonExpertMode.Text = "SimpleMode";
        textURLPrefix.Enabled = true;
        textURLPrefix.Visible = true;
        label2.Visible = true;
        textName.Enabled = true;
        textLanguage.Enabled = true;
        textVersion.Enabled = true;
        textType.Enabled = true;
        tabPageSearchPage.Enabled = true;
        tabPageDetailPage.Enabled = true;
        tabPageSearchPage.Visible = true;
        tabPageDetailPage.Visible = true;
        //tabControl1.TabPages.Add(tabPageSaveMovie);
        //tabControl1.TabPages.Add(tabPageSaveDetails);
      }
    }

    private void cbMaxActors_SelectedIndexChanged(object sender, EventArgs e)
    {
      //List<ListNode> list = xmlConf.listDetail;
      //string value = TagName.KeyCreditsMaxItems;
      //if (xmlConf.listDetail != null && TagName.KeyCreditsMaxItems != null && cbMaxActors.Text != null && cbMaxActors != null)
      if (cbMaxActors.SelectedIndex != -1)
        xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value = cbMaxActors.Text;
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      // ToDo: Adapt to new code
      // xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = chkGrabActorRoles.Checked ? "true" : "false";
    }

    private void cbMaxProducers_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxProducers.SelectedIndex != -1)
        xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value = cbMaxProducers.Text;
    }

    private void cbMaxDirectors_SelectedIndexChanged(object sender, EventArgs e)
    {
      //if (cbMaxDirectors.SelectedIndex != -1)
      //  xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value = cbMaxDirectors.Text;
    }

    private void cbMaxWriters_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxWriters.SelectedIndex != -1)
        xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value = cbMaxWriters.Text;
    }

    private void cbTtitlePreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value = cbTtitlePreferredLanguage.Text;
    }

    private void cbTtitleMaxTitles_SelectedIndexChanged(object sender, EventArgs e)
    {
      //if (cbTtitleMaxTitles.SelectedIndex != -1)
      //  xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value = cbTtitleMaxTitles.Text;
    }

    private void cbCertificationPreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbCertificationPreferredLanguage.SelectedIndex != -1)
        xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value = cbCertificationPreferredLanguage.Text;
    }

    private void InitMappingTable()
    {
      List<string> fields = Grabber_URLClass.FieldList();
      for (int i = 0; i < 40; i++)
      {
        Fields[i] = fields[i];
      }

      Column2.Items.Clear();
      Column2.Items.Add(""); // empty field to choose ....
      foreach (string field in Fields)
      {
        if (!string.IsNullOrEmpty(field) && !field.Contains("URL") && !field.Contains("All "))
          Column2.Items.Add(field);
      }
      for (int i = 0; i < 40; i++)
      {
        i = dataGridViewMapping.Rows.Add(); // add row for config
        dataGridViewMapping.Rows[i].Cells[0].Value = i;
        dataGridViewMapping.Rows[i].Cells[1].Value = Fields[i]; // adds field name
        dataGridViewMapping.Rows[i].Cells[2].Value = string.Empty;
        dataGridViewMapping.Rows[i].Cells[3].Value = false;
        dataGridViewMapping.Rows[i].Cells[4].Value = false;
        dataGridViewMapping.Rows[i].Cells[5].Value = false;
        dataGridViewMapping.Rows[i].Cells[6].Value = false;
        dataGridViewMapping.Rows[i].Cells[7].Value = false;
        dataGridViewMapping.Rows[i].Cells[8].Value = false; // apply translation
        dataGridViewMapping.Rows[i].Cells[9].Value = false; // apply replacements
      }

    }

    private void btnLoadDetailInWeb_Click(object sender, EventArgs e)
    {
      try
      {
        if (cbFileBasedReader.Checked)
        {
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "notepad.exe";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = "\"" + textURLPreview.Text + "\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater()) psi.Verb = "runas";
            p.StartInfo = psi;
            p.Start();
          }
        }
        else
        {
          //webBrowserPreview.Url = new Uri(textURLPreview.Text);
          //webBrowserPreview.Refresh();
          Process.Start(textURLPreview.Text);
        }
      }
      catch (Exception)
      {
        // throw;
      }
    }

    private void button_Load_Web_Click(object sender, EventArgs e)
    {
      if (!TextURL.Text.Contains("#Search#"))
      {
        MessageBox.Show("Please, replace search keyword by #Search# in URL !", "Error");
        return;
      }

      if (TextSearch.Text.Length == 0)
      {
        MessageBox.Show("Please, insert search keyword !", "Error");
        return;
      }
      if (TextURL.Text.Contains("#Page#") && (textPage.Text.Length == 0))
      {
        MessageBox.Show("Please, give the page number to load !", "Error");
        return;
      }

      // listPreview.Items.Clear();
      if (TextURL.Text.Length > 0)
      {
        if (TextURL.Text.StartsWith("http://") == false) TextURL.Text = "http://" + TextURL.Text;
        string strSearch = TextSearch.Text;
        strSearch = GrabUtil.CleanupSearch(strSearch, textSearchCleanup.Text);
        strSearch = GrabUtil.EncodeSearch(strSearch, textEncoding.Text);
        string wurl = TextURL.Text.Replace("#Search#", strSearch);
        wurl = wurl.Replace("#Page#", textPage.Text);
        try
        {
          //webBrowserPreview.Url = new Uri(wurl);
          //webBrowserPreview.Refresh();
          Process.Start(wurl);
        }
        catch (Exception) { throw; }
      }
    }

    private void button_NextMatch_Click(object sender, EventArgs e)
    {
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, true);
    }

    private void button_FirstMatch_Click(object sender, EventArgs e)
    {
      GLiSearchMatches = 0;
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, true);
    }

    private void dataGridViewSearchResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (e.ColumnIndex == dataGridViewSearchResults.Columns["ResultColumn6"].Index)
        {
          string Filepath = dataGridViewSearchResults["ResultColumn6", e.RowIndex].Value.ToString();
          if (!Filepath.Contains(".nfo"))
            Process.Start(Filepath);
          else
          {
            using (Process p = new Process())
            {
              ProcessStartInfo psi = new ProcessStartInfo();
              psi.FileName = "notepad.exe";
              psi.UseShellExecute = true;
              psi.WindowStyle = ProcessWindowStyle.Normal;
              psi.Arguments = "\"" + Filepath + "\"";
              psi.ErrorDialog = true;
              if (OSInfo.OSInfo.VistaOrLater()) psi.Verb = "runas";
              p.StartInfo = psi;
              p.Start();
            }
          }
        }
      }
      catch (Exception) { }
    }

    private void textBody_CursorChanged(object sender, EventArgs e)
    {
      labelSearchPosition.Text = textBody.SelectionStart < 0 ? textBody.SelectionStart.ToString() : "";
    }

    private void button_openMediafile_Click(object sender, EventArgs e)
    {
      openFileDialog1.FileName = !string.IsNullOrEmpty(this.TextSearch.Text) ? this.TextSearch.Text : String.Empty;
      if (TextSearch.Text.Contains("\\"))
        openFileDialog1.InitialDirectory = TextSearch.Text.Substring(0, TextSearch.Text.LastIndexOf("\\") + 1);
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.Filter = "All Files (*.*)|*.*";
      openFileDialog1.Title = "Open a media file for local grabbing";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        TextSearch.Text = openFileDialog1.FileName;
        // button_Load_Click(this, e);
      }
    }

    private void textBody_TextChanged(object sender, EventArgs e)
    {
      //try
      //{
      //  // Make a DataObject.
      //  DataObject data_object = new DataObject();

      //  // Add the data in various formats.
      //  data_object.SetData(DataFormats.Rtf, textBody.Rtf);
      //  data_object.SetData(DataFormats.Text, textBody.Text);

      //  // Copy data to the
      //  Clipboard.SetDataObject(data_object);
      //}
      //catch (Exception)
      //{
      //}
    }

    private void textBodyDetail_TextChanged(object sender, EventArgs e)
    {
      //try
      //{
      //  // Make a DataObject.
      //  DataObject data_object = new DataObject();

      //  // Add the data in various formats.
      //  data_object.SetData(DataFormats.Rtf, textBodyDetail.Rtf);
      //  data_object.SetData(DataFormats.Text, textBodyDetail.Text);

      //  // Copy data to the
      //  Clipboard.SetDataObject(data_object);
      //}
      //catch (Exception)
      //{
      //}
    }

    private void HideTabPage(TabPage tp)
    {
      if (tabControl1.TabPages.Contains(tp))
        tabControl1.TabPages.Remove(tp);
    }

    private void ShowTabPage(TabPage tp)
    {
      ShowTabPage(tp, tabControl1.TabPages.Count);
    }

    private void ShowTabPage(TabPage tp, int index)
    {
      if (tabControl1.TabPages.Contains(tp)) return;
      InsertTabPage(tp, index);
    }

    private void InsertTabPage(TabPage tabpage, int index)
    {
      if (index < 0 || index > tabControl1.TabCount)
        throw new ArgumentException("Index out of Range.");
      tabControl1.TabPages.Add(tabpage);
      if (index < tabControl1.TabCount - 1)
        do
        {
          SwapTabPages(tabpage, (tabControl1.TabPages[tabControl1.TabPages.IndexOf(tabpage) - 1]));
        }
        while (tabControl1.TabPages.IndexOf(tabpage) != index);
      tabControl1.SelectedTab = tabpage;
    }

    private void SwapTabPages(TabPage tp1, TabPage tp2)
    {
      if (tabControl1.TabPages.Contains(tp1) == false || tabControl1.TabPages.Contains(tp2) == false)
        throw new ArgumentException("TabPages must be in the TabControls TabPageCollection.");

      int index1 = tabControl1.TabPages.IndexOf(tp1);
      int index2 = tabControl1.TabPages.IndexOf(tp2);
      tabControl1.TabPages[index1] = tp2;
      tabControl1.TabPages[index2] = tp1;

      //Uncomment the following section to overcome bugs in the Compact Framework
      //tabControl1.SelectedIndex = tabControl1.SelectedIndex; 
      //string tp1Text, tp2Text;
      //tp1Text = tp1.Text;
      //tp2Text = tp2.Text;
      //tp1.Text=tp2Text;
      //tp2.Text=tp1Text;

    }

    private void EncodingSubPage_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingDetails2)._Value = EncodingSubPage.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric1)._Value = EncodingSubPage.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkGeneric2)._Value = EncodingSubPage.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkImg)._Value = EncodingSubPage.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersons)._Value = EncodingSubPage.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTitles)._Value = EncodingSubPage.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkCertification)._Value = EncodingSubPage.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkComment)._Value = EncodingSubPage.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkSyn)._Value = EncodingSubPage.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiPosters)._Value = EncodingSubPage.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPhotos)._Value = EncodingSubPage.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkPersonImages)._Value = EncodingSubPage.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkMultiFanart)._Value = EncodingSubPage.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEncodingLinkTrailer)._Value = EncodingSubPage.Text;
          break;

        default:
          break;
      }
    }

    private void contextMenuStripSearch_Opening(object sender, CancelEventArgs e)
    {
      // Runs before the user sees anything. A great place to set Enabled to true or false.
      copySelectionToolStripMenuItem.Enabled = textBody.SelectionLength > 0;
    }

    private void contextMenuStripDetails_Opening(object sender, CancelEventArgs e)
    {
      // Runs before the user sees anything. A great place to set Enabled to true or false.
      copySelectionToolStripMenuItem1.Enabled = textBodyDetail.SelectionLength > 0;
    }

    private void toolStripMenuSearchCopyAll_Click(object sender, EventArgs e)
    {
      try
      {
        // Make a DataObject.
        DataObject data_object = new DataObject();

        // Add the data in various formats.
        data_object.SetData(DataFormats.Rtf, textBody.Rtf);
        data_object.SetData(DataFormats.Text, textBody.Text);

        // Copy data to the Clipboard
        Clipboard.SetDataObject(data_object);
      }
      catch (Exception) { }
    }

    private void toolStripMenuDetailsCopyAll_Click(object sender, EventArgs e)
    {
      try
      {
        // Make a DataObject.
        DataObject data_object = new DataObject();

        // Add the data in various formats.
        data_object.SetData(DataFormats.Rtf, textBodyDetail.Rtf);
        data_object.SetData(DataFormats.Text, textBodyDetail.Text);

        // Copy data to the Clipboard
        Clipboard.SetDataObject(data_object);
      }
      catch (Exception) { }
    }

    private void copySelectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // Here, add the copy command to the relevant control, such as...
      textBody.Copy(); // Added manually.
    }

    private void copySelectionToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      // Here, add the copy command to the relevant control, such as...
      textBodyDetail.Copy(); // Added manually.
    }

    private void cbFileBasedReader_CheckedChanged(object sender, EventArgs e)
    {
      if (cbFileBasedReader.Checked)
      {
        label8.Text = "#Filename#";
        btnLoadDetailInWeb.Text = "Show";
        textRedir.Visible = false;
        textSearchCleanup.Visible = false;
        textUserAgent.Visible = false;
        textAccept.Visible = false;
        textHeaders.Visible = false;
        label17.Visible = false;
        label9.Visible = false;
        label35.Visible = false;
        label36.Visible = false;
        label37.Visible = false;
        groupBox5.Visible = false;
        groupBox1.Visible = false;
        textBox5.Visible = false;
        button_Find.Visible = false;
        label2.Visible = false;
        textURLPrefix.Visible = false;
        label11.Visible = false;
        textPage.Visible = false;
        button_Load_Web.Visible = false;
        cbAllowMideaInfo.Visible = true;
      }
      else
      {
        label8.Text = "#Search#";
        btnLoadDetailInWeb.Text = "Web";
        textRedir.Visible = true;
        textSearchCleanup.Visible = true;
        textUserAgent.Visible = true;
        textAccept.Visible = true;
        textHeaders.Visible = true;
        label17.Visible = true;
        label9.Visible = true;
        label35.Visible = true;
        label36.Visible = true;
        label37.Visible = true;
        groupBox5.Visible = true;
        groupBox1.Visible = true;
        textBox5.Visible = true;
        button_Find.Visible = true;
        label2.Visible = true;
        textURLPrefix.Visible = true;
        label11.Visible = true;
        textPage.Visible = true;
        button_Load_Web.Visible = true;
        cbAllowMideaInfo.Visible = false;
      }
    }

    private void btnLoadOldScript_Click(object sender, EventArgs e)
    {
      if (Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\"))
      {
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\";
        // openFileDialog1.FileName = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\*.xml";
        // openFileDialog1.FileName = @"*.xml";
        openFileDialog1.RestoreDirectory = false;
      }
      else
      {
        openFileDialog1.RestoreDirectory = true;
      }
      openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
      openFileDialog1.Title = "Find Internet Grabber Script file (xml file)";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        ResetFormControlValues(this);
        textConfig.Text = openFileDialog1.FileName;
        // take a backup so user can revert to old version 
        File.Copy(openFileDialog1.FileName, (openFileDialog1.FileName + "-bup" + DateTime.Now.ToString().Replace(":", "-") + ".xml").Replace("/", "-"));
        LoadXmloldScripts();
        // button_Load_Click(this, e); // no need to load, we only want to convert it here !

        saveFileDialog1.FileName = textConfig.Text;
        saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
        saveFileDialog1.Title = "Save Internet Grabber Script file (xml file)";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          // ToDo:  we can use this later, once we have migrated old save method to new !
          // SaveXml(textConfig.Text); 

          try
          {
            using (var fs = new FileStream(textConfig.Text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
              fs.SetLength(0); // do not append, owerwrite !
              // XmlTextWriter tw = new XmlTextWriter(File, UTF8Encoding.UTF8);
              using (var myXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.UTF8))
              {
                myXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                myXmlTextWriter.WriteStartDocument();
                grabberscript.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema); myXmlTextWriter.Flush();
                myXmlTextWriter.Close();
              }
              //xmlDoc.Save(fs);
              fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
              MessageBox.Show("Config saved !", "Info");
            }
          }
          catch (Exception ex)
          {
            MessageBox.Show("Config not saved !\n\nError: " + ex.Message, "Info"); 
          }
        }
      }
    }

    private void LoadXmloldScripts()
    {
      // InitMappingTable();
      xmlConf = new XmlConf(textConfig.Text);

      grabberscript = new GrabberScript();
      bool needsConversion = false;
      string strTemp;
      string[] split;
      List<string> fields;

      // load old script into XMLnode
      var doc = new XmlDocument();
      doc.Load(textConfig.Text);
      XmlNode script = doc.ChildNodes[1].FirstChild;

      #region init the new data structure
      if (grabberscript == null) grabberscript = new Grabber.Data.GrabberScript();

      if (grabberscript.Settings.Count == 0)
      {
        #region Create Settings
        GrabberScript.SettingsRow settings = grabberscript.Settings.NewSettingsRow();
        settings.DBName = xmlConf.find(xmlConf.listGen, TagName.DBName)._Value;
        try { settings.Version = xmlConf.find(xmlConf.listGen, TagName.Version)._Value; } catch (Exception) { settings.Version = ""; }
        settings.URLPrefix = xmlConf.find(xmlConf.listGen, TagName.URLPrefix)._Value;
        try { settings.Language = xmlConf.find(xmlConf.listGen, TagName.Language)._Value; } catch (Exception) { settings.Language = ""; }
        try { settings.Type = xmlConf.find(xmlConf.listGen, TagName.Type)._Value; }
        catch (Exception) { settings.Type = ""; }
        try { settings.SearchCleanup = xmlConf.find(xmlConf.listGen, TagName.SearchCleanup)._Value; }
        catch (Exception) { settings.SearchCleanup = ""; }
        try { settings.Encoding = xmlConf.find(xmlConf.listGen, TagName.Encoding)._Value; }
        catch (Exception) { settings.Encoding = ""; }
        try { settings.Accept = xmlConf.find(xmlConf.listGen, TagName.Accept)._Value; }
        catch (Exception) { settings.Accept = ""; }
        try { settings.Useragent = xmlConf.find(xmlConf.listGen, TagName.UserAgent)._Value; }
        catch (Exception) { settings.Useragent = ""; }
        try { settings.Headers = xmlConf.find(xmlConf.listGen, TagName.Headers)._Value; }
        catch (Exception) { settings.Headers = ""; }
        try { settings.FileBasedReader = (xmlConf.find(xmlConf.listGen, TagName.FileBasedReader)._Value == "true"); }
        catch (Exception) { settings.FileBasedReader = false; }
        grabberscript.Settings.AddSettingsRow(settings);
        #endregion
      }
      if (grabberscript.UserOptions.Count == 0)
      {
        #region Create UserOptions
        GrabberScript.UserOptionsRow useroptions = grabberscript.UserOptions.NewUserOptionsRow();
        // ToDo: Might load values here
        try { useroptions.MaxActors = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value; }
        catch { useroptions.MaxActors = string.Empty; }

        string strGrabActorRoles = "";
        try
        {
          strGrabActorRoles = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value;
          useroptions.GrabActorRoles = strGrabActorRoles == "true";
        }
        catch
        {
          useroptions.GrabActorRoles = false;
        }

        try { useroptions.MaxProducers = xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value; }
        catch { useroptions.MaxProducers = string.Empty; }

        try { useroptions.MaxDirectors = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value; }
        catch { useroptions.MaxDirectors = string.Empty; }

        try { useroptions.MaxWriters = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value; }
        catch { useroptions.MaxWriters = string.Empty; }

        try { useroptions.TitleCountry = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value; }
        catch { useroptions.TitleCountry = string.Empty; }

        try { useroptions.MaxTitles = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value; }
        catch { useroptions.MaxTitles = string.Empty; }

        try { useroptions.CertificationCountry = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value; }
        catch { useroptions.CertificationCountry = string.Empty; }
        grabberscript.UserOptions.AddUserOptionsRow(useroptions);
        #endregion
      }
      if (grabberscript.URLSearch.Count == 0)
      {
        #region Create URLSearch
        GrabberScript.URLSearchRow urlsearch = grabberscript.URLSearch.NewURLSearchRow();
        urlsearch.URL = xmlConf.find(xmlConf.listSearch, TagName.URL)._Value;
        urlsearch.Redirection = xmlConf.find(xmlConf.listSearch, TagName.URL)._Param1;
        urlsearch.StartPage = xmlConf.find(xmlConf.listSearch, TagName.KeyStartPage)._Value;
        urlsearch.NextPage = xmlConf.find(xmlConf.listSearch, TagName.KeyNextPage)._Value;
        urlsearch.StepPage = xmlConf.find(xmlConf.listSearch, TagName.KeyStepPage)._Value;
        grabberscript.URLSearch.AddURLSearchRow(urlsearch);
        // GrabberScript.SearchRow[] fields = urlsearch.GetSearchRows().Length;
        if (grabberscript.URLSearch[0].GetSearchDetailsRows().Length == 0)
        {
          #region load URLSearchDetails
          //AntMovieCatalog.CustomFieldRow cfr = data.CustomField.NewCustomFieldRow();
          //cfr.SetParentRow(data.CustomFieldsProperties[0]);
          //cfr.Tag = customFieldDefinition[0];
          //cfr.Name = (!string.IsNullOrEmpty(TranslateColumn(customFieldDefinition[0]))) ? TranslateColumn(customFieldDefinition[0]) : customFieldDefinition[1];
          //cfr.Type = customFieldDefinition[2];
          //data.CustomField.AddCustomFieldRow(cfr);
          foreach (string property in searchUrlItemsList)
          {
            GrabberScript.SearchDetailsRow search = grabberscript.SearchDetails.NewSearchDetailsRow();
            search.SetParentRow(grabberscript.URLSearch[0]);
            search.Name = property.Replace("Link", "DetailsURL");
            try { search.Start = XmlConvert.DecodeName(script.SelectSingleNodeFast("URLSearch/KeyStart" + property).InnerText); } catch (Exception) { search.Start = ""; }
            try { search.End = XmlConvert.DecodeName(script.SelectSingleNodeFast("URLSearch/KeyEnd" + property).InnerText); } catch (Exception) { search.End = ""; }
            try { search.RegExp = XmlConvert.DecodeName(script.SelectSingleNodeFast("URLSearch/Key" + property + "RegExp").InnerText); } catch (Exception) { search.RegExp = ""; }
            grabberscript.SearchDetails.AddSearchDetailsRow(search);
          }
          #endregion
        }
        #endregion
      }

      #region create the detail items collection
      GrabberScript.DetailsRow Details = grabberscript.Details.NewDetailsRow();
      grabberscript.Details.AddDetailsRow(Details);

      foreach (var grabDetailItem in Grabber_URLClass.GrabDetailItems)
      {
        string name = grabDetailItem.GrabName;

        GrabberScript.DetailRow detail = grabberscript.Detail.NewDetailRow();
        // detail.InternalName = name;
        detail.Name = grabDetailItem.DisplayName;
        detail.Type = (name.ToLower().StartsWith("url")) ? "URL" : "Value";
        detail.SetParentRow(Details);

        try 
        { 
          detail.Start = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/KeyStart" + name).InnerText) ?? "";
          detail.End = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/KeyEnd" + name).InnerText) ?? "";
          detail.Param1 = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/KeyStart" + name).Attributes["Param1"].InnerText) ?? "";
          detail.Param2 = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/KeyStart" + name).Attributes["Param2"].InnerText) ?? "";
          try { detail.Param3 = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/Key" + name + "RegExp").InnerText) ?? ""; }
          catch (Exception) { detail.Param3 = ""; }
          detail.Index = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/Key" + name + "Index").InnerText) ?? "";
          detail.SourcePage = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/Key" + name + "Page").InnerText) ?? "";
          try { detail.MaxItems = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/Key" + name + "MaxItems").InnerText) ?? ""; }
          catch (Exception) { detail.MaxItems = ""; }
          try { detail.Filter = XmlConvert.DecodeName(script.SelectSingleNodeFast("Details/Key" + name + "Language").InnerText); }
          catch (Exception) { detail.Filter = ""; }
          detail.GetRoles = true;
          detail.ApplyReplacements = false;
          detail.RunTime = 0;
          detail.FinalRegExp = "";
          // detail.Result = "";

          // strActivePage = this.LoadPage(strPage);
        }
        catch (Exception) 
        { 
          // if Start fails, we have to assume, there is no definition, so we create an empty one
          detail.Start = "";
          detail.End = "";
          detail.Param1 = "";
          detail.Param2 = "";
          detail.Param3 = "";
          detail.Index = "";
          detail.SourcePage = "";
          detail.MaxItems = "";
          detail.Filter = "";
          detail.GetRoles = true;
          detail.ApplyReplacements = false;
          detail.RunTime = 0;
          detail.FinalRegExp = "";
          // detail.Result = "";
        }

        grabberscript.Detail.AddDetailRow(detail);

      }

      #endregion

      #region create the mapper collection
      // Read Mapping Infos
      fields = Grabber_URLClass.FieldList();

      if (grabberscript.MappingRules.Count == 0)
      {
        GrabberScript.MappingRulesRow row = grabberscript.MappingRules.NewMappingRulesRow();
        grabberscript.MappingRules.AddMappingRulesRow(row);
      }

      for (int i = 0; i < 40; i++)
      {
        GrabberScript.MappingRuleRow mappingRule = grabberscript.MappingRule.NewMappingRuleRow();
        mappingRule.SetParentRow(grabberscript.MappingRules[0]);
        try
        {
          var val1 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param1;
          if (string.IsNullOrEmpty(val1))
            val1 = fields[i]; // if missing field in script, replace DB-field name with "right one"
          var val2 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param2;
          var val3 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param3;
          var val4 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param4;
          var val5 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param5;
          var val6 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param6;
          var val7 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param7;

          mappingRule.Number = i;
          mappingRule.InputField = val1;
          mappingRule.OutputField = val2;
          mappingRule.Replace = Convert.ToBoolean(val3);
          mappingRule.AddStart = Convert.ToBoolean(val4);
          mappingRule.AddEnd = Convert.ToBoolean(val5);
          mappingRule.MergePreferInput = Convert.ToBoolean(val6);
          mappingRule.MergePreferOutput = Convert.ToBoolean(val7);
          mappingRule.ApplyReplacements = false; //new param, initialized false
        }
        catch (Exception)
        {
          mappingRule.Number = i;
          mappingRule.InputField = fields[i];
          mappingRule.OutputField = "";
          mappingRule.Replace = false;
          mappingRule.AddStart = false;
          mappingRule.AddEnd = false;
          mappingRule.MergePreferInput = false;
          mappingRule.MergePreferOutput = false;
          mappingRule.ApplyReplacements = false; //new param, initialized false
        }
        grabberscript.MappingRule.AddMappingRuleRow(mappingRule);

      }

      #endregion

      #endregion

      // save new format - will overwrite the old script !!!


    }
  }
}