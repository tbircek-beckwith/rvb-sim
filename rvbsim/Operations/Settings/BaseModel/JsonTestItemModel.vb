﻿Imports Newtonsoft.Json

Public Class JsonTestItemModel

    ' JsonProperty names need to match the .json file and are case insensitive

    <JsonProperty("Description")>
    Public Property Description() As String

    <JsonProperty("Range")>
    Public Property Range() As List(Of JsonRangeModel)

    <JsonProperty("Value")>
    Public Property Value() As Integer

    <JsonProperty("Unit")>
    Public Property Unit() As String


End Class