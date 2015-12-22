Imports System.Windows.Forms
Imports System.Globalization
Imports Grabber
Imports System.Xml
Imports Grabber.Data
Imports Grabber.Util

Public Class AntRecord
    'Inherits Xml.XmlElement
    Private _movieNumber As Integer
    Private _filePath As String = String.Empty
    Private _allFilesPath As String = String.Empty
    Private _fileName As String = String.Empty
    Private _mediaLabel As String = String.Empty
    Private _mediaType As String = String.Empty
    Private _sourceField As String = "Source"
    Private _overridePath As String = String.Empty
    Private _parserPath As String = String.Empty
    Private _grabberOverrideLanguage As String = String.Empty
    Private _grabberOverrideGetRoles As String = String.Empty
    Private _grabberOverridePersonLimit As String = String.Empty
    Private _grabberOverrideTitleLimit As String = String.Empty
    'Private _xmlPath As String = String.Empty
    Private _internetLookupOk As Boolean
    Private _interactiveMode As Boolean
    Private _excludeFile As String = String.Empty
    Private _imagePath As String = String.Empty
    Private _internetLookupAlwaysPrompt As Boolean
    Private _lastOutputMessage As String = String.Empty
    Private _readDvdLabel As Boolean = False
    Private _dontAskInteractive As Boolean = False
    Private _xmlFilePath As String = String.Empty
    Private _internetSearchHint As String = String.Empty
    Private _internetSearchHintYear As String = String.Empty
    Private _internetSearchHintImdbId As String = String.Empty
    Private _groupName As String = String.Empty
    Private _downloadImage As Boolean = False
    Private _masterTitle As String = String.Empty
    Private _onlyAddMissing As Boolean = False
    Private _onlyUpdateNonEmptyData As Boolean = False
    Private _useInternetDataForLanguages As Boolean = False
    Private _databaseFields As New Hashtable
    'Private _processMode As ProcessModeNames

    Private Shared _internetData() As String
    Private Shared _mediaInfoData() As String

    Private Shared _xmlDoc As New Xml.XmlDocument
    Private Shared _xmlElement As Xml.XmlElement

    Public ProhibitInternetLookup As Boolean


    Private Enum GrabberOutput
        'OriginalTitle = 0
        'TranslatedTitle = 1
        'PicturePathLong = 2
        'Description = 3
        'Rating = 4
        'Actors = 5
        'Director = 6
        'Producer = 7
        'Year = 8
        'Country = 9
        'Category = 10
        'URL = 11
        'PicturePathShort = 12
        'Writer = 13
        'Comments = 14
        'Language = 15
        'Tagline = 16
        'Certification = 17
        'IMDB_Id = 18
        'IMDB_Rank = 19
        'Studio = 20
        'Edition = 21
        'Fanart = 22
        'Generic1 = 23
        'Generic2 = 24
        'Generic3 = 25
        'TranslatedTitleAllNames = 26
        'TranslatedTitleAllValues = 27
        'CertificationAllNames = 28
        'CertificationAllValues = 29
        'MultiPosters = 30
        'Photos = 31
        'PersonImages = 32
        'MultiFanart = 33
        'Trailer = 34
        'TMDB_Id = 35
        'Runtime = 36
        'Collection = 37
        'CollectionImageURL = 38
        'PictureURL = 39

        OriginalTitle = 40
        TranslatedTitle = 41
        PicturePathLong = 42
        Description = 43
        Rating = 44
        Actors = 45
        Director = 46
        Producer = 47
        Year = 48
        Country = 49
        Category = 50
        URL = 51
        PicturePathShort = 52
        Writer = 53
        Comments = 54
        Language = 55
        Tagline = 56
        Certification = 57
        IMDB_Id = 58
        IMDB_Rank = 59
        Studio = 60
        Edition = 61
        Fanart = 62
        Generic1 = 63
        Generic2 = 64
        Generic3 = 65
        TranslatedTitleAllNames = 66
        TranslatedTitleAllValues = 67
        CertificationAllNames = 68
        CertificationAllValues = 69
        MultiPosters = 70
        Photos = 71
        PersonImages = 72
        MultiFanart = 73
        Trailer = 74
        TMDB_Id = 75
        Runtime = 76
        Collection = 77
        CollectionImageURL = 78
        PictureURL = 79
    End Enum

    Private Enum MediaInfoOutput
        Filename = 0
        VideoFormat = 1
        VideoBitrate = 2
        Framerate = 3
        Resolution = 4
        AudioBitrate = 5
        AudioFormat = 6
        runtime = 7
        filesize = 8
        audiostreamcount = 9
        audiostreamcodeclist = 10
        audiostreamlanguagelist = 11
        textstreamcodeclist = 12
        textstreamlanguagelist = 13
    End Enum

    Public Enum ProcessModeNames
        Import
        Update
    End Enum

    Public Property InternetLookupAlwaysPrompt() As Boolean
        Get
            Return _internetLookupAlwaysPrompt
        End Get
        Set(ByVal value As Boolean)
            _internetLookupAlwaysPrompt = value
        End Set
    End Property
    Public Property SourceField() As String
        Get
            Return _sourceField
        End Get
        Set(ByVal Value As String)
            _sourceField = Value
        End Set
    End Property
    Public Property MediaType() As String
        Get
            Return _mediaType
        End Get
        Set(ByVal Value As String)
            _mediaType = Value
        End Set
    End Property
    Public Property MediaLabel() As String
        Get
            Return _mediaLabel
        End Get
        Set(ByVal Value As String)
            _mediaLabel = Value
        End Set
    End Property
    Public Property FileName() As String
        Get
            Return _fileName
        End Get
        Set(ByVal Value As String)
            _fileName = Value
        End Set
    End Property
    Public Property FilePath() As String
        Get
            Return _filePath
        End Get
        Set(ByVal Value As String)
            'If there's more than one file in FilePath just take the first one:
            If Value.Contains(";") Then
                'First reset _AllFilesPath...
                _allFilesPath = Value
                _filePath = Value.Substring(0, Value.IndexOf(";"))
            Else
                _filePath = Value
            End If

        End Set
    End Property
    Public Property AllFilesPath() As String
        Get
            Return _allFilesPath
        End Get
        Set(ByVal Value As String)
            _allFilesPath = Value
        End Set
    End Property
    Public Property OverridePath() As String
        Get
            Return _overridePath
        End Get
        Set(ByVal Value As String)
            _overridePath = Value
        End Set
    End Property
    Public Property MovieNumber() As Integer
        Get
            Return _movieNumber
        End Get
        Set(ByVal Value As Integer)
            _movieNumber = Value
        End Set
    End Property
    Public Property XMLElement() As Xml.XmlElement
        Get
            Return _xmlElement
        End Get
        Set(ByVal value As Xml.XmlElement)
            _xmlElement = value
        End Set
    End Property
    Public Property XMLDoc() As Xml.XmlDocument
        Get
            Return _xmlDoc
        End Get
        Set(ByVal value As Xml.XmlDocument)
            _xmlDoc = value
        End Set
    End Property
    Public Property ParserPath()
        Get
            Return _parserPath
        End Get
        Set(ByVal value)
            _parserPath = value
        End Set
    End Property
    Public Property GrabberOverrideLanguage()
        Get
            Return _grabberOverrideLanguage
        End Get
        Set(ByVal value)
            _grabberOverrideLanguage = value
        End Set
    End Property
    Public Property GrabberOverrideGetRoles()
        Get
            Return _grabberOverrideGetRoles
        End Get
        Set(ByVal value)
            _grabberOverrideGetRoles = value
        End Set
    End Property
    Public Property GrabberOverridePersonLimit()
        Get
            Return _grabberOverridePersonLimit
        End Get
        Set(ByVal value)
            _grabberOverridePersonLimit = value
        End Set
    End Property
    Public Property GrabberOverrideTitleLimit()
        Get
            Return _grabberOverrideTitleLimit
        End Get
        Set(ByVal value)
            _grabberOverrideTitleLimit = value
        End Set
    End Property
    Public ReadOnly Property InternetLookupOK() As Boolean
        Get
            Return _internetLookupOk
        End Get
    End Property
    Public Property InteractiveMode() As Boolean
        Get
            Return _interactiveMode
        End Get
        Set(ByVal value As Boolean)
            _interactiveMode = value
        End Set
    End Property
    Public Property ExcludeFile() As String
        Get
            Return _excludeFile
        End Get
        Set(ByVal value As String)
            _excludeFile = value
        End Set
    End Property
    Public Property ImagePath() As String
        Get
            Return _imagePath
        End Get
        Set(ByVal value As String)
            _imagePath = value
        End Set
    End Property
    Public Property Read_DVD_Label() As Boolean
        Get
            Return _readDvdLabel
        End Get
        Set(ByVal value As Boolean)
            _readDvdLabel = value
        End Set
    End Property
    Public Property Dont_Ask_Interactive() As Boolean
        Get
            Return _dontAskInteractive
        End Get
        Set(ByVal value As Boolean)
            _dontAskInteractive = value
        End Set
    End Property
    Public Property XMLFilePath() As String
        Get
            Return _xmlFilePath
        End Get
        Set(ByVal value As String)
            _xmlFilePath = value
        End Set
    End Property
    Public ReadOnly Property LastOutputMessage() As String
        Get
            Return _lastOutputMessage
        End Get
    End Property
    Public Property InternetSearchHint() As String
        Get
            Return _internetSearchHint
        End Get
        Set(ByVal value As String)
            _internetSearchHint = value
        End Set
    End Property
    Public Property InternetSearchHintYear() As String
        Get
            Return _internetSearchHintYear
        End Get
        Set(ByVal value As String)
            _internetSearchHintYear = value
        End Set
    End Property
    Public Property InternetSearchHintIMDB_Id() As String
        Get
            Return _internetSearchHintImdbId
        End Get
        Set(ByVal value As String)
            _internetSearchHintImdbId = value
        End Set
    End Property
    Public Property GroupName() As String
        Get
            Return _groupName
        End Get
        Set(ByVal value As String)
            _groupName = value
        End Set
    End Property
    Public Property MasterTitle() As String
        Get
            Return _masterTitle
        End Get
        Set(ByVal value As String)
            _masterTitle = value
        End Set
    End Property
    Public Property OnlyAddMissingData() As Boolean
        Get
            Return _onlyAddMissing
        End Get
        Set(ByVal value As Boolean)
            _onlyAddMissing = value
        End Set
    End Property
    Public Property OnlyUpdateNonEmptyData() As Boolean
        Get
            Return _onlyUpdateNonEmptyData
        End Get
        Set(ByVal value As Boolean)
            _onlyUpdateNonEmptyData = value
        End Set
    End Property
    Public Property UseInternetDataForLanguages() As Boolean
        Get
            Return _useInternetDataForLanguages
        End Get
        Set(ByVal value As Boolean)
            _useInternetDataForLanguages = value
        End Set
    End Property

    Public Sub New()

        Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
        Dim FieldName As String
        Dim FieldChecked As Boolean

        For Each blah As String In DBFields
            FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
            FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
            If Not _databaseFields.ContainsKey(FieldName) Then
                _databaseFields.Add(FieldName, FieldChecked)
            End If
            If FieldName = "picture" Then
                If FieldChecked = True Then
                    _downloadImage = True
                End If
            End If
        Next
    End Sub

    Public Sub CreateElement()
        _xmlElement = XMLDoc.CreateElement("Movie")
    End Sub

    Public Function VerifyElement(ByVal otitle As String, ByVal currentNode As Xml.XmlNode) As Xml.XmlNode
        '' added to avoid exception with empty otitle reported by z3us
        'If otitle Is Nothing Then
        '    Return currentNode
        'End If

        'Dim CurrentNode2 As Xml.XmlNode
        'Dim CurrentAttribute2 As String
        ''CurrentNode2 = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle='" & otitle & "']")
        'CurrentNode2 = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle=""" & otitle & """]")
        'If (Not CurrentNode2 Is Nothing) Then
        '    If (CurrentNode2.Attributes("Number").Value) <> (currentNode.Attributes("Number").Value) Then 'check, if two movies with same otitle but different recordnumber exist
        '        CurrentAttribute2 = _SourceField

        '        ' This also doesn't work, as there is not yet a source for new movie available in XML Element - might be called later?
        '        'If (CurrentNode2.Attributes(_SourceField).Value = currentNode.Attributes(_SourceField).Value) Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If

        '        ' Guzzi: Removed the following stuff, as it replaced existing movies with same title, but different content (e.g. director's cut or extended versions)
        '        'If _XMLElement.Attributes(CurrentAttribute2) Is Nothing Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If
        '        'If _XMLElement.Attributes(CurrentAttribute2).Value.ToString = String.Empty Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If

        '    End If
        'End If
        Return currentNode
    End Function

    Private Sub DoInternetLookup(ByVal SearchString As String, Optional ByVal Year As String = "", Optional ByVal IMDB_Id As String = "") ' Guzzi: Added year and imdb id as optional (search) parameters
        'This is now reset on ProcessFile, since all processing will begin with that Sub.
        '_LastOutputMessage = ""
        If CurrentSettings.Prohibit_Internet_Lookup = True Or ProhibitInternetLookup = True Then
            _internetLookupOk = False
            Exit Sub
        End If
        Try
            If IsInternetLookupNeeded() = True Then
                wurl.Clear()
                Dim wpage As Int16 = -1
                Dim Gb As Grabber_URLClass = New Grabber_URLClass
                While (True)
                    'If _InternetSearchHintIMDB_Id.Length > 0 Then
                    If _internetSearchHintImdbId.Length > 0 And (_dontAskInteractive = True Or _internetLookupAlwaysPrompt = False) Then
                        wurl = Gb.ReturnURL(_internetSearchHintImdbId, _parserPath, wpage, _internetLookupAlwaysPrompt, _filePath)
                        If (wurl.Count = 1) And _internetLookupAlwaysPrompt = False Then
                            _internetData = Gb.GetDetail(wurl.Item(0).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                            _internetLookupOk = True
                            _lastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _internetSearchHintImdbId & ")."
                            If bgwFolderScanUpdate.CancellationPending = True Then
                                Exit Sub
                            End If
                            Exit While
                        End If
                    End If

                    wurl.Clear() ' clear to make sure nothing left from tt request ...
                    wurl = Gb.ReturnURL(SearchString, _parserPath, wpage, True, _filePath) 'wurl = Gb.ReturnURL(SearchString, _parserPath, wpage, _internetLookupAlwaysPrompt, _filePath)
                    If (wurl.Count = 1) And _internetLookupAlwaysPrompt = False Then

                        '_InternetData = Gb.GetDetail(wurl.Item(0).URL, _ImagePath, _ParserPath, _downloadImage)
                        '_internetData = Gb.GetDetail(wurl.Item(0).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                        _internetData = Gb.GetDetail(wurl.Item(0).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles, _filePath)
                        _internetLookupOk = True
                        _lastOutputMessage = SearchString & " - " & " Movie found by 'single result' automatch."

                        If bgwFolderScanUpdate.CancellationPending = True Then
                            Exit Sub
                        End If
                        Exit While
                    Else
                        Dim wThumb As String
                        Dim wtitle As String
                        Dim wdirector As String
                        Dim wmovieurl As String
                        Dim wyear As String
                        Dim wOptions As String
                        Dim wAkas As String
                        Dim wID As String
                        Dim dyear As Double
                        Dim wimdb As String
                        Dim wtmdb As String
                        Dim wlimityear As Boolean = False
                        Dim distance As String

                        Dim CountTitleMatch As Integer = 0
                        Dim TitleMatch As String = ""
                        Dim matchingDistance As Integer = Integer.MaxValue
                        Dim matchingDistanceWithOptions As Integer = Integer.MaxValue
                        Dim index As Integer = -1
                        Dim indexWithOptions As Integer = -1
                        Dim searchyearHint As Double = 0

                        If Double.TryParse(_internetSearchHintYear, searchyearHint) = False Then
                            searchyearHint = 0
                        End If


                        'If _InteractiveMode = True Then
                        If (_interactiveMode = True And _dontAskInteractive = False) Then

                            'First try to find matches due to "try to find best match automatically" - this is enhanced matching option as feature on top of grabber matching itself

                            If _internetLookupAlwaysPrompt = False Then
                                'Try to match by IMDB_Id
                                If _internetSearchHintImdbId.Length > 0 Then
                                    For i = 0 To wurl.Count - 1
                                        wtitle = wurl.Item(i).Title.ToString
                                        wimdb = wurl.Item(i).IMDB_ID.ToString
                                        wtmdb = wurl.Item(i).TMDB_ID.ToString
                                        If (wimdb = _internetSearchHintImdbId And wimdb.Length > 3) Then
                                            _internetData = Gb.GetDetail(wurl.Item(i).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                            _internetLookupOk = True
                                            _lastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _internetSearchHintImdbId & ")."
                                            If bgwFolderScanUpdate.CancellationPending = True Then
                                                Exit Sub
                                            End If
                                            Exit While
                                        End If
                                    Next
                                End If

                                'Try to match by year hint (and name)
                                If searchyearHint > 0 Then
                                    indexWithOptions = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistanceWithOptions, CountTitleMatch, TitleMatch)
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 3 And matchingDistance <= matchingDistanceWithOptions Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 3 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                Else
                                    index = FuzzyMatch(SearchString, wurl, True, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 2 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                    index = FuzzyMatch(SearchString, wurl, False, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 2 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                End If
                            End If

                            'If no automatches found, show dialog box to choose from
                            frmList.txtSearchString.Text = SearchString
                            frmList.txtSearchintYear.Text = _internetSearchHintYear
                            frmList.txtSearchhintIMDB_Id.Text = _internetSearchHintImdbId
                            If _internetSearchHintImdbId <> "" Then
                                frmList.btnSearchAgainWithIMDB_Id.Enabled = True
                            Else
                                frmList.btnSearchAgainWithIMDB_Id.Enabled = False
                            End If
                            frmList.chkDontAskAgain.Checked = False
                            frmList.txtTmpParserFilePath.Text = _parserPath
                            frmList.txtTmpParserFilePathShort.Text = _parserPath.Substring(_parserPath.LastIndexOf("\") + 1)
                            frmList.lstOptionsExt.Rows.Clear()

                            If _fileName.ToString <> "" Then
                                frmList.Text = _fileName
                                frmList.txtSource.Text = _fileName
                            Else
                                frmList.Text = _filePath
                                frmList.txtSource.Text = _filePath
                            End If
                            frmList.txtSourceFullAllPath.Text = _allFilesPath
                            frmList.txtSourceFull.Text = _filePath
                            If (wurl.Count = 0) Then
                                frmList.lstOptionsExt.Rows.Add(New String() {Nothing, "Movie not found...", "", "", "", "", "", ""})
                                frmList.btnSearchAllPages.Enabled = False
                            Else
                                ' index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                frmList.btnSearchAllPages.Enabled = True
                                For i As Integer = 0 To wurl.Count - 1
                                    If wurl.Item(i).Year.ToString = _internetSearchHintYear Then
                                        wlimityear = True
                                    End If
                                Next
                                For i As Integer = 0 To wurl.Count - 1
                                    wThumb = wurl.Item(i).Thumb.ToString
                                    wtitle = wurl.Item(i).Title.ToString
                                    wdirector = wurl.Item(i).Director.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    wAkas = wurl.Item(i).Akas.ToString
                                    wID = wurl.Item(i).ID.ToString
                                    wOptions = wurl.Item(i).Options.ToString
                                    wmovieurl = wurl.Item(i).URL.ToString
                                    If (_internetSearchHint.Length > 0 And (wdirector.Contains(_internetSearchHint) Or wyear.Contains(_internetSearchHint)) And _internetLookupAlwaysPrompt = False And (wlimityear = False Or wyear = _internetSearchHintYear)) Then
                                        _internetData = Gb.GetDetail(wurl.Item(i).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                    If FuzziDistance(SearchString, wtitle) = Integer.MaxValue Then
                                        distance = ""
                                    Else
                                        distance = FuzziDistance(SearchString, wtitle).ToString
                                    End If

                                    'Dim img As DataGridViewImageColumn = New DataGridViewImageColumn()
                                    Dim image As System.Drawing.Image = GrabUtil.GetImageFromUrl(wThumb)
                                    'img.Image = image
                                    ' Image smallImage = image.GetThumbnailImage(20, 30, null, IntPtr.Zero);
                                    ' dataGridViewSearchResults.Rows[i].Cells[1].Style.Tag = "BLANK";
                                    ' dataGridViewSearchResults.Rows[i].Cells[1].Style.NullValue = null;

                                    If wyear = _internetSearchHintYear Then
                                        frmList.lstOptionsExt.Rows.Add(New Object() {image, wtitle & " - (+++ year match! +++)", wyear, wOptions, wAkas, wID, wmovieurl, distance})
                                    Else
                                        frmList.lstOptionsExt.Rows.Add(New Object() {image, wtitle, wyear, wOptions, wAkas, wID, wmovieurl, distance})
                                    End If
                                Next
                            End If
                            If frmList.lstOptionsExt.Rows.Count > 0 Then
                                frmList.lstOptionsExt.SelectionMode = Windows.Forms.DataGridViewSelectionMode.FullRowSelect
                                frmList.lstOptionsExt.Rows(0).Selected = True 'set first line as selected
                                frmList.btnOK.Enabled = True
                            Else
                                frmList.btnOK.Enabled = False
                            End If

                            Dim returnValue As System.Windows.Forms.DialogResult

                            'frmList.Parent = Form1
                            returnValue = frmList.ShowDialog()
                            SearchString = frmList.txtSearchString.Text
                            Dim blah4 As Boolean = frmList.chkDontAskAgain.Checked
                            'Dim wentry As Integer = frmList.lstOptions.SelectedIndex
                            Dim wentry As Integer = frmList.lstOptionsExt.Rows.GetFirstRow(DataGridViewElementStates.Selected)

                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "Movie not found...") Then
                                'If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptions.SelectedItem.ToString = "Movie not found...") Then
                                _internetLookupOk = False
                                _lastOutputMessage = "Failed to load Internet Data for " & FilePath
                                Exit While
                            End If
                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "+++") Then
                                wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                            Else
                                If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "---") Then
                                    wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                                Else
                                    If (returnValue = Windows.Forms.DialogResult.Abort) Then
                                        _internetLookupOk = False
                                        _lastOutputMessage = "UserAbort ! - Scan cancelled from Userdialog !"
                                        Exit While
                                    End If
                                    If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And (frmList.lstOptionsExt("Title", wentry).Value.ToString().Length > 0)) Then
                                        '_InternetData = Gb.GetDetail(wurl.Item(wentry).url, _ImagePath, frmList.txtTmpParserFilePath.Text, _downloadImage)
                                        _internetData = Gb.GetDetail(wurl.Item(wentry).url, _imagePath, frmList.txtTmpParserFilePath.Text, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        '_InternetData = Gb.GetDetail(frmList.lstOptionsExt("Weblink", wentry).Value.ToString(), _ImagePath, frmList.txtTmpParserFilePath.Text, _downloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by user's manual selection"
                                        Exit While
                                    Else 'If returnValue = Windows.Forms.DialogResult.Cancel Then
                                        'The ElseIf didn't always fire if you close the window or something.
                                        'User cancelled - check for 'Ignore File' flag.
                                        If frmList.chkDontAskAgain.Checked And Not CurrentSettings.Excluded_File_Strings.Contains(FilePath.ToLower) Then
                                            'Add to excluded file list to ignore in future scans:
                                            'My.Computer.FileSystem.WriteAllText(_excludeFile, FilePath.ToLower + vbCrLf, True) ' reenabled, as below option did not work
                                            Dim objWriter As New StreamWriter(_excludeFile, True)
                                            objWriter.WriteLine(FilePath.ToLower)
                                            objWriter.Close()
                                            'CurrentSettings.Excluded_File_Strings += ("|" + FilePath.ToLower) ' this might not be needed - seems not to work?
                                        End If
                                        _internetLookupOk = False
                                        'LogEvent("Failed to load Internet Data for " & FilePath, EventLogLevel.ImportantEvent)
                                        _lastOutputMessage = "Failed to load Internet Data for " & FilePath
                                        Exit While
                                    End If
                                End If
                            End If
                        Else
                            ' In batch mode (or GUI mode with 'don't ask interactive'), try to identify the right movie with  optseachstring
                            'LogEvent(SearchString & " - " & wurl.Count.ToString & " Movies found", EventLogLevel.Informational)
                            If _internetSearchHint.Length > 0 Then ' this is old searchhint method !
                                For i As Integer = 0 To wurl.Count - 1
                                    wtitle = wurl.Item(i).Title.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    wdirector = wurl.Item(i).Director.ToString
                                    If wyear.Length > 4 Then
                                        wyear = wyear.Substring(0, 4)
                                    End If
                                    If Double.TryParse(wyear, dyear) = False Or wyear.Length < 4 Then
                                        wyear = ""
                                    End If
                                    'If (_InternetSearchHint.Length > 0 And wtitle.Contains(_InternetSearchHint)) Then
                                    If ((wdirector.Contains(_internetSearchHint) Or wyear.Contains(_internetSearchHint))) Then
                                        'Dim datas As String()
                                        _internetData = Gb.GetDetail(wurl.Item(i).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        'CreateXmlnetInfos(datas)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by AMCU internetsearchhint (" & _internetSearchHint & ")."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                Next
                            End If

                            If (wurl.Count = 0) Then
                                _lastOutputMessage = SearchString & " - " & wurl.Count.ToString & " Movies found from Internet lookup."
                                _internetLookupOk = False
                                Exit While
                            Else 'If _InternetSearchHint.Length > 0 Or _InternetSearchHintYear.Length > 0 Or _InternetSearchHintIMDB_Id.Length > 0 Then

                                ' Check for direct IMDB match
                                If _internetSearchHintImdbId.Length > 0 Then
                                    For i As Integer = 0 To wurl.Count - 1
                                        wtitle = wurl.Item(i).Title.ToString
                                        wimdb = wurl.Item(i).IMDB_ID.ToString
                                        wtmdb = wurl.Item(i).TMDB_ID.ToString
                                        If (wimdb = _internetSearchHintImdbId) Then
                                            _internetData = Gb.GetDetail(wurl.Item(i).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                            _internetLookupOk = True
                                            _lastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _internetSearchHintImdbId & ")."
                                            If bgwFolderScanUpdate.CancellationPending = True Then
                                                Exit Sub
                                            End If
                                            Exit While
                                        End If
                                    Next
                                End If

                                ' searches with yearhint
                                If searchyearHint > 0 Then

                                    ' Check for exact year match (and name match) - without Options
                                    indexWithOptions = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistanceWithOptions, CountTitleMatch, TitleMatch) ' get also value with Options for proper match decision
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple exact matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter on) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 5 And matchingDistance <= matchingDistanceWithOptions Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for exact year match (and name match) - with Options (including TV, series, etc.)
                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple exact matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter off) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 5 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for "near" year match (and name match) - without Options 
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 1, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter on, close year match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 3 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint and close match (+/- 1) (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for "near" year match (and name match) - with Options (including TV, series, etc.)
                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 1, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter off, close year match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 3 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by year hint and close match (+/- 1) (" & _internetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If


                                Else ' searches without year hint

                                    ' Check for name matches without year hint and options on
                                    index = FuzzyMatch(SearchString, wurl, True, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter on, only title match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 4 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for name matches without year hint and options off
                                    index = FuzzyMatch(SearchString, wurl, False, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _internetLookupOk = False
                                        _lastOutputMessage = SearchString & " - Multiple matches (optionsfilter off, only title match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 4 Then
                                        _internetData = Gb.GetDetail(wurl.Item(index).URL, _imagePath, _parserPath, _downloadImage, _grabberOverrideLanguage, _grabberOverridePersonLimit, _grabberOverrideTitleLimit, _grabberOverrideGetRoles)
                                        _internetLookupOk = True
                                        _lastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                End If

                                ' Check  - only if "Try to find best match automatically" is chosen
                                If _internetLookupAlwaysPrompt = False Then
                                    If searchyearHint > 0 Then

                                    Else

                                    End If
                                End If

                                '' Check  - only if ""Try to find best match automatically" is chosen
                                'CountTitleMatch = 0
                                'index = 0
                                'indexFirstMatch = 0
                                'titleFirstMatch = ""
                                'titleLastMatch = ""

                                'For i As Integer = 0 To wurl.Count - 1
                                '    wtitle = wurl.Item(i).Title.ToString
                                '    wyear = wurl.Item(i).Year.ToString.Substring(0, 4)
                                '    If Double.TryParse(wyear, dyear) = False Then
                                '        wyear = ""
                                '    End If
                                '    If (wyear = _InternetSearchHintYear And _InternetLookupAlwaysPrompt = False) Then
                                '        If CountTitleMatch = 0 Then
                                '            indexFirstMatch = i
                                '            titleFirstMatch = wtitle
                                '        End If
                                '        CountTitleMatch = CountTitleMatch + 1
                                '        index = i
                                '        titleLastMatch = wtitle
                                '    End If
                                'Next
                                'If (CountTitleMatch = 1 And FuzziDistance(SearchString, wurl.Item(index).Title.ToString) < 5) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _downloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") with single match and name match (" & titleFirstMatch & ") with FuzziDistance = '" & FuzziDistance(SearchString, titleFirstMatch).ToString & "'."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If
                                '' more results found
                                'If (CountTitleMatch = 2 And FuzziDistance(SearchString, wurl.Item(index).Title.ToString) < 5) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(indexFirstMatch).URL, _ImagePath, _ParserPath, _downloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") with DOUBLE match and FuzziDistance = '" & FuzziDistance(SearchString, titleFirstMatch).ToString & "' - using first match (" & titleFirstMatch & ")."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If

                                '' Check for name matches (as we didn't have success with year&name matches)
                                'CountTitleMatch = 0
                                'index = 0
                                'TitleMatch = ""
                                'For i As Integer = 0 To wurl.Count - 1
                                '    wtitle = wurl.Item(i).Title.ToString
                                '    wyear = wurl.Item(i).Year.ToString.Substring(0, 4)
                                '    If Double.TryParse(wyear, dyear) = False Then
                                '        wyear = ""
                                '    End If
                                '    If ((wtitle.Contains(SearchString) Or FuzziDistance(SearchString, wurl.Item(i).Title.ToString) < 5) And _InternetLookupAlwaysPrompt = False) Then
                                '        CountTitleMatch = CountTitleMatch + 1
                                '        index = i
                                '        TitleMatch = wtitle
                                '    End If
                                'Next
                                'If (CountTitleMatch = 1) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _downloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with single match."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If

                            End If

                            Dim md As Integer = Integer.MaxValue
                            Dim count As Integer = 0
                            Dim title As String = ""
                            Dim idx = FuzzyMatch(SearchString, wurl, False, 0, 0, md, count, title)
                            _lastOutputMessage = SearchString & " - " & wurl.Count.ToString & " Movies found from Internet lookup - no matching possible. Closest Match Distance: '" & md.ToString & "' with '" & count.ToString & "' matches and match name '" & title & "'."
                            _internetLookupOk = False
                            Exit While
                            'dtmulti.Rows.Add(FilePath)
                        End If
                    End If
                End While
            End If
        Catch ex As Exception
            _lastOutputMessage = "ErrorEvent : ErrorEvent on Internet Lookup for " & _fileName.ToString & " : " & ex.Message.ToString & " - Stacktrace: " & ex.StackTrace.ToString
        End Try
    End Sub

    Function FuzzyMatch(ByVal searchtitle As String, ByVal wurl As ArrayList, ByVal restrictoptions As Boolean, ByVal searchyear As Double, ByVal yearDistance As Integer, ByRef matchingDistance As Integer, ByRef matchingCount As Integer, ByRef matchingTitle As String) As Integer
        Dim matchingIndex As Integer = -1
        matchingDistance = Integer.MaxValue
        matchingCount = 0
        matchingTitle = ""

        Dim isAmbiguous As Boolean = False

        Dim wyear As String
        Dim wOptions As String
        Dim dyear As Double

        'If _InternetSearchHintYear <> "" And _InternetSearchHintYear.Length >= 4 And wyear <> "" And wyear.Length >= 4 And wOptions = "" Then
        'If ((wtitle.Contains(SearchString) Or FuzziDistance(SearchString, wtitle) < 3) And wyear <> "" And ((wyear - 1) <= _InternetSearchHintYear) And ((wyear + 1) >= _InternetSearchHintYear)) Then


        For index As Integer = 0 To wurl.Count - 1
            wOptions = wurl.Item(index).Options.ToString
            ' calculate year
            If wurl.Item(index).Year.ToString.Length > 3 Then
                wyear = wurl.Item(index).Year.ToString.Substring(0, 4)
                If Double.TryParse(wyear, dyear) = False Then
                    wyear = ""
                    dyear = 0
                End If
            Else
                wyear = ""
                dyear = 0
            End If

            If restrictoptions And wOptions <> "" Then Continue For ' skip Option movies
            If searchyear > 0 And ((dyear > searchyear + yearDistance) Or (dyear < searchyear - yearDistance)) Then Continue For ' skip non year match movies
            'searchtitle = GetSearchString(searchtitle) ' might be unneccessary, as already done by AMCU
            'Dim distance As Integer = Levenshtein.Match(searchtitle, wurl.Item(index).Title.ToString.ToLower())

            ' check main title
            Dim distance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, wurl.Item(index).Title.ToString)
            If (distance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                isAmbiguous = True
                matchingCount = matchingCount + 1
            End If

            If (distance < matchingDistance) Then
                isAmbiguous = False
                matchingDistance = distance
                matchingIndex = index
                matchingCount = 1
                matchingTitle = wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")"
            End If

            ' get and check AKAs if present
            If wurl.Item(index).Akas.ToString <> "" Then
                Dim AKAstring As String = wurl.Item(index).Akas.ToString
                'Dim MatchList As MatchCollection
                'Dim matcher As Match
                'Dim p As New Regex("aka." & Chr(34) & ".*?" & Chr(34) & ".-")
                'MatchList = p.Matches(AKAstring)
                'If MatchList.Count > 0 Then
                '    For Each matcher In MatchList
                '        Dim AKAtitle As String = matcher.Value
                '        Dim AKAdistance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString)
                '        'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then
                '        '    isAmbiguous = True
                '        '    matchingCount = matchingCount + 1
                '        'End If

                '        If (AKAdistance < matchingDistance) Then
                '            isAmbiguous = False
                '            matchingDistance = AKAdistance
                '            matchingIndex = index
                '            matchingCount = 1
                '            matchingTitle = AKAtitle & " (AKA) - " & wurl.Item(index).Title.ToString
                '        End If
                '    Next
                'End If
                Dim StringList As String() = AKAstring.Split(New Char() {"|"}, System.StringSplitOptions.RemoveEmptyEntries)
                If StringList.Length > 0 And matchingDistance > 0 Then ' Only, if no perfect match yet done
                    For Each AKAtitle As String In StringList
                        Dim AKAdistance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString.Trim)
                        'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                        '    isAmbiguous = True
                        '    matchingCount = matchingCount + 1
                        'End If
                        If (AKAdistance < matchingDistance) Then
                            isAmbiguous = False
                            matchingDistance = AKAdistance
                            matchingIndex = index
                            matchingCount = 1
                            matchingTitle = "'" & AKAtitle & "' (AKA) - main title '" & wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")" & "'"
                        End If
                        If (AKAtitle.Contains(":")) Then
                            Dim iTrimIndex As Integer = 0
                            iTrimIndex = AKAtitle.IndexOf(":")
                            If iTrimIndex > 0 Then
                                AKAtitle = AKAtitle.Substring(0, iTrimIndex)
                                AKAdistance = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString.Trim)
                                If (AKAdistance < matchingDistance) Then
                                    isAmbiguous = False
                                    matchingDistance = AKAdistance
                                    matchingIndex = index
                                    matchingCount = 1
                                    matchingTitle = "'" & AKAtitle & "' (AKA-Trim) - main title '" & wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")" & "'"
                                End If
                            End If
                        End If
                    Next
                    'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                    '    isAmbiguous = True
                    '    matchingCount = matchingCount + 1
                    'End If
                End If
            End If
        Next
        If isAmbiguous = True Then
            Return -2           ' this indicates multiple matches
        End If
        Return matchingIndex    ' returns "-1" if no matches
    End Function

    Function GetSearchString(ByVal strMovie As String) As String
        Dim strUrl As String = strMovie
        strUrl = strUrl.Trim()
        Dim rx As New Regex("(([\(\{\[]|\b)((576|720|1080)[pi]|dir(ectors )?cut|dvd([r59]|rip|scr(eener)?)|(avc)?hd|wmv|ntsc|pal|mpeg|dsr|r[1-5]|bd[59]|dts|ac3|blu(-)?ray|[hp]dtv|stv|hddvd|xvid|divx|x264|dxva|(?-i)FEST[Ii]VAL|L[iI]M[iI]TED|[WF]S|PROPER|REPACK|RER[Ii]P|REAL|RETA[Ii]L|EXTENDED|REMASTERED|UNRATED|CHRONO|THEATR[Ii]CAL|DC|SE|UNCUT|[Ii]NTERNAL|V\d{1}|BR[Rr]ip|[DS]UBBED)([\]\)\}]|\b)(-[^\s]+$)?)", RegexOptions.IgnoreCase)
        strUrl = rx.Replace(strUrl, "").Replace(".", " ").Replace("_", " ").Trim()
        Return strUrl
    End Function

    Function CloseEnough(ByVal searchtitle As String, ByVal movietitle As String) As Boolean
        Const AutoApprovalTreshold As Integer = 2
        Dim distance As Integer
        distance = AdvancedStringComparer.Levenshtein(searchtitle, movietitle)
        If (distance <= AutoApprovalTreshold = True) Then
            Return True
        End If
        'For Each currAltTitle As String in movie.AlternateTitles) DO
        '    distance = Grabber.GrabUtil.AdvancedStringComparer.Levenshtein(title, currAltTitle)

        '    If (distance <= MovingPicturesCore.Settings.AutoApproveThreshold) Then
        '        Return True
        '    End If
        'Next
        Return False
    End Function

    Function FuzziDistance(ByVal searchtitle As String, ByVal movietitle As String) As Integer
        Return AdvancedStringComparer.Levenshtein(searchtitle, movietitle)
    End Function

    Public Sub ProcessFile(ByVal ProcessMode As ProcessModeNames)

        _lastOutputMessage = ""
        'LogEvent("ProcessFile() - Start !", EventLogLevel.InformationalWithGrabbing)
        Try
            'Dim attr As Xml.XmlAttribute
            'Dim element As Xml.XmlElement
            Dim CurrentAttribute As String
            Dim TempValue As String

            Dim title As String = ""
            Dim ttitle As String = ""
            Dim ftitle As String = ""
            Dim director As String = ""
            Dim year As Int16 = 0
            Dim imdb_id As String = ""

            'First ensure we have a valid movie number so the record can be saved:
            CurrentAttribute = "Number"
            TempValue = _movieNumber
            If _xmlElement.Attributes(CurrentAttribute) Is Nothing Then ' only update when there is no content
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            ElseIf ProcessMode = ProcessModeNames.Update Then
                _movieNumber = Integer.Parse(_xmlElement.Attributes(CurrentAttribute).Value)
            End If
            'LogEvent("ProcessFile() - get valid record number: '" & TempValue & "'", EventLogLevel.InformationalWithGrabbing)

            If (ProcessMode = ProcessModeNames.Import) Then
                'Second get a decent Movie Title which we can then use for Internet Lookups as well as the Original Title field.
                'LogEvent("ProcessFile() - Import: Get search & matching hints...", EventLogLevel.InformationalWithGrabbing)

                CurrentAttribute = "OriginalTitle" 'add a test for manual update when no file specified => Internet lookup with OriginalTitle     
                If (_filePath.Length > 0) Then
                    TempValue = GetTitleFromFilePath(_filePath)
                    title = TempValue
                    'LogEvent("ProcessFile() - Import - hints - title: '" & title & "'", EventLogLevel.InformationalWithGrabbing)
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "Year" 'try to get year from filepath/name 
                If (_filePath.Length > 0) Then
                    TempValue = GetYearFromFilePath(_filePath)
                    _internetSearchHintYear = TempValue
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "IMDB_Id" 'try to get IMDB Id from filepath/name
                If (_filePath.Length > 0) Then
                    TempValue = GetIMDBidFromFilePath(_filePath)
                    _internetSearchHintImdbId = TempValue
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                    End If
                    TempValue = ""
                End If
            End If

            If IsInternetLookupNeeded() = True Then
                ' set the search hints for update mode (AMC, IMDB, Year)
                If ProcessMode = ProcessModeNames.Update Then
                    Dim wdirector As String
                    If _xmlElement.Attributes("Director") IsNot Nothing Then
                        If _xmlElement.Attributes("Director").Value.Length > 0 Then
                            wdirector = System.Text.RegularExpressions.Regex.Replace(_xmlElement.Attributes("Director").Value, "\b(and|und|en|et|y|&)\b", ",")
                            If wdirector.IndexOf(",") > 0 Then
                                _internetSearchHint = wdirector.Substring(0, wdirector.IndexOf(",") - 1).Trim()
                            Else
                                _internetSearchHint = wdirector.Trim()
                            End If
                        End If
                    End If

                    If _xmlElement.Attributes("Year") IsNot Nothing Then
                        _internetSearchHintYear = _xmlElement.Attributes("Year").Value.ToString
                    End If

                    If _xmlElement.Item("IMDB_Id") IsNot Nothing Then
                        If _xmlElement.Item("IMDB_Id").InnerText.Length > 0 Then
                            _internetSearchHintImdbId = _xmlElement.Item("IMDB_Id").InnerText.ToString
                        ElseIf _xmlElement.Attributes("URL") IsNot Nothing Then
                            If _xmlElement.Attributes("URL").Value.Length > 0 Then
                                Dim URL As String = _xmlElement.Attributes("URL").Value.ToString
                                Dim IMDB As String = GetIMDBidFromFilePath(URL)
                                If IMDB.Length > 0 Then
                                    _internetSearchHintImdbId = IMDB
                                ElseIf (_filePath.Length > 0) Then 'try to get IMDB Id from filepath/name
                                    IMDB = GetIMDBidFromFilePath(_filePath)
                                    If IMDB.Length > 0 Then
                                        _internetSearchHintImdbId = IMDB
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                ' Get Internetdata with "best title possible"
                If IsValidTitle(_xmlElement, "FormattedTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_xmlElement.Attributes("FormattedTitle").Value.ToString, GetEdition(_filePath)))
                ElseIf IsValidTitle(_xmlElement, "TranslatedTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_xmlElement.Attributes("TranslatedTitle").Value.ToString, GetEdition(_filePath)))
                ElseIf IsValidTitle(_xmlElement, "OriginalTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_xmlElement.Attributes("OriginalTitle").Value.ToString, GetEdition(_filePath)))
                Else
                    DoInternetLookup(GetTitleFromFilePath(_filePath)) 'No DB title available, so use the cleaned filename instead:
                End If
            End If

            ' Check, if internetlookup has given proper title name - otherwise set to failed
            If _internetLookupOk = True Then
                TempValue = _internetData(GrabberOutput.OriginalTitle)
                If TempValue Is Nothing Or TempValue = "" Then
                    TempValue = _internetData(GrabberOutput.TranslatedTitle)
                    If TempValue Is Nothing Or TempValue = "" Then
                        _internetLookupOk = False
                        _lastOutputMessage = "ErrorEvent : ErrorEvent importing " & _fileName.ToString & " : Matching the movie was successful, but grabber failed getting movie details data (title)"
                    End If
                End If
            End If

            ' Now update all requested fields ...

            CurrentAttribute = "OriginalTitle" 'Now update the Original Title with the Internet value, if set to do so:
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetTitleFromFilePath(_filePath)
                If _internetLookupOk = True Then
                    If CurrentSettings.Movie_Title_Handling.Contains("Internet Lookup") = True Then
                        TempValue = _internetData(GrabberOutput.OriginalTitle)
                        title = TempValue
                    End If
                End If
                TempValue = AddGroupName(TempValue, "Original Title") 'Check to see if there's a group name attached to this, and apply it.
                TempValue = AddEdition(TempValue, "Original Title") 'Add Edition, if available and requested
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "TranslatedTitle"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _internetLookupOk = True Then
                    TempValue = _internetData(GrabberOutput.TranslatedTitle)
                Else
                    TempValue = GetTitleFromFilePath(_filePath)
                End If
                TempValue = AddGroupName(TempValue, "Translated Title")
                TempValue = AddEdition(TempValue, "Translated Title")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If


            ' Guzzi: Original Code does remove old entries with same otitle name
            'Try to see if entry already exist with empty movie filename => delete the new entry and update existing one
            If _databaseFields("originaltitle") = True Or _databaseFields("translatedtitle") = True Then
                If _databaseFields("originaltitle") = True Then
                    _xmlElement = VerifyElement(_xmlElement.Attributes("OriginalTitle").Value.ToString, _xmlElement)
                End If
            ElseIf ProcessMode = ProcessModeNames.Import Then
                _lastOutputMessage = "ErrorEvent : ErrorEvent importing " & _fileName.ToString & " : No originaltitle or translatedtitle activated"
            End If

            CurrentAttribute = "FormattedTitle"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _xmlElement.Attributes("TranslatedTitle") IsNot Nothing Then
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(RemoveGroupNameAndEdition(_xmlElement.Attributes("TranslatedTitle").Value.ToString, GetEdition(_filePath)))
                    TempValue = AddGroupName(TempValue, "Translated Title")
                    TempValue = AddEdition(TempValue, "Translated Title")
                ElseIf _xmlElement.Attributes("OriginalTitle") IsNot Nothing Then
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(_xmlElement.Attributes("OriginalTitle").Value)
                    TempValue = AddGroupName(TempValue, "Original Title")
                    TempValue = AddEdition(TempValue, "Original Title")
                End If
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Date"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "Date")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Checked"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = CurrentSettings.Check_Field_Handling.ToString
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "MediaLabel"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _readDvdLabel = True Then
                    Dim DriveLabel As String = String.Empty
                    DriveLabel = DrvLabel(_filePath)

                    If DriveLabel = String.Empty Then
                        TempValue = _mediaLabel
                    Else
                        TempValue = DriveLabel
                    End If
                Else
                    TempValue = _mediaLabel
                End If
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "MediaType"
            If Not String.IsNullOrEmpty(_mediaType) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = _mediaType
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = _sourceField ' Sourcefile - field depends on configuration - always update, if mode is "import"
            If Not (CurrentAttribute = "(none)" Or String.IsNullOrEmpty(CurrentAttribute)) And (IsUpdateRequested(CurrentAttribute, ProcessMode) Or ProcessMode = ProcessModeNames.Import) Then
                If (_filePath.Length > 0) Then
                    If Not String.IsNullOrEmpty(_overridePath) Then
                        TempValue = _overridePath
                    Else
                        TempValue = _filePath
                    End If
                    If CurrentSettings.Store_Short_Names_Only Then
                        If TempValue.Contains("\") Then
                            TempValue = TempValue.Substring(TempValue.LastIndexOf("\") + 1)
                        End If
                    End If
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Subtitles"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "textstreamlanguagelist")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Languages"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If Not _useInternetDataForLanguages = True Then
                    TempValue = GetFileData(_filePath, "audiostreamlanguagelist")
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Resolution"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "Resolution")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Length"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                'TempValue = fnGetFileData(_FilePath, "Runtime")
                If _allFilesPath <> "" Then
                    For Each wfile As String In _allFilesPath.Split(";")
                        If GetFileData(wfile, "runtime") <> "" And Not GetFileData(wfile, "runtime").Contains("ErrorEvent") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "runtime")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "runtime")
                            End If
                            'Diskcount += 1
                        End If
                    Next
                Else
                    TempValue = GetFileData(_filePath, "runtime")
                End If

                If _internetLookupOk = True Then
                    If TempValue.Contains("ErrorEvent") Then
                        TempValue = _internetData(GrabberOutput.Runtime)
                    End If
                End If

                If Not TempValue.Contains("ErrorEvent") Then
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "VideoFormat"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "VideoFormat")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "VideoBitrate"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "VideoBitrate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioFormat"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "AudioFormat")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioBitrate"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "AudioBitrate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioChannelCount"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "AudioChannelCount")
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "S3D"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "3d")
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Aspectratio"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If (_filePath.Length > 0) Then
                    TempValue = GetFileData(_filePath, "Aspectratio")
                Else
                    If _xmlElement.Attributes("Resolution") IsNot Nothing AndAlso _xmlElement.Attributes("Resolution").Value.ToString.Length > 0 Then
                        Dim aspectratio As Decimal
                        Dim Resolution As String = _xmlElement.Attributes("Resolution").Value.ToString
                        Try
                            If Not Decimal.TryParse(Resolution, aspectratio) Then
                                Dim arSplit() As String = Resolution.Split(New String() {"x"}, StringSplitOptions.RemoveEmptyEntries)
                                aspectratio = Math.Round(Decimal.Divide(Convert.ToInt32(arSplit(0)), Convert.ToInt32(arSplit(1))), 2)
                            End If
                            TempValue = aspectratio.ToString(CultureInfo.InvariantCulture)
                        Catch ex As Exception
                            TempValue = ""
                        End Try
                    Else
                        TempValue = ""
                    End If
                End If
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Framerate"
            If (_filePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_filePath, "Framerate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Size"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _allFilesPath <> "" Then
                    For Each wfile As String In _allFilesPath.Split(";")
                        If GetFileData(wfile, "FileSize") <> "" And Not GetFileData(wfile, "FileSize").Contains("ErrorEvent") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "FileSize")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "FileSize")
                            End If
                        End If
                    Next
                Else
                    TempValue = GetFileData(_filePath, "FileSize")
                End If

                If Not TempValue.Contains("ErrorEvent") Then
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Disks"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                Dim Diskcount As Integer = 0
                If _allFilesPath <> "" Then
                    Diskcount += _allFilesPath.Split(";").Count()
                Else
                    Diskcount = 1
                End If
                TempValue = Diskcount.ToString
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Edition" ' Get "Edition" from filename for separate field
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetEdition(_filePath)
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If


            If _internetLookupOk = False Then

                'Additional attempt to load picture with folder.jpg settings, in case Internet lookup fails
                CurrentAttribute = "Picture"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    Dim CoverFileExists As Boolean = False
                    Dim Filename As String = _filePath ' media file
                    Dim PicturePathToSave As String = String.Empty ' the strintg to save in DB field Pictures = "..."

                    If CurrentSettings.Create_Cover_From_Movie = True Then ' create missing covers by thumbnailer
                        'Not using folder.jpg - use default location instead (with the xml file, maybe using override path) _ImagePath
                        CoverFileExists = CreateCoverFromMovie(Filename, PicturePathToSave)
                    ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        Dim NewCoverThumbName As String = _filePath
                        If NewCoverThumbName.Contains("\") = True Then
                            NewCoverThumbName = NewCoverThumbName.Substring(0, NewCoverThumbName.LastIndexOf("\"))
                        End If
                        NewCoverThumbName += "\folder.jpg"
                        Try
                            If File.Exists(NewCoverThumbName) Then
                                CoverFileExists = True
                                PicturePathToSave = NewCoverThumbName
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    ' now update with either folder.jpg or filename.jpg
                    If CoverFileExists = True And PicturePathToSave <> "" Then
                        TempValue = PicturePathToSave
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If


            Else 'Load all the Internet data...

                title = _internetData(GrabberOutput.OriginalTitle)

                ' Guzzi: Update Languages, if it shouold get internet data
                CurrentAttribute = "Languages"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    If CurrentSettings.Use_InternetData_For_Languages = True Then
                        TempValue = _internetData(GrabberOutput.Language)
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "Year"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Year)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Country"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Country)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Category"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Category)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "URL"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.URL)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Rating"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Rating)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Director"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Director)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Producer"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Producer)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Actors"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Actors)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Description"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Description)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Comments"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Comments)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Picture"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    If CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        'First check to see if the file exists in a nice safe way:
                        Dim FileExists As Boolean = False
                        Dim FileIsLinked As Boolean = False
                        Dim NewFileName As String = _filePath
                        If NewFileName.Contains("\") = True Then
                            NewFileName = NewFileName.Substring(0, NewFileName.LastIndexOf("\"))
                        End If
                        NewFileName += "\folder.jpg"
                        Try
                            If File.Exists(NewFileName) Then
                                FileExists = True
                            End If
                        Catch ex As Exception

                        End Try

                        'Then check if the current record already has a reference to that file:
                        If _xmlElement.Attributes(CurrentAttribute) IsNot Nothing Then
                            If _xmlElement.Attributes(CurrentAttribute).Value.ToLower = NewFileName.ToLower Then
                                FileIsLinked = True
                            End If
                        End If


                        If ProcessMode = ProcessModeNames.Import Then
                            'Import run - use existing folder.jpg if present; add it if not.
                            If FileExists = True Then
                                'It's there; use it.  Delete the one we just downloaded, if needed:
                                If _internetData(GrabberOutput.PicturePathLong) <> String.Empty And File.Exists(GrabberOutput.PicturePathLong) Then
                                    File.Delete(_internetData(GrabberOutput.PicturePathLong))
                                End If
                            Else
                                'If we have anything from the grabber, copy it and use that.
                                If _internetData(GrabberOutput.PicturePathLong) <> String.Empty And File.Exists(GrabberOutput.PicturePathLong) Then
                                    File.Copy(_internetData(GrabberOutput.PicturePathLong), NewFileName)
                                    File.Delete(_internetData(GrabberOutput.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        Else
                            'Manual Update - have to assume we'll overwrite the existing folder.jpg with the new one, if possible.
                            If FileExists = True Then
                                'check it's used; if not just use it and we're done.
                                If FileIsLinked = True Then
                                    'It's already in use, so update it:
                                    If _internetData(GrabberOutput.PicturePathShort) <> String.Empty And File.Exists(GrabberOutput.PicturePathLong) Then
                                        If FileExists = True Then
                                            System.IO.File.Delete(NewFileName)
                                        End If
                                        File.Copy(_internetData(GrabberOutput.PicturePathLong), NewFileName)
                                        File.Delete(_internetData(GrabberOutput.PicturePathLong))
                                        FileExists = True
                                    End If
                                End If
                            Else
                                'Try and get a new one:
                                If _internetData(GrabberOutput.PicturePathShort) <> String.Empty And File.Exists(GrabberOutput.PicturePathLong) Then
                                    If FileExists = True Then
                                        System.IO.File.Delete(NewFileName)
                                    End If
                                    File.Copy(_internetData(GrabberOutput.PicturePathLong), NewFileName)
                                    File.Delete(_internetData(GrabberOutput.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        End If

                        'Now check if we have a valid link; and use it.
                        If FileExists = True Then
                            TempValue = NewFileName
                            CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                        End If

                    Else
                        'Not using folder.jpg - use default location instead (with the xml file, maybe using override path)
                        If _internetData(GrabberOutput.PicturePathShort) <> String.Empty Then
                            Dim PicturePathPrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString 'Covers\'
                            Dim PicturePathSuffix As String = CurrentSettings.Image_Download_Filename_Suffix.ToString '_(%year%)' - supports placeholder, like e.g. year, number
                            Dim PicturePathFull As String = _internetData(GrabberOutput.PicturePathLong) 'C:\Ant Movie Catalog\2001_ A Space Odyssey.jpg
                            Dim PictureFileName As String = _internetData(GrabberOutput.PicturePathShort) '2001_ A Space Odyssey.jpg
                            Dim PictureFileNameWithSuffix As String = Path.GetFileNameWithoutExtension(PictureFileName) + PictureNameWithSuffix(PicturePathSuffix, _internetData(GrabberOutput.Year), _movieNumber.ToString()) + Path.GetExtension(PictureFileName)
                            Dim PicturePathToSave As String = String.Empty


                            'Check, if the returned picture is existing - it might not due to download errors like 404
                            If System.IO.File.Exists(PicturePathFull) Then
                                'Separate the folder from the prefix string (if needed)
                                Dim PrefixString As String = String.Empty
                                Dim SuffixString As String = PictureNameWithSuffix(PicturePathSuffix, _internetData(GrabberOutput.Year), _movieNumber.ToString())
                                Dim PrefixPath As String = String.Empty
                                If PicturePathPrefix <> String.Empty Then
                                    If PicturePathPrefix.Contains("\") = True Then
                                        PrefixPath = PicturePathPrefix.Substring(0, PicturePathPrefix.LastIndexOf("\") + 1)
                                        PrefixString = PicturePathPrefix.Substring(PicturePathPrefix.LastIndexOf("\") + 1)
                                    Else
                                        PrefixString = PicturePathPrefix
                                    End If
                                End If

                                Dim NewFileName As String = String.Empty
                                If PrefixString <> String.Empty Or SuffixString <> String.Empty Then
                                    'Need to rename the file.
                                    NewFileName = PicturePathFull.Replace(PictureFileName, PrefixString & PictureFileNameWithSuffix)
                                    If Not File.Exists(NewFileName) Then
                                        File.Copy(PicturePathFull, NewFileName)
                                        Thread.Sleep(20)
                                    End If
                                    File.Delete(PicturePathFull)
                                    PicturePathFull = NewFileName
                                    PictureFileName = PrefixString & PictureFileNameWithSuffix
                                End If

                                If PrefixPath <> String.Empty Then
                                    'Need to add the new folder to the short (relative) path:
                                    PictureFileName = PrefixPath & PictureFileName
                                End If

                                If CurrentSettings.Store_Image_With_Relative_Path = True Then
                                    PicturePathToSave = PictureFileName
                                Else
                                    PicturePathToSave = PicturePathFull
                                End If

                                If PicturePathToSave <> String.Empty Then
                                    TempValue = PicturePathToSave
                                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                                End If
                            End If
                        Else
                            Dim Filename As String = _filePath ' media file
                            Dim PicturePathToSave As String = String.Empty ' the strintg to save in DB field Pictures = "..."

                            If CurrentSettings.Create_Cover_From_Movie = True Then ' create missing covers by thumbnailer
                                CreateCoverFromMovie(Filename, PicturePathToSave)
                            ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True Then
                                Dim NewCoverThumbName As String = _filePath
                                If NewCoverThumbName.Contains("\") = True Then
                                    NewCoverThumbName = NewCoverThumbName.Substring(0, NewCoverThumbName.LastIndexOf("\"))
                                End If
                                NewCoverThumbName += "\folder.jpg"
                                Try
                                    If File.Exists(NewCoverThumbName) Then
                                        PicturePathToSave = NewCoverThumbName
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                            If PicturePathToSave <> String.Empty Then
                                TempValue = PicturePathToSave
                                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                            End If
                        End If
                    End If
                End If


                CurrentAttribute = "Certification"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Certification)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Writer"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Writer)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "TagLine"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Tagline)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "IMDB_Id"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.IMDB_Id)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "IMDB_Rank"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.IMDB_Rank)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "TMDB_Id"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.TMDB_Id)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Studio"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _internetData(GrabberOutput.Studio)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Edition" ' Disabled, as we use edition from local media file in noninternet section abovedone in separate section
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    'TempValue = _InternetData(GrabberOutput.Edition)
                    'TempValue = GetEdition(_FilePath, CurrentSettings.Movie_Title_Handling)
                    'CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode) 
                End If

            End If

            'get fanart
            CurrentAttribute = "Fanart"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then ' Old: If _databaseFields(CurrentAttribute.ToLower) = True Then
                Dim fanartTitle As String
                fanartTitle = GetFanartTitle(_xmlElement, title, ttitle, ftitle, year, director)
                If fanartTitle.Length > 0 And Not String.IsNullOrEmpty(CurrentSettings.Movie_Fanart_Path) Then
                    Dim DoFallback As Boolean = True

                    If CurrentSettings.Use_Grabber_For_Fanart = False Then
                        'use fanart TMDB loader
                        If _internetLookupOk = True And CurrentSettings.Prohibit_Internet_Lookup = False Then
                            Dim fanart As List(Of DbMovieInfo)
                            Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                            fanart = Gb.GetFanart(title, ttitle, year, director, _internetSearchHintImdbId, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title, CurrentSettings.Movie_PersonArtwork_Path, CurrentSettings.Movie_Fanart_Number_Limit, CurrentSettings.Movie_Fanart_Resolution_Min, CurrentSettings.Movie_Fanart_Resolution_Max)
                            ' if there is exact = one match ... get backdrops
                            If fanart.Count = 1 Then
                                If fanart(0).Backdrops.Count > 0 Then
                                    TempValue = fanart(0).Backdrops(0).ToString
                                    If String.IsNullOrEmpty(TempValue) = False Then
                                        CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                                    End If
                                    DoFallback = False
                                End If
                                ' get person images, if enabled ...
                                If CurrentSettings.Load_Person_Images_With_Fanart = True And CurrentSettings.Movie_PersonArtwork_Path.Length > 0 And Directory.Exists(CurrentSettings.Movie_PersonArtwork_Path) Then
                                    Dim filenameperson As String = String.Empty
                                    Dim listepersons As List(Of DbPersonInfo) = fanart(0).Persons
                                    For Each person As DbPersonInfo In listepersons
                                        Dim TheMoviedb As New TheMoviedb()
                                        Dim persondetails As DbPersonInfo = TheMoviedb.GetPersonsById(person.Id, String.Empty)
                                        If persondetails.Images.Count > 0 Then
                                            For Each image As String In persondetails.Images
                                                If System.IO.File.Exists(System.IO.Path.Combine(CurrentSettings.Movie_PersonArtwork_Path, persondetails.Name + ".jpg")) Then
                                                    Exit For
                                                End If
                                                Grabber.GrabUtil.DownloadPersonArtwork(CurrentSettings.Movie_PersonArtwork_Path, image, persondetails.Name, True, True, filenameperson)
                                            Next
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    Else
                        'use script based fanart retrieval
                        If Not _internetData Is Nothing AndAlso _internetData(GrabberOutput.Fanart).Length > 0 AndAlso System.IO.File.Exists(_internetData(GrabberOutput.Fanart)) Then
                            Dim NewFanartThumbName As String = _internetData(GrabberOutput.Fanart)
                            GrabUtil.CopyFanartToFanartFolder(NewFanartThumbName, CurrentSettings.Movie_Fanart_Path, fanartTitle, True)

                            DoFallback = False
                            TempValue = NewFanartThumbName
                            CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)

                            If _internetData(GrabberOutput.MultiFanart).Length > 0 Then
                                Dim Fanarts As List(Of String) = _internetData(GrabberOutput.MultiFanart).Split(",").ToList()
                                For Each validfanart As String In From fanart In Fanarts Where fanart.Trim().Length > 0 Select fanart.Trim()
                                    GrabUtil.CopyFanartToFanartFolder(validfanart, CurrentSettings.Movie_Fanart_Path, fanartTitle, False)
                                Next
                            End If
                        End If
                    End If

                    If DoFallback = True Then
                        If CurrentSettings.Create_Cover_From_Movie Then ' create missing fanart by thumbnailer
                            Dim FanartFileExists As Boolean = False
                            ' Now set to filename-fanart.jpg to get "better matching" if existing...
                            Dim NewFanartThumbName As String = Path.GetDirectoryName(_filePath) + "\" + Path.GetFileNameWithoutExtension(_filePath) + "-fanart.jpg"
                            Try
                                If Not File.Exists(NewFanartThumbName) Then
                                    FanartFileExists = Grabber.GrabUtil.GetFanartFromMovie(fanartTitle, year, CurrentSettings.Movie_Fanart_Path, True, _filePath, NewFanartThumbName, 0)
                                End If
                            Catch ex As Exception
                            End Try
                            Try
                                If File.Exists(NewFanartThumbName) Then ' recheck, if file is existing now after trying to create thumb from movie
                                    FanartFileExists = True
                                End If
                            Catch ex As Exception
                            End Try

                            If FanartFileExists = True Then
                                TempValue = NewFanartThumbName
                                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                            End If
                        ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True Then
                            ' Now set to fanart.jpg
                            If _filePath.Contains("\") = True Then
                                Dim NewFanartThumbName As String = _filePath.Substring(0, _filePath.LastIndexOf("\")) & "\fanart.jpg"
                                If File.Exists(NewFanartThumbName) Then
                                    TempValue = NewFanartThumbName
                                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                                    GrabUtil.CopyFanartToFanartFolder(NewFanartThumbName, CurrentSettings.Movie_Fanart_Path, fanartTitle, True)
                                End If
                            End If
                        End If

                    End If
                End If
            End If
        Catch ex As Exception
            _lastOutputMessage = "ErrorEvent : ErrorEvent importing " & _fileName.ToString & " : " & ex.Message.ToString & ", " & ex.StackTrace.ToString
        End Try
    End Sub

    Private Function IsUpdateRequested(ByVal currentAttribute As String, ByVal ProcessMode As ProcessModeNames) As Boolean
        Dim attr As Xml.XmlAttribute
        Dim element As Xml.XmlElement
        Dim customfieldselement As Xml.XmlElement
        Dim customfieldsattr As Xml.XmlAttribute

        If _databaseFields(currentAttribute.ToLower) = False Then ' Field not selected !
            Return False
        Else
            If OnlyAddMissingData = True And ProcessMode = ProcessModeNames.Update Then
                attr = _xmlElement.Attributes(currentAttribute)
                element = _xmlElement.Item(currentAttribute)
                customfieldselement = _xmlElement.Item("CustomFields")
                If attr Is Nothing And element Is Nothing And customfieldselement Is Nothing Then ' no values exist at all
                    Return True
                Else
                    If Not attr Is Nothing Then ' check for standard attr value
                        If attr.Value Is Nothing Then
                            Return True
                        ElseIf attr.Value = "" Then
                            Return True
                        Else
                            Return False
                        End If
                    ElseIf Not customfieldselement Is Nothing Then  ' check for new AMC4 enhanced element value (Customfields)
                        customfieldsattr = customfieldselement.Attributes(currentAttribute)
                        If Not customfieldsattr Is Nothing Then ' check for  attr value inf customfields element
                            If customfieldsattr.Value Is Nothing Then
                                Return True
                            ElseIf customfieldsattr.Value = "" Then
                                Return True
                            Else
                                Return False
                            End If
                        Else
                            Return True
                        End If
                    ElseIf Not element Is Nothing Then  ' check for old MyFilms enhanced element value
                        If element.InnerText Is Nothing Then
                            Return True
                        ElseIf element.InnerText = "" Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            Else
                Return True
            End If
        End If
    End Function

    Private Sub CreateOrUpdateAttribute(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As ProcessModeNames)
        Dim attr As Xml.XmlAttribute
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = ProcessModeNames.Update) Then
            If _xmlElement.Attributes(currentAttribute) Is Nothing Then
                attr = _xmlDoc.CreateAttribute(currentAttribute)
                attr.Value = currentValue
                _xmlElement.Attributes.Append(attr)
            Else
                _xmlElement.Attributes(currentAttribute).Value = currentValue
            End If
            If ProcessMode = ProcessModeNames.Update Then
                If Not _lastOutputMessage.Contains(" - Updated: ") Then
                    _lastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _lastOutputMessage += ", " + currentAttribute
                End If
            End If
        End If
        currentValue = ""
    End Sub

    Private Sub CreateOrUpdateElement(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As ProcessModeNames)

        ' First create new AMC4 objects - does NOT (yet) reset currentValue
        CreateOrUpdateCustomFieldsAttribute(currentAttribute, currentValue, ProcessMode)

        Dim element As Xml.XmlElement
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = ProcessModeNames.Update) Then
            If _xmlElement.Item(currentAttribute) Is Nothing Then
                element = _xmlDoc.CreateElement(currentAttribute)
                element.InnerText = currentValue
                _xmlElement.AppendChild(element)
            Else
                _xmlElement.Item(currentAttribute).InnerText = currentValue
            End If
            If ProcessMode = ProcessModeNames.Update Then
                If Not _lastOutputMessage.Contains(" - Updated: ") Then
                    _lastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _lastOutputMessage += ", " + currentAttribute
                End If
            End If
        End If
        currentValue = ""
    End Sub

    Private Sub CreateOrUpdateCustomFieldsAttribute(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As ProcessModeNames)
        ' First check, if the CustomFields element exists or create it
        Const SubElementName As String = "CustomFields"
        Dim element As Xml.XmlElement
        If _xmlElement.Item(SubElementName) Is Nothing Then
            element = _xmlDoc.CreateElement(SubElementName)
        End If

        'now check and update or create Attributes in the CustomFields Element
        Dim attr As Xml.XmlAttribute
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = ProcessModeNames.Update) Then
            If _xmlElement.Item(SubElementName) Is Nothing Then
                element = _xmlDoc.CreateElement(SubElementName)
                element.InnerText = ""
                _xmlElement.AppendChild(element)
            End If

            If _xmlElement.Item(SubElementName).Attributes(currentAttribute) Is Nothing Then
                attr = _xmlDoc.CreateAttribute(currentAttribute)
                attr.Value = currentValue
                _xmlElement.Item(SubElementName).Attributes.Append(attr)
            Else
                _xmlElement.Item(SubElementName).Attributes(currentAttribute).Value = currentValue
            End If
            If ProcessMode = ProcessModeNames.Update Then
                If Not _lastOutputMessage.Contains(" - Updated: ") Then
                    _lastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _lastOutputMessage += ", " + currentAttribute + " (CustomFields)"
                End If
            End If
        End If
        'currentValue = ""
    End Sub

    Private Sub CleanValueForInnerXML(ByRef currentValue)
        If currentValue <> "" Then
            '' currentValue = currentValue.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;")
            '' currentValue.Replace("""", "'")
            'Dim pattern As String = "#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])"
            'Dim regex As New Regex(pattern, RegexOptions.IgnoreCase)
            'If (regex.IsMatch(currentValue)) Then
            '    currentValue = regex.Replace(currentValue, String.Empty)
            'End If
            'Dim encodedXml As String = System.Security.SecurityElement.Escape(currentValue)
        End If
    End Sub

    Private Function IsValidTitle(ByVal _XMLElement As Xml.XmlElement, ByRef TitleType As String) As Boolean
        Dim ValidTitle As Boolean = False
        If _MasterTitle = TitleType Then
            If _XMLElement.Attributes(TitleType) IsNot Nothing Then
                If Not _XMLElement.Attributes(TitleType).Value.ToString = String.Empty Then
                    ValidTitle = True
                End If
            End If
        End If
        Return ValidTitle
    End Function

    Private Function AddEdition(ByRef Title As String, ByRef TitleField As String) As String
        Dim TempValue = Title
        If TempValue.Length > 0 Then
            Dim Edition As String = GetEdition(_filePath)
            If Edition <> "" Then
                If CurrentSettings.Edition_Name_Applies_To = TitleField Or CurrentSettings.Edition_Name_Applies_To = "Both Titles" Then
                    If Not TempValue.EndsWith(" (" & Edition & ")") Then
                        TempValue = TempValue & " (" & Edition & ")"
                    End If
                End If
            End If
        End If
        Return TempValue
    End Function

    Private Function AddGroupName(ByRef Title As String, ByRef TitleField As String) As String
        Dim TempValue = Title

        If TempValue.Length > 0 Then
            If _groupName <> "" Then
                If CurrentSettings.Folder_Name_Is_Group_Name = True Then
                    If CurrentSettings.Group_Name_Applies_To = TitleField Or CurrentSettings.Group_Name_Applies_To = "Both Titles" Then
                        'If TempValue <> _GroupName.ToString And Not TempValue.StartsWith(_GroupName.ToString & "\") Then
                        If Not TempValue.StartsWith(_groupName.ToString & "\") Then
                            TempValue = _groupName.ToString & "\" & TempValue
                        End If
                    End If
                End If
            Else
                If Not _internetData Is Nothing Then
                    If Not _internetData(GrabberOutput.Collection) Is Nothing Then
                        If _internetData(GrabberOutput.Collection).Length > 0 Then
                            TempValue = _internetData(GrabberOutput.Collection).ToString() & "\" & TempValue
                        End If
                    End If
                End If
            End If
        End If
        Return TempValue
    End Function

    Public Sub UpdateElement()

        Dim CurrentNode As Xml.XmlNode
        CurrentNode = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & _MovieNumber & "']")
        CurrentNode.Attributes(_SourceField).Value = _OverridePath
        _XMLElement = CurrentNode
    End Sub

    Private Function CreateCoverFromMovie(ByVal FileName As String, ByRef PicturePathToSave As String) As Boolean
        Dim NewCoverThumbName As String = ""
        Dim CoverFileExists As Boolean = False
        If CurrentSettings.Store_Image_With_Relative_Path = True Then
            Dim PicturePathPrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString 'Covers\'
            Dim PictureFileName As String = _movieNumber.ToString + "_" + Path.GetFileName(Path.ChangeExtension(FileName, "jpg"))
            Dim PicturePathFull As String = Path.Combine(_imagePath, PictureFileName)
            NewCoverThumbName = PicturePathFull
            'Check, if the returned picture is existing - it might not due to download errors like 404
            Try
                If Not File.Exists(NewCoverThumbName) Then
                    ' CoverFileExists = CreateArtworkFromMovie(FileName, NewCoverThumbName, Artwork_Thumb_Mode.Cover) ' try creating artwork from movie
                    CoverFileExists = GrabUtil.GetCoverartFromMovie(FileName, NewCoverThumbName, GrabUtil.ArtworkThumbMode.Cover, False, True, 0) ' try creating artwork from movie
                Else
                    CoverFileExists = True
                End If
            Catch ex As Exception
            End Try

            If File.Exists(PicturePathFull) Then

                'Separate the folder from the prefix string (if needed)
                Dim PrefixString As String = String.Empty
                Dim PrefixPath As String = String.Empty
                If PicturePathPrefix <> String.Empty Then
                    If PicturePathPrefix.Contains("\") = True Then
                        PrefixPath = PicturePathPrefix.Substring(0, PicturePathPrefix.LastIndexOf("\") + 1)
                        PrefixString = PicturePathPrefix.Substring(PicturePathPrefix.LastIndexOf("\") + 1)
                    Else
                        PrefixString = PicturePathPrefix
                    End If
                End If

                Dim NewFileName As String = String.Empty
                If PrefixString <> String.Empty Then
                    'Need to rename the file.
                    NewFileName = PicturePathFull.Replace(PictureFileName, PrefixString & PictureFileName)
                    If Not File.Exists(NewFileName) Then
                        File.Copy(PicturePathFull, NewFileName)
                    End If
                    File.Delete(PicturePathFull)
                    PicturePathFull = NewFileName
                    PictureFileName = PrefixString & PictureFileName
                End If

                If PrefixPath <> String.Empty Then
                    'Need to add the new folder to the short (relative) path:
                    PictureFileName = PrefixPath & PictureFileName
                End If

                If CurrentSettings.Store_Image_With_Relative_Path = True Then
                    PicturePathToSave = PictureFileName
                Else
                    PicturePathToSave = PicturePathFull
                End If
            End If

        Else
            NewCoverThumbName = _FilePath
            NewCoverThumbName = Path.ChangeExtension(NewCoverThumbName, "jpg") ' Now set to filename.jpg and use, if it exists
            PicturePathToSave = NewCoverThumbName
            Try
                If Not File.Exists(NewCoverThumbName) Then
                    ' CoverFileExists = CreateArtworkFromMovie(FileName, NewCoverThumbName, Artwork_Thumb_Mode.Cover) ' try creating artwork from movie
                    CoverFileExists = GrabUtil.GetCoverartFromMovie(FileName, NewCoverThumbName, GrabUtil.ArtworkThumbMode.Cover, False, True, 0) ' try creating artwork from movie
                End If
            Catch ex As Exception
            End Try
            Try
                If File.Exists(NewCoverThumbName) Then
                    CoverFileExists = True
                End If
            Catch ex As Exception
            End Try
        End If
        Return CoverFileExists
    End Function

    Private Function PictureNameWithSuffix(ByVal Expression As String, ByVal Year As String, ByVal Number As String) As String
        ' Expression sample: " (%year%)"
        If Year Is Nothing Then
            Year = ""
        End If
        If Number Is Nothing Then
            Number = ""
        End If

        If Year.Length = 0 And Number.Length = 0 Then
            Return ""
        End If

        Dim NewExpression As String = Expression
        If NewExpression.Contains("%year%") Then
            NewExpression = NewExpression.Replace("%year%", Year)
            If Year.Length = 0 And NewExpression.Contains("%number%") = False Then
                Return ""
            End If
        End If
        If NewExpression.Contains("%number%") Then
            NewExpression = NewExpression.Replace("%number%", Year)
            If Number.Length = 0 Then
                Return ""
            End If
        End If
        Return NewExpression
    End Function

    Private Function DrvLabel(ByVal MediaPath As String) As String
        Dim Label As String = ""

        ' Try to get DVD-Drive-Label
        Dim DrivePath As String = String.Empty
        Dim DriveLetter As String = String.Empty
        If _FilePath.IndexOf(":") > 0 Then
            'We're scanning a drive not a UNC, check what drive:
            DriveLetter = _FilePath.Substring(0, _FilePath.IndexOf(":") + 1) ' e.g. C:
            Dim myObjectSearcher As System.Management.ManagementObjectSearcher
            Dim myObject As System.Management.ManagementObject
            myObjectSearcher = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_CDROMDrive")
            For Each myObject In myObjectSearcher.Get
                If myObject("MediaLoaded") = "True" Then
                    If myObject("Drive").ToString.ToLower = DriveLetter.ToLower Then
                        Label = myObject("VolumeName").ToString
                    End If
                End If
            Next

            If Label = String.Empty Then ' Try to get Volumelabel from Disk Drive
                DriveLetter = DriveLetter.Substring(0, 2) ' e.g. C:\
                Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
                Dim d As DriveInfo
                For Each d In allDrives
                    If d.IsReady = True Then
                        If d.Name.ToString.ToLower.Substring(0, 2) = DriveLetter.ToLower Then
                            Label = d.VolumeLabel ' d.DriveFormat, d.DriveType
                        End If
                    End If
                Next
            End If
        Else ' not a drive name, but UNC path - so populate with UNC server name
            If _FilePath.StartsWith("\\") Then
                Dim tString As String
                tString = _FilePath.Substring(2)
                If tString.IndexOf("\") > 0 Then
                    Label = _FilePath.Substring(0, tString.IndexOf("\"))
                End If
            End If
        End If
        Return Label
    End Function

    Public Sub SaveProgress()
        'XMLDoc.Save(_XMLFilePath)

        'Dim xmlFile As New FileStream(_XMLFilePath, FileMode.Open, FileAccess.Write, FileShare.Read)
        'xmlFile.SetLength(0)
        'XMLDoc.Save(xmlFile)
        'xmlFile.Close()

        Try
            Using fsLock As FileStream = File.Create(LockFilename(_xmlFilePath), 1000, FileOptions.DeleteOnClose) ' create lock file to avoid concurrent writing
                Using fs As New FileStream(_xmlFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read) ' Dim xmlFile As New FileStream(_XMLFilePath, FileMode.Open, FileAccess.Write, FileShare.Read) ' lock the file for any other use, as we do write to it now !
                    fs.SetLength(0) ' do not append, owerwrite !
                    XMLDoc.Save(fs)
                    fs.Close() ' write buffer and release lock on file (either Flush, Dispose or Close is required)
                End Using
                fsLock.Close()
            End Using
            ' retry later 
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Function LockFilename(ByVal CatalogFile As String) As String
        Dim lockerfilename As String = ""
        Try
            Dim path As String = System.IO.Path.GetDirectoryName(CatalogFile)
            Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(CatalogFile)
            Dim machineName As String = System.Environment.MachineName
            lockerfilename = path & "\" & filename & "_" & machineName & ".lck"
        Catch ex As Exception
        End Try
        Return lockerfilename
    End Function

End Class





