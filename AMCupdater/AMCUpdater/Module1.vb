Imports System.Runtime.InteropServices
Imports System
Imports System.Collections
Imports System.Linq
Imports System.Globalization
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Forms
Imports System.Security.Cryptography
Imports DirectShowLib
Imports DirectShowLib.Dvd
Imports Microsoft.VisualBasic.Logging
Imports MediaPortal.Configuration
Imports System.Xml
Imports System.Timers
Imports System.Text

Public Module Module1

    Public WithEvents bgwFolderScanUpdate As New System.ComponentModel.BackgroundWorker
    Public CurrentSettings As New AntSettings
    Public CurrentLogDirectory As String
    Public wurl As New ArrayList
    Friend BufferedLogEvents As StringBuilder = New StringBuilder()


    'Private LogEventNew As NLog.Logger = NLog.LogManager.GetCurrentClassLogger() ' add nlog logging
    Public Watch As Stopwatch = New Stopwatch()

    Public Enum EventLogLevel As Integer
        None
        InformationalWithGrabbing
        Informational
        ImportantEvent
        ErrorEvent
        WriteBuffer
    End Enum

    Private Function GetFolderPath(ByVal FileName As String) As String
        ' returns the directory containing the file
        Dim ReturnValue As New DirectoryInfo(FileName)
        Return ReturnValue.Parent.FullName
    End Function

    Public Function DrvLabel(ByVal MediaPath As String) As String
        Dim Label As String = ""

        ' Try to get DVD-Drive-Label
        Dim _FilePath As String = MediaPath
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

    Public Enum VideoFormat
        DVD = 0
        Bluray = 1
        File = 2
    End Enum

    Public Function GetIdentifier(self As VideoFormat, videoPath As String) As String
        Dim hashID As String = Nothing
        If self = VideoFormat.DVD Then
            ' get the path to the video_ts folder
            Dim vtsPath As String = videoPath.ToLower().Replace("\video_ts.ifo", "\")
            ' This will get the microsoft generated DVD Disc ID
            Try
                ' get the disc id using the DirectShowLib method
                Dim dvdInfo As IDvdInfo2 = DirectCast(New DVDNavigator(), IDvdInfo2)
                Dim discID As Long = 0
                dvdInfo.GetDiscID(vtsPath, discID)
                ' if we got a disc id, we convert it to a hexadecimal string
                If discID <> 0 Then
                    hashID = Convert.ToString(discID, 16)
                End If
            Catch e As Exception
                'If e.[GetType]() = GetType(ThreadAbortException) Then
                'Throw e
                'End If
                'Log("Disc ID: Failed, Path='" & vtsPath & "', Format='" & self.ToString() & "' ", e)
            End Try
        ElseIf self = VideoFormat.Bluray Then
            ' Standard for the Bluray Disc ID is to compute a SHA1 hash from the key file (will only work for retail disks)
            Dim path As String = videoPath.ToLower()
            If path.EndsWith("bdmv\index.bdmv") Then
                Dim keyFilePath As String = path.Replace("bdmv\index.bdmv", "AACS\Unit_Key_RO.inf")
                If File.Exists(keyFilePath) Then
                    Dim keyFile As New FileInfo(keyFilePath)
                    hashID = keyFile.ComputeSHA1Hash()
                ElseIf File.Exists(videoPath) Then
                    hashID = String.Empty
                End If
            End If
        ElseIf self = VideoFormat.File Then
            Dim file__1 As New FileInfo(videoPath)
            hashID = file__1.ComputeSmartHash()
        End If

        ' Log the result
        If [String].IsNullOrEmpty(hashID) Then
            'logger.Debug("Failed Identifier: Path='{0}', Format='{1}' ", videoPath, self)
        Else
            'logger.Debug("Identifier: Path='{0}', Format='{1}', Hash='{2}' ", videoPath, self, hashID)
        End If

        ' Return the result
        Return hashID
    End Function

    ''' <summary>
    ''' Generates a SHA1-Hash from a given filepath
    ''' </summary>
    ''' <param name="self">path to the file</param>
    ''' <returns>hash as an hexadecimal string </returns>
    <Runtime.CompilerServices.Extension()>
    Private Function ComputeSHA1Hash(self As FileInfo) As String
        Dim hashHex As String = Nothing
        If self.Exists Then
            Dim file As Stream = Nothing
            Try
                file = self.OpenRead()
                Dim hashObj As HashAlgorithm = New SHA1Managed()
                Dim hash As Byte() = hashObj.ComputeHash(file)
                hashHex = hash.ToHexString()
                'logger.Debug("SHA1: Success, File='{0}', Hash='{1}'", self.FullName, hashHex)
            Catch e As Exception
                'If e.[GetType]() = GetType(ThreadAbortException) Then
                'Throw e
                'End If

                'logger.DebugException("SHA1: Failed, File='" & Convert.ToString(self.FullName) & "' ", e)
            Finally
                If file IsNot Nothing Then
                    file.Close()
                End If
            End Try
        Else
            ' File does not exist
            'logger.Debug("SHA1: Failed, File='{0}', Reason='File is not available'", self.FullName)
        End If

        ' Return
        Return hashHex
    End Function

    ''' <summary>
    ''' Converts a byte array to a hexadecimal string (hash)
    ''' </summary>
    ''' <param name="self"></param>
    ''' <returns>hexadecimal string</returns>
    <System.Runtime.CompilerServices.Extension()>
    Private Function ToHexString(self As Byte()) As String
        Dim hexBuilder As New StringBuilder()
        For i As Integer = 0 To self.Length - 1
            hexBuilder.Append(self(i).ToString("x2"))
        Next
        Return hexBuilder.ToString()
    End Function

    ''' <summary>
    ''' Calculates a unique hash for the contents of the file.
    ''' Use this method to compute hashes of large files.
    ''' </summary>
    ''' <param name="self"></param>
    ''' <returns>a unique hash or null when error</returns>
    <System.Runtime.CompilerServices.Extension()> _
    Private Function ComputeSmartHash(self As FileInfo) As String
        Dim hexHash As String = Nothing
        Dim bytes As Byte() = Nothing
        Try
            Using input As Stream = self.OpenRead()
                Dim lhash As ULong
                Dim streamsize As Long
                streamsize = input.Length
                lhash = CULng(streamsize)

                Dim i As Long = 0
                Dim buffer As Byte() = New Byte(8 - 1) {}
                input.Position = 0
                While i < 65536 \ 8 AndAlso (input.Read(buffer, 0, 8) > 0)
                    i += 1
                    lhash += BitConverter.ToUInt64(buffer, 0)

                End While

                input.Position = Math.Max(0, streamsize - 65536)
                i = 0
                While i < 65536 \ 8 AndAlso (input.Read(buffer, 0, 8) > 0)
                    i += 1
                    lhash += BitConverter.ToUInt64(buffer, 0)

                End While
                bytes = BitConverter.GetBytes(lhash)
                Array.Reverse(bytes)

                ' convert to hexadecimal string
                hexHash = bytes.ToHexString()
            End Using
        Catch e As Exception
            'logger.DebugException("Error computing smart hash: ", e)
        End Try
        Return hexHash
    End Function
    Public Function GetMovieNameFromDVDFolderPath(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a DVD image stored in a folder.
        'DVD files may be stored directly in a folder with the name of the movie.
        'DVD files may also retain their structure so .vob files will be in \VIDEO_TS\ - assume parent of that is movie name.

        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \VIDEO_TS.IFO
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves DVDs\ShawshankRedemption, the"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "video_ts" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If

            ReturnValue = TempString

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetBRFolderPath(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a BR image stored in a folder.
        'filename = BRs\IRON MAN\BDMV\index.bdmv
        'filename = BRs\IRON MAN\index.bdmv

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \index.bdmv
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves BRs\IRON MAN"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "bdmv" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If

            ReturnValue = TempString

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetDVDFolderName(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a DVD image stored in a folder.
        'DVD files may be stored directly in a folder with the name of the movie.
        'DVD files may also retain their structure so .vob files will be in \VIDEO_TS\ - assume parent of that is movie name.

        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \VIDEO_TS.IFO
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves DVDs\ShawshankRedemption, the"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "video_ts" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetBRFolderName(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a BR image stored in a folder.
        'filename = BRs\IRON MAN\BDMV\index.bdmv
        'filename = BRs\IRON MAN\index.bdmv

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \index.bdmv
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves BRs\IRON MAN"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "bdmv" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetDVDMovieFile(ByVal FileName As String) As String
        'Function to try and guess the correct movie file by getting the biggest from directory.
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

        Dim ReturnValue As String
        Dim directory As String

        If FileName.Contains("\") Then
            directory = FileName.Substring(0, FileName.LastIndexOf("\"))
        Else
            Return FileName
        End If

        Dim dir As New System.IO.DirectoryInfo(directory)
        Dim fileList As IEnumerable(Of System.IO.FileInfo) = dir.GetFiles("*.vob", System.IO.SearchOption.AllDirectories)

        ' Return the size of the largest file
        Dim maxSize = Aggregate aFile In fileList Into Max(GetFileLength(aFile))

        Dim filesByLengDesc = From file In fileList Let filelength = GetFileLength(file) Where filelength > 0 Order By filelength Descending Select file

        Dim longestFile As FileSystemInfo = filesByLengDesc.First

        ReturnValue = longestFile.FullName.ToString
        Return ReturnValue

    End Function

    Public Function GetBRMovieFile(ByVal FileName As String) As String
        'Function to try and guess the correct movie file by getting the biggest from directory.
        'filename = BRs\IRON MAN\BDMV\index.bdmv
        'filename = BRs\IRON MAN\index.bdmv

        Dim ReturnValue As String
        Dim directory As String

        If FileName.Contains("\") Then
            directory = FileName.Substring(0, FileName.LastIndexOf("\"))
        Else
            Return FileName
        End If

        'Take a snapshot of the folder contents
        Dim dir As New DirectoryInfo(directory)
        Dim fileList = dir.GetFiles("*.m2ts", SearchOption.AllDirectories)

        ' Return the size of the largest file
        Dim maxSize = Aggregate aFile In fileList Into Max(GetFileLength(aFile))

        'Dim maxSize = fileLengths.Max
        'Console.WriteLine("The length of the largest file under {0} is {1}", directory, maxSize)

        ' Return the FileInfo object of the largest file
        ' by sorting and selecting from the beginning of the list
        Dim filesByLengDesc = From file In fileList Let filelength = GetFileLength(file) Where filelength > 0 Order By filelength Descending Select file

        Dim longestFile = filesByLengDesc.First

        'Console.WriteLine("The largest file under {0} is {1} with a length of {2} bytes", directory, longestFile.FullName, longestFile.Length)


        'Try - didn't work ...
        'Dim FileInfo As FileSystemInfo = New DirectoryInfo(directory).GetFileSystemInfos().OrderByDescending(fi >= fi.Length).First()

        ReturnValue = longestFile.FullName
        Return ReturnValue

    End Function

    Function GetFileLength(ByVal fi As System.IO.FileInfo) As Long
        ' This method is used to catch the possible exception
        ' that can be raised when accessing the FileInfo.Length property.
        ' In this particular case, it is safe to ignore the exception.
        Dim retval As Long
        Try
            retval = fi.Length
        Catch ex As FileNotFoundException
            ' If a file is no longer present,
            ' just return zero bytes. 
            retval = 0
        End Try
        Return retval
    End Function

    Public Function StripPathFromFile(ByVal FilePath As String, ByVal ScanPath As String, Optional ByVal OverridePath As String = "") As String
        'Function to take a full file path and convert it to be relative to the scanned movie folder.
        'This should always receive an override path - if none specified it should be set to the scanned movie folder path.

        'e.g. If local move path to scan is c:\files\movies and the files should be stored in the Ant Catalog as \\server\movies\
        'then it uses the OverridePath ('\\server\movies\') in place of the physical root.  If the file path doesn't contain
        'the given override path then it just crops from the last \ character to the end.
        'Also optionally trims off the file extension (not yet implemented).

        If OverridePath <> String.Empty Then
            ScanPath = OverridePath
        End If

        'If FilePath.IndexOf(OverridePath) > -1 Then
        'FilePath = Replace(FilePath, OverridePath, "")
        'Else
        'FilePath = FilePath.Substring(InStrRev(FilePath, "\"))
        'End If

        'This is case sensitive!
        'FilePath = Replace(FilePath.ToLower, ScanPath.ToLower, "")
        FilePath = Replace(FilePath, ScanPath, "", 1, -1, CompareMethod.Text)

        'FilePath = "c:\Movies\Learners 1\"
        'ScanPath = "C:\Movies\"
        'OverridePath = ""

        Return FilePath
    End Function

    Public Function GetNextMovieID(ByVal FilePath As String)
        'Function to access the given XML database file and parse it to find the next largest available MovieNumber (unique key) value.

        Dim CurrentMax As Integer = 0
        If FilePath <> "" Then
            If File.Exists(FilePath) Then
                Dim textReader As Xml.XmlTextReader = New Xml.XmlTextReader(FilePath)

                While textReader.Read()
                    If textReader.Name = "Movie" Then
                        If textReader.GetAttribute("Number") > CurrentMax Then
                            CurrentMax = textReader.GetAttribute("Number")
                        End If
                    End If
                End While

                If Not (textReader Is Nothing) Then
                    textReader.Close()
                End If
            End If
            CurrentMax = CurrentMax + 1
            Return CurrentMax
        Else
            LogEvent("ErrorEvent - Cannot access file " & FilePath & " to get next available Movie ID.", EventLogLevel.ErrorEvent)
            Return 0
        End If

    End Function

    Public Function GetTitleFromFilePath(ByVal FilePath As String)
        'File Name
        'Folder Name
        'File Name + Internet Lookup 
        'Folder Name + Internet Lookup

        Dim CleanString As String = ""

        If CurrentSettings.Series_Name_Identifier.Length > 0 And FilePath.Contains(CurrentSettings.Series_Name_Identifier) Then ' only, if identified as "series".
            'Strip Path
            CleanString = FilePath.Substring(FilePath.LastIndexOf("\") + 1)
            'Strip Extension
            CleanString = CleanString.Substring(0, CleanString.LastIndexOf("."))
        ElseIf CurrentSettings.Movie_Title_Handling.Contains("File Name") Then
            'Strip Path
            CleanString = FilePath.Substring(FilePath.LastIndexOf("\") + 1)
            'Strip Extension
            CleanString = CleanString.Substring(0, CleanString.LastIndexOf("."))
        ElseIf CurrentSettings.Movie_Title_Handling.Contains("Folder Name") Then
            If FilePath.Contains("\") Then
                'Strip filename:
                CleanString = FilePath.Substring(0, FilePath.LastIndexOf("\"))
                'Strip Path:
                CleanString = CleanString.Substring(CleanString.LastIndexOf("\") + 1)
            Else
                CleanString = FilePath
            End If
        Else
            CleanString = FilePath
        End If

        Dim CutText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
        Dim m As Match
        m = CutText.Match(CleanString)
        If m.Success = True Then
            'Finally remove anything which may be a multi-part indicator (e.g. 1of2)
            CleanString = CutText.Replace(CleanString, "")
        End If
        CutText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        m = CutText.Match(CleanString)
        If m.Success = True Then
            CleanString = CutText.Replace(CleanString, "")
        End If

        'If CurrentSettings.Scan_For_DVD_Folders = True Then
        If FilePath.ToLower.Contains("video_ts") = True Then
            CleanString = GetDVDFolderName(FilePath)
        End If
        If FilePath.ToLower.Contains("index.bdmv") = True Then
            CleanString = GetBRFolderName(FilePath)
        End If
        'End If


        'Tidy up any trailing spaces:
        CleanString = CleanString.Trim

        CleanString = RemoveNastyCharacters(CleanString)

        Return CleanString

    End Function

    Public Function GetYearFromFilePath(ByVal FilePath As String)
        'File Name
        'Folder Name
        'File Name + Internet Lookup 
        'Folder Name + Internet Lookup

        Dim CleanString As String = FilePath

        ' remove IMDB ID from text
        Dim CutIMDB As New Regex("tt\d{7}")
        Dim imdb As Match
        imdb = CutIMDB.Match(CleanString)
        If imdb.Success = True Then
            CleanString = CleanString.Replace(imdb.Value, "")
        End If

        ' Check different methods to get year from CleanString
        Dim MatchList As MatchCollection
        Dim matcher As Match

        Dim p As New Regex("\(" & "[0-9]{4}" & "\)") ' year in brackets
        MatchList = p.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value.Substring(1, 4)
        End If

        Dim p1 As New Regex("[^0-9][0-9]{4}[^0-9p]") ' year in non numbers
        MatchList = p1.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value.Substring(1, 4)
        End If

        Dim p2 As New Regex("[^0-9][0-9]{4}[^0-9]") ' year in non numbers
        MatchList = p2.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value.Substring(1, 4)
        End If

        Dim p3 As New Regex("[0-9]{4}") ' year just as 4-digit
        MatchList = p3.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value
        End If
        Return "" ' if nothing could be found ...


        ''Dim CutText As New Regex("\(" & "([^)]*)" & "\)")
        'Dim CutText As New Regex("[^0-9][0-9]{4}[^0-9]")
        'Dim m As Match
        'm = CutText.Match(CleanString)
        'If m.Success = True Then
        '    Return m.Value.Substring(1, 4) 'remove ()
        'Else
        '    ' remove IMDB ID from text
        '    Dim CutIMDB As New Regex("tt\d{7}")
        '    Dim imdb As Match
        '    imdb = CutIMDB.Match(CleanString)
        '    If imdb.Success = True Then
        '        CleanString = CleanString.Replace(imdb.Value, "")
        '    End If

        '    Dim CutText2 As New Regex("[0-9]{4}")
        '    Dim m2 As Match
        '    m2 = CutText2.Match(CleanString)
        '    If m2.Success = True Then
        '        Return m2.Value
        '    Else
        '        Return ""
        '    End If
        'End If

        'Tidy up any trailing spaces:
        CleanString = CleanString.Trim
        CleanString = RemoveNastyCharacters(CleanString)
        Return CleanString
    End Function

    Public Function GetIMDBidFromFilePath(ByVal FilePath As String)
        Dim CleanString As String = ""

        CleanString = FilePath

        Dim CutText As New Regex("" & "tt\d{7}" & "")
        Dim m As Match
        m = CutText.Match(CleanString)
        If m.Success = True Then
            Return m.Value
        Else
            Return ""
        End If
    End Function

    Public Function RemoveNastyCharacters(ByVal strText As String)

        Dim blah As String
        Dim RegCheck As Regex
        Dim NewText As String

        For Each blah In CurrentSettings.Filter_Strings.Split("|")
            If blah.Length > 0 Then
                'Check for RegEx expression special characters:
                'If blah = "." Or blah = "'" Or blah = "^" Or blah = "$" Or blah = "?" Or blah = "*" Or blah = "+" Or blah = "|" Then
                'blah = "\" & blah
                'End If
                If blah.Length = 1 Then
                    'Probably not a regex, due to complexity of any single character, just do a replace.
                    NewText = strText.Replace(blah, " ")
                Else
                    'This should work for any multi-character string:
                    RegCheck = New Regex(blah)
                    NewText = RegCheck.Replace(strText, " ")
                End If
                If NewText.Trim.Length > 0 Then
                    'Check to ensure last operation didn't wipe the string out!
                    strText = NewText
                Else
                    'If NewText is blank, then exit here with the previous value of strText
                    Exit For
                End If
            End If
        Next

        'Tidy up the beginning and end of the string:
        strText = strText.Trim

        'Loop through to remove any groups of spaces left by character replacement:
        If strText.Contains("  ") = True Then
            While strText.Contains("  ") = True
                strText = strText.Replace("  ", " ")
            End While
        End If

        Return strText
    End Function

    Public Function GetFileData(ByVal FilePath As String, ByVal DataItem As String)
        'Function to retreive information from the given file.

        Dim Directory As Boolean = False
        Dim DirectoryPath As String = ""
        If FilePath.ToLower.EndsWith("video_ts.ifo") = True Then
            Directory = True
            DirectoryPath = GetFolderPath(FilePath)
            FilePath = GetDVDMovieFile(FilePath)
        End If
        If FilePath.ToLower.EndsWith("index.bdmv") = True Then
            Directory = True
            DirectoryPath = GetFolderPath(FilePath)
            FilePath = GetBRMovieFile(FilePath)
        End If

        Dim ReturnValue As String = ""
        Dim TempInteger As Long = 0
        Dim TempString As String = ""
        Dim MI As MediaInfo = New MediaInfo
        Dim i As Integer = 0
        If Not File.Exists(FilePath) Then
            Return "ErrorEvent : File cannot be found"
            LogEvent("ErrorEvent - Cannot open file for analysis - " & FilePath, EventLogLevel.ErrorEvent)
            Exit Function
        End If

        Dim f As New IO.FileInfo(FilePath)

        Select Case DataItem.ToLower

            'Guzzi Test'
            Case "beschreibung"
                Try
                    TempString = "Test-Guzzi-Beschreibung"
                    ReturnValue = TempString

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, 3)
                    ReturnValue = ""
                End Try

            Case "filename"
                Try
                    'ReturnValue = MI.Get_(StreamKind.General, 0, "FileName")
                    TempString = f.Name
                    'Console.WriteLine(f.Extension)
                    TempString = TempString.Replace(f.Extension, "")
                    'Put this bit in here to remove the '1of2' type bits using the system variable regex expression.
                    Dim SplitText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
                    TempString = SplitText.Replace(TempString, "")
                    SplitText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
                    TempString = SplitText.Replace(TempString, "")
                    ReturnValue = TempString

                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try

            Case "runtime"
                Try
                    If Directory Then
                        ReturnValue = "ErrorEvent"
                    Else
                        MI = New MediaInfo
                        MI.Open(FilePath)
                        'PlayTime value is in miliseconds!
                        TempString = MI.Get_(StreamKind.General, 0, "PlayTime")
                        MI.Close()

                        Dim r As Boolean = Integer.TryParse(TempString, TempInteger)
                        If r Then
                            If TempInteger <> 0 Then
                                ReturnValue = CLng(TempInteger / 60000).ToString
                            Else
                                ReturnValue = ""
                            End If
                        Else
                            ReturnValue = "ErrorEvent"
                        End If
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = "ErrorEvent"
                End Try
            Case "videoformat"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "Codec/String")
                    MI.Close()
                    If TempString <> "" Then
                        ReturnValue = TempString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "videobitrate" 'divide by 1000 as returned in bps.
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "BitRate")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger <> 0 Then
                        ReturnValue = CInt(TempInteger / 1000).ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audioformat"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.Audio, 0, "Codec/String")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audiostreamcount"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "AudioCount")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audiostreamcodeclist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "Audio_Codec_List").Replace(" / ", ", ")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audiostreamlanguagelist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    'First get the count if possible
                    TempString = ""
                    Integer.TryParse(MI.Get_(StreamKind.General, 0, "AudioCount"), TempInteger)
                    If TempInteger > 0 Then
                        For i = 0 To TempInteger - 1
                            TempString = ""
                            'Try to get the 'proper' language for this stream:
                            TempString = MI.Get_(StreamKind.Audio, i, "Language/String")
                            If TempString = "" Then
                                'If not, check the IAS value - maybe has a language string there:
                                TempString = MI.Get_(StreamKind.General, 0, "IAS" & (i + 1).ToString)
                            End If
                            If TempString <> "" Then
                                'Build the list:
                                If ReturnValue = "" Then
                                    ReturnValue = TempString
                                Else
                                    ReturnValue += ", " & TempString
                                End If
                            End If
                            If ReturnValue = "" Then
                                'Still no value, maybe just put in the number of audio streams?
                                If MI.Get_(StreamKind.General, 0, "AudioCount") <> "1" Then
                                    ReturnValue = MI.Get_(StreamKind.General, 0, "AudioCount").ToString
                                End If
                            End If
                        Next
                    Else
                        'Cannot even get the count of streams - return empty:
                        ReturnValue = ""
                    End If
                    MI.Close()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audiobitrate" 'divide
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Audio, 0, "BitRate")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger <> 0 Then
                        ReturnValue = CInt((TempInteger / 1000)).ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "audiochannelcount"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Audio, 0, "Channel(s)")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger > -1 Then
                        ReturnValue = TempInteger.ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try

            Case "3d"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "MultiView_Count")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger > 1 Then
                        ReturnValue = "3D"
                    ElseIf FilePath.Contains("3DSBS") Then
                        ReturnValue = "3DSBS"
                    ElseIf FilePath.Contains("3DTAB") Then
                        ReturnValue = "3DTAB"
                    ElseIf FilePath.Contains("(3D)") Then
                        ReturnValue = "3D"
                    Else 
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "textstreamcodeclist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "Text_Codec_List").Replace(" / ", ", ")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "textstreamlanguagelist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    Integer.TryParse(MI.Get_(StreamKind.General, 0, "TextCount"), TempInteger)
                    If TempInteger <> 0 Then
                        For i = 0 To TempInteger - 1
                            TempString = ""
                            TempString = MI.Get_(StreamKind.Text, i, "Language/String")
                            If TempString <> "" Then
                                'Build the string
                                If ReturnValue = "" Then
                                    ReturnValue = TempString
                                Else
                                    ReturnValue += ", " & TempString
                                End If
                                'Check for a subtitle description:
                                TempString = MI.Get_(StreamKind.Text, i, "Title")
                                If TempString <> "" Then
                                    'Clean up the title a bit:
                                    TempString = TempString.Replace("<", "")
                                    TempString = TempString.Replace(">", "")
                                    TempString = TempString.Replace("(", "")
                                    TempString = TempString.Replace(")", "")
                                    ReturnValue += " (" & TempString & ")"
                                End If
                            End If
                        Next
                    End If
                    MI.Close()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try

            Case "resolution"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "Width") & "x" & MI.Get_(StreamKind.Visual, 0, "Height")
                    MI.Close()
                    If TempString = "x" Then
                        TempString = ""
                    End If
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "framerate"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "FrameRate")
                    MI.Close()
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "filesize" 'get in MB = divide by 1024 twice
                Try
                    If Directory Then
                        Dim d As New IO.DirectoryInfo(DirectoryPath)
                        TempInteger = getDirectorySize(d)
                    Else
                        TempString = CStr(f.Length)
                        TempInteger = CLng(TempString)
                    End If
                    ReturnValue = CLng((TempInteger / 1048576)).ToString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "date"
                Try
                    Select Case CurrentSettings.Date_Handling
                        'File Created Date
                        'Current System Date
                        'No Date
                        Case "File Created Date"
                            ReturnValue = f.CreationTime.Date 'String.Format("{0:yyyy-MM-dd}", f.CreationTime.Date) '0:yyyy/MM/dd
                        Case "File Modified Date"
                            ReturnValue = f.LastWriteTime.Date 'String.Format("{0:yyyy-MM-dd}", f.LastWriteTime.Date)
                        Case "Current System Date"
                            ReturnValue = My.Computer.Clock.LocalTime.Date 'String.Format("{0:yyyy-MM-dd}", My.Computer.Clock.LocalTime.Date)
                        Case "No Date"
                            ReturnValue = String.Empty
                        Case Else
                            ReturnValue = String.Empty
                    End Select
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case "aspectratio"
                'PixelAspectRatio/String;;;N NT;;;Pixel Aspect ratio
                'PixelAspectRatio_Original/String;;;N NT;;;Original (in the raw stream) Pixel Aspect ratio
                ' decimal ar -> 'DisplayAspectRatio;;;N YF;;;Display Aspect ratio
                ' string, e.g. "16:9" -> ""'DisplayAspectRatio/String;;;Y NT;;;Display Aspect ratio
                'DisplayAspectRatio_Original/String;;;Y NT;;;Original (in the raw stream) Display Aspect ratio

                Try
                    Dim style As NumberStyles
                    style = NumberStyles.AllowDecimalPoint 'Or NumberStyles.Float
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "DisplayAspectRatio") ' & " (" & MI.Get_(StreamKind.Visual, 0, "DisplayAspectRatio/String") & ")"
                    Dim aspectratio As Decimal
                    If Decimal.TryParse(TempString, style, CultureInfo.InvariantCulture, aspectratio) Then
                        TempString = Decimal.Round(aspectratio, 2).ToString(CultureInfo.InvariantCulture)
                    End If
                    ' if (aspectratio < (decimal)(1.4)) movieRow.Aspectratio = "4:3"; //1,33 -> 4:3
                    ' else if (aspectratio < (decimal)(1.9)) movieRow.Aspectratio = "16:9"; //1,78 -> 16:9 / widescreen //1,85 -> widescreen
                    ' else if (aspectratio >= (decimal)(1.9)) movieRow.Aspectratio = "cinemascope"; //2,35+ -> cinemascope

                    MI.Close()
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try
            Case Else
                ReturnValue = "Unknown Variable Requested"
        End Select
        'Console.WriteLine(DataItem.ToString + " - " + ReturnValue.ToString)
        '        MI.Close()
        If MI IsNot Nothing Then
            MI = Nothing
        End If

        Return ReturnValue

    End Function

    Public Function getDirectorySize(ByVal dir As IO.DirectoryInfo) As Long
        Return dir.GetDirectories().Sum(Function(_d) getDirectorySize(_d)) + dir.GetFiles().Sum(Function(_f) CLng(CStr(_f.Length)))
    End Function

    Public Function GetHTMLFileData(ByVal FilePath As String, ByVal DataItem As String)
        'Guzzi: Function to retreive information from the HTML-Files in movie directory...
        'Search for goldesel 2004.htm, index.php.htm, Detail.aspx.htm, goldesel.to - quality source for more than 10 years.htm 
        Dim ReturnValue As String = ""
        Dim TempInteger As Long = 0
        Dim TempString As String = ""
        Dim Directoryname As String = ""
        Dim HTMLfilename As String = ""
        'Dim MI As MediaInfo = New MediaInfo
        Dim i As Integer = 0

        Directoryname = fnGetFilePath(FilePath)

        If System.IO.File.Exists(Directoryname + "\" + "goldesel 2004.htm") Then
            HTMLfilename = Directoryname + "\" + "goldesel 2004.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "index.php.htm") Then
            HTMLfilename = Directoryname + "\" + "index.php.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "Detail.aspx.htm") Then
            HTMLfilename = Directoryname + "\" + "Detail.aspx.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "goldesel.to - quality source for more than 10 years.htm") Then
            HTMLfilename = Directoryname + "\" + "goldesel.to - quality source for more than 10 years.htm"
        End If

        If Not File.Exists(HTMLfilename) Then
            LogEvent("ErrorEvent - Cannot open file for analysis - " & HTMLfilename, EventLogLevel.ErrorEvent)
            Return "ErrorEvent : File " + HTMLfilename + " cannot be found"
            Exit Function
        End If

        Dim f As New FileInfo(HTMLfilename)

        Select Case DataItem.ToLower

            'Guzzi Test'
            Case "beschreibung"
                Try
                    TempString = "Test-Guzzi-Beschreibung"
                    ReturnValue = TempString

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, 3)
                    ReturnValue = ""
                End Try

            Case "description"
                'GUZZI: Added to get the Content of the HTML xxx.htm File !!!
                Dim textReader As String
                'LogEvent("fnGetDescription: Try to read Description from index.php.htm for File: " + FilePath, 2)
                Try
                    textReader = My.Computer.FileSystem.ReadAllText(FilePath, System.Text.Encoding.UTF7)
                    LogEvent("fnGetDescription: Parsing complete for  " + FilePath, 2)
                    Dim SearchString As String = textReader
                    Dim SearchChar As String = "<b>Beschreibung:"
                    Dim StartPos As Integer
                    StartPos = InStr(1, SearchString, SearchChar, CompareMethod.Text)
                    Dim MidWords As String = Mid(textReader, StartPos + 20)
                    Dim EndPos As Integer
                    SearchChar = "</td>"
                    EndPos = InStr(1, MidWords, SearchChar, CompareMethod.Text)
                    Dim MidDescription As String = Microsoft.VisualBasic.Left(MidWords, EndPos - 1)
                    Dim Guzzidescription As String = Regex.Replace(MidDescription, "\r\n", " ")
                    Dim Guzzidescription2 As String = Regex.Replace(Guzzidescription, "\<[^\>]+\>", "")
                    'MsgBox(Guzzidescription2)
                    ReturnValue = Guzzidescription2
                Catch ex As Exception
                    LogEvent("fnGetDescription: ErrorEvent - Cannot parse " + FilePath, 3)
                End Try

            Case "filename"
                Try
                    'ReturnValue = MI.Get_(StreamKind.General, 0, "FileName")
                    TempString = f.Name
                    'Console.WriteLine(f.Extension)
                    TempString = TempString.Replace(f.Extension, "")
                    'Put this bit in here to remove the '1of2' type bits using the system variable regex expression.
                    Dim splitText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
                    TempString = splitText.Replace(TempString, "")
                    splitText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
                    TempString = splitText.Replace(TempString, "")
                    ReturnValue = TempString

                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try

            Case "date"
                'GUZZI: Added to get Content (Date Added) of the index.php.htm File !!!
                'Dim FilePath As String = ""
                'Dim ReturnValue As String = ""
                Dim textReader As String = ""
                Dim FileName As String = ""
                Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
                FileName = FileName.Replace(FileNameEnd, "")
                FilePath = FileName + "\index.php.htm"
                Dim fi As New IO.FileInfo(FilePath)
                If Not fi.Exists Then
                    LogEvent("fnGetDateAdded: ErrorEvent - Cannot open file for analysis: " & FilePath, 3)
                    Return "ErrorEvent : File " + FilePath + " cannot be found"
                    Exit Function
                End If
                Try
                    textReader = My.Computer.FileSystem.ReadAllText(FilePath, System.Text.Encoding.UTF7)
                    'MsgBox(textReader)
                    LogEvent("fnGetDateAdded: Parsing complete for  " + FilePath, 2)
                    Dim SearchString As String = textReader
                    Dim SearchChar As String = "<b>Eintragsdatum:</b>"
                    Dim StartPos As Integer
                    StartPos = InStr(1, SearchString, SearchChar, CompareMethod.Text)
                    Dim MidWords As String = Mid(textReader, StartPos + 3)
                    ' MsgBox(MidWords)
                    Dim EndPos As Integer
                    SearchChar = "</td>"
                    EndPos = InStr(20, MidWords, SearchChar, CompareMethod.Text)
                    Dim MidDescription As String = Microsoft.VisualBasic.Left(MidWords, EndPos - 1)
                    Dim Guzzidescription As String = Regex.Replace(MidDescription, "\r\n", " ")
                    Dim Guzzidescription2 As String = Regex.Replace(Guzzidescription, "\<[^\>]+\>", "")
                    Dim Guzzidescription3 As String = Guzzidescription2.Substring(InStrRev(Guzzidescription2, " "))
                    ReturnValue = Guzzidescription3
                Catch ex As Exception
                    LogEvent("fnGetDateAdded: ErrorEvent - Cannot parse " + FilePath, 3)
                    'Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                    ReturnValue = ""
                End Try

            Case Else
                ReturnValue = "Unknown Variable Requested"
        End Select
        'Console.WriteLine(DataItem.ToString + " - " + ReturnValue.ToString)
        '        MI.Close()
        Return ReturnValue

    End Function

    Public Function fnGetFolderName(ByVal FileName As String) As String
        'GUZZI: Added to get the Root Folder Name of a Moviefile !!!
        Dim TempString As String
        Dim ReturnValue As String
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
        FileName = FileName.Replace(FileNameEnd, "")
        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "video_ts" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue
    End Function

    Public Function fnGetFilePath(ByVal FileName As String) As String
        'GUZZI: Added to get the Root Folder Name of a Moviefile !!!
        Dim ReturnValue As String
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
        ReturnValue = FileName.Replace(FileNameEnd, "")
        Return ReturnValue
    End Function

    Public Function GetGroupName(ByVal FilePath As String, ByVal Movie_Title_Handling As String, ByVal Group_Name_Identifier As String, ByVal Series_Name_Identifier As String)
        Dim ReturnValue As String = String.Empty

        '2001\2001 - A Space Odyssey.avi
        '2010\2010.avi
        'Cloverfield\Cloverfield 1of2.avi
        'Cloverfield\Cloverfield 2of2.avi
        'Elizabeth - The Golden Age\Elizabeth - The Golden Age.avi
        'Mist, the\Mist, the.avi
        'Oxford Murders\The Oxford Murders.avi
        'Peliculas\Resident Evil\Resident Evil 2 Apocalypse.avi
        'Peliculas\Resident Evil\Resident Evil Extinction.avi
        'Peliculas\Resident Evil\Resident Evil.avi
        'Romeo + Juliet\Romeo + Juliet.avi
        'Run Lola Run\Run Lola Run (1998).avi
        'Schindler's List\Schindler's List.avi
        'The Matrix Trilogy\The Matrix Reloaded.mkv
        'The Matrix Trilogy\The Matrix Revisited (1of2).mkv
        'The Matrix Trilogy\The Matrix Revisited (2of2).mkv
        'The Matrix Trilogy\The Matrix.mkv

        If FilePath.Contains("\") = False Then
            Return ReturnValue
        End If
        ' If parts can be found...
        If Group_Name_Identifier.Length = 0 And Series_Name_Identifier.Length = 0 Then ' no search expression defined - use standard rules
            If FilePath.Split("\").Length = 2 Then 'Just a folder and a filename it seems - use the parent.
                ReturnValue = FilePath.Substring(0, FilePath.IndexOf("\"))
            Else
                Dim FolderNames As String() = FilePath.Split("\")
                'We should now have at least 3 strings, the last of which will be the filename.  Let's use the one before that:
                ' Todo depends on Title handling ...
                'File(Name)
                'Folder(Name)
                'Relative(Name)

                If Movie_Title_Handling.Contains("Folder") And FolderNames.Length > 2 Then ' if it's "folder handling", use parent folder of the movie folder
                    ReturnValue = FolderNames(FolderNames.Length - 3)
                Else
                    ReturnValue = FolderNames(FolderNames.Length - 2) ' use movie folder itself
                End If
            End If

        Else
            If Group_Name_Identifier.Length > 0 Then ' search expression for collection found - search for last match
                'Dim FolderNames As String() = FilePath.Split("\")
                'For i As Integer = LBound(FolderNames) To UBound(FolderNames) Step 1
                '    Dim Part As String = FolderNames(i)
                '    If Part.Contains(Group_Name_Identifier) Then
                '        ReturnValue = Part.Replace(Group_Name_Identifier, "").Trim
                '    End If
                'Next
                Dim FolderNames As String() = FilePath.Split("\")
                Dim StartAdding As Boolean = False
                Dim PartialGroupName As String = ""
                Dim MaxFolderPosition As Integer
                If FolderNames.Length = 2 Then
                    MaxFolderPosition = 1
                ElseIf Movie_Title_Handling.Contains("Folder") And FolderNames.Length > 2 Then
                    MaxFolderPosition = FolderNames.Length - 3 ' parent folder of movie folder
                Else
                    MaxFolderPosition = FolderNames.Length - 2 ' movie folder itself
                End If

                For i As Integer = LBound(FolderNames) To MaxFolderPosition Step 1 'For i As Integer = LBound(FolderNames) To UBound(FolderNames) Step 1
                    Dim Part As String = FolderNames(i)
                    If Part.Contains(Group_Name_Identifier) Then
                        PartialGroupName = Part.Replace(Group_Name_Identifier, "").Trim
                        StartAdding = True
                    Else
                        PartialGroupName = Part
                    End If
                    If StartAdding = True Then
                        If (ReturnValue.Length > 0) Then
                            ReturnValue = ReturnValue & "\" & PartialGroupName
                        Else
                            ReturnValue = PartialGroupName
                        End If
                    End If
                Next
                ' finally we have the last found match, use it as ReturnValue
            End If
            If Series_Name_Identifier.Length > 0 And ReturnValue.Length = 0 Then ' only, if no Group found before ...
                Dim FolderNames As String() = FilePath.Split("\")
                If FolderNames.Length < 2 Then
                    Return ""
                End If
                Dim StartAdding As Boolean = False
                Dim PartialGroupName As String = ""

                For i As Integer = LBound(FolderNames) To (UBound(FolderNames) - 1) Step 1
                    Dim Part As String = FolderNames(i)
                    If Part.Contains(Series_Name_Identifier) Then
                        PartialGroupName = Part.Replace(Series_Name_Identifier, "").Trim
                        StartAdding = True
                    Else
                        PartialGroupName = Part
                    End If
                    If StartAdding = True Then
                        If (ReturnValue.Length > 0) Then
                            ReturnValue = ReturnValue & "\" & PartialGroupName
                        Else
                            ReturnValue = PartialGroupName
                        End If
                    End If
                Next
                ' finally we have the first found match including the rest if the path as hierarchy, use it as ReturnValue
            End If
        End If
        Return ReturnValue
    End Function

    Public Function GetEdition(ByVal FilePath As String)
        Dim ReturnValue As String = ""

        Dim RegCheck As Regex
        Dim Matches As MatchCollection

        For Each Rules In CurrentSettings.Edition_Strings.Split(";")
            Dim Rule() As String = Rules.Split("|")
            Dim EditionSearchExpression As String = Rule(0).ToString
            Dim EditionReplaceExpression As String = Rule(1).ToString
            Dim EditionFound As Boolean = False

            If EditionSearchExpression.Length > 0 Then
                If EditionSearchExpression.Length = 1 Then 'Probably not a regex, due to complexity of any single character, just do a replace.
                    If FilePath.Contains(EditionSearchExpression) Then
                        EditionFound = True
                    End If
                Else
                    RegCheck = New Regex(EditionSearchExpression) 'This should work for any multi-character string:
                    Matches = RegCheck.Matches(FilePath)
                    If (Matches.Count > 0) Then
                        EditionFound = True
                    End If
                End If
            End If
            If (EditionFound = True) Then
                ReturnValue = AddToCommaList(ReturnValue, EditionReplaceExpression)
            End If
        Next

        '[sS]tandard|Standard Edition;[eE]xtended|Extended Edition;[cC]ollector|Collectors Edition;[dD]irector|Directors Cut;[uU]nrated|Unrated
        'If FilePath.ToLower.Contains("standard") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Standard Edition")
        'End If

        'If FilePath.ToLower.Contains("extended") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Extended Edition")
        'End If

        'If FilePath.ToLower.Contains("collector") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Collectors Edition")
        'End If

        'If FilePath.ToLower.Contains("director") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Directors Cut")
        'End If

        'If FilePath.ToLower.Contains("unrated") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Unrated")
        'End If

        Return ReturnValue
    End Function

    Public Function AddToCommaList(ByVal CommaList As String, ByVal Value As String) As String

        If CommaList.Length = 0 Then
            CommaList = Value
        Else
            CommaList = CommaList + ", " + Value
        End If
        Return CommaList
    End Function

    <STAThread()> _
    Public Sub LogEvent(ByVal EventString As String, ByVal LogLevel As EventLogLevel)

        If LogLevel = EventLogLevel.WriteBuffer Then
            'If (Not DgLogWindow.IsHandleCreated And Not DgLogWindow.IsDisposed) Then Return
            dgLogWindow.txtLogWindow.AppendText(BufferedLogEvents.ToString())
            BufferedLogEvents.Length = 0 '.Remove(0, BufferedLogEvents.Length)
            'Form1.aTimer.Change(Timeout.Infinite, Timeout.Infinite)
            watch.Reset()
            watch.Start()
            Return
        End If

        'LogLevels...
        ' 0 = None (no logging)
        ' 1 = InformationalWithGrabbing
        ' 2 = Informational
        ' 3 = Verbose (most events)
        ' 4 = Debug (all events)
        ' Errors = 3
        ' Major operations = 2
        ' Individual Files = 1

        'CurrentSettings values:
        'All Events with Grabbing
        'All Events
        'Major Events
        'ErrorsOnly

        Dim CurrentLogName As String = CurrentSettings.Log_Level
        Dim LogItem As Boolean = False
        If CurrentLogName = "All Events with Grabbing" Then
            LogItem = True
        ElseIf CurrentLogName = "All Events" Then
            If LogLevel >= 2 Then
                LogItem = True
            End If
        ElseIf CurrentLogName = "Major Events" Then
            If LogLevel >= 3 Then
                LogItem = True
            End If
        ElseIf CurrentLogName = "Errors Only" Then
            If LogLevel >= 4 Then
                LogItem = True
            End If
        End If

        'Sub to write log information.  If the GUI is running then it also appends to the txtProcess box.
        Dim LogText As String = My.Computer.Clock.LocalTime.ToString + " - " + EventString

        If LogItem = True Then
            Try
                My.Computer.FileSystem.WriteAllText(CurrentLogDirectory, vbCrLf + LogText, True)
            Catch ex As Exception
            End Try
        End If

        If Form1.Visible = True Then
            'DgLogWindow.txtLogWindow.AppendText(LogText & vbCrLf)
            'LogEventNew.Debug(LogText & vbCrLf)
            BufferedLogEvents.AppendLine(LogText) 'BufferedLogEvents += LogText & vbCrLf
            If watch.Elapsed.TotalMilliseconds > 200 Then
                dgLogWindow.txtLogWindow.AppendText(BufferedLogEvents.ToString())
                BufferedLogEvents.Length = 0 '.Remove(0, BufferedLogEvents.Length)
                watch.Reset()
                watch.Start()
                'Form1.aTimer.Change(Timeout.Infinite, Timeout.Infinite)
            Else
                'Form1.aTimer.Change(1000, Timeout.Infinite) ' call buffercheck once in 1 second, if nothing else happens ...
            End If
            If LogLevel = EventLogLevel.ImportantEvent Then
                Form1.ToolStripStatusLabel.Text = EventString
            End If
        End If
    End Sub

    Public Sub LogEventCheckBuffer()
        If BufferedLogEvents.Length > 0 And watch.Elapsed.TotalMilliseconds > 500 Then
            'LogEvent("", EventLogLevel.WriteBuffer) ' write buffer in sta thread ...
            dgLogWindow.txtLogWindow.AppendText(BufferedLogEvents.ToString())
            BufferedLogEvents.Length = 0 '.Remove(0, BufferedLogEvents.Length)
            watch.Reset()
            watch.Start()
        End If
    End Sub

    '  ''' <summary>
    '  ''' Diese Methode wird von einem Background Thread (nicht UI Thread) aufgerufen
    '  ''' </summary>
    '  ''' <param name="max">Maximale Anzahl der Durchläufe</param>
    '  Private Sub BackgroundThreadAction(ByVal max As Integer)
    '      If max < 0 Then
    'InvokeIfRequired(Me.textBoxStatus, DirectCast(Sub() Me.textBoxStatus.Text = "max value must be greater than zero", MethodInvoker))
    '          Return
    '      End If
    '      For i As Integer = 0 To max - 1
    'InvokeIfRequired(Me.textBoxStatus, DirectCast(Sub() Me.textBoxStatus.Text = [String].Format("Processing value {0}", i), MethodInvoker))
    '          Thread.Sleep(10)
    '      Next
    '  End Sub
    Public Enum ButtonStatus
        ReadyToParseXML = 0
        ReadyToSearchFolders = 1
        ReadyToFindOrphans = 2
        ReadyToDoImport = 3
        DisableAll = 4
    End Enum

    Public Sub SetCheckButtonStatus(ByVal ProcessStep As ButtonStatus)
        If ProcessStep = ButtonStatus.ReadyToParseXML Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = False
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToSearchFolders Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToFindOrphans Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = True
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToDoImport Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = True
            Form1.btnUpdateXML.Enabled = True
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.DisableAll Then
            Form1.btnParseXML.Enabled = False
            Form1.btnProcessMovieList.Enabled = False
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = False
        End If
    End Sub

    Public Function GetFileChecksum(ByVal FilePath As String)

        Dim fs As FileStream = New FileStream(FilePath, FileMode.Open, FileAccess.Read)
        Dim length As Long = fs.Length
        'Falls over here because length is 734,208,000 bytes.
        length = 1024000
        Dim arr(length) As Byte
        'fs.Read(arr, 0, fs.Length)
        fs.Read(arr, 0, length)
        fs.Close()
        Dim tmpHash() As Byte
        tmpHash = New System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(arr)
        Return ByteArrayToString(tmpHash)
    End Function

    Public Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim i As Integer
        Dim sOutput As New System.Text.StringBuilder(arrInput.Length)
        For i = 0 To arrInput.Length - 1
            sOutput.Append(arrInput(i).ToString("X2"))
        Next
        Return sOutput.ToString()
    End Function

    Public Function IsFileNeeded() As Boolean

        Dim ReturnValue As Boolean = False
        If Not CurrentSettings Is Nothing Then
            Dim MediaData As New ArrayList
            MediaData.Add("date")
            MediaData.Add("length")
            MediaData.Add("videobitrate")
            MediaData.Add("audiobitrate")
            MediaData.Add("aspectratio")
            MediaData.Add("audiochannelcount")
            MediaData.Add("disks")
            MediaData.Add("videoformat")
            MediaData.Add("audioformat")
            MediaData.Add("resolution")
            MediaData.Add("framerate")
            MediaData.Add("languages")
            MediaData.Add("subtitles")
            MediaData.Add("size")

            Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
            Dim FieldName As String
            Dim FieldChecked As Boolean
            Dim DatabaseFields As New Hashtable

            For Each blah As String In DBFields
                FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
                FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
                If Not DatabaseFields.ContainsKey(FieldName) Then
                    DatabaseFields.Add(FieldName, FieldChecked)
                End If
            Next


            'If CurrentSettings.Movie_Title_Handling.Contains("Internet Lookup") Then
            '   MediaData.Add("originaltitle")
            'End If

            For Each blah In From blah1 As String In MediaData Where DatabaseFields.ContainsKey(blah1) Where DatabaseFields(blah1) = True
                ReturnValue = True
            Next
        End If

        Return ReturnValue

    End Function

    Public Function IsInternetLookupNeeded() As Boolean

        Dim ReturnValue As Boolean = False
        If Not CurrentSettings Is Nothing Then
            Dim InternetData As New ArrayList
            InternetData.Add("rating")
            InternetData.Add("year")
            InternetData.Add("translatedtitle")
            InternetData.Add("director")
            InternetData.Add("producer")
            InternetData.Add("country")
            InternetData.Add("category")
            InternetData.Add("actors")
            InternetData.Add("url")
            InternetData.Add("description")
            InternetData.Add("comments")
            InternetData.Add("picture")
            InternetData.Add("languages")
            InternetData.Add("tagline")
            InternetData.Add("certification")
            InternetData.Add("writer")
            InternetData.Add("imdb_id")
            InternetData.Add("tmdb_id")
            InternetData.Add("imdb_rank")
            InternetData.Add("studio")
            InternetData.Add("fanart") ' Guzzi: added on request Dadeo


            If CurrentSettings.Movie_Title_Handling.Contains("Internet Lookup") Then
                InternetData.Add("originaltitle")
            End If

            Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
            Dim FieldName As String
            Dim FieldChecked As Boolean
            Dim DatabaseFields As New Hashtable

            For Each blah As String In DBFields
                FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
                FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
                If Not DatabaseFields.ContainsKey(FieldName) Then
                    DatabaseFields.Add(FieldName, FieldChecked)
                End If
            Next

            For Each blah In From blah1 As String In InternetData Where DatabaseFields.ContainsKey(blah1) Where DatabaseFields(blah1) = True
                ReturnValue = True
            Next

        End If

        Return ReturnValue

    End Function

    Public Function GetFanartTitle(ByVal MovieRecord As XmlElement, ByRef title As String, ByRef ttitle As String, ByRef ftitle As String, ByRef year As Integer, ByRef director As String) As String
        Dim fanartTitle As String = ""
        Dim mastertitle As String = String.Empty
        title = String.Empty
        ttitle = String.Empty
        ftitle = String.Empty
        director = String.Empty
        year = 0

        If Not MovieRecord.Attributes("OriginalTitle") Is Nothing Then
            title = RemoveGroupNameAndEdition(MovieRecord.Attributes("OriginalTitle").Value, "")
            If CurrentSettings.Master_Title = "OriginalTitle" Then
                mastertitle = title
            End If
        End If

        If Not MovieRecord.Attributes("TranslatedTitle") Is Nothing Then
            ttitle = RemoveGroupNameAndEdition(MovieRecord.Attributes("TranslatedTitle").Value, "")
            'If ttitle.Contains("(") Then
            '    ttitle = ttitle.Substring(0, ttitle.IndexOf("("))
            'End If
            If CurrentSettings.Master_Title = "TranslatedTitle" Then
                mastertitle = ttitle
            End If
        End If

        If Not MovieRecord.Attributes("FormattedTitle") Is Nothing Then
            ftitle = RemoveGroupNameAndEdition(MovieRecord.Attributes("FormattedTitle").Value, "")
            If CurrentSettings.Master_Title = "FormattedTitle" Then
                mastertitle = ftitle
            End If
        End If

        If title.ToString().Length > 0 Then
            If Not MovieRecord.Attributes("Year") Is Nothing Then
                Try
                    year = CInt(MovieRecord.Attributes("Year").Value)
                Catch
                    year = 0
                End Try
            End If
            If Not MovieRecord.Attributes("Director") Is Nothing Then
                director = MovieRecord.Attributes("Director").Value
            Else
                director = ""
            End If
        End If


        If CurrentSettings.Master_Title = "FormattedTitle" Then ' // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
            If Not String.IsNullOrEmpty(ttitle) Then
                fanartTitle = ttitle
            ElseIf Not String.IsNullOrEmpty(title) Then
                fanartTitle = title
            Else
                fanartTitle = ftitle
            End If
        Else
            If Not String.IsNullOrEmpty(mastertitle) Then
                fanartTitle = mastertitle
            ElseIf Not String.IsNullOrEmpty(title) Then
                fanartTitle = title
            ElseIf Not String.IsNullOrEmpty(ttitle) Then
                fanartTitle = ttitle
            ElseIf Not String.IsNullOrEmpty(ftitle) Then
                fanartTitle = ftitle
            Else
                fanartTitle = ""
            End If
        End If
        Return fanartTitle
    End Function

    Public Function RemoveGroupNameAndEdition(ByVal FullName As String, ByVal Edition As String) As String
        Dim Name As String
        If FullName.Contains("\") = True Then
            'FullName = FullName.Substring(0, FullName.IndexOf("\") - 1) ' remove rest of group name part (group fanart)
            Name = FullName.Substring(FullName.LastIndexOf("\") + 1) ' remove group name part
        Else
            Name = FullName
        End If

        If Not String.IsNullOrEmpty(Edition) Then
            If Name.Contains(" (" & Edition & ")") = True Then
                Name = Name.Replace(" (" & Edition & ")", "") ' remove edition name part
            End If
        End If

        Return Name
    End Function

    Public Function WriteNfoFile(ByVal outFileName As String, ByVal node As XmlNode, ByVal updateOnlyMissing As Boolean, ByVal mastertitle As String)
        ' Create XmlWriterSettings.

        If (File.Exists(OutFileName) And UpdateOnlyMissing) Then
            ' ToDo: add logic TypeOf detect, if existing files are MF created to decide if should be overwritten
            Return False ' Do nothing, if existing nfo files should NOT be updated/overwritten
        End If

        Dim settings As XmlWriterSettings = New XmlWriterSettings()
        settings.Indent = True
        settings.Encoding = System.Text.Encoding.UTF8

        ' Delete existing nfo file, if any
        If (System.IO.File.Exists(OutFileName)) Then
            'Dim NewFileName As String = Replace(OutFileName, ".nfo", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".nfo")
            Dim newFileName As String = Replace(OutFileName, ".nfo", " - " + "mfbak" + ".nfo")
            newFileName = newFileName.Replace("/", "-")
            Try
                If (Not System.IO.File.Exists(newFileName)) Then
                    My.Computer.FileSystem.CopyFile(OutFileName, newFileName, True) ' take a backup files - can be removed later
                End If
                System.IO.File.Delete(OutFileName)
            Catch ex As Exception
            End Try
        End If

        ' Create XmlWriter.
        Using writer As XmlWriter = XmlWriter.Create(OutFileName, settings)
            ' Begin writing.
            writer.WriteStartDocument()
            writer.WriteStartElement("movie") ' Root.

            ConditionalOutput(writer, "number", GetValueFromCurrentNode(node, "Number"))
            ConditionalOutput(writer, "title", GetValueFromCurrentNode(node, mastertitle))
            ConditionalOutput(writer, "originaltitle", GetValueFromCurrentNode(node, "OriginalTitle"))
            ConditionalOutput(writer, "translatedtitle", GetValueFromCurrentNode(node, "TranslatedTitle"))
            ConditionalOutput(writer, "formattedtitle", GetValueFromCurrentNode(node, "FormattedTitle"))
            ConditionalOutput(writer, "collection", GetValueFromCurrentNode(node, "Collection")) ' new for TMDB grabbers
            ConditionalOutput(writer, "collectiondetails", GetValueFromCurrentNode(node, "CollectionDetails")) ' new for TMDB grabbers
            ConditionalOutput(writer, "edition", GetValueFromCurrentNode(node, "Edition"))
            ConditionalOutput(writer, "studio", GetValueFromCurrentNode(node, "Studio"))
            ConditionalOutput(writer, "budget", GetValueFromCurrentNode(node, "Budget"))
            ConditionalOutput(writer, "imdb_id", GetValueFromCurrentNode(node, "IMDB_Id"))
            ConditionalOutput(writer, "imdb_rank", GetValueFromCurrentNode(node, "IMDB_Rank"))
            ConditionalOutput(writer, "tmdb_id", GetValueFromCurrentNode(node, "TMDB_Id"))
            ConditionalOutput(writer, "checked", GetValueFromCurrentNode(node, "Checked"))
            ConditionalOutput(writer, "url", GetValueFromCurrentNode(node, "URL"))
            ConditionalOutput(writer, "rating", GetValueFromCurrentNode(node, "Rating"))
            ConditionalOutput(writer, "ratinguser", GetValueFromCurrentNode(node, "RatingUser"))
            ConditionalOutput(writer, "plot", GetValueFromCurrentNode(node, "Description"))
            ConditionalOutput(writer, "comments", GetValueFromCurrentNode(node, "Comments"))
            ConditionalOutput(writer, "tagline", GetValueFromCurrentNode(node, "TagLine"))
            ConditionalOutput(writer, "tags", GetValueFromCurrentNode(node, "Tags"))
            ConditionalOutput(writer, "runtime", GetValueFromCurrentNode(node, "Length"))
            ConditionalOutput(writer, "certification", GetValueFromCurrentNode(node, "Certification"))
            ConditionalOutput(writer, "mpaa", GetValueFromCurrentNode(node, "Certification"))
            ConditionalOutput(writer, "languages", GetValueFromCurrentNode(node, "Languages"))
            ConditionalOutput(writer, "genre", GetValueFromCurrentNode(node, "Category"))
            ConditionalOutput(writer, "categorytrakt", GetValueFromCurrentNode(node, "CategoryTrakt"))
            
            ConditionalOutput(writer, "country", GetValueFromCurrentNode(node, "Country"))
            ConditionalOutput(writer, "year", GetValueFromCurrentNode(node, "Year"))
            ConditionalOutput(writer, "date", GetValueFromCurrentNode(node, "Date"))
            ConditionalOutput(writer, "dateadded", GetValueFromCurrentNode(node, "DateAdded"))
            ConditionalOutput(writer, "datereleased", GetValueFromCurrentNode(node, "DateReleased"))
            ConditionalOutput(writer, "plot", GetValueFromCurrentNode(node, "Description"))

            ConditionalOutput(writer, "director", GetValueFromCurrentNode(node, "Director"))
            ConditionalOutput(writer, "credits", GetValueFromCurrentNode(node, "Writer")) 'XBMC format !
            ConditionalOutput(writer, "writer", GetValueFromCurrentNode(node, "Writer"))
            ConditionalOutput(writer, "composer", GetValueFromCurrentNode(node, "Composer"))
            ConditionalOutput(writer, "borrower", GetValueFromCurrentNode(node, "Borrower"))
            ' actors
            Dim sep As String() = New String() {",", "|", ";"}
            Dim roleSep As String() = New String() {"(", ")", "as"}

            Dim actors As String() = GetValueFromCurrentNode(node, "Actors").Split(sep, StringSplitOptions.RemoveEmptyEntries)
            If actors.Length > 0 Then
                writer.WriteStartElement("actors")
                For Each person As String In actors
                    Dim persondetails As String() = person.Split(roleSep, StringSplitOptions.RemoveEmptyEntries)
                    writer.WriteStartElement("actor")
                    writer.WriteElementString("name", persondetails(0).Trim())
                    If persondetails.Length > 1 Then
                        writer.WriteElementString("role", persondetails(1).Trim())
                    End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
            End If

            Dim directors As String() = GetValueFromCurrentNode(node, "Producer").Split(sep, StringSplitOptions.RemoveEmptyEntries)
            If directors.Length > 0 Then
                writer.WriteStartElement("producers")
                For Each person As String In directors
                    Dim persondetails As String() = person.Split(roleSep, StringSplitOptions.RemoveEmptyEntries)
                    writer.WriteStartElement("producer")
                    writer.WriteElementString("name", persondetails(0).Trim())
                    If persondetails.Length > 1 Then
                        writer.WriteElementString("role", persondetails(1).Trim())
                    End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
            End If

            writer.WriteStartElement("mediainfo")
            ConditionalOutput(writer, "medialabel", GetValueFromCurrentNode(node, "MediaLabel"))
            ConditionalOutput(writer, "mediatype", GetValueFromCurrentNode(node, "MediaType"))
            ConditionalOutput(writer, "size", GetValueFromCurrentNode(node, "Size"))
            ConditionalOutput(writer, "disks", GetValueFromCurrentNode(node, "Disks"))
            ConditionalOutput(writer, "source", GetValueFromCurrentNode(node, "Source"))
            ConditionalOutput(writer, "sourcetrailer", GetValueFromCurrentNode(node, "SourceTrailer"))
            ConditionalOutput(writer, "videoformat", GetValueFromCurrentNode(node, "VideoFormat"))
            ConditionalOutput(writer, "s3d", GetValueFromCurrentNode(node, "S3D"))
            ConditionalOutput(writer, "videobitrate", GetValueFromCurrentNode(node, "VideoBitrate"))
            ConditionalOutput(writer, "audioformat", GetValueFromCurrentNode(node, "AudioFormat"))
            ConditionalOutput(writer, "audiobitrate", GetValueFromCurrentNode(node, "AudioBitrate"))
            ConditionalOutput(writer, "audiochannelcount", GetValueFromCurrentNode(node, "AudioChannelCount"))
            ConditionalOutput(writer, "resolution", GetValueFromCurrentNode(node, "Resolution"))
            ConditionalOutput(writer, "framerate", GetValueFromCurrentNode(node, "Framerate"))
            ConditionalOutput(writer, "subtitles", GetValueFromCurrentNode(node, "Subtitles"))
            ConditionalOutput(writer, "aspectratio", GetValueFromCurrentNode(node, "Aspectratio"))
            writer.WriteEndElement()
            
            writer.WriteStartElement("artwork")
            ConditionalOutput(writer, "picture", GetValueFromCurrentNode(node, "Picture"))
            ConditionalOutput(writer, "collectionimage", GetValueFromCurrentNode(node, "CollectionImage"))
            ConditionalOutput(writer, "fanart", GetValueFromCurrentNode(node, "Fanart"))
            writer.WriteEndElement()

            writer.WriteStartElement("status")
            ConditionalOutput(writer, "isonline", GetValueFromCurrentNode(node, "IsOnline"))
            ConditionalOutput(writer, "isonlinetrailer", GetValueFromCurrentNode(node, "IsOnlineTrailer"))
            writer.WriteEndElement()

            ' End document.
            writer.WriteEndElement()
            writer.WriteEndDocument()
        End Using
        Return True
    End Function

    Private Function ConditionalOutput(ByVal writer As XmlWriter, ByVal Element As String, ByVal currentValue As String) As Boolean
        If currentValue Is Nothing Then ' no values exist at all
            Return False
        Else If currentValue.Length = 0Then
            Return False
        Else 
            writer.WriteElementString(Element, currentValue)
        Return True
        End If
    End Function

    Private Function GetValueFromCurrentNode(ByVal CurrentNode As XmlNode, ByVal currentAttribute As String) As String
        Dim attr As Xml.XmlAttribute
        Dim element As Xml.XmlElement
        Dim ReturnValue As String = ""

        attr = CurrentNode.Attributes(currentAttribute)
        element = CurrentNode.Item(currentAttribute)
        If attr Is Nothing And element Is Nothing Then ' no values exist at all
            ReturnValue = ""
        Else
            If Not attr Is Nothing Then ' check for standard attr value
                If attr.Value Is Nothing Then
                    ReturnValue = Nothing
                Else
                    ReturnValue = attr.Value
                End If
            ElseIf Not element Is Nothing Then  ' check for enhanced element value
                If element.InnerText Is Nothing Then
                    ReturnValue = Nothing
                Else
                    ReturnValue = element.InnerText
                End If
            End If
        End If
        Return ReturnValue
    End Function

    Public Function GetFormat(ByVal aspectratio As Decimal) As String
        'Formats:
        '1,33 -> 4:3
        '1,78 -> 16:9 / widescreen
        '1,85 -> widescreen
        '2,35+ -> cinemascope
        Dim ar As String = ""
        If (aspectratio < CType(1.4, Decimal)) Then
            ar = "4:3"
        ElseIf (aspectratio < CType(1.9, Decimal)) Then
            ar = "16:9"
        ElseIf (aspectratio >= CType(1.9, Decimal)) Then
            ar = "cinemascope"
        End If
        Return ar
    End Function

    Public Function ByteString(ByVal bytes As Long) As String
        Dim s As Double = bytes
        Dim format As String() = New String() {"{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", _
         "{0} EB"}

        Dim i As Integer = 0

        While i < format.Length AndAlso s >= 1024
            s = CLng(Math.Truncate(100 * s / 1024)) / 100.0
            i += 1
        End While
        Return String.Format(format(i), s)
    End Function

    ''' <summary>
    ''' Methode die die UI Interaktionen durchführt.
    ''' </summary>
    ''' <param name="target">Control welches geändert wird, werden mehrere Controls geändert muss hier das Parent Control übergeben werden</param>
    ''' <param name="methodToInvoke">der Delegate der ausgeführt werden soll (UI Aktionen)</param>
    Private Sub InvokeIfRequired(ByVal target As Control, ByVal methodToInvoke As [Delegate])
        If target.InvokeRequired Then ' Mit Hilfe von InvokeRequired wird geprüft ob der Aufruf direkt an die UI gehen kann oder ob ein Invokeing hier von Nöten ist
            target.Invoke(methodToInvoke) ' Das Control muss per Invoke geändert werden, weil der aufruf aus einem Backgroundthread kommt
        Else
            methodToInvoke.DynamicInvoke() ' Die Änderung an der UI kann direkt aufgerufen werden.
        End If
    End Sub

    '<FlagsAttribute()> Public Enum EAccessType As Integer
    '    change = 1
    '    insert = 2
    '    delete = 4
    '    move = 8
    '    copy = 16
    'End Enum

    '' ermittelt die EmumMember Namen
    '' Bsp: ist Value 7 (change AND insert AND delete) ist das Ergebnis {"change", "insert", "delete"}
    'Public Function GetEnumItemNames(ByVal Value As EAccessType) As String()
    '    Dim a As String
    '    Dim b As String
    '    Dim c As String()

    '    a = Value.ToString
    '    a = a.Replace(" ", "")
    '    c = Split(a, ",")

    '    a = ""
    '    For Each b In c
    '        If a <> "" Then a &= ","
    '        a &= GetEnumValue(b)
    '    Next
    '    c = Split(a, ",")
    '    Return c
    'End Function

    '' Ermittel den EnumMember Wert anhand dessen Namen 
    'Private Function GetEnumValue(ByVal strName As String) As EAccessType
    '    Enum.Parse(EAccessType.GetType(), strName) ' @_ntr_: danke ;-)
    'End Function
End Module
