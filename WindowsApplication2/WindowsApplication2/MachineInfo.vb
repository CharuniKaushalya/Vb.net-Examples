Public Class MachineInfo
    Private _TimeForCurrentSean As Integer = 0
    Public ReadOnly Property TimeForCurrentSean As Integer
        Get
            Return _TimeForCurrentSean
        End Get
    End Property

    Private _AverageSpeed As Integer = 0
    Public ReadOnly Property AverageSpeed As Integer
        Get
            Return _AverageSpeed
        End Get
    End Property

    Private _TotalRunTime As Integer = 0
    Public ReadOnly Property TotalRunTime As Integer
        Get
            Return _TotalRunTime
        End Get
    End Property

    Private _StoptimeBetweenSeams As Integer = 0
    Public ReadOnly Property StoptimeBetweenSeams As Integer
        Get
            Return _StoptimeBetweenSeams
        End Get
    End Property

    Public Sub New(timeForCurrentSean As Integer, averageSpeed As Integer, totalRunTime As Integer, stoptimeBetweenSeams As Integer)
        _TimeForCurrentSean = timeForCurrentSean
        _AverageSpeed = averageSpeed
        _TotalRunTime = totalRunTime
        _StoptimeBetweenSeams = stoptimeBetweenSeams
    End Sub

End Class
