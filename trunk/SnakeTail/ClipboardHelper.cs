using System;
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    sealed internal class ClipboardHelper
    {
        public static void CopyToClipboard(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            try
            {
                Clipboard.SetText(content);
            }
            catch (Exception firstException)
            {
                System.Diagnostics.Debug.WriteLine("Clipboard cannot be updated, maybe locked by another application: " + firstException.Message);
                try
                {
                    // Perform a last retry as recommended on stackoverflow
                    System.Threading.Thread.Sleep(100);
                    Clipboard.Clear();
                    System.Threading.Thread.Sleep(100);
                    Clipboard.SetDataObject(content, true, 0, 0);
                }
                catch (Exception retryException)
                {
                    System.Diagnostics.Debug.WriteLine("Clipboard cannot be updated, maybe locked by another application: " + retryException.Message);
                    throw firstException;
                }
            }
        }
    }
}
