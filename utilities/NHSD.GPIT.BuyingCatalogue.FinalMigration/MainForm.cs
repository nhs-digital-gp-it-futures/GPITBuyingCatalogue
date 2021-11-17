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

        private void dryRunButton_Click(object sender, EventArgs e)
        {
            dryRunButton.Enabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DryRun_DoWork;
            worker.RunWorkerCompleted += DryRun_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void DryRun_DoWork(object sender, DoWorkEventArgs e)
        {
            var migrator = new Migrator();
            migrator.RunMigration();
        }

        private void DryRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dryRunButton.Enabled = true;

            if (e.Error is not null)
            {
                System.Diagnostics.Trace.WriteLine($"Dry run failed. Error: {e.Error.Message}");
                MessageBox.Show("Dry run failed");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Dry run complete");
                MessageBox.Show("Dry run complete");
            }
        }
    }
}
