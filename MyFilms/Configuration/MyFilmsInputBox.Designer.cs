namespace MyFilmsPlugin.Configuration
{
    partial class MyFilmsInputBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyFilmsInputBox));
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxNewName = new System.Windows.Forms.TextBox();
      this.buttonOK = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.cbCatalogType = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.cbCountry = new System.Windows.Forms.ComboBox();
      this.lblCountry = new System.Windows.Forms.Label();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.cbUseNfoGrabber = new System.Windows.Forms.CheckBox();
      this.cbSetupType = new System.Windows.Forms.ComboBox();
      this.lblSetupType = new System.Windows.Forms.Label();
      this.lblNetworkClientInfo = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(22, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(255, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Please enter new name and select the catalog type !";
      // 
      // textBoxNewName
      // 
      this.textBoxNewName.Location = new System.Drawing.Point(112, 91);
      this.textBoxNewName.Name = "textBoxNewName";
      this.textBoxNewName.Size = new System.Drawing.Size(227, 20);
      this.textBoxNewName.TabIndex = 3;
      this.toolTip1.SetToolTip(this.textBoxNewName, "The config name must be unique - so make sure, \r\nyou don\'t use a name that alread" +
        "y exists.");
      this.textBoxNewName.TextChanged += new System.EventHandler(this.textBoxNewName_TextChanged);
      this.textBoxNewName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxNewName_KeyUp);
      // 
      // buttonOK
      // 
      this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.buttonOK.Location = new System.Drawing.Point(375, 171);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(74, 22);
      this.buttonOK.TabIndex = 8;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(22, 63);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(296, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Using an existing name overwrites the corresponding config  !";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::MyFilmsPlugin.Properties.Resources.film_reel_128x128;
      this.pictureBox1.Location = new System.Drawing.Point(367, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(85, 65);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // cbCatalogType
      // 
      this.cbCatalogType.FormattingEnabled = true;
      this.cbCatalogType.Items.AddRange(new object[] {
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
            "Ant Movie Catalog Xtended (V4.1)"});
      this.cbCatalogType.Location = new System.Drawing.Point(112, 132);
      this.cbCatalogType.Name = "cbCatalogType";
      this.cbCatalogType.Size = new System.Drawing.Size(227, 21);
      this.cbCatalogType.TabIndex = 5;
      this.toolTip1.SetToolTip(this.cbCatalogType, "Select the catalog type for this config.\r\nAMC 4.x is the \"most advacned\" and enab" +
        "led all features in MyFilms.\r\nChoose the catalog type based on your personal use" +
        " case.");
      this.cbCatalogType.SelectedIndexChanged += new System.EventHandler(this.cbCatalogType_SelectedIndexChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(22, 94);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(69, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Config name:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(22, 135);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(70, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Catalog Type";
      // 
      // cbCountry
      // 
      this.cbCountry.FormattingEnabled = true;
      this.cbCountry.Items.AddRange(new object[] {
            "Argentina",
            "Australia",
            "Austria",
            "Belgium",
            "Brazil",
            "Canada",
            "Chile",
            "China",
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
            "USA",
            "USA (Full Detail Grabbing)"});
      this.cbCountry.Location = new System.Drawing.Point(112, 172);
      this.cbCountry.Name = "cbCountry";
      this.cbCountry.Size = new System.Drawing.Size(227, 21);
      this.cbCountry.TabIndex = 7;
      this.toolTip1.SetToolTip(this.cbCountry, resources.GetString("cbCountry.ToolTip"));
      // 
      // lblCountry
      // 
      this.lblCountry.AutoSize = true;
      this.lblCountry.Location = new System.Drawing.Point(22, 175);
      this.lblCountry.Name = "lblCountry";
      this.lblCountry.Size = new System.Drawing.Size(43, 13);
      this.lblCountry.TabIndex = 6;
      this.lblCountry.Text = "Country";
      // 
      // cbUseNfoGrabber
      // 
      this.cbUseNfoGrabber.AutoSize = true;
      this.cbUseNfoGrabber.Location = new System.Drawing.Point(358, 134);
      this.cbUseNfoGrabber.Name = "cbUseNfoGrabber";
      this.cbUseNfoGrabber.Size = new System.Drawing.Size(100, 17);
      this.cbUseNfoGrabber.TabIndex = 9;
      this.cbUseNfoGrabber.Text = "use nfo grabber";
      this.toolTip1.SetToolTip(this.cbUseNfoGrabber, "When checked, nfo grabber will be set as fefault to retrieve info from your movie" +
        "s.\r\nIf unchecked, a grabber based on the country setting will be preset.");
      this.cbUseNfoGrabber.UseVisualStyleBackColor = true;
      // 
      // cbSetupType
      // 
      this.cbSetupType.FormattingEnabled = true;
      this.cbSetupType.Items.AddRange(new object[] {
            "Local Setup",
            "Central Setup - Master",
            "Central Setup - Client"});
      this.cbSetupType.Location = new System.Drawing.Point(112, 10);
      this.cbSetupType.Name = "cbSetupType";
      this.cbSetupType.Size = new System.Drawing.Size(165, 21);
      this.cbSetupType.TabIndex = 10;
      this.toolTip1.SetToolTip(this.cbSetupType, resources.GetString("cbSetupType.ToolTip"));
      this.cbSetupType.SelectedIndexChanged += new System.EventHandler(this.cbSetupType_SelectedIndexChanged);
      // 
      // lblSetupType
      // 
      this.lblSetupType.AutoSize = true;
      this.lblSetupType.Location = new System.Drawing.Point(22, 13);
      this.lblSetupType.Name = "lblSetupType";
      this.lblSetupType.Size = new System.Drawing.Size(62, 13);
      this.lblSetupType.TabIndex = 11;
      this.lblSetupType.Text = "Setup Type";
      // 
      // lblNetworkClientInfo
      // 
      this.lblNetworkClientInfo.AutoSize = true;
      this.lblNetworkClientInfo.Location = new System.Drawing.Point(97, 90);
      this.lblNetworkClientInfo.Name = "lblNetworkClientInfo";
      this.lblNetworkClientInfo.Size = new System.Drawing.Size(235, 39);
      this.lblNetworkClientInfo.TabIndex = 13;
      this.lblNetworkClientInfo.Text = "When choosing network client, \r\nyou have to choose the directory on your server \r" +
    "\nwhere you have stored your central config file.";
      // 
      // MyFilmsInputBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(470, 213);
      this.Controls.Add(this.lblNetworkClientInfo);
      this.Controls.Add(this.lblSetupType);
      this.Controls.Add(this.cbSetupType);
      this.Controls.Add(this.cbUseNfoGrabber);
      this.Controls.Add(this.lblCountry);
      this.Controls.Add(this.cbCountry);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.cbCatalogType);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.textBoxNewName);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MyFilmsInputBox";
      this.Text = "MyFilmsInputBox";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNewName;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cbCatalogType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbCountry;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbUseNfoGrabber;
        private System.Windows.Forms.ComboBox cbSetupType;
        private System.Windows.Forms.Label lblSetupType;
        private System.Windows.Forms.Label lblNetworkClientInfo;
    }
}