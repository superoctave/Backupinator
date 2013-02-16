Public Class frm_enterregistrationkey

    Private attempts As Integer

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then
            MessageBox.Show("Please enter a registration name.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TextBox1.Focus()
            Exit Sub
        ElseIf TextBox2.Text = "" Then
            MessageBox.Show("Please enter a registration key.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TextBox2.Focus()
            Exit Sub
        ElseIf TextBox2.TextLength < 20 Then
            attempts = attempts + 1
            If attempts = 5 Then
                MessageBox.Show("Sorry, that registration name and key is incorrect.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.Close()
            Else
                MessageBox.Show("Sorry, that registration name and key is incorrect.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            Exit Sub
        End If

        attempts = attempts + 1
        Dim gk, pk As String 'Generated key, provided key
        pk = TextBox2.Text
        gk = generateregistrationkey(TextBox1.Text)

        pk = pk.Replace("-", "")
        gk = gk.Replace("-", "")

        pk = pk.Remove(0, 3)
        pk = pk.Remove(7, 1)
        pk = pk.Remove(pk.Length - 2, 2)
        gk = gk.Remove(0, 3)
        gk = gk.Remove(7, 1)
        gk = gk.Remove(gk.Length - 2, 2)

        If pk = gk Then
            MessageBox.Show("Thank you for registering this copy of Backupinator!", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Global_RegisteredTo = TextBox1.Text
            Me.Close()
            Exit Sub
        End If

        If attempts = 5 Then
            MessageBox.Show("Sorry, that registration name and key is incorrect.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Me.Close()
        Else
            MessageBox.Show("Sorry, that registration name and key is incorrect.", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

End Class