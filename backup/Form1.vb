Imports System.Threading
Imports System.IO
Imports Microsoft.Win32

Public Class Form1

    Public BackgroundTimer As Timers.Timer = New Timers.Timer
    Private Msg_Str As String
    Dim frm1loading As Boolean = False
    Dim actuallyclosing As Boolean = False
    Dim buildnumber As String = "Build H" 'Just to keep track of updates

#Region " File Menu "

    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs, Optional ByVal selectedtabpage As Integer = -1) Handles OptionsToolStripMenuItem.Click
        If Global_FormRunning.CurrentlyRunning = True Then
            MessageBox.Show("You cannot change program settings while a backup is taking place!", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        Dim frm As New frm_settings
        Dim i As Integer
        Select Case Setting_Overwrite
            Case OverwriteSettings.NeverOverwrite
                frm.RadioButton1.Checked = True
            Case OverwriteSettings.OverwriteOnlyIfNewer
                frm.RadioButton2.Checked = True
            Case OverwriteSettings.AlwaysOverwrite
                frm.RadioButton3.Checked = True
        End Select

        If Setting_BlacklistFolders.Count > 0 Then
            For i = 0 To Setting_BlacklistFolders.Count - 1
                If frm.ListBox3.Items.IndexOf(Setting_BlacklistFolders.Item(i)) = -1 Then
                    frm.ListBox3.Items.Add(Setting_BlacklistFolders.Item(i))
                End If
            Next
        End If
        If Setting_BlacklistFileTypes.Count > 0 Then
            For i = 0 To Setting_BlacklistFileTypes.Count - 1
                If frm.TextBox1.Text <> "" Then
                    frm.TextBox1.Text += ", "
                End If
                frm.TextBox1.Text = frm.TextBox1.Text + Setting_BlacklistFileTypes.Item(i)
            Next
        End If
        If Setting_BlacklistFileNames.Count > 0 Then
            For i = 0 To Setting_BlacklistFileNames.Count - 1
                If frm.ListBox2.Items.IndexOf(Setting_BlacklistFileNames.Item(i)) = -1 Then
                    frm.ListBox2.Items.Add(Setting_BlacklistFileNames.Item(i))
                End If
            Next
        End If

        frm.TextBox3.Text = Setting_OriginalPrefix
        frm.TextBox2.Text = Setting_BackupPrefix
        frm.CheckBox1.Checked = Setting_IncludeSubfolders

        If Setting_RunEveryDay = True Then
            frm.RadioButton4.Checked = True
            frm.CheckBox2.Enabled = False
            frm.CheckBox3.Enabled = False
            frm.CheckBox4.Enabled = False
            frm.CheckBox5.Enabled = False
            frm.CheckBox6.Enabled = False
            frm.CheckBox7.Enabled = False
            frm.CheckBox8.Enabled = False
        Else
            frm.RadioButton5.Checked = True
        End If
        frm.CheckBox2.Checked = Setting_RunOnSunday
        frm.CheckBox3.Checked = Setting_RunOnMonday
        frm.CheckBox4.Checked = Setting_RunOnTuesday
        frm.CheckBox5.Checked = Setting_RunOnWednesday
        frm.CheckBox6.Checked = Setting_RunOnThursday
        frm.CheckBox7.Checked = Setting_RunOnFriday
        frm.CheckBox8.Checked = Setting_RunOnSaturday
        frm.DateTimePicker1.Value = Setting_RunTime

        If selectedtabpage <> -1 Then
            frm.TabControl1.SelectedIndex = selectedtabpage
        End If

        If frm.ShowDialog = DialogResult.OK Then
            If frm.RadioButton1.Checked = True Then
                Setting_Overwrite = OverwriteSettings.NeverOverwrite
            ElseIf frm.RadioButton2.Checked = True Then
                Setting_Overwrite = OverwriteSettings.OverwriteOnlyIfNewer
            ElseIf frm.RadioButton3.Checked = True Then
                Setting_Overwrite = OverwriteSettings.AlwaysOverwrite
            End If

            If frm.TextBox1.Text <> "" Then
                Dim extar As New ArrayList
                frm.TextBox1.Text = LCase(frm.TextBox1.Text.Replace(" ", ""))
                Dim s() As String = frm.TextBox1.Text.Split(",")
                For Each ext In s
                    If extar.Contains(ext) = False And ext <> "" Then
                        extar.Add(ext)
                    End If
                Next
                Setting_BlacklistFileTypes = extar
            Else
                Setting_BlacklistFileTypes = New ArrayList
            End If

            If frm.ListBox3.Items.Count = 0 Then
                Setting_BlacklistFolders = New ArrayList
            Else
                Setting_BlacklistFolders = New ArrayList
                For i = 0 To frm.ListBox3.Items.Count - 1
                    If Setting_BlacklistFolders.IndexOf(frm.ListBox3.Items(i)) = -1 Then
                        Setting_BlacklistFolders.Add(frm.ListBox3.Items(i))
                    End If
                Next
            End If
            If frm.ListBox2.Items.Count = 0 Then
                Setting_BlacklistFileNames = New ArrayList
            Else
                Setting_BlacklistFileNames = New ArrayList
                For i = 0 To frm.ListBox2.Items.Count - 1
                    If Setting_BlacklistFileNames.IndexOf(frm.ListBox2.Items(i)) = -1 Then
                        Setting_BlacklistFileNames.Add(frm.ListBox2.Items(i))
                    End If
                Next
            End If

            Setting_OriginalPrefix = frm.TextBox3.Text
            Setting_BackupPrefix = frm.TextBox2.Text

            Setting_IncludeSubfolders = frm.CheckBox1.Checked

            If frm.CheckBox2.Checked = True And frm.CheckBox3.Checked = True And frm.CheckBox4.Checked = True And frm.CheckBox5.Checked = True And frm.CheckBox6.Checked = True And frm.CheckBox7.Checked = True And frm.CheckBox8.Checked = True Then
                Setting_RunEveryDay = True
            Else
                Setting_RunEveryDay = frm.RadioButton4.Checked
            End If
            Setting_RunOnSunday = frm.CheckBox2.Checked
            Setting_RunOnMonday = frm.CheckBox3.Checked
            Setting_RunOnTuesday = frm.CheckBox4.Checked
            Setting_RunOnWednesday = frm.CheckBox5.Checked
            Setting_RunOnThursday = frm.CheckBox6.Checked
            Setting_RunOnFriday = frm.CheckBox7.Checked
            Setting_RunOnSaturday = frm.CheckBox8.Checked
            Setting_RunTime = frm.DateTimePicker1.Value

        End If

        If CheckSettings() = True Then
            'The settings are now set well
            Global_NextRunDate = GetNextDate()
            label_nextbackup.Text = GetSimpleDateAndTime(Global_NextRunDate)
        Else
            label_nextbackup.Text = "Unknown"
        End If

    End Sub

    Private Sub HideToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HideToolStripMenuItem.Click
        NotifyIcon1.Visible = True
        Me.Hide()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        If Global_FormRunning.CurrentlyRunning = True Then
            MessageBox.Show("You cannot close the program while a backup is taking place!", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        Select Case frm_close.ShowDialog()
            Case Windows.Forms.DialogResult.OK
                NotifyIcon1.Visible = True
                Me.Hide()
            Case DialogResult.Yes
                'Save settings HERE
                actuallyclosing = True
                Me.Close()
        End Select
    End Sub

#End Region

#Region " Run Menu "

    Private Sub RunNowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunNowToolStripMenuItem.Click
        If Global_FormRunning.CurrentlyRunning = True Then
            Global_FormRunning.ShowDialog()
        Else
            Global_FormRunning = New frm_running
            Global_FormRunning.startonload = True
            Global_FormRunning.ShowDialog()
        End If
        If LastBackupDate = DateTime.MinValue Then
            label_lastbackup.Text = "Unknown"
        End If
    End Sub

    Private Sub PauseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PauseToolStripMenuItem.Click
        If Global_FormRunning.CurrentlyRunning = True Then
            MessageBox.Show("You cannot pause the backup service while a backup is taking place!", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        '' '' ''If Timer1.Enabled = True Then
        If BackgroundTimer.Enabled = True Then
            If MessageBox.Show("Are you sure you want to pause the backup service?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                PauseService()
            End If
        Else
            ResumeService()
        End If
    End Sub

    Private Sub RunOnStartupToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunOnStartupToolStripMenuItem.Click
        If RunOnStartupToolStripMenuItem.Checked = True Then
            If MessageBox.Show("By default, Backupinator is configured to start when your computer starts. If you disable this feature, you will have to manually start Backupinator." + vbCrLf + vbCrLf + "Are you sure you want to disable this?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                RunOnStartupToolStripMenuItem.Checked = False
            End If
        Else
            RunOnStartupToolStripMenuItem.Checked = True
        End If
    End Sub

#End Region

#Region " Help Menu "

    Private Sub RegisterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegisterToolStripMenuItem.Click
        If LCase(Global_RegisteredTo) <> "null" And Global_RegisteredTo <> "" Then
            MessageBox.Show("This copy of Backupinator is already registered to " + Global_RegisteredTo + ".", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim frm As New frm_enterregistrationkey
            frm.ShowDialog()
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim frm As New frm_about
        frm.Label2.Text = "Version 1.0, " + buildnumber
        If LCase(Global_RegisteredTo) <> "null" And Global_RegisteredTo <> "" Then
            frm.Label3.Text = "Registered to " + Global_RegisteredTo
            frm.Button2.Enabled = False
        End If
        frm.ShowDialog()
    End Sub

#End Region

#Region " Form Events "

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadSettings()
        Global_MainForm = Me
        If CheckSettings() = False Then
            PauseService()
            Exit Sub
        End If
        Global_NextRunDate = GetNextDate()
        label_nextbackup.Text = GetSimpleDateAndTime(Global_NextRunDate)
        frm1loading = True
        BackgroundTimer = New Timers.Timer()
        AddHandler BackgroundTimer.Elapsed, AddressOf TimerTick
        BackgroundTimer.Interval = 1000
        BackgroundTimer.Start()
        PictureBox1.Image = My.Resources.img_running
        Me.Text = "Backupinator"
    End Sub

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'This is a really cheap workaround, but I don't know of a better way to do it.
        'If the program is starting from the computer startup, we want to go ahead and
        'hide the program to run in the background.
        'Form1 is also called when the user clicks the NotifyIcon, so we need this to
        'occur ONLY when the form is loading.
        If frm1loading = True Then
            frm1loading = False
            If Global_StartHidden = True Then
                HideToolStripMenuItem_Click(sender, e)
            Else
                NotifyIcon1.Visible = False
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If actuallyclosing = True Then
            NotifyIcon1.Visible = False
            SaveSettings()
        Else
            'Eh- the user doesn't really want to close, does he?
            e.Cancel = True
            HideToolStripMenuItem_Click(sender, e)
        End If
    End Sub

    Private Sub Form1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
        NotifyIcon1.Text = Me.Text
    End Sub

#End Region

#Region " Object Events "

    Private Sub label_lastbackup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles label_lastbackup.Click
        If label_lastbackup.Text = "Unknown" Then
            Exit Sub
        End If
        Dim sp As String = Application.StartupPath
        sp = FolderBackslash(sp)
        sp = sp + "log.txt"

        If File.Exists(sp) = False Then
            MessageBox.Show("There doesn't appear to be a log of the last backup.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim str As String
        Try
            str = File.ReadAllText(sp)
        Catch ex As Exception
            MessageBox.Show("There was an error reading the log from the last backup.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        If str = "" Then
            MessageBox.Show("The log of the last backup does not appear to contain any data.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim frm As New frm_log
        frm.TextBox1.Text = str
        frm.ShowDialog()
    End Sub

    Private Sub label_nextbackup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles label_nextbackup.Click
        OptionsToolStripMenuItem_Click(sender, e, 4)
    End Sub

    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click
        NotifyIcon1.Visible = False
        Me.Show()
        If Global_FormRunning.CurrentlyRunning = True Then
            Global_FormRunning.ShowDialog()
        End If
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        CheckSettings()
        If Msg_Str <> "" Then
            MessageBox.Show(Msg_Str, "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'For testing purposes only!
        'This button should be HIDDEN in the build!
        Dim dtnrd As DateTime = DateTime.Now
        dtnrd = SetTime(dtnrd, dtnrd.Hour, dtnrd.Minute + 1, 0)
        Global_NextRunDate = dtnrd
        label_nextbackup.Text = GetSimpleDateAndTime(Global_NextRunDate)
    End Sub

#End Region

#Region " Settings "

    Private Sub LoadSettings()
        'Check registry FIRST. Thus, if the settings file is gone, it's still OK.
        Try
            Dim startupReg As RegistryKey = Registry.CurrentUser
            Dim startupKey As RegistryKey = startupReg.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
            If startupKey.GetValue("Backupinator 1.0") = "" Then
                RunOnStartupToolStripMenuItem.Checked = False
            Else
                RunOnStartupToolStripMenuItem.Checked = True
            End If
        Catch ex As Exception
        End Try

        'Read the file to determine the settings
        Dim sp As String = Application.StartupPath
        sp = FolderBackslash(sp)
        sp = sp + "settings.txt"

        If File.Exists(sp) = False Then
            'We can assume this is the first time for Backupinator to run
            MessageBox.Show("Welcome to Backupinator!" + vbCrLf + vbCrLf + "Before Backupinator can run, you must specify some settings!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Setting_OriginalPrefix = ""
            Setting_BackupPrefix = ""
            NotifyIcon1.Visible = False
            Global_StartHidden = False
            OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs)
            Exit Sub
        End If

        Dim stgstr As String = ""
        Try
            stgstr = File.ReadAllText(sp)
        Catch ex As Exception
            MessageBox.Show("There was an error loading settings for Backupinator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Setting_OriginalPrefix = ""
            Setting_BackupPrefix = ""
            NotifyIcon1.Visible = False
            Global_StartHidden = False
            OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs)
            Exit Sub
        End Try

        stgstr = Decrypt(stgstr)

        Dim ar As New ArrayList
        ar = SplitStringToArrayList(stgstr, vbCrLf)

        If ar.Count <> 13 Then
            MessageBox.Show("There was an error loading settings for Backupinator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Setting_OriginalPrefix = ""
            Setting_BackupPrefix = ""
            NotifyIcon1.Visible = False
            Global_StartHidden = False
            OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs)
            Exit Sub
        End If

        If ar.Item(0) <> "Backupinator Version 1.0" Then
            MessageBox.Show("There was an error loading settings for Backupinator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Setting_OriginalPrefix = ""
            Setting_BackupPrefix = ""
            NotifyIcon1.Visible = False
            Global_StartHidden = False
            OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs)
            Exit Sub
        End If
        Setting_OriginalPrefix = ar.Item(1)
        Setting_BackupPrefix = ar.Item(2)
        If ar.Item(3) = "True" Then
            Setting_IncludeSubfolders = True
        Else
            Setting_IncludeSubfolders = False
        End If
        Select Case LCase(ar.Item(4))
            Case "alwaysoverwrite"
                Setting_Overwrite = OverwriteSettings.AlwaysOverwrite
            Case "overwriteonlyifnewer"
                Setting_Overwrite = OverwriteSettings.OverwriteOnlyIfNewer
            Case "neveroverwrite"
                Setting_Overwrite = OverwriteSettings.NeverOverwrite
            Case Else
                MessageBox.Show("There was an error loading settings for Backupinator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Setting_OriginalPrefix = ""
                Setting_BackupPrefix = ""
                OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs, 3)
                Exit Sub
        End Select
        If LCase(ar.Item(5)) <> "null" Then
            Setting_BlacklistFolders = SplitStringToArrayList(ar.Item(5), "|")
        End If
        If LCase(ar.Item(6)) <> "null" Then
            Setting_BlacklistFileTypes = SplitStringToArrayList(ar.Item(6), "|")
        End If
        If LCase(ar.Item(7)) <> "null" Then
            Setting_BlacklistFileNames = SplitStringToArrayList(ar.Item(7), "|")
        End If
        If ar.Item(8) = "True" Then
            Setting_RunEveryDay = True
        Else
            Setting_RunEveryDay = False
        End If
        Dim dtral As New ArrayList
        dtral = SplitStringToArrayList(ar.Item(9), "|")
        If dtral.Count <> 7 Then
            MessageBox.Show("Error loading settings!", "Error!")
            Setting_OriginalPrefix = ""
            Setting_BackupPrefix = ""
            NotifyIcon1.Visible = False
            Global_StartHidden = False
            OptionsToolStripMenuItem_Click(OptionsToolStripMenuItem, New System.EventArgs, 4)
            Exit Sub
        End If
        If LCase(dtral.Item(0)) = "true" Then
            Setting_RunOnSunday = True
        Else
            Setting_RunOnSunday = False
        End If
        If LCase(dtral.Item(1)) = "true" Then
            Setting_RunOnMonday = True
        Else
            Setting_RunOnMonday = False
        End If
        If LCase(dtral.Item(2)) = "true" Then
            Setting_RunOnTuesday = True
        Else
            Setting_RunOnTuesday = False
        End If
        If LCase(dtral.Item(3)) = "true" Then
            Setting_RunOnWednesday = True
        Else
            Setting_RunOnWednesday = False
        End If
        If LCase(dtral.Item(4)) = "true" Then
            Setting_RunOnThursday = True
        Else
            Setting_RunOnThursday = False
        End If
        If LCase(dtral.Item(5)) = "true" Then
            Setting_RunOnFriday = True
        Else
            Setting_RunOnFriday = False
        End If
        If LCase(dtral.Item(6)) = "true" Then
            Setting_RunOnSaturday = True
        Else
            Setting_RunOnSaturday = False
        End If
        Dim rth As Integer
        Dim rtm As Integer
        rth = Convert.ToInt32(ar.Item(10).ToString.Substring(0, 2))
        rtm = Convert.ToInt32(ar.Item(10).ToString.Substring(2, 2))
        If rth < 0 Or rth > 24 Then
            rth = 0
        End If
        If rtm < 0 Or rtm > 60 Then
            rtm = 0
        End If
        Setting_RunTime = SetTime(Setting_RunTime, rth, rtm, 0)
        If ar.Item(11) <> "null" Then
            LastBackupDate = Convert.ToDateTime(ar.Item(11))
            label_lastbackup.Text = GetSimpleDateAndTime(LastBackupDate)
        End If
        If LCase(ar.Item(12)) <> "null" Then
            Global_RegisteredTo = ar.Item(12)
        End If

    End Sub

    Private Sub SaveSettings()
        Dim sp As String = Application.StartupPath
        sp = FolderBackslash(sp)
        sp = sp + "settings.txt"
        Dim stgsstr As String
        stgsstr = "Backupinator Version 1.0"
        stgsstr += vbCrLf + Setting_OriginalPrefix
        stgsstr += vbCrLf + Setting_BackupPrefix
        stgsstr += vbCrLf + Setting_IncludeSubfolders.ToString
        stgsstr += vbCrLf + Setting_Overwrite.ToString
        stgsstr += vbCrLf + SplitArrayList(Setting_BlacklistFolders, "|", True)
        stgsstr += vbCrLf + SplitArrayList(Setting_BlacklistFileTypes, "|", True)
        stgsstr += vbCrLf + SplitArrayList(Setting_BlacklistFileNames, "|", True)
        stgsstr += vbCrLf + Setting_RunEveryDay.ToString
        stgsstr += vbCrLf + Setting_RunOnSunday.ToString + "|" + Setting_RunOnMonday.ToString + "|" + Setting_RunOnTuesday.ToString + "|" + Setting_RunOnWednesday.ToString + "|" + Setting_RunOnThursday.ToString + "|" + Setting_RunOnFriday.ToString + "|" + Setting_RunOnSaturday.ToString
        Dim rth As String = Setting_RunTime.Hour.ToString
        Dim rtm As String = Setting_RunTime.Minute.ToString
        If rth.Length = 1 Then
            rth = "0" + rth
        End If
        If rtm.Length = 1 Then
            rtm = "0" + rtm
        End If
        stgsstr += vbCrLf + rth + rtm
        If LastBackupDate = DateTime.MinValue Then
            stgsstr += vbCrLf + "null"
        Else
            stgsstr += vbCrLf + LastBackupDate.ToString
        End If
        If Global_RegisteredTo = "" Then
            stgsstr += vbCrLf + "null"
        Else
            stgsstr += vbCrLf + Global_RegisteredTo
        End If

        'Add encryption here to prevent people from messing with the settings file. Why they would? I don't know.
        stgsstr = Encrypt(stgsstr)

        Try
            If File.Exists(sp) = True Then
                File.Delete(sp)
            End If
            File.WriteAllText(sp, stgsstr)
        Catch ex As Exception
            MessageBox.Show("There was a problem trying to save your settings!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        'Add to registry
        If RunOnStartupToolStripMenuItem.Checked = True Then
            Try
                Dim startupReg As RegistryKey = Registry.CurrentUser
                Dim startupKey As RegistryKey = startupReg.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                startupKey.SetValue("Backupinator 1.0", """" + Application.ExecutablePath + """ -hidden")
            Catch ex As Exception
            End Try
        Else
            Try
                Dim startupReg As RegistryKey = Registry.CurrentUser
                Dim startupKey As RegistryKey = startupReg.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                startupKey.DeleteValue("Backupinator 1.0")
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Function CheckSettings() As Boolean
        'This should be called any time before the service runs to make sure all the settings are correct.
        Dim rtrnvalue As Boolean = True
        If Setting_OriginalPrefix = "" Or System.IO.Directory.Exists(Setting_OriginalPrefix) = False Then
            Msg_Str = "You must specify a backup location."
            rtrnvalue = False
        End If
        If Setting_BackupPrefix = "" And rtrnvalue = True Then
            'It's ok if this doesn't exist; we can try creating it
            Msg_Str = "You must specify an original location to back up."
            rtrnvalue = False
        End If
        If Setting_BackupPrefix = Setting_OriginalPrefix And rtrnvalue = True Then
            Msg_Str = "The backup location cannot be the same as the new destination."
            rtrnvalue = False
        End If
        If Setting_RunEveryDay = False And Setting_RunOnSunday = False And Setting_RunOnMonday = False And Setting_RunOnTuesday = False And Setting_RunOnWednesday = False And Setting_RunOnThursday = False And Setting_RunOnFriday = False And Setting_RunOnSaturday = False And rtrnvalue = True Then
            Msg_Str = "You must select at least one day to back up."
            rtrnvalue = False
        End If
        'Check any other settings here

        If rtrnvalue = True Then
            'If all checks out, return TRUE
            Msg_Str = ""
            If BackgroundTimer.Enabled = False Then
                Set_PictureBox_Image(1)
                Set_MainForm_Text("Backupinator - PAUSED")
            Else
                Set_PictureBox_Image(2)
                Set_MainForm_Text("Backupinator")
            End If
            Return True
        Else
            Global_NextRunDate = New DateTime
            Set_LabelNextBackup_Text("Unknown")
            Set_PictureBox_Image(0)
            Return False
        End If
    End Function

#End Region

#Region " BackgroundTimer Subs "

    Private Sub TimerTick()
        If DateTime.Now.Date = Global_NextRunDate.Date And DateTime.Now.Hour = Global_NextRunDate.Hour And DateTime.Now.Minute = Global_NextRunDate.Minute And DateTime.Now.Second = 0 And Global_FormRunning.CurrentlyRunning = False Then
            'Ooh! Time to backup!
            If CheckSettings() = False Then
                PauseService()
                Exit Sub
            End If
            If NotifyIcon1.Visible = False Then
                Global_FormRunning = New frm_running
                Global_FormRunning.startonload = True
                Global_FormRunning.ShowDialog()
            Else
                Global_FormRunning = New frm_running
                Global_FormRunning.Backup()
            End If
        End If
    End Sub

    Private Sub PauseService()
        BackgroundTimer.Enabled = False
        If CheckSettings() = False Then
            Set_MainForm_Text("Backupinator - STOPPED")
        Else
            Set_MainForm_Text("Backupinator - PAUSED")
        End If
        RunNowToolStripMenuItem.Enabled = False
        Set_PauseMenu_Text("Resume Service")
    End Sub

    Private Sub ResumeService()
        Msg_Str = ""
        If CheckSettings() = False Then
            PauseService()
            MessageBox.Show(Msg_Str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        RunNowToolStripMenuItem.Enabled = True
        Set_PauseMenu_Text("Pause Service")
        Set_MainForm_Text("Backupinator")
        Set_PictureBox_Image(2)
        BackgroundTimer.Enabled = True
    End Sub

#End Region

#Region " Threading "

    Delegate Sub SetMainFormTextCallback(ByVal [text] As String)
    Private Sub Set_MainForm_Text(ByVal [text] As String)
        If Me.InvokeRequired Then
            Dim d As New SetMainFormTextCallback(AddressOf Set_MainForm_Text)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.Text = [text]
        End If
    End Sub
    Delegate Sub SetLabelLastBackupTextCallback(ByVal [text] As String)
    Private Sub Set_LabelLastBackup_Text(ByVal [text] As String)
        If Me.label_lastbackup.InvokeRequired Then
            Dim d As New SetLabelLastBackupTextCallback(AddressOf Set_LabelLastBackup_Text)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.label_lastbackup.Text = [text]
        End If
    End Sub
    Delegate Sub SetLabelNextBackupTextCallback(ByVal [text] As String)
    Private Sub Set_LabelNextBackup_Text(ByVal [text] As String)
        If Me.label_nextbackup.InvokeRequired Then
            Dim d As New SetLabelNextBackupTextCallback(AddressOf Set_LabelNextBackup_Text)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.label_nextbackup.Text = [text]
        End If
    End Sub
    Delegate Sub SetPBImageCallback(ByVal [value] As Integer)
    Private Sub Set_PictureBox_Image(ByVal [value] As Integer)
        If Me.PictureBox1.InvokeRequired Then
            Dim d As New SetPBImageCallback(AddressOf Set_PictureBox_Image)
            Me.Invoke(d, New Object() {[value]})
        Else
            Select Case [value]
                Case 0
                    Me.PictureBox1.Image = My.Resources.img_stopped
                Case 1
                    Me.PictureBox1.Image = My.Resources.img_paused
                Case 2
                    Me.PictureBox1.Image = My.Resources.img_running
            End Select
        End If
    End Sub
    Delegate Sub SetPauseMenuItemTextCallback(ByVal [text] As String)
    Private Sub Set_PauseMenu_Text(ByVal [text] As String)
        If Me.MenuStrip1.InvokeRequired Then
            Dim d As New SetPauseMenuItemTextCallback(AddressOf Set_PauseMenu_Text)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.PauseToolStripMenuItem.Text = [text]
        End If
    End Sub

#End Region


End Class
