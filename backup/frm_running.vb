Imports System.IO
Imports System.Threading
Imports System.Text

Public Class frm_running

    Private consoletext As New StringBuilder
    Private fileincrement As Integer = 6
    Private filecount As Integer
    Public startonload As Boolean = False
    Public CurrentlyRunning As Boolean = False

#Region " Form and Object Subs "

    Private Sub frm_running_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If startonload = True Then
            Backup()
        End If
    End Sub

    Private Sub frm_running_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If CurrentlyRunning = True Then
            e.Cancel = True
            Me.Hide()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If MessageBox.Show("Are you sure you want to stop the backup?!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Global_Thread.Abort()
            Global_MainForm.NotifyIcon1.Text = "Backupinator"
            'Show the log window in case the user wants to save the text
            Dim frm As New frm_log
            frm.TextBox1.Text = consoletext.ToString
            frm.ShowDialog()
            CurrentlyRunning = False
            Me.Close()
            Global_MainForm.BackgroundTimer.Enabled = True
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If SaveFileDialog1.ShowDialog = DialogResult.OK Then
            Try
                File.WriteAllText(SaveFileDialog1.FileName, consoletext.ToString)
            Catch ex As Exception
                MessageBox.Show("There was a problem trying to save the log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        If TextBox1.SelectedText.Length = 0 Then
            Clipboard.SetText(TextBox1.Text)
        Else
            Clipboard.SetText(TextBox1.SelectedText)
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TextBox1.Select(TextBox1.Text.Length, 0)
        'TextBox1.ScrollToCaret()
    End Sub

#End Region

#Region " Backup! "

    Public Sub Backup()
        CurrentlyRunning = True
        Global_MainForm.NotifyIcon1.Text = "Backupinator (Running)"
        Global_Thread = New Thread(AddressOf Backupthread)
        Global_Thread.IsBackground = True
        Global_Thread.Start()
    End Sub

    Private Sub Backupthread()
        consoletext.Clear()
        Global_MainForm.BackgroundTimer.Enabled = False 'Pause the service
        startonload = False
        Dim i As Integer
        Dim abortprocess As Boolean = False
        Dim ar As New ArrayList

        Dim startdatetime As DateTime = DateTime.Now

        AddToConsole("Starting backup at " + GetSimpleDateAndTime(startdatetime) + "...", True, True)
        AddToConsole("----------------------------------------", True, False)

        'First: make sure the original prefix can be found. If not, try to create it.
        'The user will likely be backing up onto an external hard drive - we have to
        'expect that it may have gotten unplugged, etc
        Try
            If Directory.Exists(Setting_BackupPrefix) = False Then
                Directory.CreateDirectory(Setting_BackupPrefix)
            End If
        Catch ex As Exception
            abortprocess = True
            AddToConsole("Error! A backup cannot be completed with the specified backup destination.", True, False)
            ProgressBarValue(0)
            GoTo EndOfBackup
        End Try

        'Now, collect all the directories, etc
        ar.Add(Setting_OriginalPrefix)
        Dim cdas As String = ""
        If Setting_IncludeSubfolders = True Then
            cdas = "and subdirectories "
        End If
        AddToConsole("Collecting directories " + cdas + "[" + Setting_OriginalPrefix + "]", True, True)
        If Setting_IncludeSubfolders = True Then
            ar.AddRange(GetSubDirs(Setting_OriginalPrefix))
        End If

        SetProgressBarMax(ar.Count + (ar.Count * fileincrement))
        ProgressBarValue(ar.Count)

        CreateSubDirs(ar)

        Dim filenm As New filename("", "")
        Dim dirfiles() As String
        For i = 0 To ar.Count - 1
            Try
                dirfiles = Directory.GetFiles(ar.Item(i))
                For Each fil In dirfiles
                    filenm.SetVars(fil, ar.Item(i))
                    BackupFile(filenm)
                Next
            Catch ex As Exception
                AddToConsole("Error retrieving files [" + ar.Item(i) + "]", True, True)
            End Try
            ProgressBarIncrement(fileincrement)
        Next

        LastBackupDate = DateTime.Now
        SetMainFormLabelText(GetSimpleDateAndTime(LastBackupDate))

EndOfBackup:
        AddToConsole("----------------------------------------", True, False)
        AddToConsole("Done! Finished at " + GetSimpleDateAndTime(LastBackupDate) + ".", True, True)

        If abortprocess = False Then
            Dim tsp As TimeSpan = LastBackupDate - startdatetime
            Dim tss As String = "" 'Timespan string
            'Generate a nice looking timespan
            If tsp.Hours = 0 Then
                If tsp.Minutes = 0 Then
                    'No hours, no minutes, but MUST be seconds
                    tss = DeterminePlural(tsp.Seconds, tsp.Seconds.ToString + " second") + "."
                    If tss = "0 seconds." Then
                        tss = "no time at all! (Less than 1 second)."
                    End If
                Else
                    'No hours, but minutes and seconds
                    tss = DeterminePlural(tsp.Minutes, tsp.Minutes.ToString + " minute") + " and " + DeterminePlural(tsp.Seconds, tsp.Seconds.ToString + " second") + "."
                End If
            Else
                'Took longer than an hour
                tss = DeterminePlural(tsp.Hours, tsp.Hours.ToString + " hour") + ", " + DeterminePlural(tsp.Minutes, tsp.Minutes.ToString + " minute") + ", and " + DeterminePlural(tsp.Seconds, tsp.Seconds.ToString + " second") + "."
            End If
            AddToConsole("Backup took " + tss, True, False)
            AddToConsole(DeterminePlural(ar.Count, ar.Count.ToString + " folder") + " and " + DeterminePlural(filecount, filecount.ToString + " file") + " backed up.", True, False)
        End If

        If abortprocess = False Then
            'Save the log:
            Dim sp As String = Application.StartupPath
            sp = FolderBackslash(sp)
            sp = sp + "log.txt"
            Try
                File.WriteAllText(sp, consoletext.ToString)
            Catch ex As Exception
                If MessageBox.Show("Error saving the backup log. Do you want to try saving it elsewhere?", "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                    Button3_Click(Button3, New System.EventArgs)
                End If
            End Try
        End If

        ProgressBarValue(0)
        SetButtonText("Close")
        SetButtonEnabled(False)
        SetButtonVisible(True)
        CurrentlyRunning = False

        Global_NextRunDate = GetNextDate()
        SetMainFormLabel2Text(GetSimpleDateAndTime(Global_NextRunDate))

        Global_MainForm.BackgroundTimer.Enabled = True
        Global_MainForm.NotifyIcon1.Text = "Backupinator"
        Global_MainForm.NotifyIcon1.ShowBalloonTip(2000, "Done!", "Done backing up!", ToolTipIcon.None)
    End Sub

    Private Sub BackupFile(ByVal original_file As filename)
        If File.Exists(original_file.Complete) = False Then
            'This should NEVER happen, but just in case
            Exit Sub
        End If

        If Setting_BlacklistFileTypes.Count > 0 Then
            'If we're excluding any filetypes, we need to check
            If Setting_BlacklistFileTypes.IndexOf(LCase(GetExtension(original_file.Complete))) <> -1 Then
                Exit Sub
            End If
        End If
        If Setting_BlacklistFileNames.Count > 0 Then
            'Check to see if we're not supposed to copy this file
            If Setting_BlacklistFileNames.IndexOf(LCase(original_file.Complete)) <> -1 Then
                Exit Sub
            End If
        End If

        Dim np As String 'New prefix
        np = original_file.prefix
        np = Setting_BackupPrefix + original_file.prefix.Remove(0, Setting_OriginalPrefix.Length)

        If Setting_Overwrite = OverwriteSettings.OverwriteOnlyIfNewer Then
            'Check to see if the original file is newer

            If File.Exists(np + original_file.filename) = True Then
                Dim d1, d2 As Date
                d1 = File.GetLastWriteTime(original_file.Complete)
                d2 = File.GetLastWriteTime(np + original_file.filename)

                'If a newer version exists, then copy it.
                'If the version is the same, or if the backup is newer, do nothing.
                If d1 > d2 Then
                    CopyFile(original_file.Complete, np + original_file.filename)
                End If
            Else
                'A backed up version doesn't even exist, so we can just go ahead and copy it
                CopyFile(original_file.Complete, np + original_file.filename)
            End If

        Else
            'We're either going to overwrite, or not
            If (Setting_Overwrite = OverwriteSettings.AlwaysOverwrite) Or (Setting_Overwrite = OverwriteSettings.NeverOverwrite And File.Exists(np + original_file.filename) = False) Then
                CopyFile(original_file.Complete, np + original_file.filename)
            End If

        End If

    End Sub

    Private Sub CopyFile(ByVal oldfilename As String, ByVal newfilename As String, Optional ByVal ovrwrite As Boolean = True)
        Try
            File.Copy(oldfilename, newfilename, ovrwrite)
            AddToConsole("File copied [" + oldfilename + "]", True, True, "File copied [" + GetSimpleFileName(oldfilename) + "]")
            filecount = filecount + 1
        Catch ex As Exception
            AddToConsole("Error copying file [" + oldfilename + "]", True, False)
        End Try
    End Sub

    Private Function GetSubDirs(ByVal startdir As String) As ArrayList
        Dim dir() As String
        Dim rdir As New ArrayList
        'MessageBox.Show(startdir)
        If Setting_BlacklistFolders.IndexOf(LCase(FolderBackslash(startdir))) <> -1 Then
            Return rdir
            Exit Function
        End If
        Try
            dir = Directory.GetDirectories(startdir)
            For Each subdir In dir
                If rdir.IndexOf(subdir + "\") = -1 And Setting_BlacklistFolders.IndexOf(LCase(FolderBackslash(subdir))) = -1 Then
                    rdir.Add(subdir + "\")
                    AddToConsole("Collecting directories [" + FolderBackslash(subdir) + "]", False, True, "Collecting directories [" + GetSimpleFileName(subdir) + "]")
                End If
                rdir.AddRange(GetSubDirs(subdir))
            Next
        Catch ex As Exception
            AddToConsole("Error collecting directory [" + FolderBackslash(startdir) + "]", True, False)
        End Try

        Return rdir
    End Function

    Private Sub CreateSubDirs(ByVal ar As ArrayList)
        ar.Sort()
        Dim i As Integer
        For i = 0 To ar.Count - 1
            Dim dir As String = ar.Item(i)
            dir = dir.Remove(0, Setting_OriginalPrefix.Length)

            If Directory.Exists(Setting_BackupPrefix + dir) = False Then
                Try
                    Directory.CreateDirectory(Setting_BackupPrefix + dir)
                    AddToConsole("Creating directories [" + Setting_BackupPrefix + dir + "]", True, True, "Creating directories [" + GetSimpleFileName(dir) + "]")
                Catch ex As Exception
                    AddToConsole("Error creating directory [" + Setting_BackupPrefix + dir + "]", True, False)
                End Try
            End If

            ProgressBarIncrement(1)
        Next
    End Sub

    Private Sub AddToConsole(ByVal msgtxt As String, ByVal addtotextbox As Boolean, Optional ByVal ChangeStatusLabel As Boolean = True, Optional ByVal condensedstring As String = "")
        If ChangeStatusLabel = True Then
            If condensedstring = "" Then
                SetLabelText(msgtxt)
            Else
                SetLabelText(condensedstring)
            End If
        End If
        consoletext.AppendLine(msgtxt)
        If addtotextbox = True Then
            AppendTextBoxText(msgtxt)
        End If
    End Sub

#End Region

#Region " Threading "

    Delegate Sub AppendTextCallback(ByVal [text] As String)
    Private Sub AppendTextBoxText(ByVal [text] As String)
        If Me.TextBox1.InvokeRequired Then
            Dim d As New AppendTextCallback(AddressOf AppendTextBoxText)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.TextBox1.AppendText([text] + vbCrLf)
        End If
    End Sub
    Delegate Sub SetLabelTextCallback(ByVal [text] As String)
    Private Sub SetLabelText(ByVal [text] As String)
        If Me.Label1.InvokeRequired Then
            Dim d As New SetLabelTextCallback(AddressOf SetLabelText)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.Label1.Text = [text]
        End If
    End Sub
    Delegate Sub ProgressBarCallback(ByVal [value] As Integer)
    Private Sub SetProgressBarMax(ByVal [value] As Integer)
        If Me.ProgressBar1.InvokeRequired Then
            Dim d As New ProgressBarCallback(AddressOf SetProgressBarMax)
            Me.Invoke(d, New Object() {[value]})
        Else
            Me.ProgressBar1.Style = ProgressBarStyle.Blocks
            Me.ProgressBar1.Maximum = [value]
        End If
    End Sub
    Private Sub ProgressBarIncrement(ByVal [value] As Integer)
        If Me.Label1.InvokeRequired Then
            Dim d As New ProgressBarCallback(AddressOf ProgressBarIncrement)
            Me.Invoke(d, New Object() {[value]})
        Else
            Me.ProgressBar1.Increment([value])
        End If
    End Sub
    Private Sub ProgressBarValue(ByVal [value] As Integer)
        If Me.ProgressBar1.InvokeRequired Then
            Dim d As New ProgressBarCallback(AddressOf ProgressBarValue)
            Me.Invoke(d, New Object() {[value]})
        Else
            Me.ProgressBar1.Value = [value]
        End If
    End Sub
    Delegate Sub SetButtonCallback(ByVal [text] As String)
    Private Sub SetButtonText(ByVal [text] As String)
        If Me.Button1.InvokeRequired Then
            Dim d As New SetButtonCallback(AddressOf SetButtonText)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.Button1.Text = [text]
        End If
    End Sub
    Delegate Sub SetButtonEnabledCallback(ByVal [value] As Boolean)
    Private Sub SetButtonEnabled(ByVal [value] As Boolean)
        If Me.Button2.InvokeRequired Then
            Dim d As New SetButtonEnabledCallback(AddressOf SetButtonEnabled)
            Me.Invoke(d, New Object() {[value]})
        Else
            Me.Button2.Enabled = [value]
        End If
    End Sub
    Delegate Sub SetButtonVisibleCallback(ByVal [value] As Boolean)
    Private Sub SetButtonVisible(ByVal [value] As Boolean)
        If Me.Button3.InvokeRequired Then
            Dim d As New SetButtonEnabledCallback(AddressOf SetButtonVisible)
            Me.Invoke(d, New Object() {[value]})
        Else
            Me.Button3.Visible = [value]
        End If
    End Sub
    Delegate Sub SetMainFormLabelCallback(ByVal [text] As String)
    Private Sub SetMainFormLabelText(ByVal [text] As String)
        If Global_MainForm.label_lastbackup.InvokeRequired Then
            Dim d As New SetMainFormLabelCallback(AddressOf SetMainFormLabelText)
            Global_MainForm.Invoke(d, New Object() {[text]})
        Else
            Global_MainForm.label_lastbackup.Text = [text]
        End If
    End Sub
    Delegate Sub SetMainFormLabel2Callback(ByVal [text] As String)
    Private Sub SetMainFormLabel2Text(ByVal [text] As String)
        If Global_MainForm.label_nextbackup.InvokeRequired Then
            Dim d As New SetMainFormLabel2Callback(AddressOf SetMainFormLabel2Text)
            Global_MainForm.Invoke(d, New Object() {[text]})
        Else
            Global_MainForm.label_nextbackup.Text = [text]
        End If
    End Sub

#End Region

End Class