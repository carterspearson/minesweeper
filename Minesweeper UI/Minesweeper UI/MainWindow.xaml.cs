using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace Minesweeper_UI
{
    public partial class MainWindow : Window
    {
        public minesweeperGrid grid;
        bool firstClick = true;

        public MainWindow()
        {
            InitializeComponent();
            grid = new minesweeperGrid(UIGrid, 38, 20, 100, 25);
        }

        // handles left mouse click
        private void UIGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // get coordinate from selected grid space
            int x = (int)(e.GetPosition(UIGrid).X / grid.squareSize);
            int y = (int)(e.GetPosition(UIGrid).Y / grid.squareSize);
            // if player is clicking for the first time, populate the mines
            if (firstClick)
            {
                grid.PopulateMines(x, y);
                firstClick = false;
            }
            // uncover selected grid space
            List<Minesweeper.GridSpace> inputList = new List<Minesweeper.GridSpace>();
            inputList.Add(grid.gridSpaceArray[y,x]);
            try
            {
                // if the selected space is a mine, game over
                if (!grid.uncover(inputList))
                {
                    grid.uncoverMines();
                    MessageBoxResult result = MessageBox.Show("Game over!  Play again?", "Game over!" , MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                        this.Close();
                    else
                    {
                        firstClick = true;
                        grid.reset();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // handles right mouse click
        private void UIGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // get coordinates from clicked on space
            int x = (int)(e.GetPosition(UIGrid).X / grid.squareSize);
            int y = (int)(e.GetPosition(UIGrid).Y / grid.squareSize);
            // if flag returns true, all mines have been flagged and player wins
            if (grid.flag(x, y))
            {
                MessageBoxResult result = MessageBox.Show("You win!  Play again?", "You win!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    this.Close();
                else
                {
                    firstClick = true;
                    grid.reset();
                }
            }
        }

    }
}
