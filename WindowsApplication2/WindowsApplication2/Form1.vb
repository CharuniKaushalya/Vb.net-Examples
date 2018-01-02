Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1
    Dim serverSocket As TcpListener
    Dim clientSocket As TcpClient

    Private _Listener As TcpListener
    Private _Connections As New List(Of ConnectionInfo)
    Private _ConnectionMontior As Task
    Private ServerStarted As Boolean = False
    'Create delegate for updating output display
    Dim doAppendOutput As New Action(Of String)(AddressOf AppendOutput)
    'Create delegate for updating connection count label
    Dim doUpdateConnectionCountLabel As New Action(AddressOf UpdateConnectionCountLabel)



    Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Close()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Form3.Show()
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Machine2.Click
        Dim pass As String = "10"
        Dim frm As New Form2(pass)
        frm.ShowDialog()
        Me.Hide()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtConsole.Text = ">> " + "Please Start Server" & vbNewLine
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles btnServerStart.Click
        If ServerStarted = False Then
            ServerStarted = True
            _Listener = New TcpListener(IPAddress.Any, CInt(PortTextBox.Text))
            _Listener.Start()
            Dim monitor As New MonitorInfo(_Listener, _Connections)
            ListenForClient(monitor)
            _ConnectionMontior = Task.Factory.StartNew(AddressOf DoMonitorConnections, monitor, TaskCreationOptions.LongRunning)
            txtConsole.Text = ">> " + "Server Started" & vbNewLine
            btnServerStart.Text = "Stop Server"
        Else
            ServerStarted = False
            CType(_ConnectionMontior.AsyncState, MonitorInfo).Cancel = True
            _Listener.Stop()
            _Listener = Nothing
            btnServerStart.Text = "Start Server"
        End If
    End Sub

    Private Sub Button1_Click_2(sender As Object, e As EventArgs) Handles btnPrintClients.Click
        ''add Print Client Code
        Machine4.BackColor = Color.Blue


        For x As Integer = 0 To _Connections.Count - 1
            Try
                Debug.Print(_Connections(x)._Client.Client.RemoteEndPoint.ToString)
                'UpdateList(clientList(x).ClientIP, True)
                Me.Invoke(doAppendOutput, _Connections(x)._Client.Client.RemoteEndPoint.ToString)
            Catch ex As Exception
            End Try


        Next
    End Sub



    Private Sub ListenForClient(monitor As MonitorInfo)
        Dim info As New ConnectionInfo(monitor)
        _Listener.BeginAcceptTcpClient(AddressOf DoAcceptClient, info)
    End Sub

    Private Sub DoAcceptClient(result As IAsyncResult)
        Dim monitorInfo As MonitorInfo = CType(_ConnectionMontior.AsyncState, MonitorInfo)
        If monitorInfo.Listener IsNot Nothing AndAlso Not monitorInfo.Cancel Then
            Dim info As ConnectionInfo = CType(result.AsyncState, ConnectionInfo)
            monitorInfo.Connections.Add(info)
            info.AcceptClient(result)
            ListenForClient(monitorInfo)
            info.AwaitData()
            Dim doUpdateConnectionCountLabel As New Action(AddressOf UpdateConnectionCountLabel)
            Invoke(doUpdateConnectionCountLabel)
        End If
    End Sub



    Private Sub DoMonitorConnections()
        'Get MonitorInfo instance from thread-save Task instance
        Dim monitorInfo As MonitorInfo = CType(_ConnectionMontior.AsyncState, MonitorInfo)

        'Report progress
        Me.Invoke(doAppendOutput, "Monitor Started.")

        'Implement client connection processing loop
        Do
            'Create temporary list for recording closed connections
            Dim lostCount As Integer = 0
            'Examine each connection for processing
            For index As Integer = monitorInfo.Connections.Count - 1 To 0 Step -1
                Dim info As ConnectionInfo = monitorInfo.Connections(index)
                If info.Client.Connected Then
                    'Process connected client
                    If info.DataQueue.Count > 0 Then
                        'The code in this If-Block should be modified to build 'message' objects
                        'according to the protocol you defined for your data transmissions.
                        'This example simply sends all pending message bytes to the output textbox.
                        'Without a protocol we cannot know what constitutes a complete message, so
                        'with multiple active clients we could see part of client1's first message,
                        'then part of a message from client2, followed by the rest of client1's
                        'first message (assuming client1 sent more than 64 bytes).
                        Dim messageBytes As New List(Of Byte)
                        While info.DataQueue.Count > 0
                            Dim value As Byte
                            If info.DataQueue.TryDequeue(value) Then
                                messageBytes.Add(value)
                            End If
                        End While
                        Dim message As String = System.Text.Encoding.ASCII.GetString(messageBytes.ToArray)

                        ResponseHandler(message)
                        Me.Invoke(doAppendOutput, info.Client.Client.RemoteEndPoint.ToString + ": " + message + ", " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                    End If
                Else
                    'Clean-up any closed client connections
                    monitorInfo.Connections.Remove(info)
                    lostCount += 1
                End If
            Next
            If lostCount > 0 Then
                Invoke(doUpdateConnectionCountLabel)
            End If

            'Throttle loop to avoid wasting CPU time
            _ConnectionMontior.Wait(1)
        Loop While Not monitorInfo.Cancel

        'Close all connections before exiting monitor
        For Each info As ConnectionInfo In monitorInfo.Connections
            info.Client.Close()
        Next
        monitorInfo.Connections.Clear()

        'Update the connection count label and report status
        Invoke(doUpdateConnectionCountLabel)
        Me.Invoke(doAppendOutput, "Monitor Stopped.")
    End Sub

    Private Sub AppendOutput(message As String)
        If txtConsole.TextLength > 0 Then
            txtConsole.AppendText(ControlChars.NewLine)
        End If
        txtConsole.AppendText(">> " + message)
        txtConsole.ScrollToCaret()
    End Sub

    Private Sub UpdateConnectionCountLabel()
        ConnectionCountLabel.Text = String.Format("{0} Connections", _Connections.Count)
        Dim btnName As String = "Machine" + _Connections.Count.ToString
        Dim b As New Button
        b = Me.Controls.Find(btnName, True).First
        b.BackColor = Color.Lime
    End Sub

    Private Sub PortTextBox_Validating_1(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles PortTextBox.Validating
        Dim deltaPort As Integer
        If Not Integer.TryParse(PortTextBox.Text, deltaPort) OrElse deltaPort < 1 OrElse deltaPort > 65535 Then
            MessageBox.Show("Port number must be an integer between 1 and 65535.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            PortTextBox.SelectAll()
            e.Cancel = True
        End If
    End Sub

    Private Sub ResponseHandler(message As String)
        MsgBox(message)
        Dim strArr() As String
        Dim count As Integer
        strArr = message.Split(",")


    End Sub

End Class



