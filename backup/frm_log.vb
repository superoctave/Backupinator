Imports System.IO

Public Class frm_log

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        If TextBox1.SelectedText.Length = 0 Then
            Clipboard.SetText(TextBox1.Text)
        Else
            Clipboard.SetText(TextBox1.SelectedText)
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If SaveFileDialog1.ShowDialog = DialogResult.OK Then
            Try
                File.WriteAllText(SaveFileDialog1.FileName, TextBox1.Text)
            Catch ex As Exception
                MessageBox.Show("There was a problem trying to save the log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub frm_log_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Select(0, 0)
    End Sub

End Class