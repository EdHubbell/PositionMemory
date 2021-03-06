﻿Imports System.IO

Public Class Form1
    Private EditMode As Boolean = False
    Private PosMemoryPath As String = "PosMemory.txt"
    Private PosMemorySettingsPath As String = "PosMemorySettings.xml"
    Private PosMemory(9, 9) As Double
    Private PosMemoryBtns As New List(Of Button)
    Private PosTextBoxes As New List(Of TextBox)

    Private PosMemorySettings As PositionMemorySettings

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializePosMemoryUI()

        If File.Exists(PosMemoryPath) Then
            LoadPosMemory()
        Else
            SavePosMemory(create:=True)
        End If

        If File.Exists(PosMemorySettingsPath) Then
            PosMemorySettings = PositionMemorySettings.Load(Of PositionMemorySettings)(PosMemorySettingsPath)
        Else
            PosMemorySettings = New PositionMemorySettings
            PosMemorySettings.SaveToXMLFile(PosMemorySettingsPath)
        End If

    End Sub

    Private Sub InitializePosMemoryUI()
        For Each btn As Button In TableLayoutPanel1.Controls.OfType(Of Button)
            If btn.AccessibleDescription = 1 Then ' The memory buttons have a tag
                PosMemoryBtns.Add(btn)
                AddHandler btn.MouseUp, AddressOf ClickButton
            End If
        Next
        For Each txt As TextBox In TableLayoutPanel1.Controls.OfType(Of TextBox)
            PosTextBoxes.Add(txt)
            AddHandler txt.MouseUp, AddressOf ClickTextBox
        Next
    End Sub

    Private Sub LoadPosMemory()
        Dim fileReader = My.Computer.FileSystem.ReadAllText(PosMemoryPath)
        Dim data = fileReader.Split(vbCrLf)
        For i As Integer = 0 To 8
            Dim vals = data(i).Split(",")
            Dim TestPosSum As Double = 0 ' Will be -9 if all are -1 (No pos saved)
            For j As Integer = 0 To 8
                PosMemory(i, j) = vals(j)
                TestPosSum += vals(j)
            Next
            If TestPosSum <> -9 Then PosMemoryBtns(i).FlatAppearance.BorderColor = Color.LawnGreen
        Next
    End Sub

    Private Sub SavePosMemory(Optional create As Boolean = False)
        FileOpen(1, PosMemoryPath, OpenMode.Output, OpenAccess.Write)
        For i As Integer = 0 To 8
            For j As Integer = 0 To 8
                If create Then PosMemory(i, j) = -1 ' Set to default -1
                Write(1, PosMemory(i, j))
            Next
            WriteLine(1)
        Next
        FileClose(1)
    End Sub


    Private Sub ClickButton(sender As Button, e As MouseEventArgs)
        'Dim MemoryIdx As Integer = CInt(sender.Name.Replace("btn", "")) - 1 ' Get array idx from btn name

        ' A trick I've used here - You could also do this with a tag value on the control, like so:
        ' This is more for future reference when you need to tie a control to some kind of in-code index. These buttons are low risk for renaming, as it's pretty much 1 thru 9. I'd keep what you have in place.  
        Dim MemoryIdx As Integer = CInt(sender.Tag) - 1 ' Get array idx from tag


        With sender.FlatAppearance ' This color defines the logic
            If .BorderColor <> Color.Gold And EditMode Then Exit Sub ' Cannot be editing more than one pos at once
            Select Case e.Button
                Case MouseButtons.Right
                    Select Case .BorderColor
                        Case SystemColors.ControlDark ' Begin edit
                            .BorderColor = Color.Gold
                            EditMode = True
                        Case Color.Gold ' Finish edit (Store and save values)
                            For Each txt As TextBox In PosTextBoxes
                                If txt.BackColor = Color.Gold Then
                                    PosMemory(MemoryIdx, txt.AccessibleDescription) = CDbl(txt.Text)

                                End If
                                txt.BackColor = SystemColors.Control
                            Next
                            SavePosMemory()
                            PosMemorySettings.SaveToXMLFile(PosMemorySettingsPath)
                            .BorderColor = Color.LawnGreen
                            EditMode = False
                    End Select
                Case MouseButtons.Middle ' Clear memory pos values and save
                    Dim confirm = MsgBox("Confirm Clear Memory Pos " & (MemoryIdx + 1).ToString(), vbYesNo, "Clear Memory Position")
                    If confirm <> vbYes Then Exit Sub
                    For i As Integer = 0 To 8
                        PosMemory(MemoryIdx, i) = -1
                    Next
                    .BorderColor = SystemColors.ControlDark
                    SavePosMemory()
                    PosMemorySettings.SaveToXMLFile(PosMemorySettingsPath)
                Case MouseButtons.Left
                    If .BorderColor <> Color.LawnGreen Then Exit Sub ' Do nothing if there is no saved pos
                    Dim confirm = MsgBox("Confirm Move to Memory Pos " & (MemoryIdx + 1).ToString(), vbYesNo, "Goto Memory Position")
                    If confirm <> vbYes Then Exit Sub
                    For Each txt As TextBox In PosTextBoxes
                        Dim GotoPos = PosMemory(MemoryIdx, txt.AccessibleDescription)
                        If GotoPos <> -1 Then txt.Text = GotoPos
                    Next
            End Select
        End With
    End Sub

    Private Sub ClickTextBox(sender As TextBox, e As MouseEventArgs)
        If EditMode Then sender.BackColor = Color.Gold ' Highlight pos if selected to be saved to memory
    End Sub

    Private Sub btnCenterSample_Click(sender As Object, e As EventArgs) Handles btnCenterSample.Click
        txtX.Text = "250"
        txtY.Text = "250"
    End Sub

    Private Sub btnCenterOptics_Click(sender As Object, e As EventArgs) Handles btnCenterOptics.Click
        txtXo.Text = "0"
        txtYo.Text = "0"
    End Sub

    Private Sub btnGotoClearance_Click(sender As Object, e As EventArgs) Handles btnGotoClearance.Click
        txtZ.Text = "-18"
    End Sub

    Private Sub btnZeroTheta_Click(sender As Object, e As EventArgs) Handles btnZeroTheta.Click
        txtTx.Text = "0"
        txtTy.Text = "0"
        txtTz.Text = "0"
    End Sub

    Private Sub btnHomeAll_Click(sender As Object, e As EventArgs) Handles btnHomeAll.Click
        For Each txt As TextBox In PosTextBoxes
            txt.Text = "0"
        Next
    End Sub

    Private Sub btnGotoRandom_Click(sender As Object, e As EventArgs) Handles btnGotoRandom.Click
        For Each txt As TextBox In PosTextBoxes
            txt.Text = Math.Round(500 * Rnd(), 3)
        Next
    End Sub
End Class