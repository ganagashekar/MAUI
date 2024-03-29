<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.listBox1 = New System.Windows.Forms.ListBox()
        Me.btnStartMonitoring = New System.Windows.Forms.Button()
        Me.fileSystemWatcher1 = New System.IO.FileSystemWatcher()
        CType(Me.fileSystemWatcher1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'listBox1
        '
        Me.listBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.listBox1.FormattingEnabled = True
        Me.listBox1.Location = New System.Drawing.Point(36, 70)
        Me.listBox1.Name = "listBox1"
        Me.listBox1.Size = New System.Drawing.Size(462, 355)
        Me.listBox1.TabIndex = 3
        '
        'btnStartMonitoring
        '
        Me.btnStartMonitoring.Location = New System.Drawing.Point(36, 25)
        Me.btnStartMonitoring.Name = "btnStartMonitoring"
        Me.btnStartMonitoring.Size = New System.Drawing.Size(232, 23)
        Me.btnStartMonitoring.TabIndex = 2
        Me.btnStartMonitoring.Text = "Start Monitoring"
        Me.btnStartMonitoring.UseVisualStyleBackColor = True
        '
        'fileSystemWatcher1
        '
        Me.fileSystemWatcher1.EnableRaisingEvents = True
        Me.fileSystemWatcher1.SynchronizingObject = Me
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(535, 450)
        Me.Controls.Add(Me.listBox1)
        Me.Controls.Add(Me.btnStartMonitoring)
        Me.Name = "Form1"
        Me.Text = "www.emoreau.com - Demo file trailer reading (VB)"
        CType(Me.fileSystemWatcher1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents listBox1 As ListBox
    Private WithEvents btnStartMonitoring As Button
    Private WithEvents fileSystemWatcher1 As IO.FileSystemWatcher
End Class
