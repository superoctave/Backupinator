Module mod_globals

    Public Enum OverwriteSettings
        AlwaysOverwrite
        OverwriteOnlyIfNewer
        NeverOverwrite
    End Enum


    '########## Settings ##########
    Public Setting_OriginalPrefix As String = "C:\Original\"
    Public Setting_BackupPrefix As String = "C:\Backup\"
    Public Setting_IncludeSubfolders As Boolean = True
    Public Setting_Overwrite As OverwriteSettings = OverwriteSettings.OverwriteOnlyIfNewer
    Public Setting_BlacklistFolders As New ArrayList
    Public Setting_BlacklistFileTypes As New ArrayList
    Public Setting_BlacklistFileNames As New ArrayList
    'Backupdate settings
    Public Setting_RunEveryDay As Boolean = True
    Public Setting_RunOnSunday As Boolean = False
    Public Setting_RunOnMonday As Boolean = False
    Public Setting_RunOnTuesday As Boolean = False
    Public Setting_RunOnWednesday As Boolean = False
    Public Setting_RunOnThursday As Boolean = False
    Public Setting_RunOnFriday As Boolean = False
    Public Setting_RunOnSaturday As Boolean = False
    Public Setting_RunTime As DateTime = New DateTime(2000, 1, 1, 0, 0, 0, 0)

    '########## Global vars ##########
    Public LastBackupDate As DateTime
    Public Global_Thread As New System.Threading.Thread(AddressOf frm_running.Backup)
    Public Global_FormRunning As New frm_running
    Public Global_MainForm As Form1
    Public Global_NextRunDate As DateTime
    Public Global_StartHidden As Boolean = False
    'Registration
    Public Global_RegisteredTo As String = "null"

End Module
