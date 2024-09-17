<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        ListBox1 = New ListBox()
        SplitContainer1 = New SplitContainer()
        ListBox2 = New ListBox()
        TextBox1 = New TextBox()
        Button1 = New Button()
        Button2 = New Button()
        FolderBrowserDialog1 = New FolderBrowserDialog()
        Label1 = New Label()
        ComboBox1 = New ComboBox()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        SuspendLayout()
        ' 
        ' ListBox1
        ' 
        ListBox1.Dock = DockStyle.Fill
        ListBox1.FormattingEnabled = True
        ListBox1.IntegralHeight = False
        ListBox1.Location = New Point(0, 0)
        ListBox1.Name = "ListBox1"
        ListBox1.SelectionMode = SelectionMode.MultiExtended
        ListBox1.Size = New Size(278, 439)
        ListBox1.TabIndex = 0
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        SplitContainer1.Location = New Point(14, 15)
        SplitContainer1.Name = "SplitContainer1"
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(ListBox1)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(ListBox2)
        SplitContainer1.Size = New Size(893, 439)
        SplitContainer1.SplitterDistance = 278
        SplitContainer1.SplitterWidth = 5
        SplitContainer1.TabIndex = 1
        ' 
        ' ListBox2
        ' 
        ListBox2.Dock = DockStyle.Fill
        ListBox2.FormattingEnabled = True
        ListBox2.IntegralHeight = False
        ListBox2.Location = New Point(0, 0)
        ListBox2.Name = "ListBox2"
        ListBox2.SelectionMode = SelectionMode.MultiExtended
        ListBox2.Size = New Size(610, 439)
        ListBox2.TabIndex = 0
        ' 
        ' TextBox1
        ' 
        TextBox1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        TextBox1.Location = New Point(14, 496)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(719, 27)
        TextBox1.TabIndex = 2
        ' 
        ' Button1
        ' 
        Button1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Button1.Location = New Point(741, 495)
        Button1.Name = "Button1"
        Button1.Size = New Size(50, 29)
        Button1.TabIndex = 3
        Button1.Text = "..."
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Button2.Location = New Point(798, 493)
        Button2.Name = "Button2"
        Button2.Size = New Size(109, 33)
        Button2.TabIndex = 4
        Button2.Text = "Export"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Label1.AutoEllipsis = True
        Label1.Location = New Point(14, 456)
        Label1.Name = "Label1"
        Label1.Size = New Size(721, 35)
        Label1.TabIndex = 5
        Label1.Text = "Select a resource to show information here"
        ' 
        ' ComboBox1
        ' 
        ComboBox1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox1.FormattingEnabled = True
        ComboBox1.Items.AddRange(New Object() {"Available", "Unavailable", "All"})
        ComboBox1.Location = New Point(741, 461)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(166, 28)
        ComboBox1.TabIndex = 6
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(920, 541)
        Controls.Add(ComboBox1)
        Controls.Add(Label1)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(TextBox1)
        Controls.Add(SplitContainer1)
        Name = "Form1"
        Text = "Logos Resource Loader"
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents ListBox2 As ListBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents Label1 As Label
    Friend WithEvents ComboBox1 As ComboBox

End Class
