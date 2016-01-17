Public Class FileFolderEnum

    Private _Root As String
    Private _Files As List(Of String)
    Private _Folders As List(Of String)
    Private _TotalSize As Long
    Private _TotalFiles As Long
    Private _TotalFolders As Long

    Private Shared tblExcludedFiles As Hashtable
    Private Shared tblExcludedFolders As Hashtable

    Public ExcludedFiles As String = String.Empty
    Public ExcludedFolders As String = String.Empty


    Public Property Root() As String
        Get
            Return _Root
        End Get
        Set(ByVal value As String)
            _Root = value
        End Set
    End Property


    Public Property Files() As List(Of String)
        Get
            _Files.Sort()
            Return _Files
        End Get
        Set(ByVal value As List(Of String))
            _Files = value
        End Set
    End Property


    Public Property Folders() As List(Of String)
        Get
            _Folders.Sort()
            Return _Folders
        End Get
        Set(ByVal value As List(Of String))
            _Folders = value
        End Set
    End Property


    Public Property TotalSize() As Long
        Get
            Return _TotalSize
        End Get
        Set(ByVal value As Long)
            _TotalSize = value
        End Set
    End Property


    Public Property TotalFiles() As Long
        Get
            Return _TotalFiles
        End Get
        Set(ByVal value As Long)
            _TotalFiles = value
        End Set
    End Property


    Public Property TotalFolders() As Long
        Get
            Return _TotalFolders
        End Get
        Set(ByVal value As Long)
            _TotalFolders = value
        End Set
    End Property

    Private Sub LoadExclusions()
        tblExcludedFiles = New Hashtable
        tblExcludedFolders = New Hashtable

        If ExcludedFiles.Length > 0 Then
            For Each item In From item1 In ExcludedFiles.Split("|") Where item1.Length > 0
                tblExcludedFiles.Add(item, item)
            Next
        End If

        If ExcludedFolders.Length > 0 Then
            For Each item In From item1 In ExcludedFolders.Split("|") Where item1.Length > 0
                tblExcludedFolders.Add(item, item)
            Next
        End If
    End Sub


    Public Sub New()


        _Root = ""
        _Files = New List(Of String)
        _Folders = New List(Of String)
        _TotalSize = 0
        _TotalFiles = 0
        _TotalFolders = 0
        LoadExclusions()

    End Sub

    Public Sub New(ByVal Root As String)


        _Root = Root
        _Files = New List(Of String)
        _Folders = New List(Of String)
        _TotalSize = 0
        _TotalFiles = 0
        _TotalFolders = 0
        LoadExclusions()
        'Me.GetFiles(_Root)


    End Sub


    Public Sub GetFiles(ByVal path As String)

        LoadExclusions()

        Dim myDirectoryRoot As New DirectoryInfo(Path)
        Dim di As DirectoryInfo
        Dim fi As FileInfo
        Dim lSize As Long = 0
        Dim IncludeItem As Boolean = True

        Try
            For Each fi In myDirectoryRoot.GetFiles
                If Not (fi.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                    If Not fnMatchExclusions(IO.Path.GetFileNameWithoutExtension(fi.Name), tblExcludedFiles) = True Then
                        _Files.Add(fi.FullName)
                        _TotalFiles += 1
                        _TotalSize += fi.Length
                    Else
                        LogEvent("  File Skipped - " & fi.Name, EventLogLevel.Informational)
                    End If
                End If
            Next
        Catch ex As Exception
            LogEvent("  File Scan Error - " & ex.Message, EventLogLevel.ErrorEvent)
        End Try

        Try
            For Each di In myDirectoryRoot.GetDirectories()
                'Check folder not marked as hidden (don't recurse any more if so):
                If Not (di.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                    If Not fnMatchExclusions(di.Name, tblExcludedFolders) = True Then
                        _Folders.Add(di.FullName)
                        _TotalFolders += 1
                        GetFiles(di.FullName)
                    End If
                Else
                    LogEvent("  Directory skipped - " & di.Name, EventLogLevel.Informational)
                End If
            Next
        Catch ex As Exception
            LogEvent("  Directory Scan Error - " & ex.Message, EventLogLevel.ErrorEvent)
        End Try
        myDirectoryRoot = Nothing
    End Sub

    Private Function fnMatchExclusions(ByVal itemName As String, ByVal exclusionList As Hashtable) As Boolean

        'Return (From s As String In ExclusionList.Keys Select regCheck = New Regex(s.ToLower) Select regCheck.Match(itemName.ToLower)).Any(Function(m) m.Success)
        Return ExclusionList.Keys.Cast (Of String)().Any(Function(s) (itemName.ToLower.Contains(s.ToLower)))

        'Dim RegCheck As Regex
        'Dim m As Match

        'For Each blah as string In ExclusionList.Keys
        '    If blah.Length > 0 Then
        '        RegCheck = New Regex(blah.ToLower)
        '        m = RegCheck.Match(ItemName.ToLower)
        '        If m.Success Then
        '            ReturnValue = True
        '        End If
        '    End If
        'Next

        'For Each blah In From blah1 As String In ExclusionList.Keys Where blah1.Length > 0 Where itemName.ToLower.Contains(blah1.ToLower)
        '    return True
        'Next
    End Function

End Class

