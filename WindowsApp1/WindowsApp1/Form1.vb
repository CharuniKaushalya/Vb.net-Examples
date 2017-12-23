Public Class Form1
    Shared random As New Random()
    Dim duration As Date

    Function UpdataTextBox()
        ' For Sensor 01 */
        Dim str As String
        Dim strArr() As String
        Dim count As Integer
        str = "1,2,3,4,5,6"
        strArr = str.Split(",")
        MsgBox("Sensor 1 data: " + str)
        For count = 0 To strArr.Length - 1
            TextBox1.Text = strArr(count)
        Next

        ' For Sensor 02 */
        Dim sensor_data2 As String
        Dim sensorArr2() As String
        Dim count2 As Integer
        sensor_data2 = "1," + Convert.ToString(random.Next(10, 20)) + "," + Convert.ToString(random.Next(10, 20)) + "," + Convert.ToString(random.Next(10, 20))
        sensorArr2 = sensor_data2.Split(",")
        MsgBox("Sensor 2 data: " + sensor_data2)
        For count2 = 0 To sensorArr2.Length - 1
            TextBox2.Text = sensorArr2(count2)
        Next

    End Function

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        UpdataTextBox()
        duration = DateTime.Now.AddMinutes(1)
        Timer1.Start()


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim ts As TimeSpan = duration - DateTime.Now.AddSeconds(-1)
        Label4.Text = ts.Minutes.ToString("00") & ":" & ts.Seconds.ToString("00")
        If Label4.Text = "00:00" Then
            Timer1.Stop()
            MsgBox("Update Sensor textboxes!")
            UpdataTextBox()
            duration = DateTime.Now.AddMinutes(1)
            Timer1.Start()
        End If

    End Sub
End Class
