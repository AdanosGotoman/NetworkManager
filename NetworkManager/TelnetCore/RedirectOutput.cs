using System.IO;
using System.Text;
using System.Windows.Controls;

namespace NetworkManager.TelnetCore
{
    public class RedirectOutput : TextWriter
    {
        TextBox outputText = null;

        public RedirectOutput(TextBox output)
        {
            outputText = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            outputText.AppendText(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}