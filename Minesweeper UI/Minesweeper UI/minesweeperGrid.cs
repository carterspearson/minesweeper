using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Minesweeper;
using System.Linq;

namespace Minesweeper_UI
{
    // class representing a grid of grid spaces
    public class minesweeperGrid
    {
        public Grid uiGrid;
        public GridSpace[,] gridSpaceArray;
        public int maximumMines, numberMines, length, height, squareSize;

        // constructor
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

        // resets grid
        public void reset()
        {
            gridSpaceArray = new GridSpace[height, length];
            InitializeGrid();
        }

        // populates mines in a grid, does not create a mine where the user first clicks
        public void PopulateMines(int x0, int y0)
        {
            numberMines = 0;
            Random rnd = new Random();
            while (numberMines <= maximumMines)
            {
                int random = rnd.Next(0, length * height);
                int x = random % length; // get x and y coordinate from random number
                int y = random / length;
                if (x != x0 || y != y0) // only create a mine if it is not the one user clicked on
                {
                    if (gridSpaceArray[y, x] == null) // only create a mine if the space has not been initialized
                    {
                        gridSpaceArray[y, x] = new GridSpace(true, x, y);
                        numberMines++;
                    }
                }
            }
            for (int i = 0; i < height; i++) // iterate through the rest of the grid spaces and initialize them
            {
                for (int k = 0; k < length; k++)
                {
                    if (gridSpaceArray[i, k] == null)
                        gridSpaceArray[i, k] = new GridSpace(false, k, i);
                }
            }
        }

        // creates a grid
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

        // recursively uncovers grid spaces
        // if a grid space does not have a neighbouring mine, uncover each of its neighbours
        public bool uncover(List<GridSpace> listToUncover)
        {
            bool continueRecurse = true;
            List<GridSpace> tempListToUncover = new List<GridSpace>();
            GridSpace currentGridSpace = listToUncover.First();

            // check if the selected grid space contains a mine
            if (currentGridSpace.mine == true)
            {
                (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Background = new SolidColorBrush(GridColors.mineColor);
                return false;
            }
            if (currentGridSpace.uncovered == true && listToUncover.Count == 1)
                throw new Exception();
            currentGridSpace.uncovered = true;

            // iterate through each of the neighbours of the current grid space
            for (int i = currentGridSpace.x - 1; i <= currentGridSpace.x + 1; i++)
            {
                if (i >= 0 && i < length)
                {
                    for (int k = currentGridSpace.y - 1; k <= currentGridSpace.y + 1; k++)
                    {
                        if (k >= 0 && k < height)
                        {
                            // if space has a neighbouring mine, stop recursing
                            if (gridSpaceArray[k, i].mine == true)
                            {
                                currentGridSpace.surroundingMines += 1;
                                continueRecurse = false;
                            }
                            // if the neighbour has not been uncovered and is not a mine, add it to a list of spaces to uncover
                            else if (gridSpaceArray[k, i].uncovered != true)
                            {
                                // only add to list to uncover if the list does not already contain it
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
                // set color of the uncovered grid, as well as the number of surrounding mines
                (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Background = new SolidColorBrush(GridColors.uncoveredColor);
                if (currentGridSpace.surroundingMines == 0)
                    (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Content = "";
                else
                    (uiGrid.Children[currentGridSpace.x + currentGridSpace.y * length] as Label).Content = currentGridSpace.surroundingMines;
            }
            listToUncover.Remove(currentGridSpace);
            // if all possible spaces have been uncovered, return true
            if (listToUncover.Count == 0 && tempListToUncover.Count == 0)
                return true;
            // if we need to stop recursing, do not add current list to master
            // master list to uncover still needs to be checked
            else if (continueRecurse == false)
            {
                if (listToUncover.Count == 0)
                    return true;
                else
                    return uncover(listToUncover);
            }
            // add newly created list to uncover to master list to uncover
            else
            {
                listToUncover = listToUncover.Concat(tempListToUncover).ToList();
                return uncover(listToUncover);
            }
        }

        // change a grid space status to flagged
        public bool flag(int x, int y)
        {
            // only allow flagging on uncovered spaces
            if (gridSpaceArray[y, x].uncovered != true)
            {
                bool win = true;
                // if a space is flagged, flagging it again will unflag it
                if (gridSpaceArray[y, x].flagged == true)
                {
                    gridSpaceArray[y, x].flagged = false;

                    (uiGrid.Children[x + y * length] as Label).Background = new SolidColorBrush(GridColors.hiddenColor);
                }
                else
                {
                    (uiGrid.Children[x + y * length] as Label).Background = new SolidColorBrush(GridColors.flaggedColor);
                    gridSpaceArray[y, x].flagged = true;
                }

                // check for any incorrect flags or unflagged mines 
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

                // if no incorrect flags or missed mines, the player wins
                return win;
            }
            else
                return false;
        }

        // display all spaces which contain mines
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



	

