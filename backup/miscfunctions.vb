Imports System.Runtime.CompilerServices

Module miscfunctions

#Region " Dates and Times "

    <Extension()>
    Public Function ConvertDayOfWeekToInteger(ByVal date_dayofweek As DayOfWeek) As Integer
        Select Case date_dayofweek
            Case DayOfWeek.Sunday
                Return 1
            Case DayOfWeek.Monday
                Return 2
            Case DayOfWeek.Tuesday
                Return 3
            Case DayOfWeek.Wednesday
                Return 4
            Case DayOfWeek.Thursday
                Return 5
            Case DayOfWeek.Friday
                Return 6
            Case DayOfWeek.Saturday
                Return 7
            Case Else
                Return 1
        End Select
    End Function

    Public Function GetNextDate() As DateTime
        Dim nxd As DateTime = DateTime.Now

        If Setting_RunEveryDay = True Then
            nxd = nxd.AddDays(1)
        Else
            'It's not set to run every day, so determine when the next date is
            Dim cdow As Integer 'Current day of week
            cdow = DateTime.Now.DayOfWeek.ConvertDayOfWeekToInteger
            Dim ndow As Integer 'Next day of week

            ndow = GetNumberOfDaysBeforeNextRun(cdow)
            nxd = nxd.AddDays(ndow)

        End If

        nxd = nxd.SetTime(Setting_RunTime.Hour, Setting_RunTime.Minute, Setting_RunTime.Second)
        Return nxd

    End Function

    Public Function GetNumberOfDaysBeforeNextRun(ByVal dow As Integer) As Integer
        'Is there a more efficient way to do this???
        Select Case dow
            Case 1
                If Setting_RunOnMonday = True Then
                    Return 1
                ElseIf Setting_RunOnTuesday = True Then
                    Return 2
                ElseIf Setting_RunOnWednesday = True Then
                    Return 3
                ElseIf Setting_RunOnThursday = True Then
                    Return 4
                ElseIf Setting_RunOnFriday = True Then
                    Return 5
                ElseIf Setting_RunOnSaturday = True Then
                    Return 6
                ElseIf Setting_RunOnSunday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 2
                If Setting_RunOnTuesday = True Then
                    Return 1
                ElseIf Setting_RunOnWednesday = True Then
                    Return 2
                ElseIf Setting_RunOnThursday = True Then
                    Return 3
                ElseIf Setting_RunOnFriday = True Then
                    Return 4
                ElseIf Setting_RunOnSaturday = True Then
                    Return 5
                ElseIf Setting_RunOnSunday = True Then
                    Return 6
                ElseIf Setting_RunOnMonday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 3
                If Setting_RunOnWednesday = True Then
                    Return 1
                ElseIf Setting_RunOnThursday = True Then
                    Return 2
                ElseIf Setting_RunOnFriday = True Then
                    Return 3
                ElseIf Setting_RunOnSaturday = True Then
                    Return 4
                ElseIf Setting_RunOnSunday = True Then
                    Return 5
                ElseIf Setting_RunOnMonday = True Then
                    Return 6
                ElseIf Setting_RunOnTuesday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 4
                If Setting_RunOnThursday = True Then
                    Return 1
                ElseIf Setting_RunOnFriday = True Then
                    Return 2
                ElseIf Setting_RunOnSaturday = True Then
                    Return 3
                ElseIf Setting_RunOnSunday = True Then
                    Return 4
                ElseIf Setting_RunOnMonday = True Then
                    Return 5
                ElseIf Setting_RunOnTuesday = True Then
                    Return 6
                ElseIf Setting_RunOnWednesday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 5
                If Setting_RunOnFriday = True Then
                    Return 1
                ElseIf Setting_RunOnSaturday = True Then
                    Return 2
                ElseIf Setting_RunOnSunday = True Then
                    Return 3
                ElseIf Setting_RunOnMonday = True Then
                    Return 4
                ElseIf Setting_RunOnTuesday = True Then
                    Return 5
                ElseIf Setting_RunOnWednesday = True Then
                    Return 6
                ElseIf Setting_RunOnThursday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 6
                If Setting_RunOnSaturday = True Then
                    Return 1
                ElseIf Setting_RunOnSunday = True Then
                    Return 2
                ElseIf Setting_RunOnMonday = True Then
                    Return 3
                ElseIf Setting_RunOnTuesday = True Then
                    Return 4
                ElseIf Setting_RunOnWednesday = True Then
                    Return 5
                ElseIf Setting_RunOnThursday = True Then
                    Return 6
                ElseIf Setting_RunOnFriday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case 7
                If Setting_RunOnSunday = True Then
                    Return 1
                ElseIf Setting_RunOnMonday = True Then
                    Return 2
                ElseIf Setting_RunOnTuesday = True Then
                    Return 3
                ElseIf Setting_RunOnWednesday = True Then
                    Return 4
                ElseIf Setting_RunOnThursday = True Then
                    Return 5
                ElseIf Setting_RunOnFriday = True Then
                    Return 6
                ElseIf Setting_RunOnSaturday = True Then
                    Return 7
                Else
                    Return 1
                End If
            Case Else
                Return 1
        End Select
    End Function

    Public Function GetSimpleDateAndTime(ByVal dt As DateTime) As String
        If dt = DateTime.MinValue Then
            Return "Unknown"
        Else
            Return dt.Month.ToString + "/" + dt.Day.ToString + "/" + dt.Year.ToString + " at " + dt.ToShortTimeString
        End If
    End Function

    <Extension()>
    Public Function SetTime(ByVal originaldate As DateTime, ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer) As DateTime
        'I can't believe it isn't possible to set the TIME on a datetime var.
        Return New DateTime(originaldate.Year, originaldate.Month, originaldate.Day, hours, minutes, seconds)
    End Function

#End Region

#Region " Encryption and Registration "

    Public Function generateregistrationkey(ByVal name As String) As String
        'This can be done a million different ways. This is ONE.
        name = UCase(name)
        Dim ns As String = ""
        Dim ss As String
        Dim i As Integer
        Dim objRandom As New System.Random(CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))
        For i = 0 To name.Length - 1
            ss = name.Substring(i, 1)
            If ss = "B" Or ss = "C" Or ss = "D" Or ss = "F" Or ss = "G" Or ss = "H" Or ss = "J" Or ss = "K" Or ss = "L" Or ss = "M" Or ss = "N" Or ss = "P" Or ss = "Q" Or ss = "R" Or ss = "S" Or ss = "T" Or ss = "V" Or ss = "W" Or ss = "X" Or ss = "Z" Then
                ns = ss + ns
            ElseIf ss = "A" Then
                ns = "Y" + ns
            ElseIf ss = "E" Then
                ns += "I"
            ElseIf ss = "I" Then
                ns = "U" + ns
            ElseIf ss = "O" Then
                ns = "E" + ns
            ElseIf ss = "U" Then
                ns += "O"
            ElseIf ss = "Y" Then
                ns = "A" + ns
            End If
        Next

        ns = ns + "QXIOUEWRNIJGJLKMIONT" 'Somewhat random.
        ns = ns.Substring(0, 20)
        ns = ns.Substring(12, 8) + ns.Substring(0, 12)

        ns = ns.Replace("Z", "W")
        name = ""
        For i = 0 To ns.Length - 1
            name += Chr(Asc(ns.Substring(i, 1)) + 1)
        Next

        'Throw in some random characters just for fun!
        'Note: The more random characters you add, the harder it will be to figure out what the
        'encrypting scheme is; but also means that there are multiple possibilities for a key and
        'could be guessed easier.
        name = name.Remove(0, 3)
        name = Chr(objRandom.Next(65, 91)) + name
        name = Chr(objRandom.Next(65, 91)) + name
        name = Chr(objRandom.Next(65, 91)) + name
        name = name.Substring(0, 10) + Chr(objRandom.Next(65, 91)) + name.Substring(11, 9)
        name = name.Remove(name.Length - 2, 2)
        name = name + Chr(objRandom.Next(65, 91))
        name = name + Chr(objRandom.Next(65, 91))

        name = name.Insert(5, "-")
        name = name.Insert(11, "-")
        name = name.Insert(17, "-")
        Return name
    End Function

    Public Function Encrypt(ByVal str As String) As String
        'Essentially copied from the internet
        Dim lonDataPtr As Long
        Dim strDataOut As String = ""
        Dim temp As Integer
        Dim tempstring As String
        Dim intXOrValue1 As Integer
        Dim intXOrValue2 As Integer
        For lonDataPtr = 1 To Len(str)
            intXOrValue1 = Asc(Mid$(str, lonDataPtr, 1))
            intXOrValue2 = Asc(Mid$("123", ((lonDataPtr Mod Len("123")) + 1), 1))
            temp = (intXOrValue1 Xor intXOrValue2)
            tempstring = Hex(temp)
            If Len(tempstring) = 1 Then tempstring = "0" & tempstring
            strDataOut = strDataOut + tempstring
        Next lonDataPtr
        Return strDataOut
    End Function

    Public Function Decrypt(ByVal str As String) As String
        Dim lonDataPtr As Long
        Dim strDataOut As String = ""
        Dim intXOrValue1 As Integer
        Dim intXOrValue2 As Integer
        For lonDataPtr = 1 To (Len(str) / 2)
            intXOrValue1 = Val("&H" & (Mid$(str, (2 * lonDataPtr) - 1, 2)))
            intXOrValue2 = Asc(Mid$("123", ((lonDataPtr Mod Len("123")) + 1), 1))
            strDataOut = strDataOut + Chr(intXOrValue1 Xor intXOrValue2)
        Next lonDataPtr
        Return strDataOut
    End Function

#End Region

#Region " Type Conversion "

    Public Function SplitArrayList(ByVal ArrayListToSplit As ArrayList, ByVal character As String, Optional ByVal ReturnNullValueIfEmpty As Boolean = False) As String
        If ArrayListToSplit.Count = 0 Then
            If ReturnNullValueIfEmpty = True Then
                Return "null"
            Else
                Return ""
            End If
        End If
        If ArrayListToSplit.Count = 1 Then
            Return ArrayListToSplit.Item(0).ToString
        End If
        Dim i As Integer
        Dim returnstring As String = ""
        For i = 0 To ArrayListToSplit.Count - 1
            If returnstring = "" Then
                returnstring = ArrayListToSplit.Item(i)
            Else
                returnstring = returnstring + character + ArrayListToSplit.Item(i)
            End If
        Next
        Return returnstring
    End Function

    Public Function SplitStringToArrayList(ByVal StringToConvert As String, ByVal character As String) As ArrayList
        If StringToConvert = "" Then
            Return New ArrayList
        End If
        Dim arraylisttoreturn As New ArrayList
        If StringToConvert.IndexOf(character) = -1 Then
            arraylisttoreturn.Add(StringToConvert)
            Return arraylisttoreturn
        End If
        Dim sta As String
        Do Until StringToConvert.IndexOf(character) = -1
            sta = StringToConvert.Substring(0, StringToConvert.IndexOf(character))
            arraylisttoreturn.Add(sta)
            StringToConvert = StringToConvert.Remove(0, sta.Length + 1)
            If character = vbCrLf Then
                'For some reason, the VBCRLF character equals the space of two characters
                StringToConvert = StringToConvert.Remove(0, 1)
            End If
            If StringToConvert.IndexOf(character) = -1 Then
                arraylisttoreturn.Add(StringToConvert)
            End If
        Loop
        Return arraylisttoreturn
    End Function

#End Region

#Region " Miscellaneous "

    Public Function FolderBackslash(ByVal str As String) As String
        If str.Substring(str.Length - 1, 1) <> "\" Then
            str = str + "\"
        End If
        Return str
    End Function

    Public Function DeterminePlural(ByVal int As Integer, ByVal str As String, Optional ByVal pluralversion As String = "") As String
        'Determines if the number should be listed as plural
        'Also specifies a plural version (like "boxES"), if there is one
        If int = 1 Then
            Return str
        Else
            'Use plural version
            If pluralversion = "" Then
                Return str + "s"
            Else
                Return pluralversion
            End If
        End If
    End Function

    Public Function GetSimpleFileName(ByVal fn As String) As String
        If fn.IndexOf("\") = -1 Then
            Return fn
        Else
            Return fn.Substring(fn.LastIndexOf("\") + 1)
        End If
    End Function

    Public Function GetExtension(ByVal filename As String) As String
        If filename.IndexOf(".") = -1 Then
            Return ""
        Else
            Return filename.Substring(filename.LastIndexOf(".") + 1)
        End If
        Exit Function
    End Function

#End Region

End Module
