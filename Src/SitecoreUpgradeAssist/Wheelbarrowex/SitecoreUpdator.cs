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
using VHQLabs.TargetFrameworkMigrator.Wheelbarrowex;
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
            InitializeBackgroundWorker();
        }

        public event Action UpdateFired;
        public event Action ReloadFired;
        public event Action UpdateMSSCPkgFired;
        public event Action UpdateGlassPkgFired;
        public event Action MigrateToPackageReferencing;

        public string CurrentGlassVersion;

        public IEnumerable<SitecoreVersionModel> AvailableVersions
        {
            set
            {
                comboBox1.DataSource = value;
            }
        }
        
        private object _lock = new object();
        public List<ProjectModel> Projects
        {
            set
            {
                var wrapperBindingList = new SortableBindingList<ProjectModel>(value);
                try
                {
                    lock (_lock)
                    {
                        dataGridView1.DataSource = wrapperBindingList;
                        dataGridView1.Refresh();
                    }
                }
                catch (InvalidOperationException)
                {
                    lock (_lock)
                    {
                        Invoke(new EventHandler(delegate
                        {
                            dataGridView1.DataSource = wrapperBindingList;
                            dataGridView1.Refresh();
                        }));
                    }
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
        
        private void InitializeBackgroundWorker()
        {
            netFrameworkWorker.DoWork +=
                new DoWorkEventHandler(NetFrameworkWorker_DoWork);
            //netFrameworkWorker.RunWorkerCompleted +=
            //    new RunWorkerCompletedEventHandler(
            //        backgroundWorker1_RunWorkerCompleted);
            //netFrameworkWorker.ProgressChanged +=
            //    new ProgressChangedEventHandler(
            //        backgroundWorker1_ProgressChanged);
        }

        private void NetFrameworkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            var actions = e.Argument as Action;

            e.Result = actions?.BeginInvoke(null,e);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var onUpdate = UpdateFired;
            if (onUpdate != null && !netFrameworkWorker.IsBusy)
            {
                netFrameworkWorker.RunWorkerAsync(onUpdate);
            };
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
            if (onReloadFired != null && !netFrameworkWorker.IsBusy)
            {
                netFrameworkWorker.RunWorkerAsync(onReloadFired);
            };
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private async void MSSCUpgradeBtn_Click(object sender, EventArgs e)
        {
            var onUpdate = UpdateMSSCPkgFired;
            if (onUpdate != null && !netFrameworkWorker.IsBusy)
            {
                netFrameworkWorker.RunWorkerAsync(onUpdate);
            };
        }

        private async void glassUpgradebtn_Click(object sender, EventArgs e)
        {
            var dialog = new GlassSettingForm();


            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                var version = dialog.GetSelectedVersion;
                dialog.Dispose();
                // Read the contents of testDialog's TextBox.

                var onUpdate = UpdateGlassPkgFired;
                if (onUpdate != null && !netFrameworkWorker.IsBusy)
                {
                    netFrameworkWorker.RunWorkerAsync(onUpdate);
                };
            }
            else
            {
                dialog.Dispose();
            }
            
                        
        }

        private async void pkgReferensingBtn_Click(object sender, EventArgs e)
        {
            var onUpdate = MigrateToPackageReferencing;
            if (onUpdate != null && !netFrameworkWorker.IsBusy)
            {
                netFrameworkWorker.RunWorkerAsync(onUpdate);
            };
        }
    }
}
