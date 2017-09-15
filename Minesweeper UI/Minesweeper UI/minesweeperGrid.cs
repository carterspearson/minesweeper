using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Minesweeper;
using System.Linq;

namespace Minesweeper_UI
{
    public class minesweeperGrid
    {
        public Grid uiGrid;
        public GridSpace[,] gridSpaceArray;
        public int maximumMines, numberMines, length, height, squareSize;

        public minesweeperGrid(Grid uiGrid0, int length0, int height0, int maximumMines0, int squareSize0)
        {
            uiGrid = uiGrid0;
            length = length0;
            height = height0;
            maximumMines = maximumMines0;
            squareSize = squareSize0;
            gridSpaceArray = new GridSpace [height, length];
            InitializeGrid();
        }

        public void reset()
        {
            gridSpaceArray = new GridSpace[height, length];
            InitializeGrid();
        }

        public void InitializeGridArray(int x0, int y0)
        {
            numberMines = 0;
            Random rnd = new Random();
            while (numberMines <= maximumMines)
            {
                int random = rnd.Next(0, length * height);
                int x = random % length;
                int y = random / length;
                if (x != x0 || y != y0)
                {
                    gridSpaceArray[y, x] = new GridSpace(true, x, y);
                    numberMines++;
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < length; k++)
                {
                    if (gridSpaceArray[i, k] == null)
                        gridSpaceArray[i, k] = new GridSpace(false, k, i);
                }
            }
        }

        private void InitializeGrid()
        {
            uiGrid.Children.Clear();
            uiGrid.Width = squareSize*length;
            uiGrid.Height = squareSize * height;
            GridLengthConverter myGridLengthConverter = new GridLengthConverter();
            GridLength side = (GridLength)myGridLengthConverter.ConvertFromString("Auto");
            for (int i = 0; i < length; i++)
            {
                uiGrid.ColumnDefinitions.Add(new ColumnDefinition());
                uiGrid.ColumnDefinitions[i].Width = side;
            }
            for (int i = 0; i < height; i++)
            {
                uiGrid.RowDefinitions.Add(new RowDefinition());
                uiGrid.RowDefinitions[i].Height = side;
            }

            Label[,] square = new Label[height, length];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    square[row, col] = new Label();
                    Label currentLabel = square[row, col];
                    currentLabel.Height = squareSize;
                    currentLabel.Width = squareSize;
                    currentLabel.Content = "";
                    currentLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    currentLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    currentLabel.BorderThickness = new Thickness(1);
                    Grid.SetColumn(currentLabel, col);
                    Grid.SetRow(currentLabel, row);
                    
                    currentLabel.Background = new SolidColorBrush(GridColors.hiddenColor);
                    currentLabel.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

                    uiGrid.Children.Add(currentLabel);
                }
            }
        }

        public bool uncover(List<GridSpace> listToUncover)
        {
            bool continueRecurse = true;
            List<GridSpace> tempListToUncover = new List<GridSpace>();
            GridSpace currentGridSpace = listToUncover.First();

            if (currentGridSpace.mine == true)
            {
                (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Background = new SolidColorBrush(GridColors.mineColor);
                return false;
            }
            if (currentGridSpace.uncovered == true && listToUncover.Count == 1)
                throw new Exception();
            currentGridSpace.uncovered = true;

            for (int i = currentGridSpace.x - 1; i <= currentGridSpace.x + 1; i++)
            {
                if (i >= 0 && i < length)
                {
                    for (int k = currentGridSpace.y - 1; k <= currentGridSpace.y + 1; k++)
                    {
                        if (k >= 0 && k < height)
                        {
                            if (gridSpaceArray[k, i].mine == true)
                            {
                                currentGridSpace.surroundingMines += 1;
                                continueRecurse = false;
                            }
                            else if (gridSpaceArray[k, i].uncovered != true)
                            {
                                if (!listToUncover.Contains(gridSpaceArray[k, i]))
                                {
                                    if (i != currentGridSpace.x && k != currentGridSpace.y && currentGridSpace.corner == false)
                                    {
                                        tempListToUncover.Add(gridSpaceArray[k, i]);
                                        gridSpaceArray[k, i].corner = true;
                                    }
                                    else
                                    {
                                        tempListToUncover.Add(gridSpaceArray[k, i]);
                                        gridSpaceArray[k, i].corner = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (currentGridSpace.flagged != true)
            {
                (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Background = new SolidColorBrush(GridColors.uncoveredColor);
                if (currentGridSpace.surroundingMines == 0)
                    (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Content = "";
                else
                    (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Content = currentGridSpace.surroundingMines;
            }
            listToUncover.Remove(currentGridSpace);
            if (listToUncover.Count == 0 && tempListToUncover.Count == 0)
                return true;
            else if (continueRecurse == false)
            {
                if (listToUncover.Count == 0)
                    return true;
                else
                    return uncover(listToUncover);
            }
            else
            {
                listToUncover = listToUncover.Concat(tempListToUncover).ToList();
                return uncover(listToUncover);
            }
        }

        public bool flag(int x, int y)
        {
            if (gridSpaceArray[y, x].uncovered != true)
            {
                bool win = true;
                if (gridSpaceArray[y, x].flagged == true)
                {
                    gridSpaceArray[y, x].flagged = false;

                    (uiGrid.Children[x + y * length] as Label).Background = new SolidColorBrush(GridColors.hiddenColor);

                    win = false;
                }
                else
                {
                    (uiGrid.Children[x + y * length] as Label).Background = new SolidColorBrush(GridColors.flaggedColor);
                    gridSpaceArray[y, x].flagged = true;

                    for (int i = 0; i < length; i++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            if (gridSpaceArray[k, i].mine == true && gridSpaceArray[k, i].flagged == false)
                                win = false;
                            else if (gridSpaceArray[k, i].mine == false && gridSpaceArray[k, i].flagged == true)
                                win = false;
                        }
                    }
                }
                return win;
            }
            else
                return false;
        }

        public void uncoverMines()
        {
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < length; k++)
                {
                    if (gridSpaceArray[i, k].mine == true)
                        (uiGrid.Children[k + i * length] as Label).Background = new SolidColorBrush(GridColors.mineColor);
                }
            }
        }
    }
}



	

