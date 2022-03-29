Imports Newtonsoft.Json
Imports System.IO
Imports QuickMailer.Tools
Imports System.Text
Public Class Settings
    Shared ReadOnly Lock As String = RabbitHole.Dive(Mem.Keys(1), Mem.Keys(0), Mem.Keys(2))
    Public Shared Sub Check()
        If Not File.Exists(Lock) Then
            WL("Settings File Not Found.  Would you like to create one (Y/N)?")
            If IsItYN(RL()) = True Then
                CreateSettings()
            Else
                Dim Msg As String = "Missing Settings File.  Cannot Continue."
                WL(Msg)
                WriteToLog(Msg, Nothing)
                Environment.Exit(0)
            End If
        End If
        LoadSettings()
    End Sub
    Private Shared Function SettingsData(data() As String) As String
        Dim DataSet As New SetData() With {
            .Username = data(0),
            .Password = data(1),
            .Server = data(2),
            .Port = data(3),
            .SSL = data(4)
            }
        Dim SetFinal As New List(Of SetData)() From {DataSet}
        Return JsonConvert.SerializeObject(SetFinal)
    End Function
    Public Shared Sub CreateSettings()
        Try
            CreateSettingsFile(BuildSettings())
            WL("New Settings File Created.")
            Environment.Exit(0)
        Catch ex As Exception
            Dim Msg As String = "Something went wrong trying to create the Settings file."
            WL(Msg)
            WriteToLog(Msg, ex)
        End Try
    End Sub
    Private Shared Function BuildSettings() As String()
        WL("Please enter the SMTP E-Mail address:")
        Dim D0 As String = RL()
        If IsItAddress(D0) = False Then Invalid("e-mail address")
        WL("Please enter the SMTP E-Mail password (characters will be hidden):")
        Dim D1 As String = SetPassword()
        If D1.Length = 0 Then Invalid("password")
        WL("Please enter the SMTP E-Mail Server or IP:")
        Dim D2 As String = RL()
        If IsItServer(D2) = False Then Invalid("server or IP")
        WL("Please enter the SMTP E-Mail Port:")
        Dim D3 As String = RL()
        If IsItNumber(D3) = False Then Invalid("port number")
        WL("Is this Port SSL (Y/N)?")
        Dim D4 As String, Q4 As String = RL()
        If IsItYN(Q4) = True Then D4 = "True" Else D4 = "False"
        Return {D0, D1, D2, D3, D4}
    End Function
    Private Shared Sub Invalid(type As String)
        WL($"Invalid {type} entered.  Please try again")
        Environment.Exit(0)
    End Sub
    Private Shared Sub CreateSettingsFile(rawdata As String())
        If File.Exists(Lock) Then File.Delete(Lock)
        Dim fs As FileStream = File.Create(Lock)
        Dim JsonData As String = SettingsData({rawdata(0), rawdata(1), rawdata(2), rawdata(3), rawdata(4)})
        Dim data As Byte() = New UTF8Encoding(True).GetBytes(RabbitHole.Jump(Mem.Keys(1), Mem.Keys(0), JsonData))
        fs.Write(data, 0, data.Length)
        fs.Close()
        fs.Dispose()
    End Sub
    Private Shared Function SetPassword() As String
        Dim input As New StringBuilder()
        While True
            Dim key = Console.ReadKey(True)
            If key.Key = ConsoleKey.Enter Then Exit While
            If key.Key = ConsoleKey.Backspace AndAlso input.Length > 0 Then
                input.Remove(input.Length - 1, 1)
            ElseIf key.Key <> ConsoleKey.Backspace Then
                input.Append(key.KeyChar)
            End If
        End While
        Return input.ToString()
    End Function
    Public Shared Sub LoadSettings()
        Dim reader As StreamReader
        reader = My.Computer.FileSystem.OpenTextFileReader($"{Lock}")
        Dim Love As String = reader.ReadToEnd()
        reader.Close()
        reader.Dispose()
        Dim RawData As String = RabbitHole.Dive(Mem.Keys(1), Mem.Keys(0), Love)
        Dim Converted = JsonConvert.DeserializeObject(Of List(Of SetData))(RawData)
        Mem.Data = {Converted(0).Username.ToString, Converted(0).Password.ToString,
            Converted(0).Server.ToString, Converted(0).Port.ToString, Converted(0).SSL.ToString}
    End Sub
End Class
Public Class SetData
    Public Property Username As String
    Public Property Password As String
    Public Property Server As String
    Public Property Port As String
    Public Property SSL As String
End Class
