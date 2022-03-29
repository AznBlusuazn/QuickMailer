Imports ClarkTribeGames
Public Class RabbitHole
    Public Shared Function Dive(sticks As String, stones As String, bones As String) As String
        Dim X As String = New Coder(My.Application.Info.CompanyName.ToString).DecryptData(sticks)
        Return New Coder(X).DecryptData(bones)
    End Function
    Public Shared Function Jump(sticks As String, stones As String, bones As String) As String
        Dim X As String = New Coder(My.Application.Info.CompanyName.ToString).DecryptData(sticks)
        Return New Coder(X).EncryptData(bones)
    End Function
End Class
