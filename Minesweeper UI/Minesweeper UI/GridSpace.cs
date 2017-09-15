using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Minesweeper
{
    public class GridSpace
    {
        public bool flagged, mine, uncovered, corner;
        public int surroundingMines, x, y;

        public GridSpace()
        {
            flagged = false;
            mine = false;
            uncovered = false;
            surroundingMines = 0;
            x = 0;
            y = 0;
        }

        public GridSpace(bool mineStatus, int x0, int y0)
        {
            flagged = false;
            mine = mineStatus;
            uncovered = false;
            surroundingMines = 0;
            x = x0;
            y = y0;
        }
    }
}
