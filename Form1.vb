Imports System.Collections.ObjectModel
Imports System.Data.SQLite
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Xml
Imports Libronix.DigitalLibrary
Imports Libronix.DigitalLibrary.Resources.BibleKnowledgebase
Imports Libronix.DigitalLibrary.Resources.Core
Imports Libronix.DigitalLibrary.Resources.Logos
Imports Libronix.DigitalLibrary.RichText
Imports Libronix.DigitalLibrary.Utility
Imports Libronix.RichText
Imports Libronix.Utility
Imports Libronix.Utility.Data
Imports Libronix.Utility.Threading

Public Class Form1

    Dim dls As Libronix.DigitalLibrary.Utility.DigitalLibraryServices
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)
        ComboBox1.SelectedIndex = 0
        TextBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\LogosResources"
        Try
            LogosResourceDriver.Initialize()
            Dim um As New UserManager(Path.Combine(Directory.GetCurrentDirectory(), "..\Users"))
            Dim u As User = um.GetLastUser()
            dls = DigitalLibraryServices.CreateForUser(u, New DigitalLibraryServicesSettings())

            For Each x As String In dls.LibraryCatalog.GetAvailableResourceTypes()
                ListBox1.Items.Add(x)
            Next
        Catch ex As Exception
            MsgBox(ex.ToString(), MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, "Loading failed")
            ListBox1.Items.Clear()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FolderBrowserDialog1.SelectedPath = TextBox1.Text
        FolderBrowserDialog1.ShowDialog(Me)
        TextBox1.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged, ComboBox1.SelectedIndexChanged
        ListBox2.Items.Clear()
        For Each x As String In ListBox1.SelectedItems
            For Each y As ResourceInfo In dls.LibraryCatalog.GetResourcesByResourceType(x, ResourceFieldSet.All, LibraryCatalogRecordOptions.All)
                Dim available As Boolean = (y.Availability = ResourceAvailability.Available)
                If (ComboBox1.SelectedIndex = 0 AndAlso available) OrElse (ComboBox1.SelectedIndex = 1 AndAlso Not available) OrElse ComboBox1.SelectedIndex = 2 Then
                    ListBox2.Items.Add("[" & y.ResourceId & "] " & y.Title & " {" & y.Availability.ToString() & If(y.IsDataset, ", Dataset", "") & "}")
                End If
            Next
        Next
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        Me.Cursor = Cursors.WaitCursor
        If ListBox2.SelectedItems.Count = 0 Then
            Label1.Text = "Select a resource to show information here"
        ElseIf ListBox2.SelectedItems.Count > 1 Then
            Label1.Text = ListBox1.SelectedItems.Count & " resources selected"
        Else
            Dim r = dls.ResourceManager.OpenResource(Regex.Replace(ListBox2.SelectedItems.Item(0).ToString, "^\[(.*?)\] .*$", "$1"))
            If r Is Nothing Then
                Label1.Text = "Unavailable resource"
            Else
                Dim dummy As Boolean
                Label1.Text = Analyze(r, dummy, Nothing)
            End If
        End If
        Me.Cursor = Cursors.Default
    End Sub

    Private Shared Function Analyze(r As Resource, ByRef hasProblems As Boolean, exportPath As String) As String
        Dim labelText As String = ""
        Try
            Dim coreType As Type = If(TypeOf r Is BibleKnowledgebaseResource, GetType(BibleKnowledgebaseResource), r.GetType)
            Dim coreField As FieldInfo = coreType.GetField("m_core", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim core As ResourceCore = If(coreField Is Nothing, Nothing, CType(coreField.GetValue(r), ResourceCore))
            labelText = "Class: " & r.GetType().FullName & If(core Is Nothing, "", ", CoreClass: " & core.GetType().FullName)
            If TypeOf r Is LogosResource Then
                Dim res As LogosResource = CType(r, LogosResource)
                labelText &= ", Logos Resource with " & res.GetArticleCount() & " articles"
                If exportPath IsNot Nothing Then
                    DumpLogosResource(res, CType(core, LogosResourceCore), exportPath)
                End If
            ElseIf core IsNot Nothing AndAlso core.GetType().IsSubclassOf(GetType(ResourceCore).Assembly.GetType("Libronix.DigitalLibrary.Resources.Core.EncryptedVolumeResourceCore")) Then
                labelText &= ", Dataset with databases:"
                Dim databases As New Dictionary(Of String, ObjectPool(Of IConnector))
                Dim t As Type = core.GetType
                While t IsNot Nothing
                    For Each fi As FieldInfo In t.GetFields(BindingFlags.NonPublic Or BindingFlags.Instance)
                        If fi.FieldType = GetType(ObjectPool(Of IConnector)) Then
                            labelText &= " " + fi.Name
                            Dim op As ObjectPool(Of IConnector) = CType(fi.GetValue(core), ObjectPool(Of IConnector))
                            If op IsNot Nothing Then
                                Dim oo As IConnector = op.BorrowObject()
                                Try
                                    Dim dbname As String = Regex.Replace(oo.Connection.ConnectionString, ".*?;(.*?\.db)"";.*", "$1")
                                    labelText &= "[" & dbname & "]"
                                    databases(dbname) = op
                                Finally
                                    op.ReturnObject(oo)
                                End Try
                            End If
                        End If
                    Next
                    t = t.BaseType
                End While
                If exportPath IsNot Nothing Then
                    DumpDatasetResource(r, core, exportPath, databases)
                End If
            Else
                labelText &= ", Unknown"
                hasProblems = True
            End If
        Catch ex As Exception
            MsgBox(ex.ToString(), MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, "Analysing failed")
            hasProblems = True
        End Try
        Return labelText
    End Function

    Shared Sub DumpLogosResource(lr As LogosResource, lrc As LogosResourceCore, exportPath As String)
        Using coverwriter As FileStream = File.Open(Path.Combine(exportPath, "cover.jpg"), FileMode.CreateNew)
            Dim cover As Byte() = CType(lr.GetType().GetMethod("GetCoverImage", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(lr, Array.Empty(Of Object)()), Byte())
            coverwriter.Write(cover, 0, cover.Length)
        End Using
        Dim mx As XmlReader = CType(lrc.GetType().GetMethod("GetLogos4MetadataXml", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(lrc, Array.Empty(Of Object)()), XmlReader)
        Using xw As XmlWriter = XmlWriter.Create(Path.Combine(exportPath, "metadata.xml"))
            xw.WriteNode(mx, True)
        End Using
        Dim dx As XmlReader = CType(lrc.GetType().GetMethod("GetResourceDocumentXml", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(lrc, Array.Empty(Of Object)()), XmlReader)
        Using xw As XmlWriter = XmlWriter.Create(Path.Combine(exportPath, "document.xml"))
            xw.WriteNode(dx, True)
        End Using
        Dim rrs As New ResourceRichTextSettings With {
            .IncludeAnchors = True,
            .IncludeFontFamily = True,
            .IncludeInputRuns = True,
            .IncludeTags = True
        }
        Dim pwss As New PausableWorkStateSource()
        Dim articleCount As Integer = lr.GetArticleCount()
        Dim i As Integer = 0
        Using listwriter As New StreamWriter(File.Open(Path.Combine(exportPath, "articlelist.csv"), FileMode.CreateNew))
            While i < articleCount
                Dim art = lr.GetArticle(i)
                If art Is Nothing Then
                    articleCount += 1
                Else
                    Dim artFile = i & "-" & art.Name & ".xml"
                    listwriter.WriteLine(i & Chr(9) & If(art.PreviousArticle Is Nothing, "", "<") & "-" & If(art.NextArticle Is Nothing, "", ">") & Chr(9) & art.Name & Chr(9) & art.Title)
                    SerializeRichText(art.RichTextTitle, Path.Combine(exportPath, "articletitle-" + artFile))
                    Dim tr = lr.DoCreateTextRangeFromArticle(i)
                    Dim rtc As ReadOnlyCollection(Of RichTextElement) = CType(tr.GetType().GetMethod("GetRichTextContentCore", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(tr, New Object() {rrs, pwss.State}), ReadOnlyCollection(Of RichTextElement))
                    SerializeRichText(rtc, Path.Combine(exportPath, "article-" + artFile))
                End If
                i += 1
            End While
        End Using
    End Sub

    Shared Sub DumpDatasetResource(lr As Resource, rc As ResourceCore, exportPath As String, databases As Dictionary(Of String, ObjectPool(Of IConnector)))
        Dim dx As XmlReader = CType(GetType(ResourceCore).Assembly.GetType("Libronix.DigitalLibrary.Resources.Core.EncryptedVolumeResourceCore").GetMethod("GetResourceDocumentXml", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(rc, Array.Empty(Of Object)()), XmlReader)
        Using xw As XmlWriter = XmlWriter.Create(Path.Combine(exportPath, "document.xml"))
            xw.WriteNode(dx, True)
        End Using
        For Each kvp As KeyValuePair(Of String, ObjectPool(Of IConnector)) In databases
            Dim ic As IConnector = kvp.Value.BorrowObject()
            Try
                Dim conn As SQLiteConnection = CType(ic.Connection.GetType().GetField("m_connection", BindingFlags.Instance Or BindingFlags.NonPublic).GetValue(ic.Connection), SQLiteConnection)
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If
                Using tgt As New SQLiteConnection("Data Source=" & exportPath & "\" & kvp.Key & ".sqlite;Version=3;")
                    tgt.Open()
                    conn.BackupDatabase(tgt, "main", "main", -1, Nothing, -1)
                End Using
            Finally
                kvp.Value.ReturnObject(ic)
            End Try
        Next
    End Sub

    Private Shared Sub SerializeRichText(rtc As IEnumerable(Of RichTextElement), fileName As String)
        Dim rts = DigitalLibraryRichText.Serializer.Clone()
        Dim settings As New XmlWriterSettings With {
            .ConformanceLevel = ConformanceLevel.Fragment
        }
        Using xw As XmlWriter = XmlWriter.Create(fileName, settings)
            rts.WriteRichTextToXml(rtc, xw)
        End Using
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Environment.Exit(0)
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        MsgBox(Label1.Text)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Cursor = Cursors.WaitCursor
        Dim log As String = ""
        For Each item In ListBox2.SelectedItems
            Dim rid As String = Regex.Replace(item.ToString(), "^\[(.*?)\] .*$", "$1")
            Dim r As Libronix.DigitalLibrary.Resource = dls.ResourceManager.OpenResource(rid)
            If r Is Nothing Then
                log += "Unavailable resource: " & rid & vbCrLf
            Else
                Dim ri = dls.LibraryCatalog.GetResourceInfo(rid, ResourceFieldSet.All).Value
                Dim problems As Boolean = False
                Dim exportPath As String = Path.Combine(TextBox1.Text, ri.Type, ri.ResourceId.Replace(":", "."))
                Directory.CreateDirectory(exportPath)
                Using sw = New StreamWriter(File.Open(Path.Combine(exportPath, "resourceinfo.txt"), FileMode.CreateNew))
                    For Each prop In ri.GetType().GetProperties()
                        If prop.Name = "MostUsedRank" Then
                            ' skip this field as it is not populated
                        ElseIf prop.PropertyType = GetType(IEnumerable(Of RichTextElement)) Then
                            sw.WriteLine(prop.Name & ": [Rich Text]")
                            Dim v As IEnumerable(Of RichTextElement) = CType(prop.GetValue(ri), IEnumerable(Of RichTextElement))
                            SerializeRichText(v, Path.Combine(exportPath, "resourceinfo-" + prop.Name + ".xml"))
                        ElseIf prop.GetValue(ri) Is Nothing Then
                            sw.WriteLine(prop.Name & ": [null]")
                        Else
                            sw.WriteLine(prop.Name & ": " & prop.GetValue(ri).ToString())
                        End If
                    Next
                End Using
                Using sw = New StreamWriter(File.Open(Path.Combine(exportPath, "info.txt"), FileMode.CreateNew))
                    sw.Write(Analyze(r, problems, exportPath))
                End Using
                If problems Then
                    log += "Export incomplete: " & rid & vbCrLf
                End If
            End If
        Next
        If log <> "" Then
            MsgBox(log, MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, "Export warnings")
        Else
            MsgBox("Export complete", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Export complete")
        End If
        Me.Cursor = Cursors.Default
    End Sub
End Class
