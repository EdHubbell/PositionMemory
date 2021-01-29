Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
'Imports NLog


Public Class XMLObject

    Public Shared Function Load(Of t)(ByVal filePath As String) As t
        Dim oDeserializedObject As t = Nothing

        Try
            Dim serializer As XmlSerializer = New XmlSerializer(GetType(t))

            Dim reader As StreamReader = New StreamReader(filePath)
            oDeserializedObject = CType(serializer.Deserialize(reader), t)
            reader.Close()
        Catch ex As Exception
            Return Nothing
        Finally
        End Try

        Return oDeserializedObject
    End Function


    Function AsXMLString() As String
        Dim xmlSerializer As XmlSerializer = New XmlSerializer(Me.[GetType]())

        Using stream = New StringWriter()
            Using writer = XmlWriter.Create(stream, GetXmlWriterSettings())
                xmlSerializer.Serialize(writer, Me, GetBlankXmlNamespaces())
                Return stream.ToString()
            End Using
        End Using
    End Function

    Public Sub SaveToXMLFile(ByVal sFileFullPath As String)
        Try

            Using file As New System.IO.StreamWriter(sFileFullPath)
                file.Write(Me.AsXMLString)
                file.Close()
            End Using

        Catch ex As Exception
            'WriteEventLog("ERROR: Exception in SaveToXMLFile: ", ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub

    Function GetXmlWriterSettings() As XmlWriterSettings
        Dim oXMLWriterSettings As XmlWriterSettings = New XmlWriterSettings()
        Dim _with1 = oXMLWriterSettings
        _with1.Indent = True
        _with1.OmitXmlDeclaration = True
        _with1.Encoding = System.Text.Encoding.UTF8
        Return oXMLWriterSettings
    End Function

    Function GetBlankXmlNamespaces() As XmlSerializerNamespaces
        Dim blankXmlNamespaces As XmlSerializerNamespaces = New XmlSerializerNamespaces()
        blankXmlNamespaces.Add(String.Empty, String.Empty)
        Return blankXmlNamespaces
    End Function

End Class

