Public Class SeeClearCSV
    Private reader As CSVReader
    Private identifierArray As Dictionary(Of String, String)
    Private build As Dictionary(Of Integer, List(Of NxAttribute))
    Private identityBuild As Dictionary(Of String, Dictionary(Of Integer, List(Of NxAttribute)))
    Private useIdentifiers As Boolean

    Sub New(fileLoadName As String, path As String) ' Without Identifiers
        reader = New CSVReader(path)
        useIdentifiers = False
        build = FileLoadBuilder.BuildFileLoad(fileLoadName)
    End Sub

    Sub New(fileLoadName As String, path As String, identifiers As Dictionary(Of String, String)) ' With Identitifers
        reader = New CSVReader(path)
        useIdentifiers = True
        identifierArray = identifiers
        identityBuild = FileLoadBuilder.BuildFileLoadIdentity(fileLoadName)
    End Sub

    Private Const DELIMITER As CSVReaderToken = CSVReaderToken.Delimiter
    Private Const DATA As CSVReaderToken = CSVReaderToken.Data
    Private Const NEWLINE As CSVReaderToken = CSVReaderToken.Newline
    Private Const EOF As CSVReaderToken = CSVReaderToken.EoF

    Public Sub Execute()
        reader.NextToken() ' Get first token

        While Not reader.CurrentToken = EOF ' Will run through the whole file
            ' --- Pre Record ---
            Dim columnNum As Integer = 0
            Dim currentIdentifier As String ' Would only be called if Identity is active
            Dim invalidIdentifier As Boolean = False

            If useIdentifiers Then
                If reader.CurrentToken = DATA Then
                    If identifierArray.ContainsKey(reader.CurrentData) Then
                        currentIdentifier = identifierArray.Item(reader.CurrentData)
                        reader.NextToken()
                        build = identityBuild.Item(currentIdentifier)
                    Else
                        invalidIdentifier = True ' Skip remaining record and log
                    End If
                End If
            End If

            While Not reader.CurrentToken = NEWLINE Or Not reader.CurrentToken = EOF ' Will run through each record
                If reader.CurrentToken = DATA Or (reader.CurrentToken = DELIMITER And reader.LastToken = DELIMITER) Then
                    columnNum += 1
                End If

                If (reader.CurrentToken = DATA And build.ContainsKey(columnNum)) And Not (useIdentifiers And invalidIdentifier) Then
                    For Each attr As NxAttribute In build.Item(columnNum)
                        attr.Write(reader.CurrentData)
                    Next
                End If

                reader.NextToken()
            End While

            reader.NextToken()
        End While
    End Sub

End Class
