using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public interface ITailForm
    {
        Form TailWindow { get; }

        bool SearchForText(string searchText, bool matchCase, bool searchForward, bool keywordHighlights);

        void SaveConfig(TailFileConfig config);

        void LoadConfig(TailFileConfig config, string configPath);
    }
}
