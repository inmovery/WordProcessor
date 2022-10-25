namespace WordProcessor
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.RichTextBox = new System.Windows.Forms.RichTextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.WordDictionary = new System.Windows.Forms.ToolStripMenuItem();
			this.CreateDictionaryCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.UpdateDictionaryCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.ClearDictionaryCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// RichTextBox
			// 
			this.RichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.RichTextBox.DetectUrls = false;
			this.RichTextBox.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.RichTextBox.Location = new System.Drawing.Point(16, 37);
			this.RichTextBox.Name = "RichTextBox";
			this.RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.RichTextBox.Size = new System.Drawing.Size(758, 423);
			this.RichTextBox.TabIndex = 0;
			this.RichTextBox.Text = "";
			this.RichTextBox.SelectionChanged += new System.EventHandler(this.RichTextBox_SelectionChanged);
			this.RichTextBox.Click += new System.EventHandler(this.RichTextBox_Click);
			this.RichTextBox.TextChanged += new System.EventHandler(this.RichTextBox_TextChanged);
			this.RichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox_KeyDown);
			this.RichTextBox.Leave += new System.EventHandler(this.RichTextBox_Leave);
			this.RichTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.RichTextBox_PreviewKeyDown);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WordDictionary});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(789, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// WordDictionary
			// 
			this.WordDictionary.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateDictionaryCommand,
            this.UpdateDictionaryCommand,
            this.ClearDictionaryCommand});
			this.WordDictionary.Name = "WordDictionary";
			this.WordDictionary.Size = new System.Drawing.Size(73, 20);
			this.WordDictionary.Text = "Dictionary";
			// 
			// CreateDictionaryCommand
			// 
			this.CreateDictionaryCommand.Name = "CreateDictionaryCommand";
			this.CreateDictionaryCommand.Size = new System.Drawing.Size(112, 22);
			this.CreateDictionaryCommand.Text = "Create";
			this.CreateDictionaryCommand.Click += new System.EventHandler(this.CreateDictionaryCommand_Click);
			// 
			// UpdateDictionaryCommand
			// 
			this.UpdateDictionaryCommand.Name = "UpdateDictionaryCommand";
			this.UpdateDictionaryCommand.Size = new System.Drawing.Size(112, 22);
			this.UpdateDictionaryCommand.Text = "Update";
			this.UpdateDictionaryCommand.Click += new System.EventHandler(this.UpdateDictionaryCommand_Click);
			// 
			// ClearDictionaryCommand
			// 
			this.ClearDictionaryCommand.Name = "ClearDictionaryCommand";
			this.ClearDictionaryCommand.Size = new System.Drawing.Size(112, 22);
			this.ClearDictionaryCommand.Text = "Clear";
			this.ClearDictionaryCommand.Click += new System.EventHandler(this.ClearDictionaryCommand_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.ClientSize = new System.Drawing.Size(789, 474);
			this.Controls.Add(this.RichTextBox);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "Word Processor";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox RichTextBox;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem WordDictionary;
		private ToolStripMenuItem CreateDictionaryCommand;
		private ToolStripMenuItem UpdateDictionaryCommand;
		private ToolStripMenuItem ClearDictionaryCommand;
	}
}