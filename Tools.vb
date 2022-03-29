Imports System.IO
Imports System.Text.RegularExpressions
Public Class Tools
    Public Shared Sub WL(text As String)
        Console.WriteLine(text)
    End Sub
    Public Shared Function RL() As String
        Return Console.ReadLine()
    End Function
    Public Shared Function IsItAddress(addy As String) As Boolean
        If addy.Contains("@") And addy.Contains(".") Then Return True Else Return False
    End Function
    Public Shared Function IsItServer(addy As String) As Boolean
        If Regex.Replace(addy, "[^0-9\-/]", "").Length > 0 Then
            If (addy.Split(".").Count - 1) = 3 Then Return True Else Return False
        Else
            If addy.Contains(".") Then Return True Else Return False
        End If
    End Function
    Public Shared Function IsItNumber(text As String) As Boolean
        If Regex.Replace(text, "[^0-9]", "").Length > 0 Then Return True Else Return False
    End Function
    Public Shared Function IsItYN(text As String) As Boolean
        If text.ToLower.StartsWith("y") Then Return True Else Return False
    End Function
    Public Shared Sub WriteToLog(info As String, ex As Exception)
        Dim DateStamp As String = $"{DateTime.Now:yyyyMMdd}"
        Dim TimeStamp As String = $"{DateTime.Now:HHmm}"
        Dim fs As New FileStream(Mem.LogFile, FileMode.OpenOrCreate,
            FileAccess.ReadWrite)
        Dim s As New StreamWriter(fs)
        s.Close()
        fs.Close()
        Dim fs1 As New FileStream(Mem.LogFile, FileMode.Append,
            FileAccess.Write)
        Dim s1 As New StreamWriter(fs1)
        s1.WriteLine($"Date/Time:{DateStamp}_{TimeStamp}:{info}")
        If ex IsNot Nothing Then
            s1.WriteLine("Exception:")
            s1.WriteLine(ex.ToString)
        End If
        s1.WriteLine("================================================")
        s1.Close()
        fs1.Close()
        s.Dispose()
        fs.Dispose()
        s1.Dispose()
        fs1.Dispose()
    End Sub
End Class
