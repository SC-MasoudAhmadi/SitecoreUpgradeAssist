using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VHQLabs.TargetFrameworkMigrator;
using Wheelbarrowex.Models;

namespace Wheelbarrowex.Forms
{
    public partial class SitecoreUpdator : Form
    {
        public SitecoreUpdator()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public event Action UpdateFired;
        public event Action ReloadFired;
        public event Action UpdateMSSCPkgFired;

        public IEnumerable<SitecoreVersionModel> AvailableVersions
        {
            set
            {
                comboBox1.DataSource = value;
            }
        }
        public List<ProjectModel> Projects
        {
            set
            {
                var wrapperBindingList = new SortableBindingList<ProjectModel>(value);
                try
                {
                    dataGridView1.DataSource = wrapperBindingList;
                    dataGridView1.Refresh();
                }
                catch (InvalidOperationException)
                {
                    Invoke(new EventHandler(delegate
                    {
                        dataGridView1.DataSource = wrapperBindingList;
                        dataGridView1.Refresh();
                    }));
                }
            }
            get
            {
                SortableBindingList<ProjectModel> wrapperBindingList = null;
                try
                {
                    wrapperBindingList = (SortableBindingList<ProjectModel>)dataGridView1.DataSource;
                }
                catch (InvalidOperationException)
                {
                    Invoke(new EventHandler(delegate
                    {
                        wrapperBindingList = (SortableBindingList<ProjectModel>)dataGridView1.DataSource;
                    }));
                }
                return wrapperBindingList.WrappedList;
            }
        }

        public SitecoreVersionModel SelectedSitecoreVersion
        {
            get
            {
                SitecoreVersionModel model = null;
                Invoke(new EventHandler(delegate
                {
                    model = (SitecoreVersionModel)comboBox1.SelectedItem;
                }));
                return model;
            }
        }

        public string State
        {
            set
            {
                try
                {
                    richTextBox1.AppendText(Environment.NewLine + value);
                }
                catch (InvalidOperationException)
                {
                    Invoke(new EventHandler(delegate
                    {
                        richTextBox1.AppendText(Environment.NewLine + value);
                    }));
                }
            }
        }

        public void EnableNextStep()
        {
            button3.Enabled = false;
            MSSCUpgradeBtn.Enabled = true;
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            var onUpdate = UpdateFired;
            if (onUpdate != null)
                await Task.Run(() =>
                {
                    onUpdate.Invoke();
                });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var projectModel in Projects)
            {
                projectModel.IsSelected = true;
            }
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var projectModel in Projects)
            {
                projectModel.IsSelected = false;
            }
            dataGridView1.Refresh();
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            var onReloadFired = ReloadFired;
            if (onReloadFired != null)
                onReloadFired.Invoke();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private async void MSSCUpgradeBtn_Click(object sender, EventArgs e)
        {
            var onUpdate = UpdateMSSCPkgFired;
            if (onUpdate != null)
                await Task.Run(() =>
                {
                    onUpdate.Invoke();
                });
        }
    }
}
