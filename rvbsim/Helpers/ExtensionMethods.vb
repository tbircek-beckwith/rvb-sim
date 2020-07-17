﻿
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading

''' <summary>
''' Extension methods
''' </summary>
Public Module ExtensionMethods

    ''' <summary>
    ''' Gets Child controls of the specified container control.
    ''' </summary>
    ''' <typeparam name="T">Container control to get child controls</typeparam>
    ''' <param name="parent">Container</param>
    ''' <returns>Returns a List of Child controls of the specified container control.</returns>
    <Extension()>
    Public Function GetChildControls(Of T As Control)(ByVal parent As Control) As List(Of T)
        Dim result As New List(Of Control)

        For Each control As Control In parent.Controls
            If TypeOf control Is T Then result.Add(control)
            result.AddRange(control.GetChildControls(Of T)())
        Next
        Return result.ToArray().Select(Of T)(Function(arg1) CType(arg1, T)).ToList()
    End Function

    ''' <summary>
    ''' Gets all controls in the form.
    ''' </summary>
    ''' <returns>Returns all controls in the form.</returns>
    <Obsolete("not in use. will remove next iteration.", True)>
    Friend Function GetControls() As List(Of Control)

        ' returns every control
        Dim controls = RVBSim.CommunicationDetails.GetChildControls(Of Control)

        For Each control As Control In controls
            Debug.WriteLine($"control.name ={control.Name}, control.text = {control.Text}, control.type = {control.GetType()}")
        Next

        Return controls
    End Function

    ''' <summary>
    ''' Gets the specified <see cref="Control"/>
    ''' </summary>
    ''' <param name="protocol">the name protocol in use. <see cref="CommunicationBaseModel(Of T).Name"/></param>
    ''' <param name="settingName">the name of the setting value to use. ex: <see cref="DnpCommunicationModel.LocalVoltage"/></param>
    ''' <param name="regulatorNumber">the regulator <see cref="CommunicationBaseModel(Of T).Id"/></param>
    ''' <param name="searchAllChildren">whether search children <see cref="Control"/> or not</param>
    ''' <returns>returns the specified controls</returns>
    ''' <example>
    ''' <code>
    ''' control = GetControls(protocol:=modbusRegister.Name, settingName:=NameOf(modbusRegister.RVBMax), regulatorNumber:=modbusRegister.Id, searchAllChildren:=True)(0)
    ''' modbusWrite.WriteSingleRegister(CType(control, NumericUpDown).Value, .RVBMax.Value * M2001D_Comm_Scale)
    ''' </code>
    ''' </example>
    <Obsolete("not in use. will remove next iteration.", True)>
    <Extension()>
    Public Function GetControls(ByVal protocol As String, ByVal settingName As String, ByVal regulatorNumber As Integer, Optional ByVal searchAllChildren As Boolean = True) As Control()
        Dim t() As Control = RVBSim.Controls.Find($"{protocol}{settingName}Reg{regulatorNumber}", searchAllChildren)
        Return t
    End Function

    ''' <summary>
    ''' Sets values of controls and enable/disable
    ''' </summary>
    ''' <param name="enable">True sets the control enabled, false disables the control</param>
    <Extension()>
    Public Sub SetValuesFromJson(ByRef enable As Boolean)

        Try

            ' retrieve test settings per selected protocol
            testJsonSettings = jsonRead.GetSettings(Of JsonRoot)(Path.Combine(path1:=BaseJsonSettingsFileLocation, path2:=$"{SettingFileName}-{ProtocolInUse}.json"))
            testJsonSettingsRegulators = testJsonSettings.Test

            ' populate communication data
            RVBSim.ReadIpAddr.Text = baseJsonSettings.Read
            RVBSim.WriteIpAddr.Text = baseJsonSettings.Write
            RVBSim.PortReg1.Text = testJsonSettings.Port

            ' populate test settings
            PopulateControls(testJsonSettingsRegulators, True)

        Catch ex As Exception
            Dim trace = New StackTrace(ex, True)
            Dim message As String = $"{Now}:{vbCrLf}Line #: {trace.GetFrame(0).GetFileLineNumber().ToString()}{vbCrLf}{ex.StackTrace}:{vbCrLf}{ex.Message}"
            SetText(RVBSim.lblMsgCenter, message)
            sb.AppendLine(message)

        Finally
            Debug.WriteLine($"Current thread is # {Thread.CurrentThread.GetHashCode} {NameOf(SetValuesFromJson)}")
        End Try

    End Sub

    ''' <summary>
    ''' Populates Controls per selected protocol on boot up.
    ''' </summary>
    ''' <param name="regulators"></param>
    ''' <param name="enable"></param>
    Friend Sub PopulateControls(regulators As JsonTest, enable As Boolean)

        Try

            ' scan the values
            For Each model In regulators.Regulator

                For Each regulator In model.Values  '.Regulator


                    ' stitch the control name
                    Dim controlName As String = String.Empty

                    ' if "NO" Protocol specified use settings.json file value
                    If String.IsNullOrWhiteSpace(ProtocolInUse) Then

                        ' update communication setting
                        controlName = $"Settings{regulator.Name}Reg{model.Id}"

                    Else

                        controlName = $"{ProtocolInUse}{regulator.Name}Reg{model.Id}"
                    End If



                    ' find the control name
                    Dim t() As Control = RVBSim.Controls.Find(controlName, True)
                    If t.Length > 0 Then

                        ' assigned values to the control
                        Dim textValue As String = regulator.Value
                        If String.Equals(ProtocolInUse, "iec") Then
                            textValue = $"{regulator.Value}${regulator.Fc}${regulator.Sdi}${regulator.Dai}"
                        End If

                        Select Case t(0).GetType()
                            Case GetType(RadioButton)
                                Dim useRelative As RadioButton = CType(t(0), RadioButton)

                                Dim isUseRelative As Boolean = regulator.Value

                                Dim alternateName As String = $"SettingsUsefixedReg{model.Id}"
                                Dim useFixed As RadioButton = CType(RVBSim.Controls.Find(alternateName, True)(0), RadioButton)

                                If isUseRelative Then
                                    useRelative.PerformClick()
                                Else
                                    useFixed.PerformClick()
                                End If

                                Debug.WriteLine($"Control name: {controlName}, value: {regulator.Value}")

                            Case Else
                                t(0).Text = textValue
                        End Select

                        t(0).Enabled = enable
                    End If

                    Debug.WriteLine($"Control name: {controlName}, value: {regulator.Value}")

                Next

                '  Debug.WriteLine("new extension")
            Next
        Catch ex As Exception
            Dim message As String = $"{Now}: ({NameOf(Populatetheform)}) {vbCrLf}{ex.StackTrace}:{vbCrLf}{ex.Message}"
            SetText(RVBSim.lblMsgCenter, message)
            sb.AppendLine(message)
        End Try

    End Sub
End Module
