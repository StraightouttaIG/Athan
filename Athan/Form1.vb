Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Form1
    Dim times As New List(Of String)
    Dim nextPraytime
    Dim notified As Boolean = False
    Dim url As String = "https://www.islamicfinder.org/prayer-widget/110336/shafi/4/0/18.5/10"
    Dim pos As Point

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(112, 72)
        Control.CheckForIllegalCrossThreadCalls = False
        Dim thread As New Thread(AddressOf getTimes) : thread.Start()
        Dim thread2 As New Thread(AddressOf _loop) : thread2.Start()

        Me.Close()
    End Sub
    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += Control.MousePosition - pos
        End If
        pos = Control.MousePosition
    End Sub
    Sub getTimes()
        Do
            If My.Computer.Network.IsAvailable = True Then
                Try
                    'Clear Existing times

                    times.Clear()

                    Dim wc = New Net.WebClient

                    'Get Prayer times
                    Dim response As String = wc.DownloadString(url)
                    Dim matches As MatchCollection = Regex.Matches(response, "<p>(.*?)</p>")
                    For Each m As Match In matches
                        Dim matched = m.Groups.Item(1).Value
                        times.Add(matched)
                    Next
                    times.Remove(times(1))
                    Thread.Sleep(TimeSpan.FromHours(8))
                Catch ex As Exception
                End Try
            End If
        Loop
    End Sub
    Sub _loop()
        While True
            nextprayer()
            Thread.Sleep(TimeSpan.FromSeconds(2))
        End While
    End Sub
    Sub nextprayer()
        Try
            For i = 0 To times.Count - 1
                Dim curr As Date = Date.Now
                Dim prayerTime As New Date(curr.Year, curr.Month, curr.Day, CDate(times(i)).Hour, CDate(times(i)).Minute, 0)
                If (curr <= prayerTime) Then
                    nextPraytime = prayerTime
                    Me.Text = "Next: " + prayerTime.ToShortTimeString
                    TimeChecker()
                    Exit Sub
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
    Sub TimeChecker()
        Try
            Dim curr As Date = Date.Now
            Dim prayerTime As New Date(curr.Year, curr.Month, curr.Day, CDate(nextPraytime).Hour, CDate(nextPraytime).Minute, 0)
            If (curr <= prayerTime.AddSeconds(-10)) Then
                'not due yet

                notified = False
            Else
                'due in 10 seconds

                If Not notified Then
                    NotifyIcon1.Visible = True
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
                    NotifyIcon1.BalloonTipTitle = "Athan"
                    For i = 0 To times.Count - 1
                        If CDate(times(i)) = prayerTime.ToString("hh:mm tt") Then
                            If i = 0 Then
                                NotifyIcon1.BalloonTipText = $"أذان الفجر"
                            ElseIf i = 1 Then
                                NotifyIcon1.BalloonTipText = "أذان الظهر"
                            ElseIf i = 2 Then
                                NotifyIcon1.BalloonTipText = "أذان العصر"
                            ElseIf i = 3 Then
                                NotifyIcon1.BalloonTipText = "أذان المغرب"
                            ElseIf i = 4 Then
                                NotifyIcon1.BalloonTipText = "أذان العشاء"
                            End If
                            NotifyIcon1.ShowBalloonTip(50000)
                        End If
                    Next
                    notified = True
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
#Region "Form1 Close/Hide Events"
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        'Hides app in background and show a Notification
        e.Cancel = True
        NotifyIcon1.Visible = True
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
        NotifyIcon1.BalloonTipTitle = "Application is running in background"
        NotifyIcon1.BalloonTipText = "Click the Application icon to open the App"
        NotifyIcon1.ShowBalloonTip(50000)
        Me.Hide()
        ShowInTaskbar = False
    End Sub
    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click

        Me.Show()
        ShowInTaskbar = True
        NotifyIcon1.Visible = False
    End Sub

#End Region
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        End
    End Sub
End Class
