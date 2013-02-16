Public Class frm_settings

#Region " Form and Object Subs "

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If checkextensions() = False Then
            TabControl1.SelectedIndex = 1
            MessageBox.Show("Op! Please check your excluded file extensions list." + vbCrLf + vbCrLf + "Only alphanumeric characters are allowed! (a-z and 0-9)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        If RadioButton5.Checked = True And CheckBox2.Checked = False And CheckBox3.Checked = False And CheckBox4.Checked = False And CheckBox5.Checked = False And CheckBox6.Checked = False And CheckBox7.Checked = False And CheckBox8.Checked = False Then
            TabControl1.SelectedIndex = 4
            MessageBox.Show("Er! You must select at least one day to run.", "Error!")
            Exit Sub
        End If
        If TextBox2.Text = "" Then
            TabControl1.SelectedIndex = 2
            MessageBox.Show("Ak! You must specify a backup location!", "Error!")
            Exit Sub
        End If
        If TextBox3.Text = "" Then
            TabControl1.SelectedIndex = 0
            MessageBox.Show("Ak! You must specify a base location to back up!", "Error!")
            Exit Sub
        End If
        Me.DialogResult = DialogResult.OK
    End Sub

#End Region

#Region " Tab 1: Include "

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FolderBrowserDialog1.ShowNewFolderButton = False
        FolderBrowserDialog1.SelectedPath = Setting_OriginalPrefix
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Dim s As String
            s = FolderBrowserDialog1.SelectedPath
            s = FolderBackslash(s)
            If LCase(s) = LCase(TextBox2.Text) Then
                MessageBox.Show("The backup location cannot be the same as the new destination.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf ListBox3.Items.IndexOf(LCase(s)) <> -1 Then
                MessageBox.Show("You can't specify a folder to include if it is already in your list of folders to exclude.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                TextBox3.Text = s
            End If
        End If
    End Sub

#End Region

#Region " Tab 2: Exclude "

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        If FolderBrowserDialog2.ShowDialog = DialogResult.OK Then
            Dim fol As String = FolderBrowserDialog2.SelectedPath
            fol = FolderBackslash(fol)
            If LCase(TextBox3.Text) <> LCase(fol) Then
                If ListBox3.Items.IndexOf(LCase(fol)) = -1 Then
                    ListBox3.Items.Add(LCase(fol))
                End If
            Else
                MessageBox.Show("You can't exclude this folder since it is already specified to be included as your base folder.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        If ListBox3.Items.Count > 0 And ListBox3.SelectedItems.Count > 0 Then
            'This little diddly just makes things a tad easier to navigate
            Dim i As Integer
            i = ListBox3.SelectedIndex
            ListBox3.Items.RemoveAt(i)
            If ListBox3.Items.Count > i Then
                ListBox3.SelectedIndex = i
            ElseIf ListBox3.Items.Count = i Then
                ListBox3.SelectedIndex = i - 1
            End If
        End If
    End Sub

    Private Function checkextensions() As Boolean
        Dim s As String
        s = LCase(TextBox1.Text)

        s = s.Replace(" ", "")
        s = s.Replace(",", "")
        s = s.Replace("a", "")
        s = s.Replace("b", "")
        s = s.Replace("c", "")
        s = s.Replace("d", "")
        s = s.Replace("e", "")
        s = s.Replace("f", "")
        s = s.Replace("g", "")
        s = s.Replace("h", "")
        s = s.Replace("i", "")
        s = s.Replace("j", "")
        s = s.Replace("k", "")
        s = s.Replace("l", "")
        s = s.Replace("m", "")
        s = s.Replace("n", "")
        s = s.Replace("o", "")
        s = s.Replace("p", "")
        s = s.Replace("q", "")
        s = s.Replace("r", "")
        s = s.Replace("s", "")
        s = s.Replace("t", "")
        s = s.Replace("u", "")
        s = s.Replace("v", "")
        s = s.Replace("w", "")
        s = s.Replace("x", "")
        s = s.Replace("y", "")
        s = s.Replace("z", "")
        s = s.Replace("0", "")
        s = s.Replace("1", "")
        s = s.Replace("2", "")
        s = s.Replace("3", "")
        s = s.Replace("4", "")
        s = s.Replace("5", "")
        s = s.Replace("6", "")
        s = s.Replace("7", "")
        s = s.Replace("8", "")
        s = s.Replace("9", "")

        If s = "" Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim files() As String = OpenFileDialog1.FileNames
            For Each file In files
                If ListBox2.Items.IndexOf(LCase(file)) = -1 Then
                    ListBox2.Items.Add(LCase(file))
                End If
            Next
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If ListBox2.Items.Count > 0 And ListBox2.SelectedItems.Count > 0 Then
            'This little diddly just makes things a tad easier to navigate
            Dim i As Integer
            i = ListBox2.SelectedIndex
            ListBox2.Items.RemoveAt(i)
            If ListBox2.Items.Count > i Then
                ListBox2.SelectedIndex = i
            ElseIf ListBox2.Items.Count = i Then
                ListBox2.SelectedIndex = i - 1
            End If
        End If
    End Sub

#End Region

#Region " Tab 3: Backup Location "

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        FolderBrowserDialog1.ShowNewFolderButton = True
        FolderBrowserDialog1.SelectedPath = Setting_BackupPrefix
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Dim s As String
            s = FolderBrowserDialog1.SelectedPath
            s = FolderBackslash(s)
            If LCase(s) = LCase(TextBox3.Text) Then
                MessageBox.Show("The backup destination cannot be the same as the base folder.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf ListBox3.Items.IndexOf(LCase(s)) <> -1 Then
                MessageBox.Show("You can't specify a folder as your backup destination if it is already in your list of folders to exclude.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                TextBox2.Text = s
            End If
        End If
    End Sub

#End Region

#Region " Tab 5: Schedule "

    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged
        If RadioButton5.Checked = False Then
            CheckBox2.Enabled = False
            CheckBox3.Enabled = False
            CheckBox4.Enabled = False
            CheckBox5.Enabled = False
            CheckBox6.Enabled = False
            CheckBox7.Enabled = False
            CheckBox8.Enabled = False
        Else
            CheckBox2.Enabled = True
            CheckBox3.Enabled = True
            CheckBox4.Enabled = True
            CheckBox5.Enabled = True
            CheckBox6.Enabled = True
            CheckBox7.Enabled = True
            CheckBox8.Enabled = True
        End If
    End Sub

#End Region

End Class