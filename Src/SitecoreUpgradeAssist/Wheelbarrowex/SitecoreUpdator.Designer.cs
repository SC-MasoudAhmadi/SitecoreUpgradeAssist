﻿using System.Windows.Forms;

namespace Wheelbarrowex.Forms
{
    partial class SitecoreUpdator : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SitecoreUpdator));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Update = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Framework = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.reloadButton = new System.Windows.Forms.Button();
            this.MSSCUpgradeBtn = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.glassUpgradebtn = new System.Windows.Forms.Button();
            this.glassRefactorBtn = new System.Windows.Forms.Button();
            this.pkgReferensingBtn = new System.Windows.Forms.Button();
            this.netFrameworkWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Update,
            this.ProjectName,
            this.Framework});
            this.dataGridView1.Location = new System.Drawing.Point(12, 71);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(772, 266);
            this.dataGridView1.TabIndex = 0;
            // 
            // Update
            // 
            this.Update.DataPropertyName = "IsSelected";
            this.Update.HeaderText = "Update";
            this.Update.Name = "Update";
            // 
            // ProjectName
            // 
            this.ProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ProjectName.DataPropertyName = "Name";
            this.ProjectName.HeaderText = "Project Name";
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.ReadOnly = true;
            // 
            // Framework
            // 
            this.Framework.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Framework.DataPropertyName = "Framework";
            this.Framework.HeaderText = "Current Framework";
            this.Framework.Name = "Framework";
            this.Framework.ReadOnly = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(307, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Select All";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(93, 39);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Select None";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button3.Location = new System.Drawing.Point(12, 621);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 52);
            this.button3.TabIndex = 4;
            this.button3.Text = "Upgrade Framework";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(408, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 5;
            // 
            // reloadButton
            // 
            this.reloadButton.Location = new System.Drawing.Point(174, 39);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(145, 23);
            this.reloadButton.TabIndex = 6;
            this.reloadButton.Text = "Reload Projects List";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // MSSCUpgradeBtn
            // 
            this.MSSCUpgradeBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.MSSCUpgradeBtn.Location = new System.Drawing.Point(94, 621);
            this.MSSCUpgradeBtn.Name = "MSSCUpgradeBtn";
            this.MSSCUpgradeBtn.Size = new System.Drawing.Size(76, 52);
            this.MSSCUpgradeBtn.TabIndex = 7;
            this.MSSCUpgradeBtn.Text = "Package Upgrade";
            this.MSSCUpgradeBtn.UseVisualStyleBackColor = true;
            this.MSSCUpgradeBtn.Click += new System.EventHandler(this.MSSCUpgradeBtn_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 348);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(772, 238);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // glassUpgradebtn
            // 
            this.glassUpgradebtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.glassUpgradebtn.Location = new System.Drawing.Point(176, 621);
            this.glassUpgradebtn.Name = "glassUpgradebtn";
            this.glassUpgradebtn.Size = new System.Drawing.Size(76, 52);
            this.glassUpgradebtn.TabIndex = 9;
            this.glassUpgradebtn.Text = "Glass Upgrade";
            this.glassUpgradebtn.UseVisualStyleBackColor = true;
            this.glassUpgradebtn.Click += new System.EventHandler(this.glassUpgradebtn_Click);
            // 
            // glassRefactorBtn
            // 
            this.glassRefactorBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.glassRefactorBtn.Enabled = false;
            this.glassRefactorBtn.Location = new System.Drawing.Point(258, 621);
            this.glassRefactorBtn.Name = "glassRefactorBtn";
            this.glassRefactorBtn.Size = new System.Drawing.Size(76, 52);
            this.glassRefactorBtn.TabIndex = 10;
            this.glassRefactorBtn.Text = "Refactor to Glass 5";
            this.glassRefactorBtn.UseVisualStyleBackColor = true;
            this.glassRefactorBtn.Click += new System.EventHandler(this.glassRefactorBtn_Click);
            // 
            // pkgReferensingBtn
            // 
            this.pkgReferensingBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pkgReferensingBtn.Location = new System.Drawing.Point(340, 621);
            this.pkgReferensingBtn.Name = "pkgReferensingBtn";
            this.pkgReferensingBtn.Size = new System.Drawing.Size(117, 52);
            this.pkgReferensingBtn.TabIndex = 11;
            this.pkgReferensingBtn.Text = "Migrate To PackageReferencing";
            this.pkgReferensingBtn.UseVisualStyleBackColor = true;
            this.pkgReferensingBtn.Click += new System.EventHandler(this.pkgReferensingBtn_Click);
            // 
            // netFrameworkWorker
            // 
            this.netFrameworkWorker.WorkerReportsProgress = true;
            this.netFrameworkWorker.WorkerSupportsCancellation = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 592);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(772, 23);
            this.progressBar1.TabIndex = 12;
            // 
            // SitecoreUpdator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 683);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pkgReferensingBtn);
            this.Controls.Add(this.glassRefactorBtn);
            this.Controls.Add(this.glassUpgradebtn);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.MSSCUpgradeBtn);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SitecoreUpdator";
            this.Text = "Sitecore Upgrade Assist";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private new System.Windows.Forms.DataGridViewCheckBoxColumn Update;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Framework;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button reloadButton;
        private Button MSSCUpgradeBtn;
        private RichTextBox richTextBox1;
        private Button glassUpgradebtn;
        private Button glassRefactorBtn;
        private Button pkgReferensingBtn;
        private System.ComponentModel.BackgroundWorker netFrameworkWorker;
        private ProgressBar progressBar1;
    }
}