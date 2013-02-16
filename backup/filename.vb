Public Class filename

    Public prefix As String
    Public filename As String

    Public Sub New(ByVal FilePrefix As String, ByVal FileFilename As String)
        prefix = FilePrefix
        filename = FileFilename
    End Sub

    Public Function Complete() As String
        Return prefix + filename
    End Function

    Public Sub SetVars(ByVal FullFileName As String, ByVal FilePrefix As String)
        prefix = FilePrefix
        filename = FullFileName.Substring(FilePrefix.Length, FullFileName.Length - FilePrefix.Length)
    End Sub


End Class
