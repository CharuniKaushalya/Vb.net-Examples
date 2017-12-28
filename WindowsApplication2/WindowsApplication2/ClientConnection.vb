Imports System.Net.Sockets
Imports System.IO
Public Class ClientConnection
    Public Event getMessage(ByVal str As String)
    Public Event clientLogout(ByVal client As ClientConnection)
    Private sendMessage As StreamWriter
    Private listClient As TcpClient
    Public ClientIP As String
    Sub New(ByVal forClient As TcpClient)
        listClient = forClient
        ClientIP = forClient.Client.RemoteEndPoint.ToString
        Dim bytesFrom(10024) As Byte
        listClient.GetStream.BeginRead(bytesFrom, 0, bytesFrom.Length, AddressOf ReadAllClient, Nothing)

    End Sub
    Private Sub ReadAllClient()
        Try
            RaiseEvent getMessage(New StreamReader(listClient.GetStream).ReadLine)
            Dim networkStream As NetworkStream = listClient.GetStream()

            Dim bytesFrom(10024) As Byte

            networkStream.Read(bytesFrom, 0, bytesFrom.Length)

            Dim dataFromClient As String = System.Text.Encoding.ASCII.GetString(bytesFrom)
            Debug.Print("Data came-2 " + dataFromClient)

            listClient.GetStream.BeginRead(New Byte() {0}, 0, 0, New AsyncCallback(AddressOf ReadAllClient), Nothing)
        Catch ex As Exception
            RaiseEvent clientLogout(Me)
        End Try
    End Sub
    Public Sub Send(ByVal Messsage As String)
        sendMessage = New StreamWriter(listClient.GetStream)
        sendMessage.WriteLine(Messsage)
        sendMessage.Flush()
    End Sub
End Class
