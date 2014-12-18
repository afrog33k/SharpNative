using System.IO;
using System.Text;
using System.Windows.Controls;

namespace CsNativeVisual
{
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void WriteLine(string value)
        {
            value += "\n";
            textbox.Dispatcher.InvokeAsync(() => textbox.Text += value);
            ;
        }

        public override void Write(char value)
        {
            textbox.Dispatcher.InvokeAsync(() => textbox.Text += value.ToString());
          
        }

        public override void Write(string value)
        {
            textbox.Dispatcher.InvokeAsync(() => textbox.Text += value);
          
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}