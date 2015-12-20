using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using MyFilmsPlugin.MyFilms;

namespace MyFilmsPlugin.Configuration
{
  public partial class HyperLinkParamGenerator : Form
  {
    public HyperLinkParamGenerator()
    {
      InitializeComponent();
    }

    public string StartParameters
    {
          get { return tbEditorStartParamsOutput.Text; }
          set { tbEditorStartParamsOutput.Text = value; }
    }

    private void btnLoadEditorValues_Click(object sender, EventArgs e)
    {
      tbEditorStartParamsOutput.Text = "";
      cbEditorConfigs.Items.Clear();
      cbEditorViews.Items.Clear();
      cbEditorViewValues.Items.Clear();
      cbEditorConfigs.Text = "";
      cbEditorViews.Text = "";
      cbEditorViewValues.Text = "";

      ArrayList myFilmsEditor = BaseMesFilms.GetConfigViewLists();
      foreach (BaseMesFilms.MfConfig config in myFilmsEditor)
      {
        cbEditorConfigs.Items.Add(config.Name);
      }
    }

    private void cbEditorConfigs_SelectedIndexChanged(object sender, EventArgs e)
    {
      ArrayList myFilmsEditor = BaseMesFilms.GetConfigViewLists();
      tbEditorStartParamsOutput.Text = "";
      cbEditorViews.Items.Clear();
      cbEditorViewValues.Items.Clear();
      cbEditorViews.Text = "";
      cbEditorViewValues.Text = "";

      foreach (KeyValuePair<string, string> view in from BaseMesFilms.MfConfig config in myFilmsEditor where config.Name == cbEditorConfigs.Text from view in config.ViewList select view)
      {
        // key = viewname, value = translated viewname
        cbEditorViews.Items.Add(view.Value);
      }

    }

    private void cbEditorViews_SelectedIndexChanged(object sender, EventArgs e)
    {
      string viewCallName = "";
      tbEditorStartParamsOutput.Text = "";
      cbEditorViewValues.Items.Clear();
      cbEditorViewValues.Text = "";
      ArrayList myFilmsEditor = BaseMesFilms.GetConfigViewLists();
      foreach (KeyValuePair<string, string> view in from BaseMesFilms.MfConfig config in myFilmsEditor where config.Name == cbEditorConfigs.Text from view in config.ViewList where view.Value == cbEditorViews.Text select view)
      {
        viewCallName = view.Key;
      }

      if (!string.IsNullOrEmpty(cbEditorConfigs.Text) && !string.IsNullOrEmpty(viewCallName))
      {
        IEnumerable<string> viewValues = BaseMesFilms.GetViewListValues(cbEditorConfigs.Text, viewCallName);
        if (viewValues != null)
          foreach (string value in viewValues)
          {
            cbEditorViewValues.Items.Add(value);
          }
      }

    }

    private void btnSaveEditorStartParams_Click(object sender, EventArgs e)
    {
      string startParamOutput = "";
      ArrayList myFilmsEditor = BaseMesFilms.GetConfigViewLists();

      if (!string.IsNullOrEmpty(cbEditorConfigs.Text))
        startParamOutput += "config:" + cbEditorConfigs.Text;
      if (!string.IsNullOrEmpty(cbEditorViews.Text))
      {
        string viewCallName = "";
        if (!string.IsNullOrEmpty(startParamOutput)) startParamOutput += "|";
        foreach (KeyValuePair<string, string> view in from BaseMesFilms.MfConfig config in myFilmsEditor where config.Name == cbEditorConfigs.Text from view in config.ViewList where view.Value == cbEditorViews.Text select view)
        {
          viewCallName = view.Key;
        }
        startParamOutput += "view:" + viewCallName;
      }
      if (!string.IsNullOrEmpty(cbEditorViewValues.Text))
      {
        if (!string.IsNullOrEmpty(startParamOutput)) startParamOutput += "|";
        startParamOutput += "viewvalue:" + cbEditorViewValues.Text;
      }
      if (!string.IsNullOrEmpty(cbEditorLayout.Text))
      {
        if (!string.IsNullOrEmpty(startParamOutput)) startParamOutput += "|";
        startParamOutput += "layout:" + cbEditorLayout.SelectedIndex;
      }
      if (!string.IsNullOrEmpty(tbEditorSearchExpression.Text))
      {
        if (!string.IsNullOrEmpty(startParamOutput)) startParamOutput += "|";
        startParamOutput += "search:" + tbEditorSearchExpression.Text;
      }
      tbEditorStartParamsOutput.Text = startParamOutput;
      Clipboard.SetText(startParamOutput);
    }

    private void cbEditorLayout_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbEditorLayout.SelectedIndex < 0 || cbEditorLayout.SelectedIndex > 6)
        cbEditorLayout.SelectedIndex = 0;
      if (cbEditorLayout.SelectedIndex == 0)
        cbEditorLayout.Text = "";
    }
  }
}
