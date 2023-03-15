using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Enumeration for chip states
/// </summary>
public enum ChipType
{
    Empty = 0,
    Player = 1,
    Computer = 2
}

/// <summary>
/// The Connect Four engine.
/// All the game logic occurs here
/// </summary>
public class ConnectFourLogic
{
    #region Constants
    public const int COLUMN_COUNT = 7;
    public const int ROW_COUNT = 6;
    #endregion

    #region Public Properties
    public List<List<ChipType>> BoardMatrix  = new List<List<ChipType>>();
    #endregion

    #region Constructor
    public ConnectFourLogic()
    {
        InitialzeBoardMatrix();
    }
    #endregion

    #region Private Methods
    private void InitialzeBoardMatrix()
    {
        for (int c = 0; c < COLUMN_COUNT; c++)
        {
            BoardMatrix.Add(new List<ChipType>());
            for (int r = 0; r < ROW_COUNT; r++)
            {
                BoardMatrix[c].Add(ChipType.Empty);
            }
        }
    }
    
    /// <summary>
    /// Attempts to get the total score of the provided cell coordinates.
    /// Scans horizontally, vertically, and diagonally tallying the chips to get a score
    /// for the player and the computer. The total score is the sum of both to account for the player's
    /// best move but also to prevent the computer from making its best move
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private int GetCellScore(int col,int row)
    {
        var playerScore = 0;
        var computerScore = 0;
        
        var index = 0;
        while (index <= 3)
        {
            //horizontal
            playerScore += GetScore(new[] {col - index, row}, new[] {col - index + 3, row }, ChipType.Player);
            computerScore += GetScore(new[] {col - index, row}, new[] {col - index + 3, row }, ChipType.Computer);
            //ascending L-R
            playerScore += GetScore(new [] {col - index, row - index}, new[] {col - index + 3, row - index + 3}, ChipType.Player );
            computerScore += GetScore(new [] {col - index, row - index}, new[] {col - index + 3, row - index + 3}, ChipType.Computer );
            //desccending L-r
            playerScore += GetScore(new [] {col - index, row + index}, new[] {col - index + 3, row + index - 3}, ChipType.Player);
            computerScore += GetScore(new [] {col - index, row + index}, new[] {col - index + 3, row + index - 3}, ChipType.Computer);
            //down
            playerScore += GetScore(new[] {col, row + index}, new[] {col, row + index - 3}, ChipType.Player);
            computerScore += GetScore(new[] {col, row + index}, new[] {col, row + index - 3}, ChipType.Computer);
            index++;
        }
        
        int total = playerScore + computerScore;
        //Debug.Log("***************Column " + col + " Player Score: " + playerScore + " computer score: " + computerScore + " total: " + total);
        return total;
    }

    /// <summary>
    /// Gets the score for a starting and ending cell
    /// </summary>
    /// <param name="startCell"></param>
    /// <param name="endCell"></param>
    /// <param name="forChipType"></param>
    /// <returns></returns>
    private int GetScore(int[] startCell, int[] endCell, ChipType forChipType)
    {
        var startCol = startCell[0];
        var startRow = startCell[1];
        var endCol = endCell[0];
        var endRow = endCell[1];
        
        // bounds check
        if (startRow < 0 
            || startRow >= ROW_COUNT
            || endRow < 0
            || endRow >= ROW_COUNT
            || startCol < 0 
            || startCol >= COLUMN_COUNT
            || endCol < 0
            || endCol >= COLUMN_COUNT
            )
        { 
            // Debug.Log("Out of Range: " + startCol + ":" + startRow + "   " + endCol + ":" + endRow);
            return 0;
        }

        var score = 0;
        var c = startCol;
        var r = startRow;

        var dx = Math.Abs(endCol - startCol);
        var dy = Math.Abs(endRow - startRow);
        var sx = (startCol < endCol) ? 1 : -1;
        var sy = (startRow < endRow) ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            var chip = BoardMatrix[c][r];
            // Debug.Log("chip at: " + c + " " + r + " is " + chip + " for: " + forChipType);
            if (chip == forChipType)
            {
                score += 3;
            }
            else if (chip == ChipType.Empty)
            {
                score += 1;
            }
            else
            {
                return 0;
            }

            // Multiply score by a large amount for 3 chips
            if (score >= 9) 
            {
                if (chip == ChipType.Player)
                {
                    score *= 100;
                }
                else if (chip == ChipType.Computer)
                {
                    score *= 50;
                }
            }

            // Multiply score by a modest amount for 2 chips
            if (score >= 6)
            {
                if (chip == ChipType.Player)
                {
                    score *= 25;
                }
                else if (chip == ChipType.Computer)
                {
                    score *= 10;
                }
            }

            // get coordinates for next cell
            if (c == endCol && (r == endRow))
            {
                break;
            }

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                c += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                r += sy;
            }
        }
        
        // Debug.Log("CHIP: " + forChipType +" StartCell: " + startCol + ":" + startRow + " EndCell: " + endCol + ":" + endRow + " Score: " + score);
        return score;
    }
    #endregion

    #region Public Methods
    public void ResetBoardMatrix()
    {
        for (int c = 0; c < COLUMN_COUNT; c++)
        {
            for (int r = 0; r < ROW_COUNT; r++)
            {
                BoardMatrix[c][r] = ChipType.Empty;
            }
        }
    }

    public bool IsCellEmpty(int column, int row)
    {
        return BoardMatrix[column][row] == ChipType.Empty;
    }

    public void SetChipType(int column, int row, ChipType type)
    {
        BoardMatrix[column][row] = type;
    }

    public bool IsColumnFull(int column)
    {
        foreach (var chip in BoardMatrix[column])
        {
            if (chip == ChipType.Empty)
            {
                return false;
            }
        }
        return true;
    }

    public int FindRandomColumn()
    {
        while (true)
        {
            var randomColumn = Random.Range(0, ConnectFourLogic.COLUMN_COUNT);
            if (!IsColumnFull(randomColumn))
            {
                return randomColumn;
            }
        }
    }
    
    /// <summary>
    /// Checks to see if there is a winning combination
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CheckForWin(ChipType type)
    {
        /** Could be optimized by checking around the last chip drop instead of scanning the whole board**/

        // vertical
        for (int r = 0; r < ROW_COUNT - 3; r++ ){
            for (int c = 0; c < COLUMN_COUNT; c++){
                if (BoardMatrix[c][r] == type 
                    && BoardMatrix[c][r+1] == type 
                    && BoardMatrix[c][r+2] == type 
                    && BoardMatrix[c][r+3] == type)
                {
                    return true;
                }           
            }
        }
        
        // horizontal
        for (int c = 0; c < COLUMN_COUNT - 3; c++ ){
            for (int r = 0; r < ROW_COUNT; r++){
                if (BoardMatrix[c][r] == type 
                    && BoardMatrix[c+1][r] == type 
                    && BoardMatrix[c+2][r] == type 
                    && BoardMatrix[c+3][r] == type)
                {
                    return true;
                }           
            }
        }
        
        // ascending diagonal
        for (int c = 3; c<COLUMN_COUNT; c++){
            for (int r = 0; r<ROW_COUNT - 3; r++){
                if (BoardMatrix[c][r] == type 
                    && BoardMatrix[c-1][r+1] == type 
                    && BoardMatrix[c-2][r+2] == type 
                    && BoardMatrix[c-3][r+3] == type)
                    return true;
            }
        }
        
        // descending diagonal
        for (int i = 3; i<COLUMN_COUNT; i++){
            for (int j = 3; j<ROW_COUNT; j++){
                if (BoardMatrix[i][j] == type 
                    && BoardMatrix[i-1][j-1] == type 
                    && BoardMatrix[i-2][j-2] == type 
                    && BoardMatrix[i-3][j-3] == type)
                    return true;
            }
        }
        
        return false;
    }

    public bool IsGameOver()
    {
        for (int c = 0; c < COLUMN_COUNT; c++)
        {
            if (!IsColumnFull(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Find the best column(s) to play for the next user move based on the column score
    /// 
    /// Helper functions:
    ///     IsColumnFull(int column) returns bool
    ///     IsCellEmpty(int column, int row) returns bool
    ///     GetCellScore(int column, int row) returns int
    ///     
    /// </summary>
    /// <returns></returns>
    public List<int> BestColumns()
    {
        return new List<int>();
    }
    #endregion
}
