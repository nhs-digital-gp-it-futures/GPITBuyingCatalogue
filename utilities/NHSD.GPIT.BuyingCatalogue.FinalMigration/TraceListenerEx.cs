using System.Diagnostics;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    public class TraceListenerEx : TraceListener
    {
        public delegate void WriteMessage(object sender, string message);
        public event WriteMessage OnWriteMessage;

        public override void WriteLine(string message)
        {
            OnWriteMessage(this, message);
        }

        public override void Write(string message)
        {
            OnWriteMessage(this, message);
        }
    }
}
