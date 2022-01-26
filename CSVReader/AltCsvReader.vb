Imports System.IO

Public Enum ReaderTokenType
    Delimiter
    Data
    EoF ' End of File
    EoR ' End of Record
End Enum

Public Structure ReaderToken
    Public Type As ReaderTokenType
    Public Data As String
End Structure

Public Class AltCsvReader
    Private stream As Stream

    Private Const DELIMITER As ReaderTokenType = ReaderTokenType.Delimiter
    Private Const DATA As ReaderTokenType = ReaderTokenType.Data
    Private Const EOF As ReaderTokenType = ReaderTokenType.EoF
    Private Const EOR As ReaderTokenType = ReaderTokenType.EoR

    Public Sub New(stream As Stream)
        Me.stream = stream

        nextChar() ' Start initial character read
    End Sub

    Dim currentChar As Integer

    Private Sub nextChar()
        currentChar = stream.ReadByte
    End Sub

    Public CurrentToken As ReaderToken = New ReaderToken
    Public LastToken As ReaderToken = New ReaderToken

    Private readToken As Boolean = True

    Private Sub setToken(token As ReaderTokenType, Optional data As String = "")
        LastToken = CurrentToken

        CurrentToken.Type = token
        CurrentToken.Data = data

        readToken = False
    End Sub

    Public Sub NextToken()
        readToken = True

        While readToken
            If currentChar = 44 Then ' [,] Delimiter
                setToken(DELIMITER)
            ElseIf currentChar = 34 Then ' ["] Enclosed Data
                nextChar() ' Skip opening speech marks

                Dim dat As String = ""
                While Not currentChar = 34
                    dat += Chr(currentChar)
                    nextChar()
                End While

                setToken(DATA, dat)
            ElseIf currentChar = 13 Then ' Carriage Return
                nextChar()

                If currentChar = 10 Then
                    nextChar()
                End If

                setToken(EOR)
            ElseIf currentChar = 10 Then ' Line Feed
                setToken(EOR)
            ElseIf currentChar = -1 Then ' End of File
                setToken(EOF)
            Else ' General Data
                Dim dat As String = ""

                While Not currentChar = 44
                    dat += Chr(currentChar)
                    nextChar()
                End While

                setToken(DATA, dat)
                Exit While
            End If

            nextChar()
        End While
    End Sub
End Class
