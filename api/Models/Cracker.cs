﻿using refs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Cracker
    {
        private List<Cell> _grid;
        private readonly int[] _range = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private Dictionary<int, List<string>> _rowPossibilities = new Dictionary<int, List<string>>();

        public Cracker(List<ICell> grid)
        {
            this._grid = grid.Select(x => new Cell(x.Row, x.Column, x.Value)).ToList();
        }

        public List<ICell> Solve()
        {
            // check for invalid values and missing cells
            PrepareCells();

            // find all the values each cell can possibly be
            DeterminePossiblesPerCell();

            // find all possibilities for each row
            DetermineRowPossibilities();            

            var solution = FindSolution();

            return solution;
        }

        private void DetermineRowPossibilities()
        {
            // loop through each row
            for (var i = 0; i < 9; i++)
            {
                var row = this._grid.Where(x => x.Row == i).OrderBy(x => x.Column);

                var possibles = new List<string>();

                // add the possible values from the first cell
                foreach (var value in row.First().PossibleValues)
                {
                    possibles.Add(value.ToString());
                }
                
                // loop through all cells after the first one
                foreach (var cell in row.Skip(1))
                {
                    var tmp = new List<string>();

                    // loop through possible values for the cell
                    foreach (var value in cell.PossibleValues)
                    {                        

                        // append each of these cells possible values onto the already possible values
                        foreach (var possible in possibles)
                        {
                            tmp.Add($"{possible},{value}");
                        }
                    }

                    // set row possibles to the possibles with this cell
                    possibles = tmp.Select(x => x).ToList(); // this makes a copy
                }

                var rowPossibles = new List<string>();
                // loop through all row possibles
                foreach (var p in possibles)
                {
                    var arr = p.Split(',').ToList();

                    // duplicate numbers in this make this no longer a possibility
                    if (arr.Distinct().Count() != 9)
                    {
                        continue;
                    }

                    // add distinct possibles
                    rowPossibles.Add(p);
                }


                _rowPossibilities.Add(i, rowPossibles);
            }
        }

        private void DeterminePossiblesPerCell()
        {
            foreach (var cell in this._grid)
            {
                if (cell.Value.HasValue)
                {
                    cell.PossibleValues = new List<int> { cell.Value.Value };
                    continue;
                }

                // get values from cells in same row
                var inRow = this._grid.Where(x => x.Row == cell.Row && x.Column != cell.Column).Select(x => x.Value);

                // get values from cells in same column
                var inCol = this._grid.Where(x => x.Column == cell.Column && x.Row != cell.Row).Select(x => x.Value);

                // get values from cells in same box
                var inBox = this._grid.Where(x => x.Box == cell.Box && x.Row != cell.Row && x.Column != cell.Column).Select(x => x.Value);

                // get a distinct list of numbers that are already used
                var reservedValues = (inRow.Union(inCol).Union(inBox)).Distinct();

                // using the distinct list, get values this cell could be
                var possibles = _range.Where(x => !reservedValues.Contains(x)).ToList();
                cell.PossibleValues = possibles;
            }
        }

        private List<ICell> FindSolution()
        {
            var row1Count = _rowPossibilities[0].Count;
            var row2Count = _rowPossibilities[1].Count;
            var row3Count = _rowPossibilities[2].Count;
            var row4Count = _rowPossibilities[3].Count;
            var row5Count = _rowPossibilities[4].Count;
            var row6Count = _rowPossibilities[5].Count;
            var row7Count = _rowPossibilities[6].Count;
            var row8Count = _rowPossibilities[7].Count;
            var row9Count = _rowPossibilities[8].Count;

            // loop through row one
            for (var one = 0; one < row1Count; one++)
            {
                var row1Possible = _rowPossibilities[0][one];

                // row two
                for (var two = 0; two < row2Count; two++)
                {
                    var row2Possible = _rowPossibilities[1][two];

                    var gridTwo = new List<string[]> { row1Possible.Split(','), row2Possible.Split(',') };

                    if (!ValidateColumns(gridTwo))
                    {
                        continue;
                    }

                    // row three
                    for (var three = 0; three < row3Count; three++)
                    {
                        var row3Possible = _rowPossibilities[2][three];

                        var gridThree = gridTwo.ToList();
                        gridThree.Add(row3Possible.Split(','));

                        if (!ValidateColumns(gridThree))
                        {
                            continue;
                        }

                        // row four
                        for (var four = 0; four < row4Count; four++)
                        {
                            var row4Possible = _rowPossibilities[3][four];

                            var gridFour = gridThree.ToList();
                            gridFour.Add(row4Possible.Split(','));

                            if (!ValidateColumns(gridFour))
                            {
                                continue;
                            }

                            // row five
                            for (var five = 0; five < row5Count; five++)
                            {
                                var row5Possible = _rowPossibilities[4][five];

                                var gridFive = gridFour.ToList();
                                gridFive.Add(row5Possible.Split(','));

                                if (!ValidateColumns(gridFive))
                                {
                                    continue;
                                }

                                // row six
                                for (var six = 0; six < row6Count; six++)
                                {
                                    var row6Possible = _rowPossibilities[5][six];

                                    var gridSix = gridFive.ToList();
                                    gridSix.Add(row6Possible.Split(','));

                                    if (!ValidateColumns(gridSix))
                                    {
                                        continue;
                                    }

                                    // row seven
                                    for (var seven = 0; seven < row7Count; seven++)
                                    {
                                        var row7Possible = _rowPossibilities[6][seven];

                                        var gridSeven = gridSix.ToList();
                                        gridSeven.Add(row7Possible.Split(','));

                                        if (!ValidateColumns(gridSeven))
                                        {
                                            continue;
                                        }

                                        // row eight
                                        for (var eight = 0; eight < row8Count; eight++)
                                        {
                                            var row8Possible = _rowPossibilities[7][eight];

                                            var gridEight = gridSeven.ToList();
                                            gridEight.Add(row8Possible.Split(','));

                                            if (!ValidateColumns(gridEight))
                                            {
                                                continue;
                                            }

                                            // row nine
                                            for (var nine = 0; nine < row9Count; nine++)
                                            {
                                                var row9Possible = _rowPossibilities[8][nine];

                                                var gridNine = gridEight.ToList();
                                                gridNine.Add(row9Possible.Split(','));

                                                if (!ValidateColumns(gridNine))
                                                {
                                                    continue;
                                                }

                                                if (ValidateBoxes(gridNine))
                                                {
                                                    var finalGrid = GetRow(row1Possible, 0)
                                                        .Union(GetRow(row2Possible, 1))
                                                        .Union(GetRow(row3Possible, 2))
                                                        .Union(GetRow(row4Possible, 3))
                                                        .Union(GetRow(row5Possible, 4))
                                                        .Union(GetRow(row6Possible, 5))
                                                        .Union(GetRow(row7Possible, 6))
                                                        .Union(GetRow(row8Possible, 7))
                                                        .Union(GetRow(row9Possible, 8));

                                                    return finalGrid.Select(x => (ICell)x).ToList();
                                                        
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerable<Cell> GetRow(string rowPossibles, int rowInx)
        {
            var values = rowPossibles.Split(',');

            return values.Select(x => new Cell
            {
                Value = int.Parse(x),
                Row = rowInx,
                Column = Array.IndexOf(values, x),
                Box = GetBox(rowInx, Array.IndexOf(values, x))
            });
        }

        private bool ValidateColumns(List<string[]> grid)
        {
            var distinct = grid.Count;

            // check the columns
            for (var c = 0; c < 9; c++)
            {
                var column = grid.SelectMany(x => x[c]);

                // if all the values are distinct, the column is good
                if (column.Distinct().Count() != distinct) return false;

            }

            return true;
        }

        private bool ValidateBoxes(List<string[]> grid)
        {
            // figure out boxes
            var boxes = new string[] { "", "", "", "", "", "", "", "", "" };
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    var box = GetBox(r, c);
                    
                    boxes[box] += (string.IsNullOrEmpty(boxes[box]) ? grid[r][c] : $",{grid[r][c]}");
                }
            }

            // check the boxes
            for (var b = 0; b < 9; b++)
            {
                // if all the values are distinct, the column is good
                if (boxes[b].Split(',').Distinct().Count() != 9) return false;

            }

            // this is the answer
            return true;

        }

        private void PrepareCells()
        {
            for (var r = 0; r < 9; r++)
            {
                var rows = this._grid.Where(x => x.Row == r);

                for (var c = 0; c < 9; c++)
                {
                    var cell = rows.SingleOrDefault(x => x.Column == c);

                    if (cell == null)
                    {
                        cell = new Cell(r, c);

                        // add the cell to the collection
                        this._grid.Add(cell);
                    }

                    if (cell.Row < 0 || cell.Row > 8 || cell.Column < 0 || cell.Column > 8)
                    {
                        throw new ArgumentOutOfRangeException($"coordinates {cell.Row}, {cell.Column} are invalid");
                    }

                    cell.Box = GetBox(r, c);
                }
            }

        }

        private int GetBox(int row, int column)
        {
            if (row <= 2 && column <= 2)
            {
                return 0;
            }

            if (row <= 2 && column <= 5)
            {
                return 1;
            }

            // no need to check the column on this, it would just be in the last box
            if (row <= 2)
            {
                return 2;
            }

            if (row <= 5 && column <= 2)
            {
                return 3;
            }

            if (row <= 5 && column <= 5)
            {
                return 4;
            }

            // no need to check the column on this, it would just be in the last box
            if (row <= 5)
            {
                return 5;
            }

            // no longer need to check the row
            if (column <= 2)
            {
                return 6;
            }

            if (column <= 5)
            {
                return 7;
            }

            // last box
            return 8;
        }

    }
}