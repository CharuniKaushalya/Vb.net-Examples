Public Class Form2

    Dim machineInfo As MachineInfo
    Public Sub New(MachineInfo As MachineInfo)
        InitializeComponent()
        MachineInfo = MachineInfo
        If MachineInfo IsNot Nothing Then
            TextBox5.Text = MachineInfo.TimeForCurrentSean
            TextBox2.Text = MachineInfo.AverageSpeed
            TextBox3.Text = MachineInfo.TotalRunTime
            TextBox4.Text = MachineInfo.StoptimeBetweenSeams
        End If
    End Sub



    Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer

    Enum ProgressBarColor
        Green = &H1
        Red = &H2
        Yellow = &H3
    End Enum

    Private Shared Sub ChangeProgBarColor(ByVal ProgressBar_Name As System.Windows.Forms.ProgressBar, ByVal ProgressBar_Color As ProgressBarColor)
        SendMessage(ProgressBar_Name.Handle, &H410, ProgressBar_Color, 0)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
        Form1.Show()
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click, Button3.Click
        Txt5.Text = Txt5.Text + 1
        ProgressBar1.Value = Txt5.Text
        If (ProgressBar1.Value < 35) Then
            ProgressBar1.ForeColor = Color.Green
        Else
            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Red)
        End If
    End Sub
End Class