Option Explicit On
Option Strict On

Imports System.Xml.Serialization


<Serializable>
<XmlRoot("PositionMemorySettings")>
Public Class PositionMemorySettings
    Inherits XMLObject

    Public XYZPositionSets As New List(Of XYZPositionSet)

End Class


<Serializable>
<XmlRoot("XYZPositionSet")>
Public Class XYZPositionSet
    Public Index As Integer
    Public Sample As New XYZPosition
    Public Optics As New XYZPosition
    Public Head As New XYZPosition

End Class


<Serializable>
<XmlRoot("XYZPosition")>
Public Class XYZPosition
    Public X As Decimal = -1
    Public Y As Decimal = -1
    Public Z As Decimal = -1
End Class