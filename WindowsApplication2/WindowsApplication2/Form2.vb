Public Class Form2

    Dim eid As String = ""
    Public Sub New(ByVal empid As String)
        InitializeComponent()
        eid = empid
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
        TextBox1.Text = eid



        Txt5.Text = "10"
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