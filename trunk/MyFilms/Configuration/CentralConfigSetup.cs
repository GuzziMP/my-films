﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyFilmsPlugin.Configuration
{
  using MediaPortal.Configuration;

  using MyFilmsPlugin.MyFilms.Utils;

  public partial class CentralConfigSetup : Form
  {
    public CentralConfigSetup()
    {
      InitializeComponent();
    }

    private void CentralConfigSetup_Load(object sender, EventArgs e)
    {
      XmlConfig MyFilmsServer = new XmlConfig();
      MyFilmsCentralConfigDir.Text = MyFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", "");
      cbSyncFromServerOnStartup.Checked = MyFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
    }

    private void cbSyncFromServerOnStartup_CheckedChanged(object sender, EventArgs e)
    {
      if (cbSyncFromServerOnStartup.Checked)
      {
        // first check if a remote path is configured and available
        string serverConfigFile = MyFilmsCentralConfigDir.Text + @"\MyFilms.xml";
        if (!System.IO.Directory.Exists(MyFilmsCentralConfigDir.Text))
        {
          MessageBox.Show("Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          cbSyncFromServerOnStartup.Checked = false;
          return;
        }
        if (!System.IO.File.Exists(serverConfigFile))
        {
          MessageBox.Show("Remote MyFilms.xml not found - make sure you uploaded a config first !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          cbSyncFromServerOnStartup.Checked = false;
          return;
        }
        // all ok, set color for indication to red and save
        cbSyncFromServerOnStartup.ForeColor = System.Drawing.Color.Red;
        SaveCentralConfig();
      }
      else
      {
        cbSyncFromServerOnStartup.ResetForeColor();
        SaveCentralConfig();
      }
    }

    private void SaveCentralConfig()
    {
      XmlConfig MyFilmsServer = new XmlConfig();
      MyFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", MyFilmsCentralConfigDir.Text);
      MyFilmsServer.WriteXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", cbSyncFromServerOnStartup.Checked);
      MyFilmsServer.Save();
    }

    private void btnSyncToServer_Click(object sender, EventArgs e)
    {
      string serverConfigFile = MyFilmsCentralConfigDir.Text + @"\MyFilms.xml";
      string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
      if (!System.IO.Directory.Exists(MyFilmsCentralConfigDir.Text))
      {
        MessageBox.Show(
          "Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show("Successfully copied local config to remote directory !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception)
      {
        MessageBox.Show("Cannot write to Server directory - Missing access rights? - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnSyncFromServer_Click(object sender, EventArgs e)
    {
      string serverConfigFile = MyFilmsCentralConfigDir.Text + @"\MyFilms.xml";
      string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
      if (!System.IO.Directory.Exists(MyFilmsCentralConfigDir.Text))
      {
        MessageBox.Show("Your remote directory does not exist - cannot continue !\nPlease make sure the path is existing and accessible.", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      if (!System.IO.File.Exists(serverConfigFile))
      {
        MessageBox.Show("Remote MyFilms.xml not found - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        catch (Exception)
        {
          MessageBox.Show("Cannot write to local directory - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }
      try
      {
        System.IO.File.Copy(serverConfigFile, localConfigFile, true);
        MessageBox.Show("Successfully copied remote config to local directory !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        MessageBox.Show("MyFilms Setup will now reload the updated local MyFilms.xml config file !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      catch (Exception)
      {
        MessageBox.Show("Cannot copy to local directory - cannot continue !", "MyFilms Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnMyFilmsCentralConfigFile_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
      if (!String.IsNullOrEmpty(MyFilmsCentralConfigDir.Text))
      {
        folderBrowserDialog1.SelectedPath = MyFilmsCentralConfigDir.Text;
        if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
      }
      else
        folderBrowserDialog1.SelectedPath = String.Empty;
      folderBrowserDialog1.Description = "Path for Central Config File";
      if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
      {
        if (folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != folderBrowserDialog1.SelectedPath.Length - 1)
          folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";
        MyFilmsCentralConfigDir.Text = folderBrowserDialog1.SelectedPath;
      }
      this.SaveCentralConfig();
    }

    private void btnQuit_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Abort;
      this.Close();
    }

    private void CentralConfigSetup_Leave(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Abort;
      this.Close();
    }

    private void btnResetRemotePath_Click(object sender, EventArgs e)
    {
      MyFilmsCentralConfigDir.Text = "";
      this.SaveCentralConfig();
    }

  }
}
