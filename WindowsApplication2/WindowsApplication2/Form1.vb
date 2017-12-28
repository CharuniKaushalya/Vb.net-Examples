Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1
    Dim serverSocket As TcpListener
    Dim clientSocket As TcpClient
    Dim clientList As New List(Of ClientConnection)
    Dim pReader As StreamReader
    Dim pClient As ClientConnection

    Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Close()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Form3.Show()
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles btnServerStart.Click
        serverSocket = New TcpListener(IPAddress.Any, 8888)
        serverSocket.Start()
        txtConsole.Text = ">> " + "Server Started" & vbNewLine
        serverSocket.BeginAcceptTcpClient(New AsyncCallback(AddressOf AcceptClient), serverSocket)
        btnServerStart.Enabled = False
    End Sub

    Sub AcceptClient(ByVal ar As IAsyncResult)
        pClient = New ClientConnection(serverSocket.EndAcceptTcpClient(ar))
        AddHandler(pClient.getMessage), AddressOf MessageReceived
        AddHandler(pClient.clientLogout), AddressOf ClientExited
        clientList.Add(pClient)
        UpdateList("New Client Joined!", True)
        serverSocket.BeginAcceptTcpClient(New AsyncCallback(AddressOf AcceptClient), serverSocket)
    End Sub

    Sub MessageReceived(ByVal str As String)
        UpdateList("New Client Joined!", True)
        UpdateList(str, True)

    End Sub
    Sub ClientExited(ByVal client As ClientConnection)
        clientList.Remove(client)
        UpdateList("Client Exited!", True)
    End Sub

    Delegate Sub _cUpdate(ByVal str As String, ByVal relay As Boolean)
    Sub UpdateList(ByVal str As String, ByVal relay As Boolean)
        On Error Resume Next
        If InvokeRequired Then
            Invoke(New _cUpdate(AddressOf UpdateList), str, relay)
        Else
            txtConsole.AppendText(">> " + str & vbNewLine)
            ' if relay we will send a string
            If relay Then send(str)
        End If
    End Sub
    Sub send(ByVal str As String)
        For x As Integer = 0 To clientList.Count - 1
            Try
                clientList(x).Send(str)
            Catch ex As Exception
                clientList.RemoveAt(x)
            End Try
        Next
    End Sub

    Private Sub Button1_Click_2(sender As Object, e As EventArgs) Handles Button1.Click
        For x As Integer = 0 To clientList.Count - 1
            Try
                Debug.Print(clientList(x).ClientIP)
                UpdateList(clientList(x).ClientIP, True)
            Catch ex As Exception
            End Try
        Next
    End Sub
End Class
