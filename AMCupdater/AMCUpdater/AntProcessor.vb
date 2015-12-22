Imports System.Threading
Imports System.Xml
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms
Imports Grabber
Imports Grabber.Data
Imports Grabber.Util

Public Class AntProcessor

    Private Shared WithEvents bgwFolderScanUpdate As New BackgroundWorker ' changed to public to get progress from MF plugin
    Private Shared WithEvents bgwManualUpdate As New BackgroundWorker
    Private Shared WithEvents bgwManualMovieUpdate As New BackgroundWorker

    Private Shared ds As DataSet

    Public Shared XMLDoc As Xml.XmlDocument = New Xml.XmlDocument

    Private Shared _countXmlRecords As Integer
    Private Shared _countOrphanFiles As Integer = 0
    Private Shared _countOrphanRecords As Integer = 0
    Private Shared _countRecordsAdded As Integer = 0
    Private Shared _countRecordsDeleted As Integer = 0
    Private Shared _countMultiPartFiles As Integer = 0
    Private Shared _countMultiPartFilesMerged As Integer = 0

    Private Shared _isMultiFolderScan As Boolean
    Private Shared _interactiveMode As Boolean = True

    Private Shared _manualFieldName As String
    Private Shared _manualFieldNameDestination As String
    Private Shared _manualFieldValue As String
    Private Shared _manualFieldOldValue As String
    Private Shared _manualOperation As String
    Private Shared _manualParameterField1 As String
    Private Shared _manualParameterOperator1 As String
    Private Shared _manualParameterValue1 As String
    Private Shared _manualParameterField2 As String
    Private Shared _manualParameterOperator2 As String
    Private Shared _manualParameterValue2 As String
    Private Shared _manualParameterAndOr As String
    Private Shared _manualParameterMatchAll As Boolean
    Private Shared _manualXmlPath As String = String.Empty
    Private Shared _manualParserPath As String = String.Empty
    Private Shared _manualExcludedMoviesPath As String = String.Empty
    Private Shared _manualInternetLookupAlwaysPrompt As Boolean = False
    Private Shared _manualMissingFanartDownload As Boolean = True
    Private Shared _manualMissingTrailer As Boolean = True
    Private Shared _manualNfoFileHandling As String
    Private Shared _manualNfoFileOnlyUpdateMissing As Boolean
    Private Shared _tempXMLBackupFile As String
    Private Shared _operationCancelled As Boolean = False

    Public ReadOnly Property CountXMLRecords() As Integer
        Get
            _CountXMLRecords = 0
            _CountXMLRecords = ds.Tables("tblXML").Rows.Count
            Return _CountXMLRecords
        End Get
    End Property
    Public ReadOnly Property CountFilesFound() As Integer
        Get
            Dim FileCount As Integer = 0
            If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundMediaFiles").Rows.Count
            End If
            If ds.Tables("tblFoundTrailerFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundTrailerFiles").Rows.Count
            End If
            If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundNonMediaFiles").Rows.Count
            End If
            Return FileCount
        End Get
    End Property
    Public ReadOnly Property CountMediaFilesFound() As Integer
        Get
            Dim FileCount As Integer = 0
            If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundMediaFiles").Rows.Count
            End If
            Return FileCount
        End Get
    End Property
    Public ReadOnly Property CountNonMediaFilesFound() As Integer
        Get
            Dim FileCount As Integer = 0
            If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundNonMediaFiles").Rows.Count
            End If
            Return FileCount
        End Get
    End Property
    Public ReadOnly Property CountTrailerFilesFound() As Integer
        Get
            Dim FileCount As Integer = 0
            If ds.Tables("tblFoundTrailerFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundTrailerFiles").Rows.Count
            End If
            Return FileCount
        End Get
    End Property
    Public ReadOnly Property CountOrphanFiles() As Integer
        Get
            _CountOrphanFiles = 0
            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                _CountOrphanFiles += ds.Tables("tblOrphanedMediaFiles").Rows.Count
            End If
            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                _CountOrphanFiles += ds.Tables("tblOrphanedNonMediaFiles").Rows.Count
            End If
            If ds.Tables("tblOrphanedTrailerMediaFiles") IsNot Nothing Then
                _CountOrphanFiles += ds.Tables("tblOrphanedTrailerMediaFiles").Rows.Count
            End If
            Return _CountOrphanFiles
        End Get
    End Property
    Public ReadOnly Property CountOrphanRecords() As Integer
        Get
            _CountOrphanRecords = 0
            If ds.Tables("tblOrphanedAntRecords").Rows.Count > 0 Then
                _CountOrphanRecords = ds.Tables("tblOrphanedAntRecords").Rows.Count
            End If

            Return _CountOrphanRecords
        End Get
    End Property
    Public ReadOnly Property CountRecordsAdded() As Integer
        Get
            Return _CountRecordsAdded
        End Get
    End Property
    Public ReadOnly Property CountRecordsDeleted() As Integer
        Get
            Return _CountRecordsDeleted
        End Get
    End Property
    Public ReadOnly Property CountMultiPartFiles() As Integer
        Get
            If ds.Tables("tblMultiPartFiles") IsNot Nothing Then
                _CountMultiPartFiles = ds.Tables("tblMultiPartFiles").Rows.Count
            End If

            Return _CountMultiPartFiles
        End Get
    End Property
    Public ReadOnly Property CountMultiPartFilesMerged() As Integer
        Get
            Return _CountMultiPartFilesMerged
        End Get
    End Property
    Public ReadOnly Property GetAntFieldNames() As DataTable
        Get
            Dim dt As DataTable = ds.Tables("tblAntFields")
            dt.DefaultView.Sort = "FieldName"
            Return dt
        End Get
    End Property

    Public Property ManualFieldName() As String
        Get
            Return _ManualFieldName
        End Get
        Set(ByVal value As String)
            _ManualFieldName = value
        End Set
    End Property
    Public Property ManualFieldNameDestination() As String
        Get
            Return _ManualFieldNameDestination
        End Get
        Set(ByVal value As String)
            _ManualFieldNameDestination = value
        End Set
    End Property
    Public Property ManualFieldValue() As String
        Get
            Return _ManualFieldValue
        End Get
        Set(ByVal value As String)
            _ManualFieldValue = value
        End Set
    End Property
    Public Property ManualFieldOldValue() As String
        Get
            Return _ManualFieldOldValue
        End Get
        Set(ByVal value As String)
            _ManualFieldOldValue = value
        End Set
    End Property
    Public Property ManualOperation() As String
        Get
            Return _ManualOperation
        End Get
        Set(ByVal value As String)
            _ManualOperation = value
        End Set
    End Property
    Public Property ManualParameterField1() As String
        Get
            Return _ManualParameterField1
        End Get
        Set(ByVal value As String)
            _ManualParameterField1 = value
        End Set
    End Property
    Public Property ManualParameterOperator1() As String
        Get
            Return _ManualParameterOperator1
        End Get
        Set(ByVal value As String)
            _ManualParameterOperator1 = value
        End Set
    End Property
    Public Property ManualParameterValue1() As String
        Get
            Return _ManualParameterValue1
        End Get
        Set(ByVal value As String)
            _ManualParameterValue1 = value
        End Set
    End Property
    Public Property ManualParameterField2() As String
        Get
            Return _ManualParameterField2
        End Get
        Set(ByVal value As String)
            _ManualParameterField2 = value
        End Set
    End Property
    Public Property ManualParameterOperator2() As String
        Get
            Return _ManualParameterOperator2
        End Get
        Set(ByVal value As String)
            _ManualParameterOperator2 = value
        End Set
    End Property
    Public Property cbManualParameterAndOr() As String
        Get
            Return _ManualParameterAndOr
        End Get
        Set(ByVal value As String)
            _ManualParameterAndOr = value
        End Set
    End Property
    Public Property ManualParameterValue2() As String
        Get
            Return _ManualParameterValue2
        End Get
        Set(ByVal value As String)
            _ManualParameterValue2 = value
        End Set
    End Property

    Public Property ManualParameterMatchAll() As Boolean
        Get
            Return _ManualParameterMatchAll
        End Get
        Set(ByVal value As Boolean)
            _ManualParameterMatchAll = value
        End Set
    End Property
    Public Property ManualMissingFanartDownload() As Boolean
        Get
            Return _ManualMissingFanartDownload
        End Get
        Set(ByVal value As Boolean)
            _ManualMissingFanartDownload = value
        End Set
    End Property
    Public Property ManualMissingTrailer() As Boolean
        Get
            Return _ManualMissingTrailer
        End Get
        Set(ByVal value As Boolean)
            _ManualMissingTrailer = value
        End Set
    End Property
    Public Property ManualXMLPath() As String
        Get
            Return _ManualXMLPath
        End Get
        Set(ByVal value As String)
            _ManualXMLPath = value
        End Set
    End Property
    Public Property ManualParserPath() As String
        Get
            Return _ManualParserPath
        End Get
        Set(ByVal value As String)
            _ManualParserPath = value
        End Set
    End Property
    Public Property ManualNfoFileHandling() As String
        Get
            Return _ManualNfoFileHandling
        End Get
        Set(ByVal value As String)
            _ManualNfoFileHandling = value
        End Set
    End Property
    Public Property ManualNfoFileOnlyUpdateMissing() As Boolean
        Get
            Return _ManualNfoFileOnlyUpdateMissing
        End Get
        Set(ByVal value As Boolean)
            _ManualNfoFileOnlyUpdateMissing = value
        End Set
    End Property
    Public Property ManualExcludedMoviesPath()
        Get
            Return _ManualExcludedMoviesPath
        End Get
        Set(ByVal value)
            _ManualExcludedMoviesPath = value
        End Set
    End Property
    Public Property ManualInternetLookupAlwaysPrompt() As Boolean
        Get
            Return _ManualInternetLookupAlwaysPrompt
        End Get
        Set(ByVal value As Boolean)
            _ManualInternetLookupAlwaysPrompt = value
        End Set
    End Property
    Public ReadOnly Property ManualTestResultCount() As Integer
        Get
            If ds.Tables("tblNodesToProcess") Is Nothing Then
                Return 0
            Else
                Return ds.Tables("tblNodesToProcess").Rows.Count
            End If
        End Get
    End Property

    Public Property InteractiveMode() As Boolean
        Get
            Return _InteractiveMode
        End Get
        Set(ByVal value As Boolean)
            _InteractiveMode = value
        End Set
    End Property

    Public Sub New()
        BuildTables()
        bgwFolderScanUpdate.WorkerReportsProgress = True
        bgwFolderScanUpdate.WorkerSupportsCancellation = True
        bgwManualUpdate.WorkerReportsProgress = True
        bgwManualUpdate.WorkerSupportsCancellation = True
        bgwManualMovieUpdate.WorkerReportsProgress = True
        bgwManualMovieUpdate.WorkerSupportsCancellation = True
    End Sub

#Region "Manual Updater"

    Public Function ManualControlRecord(ByVal ManualParameterField As String, ByVal ManualParameterOperator As String, ByVal ManualParameterValue As String, ByRef CurrentNode As XmlNode) As Integer
        Select Case ManualParameterOperator
            Case "LIKE"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    Dim str1 As String = GetValue(CurrentNode, ManualParameterField).ToString
                    Dim str2 As String = ManualParameterValue

                    If str1.ToLower.IndexOf(str2.ToLower) >= 0 Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "NOT LIKE"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    Dim str1 As String = GetValue(CurrentNode, ManualParameterField).ToString
                    Dim str2 As String = ManualParameterValue

                    If str1.ToLower.IndexOf(str2.ToLower) >= 0 Then
                        Return 0
                    Else
                        Return 1
                    End If
                Else
                    Return 1
                End If
            Case "="
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    If GetValue(CurrentNode, ManualParameterField).ToString = ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "!="
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    'Attribute exists, check it's not a match:
                    If GetValue(CurrentNode, ManualParameterField).ToString <> ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    'Not there, so obviously not equal to our parameter!
                    Return 1
                End If
            Case ">"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    If GetValue(CurrentNode, ManualParameterField).ToString > ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case ">Num"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    If Val(GetValue(CurrentNode, ManualParameterField).ToString) > Val(ManualParameterValue) Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "<"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    'Attribute exists, check it's not a match:
                    If GetValue(CurrentNode, ManualParameterField).ToString < ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                End If
            Case "<Num"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then
                    'Attribute exists, check it's not a match:
                    If Val(GetValue(CurrentNode, ManualParameterField).ToString) < Val(ManualParameterValue) Then
                        Return 1
                    Else
                        Return 0
                    End If
                End If
            Case "EXISTS"
                If GetValue(CurrentNode, ManualParameterField) IsNot Nothing Then 'If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    Return 1
                Else
                    Return 0
                End If
            Case "NOT EXISTS"
                If GetValue(CurrentNode, ManualParameterField) Is Nothing Then
                    Return 1
                Else
                    Return 0
                End If
        End Select

    End Function


    Public Sub ManualTestOperation()

        '_ManualOperation = 'Update Value', 'Delete Record', 'Delete Value', 'Update Record'
        '_ManualFieldName = For Update or Delete Value
        '_ManualFieldValue = as above
        '_ManualParameterField = ant field to check
        '_ManualParameterOperator = '=', '!=', 'EXISTS', 'NOT EXISTS'
        '_ManualParameterValue = value to set.
        '_ManualParameterMatchAll = ignore params and match all

        'Dim NodesToProcess As New ArrayList()
        Dim TextReader As Xml.XmlTextReader
        TextReader = New Xml.XmlTextReader(_ManualXMLPath)
        Dim CurrentNode As Xml.XmlNode

        'Update a value = check if attribute exists and update it -
        'Delete a record - check the parameter and delete if match. - 
        'Delete Value = check if attribute exists and delete it - 

        LogEvent("Manual Database Operation Starting", EventLogLevel.ImportantEvent)
        LogEvent(" - Operation : " & _ManualOperation.ToString, EventLogLevel.ImportantEvent)
        If (_ManualOperation.ToString = "Update Value") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to Set : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Replace String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to be replaced : " & _ManualFieldOldValue.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to replace with : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Add String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to add to existing value : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Insert String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to insert to existing value : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Remove String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to remove from existing value : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Copy Value") Then
            LogEvent(" - Field to copy from : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Field to copy to   : " & _ManualFieldNameDestination.ToString, EventLogLevel.ImportantEvent)
        End If
        If _ManualParameterMatchAll = True Then
            LogEvent(" - Parameters - Match All Records : True", EventLogLevel.ImportantEvent)
        Else
            LogEvent(" - Parameters - Field to Check : " & _ManualParameterField1.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Parameters - Operator : " & _ManualParameterOperator1.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Parameters - Value to Find : " & _ManualParameterValue1.ToString, EventLogLevel.ImportantEvent)
            If _ManualParameterAndOr = "and" Or _ManualParameterAndOr = "or" Then
                LogEvent(" - Parameters - More : " & _ManualParameterAndOr.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Field to Check : " & _ManualParameterField2.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Operator : " & _ManualParameterOperator2.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Value to Find : " & _ManualParameterValue2.ToString, EventLogLevel.ImportantEvent)
            End If
            If _ManualParserPath <> "" Then
                LogEvent(" - Parameters - Internet Grabber Script : " & _ManualParserPath.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Excluded Movies File : " & _ManualExcludedMoviesPath.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Internet Lookup Always Prompt : " & _ManualInternetLookupAlwaysPrompt.ToString, EventLogLevel.ImportantEvent)
            End If
        End If
        If (_ManualOperation.ToString = "DownLoad Fanart") Then
            LogEvent(" - Download only Missing Fanart : " & _ManualMissingFanartDownload.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Register Trailer") Then
            LogEvent(" - Register only Missing Trailer : " & _ManualMissingTrailer.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update NFO File") Then
            LogEvent(" - Update NFO File : " & _ManualNfoFileHandling.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Delete NFO File") Then
            LogEvent(" - Delete NFO File : " & _ManualNfoFileHandling.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Add IMDB to File") Then
            LogEvent(" - Renaming movie media file - adding IMDB id where missing", EventLogLevel.ImportantEvent)
        End If


        'Dim XmlDoc As New XmlDocument
        XMLDoc = New XmlDocument
        'XMLDoc.Load(_ManualXMLPath)

        Dim xmlFile As New FileStream(_ManualXMLPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        XMLDoc.Load(xmlFile)

        Dim CurrentMovieNumber As Integer
        Dim movielist As XmlNodeList
        movielist = XMLDoc.DocumentElement.SelectNodes("/AntMovieCatalog/Catalog/Contents/Movie")
        For Each CurrentNode In movielist
            If CurrentNode.Attributes("Number").Value Is Nothing Then

            End If
            If (Not IsNothing(CurrentNode.Attributes("Number"))) Then
                If (CurrentNode.Attributes("Number").Value.Length <> 0) Then
                    CurrentMovieNumber = CurrentNode.Attributes("Number").Value
                Else
                    Continue For
                End If
            Else
                Continue For
            End If

            'CurrentMovieNumber = TextReader.GetAttribute("Number")
            'CurrentNode = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & CurrentMovieNumber.ToString & "']")
            ''CurrentNode = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & CurrentMovieNumber.ToString & "']")
            If CurrentNode IsNot Nothing Then
                Dim wtitle As String
                If (Not IsNothing(CurrentNode.Attributes("TranslatedTitle"))) Then
                    If (CurrentNode.Attributes("TranslatedTitle").Value.Length <> 0) Then
                        wtitle = CurrentNode.Attributes("TranslatedTitle").Value
                    Else
                        wtitle = CurrentNode.Attributes("OriginalTitle").Value
                    End If
                Else
                    wtitle = CurrentNode.Attributes("OriginalTitle").Value
                End If

                'Dim wotitle As String = wtitle
                'If (Not IsNothing(CurrentNode.Attributes("OriginalTitle"))) Then
                '    If (CurrentNode.Attributes("OriginalTitle").Value.Length <> 0) Then
                '        wotitle = Grabber.GrabUtil.normalizeTitle(CurrentNode.Attributes("OriginalTitle").Value)
                '    End If
                'End If

                ' skip those:
                If _manualOperation = "Download Fanart" Then
                    If _manualMissingFanartDownload Then
                        If Not ManualTestMissingFanart_IsUpdateNeeded(wtitle) Then Continue For
                    End If
                End If

                ' get search hints for better matching from DB
                Dim wyear As String = ""
                If (Not IsNothing(CurrentNode.Attributes("Year"))) Then
                    wyear = CurrentNode.Attributes("Year").Value
                End If
                Dim wdirector As String = ""
                If (Not IsNothing(CurrentNode.Attributes("Director"))) Then
                    wdirector = CurrentNode.Attributes("Director").Value
                End If

                Dim wIMDB_Id As String = GetValue(CurrentNode, "IMDB_Id")
                If (Not String.IsNullOrEmpty(wIMDB_Id)) Then
                    If (Not IsNothing(CurrentNode.Attributes("URL"))) Then
                        Dim wIMDBfromURL As String = GetIMDBidFromFilePath(CurrentNode.Attributes("URL").Value) ' tries to get IMDBid from URL
                        If wIMDBfromURL.Length > 0 Then
                            wIMDB_Id = wIMDBfromURL
                        End If
                    End If
                End If

                If _manualParameterMatchAll = True Then
                    'We're matching all records - proceed with editing
                    ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
                    LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                Else
                    'Parameters in use - check first then proceed
                    If CurrentNode IsNot Nothing Then
                        Dim wrecord As Integer
                        wrecord = ManualControlRecord(_manualParameterField1, _manualParameterOperator1, _manualParameterValue1, CurrentNode)
                        wrecord = wrecord + ManualControlRecord(_manualParameterField2, _manualParameterOperator2, _manualParameterValue2, CurrentNode)
                        If (_manualParameterAndOr <> "and" And wrecord > 0) Then
                            ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
                            LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                        Else
                            If (_manualParameterAndOr = "and" And wrecord = 2) Then
                                ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
                                LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                            End If
                        End If
                    End If
                End If

            End If
        Next


        'While TextReader.Read()
        '    If TextReader.Name = "Movie" Then
        '        CurrentMovieNumber = TextReader.GetAttribute("Number")
        '        CurrentNode = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & CurrentMovieNumber.ToString & "']")
        '        'CurrentNode = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & CurrentMovieNumber.ToString & "']")
        '        If CurrentNode IsNot Nothing Then
        '            Dim wtitle As String
        '            If (Not IsNothing(CurrentNode.Attributes("TranslatedTitle"))) Then
        '                If (CurrentNode.Attributes("TranslatedTitle").Value.Length <> 0) Then
        '                    wtitle = CurrentNode.Attributes("TranslatedTitle").Value
        '                Else
        '                    wtitle = CurrentNode.Attributes("OriginalTitle").Value
        '                End If
        '            Else
        '                wtitle = CurrentNode.Attributes("OriginalTitle").Value
        '            End If

        '            'Dim wotitle As String = wtitle
        '            'If (Not IsNothing(CurrentNode.Attributes("OriginalTitle"))) Then
        '            '    If (CurrentNode.Attributes("OriginalTitle").Value.Length <> 0) Then
        '            '        wotitle = Grabber.GrabUtil.normalizeTitle(CurrentNode.Attributes("OriginalTitle").Value)
        '            '    End If
        '            'End If

        '            ' skip those:
        '            If _ManualOperation = "Download Fanart" Then
        '                If _ManualMissingFanartDownload Then
        '                    If Not ManualTestMissingFanart_IsUpdateNeeded(wtitle) Then Continue While
        '                End If
        '            End If

        '            ' get search hints for better matching from DB
        '            Dim wyear As String = ""
        '            If (Not IsNothing(CurrentNode.Attributes("Year"))) Then
        '                wyear = CurrentNode.Attributes("Year").Value
        '            End If
        '            Dim wdirector As String = ""
        '            If (Not IsNothing(CurrentNode.Attributes("Director"))) Then
        '                wdirector = CurrentNode.Attributes("Director").Value
        '            End If

        '            Dim wIMDB_Id As String = ""
        '            If (Not IsNothing(CurrentNode.Attributes("IMDB_Id"))) Then
        '                wIMDB_Id = CurrentNode.Attributes("IMDB_Id").Value
        '            ElseIf CurrentNode.Item("IMDB_Id") IsNot Nothing Then
        '                If CurrentNode.Item("IMDB_Id").InnerText.Length > 0 Then
        '                    wIMDB_Id = CurrentNode.Item("IMDB_Id").InnerText.ToString
        '                ElseIf (Not IsNothing(CurrentNode.Attributes("URL"))) Then
        '                    Dim wIMDBfromURL As String = GetIMDBidFromFilePath(CurrentNode.Attributes("URL").Value) ' tries to get IMDBid from URL
        '                    If wIMDBfromURL.Length > 0 Then
        '                        wIMDB_Id = wIMDBfromURL
        '                    End If
        '                End If
        '            End If

        '            If _ManualParameterMatchAll = True Then
        '                'We're matching all records - proceed with editing
        '                ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
        '                LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
        '            Else
        '                'Parameters in use - check first then proceed
        '                If CurrentNode IsNot Nothing Then
        '                    Dim wrecord As Integer
        '                    wrecord = ManualControlRecord(_ManualParameterField1, _ManualParameterOperator1, _ManualParameterValue1, CurrentNode)
        '                    wrecord = wrecord + ManualControlRecord(_ManualParameterField2, _ManualParameterOperator2, _ManualParameterValue2, CurrentNode)
        '                    If (_ManualParameterAndOr <> "and" And wrecord > 0) Then
        '                        ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
        '                        LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
        '                    Else
        '                        If (_ManualParameterAndOr = "and" And wrecord = 2) Then
        '                            ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, wdirector, wyear, wIMDB_Id})
        '                            LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
        '                        End If
        '                    End If
        '                End If
        '            End If

        '        End If
        '    End If
        'End While

        If ds.Tables("tblNodesToProcess").Rows.Count > 0 Then
            LogEvent("Ready to Proceed. " & ds.Tables("tblNodesToProcess").Rows.Count.ToString & " Records Matched.", EventLogLevel.ImportantEvent)
            Form1.btnManualApplyChanges.Enabled = True
        Else
            LogEvent("Nothing to process - " & ds.Tables("tblNodesToProcess").Rows.Count.ToString & " Records Matched.", EventLogLevel.ImportantEvent)
            Form1.btnManualApplyChanges.Enabled = False
        End If
        LogEvent("===================================================================================================", EventLogLevel.Informational)


        If Not (TextReader Is Nothing) Then
            TextReader.Close()
        End If

        'If Not (xmlFile Is Nothing) Then
        '    xmlFile.Close()
        'End If

    End Sub

    Public Sub ManualRunOperation()

        _OperationCancelled = False

        ' backup the xml file before updating 
        If CurrentSettings.Backup_XML_First = True Then
            LogEvent("Backing up xml file.", EventLogLevel.ImportantEvent)
            Dim NewFileName As String = Replace(CurrentSettings.XML_File, ".xml", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".xml")
            NewFileName = NewFileName.Replace("/", "-")

            Try
                My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, NewFileName, True)
                Dim NewFileNameShort As String = System.IO.Path.GetFileName(NewFileName)
                LogEvent("Backing up xml file - done. (" & NewFileNameShort & ")", EventLogLevel.ImportantEvent)
            Catch ex As Exception
                LogEvent("ErrorEvent : Cannot back up xml file : " & ex.Message, EventLogLevel.ErrorEvent)
            End Try
        End If

        If ds.Tables("tblNodesToProcess") IsNot Nothing Then
            LogEvent("Performing Manual Update Process", EventLogLevel.ImportantEvent)
            Form1.ToolStripProgressBar.Minimum = 0
            Form1.ToolStripProgressBar.Maximum = ds.Tables("tblNodesToProcess").Rows.Count + 1
            Form1.ToolStripProgressBar.Value = 0
            Form1.btnManualCancel.Enabled = True
            Form1.btnManualDoTest.Enabled = False
            Form1.btnManualApplyChanges.Enabled = False
            Dim muc As New ManualUpdateClass
            muc.XmlDoc = XMLDoc

            'TO SWITCH FROM SINGLE-THREAD to MULTI-THREAD, CHANGE HERE!
            bgwManualUpdate.RunWorkerAsync(muc)
            'muc.RunUpdate()
            'bgwManualUpdate_PostProcessing()
        End If
    End Sub

    Public Sub ManualUpdateOperation()

        _OperationCancelled = False

        ' backup the xml file before updating 
        If CurrentSettings.Backup_XML_First = True Then
            LogEvent("Backing up xml file.", EventLogLevel.ImportantEvent)
            Dim NewFileName As String = Replace(CurrentSettings.XML_File, ".xml", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".xml")
            NewFileName = NewFileName.Replace("/", "-")

            Try
                My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, NewFileName, True)
                Dim NewFileNameShort As String = Path.GetFileName(NewFileName)
                LogEvent("Backing up xml file - done. (" & NewFileNameShort & ")", EventLogLevel.ImportantEvent)
            Catch ex As Exception
                LogEvent("ErrorEvent : Cannot back up xml file : " & ex.Message, EventLogLevel.ErrorEvent)
            End Try
        End If


        'Dim XmlDoc As New XmlDocument
        XMLDoc = New XmlDocument
        'XMLDoc.Load(_ManualXMLPath)

        Dim xmlFile As New FileStream(_ManualXMLPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        XMLDoc.Load(xmlFile)

        LogEvent("Performing Single Update Process", EventLogLevel.ImportantEvent)
        Dim smuc As New SingleMovieUpdateClass
        smuc.XmlDoc = XMLDoc

        'TO SWITCH FROM SINGLE-THREAD to MULTI-THREAD, CHANGE HERE!
        'bgwManualMovieUpdate.RunWorkerAsync(smuc)
        smuc.RunUpdate()
        If smuc.DoSave = True Then
            bgwManualUpdate_PostProcessing()
        End If

    End Sub

    Private Class ManualUpdateClass

        Public XmlDoc As XmlDocument
        'Public ParserPath As String
        'Public ExcludeFile As String
        'Public InteractiveMode As Boolean
        'Public InternetLookupAlwaysPrompt As Boolean

        Public Sub RunUpdate()

            Dim CurrentNode As Xml.XmlNode
            'Dim newAttr As Xml.XmlAttribute
            Dim MovieRootNode As Xml.XmlNode = XmlDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents")
            Dim ProcessCounter As Integer = 0
            Dim DoScan As Boolean = True

            ' read 'Excluded Movie File' for media files to skip, if option is set
            Dim XMLExclTable As New Hashtable ' "always ignore" movies
            If (File.Exists(CurrentSettings.Excluded_Movies_File)) Then
                Dim sr As StreamReader = File.OpenText(CurrentSettings.Excluded_Movies_File)
                Dim i As Integer = 0
                Do While sr.Peek() >= 0
                    XMLExclTable.Add(i, sr.ReadLine.ToLower)
                    i += 1
                Loop
                sr.Close()
            End If

            For Each row As DataRow In ds.Tables("tblNodesToProcess").Rows
                CurrentNode = XmlDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & row("AntID") & "']")
                If bgwManualUpdate.CancellationPending = True Then
                    Exit Sub
                End If

                Select Case _manualOperation
                    'Update Value
                    'Delete Value
                    'Copy Value
                    'Update Record
                    'Delete Record
                    'Download Fanart
                    'Update NFO File

                    Case "Update Value"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            SetValue(CurrentNode, _manualFieldName, _manualFieldValue)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (and added) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            SetValue(CurrentNode, _manualFieldName, _manualFieldValue)
                            'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                        '    newAttr.Value = _ManualFieldValue
                        '    CurrentNode.Attributes.Append(newAttr)
                        '    'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (and added) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'Else
                        '    CurrentNode.Attributes(_ManualFieldName).Value = _ManualFieldValue
                        '    'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'End If
                    Case "Update Value - Replace String"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            ' Do nothing, as old value to be replaced is not contained !
                            'newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                            'newAttr.Value = ""
                            'CurrentNode.Attributes.Append(newAttr)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            If GetValue(CurrentNode, _manualFieldName).Contains(_manualFieldOldValue) = True Then
                                SetValue(CurrentNode, _manualFieldName, GetValue(CurrentNode, _manualFieldName).Replace(_manualFieldOldValue, _manualFieldValue))
                                'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Replaced String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value not updated (Replace String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    ' Do nothing, as old value to be replaced is not contained !
                        '    'newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                        '    'newAttr.Value = ""
                        '    'CurrentNode.Attributes.Append(newAttr)
                        '    'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'Else
                        '    If CurrentNode.Attributes(_ManualFieldName).Value.Contains(_ManualFieldOldValue) = True Then
                        '        CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value.Replace(_ManualFieldOldValue, _ManualFieldValue)
                        '        'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Replaced String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    Else
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value not updated (Replace String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    End If
                        'End If
                    Case "Update Value - Add String"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            SetValue(CurrentNode, _manualFieldName, _manualFieldValue)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added Field and String) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            SetValue(CurrentNode, _manualFieldName, GetValue(CurrentNode, _manualFieldName) & _manualFieldValue)
                            'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                        '    newAttr.Value = _ManualFieldValue
                        '    CurrentNode.Attributes.Append(newAttr)
                        '    'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added Field and String) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'Else
                        '    CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value & _ManualFieldValue
                        '    'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'End If
                    Case "Update Value - Insert String"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            SetValue(CurrentNode, _manualFieldName, _manualFieldValue)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added Field and String) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            SetValue(CurrentNode, _manualFieldName, _manualFieldValue & GetValue(CurrentNode, _manualFieldName))
                            'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Inserted String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                        '    newAttr.Value = _ManualFieldValue
                        '    CurrentNode.Attributes.Append(newAttr)
                        '    'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added Field and String) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'Else
                        '    CurrentNode.Attributes(_ManualFieldName).Value = _ManualFieldValue & CurrentNode.Attributes(_ManualFieldName).Value
                        '    'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Inserted String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'End If
                    Case "Update Value - Remove String"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            ' Do nothing
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Removed - (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            If GetValue(CurrentNode, _manualFieldName).Contains(_manualFieldValue) = True Then
                                SetValue(CurrentNode, _manualFieldName, GetValue(CurrentNode, _manualFieldName).Replace(_manualFieldValue, ""))
                                'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Removed String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (Remove String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    ' Do nothing
                        '    'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Removed - (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'Else
                        '    If CurrentNode.Attributes(_ManualFieldName).Value.Contains(_ManualFieldValue) = True Then
                        '        CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value.Replace(_ManualFieldValue, "")
                        '        'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Removed String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    Else
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (Remove String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    End If
                        'End If
                    Case "Delete Record"
                        If Not CurrentNode Is Nothing Then
                            MovieRootNode.RemoveChild(CurrentNode)
                            'LogEvent("Record Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Record Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                    Case "Delete Value"
                        If Not GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            RemoveValue(CurrentNode, _manualFieldName)
                            'LogEvent("Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                        'If Not CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    CurrentNode.Attributes.Remove(CurrentNode.Attributes.GetNamedItem(_ManualFieldName))
                        '    'LogEvent("Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'End If
                    Case "Copy Value"
                        If GetValue(CurrentNode, _manualFieldName) Is Nothing Then
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied (No source value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        ElseIf Not CurrentSettings.Only_Update_With_Nonempty_Data Or (CurrentSettings.Only_Update_With_Nonempty_Data And Not String.IsNullOrEmpty(GetValue(CurrentNode, _manualFieldName))) Then
                            If GetValue(CurrentNode, _manualFieldNameDestination) Is Nothing Then
                                SetValue(CurrentNode, _manualFieldNameDestination, GetValue(CurrentNode, _manualFieldName))
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value copied (and field created): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            ElseIf Not CurrentSettings.Only_Add_Missing_Data Or (CurrentSettings.Only_Add_Missing_Data And String.IsNullOrEmpty(GetValue(CurrentNode, _manualFieldNameDestination))) Then
                                SetValue(CurrentNode, _manualFieldNameDestination, GetValue(CurrentNode, _manualFieldName))
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value copied: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied - non empty destination should not be overwritten: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        Else
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied - destination should not be overwritten with empty data: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                        'If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied (No source value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'ElseIf Not CurrentSettings.Only_Update_With_Nonempty_Data Or (CurrentSettings.Only_Update_With_Nonempty_Data And Not String.IsNullOrEmpty(CurrentNode.Attributes(_ManualFieldName).Value)) Then
                        '    If CurrentNode.Attributes(_ManualFieldNameDestination) Is Nothing Then
                        '        newAttr = XmlDoc.CreateAttribute(_ManualFieldNameDestination)
                        '        newAttr.Value = CurrentNode.Attributes(_ManualFieldName).Value
                        '        CurrentNode.Attributes.Append(newAttr)
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value copied (and field created): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    ElseIf Not CurrentSettings.Only_Add_Missing_Data Or (CurrentSettings.Only_Add_Missing_Data And String.IsNullOrEmpty(CurrentNode.Attributes(_ManualFieldNameDestination).Value)) Then
                        '        CurrentNode.Attributes(_ManualFieldNameDestination).Value = CurrentNode.Attributes(_ManualFieldName).Value
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value copied: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    Else
                        '        bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied - non empty destination should not be overwritten: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        '    End If
                        'Else
                        '    bgwManualUpdate.ReportProgress(ProcessCounter, "Value not copied - destination should not be overwritten with empty data: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        'End If
                    Case "Update Record"
                        Dim FileToScan As String = String.Empty
                        Dim AllFilesPath As String = String.Empty
                        If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                            FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                        End If

                        If FileToScan.IndexOf(";") > 0 Then
                            'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                            AllFilesPath = FileToScan
                            FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
                        End If

                        Dim wDirector As String = ""
                        Dim wYear As String = ""
                        Dim wIMDB_Id As String = ""
                        If row("AntTitle").ToString.Length > 0 Then
                            If (row("AntDirector").ToString.Length > 0) Then
                                If (row("AntDirector").ToString.IndexOf(",") > 0) Then
                                    wDirector = row("AntDirector").ToString.Substring(0, row("AntDirector").ToString.IndexOf(","))
                                Else
                                    wDirector = row("AntDirector").ToString
                                End If
                            End If
                            If (row("AntYear").ToString.Length > 0) Then
                                wYear = row("AntYear").ToString
                            End If
                            If (row("AntIMDB_Id").ToString.Length > 0) Then
                                wIMDB_Id = row("AntIMDB_Id").ToString
                            End If
                        End If

                        DoScan = True
                        ' search Filename
                        Dim wtitle As String = row("AntTitle").ToString
                        If IsFileNeeded() = True Then
                            'If My.Computer.FileSystem.FileExists(FileToScan) = False Then
                            If (CurrentSettings.Folder_Name_Is_Group_Name) Then
                                If (CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles") Then
                                    wtitle = "OriginalTitle"
                                Else
                                    If (CurrentSettings.Group_Name_Applies_To = "Translated Title") Then
                                        wtitle = "TranslatedTitle"
                                    ElseIf (CurrentSettings.Group_Name_Applies_To = "Formatted Title") Then
                                        wtitle = "FormattedTitle"
                                    End If
                                End If
                                Try
                                    wtitle = CurrentNode.Attributes(wtitle).Value
                                Catch ex As Exception
                                    wtitle = row("AntTitle").ToString
                                End Try
                            End If
                            SearchFile(FileToScan, CurrentSettings.Movie_Scan_Path, CurrentNode.Attributes("Number").Value, wtitle)
                            If Not FileToScan.Length > 0 Then
                                bgwManualUpdate.ReportProgress(ProcessCounter, "File Not Scanned (File Not Found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                'DoScan = False
                            End If
                        End If

                        If (CurrentSettings.Skip_Excluded_Movie_Files And FileToScan.Length > 0 And XMLExclTable.ContainsValue(FileToScan.ToLower) = True) Then ' if in "always ignore" file, skip update
                            bgwManualUpdate.ReportProgress(ProcessCounter, "File Scanned for Data : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString & " | " & "Skipped due to 'Excluded Movie File'")
                            DoScan = False
                        End If

                        'If FileToScan.Length > 0 Then
                        If DoScan = True Then
                            Dim Ant As New AntRecord()
                            With Ant
                                .XMLDoc = XmlDoc
                                .XMLElement = CurrentNode
                                .XMLFilePath = CurrentSettings.XML_File
                                .FilePath = FileToScan
                                .AllFilesPath = AllFilesPath
                                If AllFilesPath.Length > 0 Then
                                    .OverridePath = AllFilesPath
                                Else
                                    .OverridePath = FileToScan
                                End If
                                'File(Name)
                                'Folder(Name)
                                'Relative(Name)
                                If FileToScan.Length > 0 Then ' first try to get groupname by default rules from filename
                                    If FileToScan.ToString.Contains("\") Then
                                        .GroupName = GetGroupName(FileToScan, CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                                    End If
                                Else
                                    If wtitle.Length > 0 Then ' fallback to possibly existing group name
                                        If wtitle.Contains("\") = True Then
                                            .GroupName = wtitle.Substring(0, wtitle.LastIndexOf("\"))
                                            'Console.WriteLine("-" & .GroupName.ToString & "-")
                                        End If
                                    End If
                                End If
                                'If CurrentNode.Attributes("MovieNumber") Then
                                .ExcludeFile = CurrentSettings.Excluded_Movies_File
                                .InternetLookupAlwaysPrompt = CurrentSettings.Manual_Internet_Lookup_Always_Prompt
                                .MasterTitle = CurrentSettings.Master_Title
                                .InteractiveMode = True
                                Dim ImagePath As String = CurrentSettings.XML_File.Substring(0, CurrentSettings.XML_File.LastIndexOf("\"))
                                Dim ImagePrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString
                                If (CurrentSettings.Use_Folder_Dot_Jpg = False And CurrentSettings.Image_Download_Filename_Prefix.Length > 0) Then
                                    If ImagePrefix.LastIndexOf("\") > -1 Then 'Example : "foldername\" or "foldername\prefix_"
                                        Dim PictureFolder As String = ImagePrefix.Substring(0, ImagePrefix.IndexOf("\"))
                                        ImagePath = ImagePath & "\" & PictureFolder
                                        If Not My.Computer.FileSystem.DirectoryExists(ImagePath) Then
                                            My.Computer.FileSystem.CreateDirectory(ImagePath)
                                        Else
                                            'ImagePath = ImagePath & "\" & ImagePrefix
                                        End If
                                    End If
                                End If
                                .ImagePath = ImagePath
                                .InternetSearchHint = wDirector
                                .InternetSearchHintYear = wYear
                                .InternetSearchHintIMDB_Id = wIMDB_Id
                                .ParserPath = CurrentSettings.Manual_Internet_Parser_Path
                                .OnlyAddMissingData = CurrentSettings.Only_Add_Missing_Data ' added for "add missing data" mode"
                                .OnlyUpdateNonEmptyData = CurrentSettings.Only_Update_With_Nonempty_Data
                                .Dont_Ask_Interactive = CurrentSettings.Manual_Dont_Ask_Interactive ' added for silent updates without asking user to choose movie on failed auto matches
                                .GrabberOverrideLanguage = CurrentSettings.Grabber_Override_Language
                                .GrabberOverridePersonLimit = CurrentSettings.Grabber_Override_PersonLimit
                                .GrabberOverrideTitleLimit = CurrentSettings.Grabber_Override_TitleLimit
                                .GrabberOverrideGetRoles = CurrentSettings.Grabber_Override_GetRoles
                                .ProcessFile(AntRecord.ProcessModeNames.Update)
                                .SaveProgress()
                            End With

                            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
                                bgwManualUpdate.ReportProgress(ProcessCounter, Ant.LastOutputMessage)
                            Else
                                If (CurrentNode.Attributes("Number") Is Nothing) Then
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "File Scanned for Data : " & row(0).ToString & " | " & row("AntTitle").ToString & " | " & Ant.LastOutputMessage)
                                Else
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "File Scanned for Data : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString & " | " & Ant.LastOutputMessage)
                                End If
                            End If

                        End If
                    Case "Download Fanart"
                        If Not CurrentNode Is Nothing Then

                            If CurrentSettings.Use_Grabber_For_Fanart = True Then
                                'Todo: Add logic
                            End If

                            Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                            Dim fanart As List(Of DbMovieInfo)

                            Dim FileToScan As String = String.Empty
                            Dim AllFilesPath As String = String.Empty
                            If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                                FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                            End If

                            If FileToScan.IndexOf(";") > 0 Then
                                'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                                AllFilesPath = FileToScan
                                FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
                            End If

                            Dim wIMDB_Id As String = ""
                            If row("AntTitle").ToString.Length > 0 Then
                                If (row("AntIMDB_Id").ToString.Length > 0) Then
                                    wIMDB_Id = row("AntIMDB_Id").ToString
                                End If
                            End If

                            Dim fanartTitle As String = ""
                            Dim year As Int16 = 0

                            Dim ftitle As String = ""
                            Dim director As String = ""
                            Dim title As String = String.Empty
                            Dim ttitle As String = String.Empty

                            fanartTitle = GetFanartTitle(CurrentNode, title, ttitle, ftitle, year, director)

                            If fanartTitle.Length > 0 Then
                                fanart = Gb.GetFanart(title, ttitle, year, director, wIMDB_Id, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title, String.Empty, CurrentSettings.Movie_Fanart_Number_Limit, CurrentSettings.Movie_Fanart_Resolution_Min, CurrentSettings.Movie_Fanart_Resolution_Max)
                                'End If
                                If (fanart.Count > 0) Then
                                    If (fanart(0).Name = "already") Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart already downloaded : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name = "added") Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart added : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name.StartsWith("resolution")) Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Resolution requirements missed : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name.StartsWith("numberlimit")) Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart added (download limit reached) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name.StartsWith("(toomany)")) Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Too many Movies found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    Else
                                        If (fanart.Count = 1 And fanart(0).Backdrops.Count > 0) Then
                                            bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart downloaded : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                        Else
                                            bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart not Found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                        End If
                                    End If
                                Else
                                    fanartTitle = Grabber.GrabUtil.CreateFilename(fanartTitle.ToLower().Trim.Replace(" ", "."))
                                    If Not (System.IO.Directory.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & fanartTitle & "}")) Then
                                        System.IO.Directory.CreateDirectory(CurrentSettings.Movie_Fanart_Path & "\\{" & fanartTitle & "}")
                                    End If
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart not Found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                End If

                                If CurrentSettings.Movie_PersonArtwork_Path.Length > 0 And System.IO.Directory.Exists(CurrentSettings.Movie_PersonArtwork_Path) Then
                                    Dim filenameperson As String = String.Empty
                                    Dim filename1person As String = String.Empty
                                    Dim filename2person As String = String.Empty
                                    Dim listepersons As List(Of DbPersonInfo) = fanart(0).Persons
                                    For Each person As DbPersonInfo In listepersons
                                        Dim firstpersonimage As Boolean = True
                                        Dim onlysinglepersonimage As Boolean = True
                                        Dim persondetails As DbPersonInfo = New DbPersonInfo()
                                        Dim TheMoviedb As New TheMoviedb()
                                        persondetails = TheMoviedb.GetPersonsById(person.Id, String.Empty)
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "PersonImages : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString & "Person Artwork - " + persondetails.Images.Count.ToString() & " Images found for '" + persondetails.Name & "'")
                                        If persondetails.Images.Count > 0 Then
                                            Dim i As Integer = 0
                                            For Each image As String In persondetails.Images
                                                filename1person = Grabber.GrabUtil.DownloadPersonArtwork(CurrentSettings.Movie_PersonArtwork_Path, image, persondetails.Name, True, firstpersonimage, filenameperson)
                                                If filenameperson = String.Empty Then
                                                    filenameperson = filename1person
                                                End If
                                                If Not (filenameperson = "already" AndAlso filename1person = "already") Then
                                                    filenameperson = "added"
                                                End If
                                                firstpersonimage = False
                                                i += 1
                                                If onlysinglepersonimage Then
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    Case "Update NFO File"
                        Dim FileToScan As String = String.Empty
                        Dim AllFilesPath As String = String.Empty
                        If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                            FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                        End If

                        If FileToScan.IndexOf(";") > 0 Then
                            'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                            AllFilesPath = FileToScan
                            FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
                        End If

                        DoScan = True
                        ' search Filename
                        Dim wtitle As String = row("AntTitle").ToString
                        If IsFileNeeded() = True Then
                            If (CurrentSettings.Folder_Name_Is_Group_Name) Then
                                If (CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles") Then
                                    wtitle = "OriginalTitle"
                                Else
                                    If (CurrentSettings.Group_Name_Applies_To = "Translated Title") Then
                                        wtitle = "TranslatedTitle"
                                    ElseIf (CurrentSettings.Group_Name_Applies_To = "Formatted Title") Then
                                        wtitle = "FormattedTitle"
                                    End If
                                End If
                                Try
                                    wtitle = CurrentNode.Attributes(wtitle).Value
                                Catch ex As Exception
                                    wtitle = row("AntTitle").ToString
                                End Try
                            End If
                            SearchFile(FileToScan, CurrentSettings.Movie_Scan_Path, CurrentNode.Attributes("Number").Value, wtitle)
                            If Not FileToScan.Length > 0 Then
                                DoScan = False
                            End If
                        End If

                        If DoScan = True Then
                            Dim NfoMyFilmsFilename As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileToScan), "MyFilms.nfo")
                            Dim NfoMovieFilename As String = System.IO.Path.ChangeExtension(FileToScan, "nfo")

                            Dim NfoMyFilmsFilenameShort As String = System.IO.Path.GetFileName(NfoMyFilmsFilename)
                            Dim NfoMovieFilenameShort As String = System.IO.Path.GetFileName(NfoMovieFilename)


                            Try
                                If _manualNfoFileHandling = "MyFilms.nfo" Then
                                    WriteNfoFile(NfoMyFilmsFilename, CurrentNode, _manualNfoFileOnlyUpdateMissing, CurrentSettings.Master_Title)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO file '" & NfoMyFilmsFilenameShort & "' written for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                ElseIf _manualNfoFileHandling = "Movie Name" Then
                                    WriteNfoFile(NfoMovieFilename, CurrentNode, _manualNfoFileOnlyUpdateMissing, CurrentSettings.Master_Title)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO file '" & NfoMovieFilenameShort & "' written for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                ElseIf _manualNfoFileHandling = "Both" Then
                                    WriteNfoFile(NfoMyFilmsFilename, CurrentNode, _manualNfoFileOnlyUpdateMissing, CurrentSettings.Master_Title)
                                    WriteNfoFile(NfoMovieFilename, CurrentNode, _manualNfoFileOnlyUpdateMissing, CurrentSettings.Master_Title)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO files '" & NfoMyFilmsFilenameShort & ", " & NfoMovieFilenameShort & "' written for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                End If
                            Catch ex As Exception
                                bgwManualUpdate.ReportProgress(ProcessCounter, "ErrorEvent creating/updating NFO file : " & ex.Message)
                            End Try
                        Else
                            bgwManualUpdate.ReportProgress(ProcessCounter, "NFO File(s) not created/updated (File/Path Not Found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If

                    Case "Delete NFO File"
                        Dim FileToScan As String = String.Empty
                        Dim AllFilesPath As String = String.Empty
                        If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                            FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                        End If

                        If FileToScan.IndexOf(";") > 0 Then
                            'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                            AllFilesPath = FileToScan
                            FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
                        End If

                        DoScan = True
                        ' search Filename
                        Dim wtitle As String = row("AntTitle").ToString
                        If IsFileNeeded() = True Then
                            If (CurrentSettings.Folder_Name_Is_Group_Name) Then
                                If (CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles") Then
                                    wtitle = "OriginalTitle"
                                Else
                                    If (CurrentSettings.Group_Name_Applies_To = "Translated Title") Then
                                        wtitle = "TranslatedTitle"
                                    ElseIf (CurrentSettings.Group_Name_Applies_To = "Formatted Title") Then
                                        wtitle = "FormattedTitle"
                                    End If
                                End If
                                Try
                                    wtitle = CurrentNode.Attributes(wtitle).Value
                                Catch ex As Exception
                                    wtitle = row("AntTitle").ToString
                                End Try
                            End If
                            SearchFile(FileToScan, CurrentSettings.Movie_Scan_Path, CurrentNode.Attributes("Number").Value, wtitle)
                            If Not FileToScan.Length > 0 Then
                                DoScan = False
                            End If
                        End If

                        If DoScan = True Then
                            Dim NfoMyFilmsFilename As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileToScan), "MyFilms.nfo")
                            Dim NfoMovieFilename As String = System.IO.Path.ChangeExtension(FileToScan, "nfo")

                            Dim NfoMyFilmsFilenameShort As String = System.IO.Path.GetFileName(NfoMyFilmsFilename)
                            Dim NfoMovieFilenameShort As String = System.IO.Path.GetFileName(NfoMovieFilename)

                            Try
                                If _manualNfoFileHandling = "MyFilms.nfo" Then
                                    System.IO.File.Delete(NfoMyFilmsFilename)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO file '" & NfoMyFilmsFilenameShort & "' deleted for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                ElseIf _manualNfoFileHandling = "Movie Name" Then
                                    System.IO.File.Delete(NfoMovieFilename)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO file '" & NfoMovieFilenameShort & "' deleted for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                ElseIf _manualNfoFileHandling = "Both" Then
                                    System.IO.File.Delete(NfoMyFilmsFilename)
                                    System.IO.File.Delete(NfoMovieFilename)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "NFO files '" & NfoMyFilmsFilenameShort & ", " & NfoMovieFilenameShort & "' deleted for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                End If
                            Catch ex As Exception
                                bgwManualUpdate.ReportProgress(ProcessCounter, "ErrorEvent deleting NFO file(s) : " & ex.Message)
                            End Try
                        Else
                            bgwManualUpdate.ReportProgress(ProcessCounter, "NFO File(s) not deleted (File/Path Not Found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If

                    Case "Add IMDB to File"
                        Dim imdb As String = GetValue(CurrentNode, "IMDB_Id")
                        If (imdb Is Nothing)  Or (New Regex("" & "tt\d{7}" & "").Match(imdb).Success <> True) Then
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Cannot rename directory (no IMDB id found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            Dim FileNames As String() = New String(){}
                            Dim AllFilesPath As String = String.Empty
                            If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                                AllFilesPath = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                                FileNames  = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value.Split(";")
                            End If

                            ' check, if all files do exist            
                            DoScan = True
                            For Each fileName In From fileName1 In FileNames Where Not fileName1.Length > 0 Or File.Exists(fileName1) <> True
                                DoScan = False
                            Next

                            ' check, if imdb if already in filenam(s)
                            For Each fileName In From fileName1 In FileNames Where (New Regex("" & "tt\d{7}" & "").Match(fileName1).Success)
                                DoScan = False
                            Next

                            If DoScan = True Then
                                Dim NewAllFilesPath As String = ""

                                For Each fileName As String In FileNames
                                    Try
                                        Dim newFilename = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) & "{" & imdb & "}" & Path.GetExtension(fileName))
                                        My.Computer.FileSystem.RenameFile(fileName, newFilename)
                                        Dim filenameShort As String = Path.GetFileName(fileName)
                                        Dim newFilenameShort As String = Path.GetFileName(newFilename)
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Movie renamed: " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString & " from: '" & filenameShort & "' to '" & newFilenameShort & "'")
                                        If (NewAllFilesPath.Length > 0) Then
                                            NewAllFilesPath = NewAllFilesPath & ";"
                                        End If
                                        NewAllFilesPath = NewAllFilesPath & newFilename
                                    Catch ex As Exception
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "ErrorEvent renaming movie file '" & fileName & "' : " & ex.Message)
                                        DoScan = False
                                    End Try
                                Next

                                ' If the rename was successful, we need to update the source path in the DB
                                If DoScan Then
                                    SetValue(CurrentNode, CurrentSettings.Ant_Database_Source_Field, NewAllFilesPath)
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "DB updated (renamed Source): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString & " -> '" & NewAllFilesPath & "'")
                                End If
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Files not renamed (File/Path Not Found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        End If
                End Select
                ProcessCounter += 1
            Next

        End Sub

        Public Sub New()

        End Sub

    End Class

    Private Class SingleMovieUpdateClass

        Public XmlDoc As XmlDocument
        Public DoSave As Boolean = False

        Public Sub RunUpdate()
            ' Check, if it is a valid movie number
            Dim IsValidNumber As Boolean
            Dim Number As Integer
            IsValidNumber = Integer.TryParse(Form1.TextBox2.Text, Number)
            If IsValidNumber = False Then
                Return
            End If

            Dim CurrentNode As Xml.XmlNode
            CurrentNode = XmlDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & Form1.TextBox2.Text & "']")
            If CurrentNode Is Nothing Then
                Return
            End If

            ' Save current / old state to be able to revert ...
            Dim CurrentNodeOriginalValue As Xml.XmlNode = CurrentNode.CloneNode(True)

            Dim FileToScan As String = String.Empty
            Dim AllFilesPath As String = String.Empty
            If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
            End If

            If FileToScan.IndexOf(";") > 0 Then
                'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                AllFilesPath = FileToScan
                FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
            End If

            Dim wDirector As String = ""
            Dim wYear As String = ""
            Dim wIMDB_Id As String = ""
            If CurrentNode.Attributes(CurrentSettings.Master_Title).ToString.Length > 0 Then '            If row("AntTitle").ToString.Length > 0 Then
                If Not CurrentNode.Item("Director") Is Nothing Then
                    If (CurrentNode.Attributes("Director").ToString.Length > 0) Then
                        If (CurrentNode.Attributes("Director").ToString.IndexOf(",") > 0) Then
                            wDirector = CurrentNode.Attributes("Director").ToString.Substring(0, CurrentNode.Attributes("Director").ToString.IndexOf(","))
                        Else
                            wDirector = CurrentNode.Attributes("Director").ToString
                        End If
                    End If
                End If
                If Not CurrentNode.Item("Year") Is Nothing Then
                    If (CurrentNode.Attributes("Year").ToString.Length > 0) Then
                        wYear = CurrentNode.Attributes("Year").ToString
                    End If
                End If
                If Not CurrentNode.Item("IMDB_Id") Is Nothing Then
                    If (CurrentNode.Item("IMDB_Id").ToString.Length > 0) Then
                        wIMDB_Id = CurrentNode.Item("IMDB_Id").ToString
                    End If
                End If
            End If

            ' search Filename
            Dim wtitle As String = CurrentNode.Attributes(CurrentSettings.Master_Title).ToString
            If IsFileNeeded() = True Then
                'If My.Computer.FileSystem.FileExists(FileToScan) = False Then
                If (CurrentSettings.Folder_Name_Is_Group_Name) Then
                    If (CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles") Then
                        wtitle = "OriginalTitle"
                    Else
                        If (CurrentSettings.Group_Name_Applies_To = "Translated Title") Then
                            wtitle = "TranslatedTitle"
                        ElseIf (CurrentSettings.Group_Name_Applies_To = "Formatted Title") Then
                            wtitle = "FormattedTitle"
                        End If
                    End If
                    Try
                        wtitle = CurrentNode.Attributes(wtitle).Value
                    Catch ex As Exception
                        wtitle = CurrentNode.Attributes(CurrentSettings.Master_Title).ToString
                    End Try
                End If
            End If

            Dim Ant As New AntRecord()

            With Ant
                .XMLDoc = XmlDoc
                .XMLElement = CurrentNode
                .XMLFilePath = CurrentSettings.XML_File
                .FilePath = FileToScan
                .AllFilesPath = AllFilesPath
                If AllFilesPath.Length > 0 Then
                    .OverridePath = AllFilesPath
                Else
                    .OverridePath = FileToScan
                End If
                'File(Name)
                'Folder(Name)
                'Relative(Name)
                If FileToScan.Length > 0 Then ' first try to get groupname by default rules from filename
                    If FileToScan.ToString.Contains("\") Then
                        .GroupName = GetGroupName(FileToScan, CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                    End If
                Else
                    If wtitle.Length > 0 Then ' fallback to possibly existing group name
                        If wtitle.Contains("\") = True Then
                            .GroupName = wtitle.Substring(0, wtitle.LastIndexOf("\"))
                            'Console.WriteLine("-" & .GroupName.ToString & "-")
                        End If
                    End If
                End If
                'If CurrentNode.Attributes("MovieNumber") Then
                .ExcludeFile = CurrentSettings.Excluded_Movies_File
                .InternetLookupAlwaysPrompt = True
                .MasterTitle = CurrentSettings.Master_Title
                .InteractiveMode = True
                Dim ImagePath As String = CurrentSettings.XML_File.Substring(0, CurrentSettings.XML_File.LastIndexOf("\"))
                Dim ImagePrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString

                If (CurrentSettings.Use_Folder_Dot_Jpg = False And CurrentSettings.Image_Download_Filename_Prefix.Length > 0) Then
                    If ImagePrefix.LastIndexOf("\") > -1 Then 'Example : "foldername\" or "foldername\prefix_"
                        Dim PictureFolder As String = ImagePrefix.Substring(0, ImagePrefix.IndexOf("\"))
                        ImagePath = ImagePath & "\" & PictureFolder
                        If Not My.Computer.FileSystem.DirectoryExists(ImagePath) Then
                            My.Computer.FileSystem.CreateDirectory(ImagePath)
                        Else
                            'ImagePath = ImagePath & "\" & ImagePrefix
                        End If
                    End If
                End If

                Dim ImagePathTemp = System.IO.Path.GetTempPath()

                '.ImagePath = ImagePath
                .ImagePath = ImagePathTemp

                .InternetSearchHint = wDirector
                .InternetSearchHintYear = wYear
                .InternetSearchHintIMDB_Id = wIMDB_Id
                .ParserPath = CurrentSettings.Manual_Internet_Parser_Path
                .OnlyAddMissingData = False ' added for "add missing data" mode"
                .OnlyUpdateNonEmptyData = False
                .Dont_Ask_Interactive = False ' added for silent updates without asking user to choose movie on failed auto matches
                .UseInternetDataForLanguages = CurrentSettings.Use_InternetData_For_Languages
                .GrabberOverrideLanguage = CurrentSettings.Grabber_Override_Language
                .GrabberOverridePersonLimit = CurrentSettings.Grabber_Override_PersonLimit
                .GrabberOverrideTitleLimit = CurrentSettings.Grabber_Override_TitleLimit
                .GrabberOverrideGetRoles = CurrentSettings.Grabber_Override_GetRoles

                'Dim t As Thread
                't = New Thread(AddressOf Ant.ProcessFile)
                't.Start(AntRecord.ProcessModeNames.Update)
                't.Join()
                ''If t.Join(500) Then
                ''    MsgBox("error in thread execution")
                ''End If

                .ProcessFile(AntRecord.ProcessModeNames.Update)

                Dim UpdateMovieDialog As New frmMovieUpdate()

                With UpdateMovieDialog
                    .PictureBoxOld.ImageLocation = ""
                    .PictureBoxNew.ImageLocation = ""
                    Dim item As Object
                    Dim FieldName As String
                    Dim FieldChecked As Boolean
                    Dim ValueOld As String
                    Dim ValueNew As String

                    For Each item In Form1.cbDatabaseFields.Items
                        FieldName = item.ToString
                        FieldChecked = Form1.cbDatabaseFields.GetItemChecked(Form1.cbDatabaseFields.Items.IndexOf(FieldName))
                        Try
                            ValueOld = GetValue(CurrentNodeOriginalValue, FieldName) 'ValueOld = CurrentNodeOriginalValue.Attributes(FieldName).Value
                            If Not ValueOld Is Nothing Then
                                If ValueOld.StartsWith("ErrorEvent :") Then
                                    ValueOld = Nothing
                                End If
                            End If
                            ValueNew = GetValue(CurrentNode, FieldName) 'ValueNew = Ant.XMLElement.Attributes(FieldName).Value
                            If Not ValueNew Is Nothing Then
                                If ValueNew.StartsWith("ErrorEvent :") Then
                                    ValueNew = Nothing
                                End If
                            End If

                            If FieldName = "Picture" Then
                                Dim OldImage As String = ValueOld
                                If ValueOld Is Nothing Then
                                    OldImage = ""
                                ElseIf ValueOld.Contains("\") Then
                                    OldImage = ValueOld.Substring(ValueOld.LastIndexOf("\") + 1)
                                End If

                                Dim NewImage As String = ValueNew
                                If ValueNew Is Nothing Then
                                    NewImage = ""
                                ElseIf ValueNew.Contains("\") Then
                                    NewImage = ValueNew.Substring(ValueNew.LastIndexOf("\") + 1)
                                End If

                                If Not File.Exists(Path.Combine(ImagePathTemp, NewImage)) Then
                                    NewImage = Nothing
                                    ValueNew = ""
                                End If
                                .PictureBoxOld.ImageLocation = If((Path.Combine(ImagePath, OldImage)), "")
                                .PictureBoxNew.ImageLocation = If((Path.Combine(ImagePathTemp, NewImage)), "")
                            End If

                            .DgvUpdateMovie.Rows.Add(New Object() {FieldChecked, FieldName, ValueOld, ValueNew})
                        Catch ex As Exception
                            MsgBox("Error adding data ('" + FieldName + "') to Movie Update Dialog: " + ex.Message, MsgBoxStyle.OkOnly)
                        End Try
                    Next

                    If .ShowDialog = Windows.Forms.DialogResult.OK Then
                        DoSave = True
                        For i As Integer = 0 To .DgvUpdateMovie.RowCount - 1
                            Dim itemName As String = ""
                            Dim itemValue As String = ""
                            Try
                                itemName = .DgvUpdateMovie(1, i).Value
                                If .DgvUpdateMovie(0, i).Value = True Then
                                    itemValue = .DgvUpdateMovie(3, i).Value

                                    Select Case itemName
                                        Case "Picture"
                                            Dim TmpImage As String = itemValue
                                            If itemValue.Contains("\") Then
                                                TmpImage = itemValue.Substring(itemValue.LastIndexOf("\") + 1)
                                            End If
                                            If File.Exists(Path.Combine(ImagePathTemp, TmpImage)) Then
                                                File.Copy(Path.Combine(ImagePathTemp, TmpImage), Path.Combine(ImagePath, TmpImage), True)
                                                Thread.Sleep(20)
                                                File.Delete(Path.Combine(ImagePathTemp, TmpImage))
                                            Else
                                                itemValue = Nothing
                                            End If
                                        Case "Rating", "Picture", "Date"
                                            If String.IsNullOrEmpty(itemValue) Then
                                                itemValue = Nothing
                                            End If
                                    End Select
                                Else
                                    itemValue = .DgvUpdateMovie(2, i).Value
                                End If
                                'If itemName = "Date" Then
                                '    Try
                                '        itemValue = String.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(itemValue))
                                '    Catch ex As Exception
                                '        itemValue = Nothing 'String.Format("{0:yyyy/MM/dd}", DateTime.Now)
                                '    End Try
                                'End If
                            Catch ex As Exception
                                MsgBox("Exception updating data ('" + itemName + "') to Movie: " + ex.Message, MsgBoxStyle.OkOnly)
                            End Try
                            Try
                                If CurrentNode.Attributes(itemName) IsNot Nothing Then

                                    If itemValue IsNot Nothing Then
                                        CurrentNode.Attributes(itemName).Value = itemValue
                                    Else
                                        RemoveValue(CurrentNode, itemName)
                                    End If
                                Else
                                    If CurrentNode.Item("CustomFields") IsNot Nothing Then
                                        Dim customfieldselement As Xml.XmlElement = CurrentNode.Item("CustomFields")
                                        If customfieldselement.Attributes(itemName) IsNot Nothing Then
                                            customfieldselement.Attributes(itemName).Value = itemValue
                                        End If
                                    End If
                                    If CurrentNode.Item(itemName) IsNot Nothing Then
                                        CurrentNode.Item(itemName).InnerText = itemValue
                                    End If
                                End If
                            Catch ex As Exception
                                MsgBox("Exception updating data ('" + itemName + "') to Movie: " + ex.Message + ", " + ex.StackTrace, MsgBoxStyle.OkOnly)
                            End Try
                        Next
                    Else
                        CurrentNode = CurrentNodeOriginalValue
                    End If
                End With

                If DoSave Then
                    Ant.SaveProgress()
                End If
                'If frmMovieUpdate.DialogResult = DialogResult.OK Then
                '    .SaveProgress()
                'End If
            End With

            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
                bgwManualMovieUpdate.ReportProgress(100, Ant.LastOutputMessage)
            Else
                If (CurrentNode.Attributes("Number") Is Nothing) Then
                    bgwManualMovieUpdate.ReportProgress(100, "File Scanned for Data : " & CurrentNode.Attributes(CurrentSettings.Master_Title).ToString & " | " & Ant.LastOutputMessage)
                Else
                    bgwManualMovieUpdate.ReportProgress(100, "File Scanned for Data : " & CurrentNode.Attributes("Number").Value & " | " & CurrentNode.Attributes(CurrentSettings.Master_Title).Value.ToString & " | " & Ant.LastOutputMessage)
                End If
            End If

        End Sub

        Public Sub New()

        End Sub

    End Class

    Private Shared Sub bgwManualUpdate_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwManualUpdate.DoWork
        Dim ManualUpdateObject As ManualUpdateClass = CType(e.Argument, ManualUpdateClass)
        'e.Result = ManualUpdateObject.RunUpdate()
        ManualUpdateObject.RunUpdate()
    End Sub

    Private Shared Sub bgwManualMovieUpdate_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwManualMovieUpdate.DoWork
        Dim SingleMovieUpdateObject As SingleMovieUpdateClass = CType(e.Argument, SingleMovieUpdateClass)
        SingleMovieUpdateObject.RunUpdate()
    End Sub

    Private Shared Sub bgwManualUpdate_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwManualUpdate.RunWorkerCompleted

        If e.Error IsNot Nothing Then
            'If e.ErrorEvent.Message.ToString <> "" Then
            LogEvent("ErrorEvent : " & e.Error.Message.ToLower, EventLogLevel.ErrorEvent)
            'End If
        End If


        If _OperationCancelled = True Then
            If MsgBox("Save work done so far?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                LogEvent("Operation Cancelled.", EventLogLevel.ErrorEvent)
                Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
                Try
                    My.Computer.FileSystem.CopyFile(_tempXMLBackupFile, CurrentSettings.XML_File, True)
                    My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Catch ex As Exception

                End Try
                Exit Sub
            Else
                LogEvent("Operation Cancelled - Save Continuing", EventLogLevel.ErrorEvent)
            End If
        End If

        Form1.txtConfigFilePath_TextChanged(sender, e) ' added to force refresh of View Collection Tab

        bgwManualUpdate_PostProcessing()

    End Sub
    Private Shared Sub bgwManualMovieUpdate_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwManualMovieUpdate.RunWorkerCompleted

        If e.Error IsNot Nothing Then
            LogEvent("ErrorEvent : " & e.Error.Message.ToLower, EventLogLevel.ErrorEvent)
        End If

        If _OperationCancelled = True Then
            If MsgBox("Save work done so far?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                LogEvent("Operation Cancelled.", EventLogLevel.ErrorEvent)
                Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
                Try
                    My.Computer.FileSystem.CopyFile(_tempXMLBackupFile, CurrentSettings.XML_File, True)
                    My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Catch ex As Exception

                End Try
                Exit Sub
            Else
                LogEvent("Operation Cancelled - Save Continuing", EventLogLevel.ErrorEvent)
            End If
        End If

        Form1.txtConfigFilePath_TextChanged(sender, e) ' added to force refresh of View Collection Tab

        'XMLDoc.Save(_ManualXMLPath)
        Dim xmlFile As New FileStream(_ManualXMLPath, FileMode.Open, FileAccess.Write, FileShare.Read)
        xmlFile.SetLength(0)
        XMLDoc.Save(xmlFile)
        xmlFile.Close()

        'Using s As Stream = File.OpenWrite(CurrentSettings.XML_File)
        '    XMLDoc.Save(s)
        '    s.Close()
        'End Using

        Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
        LogEvent("Manual Single Movie Update Complete.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)
    End Sub

    Private Shared Sub bgwManualUpdate_PostProcessing()
        'XMLDoc.Save(_ManualXMLPath)
        Dim xmlFile As New FileStream(_ManualXMLPath, FileMode.Open, FileAccess.Write, FileShare.Read)
        xmlFile.SetLength(0)
        XMLDoc.Save(xmlFile)
        xmlFile.Close()

        'Using s As Stream = File.OpenWrite(CurrentSettings.XML_File)
        '    XMLDoc.Save(s)
        '    s.Close()
        'End Using

        Form1.btnManualCancel.Enabled = False
        Form1.btnManualApplyChanges.Enabled = False
        Form1.btnManualDoTest.Enabled = True
        Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
        LogEvent("Manual Update Process Complete.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

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
        Form1.dgExcludedFileStrings.DataSource = dtFiles
        Form1.dgExcludedFileStrings.Columns(0).Width = Form1.dgExcludedFileStrings.Width - Form1.dgExcludedFileStrings.RowHeadersWidth - 20

        If CurrentSettings.Execute_Program = False Or CurrentSettings.Execute_Program_Path = "" Then
            MsgBox("Process Complete", MsgBoxStyle.Information)
        ElseIf CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
            If CurrentSettings.Execute_Only_For_Orphans = False Then
                Try
                    System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                Catch ex As Exception

                End Try
            Else
                If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception

                    End Try

                End If
            End If
        End If
    End Sub

    Private Shared Sub bgwManualUpdate_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bgwManualUpdate.ProgressChanged

        'Console.WriteLine(e.UserState)
        'Console.WriteLine("Min: " & Form1.ToolStripProgressBar.Minimum.ToString & ", Max : " & Form1.ToolStripProgressBar.Maximum.ToString & ", Value : " & Form1.ToolStripProgressBar.Value.ToString)
        If Form1.ToolStripProgressBar.Value < Form1.ToolStripProgressBar.Maximum Then
            Form1.ToolStripProgressBar.Value += 1
            Form1.ToolStripProgressMessage.Text = "status: " & Form1.ToolStripProgressBar.Value.ToString & " of " & Form1.ToolStripProgressBar.Maximum.ToString & " total movie(s)"
        End If
        'The e.UserState includes messages from the background process.  In this case, it's the 'File Imported - Filename' message.
        LogEvent(e.UserState, EventLogLevel.Informational)

    End Sub

    Private Shared Sub bgwManualMovieUpdate_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bgwManualMovieUpdate.ProgressChanged

        'Console.WriteLine(e.UserState)
        'Console.WriteLine("Min: " & Form1.ToolStripProgressBar.Minimum.ToString & ", Max : " & Form1.ToolStripProgressBar.Maximum.ToString & ", Value : " & Form1.ToolStripProgressBar.Value.ToString)
        If Form1.ToolStripProgressBar.Value < Form1.ToolStripProgressBar.Maximum Then
            Form1.ToolStripProgressBar.Value += 10
            Form1.ToolStripProgressMessage.Text = "status: " & Form1.ToolStripProgressBar.Value.ToString & " of " & Form1.ToolStripProgressBar.Maximum.ToString & " total movie(s)"
        End If
        'The e.UserState includes messages from the background process.  In this case, it's the 'File Imported - Filename' message.
        LogEvent(e.UserState, EventLogLevel.Informational)

    End Sub

    Public Sub bgwManualUpdate_Cancel()
        bgwManualUpdate.CancelAsync()
        Form1.btnManualDoTest.Enabled = True
        Form1.btnManualApplyChanges.Enabled = True
        Form1.btnManualCancel.Enabled = False

        _OperationCancelled = True

        LogEvent("Halting Operation by user request - Please Wait.", EventLogLevel.ErrorEvent)
    End Sub

    Public Sub bgwManualMovieUpdate_Cancel()
        bgwManualMovieUpdate.CancelAsync()

        _OperationCancelled = True

        LogEvent("Halting Operation by user request - Please Wait.", EventLogLevel.ErrorEvent)
    End Sub

#End Region

#Region "Main Import Run"
    Public Sub ProcessXML(Optional ByVal XMLFilePath As String = "")
        Dim textReader As Xml.XmlTextReader
        Dim CurrentMovieTitle As String = ""
        Dim CurrentMovieNumber As Integer
        Dim objReader As System.IO.StreamReader
        Dim PlaylistEntry As String = String.Empty

        Try
            If ds.Tables("tblXML") IsNot Nothing Then
                ds.Tables("tblXML").Clear()
            End If

            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Parsing XML File", EventLogLevel.ImportantEvent)
            LogEvent("===================================================================================================", EventLogLevel.Informational)


            'If XMLFilePath = "" Then
            textReader = New Xml.XmlTextReader(CurrentSettings.XML_File)
            'Else
            'textReader = New Xml.XmlTextReader(XMLFilePath)
            'End If
            While textReader.Read()
                If textReader.Name = "Movie" Then
                    CurrentMovieTitle = textReader.GetAttribute(CurrentSettings.Ant_Database_Source_Field)
                    CurrentMovieNumber = textReader.GetAttribute("Number")
                    If Not CurrentMovieTitle Is Nothing Then
                        If CurrentMovieTitle.ToLower.EndsWith(".m3u") = True Then
                            If File.Exists(CurrentMovieTitle) Then
                                objReader = New System.IO.StreamReader(CurrentMovieTitle)
                                Do While objReader.Peek() <> -1
                                    PlaylistEntry = objReader.ReadLine()
                                    If PlaylistEntry <> String.Empty Then
                                        '
                                    End If
                                Loop

                            Else
                                'Referenced .m3u file doesn't exist or not present.
                            End If
                        Else
                            'Check for multi-valued entries:
                            'If CurrentMovieTitle.IndexOf(";") > -1 Then
                            Dim TitleList() As String
                            TitleList = CurrentMovieTitle.Split(";")
                            Dim FolderList() As String
                            FolderList = CurrentSettings.Movie_Scan_Path.Split(";")
                            Dim myEnumerator As System.Collections.IEnumerator = TitleList.GetEnumerator()
                            Dim shortitle As String = String.Empty
                            While myEnumerator.MoveNext()
                                shortitle = myEnumerator.Current.ToString
                                For Each folder As String In FolderList
                                    shortitle = StripPathFromFile(myEnumerator.Current.ToString, folder, CurrentSettings.Override_Path)
                                    If Not shortitle = myEnumerator.Current.ToString Then
                                        Exit For
                                    End If
                                Next

                                ds.Tables("tblXML").Rows.Add(New Object() {CurrentMovieNumber, myEnumerator.Current.ToString, shortitle})
                                LogEvent("  Entry Found - " & myEnumerator.Current.ToString, EventLogLevel.Informational)
                            End While
                            TitleList = Nothing
                        End If
                    End If
                End If
            End While

            If Not (textReader Is Nothing) Then
                textReader.Close()
            End If

            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Parsing XML File - Complete - " & ds.Tables("tblXML").Rows.Count.ToString & " entries found.", EventLogLevel.ImportantEvent)
            LogEvent("===================================================================================================", EventLogLevel.Informational)
        Catch ex As Exception
            LogEvent("ErrorEvent : Cannot parse XML file", EventLogLevel.ErrorEvent)
        End Try

    End Sub

    Public Sub ProcessMovieFolder()
        'Sub to enumerate all files in the given MoviePath location.

        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundNonMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundTrailerFiles") IsNot Nothing Then
            ds.Tables("tblFoundTrailerFiles").Clear()
        End If

        Dim XMLExclTable As New Hashtable ' "always ignore" movies
        If (File.Exists(CurrentSettings.Excluded_Movies_File)) Then
            Dim sr As StreamReader = File.OpenText(CurrentSettings.Excluded_Movies_File)
            Dim i As Integer = 0
            Do While sr.Peek() >= 0
                XMLExclTable.Add(i, sr.ReadLine.ToLower)
                i += 1
            Loop
            sr.Close()
        End If

        Dim FoundFileName As String
        Dim ValidMediaExtensions As String() = CurrentSettings.File_Types_Media.Split(";")
        Dim ValidNonMediaExtensions As String() = CurrentSettings.File_Types_Non_Media.Split(";")
        Dim ValidTrailerExtensions As String() = CurrentSettings.File_Types_Trailer.Split(";") ' Added for trailer detection
        Dim row As DataRow

        Dim CurrentMoviePath As String
        For Each CurrentMoviePath In CurrentSettings.Movie_Scan_Path.Split(";")

            Dim dir As New DirectoryInfo(CurrentMoviePath)
            If Not dir.Exists Then
                LogEvent("ErrorEvent : Cannot access folder '" + CurrentMoviePath.ToString + "'", EventLogLevel.ErrorEvent)
            Else
                If Not CurrentMoviePath.EndsWith("\") = True Then
                    CurrentMoviePath = CurrentMoviePath & "\"
                End If

                LogEvent("Processing Movie Folder : " & CurrentMoviePath.ToString, EventLogLevel.ImportantEvent)
                LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)

                LogEvent("Scanning Files ...", EventLogLevel.ImportantEvent)
                Dim blah As New FileFolderEnum()
                'blah.Root = CurrentMoviePath
                blah.ExcludedFiles = CurrentSettings.Excluded_File_Strings
                blah.ExcludedFolders = CurrentSettings.Excluded_Folder_Strings
                blah.GetFiles(CurrentMoviePath)
                LogEvent("Scanning Files done - TotalFiles: " & blah.TotalFiles.ToString & ", TotalSize: " & blah.TotalSize.ToString & ", TotalFolders: " & blah.TotalFolders.ToString, EventLogLevel.ImportantEvent)
                LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
                LogEvent("Identifying Files ...", EventLogLevel.ImportantEvent)
                For Each foundFile As String In blah.Files

                    If (XMLExclTable.ContainsValue(foundFile.ToLower) = False) Then ' if not in "always ignore" file
                        'Check for match against movie file types:
                        Try
                            'I took out the Override path here so every file gets loaded into tblFoundMovieFiles properly.  Override path handling moved to Processor
                            FoundFileName = StripPathFromFile(foundFile, CurrentMoviePath)

                            'File Handling - compare extension to known media filetypes (trailer or movies)
                            Dim extension As String = foundFile.Substring(InStrRev(foundFile, ".")).ToLower
                            If Array.Exists(ValidMediaExtensions, Function(s) s.ToString.ToLower.Equals(extension)) = True Then
                                'Check, if it's a trailer
                                Dim isTrailer As Boolean = ValidTrailerExtensions.Any(Function(TrailerProp) FoundFileName.ToLower.Contains(TrailerProp.ToLower))
                                If isTrailer = True Then
                                    LogEvent("  File Found (trailer) - " & FoundFileName, EventLogLevel.Informational)

                                    row = ds.Tables("tblFoundTrailerFiles").NewRow()
                                    row("FileName") = FoundFileName
                                    row("FilePath") = CurrentMoviePath
                                    ds.Tables("tblFoundTrailerFiles").Rows.Add(row)
                                Else
                                    LogEvent("  File Found (movie) - " & FoundFileName, EventLogLevel.Informational)

                                    row = ds.Tables("tblFoundMediaFiles").NewRow()
                                    row("FileName") = FoundFileName
                                    row("FilePath") = CurrentMoviePath
                                    ds.Tables("tblFoundMediaFiles").Rows.Add(row)
                                End If

                            ElseIf Array.Exists(ValidNonMediaExtensions, Function(s) s.ToString.ToLower.Equals(extension)) = True Then 'If Array.IndexOf(ValidNonMediaExtensions, foundFile.Substring(InStrRev(foundFile, "."))) >= 0 Then
                                'Check for match against non-movie file types:
                                LogEvent("  File Found (NonMedia) - " & FoundFileName, EventLogLevel.Informational)

                                row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                                row("FileName") = FoundFileName
                                row("FilePath") = CurrentMoviePath
                                ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)

                            ElseIf CurrentSettings.Scan_For_DVD_Folders = True Then
                                'Finally special handling to check for DVD images in folders.
                                If Right(FoundFileName, 12).ToLower = "video_ts.ifo" Then
                                    LogEvent("  File Found (DVD folder) - " & FoundFileName, EventLogLevel.Informational)

                                    row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                                    row("FileName") = FoundFileName
                                    row("FilePath") = CurrentMoviePath
                                    ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)
                                End If
                                'Finally special handling to check for BR images in folders.
                                If Right(FoundFileName, 10).ToLower = "index.bdmv" And Not Right(FoundFileName, 18).ToLower = "\backup\index.bdmv" Then
                                    LogEvent("  File Found (BR folder) - " & FoundFileName, EventLogLevel.Informational)

                                    row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                                    row("FileName") = FoundFileName
                                    row("FilePath") = CurrentMoviePath
                                    ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)
                                End If
                            Else
                                'LogEvent("  File Excluded - " & FoundFileName, EventLogLevel.Informational)
                            End If
                        Catch ex As Exception
                            LogEvent("ErrorEvent : " & ex.Message & ", " & ex.InnerException.ToString, EventLogLevel.ErrorEvent)
                        End Try
                    Else
                        LogEvent("  File Excluded - " & foundFile & "  - (always ignore)", EventLogLevel.Informational)
                    End If

                Next

            End If
        Next
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Movie Folder - Done - " & CountFilesFound.ToString & " files found.", EventLogLevel.ImportantEvent)
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent(" Results: " & CountMediaFilesFound.ToString & " media files found.", EventLogLevel.ImportantEvent)
        LogEvent(" Results: " & CountNonMediaFilesFound.ToString & " non media files found.", EventLogLevel.ImportantEvent)
        LogEvent(" Results: " & CountTrailerFilesFound.ToString & " trailer files found.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)
    End Sub


    Public Sub ProcessMovieFolderForSingleMovie(ByVal filenames As List(Of String))
        'Sub to enumerate file in the given MoviePath location - comes from watcher ...

        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundNonMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundTrailerFiles") IsNot Nothing Then
            ds.Tables("tblFoundTrailerFiles").Clear()
        End If

        Dim XMLExclTable As New Hashtable ' "always ignore" movies
        If (File.Exists(CurrentSettings.Excluded_Movies_File)) Then
            Dim sr As StreamReader = File.OpenText(CurrentSettings.Excluded_Movies_File)
            Dim i As Integer = 0
            Do While sr.Peek() >= 0
                XMLExclTable.Add(i, sr.ReadLine.ToLower)
                i += 1
            Loop
            sr.Close()
        End If

        Dim FoundFileName As String
        Dim ValidMediaExtensions As String() = CurrentSettings.File_Types_Media.Split(";")
        Dim ValidNonMediaExtensions As String() = CurrentSettings.File_Types_Non_Media.Split(";")
        Dim ValidTrailerExtensions As String() = CurrentSettings.File_Types_Trailer.Split(";") ' Added for trailer detection
        Dim row As DataRow

        Dim CurrentMoviePath As String

        LogEvent("Identifying Files from Watcher ...", EventLogLevel.ImportantEvent)
        For Each foundFile As String In filenames

            CurrentMoviePath = System.IO.Path.GetDirectoryName(foundFile)
            If Not CurrentMoviePath.EndsWith("\") = True Then
                CurrentMoviePath = CurrentMoviePath & "\"
            End If

            If (XMLExclTable.ContainsValue(foundFile.ToLower) = False) Then ' if not in "always ignore" file
                'Check for match against movie file types:
                Try
                    'I took out the Override path here so every file gets loaded into tblFoundMovieFiles properly.  Override path handling moved to Processor
                    FoundFileName = StripPathFromFile(foundFile, CurrentMoviePath)

                    'File Handling - compare extension to known media filetypes (trailer or movies)
                    Dim extension As String = foundFile.Substring(InStrRev(foundFile, ".")).ToLower
                    If Array.Exists(ValidMediaExtensions, Function(s) s.ToString.ToLower.Equals(extension)) = True Then
                        'Check, if it's a trailer
                        Dim isTrailer As Boolean = 
                            (ValidTrailerExtensions.Any(Function(TrailerProp) FoundFileName.ToLower.Contains(TrailerProp.ToLower))) OrElse 
                            ((From s In ValidTrailerExtensions Select r = new Regex(s, RegexOptions.IgnoreCase) Select r.Match(FoundFileName)).Any(Function(m) m.Success = True))

                        If isTrailer = True Then
                            LogEvent("  File Found (trailer) - " & FoundFileName, EventLogLevel.Informational)

                            row = ds.Tables("tblFoundTrailerFiles").NewRow()
                            row("FileName") = FoundFileName
                            row("FilePath") = CurrentMoviePath
                            ds.Tables("tblFoundTrailerFiles").Rows.Add(row)
                        Else
                            LogEvent("  File Found (movie) - " & FoundFileName, EventLogLevel.Informational)

                            row = ds.Tables("tblFoundMediaFiles").NewRow()
                            row("FileName") = FoundFileName
                            row("FilePath") = CurrentMoviePath
                            ds.Tables("tblFoundMediaFiles").Rows.Add(row)
                        End If

                    ElseIf Array.Exists(ValidNonMediaExtensions, Function(s) s.ToString.ToLower.Equals(extension)) = True Then 'If Array.IndexOf(ValidNonMediaExtensions, foundFile.Substring(InStrRev(foundFile, "."))) >= 0 Then
                        'Check for match against non-movie file types:
                        LogEvent("  File Found (NonMedia) - " & FoundFileName, EventLogLevel.Informational)

                        row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                        row("FileName") = FoundFileName
                        row("FilePath") = CurrentMoviePath
                        ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)

                    ElseIf CurrentSettings.Scan_For_DVD_Folders = True Then
                        'Finally special handling to check for DVD images in folders.
                        If Right(FoundFileName, 12).ToLower = "video_ts.ifo" Then
                            LogEvent("  File Found (DVD folder) - " & FoundFileName, EventLogLevel.Informational)

                            row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                            row("FileName") = FoundFileName
                            row("FilePath") = CurrentMoviePath
                            ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)
                        End If
                        'Finally special handling to check for BR images in folders.
                        If Right(FoundFileName, 10).ToLower = "index.bdmv" And Not Right(FoundFileName, 18).ToLower = "\backup\index.bdmv" Then
                            LogEvent("  File Found (BR folder) - " & FoundFileName, EventLogLevel.Informational)

                            row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                            row("FileName") = FoundFileName
                            row("FilePath") = CurrentMoviePath
                            ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)
                        End If
                    Else
                        'LogEvent("  File Excluded - " & FoundFileName, EventLogLevel.Informational)
                    End If
                Catch ex As Exception
                    LogEvent("ErrorEvent : " & ex.Message & ", " & ex.InnerException.ToString, EventLogLevel.ErrorEvent)
                End Try
            Else
                LogEvent("  File Excluded - " & foundFile & "  - (always ignore)", EventLogLevel.Informational)
            End If

        Next
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing File(s) from Watcher Done - " & CountFilesFound.ToString & " files found.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)
    End Sub

    Public Sub ProcessOrphanFiles()

        Dim MovedFilesCount As Integer = 0

        'Whenever this is called, ensure the OrphanedMovies tables are cleared out first.
        If ds.Tables("tblOrphanedMediaFiles").Rows.Count > 0 Then
            ds.Tables("tblOrphanedMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedNonMediaFiles").Rows.Count > 0 Then
            ds.Tables("tblOrphanedNonMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedTrailerMediaFiles").Rows.Count > 0 Then ' Orphaned Trailerfiles
            ds.Tables("tblOrphanedTrailerMediaFiles").Clear()
        End If
        If ds.Tables("tblMultiPartFiles").Rows.Count > 0 Then
            ds.Tables("tblMultiPartFiles").Clear()
        End If

        LogEvent("Finding Orphaned Movies (not in XML file)", EventLogLevel.ImportantEvent)
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)

        '_NextAntID = fnGetNextMovieID(_XMLFilePath)

        Dim dvXML As New DataView
        dvXML = ds.Tables("tblXML").DefaultView
        dvXML.Sort = "AntPath"

        Dim dvOrphanedMediaFiles As New DataView
        dvOrphanedMediaFiles = ds.Tables("tblOrphanedMediaFiles").DefaultView
        dvOrphanedMediaFiles.Sort = "FileName"

        Dim dvFoundMediaFiles As New DataView
        dvFoundMediaFiles = ds.Tables("tblFoundMediaFiles").DefaultView
        dvFoundMediaFiles.Sort = "FileName"

        Dim SplitText As New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        Dim m As Match

        Dim row As DataRow
        Dim CutText As New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        Dim SearchName As String = String.Empty

        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            For Each row In ds.Tables("tblFoundMediaFiles").Rows
                m = SplitText.Match(row("FileName"))
                Dim wfile As String = String.Empty
                Dim tfile As String = String.Empty
                Dim tpath As String = String.Empty
                If CurrentSettings.Store_Short_Names_Only Then
                    wfile = row("FileName")
                    If wfile.Contains("\") Then
                        wfile = wfile.Substring(wfile.LastIndexOf("\") + 1)
                    End If
                Else
                    If CurrentSettings.Override_Path <> "" Then
                        wfile = CurrentSettings.Override_Path + row("FileName")
                    Else
                        wfile = row("FilePath") + row("FileName")
                    End If
                End If
                ' for debugging - to be removed
                tfile = row("FileName")
                tpath = row("FilePath")

                Dim FileMoved As Boolean = False

                If dvXML.Find(wfile) = -1 Then 'This is case-sensitive!

                    'Found Media Files Table
                    '("tblFoundMediaFiles")
                    'column.ColumnName = "FileName" (string)
                    'column.ColumnName = "FilePath" (string)

                    '("tblOrphanedAntRecords")
                    'column.ColumnName = "AntID" (int32)
                    'column.ColumnName = "PhysicalPath" (string)
                    'column.ColumnName = "FileName" (string)

                    '("tblOrphanedMediaFiles")
                    'column.ColumnName = "AntID" (int32)
                    'column.ColumnName = "PhysicalPath"
                    'column.ColumnName = "VirtualPath"
                    'column.ColumnName = "FileName"
                    'column.ColumnName = "Moved" (boolean)
                    'column.ColumnName = "GroupName"

                    'XML Table
                    '("tblXML")
                    'column.ColumnName = "AntID" (int32)
                    'column.ColumnName = "AntPath" (string)
                    'column.ColumnName = "AntShortPath" (string)

                    'File not already in XML - check, if moved or add it.
                    If dvOrphanedMediaFiles.Find(row("FileName")) = -1 Then
                        dvXML.Sort = "AntShortPath"
                        If dvXML.Find(row("FileName")) = -1 Then
                            ' Shortname not found - might be new or might have moved.
                            For Each row2 As DataRow In ds.Tables("tblXML").Rows
                                'take just the filename without any path:

                                SearchName = row2("AntPath").ToString.Substring(row2("AntPath").ToString.LastIndexOf("\") + 1)
                                If SearchName = wfile.Substring(wfile.LastIndexOf("\") + 1) Then
                                    'Match found - check, if file already used by existing record:
                                    'dvXML.Sort = "AntPath"
                                    'If wfile = row2("AntPath") Or dvXML.Find(row2("AntPath")) = -1 Then ' if this file is not already in an existing record or the record is the one to be moved ...
                                    dvFoundMediaFiles.Sort = "FileName"
                                    If dvFoundMediaFiles.Find(row2("AntShortPath")) = -1 Then ' if this file is not already in an existing record or the record is the one to be moved ...
                                        ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {row2("AntID"), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                                        LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                                        MovedFilesCount += 1
                                        FileMoved = True
                                        Exit For
                                    End If
                                End If
                            Next
                            If FileMoved = False Then
                                'If dvXML.Find(SearchName) > -1 Then
                                'Match found for short name - file moved.
                                'ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                                'LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                                'MovedFilesCount += 1
                                'Else
                                ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {0, row("FilePath"), CurrentSettings.Override_Path, row("FileName"), False})
                                LogEvent("  Orphaned File : " + row("FileName"), EventLogLevel.Informational)
                            End If
                            FileMoved = False
                        Else
                            ' short name found file moved
                            ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                            LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                            MovedFilesCount += 1
                        End If
                        If m.Success Then
                            ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {row("FilePath"), CurrentSettings.Override_Path, row("FileName"), CutText.Replace(row("FileName"), "")})
                        End If
                        dvXML.Sort = "AntPath"
                    End If
                End If
            Next row
        End If

        Dim dvOrphanedNonMediaFiles As New DataView
        dvOrphanedNonMediaFiles = ds.Tables("tblOrphanedNonMediaFiles").DefaultView
        dvOrphanedNonMediaFiles.Sort = "FileName"
        dvXML.Sort = "AntPath"

        'Repeat for non-movie types.
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            For Each row In ds.Tables("tblFoundNonMediaFiles").Rows
                m = SplitText.Match(row("FileName"))
                Dim wfile As String = String.Empty
                If CurrentSettings.Store_Short_Names_Only Then
                    wfile = row("FileName")
                    If wfile.Contains("\") Then
                        wfile = wfile.Substring(wfile.LastIndexOf("\") + 1)
                    End If
                Else
                    If CurrentSettings.Override_Path <> "" Then
                        wfile = CurrentSettings.Override_Path + row("FileName")
                    Else
                        wfile = row("FilePath") + row("FileName")
                    End If
                End If
                If dvXML.Find(wfile) = -1 Then
                    If dvOrphanedNonMediaFiles.Find(row("FileName")) = -1 Then
                        dvXML.Sort = "AntShortPath"
                        If dvXML.Find(row("FileName")) = -1 Then
                            ' Shortname not found new file
                            ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {0, row("FilePath"), CurrentSettings.Override_Path, row("FileName"), False})
                            'ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {row("FilePath"), _OverridePath, row("FileName"), False})
                            '_NextAntID += 1
                            LogEvent("  Orphaned File : " + row("FileName"), EventLogLevel.Informational)
                        Else
                            ' short name found file moved
                            MovedFilesCount += 1
                            ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                            'ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {row("FilePath"), _OverridePath, row("FileName"), True})
                            LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                        End If
                        If m.Success Then
                            'ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {_NextAntID, row("FilePath"), _OverridePath, row("FileName"), CutText.Replace(row("FileName"), "")})
                            ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {row("FilePath"), CurrentSettings.Override_Path, row("FileName"), CutText.Replace(row("FileName"), "")})
                        End If
                        dvXML.Sort = "AntPath"
                    End If
                End If
            Next row
        End If

        If CurrentSettings.Folder_Name_Is_Group_Name = True Then
            'Scan through the new tables and check for group names (multiple movies in a folder together)
            Dim FolderNames As New Hashtable
            Dim GroupNames As New Hashtable
            Dim CurrentFoldername As String

            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                Dim IsMultiPart As Boolean = False
                For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"), CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                        'If FolderNames.ContainsValue(CurrentFoldername) Then
                        '    'Already there - this is a group name, unless it's a multi-part file:
                        '    m = SplitText.Match(row("FileName"))
                        '    If Not m.Success Then
                        'It's not a multi-part, so it's a group name.  Add it.
                        GroupNames.Add(row("FileName"), CurrentFoldername)
                        'End If
                        '    Else
                        ''Not there - add to foldernames:
                        'FolderNames.Add(row("FileName"), CurrentFoldername)
                        '    End If
                    End If
                Next
                'Repeat to catch the missing ones (bad technique but it should work!)
                'For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                '    If row("FileName").ToString.Contains("\") Then
                '        CurrentFoldername = GetGroupName(row("FileName"))
                '        If GroupNames.ContainsValue(CurrentFoldername) Then
                '            'Make sure this entry is in there too!
                '            If Not GroupNames.ContainsKey(row("FileName")) Then
                '                GroupNames.Add(row("FileName"), CurrentFoldername)
                '            End If
                '        End If
                '    End If
                'Next
                If GroupNames.Count > 0 Then
                    'Need to update the shared datatable with the group names:
                    For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                        If row("FileName").ToString.Contains("\") Then
                            CurrentFoldername = GetGroupName(row("FileName"), CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                            'CurrentFoldername = row("FileName").ToString
                            'CurrentFoldername = CurrentFoldername.Substring(0, CurrentFoldername.IndexOf("\"))
                            If GroupNames.ContainsValue(CurrentFoldername) Then
                                row("GroupName") = CurrentFoldername
                            End If
                        End If
                    Next
                End If
            End If

            FolderNames = New Hashtable
            GroupNames = New Hashtable

            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"), CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                        If FolderNames.ContainsValue(CurrentFoldername) Then
                            'Already there - this is a group name, unless it's a multi-part file:
                            m = SplitText.Match(row("FileName"))
                            If Not m.Success Then
                                'It's not a multi-part, so it's a group name.  Add it.
                                GroupNames.Add(row("FileName"), CurrentFoldername)
                            End If
                        Else
                            'Not there - add to foldernames:
                            FolderNames.Add(row("FileName"), CurrentFoldername)
                        End If
                    End If
                Next
                'Repeat to catch the missing ones (bad technique but it should work!)
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"), CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                        If GroupNames.ContainsValue(CurrentFoldername) Then
                            'Make sure this entry is in there too!
                            If Not GroupNames.ContainsKey(row("FileName")) Then
                                GroupNames.Add(row("FileName"), CurrentFoldername)
                            End If
                        End If
                    End If
                Next
                'Need to update the shared datatable with the group names:
                If GroupNames.Count > 0 Then
                    For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                        If row("FileName").ToString.Contains("\") Then
                            CurrentFoldername = GetGroupName(row("FileName"), CurrentSettings.Movie_Title_Handling, CurrentSettings.Group_Name_Identifier, CurrentSettings.Series_Name_Identifier)
                            'CurrentFoldername = row("FileName").ToString
                            'CurrentFoldername = CurrentFoldername.Substring(0, CurrentFoldername.IndexOf("\"))
                            If GroupNames.ContainsValue(CurrentFoldername) Then
                                row("GroupName") = CurrentFoldername
                            End If
                        End If
                    Next
                End If
            End If
        End If

        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Complete - Found " & CountMultiPartFiles.ToString & " Multi Part Files Entries.", EventLogLevel.ImportantEvent)
        LogEvent("Processing Complete - Found " & MovedFilesCount.ToString & " to be Updated Entries.", EventLogLevel.ImportantEvent)
        LogEvent("Processing Complete - Found " & (CountOrphanFiles - MovedFilesCount).ToString & " Missing Entries.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        If CurrentSettings.Purge_Missing_Files = True Then
            ProcessOrphanRecords()
        End If

    End Sub

    Public Sub ProcessOrphanRecords()

        LogEvent("Finding Missing Files (In XML file, not on hard disk)", EventLogLevel.ImportantEvent)

        Dim row As DataRow
        ds.Tables("tblOrphanedAntRecords").Clear()

        Dim dvFoundMediaFiles As New DataView
        dvFoundMediaFiles = ds.Tables("tblFoundMediaFiles").DefaultView
        dvFoundMediaFiles.Sort = "FileName"

        Dim dvFoundNonMediaFiles As New DataView
        dvFoundNonMediaFiles = ds.Tables("tblFoundNonMediaFiles").DefaultView
        dvFoundNonMediaFiles.Sort = "FileName"

        Dim dvOrphanedMediaFiles As New DataView
        dvOrphanedMediaFiles = ds.Tables("tblOrphanedMediaFiles").DefaultView
        dvOrphanedMediaFiles.Sort = "FileName"

        Dim Path() As String = CurrentSettings.Movie_Scan_Path.Split(";")   ' scan pathes
        Dim PathAvailable(Path.Length - 1) As Boolean                       ' scan path availability
        For i As Integer = 0 To Path.Length - 1
            If System.IO.Directory.Exists(Path(i)) Then
                PathAvailable(i) = True
            Else
                PathAvailable(i) = False
            End If
        Next
        Dim iTemp As Integer
        Dim strTemp, strTemp2 As String
        Dim FileMoved As Boolean = False

        'Const RemoveOrphansWhenPathNotAvailable As Boolean = True ' Todo: To make configurable in setup

        For Each row In ds.Tables("tblXML").Rows
            If dvFoundMediaFiles.Find(row("AntShortPath")) = -1 Then
                If dvFoundNonMediaFiles.Find(row("AntShortPath")) = -1 Then

                    '("tblOrphanedMediaFiles")
                    'column.ColumnName = "AntID" (int32)
                    'column.ColumnName = "PhysicalPath"
                    'column.ColumnName = "VirtualPath"
                    'column.ColumnName = "FileName"
                    'column.ColumnName = "Moved" (boolean)
                    'column.ColumnName = "GroupName"

                    'XML Table
                    '("tblXML")
                    'column.ColumnName = "AntID" (int32)
                    'column.ColumnName = "AntPath" (string)
                    'column.ColumnName = "AntShortPath" (string)

                    ' check if it is a moved file ...
                    FileMoved = False
                    For Each row2 As DataRow In ds.Tables("tblOrphanedMediaFiles").Rows
                        If row("AntID").ToString = row2("AntID").ToString And row2("Moved") = True Then
                            LogEvent(" - Orphaned Ant Record is a Moved File : " + row("AntPath") + ", File: " + row2("FileName"), EventLogLevel.Informational)
                            FileMoved = True
                            Exit For
                        End If
                    Next

                    If FileMoved = False Then
                        'And if not there, it's probably an orphan - check to see if the path is different:
                        For i As Integer = 0 To Path.Length - 1 ' for each scan path
                            If CurrentSettings.Purge_Missing_Files_When_Source_Unavailable = True Or PathAvailable(i) = True Then ' check only for orphans, if path is available or user explicitely wants to check 
                                If CurrentSettings.Override_Path = "" Then
                                    strTemp = row("AntPath").ToString.ToLower
                                    strTemp2 = Path(i).ToLower  ' scan pathes
                                    iTemp = row("AntPath").ToString.ToLower.IndexOf(Path(i).ToLower)
                                    If row("AntPath").ToString.ToLower.IndexOf(Path(i).ToLower) > -1 Then
                                        'Match - we're scanning the path that this entry refers to - must be orphaned.
                                        ds.Tables("tblOrphanedAntRecords").Rows.Add(New Object() {row("AntID"), row("AntPath"), row("AntShortPath")})
                                        LogEvent(" - Orphaned Ant Record : " & row("AntPath"), EventLogLevel.Informational)
                                    End If
                                Else
                                    'Match - the ant records refers to the location we're using as override path.
                                    strTemp = row("AntPath").ToString.ToLower
                                    strTemp2 = CurrentSettings.Override_Path.ToLower
                                    iTemp = row("AntPath").ToString.ToLower.IndexOf(CurrentSettings.Override_Path.ToLower)
                                    If row("AntPath").ToString.ToLower.IndexOf(CurrentSettings.Override_Path.ToLower) > -1 Then
                                        'Match - we're scanning the path that this entry refers to - must be orphaned.
                                        ds.Tables("tblOrphanedAntRecords").Rows.Add(New Object() {row("AntID"), row("AntPath"), row("AntShortPath")})
                                        LogEvent(" - Orphaned Ant Record : " & row("AntPath"), EventLogLevel.Informational)
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        Next row
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Complete - Found " & CountOrphanRecords.ToString & " Missing Files.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

    End Sub

    Public Sub UpdateXMLFile()

        If (Not (System.IO.File.Exists(CurrentSettings.Internet_Parser_Path.ToString))) Then
            MsgBox("ErrorEvent : Cannot find Parser Configuration file : " & CurrentSettings.Internet_Parser_Path.ToString, MsgBoxStyle.Critical, "Missing File")
            SetCheckButtonStatus(ButtonStatus.ReadyToDoImport)
            Exit Sub
        End If
        _OperationCancelled = False

        LogEvent("Starting XML update process.", EventLogLevel.ImportantEvent)
        LogEvent("  FilePath : " + CurrentSettings.XML_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  GrabberScriptPath : " + CurrentSettings.Internet_Parser_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  ExcludePath : " + CurrentSettings.Excluded_Movies_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MoviePath : " + CurrentSettings.Movie_Scan_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  OverridePath : " + CurrentSettings.Override_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  FanartPath : " + CurrentSettings.Movie_Fanart_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Fanart Resolution Limits (Min/Max): " & CurrentSettings.Movie_Fanart_Resolution_Min & ", " & CurrentSettings.Movie_Fanart_Resolution_Max, EventLogLevel.ImportantEvent)
        LogEvent("  Fanart Download Limit: " & CurrentSettings.Movie_Fanart_Number_Limit, EventLogLevel.ImportantEvent)
        LogEvent("  PersonArtworkPath : " + CurrentSettings.Movie_PersonArtwork_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Store Short Names : " + CurrentSettings.Store_Short_Names_Only.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  OverwriteFlag : " + CurrentSettings.Overwrite_XML_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  BackupFlag : " + CurrentSettings.Backup_XML_First.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MediaType : " + CurrentSettings.Ant_Media_Type.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MediaLabel : " + CurrentSettings.Ant_Media_Label.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  SourceField : " + CurrentSettings.Ant_Database_Source_Field.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  PurgeMissing : " + CurrentSettings.Purge_Missing_Files.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  PurgeMissing Always: " + CurrentSettings.Purge_Missing_Files_When_Source_Unavailable.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Import on Internet Lookup Failure : " + CurrentSettings.Import_File_On_Internet_Lookup_Failure.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Import on Internet Lookup Failure in GuiMode: " + CurrentSettings.Import_File_On_Internet_Lookup_Failure_In_Guimode.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Prohibit Internet Lookup : " + CurrentSettings.Prohibit_Internet_Lookup.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Grabber_Override_Language    : " + CurrentSettings.Grabber_Override_Language.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Grabber_Override_PersonLimit : " + CurrentSettings.Grabber_Override_PersonLimit.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Grabber_Override_TitleLimit  : " + CurrentSettings.Grabber_Override_TitleLimit.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Grabber_Override_GetRoles    : " + CurrentSettings.Grabber_Override_GetRoles.ToString, EventLogLevel.ImportantEvent)
        LogEvent("Starting file analysis and import - " & _CountOrphanFiles.ToString & " orphaned files found.", EventLogLevel.ImportantEvent)

        'XMLDoc.Load(CurrentSettings.XML_File)

        Dim xmlFile As New FileStream(CurrentSettings.XML_File, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
        XMLDoc.Load(xmlFile)
        xmlFile.Close()

        'Using s As Stream = File.OpenRead(CurrentSettings.XML_File)
        '    XMLDoc.Load(s)
        'End Using

        If _InteractiveMode = True Then
            Form1.ToolStripProgressBar.Minimum = 0
            Form1.ToolStripProgressBar.Maximum = _CountOrphanFiles + _CountOrphanRecords
            Form1.ToolStripProgressBar.Value = 0
            Form1.btnCancelProcessing.Enabled = True
        End If

        ' Clear the multipart Files Processed Table
        ds.Tables("tblMultiPartProcessedFiles").Clear()

        Dim XMLUpdateObject As New XMLUpdateClass
        With XMLUpdateObject
            .xmldoc = XMLDoc
            .ds = ds
            .InteractiveMode = _InteractiveMode
            .objSettings = CurrentSettings
        End With

        Dim DoProcess As Boolean = True

        Dim NewFileName As String

        If CurrentSettings.Backup_XML_First = True Then
            LogEvent("Backing up xml file.", EventLogLevel.ImportantEvent)

            NewFileName = Replace(CurrentSettings.XML_File, ".xml", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".xml")
            NewFileName = NewFileName.Replace("/", "-")

            Try
                My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, NewFileName, True)
                LogEvent("Backing up xml file - done.", EventLogLevel.ImportantEvent)
            Catch ex As Exception
                DoProcess = False
                LogEvent("ErrorEvent : Cannot back up xml file : " & ex.Message, EventLogLevel.ErrorEvent)
            End Try
        End If

        'Take an additional backup regardless and store in the application directory.
        ' _TempXMLBackupFile = My.Application.Info.DirectoryPath & "\AntCatalogAutoBackup_" & My.Computer.Clock.LocalTime.ToString.Replace(":", "-") & ".xml"
        If Not (Directory.Exists(MediaPortal.Configuration.Config.GetDirectoryInfo(MediaPortal.Configuration.Config.Dir.Config).ToString & "\MyFilms\AMCUbackup")) Then
            Directory.CreateDirectory(MediaPortal.Configuration.Config.GetDirectoryInfo(MediaPortal.Configuration.Config.Dir.Config).ToString & "\MyFilms\AMCUbackup")
        End If

        _TempXMLBackupFile = MediaPortal.Configuration.Config.GetDirectoryInfo(MediaPortal.Configuration.Config.Dir.Config).ToString & "\MyFilms\AMCUbackup\AntCatalogAutoBackup_" & My.Computer.Clock.LocalTime.ToString.Replace(":", "-") & ".xml"
        _TempXMLBackupFile = _TempXMLBackupFile.Replace("/", "-")
        My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, _TempXMLBackupFile)

        If DoProcess = True Then
            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Processing Files.", EventLogLevel.ImportantEvent)
            If (_InteractiveMode) Then
                bgwFolderScanUpdate.RunWorkerAsync(XMLUpdateObject)
            Else
                XMLUpdateObject.UpdateXML()
                UpdateXMLFile_PostProcessing()
            End If
            LogEvent("===================================================================================================", EventLogLevel.Informational)
        End If

    End Sub

    Private Shared Sub bgwFolderScanUpdate_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bgwFolderScanUpdate.ProgressChanged

        If _InteractiveMode = True Then
            Form1.ToolStripProgressBar.Value += 1
            Form1.ToolStripProgressMessage.Text = "status: " & Form1.ToolStripProgressBar.Value.ToString & " of " & Form1.ToolStripProgressBar.Maximum.ToString & " total movie(s)"
        End If

        '' Tell the UI we are done.
        'Try
        '    ' Invoke the delegate on the form.
        '    Me.Invoke(New BarDelegate(UpdateBar))

        'Catch

        '    ' Some problem occurred but we can recover.

        'The e.UserState includes messages from the background process.  In this case, it's the 'File Imported - Filename' message.
        LogEvent(e.UserState, EventLogLevel.Informational)

    End Sub

    Private Shared Sub bgwFolderScanUpdate_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwFolderScanUpdate.DoWork

        Dim XMLUpdateObject As XMLUpdateClass = CType(e.Argument, XMLUpdateClass)
        ' Return the value through the Result property.
        'Try-Catch removed - causes problems as this section is still within the 2nd thread and can't log properly to the window.  OH 07Sep2008.
        'Try
        e.Result = XMLUpdateObject.UpdateXML()
        'Catch ex As Exception
        'LogEvent("ErrorEvent : " & ex.Message, EventLogLevel.ErrorEvent)
        'End Try

    End Sub

    Public Sub bgwFolderScanUpdate_Cancel()
        bgwFolderScanUpdate.CancelAsync()
        If _InteractiveMode = True Then
            Form1.btnCancelProcessing.Enabled = False
        End If
        _OperationCancelled = True
        LogEvent("Halting Operation by user request - Please Wait.", EventLogLevel.ErrorEvent)
    End Sub

    Private Class XMLUpdateClass
        Public xmldoc As XmlDocument
        Public ds As DataSet
        Public InteractiveMode As Boolean
        Public objSettings As AntSettings

        Function UpdateXML() As Integer

            Dim FilePath As String
            Dim ReplacementPath As String
            Dim FileName As String
            Dim ShortName As String
            Dim CutText As New Regex(objSettings.RegEx_Check_For_MultiPart_Files)
            Dim NewAntID As Integer = GetNextMovieID(objSettings.XML_File)
            Dim ImagePath As String = String.Empty
            Dim XMLPath As String = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
            Dim ImagePrefix As String = objSettings.Image_Download_Filename_Prefix.ToString
            ImagePath = XMLPath
            If objSettings.Use_Folder_Dot_Jpg = False And ImagePrefix.Length > 0 Then
                If ImagePrefix.Contains("\") = True Then
                    'Example : "foldername\" or "foldername\prefix_"
                    Dim PictureFolder As String = ImagePrefix.Substring(0, ImagePrefix.IndexOf("\"))
                    ImagePath = ImagePath.ToString & "\" & PictureFolder.ToString
                    If Not My.Computer.FileSystem.DirectoryExists(ImagePath) Then
                        My.Computer.FileSystem.CreateDirectory(ImagePath)
                    End If
                End If
            End If

            ' read 'Excluded Movie File' for media files to skip, if option is set
            Dim XMLExclTable As New Hashtable ' "always ignore" movies
            If (File.Exists(CurrentSettings.Excluded_Movies_File)) Then
                Dim sr As StreamReader = File.OpenText(CurrentSettings.Excluded_Movies_File)
                Dim i As Integer = 0
                Do While sr.Peek() >= 0
                    XMLExclTable.Add(i, sr.ReadLine.ToLower)
                    i += 1
                Loop
                sr.Close()
            End If

            Dim MovieRootNode As Xml.XmlNode
            MovieRootNode = xmldoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents")
            Dim row As DataRow

            Dim wtime As String = My.Computer.Clock.LocalTime.Year.ToString() + My.Computer.Clock.LocalTime.Month.ToString() + My.Computer.Clock.LocalTime.Day.ToString() + "-" + My.Computer.Clock.LocalTime.Hour.ToString() + My.Computer.Clock.LocalTime.Minute.ToString()

            Dim f As New FileInfo(objSettings.Internet_Parser_Path)
            'If Not (Directory.Exists(f.DirectoryName + "\log")) Then
            '    Directory.CreateDirectory(f.DirectoryName + "\log")
            'End If
            If Not (Directory.Exists(f.DirectoryName)) Then
                Directory.CreateDirectory(f.DirectoryName)
            End If

            Dim dvMultiPartFiles As New DataView
            dvMultiPartFiles = ds.Tables("tblMultiPartFiles").DefaultView
            'dvMultiPartFiles.
            dvMultiPartFiles.Sort = "FileName"

            Dim dvMultiPartProcessedFiles As New DataView
            dvMultiPartProcessedFiles = ds.Tables("tblMultiPartProcessedFiles").DefaultView
            dvMultiPartProcessedFiles.Sort = "FileName"

            Dim AllFilesPath As String = String.Empty


            ' *************** Process orphaned movies ***************

            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                    If bgwFolderScanUpdate.CancellationPending = True Then
                        Exit Function
                    End If

                    If dvMultiPartProcessedFiles.Find(row("FileName")) = -1 Then
                        FileName = row("FileName")
                        FilePath = row("PhysicalPath") & FileName
                        ShortName = CutText.Replace(row("FileName"), "")
                        AllFilesPath = ""
                        ReplacementPath = ""

                        ' search if it's a part of a multipart movie. In that case Replacement contains all movie files and AllFilesPath complete path for all movies
                        If (dvMultiPartFiles.Find(row("FileName")) <> -1) Then
                            For Each row2 As DataRow In ds.Tables("tblMultiPartFiles").Rows
                                If (row2("ShortName") = ShortName) Then
                                    ds.Tables("tblMultiPartProcessedFiles").Rows.Add(row2("FileName"))
                                    Dim wpath As String = String.Empty
                                    If (objSettings.Store_Short_Names_Only = True) Then
                                        wpath = row2("FileName")
                                        If wpath.Contains("\") Then
                                            wpath = wpath.Substring(wpath.LastIndexOf("\") + 1)
                                        End If
                                    Else
                                        If row2("VirtualPath") = "" Then
                                            wpath = row2("PhysicalPath") & row2("FileName")
                                        Else
                                            wpath = row2("VirtualPath") & row2("FileName")
                                        End If
                                    End If
                                    If ReplacementPath.Length = 0 Then
                                        ReplacementPath = wpath
                                    Else
                                        ReplacementPath = ReplacementPath + ";" + wpath
                                    End If
                                    If AllFilesPath.Length = 0 Then
                                        AllFilesPath = FilePath
                                    Else
                                        AllFilesPath = AllFilesPath + ";" + FilePath
                                    End If
                                End If
                            Next
                        Else
                            'Not a multi-part entry, so just set the ReplacementPath value and continue:
                            If (objSettings.Store_Short_Names_Only = True) Then
                                ReplacementPath = FileName
                                If ReplacementPath.Contains("\") Then
                                    ReplacementPath = ReplacementPath.Substring(ReplacementPath.LastIndexOf("\") + 1)
                                End If
                            Else
                                If row("VirtualPath") = "" Then
                                    ReplacementPath = FilePath
                                Else
                                    ReplacementPath = row("VirtualPath") & FileName
                                End If
                            End If
                        End If

                        ' search and load internet data, mediainfo, etc.
                        Dim Ant As New AntRecord()
                        With Ant
                            .FileName = FileName
                            .FilePath = FilePath
                            .AllFilesPath = AllFilesPath
                            .MediaLabel = objSettings.Ant_Media_Label
                            .MediaType = objSettings.Ant_Media_Type
                            .SourceField = objSettings.Ant_Database_Source_Field
                            .OverridePath = ReplacementPath
                            .MovieNumber = NewAntID
                            .XMLDoc = xmldoc
                            .ParserPath = objSettings.Internet_Parser_Path
                            .InteractiveMode = InteractiveMode
                            .ExcludeFile = objSettings.Excluded_Movies_File
                            '.ImagePath = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
                            .ImagePath = ImagePath
                            .InternetLookupAlwaysPrompt = objSettings.Internet_Lookup_Always_Prompt
                            .Read_DVD_Label = objSettings.Read_DVD_Label
                            .Dont_Ask_Interactive = objSettings.Dont_Ask_Interactive
                            .XMLFilePath = objSettings.XML_File
                            .GroupName = row("GroupName").ToString
                            .OnlyAddMissingData = False ' always add all selected data for new records
                            .OnlyUpdateNonEmptyData = False
                            .MasterTitle = objSettings.Master_Title
                            .GrabberOverrideLanguage = objSettings.Grabber_Override_Language
                            .GrabberOverridePersonLimit = objSettings.Grabber_Override_PersonLimit
                            .GrabberOverrideTitleLimit = objSettings.Grabber_Override_TitleLimit
                            .GrabberOverrideGetRoles = objSettings.Grabber_Override_GetRoles
                        End With

                        If (row("Moved")) Then
                            Ant.MovieNumber = row("AntID")
                            ' filename exist but with wrong path (may be moved. Entry don't be created but updated    
                            Ant.UpdateElement()

                            If (FilePath.Length > 0 And XMLExclTable.ContainsValue(FilePath.ToLower) = True) Then ' if in "always ignore" file, skip update of properties (internet lookup)
                                Ant.ProhibitInternetLookup = False
                            Else
                                Ant.ProhibitInternetLookup = Not CurrentSettings.Rescan_Moved_Files ' Ant.ProhibitInternetLookup = True
                            End If

                            Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
                            Ant.SaveProgress()
                            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                            Else
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File Updated (moved) - " & ReplacementPath & " - " & Ant.LastOutputMessage)
                            End If
                        Else
                            'need to create a new entry:
                            Ant.CreateElement() ' find movie and add it
                            If (FilePath.Length > 0 And XMLExclTable.ContainsValue(FilePath.ToLower) = True) Then ' if in "always ignore" file, skip update of properties (internet lookup)
                                Ant.ProhibitInternetLookup = False
                            End If
                            Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
                            Ant.SaveProgress()

                            If Ant.LastOutputMessage.StartsWith("UserAbort") = True Then
                                bgwFolderScanUpdate.CancelAsync()
                            End If

                            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Or Ant.LastOutputMessage.StartsWith("UserAbort") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                                'LogEvent("ErrorEvent : " & blah.LastOutputMessage, EventLogLevel.ErrorEvent)
                            Else
                                If (_interactiveMode = False And CurrentSettings.Import_File_On_Internet_Lookup_Failure) Or (_interactiveMode = True And CurrentSettings.Import_File_On_Internet_Lookup_Failure_In_Guimode = True) Then
                                    'Doesn't matter if the Internet loookup worked; just load the entry:
                                    MovieRootNode.AppendChild(Ant.XMLElement)
                                    _countRecordsAdded += 1
                                    Dim OutputMessage As String = String.Empty
                                    If ReplacementPath.IndexOf(";") >= 0 Then
                                        OutputMessage = " Files Imported - " & ReplacementPath
                                    Else
                                        OutputMessage = " File  Imported - " & ReplacementPath
                                    End If
                                    If Ant.InternetLookupOK = False Then
                                        OutputMessage += " (Internet Lookup Failed)"
                                    End If
                                    bgwFolderScanUpdate.ReportProgress(_countRecordsAdded, OutputMessage & " - " & Ant.LastOutputMessage)
                                    NewAntID += 1
                                Else
                                    'First check if the Internet Lookup works:
                                    If Ant.InternetLookupOK = True Then
                                        MovieRootNode.AppendChild(Ant.XMLElement)
                                        _countRecordsAdded += 1
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_countRecordsAdded, " Files Imported - " & ReplacementPath & " - " & Ant.LastOutputMessage)
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_countRecordsAdded, " File  Imported - " & ReplacementPath & " - " & Ant.LastOutputMessage)
                                        End If
                                        NewAntID += 1
                                    Else
                                        'Mark as Ignored - do not import.
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_countRecordsAdded, " Files Not Imported - **********  " & ReplacementPath & "  *****" & " - " & Ant.LastOutputMessage)
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_countRecordsAdded, " File  Not Imported - **********  " & ReplacementPath & "  *****" & " - " & Ant.LastOutputMessage)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next row
            End If


            ' *************** Process orphaned non media files (ISO, etc.) ***************

            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                    'Dim e As New FilmUpdatedEventArgs("Info", "Testmessage from Event")
                    'OnFilmUpdated(e)
                    'If EventNeu() IsNot Nothing Then
                    '    EventNeu("Test Eventhandler");
                    'End If

                    If bgwFolderScanUpdate.CancellationPending = True Then
                        Exit Function
                    End If
                    If dvMultiPartProcessedFiles.Find(row("FileName")) = -1 Then
                        FileName = row("FileName")
                        FilePath = row("PhysicalPath") & FileName
                        ShortName = CutText.Replace(row("FileName"), "")
                        AllFilesPath = ""
                        ReplacementPath = ""
                        ' search if it's a part of a multipart movie. In that case Replacement contains all movie files and AllFilesPath complete path for all movies
                        If (dvMultiPartFiles.Find(row("FileName")) <> -1) Then
                            For Each row2 As DataRow In ds.Tables("tblMultiPartFiles").Rows
                                If (row2("ShortName") = ShortName) Then
                                    ds.Tables("tblMultiPartProcessedFiles").Rows.Add(row2("FileName"))
                                    Dim wpath As String = String.Empty
                                    If (objSettings.Store_Short_Names_Only = True) Then
                                        wpath = row2("FileName")
                                        If wpath.Contains("\") Then
                                            wpath = wpath.Substring(wpath.LastIndexOf("\") + 1)
                                        End If
                                    Else
                                        If row("VirtualPath") = "" Then
                                            wpath = row2("PhysicalPath") & row2("FileName")
                                        Else
                                            wpath = row2("VirtualPath") & row2("FileName")
                                        End If
                                    End If
                                    If ReplacementPath.Length = 0 Then
                                        ReplacementPath = wpath
                                    Else
                                        ReplacementPath = ReplacementPath + ";" + wpath
                                    End If
                                    If AllFilesPath.Length = 0 Then
                                        AllFilesPath = FilePath
                                    Else
                                        AllFilesPath = AllFilesPath + ";" + FilePath
                                    End If
                                End If
                            Next
                        Else
                            If (objSettings.Store_Short_Names_Only = True) Then
                                ReplacementPath = FileName
                                If ReplacementPath.Contains("\") Then
                                    ReplacementPath = ReplacementPath.Substring(ReplacementPath.LastIndexOf("\") + 1)
                                End If
                            Else
                                If row("VirtualPath") = "" Then
                                    ReplacementPath = FilePath
                                Else
                                    ReplacementPath = row("VirtualPath") & FileName
                                End If
                            End If
                        End If

                        Dim Ant As New AntRecord()
                        With Ant
                            .FileName = FileName
                            .FilePath = FilePath
                            .AllFilesPath = AllFilesPath
                            .MediaLabel = objSettings.Ant_Media_Label
                            .MediaType = objSettings.Ant_Media_Type
                            .SourceField = objSettings.Ant_Database_Source_Field
                            .OverridePath = ReplacementPath
                            .MovieNumber = NewAntID
                            .XMLDoc = xmldoc
                            .ParserPath = objSettings.Internet_Parser_Path
                            .InteractiveMode = InteractiveMode
                            .ExcludeFile = objSettings.Excluded_Movies_File
                            '.ImagePath = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
                            .ImagePath = ImagePath
                            .InternetLookupAlwaysPrompt = objSettings.Internet_Lookup_Always_Prompt
                            .Read_DVD_Label = objSettings.Read_DVD_Label
                            .Dont_Ask_Interactive = objSettings.Dont_Ask_Interactive
                            .XMLFilePath = objSettings.XML_File
                            .GroupName = row("GroupName").ToString
                            .OnlyAddMissingData = False ' always add all selected data for new records
                            .OnlyUpdateNonEmptyData = False
                            .MasterTitle = objSettings.Master_Title
                            .GrabberOverrideLanguage = objSettings.Grabber_Override_Language
                            .GrabberOverridePersonLimit = objSettings.Grabber_Override_PersonLimit
                            .GrabberOverrideTitleLimit = objSettings.Grabber_Override_TitleLimit
                            .GrabberOverrideGetRoles = objSettings.Grabber_Override_GetRoles
                        End With
                        If (row("Moved")) Then
                            ' filename exist but with wrong path (maybe moved. Entry don't be created but updated  
                            Ant.UpdateElement()
                            If (FilePath.Length > 0 And XMLExclTable.ContainsValue(FilePath.ToLower) = True) Then ' if in "always ignore" file, skip update of properties (internet lookup)
                                Ant.ProhibitInternetLookup = False
                            Else
                                Ant.ProhibitInternetLookup = True
                            End If
                            Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
                            Ant.SaveProgress()
                            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                            Else
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Updated - " & ReplacementPath)
                            End If
                        Else
                            'Try
                            Ant.CreateElement()
                            If (FilePath.Length > 0 And XMLExclTable.ContainsValue(FilePath.ToLower) = True) Then ' if in "always ignore" file, skip update of properties (internet lookup)
                                Ant.ProhibitInternetLookup = False
                            End If
                            Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
                            Ant.SaveProgress()
                            'Catch ex As Exception
                            'LogEvent("ErrorEvent : " & ex.Message.ToString, EventLogLevel.ErrorEvent)
                            'End Try


                            If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                                'LogEvent("ErrorEvent : " & blah.LastOutputMessage, EventLogLevel.ErrorEvent)
                            Else
                                'Check to see whether to save record:
                                If CurrentSettings.Import_File_On_Internet_Lookup_Failure = True Then
                                    'Doesn't matter if the Internet loookup worked; just load the entry:
                                    MovieRootNode.AppendChild(Ant.XMLElement)
                                    _CountRecordsAdded += 1
                                    If ReplacementPath.IndexOf(";") >= 0 Then
                                        bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
                                    Else
                                        bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
                                    End If
                                    NewAntID += 1
                                Else
                                    'First check if the Internet Lookup works:
                                    If Ant.InternetLookupOK = True Then
                                        MovieRootNode.AppendChild(Ant.XMLElement)
                                        _CountRecordsAdded += 1
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
                                        End If
                                        NewAntID += 1
                                    Else
                                        'Mark as Ignored - do not import.
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Ignored - **********  " & ReplacementPath & "  *****")
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Ignored - **********  " & ReplacementPath & "  *****")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next row
            End If



            ' *************** Process orphaned trailers ***************

            ' we do 3 things here:
            ' 1. try to register the trailer with an existing movie
            ' 2. if it cannot be matched, show dialog to let user choose the correct movie or btter search name (interactive mode only)
            ' 3. if trailer cannot be registered to a movie, anew movie record will be created, trailer registered and internet data loaded (same as movie import)


            'If ds.Tables("tblOrphanedTrailerMediaFiles") IsNot Nothing Then
            '    For Each row In ds.Tables("tblOrphanedTrailerMediaFiles").Rows
            '        If bgwFolderScanUpdate.CancellationPending = True Then
            '            Exit Function
            '        End If
            '        If dvMultiPartProcessedFiles.Find(row("FileName")) = -1 Then
            '            FileName = row("FileName")
            '            FilePath = row("PhysicalPath") & FileName
            '            ShortName = CutText.Replace(row("FileName"), "")
            '            AllFilesPath = ""
            '            ReplacementPath = ""
            '            ' search if it's a part of a multipart movie. In that case Replacement contains all movie files and AllFilesPath complete path for all movies
            '            If (dvMultiPartFiles.Find(row("FileName")) <> -1) Then
            '                For Each row2 As DataRow In ds.Tables("tblMultiPartFiles").Rows
            '                    If (row2("ShortName") = ShortName) Then
            '                        ds.Tables("tblMultiPartProcessedFiles").Rows.Add(row2("FileName"))
            '                        Dim wpath As String = String.Empty
            '                        If row("VirtualPath") = "" Then
            '                            wpath = row2("PhysicalPath") & row2("FileName")
            '                        Else
            '                            wpath = row2("VirtualPath") & row2("FileName")
            '                        End If
            '                        If ReplacementPath.Length = 0 Then
            '                            ReplacementPath = wpath
            '                        Else
            '                            ReplacementPath = ReplacementPath + ";" + wpath
            '                        End If
            '                        If AllFilesPath.Length = 0 Then
            '                            AllFilesPath = FilePath
            '                        Else
            '                            AllFilesPath = AllFilesPath + ";" + FilePath
            '                        End If
            '                    End If
            '                Next
            '            Else
            '                If (objSettings.Store_Short_Names_Only = True) Then
            '                    ReplacementPath = FileName
            '                    If ReplacementPath.Contains("\") Then
            '                        ReplacementPath = ReplacementPath.Substring(ReplacementPath.LastIndexOf("\") + 1)
            '                    End If
            '                Else
            '                    If row("VirtualPath") = "" Then
            '                        ReplacementPath = FilePath
            '                    Else
            '                        ReplacementPath = row("VirtualPath") & FileName
            '                    End If
            '                End If
            '            End If

            '            Dim Ant As New AntRecord()
            '            With Ant
            '                .FileName = FileName
            '                .FilePath = FilePath
            '                .AllFilesPath = AllFilesPath
            '                .MediaLabel = objSettings.Ant_Media_Label
            '                .MediaType = objSettings.Ant_Media_Type
            '                .SourceField = objSettings.Ant_Database_Source_Field
            '                .OverridePath = ReplacementPath
            '                .MovieNumber = NewAntID
            '                .XMLDoc = xmldoc
            '                .ParserPath = objSettings.Internet_Parser_Path
            '                .GrabberOverrideLanguage = objSettings.Grabber_Override_Language
            '                .GrabberOverrideGetRoles = objSettings.Grabber_Override_GetRoles
            '                .GrabberOverridePersonLimit = objSettings.Grabber_Override_PersonLimit
            '                .GrabberOverrideTitleLimit = objSettings.Grabber_Override_TitleLimit
            '                .InteractiveMode = InteractiveMode
            '                .ExcludeFile = objSettings.Excluded_Movies_File
            '                '.ImagePath = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
            '                .ImagePath = ImagePath
            '                .InternetLookupAlwaysPrompt = objSettings.Internet_Lookup_Always_Prompt
            '                .DateHandling = objSettings.Date_Handling
            '                .Read_DVD_Label = objSettings.Read_DVD_Label
            '                .Dont_Ask_Interactive = objSettings.Dont_Ask_Interactive
            '                .XMLFilePath = objSettings.XML_File
            '                .MovieTitleHandling = objSettings.Movie_Title_Handling
            '                .GroupName = row("GroupName").ToString
            '            End With
            '            If (row("Moved")) Then
            '                ' filename exist but with wrong path (maybe moved. Entry don't be created but updated  
            '                Ant.UpdateElement()
            '                Ant.ProhibitInternetLookup = True
            '                Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
            '                Ant.SaveProgress()
            '                If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
            '                    bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
            '                Else
            '                    bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Updated - " & ReplacementPath)
            '                End If
            '            Else
            '                'Try
            '                Ant.CreateElement()
            '                Ant.ProcessFile(AntRecord.ProcessModeNames.Import)
            '                Ant.SaveProgress()
            '                'Catch ex As Exception
            '                'LogEvent("ErrorEvent : " & ex.Message.ToString, EventLogLevel.ErrorEvent)
            '                'End Try


            '                If Ant.LastOutputMessage.StartsWith("ErrorEvent") = True Then
            '                    bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
            '                    'LogEvent("ErrorEvent : " & blah.LastOutputMessage, EventLogLevel.ErrorEvent)
            '                Else
            '                    'Check to see whether to save record:
            '                    If CurrentSettings.Import_File_On_Internet_Lookup_Failure = True Then
            '                        'Doesn't matter if the Internet loookup worked; just load the entry:
            '                        MovieRootNode.AppendChild(Ant.XMLElement)
            '                        _CountRecordsAdded += 1
            '                        If ReplacementPath.IndexOf(";") >= 0 Then
            '                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
            '                        Else
            '                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
            '                        End If
            '                        NewAntID += 1
            '                    Else
            '                        'First check if the Internet Lookup works:
            '                        If Ant.InternetLookupOK = True Then
            '                            MovieRootNode.AppendChild(Ant.XMLElement)
            '                            _CountRecordsAdded += 1
            '                            If ReplacementPath.IndexOf(";") >= 0 Then
            '                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
            '                            Else
            '                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
            '                            End If
            '                            NewAntID += 1
            '                        Else
            '                            'Mark as Ignored - do not import.
            '                            If ReplacementPath.IndexOf(";") >= 0 Then
            '                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Ignored - **********  " & ReplacementPath & "  *****")
            '                            Else
            '                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Ignored - **********  " & ReplacementPath & "  *****")
            '                            End If
            '                        End If
            '                    End If
            '                End If
            '            End If
            '        End If
            '    Next row
            'End If


        End Function

        Public Sub New()

        End Sub
    End Class


    Private Shared Sub bgwFolderScanUpdate_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwFolderScanUpdate.RunWorkerCompleted
        'Now new XML document is in memory; and we have an arraylist of movies with multiple parts:

        If e.Error IsNot Nothing Then
            'If e.ErrorEvent.Message.ToString <> "" Then
            LogEvent("ErrorEvent : " & e.Error.Message.ToLower, EventLogLevel.ErrorEvent)
            'End If
        End If

        'Check to see if the cancel button was clicked; if so prompt on how to continue
        If _OperationCancelled = True Then
            If MsgBox("Save work done so far?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                LogEvent("Operation Cancelled.", EventLogLevel.ErrorEvent)
                'If _InteractiveMode = True Then
                Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
                Form1.btnCancelProcessing.Enabled = False
                SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
                Try
                    My.Computer.FileSystem.CopyFile(_TempXMLBackupFile, CurrentSettings.XML_File, True)
                    My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Catch ex As Exception

                End Try
                Exit Sub
            Else
                LogEvent("Operation Cancelled - Save Continuing", EventLogLevel.ErrorEvent)
            End If
        End If


        UpdateXMLFile_PostProcessing()

    End Sub

    Private Shared Sub UpdateXMLFile_PostProcessing()
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("File Import Complete - " & _CountRecordsAdded.ToString & " files loaded.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        'Purge database entries for missing files (optional)
        If CurrentSettings.Purge_Missing_Files = True Then
            Dim MovieRootNode As Xml.XmlNode = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents")
            Dim NodeToDelete As Xml.XmlNode
            Dim row As DataRow

            LogEvent("Removing Missing Files from Database", EventLogLevel.ImportantEvent)

            For Each row In ds.Tables("tblOrphanedAntRecords").Rows

                NodeToDelete = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & row("AntID") & "']")
                If Not NodeToDelete Is Nothing Then
                    _CountRecordsDeleted += 1
                    MovieRootNode.RemoveChild(NodeToDelete)
                    LogEvent(" Entry Removed : " & row("FileName"), EventLogLevel.Informational)
                End If
            Next row
        End If


        LogEvent("Writing changes to XML file", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        Try
            If CurrentSettings.Overwrite_XML_File = True Then
                'XMLDoc.Save(CurrentSettings.XML_File)
                Dim xmlFile As New FileStream(CurrentSettings.XML_File, FileMode.Open, FileAccess.Write, FileShare.None)
                xmlFile.SetLength(0)
                XMLDoc.Save(xmlFile)
                xmlFile.Close()

                'Using s As Stream = File.OpenWrite(CurrentSettings.XML_File)
                '    XMLDoc.Save(s)
                '    s.Close()
                'End Using
            Else
                XMLDoc.Save(CurrentSettings.XML_File.Replace(".xml", "-NEW.xml"))
            End If
            My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        Catch ex As Exception
            LogEvent("ErrorEvent SAVING CHANGES TO XML FILE.", EventLogLevel.ErrorEvent)
        End Try


        LogEvent("XML update process complete.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        If _InteractiveMode = True Then
            SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
            Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
            Form1.btnCancelProcessing.Enabled = False

            If CurrentSettings.Execute_Program = False Or CurrentSettings.Execute_Program_Path = "" Then
                MsgBox("Process Complete", MsgBoxStyle.Information)
            ElseIf CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
                If CurrentSettings.Execute_Only_For_Orphans = False Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception
                    End Try
                Else
                    If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                        Try
                            System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End If
        Else
            If CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
                If CurrentSettings.Execute_Only_For_Orphans = False Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception

                    End Try
                Else
                    If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                        Try
                            System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If
        End If
    End Sub

#End Region

    Public Sub Reset()
        If ds.Tables("tblXML") IsNot Nothing Then
            ds.Tables("tblXML").Clear()
        End If
        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundNonMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundTrailerFiles") IsNot Nothing Then
            ds.Tables("tblFoundTrailerFiles").Clear()
        End If
        If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
            ds.Tables("tblOrphanedMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblOrphanedNonMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedTrailerMediaFiles") IsNot Nothing Then
            ds.Tables("tblOrphanedTrailerMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedAntRecords") IsNot Nothing Then
            ds.Tables("tblOrphanedAntRecords").Clear()
        End If
        If ds.Tables("tblMultiPartFiles") IsNot Nothing Then
            ds.Tables("tblMultiPartFiles").Clear()
        End If
    End Sub

    Public Sub TEST_ListTable(ByVal TableName As String)
        Dim ColumnNames As String = String.Empty
        Dim RowText As String = String.Empty

        If ds.Tables(TableName) IsNot Nothing Then
            LogEvent(TableName & " : " & ds.Tables(TableName).Rows.Count.ToString & " rows.", EventLogLevel.ImportantEvent)
            If ds.Tables(TableName).Rows.Count > 0 Then
                ColumnNames = ds.Tables(TableName).Columns.Cast(Of DataColumn)().Aggregate(ColumnNames, Function(current, col) current + (col.ColumnName & " | "))
                LogEvent(ColumnNames.Substring(0, ColumnNames.Length - 1), EventLogLevel.ImportantEvent)
                For Each row As DataRow In ds.Tables(TableName).Rows
                    For i As Integer = 0 To ds.Tables(TableName).Columns.Count - 1
                        RowText += row(i).ToString & " | "
                    Next
                    LogEvent(RowText.Substring(0, RowText.Length - 1), EventLogLevel.ImportantEvent)
                    RowText = String.Empty
                Next
                LogEvent("End of table dump", EventLogLevel.ImportantEvent)
            End If
        End If
    End Sub

    Public Shared Sub SearchFile(ByRef FileToScan As String, ByVal FoldersToScan As String, ByVal Number As String, ByRef wtitle As String)
        'NOTE : cannot call LogEvent from here as this will be running on a separate thread and cannot check the form1.visible property!
        Try
            If File.Exists(FileToScan) Then
                Return
            End If
        Catch ex As Exception
            'LogEvent("File ignored (invalid filename): " & Number & " | " & FileToScan, EventLogLevel.Informational)
            FileToScan = String.Empty
            Return
        End Try

        If (FileToScan.IndexOf("\\") > 0 Or FileToScan.IndexOf("//") > 0 Or FileToScan.IndexOf(":") > 0) Then
            ' File Moved, need to update the xml first
            'LogEvent("File Moved, Please Update the XML first : " & Number & " | " & FileToScan, EventLogLevel.ImportantEvent)
            FileToScan = String.Empty
            Return
        End If
        Dim subfolder As String = String.Empty
        If (wtitle.IndexOf("\") > 0) Then
            subfolder = wtitle.Substring(0, wtitle.LastIndexOf("\"))
        End If
        Dim FolderList() As String
        FolderList = FoldersToScan.Split(";")
        Dim myEnumerator As System.Collections.IEnumerator = FolderList.GetEnumerator()
        While myEnumerator.MoveNext()
            For Each folder As String In FolderList
                If File.Exists(folder + FileToScan) Then
                    FileToScan = folder + FileToScan
                    Return
                Else
                    If File.Exists(folder + subfolder + "\" + FileToScan) Then
                        FileToScan = folder + subfolder + "\" + FileToScan
                        Return
                    End If
                End If
                Dim dirInfo As New DirectoryInfo(folder)
                Dim filelist As List(Of FileInfo) = Grabber.GrabUtil.GetFilesRecursive(dirInfo)
                For Each file As FileInfo In filelist
                    If (file.Name = FileToScan) Then
                        FileToScan = file.FullName
                        wtitle = file.FullName.Substring(folder.Length)
                        Return
                    End If
                Next

            Next
        End While
        FileToScan = String.Empty
    End Sub

    Private Sub BuildTables()

        Dim table As DataTable
        Dim column As DataColumn
        ds = New DataSet

        'XML Table
        table = New DataTable("tblXML")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntShortPath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'Found Media Files Table
        table = New DataTable("tblFoundMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FilePath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'Found Non-Media Files Table
        table = New DataTable("tblFoundNonMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FilePath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'Found Trailer-Media Files Table
        table = New DataTable("tblFoundTrailerFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FilePath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Boolean")
        column.ColumnName = "Moved"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "GroupName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedNonMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Boolean")
        column.ColumnName = "Moved"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "GroupName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedTrailerMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Boolean")
        column.ColumnName = "Moved"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "GroupName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedAntRecords")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblMultiPartFiles")
        'column = New DataColumn()
        'column.DataType = System.Type.GetType("System.Int32")
        'column.ColumnName = "AntID"
        'table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "ShortName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblMultiPartProcessedFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblAntFields")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FieldName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FieldDataType"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'XML Table
        table = New DataTable("tblNodesToProcess")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntTitle"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntDirector"
        table.Columns.Add(column)
        ' Guzzi added: Year for better matching
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntYear"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntIMDB_Id"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("dtNone")
        table.Columns.Add(New DataColumn("FileName"))
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'table = New DataTable("dtDone")
        'table.Columns.Add(New DataColumn("FileName"))
        'table.CaseSensitive = False
        'ds.Tables.Add(table)

        PopulateReferenceTables()

    End Sub

    Private Sub PopulateReferenceTables()
        If ds.Tables("tblAntFields") IsNot Nothing Then
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Number", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Date", "Date"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Rating", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Year", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Length", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"VideoBitrate", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"AudioBitrate", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Disks", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Checked", "Boolean"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"MediaLabel", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"MediaType", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Source", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Borrower", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"OriginalTitle", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"TranslatedTitle", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"FormattedTitle", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Director", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Producer", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Country", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Category", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Actors", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"URL", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Description", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Comments", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"VideoFormat", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"AudioFormat", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Resolution", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Framerate", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Languages", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Subtitles", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Size", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Picture", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Certification", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Writer", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"TagLine", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"IMDB_Id", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"TMDB_Id", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"IMDB_Rank", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Studio", "String"})

            ds.Tables("tblAntFields").Rows.Add(New Object() {"SourceTrailer", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Edition", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Fanart", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"RatingUser", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"CategoryTrakt", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"AudioChannelCount", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Aspectratio", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Watched", "String"})

            ds.Tables("tblAntFields").Rows.Add(New Object() {"DateWatched", "Date"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Favorite", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Tags", "String"})

            ds.Tables("tblAntFields").Rows.Add(New Object() {"CustomField1", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"CustomField2", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"CustomField3", "String"})

            'Currently unused or intentionally disabled fields
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"Persons", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"IsOnline", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"IsOnlineTrailer", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"RecentlyAdded", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"IndexedTitle", "String"})
            'ds.Tables("tblAntFields").Rows.Add(New Object() {"AgeAdded", "String"})
        End If

    End Sub

    Public Function ManualTestMissingFanart_IsUpdateNeeded(ByVal wtitle As String) As Boolean

        wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower().Trim()).Replace(" ", ".")
        '        If (System.IO.Directory.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}")) Then
        '        If (System.IO.File.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}\\{" & wtitle & "}.jpg")) And (System.IO.Directory.GetFileSystemEntries(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}").Length > 1) Then
        If (System.IO.File.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}\\{" & wtitle & "}.jpg")) Then
            Return False
        End If
        '        End If
        Return True

    End Function

    Public Function ManualTestMissingTrailer(ByVal wtitle As String) As Boolean

        'Guzzi: To be added
        Return True

    End Function

    Private Shared Function GetValue(ByVal CurrentNode As Xml.XmlNode, ByVal currentAttribute As String) As String
        'Dim currentValue As String = Nothing
        Dim attr As Xml.XmlAttribute
        Dim element As Xml.XmlElement
        Dim customfieldselement As Xml.XmlElement
        Dim customfieldsattr As Xml.XmlAttribute

        attr = CurrentNode.Attributes(currentAttribute)
        element = CurrentNode.Item(currentAttribute)
        customfieldselement = CurrentNode.Item("CustomFields")
        If attr Is Nothing And element Is Nothing And customfieldselement Is Nothing Then ' no values exist at all
            Return Nothing
        Else
            If Not attr Is Nothing Then ' check for standard attr value
                If attr.Value Is Nothing Then
                    Return Nothing
                ElseIf attr.Value = "" Then
                    Return ""
                Else
                    Return attr.Value
                End If
            ElseIf Not customfieldselement Is Nothing Then  ' check for new AMC4 enhanced element value (Customfields)
                customfieldsattr = customfieldselement.Attributes(currentAttribute)
                If Not customfieldsattr Is Nothing Then ' check for  attr value inf customfields element
                    If customfieldsattr.Value Is Nothing Then
                        Return Nothing
                    ElseIf customfieldsattr.Value = "" Then
                        Return ""
                    Else
                        Return customfieldsattr.Value
                    End If
                Else
                    Return Nothing
                End If
            ElseIf Not element Is Nothing Then  ' check for old MyFilms enhanced element value
                If element.InnerText Is Nothing Then
                    Return Nothing
                ElseIf element.InnerText = "" Then
                    Return ""
                Else
                    Return element.InnerText
                End If
            End If
        End If
        Return Nothing
    End Function

    Private Shared Sub RemoveValue(ByRef CurrentNode As Xml.XmlNode, ByVal currentAttribute As String)
        If (IsExtendedField(currentAttribute)) Then
            Const SubElementName As String = "CustomFields"

            ' remove CustomFields Attribute
            If Not CurrentNode.Item(SubElementName).Attributes(currentAttribute) Is Nothing Then
                CurrentNode.Item(SubElementName).Attributes.Remove(CurrentNode.Item(SubElementName).Attributes.GetNamedItem(currentAttribute))
            End If

            ' also remove Element, in case there is any left
            If Not CurrentNode.Item(currentAttribute) Is Nothing Then
                CurrentNode.RemoveChild(CurrentNode.Item(currentAttribute))
            End If
        Else
            If Not CurrentNode.Attributes(currentAttribute) Is Nothing Then
                CurrentNode.Attributes.Remove(CurrentNode.Attributes.GetNamedItem(currentAttribute))
            End If
        End If
    End Sub

    Private Shared Sub SetValue(ByRef CurrentNode As Xml.XmlNode, ByVal currentAttribute As String, ByVal value As String)
        If (IsExtendedField(currentAttribute)) Then
            CreateOrUpdateCustomFieldsAttribute(CurrentNode, currentAttribute, value)
        Else
            CreateOrUpdateAttribute(CurrentNode, currentAttribute, value)
        End If
    End Sub

    Private Shared Sub CreateOrUpdateAttribute(ByRef CurrentNode As Xml.XmlElement, ByVal currentAttribute As String, ByVal currentValue As String)
        'Dim _XMLDoc As New Xml.XmlDocument
        'Dim _XMLElement As Xml.XmlElement
        'CurrentSettings.Only_Add_Missing_Data

        Dim attr As Xml.XmlAttribute
        'If currentValue <> "" Then
        '    CleanValueForInnerXML(currentValue)
        'End If
        'If currentValue <> "" Or (currentValue = "" And CurrentSettings.Only_Update_With_Nonempty_Data = False) Then
        'End If
        If CurrentNode.Attributes(currentAttribute) Is Nothing Then
            attr = XMLDoc.CreateAttribute(currentAttribute)
            attr.Value = currentValue
            CurrentNode.Attributes.Append(attr)
        Else
            CurrentNode.Attributes(currentAttribute).Value = currentValue
        End If
    End Sub

    Private Sub CreateOrUpdateElement(ByRef CurrentNode As Xml.XmlElement, ByVal currentAttribute As String, ByVal currentValue As String)
        'Dim _XMLDoc As New Xml.XmlDocument
        'Dim _XMLElement As Xml.XmlElement
        ' First create new AMC4 objects - does NOT (yet) reset currentValue
        CreateOrUpdateCustomFieldsAttribute(CurrentNode, currentAttribute, currentValue)

        Dim element As Xml.XmlElement
        'If currentValue <> "" Then
        '    CleanValueForInnerXML(currentValue)
        'End If
        'If currentValue <> "" Or (currentValue = "" And CurrentSettings.Only_Update_With_Nonempty_Data = False) Then
        'End If
        If CurrentNode.Item(currentAttribute) Is Nothing Then
            element = XMLDoc.CreateElement(currentAttribute)
            element.InnerText = currentValue
            CurrentNode.AppendChild(element)
        Else
            CurrentNode.Item(currentAttribute).InnerText = currentValue
        End If
    End Sub

    Private Shared Sub CreateOrUpdateCustomFieldsAttribute(ByRef CurrentNode As Xml.XmlElement, ByVal currentAttribute As String, ByVal currentValue As String)
        Const SubElementName As String = "CustomFields"
        'Dim _XMLDoc As New Xml.XmlDocument
        'Dim _XMLElement As Xml.XmlElement

        'now check and update or create Attributes in the CustomFields Element
        Dim attr As Xml.XmlAttribute
        'If currentValue <> "" Then
        '    CleanValueForInnerXML(currentValue)
        'End If
        'If currentValue <> "" Or (currentValue = "" And CurrentSettings.Only_Update_With_Nonempty_Data = False) Then
        'End If
        If CurrentNode.Item(SubElementName) Is Nothing Then
            Dim element As Xml.XmlElement
            element = XMLDoc.CreateElement(SubElementName)
            element.InnerText = ""
            CurrentNode.AppendChild(element)
        End If

        If CurrentNode.Item(SubElementName).Attributes(currentAttribute) Is Nothing Then
            attr = XMLDoc.CreateAttribute(currentAttribute)
            attr.Value = currentValue
            CurrentNode.Item(SubElementName).Attributes.Append(attr)
        Else
            CurrentNode.Item(SubElementName).Attributes(currentAttribute).Value = currentValue
        End If
    End Sub

    Private Shared Function IsExtendedField(ByVal fieldname) As Boolean
        Select Case fieldname
            Case "Number", "Date", "Rating", "Year", "Length", "VideoBitrate", "AudioBitrate", "Disks", "Checked", "MediaLabel", "MediaType", "Source", "Borrower", "OriginalTitle", "TranslatedTitle", "FormattedTitle", "Director", "Producer", "Country", "Category", "Actors", "URL", "Description", "Comments", "VideoFormat", "AudioFormat", "Resolution", "Framerate", "Languages", "Subtitles", "Size", "Picture"
                Return False
            Case Else
                Return True
        End Select
    End Function

End Class