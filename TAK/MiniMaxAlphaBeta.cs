using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAK
{
    class MiniMaxAlphaBeta
    {
        private int rowSize { get; set; }
        private int colSize { get; set; }
        public bool isFirstTurn { get; set; }
        private char pion { get; set; }
        private char super { get; set; }
        private char stand { get; set; }
        private char ePion { get; set; }
        private char eSuper { get; set; }
        private char eStand { get; set; }
        public List<char>[,] result { get; set; }
        public string previewResult { get; set; }
        public bool status { get; set; }

        public MiniMaxAlphaBeta(int rowSize, int colSize, char pion)
        {
            this.rowSize = rowSize;
            this.colSize = colSize;
            this.isFirstTurn = false;
            this.pion = pion;
            this.super = pion == 'W' ? 'X' : 'Y';
            this.stand = pion == 'W' ? 'T' : 'U';
            this.ePion = pion == 'W' ? 'B' : 'W';
            this.eSuper = ePion == 'W' ? 'X' : 'Y';
            this.eStand = ePion == 'W' ? 'T' : 'U';
            this.result = new List<char>[rowSize, colSize];
            this.previewResult = "";
            this.status = false;
        }

        public int MiniMax(List<char>[,] prevBoard, int ply, bool isMax, int alpha, int beta, int pionLeft, int superLeft)
        {
            //if (!isFirstTurn && !isMax) isFirstTurn = true;
            
            List<char>[,] board = new List<char>[rowSize, colSize];
            for (int x = 0; x < rowSize * colSize; x++)
            {
                int nRow = x / rowSize;
                int nCol = x % colSize;
                board[nRow, nCol] = new List<char>(prevBoard[nRow, nCol]);
            }

            if (IsGameOver(board) != "" || ply == 0)
            {
                return GetSBE(board);
            }

            if (isMax)
            {
                int maxEval = int.MinValue;
                List<List<char>[,]> possibleMove = GetPossibleMove(board, true, pionLeft, superLeft);

                int idx = 0;
                for (int i = 0; i < possibleMove.Count; i++)
                {
                    bool ipm = isPionMove(board, possibleMove[i]);
                    int eval = MiniMax(possibleMove[i], ply - 1, false, alpha, beta, ipm ? pionLeft - 1 : pionLeft, ipm ? superLeft : superLeft - 1);

                    if (Math.Max(maxEval, eval) == eval) idx = i;

                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha) break;
                }

                string temp = "SBE : " + maxEval + "\n|";
                for (int j = 0; j < rowSize; j++)
                {
                    for (int k = 0; k < colSize; k++)
                    {
                        temp += possibleMove[idx][k, j][possibleMove[idx][k, j].Count - 1].ToString() + "|";
                    }
                    if (j < rowSize - 1) temp += "\n|";
                }
                previewResult = temp;

                result = new List<char>[rowSize, colSize];
                for (int x = 0; x < rowSize * colSize; x++)
                {
                    int nRow = x / rowSize;
                    int nCol = x % colSize;
                    result[nRow, nCol] = new List<char>(possibleMove[idx][nRow, nCol]);
                }

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                List<List<char>[,]> possibleMove = GetPossibleMove(board, false, pionLeft, superLeft);
                
                int idx = 0;
                for (int i = 0; i < possibleMove.Count; i++)
                {
                    bool ipm = isPionMove(board, possibleMove[i]);
                    int eval = MiniMax(possibleMove[i], ply - 1, true, alpha, beta, ipm ? pionLeft - 1 : pionLeft, ipm ? superLeft : superLeft - 1);

                    if (Math.Min(minEval, eval) == eval) idx = i;

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha) break;
                }

                string temp = "SBE : " + minEval + "\n|";
                for (int j = 0; j < rowSize; j++)
                {
                    for (int k = 0; k < colSize; k++)
                    {
                        temp += possibleMove[idx][k, j][possibleMove[idx][k, j].Count - 1].ToString() + "|";
                    }
                    if (j < rowSize - 1) temp += "\n|";
                }
                previewResult = temp;

                result = new List<char>[rowSize, colSize];
                for (int x = 0; x < rowSize * colSize; x++)
                {
                    int nRow = x / rowSize;
                    int nCol = x % colSize;
                    result[nRow, nCol] = new List<char>(possibleMove[idx][nRow, nCol]);
                }

                return minEval;
            }
        }

        private List<List<char>[,]> GetPossibleMove(List<char>[,] board, bool isAIMove, int pionLeft, int superLeft)
        {
            List<List<char>[,]> possibleMove = new List<List<char>[,]>();

            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;

                char b = board[row, col][board[row, col].Count - 1];

                if (b == ' ')
                {
                    if (pionLeft > 0)
                    {
                        // Possible move untuk pion
                        List<char>[,] newBoard = new List<char>[rowSize, colSize];
                        for (int x = 0; x < rowSize * colSize; x++)
                        {
                            int nRow = x / rowSize;
                            int nCol = x % colSize;
                            newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                        }
                        newBoard[row, col].Add(isAIMove ? pion : ePion);
                        possibleMove.Add(newBoard);

                        if (!isFirstTurn)
                        {
                            // Possible move untuk stand
                            newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col].Add(isAIMove ? stand : eStand);
                            possibleMove.Add(newBoard);
                        }

                    }

                    if (superLeft > 0 && !isFirstTurn)
                    {
                        // Possible move untuk super
                        List<char>[,] newBoard = new List<char>[rowSize, colSize];
                        for (int x = 0; x < rowSize * colSize; x++)
                        {
                            int nRow = x / rowSize;
                            int nCol = x % colSize;
                            newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                        }
                        newBoard[row, col].Add(isAIMove ? super : eSuper);
                        possibleMove.Add(newBoard);
                        
                        
                        // string hehe = "";
                        // for (int j = 0; j < rowSize; j++)
                        // {
                        //     for (int k = 0; k < colSize; k++)
                        //     {
                        //         hehe += newBoard[j, k][newBoard[j, k].Count - 1].ToString() + "|";
                        //     }
                        //     hehe += "\n";
                        // }
                        // System.Windows.Forms.MessageBox.Show(hehe);
                    }
                }
                else if (b == (isAIMove ? pion : ePion) || b == (isAIMove ? stand : eStand) || b == (isAIMove ? super : eSuper))
                {
                    // Possible move untuk stack
                    List<char> stack = board[row, col];
                    int stackCount = board[row, col].Count;
                    List<List<int>> partition = Partition(stackCount);

                    int left = row;
                    int right = (rowSize - 1) - row;
                    int top = col;
                    int bottom = (colSize - 1) - col;

                    for (int j = 0; j < partition.Count; j++)
                    {
                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= left)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 1; k <= partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k - 1]; l++)
                                {
                                    char topChar = newBoard[row - k, col][newBoard[row - k, col].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row - k, col][newBoard[row - k, col].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row - k, col].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= right)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 1; k <= partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k - 1]; l++)
                                {
                                    char topChar = newBoard[row + k, col][newBoard[row + k, col].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row + k, col][newBoard[row + k, col].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row + k, col].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= top)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 1; k <= partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k - 1]; l++)
                                {
                                    char topChar = newBoard[row, col - k][newBoard[row, col - k].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row, col - k][newBoard[row, col - k].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row, col - k].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= bottom)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 1; k <= partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k - 1]; l++)
                                {
                                    char topChar = newBoard[row, col + k][newBoard[row, col + k].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row, col + k][newBoard[row, col + k].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row, col + k].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        /*stackCount = board[row, col].Count;
                        if (partition[j].Count <= left + 1)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 0; k < partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k]; l++)
                                {
                                    char topChar = newBoard[row - k, col][newBoard[row - k, col].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row - k, col][newBoard[row - k, col].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row - k, col].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= right + 1)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 0; k < partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k]; l++)
                                {
                                    char topChar = newBoard[row + k, col][newBoard[row + k, col].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row + k, col][newBoard[row + k, col].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row + k, col].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= top + 1)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 0; k < partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k]; l++)
                                {
                                    char topChar = newBoard[row, col - k][newBoard[row, col - k].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row, col - k][newBoard[row, col - k].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row, col - k].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }

                        stackCount = board[row, col].Count;
                        if (partition[j].Count <= bottom + 1)
                        {
                            List<char>[,] newBoard = new List<char>[rowSize, colSize];
                            for (int x = 0; x < rowSize * colSize; x++)
                            {
                                int nRow = x / rowSize;
                                int nCol = x % colSize;
                                newBoard[nRow, nCol] = new List<char>(board[nRow, nCol]);
                            }
                            newBoard[row, col] = new List<char>();
                            newBoard[row, col].Add(' ');

                            for (int k = 0; k < partition[j].Count; k++)
                            {
                                bool valid = true;
                                for (int l = 0; l < partition[j][k]; l++)
                                {
                                    char topChar = newBoard[row, col + k][newBoard[row, col + k].Count - 1];
                                    char currChar = stack[stack.Count - stackCount];
                                    if (topChar != super &&
                                        topChar != eSuper &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != stand) &&
                                        (currChar == (isAIMove ? super : eSuper) || topChar != eStand))
                                    {
                                        if (currChar == (isAIMove ? super : eSuper) && (topChar == stand || topChar == eStand))
                                        {
                                            newBoard[row, col + k][newBoard[row, col + k].Count - 1] = topChar == 'T' ? 'W' : 'B';
                                        }
                                        newBoard[row, col + k].Add(stack[stack.Count - stackCount]);
                                        stackCount--;
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                                if (valid) possibleMove.Add(newBoard);
                            }
                        }*/
                    }
                }
            }

            return possibleMove;
        }

        private bool isPionMove(List<char>[,] prev, List<char>[,] pos)
        {
            int prevSuper = 0;
            int prevESuper = 0;
            int super = 0;
            int eSuper = 0;

            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;

                char pr = prev[row, col][prev[row, col].Count - 1];
                if (pr == super) prevSuper = 1;
                else if (pr == eSuper) prevESuper = 1;

                char po = pos[row, col][pos[row, col].Count - 1];
                if (po == super) super = 1;
                else if (po == eSuper) eSuper = 1;
            }

            if (prevSuper == super && prevESuper == eSuper) return true;

            return false; 
        }

        private List<List<int>> Partition(int stackSize)
        {
            List<List<int>> result = new List<List<int>>();

            if (stackSize <= 0)
                return result;

            if (stackSize == 1)
            {
                List<int> stacks = new List<int> { 1 };
                result.Add(stacks);
                return result;
            }

            for (int i = 1; i < stackSize; i++)
            {
                int j = stackSize - i;
                List<int> p = new List<int>();
                List<List<int>> part = Partition(j);

                foreach (var partition in part)
                {
                    p = new List<int>(partition);
                    p.Insert(0, i);
                    result.Add(p);
                }
            }

            List<int> mulStacks = new List<int> { stackSize };
            result.Add(mulStacks);
            return result;
        }

        private int GetSBE(List<char>[,] board)
        {
            int flatCount = CalculateFlatCount(board);
            int control = CalculateControl(board);
            int stackHeight = CalculateStackHeight(board);
            int captures = CalculateCaptures(board);
            int roadProgress = CalculateRoadProgress(board);
            //int superValue = CalculateSuperValue(board);
            //int standValue = CalculateStandValue(board);

            int sbe = 10 * flatCount + 1 * control + 1 * stackHeight + 7 * captures + 15 * roadProgress;
            if(status == true)
            {
                Console.WriteLine("flatcount: " + flatCount);
                Console.WriteLine("control: " + control);
                Console.WriteLine("stackheight: " + stackHeight);
                Console.WriteLine("captures: " + captures);
                Console.WriteLine("roadprogress: " + roadProgress);
            }

            if (IsWin(board, true)) sbe += 500;
            if (IsWin(board, false)) sbe -= 1000;

            return sbe;
        }

        private int CalculateFlatCount(List<char>[,] board)
        {
            int counter = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col][board[row, col].Count - 1] == pion) counter++;
                /*if (board[row, col][board[row, col].Count - 1] == stand) counter++;
                if (board[row, col][board[row, col].Count - 1] == super) counter+=5;*/
            }
            return counter;
        }

        private int CalculateControl(List<char>[,] board)
        {
            int counter = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col][board[row, col].Count - 1] == pion) counter++;
                else if (board[row, col][board[row, col].Count - 1] == stand) counter++;
                else if (board[row, col][board[row, col].Count - 1] == super) counter++;
            }
            int boardSize = rowSize * colSize;
            int result = Convert.ToInt32((counter * 10) / boardSize);
            return result;
        }

        private int CalculateStackHeight(List<char>[,] board)
        {
            int totalStack = 0;
            int totalEmpty = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                char b = board[row, col][board[row, col].Count - 1];
                if (b == pion || b == stand || b == super) totalStack += board[row, col].Count;
                else if (b == ' ') totalEmpty++;
            }
            int result = totalEmpty > 0 ? Convert.ToInt32((totalStack * 5) / totalEmpty) : 0;
            return result;
        }

        private int CalculateCaptures(List<char>[,] board)
        {
            int counter = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                char b = board[row, col][board[row, col].Count - 1];
                if (b == ePion || b == eStand || b == eSuper) counter--;
            }
            return counter;
        }

        private int CalculateRoadProgress(List<char>[,] board)
        {
            int progress = 0;

            for (int col = 0; col < colSize; col++)
            {
                char b = board[0, col][board[0, col].Count - 1];
                if (b == pion || b == super)
                {
                    progress += DFS(board, 0, col, new bool[rowSize, colSize], true);
                    progress += DFS(board, 4, col, new bool[rowSize, colSize], true);
                }
                else if (b == ePion || b == eSuper)
                {
                    int e1 = DFS(board, 0, col, new bool[rowSize, colSize], false);
                    int e2 = DFS(board, 4, col, new bool[rowSize, colSize], false);
                    progress -= ((e1 * 3) + (e2 * 3));
                    /*Console.WriteLine("e1: " + e1);
                    Console.WriteLine("e2: " + e2);*/
                    //progress -= e1 > 2 ? e1 * 4 : e1 * 2;
                    //progress -= e2 > 2 ? e2 * 4 : e2 * 2;
                }
            }

            return progress;
        }

        private int DFS(List<char>[,] board, int row, int col, bool[,] visited, bool isCheckingAI)
        {
            if (row < 0 || row >= rowSize || col < 0 || col >= colSize || visited[row, col]) return 0;

            char b = board[row, col][board[row, col].Count - 1];
            if (isCheckingAI && b != pion && b != super) return 0;
            else if (!isCheckingAI && b != ePion && b != eSuper) return 0;

            visited[row, col] = true;
            int connectedPieces = 1;

            connectedPieces += DFS(board, row + 1, col, visited, isCheckingAI);
            connectedPieces += DFS(board, row - 1, col, visited, isCheckingAI);
            connectedPieces += DFS(board, row, col + 1, visited, isCheckingAI);
            connectedPieces += DFS(board, row, col - 1, visited, isCheckingAI);

            return connectedPieces;
        }

        private int CalculateSuperValue(List<char>[,] board)
        {
            int counter = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col][board[row, col].Count - 1] == super)
                {
                    if (row > 0)
                    {
                        if (board[row - 1, col][board[row - 1, col].Count - 1] == ePion) counter--;
                        else if (board[row - 1, col][board[row - 1, col].Count - 1] == pion) counter++;
                    }
                    if (row < rowSize - 1)
                    {
                        if (board[row + 1, col][board[row + 1, col].Count - 1] == ePion) counter--;
                        else if (board[row + 1, col][board[row + 1, col].Count - 1] == pion) counter++;
                    }
                    if (col > 0)
                    {
                        if (board[row, col - 1][board[row, col - 1].Count - 1] == ePion) counter--;
                        else if (board[row, col - 1][board[row, col - 1].Count - 1] == pion) counter++;
                    }
                    if (col < colSize - 1)
                    {
                        if (board[row, col + 1][board[row, col + 1].Count - 1] == ePion) counter--;
                        else if (board[row, col + 1][board[row, col + 1].Count - 1] == pion) counter++;
                    }
                }
            }
            int boardSize = rowSize * colSize;
            int result = Convert.ToInt32((counter * 10) / boardSize);
            return result;
        }
        
        private int CalculateStandValue(List<char>[,] board)
        {
            int counter = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col][board[row, col].Count - 1] == stand)
                {
                    if (row > 0)
                    {
                        if (board[row - 1, col][board[row - 1, col].Count - 1] == ePion) counter++;
                    }
                    if (row < rowSize - 1)
                    {
                        if (board[row + 1, col][board[row + 1, col].Count - 1] == ePion) counter++;
                    }
                    if (col > 0)
                    {
                        if (board[row, col - 1][board[row, col - 1].Count - 1] == ePion) counter++;
                    }
                    if (col < colSize - 1)
                    {
                        if (board[row, col + 1][board[row, col + 1].Count - 1] == ePion) counter++;
                    }
                }
            }
            int boardSize = rowSize * colSize;
            int result = Convert.ToInt32((counter * 10) / boardSize);
            return result;
        }

        private string IsGameOver(List<char>[,] board)
        {
            // Cek board kalo ada yang warnanya ujung ke ujung
            List<string> deadMove;

            for (int i = 0; i < colSize; i++)
            {
                if (board[i, 0].Count > 1)
                {
                    deadMove = new List<string>();
                    Stack<string> moveXY = new Stack<string>();

                    char curr = board[i, 0][board[i, 0].Count - 1];
                    char next = board[i, 1][board[i, 1].Count - 1];

                    int currX = i;
                    int currY = 1;
                    int defX = i;
                    int defY = 0;
                    bool isRolledBack = false;
                    moveXY.Push(i + ",0");
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kebawah
                        if ((((curr == 'W' || curr == 'X') && (next == 'W' || next == 'X')) || ((curr == 'B' || curr == 'Y') && (next == 'B' || next == 'Y'))) && !isRolledBack)
                        {
                            if (currY == colSize - 1) return curr == pion || curr == super ? "AI" : "Player";

                            if (!moveXY.Contains(currX + "," + currY))
                            {
                                moveXY.Push(currX + "," + currY);
                                defX = currX;
                                defY = currY;
                                curr = next;
                                currY++;
                                next = board[currX, currY][board[currX, currY].Count - 1];
                                continue;
                            }
                        }

                        // Kalo tidak sama, jadikan sebagai deadmove
                        deadMove.Add(currX + "," + currY);
                        currX = defX;
                        currY = defY;
                        isRolledBack = false;

                        // Cek jika next ke kanan
                        if (currX < colSize - 1 && !deadMove.Contains((currX + 1) + "," + currY) && !moveXY.Contains((currX + 1) + "," + currY))
                        {
                            currX++;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke kiri
                        if (currX > 0 && !deadMove.Contains((currX - 1) + "," + currY) && !moveXY.Contains((currX - 1) + "," + currY))
                        {
                            currX--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke atas
                        if (currY > 0 && !deadMove.Contains(currX + "," + (currY - 1)) && !moveXY.Contains(currX + "," + (currY - 1)))
                        {
                            currY--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        string rollback = moveXY.Pop();
                        if (moveXY.Count == 0) break;
                        defX = Convert.ToInt32(rollback.Split(',')[0]);
                        defY = Convert.ToInt32(rollback.Split(',')[1]);
                        next = curr;
                        curr = board[defX, defY][board[defX, defY].Count - 1];
                        isRolledBack = true;
                    }
                }
            }

            for (int i = 0; i < rowSize; i++)
            {
                if (board[0, i].Count > 1)
                {
                    deadMove = new List<string>();
                    Stack<string> moveXY = new Stack<string>();

                    char curr = board[0, i][board[0, i].Count - 1];
                    char next = board[1, i][board[1, i].Count - 1];

                    int currX = 1;
                    int currY = i;
                    int defX = 0;
                    int defY = i;
                    bool isRolledBack = false;
                    moveXY.Push("0," + i);
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kanan
                        if ((((curr == 'W' || curr == 'X') && (next == 'W' || next == 'X')) || ((curr == 'B' || curr == 'Y') && (next == 'B' || next == 'Y'))) && !isRolledBack)
                        {
                            if (currX == rowSize - 1) return curr == pion || curr == super ? "AI" : "Player";

                            if (!moveXY.Contains(currX + "," + currY))
                            {
                                moveXY.Push(currX + "," + currY);
                                defX = currX;
                                defY = currY;
                                curr = next;
                                currX++;
                                next = board[currX, currY][board[currX, currY].Count - 1];
                                continue;
                            }
                        }

                        // Kalo tidak sama, jadikan sebagai deadmove
                        deadMove.Add(currX + "," + currY);
                        currX = defX;
                        currY = defY;
                        isRolledBack = false;

                        // Cek jika next ke atas
                        if (currY < rowSize - 1 && !deadMove.Contains(currX + "," + (currY + 1)) && !moveXY.Contains(currX + "," + (currY + 1)))
                        {
                            currY++;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke bawah
                        if (currY > 0 && !deadMove.Contains(currX + "," + (currY - 1)) && !moveXY.Contains(currX + "," + (currY - 1)))
                        {
                            currY--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke kiri
                        if (currX > 0 && !deadMove.Contains((currX - 1) + "," + currY) && !moveXY.Contains((currX - 1) + "," + currY))
                        {
                            currX--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        string rollback = moveXY.Pop();
                        if (moveXY.Count == 0) break;
                        defX = Convert.ToInt32(rollback.Split(',')[0]);
                        defY = Convert.ToInt32(rollback.Split(',')[1]);
                        next = curr;
                        curr = board[defX, defY][board[defX, defY].Count - 1];
                        isRolledBack = true;
                    }
                }
            }

            return "";
        }

        private bool IsWin(List<char>[,] board, bool isAIWin)
        {
            // Cek board kalo ada yang warnanya ujung ke ujung
            List<string> deadMove;

            for (int i = 0; i < colSize; i++)
            {
                if (board[i, 0].Count > 1)
                {

                    //if (isAIWin && curr != pion && curr != super) continue;
                    //else if (!isAIWin && curr != ePion && curr != eSuper) continue;

                    deadMove = new List<string>();
                    Stack<string> moveXY = new Stack<string>();

                    char curr = board[i, 0][board[i, 0].Count - 1];
                    char next = board[i, 1][board[i, 1].Count - 1];

                    int currX = i;
                    int currY = 1;
                    int defX = i;
                    int defY = 0;
                    bool isRolledBack = false;
                    moveXY.Push(i + ",0");
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kebawah
                        if ((((curr == pion || curr == super) && (next == pion || next == super)) || ((curr == ePion || curr == eSuper) && (next == ePion || next == eSuper))) && !isRolledBack)
                        {
                            if (currY == colSize - 1)
                            {
                                if ((isAIWin && (curr == pion || curr == super)) || (!isAIWin && (curr == ePion || curr == eSuper)))
                                {
                                    string temp = "|";
                                    for (int j = 0; j < rowSize; j++)
                                    {
                                        for (int k = 0; k < colSize; k++)
                                        {
                                            temp += board[k, j][board[k, j].Count - 1].ToString() + "|";
                                        }
                                        if (j < rowSize - 1) temp += "\n|";
                                    }
                                    //System.Windows.Forms.MessageBox.Show(temp);
                                    return true;
                                }
                                else break;
                            }

                            if (!moveXY.Contains(currX + "," + currY))
                            {
                                moveXY.Push(currX + "," + currY);
                                defX = currX;
                                defY = currY;
                                curr = next;
                                currY++;
                                next = board[currX, currY][board[currX, currY].Count - 1];
                                continue;
                            }
                        }

                        // Kalo tidak sama, jadikan sebagai deadmove
                        deadMove.Add(currX + "," + currY);
                        currX = defX;
                        currY = defY;
                        isRolledBack = false;

                        // Cek jika next ke kanan
                        if (currX < colSize - 1 && !deadMove.Contains((currX + 1) + "," + currY) && !moveXY.Contains((currX + 1) + "," + currY))
                        {
                            currX++;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke kiri
                        if (currX > 0 && !deadMove.Contains((currX - 1) + "," + currY) && !moveXY.Contains((currX - 1) + "," + currY))
                        {
                            currX--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke atas
                        if (currY > 0 && !deadMove.Contains(currX + "," + (currY - 1)) && !moveXY.Contains(currX + "," + (currY - 1)))
                        {
                            currY--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        string rollback = moveXY.Pop();
                        if (moveXY.Count == 0) break;
                        defX = Convert.ToInt32(rollback.Split(',')[0]);
                        defY = Convert.ToInt32(rollback.Split(',')[1]);
                        next = curr;
                        curr = board[defX, defY][board[defX, defY].Count - 1];
                        isRolledBack = true;
                    }
                }
            }

            for (int i = 0; i < rowSize; i++)
            {
                if (board[0, i].Count > 1)
                {

                    //if (isAIWin && curr != pion && curr != super) continue;
                    //else if (!isAIWin && curr != ePion && curr != eSuper) continue;

                    deadMove = new List<string>();
                    Stack<string> moveXY = new Stack<string>();

                    char curr = board[0, i][board[0, i].Count - 1];
                    char next = board[1, i][board[1, i].Count - 1];

                    int currX = 1;
                    int currY = i;
                    int defX = 0;
                    int defY = i;
                    bool isRolledBack = false;
                    moveXY.Push("0," + i);
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kanan
                        if ((((curr == pion || curr == super) && (next == pion || next == super)) || ((curr == ePion || curr == eSuper) && (next == ePion || next == eSuper))) && !isRolledBack)
                        {
                            if (currX == rowSize - 1)
                            {
                                if ((isAIWin && (curr == pion || curr == super)) || (!isAIWin && (curr == ePion || curr == eSuper)))
                                {
                                    string temp = "|";
                                    for (int j = 0; j < rowSize; j++)
                                    {
                                        for (int k = 0; k < colSize; k++)
                                        {
                                            temp += board[k, j][board[k, j].Count - 1].ToString() + "|";
                                        }
                                        if (j < rowSize - 1) temp += "\n|";
                                    }
                                    //System.Windows.Forms.MessageBox.Show(temp);
                                    return true;
                                }
                                else break;
                            }

                            if (!moveXY.Contains(currX + "," + currY))
                            {
                                moveXY.Push(currX + "," + currY);
                                defX = currX;
                                defY = currY;
                                curr = next;
                                currX++;
                                next = board[currX, currY][board[currX, currY].Count - 1];
                                continue;
                            }
                        }

                        // Kalo tidak sama, jadikan sebagai deadmove
                        deadMove.Add(currX + "," + currY);
                        currX = defX;
                        currY = defY;
                        isRolledBack = false;

                        // Cek jika next ke atas
                        if (currY < rowSize - 1 && !deadMove.Contains(currX + "," + (currY + 1)) && !moveXY.Contains(currX + "," + (currY + 1)))
                        {
                            currY++;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke bawah
                        if (currY > 0 && !deadMove.Contains(currX + "," + (currY - 1)) && !moveXY.Contains(currX + "," + (currY - 1)))
                        {
                            currY--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        // Cek jika next ke kiri
                        if (currX > 0 && !deadMove.Contains((currX - 1) + "," + currY) && !moveXY.Contains((currX - 1) + "," + currY))
                        {
                            currX--;
                            next = board[currX, currY][board[currX, currY].Count - 1];
                            continue;
                        }

                        string rollback = moveXY.Pop();
                        if (moveXY.Count == 0) break;
                        defX = Convert.ToInt32(rollback.Split(',')[0]);
                        defY = Convert.ToInt32(rollback.Split(',')[1]);
                        next = curr;
                        curr = board[defX, defY][board[defX, defY].Count - 1];
                        isRolledBack = true;
                    }
                }
            }

            return false;
        }

        public Stack<string> GetAIMove(List<char>[,] board)
        {
            this.status = true;
            this.GetSBE(result);
            this.status = false;
            Stack<string> moveAI = new Stack<string>();

            int ctr = 0;
            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col].Count < result[row, col].Count && board[row, col][board[row, col].Count - 1] == ' ') ctr++;
                else if (board[row, col][board[row, col].Count - 1] != ' ' && result[row, col][result[row, col].Count - 1] == ' ') ctr++;
            }

            for (int i = 0; i < rowSize * colSize; i++)
            {
                int row = i / rowSize;
                int col = i % colSize;
                if (board[row, col].Count < result[row, col].Count && board[row, col][board[row, col].Count - 1] == ' ' && ctr == 1)
                {
                    moveAI.Push(row + "," + col + "," + result[row, col][result[row, col].Count - 1]);
                }
                else if (board[row, col][board[row, col].Count - 1] != ' ' && result[row, col][result[row, col].Count - 1] == ' ')
                {
                    Stack<string> temp = new Stack<string>();
                    temp.Push(row + "," + col);
                    int stackCount = board[row, col].Count;
                    bool pathFound = false;

                    if (row > 0)
                    {
                        int up = row;
                        while (true)
                        {
                            up--;
                            if (board[up, col].Count != result[up, col].Count)
                            {
                                int diff = Math.Abs(board[up, col].Count - result[up, col].Count);
                                for (int j = 0; j < diff; j++)
                                {
                                    //if (result[up, col][result[up, col].Count - (diff - j + 1)] != ' ')
                                        temp.Push(up + "," + col);
                                }
                                pathFound = true;
                            }
                            else break;

                            if (up == 0) break;
                        }
                    }
                    if (row < rowSize - 1 && !pathFound)
                    {
                        int down = row;
                        while (true)
                        {
                            down++;
                            if (board[down, col].Count != result[down, col].Count)
                            {
                                int diff = Math.Abs(board[down, col].Count - result[down, col].Count);
                                for (int j = 0; j < diff; j++)
                                {
                                    //if (result[down, col][result[down, col].Count - (diff - j + 1)] != ' ')
                                        temp.Push(down + "," + col);
                                }
                                pathFound = true;
                            }
                            else break;

                            if (down == rowSize - 1) break;
                        }
                    }
                    if (col > 0 && !pathFound)
                    {
                        int left = col;
                        while (true)
                        {
                            left--;
                            if (board[row, left].Count != result[row, left].Count)
                            {
                                int diff = Math.Abs(board[row, left].Count - result[row, left].Count);
                                for (int j = 0; j < diff; j++)
                                {
                                    //if (result[row, left][result[row, left].Count - (diff - j + 1)] != ' ')
                                        temp.Push(row + "," + left);
                                }
                                pathFound = true;
                            }
                            else break;

                            if (left == 0) break;
                        }
                    }
                    if (col < colSize - 1 && !pathFound)
                    {
                        int right = col;
                        while (true)
                        {
                            right++;
                            if (board[row, right].Count != result[row, right].Count)
                            {
                                int diff = Math.Abs(board[row, right].Count - result[row, right].Count);
                                for (int j = 0; j < diff; j++)
                                {
                                    //if (result[row, right][result[row, right].Count - (diff - j + 1)] != ' ')
                                        temp.Push(row + "," + right);
                                }
                                pathFound = true;
                            }
                            else break;

                            if (right == colSize - 1) break;
                        }
                    }

                    while (temp.Count > 0) {
                        moveAI.Push(temp.Pop());
                    }
                }
            }
            
            return moveAI;
        }
    }
}
