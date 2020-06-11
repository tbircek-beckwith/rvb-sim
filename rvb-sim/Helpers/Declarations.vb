﻿Imports System.Text
Imports System.Threading
'Imports Automatak.DNP3.Interface
Imports RVBSim.Communication.Operations
Imports RVBSim.PeriodicOperations

Module Declarations

    ' Friend Const ConsoleWriteEnable As Boolean = True       'False          ' 
    Friend Const SupportedRVBRevision As String = "15"      'Supported RVB feature document revision
    Friend Const OperatingVoltage As Integer = 900
    Friend Const BecoCommunicationScaleFactor As Integer = 10   ' all products
    Friend Const MaxDeltaVoltage As Integer = 100
    Friend Const MinDeltaVoltage As Integer = -100
    Friend Const DeltaMessage As String = "Local Voltage + "
    Friend Const DirectMessage As String = "RVB Voltage is ="
    Friend Const DNP_BufferSize As Integer = 29
    ' Friend Const Modbus_BufferSize As Integer = 12
    Friend Const IEC_BufferSize As Integer = 200

    Friend sb As New StringBuilder
    Friend IPs As String() = New String(1) {}

    Friend Regulators As List(Of Regulator) = New List(Of Regulator)()

    ''' <summary>
    ''' new modbus communication libraries
    ''' </summary>
    Friend modbusRead As EasyModbus.ModbusClient
    Friend modbusWrite As EasyModbus.ModbusClient

    '''' <summary>
    '''' new dnp3.0 communication library
    '''' </summary>
    ' Friend dnpReadManager As IDNP3Manager
    'Friend dnpPollingData As IEnumerable(Of IndexedValue(Of Analog))

    Friend start As New StartOperation
    Friend pause As New PauseOperation

    Friend periodicRead As New ReadEvents
    Friend periodicWrite As New WriteEvents
    Friend periodicReset As New ResetEvents

    Friend dnp As tcpdnp.AsyncDNP3_0
    'Friend modbus As tcpmodbus.AsyncModbus
    Friend iec61850 As iec.AsyncIEC61850

    Friend processID As Integer = 0
    Friend support As Boolean      'True rev 15 or greater False rev 8
    Friend errorCounter As Integer = 0

    Friend WriteTickerDone As New ManualResetEvent(False)
    Friend ReadTickerDone As New ManualResetEvent(False)
    Friend TimersEvent As New ManualResetEvent(False)

    Friend WriteRegisterWait As RegisteredWaitHandle
    Friend ReadRegisterWait As RegisteredWaitHandle

    Friend visibility As Boolean = True
    Friend testSetting As TestSettings
    'Friend modbusRegister As ModbusSettings
    Friend dnpSetting As DnpSettings
    Friend iecSetting As IECSettings

    Friend Structure TestSettings
        Dim Protocol As String
        Dim readIpAddress As String
        Dim writeIpAddress As String
        Dim HeartbeatTimer As UShort
        Dim FwdRVBVoltage As Double
        Dim RevRVBVoltage As Double
        Dim FwdRVBVoltageScale As Double
        Dim RevRVBVoltageScale As Double
        Dim RVBMax As Double
        Dim RVBMin As Double
    End Structure

    'Friend Structure ModbusSettings
    '    ' Dim Port As String
    '    Dim LocalVoltage As UShort
    '    Dim RVBEnable As UShort
    '    Dim FRVBValue As UShort
    '    Dim FRVBScale As UShort
    '    Dim RVBHeartBeatTimer As UShort
    '    Dim RVBActive As UShort
    '    Dim RRVBValue As UShort
    '    Dim RRVBScale As UShort
    '    Dim RVBMax As UShort
    '    Dim RVBMin As UShort
    '    Dim Factory As UShort
    'End Structure

    Friend Structure DnpSettings
        ' Dim Port As String
        Dim source As UShort
        Dim destination As UShort
        'just get value of point, DNP only cares about the point values
        Dim LocalVoltage As UShort
        Dim RVBEnable As UShort
        Dim FRVBValue As UShort
        Dim FRVBScale As UShort
        Dim RVBHeartBeatTimer As UShort
        Dim RRVBValue As UShort
        Dim RRVBScale As UShort
        Dim RVBMax As UShort
        Dim RVBMin As UShort
    End Structure

    Friend Structure IECSettings
        ' Dim Port As String
        Dim ReadIEDName As String
        Dim WriteIEDName As String
        Dim iedClass As String
        Dim LocalVoltage As String
        Dim RVBEnable As String
        Dim FRVBValue As String
        Dim FRVBScale As String
        Dim RVBHeartBeatTimer As String
        Dim RRVBValue As String
        Dim RRVBScale As String
        Dim RVBMax As String
        Dim RVBMin As String
    End Structure

    ''' <summary>
    ''' Holds supported dnp3.0 data variations
    ''' </summary>
    Friend Enum Variations As Byte
        AnaOutBlockShort = 2
        AnaInput16bitVar4 = 4
    End Enum

    ''' <summary>
    ''' Holds supported dnp3.0 qualifier fields
    ''' </summary>
    Friend Enum QualifierField As Byte
        AnaInput8bitStartStop = 0
        AnaInput16bitStartStop = 1
        AnaOutBlock8bitIndex = 23
        AnaOutBlock16bitIndex = 40
    End Enum

    ''' <summary>
    ''' Holds supported dnp3.0 objects types
    ''' </summary>
    Friend Enum Objects As Byte
        AnalogInput = 30
        AnalogOutputStatus = 40
        AnalogOutput = 41
    End Enum

    Friend Property WriteInterval() As Integer

    Friend Property ReadInterval() As Integer

    Friend Property ProtocolInUse() As String

    Friend Property Heart_Beat_Timer() As Integer

    Friend Property ActualLocalVoltage() As Double = 0.0

    Friend Property ActualSourceVoltage() As Double = 0.0

    Friend Property Forward_RVBVoltage2Write() As Double = 0.0

    Friend Property Reverse_RVBVoltage2Write() As Double = 0.0

    Friend Property LocalVoltageReadresult() As UShort = 0

    Friend Property SourceVoltageReadresult() As UShort = 0

    Friend Property ReceivedErrorMsg() As String = String.Empty

End Module
