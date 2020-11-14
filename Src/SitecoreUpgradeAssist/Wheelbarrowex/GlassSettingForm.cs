using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VHQLabs.TargetFrameworkMigrator.Wheelbarrowex
{
    public partial class GlassSettingForm : Form
    {
        public GlassSettingForm()
        {
            InitializeComponent();
        }

        public string GetSelectedVersion { get => comboBox1.Text; }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
