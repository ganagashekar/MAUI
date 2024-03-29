Option Strict On

Imports System.IO

Public Class Form1

    Private ReadOnly _strPath As String = "c:\_temp"
    Private ReadOnly _strFileName As String = "test.txt"

    Private _textReader As StreamReader
    Private _fileLength As Integer

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnStartMonitoring_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStartMonitoring.Click
        listBox1.Items.Add($"Start monitoring {_strFileName} at {DateTime.Now.ToLongTimeString()}")

        'load the current file
        FullLoad()

        'initialize the file system watcher and start monitoring
        fileSystemWatcher1.Path = _strPath
        fileSystemWatcher1.Filter = _strFileName
        fileSystemWatcher1.NotifyFilter = NotifyFilters.LastWrite
        fileSystemWatcher1.EnableRaisingEvents = True
    End Sub

    Private Sub FullLoad()
        Try
            Dim fs = New FileStream(Path.Combine(_strPath, _strFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            _textReader = New StreamReader(fs)

            Do
                Dim line As String = _textReader.ReadLine()
                If line Is Nothing Then Exit Do
                listBox1.Items.Add(line)
            Loop

            listBox1.SelectedIndex = listBox1.Items.Count - 1
            _fileLength = CInt(_textReader.BaseStream.Length)
        Catch ex As FileNotFoundException
            MessageBox.Show("You need to create your test file first")
        End Try
    End Sub

    Private Sub fileSystemWatcher_Changed(ByVal sender As Object, ByVal e As FileSystemEventArgs) Handles fileSystemWatcher1.Changed
        'sometimes the event is triggered with an invalid stream
        If _textReader.BaseStream.Length = 0 Then Return

        listBox1.Items.Add(New String("-"c, 40))
        listBox1.Items.Add($"{e.ChangeType} - Previous length={_fileLength} / Current length={_textReader.BaseStream.Length}")

        If _textReader.BaseStream.Length > _fileLength Then
            listBox1.Items.Add(New String("-"c, 40))
            listBox1.Items.Add($"Adding new items at {DateTime.Now.ToLongTimeString()}")
            Dim strEndOfFile = _textReader.ReadToEnd()

            For Each strNewItem As String In strEndOfFile.Split({vbCrLf}, StringSplitOptions.None)
                listBox1.Items.Add(strNewItem)
            Next

            listBox1.SelectedIndex = listBox1.Items.Count - 1
            _fileLength = CInt(_textReader.BaseStream.Length)
        ElseIf _textReader.BaseStream.Length < _fileLength Then
            'file is shorter, just reload to start fresh
            listBox1.Items.Add($"Stream is shorter than the previous one. Fully reloading the file at {DateTime.Now.ToLongTimeString()}")
            FullLoad()
        End If
    End Sub

End Class
