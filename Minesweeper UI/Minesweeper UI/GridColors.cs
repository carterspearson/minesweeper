using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper_UI
{
    class GridColors
    {
        public static System.Windows.Media.Color mineColor = System.Windows.Media.Color.FromRgb(255, 0, 0);
        public static System.Windows.Media.Color uncoveredColor = System.Windows.Media.Color.FromRgb(211, 0, 211);
        public static System.Windows.Media.Color hiddenColor = System.Windows.Media.Color.FromRgb(128, 128, 128);
        public static System.Windows.Media.Color flaggedColor = System.Windows.Media.Color.FromRgb(0, 0, 255);
    }
}
