namespace HW4
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvUnitTable = new System.Windows.Forms.DataGridView();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnitTable)).BeginInit();
            this.ssStatus.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvUnitTable
            // 
            this.dgvUnitTable.AllowUserToAddRows = false;
            this.dgvUnitTable.AllowUserToDeleteRows = false;
            this.dgvUnitTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnitTable.Location = new System.Drawing.Point(12, 27);
            this.dgvUnitTable.MultiSelect = false;
            this.dgvUnitTable.Name = "dgvUnitTable";
            this.dgvUnitTable.RowHeadersWidth = 72;
            this.dgvUnitTable.Size = new System.Drawing.Size(875, 479);
            this.dgvUnitTable.TabIndex = 0;
            this.dgvUnitTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvUnitTable_CellValueChanged);
            this.dgvUnitTable.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DgvUnitTable_DataError);
            // 
            // ssStatus
            // 
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.ssStatus.Location = new System.Drawing.Point(0, 509);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(899, 22);
            this.ssStatus.TabIndex = 1;
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(84, 17);
            this.tsslStatus.Text = "Current status.";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGameToolStripMenuItem,
            this.saveGameToolStripMenuItem,
            this.playGameToolStripMenuItem,
            this.attackToolStripMenuItem,
            this.runToolStripMenuItem,
            this.defenceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(899, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadGameToolStripMenuItem
            // 
            this.loadGameToolStripMenuItem.Name = "loadGameToolStripMenuItem";
            this.loadGameToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.loadGameToolStripMenuItem.Text = "Load game";
            this.loadGameToolStripMenuItem.Click += new System.EventHandler(this.LoadGameToolStripMenuItem_Click);
            // 
            // saveGameToolStripMenuItem
            // 
            this.saveGameToolStripMenuItem.Name = "saveGameToolStripMenuItem";
            this.saveGameToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.saveGameToolStripMenuItem.Text = "Save game";
            this.saveGameToolStripMenuItem.Visible = false;
            this.saveGameToolStripMenuItem.Click += new System.EventHandler(this.SaveGameToolStripMenuItem_Click);
            // 
            // playGameToolStripMenuItem
            // 
            this.playGameToolStripMenuItem.Name = "playGameToolStripMenuItem";
            this.playGameToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.playGameToolStripMenuItem.Text = "Play game";
            this.playGameToolStripMenuItem.Click += new System.EventHandler(this.PlayGameToolStripMenuItem_Click);
            // 
            // attackToolStripMenuItem
            // 
            this.attackToolStripMenuItem.Name = "attackToolStripMenuItem";
            this.attackToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.attackToolStripMenuItem.Text = "Attack!";
            this.attackToolStripMenuItem.Visible = false;
            this.attackToolStripMenuItem.Click += new System.EventHandler(this.GameActionToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.runToolStripMenuItem.Text = "Run!";
            this.runToolStripMenuItem.Visible = false;
            this.runToolStripMenuItem.Click += new System.EventHandler(this.GameActionToolStripMenuItem_Click);
            // 
            // defenceToolStripMenuItem
            // 
            this.defenceToolStripMenuItem.Name = "defenceToolStripMenuItem";
            this.defenceToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.defenceToolStripMenuItem.Text = "Defence!";
            this.defenceToolStripMenuItem.Visible = false;
            this.defenceToolStripMenuItem.Click += new System.EventHandler(this.GameActionToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 531);
            this.Controls.Add(this.dgvUnitTable);
            this.Controls.Add(this.ssStatus);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(320, 180);
            this.Name = "MainForm";
            this.Text = "Main form";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnitTable)).EndInit();
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvUnitTable;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defenceToolStripMenuItem;
    }
}

