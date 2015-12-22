namespace MyVideoGrabber
{
  using System;
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.IO;
  using MediaPortal.Configuration;

  public partial class MultiGrabberForm : Form
  {
    public MultiGrabberForm()
    {
      InitializeComponent();
      System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
      label_Version.Text = "V" + asm.GetName().Version;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog1 = new OpenFileDialog
      {
        RestoreDirectory = true,
        InitialDirectory = Config.GetFolder(Config.Dir.Config) + "\\MyVideoGrabberScripts",
        Filter = "Load xml (*.xml)|*.xml"
      };

      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        listView1.Items.Add(openFileDialog1.FileName);
        //textBox1.Text = openFileDialog1.FileName;        
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var tw = new XmlTextWriter(Config.GetFolder(Config.Dir.Config) + "\\MyVideoGrabber.xml", Encoding.UTF8)
      {
        Formatting = Formatting.Indented
      };
      tw.WriteStartDocument(true);
      tw.WriteStartElement("Profile");
      tw.WriteStartElement("Section");

      for (int i = 0; i < listView1.Items.Count; i++)
      {
        tw.WriteStartElement("ConfigFile");
        tw.WriteString(listView1.Items[i].SubItems[0].Text);
        tw.WriteEndElement();
      }

      tw.WriteEndElement();
      tw.WriteEndElement();
      tw.WriteEndDocument();
      tw.Flush();
      tw.Close();

      Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void MyVideoGrabberForm_Load(object sender, EventArgs e)
    {
      if (File.Exists(Config.GetFolder(Config.Dir.Config) + "\\MyVideoGrabber.xml"))
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(Config.GetFolder(Config.Dir.Config) + "\\MyVideoGrabber.xml");
        XmlNode n = doc.ChildNodes[1].FirstChild;

        for (int i = 0; i < n.ChildNodes.Count; i++)
        {
          listView1.Items.Add(n.ChildNodes[i].InnerText);
        }

      }
    }

    private void button4_Click(object sender, EventArgs e)
    {
      ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
      // guilty entry?
      if (indexes.Count == 1)
      {
        int index = indexes[0];
        // not the first entry?
        if (index > 0)
        {
          // save current text
          string strSub0 = listView1.Items[index - 1].SubItems[0].Text;

          // copy text
          listView1.Items[index - 1].SubItems[0].Text = listView1.Items[index].SubItems[0].Text;
          // restore backuped text
          listView1.Items[index].SubItems[0].Text = strSub0;

          // move the selection up
          listView1.Items[index].Selected = false;
          listView1.Items[index - 1].Selected = true;
        }

      }
    }

    private void button5_Click(object sender, EventArgs e)
    {
      // Moves the selected entry down
      // get the entry
      ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
      // guilty entry?
      if (indexes.Count == 1)
      {
        int index = indexes[0];
        // not the last entry?
        if (index < listView1.Items.Count - 1)
        {
          // save current text
          string strSub0 = listView1.Items[index + 1].SubItems[0].Text;
          // copy text
          listView1.Items[index + 1].SubItems[0].Text = listView1.Items[index].SubItems[0].Text;
          // restore backuped text
          listView1.Items[index].SubItems[0].Text = strSub0;
          // move the selection down
          listView1.Items[index].Selected = false;
          listView1.Items[index + 1].Selected = true;
        }
      }
    }

    private void button6_Click(object sender, EventArgs e)
    {
      if (listView1.SelectedIndices.Count > 0)
      {
        string strSub0 = listView1.SelectedItems[0].SubItems[0].Text;
        int index = listView1.SelectedItems[0].Index;
        listView1.Items.Remove(listView1.SelectedItems[0]);
        listView1.Update();
        if (listView1.Items.Count > 0)
        {
          if (index >= listView1.Items.Count)
          {
            index = listView1.Items.Count - 1;
          }
          listView1.SelectedIndices.Clear();
          listView1.SelectedIndices.Add(index);
        }

      }

    }

  }
}