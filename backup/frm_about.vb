Public Class frm_about

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If LCase(Global_RegisteredTo) = "null" Or Global_RegisteredTo <> "" Then
            Dim frm As New frm_enterregistrationkey
            frm.ShowDialog()
            If Global_RegisteredTo <> "null" And Global_RegisteredTo <> "" Then
                Button2.Enabled = False
                Label3.Text = "Registered to " + Global_RegisteredTo
            End If
        End If
    End Sub

End Class