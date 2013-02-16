Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'First determine if Backupinator is already running
            If Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName).Length > 1 Then
                MessageBox.Show("An instance of Backupinator is already running!", "Backupinator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                e.Cancel = True
                Exit Sub
            End If

            'THIS is where we can determine if we should HIDE at runtime
            Dim i As Integer
            For i = 0 To e.CommandLine.Count - 1
                If LCase(e.CommandLine.Item(i)) = "-hidden" Then
                    Global_StartHidden = True
                End If
            Next

        End Sub

    End Class


End Namespace

