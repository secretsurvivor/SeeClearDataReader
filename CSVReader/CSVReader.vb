Imports System.IO

Public Enum CSVReaderToken
    Delimiter
    Data
    Newline
    EoF
End Enum

Public Class CSVReader
    Implements IDisposable
    Private reader As FileStream

    Public Sub New(path As String)
        Me.reader = File.OpenRead(path)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        reader.Dispose()
    End Sub

    Private Const DELIMITER As CSVReaderToken = CSVReaderToken.Delimiter
    Private Const DATA As CSVReaderToken = CSVReaderToken.Data
    Private Const NEWLINE As CSVReaderToken = CSVReaderToken.Newline
    Private Const EOF As CSVReaderToken = CSVReaderToken.EoF

    Private lastChar As Integer
    Private currentChar As Integer

    Private Sub readNext()
        lastChar = currentChar
        currentChar = reader.ReadByte()
    End Sub

    Private Function compileChars(delimiter As Integer) As String
        Dim compiledString As String

        While True
            If currentChar = delimiter Then
                Return compiledString
            Else
                compiledString = compiledString & Chr(currentChar)
            End If

            readNext()
        End While
    End Function

    Public LastToken As CSVReaderToken ' Last Token Type
    Public CurrentToken As CSVReaderToken ' Token Type
    Public CurrentData As String ' Token Information

    Private Function setToken(t As CSVReaderToken, Optional d As String = "") As CSVReaderToken
        lastToken = currentToken

        If t = CSVReaderToken.Data Then
            currentData = d
        End If

        currentToken = t

        Return t
    End Function

    Public Function NextToken() As CSVReaderToken
        Dim active As Boolean = True
        Dim newToken As CSVReaderToken
        Dim newData As String = ""

        While active
            If currentChar = 44 Then ' [,] Delimiter
                newToken = DELIMITER
            ElseIf currentChar = 34 Then ' ["] Enclosed Data
                readNext() ' Skip quote
                newToken = DATA
                newData = compileChars(34)
            ElseIf currentChar = 13 Then ' Carriage Return
                readNext()
                If currentChar = 10 Then ' Support Macintosh Newline
                    newToken = NEWLINE
                Else ' Support Windows Newline
                    Return setToken(NEWLINE)
                End If
            ElseIf currentChar = 10 Then ' Support Unix Newline
                newToken = NEWLINE
            ElseIf currentChar = -1 Then ' EOF
                Return setToken(EOF)
            Else ' Data
                Dim compiledString = compileChars(44)

                Return setToken(DATA, compiledString)
            End If

            readNext()
        End While

        Return setToken(newToken, newData)
    End Function


End Class
