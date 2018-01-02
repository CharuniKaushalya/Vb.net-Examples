'Provides a container object to serve as the state object for async client and stream operations
Imports System.Net.Sockets

Public Class ConnectionInfo

    Public Property ID As Integer
    'hold a reference to entire monitor instead of just the listener
    Private _Monitor As MonitorInfo
    Public ReadOnly Property Monitor As MonitorInfo
        Get
            Return _Monitor
        End Get
    End Property

    Public _Client As TcpClient
    Public ReadOnly Property Client As TcpClient
        Get
            Return _Client
        End Get
    End Property

    Private _Stream As NetworkStream
    Public ReadOnly Property Stream As NetworkStream
        Get
            Return _Stream
        End Get
    End Property

    Private _DataQueue As System.Collections.Concurrent.ConcurrentQueue(Of Byte)
    Public ReadOnly Property DataQueue As System.Collections.Concurrent.ConcurrentQueue(Of Byte)
        Get
            Return _DataQueue
        End Get
    End Property

    Private _LastReadLength As Integer
    Public ReadOnly Property LastReadLength As Integer
        Get
            Return _LastReadLength
        End Get
    End Property

    Public Property MachineInfo As MachineInfo

    'The buffer size is an arbitrary value which should be selected based on the
    'amount of data you need to transmit, the rate of transmissions, and the
    'anticipalted number of clients. These are the considerations for designing
    'the communicaition protocol for data transmissions, and the size of the read
    'buffer must be based upon the needs of the protocol.
    Private _Buffer(63) As Byte

    Public Sub New(monitor As MonitorInfo)
        _Monitor = monitor
        _DataQueue = New System.Collections.Concurrent.ConcurrentQueue(Of Byte)
    End Sub

    Public Sub AcceptClient(result As IAsyncResult)
        _Client = _Monitor.Listener.EndAcceptTcpClient(result)
        Debug.Print(_Client.Client.RemoteEndPoint.ToString)
        If _Client IsNot Nothing AndAlso _Client.Connected Then
            _Stream = _Client.GetStream
        End If
    End Sub

    Public Sub AwaitData()
        _Stream.BeginRead(_Buffer, 0, _Buffer.Length, AddressOf DoReadData, Me)
    End Sub

    Private Sub DoReadData(result As IAsyncResult)
        Dim info As ConnectionInfo = CType(result.AsyncState, ConnectionInfo)
        Try
            'If the stream is valid for reading, get the current data and then
            'begin another async read
            If info.Stream IsNot Nothing AndAlso info.Stream.CanRead Then
                info._LastReadLength = info.Stream.EndRead(result)
                For index As Integer = 0 To _LastReadLength - 1
                    info._DataQueue.Enqueue(info._Buffer(index))
                Next

                'The example responds to all data reception with the number of bytes received;
                'you would likely change this behavior when implementing your own protocol.
                info.SendMessage("Received " & info._LastReadLength & " Bytes")

                For Each otherInfo As ConnectionInfo In info.Monitor.Connections
                    If Not otherInfo Is info Then
                        otherInfo.SendMessage(System.Text.Encoding.ASCII.GetString(info._Buffer))
                    End If
                Next

                info.AwaitData()
            Else
                'If we cannot read from the stream, the example assumes the connection is
                'invalid and closes the client connection. You might modify this behavior
                'when implementing your own protocol.
                info.Client.Close()
            End If
        Catch ex As Exception
            info._LastReadLength = -1
        End Try
    End Sub

    Private Sub SendMessage(message As String)
        If _Stream IsNot Nothing Then
            Dim messageData() As Byte = System.Text.Encoding.ASCII.GetBytes(message)
            Stream.Write(messageData, 0, messageData.Length)
        End If
    End Sub
End Class