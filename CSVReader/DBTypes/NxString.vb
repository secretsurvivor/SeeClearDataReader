Public Class NxString
    Implements NxAttribute

    Private val As String
    Private nam As String

    Public Sub New(name As String)
        Me.val = Value
    End Sub

    Public ReadOnly Property Value As Object Implements NxAttribute.Value
        Get
            Return val
        End Get
    End Property

    Public ReadOnly Property IsNull As Boolean Implements NxAttribute.IsNull
        Get
            Return val = Nothing
        End Get
    End Property

    Public ReadOnly Property Name As String Implements NxAttribute.Name
        Get
            Return nam
        End Get
    End Property

    Public Sub Write(obj As Object) Implements NxAttribute.Write
        ' Write to Database
    End Sub
End Class
