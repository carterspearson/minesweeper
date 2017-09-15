using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public minesweeperGrid grid;
        bool firstClick = true;

        public MainWindow()
        {
            InitializeComponent();
            grid = new minesweeperGrid(UIGrid, 38, 20, 100, 25);
        }

        private void UIGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int x = (int)(e.GetPosition(UIGrid).X / grid.squareSize);
            int y = (int)(e.GetPosition(UIGrid).Y / grid.squareSize);
            if (firstClick)
            {
                grid.InitializeGridArray(x, y);
                firstClick = false;
            }
            List<Minesweeper.GridSpace> inputList = new List<Minesweeper.GridSpace>();
            inputList.Add(grid.gridSpaceArray[y,x]);
            try
            {
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

        private void UIGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int x = (int)(e.GetPosition(UIGrid).X / grid.squareSize);
            int y = (int)(e.GetPosition(UIGrid).Y / grid.squareSize);
            if (grid.flag(x, y))
                MessageBox.Show("You win!");
        }

    }
}
