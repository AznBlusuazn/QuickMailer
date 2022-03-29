Imports ClarkTribeGames
Imports QuickMailer.Tools
Imports System.IO
Imports System.Net.Mail
Module Engine
    Sub Main()
        Mem.LogFile = $"{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmm}.log"
        Dim clArgs() As String = Environment.GetCommandLineArgs()
        Try
            If clArgs(1).ToLower = "-new" Then Settings.CreateSettings()
            If clArgs(1).ToLower = "-update" Then CheckUpdate(True)
        Catch
        End Try
        CheckUpdate(False)
        Settings.Check()
        Try
            Dim test As Integer = clArgs(1).Length + clArgs(2).Length + clArgs(3).Length
            If test > 0 Then
                If clArgs(1).Length = 0 Or clArgs(2).Length = 0 Or clArgs(3).Length = 0 Then
                    ArgumentsError()
                    Environment.Exit(0)
                End If
            End If
        Catch ex As Exception
            ArgumentsError()
            WriteToLog("Incorrect Arguments in Command Line", ex)
        End Try
        Try
            Dim Smtp As New SmtpClient
            Dim Mail As New MailMessage()
            Smtp.UseDefaultCredentials = False
            Smtp.Credentials = New Net.NetworkCredential(Mem.Data(0), Mem.Data(1))
            Smtp.Port = CInt(Mem.Data(3))
            If Mem.Data(4) = "True" Then Smtp.EnableSsl = True Else Smtp.EnableSsl = False
            Smtp.Host = Mem.Data(2)
            Mail.From = New MailAddress(Mem.Data(0))
            Mail.To.Add(clArgs(1))
            Mail.Subject = clArgs(2)
            Mail.IsBodyHtml = False
            Mail.Body = clArgs(3)
            Smtp.Send(Mail)
            WL($"Email Sent to {clArgs(1)}.")
        Catch ex As Exception
            Dim Msg As String = "Something went wrong trying to send email."
            WL(Msg)
            WriteToLog(Msg, ex)
        End Try
    End Sub
    Private Sub CheckUpdate(requested As Boolean)
        Try
            Dim Version As String = Converters.GetVersion(My.Application.Info.Version.ToString)
            Dim Prog As String = RabbitHole.Dive(Mem.Keys(1), Mem.Keys(0), Mem.Keys(3))
            Dim UDate As String = MySQLReader.Query(LCase(Prog).Replace(".exe", ""), "d")
            If File.Exists(Prog) Then
                If File.GetLastWriteTime(Prog) < Convert.ToDateTime(UDate) Then
                    File.Delete(Prog)
                    Updater.GetUpdater()
                End If
            Else
                Updater.GetUpdater()
            End If
            Dim Available As String = MySQLReader.Query(LCase(My.Application.Info.ProductName.ToString()), "v")
            Dim URL As String = MySQLReader.Query(LCase(My.Application.Info.ProductName.ToString()), "u")
            Updater.Checker(Version, Available)
            If Updater.Checker(Version, Available) = True Then
                WL($"Update {Available} Available!{vbCrLf}{vbCrLf}Would you like to update now (Y/N)?")
                If IsItYN(RL()) = True Then Updater.InstallUpdate(My.Application.Info.ProductName.ToString, URL) _
                    Else WL("Please update as soon as possible!")
            Else
                If requested = True Then WL("No update available.")
            End If
        Catch ex As Exception
            Dim Msg As String = "Something went wrong with the CTGUpdater."
            WL(Msg)
            WriteToLog(Msg, ex)
        End Try
        If requested = True Then Environment.Exit(0)
    End Sub
    Private Sub ArgumentsError()
        WL("Please check your arguments.  Your arguments should be in this format (note the quotes):")
        WL(" ")
        WL("QuickMailer.exe toaddress@server.com ""Subject"" ""Body Text""")
        WL(" ")
        WL("To create a new settings file:  QuickMailer.exe -new")
        WL("To check for program updates:   QuickMailer.exe -update")
        WL(" ")
    End Sub
End Module
