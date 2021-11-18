using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var traceListener = new TraceListenerEx();
            traceListener.OnWriteMessage += TraceListener_OnWriteMessage;
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(traceListener);
        }

        private void TraceListener_OnWriteMessage(object sender, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    TraceListener_OnWriteMessage(sender, message);
                }));
            }
            else
            {
                outputTextBox.SelectedText = $"{message}{Environment.NewLine}";
            }
        }

        private void runMigrationButton_Click(object sender, EventArgs e)
        {
            runMigrationButton.Enabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Migration_DoWork;
            worker.RunWorkerCompleted += Migration_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Migration_DoWork(object sender, DoWorkEventArgs e)
        {
            var migrator = new Migrator();
            migrator.RunMigration();
        }

        private void Migration_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runMigrationButton.Enabled = true;

            if (e.Error is not null)
            {
                System.Diagnostics.Trace.WriteLine($"Migration failed. Error: {e.Error.Message}");
                MessageBox.Show("Migration failed");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Migration complete");
                MessageBox.Show("Migration complete");
            }
        }
    }
}
