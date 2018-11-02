Imports System.IO

Public Class frmMain
    Private strFileName As String
    Private WithEvents Welcome As subfrmWelcome

    Private arrSubForms As ArrayList

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        arrSubForms = New ArrayList
        Welcome = New subfrmWelcome

        'add all subforms to the ArrayList
        arrSubForms.Add(Welcome)

        'add each subform to the work area GroupBox
        grpWorkArea.Controls.Add(Welcome)

        Welcome.Location = CalcLocation(grpWorkArea, Welcome)
        Welcome.Visible = True
    End Sub

    Private Sub HideAllSubforms()
        For Each obj As UserControl In arrSubForms
            obj.Visible = False
        Next
    End Sub

    Private Function CalcLocation(grpBox As GroupBox, subForm As UserControl) As Point
        Return New Point((grpBox.Width - subForm.Width) / 2, (grpBox.Height - subForm.Height) / 2)
    End Function

    Private Sub OpenFile(strType As String)
        Dim intResult As Integer
        ofdData.InitialDirectory = Application.StartupPath
        ofdData.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt"
        ofdData.FilterIndex = 2
        Select Case strType
            Case "CustomerData"
                ofdData.Title = "Select Customer Data File"
            Case "OrderData"
                ofdData.Title = "Select Order Data File"
            Case Else
                MessageBox.Show("Unexpected data type in OpenFile", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Select
        intResult = ofdData.ShowDialog
        If intResult = DialogResult.Cancel Then
            Exit Sub
        End If
        strFileName = ofdData.FileName
        Try
            ReadInputFile(strFileName, strType)
        Catch ex As FileNotFoundException
            MessageBox.Show("FileNotFound exception at ReadInputFile in OpenFile", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Catch ex As IndexOutOfRangeException
            MessageBox.Show("Selected file incompatible or corrupt", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Catch ex As IOException
            MessageBox.Show("IOException at ReadInputFile in OpenFile", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Catch ex As Exception
            MessageBox.Show("Unhandled exception at ReadInputFile in OpenFile", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub

        End Try
    End Sub

    Private Sub ReadInputFile(strFileIn As String, strType As String)
        Dim fileIn As StreamReader
        Dim strLineIn As String
        Dim strFields() As String
        Dim i As Integer
        fileIn = New StreamReader(strFileIn)
        fileIn.ReadLine() 'throw away the first record in the file
        Select Case strType

            Case "CustomerData"
                tvwCust.Nodes.Clear()
                'build letter top nodes
                For i = 65 To 90 'ASCII codes for uppercase A-Z
                    tvwCust.Nodes.Add(Chr(i), Chr(i)) 'add node with key value and text value
                Next
                While Not fileIn.EndOfStream
                    strLineIn = fileIn.ReadLine()
                    strFields = strLineIn.Split(","c)
                    'Dim newNode As New TreeNode
                    'newNode.Text = strFields(1) & ", " & strFields(2)
                    'newNode.Tag = strFields(0) 'hide the ID lookup value
                    'tvwCust.Nodes.Add(newNode)
                    Dim parentNode() As TreeNode
                    parentNode = tvwCust.Nodes.Find(strFields(1).Substring(0, 1), True) 'find the correct parent node
                    parentNode(0).Nodes.Add(strFields(0), strFields(1) & ", " & strFields(2))
                End While
                tvwCust.ExpandAll()
                tvwCust.Refresh()
            Case "OrderData"


            Case Else
                MessageBox.Show("Unhandled exception in ReadInputFile", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Select

    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        OpenFile("CustomerData")
    End Sub
End Class
