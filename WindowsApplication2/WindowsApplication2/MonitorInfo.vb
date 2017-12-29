'Provides a simple container object to be used for the state object passed to the connection monitoring thread
Imports System.Net.Sockets

Public Class MonitorInfo
    Public Property Cancel As Boolean

    Private _Connections As List(Of ConnectionInfo)
    Public ReadOnly Property Connections As List(Of ConnectionInfo)
        Get
            Return _Connections
        End Get
    End Property

    Private _Listener As TcpListener
    Public ReadOnly Property Listener As TcpListener
        Get
            Return _Listener
        End Get
    End Property

    Public Sub New(tcpListener As TcpListener, connectionInfoList As List(Of ConnectionInfo))
        _Listener = tcpListener
        _Connections = connectionInfoList
    End Sub
End Class
