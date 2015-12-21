using System;
using System.Data;
using System.Windows.Forms;

namespace MyFilmsPlugin.Configuration
{
  using DataBase;

  public partial class FilterEditor : Form
  {
    public FilterEditor()
    {
      InitializeComponent();
      using (var ds = new AntMovieCatalog())
      {
        AntFilterItem1.Items.Add("(none)");
        AntFilterItem2.Items.Add("(none)");
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          if (dc.ColumnName != "Contents_Id" && dc.ColumnName != "Movie_Id" && dc.ColumnName != "IsOnline" &&
              dc.ColumnName != "IsOnlineTrailer" && dc.ColumnName != "LastPosition" && dc.ColumnName != "Picture" &&
              dc.ColumnName != "Fanart" &&
              (ExtendedFields ||
               (dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" && dc.ColumnName != "Watched" &&
                dc.ColumnName != "Certification" && dc.ColumnName != "Writer" && dc.ColumnName != "SourceTrailer" &&
                dc.ColumnName != "TagLine" && dc.ColumnName != "Tags" && dc.ColumnName != "RatingUser" &&
                dc.ColumnName != "Studio" && dc.ColumnName != "IMDB_Rank" && dc.ColumnName != "Edition" &&
                dc.ColumnName != "Aspectratio" && dc.ColumnName != "CategoryTrakt" && dc.ColumnName != "Favorite")))
          {
            if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded") // added "DatedAdded" to remove filter
            {
              AntFilterItem1.Items.Add(dc.ColumnName);
              AntFilterItem2.Items.Add(dc.ColumnName);
            }
          }
        }
      }
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    private bool extendedFields = true;
    public bool ExtendedFields
    {
      get { return extendedFields; }
      set { extendedFields = value; }
    }

    public string FilterItem1
    {
      get { return AntFilterItem1.Text; }
      set { AntFilterItem1.Text = value; }
    }

    public string FilterSign1
    {
      get { return AntFilterSign1.Text; }
      set { AntFilterSign1.Text = value; }
    }

    public string FilterText1
    {
      get { return AntFilterText1.Text; }
      set { AntFilterText1.Text = value; }
    }

    public string FilterItem2
    {
      get { return AntFilterItem2.Text; }
      set { AntFilterItem2.Text = value; }
    }

    public string FilterSign2
    {
      get { return AntFilterSign2.Text; }
      set { AntFilterSign2.Text = value; }
    }

    public string FilterText2
    {
      get { return AntFilterText2.Text; }
      set { AntFilterText2.Text = value; }
    }

    public string FilterFreeText
    {
      get { return AntFilterFreeText.Text; }
      set { AntFilterFreeText.Text = value; }
    }

    public string FilterComb
    {
      get { return AntFilterComb.Text; }
      set { AntFilterComb.Text = value; }
    }

    public string ConfigString
    {
      get { return ExpressionPreview.Text; }
      set { ExpressionPreview.Text = value; }
    }

    public string StrDfltSelect { get; set; }

    private string StrViewFilterSelect;

    public string MasterTitle { get; set; }

    private void btnReset_Click(object sender, EventArgs e)
    {
      AntFilterItem1.ResetText();
      AntFilterItem2.ResetText();
      AntFilterText1.ResetText();
      AntFilterText2.ResetText();
      AntFilterFreeText.ResetText();
      AntFilterSign1.ResetText();
      AntFilterSign2.ResetText();
      AntFilterComb.ResetText();
      ExpressionPreview.ResetText();
    }

    private void AntFilterItem1_SelectedIndexChanged(object sender, EventArgs e)
    {

      if (AntFilterItem1.Text == "(none)")
        AntFilterText1.Clear();
    }

    private void AntFilterItem2_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (AntFilterItem2.Text == "(none)")
        AntFilterText2.Clear();
    }

    private void Selected_Enreg_TextChanged()
    {
      StrViewFilterSelect = "";
      string wAntFilterSign;
      wAntFilterSign = AntFilterSign1.Text == "#" ? "<>" : AntFilterSign1.Text;
      if ((AntFilterItem1.Text.Length > 0) && AntFilterItem1.Text != "(none)")
        if (AntFilterItem1.Text == "DateAdded")
          switch (AntFilterSign1.Text)
          {
            case "not like":
            case "#":
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# or " + AntFilterItem1.Text + " is null) ";
              break;
            case "not in":
            case "in":
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText1.Text, true) + ")) ";
              break;
            case "like in":
              StrViewFilterSelect = "(" + TransformedLikeIn(AntFilterItem1.Text, AntFilterText1.Text, true) + ") ";
              break;
            default:
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# ) ";
              break;
          }
        else
          switch (AntFilterSign1.Text)
          {
            case "not like":
            case "#":
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "' or " + AntFilterItem1.Text + " is null) ";
              break;
            case "not in":
            case "in":
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText1.Text, false) + ")) ";
              break;
            case "like in":
              StrViewFilterSelect = "(" + TransformedLikeIn(AntFilterItem1.Text, AntFilterText1.Text, false) + ") ";
              break;
            default:
              StrViewFilterSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "') ";
              break;
          }
      if ((AntFilterComb.Text == "or") && (StrViewFilterSelect.Length > 0))
        StrViewFilterSelect = StrViewFilterSelect + " OR ";
      else
        if (StrViewFilterSelect.Length > 0)
          StrViewFilterSelect = StrViewFilterSelect + " AND ";

      wAntFilterSign = AntFilterSign2.Text == "#" ? "<>" : AntFilterSign2.Text;

      if ((AntFilterItem2.Text.Length > 0) && AntFilterItem2.Text != "(none)")
        if (AntFilterItem2.Text == "DateAdded")
          switch (AntFilterSign2.Text)
          {
            case "not like":
            case "#":
              StrViewFilterSelect = "(" + StrViewFilterSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# or " + AntFilterItem2.Text + " is null)) AND ";
              break;
            case "not in":
            case "in":
              StrViewFilterSelect = "(" + AntFilterItem2.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText2.Text, true) + ")) AND ";
              break;
            case "like in":
              StrViewFilterSelect = "(" + TransformedLikeIn(AntFilterItem2.Text, AntFilterText2.Text, true) + ") AND ";
              break;
            default:
              StrViewFilterSelect = "(" + StrViewFilterSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# )) AND ";
              break;
          }
        else
          switch (AntFilterSign2.Text)
          {
            case "not like":
            case "#":
              StrViewFilterSelect = "(" + StrViewFilterSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' or " + AntFilterItem2.Text + " is null)) AND ";
              break;
            case "not in":
            case "in":
              StrViewFilterSelect = "(" + AntFilterItem2.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText2.Text, false) + ")) AND ";
              break;
            case "like in":
              StrViewFilterSelect = "(" + TransformedLikeIn(AntFilterItem2.Text, AntFilterText2.Text, true) + ") AND ";
              break;
            default:
              StrViewFilterSelect = "(" + StrViewFilterSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' )) AND ";
              break;
          }
      if (!string.IsNullOrEmpty(AntFilterFreeText.Text))
        StrViewFilterSelect = StrViewFilterSelect + AntFilterFreeText.Text + " AND ";
      StrDfltSelect = StrViewFilterSelect; // this is for config filters, inlcuding the "AND" at the end ...
      if (StrViewFilterSelect.EndsWith(" AND ")) StrViewFilterSelect = StrViewFilterSelect.Substring(0, StrViewFilterSelect.Length - 5);
      ExpressionPreview.Text = StrViewFilterSelect; //ExpressionPreview.Text = this.StrViewFilterSelect + masterTitle + " not like ''" + " AND ";
      LogMyFilms.Debug("MyFilms (Build Selected Enreg) - Selected_Enreg: '" + ExpressionPreview.Text + "'");
      ConfigString = ExpressionPreview.Text;
    }

    private string DBitemList(string inputstring, bool isdate)
    {
      string returnValue = string.Empty;
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

    private void btnPreview_Click(object sender, EventArgs e)
    {
      Selected_Enreg_TextChanged();
      CheckExpression();
    }

    private void CheckExpression()
    {
      if (AntFilterItem1.Text.Length == 0) AntFilterItem1.Text = "(none)";
      if (AntFilterItem1.Text != "(none)" && AntFilterSign1.Text.Length == 0)
      {
        MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterSign1.Focus();
        return;
      }
      if (AntFilterItem1.Text != "(none)" && AntFilterText1.Text.Length == 0)
      {
        MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterText1.Focus();
        return;
      }
      if (AntFilterItem2.Text.Length == 0) AntFilterItem2.Text = "(none)";
      if (AntFilterItem2.Text != "(none)" && AntFilterSign2.Text.Length == 0)
      {
        MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterSign2.Focus();
        return;
      }
      if (AntFilterItem2.Text != "(none)" && AntFilterText2.Text.Length == 0)
      {
        MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterText2.Focus();
        return;
      }
      if (AntFilterItem1.Text != "(none)" && AntFilterItem2.Text != "(none)")
      {
        if (AntFilterComb.Text.Length == 0)
        {
          MessageBox.Show("Must be 'or' or 'and' for filter combination", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          AntFilterComb.Focus();
          return;
        }
      }
      if (AntFilterItem1.Text == "DateAdded")
      {
        try
        {
          DateTime wdate = Convert.ToDateTime(AntFilterText1.Text);
        }
        catch
        {
          MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          AntFilterText1.Focus();
        }
      }
      if (AntFilterItem2.Text == "DateAdded")
      {
        try
        {
          DateTime wdate = Convert.ToDateTime(AntFilterText2.Text);
        }
        catch
        {
          MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          AntFilterText2.Focus();
        }
      }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Selected_Enreg_TextChanged();
      CheckExpression();
      DialogResult = DialogResult.OK;
      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }


  }

}
