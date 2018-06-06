

namespace Minesweeper
{
    // class representing a single grid space
    public class GridSpace
    {
        public bool flagged, mine, uncovered, corner;
        public int surroundingMines, x, y;

        // default constructor
        public GridSpace()
        {
            flagged = false;
            mine = false;
            uncovered = false;
            surroundingMines = 0;
            x = 0;
            y = 0;
        }

        // constructor for grid space
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
