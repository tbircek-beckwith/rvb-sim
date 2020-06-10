﻿Imports System.Threading

Module DisEnableCheckBoxes

    ''' <summary>
    ''' Enables or disables controls per Start button
    ''' </summary>
    Friend Sub Disenable()

        Try
            With RVBSim.StartButton
                Debug.WriteLine($"Current thread is # {Thread.CurrentThread.GetHashCode} --- {NameOf(Disenable)}")

                ' Set values and dis/enable of controls in Regulator List
                SetValues(.Enabled)

                ' Set Protocol options
                SetEnable(RVBSim.ProtocolBox, .Enabled)

                'communication settings dis/enable
                ' except Start and Stop buttons
                SetEnable(RVBSim.WriteIpAddr, .Enabled)
                SetEnable(RVBSim.ReadIpAddr, .Enabled)
                SetEnable(RVBSim.PortReg1, .Enabled)

                'general rvb settings dis/enable
                SetEnable(RVBSim.RVBSettings, .Enabled)

            End With

        Catch ex As Exception
            SetText(RVBSim.lblMsgCenter, ex.Message)
            sb.AppendLine($"{Now} {ex.Message}")
        End Try
    End Sub

End Module
