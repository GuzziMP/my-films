Imports System.Threading
Imports System.ComponentModel
Imports System.Text
Imports System.Security.Cryptography
Imports System.Management
Imports System.Windows.Forms
Imports System.Windows.Forms.SystemInformation
Imports System.Drawing
Imports System.Xml

Imports MediaPortal.Configuration
Imports NLog
Imports NLog.Targets
Imports MediaPortal.Services
Imports NLog.Config
Imports System.Timers
Imports System.Security.Policy
Imports ShareWatcherHelper.ShareWatcherHelper
Imports Grabber.Importer
'Imports MyFilmsPlugin

Public Class Form1

    Public XMLMovies As New ArrayList
    Public PhysicalMovies As New ArrayList
    Public OrphanedMovies As New ArrayList

    Public AntProcessor As New AntProcessor
    Public myMovieCatalog As AntMovieCatalog = New AntMovieCatalog()
    'Public myMovieCatalog As MyFilmsPlugin.AntMovieCatalog = New MyFilmsPlugin.AntMovieCatalog()

    Public OverridePath As String
    Public SourceField As String

    Public Shared aTimer As System.Threading.Timer

    Private ValidOptions As Boolean = True
    'Private Shared LogEventNew As NLog.Logger = NLog.LogManager.GetCurrentClassLogger() ' add nlog logging

    Private watcherIsActive As Boolean = False
    Public Shared watcher As ShareWatcherHelper.ShareWatcherHelper = Nothing

    Shared MediaData As Hashtable
    Shared InternetData As Hashtable

    Public Sub New() ' Public Sub New(ByVal pub As Publisher)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        'InitLogger()

        ' Create a timer that will call the OnTimedEvent method every second.
        aTimer = New System.Threading.Timer(AddressOf OnTimedEvent, Nothing, 0, 2000)
        'aTimer = New System.Threading.Timer(AddressOf OnTimedEvent, Nothing, Timeout.Infinite, Timeout.Infinite) ' initialize disabled timer
        watch.Reset()
        watch.Start()

        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Label_VersionNumber.Text = "V" + asm.GetName().Version.ToString()
        'Subscribe to Importer Event...
        AddHandler MediaFound, AddressOf MediaFoundEventHandler
        ' Won't throw an exception if obj is Nothing
        'RaiseEvent MsgArrivedEvent("Test message")
        'RemoveHandler MsgArrivedEvent, AddressOf My_MsgArrivedCallback


#If Config = "Release" Then
        ToolStripMenuItemDebug.Visible = False
        ToolStripMenuItemOptions.Visible = False
        BtnImportWatcher.Visible = False
        HideTabPage(ViewPersons)
        HideTabPage(ViewCatalog)
        cbInternetLookupAlwaysPrompt.Visible = False
        chkDontAskInteractive.Visible = False
        chkImportOnInternetFailInGuiMode.Visible = False
        cbManualInternetLookupAlwaysPrompt.Visible = False
        chkManualDontAskInteractive.Visible = False
        btnImportFromDrive.Visible = False
#Else
        ToolStripMenuItemDebug.Visible = True
        ToolStripMenuItemOptions.Visible = True
        BtnImportWatcher.Visible = True
        btnImportFromDrive.Visible = True
#End If


    End Sub

    Sub OnTimedEvent(ByVal x As Object)
        '' Don't do anything if the form's handle hasn't been created 
        '' or the form has been disposed.
        If (Not Me.IsHandleCreated And Not Me.IsDisposed) Then Return

        If Not Me.InvokeRequired Then
            WriteBufferToLogWindow()
        Else
            Try
                Dim mi As MethodInvoker = AddressOf WriteBufferToLogWindow ' Create the method invoker. - The method body adds the logbuffer in sta thread
                Me.Invoke(mi)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Sub WriteBufferToLogWindow()
        If BufferedLogEvents.Length > 0 And watch.Elapsed.TotalMilliseconds > 500 Then
            dgLogWindow.txtLogWindow.AppendText(BufferedLogEvents.ToString())
            BufferedLogEvents.Length = 0 '.Remove(0, BufferedLogEvents.Length)
            watch.Reset()
            watch.Start()
        End If
    End Sub

    Private Sub MediaFoundEventHandler(ByVal mediafiles As List(Of String), ByVal removeorphans As Boolean)
        UpdateSingleMovies(mediafiles, removeorphans)
    End Sub

    Private Sub HideTabPage(ByVal tp As TabPage)
        If TabControl1.TabPages.Contains(tp) Then TabControl1.TabPages.Remove(tp)
    End Sub

    Private Sub ShowTabPage(ByVal tp As TabPage)
        ShowTabPage(tp, TabControl1.TabPages.Count)
    End Sub

    Private Sub ShowTabPage(ByVal tp As TabPage, ByVal index As Integer)
        If TabControl1.TabPages.Contains(tp) Then Return
        InsertTabPage(tp, index)
    End Sub

    Private Sub InsertTabPage(ByVal [tabpage] As TabPage, ByVal [index] As Integer)
        If [index] < 0 Or [index] > TabControl1.TabCount Then
            Throw New ArgumentException("Index out of Range.")
        End If
        TabControl1.TabPages.Add([tabpage])
        If [index] < TabControl1.TabCount - 1 Then
            Do While TabControl1.TabPages.IndexOf([tabpage]) <> [index]
                SwapTabPages([tabpage], (TabControl1.TabPages(TabControl1.TabPages.IndexOf([tabpage]) - 1)))
            Loop
        End If
        TabControl1.SelectedTab = [tabpage]
    End Sub

    Private Sub SwapTabPages(ByVal tp1 As TabPage, ByVal tp2 As TabPage)
        If TabControl1.TabPages.Contains(tp1) = False Or TabControl1.TabPages.Contains(tp2) = False Then
            Throw New ArgumentException("TabPages must be in the TabCotrols TabPageCollection.")
        End If
        Dim Index1 As Integer = TabControl1.TabPages.IndexOf(tp1)
        Dim Index2 As Integer = TabControl1.TabPages.IndexOf(tp2)
        TabControl1.TabPages(Index1) = tp2
        TabControl1.TabPages(Index2) = tp1

        'Uncomment the following section to overcome bugs in the Compact Framework
        'TabControl1.SelectedIndex = TabControl1.SelectedIndex 
        'Dim tp1Text, tp2Text As String
        'tp1Text = tp1.Text
        'tp2Text = tp2.Text
        'tp1.Text=tp2Text
        'tp2.Text=tp1Text

    End Sub

    Private Sub Form1_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Enter
        'InitLogger()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If My.Settings.MainFormLocation.X > 0 And My.Settings.MainFormLocation.Y > 0 Then
            Me.Location = My.Settings.MainFormLocation
        End If
        If My.Settings.MainFormSize.Height > 0 And My.Settings.MainFormSize.Width > 0 Then
            Me.Size = My.Settings.MainFormSize
        End If

        Me.AddOwnedForm(dgLogWindow)
        If My.Settings.LogFormVisible = True Then
            dgLogWindow.Visible = True
        Else
            dgLogWindow.Visible = False
        End If
        If My.Settings.LogFormLocation.X > 0 And My.Settings.LogFormLocation.Y > 0 And (My.Settings.LogFormLocation.X + My.Settings.LogFormSize.Width) < VirtualScreen.Width And (My.Settings.LogFormLocation.Y + My.Settings.LogFormSize.Height) < VirtualScreen.Height Then
            dgLogWindow.Location = My.Settings.LogFormLocation
        End If
        If My.Settings.LogFormSize.Height > 0 And My.Settings.LogFormSize.Width > 0 Then
            dgLogWindow.Size = My.Settings.LogFormSize
        End If

        'Dim lngLeft As Long
        'Dim lngTop As Long
        'Dim lngWidth As Long
        'Dim lngHeight As Long

        '' Get the latest position and size values
        'lngLeft = Val(GetSetting(App.EXEName, "Settings", "Left", "0"))
        'lngTop = Val(GetSetting(App.EXEName, "Settings", "Top", "0"))
        'lngWidth = Val(GetSetting(App.EXEName, "Settings", "Width", "0"))
        'lngHeight = Val(GetSetting(App.EXEName, "Settings", "Height", "0"))

        'If lngWidth > 0 Then
        '    Me.Move(lngLeft, lngTop, lngWidth, lngHeight)
        'End If


        'Me.Visible = True
        'Dim x As Integer
        'Dim y As Integer
        'x = Screen.PrimaryScreen.WorkingArea.Width
        'y = Screen.PrimaryScreen.WorkingArea.Height - Me.Height

        'Do Until x = Screen.PrimaryScreen.WorkingArea.Width - Me.Width
        '    x = x - 1
        '    Me.Location = New Point(x, y)
        'Loop

        If ToolStripStatusLabel.Text = "" Then
            ToolStripStatusLabel.Text = "Ready"
        End If
        If dgLogWindow.Visible = True Then
            btnShowHideLog.Text = "<< Hide Log"
        Else
            btnShowHideLog.Text = "Show Log >>"
        End If

        Dim dvSelectField As DataView = New DataView(AntProcessor.GetAntFieldNames())
        dvSelectField.Sort = "FieldName ASC"
        cbManualSelectField.DataSource = dvSelectField
        cbManualSelectField.DisplayMember = "FieldName"
        cbManualSelectField.ValueMember = "FieldName"
        cbManualSelectField.SelectedIndex = -1

        Dim dvSelectFieldDestination As DataView = New DataView(AntProcessor.GetAntFieldNames())
        dvSelectFieldDestination.Sort = "FieldName ASC"
        cbManualSelectFieldDestination.DataSource = dvSelectFieldDestination
        cbManualSelectFieldDestination.DisplayMember = "FieldName"
        cbManualSelectFieldDestination.ValueMember = "FieldName"
        cbManualSelectFieldDestination.SelectedIndex = -1

        Dim dvParameterFieldList1 As DataView = New DataView(AntProcessor.GetAntFieldNames())
        dvParameterFieldList1.Sort = "FieldName ASC"
        cbManualParameterFieldList1.DataSource = dvParameterFieldList1
        cbManualParameterFieldList1.DisplayMember = "FieldName"
        cbManualParameterFieldList1.ValueMember = "FieldName"
        cbManualParameterFieldList1.SelectedIndex = -1

        Dim dvParameterFieldList2 As DataView = New DataView(AntProcessor.GetAntFieldNames())
        dvParameterFieldList2.Sort = "FieldName ASC"
        cbManualParameterFieldList2.DataSource = dvParameterFieldList2
        cbManualParameterFieldList2.DisplayMember = "FieldName"
        cbManualParameterFieldList2.ValueMember = "FieldName"
        cbManualParameterFieldList2.SelectedIndex = -1

        'Dim dtInternetLookupBehaviour As New DataTable
        'With dtInternetLookupBehaviour
        '    .Columns.Add("Value", System.Type.GetType("System.Boolean"))
        '    .Columns.Add("Display", System.Type.GetType("System.String"))
        '    .Rows.Add(True, "Always offer choice of movie")
        '    .Rows.Add(False, "Try to find best match automatically")
        'End With
        'Dim dvInternetLookupBehaviour1 As New DataView(dtInternetLookupBehaviour)
        'Dim dvInternetLookupBehaviour2 As New DataView(dtInternetLookupBehaviour)

        'With cbInternetLookupBehaviour
        '    .DataSource = dvInternetLookupBehaviour1
        '    .DisplayMember = "Display"
        '    .ValueMember = "Value"
        '    .SelectedIndex = -1
        'End With

        'With cbManualInternetLookupBehaviour
        '    .DataSource = dvInternetLookupBehaviour2
        '    .DisplayMember = "Display"
        '    .ValueMember = "Value"
        '    .SelectedIndex = -1
        'End With

        MediaData = New Hashtable
        'MediaData.Add("originaltitle", "originaltitle")
        MediaData.Add("date", "date")
        MediaData.Add("length", "length")
        MediaData.Add("videobitrate", "videobitrate")
        MediaData.Add("audiobitrate", "audiobitrate")
        MediaData.Add("disks", "disks")
        MediaData.Add("checked", "checked")
        MediaData.Add("medialabel", "medialabel")
        MediaData.Add("mediatype", "mediatype")
        MediaData.Add("videoformat", "videoformat")
        MediaData.Add("audioformat", "audioformat")
        MediaData.Add("resolution", "resolution")
        MediaData.Add("framerate", "framerate")
        MediaData.Add("size", "size")
        MediaData.Add("source", "source")
        MediaData.Add("subtitles", "subtitles")
        MediaData.Add("languages", "languages") ' also enabled here, as we also can load mediainfo data in that field ...
        MediaData.Add("aspectratio", "aspectratio")
        MediaData.Add("audiochannelcount", "audiochannelcount")

        InternetData = New Hashtable
        InternetData.Add("rating", "rating")
        InternetData.Add("year", "year")
        'InternetData.Add("originaltitle", "originaltitle")
        InternetData.Add("translatedtitle", "translatedtitle")
        InternetData.Add("director", "director")
        InternetData.Add("producer", "producer")
        InternetData.Add("country", "country")
        InternetData.Add("category", "category")
        InternetData.Add("actors", "actors")
        InternetData.Add("url", "url")
        InternetData.Add("description", "description")
        InternetData.Add("comments", "comments")
        InternetData.Add("picture", "picture")
        InternetData.Add("languages", "languages") ' could also retrieve mediainfo - but we add it here
        ' Guzzi added: Extended fields
        InternetData.Add("writer", "writer")
        InternetData.Add("certification", "certification")
        InternetData.Add("tagline", "tagline")
        InternetData.Add("imdb_id", "imdb_id")
        InternetData.Add("tmdb_id", "tmdb_id")
        InternetData.Add("imdb_rank", "imdb_rank")
        InternetData.Add("studio", "studio")
        InternetData.Add("fanart", "fanart") ' Guzzi: added on request Dadeo

        SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)

        LoadSettings()

        If chkExecuteProgram.Checked = False Then
            chkExecuteOnlyForOrphans.Enabled = False
            txtExecuteProgramPath.Enabled = False
        Else
            chkExecuteOnlyForOrphans.Enabled = True
            txtExecuteProgramPath.Enabled = True
        End If

        Me.ValidateChildren()

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Try
            'Set my user settings to save size and position
            My.Settings.MainFormSize = Me.Size
            My.Settings.MainFormLocation = Me.Location
            My.Settings.LogFormSize = dgLogWindow.Size
            My.Settings.LogFormLocation = dgLogWindow.Location

            If dgLogWindow.Visible = True Then
                My.Settings.LogFormVisible = True 'save visible status 
            Else
                My.Settings.LogFormVisible = False
            End If

            My.Settings.Save() 'Save the user settings so next time the window will be the same size and location
            'MsgBox("Your settings were saved successfully.", _
            'MsgBoxStyle.OkOnly, "Save...")
        Catch ex As Exception
            MsgBox("There was a problem saving your settings.", _
            MsgBoxStyle.Critical, "Save ErrorEvent...")
        End Try

        If dgLogWindow.Visible = True Then
            dgLogWindow.Close()
        End If
        ApplySettings()
        CurrentSettings.SaveDefaultSettings()
    End Sub

    ''Save position and size values to a system registry
    'Private Sub Form_QueryUnload(ByVal Cancel As Integer, ByVal UnloadMode As Integer)
    '    SaveSetting(App.EXEName, "Settings", "Left", Me.Left)
    '    SaveSetting(App.EXEName, "Settings", "Top", Me.Top)
    '    SaveSetting(AMCUpdater.EXEName, "Settings", "Width", Me.Width)
    '    SaveSetting(App.EXEName, "Settings", "Height", Me.Height)
    'End Sub

    ' Save the form's size and position.
    Private Sub SavePosition(ByVal frm As Form, ByVal app_name As String)
        SaveSetting(app_name, "Geometry", "WindowState", frm.WindowState)
        If frm.WindowState = FormWindowState.Normal Then
            SaveSetting(app_name, "Geometry", "Left", frm.Left)
            SaveSetting(app_name, "Geometry", "Top", frm.Top)
            SaveSetting(app_name, "Geometry", "Width", frm.Width)
            SaveSetting(app_name, "Geometry", "Height", frm.Height)
        Else
            SaveSetting(app_name, "Geometry", "Left", frm.RestoreBounds.Left)
            SaveSetting(app_name, "Geometry", "Top", frm.RestoreBounds.Top)
            SaveSetting(app_name, "Geometry", "Width", frm.RestoreBounds.Width)
            SaveSetting(app_name, "Geometry", "Height", frm.RestoreBounds.Height)
        End If
    End Sub
    ' Restore the form's size and position.
    Private Sub RestorePosition(ByVal frm As Form, ByVal app_name As String)
        frm.SetBounds( _
            GetSetting(app_name, "Geometry", "Left", Me.RestoreBounds.Left), _
            GetSetting(app_name, "Geometry", "Top", Me.RestoreBounds.Top), _
            GetSetting(app_name, "Geometry", "Width", Me.RestoreBounds.Width), _
            GetSetting(app_name, "Geometry", "Height", Me.RestoreBounds.Height) _
        )
        Me.WindowState = GetSetting(app_name, "Geometry", "WindowState", Me.WindowState)
    End Sub


#Region "Main Interactive Actions"
    Public Sub btnParseXML_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnParseXML.Click

        ApplySettings()

        If txtConfigFilePath.Text = String.Empty Then
            MsgBox("You must enter a path to your Ant database file.")
            Exit Sub
        End If


        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum
        Me.ToolStripProgressMessage.Text = "- idle -"

        Dim f As New IO.FileInfo(txtConfigFilePath.Text)
        If Not f.Exists Then
            Dim rep = MsgBox("File : " + f.FullName + " doesn't exist! Do you want to create it ?", MsgBoxStyle.YesNoCancel)
            If rep = 6 Then
                Dim destXml As New System.Xml.XmlTextWriter(txtConfigFilePath.Text, System.Text.Encoding.Default)
                destXml.WriteStartDocument(False)
                destXml.Formatting = System.Xml.Formatting.Indented
                destXml.WriteStartElement("AntMovieCatalog")
                destXml.WriteStartElement("Catalog")
                destXml.WriteElementString("Properties", "")
                destXml.WriteStartElement("Contents")
                destXml.WriteEndElement()
                destXml.WriteEndElement()
                destXml.WriteEndElement()
                destXml.Close()
                LogEvent("Creating XML File", EventLogLevel.ImportantEvent)
                LogEvent(" - FilePath : " + txtConfigFilePath.Text.ToString, EventLogLevel.ImportantEvent)
            Else
                MsgBox("XML File Not Found", MsgBoxStyle.Exclamation)
                SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
                Me.Cursor = Windows.Forms.Cursors.Arrow
                Exit Sub
            End If
        End If

        AntProcessor.ProcessXML()

        SetCheckButtonStatus(ButtonStatus.ReadyToSearchFolders)

        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnProcessMovieList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcessMovieList.Click

        ApplySettings()

        Dim MovieFolder As String
        Dim DoProcess As Boolean = True
        Dim ErrorText As String = ""
        If txtMovieFolder.Text = String.Empty Then
            MsgBox("You must select the folder you want to scan for movies!")
            Exit Sub
        End If
        For Each MovieFolder In txtMovieFolder.Text.Split(New Char() {";", ","}, System.StringSplitOptions.RemoveEmptyEntries)

            Dim f As New IO.DirectoryInfo(MovieFolder)

            If Not f.Exists Then
                DoProcess = False
                If ErrorText = "" Then
                    ErrorText = MovieFolder
                Else
                    ErrorText += "," & MovieFolder
                End If
            End If
        Next

        If DoProcess = True Then
            Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum
            Me.Cursor = Windows.Forms.Cursors.WaitCursor

            With AntProcessor
                .ProcessMovieFolder()
            End With

            SetCheckButtonStatus(ButtonStatus.ReadyToFindOrphans)
        Else
            MsgBox("ErrorEvent : Movie Folder(s) Not Found : " & ErrorText, MsgBoxStyle.Exclamation)
        End If
        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnFindOrphans_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindOrphans.Click

        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum

        ApplySettings()

        AntProcessor.ProcessOrphanFiles()

        If AntProcessor.CountOrphanFiles > 0 Or AntProcessor.CountOrphanRecords > 0 Then
            SetCheckButtonStatus(ButtonStatus.ReadyToDoImport)
        End If

        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnUpdateXML_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateXML.Click

        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum

        ApplySettings()

        SetCheckButtonStatus(ButtonStatus.DisableAll)

        Try
            AntProcessor.UpdateXMLFile()
        Catch ex As Exception
            LogEvent("ErrorEvent : " & ex.Message & " - Stacktrace: " & ex.StackTrace.ToString, EventLogLevel.ErrorEvent)
            'Finally
            'fnSetCheckButtonStatus(ButtonStatus.ParseXML)
        End Try
        ' added to force refresh of View Collection Tab
        txtConfigFilePath_TextChanged(sender, e)
        'Me.txtConfigFilePath.Text = Me.txtConfigFilePath.Text
        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Private Sub btnJustDoIt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnJustDoIt.Click
        Dim currentusecase As String = cbInternetLookupBehaviour.Text ' saves current settings to later restore it
        If (currentusecase = "Auto match, then manual match") Then

            cbInternetLookupBehaviour.Text = "Auto match only"
            ApplyUseCasesToSettings()

            btnParseXML_Click(Nothing, Nothing)
            btnProcessMovieList_Click(Nothing, Nothing)
            btnFindOrphans_Click(Nothing, Nothing)
            If btnUpdateXML.Enabled = True Then
                btnUpdateXML_Click(Nothing, Nothing)
            End If

            cbInternetLookupBehaviour.Text = "Manual match (always ask)"
            ApplyUseCasesToSettings()

            btnParseXML_Click(Nothing, Nothing)
            btnProcessMovieList_Click(Nothing, Nothing)
            btnFindOrphans_Click(Nothing, Nothing)
            If btnUpdateXML.Enabled = True Then
                btnUpdateXML_Click(Nothing, Nothing)
            End If

            cbInternetLookupBehaviour.Text = currentusecase
            ApplyUseCasesToSettings()
        Else
            btnParseXML_Click(Nothing, Nothing)
            btnProcessMovieList_Click(Nothing, Nothing)
            'btnProcessTrailerList_Click(Nothing, Nothing) ' still has to be implemented - is just a remark
            btnFindOrphans_Click(Nothing, Nothing)
            If btnUpdateXML.Enabled = True Then
                btnUpdateXML_Click(Nothing, Nothing)
            End If
        End If
    End Sub

    Private Sub BtnImportWatcher_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnImportWatcher.Click
        If InitWatcher() = True Then
            watcherIsActive = True
            BtnImportWatcher.BackColor = Color.DarkSeaGreen
            BtnImportWatcher.Text = "Stop Import Watcher"
            SetCheckButtonStatus(ButtonStatus.DisableAll)
        Else
            If watcherIsActive Then
                watcherIsActive = False
                BtnImportWatcher.BackColor = Color.Empty
                BtnImportWatcher.Text = "Start Import Watcher"
                SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
                watcher.ChangeMonitoring(False)
            Else
                watcherIsActive = True
                BtnImportWatcher.BackColor = Color.DarkSeaGreen
                SetCheckButtonStatus(ButtonStatus.DisableAll)
                watcher.ChangeMonitoring(True)
            End If
        End If
    End Sub

    Private Function InitWatcher() As Boolean
        If watcher Is Nothing Then
            'Thread.CurrentThread.Name = "ShareWatcher"
            Dim scandirectories As List(Of String) = New List(Of String)()
            For Each CurrentMoviePath In CurrentSettings.Movie_Scan_Path.Split(";")
                If CurrentMoviePath Is Nothing Or CurrentMoviePath.Length = 0 Then Continue For
                Dim dir As New DirectoryInfo(CurrentMoviePath)
                If Not dir.Exists Then
                    LogEvent("ErrorEvent : Cannot access folder '" + CurrentMoviePath.ToString + "'", EventLogLevel.ErrorEvent)
                Else
                    scandirectories.Add(CurrentMoviePath)
                End If
            Next
            watcher = New ShareWatcherHelper.ShareWatcherHelper(scandirectories) ' Setup the Watching
            watcher.SetMonitoring(True)
            watcher.StartMonitor()
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub UpdateSingleMovies(ByVal mediafiles As List(Of String), ByVal removeorphans As Boolean)
        For Each mediafile As String In mediafiles
            BufferedLogEvents.AppendLine("Watcher detected file - '" & mediafile & "'")
        Next
        Return

        'ToDo: to be activated and tested/debugged
        'btnParseXML_Click(Nothing, Nothing) ' read DB
        ''btnProcessMovieList_Click(Nothing, Nothing) ' check existing files

        'ApplySettings()
        'With AntProcessor
        '    .ProcessMovieFolderForSingleMovie(mediafiles)
        'End With

        'btnFindOrphans_Click(Nothing, Nothing) ' find orphans
        'If AntProcessor.CountOrphanFiles > 0 Then
        '    SetCheckButtonStatus(ButtonStatus.DisableAll)
        '    Try
        '        AntProcessor.UpdateXMLFile()
        '    Catch ex As Exception
        '        LogEvent("ErrorEvent : " & ex.Message & " - Stacktrace: " & ex.StackTrace.ToString, EventLogLevel.ErrorEvent)
        '    End Try
        '    ' added to force refresh of View Collection Tab
        '    txtConfigFilePath_TextChanged(Nothing, Nothing) 'Me.txtConfigFilePath.Text = Me.txtConfigFilePath.Text
        'End If
        'SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
    End Sub

    Private Sub setUpFolderWatches()
        Dim importFolders As List(Of String) = New List(Of String)

        For Each CurrentMoviePath In CurrentSettings.Movie_Scan_Path.Split(";")

            Dim dir As New DirectoryInfo(CurrentMoviePath)
            If Not dir.Exists Then
                LogEvent("ErrorEvent : Cannot access folder '" + CurrentMoviePath.ToString + "'", EventLogLevel.ErrorEvent)
            Else
                If Not CurrentMoviePath.EndsWith("\") = True Then
                    CurrentMoviePath = CurrentMoviePath & "\"
                    importFolders.Add(CurrentMoviePath)
                End If
            End If
        Next


        'watcher = New Watcher(importFolders, 5)
        'watcher.WatcherProgress += New Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress)
        'watcher.StartFolderWatch()
    End Sub

    Private Sub stopFolderWatches()
        'watcher.StopFolderWatch()
        'watcher.WatcherProgress -= New Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress)
        'watcher = Nothing
    End Sub

#End Region

#Region "Manual Updater"
    Private Sub btnManualDoTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualDoTest.Click

        btnManualApplyChanges.Enabled = False
        ApplySettings()

        'txtManualXMLPath - check for valid file.
        'cbManualSelectOperation - find out what they want to do - 'Update Value','Delete Record','Delete Value'
        'cbManualSelectField - field they might want to fiddle with
        'txtManualNewValue - value they may want to set.

        'chkManualParametersUpdateAll - means they want to apply to all entries (scary!)
        'cbManualParameterFieldList - otherwise use this Ant field as a parameter
        'cbManualParameterOperator - and this is the operation to perform - '=', '!=', 'EXISTS', 'NOT EXISTS', 'LIKE'
        'txtManualParameterValue - and this is the parameter to check for.
        If txtConfigFilePath.Text = String.Empty Then
            Exit Sub
        End If


        Dim f As New IO.FileInfo(txtConfigFilePath.Text)
        If Not f.Exists Then
            MsgBox("File : " + f.FullName + " doesn't exist !", MsgBoxStyle.Critical)
            txtConfigFilePath.Focus()
            Return
        End If
        If cbManualSelectOperation.SelectedIndex = -1 Then
            MsgBox("Operation Mandatory", MsgBoxStyle.Critical, "Missing Operation")
            cbManualSelectOperation.Focus()
            Return
        End If
        If cbManualSelectOperation.SelectedItem.ToString = "Update Value" Then
            If cbManualSelectField.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Update Operation", MsgBoxStyle.Critical, "Missing Update Field")
                cbManualSelectField.Focus()
                Return
            End If
            If txtManualNewValue.Text.Length = 0 Then
                MsgBox("New Value Mandatory with Update Operation", MsgBoxStyle.Critical, "Missing Update Value")
                txtManualNewValue.Focus()
                Return
            End If
        End If
        If cbManualSelectOperation.SelectedItem.ToString = "Copy Value" Then
            If cbManualSelectField.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Copy Operation", MsgBoxStyle.Critical, "Missing Source Field")
                cbManualSelectField.Focus()
                Return
            End If
            If cbManualSelectFieldDestination.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Copy Operation", MsgBoxStyle.Critical, "Missing Destination Field")
                cbManualSelectFieldDestination.Focus()
                Return
            End If
        End If
        If (chkManualParametersUpdateAll.Checked = False) Then
            If cbManualParameterFieldList1.SelectedIndex = -1 Then
                MsgBox("Field Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Field Parameter")
                cbManualParameterFieldList1.Focus()
                Return
            End If
            If cbManualParameterOperator1.SelectedIndex = -1 Then
                MsgBox("Operator Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Operator Parameter")
                cbManualParameterOperator1.Focus()
                Return
            End If
            If txtManualParameterValue1.Text.Length = 0 And (cbManualParameterOperator1.SelectedItem <> "EXISTS") And (cbManualParameterOperator1.SelectedItem <> "NOT EXISTS") Then
                MsgBox("Value Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Value Parameter")
                txtManualParameterValue1.Focus()
                Return
            End If
            If cbManualParameterAndOr.SelectedIndex <> -1 Then
                If cbManualParameterAndOr.SelectedItem.ToString <> " " Then
                    If cbManualParameterFieldList2.SelectedIndex = -1 Then
                        MsgBox("Field Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Field Parameter")
                        cbManualParameterFieldList2.Focus()
                        Return
                    End If
                    If cbManualParameterOperator2.SelectedIndex = -1 Then
                        MsgBox("Operator Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Operator Parameter")
                        cbManualParameterOperator2.Focus()
                        Return
                    End If
                    If txtManualParameterValue2.Text.Length = 0 And (cbManualParameterOperator2.SelectedItem <> "EXISTS") And (cbManualParameterOperator2.SelectedItem <> "NOT EXISTS") Then
                        MsgBox("Value Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Value Parameter")
                        txtManualParameterValue2.Focus()
                        Return
                    End If
                End If
            End If
        End If

        AntProcessor = New AntProcessor()

        AntProcessor.ManualOperation = cbManualSelectOperation.SelectedItem.ToString
        If cbManualSelectField.SelectedIndex > -1 Then
            AntProcessor.ManualFieldName = cbManualSelectField.SelectedValue.ToString
        End If
        If cbManualSelectFieldDestination.SelectedIndex > -1 Then
            AntProcessor.ManualFieldNameDestination = cbManualSelectFieldDestination.SelectedValue.ToString
        End If
        AntProcessor.ManualFieldValue = txtManualNewValue.Text
        AntProcessor.ManualFieldOldValue = txtManualOldValue.Text
        If cbManualParameterFieldList1.SelectedIndex > -1 Then
            AntProcessor.ManualParameterField1 = cbManualParameterFieldList1.SelectedValue.ToString
        End If
        If cbManualParameterOperator1.SelectedIndex > -1 Then
            AntProcessor.ManualParameterOperator1 = cbManualParameterOperator1.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterValue1 = txtManualParameterValue1.Text
        If cbManualParameterFieldList2.SelectedIndex > -1 Then
            AntProcessor.ManualParameterField2 = cbManualParameterFieldList2.SelectedValue.ToString
        End If
        If cbManualParameterOperator2.SelectedIndex > -1 Then
            AntProcessor.ManualParameterOperator2 = cbManualParameterOperator2.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterValue2 = txtManualParameterValue2.Text
        If cbManualParameterAndOr.SelectedIndex > -1 Then
            AntProcessor.cbManualParameterAndOr = cbManualParameterAndOr.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterMatchAll = chkManualParametersUpdateAll.Checked
        AntProcessor.ManualMissingFanartDownload = chkManualMissingFanartDownload.Checked
        If cbManualNfoFileHandling.SelectedIndex > -1 Then
            AntProcessor.ManualNfoFileHandling = cbManualNfoFileHandling.SelectedItem.ToString
        End If
        AntProcessor.ManualNfoFileOnlyUpdateMissing = chkManualNfoFilesOnlyAddMissing.Checked
        'AntProcessor.DateHandling = cbDateHandling.SelectedValue
        'If AntProcessor.ManualOperation = "Scan Media Data" Then
        'AntProcessor.ManualFieldValue = TxtManualPathToMovies.Text
        'End If
        AntProcessor.ManualXMLPath = txtConfigFilePath.Text

        AntProcessor.ManualTestOperation()

        If AntProcessor.ManualTestResultCount > 0 Then
            btnManualApplyChanges.Enabled = True
        Else
            btnManualApplyChanges.Enabled = False
        End If

    End Sub
    Private Sub btnManualApplyChanges_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualApplyChanges.Click
        ApplySettings()
        If AntProcessor.ManualTestResultCount > 0 Then
            AntProcessor.ManualRunOperation()
        End If
        'Me.txtConfigFilePath.Text = txtConfigFilePath.Text

    End Sub


#End Region

#Region "Misc GUI Actions"
    Private Sub btnSelectMovieFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectMovieFolder.Click
        Dim wpath As String
        Dim currentPath As String
        currentPath = txtMovieFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If

        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your movie files are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    If .SelectedPath.EndsWith("\") <> True Then
                        wpath = .SelectedPath + "\"
                    Else
                        wpath = .SelectedPath
                    End If
                    'Moved this logic here since it would prompt if there already was a path listed even if the user cancels the dialog box.  OH.
                    If txtMovieFolder.Text = "" Then
                        txtMovieFolder.Text = wpath
                    Else
                        'txtMovieFolder.Text not empty ask if we want to add or replace
                        Dim rep As Int32 = MsgBox("Do you want to Add that entry to the existing one ?", MsgBoxStyle.YesNoCancel)
                        If rep = MsgBoxResult.Cancel Then
                            Exit Sub
                        Else
                            If rep = MsgBoxResult.Yes Then
                                txtMovieFolder.Text = txtMovieFolder.Text + ";" + wpath
                            Else
                                txtMovieFolder.Text = wpath
                            End If
                        End If
                    End If

                End If
            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnSelectConfigFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectConfigFile.Click

        '' originaol code from filebrowsing in update tab
        'Try
        '    With OpenFileDialog1
        '        .InitialDirectory = Environment.SpecialFolder.Desktop
        '        .FileName = ""
        '        .CheckFileExists = True
        '        .CheckPathExists = True
        '        .DefaultExt = "xml"
        '        .DereferenceLinks = True
        '        .Filter = "XML files (*.xml)|*.xml|All files|*.*"
        '        .Multiselect = False
        '        .RestoreDirectory = True
        '        .ShowHelp = True
        '        .ShowReadOnly = False
        '        .ReadOnlyChecked = False
        '        .Title = "Select a file to open"
        '        .ValidateNames = True
        '        If .ShowDialog = Windows.Forms.DialogResult.OK Then
        '            Try
        '                txtManualXMLPath.Text = .FileName
        '            Catch fileException As Exception
        '                LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
        '            End Try
        '        End If

        '    End With
        'Catch ex As Exception
        '    LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
        '    Console.WriteLine(ex.Message)
        '    MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        'End Try
        'Me.ValidateChildren()

        Dim currentDirectory As String
        currentDirectory = txtConfigFilePath.Text
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = currentDirectory
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtConfigFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnGetSampleFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetSampleFile.Click
        Try
            With OpenFileDialog1
                .CheckFileExists = True
                .CheckPathExists = True
                .FileName = ""
                .DefaultExt = "avi"
                .DereferenceLinks = True
                .Filter = "All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtSampleFile.Text = .FileName
                    Catch fileException As Exception
                        Throw fileException
                    End Try
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnTestAnalyse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestAnalyse.Click
        If File.Exists(txtSampleFile.Text) Then
            Dim StartTime As Date = DateTime.Now

            Try
                txtSampleVideoCodec.Text = GetFileData(txtSampleFile.Text, "VideoFormat")
                txtSampleVideoBitrate.Text = GetFileData(txtSampleFile.Text, "VideoBitrate")
                txtSampleAspectRatio.Text = GetFileData(txtSampleFile.Text, "Aspectratio")
                txtSampleVideoFramerate.Text = GetFileData(txtSampleFile.Text, "Framerate")
                txtSampleVideoResolution.Text = GetFileData(txtSampleFile.Text, "Resolution")
                txtSampleVideo3D.Text = GetFileData(txtSampleFile.Text, "3d")
                txtSampleAudioBitrate.Text = GetFileData(txtSampleFile.Text, "AudioBitrate")
                txtSampleAudioCodec.Text = GetFileData(txtSampleFile.Text, "AudioFormat")
                txtSampleFileLength.Text = GetFileData(txtSampleFile.Text, "runtime")
                txtSampleFileSize.Text = GetFileData(txtSampleFile.Text, "filesize")
                txtSampleAudioChannelCount.Text = GetFileData(txtSampleFile.Text, "audiochannelcount")
                txtSampleAudioStreamCount.Text = GetFileData(txtSampleFile.Text, "audiostreamcount")
                txtSampleAudioStreamList.Text = GetFileData(txtSampleFile.Text, "audiostreamcodeclist")
                txtSampleAudioLanguageList.Text = GetFileData(txtSampleFile.Text, "audiostreamlanguagelist")
                txtSampleTextCodecList.Text = GetFileData(txtSampleFile.Text, "textstreamcodeclist")
                txtSampleTextLanguageList.Text = GetFileData(txtSampleFile.Text, "textstreamlanguagelist")

            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

            If 1 = 0 Then
                Dim fs As FileStream = New FileStream(txtSampleFile.Text, FileMode.Open, FileAccess.Read)
                Dim length As Long = fs.Length
                'Falls over here because length is 734,208,000 bytes.
                length = 1024000
                Dim arr(length) As Byte
                fs.Read(arr, 0, length)
                fs.Close()
                Dim tmpHash() As Byte
                tmpHash = New MD5CryptoServiceProvider().ComputeHash(arr)
                'txtTestScanChecksumBox.Text = ByteArrayToString(tmpHash)
            End If

            Dim EndTime As Date = DateTime.Now
            MsgBox("Operation took : " & (EndTime - StartTime).ToString)

        End If
    End Sub
    Private Sub btnSelectParserFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectParserFile.Click
        Dim currentPath As String
        currentPath = txtParserFilePath.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = currentPath
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtParserFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnSelectExcludeFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectExcludeFile.Click
        '' old code from manual exclusion file
        'Try
        '    With OpenFileDialog1
        '        .InitialDirectory = My.Application.Info.DirectoryPath
        '        .FileName = "AMCUpdater_Excluded_Files.txt"
        '        .CheckFileExists = False
        '        .CheckPathExists = True
        '        .DefaultExt = "txt"
        '        .DereferenceLinks = True
        '        .Filter = "txt files (*.txt)|*.txt|All files|*.*"
        '        .Multiselect = False
        '        .RestoreDirectory = True
        '        .ShowHelp = True
        '        .ShowReadOnly = False
        '        .ReadOnlyChecked = False
        '        .Title = "Select a file for Excluded Movies Files"
        '        .ValidateNames = True
        '        If .ShowDialog = Windows.Forms.DialogResult.OK Then
        '            Try
        '                txtManualExcludedMoviesPath.Text = .FileName
        '            Catch fileException As Exception
        '                LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
        '            End Try
        '        End If

        '    End With
        'Catch ex As Exception
        '    LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
        '    MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        'End Try
        'Me.ValidateChildren()

        Dim currentPath As String
        currentPath = txtExcludeFilePath.Text
        Try
            With OpenFileDialog1
                If Not String.IsNullOrEmpty(currentPath) Then
                    .InitialDirectory = Path.GetDirectoryName(currentPath)
                    .FileName = currentPath
                Else
                    .InitialDirectory = My.Application.Info.DirectoryPath
                    .FileName = "AMCUpdater_Excluded_Files.txt"
                End If
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "txt"
                .DereferenceLinks = True
                .Filter = "txt files (*.txt)|*.txt|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file for Excluded Movies Files"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtExcludeFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnExecuteProgramSelectPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecuteProgramSelectPath.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = ""
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "All files|*.*"
                .Multiselect = False
                .RestoreDirectory = False
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtExecuteProgramPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If
            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            Console.WriteLine(ex.Message)
            '            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub btnCancelProcessing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelProcessing.Click
        AntProcessor.bgwFolderScanUpdate_Cancel()
    End Sub

    Private Sub btnManualCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualCancel.Click
        AntProcessor.bgwManualUpdate_Cancel()
    End Sub


    Private Sub btnShowHideLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowHideLogTest.Click, btnShowHideLog.Click
        If dgLogWindow.Visible = True Then
            dgLogWindow.Hide()
            btnShowHideLog.Text = "Show Log >>"
            btnShowHideLogTest.Text = "Show Log >>"
        Else
            'dgLogWindow.SetDesktopLocation(Me.Location.X, Me.Location.Y + Me.Height)
            'dgLogWindow.Width = Me.Width
            ''dgLogWindow.Height = 150
            dgLogWindow.Show()

            'dgLogWindow.Height = Me.Height
            'dgLogWindow.SetDesktopLocation(Me.Location.X + Me.Width, Me.Location.Y)
            'dgLogWindow.Show()
            ''Me.AddOwnedForm(dgLogWindow)

            btnShowHideLog.Text = "<< Hide Log"
            btnShowHideLogTest.Text = "<< Hide Log"
        End If
    End Sub

    Private Sub Form1_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        If dgLogWindow.Visible = True And dgLogWindow.Location.X = Me.Location.X Then

            dgLogWindow.SetDesktopLocation(Me.Location.X, Me.Location.Y + Me.Height)
            'dgLogWindow.SetDesktopLocation(Me.Location.X + Me.Width, Me.Location.Y)

            'dgLogWindow.BringToFront()
            'Me.BringToFront()
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
    Private Sub btnDBFieldsSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAll.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                cbDatabaseFields.SetItemChecked(i, True)
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNone.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            cbDatabaseFields.SetItemChecked(i, False)
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectAllMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                    cbDatabaseFields.SetItemChecked(i, True)
                End If
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNoMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectAllInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                    cbDatabaseFields.SetItemChecked(i, True)
                End If
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNoInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
        Database_Fields_Validation()
    End Sub

    Private Sub btnManualSelectInternetParserPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualSelectInternetParserPath.Click
        Dim currentPath As String
        currentPath = txtManualInternetParserPath.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = currentPath
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtManualInternetParserPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

#End Region

#Region "Validation"
    Private Sub chkOverwriteXML_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkOverwriteXML.CheckedChanged

        'AntProcessor.OverwriteXMLFile = chkOverwriteXML.Checked

        If chkOverwriteXML.Checked = True Then
            chkBackupXMLFirst.Enabled = True
        Else
            chkBackupXMLFirst.Checked = False
            chkBackupXMLFirst.Enabled = False
        End If
    End Sub
    Private Sub txtOverridePath_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOverridePath.LostFocus
        If txtOverridePath.Text <> String.Empty And txtOverridePath.Text.EndsWith("\") = False Then
            txtOverridePath.Text += "\"
        End If
    End Sub
    Private Sub chkPurgeMissing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPurgeMissing.Click

        If chkPurgeMissing.Checked = True Then
            Dim Warning As String = "Warning : The 'Purge missing' option removes invalid entries from your database file."
            Warning += vbCrLf + "It thus might remove content from your database - please proceed with caution."
            Warning += vbCrLf + vbCrLf + "Do you wish to continue?"

            Dim ReturnValue As Integer

            ReturnValue = MsgBox(Warning, MsgBoxStyle.OkCancel, "Warning")

            If Not ReturnValue = 1 Then
                chkPurgeMissing.Checked = False
            End If
        End If
    End Sub
    Private Sub cbManualSelectOperation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectOperation.SelectedIndexChanged
        If cbManualSelectOperation.SelectedIndex > -1 Then
            lblManualEnterNewValue.Visible = False
            txtManualNewValue.Visible = False
            lblManualEnterOldValue.Visible = False
            txtManualOldValue.Visible = False
            lblManualSelectField.Visible = False
            cbManualSelectField.Visible = False
            lblManualSelectFieldDestination.Visible = False
            cbManualSelectFieldDestination.Visible = False

            grpManualInternetLookupSettings.Visible = False
            lblManualDatabaseFieldsPrompt.Visible = False
            chkManualMissingFanartDownload.Visible = False

            lblManualNfoFileHandling.Visible = False
            cbManualNfoFileHandling.Visible = False
            chkManualNfoFilesOnlyAddMissing.Visible = False

            chkManualUpdateRecordsOnlyMissingData.Visible = False
            chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = False

            grpManualUpdatesParameters.Visible = True

            cbSkipExcludedMovieFiles.Visible = False


            If cbManualSelectOperation.SelectedItem = "Update Value" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Replace String" Then
                lblManualEnterOldValue.Visible = True
                txtManualOldValue.Visible = True
                txtManualOldValue.Clear()
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Add String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Insert String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Remove String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Delete Value" Then
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Copy Value" Then
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                lblManualSelectFieldDestination.Visible = True
                cbManualSelectFieldDestination.Visible = True
                chkManualUpdateRecordsOnlyMissingData.Visible = True
                chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Update Record" Then
                grpManualInternetLookupSettings.Visible = True
                lblManualDatabaseFieldsPrompt.Visible = True
                chkManualUpdateRecordsOnlyMissingData.Visible = True
                chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = True
                cbSkipExcludedMovieFiles.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Download Fanart" Then
                chkManualMissingFanartDownload.Visible = True
                'grpManualInternetLookupSettings.Visible = True
                lblManualDatabaseFieldsPrompt.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Update NFO File" Then
                lblManualNfoFileHandling.Visible = True
                cbManualNfoFileHandling.Visible = True
                chkManualNfoFilesOnlyAddMissing.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Delete NFO File" Then
                lblManualNfoFileHandling.Visible = True
                cbManualNfoFileHandling.Visible = True
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualOperator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterOperator1.SelectedIndexChanged
        If cbManualParameterOperator1.SelectedIndex >= 0 Then
            If cbManualParameterOperator1.SelectedItem <> "EXISTS" And cbManualParameterOperator1.SelectedItem <> "NOT EXISTS" Then
                txtManualParameterValue1.Visible = True
                lblManualParametersValue1.Visible = True
            Else
                txtManualParameterValue1.Visible = False
                lblManualParametersValue1.Visible = False
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub chkManualParametersUpdateAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkManualParametersUpdateAll.CheckedChanged
        If chkManualParametersUpdateAll.Checked = True Then
            cbManualParameterFieldList1.Enabled = False
            cbManualParameterOperator1.Enabled = False
            txtManualParameterValue1.Enabled = False
            cbManualParameterFieldList2.Enabled = False
            cbManualParameterOperator2.Enabled = False
            txtManualParameterValue2.Enabled = False
            cbManualParameterAndOr.Enabled = False
        Else
            cbManualParameterFieldList1.Enabled = True
            cbManualParameterOperator1.Enabled = True
            txtManualParameterValue1.Enabled = True
            cbManualParameterFieldList2.Enabled = True
            cbManualParameterOperator2.Enabled = True
            txtManualParameterValue2.Enabled = True
            cbManualParameterAndOr.Enabled = True
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterFieldList2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterFieldList2.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterOperator2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterOperator2.SelectedIndexChanged
        If cbManualParameterOperator2.SelectedIndex >= 0 Then
            If cbManualParameterOperator2.SelectedItem <> "EXISTS" And cbManualParameterOperator2.SelectedItem <> "NOT EXISTS" Then
                txtManualParameterValue2.Visible = True
                lblManualParametersValue2.Visible = True
            Else
                txtManualParameterValue2.Visible = False
                lblManualParametersValue2.Visible = False
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterAndOr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterAndOr.SelectedIndexChanged
        If cbManualParameterAndOr.SelectedIndex >= 0 Then
            If cbManualParameterAndOr.SelectedItem.ToString = " " Then
                cbManualParameterAndOr.SelectedIndex = -1
                Me.ValidateChildren()
                Exit Sub
            Else
                lblManualParametersField2.Visible = True
                lblManualParametersOperator2.Visible = True
                cbManualParameterFieldList2.Visible = True
                cbManualParameterOperator2.Visible = True
            End If
        Else
            lblManualParametersField2.Visible = False
            lblManualParametersOperator2.Visible = False
            lblManualParametersValue2.Visible = False
            cbManualParameterFieldList2.Visible = False
            cbManualParameterOperator2.Visible = False
            txtManualParameterValue2.Visible = False
            cbManualParameterFieldList2.Text = ""
            cbManualParameterOperator2.Text = ""
            txtManualParameterValue2.Text = ""
        End If
        Me.ValidateChildren()

    End Sub
    Private Sub txtManualParameterValue2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualParameterValue2.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub txtManualParameterValue1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualParameterValue1.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterFieldList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterFieldList1.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualSelectField_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectField.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualSelectFieldDestination_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectFieldDestination.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualNfoFileHandling_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualNfoFileHandling.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub txtMovieFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMovieFolder.LostFocus
        'txtOverridePath_LostFocus(sender, e)
        Dim Path As String() = txtMovieFolder.Text.Split(";")
        Dim PathItem As String = String.Empty
        Dim ReturnPath As String = String.Empty
        For i As Integer = 0 To Path.Length - 1
            PathItem = Path(i)
            If Not ReturnPath.Contains(PathItem) Then
                If Not PathItem.EndsWith("\") = True Then
                    PathItem += "\"
                End If
                ReturnPath += PathItem & ";"
            End If
        Next
        If ReturnPath.EndsWith(";") = True Then
            ReturnPath = ReturnPath.Substring(0, ReturnPath.Length - 1)
        End If

        txtMovieFolder.Text = ReturnPath

    End Sub
    Private Sub chkExecuteProgram_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkExecuteProgram.CheckedChanged
        If chkExecuteProgram.Checked Then
            txtExecuteProgramPath.Enabled = True
            btnExecuteProgramSelectPath.Enabled = True
            chkExecuteOnlyForOrphans.Enabled = True
        Else
            txtExecuteProgramPath.Enabled = False
            btnExecuteProgramSelectPath.Enabled = False
            chkExecuteOnlyForOrphans.Enabled = False
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub txtDefaultFileTypes_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultFileTypes.LostFocus
        Dim FileTypes() As String
        Dim OutputString As String = String.Empty
        If txtDefaultFileTypes.Text <> String.Empty Then
            FileTypes = txtDefaultFileTypes.Text.Split(";")
            For Each blah As String In FileTypes
                If blah.ToLower.Trim = "ifo" Then
                    MsgBox("You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
                ElseIf blah.ToLower.Trim = "bdmv" Then
                    MsgBox("You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
                Else
                    OutputString += blah & ";"
                End If
            Next
            If OutputString.EndsWith(";") Then
                OutputString = OutputString.Substring(0, OutputString.Length - 1)
            End If

            txtDefaultFileTypes.Text = OutputString

        End If
        'Me.ValidateChildren()
    End Sub
    Private Sub txtDefaultFileTypesNonMedia_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultFileTypesNonMedia.LostFocus
        Dim FileTypes() As String
        Dim OutputString As String = String.Empty
        If txtDefaultFileTypesNonMedia.Text <> String.Empty Then
            FileTypes = txtDefaultFileTypesNonMedia.Text.Split(";")
            For Each blah As String In FileTypes
                If blah.ToLower.Trim = "ifo" Then
                    MsgBox("You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
                ElseIf blah.ToLower.Trim = "bdmv" Then
                    MsgBox("You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
                Else
                    OutputString += blah & ";"
                End If
            Next
            If OutputString.EndsWith(";") Then
                OutputString = OutputString.Substring(0, OutputString.Length - 1)
            End If

            txtDefaultFileTypesNonMedia.Text = OutputString

        End If
    End Sub
    Private Sub txtManualInternetParserPath_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualInternetParserPath.LostFocus
        Me.ValidateChildren()
    End Sub
    Private Sub cbDatabaseFields_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles cbDatabaseFields.ItemCheck
        Database_Fields_Validation()
    End Sub
    Private Sub cbDatabaseFields_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbDatabaseFields.LostFocus
        Database_Fields_Validation()
    End Sub
    Private Sub cbTitleHandling_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbTitleHandling.SelectedIndexChanged
        Database_Fields_Validation()
    End Sub
    Private Sub cbDatabaseFields_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbDatabaseFields.MouseUp
        Database_Fields_Validation()
    End Sub
    Private Sub chkFolderNameIsGroupName_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFolderNameIsGroupName.CheckedChanged
        Database_Fields_Validation()
    End Sub
    Private Sub txtPictureFilenamePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPictureFilenamePrefix.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub txtPictureFilenameSuffix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPictureFilenameSuffix.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbPictureHandling_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPictureHandling.SelectedIndexChanged
        Database_Fields_Validation()
    End Sub

    Private Sub Interactive_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOverridePath.Validating, txtMovieFolder.Validating, txtConfigFilePath.Validating, txtParserFilePath.Validating, txtExcludeFilePath.Validating
        'Override Path and Movie Folder (Check for empty; check that no override path used with multiple scan folders
        Dim IsValid As Boolean = True


        If txtOverridePath.Text <> String.Empty And txtMovieFolder.Text.Contains(";") Then
            epInteractive.SetError(txtOverridePath, "Cannot have an Override Path when multiple folders are scanned")
            epInteractive.SetError(txtMovieFolder, "Cannot have an Override Path when multiple folders are scanned")
            IsValid = False
        Else
            If txtMovieFolder.Text = String.Empty Then
                epInteractive.SetError(txtMovieFolder, "Path to folder containing movie files must be entered.")
                IsValid = False
            Else
                epInteractive.SetError(txtMovieFolder, "")
            End If
            epInteractive.SetError(txtOverridePath, "")
        End If
        'XML file path required.
        If txtConfigFilePath.Text = String.Empty Then
            epInteractive.SetError(txtConfigFilePath, "Path to Ant XML database file must be entered")
            IsValid = False
        Else
            epInteractive.SetError(txtConfigFilePath, "")
        End If
        'Parser File Path Required
        If txtParserFilePath.Text = String.Empty Then
            epInteractive.SetError(txtParserFilePath, "Path to Internet Parser File must be entered")
            IsValid = False
        Else
            Dim wpath As String = txtParserFilePath.Text
            If Not wpath.Contains("\") Then
                'Not a path without a backslash!
                epInteractive.SetError(txtParserFilePath, "Please enter a valid Internet Parser File path.")
                IsValid = False
            Else
                If System.IO.File.Exists(wpath) Then
                    epInteractive.SetError(txtParserFilePath, "")
                Else
                    epInteractive.SetError(txtParserFilePath, "Please enter a valid Internet Parser File path.")
                    IsValid = False
                End If
            End If
        End If

        'Excluded Movie File Path Required
        If txtExcludeFilePath.Text = String.Empty Then
            epInteractive.SetError(txtExcludeFilePath, "Path to excluded movies file must be entered")
            IsValid = False
        Else
            'Check it's a valid path:
            Dim wpath As String = txtExcludeFilePath.Text
            If Not wpath.Contains("\") Then
                'Not a path without a backslash!
                epInteractive.SetError(txtExcludeFilePath, "Please enter a valid file path")
                IsValid = False
            Else
                wpath = wpath.Substring(0, wpath.LastIndexOf("\") + 1)
                If System.IO.Directory.Exists(wpath) Then
                    If txtExcludeFilePath.Text.EndsWith("\") = True Then
                        txtExcludeFilePath.Text += "AMCUpdater_Excluded_Files.txt"
                    ElseIf txtExcludeFilePath.Text.EndsWith(".txt") = True Then
                    Else
                        txtExcludeFilePath.Text += "\AMCUpdater_Excluded_Files.txt"
                    End If
                    epInteractive.SetError(txtExcludeFilePath, "")
                Else
                    epInteractive.SetError(txtExcludeFilePath, "Please enter a valid file path")
                    IsValid = False
                End If
            End If
        End If

        If IsValid = False Then
            btnJustDoIt.Enabled = False
            SetCheckButtonStatus(ButtonStatus.DisableAll)
        Else
            btnJustDoIt.Enabled = True
            SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
        End If

    End Sub
    Private Sub ManualUpdater_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cbManualParameterOperator1.Validating, cbManualParameterFieldList1.Validating, txtManualNewValue.Validating, txtManualParameterValue1.Validating, cbManualSelectField.Validating, cbManualSelectOperation.Validating, chkManualParametersUpdateAll.Validating
        Dim IsValid As Boolean = True
        Dim TestState As Boolean = btnManualDoTest.Enabled
        Dim ApplyState As Boolean = btnManualApplyChanges.Enabled

        'Check a path has been entered:
        If txtConfigFilePath.Text = String.Empty Then
            epManualUpdater.SetError(txtConfigFilePath, "Path to Ant XML database file must be entered")
            IsValid = False
        Else
            epManualUpdater.SetError(txtConfigFilePath, "")
            If Not File.Exists(txtConfigFilePath.Text) Then
                epManualUpdater.SetError(txtConfigFilePath, "Path to Ant XML database file is not valid")
            End If
        End If

        'Check an operation is selected:
        If cbManualSelectOperation.SelectedIndex < 0 Then
            epManualUpdater.SetError(cbManualSelectOperation, "You must select an operation.")
            IsValid = False
            grpManualInternetLookupSettings.Visible = False
        Else
            epManualUpdater.SetError(cbManualSelectOperation, "")
            'If so, check the cbManualSelectField and txtManualNewValue fields:
            If cbManualSelectOperation.SelectedItem = "Update Value" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Replace String" Then
                'Update value replace : requires cbManualSelectField and txtManualOldValue and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualOldValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualOldValue, "Please enter the old value to be replaced")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualOldValue, "")
                End If
                If txtManualNewValue.Text = txtManualOldValue.Text Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to replace DIFFERENT than old one")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Add String" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to add")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Insert String" Then
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to insert")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Remove String" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to remove")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Delete Value" Then
                'Delete Value : requires cbManualSelectField
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to delete")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Copy Value" Then
                'Copy Value : requires cbManualSelectField and cbManualSelectFieldDestination
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field as source to copy from")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If cbManualSelectFieldDestination.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectFieldDestination, "Please select a field as destination to copy to")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectFieldDestination, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Download Fanart" Then
                'Delete Value : requires cbManualSelectField
                If txtFanartFolder.Text = String.Empty Then
                    epManualUpdater.SetError(cbManualSelectOperation, "Please select a Path to Fanart download")
                    IsValid = False
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update NFO File" Or cbManualSelectOperation.SelectedItem = "Delete NFO File" Then
                If cbManualNfoFileHandling.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualNfoFileHandling, "Please select NFO file handling")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualNfoFileHandling, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Record" Then
                If IsInternetLookupNeeded() = True Then
                    grpManualInternetLookupSettings.Visible = True
                    'Excluded Movie File Path Required:
                    If txtExcludeFilePath.Text = String.Empty Then
                        epManualUpdater.SetError(txtExcludeFilePath, "Path to excluded movies file must be entered")
                        IsValid = False
                    Else
                        'Check it's a valid path:
                        Dim wpath As String = txtExcludeFilePath.Text
                        If Not wpath.Contains("\") Then
                            'Not a path without a backslash!
                            epManualUpdater.SetError(txtExcludeFilePath, "Please enter a valid file path")
                            IsValid = False
                        Else
                            wpath = wpath.Substring(0, wpath.LastIndexOf("\"))
                            If Directory.Exists(wpath) Then
                                If txtExcludeFilePath.Text.EndsWith("\") = True Then
                                    txtExcludeFilePath.Text += "AMCUpdater_Excluded_Files.txt"
                                ElseIf txtExcludeFilePath.Text.EndsWith(".txt") = True Then
                                    'Else
                                    'txtManualExcludedMoviesPath.Text += "\AMCUpdater_Excluded_Files.txt"
                                End If
                                epInteractive.SetError(txtExcludeFilePath, "")
                            Else
                                epInteractive.SetError(txtExcludeFilePath, "Please enter a valid file path")
                                IsValid = False
                            End If
                        End If
                    End If

                    'Internet parser file required:
                    If txtManualInternetParserPath.Text = String.Empty Then
                        epManualUpdater.SetError(txtManualInternetParserPath, "Path to Internet Grabber Script must be entered.")
                        epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Path to Internet Grabber Script must be entered.")
                        IsValid = False
                    Else
                        Dim wpath As String = txtManualInternetParserPath.Text
                        If Not wpath.Contains("\") Then
                            'Not a path without a backslash!
                            epManualUpdater.SetError(txtManualInternetParserPath, "Please enter a valid Internet Grabber Script path.")
                            epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Please enter a valid Internet Grabber Script path.")
                            IsValid = False
                        Else
                            If File.Exists(wpath) Then
                                epManualUpdater.SetError(txtManualInternetParserPath, "")
                                epManualUpdater.SetError(txtManualInternetParserPathDisplay, "")
                            Else
                                epManualUpdater.SetError(txtManualInternetParserPath, "Please enter a valid Internet Grabber Script path.")
                                epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Please enter a valid Internet Grabber Script path.")
                                IsValid = False
                            End If
                        End If
                    End If
                End If

            ElseIf cbManualSelectOperation.SelectedItem = "Add IMDB to File" Then
                'Add logic checks here if necessary
            End If
        End If

        If chkManualParametersUpdateAll.Checked Then
            epManualUpdater.SetError(cbManualParameterFieldList1, "")
            epManualUpdater.SetError(cbManualParameterOperator1, "")
            epManualUpdater.SetError(txtManualParameterValue1, "")
            epManualUpdater.SetError(cbManualParameterFieldList2, "")
            epManualUpdater.SetError(cbManualParameterOperator2, "")
            epManualUpdater.SetError(txtManualParameterValue2, "")
        Else
            If cbManualParameterFieldList1.SelectedIndex < 0 Then
                epManualUpdater.SetError(cbManualParameterFieldList1, "Please select a field")
                IsValid = False
            Else
                epManualUpdater.SetError(cbManualParameterFieldList1, "")
            End If
            If cbManualParameterOperator1.SelectedIndex < 0 Then
                epManualUpdater.SetError(cbManualParameterOperator1, "Please select a comparison operator")
                IsValid = False
            Else
                epManualUpdater.SetError(cbManualParameterOperator1, "")
                If cbManualParameterOperator1.SelectedItem <> "EXISTS" And cbManualParameterOperator1.SelectedItem <> "NOT EXISTS" Then
                    If txtManualParameterValue1.Text = String.Empty Then
                        epManualUpdater.SetError(txtManualParameterValue1, "Please enter a value")
                        IsValid = False
                    Else
                        epManualUpdater.SetError(txtManualParameterValue1, "")
                    End If
                Else
                    epManualUpdater.SetError(txtManualParameterValue1, "")
                End If
            End If
            If cbManualParameterAndOr.SelectedIndex >= 0 Then
                If cbManualParameterFieldList2.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualParameterFieldList2, "Please select a field")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualParameterFieldList2, "")
                End If
                If cbManualParameterOperator2.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualParameterOperator2, "Please select a comparison operator")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualParameterOperator2, "")
                    If cbManualParameterOperator2.SelectedItem <> "EXISTS" And cbManualParameterOperator2.SelectedItem <> "NOT EXISTS" Then
                        If txtManualParameterValue2.Text = String.Empty Then
                            epManualUpdater.SetError(txtManualParameterValue2, "Please enter a value")
                            IsValid = False
                        Else
                            epManualUpdater.SetError(txtManualParameterValue2, "")
                        End If
                    Else
                        epManualUpdater.SetError(txtManualParameterValue2, "")
                    End If
                End If
            End If

        End If

        If IsValid = True Then
            'btnManualApplyChanges.Enabled = True
            btnManualDoTest.Enabled = True
            btnManualCancel.Enabled = False
        Else
            btnManualApplyChanges.Enabled = False
            btnManualDoTest.Enabled = False
        End If

    End Sub
    Private Sub Options_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtDefaultSourceField.Validating, chkExecuteProgram.Validating, txtExecuteProgramPath.Validating
        'txtDefaultFileType
        'txtDefaultSourceField
        'chkExecuteProgram
        'txtExecuteProgramPath
        'Dim IsValid As Boolean = True

        ValidOptions = True

        If txtDefaultFileTypes.Text.Length < 1 Then
            epOptions.SetError(txtDefaultFileTypes, "Some file types must be entered.")
            ValidOptions = False
        ElseIf txtDefaultFileTypes.Text.ToLower.Contains("ifo") Then
            epOptions.SetError(txtDefaultFileTypes, "You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
            ValidOptions = False
        ElseIf txtDefaultFileTypes.Text.ToLower.Contains("bdmv") Then
            epOptions.SetError(txtDefaultFileTypes, "You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultFileTypes, "")
        End If

        If txtDefaultFileTypesNonMedia.Text.ToLower.Contains("ifo") Then
            epOptions.SetError(txtDefaultFileTypesNonMedia, "You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
            ValidOptions = False
        ElseIf txtDefaultFileTypesNonMedia.Text.ToLower.Contains("bdmv") Then
            epOptions.SetError(txtDefaultFileTypesNonMedia, "You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultFileTypesNonMedia, "")
        End If

        Dim SourceChecked As String
        Dim SourceIndex As Integer = cbDatabaseFields.Items.IndexOf(txtDefaultSourceField.Text)
        If SourceIndex > -1 Then
            SourceChecked = cbDatabaseFields.GetItemChecked(SourceIndex).ToString
        Else
            SourceChecked = ""
        End If

        If txtDefaultSourceField.Text.Length < 1 Then
            epOptions.SetError(txtDefaultSourceField, "Source Field is mandatory. The default is 'Source'.")
            ValidOptions = False
        ElseIf SourceChecked = "True" Then
            epOptions.SetError(txtDefaultSourceField, "Make sure the media Source Field is not selected to be updated in database fields.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultSourceField, "")
        End If

        Dim DuplicateMediaTypes As Boolean = False
        Dim MediaTypes() As String
        Dim NonMediaTypes() As String
        MediaTypes = txtDefaultFileTypes.Text.Split(";")
        NonMediaTypes = txtDefaultFileTypesNonMedia.Text.Split(";")
        If MediaTypes.Length > 0 And NonMediaTypes.Length > 0 Then
            For i As Integer = 0 To MediaTypes.Length - 1
                For ii As Integer = 0 To NonMediaTypes.Length - 1
                    If MediaTypes(i).ToLower = NonMediaTypes(ii).ToLower Then
                        DuplicateMediaTypes = True
                    End If
                Next
            Next
        End If

        If DuplicateMediaTypes = True Then
            epOptions.SetError(txtDefaultFileTypes, "Duplicate entries detected.")
            epOptions.SetError(txtDefaultFileTypesNonMedia, "Duplicate entries detected.")
        Else
            epOptions.SetError(txtDefaultFileTypes, "")
            epOptions.SetError(txtDefaultFileTypesNonMedia, "")
        End If

        'Dim DBFields() As String
        'DBFields = CurrentSettings.Database_Fields_To_Import.Split(";")
        'Dim FieldName As String
        'Dim FieldSelected As Boolean
        'cbDatabaseFields.Items.Clear()
        'For i As Integer = 0 To DBFields.Length - 1
        '    If DBFields(i).Length > 0 Then
        '        FieldName = DBFields(i).Substring(0, DBFields(i).IndexOf("|"))
        '        FieldSelected = DBFields(i).Substring(DBFields(i).IndexOf("|") + 1)
        '        cbDatabaseFields.Items.Add(FieldName, FieldSelected)
        '    End If
        'Next

        If chkExecuteProgram.Checked = True Then
            If txtExecuteProgramPath.Text.Length < 1 Then
                epOptions.SetError(txtExecuteProgramPath, "Program path must be entered if 'execute program' is enabled.")
                ValidOptions = False
            Else
                epOptions.SetError(txtExecuteProgramPath, "")
            End If
        Else
            epOptions.SetError(txtExecuteProgramPath, "")
        End If

        Dim ErrorText As String = String.Empty
        ErrorText = "Prefix cannot begin with a '\' character." & vbCrLf
        ErrorText += "Entry must be either a string to append, or a relative path, or both." & vbCrLf
        ErrorText += "Valid entries include:" & vbCrLf
        ErrorText += "'prefix' to add a prefix to each filename." & vbCrLf
        ErrorText += "'Folder\' to store images in a subfolder.  Only one subfolder is permitted" & vbCrLf
        ErrorText += "'Folder\prefix' to do both." & vbCrLf

        Dim PathCounter As Integer = 0
        For i As Integer = 0 To txtPictureFilenamePrefix.Text.Length - 1
            If txtPictureFilenamePrefix.Text.Chars(i) = "\" Then
                PathCounter += 1
            End If
        Next

        If txtPictureFilenamePrefix.Text.StartsWith("\") = True Or PathCounter > 1 Then
            epOptions.SetError(txtPictureFilenamePrefix, ErrorText)
        Else
            epOptions.SetError(txtPictureFilenamePrefix, "")
            ValidOptions = False
        End If

    End Sub
    Private Sub Database_Fields_Validation()
        Dim item As Object
        Dim FieldName As String
        Dim FieldChecked As String
        Dim InternetLookupNeeded As Boolean = False

        For Each item In cbDatabaseFields.Items
            FieldName = item.ToString
            FieldChecked = cbDatabaseFields.GetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName)).ToString

            If FieldChecked = "True" Then
                If InternetData.ContainsValue(FieldName.ToLower) Then
                    If FieldName = "Languages" Then
                        If chkUseInternetDataForLanguagesField.Checked = True Then
                            InternetLookupNeeded = True
                        End If
                    Else
                        InternetLookupNeeded = True
                    End If
                End If
            End If


            If FieldName = txtDefaultSourceField.Text Then
                If FieldChecked = "True" Then
                    epOptions.SetError(txtDefaultSourceField, "Make sure the media Source Field is not selected to be updated in database fields.")
                    ' cbDatabaseFields.SetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName), False)
                Else
                    epOptions.SetError(txtDefaultSourceField, "")
                End If
            End If

            If FieldName = "OriginalTitle" Then
                'Original Title needs special handling - only required if TitleHandling says to use Internet lookup:
                If FieldChecked = True Then
                    If cbTitleHandling.SelectedIndex > -1 Then
                        If cbTitleHandling.SelectedItem.ToString.Contains("Internet Lookup") = True Then
                            InternetLookupNeeded = True
                        End If
                    End If
                    cbTitleHandling.Enabled = True
                Else
                    cbTitleHandling.Enabled = False
                End If
            ElseIf FieldName = "Date" Then
                If FieldChecked = True Then
                    cbDateHandling.Enabled = True
                Else
                    cbDateHandling.Enabled = False
                End If
            ElseIf FieldName = "Checked" Then
                If FieldChecked = True Then
                    cbCheckHandling.Enabled = True
                Else
                    cbCheckHandling.Enabled = False
                End If
            ElseIf FieldName = "Picture" Then
                If FieldChecked = True Then
                    cbPictureHandling.Enabled = True
                    txtPictureFilenamePrefix.Enabled = True
                    txtPictureFilenameSuffix.Enabled = True
                    lblPictureHandling.Enabled = True
                    lblPicturePrefix.Enabled = True
                Else
                    cbPictureHandling.Enabled = False
                    txtPictureFilenamePrefix.Enabled = False
                    txtPictureFilenameSuffix.Enabled = False
                    lblPictureHandling.Enabled = False
                    lblPicturePrefix.Enabled = False
                End If
            ElseIf FieldName = "Fanart" Then
                If FieldChecked = True Then
                    cbPictureHandling.Enabled = True
                    lblPictureHandling.Enabled = True
                    cbFanartLimitResolutionMin.Enabled = True
                    cbFanartLimitResolutionMax.Enabled = True
                    cbNumFanartLimitNumber.Enabled = True

                Else
                    'cbPictureHandling.Enabled = False
                    'lblPictureHandling.Enabled = False
                    cbFanartLimitResolutionMin.Enabled = False
                    cbFanartLimitResolutionMax.Enabled = False
                    cbNumFanartLimitNumber.Enabled = False
                End If
            End If

        Next

        If InternetLookupNeeded = True Then
            lblInternetLookupRequired.Visible = True
        Else
            lblInternetLookupRequired.Visible = False
        End If

        If chkFolderNameIsGroupName.Checked Then
            lblGroupNameAppliesTo.Enabled = True
            cbGroupNameAppliesTo.Enabled = True
        Else
            lblGroupNameAppliesTo.Enabled = False
            cbGroupNameAppliesTo.Enabled = False
        End If

        If cbPictureHandling.SelectedIndex > -1 Then
            If cbPictureHandling.SelectedItem = "Use Folder.jpg" Then
                txtPictureFilenamePrefix.Enabled = False
                txtPictureFilenameSuffix.Enabled = False
                lblPicturePrefix.Enabled = False
            Else
                txtPictureFilenamePrefix.Enabled = True
                txtPictureFilenameSuffix.Enabled = True
                lblPicturePrefix.Enabled = True
            End If
        Else
            txtPictureFilenamePrefix.Enabled = True
            txtPictureFilenameSuffix.Enabled = True
            lblPicturePrefix.Enabled = True
        End If

        If chkProhibitInternetLookup.Checked = True Then
            lblInternetLookupRequired.Visible = False
        End If
    End Sub

#End Region

#Region "Menus etc."
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        frmAbout.ShowDialog()
    End Sub
    Private Sub MyFilmsWikiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyFilmsWikiToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films/Updating_AMC_Data/AMC_Updater")
    End Sub
    Private Sub AntMovieCatalogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AntMovieCatalogToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://www.antp.be/software/moviecatalog")

    End Sub
    Private Sub MediaInfodllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaInfodllToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://mediainfo.sourceforge.net")
    End Sub
    Private Sub XMLToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XMLToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblXml")
    End Sub
    Private Sub MediaFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaFileToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundMediaFiles")
    End Sub
    Private Sub NonMediaFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NonMediaFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundNonMediaFiles")
    End Sub
    Private Sub TrailerFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaFileToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundTrailerFiles")
    End Sub
    Private Sub OrphanMediaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanMediaToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedMediaFiles")
    End Sub
    Private Sub OrphanNonMediaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanNonMediaToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedNonMediaFiles")
    End Sub
    Private Sub OrphanTrailerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanTrailerToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedTrailerMediaFiles")
    End Sub
    Private Sub MultiPartFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultiPartFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblMultiPartFiles")
    End Sub
    Private Sub OrphanAntToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanAntToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedAntRecords")
    End Sub
    Private Sub MultipartProcessedFilesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MultiPartProcessedFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblMultiPartProcessedFiles")
    End Sub
    Private Sub AntFieldsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AntFieldsToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblAntFields")
    End Sub
    Private Sub NodesToProcessToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NodesToProcessToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblNodesToProcess")
    End Sub
    Public Sub LoadConfigurationFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadConfigurationFileToolStripMenuItem.Click
        Dim OldSettingsFile As String
        OldSettingsFile = CurrentSettings.UserSettingsFile
        Dim SettingsFile As String = String.Empty
        Dim InitialDirectory As String
        Dim MePoConfigPath As String = Config.GetDirectoryInfo(Config.Dir.Config).ToString
        If System.IO.Directory.Exists(MePoConfigPath) Then
            InitialDirectory = MePoConfigPath
        Else
            InitialDirectory = My.Application.Info.DirectoryPath
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = InitialDirectory
                .FileName = "*.xml"
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a configuration file to load"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        SettingsFile = .FileName
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        If SettingsFile.Length > 0 Then
            'CurrentSettings.UserSettingsFile = SettingsFile
            CurrentSettings.LoadUserSettings(SettingsFile)
            LoadSettings()
            ' Add config file name to window title
            If (SettingsFile.ToString.Length > 0) Then
                Me.Text = "AMC Updater - " & SettingsFile.ToString.Substring(SettingsFile.ToString.LastIndexOf("\") + 1)
            Else
                Me.Text = "Ant Movie Catalog Auto-Updater"
            End If

            Me.ValidateChildren()
        End If

    End Sub
    Private Sub SaveConfigFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveConfigFileAsToolStripMenuItem.Click
        ApplySettings()
        Dim MePoConfigPath As String = Config.GetDirectoryInfo(Config.Dir.Config).ToString
        Try
            With SaveFileDialog1
                If System.IO.Directory.Exists(MePoConfigPath) Then
                    .InitialDirectory = MePoConfigPath
                Else
                    .InitialDirectory = My.Application.Info.DirectoryPath
                End If
                .FileName = "AMCUpdater_Settings.xml"
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                '.Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                '.ShowReadOnly = False
                '.ReadOnlyChecked = False
                .Title = "Select a configuration file to save"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        CurrentSettings.UserSettingsFile = .FileName
                        CurrentSettings.SaveUserSettings()
                    Catch fileException As Exception
                        LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try

    End Sub
    Private Sub UserManualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' Currently not used (no updated manual available), Menu entry removed from About Dropdown
        Try
            If My.Computer.FileSystem.FileExists(My.Application.Info.DirectoryPath & "\AMCUpdater User Guide.pdf") Then
                System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\AMCUpdater User Guide.pdf")
            Else
                MsgBox("Sorry - User guide not found.")
            End If
        Catch ex As Exception
            MsgBox("Sorry - Cannot load user guide.  Do you have Acrobat Reader installed?")
        End Try
    End Sub
#End Region

#Region "Settings"
    Private Sub ApplySettings()
        CurrentSettings.Ant_Database_Source_Field = txtDefaultSourceField.Text
        CurrentSettings.Ant_Media_Label = txtMediaLabel.Text
        CurrentSettings.Ant_Media_Type = txtMediaType.Text
        CurrentSettings.Backup_XML_First = chkBackupXMLFirst.Checked
        CurrentSettings.Date_Handling = cbDateHandling.SelectedItem
        CurrentSettings.Movie_Title_Handling = cbTitleHandling.SelectedItem
        CurrentSettings.Excluded_Movies_File = txtExcludeFilePath.Text
        CurrentSettings.Execute_Only_For_Orphans = chkExecuteOnlyForOrphans.Checked
        CurrentSettings.Execute_Program = chkExecuteProgram.Checked
        CurrentSettings.Execute_Program_Path = txtExecuteProgramPath.Text
        CurrentSettings.File_Types_Media = txtDefaultFileTypes.Text
        CurrentSettings.File_Types_Non_Media = txtDefaultFileTypesNonMedia.Text
        CurrentSettings.File_Types_Trailer = txtTrailerIentificationStrings.Text
        CurrentSettings.Internet_Parser_Path = txtParserFilePath.Text
        CurrentSettings.Dont_Ask_Interactive = chkDontAskInteractive.Checked
        CurrentSettings.Manual_Dont_Ask_Interactive = chkManualDontAskInteractive.Checked
        CurrentSettings.Log_Level = cbLogLevel.SelectedItem
        CurrentSettings.Movie_Fanart_Path = txtFanartFolder.Text
        CurrentSettings.Movie_Fanart_Resolution_Min = cbFanartLimitResolutionMin.Text
        CurrentSettings.Movie_Fanart_Resolution_Max = cbFanartLimitResolutionMax.Text
        CurrentSettings.Movie_Fanart_Number_Limit = cbNumFanartLimitNumber.Value
        CurrentSettings.Movie_PersonArtwork_Path = txtPersonArtworkFolder.Text
        CurrentSettings.Movie_Scan_Path = txtMovieFolder.Text
        CurrentSettings.Override_Path = txtOverridePath.Text
        CurrentSettings.Overwrite_XML_File = chkOverwriteXML.Checked
        CurrentSettings.Purge_Missing_Files = chkPurgeMissing.Checked
        CurrentSettings.Purge_Missing_Files_When_Source_Unavailable = chkPurgeMissingAlways.Checked
        CurrentSettings.RegEx_Check_For_MultiPart_Files = txtRegExSearchMultiPart.Text
        CurrentSettings.Scan_For_DVD_Folders = chkCheckDVDFolders.Checked
        CurrentSettings.Store_Short_Names_Only = chkShortNames.Checked
        CurrentSettings.XML_File = txtConfigFilePath.Text
        CurrentSettings.Read_DVD_Label = chkReadDVDLabel.Checked
        CurrentSettings.Folder_Name_Is_Group_Name = chkFolderNameIsGroupName.Checked
        CurrentSettings.Parse_Playlist_Files = chkParsePlaylistFiles.Checked
        CurrentSettings.Parse_Trailers = chkParseTrailers.Checked
        CurrentSettings.Image_Download_Filename_Prefix = txtPictureFilenamePrefix.Text
        CurrentSettings.Image_Download_Filename_Suffix = txtPictureFilenameSuffix.Text
        CurrentSettings.Prohibit_Internet_Lookup = chkProhibitInternetLookup.Checked
        CurrentSettings.Parse_Subtitle_Files = chkParseSubtitleFiles.Checked
        CurrentSettings.Rescan_Moved_Files = chkRescanMovedFiles.Checked
        CurrentSettings.Only_Add_Missing_Data = chkManualUpdateRecordsOnlyMissingData.Checked
        CurrentSettings.Only_Update_With_Nonempty_Data = chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Checked

        CurrentSettings.Manual_Internet_Lookup_Always_Prompt = cbManualInternetLookupAlwaysPrompt.Checked 'cbManualInternetLookupBehaviour.SelectedValue
        CurrentSettings.Manual_Internet_Parser_Path = txtManualInternetParserPath.Text

        CurrentSettings.Import_File_On_Internet_Lookup_Failure = chkImportOnInternetFail.Checked
        CurrentSettings.Import_File_On_Internet_Lookup_Failure_In_Guimode = chkImportOnInternetFailInGuiMode.Checked
        CurrentSettings.Internet_Lookup_Always_Prompt = cbInternetLookupAlwaysPrompt.Checked 'cbInternetLookupBehaviour.SelectedValue

        CurrentSettings.Group_Name_Identifier = txtGroupNameIdentifier.Text
        CurrentSettings.Series_Name_Identifier = txtSeriesNameIdentifier.Text

        CurrentSettings.Use_InternetData_For_Languages = chkUseInternetDataForLanguagesField.Checked
        CurrentSettings.Use_Grabber_For_Fanart = chkUseGrabberForFanart.Checked
        CurrentSettings.Load_Person_Images_With_Fanart = chkLoadPersonImagesWithFanart.Checked

        CurrentSettings.Skip_Excluded_Movie_Files = cbSkipExcludedMovieFiles.Checked

        Select Case cbPictureHandling.SelectedItem
            Case "Relative Path"
                CurrentSettings.Store_Image_With_Relative_Path = True
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Relative Path & Create Moviethumb"
                CurrentSettings.Store_Image_With_Relative_Path = True
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = True
            Case "Use Folder.jpg"
                CurrentSettings.Use_Folder_Dot_Jpg = True
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Create Moviethumb"
                CurrentSettings.Use_Folder_Dot_Jpg = True
                CurrentSettings.Create_Cover_From_Movie = True
            Case "Full Path"
                CurrentSettings.Store_Image_With_Relative_Path = False
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Full Path & Create Moviethumb"
                CurrentSettings.Store_Image_With_Relative_Path = False
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = True
        End Select


        If cbCheckHandling.SelectedItem = "Checked" Then
            CurrentSettings.Check_Field_Handling = True
        Else
            CurrentSettings.Check_Field_Handling = False
        End If


        Dim DBFields As String = ""
        Dim FieldName As String
        Dim FieldChecked As String
        Dim item As Object

        'My.Settings.DatabaseFields = ""
        For Each item In cbDatabaseFields.Items
            FieldName = item.ToString
            FieldChecked = cbDatabaseFields.GetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName)).ToString
            If Not DBFields.Contains(FieldName) Then
                DBFields += FieldName & "|" & FieldChecked & ";"
            End If
        Next
        If DBFields.EndsWith(";") Then
            DBFields = DBFields.Substring(0, DBFields.Length - 1)
        End If
        CurrentSettings.Database_Fields_To_Import = DBFields

        'Console.WriteLine(dgExcludedFileStrings.Rows.Count)
        Dim dgRow As System.Windows.Forms.DataGridViewRow
        Dim ExcludedFileString As String = String.Empty
        For Each dgRow In dgExcludedFileStrings.Rows.Cast(Of DataGridViewRow)()
            If (dgRow.Cells(0).Value IsNot Nothing) AndAlso dgRow.Cells(0).Value.ToString.Length > 0 Then
                ExcludedFileString += dgRow.Cells(0).Value.ToString & "|"
            End If
        Next
        If ExcludedFileString.Length > 0 Then
            ExcludedFileString = ExcludedFileString.Substring(0, ExcludedFileString.Length - 1)
        End If
        CurrentSettings.Excluded_File_Strings = ExcludedFileString
        'CurrentSettings.Excluded_Files_Table = dgExcludedFileStrings.DataSource

        Dim ExcludedFolderString As String = String.Empty
        For Each dgRow In dgExcludedFolderStrings.Rows.Cast(Of DataGridViewRow)()
            If (dgRow.Cells(0).Value IsNot Nothing) AndAlso dgRow.Cells(0).Value.ToString.Length > 0 Then
                ExcludedFolderString += dgRow.Cells(0).Value.ToString & "|"
            End If
        Next
        If ExcludedFolderString.Length > 0 Then
            ExcludedFolderString = ExcludedFolderString.Substring(0, ExcludedFolderString.Length - 1)
        End If
        CurrentSettings.Excluded_Folder_Strings = ExcludedFolderString

        Dim FilterString As String = String.Empty
        For Each dgRow In dgFilterStrings.Rows.Cast(Of DataGridViewRow)()
            If (dgRow.Cells(0).Value IsNot Nothing) AndAlso dgRow.Cells(0).Value.ToString.Length > 0 Then
                FilterString += dgRow.Cells(0).Value.ToString & "|"
            End If
        Next
        If FilterString.Length > 0 Then
            FilterString = FilterString.Substring(0, FilterString.Length - 1)
        End If
        CurrentSettings.Filter_Strings = FilterString

        Dim EditionString As String = String.Empty
        For Each dgRow In dgEditionStrings.Rows.Cast(Of DataGridViewRow)()
            If dgRow.Cells(0).Value IsNot Nothing And dgRow.Cells(1).Value IsNot Nothing Then
                If dgRow.Cells(0).Value.ToString.Length > 0 And dgRow.Cells(1).Value.ToString.Length > 0 Then
                    EditionString += dgRow.Cells(0).Value.ToString & "|" & dgRow.Cells(1).Value.ToString & ";"
                End If
            End If
        Next
        If EditionString.Length > 0 Then
            EditionString = EditionString.Substring(0, EditionString.Length - 1) ' strip last separator
        End If
        CurrentSettings.Edition_Strings = EditionString

        CurrentSettings.Group_Name_Applies_To = cbGroupNameAppliesTo.SelectedItem.ToString

        If Not cbEditionNameAppliesTo.SelectedItem Is Nothing Then
            CurrentSettings.Edition_Name_Applies_To = cbEditionNameAppliesTo.SelectedItem.ToString
        Else
            CurrentSettings.Edition_Name_Applies_To = ""
        End If

        CurrentSettings.Master_Title = cbMasterTitle.SelectedItem.ToString
        CurrentSettings.Grabber_Override_Language = chkGrabberOverrideLanguage.Text.ToString()
        CurrentSettings.Grabber_Override_GetRoles = chkGrabberOverrideGetRoles.Text.ToString
        CurrentSettings.Grabber_Override_PersonLimit = chkGrabberOverridePersonLimit.Text.ToString
        CurrentSettings.Grabber_Override_TitleLimit = chkGrabberOverrideTitleLimit.Text.ToString

    End Sub
    Private Sub LoadSettings()
        If Not CurrentSettings Is Nothing Then
            txtDefaultSourceField.Text = CurrentSettings.Ant_Database_Source_Field
            txtMediaLabel.Text = CurrentSettings.Ant_Media_Label
            txtMediaType.Text = CurrentSettings.Ant_Media_Type
            chkBackupXMLFirst.Checked = CurrentSettings.Backup_XML_First
            cbDateHandling.SelectedItem = CurrentSettings.Date_Handling
            cbTitleHandling.SelectedItem = CurrentSettings.Movie_Title_Handling
            txtExcludeFilePath.Text = CurrentSettings.Excluded_Movies_File
            chkExecuteOnlyForOrphans.Checked = CurrentSettings.Execute_Only_For_Orphans
            chkExecuteProgram.Checked = CurrentSettings.Execute_Program
            txtExecuteProgramPath.Text = CurrentSettings.Execute_Program_Path
            txtDefaultFileTypes.Text = CurrentSettings.File_Types_Media
            txtDefaultFileTypesNonMedia.Text = CurrentSettings.File_Types_Non_Media
            txtTrailerIentificationStrings.Text = CurrentSettings.File_Types_Trailer
            chkImportOnInternetFail.Checked = CurrentSettings.Import_File_On_Internet_Lookup_Failure
            chkImportOnInternetFailInGuiMode.Checked = CurrentSettings.Import_File_On_Internet_Lookup_Failure_In_Guimode
            cbInternetLookupAlwaysPrompt.Checked = CurrentSettings.Internet_Lookup_Always_Prompt 'cbInternetLookupBehaviour.SelectedValue = CurrentSettings.Internet_Lookup_Always_Prompt
            txtParserFilePath.Text = CurrentSettings.Internet_Parser_Path
            cbLogLevel.SelectedItem = CurrentSettings.Log_Level
            txtMovieFolder.Text = CurrentSettings.Movie_Scan_Path
            txtFanartFolder.Text = CurrentSettings.Movie_Fanart_Path
            cbFanartLimitResolutionMin.Text = CurrentSettings.Movie_Fanart_Resolution_Min
            cbFanartLimitResolutionMax.Text = CurrentSettings.Movie_Fanart_Resolution_Max
            cbNumFanartLimitNumber.Value = CurrentSettings.Movie_Fanart_Number_Limit
            txtPersonArtworkFolder.Text = CurrentSettings.Movie_PersonArtwork_Path
            txtOverridePath.Text = CurrentSettings.Override_Path
            chkOverwriteXML.Checked = CurrentSettings.Overwrite_XML_File
            chkPurgeMissing.Checked = CurrentSettings.Purge_Missing_Files
            If (chkPurgeMissing.Checked) Then
                chkPurgeMissingAlways.Enabled = True
            Else
                chkPurgeMissingAlways.Enabled = False
            End If
            chkPurgeMissingAlways.Checked = CurrentSettings.Purge_Missing_Files_When_Source_Unavailable
            txtRegExSearchMultiPart.Text = CurrentSettings.RegEx_Check_For_MultiPart_Files
            chkCheckDVDFolders.Checked = CurrentSettings.Scan_For_DVD_Folders
            chkShortNames.Checked = CurrentSettings.Store_Short_Names_Only
            txtConfigFilePath.Text = CurrentSettings.XML_File
            chkReadDVDLabel.Checked = CurrentSettings.Read_DVD_Label
            chkDontAskInteractive.Checked = CurrentSettings.Dont_Ask_Interactive
            chkManualDontAskInteractive.Checked = CurrentSettings.Manual_Dont_Ask_Interactive
            chkFolderNameIsGroupName.Checked = CurrentSettings.Folder_Name_Is_Group_Name
            chkParsePlaylistFiles.Checked = CurrentSettings.Parse_Playlist_Files
            chkParseTrailers.Checked = CurrentSettings.Parse_Trailers
            txtPictureFilenamePrefix.Text = CurrentSettings.Image_Download_Filename_Prefix
            txtPictureFilenameSuffix.Text = CurrentSettings.Image_Download_Filename_Suffix
            chkProhibitInternetLookup.Checked = CurrentSettings.Prohibit_Internet_Lookup
            chkParseSubtitleFiles.Checked = CurrentSettings.Parse_Subtitle_Files
            chkRescanMovedFiles.Checked = CurrentSettings.Rescan_Moved_Files
            chkManualUpdateRecordsOnlyMissingData.Checked = CurrentSettings.Only_Add_Missing_Data
            chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Checked = CurrentSettings.Only_Update_With_Nonempty_Data
            cbSkipExcludedMovieFiles.Checked = CurrentSettings.Skip_Excluded_Movie_Files


            chkUseInternetDataForLanguagesField.Checked = CurrentSettings.Use_InternetData_For_Languages
            chkUseGrabberForFanart.Checked = CurrentSettings.Use_Grabber_For_Fanart
            chkLoadPersonImagesWithFanart.Checked = CurrentSettings.Load_Person_Images_With_Fanart

            txtManualInternetParserPath.Text = CurrentSettings.Manual_Internet_Parser_Path
            If CurrentSettings.Manual_Internet_Lookup_Always_Prompt = "True" Then
                cbManualInternetLookupAlwaysPrompt.Checked = True ' cbManualInternetLookupBehaviour.SelectedValue = True
            Else
                cbManualInternetLookupAlwaysPrompt.Checked = False ' cbManualInternetLookupBehaviour.SelectedValue = False
            End If
            If CurrentSettings.Check_Field_Handling = True Then
                cbCheckHandling.SelectedItem = "Checked"
            Else
                cbCheckHandling.SelectedItem = "Unchecked"
            End If

            cbGroupNameAppliesTo.SelectedItem = CurrentSettings.Group_Name_Applies_To
            cbEditionNameAppliesTo.SelectedItem = CurrentSettings.Edition_Name_Applies_To
            cbMasterTitle.SelectedItem = CurrentSettings.Master_Title
            chkGrabberOverrideLanguage.Text = CurrentSettings.Grabber_Override_Language
            chkGrabberOverrideGetRoles.Text = CurrentSettings.Grabber_Override_GetRoles
            chkGrabberOverridePersonLimit.Text = CurrentSettings.Grabber_Override_PersonLimit
            chkGrabberOverrideTitleLimit.Text = CurrentSettings.Grabber_Override_TitleLimit

            txtGroupNameIdentifier.Text = CurrentSettings.Group_Name_Identifier
            txtSeriesNameIdentifier.Text = CurrentSettings.Series_Name_Identifier

            Dim DBFields() As String
            DBFields = CurrentSettings.Database_Fields_To_Import.Split(";")
            Array.Sort(DBFields)
            Dim FieldName As String
            Dim FieldSelected As Boolean
            Dim FieldFanart As Boolean = False
            Dim FormattedTitle As Boolean = False
            cbDatabaseFields.Items.Clear()
            For i As Integer = 0 To DBFields.Length - 1
                If DBFields(i).Length > 0 Then
                    FieldName = DBFields(i).Substring(0, DBFields(i).IndexOf("|"))
                    If (FieldName = "Fanart") Then
                        FieldFanart = True
                    End If
                    If (FieldName = "FormattedTitle") Then
                        FormattedTitle = True
                    End If
                    FieldSelected = DBFields(i).Substring(DBFields(i).IndexOf("|") + 1)
                    cbDatabaseFields.Items.Add(FieldName, FieldSelected)
                End If
            Next
            If Not FieldFanart Then
                cbDatabaseFields.Items.Add("Fanart", False)
            End If
            If Not FormattedTitle Then
                cbDatabaseFields.Items.Add("FormattedTitle", False)
            End If
            Dim dtFolders As New DataTable
            dtFolders.Columns.Add("FolderString", System.Type.GetType("System.String"))
            If CurrentSettings.Excluded_Folder_Strings.Length > 0 Then
                Dim ExcludeFolders() As String
                ExcludeFolders = CurrentSettings.Excluded_Folder_Strings.Split("|")
                For i As Integer = 0 To ExcludeFolders.Length - 1
                    dtFolders.Rows.Add(ExcludeFolders(i))
                Next
            End If
            dgExcludedFolderStrings.DataSource = dtFolders
            dgExcludedFolderStrings.Columns(0).Width = dgExcludedFolderStrings.Width - dgExcludedFolderStrings.RowHeadersWidth - 20

            Dim dtFiles As New DataTable
            dtFiles.Columns.Add("FileString", System.Type.GetType("System.String"))
            If CurrentSettings.Excluded_File_Strings.Length > 0 Then
                Dim ExcludeFiles() As String
                ExcludeFiles = CurrentSettings.Excluded_File_Strings.Split("|")
                If ExcludeFiles.Length > 0 Then
                    For i As Integer = 0 To ExcludeFiles.Length - 1
                        dtFiles.Rows.Add(ExcludeFiles(i))
                    Next
                End If
            End If
            dgExcludedFileStrings.DataSource = dtFiles
            dgExcludedFileStrings.Columns(0).Width = dgExcludedFileStrings.Width - dgExcludedFileStrings.RowHeadersWidth - 20

            Dim dtFilterStrings As New DataTable
            dtFilterStrings.Columns.Add("FilterString", System.Type.GetType("System.String"))
            If CurrentSettings.Filter_Strings.Length > 0 Then
                Dim FilterStrings() As String
                FilterStrings = CurrentSettings.Filter_Strings.Split("|")
                If FilterStrings.Length > 0 Then
                    For i As Integer = 0 To FilterStrings.Length - 1
                        dtFilterStrings.Rows.Add(FilterStrings(i))
                    Next
                End If
            End If
            dgFilterStrings.DataSource = dtFilterStrings
            dgFilterStrings.Columns(0).Width = dgFilterStrings.Width - dgFilterStrings.RowHeadersWidth - 20

            dgEditionStrings.Rows.Clear()
            If CurrentSettings.Edition_Strings.Length > 0 Then
                Dim EditionStrings() As String
                EditionStrings = CurrentSettings.Edition_Strings.Split(";")
                If EditionStrings.Length > 0 Then
                    For i As Integer = 0 To EditionStrings.Length - 1
                        Dim ValueStrings() As String
                        ValueStrings = EditionStrings(i).Split("|")
                        dgEditionStrings.Rows.Add(ValueStrings(0), ValueStrings(1))
                    Next
                End If
            End If

            If (CurrentSettings.Import_File_On_Internet_Lookup_Failure = "True") Then
                chkImportOnInternetFail.Checked = True
            Else
                chkImportOnInternetFail.Checked = False
            End If

            If (CurrentSettings.Import_File_On_Internet_Lookup_Failure_In_Guimode = "True") Then
                chkImportOnInternetFailInGuiMode.Checked = True
            Else
                chkImportOnInternetFailInGuiMode.Checked = False
            End If

            If chkOverwriteXML.Checked = True Then
                chkBackupXMLFirst.Enabled = True
            Else
                chkBackupXMLFirst.Checked = False
                chkBackupXMLFirst.Enabled = False
            End If

            If CurrentSettings.Internet_Lookup_Always_Prompt = "True" Then
                cbInternetLookupAlwaysPrompt.Checked = True ' cbInternetLookupBehaviour.SelectedValue = True
            Else
                cbInternetLookupAlwaysPrompt.Checked = False ' cbInternetLookupBehaviour.SelectedValue = False
            End If

            If CurrentSettings.Store_Image_With_Relative_Path = True Then
                cbPictureHandling.SelectedItem = "Relative Path"
            Else
                cbPictureHandling.SelectedItem = "Full Path"
            End If

            If CurrentSettings.Use_Folder_Dot_Jpg = True Then
                cbPictureHandling.SelectedItem = "Use Folder.jpg"
            End If

            If CurrentSettings.Create_Cover_From_Movie = True Then
                cbPictureHandling.SelectedItem = "Create Moviethumb"
            End If

            ' set use case selector according settings for import tab
            If cbInternetLookupAlwaysPrompt.Checked = False And chkDontAskInteractive.Checked = True And chkImportOnInternetFailInGuiMode.Checked = False Then
                cbInternetLookupBehaviour.Text = "Auto match only" '"Silent Mode - no import if no match"
            ElseIf cbInternetLookupAlwaysPrompt.Checked = False And chkDontAskInteractive.Checked = True And chkImportOnInternetFailInGuiMode.Checked = True Then
                cbInternetLookupBehaviour.Text = "Auto match & media only if no match" ' "Silent Mode - import media if no match"
            ElseIf cbInternetLookupAlwaysPrompt.Checked = False And chkDontAskInteractive.Checked = False And chkImportOnInternetFailInGuiMode.Checked = False Then
                cbInternetLookupBehaviour.Text = "Auto match & ask if no match" ' default? ' "Interactive Mode - only ask if no match"
                'ElseIf cbInternetLookupAlwaysPrompt.Checked = False And chkDontAskInteractive.Checked = False And chkImportOnInternetFailInGuiMode.Checked = True Then 'use case to import media, if manual selection fails - not supported - we map to 'auto match & ask if no match'
                '    cbInternetLookupBehaviour.Text = "Auto match & ask if no match" '
            ElseIf cbInternetLookupAlwaysPrompt.Checked = True And chkDontAskInteractive.Checked = False And chkImportOnInternetFailInGuiMode.Checked = False Then
                cbInternetLookupBehaviour.Text = "Manual match (always ask)" ' "Interactive Mode - always ask"
            Else
                cbInternetLookupBehaviour.Text = ""
            End If

            ' set use case selector according settings for update tab
            If cbManualInternetLookupAlwaysPrompt.Checked = False And chkManualDontAskInteractive.Checked = True Then
                cbManualInternetLookupBehaviour.Text = "Auto match only" '"Silent Mode - no import if no match"
            ElseIf cbManualInternetLookupAlwaysPrompt.Checked = False And chkManualDontAskInteractive.Checked = False Then
                cbManualInternetLookupBehaviour.Text = "Auto match & ask if no match" ' default? ' "Interactive Mode - only ask if no match"
            ElseIf cbManualInternetLookupAlwaysPrompt.Checked = True And chkManualDontAskInteractive.Checked = False Then
                cbManualInternetLookupBehaviour.Text = "Manual match (always ask)" ' "Interactive Mode - always ask"
            Else
                cbManualInternetLookupBehaviour.Text = ""
            End If

            Me.ValidateChildren()

        End If
    End Sub
    Private Sub ApplyUseCasesToSettings()

        Select Case cbInternetLookupBehaviour.SelectedItem
            Case "Auto match, then manual match" ' ' 2 runs one after each other
                btnParseXML.Enabled = False
                btnProcessMovieList.Enabled = False
                btnFindOrphans.Enabled = False
                btnUpdateXML.Enabled = False

            Case Else
                btnParseXML.Enabled = True
                btnProcessMovieList.Enabled = True
                btnFindOrphans.Enabled = True
                btnUpdateXML.Enabled = True
        End Select

        Select Case cbInternetLookupBehaviour.SelectedItem
            Case "Auto match only" '"Silent Mode - no import if no match"
                cbInternetLookupAlwaysPrompt.Checked = False  ' internetlookup always prompt
                chkDontAskInteractive.Checked = True ' Don't ask if no match
                chkImportOnInternetFailInGuiMode.Checked = False ' import if matching fails in AMC GUI mode
                lblInternetLookupCaseExplanation.Text = "Unattended - this mode allows you to import movies unattended, but only with correct matches." & Environment.NewLine & "Films that cannot be matched will not be imported. You can rerun the import using one of the interactive modes and select correct matches for the ones AMCU could not match automatically."

            Case "Auto match & media only if no match" ' "Silent Mode - import media if no match"
                cbInternetLookupAlwaysPrompt.Checked = False
                chkDontAskInteractive.Checked = True
                chkImportOnInternetFailInGuiMode.Checked = True
                lblInternetLookupCaseExplanation.Text = "Unattended - this mode allows you to import movies unattended and include films, that cannot be automatically matched." & Environment.NewLine & "AMCU will not ask you if it does not find a match, instead it will import only the file name, title and media info."

            Case "Auto match & ask if no match" ' default? ' "Interactive Mode - only ask if no match"
                cbInternetLookupAlwaysPrompt.Checked = False
                chkDontAskInteractive.Checked = False
                chkImportOnInternetFailInGuiMode.Checked = False
                lblInternetLookupCaseExplanation.Text = "Partly interactive - AMCU will match most of your films correctly. If it finds multiple, bad or no matches it will provide you a list with recommended matches based on year or IMDb tt numbers if available." & Environment.NewLine & "This is the most commonly used mode for most users as it provides the best results with only minimal interaction. The import process cannot run unattended because user action is required each time a movie cannot be matched."

            Case "Manual match (always ask)" ' "Interactive Mode - always ask"
                cbInternetLookupAlwaysPrompt.Checked = True
                chkDontAskInteractive.Checked = False
                chkImportOnInternetFailInGuiMode.Checked = False
                lblInternetLookupCaseExplanation.Text = "Fully interactive - AMCU will not match any film automatically, instead always allows you to review the result and confirm and select the match you want." & Environment.NewLine & "This is a pure interactive mode, which allows you to always select the correct match for your films, including the manual selection of alternative websites for better results."

            Case "Auto match, then manual match" ' ' 2 runs one after each other
                lblInternetLookupCaseExplanation.Text = "Combined - this is a combination of 'Auto match only' and 'Manual match (always ask)'." & Environment.NewLine & "On first run, movies that can be matched will be imported, on a second run, the remaining will be handled interactively." & Environment.NewLine & "Note: This setting will not be saved."

            Case Else
                lblInternetLookupCaseExplanation.Text = "Default case - should not happen !" ' explanation of use case
        End Select
    End Sub

    Private Sub ApplyManualUseCasesToSettings()
        Select Case cbManualInternetLookupBehaviour.SelectedItem
            Case "Auto match only" ' never ask - no update if no match"
                cbManualInternetLookupAlwaysPrompt.Checked = False  ' internetlookup always prompt
                chkManualDontAskInteractive.Checked = True ' Don't ask if no match

            Case "Auto match & ask if no match" ' only ask if no match
                cbManualInternetLookupAlwaysPrompt.Checked = False
                chkManualDontAskInteractive.Checked = False

            Case "Manual match (always ask)" ' always ask
                cbManualInternetLookupAlwaysPrompt.Checked = True
                chkManualDontAskInteractive.Checked = False
            Case Else
        End Select
    End Sub
#End Region

    Private Sub btnSelectFanartFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFanartFolder.Click
        Dim currentPath As String
        currentPath = txtFanartFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your movie backdrops fanart are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtFanartFolder.Text = .SelectedPath
                End If
            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub btnSelectPersonArtworkFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectPersonArtworkFolder.Click
        Dim currentPath As String
        currentPath = txtPersonArtworkFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your person artwork thumbs are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtPersonArtworkFolder.Text = .SelectedPath
                End If
            End With
        Catch ex As Exception
            LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()

    End Sub

    Public Sub txtConfigFilePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtConfigFilePath.TextChanged
        CurrentSettings.XML_File = txtConfigFilePath.Text
    End Sub

    Private Sub VidéoBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VidéoBindingNavigatorSaveItem.Click

        BindingNavigatorPositionItem.Focus()

        ' copy custom fields to AMC4 extended fields
        Dim commonColumns As IEnumerable(Of DataColumn) = myMovieCatalog.Movie.Columns.OfType(Of DataColumn)().Intersect(myMovieCatalog.CustomFields.Columns.OfType(Of DataColumn)(), New DataColumnComparer()).Where(Function(x) x.ColumnName <> "Movie_Id").ToList()
        For Each movieRow As AntMovieCatalog.MovieRow In myMovieCatalog.Movie
            If movieRow.RowState <> DataRowState.Deleted Then
                movieRow.BeginEdit()
                Dim customFields As AntMovieCatalog.CustomFieldsRow = Nothing
                If movieRow.GetCustomFieldsRows().Length = 0 Then
                    ' create CustomFields Element, if not existing ...
                    customFields = myMovieCatalog.CustomFields.NewCustomFieldsRow()
                    customFields.SetParentRow(movieRow)
                    myMovieCatalog.CustomFields.AddCustomFieldsRow(customFields)
                End If
                customFields = movieRow.GetCustomFieldsRows()(0)
                For Each dc As DataColumn In commonColumns
                    customFields(dc.ColumnName) = movieRow(dc.ColumnName)
                Next
            End If
        Next
        myMovieCatalog.Movie.AcceptChanges()

        ' new method to save:
        Try
            Using fsTmp = File.Create(CurrentSettings.XML_File.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose) ' creates "lock file"
                Using fs = New FileStream(CurrentSettings.XML_File, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read) ' make sure, only one process is writing to file !
                    ' lock the file for any other use, as we do write to it now !
                    fs.SetLength(0) ' do not append, owerwrite !
                    Using myXmlTextWriter = New XmlTextWriter(fs, System.Text.Encoding.Default)
                        myXmlTextWriter.Formatting = Formatting.Indented
                        myXmlTextWriter.WriteStartDocument()
                        myMovieCatalog.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema)
                        myXmlTextWriter.Flush()
                        myXmlTextWriter.Close()
                    End Using
                    'xmlDoc.Save(fs);
                    fs.Close() ' write buffer and release lock on file (either Flush, Dispose or Close is required)
                End Using
                fsTmp.Close()
            End Using

            Dim info As New FileInfo(CurrentSettings.XML_File)
            Dim length As Long = info.Length
            LogEvent("Finished saving xml file to disk  - movies: '" & myMovieCatalog.Movie.Count() & "', Size = '" & length & "' bytes.", EventLogLevel.Informational)
        Catch ex As Exception
            MessageBox.Show("Error saving xml - error : " & ex.Message, "AMC Updater - Save", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        ' old method:
        'Dim destXml As New System.Xml.XmlTextWriter(CurrentSettings.XML_File, System.Text.Encoding.Default)
        'destXml.WriteStartDocument(False)
        'destXml.Formatting = System.Xml.Formatting.Indented
        'myMovieCatalog.WriteXml(destXml)
        'destXml.Close()
    End Sub

    Private Sub SpeichernToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpeichernToolStripButton.Click
        BindingNavigatorPositionItem.Focus()
        Dim destXml As New System.Xml.XmlTextWriter(CurrentSettings.XML_File, System.Text.Encoding.Default)
        destXml.WriteStartDocument(False)
        destXml.Formatting = System.Xml.Formatting.Indented
        myMovieCatalog.WriteXml(destXml)
        destXml.Close()
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorAddNewItem.Click
        Dim newMovie As AntMovieCatalog.MovieRow
        newMovie = myMovieCatalog.Movie.NewMovieRow()

        Dim x = (From y In myMovieCatalog.Movie _
                 Where y.Number <> Nothing _
                 Select CType(y.Number, Integer?)).Max

        newMovie.Contents_Id = 0
        newMovie.Number = x + 1
        newMovie.OriginalTitle = "New Movie"
        myMovieCatalog.Movie.AddMovieRow(newMovie)
        LogEvent("Added new Movie : '" & newMovie.OriginalTitle & "', Number : '" & newMovie.Number & "'", EventLogLevel.Informational)
        'LogEvent("ErrorEvent : ", EventLogLevel.Informational)
    End Sub

    Private Sub BindingNavigatorAddNewItemPerson_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorAddNewItemPerson.Click
        Dim amc As AntMovieCatalog = New AntMovieCatalog()
        Dim newPerson As AntMovieCatalog.PersonRow
        newPerson = amc.Person.NewPersonRow()
        newPerson.Persons_Id = 0
        newPerson.Name = "New Person"
        newPerson.IsActor = False
        newPerson.IsProducer = False
        newPerson.IsDirector = False
        newPerson.IsWriter = False
        myMovieCatalog.Person.AddPersonRow(newPerson)
        LogEvent("Added new Person : '" & newPerson.Name & "'", EventLogLevel.Informational)
    End Sub

    Private Sub TabControl1_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles TabControl1.Selected

        If TabControl1.SelectedIndex = 6 Or TabControl1.SelectedIndex = 7 Or TabControl1.SelectedIndex = 8 Then
            Dim myMovieTable As AMCUpdater.AntMovieCatalog.MovieDataTable = Nothing
            Dim myMovieView As DataView
            Dim myPersonTable As AMCUpdater.AntMovieCatalog.PersonDataTable = Nothing
            Dim myPersonView As DataView
            Dim myProperties As AMCUpdater.AntMovieCatalog.PropertiesDataTable = Nothing
            Dim wdir As String

            If (txtConfigFilePath.Text.Length > 0) Then
                myMovieCatalog = New AntMovieCatalog()

                If TabControl1.SelectedIndex = 6 Then
                    Try
                        wdir = System.IO.Path.GetDirectoryName(txtConfigFilePath.Text)
                    Catch ex As Exception
                        wdir = String.Empty
                    End Try
                ElseIf TabControl1.SelectedIndex = 7 Then
                    Try
                        wdir = System.IO.Path.GetDirectoryName(txtPersonArtworkFolder.Text)
                    Catch ex As Exception
                        wdir = String.Empty
                    End Try
                Else
                    wdir = String.Empty
                End If

                ' Set current directory to retrieve images without full pathnames
                If (System.IO.Directory.Exists(wdir)) Then
                    Directory.SetCurrentDirectory(wdir)
                End If

                If (System.IO.File.Exists(txtConfigFilePath.Text)) Then
                    LoadMyFilmsFromDisk(txtConfigFilePath.Text)
                    'myMovieCatalog.ReadXml(txtConfigFilePath.Text)

                    'myMovieTable = myMovieCatalog.Tables("Movie")
                    myMovieTable = myMovieCatalog.Movie
                    myMovieView = New DataView(myMovieTable)
                    MovieBindingSource.DataSource = myMovieTable

                    myPersonTable = myMovieCatalog.Person
                    myPersonView = New DataView(myPersonTable)
                    PersonBindingSource.DataSource = myPersonTable

                    myProperties = myMovieCatalog.Properties
                    PropertiesBindingSource.DataSource = myProperties
                Else
                    MovieBindingSource.DataSource = myMovieTable
                    PersonBindingSource.DataSource = myPersonTable
                    PropertiesBindingSource.DataSource = myProperties
                End If
            End If
            MovieBindingSource.ResumeBinding()
            PersonBindingSource.ResumeBinding()
            PropertiesBindingSource.ResumeBinding()
        Else
            MovieBindingSource.SuspendBinding()
            PersonBindingSource.SuspendBinding()
            PropertiesBindingSource.SuspendBinding()
        End If
    End Sub

    Private Sub BindingNavigatorPositionItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorPositionItem.Click

    End Sub

    Private Function LoadMyFilmsFromDisk(ByVal catalogfile As String) As Boolean
        '#Region "load catalog from file into dataset"
        Dim success As Boolean = False
        Try
            Using fs As New FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read)
                For Each dataTable As DataTable In myMovieCatalog.Tables
                    dataTable.BeginLoadData()
                Next
                '''/ synchronize dataset with hierarchical XMLdoc
                'xmlDoc = new XmlDataDocument(myMovieCatalog);
                'xmlDoc.Load(fs);
                myMovieCatalog.ReadXml(fs)
                For Each dataTable As DataTable In myMovieCatalog.Tables
                    dataTable.EndLoadData()
                Next
                fs.Close()
            End Using
            success = True

            Dim info As New FileInfo(catalogfile)
            Dim length As Long = info.Length
            LogEvent("Finished loading xml file from disk  - movies: '" & myMovieCatalog.Movie.Count() & "', Size = '" & length & "' bytes.", EventLogLevel.Informational)
        Catch e As Exception
            success = False
            Dim Message As String = "Error reading xml database after " & myMovieCatalog.Movie.Count & " records; "
            If myMovieCatalog.Movie.Count > 0 Then
                Message = Message & "movie: '" & myMovieCatalog.Movie(myMovieCatalog.Movie.Count - 1).OriginalTitle & "'; "
            End If
            Message = Message & "error : " & e.Message
            MessageBox.Show(Message, "AMC Updater - DB Reader", MessageBoxButtons.OK, MessageBoxIcon.Error)
#If Config = "Release" Then
#Else
            MessageBox.Show(e.StackTrace, "Exception Details (Debug only)", MessageBoxButtons.OK, MessageBoxIcon.Error)
#End If

        Finally
        End Try

        'CreateOrUpdateCustomsFieldsProperties()

        'CreateMissingCustomFieldsEntries()

        Dim commonColumns As IEnumerable(Of DataColumn) = myMovieCatalog.Movie.Columns.OfType(Of DataColumn)().Intersect(myMovieCatalog.CustomFields.Columns.OfType(Of DataColumn)(), New DataColumnComparer()).Where(Function(x) x.ColumnName <> "Movie_Id").ToList()
        ''data.Movie.BeginLoadData();
        ''data.EnforceConstraints = false; // primary key uniqueness, foreign key referential integrity and nulls in columns with AllowDBNull = false etc...
        For Each movieRow As AntMovieCatalog.MovieRow In myMovieCatalog.Movie
            '    movieRow.BeginEdit()
            '    ' Convert(Date,'System.DateTime')
            '    Dim added As DateTime
            '    Dim iAge As Integer = 9999
            '    ' set default to 9999 for those, where we do not have date(added) in DB ...
            '    ' CultureInfo ci = CultureInfo.CurrentCulture;
            '    If Not movieRow.IsDateNull() AndAlso DateTime.TryParse(movieRow.[Date], added) Then
            '        ' CultureInfo.InvariantCulture ??? // else movieRow.DateAdded = DateTime.MinValue; ???
            '        movieRow.DateAdded = added
            '        ' iAge = (!movieRow.IsDateAddedNull()) ? ((int)now.Subtract(movieRow.DateAdded).TotalDays) : 9999;
            '        iAge = CInt(now.Subtract(added).TotalDays)
            '    End If
            '    movieRow.AgeAdded = iAge
            '    ' sets integer value
            '    movieRow.RecentlyAdded = MyFilms.GetDayRange(iAge)
            '    Dim index As String = movieRow(MyFilms.conf.StrTitle1).ToString()
            '    movieRow.IndexedTitle = If((index.Length > 0), index.Substring(0, 1).ToUpper(), "")
            '    movieRow.Persons = (If(movieRow.Actors, " ")) & ", " & (If(movieRow.Producer, " ")) & ", " & (If(movieRow.Director, " ")) & ", " & (If(movieRow.Writer, " "))
            '    ' Persons: ISNULL(Actors,' ') + ', ' + ISNULL(Producer, ' ') + ', ' + ISNULL(Director, ' ') + ', ' + ISNULL(Writer, ' ')
            '    ' if (!movieRow.IsLengthNull()) movieRow.Length_Num = Convert.ToInt32(movieRow.Length);
            If movieRow.GetCustomFieldsRows().Length > 0 Then
                ' customfields are present - use it! (we only create them on saving)
                Dim customFields = movieRow.GetCustomFieldsRows()(0)
                ' Relations["Movie_CustomFields"]
                For Each dc As DataColumn In commonColumns
                    'object temp;
                    '''/ only copy CustomFields, if not nothing, as user might have initial values in Elements!
                    'if (DBNull.Value != (temp = customFields[dc.ColumnName]))
                    '  movieRow[dc.ColumnName] = temp;
                    movieRow(dc.ColumnName) = customFields(dc.ColumnName)
                Next
            End If            '    ' Copy CustomFields data ....
            '    Dim customFields As AntMovieCatalog.CustomFieldsRow = movieRow.GetCustomFieldsRows()(0)
            '    ' Relations["Movie_CustomFields"]
            '    For Each dc As DataColumn In commonColumns
            '        Dim temp As Object
            '        If dc.ColumnName <> "Movie_Id" AndAlso DBNull.Value <> (InlineAssignHelper(temp, customFields(dc.ColumnName))) Then
            '            movieRow(dc.ColumnName) = temp
            '        End If
            '    Next
        Next
        ''data.EnforceConstraints = true;
        ''data.Movie.EndLoadData();
        myMovieCatalog.Movie.AcceptChanges()

        Return success
    End Function

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        '        TextBox2.Text = 1
        If TextBox2.Text.Length = 0 Then
            BindingNavigatorUpdate.Enabled = False
        Else
            BindingNavigatorUpdate.Enabled = True
        End If
    End Sub

    Private Sub SaveConfigFileToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveConfigFileToolStripMenuItem.Click
        ApplySettings()
        Dim MePoConfigPath As String = Config.GetDirectoryInfo(Config.Dir.Config).ToString
        If My.Application.CommandLineArgs.Count > 0 Then
            Try
                Try
                    CurrentSettings.UserSettingsFile = My.Application.CommandLineArgs.Item(0)
                    CurrentSettings.SaveUserSettings()
                Catch fileException As Exception
                    LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                End Try

            Catch ex As Exception
                LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
                Console.WriteLine(ex.Message)
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
            End Try
        Else
            Try
                With SaveFileDialog1
                    If System.IO.Directory.Exists(MePoConfigPath) Then
                        .InitialDirectory = MePoConfigPath
                    Else
                        .InitialDirectory = My.Application.Info.DirectoryPath
                    End If
                    .FileName = "AMCUpdater_Settings.xml"
                    .CheckFileExists = False
                    .CheckPathExists = True
                    .DefaultExt = "xml"
                    .DereferenceLinks = True
                    .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                    '.Multiselect = False
                    .RestoreDirectory = True
                    .ShowHelp = True
                    '.ShowReadOnly = False
                    '.ReadOnlyChecked = False
                    .Title = "Select a configuration file to save"
                    .ValidateNames = True
                    If .ShowDialog = Windows.Forms.DialogResult.OK Then
                        Try
                            CurrentSettings.UserSettingsFile = .FileName
                            CurrentSettings.SaveUserSettings()
                        Catch fileException As Exception
                            LogEvent("ErrorEvent : " + fileException.Message, EventLogLevel.ErrorEvent)
                        End Try
                    End If

                End With
            Catch ex As Exception
                LogEvent("ErrorEvent : " + ex.Message, EventLogLevel.ErrorEvent)
                Console.WriteLine(ex.Message)
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
            End Try

        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtParserFilePath.Text & """"
            psi.ErrorDialog = True
            If (OSInfo.OSInfo.VistaOrLater()) Then
                psi.Verb = "runas"
            End If
            p.StartInfo = psi
            Try
                p.Start()
            Catch
            End Try
        End Using
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtManualInternetParserPath.Text & """"
            psi.ErrorDialog = True
            If (OSInfo.OSInfo.VistaOrLater()) Then
                psi.Verb = "runas"
            End If
            p.StartInfo = psi
            Try
                p.Start()
            Catch
            End Try
        End Using
    End Sub

    Private Sub txtParserFilePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtParserFilePath.TextChanged
        If txtParserFilePath.Text <> "" Then
            txtParserFilePathDisplay.Text = Path.GetFileName(txtParserFilePath.Text)
        Else
            txtParserFilePathDisplay.Text = ""
        End If
    End Sub

    Private Sub btnExcludeFileShow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcludeFileShow.Click
        Dim t As String = txtExcludeFilePath.Text
        If Not Directory.Exists(Path.GetDirectoryName(txtExcludeFilePath.Text)) Then
            Directory.CreateDirectory(Path.GetDirectoryName(txtExcludeFilePath.Text).ToString())
        End If
        If Not File.Exists(txtExcludeFilePath.Text) Then
            Dim sr As StreamWriter = File.CreateText(txtExcludeFilePath.Text)
            'sr.WriteLine("This is my file.")
            sr.Close()
            'Dim xmlFile As New FileStream(txtManualExcludedMoviesPath.Text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
            'xmlFile.SetLength(0)
            'xmlFile.Close()
        End If
        Try
            Process.Start(t)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnExcludeFileDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcludeFileDelete.Click
        Dim response As MsgBoxResult = MessageBox.Show("Are you sure you want to delete the file ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If response = MsgBoxResult.Yes Then
            Try
                'take a backup for manual restore first
                LogEvent("Backing up exclusion file.", EventLogLevel.ImportantEvent)
                Dim NewFileName As String = Replace(txtExcludeFilePath.Text, ".txt", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-").Replace("/", "-") + ".txt")
                My.Computer.FileSystem.CopyFile(txtExcludeFilePath.Text, NewFileName, True)
                My.Computer.FileSystem.DeleteFile(txtExcludeFilePath.Text)
                MsgBox("File deleted !", MsgBoxStyle.Information)
            Catch deleteException As Exception
                MsgBox("An ErrorEvent occurred ! - ErrorEvent: " + deleteException.Message, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub txtManualInternetParserPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualInternetParserPath.TextChanged
        If txtManualInternetParserPath.Text <> "" Then
            txtManualInternetParserPathDisplay.Text = Path.GetFileName(txtManualInternetParserPath.Text)
        Else
            txtManualInternetParserPathDisplay.Text = ""
        End If
    End Sub

    Private Sub LinkLabelMyFilmsWiki_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelMyFilmsWiki.LinkClicked
        System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films/Updating_AMC_Data/AMC_Updater")
    End Sub

    Private Sub ToolStripButtonAddMissingPersons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButtonAddMissingPersons.Click
        Dim Persons As String()
        For Each row As AntMovieCatalog.MovieRow In MovieBindingSource.DataSource
            Try
                Persons = row.Actors.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Actor)
                Next
            Catch ex As Exception
            End Try

            Try
                Persons = row.Producer.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Producer)
                Next
            Catch ex As Exception
            End Try

            Try
                Persons = row.Director.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Director)
                Next
            Catch ex As Exception
            End Try

            Try
                Persons = row.Writer.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Writer)
                Next
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Enum PersonType
        Actor
        Producer
        Director
        Writer
    End Enum

    Private Sub AddOrUpdatePerson(ByVal person As String, ByVal type As PersonType)
        person = person.Trim
        If person.Contains("(") Then
            person = person.Substring(0, person.IndexOf("(")).Trim
        End If
        Dim personRows() As AntMovieCatalog.PersonRow
        personRows = PersonBindingSource.DataSource.Select("Name = '" + person + "'")

        'Dim personRow As MyFilmsPlugin.AntMovieCatalog.PersonRow
        'personRow = PersonBindingSource.DataSource.FindByName(name) ' .FindByMiniBiography()

        If personRows.Count > 0 Then
            personRows(0).Name = person
            personRows(0).IsActor = False
            personRows(0).IsProducer = False
            personRows(0).IsDirector = False
            personRows(0).IsWriter = False
            Select Case type
                Case PersonType.Actor
                    personRows(0).IsActor = True
                Case PersonType.Producer
                    personRows(0).IsProducer = True
                Case PersonType.Director
                    personRows(0).IsDirector = True
                Case PersonType.Writer
                    personRows(0).IsWriter = True
            End Select
        Else
            'Dim newPerson As AMCUpdater.AntMovieCatalog.MovieRow
            Dim newPerson As AMCUpdater.AntMovieCatalog.PersonRow
            newPerson = PersonBindingSource.DataSource.NewPersonRow()
            newPerson.Persons_Id = 0
            newPerson.Name = person
            newPerson.IsActor = False
            newPerson.IsProducer = False
            newPerson.IsDirector = False
            newPerson.IsWriter = False
            Select Case type
                Case PersonType.Actor
                    newPerson.IsActor = True
                Case PersonType.Producer
                    newPerson.IsProducer = True
                Case PersonType.Director
                    newPerson.IsDirector = True
                Case PersonType.Writer
                    newPerson.IsWriter = True
            End Select
            PersonBindingSource.DataSource.Rows.Add(newPerson)
        End If
    End Sub

    Private Sub ToolStripButtonGrabPersons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButtonGrabPersons.Click
        MsgBox("Not yet implemented !", MsgBoxStyle.Information)
    End Sub

    Private Shared Sub InitLogger()
        'Dim config As Nlog.LoggingConfiguration = LogManager.Configuration ?? new LoggingConfiguration
        Dim LogEvent As LoggingConfiguration
        If Not LogManager.Configuration Is Nothing Then
            LogEvent = LogManager.Configuration
        Else
            LogEvent = New LoggingConfiguration
        End If

        Const LogFileName As String = "MyFilmsAMCU.log"
        Const OldLogFileName As String = "MyFilmsAMCU-old.log"
        Try
            Dim logFile As FileInfo = New FileInfo(Config.GetFile(Config.Dir.Log, LogFileName))
            If logFile.Exists = True Then
                If (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileName))) Then
                    File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileName))

                    logFile.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileName))
                    logFile.Delete()
                End If
            End If
        Catch ex As Exception
        End Try

        Dim fileTarget As FileTarget = New FileTarget
        fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName)
        fileTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff} " & "${level:fixedLength=true:padding=5} " & "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " & "${exception:format=tostring}"
        LogEvent.AddTarget("file", fileTarget)

        'Dim logWindowTarget As RichTextBoxTarget = New RichTextBoxTarget()
        'logWindowTarget.Name = "WindowLog"
        'logWindowTarget.FormName = "dgLogWindow"
        'logWindowTarget.ControlName = "RichTextBoxLogWindow" 'dgLogWindow.RichTextBoxLogWindow.Name
        'logWindowTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff} " & "${level:fixedLength=true:padding=5} " & "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " & "${exception:format=tostring}"
        ''logWindowTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}"
        'logWindowTarget.UseDefaultRowColoringRules = True
        ''logWindowTarget.WordColoringRules.Add("backgroundColor=grey fontColor=black ignoreCase=False regex="String" style="Enum" text="String" wholeWords = "Boolean")
        'LogEvent.AddTarget("logwindow", logWindowTarget)


        ' Get current Log Level from MediaPortal 
        Dim logLevel As NLog.LogLevel

        'logLevel = logLevel.ErrorEvent
        'logLevel = logLevel.Warn
        'logLevel = logLevel.Info
        logLevel = logLevel.Debug
#If DEBUG Then
        logLevel = logLevel.Debug
#End If

        Dim Rule As LoggingRule = New LoggingRule("*", logLevel, fileTarget)
        LogEvent.LoggingRules.Add(Rule)
        NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(fileTarget, logLevel.Trace)
        'Dim Rule2 As LoggingRule = New LoggingRule("*", logLevel, logWindowTarget)
        'LogEvent.LoggingRules.Add(Rule2)
        'NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(logWindowTarget, logLevel.Debug)
        LogManager.Configuration = LogEvent
    End Sub

    Private Sub btnOpenLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenLog.Click
        If CurrentLogDirectory.Length = 0 Then
            MessageBox.Show("The file '" + CurrentLogDirectory + "' does not exist!", "AMC Updater - Logfile", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        Else
            Using p As Process = New Process()
                Dim psi As ProcessStartInfo = New ProcessStartInfo()
                psi.FileName = "notepad.exe"
                psi.UseShellExecute = True
                psi.WindowStyle = ProcessWindowStyle.Normal
                psi.Arguments = CurrentLogDirectory
                psi.ErrorDialog = True
                If (OSInfo.OSInfo.VistaOrLater()) Then
                    psi.Verb = "runas"
                End If
                p.StartInfo = psi
                Try
                    p.Start()
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub chkPurgeMissing_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPurgeMissing.CheckedChanged
        If (chkPurgeMissing.Checked) Then
            chkPurgeMissingAlways.Enabled = True
            chkPurgeMissing.ForeColor = Color.Red
        Else
            chkPurgeMissing.ResetForeColor()
            chkPurgeMissingAlways.Enabled = False
            chkPurgeMissingAlways.Checked = False
        End If
    End Sub

    Private Sub BindingNavigatorUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorUpdate.Click

        Dim f As New IO.FileInfo(txtConfigFilePath.Text)
        If Not f.Exists Then
            MsgBox("File : " + f.FullName + " doesn't exist !", MsgBoxStyle.Critical)
            txtConfigFilePath.Focus()
            Return
        End If

        AntProcessor = New AntProcessor()
        AntProcessor.ManualOperation = "Update"
        AntProcessor.ManualXMLPath = txtConfigFilePath.Text
        ApplySettings()
        AntProcessor.ManualUpdateOperation()

        ReloadMovies()
    End Sub

    Private Sub ReloadMovies()

        Dim position As Integer = MovieBindingSource.Position

        Dim myMovieTable As AMCUpdater.AntMovieCatalog.MovieDataTable = Nothing
        Dim myPersonTable As AMCUpdater.AntMovieCatalog.PersonDataTable = Nothing
        Dim myProperties As AMCUpdater.AntMovieCatalog.PropertiesDataTable = Nothing
        myMovieCatalog = New AntMovieCatalog()
        Try
            Directory.SetCurrentDirectory(Path.GetDirectoryName(txtConfigFilePath.Text)) ' Set current directory to retrieve images without full pathnames
        Catch ex As Exception
        End Try
        If (File.Exists(txtConfigFilePath.Text)) Then
            LoadMyFilmsFromDisk(txtConfigFilePath.Text)
            myMovieTable = myMovieCatalog.Movie
            MovieBindingSource.DataSource = myMovieTable
            myProperties = myMovieCatalog.Properties
            PropertiesBindingSource.DataSource = myProperties
        Else
            MovieBindingSource.DataSource = myMovieTable
            PersonBindingSource.DataSource = myPersonTable
            PropertiesBindingSource.DataSource = myProperties
        End If
        MovieBindingSource.ResumeBinding()
        'MovieBindingSource.ResetBindings(False)
        PersonBindingSource.ResumeBinding()
        PropertiesBindingSource.ResumeBinding()
        MovieBindingSource.Position = position
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Dim ImageViewer As New frmImageViewer()
        Dim imagefile = Me.PictureBox1.ImageLocation
        If Not File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        Dim ImageViewer As New frmImageViewer()
        Dim imagefile = Me.PictureBox2.ImageLocation
        If Not File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub

    Private Sub cbInternetLookupBehaviour_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbInternetLookupBehaviour.SelectedIndexChanged
        ApplyUseCasesToSettings()
    End Sub

    Private Sub cbManualInternetLookupBehaviour_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualInternetLookupBehaviour.SelectedIndexChanged
        ApplyManualUseCasesToSettings()
    End Sub

    Private Sub btnImportFromDrive_Click(sender As System.Object, e As System.EventArgs) Handles btnImportFromDrive.Click
        Dim DriveLabel As String = DrvLabel("")
        Dim DiscID As String = GetIdentifier(VideoFormat.DVD, "")
        MsgBox(("DriveLabel = '" + DriveLabel + "', DiscID = '" & "'" & DiscID), MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class

Friend Class DataColumnComparer
    Implements IEqualityComparer(Of DataColumn)

    Public Overloads Function Equals(ByVal x As DataColumn, ByVal y As DataColumn) As Boolean Implements IEqualityComparer(Of DataColumn).Equals
        Return x.Caption = y.Caption
    End Function

    Public Overloads Function GetHashCode(ByVal obj As DataColumn) As Integer Implements IEqualityComparer(Of DataColumn).GetHashCode
        Return obj.Caption.GetHashCode()
    End Function

End Class


