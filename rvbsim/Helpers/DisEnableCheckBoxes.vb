﻿Imports System.Threading

Module DisEnableCheckBoxes

    ''' <summary>
    ''' Enables or disables controls per Start button enable state.
    ''' thread safe
    ''' </summary>
    Friend Sub Disenable()

        Try
            With RVBSim.StartButton
                Debug.WriteLine($"Current thread is # {Thread.CurrentThread.GetHashCode} --- {NameOf(Disenable)}")

                ' Set dis/enable of controls in Communication Details
                SetEnable(RVBSim.CommunicationDetails, .Enabled)

                ' Set Protocol options
                SetEnable(RVBSim.ProtocolBox, .Enabled)

                'communication settings dis/enable
                ' except Start and Stop buttons
                SetEnable(RVBSim.WriteIpAddr, .Enabled)
                SetEnable(RVBSim.ReadIpAddr, .Enabled)
                SetEnable(RVBSim.PortReg1, .Enabled)

                'general rvb settings dis/enable
                SetEnable(RVBSim.RVBSettings, .Enabled)

                ' single phase
                SetEnable(RVBSim.SinglePhaseCheckBox, .Enabled)

            End With

        Catch ex As Exception
            Dim message As String = $"{Now}{vbCrLf}{ex.StackTrace}:{vbCrLf}{ex.Message}"
            ' SetText(RVBSim.lblMsgCenter, message)
            SetTextBox(textbox:=RVBSim.ErrorsTextBox, text:=message)
            sb.AppendLine(message)

        Finally
            Debug.WriteLine($"Current thread is # {Thread.CurrentThread.GetHashCode} --- {NameOf(Disenable)} -- ENDS")

        End Try
    End Sub

End Module
