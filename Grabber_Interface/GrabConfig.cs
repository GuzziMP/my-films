using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using Grabber;
using System.Collections;
using System.Globalization;
using System.Reflection;


namespace Grabber_Interface
{
  using System.Diagnostics;
  using System.Linq;

  using MediaPortal.Configuration;

  public partial class GrabConfig : Form
  {
    private System.Resources.ResourceManager _rm = new System.Resources.ResourceManager("Localisation.Form1", Assembly.GetExecutingAssembly());
    private readonly CultureInfo _englishCulture = new CultureInfo("en-US");
    private readonly CultureInfo _frenchCulture = new CultureInfo("fr-FR");

    static OpenFileDialog openFileDialog1 = new OpenFileDialog();
    static SaveFileDialog saveFileDialog1 = new SaveFileDialog();

    private int GLiSearch = 0;
    private int GLiSearchD = 0;
    private int GLiSearchMatches = 0; // Added for search match highlighting
    //block auto text changed.
    private bool GLbBlock = false;
    //block auto selection changed in body
    private bool GLbBlockSelect = false;
    private string Body = string.Empty;
    private string BodyStripped = string.Empty; // added to hold stripped search page
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
    private ArrayList listUrl = new ArrayList();
    private CookieContainer cookie = new CookieContainer();

    //TabPage tabPageSaveMovie = null;
    //TabPage tabPageSaveDetails = null;

    private string[] Fields = new string[40]; // List to hold all possible grab fields ...

    public GrabConfig(string[] args)
    {
      System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
      InitializeComponent();

      InitMappingTable(); // load Mappingtable with values and other initialization

      if (CultureInfo.InstalledUICulture.ToString() == _frenchCulture.ToString())
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
      openFileDialog1.Title = "Open Internet Grabber Script file (xml file)";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        ResetFormControlValues(this);
        textConfig.Text = openFileDialog1.FileName;
        LoadXml();
        button_Load_Click(this, e);
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

      switch (cb_Parameter.SelectedIndex)
      {
        case 0: // Start - End
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartList).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndList).Value;
          buttonPrevParam1.Visible = false;
          break;
        case 1: // Title
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndTitle).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Param2;
          break;
        case 2: // Year
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndYear).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Param2;
          break;
        case 3: // Director
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndDirector).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Param2;
          break;
        case 4: // details page URL
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndLink).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Param2;
          break;
        case 5: // ID (e.g. IMDB_Id)
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndID).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Param2;
          break;
        case 6: // Options (e.g. Groups like "Video, Kino, TV, Series, etc.)
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndOptions).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Param2;
          break;
        case 7: // Akas (other title infos)
          labelSearchAkasRegex.Visible = true;
          textboxSearchAkasRegex.Visible = true;
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndAkas).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Param2;
          textboxSearchAkasRegex.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyAkasRegExp).Value;
          break;
        case 8: // Thumb
          TextKeyStart.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndThumb).Value;
          textReplace.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Param2;
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

    private void button_Load_Click(object sender, EventArgs e)
    {
      if (cbFileBasedReader.Checked == false && !TextURL.Text.Contains("#Search#"))
      {
        MessageBox.Show("Please, replace search keyword by #Search# in URL !", "Error");
        return;
      }

      if (cbFileBasedReader.Checked && !TextSearch.Text.Contains("\\"))
      {
        MessageBox.Show("Please, make sure you have a media path for local (nfo) grabbing in Testpage !", "Error");
        return;
      }

      if (TextSearch.Text.Length == 0)
      {
        MessageBox.Show("Please, insert search keyword !", "Error");
        return;
      }
      if (TextURL.Text.Contains("#page#") && (textPage.Text.Length == 0))
      {
        MessageBox.Show("Please, give the page number to load !", "Error");
        return;
      }

      dataGridViewSearchResults.Rows.Clear();
      if (TextURL.Text.Length > 0)
      {
        string absoluteUri;
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

        string strSearch = TextSearch.Text;
        strSearch = GrabUtil.ReplaceNormalOrRegex(strSearch, textSearchCleanup.Text); // cleanup search expression
        if (!strSearch.Contains("\\"))
          strSearch = GrabUtil.EncodeSearch(strSearch, textEncoding.Text);

        string wurl = TextURL.Text.Replace("#Search#", strSearch);
        wurl = wurl.Replace("#page#", textPage.Text);

        if (wurl.StartsWith("http://") == false && !TextSearch.Text.Contains("\\"))
          wurl = "http://" + wurl;

        Body = GrabUtil.GetPage(wurl, textEncoding.Text, out absoluteUri, cookie, textHeaders.Text, textAccept.Text, textUserAgent.Text);

        //1 resultat -> redirection automatique vers la fiche
        if (!wurl.Equals(absoluteUri))
        {
          Body = "";
          textBody.Text = "";
          listUrl.Clear();
          listUrl.Add(new GrabberUrlClass.IMDBUrl(absoluteUri, TextSearch.Text + " (AutoRedirect)", null, null));

          dataGridViewSearchResults.Rows.Clear();
          while (dataGridViewSearchResults.Rows.Count > 0)
          {
            dataGridViewSearchResults.Rows.RemoveAt(0);
          }
          for (int i = 0; i < 1; i++) // only add 1 line ...
          {
            GrabberUrlClass.IMDBUrl singleUrl = (GrabberUrlClass.IMDBUrl)listUrl[i];
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
            //dataGridViewSearchResults.Rows[i].Cells[2].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).Title;
            //dataGridViewSearchResults.Rows[i].Cells[3].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).Year;
            //dataGridViewSearchResults.Rows[i].Cells[4].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).Options;
            //dataGridViewSearchResults.Rows[i].Cells[5].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).ID;
            //dataGridViewSearchResults.Rows[i].Cells[6].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).URL;
            //dataGridViewSearchResults.Rows[i].Cells[7].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).Director;
            //dataGridViewSearchResults.Rows[i].Cells[8].Value = ((GrabberUrlClass.IMDBUrl)listUrl[0]).Akas;
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
        starttext = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartList).Value;
        endtext = xmlConf.find(xmlConf.ListSearch, TagName.KeyEndList).Value;

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

    private void LoadXml()
    {
      // InitMappingTable();
      xmlConf = new XmlConf(textConfig.Text);

      textName.Text = xmlConf.find(xmlConf.ListGen, TagName.DBName).Value;
      textURLPrefix.Text = xmlConf.find(xmlConf.ListGen, TagName.URLPrefix).Value;
      try { textEncoding.Text = xmlConf.find(xmlConf.ListGen, TagName.Encoding).Value; }
      catch (Exception) { textEncoding.Text = ""; }
      try { textLanguage.Text = xmlConf.find(xmlConf.ListGen, TagName.Language).Value; }
      catch (Exception) { textLanguage.Text = ""; }
      try { textVersion.Text = xmlConf.find(xmlConf.ListGen, TagName.Version).Value; }
      catch (Exception) { textVersion.Text = ""; }
      try { textType.Text = xmlConf.find(xmlConf.ListGen, TagName.Type).Value; }
      catch (Exception) { textType.Text = ""; }
      try { textSearchCleanup.Text = xmlConf.find(xmlConf.ListGen, TagName.SearchCleanup).Value; }
      catch (Exception) { textSearchCleanup.Text = ""; }
      try { textAccept.Text = xmlConf.find(xmlConf.ListGen, TagName.Accept).Value; }
      catch (Exception) { textAccept.Text = ""; }
      try { textUserAgent.Text = xmlConf.find(xmlConf.ListGen, TagName.UserAgent).Value; }
      catch (Exception) { textUserAgent.Text = ""; }
      try { textHeaders.Text = xmlConf.find(xmlConf.ListGen, TagName.Headers).Value; }
      catch (Exception) { textHeaders.Text = ""; }
      try { cbFileBasedReader.Checked = xmlConf.find(xmlConf.ListGen, TagName.FileBasedReader).Value == "true"; }
      catch (Exception) { cbFileBasedReader.Checked = false; }

      TextURL.Text = xmlConf.find(xmlConf.ListSearch, TagName.URL).Value;
      textRedir.Text = xmlConf.find(xmlConf.ListSearch, TagName.URL).Param1;
      textNextPage.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyNextPage).Value;
      textStartPage.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStartPage).Value;
      textStepPage.Text = xmlConf.find(xmlConf.ListSearch, TagName.KeyStepPage).Value;
      textPage.Text = textStartPage.Text;
      // Load User Settings page...
      try { cbMaxActors.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsMaxItems).Value; }
      catch { cbMaxActors.Text = string.Empty; }
      cbMaxActors.Enabled = !string.IsNullOrEmpty(cbMaxActors.Text);

      string strGrabActorRoles = "";
      string strGrabActorRegex = "";
      try
      {
        strGrabActorRoles = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsGrabActorRoles).Value;
        strGrabActorRegex = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsRegExp).Value;
        chkGrabActorRoles.Checked = strGrabActorRoles == "true";
      }
      catch
      {
        chkGrabActorRoles.Checked = false;
        chkGrabActorRoles.Enabled = false;
      }
      if (string.IsNullOrEmpty(strGrabActorRoles) || string.IsNullOrEmpty(strGrabActorRegex)) chkGrabActorRoles.Enabled = false;
      else chkGrabActorRoles.Enabled = true;

      try { cbMaxProducers.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyProductMaxItems).Value; }
      catch { cbMaxProducers.Text = string.Empty; }
      cbMaxProducers.Enabled = !string.IsNullOrEmpty(cbMaxProducers.Text);

      try { cbMaxDirectors.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseMaxItems).Value; }
      catch { cbMaxDirectors.Text = string.Empty; }
      cbMaxDirectors.Enabled = !string.IsNullOrEmpty(cbMaxDirectors.Text);

      try { cbMaxWriters.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterMaxItems).Value; }
      catch { cbMaxWriters.Text = string.Empty; }
      cbMaxWriters.Enabled = !string.IsNullOrEmpty(cbMaxWriters.Text);

      try { cbTtitlePreferredLanguage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguage).Value; }
      catch { cbTtitlePreferredLanguage.Text = string.Empty; }
      //if (string.IsNullOrEmpty(cbTtitlePreferredLanguage.Text)) cbTtitlePreferredLanguage.Enabled = false;
      //else cbTtitlePreferredLanguage.Enabled = true;

      try { cbTtitleMaxTitles.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleMaxItems).Value; }
      catch { cbTtitleMaxTitles.Text = string.Empty; }
      cbTtitleMaxTitles.Enabled = !string.IsNullOrEmpty(cbTtitleMaxTitles.Text);

      try { cbCertificationPreferredLanguage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguage).Value; }
      catch { cbCertificationPreferredLanguage.Text = string.Empty; }
      //if (string.IsNullOrEmpty(cbCertificationPreferredLanguage.Text)) cbCertificationPreferredLanguage.Enabled = false;
      //else cbCertificationPreferredLanguage.Enabled = true;

      // Add Dropdownentries to User Options
      cbTtitlePreferredLanguage.Items.Clear();
      string strTemp;
      try { strTemp = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguageAll).Value; }
      catch { strTemp = string.Empty; }
      string[] split = strTemp.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbTtitlePreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbTtitlePreferredLanguage.Items.Add(strDroptext.Trim());
      }
      cbTtitlePreferredLanguage.Enabled = cbTtitlePreferredLanguage.Items.Count > 0;

      cbCertificationPreferredLanguage.Items.Clear();
      try { strTemp = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguageAll).Value; }
      catch { strTemp = string.Empty; }
      split = strTemp.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbCertificationPreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbCertificationPreferredLanguage.Items.Add(strDroptext.Trim());
      }
      cbCertificationPreferredLanguage.Enabled = cbCertificationPreferredLanguage.Items.Count > 0;

      // Read Mapping Infos
      List<string> fields = GrabberUrlClass.FieldList();

      for (int i = 0; i < 40; i++)
      {
        try
        {
          string val1 = string.Empty, val2 = string.Empty, val3 = string.Empty, val4 = string.Empty, val5 = string.Empty, val6 = string.Empty, val7 = string.Empty;
          val1 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param1;
          if (string.IsNullOrEmpty(val1)) val1 = fields[i]; // if missing field in script, replace DB-field name with "right one"
          val2 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param2;
          val3 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param3;
          val4 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param4;
          val5 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param5;
          val6 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param6;
          val7 = xmlConf.find(xmlConf.ListMapping, "Field_" + i).Param7;

          dataGridViewMapping.Rows[i].Cells[1].Value = val1;
          dataGridViewMapping.Rows[i].Cells[2].Value = val2;
          dataGridViewMapping.Rows[i].Cells[3].Value = Convert.ToBoolean(val3);
          dataGridViewMapping.Rows[i].Cells[4].Value = Convert.ToBoolean(val4);
          dataGridViewMapping.Rows[i].Cells[5].Value = Convert.ToBoolean(val5);
          dataGridViewMapping.Rows[i].Cells[6].Value = Convert.ToBoolean(val6);
          dataGridViewMapping.Rows[i].Cells[7].Value = Convert.ToBoolean(val7);
        }
        catch (Exception)
        {
          dataGridViewMapping.Rows[i].Cells[1].Value = fields[i];
          dataGridViewMapping.Rows[i].Cells[2].Value = "";
          dataGridViewMapping.Rows[i].Cells[3].Value = false;
          dataGridViewMapping.Rows[i].Cells[4].Value = false;
          dataGridViewMapping.Rows[i].Cells[5].Value = false;
          dataGridViewMapping.Rows[i].Cells[6].Value = false;
          dataGridViewMapping.Rows[i].Cells[7].Value = false;
        }
      }
    }

    private void SaveXml(string file)
    {
      xmlConf.find(xmlConf.ListGen, TagName.DBName).Value = textName.Text;
      xmlConf.find(xmlConf.ListGen, TagName.URLPrefix).Value = textURLPrefix.Text;
      try { xmlConf.find(xmlConf.ListGen, TagName.Encoding).Value = textEncoding.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.Language).Value = textLanguage.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.Version).Value = textVersion.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.Type).Value = textType.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.SearchCleanup).Value = textSearchCleanup.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.Accept).Value = textAccept.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.UserAgent).Value = textUserAgent.Text; }
      catch (Exception) { }
      try { xmlConf.find(xmlConf.ListGen, TagName.Headers).Value = textHeaders.Text; }
      catch (Exception) { }

      try
      {
        xmlConf.find(xmlConf.ListGen, TagName.FileBasedReader).Value = cbFileBasedReader.Checked ? "true" : "false";
      }
      catch (Exception) { }

      xmlConf.find(xmlConf.ListSearch, TagName.URL).Value = TextURL.Text;
      xmlConf.find(xmlConf.ListSearch, TagName.URL).Param1 = textRedir.Text;
      xmlConf.find(xmlConf.ListSearch, TagName.KeyNextPage).Value = textNextPage.Text;
      xmlConf.find(xmlConf.ListSearch, TagName.KeyStartPage).Value = textStartPage.Text;
      xmlConf.find(xmlConf.ListSearch, TagName.KeyStepPage).Value = textStepPage.Text;


      XmlTextWriter tw = new XmlTextWriter(file, Encoding.UTF8) {Formatting = Formatting.Indented};
      tw.WriteStartDocument(true);
      tw.WriteStartElement("Profile");
      tw.WriteStartElement("Section");

      foreach (ListNode t in xmlConf.ListGen)
      {
        tw.WriteStartElement(t.Tag);
        tw.WriteString(t.Value);
        tw.WriteEndElement();
      }

      tw.WriteStartElement("URLSearch");

      foreach (ListNode t in xmlConf.ListSearch)
      {
        tw.WriteStartElement(t.Tag);
        if (t.Tag.StartsWith("KeyStart") || t.Tag.Equals("URL"))
        {
          tw.WriteAttributeString("Param1", XmlConvert.EncodeName(t.Param1));
          tw.WriteAttributeString("Param2", XmlConvert.EncodeName(t.Param2));
        }
        tw.WriteString(XmlConvert.EncodeName(t.Value));
        tw.WriteEndElement();
      }

      tw.WriteEndElement();
      tw.WriteStartElement("Details");

      foreach (ListNode t in xmlConf.ListDetail)
      {
        tw.WriteStartElement(t.Tag);
        if (t.Tag.StartsWith("KeyStart"))
        {
          tw.WriteAttributeString("Param1", XmlConvert.EncodeName(t.Param1));
          tw.WriteAttributeString("Param2", XmlConvert.EncodeName(t.Param2));
        }
        tw.WriteString(XmlConvert.EncodeName(t.Value));
        tw.WriteEndElement();
      }
      tw.WriteEndElement();

      // Write Mapping Infos
      tw.WriteStartElement("Mapping");
      for (int i = 0; i < dataGridViewMapping.Rows.Count; i++)
      {
        tw.WriteStartElement("Field_" + i.ToString()); // fieldnumer
        string val1 = string.Empty, val2 = string.Empty, val3 = string.Empty, val4 = string.Empty, val5 = string.Empty, val6 = string.Empty, val7 = string.Empty;
        if (dataGridViewMapping.Rows[i].Cells[1].Value != null) val1 = dataGridViewMapping.Rows[i].Cells[1].Value.ToString(); // source
        if (dataGridViewMapping.Rows[i].Cells[2].Value != null) val2 = dataGridViewMapping.Rows[i].Cells[2].Value.ToString(); // destination
        if (dataGridViewMapping.Rows[i].Cells[3].Value != null) val3 = dataGridViewMapping.Rows[i].Cells[3].Value.ToString(); // replace
        if (dataGridViewMapping.Rows[i].Cells[4].Value != null) val4 = dataGridViewMapping.Rows[i].Cells[4].Value.ToString(); // add before
        if (dataGridViewMapping.Rows[i].Cells[5].Value != null) val5 = dataGridViewMapping.Rows[i].Cells[5].Value.ToString(); // add after
        if (dataGridViewMapping.Rows[i].Cells[6].Value != null) val6 = dataGridViewMapping.Rows[i].Cells[6].Value.ToString(); // merge prefer source
        if (dataGridViewMapping.Rows[i].Cells[7].Value != null) val7 = dataGridViewMapping.Rows[i].Cells[7].Value.ToString(); // merge prefer destination

        tw.WriteAttributeString("source", XmlConvert.EncodeName(val1));
        tw.WriteAttributeString("destination", XmlConvert.EncodeName(val2));
        tw.WriteAttributeString("replace", XmlConvert.EncodeName(val3));
        tw.WriteAttributeString("addstart", XmlConvert.EncodeName(val4));
        tw.WriteAttributeString("addend", XmlConvert.EncodeName(val5));
        tw.WriteAttributeString("mergeprefersource", XmlConvert.EncodeName(val6));
        tw.WriteAttributeString("mergepreferdestination", XmlConvert.EncodeName(val7));
        tw.WriteEndElement();
      }

      tw.WriteEndElement();
      tw.WriteEndElement();
      tw.WriteEndDocument();
      // on ferme ensuite le fichier xml
      tw.Flush();
      // pour finir on va vider le buffer , et on ferme le fichier
      tw.Close();

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
              if (c.Name != "TextSearch" && c.Name != "textPage")
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
        MessageBox.Show("You have to select a movie FILE, when file Based Reader is selected !", "Error");
        return;
      }
      else
      {
        SaveXml(textConfig.Text + ".tmp");
        Load_Preview(true); // always ask - gives all matching results! - was false in earlier versions ...
      }
    }

    private void Load_Preview(bool alwaysAsk)
    {
      // dataGridViewSearchResults.Rows.Clear();
      while (dataGridViewSearchResults.Rows.Count > 0)
      {
        dataGridViewSearchResults.Rows.RemoveAt(0);
      }
      button_GoDetailPage.Enabled = false;
      button_Preview.Enabled = false;

      GrabberUrlClass grab = new GrabberUrlClass();
      int pageNumber = -1;
      if (!string.IsNullOrEmpty(textPage.Text))
        pageNumber = Convert.ToInt16(textPage.Text);
      try
      {
        listUrl = grab.ReturnURL(TextSearch.Text, textConfig.Text + ".tmp", pageNumber, alwaysAsk, string.Empty);
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

        var wurl = (GrabberUrlClass.IMDBUrl)listUrl[i];
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

      switch (cb_Parameter.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartList).Value = TextKeyStart.Text;
          break;
        case 1:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Value = TextKeyStart.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Value = TextKeyStart.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Value = TextKeyStart.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Value = TextKeyStart.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Value = TextKeyStart.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Value = TextKeyStart.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Value = TextKeyStart.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Value = TextKeyStart.Text;
          break;
        default:
          TextKeyStart.Text = "";
          break;
      }

      textBody.Text = cb_Parameter.SelectedIndex > 0 ? BodyStripped : Body;
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, false);

    }

    private void TextKeyStop_TextChanged(object sender, EventArgs e)
    {
      switch (cb_Parameter.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndList).Value = TextKeyStop.Text;
          try { textBody.Text = BodyStripped; }
          catch { textBody.Text = Body; }
          break;
        case 1:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndTitle).Value = TextKeyStop.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndYear).Value = TextKeyStop.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndDirector).Value = TextKeyStop.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndLink).Value = TextKeyStop.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndID).Value = TextKeyStop.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndOptions).Value = TextKeyStop.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndAkas).Value = TextKeyStop.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyEndThumb).Value = TextKeyStop.Text;
          break;
        default:
          TextKeyStop.Text = "";
          break;
      }

      textBody.Text = cb_Parameter.SelectedIndex > 0 ? BodyStripped : Body;  // && TextKeyStop.Text.Length > 0
      textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text, false);
    }

    private void textboxSearchAkasRegex_TextChanged(object sender, EventArgs e)
    {
      switch (cb_Parameter.SelectedIndex)
      {
        case 7: // Akas Search Regex
          xmlConf.find(xmlConf.ListSearch, TagName.KeyAkasRegExp).Value = textboxSearchAkasRegex.Text;
          break;
      }
    }

    private void textBody_SelectionChanged(object sender, EventArgs e)
    {
      if (GLbBlockSelect)
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
      lblResultsFound.Text = nb + " match found";
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
            string strUrl = TextURLDetail.Text;
            if (File.Exists(strUrl))
            {
              string movieDirectory = Path.GetDirectoryName(strUrl);
              string movieFilename = Path.GetFileNameWithoutExtension(strUrl);
              // Set DetailsPath
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<directory>" + movieDirectory + "</directory>";
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<filename>" + movieFilename + "</filename>";
              if (movieDirectory != null)
              {
                string[] files = Directory.GetFiles(movieDirectory, "*", SearchOption.AllDirectories);

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
        if (xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value.Length > 0)
        {
          iStart = BodyDetail.IndexOf(xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            iEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndBody).Value != "" ? BodyDetail.IndexOf(xmlConf.find(xmlConf.ListDetail, TagName.KeyEndBody).Value, iStart) : BodyDetail.Length;

            if (iEnd == -1)
              iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value.Length;
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
            textBodyDetail.Text = BodyDetail;
          }
          else
            textBodyDetail.Text = BodyDetail;
        }
        else
          textBodyDetail.Text = BodyDetail;

        watch.Stop();
        TimeBodyDetail = " (" + watch.ElapsedMilliseconds + " ms)";
        #endregion
      }

      #region Test if there is a page for Secondary Details (like OFDB GW) and load page in BodyDetails2
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Value;
        strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndDetails2).Value;
        strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param1;
        strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param2;
        strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Index).Value;
        strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Page).Value;
        try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingDetails2).Value; }
        catch (Exception) { strPage = ""; }

        strActivePage = LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyDetail2 = strTemp;
          BodyDetail2 = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
        }
        else
          BodyDetail2 = "";
        watch.Stop();
        TimeBodyDetail2 = " (" + watch.ElapsedMilliseconds + " ms)";
      }
      catch
      {
        BodyDetail2 = "";
      }
      #endregion

      #region Test if there is a page for Generic 1 page
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Value;
        strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric1).Value;
        strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param1;
        strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param2;
        strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Index).Value;
        strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Page).Value;
        try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric1).Value; }
        catch (Exception) { strPage = ""; }

        strActivePage = LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyLinkGeneric1 = strTemp;
          BodyLinkGeneric1 = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
        }
        else
          BodyLinkGeneric1 = "";
        watch.Stop();
        TimeBodyLinkGeneric1 = " (" + watch.ElapsedMilliseconds + " ms)";
      }
      catch
      {
        BodyLinkGeneric1 = "";
      }
      #endregion

      #region Test if there is a page for Generic 2 page
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Value;
        strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric2).Value;
        strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param1;
        strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param2;
        strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Index).Value;
        strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Page).Value;
        try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric2).Value; }
        catch (Exception) { strEncoding = ""; }

        strActivePage = LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyLinkGeneric2 = strTemp;
          BodyLinkGeneric2 = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
        }
        else
          BodyLinkGeneric2 = "";
        watch.Stop();
        TimeBodyLinkGeneric2 = " (" + watch.ElapsedMilliseconds + " ms)";
      }
      catch
      {
        BodyLinkGeneric2 = "";
      }
      #endregion

      #region Test if there is a redirection page for Covers and load page in BodyLinkImg
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkImg).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkImg).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkImg = strTemp;
        BodyLinkImg = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkImg = "";
      watch.Stop();
      TimeBodyLinkImg = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Persons and load page in BodyLinkPersons
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersons).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersons).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkPersons = strTemp;
        BodyLinkPersons = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkPersons = "";
      watch.Stop();
      TimeBodyLinkPersons = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Titles and load page in BodyLinkTitles
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTitles).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTitles).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkTitles = strTemp;
        BodyLinkTitles = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkTitles = "";
      watch.Stop();
      TimeBodyLinkTitles = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Certification and load page in BodyLinkCertification
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkCertification).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkCertification).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkCertification = strTemp;
        BodyLinkCertification = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkCertification = "";
      watch.Stop();
      TimeBodyLinkCertification = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Synopsis/Description and load page in BodyLinkSyn
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkSyn).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkSyn).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkSyn = strTemp;
        BodyLinkSyn = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkSyn = "";
      watch.Stop();
      TimeBodyLinkSyn = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Comment and load page in BodyLinkComment
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkComment).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkComment).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkComment = strTemp;
        BodyLinkComment = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkComment = "";
      watch.Stop();
      TimeBodyLinkComment = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for MultiPosters and load page in BodyLinkMultiPosters
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiPosters).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiPosters).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkMultiPosters = strTemp;
        BodyLinkMultiPosters = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkMultiPosters = "";
      watch.Stop();
      TimeBodyLinkMultiPosters = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Photos and load page in BodyLinkPhotos
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPhotos).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPhotos).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkPhotos = strTemp;
        BodyLinkPhotos = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkPhotos = "";
      watch.Stop();
      TimeBodyLinkPhotos = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for PersonImages and load page in BodyLinkPersonImages
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersonImages).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersonImages).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkPersonImages = strTemp;
        BodyLinkPersonImages = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkPersonImages = "";
      watch.Stop();
      TimeBodyLinkPersonImages = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for MultiFanart and load page in BodyLinkMultiFanart
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiFanart).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiFanart).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkMultiFanart = strTemp;
        BodyLinkMultiFanart = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkMultiFanart = "";
      watch.Stop();
      TimeBodyLinkMultiFanart = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion

      #region Test if there is a redirection page for Trailer and load page in BodyLinkTrailer
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Value;
      strEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTrailer).Value;
      strParam1 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param1;
      strParam2 = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param2;
      strIndex = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerIndex).Value;
      strPage = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerPage).Value;
      try { strEncoding = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTrailer).Value; }
      catch (Exception) { strEncoding = ""; }

      strActivePage = LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkTrailer = strTemp;
        BodyLinkTrailer = GrabUtil.GetPage(strTemp, string.IsNullOrEmpty(strEncoding) ? textEncoding.Text : strEncoding, out absoluteUri, new CookieContainer(), textHeaders.Text, textAccept.Text, textUserAgent.Text);
      }
      else
        BodyLinkTrailer = "";
      watch.Stop();
      TimeBodyLinkTrailer = " (" + watch.ElapsedMilliseconds + " ms)";
      #endregion
    }

    private string LoadPage(string page)
    {
      string strActivePage;
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
        case 0: // Start/end page
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndBody).Value;
          buttonPrevParamDetail.Visible = false;
          break;
        case 1: // Original Title
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyOTitlePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndOTitle).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyOTitleIndex).Value;
          break;
        case 2: // Translated Title
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitlePage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkTitles)) textBodyDetail.Text = BodyLinkTitles;
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguageAll).Value; }
          catch { textLanguagesAll.Text = string.Empty; }
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTTitle).Value;
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleIndex).Value;
          break;
        case 3: // Coverimage
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyImgPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkImg)) textBodyDetail.Text = BodyLinkImg;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndImg).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyImgIndex).Value;
          break;
        case 4: // Rating 1
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRatePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRate).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.BaseRating).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRateIndex).Value;
          break;
        case 5: // Rating 2
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRate2Page).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRate2).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.BaseRating).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRate2Index).Value;
          break;
        case 6: // Director
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRealisePage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRealise).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseIndex).Value;
          break;
        case 7: // Producer
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyProductPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndProduct).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyProductRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyProductMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyProductIndex).Value;
          break;
        case 8: // Writer
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndWriter).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterIndex).Value;
          break;
        case 9: // Actors (Credits)
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCredits).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          string strActorRoles;
          try
          {
            strActorRoles = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsGrabActorRoles).Value;
            chkActorRoles.Checked = strActorRoles == "true";
          }
          catch { chkActorRoles.Checked = false; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsIndex).Value;
          break;
        case 10: // Country
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCountry).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryIndex).Value;
          break;
        case 11: // Category
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGenrePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGenre).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGenreRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGenreIndex).Value;
          break;
        case 12: // Year
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyYearPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndYear).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyYearIndex).Value;
          break;
        case 13: // IMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_IdPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndIMDB_Id).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_IdIndex).Value;
          break;
        case 14: // Description
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeySynPage).Value;
          //if (!textBodyDetail.Text.Equals(BodyLinkSyn)) textBodyDetail.Text = BodyLinkSyn;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndSyn).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeySynIndex).Value;
          break;
        case 15: // Comment
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkComment)) textBodyDetail.Text = BodyLinkComment;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndComment).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentIndex).Value;
          break;
        case 16: // Language
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguagePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLanguage).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguageRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguageIndex).Value;
          break;
        case 17: // Tagline
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTaglinePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTagline).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTaglineIndex).Value;
          break;
        case 18: // Certification
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationPage).Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkCertification)) textBodyDetail.Text = BodyLinkCertification;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCertification).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguageAll).Value; }
          catch { textLanguagesAll.Text = string.Empty; }
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationIndex).Value;
          break;
        case 19: // Studio
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndStudio).Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioIndex).Value;
          break;
        case 20: // Edition
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEditionPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndEdition).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEditionIndex).Value;
          break;
        case 21: // IMDB_Rank
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_RankPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndIMDB_Rank).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_RankIndex).Value;
          break;
        case 22: // TMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTMDB_IdPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTMDB_Id).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTMDB_IdIndex).Value;
          break;
        case 23: // Generic Field 1
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Page).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric1).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1RegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Language).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1MaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Index).Value;
          break;
        case 24: // Generic Field 2
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Page).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric2).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2RegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Language).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2MaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Index).Value;
          break;
        case 25: // Generic Field 3
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Page).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric3).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3RegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Language).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3MaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Index).Value;
          break;
        case 26: // Link Secondary Details Base page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Page).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndDetails2).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Index).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingDetails2).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 27: // Link Generic 1
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Page).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric1).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Index).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric1).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 28: // Link Generic 2
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Page).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric2).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Index).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric2).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 29: // Link Coverart-Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkImg).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkImg).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 30: // Link Persons page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersons).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersons).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 31: // Link Titles-Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTitles).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTitles).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 32: // Link Certification-Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkCertification).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkCertification).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 33: // Link Comment-Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkComment).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkComment).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 34: // Link Synopsis/Description-Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkSyn).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkSyn).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 35: // Link MultiPosters - Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiPosters).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiPosters).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 36: // Link Photos - Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPhotos).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPhotos).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 37: // Link PersonImages - Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersonImages).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersonImages).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 38: // Link MultiFanart - Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiFanart).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiFanart).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 39: // Link Trailer - Secondary page
          lblEncodingSubPage.Visible = true;
          EncodingSubPage.Visible = true;
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTrailer).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerIndex).Value;
          try { EncodingSubPage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTrailer).Value; }
          catch { EncodingSubPage.Text = string.Empty; }
          break;
        case 40: // MultiPosters
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersPage).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndMultiPosters).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersIndex).Value;
          break;
        case 41: // Photos
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosPage).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndPhotos).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosIndex).Value;
          break;
        case 42: // PersonImages
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesPage).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndPersonImages).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesIndex).Value;
          break;
        case 43: // MultiFanart
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartPage).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndMultiFanart).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartIndex).Value;
          break;
        case 44: // Trailer
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerPage).Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTrailer).Value;
          try { textComplement.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerRegExp).Value; }
          catch { textComplement.Text = string.Empty; }
          try { textLanguages.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerLanguage).Value; }
          catch { textLanguages.Text = string.Empty; }
          try { textMaxItems.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerMaxItems).Value; }
          catch { textMaxItems.Text = string.Empty; }
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerIndex).Value;
          break;
        case 45: // Runtime
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRuntimePage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRuntime).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyRuntimeIndex).Value;
          break;
        case 46: // Collection
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCollection).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionIndex).Value;
          break;

        case 47: // Collection Image URL
          URLpage.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionImageURLPage).Value;
          textDReplace.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCollectionImageURL).Value;
          Index.Text = xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionImageURLIndex).Value;
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

      if (lblComplement.Visible)
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

      // load selected page into webpage window
      textBodyDetail.Text = LoadPage(URLpage.Text);

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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value = TextKeyStartD.Text;
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

              //Cutting the body
              iStart += TextKeyStartD.Text.Length;
              textBodyDetail.Text = BodyDetail.Substring(iStart, iEnd - iStart);

            }
            else
              textBodyDetail.Text = BodyDetail;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Value = TextKeyStartD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Value = TextKeyStartD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Value = TextKeyStartD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Value = TextKeyStartD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Value = TextKeyStartD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Value = TextKeyStartD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Value = TextKeyStartD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Value = TextKeyStartD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Value = TextKeyStartD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Value = TextKeyStartD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Value = TextKeyStartD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Value = TextKeyStartD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Value = TextKeyStartD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Value = TextKeyStartD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Value = TextKeyStartD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Value = TextKeyStartD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Value = TextKeyStartD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Value = TextKeyStartD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Value = TextKeyStartD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Value = TextKeyStartD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Value = TextKeyStartD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Value = TextKeyStartD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Value = TextKeyStartD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Value = TextKeyStartD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Value = TextKeyStartD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Value = TextKeyStartD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Value = TextKeyStartD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Value = TextKeyStartD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Value = TextKeyStartD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Value = TextKeyStartD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Value = TextKeyStartD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Value = TextKeyStartD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Value = TextKeyStartD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Value = TextKeyStartD.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Value = TextKeyStartD.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Value = TextKeyStartD.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Value = TextKeyStartD.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Value = TextKeyStartD.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Value = TextKeyStartD.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Value = TextKeyStartD.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Value = TextKeyStartD.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Value = TextKeyStartD.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Value = TextKeyStartD.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Value = TextKeyStartD.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Value = TextKeyStartD.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Value = TextKeyStartD.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Value = TextKeyStartD.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value = TextKeyStartD.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndOTitle).Value = TextKeyStopD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTTitle).Value = TextKeyStopD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndImg).Value = TextKeyStopD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRate).Value = TextKeyStopD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRate2).Value = TextKeyStopD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRealise).Value = TextKeyStopD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndProduct).Value = TextKeyStopD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndWriter).Value = TextKeyStopD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCredits).Value = TextKeyStopD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCountry).Value = TextKeyStopD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGenre).Value = TextKeyStopD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndYear).Value = TextKeyStopD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndIMDB_Id).Value = TextKeyStopD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndSyn).Value = TextKeyStopD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndComment).Value = TextKeyStopD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLanguage).Value = TextKeyStopD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTagline).Value = TextKeyStopD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCertification).Value = TextKeyStopD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndStudio).Value = TextKeyStopD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndEdition).Value = TextKeyStopD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndIMDB_Rank).Value = TextKeyStopD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTMDB_Id).Value = TextKeyStopD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric1).Value = TextKeyStopD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric2).Value = TextKeyStopD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndGeneric3).Value = TextKeyStopD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndDetails2).Value = TextKeyStopD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric1).Value = TextKeyStopD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkGeneric2).Value = TextKeyStopD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkImg).Value = TextKeyStopD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersons).Value = TextKeyStopD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTitles).Value = TextKeyStopD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkCertification).Value = TextKeyStopD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkComment).Value = TextKeyStopD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkSyn).Value = TextKeyStopD.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiPosters).Value = TextKeyStopD.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPhotos).Value = TextKeyStopD.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkPersonImages).Value = TextKeyStopD.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkMultiFanart).Value = TextKeyStopD.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndLinkTrailer).Value = TextKeyStopD.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndMultiPosters).Value = TextKeyStopD.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndPhotos).Value = TextKeyStopD.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndPersonImages).Value = TextKeyStopD.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndMultiFanart).Value = TextKeyStopD.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndTrailer).Value = TextKeyStopD.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndRuntime).Value = TextKeyStopD.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCollection).Value = TextKeyStopD.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEndCollectionImageURL).Value = TextKeyStopD.Text;
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
        label10.Text = nb.ToString() + " match found";
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
        button_GoDetailPage.Text = "Display Next page";
      else if (rowSelected >= 0 && dataGridViewSearchResults["ResultColumn2", rowSelected].Value.ToString() == "---")
        button_GoDetailPage.Text = "Display Previous page";
      else
        button_GoDetailPage.Text = "Use with Detail page";
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
              GrabberUrlClass.IMDBUrl wurl = (GrabberUrlClass.IMDBUrl)listUrl[rowSelected];
              Load_Preview(true); // always ask - was false in earlier versions ...
              button_GoDetailPage.Enabled = false;
            }
            break;
          case "---":
            {
              textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) - Convert.ToInt16(textStepPage.Text));
              GrabberUrlClass.IMDBUrl wurl = (GrabberUrlClass.IMDBUrl)listUrl[rowSelected];
              Load_Preview(true); // always ask - gives all results - was "false" in earlier versions
              button_GoDetailPage.Enabled = false;
            }
            break;
          default:
            {
              File.Delete(textConfig.Text + ".tmp");
              GrabberUrlClass.IMDBUrl wurl = (GrabberUrlClass.IMDBUrl)listUrl[rowSelected];
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
      GrabberUrlClass grab = new GrabberUrlClass();
      string[] result = new string[80];

      try // http://akas.imdb.com/title/tt0133093/
      {
        result = grab.GetDetail(TextURLDetail.Text, Environment.GetEnvironmentVariable("TEMP"), textConfig.Text + ".tmp", true, string.Empty, string.Empty, string.Empty, string.Empty, null);
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
      for (int i = 0; i < result.Length; i++)
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
                pictureBoxPreviewCover.ImageLocation = result[i];

                // Create new FileInfo object and get the Length.
                FileInfo f = new FileInfo(result[i]);
                //long s1 = f.Length;
                //labelImageSize.Text = s1.ToString();
                labelImageSize.Text = ByteString(f.Length);
              }
              catch (Exception)
              {
                labelImageSize.Text = "n/a";
                //MessageBox.Show("An error ocurred in image preview - check your config.\n" + ex.Message + "\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
              }
              try
              {
                pictureBoxPreviewCollection.ImageLocation = Path.Combine(Path.GetDirectoryName(result[i]), "Collection_" + Path.GetFileName(result[i]));
                //FileInfo f = new FileInfo(Result[i]);
                //labelImageSize.Text = this.ByteString(f.Length);
              }
              catch (Exception)
              {
                // labelImageSize.Text = "n/a";
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
            break;
          case 39:
          case 79:
            textPreview.SelectedText += "(" + i + ") " + "Picture URL'" + mapped + Environment.NewLine;
            break;
          default:
            textPreview.SelectedText += "(" + i + ") " + "Mapping Output Field '" + (i - 40) + "'" + mapped + Environment.NewLine;
            break;
        }
        if (i <= 80) // Changed to support new fields...
          textPreview.AppendText(result[i] + Environment.NewLine);
      }
      // List of Grab Pages used for Grabber results:
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
      textPreview.SelectedText += "*** Infos about used Grab Pages ***" + Environment.NewLine;
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("Base page:" + TimeBodyDetail + Environment.NewLine);
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
          xmlConf.find(xmlConf.ListDetail, TagName.BaseRating).Value = textComplement.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleRegExp).Value = textComplement.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseRegExp).Value = textComplement.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyProductRegExp).Value = textComplement.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterRegExp).Value = textComplement.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsRegExp).Value = textComplement.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryRegExp).Value = textComplement.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGenreRegExp).Value = textComplement.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentRegExp).Value = textComplement.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguageRegExp).Value = textComplement.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationRegExp).Value = textComplement.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioRegExp).Value = textComplement.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1RegExp).Value = textComplement.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2RegExp).Value = textComplement.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3RegExp).Value = textComplement.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersRegExp).Value = textComplement.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosRegExp).Value = textComplement.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesRegExp).Value = textComplement.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartRegExp).Value = textComplement.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerRegExp).Value = textComplement.Text;
          break;
      }
    }

    private void textMaxItems_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleMaxItems).Value = textMaxItems.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseMaxItems).Value = textMaxItems.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyProductMaxItems).Value = textMaxItems.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterMaxItems).Value = textMaxItems.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsMaxItems).Value = textMaxItems.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1MaxItems).Value = textMaxItems.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2MaxItems).Value = textMaxItems.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3MaxItems).Value = textMaxItems.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersMaxItems).Value = textMaxItems.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosMaxItems).Value = textMaxItems.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesMaxItems).Value = textMaxItems.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartMaxItems).Value = textMaxItems.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerMaxItems).Value = textMaxItems.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsGrabActorRoles).Value = chkActorRoles.Checked ? "true" : "false";
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguage).Value = textLanguages.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguage).Value = textLanguages.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Language).Value = textLanguages.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Language).Value = textLanguages.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Language).Value = textLanguages.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersLanguage).Value = textLanguages.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosLanguage).Value = textLanguages.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesLanguage).Value = textLanguages.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartLanguage).Value = textLanguages.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerLanguage).Value = textLanguages.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguageAll).Value = textLanguagesAll.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguageAll).Value = textLanguagesAll.Text;
          break;
      }
    }


    private void radioButtonFR_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = _frenchCulture;
      ApplyCulture(_frenchCulture);
    }

    private void ApplyCulture(CultureInfo culture)
    {
      // Applies culture to current Thread.
      System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

      // Create a resource manager for this Form and determine its fields via reflection.
      ComponentResourceManager resources = new ComponentResourceManager(GetType());
      FieldInfo[] fieldInfos = GetType().GetFields(BindingFlags.Instance |
          BindingFlags.DeclaredOnly | BindingFlags.NonPublic);

      // Call SuspendLayout for Form and all fields derived from Control, so assignment of 
      //   localized text doesn't change layout immediately.
      SuspendLayout();
      foreach (FieldInfo t in fieldInfos.Where(t => t.FieldType.IsSubclassOf(typeof(Control))))
      {
        t.FieldType.InvokeMember("SuspendLayout",
          BindingFlags.InvokeMethod, null,
          t.GetValue(this), null);
      }

      // If available, assign localized text to Form and fields with Text property.
      String text = resources.GetString("$this.Text");
      if (text != null)
        Text = text;
      foreach (FieldInfo t in fieldInfos.Where(t => t.FieldType.GetProperty("Text", typeof(String)) != null))
      {
        text = resources.GetString(t.Name + ".Text");
        if (text != null)
        {
          t.FieldType.InvokeMember("Text",
            BindingFlags.SetProperty, null,
            t.GetValue(this), new object[] { text });
        }
      }

      // Call ResumeLayout for Form and all fields derived from Control to resume layout logic.
      // Call PerformLayout, so layout changes due to assignment of localized text are performed.
      foreach (FieldInfo t in fieldInfos.Where(t => t.FieldType.IsSubclassOf(typeof(Control))))
      {
        t.FieldType.InvokeMember("ResumeLayout",
          BindingFlags.InvokeMethod, null,
          t.GetValue(this), new object[] { false });
      }
      ResumeLayout(true);

    }

    private void radioButtonEN_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = _englishCulture;
      ApplyCulture(_englishCulture);
    }

    private void textDReplace_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Param1 = textDReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Param1 = textDReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Param1 = textDReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Param1 = textDReplace.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Param1 = textDReplace.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Param1 = textDReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Param1 = textDReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Param1 = textDReplace.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Param1 = textDReplace.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Param1 = textDReplace.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Param1 = textDReplace.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Param1 = textDReplace.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Param1 = textDReplace.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Param1 = textDReplace.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Param1 = textDReplace.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Param1 = textDReplace.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Param1 = textDReplace.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Param1 = textDReplace.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Param1 = textDReplace.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Param1 = textDReplace.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Param1 = textDReplace.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Param1 = textDReplace.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Param1 = textDReplace.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Param1 = textDReplace.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Param1 = textDReplace.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param1 = textDReplace.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param1 = textDReplace.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param1 = textDReplace.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param1 = textDReplace.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param1 = textDReplace.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param1 = textDReplace.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param1 = textDReplace.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param1 = textDReplace.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param1 = textDReplace.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param1 = textDReplace.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param1 = textDReplace.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param1 = textDReplace.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param1 = textDReplace.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param1 = textDReplace.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Param1 = textDReplace.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Param1 = textDReplace.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Param1 = textDReplace.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Param1 = textDReplace.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Param1 = textDReplace.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Param1 = textDReplace.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Param1 = textDReplace.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Param1 = textDReplace.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Param2 = textDReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Param2 = textDReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Param2 = textDReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Param2 = textDReplaceWith.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Param2 = textDReplaceWith.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Param2 = textDReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Param2 = textDReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Param2 = textDReplaceWith.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Param2 = textDReplaceWith.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Param2 = textDReplaceWith.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Param2 = textDReplaceWith.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Param2 = textDReplaceWith.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Param2 = textDReplaceWith.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Param2 = textDReplaceWith.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Param2 = textDReplaceWith.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Param2 = textDReplaceWith.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Param2 = textDReplaceWith.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Param2 = textDReplaceWith.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Param2 = textDReplaceWith.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Param2 = textDReplaceWith.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Param2 = textDReplaceWith.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Param2 = textDReplaceWith.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Param2 = textDReplaceWith.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Param2 = textDReplaceWith.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Param2 = textDReplaceWith.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartDetails2).Param2 = textDReplaceWith.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric1).Param2 = textDReplaceWith.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkGeneric2).Param2 = textDReplaceWith.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkImg).Param2 = textDReplaceWith.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersons).Param2 = textDReplaceWith.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Param2 = textDReplaceWith.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkCertification).Param2 = textDReplaceWith.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkComment).Param2 = textDReplaceWith.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkSyn).Param2 = textDReplaceWith.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiPosters).Param2 = textDReplaceWith.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPhotos).Param2 = textDReplaceWith.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkPersonImages).Param2 = textDReplaceWith.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkMultiFanart).Param2 = textDReplaceWith.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTrailer).Param2 = textDReplaceWith.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiPosters).Param2 = textDReplaceWith.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPhotos).Param2 = textDReplaceWith.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartPersonImages).Param2 = textDReplaceWith.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartMultiFanart).Param2 = textDReplaceWith.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTrailer).Param2 = textDReplaceWith.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Param2 = textDReplaceWith.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Param2 = textDReplaceWith.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Param2 = textDReplaceWith.Text;
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
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Param1 = textReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Param1 = textReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Param1 = textReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Param1 = textReplace.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Param1 = textReplace.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Param1 = textReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Param1 = textReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Param1 = textReplace.Text;
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
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartTitle).Param2 = textReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartYear).Param2 = textReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartDirector).Param2 = textReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartLink).Param2 = textReplaceWith.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartID).Param2 = textReplaceWith.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartOptions).Param2 = textReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartAkas).Param2 = textReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListSearch, TagName.KeyStartThumb).Param2 = textReplaceWith.Text;
          break;
      }
      //if (comboBox1.SelectedIndex > 0)
      //    textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);
    }
    private int ExtractBody(string body, string paramStart)
    {
      string strStart = string.Empty;
      switch (paramStart)
      {
        case "Original Title":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartOTitle).Value;
          break;
        case "Translated Title":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTTitle).Value;
          break;
        case "URL cover":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartImg).Value;
          break;
        case "Rate 1":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate).Value;
          break;
        case "Rate 2":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRate2).Value;
          break;
        case "Synopsys":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartSyn).Value;
          break;
        case "Director":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRealise).Value;
          break;
        case "Producer":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartProduct).Value;
          break;
        case "Actors":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCredits).Value;
          break;
        case "Country":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCountry).Value;
          break;
        case "Category":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGenre).Value;
          break;
        case "Year":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartYear).Value;
          break;
        // New Added
        case "Comment":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartComment).Value;
          break;
        case "Language":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLanguage).Value;
          break;
        case "Tagline":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTagline).Value;
          break;
        case "Certification":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCertification).Value;
          break;
        case "Writer":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartWriter).Value;
          break;
        case "Studio":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartStudio).Value;
          break;
        case "Edition":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartEdition).Value;
          break;
        case "IMDB_Rank":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Rank).Value;
          break;
        case "IMDB_Id":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartIMDB_Id).Value;
          break;
        case "TMDB_Id":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartTMDB_Id).Value;
          break;
        case "Runtime":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartRuntime).Value;
          break;
        case "Collection":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollection).Value;
          break;
        case "CollectionImageURL":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartCollectionImageURL).Value;
          break;
        case "Generic Field 1":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric1).Value;
          break;
        case "Generic Field 2":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric2).Value;
          break;
        case "Generic Field 3":
          strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartGeneric3).Value;
          break;
        //case "TitlesURL":
        //  strStart = xmlConf.find(xmlConf.ListDetail, TagName.KeyStartLinkTitles).Value;
        //  break;
      }
      return strStart.Length > 0 ? body.IndexOf(strStart) : 0;
    }

    private void Index_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyOTitleIndex).Value = Index.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleIndex).Value = Index.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyImgIndex).Value = Index.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRateIndex).Value = Index.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRate2Index).Value = Index.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseIndex).Value = Index.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyProductIndex).Value = Index.Text;
          break;
        case 8: // writer
          xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterIndex).Value = Index.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsIndex).Value = Index.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryIndex).Value = Index.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGenreIndex).Value = Index.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyYearIndex).Value = Index.Text;
          break;
        case 13: // added for IMDB_Id
          xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_IdIndex).Value = Index.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeySynIndex).Value = Index.Text;
          break;
        case 15: // added for comments
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentIndex).Value = Index.Text;
          break;
        case 16: // added for languages
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguageIndex).Value = Index.Text;
          break;
        case 17: // added for tagline
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTaglineIndex).Value = Index.Text;
          break;
        case 18: // added for certification 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationIndex).Value = Index.Text;
          break;
        case 19: // added for Studio
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioIndex).Value = Index.Text;
          break;
        case 20: // added for Edition
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEditionIndex).Value = Index.Text;
          break;
        case 21: // added for IMDB_Rank
          xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_RankIndex).Value = Index.Text;
          break;
        case 22: // added for TMDB_Id
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTMDB_IdIndex).Value = Index.Text;
          break;
        case 23: // added for Generic Field 1
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Index).Value = Index.Text;
          break;
        case 24: // added for Generic Field 2
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Index).Value = Index.Text;
          break;
        case 25: // added for Generic Field 3
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Index).Value = Index.Text;
          break;
        case 26: // added for details base page 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Index).Value = Index.Text;
          break;
        case 27: // added for secondary Generic 1 linkpage
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Index).Value = Index.Text;
          break;
        case 28: // added for secondary Generic 2 linkpage
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Index).Value = Index.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgIndex).Value = Index.Text;
          break;
        case 30: // added for secondary persons page
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsIndex).Value = Index.Text;
          break;
        case 31: // added for secondary titles page 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesIndex).Value = Index.Text;
          break;
        case 32: // added for secondary certification 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationIndex).Value = Index.Text;
          break;
        case 33: // added for secondary comment 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentIndex).Value = Index.Text;
          break;
        case 34: // added for secondary Synopsis/description 
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynIndex).Value = Index.Text;
          break;

        case 35: // added for secondary MultiPosters
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersIndex).Value = Index.Text;
          break;
        case 36: // added for secondary Photos
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosIndex).Value = Index.Text;
          break;
        case 37: // added for secondary PersonImages
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesIndex).Value = Index.Text;
          break;
        case 38: // added for secondary MultiFanart
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartIndex).Value = Index.Text;
          break;
        case 39: // added for secondary Trailer
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerIndex).Value = Index.Text;
          break;

        case 40: // added for MultiPosters
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersIndex).Value = Index.Text;
          break;
        case 41: // added for Photos
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosIndex).Value = Index.Text;
          break;
        case 42: // added for PersonImages
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesIndex).Value = Index.Text;
          break;
        case 43: // added for MultiFanart
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartIndex).Value = Index.Text;
          break;
        case 44: // added for Trailer
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerIndex).Value = Index.Text;
          break;
        case 45: // added for Runtime (by web)
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRuntimeIndex).Value = Index.Text;
          break;
        case 46: // added for Collection
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionIndex).Value = Index.Text;
          break;
        case 47: // added for Collection Image URL
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionImageURLIndex).Value = Index.Text;
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyOTitlePage).Value = URLpage.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitlePage).Value = URLpage.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyImgPage).Value = URLpage.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRatePage).Value = URLpage.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRate2Page).Value = URLpage.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRealisePage).Value = URLpage.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyProductPage).Value = URLpage.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterPage).Value = URLpage.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsPage).Value = URLpage.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCountryPage).Value = URLpage.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGenrePage).Value = URLpage.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyYearPage).Value = URLpage.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_IdPage).Value = URLpage.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.ListDetail, TagName.KeySynPage).Value = URLpage.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCommentPage).Value = URLpage.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLanguagePage).Value = URLpage.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTaglinePage).Value = URLpage.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationPage).Value = URLpage.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyStudioPage).Value = URLpage.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEditionPage).Value = URLpage.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyIMDB_RankPage).Value = URLpage.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTMDB_IdPage).Value = URLpage.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric1Page).Value = URLpage.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric2Page).Value = URLpage.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyGeneric3Page).Value = URLpage.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyDetails2Page).Value = URLpage.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric1Page).Value = URLpage.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkGeneric2Page).Value = URLpage.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkImgPage).Value = URLpage.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonsPage).Value = URLpage.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTitlesPage).Value = URLpage.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCertificationPage).Value = URLpage.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkCommentPage).Value = URLpage.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkSynPage).Value = URLpage.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiPostersPage).Value = URLpage.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPhotosPage).Value = URLpage.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkPersonImagesPage).Value = URLpage.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkMultiFanartPage).Value = URLpage.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyLinkTrailerPage).Value = URLpage.Text;
          break;

        case 40:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiPostersPage).Value = URLpage.Text;
          break;
        case 41:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPhotosPage).Value = URLpage.Text;
          break;
        case 42:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyPersonImagesPage).Value = URLpage.Text;
          break;
        case 43:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyMultiFanartPage).Value = URLpage.Text;
          break;
        case 44:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyTrailerPage).Value = URLpage.Text;
          break;
        case 45:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyRuntimePage).Value = URLpage.Text;
          break;
        case 46:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionPage).Value = URLpage.Text;
          break;
        case 47:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyCollectionImageURLPage).Value = URLpage.Text;
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

        string find = textDReplace.Text.Length > 0 ? GrabUtil.FindWithAction(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text, textDReplace.Text, textDReplaceWith.Text, textComplement.Text, textMaxItems.Text, textLanguages.Text, out allNames, out allRoles, chkActorRoles.Checked) : GrabUtil.Find(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text);

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
            //try { System.IO.file.Delete(strTemp); }
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

        if (xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value.Length > 0)
        {
          iStart = BodyDetail.IndexOf(xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            iEnd = xmlConf.find(xmlConf.ListDetail, TagName.KeyEndBody).Value != "" ? BodyDetail.IndexOf(xmlConf.find(xmlConf.ListDetail, TagName.KeyEndBody).Value, iStart) : BodyDetail.Length;

            if (iEnd == -1) iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.ListDetail, TagName.KeyStartBody).Value.Length;
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

    private void pictureBoxFranceFlag_Click(object sender, EventArgs e)
    {

    }

    private void pictureBoxUSFlag_Click(object sender, EventArgs e)
    {

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
      openFileDialog1.Title = "Load HTML file";
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
      //List<ListNode> list = xmlConf.ListDetail;
      //string value = TagName.KeyCreditsMaxItems;
      //if (xmlConf.ListDetail != null && TagName.KeyCreditsMaxItems != null && cbMaxActors.Text != null && cbMaxActors != null)
      if (cbMaxActors.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsMaxItems).Value = cbMaxActors.Text;
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      xmlConf.find(xmlConf.ListDetail, TagName.KeyCreditsGrabActorRoles).Value = chkGrabActorRoles.Checked ? "true" : "false";
    }

    private void cbMaxProducers_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxProducers.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyProductMaxItems).Value = cbMaxProducers.Text;
    }

    private void cbMaxDirectors_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxDirectors.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyRealiseMaxItems).Value = cbMaxDirectors.Text;
    }

    private void cbMaxWriters_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxWriters.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyWriterMaxItems).Value = cbMaxWriters.Text;
    }

    private void cbTtitlePreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleLanguage).Value = cbTtitlePreferredLanguage.Text;
    }

    private void cbTtitleMaxTitles_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbTtitleMaxTitles.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyTTitleMaxItems).Value = cbTtitleMaxTitles.Text;
    }

    private void cbCertificationPreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbCertificationPreferredLanguage.SelectedIndex != -1)
        xmlConf.find(xmlConf.ListDetail, TagName.KeyCertificationLanguage).Value = cbCertificationPreferredLanguage.Text;
    }

    private void InitMappingTable()
    {
      List<string> fields = GrabberUrlClass.FieldList();
      for (int i = 0; i < 40; i++)
      {
        Fields[i] = fields[i];
      }

      Column2.Items.Clear();
      Column2.Items.Add(""); // empty field to choose ....
      foreach (string field in Fields.Where(field => !string.IsNullOrEmpty(field) && !field.Contains("URL") && !field.Contains("All ")))
      {
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
            ProcessStartInfo psi = new ProcessStartInfo
            {
              FileName = "notepad.exe",
              UseShellExecute = true,
              WindowStyle = ProcessWindowStyle.Normal,
              Arguments = "\"" + textURLPreview.Text + "\"",
              ErrorDialog = true
            };
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
      if (TextURL.Text.Contains("#page#") && (textPage.Text.Length == 0))
      {
        MessageBox.Show("Please, give the page number to load !", "Error");
        return;
      }

      // listPreview.Items.Clear();
      if (TextURL.Text.Length > 0)
      {
        if (TextURL.Text.StartsWith("http://") == false) TextURL.Text = "http://" + TextURL.Text;
        string strSearch = TextSearch.Text;
        strSearch = GrabUtil.ReplaceNormalOrRegex(strSearch, textSearchCleanup.Text);
        strSearch = GrabUtil.EncodeSearch(strSearch, textEncoding.Text);
        string wurl = TextURL.Text.Replace("#Search#", strSearch);
        wurl = wurl.Replace("#page#", textPage.Text);
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
          string filepath = dataGridViewSearchResults["ResultColumn6", e.RowIndex].Value.ToString();
          if (!filepath.Contains(".nfo"))
            Process.Start(filepath);
          else
          {
            using (Process p = new Process())
            {
              ProcessStartInfo psi = new ProcessStartInfo
              {
                FileName = "notepad.exe",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
                Arguments = "\"" + filepath + "\"",
                ErrorDialog = true
              };
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
      openFileDialog1.FileName = !string.IsNullOrEmpty(TextSearch.Text) ? TextSearch.Text : String.Empty;
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
          SwapTabPages(tabpage, tabControl1.TabPages[tabControl1.TabPages.IndexOf(tabpage) - 1]);
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
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingDetails2).Value = EncodingSubPage.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric1).Value = EncodingSubPage.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkGeneric2).Value = EncodingSubPage.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkImg).Value = EncodingSubPage.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersons).Value = EncodingSubPage.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTitles).Value = EncodingSubPage.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkCertification).Value = EncodingSubPage.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkComment).Value = EncodingSubPage.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkSyn).Value = EncodingSubPage.Text;
          break;

        case 35:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiPosters).Value = EncodingSubPage.Text;
          break;
        case 36:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPhotos).Value = EncodingSubPage.Text;
          break;
        case 37:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkPersonImages).Value = EncodingSubPage.Text;
          break;
        case 38:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkMultiFanart).Value = EncodingSubPage.Text;
          break;
        case 39:
          xmlConf.find(xmlConf.ListDetail, TagName.KeyEncodingLinkTrailer).Value = EncodingSubPage.Text;
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
        DataObject dataObject = new DataObject();

        // Add the data in various formats.
        dataObject.SetData(DataFormats.Rtf, textBody.Rtf);
        dataObject.SetData(DataFormats.Text, textBody.Text);

        // Copy data to the Clipboard
        Clipboard.SetDataObject(dataObject);
      }
      catch (Exception) { }
    }

    private void toolStripMenuDetailsCopyAll_Click(object sender, EventArgs e)
    {
      try
      {
        // Make a DataObject.
        DataObject dataObject = new DataObject();

        // Add the data in various formats.
        dataObject.SetData(DataFormats.Rtf, textBodyDetail.Rtf);
        dataObject.SetData(DataFormats.Text, textBodyDetail.Text);

        // Copy data to the Clipboard
        Clipboard.SetDataObject(dataObject);
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
        // lblCleanup.Visible = false;
        // textSearchCleanup.Visible = false;
        textUserAgent.Visible = false;
        textAccept.Visible = false;
        textHeaders.Visible = false;
        label17.Visible = false;
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
        cbIgnoreCase.Visible = false;
      }
      else
      {
        label8.Text = "#Search#";
        btnLoadDetailInWeb.Text = "Web";
        textRedir.Visible = true;
        // lblCleanup.Visible = true;
        // textSearchCleanup.Visible = true;
        textUserAgent.Visible = true;
        textAccept.Visible = true;
        textHeaders.Visible = true;
        label17.Visible = true;
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
        cbIgnoreCase.Visible = true;
      }
    }

  }
}