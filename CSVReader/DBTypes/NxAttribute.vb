Public Interface NxAttribute
    ReadOnly Property Value
    ReadOnly Property IsNull As Boolean
    ReadOnly Property Name As String
    Sub Write(obj As Object)
End Interface
