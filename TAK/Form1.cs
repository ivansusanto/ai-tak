using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TAK.Properties;

namespace TAK
{
    public partial class TAK : Form
    {
        int rowSize = 5;
        int colSize = 5;
        int startX = 217;
        int startY = 100;
        bool isPlayerFirst;
        List<Button>[,] board;

        bool isWhiteTurn;
        bool isPion;
        int counterFirstTurn;
        
        List<Button> selected;
        int selectedCount;
        MiniMaxAlphaBeta Tree;

        int stackRow;
        int stackCol;
        string possibleStackMove;

        public TAK()
        {
            InitializeComponent();
            InitForm();
        }

        public TAK(TAK t)
        {
            InitializeComponent();
            InitForm();
            t.Hide();
        }

        private void InitForm()
        {
            isWhiteTurn = true;
            isPion = true;
            counterFirstTurn = 1;
            isPlayerFirst = true;

            selected = new List<Button>();
            selectedCount = 0;
            stackRow = 0;
            stackCol = 0;
            possibleStackMove = "";

            board = new List<Button>[rowSize, colSize];

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < colSize; j++)
                {
                    board[i, j] = new List<Button>();
                }
            }

            pionWhite.Enabled = false;
            superWhite.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int row = 0; row < rowSize; row++)
            {
                for (int col = 0; col < colSize; col++)
                {
                    Button btn = new Button();
                    btn.Width = 75;
                    btn.Height = 75;

                    btn.Location = new Point(startX + (row * 74), startY + (col * 74));
                    btn.Font = new Font("Microsoft Sans Serif", 20.25f, FontStyle.Regular);
                    btn.BackColor = Color.White;

                    btn.Click += Button_Click;

                    board[row, col].Add(btn);
                    this.Controls.Add(btn);
                }

            }

            for (int i = 0; i < rowSize; i++)
            {
                string text = i.ToString();

                Label lbl = new Label();
                lbl.Text = text;
                lbl.BackColor = Color.Transparent;
                lbl.Location = new Point(startX - 20, startY + 32 + (i * 74));
                this.Controls.Add(lbl);
                
                lbl = new Label();
                lbl.Text = text;
                lbl.BackColor = Color.Transparent;
                lbl.Location = new Point(startX + 10 + ((5) * 74), startY + 32 + (i * 74));
                this.Controls.Add(lbl);

                lbl = new Label();
                lbl.Text = (colSize - 1 - i).ToString();
                lbl.BackColor = Color.Transparent;
                lbl.Location = new Point(startX + 32 + ((colSize - 1 - i) * 74), startY - 20);
                this.Controls.Add(lbl);
                
                lbl = new Label();
                lbl.Text = (colSize - 1 - i).ToString();
                lbl.BackColor = Color.Transparent;
                lbl.Location = new Point(startX + 32 + ((colSize - 1 - i) * 74), startY + 10 + ((5) * 74));
                this.Controls.Add(lbl);
            }
            
            // Console.WriteLine(i + " : " + (startX + 32 + (i * 74)) + "," + (startY - 20));

            Tree = new MiniMaxAlphaBeta(rowSize, colSize, isPlayerFirst ? 'B' : 'W');
            if (!isPlayerFirst) AIMove();
        }

        private Button ResetPosition(Button clickedButton)
        {
            if (clickedButton.Width == 37)
            {
                clickedButton.Location = new Point(clickedButton.Location.X - 19, clickedButton.Location.Y);
            }
            else if (clickedButton.Width == 65)
            {
                clickedButton.Location = new Point(clickedButton.Location.X - 5, clickedButton.Location.Y - 5);
            }

            clickedButton.Width = 75;
            clickedButton.Height = 75;

            Button b = new Button();
            b.Width = clickedButton.Width;
            b.Height = clickedButton.Height;
            b.Location = new Point(clickedButton.Location.X, clickedButton.Location.Y);
            b.Click += Button_Click;

            return b;
        }

        private Button SuperPion(Button clickedButton)
        {
            RoundedButton rb = new RoundedButton();
            rb.Width = 65;
            rb.Height = 65;

            rb.Location = new Point(clickedButton.Location.X + 5, clickedButton.Location.Y + 5);
            rb.Font = new Font("Microsoft Sans Serif", 20.25f, FontStyle.Regular);

            rb.Click += Button_Click;

            this.Controls.Remove(clickedButton);
            this.Controls.Add(rb);

            int row = Convert.ToInt32((clickedButton.Location.X - startX) / 74);
            int col = Convert.ToInt32((clickedButton.Location.Y - startY) / 74);
            board[row, col][board[row, col].Count - 1] = rb;

            return rb;
        }

        private bool IsValidMove(Button clickedButton)
        {
            // Awal gabole super / stand    
            if (counterFirstTurn < 3 && (!isPion || standBlack.Visible || standWhite.Visible))
            {
                return false;
            }
            
            // Naruh pion gabole di tempat yang ada isinya
            if (clickedButton.BackColor != Color.White)
            {
                return false;
            }
            
            // Kondisi salah satu pion / super habis
            if (isWhiteTurn)
            {
                if (isPion && Convert.ToInt32(pionWhite.Text) == 0)
                {
                    return false;
                }
                else if (!isPion && Convert.ToInt32(superWhite.Text) == 0)
                {
                    return false;
                }
            }
            else
            {
                if (isPion && Convert.ToInt32(pionBlack.Text) == 0)
                {
                    return false;
                }
                else if (!isPion && Convert.ToInt32(superBlack.Text) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsMoveNotStand(int r, int c)
        {
            // Cek possible move tidak boleh menumpuk stand
            Button super = board[r, c][board[r, c].Count - 1];
            if (super.Width == 37)  return false;
            return true;
        }

        private bool IsMoveNotSuper(int r, int c)
        {
            // Cek possible move tidak boleh menumpuk super
            Button super = board[r, c][board[r, c].Count - 1];
            if (super.Width == 65) return false;
            return true;
        }

        private string CheckWin()
        {
            // Cek board kalo ada yang warnanya ujung ke ujung
            List<string> deadMove;

            for (int i = 0; i < colSize; i++)
            {
                if (board[i, 0].Count > 1)
                {
                    deadMove = new List<string>();
                    Stack<string> moveXY = new Stack<string>();

                    Button curr = board[i, 0][board[i, 0].Count - 1];
                    Button next = board[i, 1][board[i, 1].Count - 1];

                    int currX = i;
                    int currY = 1;
                    int defX = i;
                    int defY = 0;
                    bool isRolledBack = false;
                    moveXY.Push(i + ",0");
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kebawah
                        if (curr.BackColor == next.BackColor && curr.Width != 37 && next.Width != 37 && !isRolledBack)
                        {
                            if (currY == colSize - 1) return curr.BackColor == Color.NavajoWhite ? "White" : "Black";

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

                    Button curr = board[0, i][board[0, i].Count - 1];
                    Button next = board[1, i][board[1, i].Count - 1];

                    int currX = 1;
                    int currY = i;
                    int defX = 0;
                    int defY = i;
                    bool isRolledBack = false;
                    moveXY.Push("0," + i);
                    while (true)
                    {
                        // Cek current dan next apakah sama dan tidak dirollback. Kalau sama, next kanan
                        if (curr.BackColor == next.BackColor && curr.Width != 37 && next.Width != 37 && !isRolledBack)
                        {
                            if (currX == rowSize - 1) return curr.BackColor == Color.NavajoWhite ? "White" : "Black";

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

        private void PlacePion(Button clickedButton)
        {
            if (IsValidMove(clickedButton))
            {
                int row = (clickedButton.Location.X - startX) / 74;
                int col = (clickedButton.Location.Y - startY) / 74;
                board[row, col].Add(ResetPosition(clickedButton));
                Button temp = board[row, col][board[row, col].Count - 1];

                if (isWhiteTurn)
                {
                    if (!isPion)
                    {
                        temp = SuperPion(temp);
                    }
                    temp.BackColor = counterFirstTurn < 3 ? Color.LightGray : Color.NavajoWhite;
                    temp.ForeColor = Color.Black;
                    if (standWhite.Visible)
                    {
                        temp.Width = 37;
                        temp.Location = new Point(temp.Location.X + 19, temp.Location.Y);
                    }

                    // Reset
                    if (isPion)
                    {
                        if (counterFirstTurn < 3) pionBlack.Text = (Convert.ToInt32(pionBlack.Text) - 1).ToString();
                        else pionWhite.Text = (Convert.ToInt32(pionWhite.Text) - 1).ToString();
                    }
                    else
                    {
                        superWhite.Text = "0";
                    }

                    string result = CheckWin();
                    if (result != "")
                    {
                        MessageBox.Show(result + " won!");
                        // TAK ta = new TAK(this);
                        // ta.Show();
                    }
                    pionWhite.Enabled = false;
                    superWhite.Enabled = false;
                    pionBlack.Enabled = true;
                    superBlack.Enabled = true;
                    standWhite.Visible = false;
                    isWhiteTurn = false;
                    isPion = true;
                }
                else
                {
                    if (!isPion)
                    {
                        temp = SuperPion(temp);
                    }
                    temp.BackColor = counterFirstTurn < 3 ? Color.NavajoWhite : Color.LightGray;
                    temp.ForeColor = Color.Black;
                    if (standBlack.Visible)
                    {
                        temp.Width = 37;
                        temp.Location = new Point(temp.Location.X + 19, temp.Location.Y);
                    }

                    // Reset
                    if (isPion)
                    {
                        if (counterFirstTurn < 3) pionWhite.Text = (Convert.ToInt32(pionWhite.Text) - 1).ToString();
                        else pionBlack.Text = (Convert.ToInt32(pionBlack.Text) - 1).ToString();
                    }
                    else
                    {
                        superBlack.Text = "0";
                    }

                    string result = CheckWin();
                    if (result != "")
                    {
                        MessageBox.Show(result + " won!");
                        // TAK ta = new TAK(this);
                        // ta.Show();
                    }
                    pionWhite.Enabled = true;
                    superWhite.Enabled = true;
                    pionBlack.Enabled = false;
                    superBlack.Enabled = false;
                    standBlack.Visible = false;
                    isWhiteTurn = true;
                    isPion = true;
                }

                counterFirstTurn++;

                this.Controls.Add(temp);
                temp.BringToFront();
            }
            else
            {
                MessageBox.Show("Invalid move!");
            }
        }

        private void MoveStack(Button clickedButton)
        {
            int row = (clickedButton.Location.X - startX) / 74;
            int col = (clickedButton.Location.Y - startY) / 74;

            if (possibleStackMove.Contains(row + "," + col))
            {
                int prevRow = Convert.ToInt32(possibleStackMove.Substring(0, 1));
                int prevCol = Convert.ToInt32(possibleStackMove.Substring(2, 1));
                if (prevRow != row || prevCol != col)
                {
                    int posRow = row + (row - prevRow);
                    int posCol = col + (col - prevCol);
                    possibleStackMove = "";
                    // Cek possible move harus di dalam board
                    if (selected.Count > 2)
                    {
                        possibleStackMove = row + "," + col;
                        if (posRow > -1 && posRow < rowSize && posCol > -1 && posCol < colSize)
                        {
                            if ((selected[2].Width == 65 || IsMoveNotStand(posRow, posCol)) && IsMoveNotSuper(posRow, posCol))
                            {
                                possibleStackMove += ";" + posRow + "," + posCol;
                            }
                        }
                    }
                }
                else if (selected.Count > 2 && selected[2].Width == 65)
                {
                    // Cek jika next stack is super dan sebelumnya tetap di tempat awal
                    if (!possibleStackMove.Contains((row + 1) + "," + col) && row + 1 < rowSize && IsMoveNotSuper(row + 1, col))
                    {
                        possibleStackMove += ";" + (row + 1) + "," + col;
                    }

                    if (!possibleStackMove.Contains((row - 1) + "," + col) && row - 1 > -1 && IsMoveNotSuper(row - 1, col))
                    {
                        possibleStackMove += ";" + (row - 1) + "," + col;
                    }

                    if (!possibleStackMove.Contains(row + "," + (col + 1)) && col + 1 < colSize && IsMoveNotSuper(row, col + 1))
                    {
                        possibleStackMove += ";" + row + "," + (col + 1);
                    }

                    if (!possibleStackMove.Contains(row + "," + (col - 1)) && col - 1 > -1 && IsMoveNotSuper(row, col - 1))
                    {
                        possibleStackMove += ";" + row + "," + (col - 1);
                    }
                }

                board[row, col].Add(selected[1]);
                Button temp = board[row, col][board[row, col].Count - 1];
                temp.Location = new Point(
                    clickedButton.Width == 37 ? clickedButton.Location.X - 19 : clickedButton.Width == 65 ? clickedButton.Location.X - 5 : clickedButton.Location.X,
                    clickedButton.Width == 65 ? clickedButton.Location.Y - 5 : clickedButton.Location.Y
                );

                if (temp.Width == 37)
                {
                    temp.Height = 75;
                    temp.Location = new Point(temp.Location.X + 19, temp.Location.Y);
                }
                else if (temp.Width == 75)
                {
                    temp.Height = 75;
                }
                else
                {
                    temp.Height = 65;
                    temp.Location = new Point(temp.Location.X + 5, temp.Location.Y + 5);
                }
                temp.BringToFront();

                Button under = board[row, col][board[row, col].Count - 2];
                if (selected[1].Width == 65 && under.Width == 37)
                {
                    under.Width = 75;
                    under.Location = new Point(under.Location.X - 19, under.Location.Y);
                }

                selected.Remove(selected[1]);
                selectedCount--;
            
                if (selectedCount == 0 && (row != stackRow || col != stackCol))
                {
                    string result = CheckWin();
                    if (result != "")
                    {
                        MessageBox.Show(result + " won!");
                        // TAK ta = new TAK(this);
                        // ta.Show();
                    }

                    isWhiteTurn = !isWhiteTurn;
                    pionWhite.Enabled = isWhiteTurn;
                    superWhite.Enabled = isWhiteTurn;
                    standWhite.Visible = false;
                    pionBlack.Enabled = !isWhiteTurn;
                    superBlack.Enabled = !isWhiteTurn;
                    standBlack.Visible = false;
                    isPion = true;
                }
            }
            else
            {
                MessageBox.Show("Invalid move!");
            }
        }

        private void CheckMove(Button clickedButton)
        {
            if (selectedCount > 0)
            {
                MoveStack(clickedButton);
            }
            else if ((clickedButton.BackColor == Color.NavajoWhite && isWhiteTurn) || (clickedButton.BackColor == Color.LightGray && !isWhiteTurn))
            {
                //591, 13 | 591, 22
                int row = (clickedButton.Location.X - startX) / 74;
                int col = (clickedButton.Location.Y - startY) / 74;
                selected = board[row, col];
                selectedCount = selected.Count - 1;
                for (int i = 1; i < selected.Count; i++)
                {
                    selected[i].Location = new Point(
                        selected[i].Width == 37 ? 610 : selected[i].Width == 65 ? 596 : 591,
                        13 + (9 * (selected.Count - i)
                    ));
                    selected[i].Height = 22;
                    selected[i].BringToFront();
                }

                stackRow = row;
                stackCol = col;
                possibleStackMove = row + "," + col;

                // Cek possible move pertama button ditekan
                if (row == 0)
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row + 1, col)) && IsMoveNotSuper(row + 1, col))
                    {
                        possibleStackMove += ";" + (row + 1) + "," + col;
                    }
                }
                else if (row == rowSize - 1)
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row - 1, col)) && IsMoveNotSuper(row - 1, col))
                    {
                        possibleStackMove += ";" + (row - 1) + "," + col;
                    }
                }
                else
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row + 1, col)) && IsMoveNotSuper(row + 1, col))
                    {
                        possibleStackMove += ";" + (row + 1) + "," + col;
                    }

                    if ((selected[1].Width == 65 || IsMoveNotStand(row - 1, col)) && IsMoveNotSuper(row - 1, col))
                    {
                        possibleStackMove += ";" + (row - 1) + "," + col;
                    }
                }

                if (col == 0)
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row, col + 1)) && IsMoveNotSuper(row, col + 1))
                    {
                        possibleStackMove += ";" + row + "," + (col + 1);
                    }
                }
                else if (col == colSize - 1)
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row, col - 1)) && IsMoveNotSuper(row, col - 1))
                    {
                        possibleStackMove += ";" + row + "," + (col - 1);
                    }
                }
                else
                {
                    if ((selected[1].Width == 65 || IsMoveNotStand(row, col + 1)) && IsMoveNotSuper(row, col + 1))
                    {
                        possibleStackMove += ";" + row + "," + (col + 1);
                    }

                    if ((selected[1].Width == 65 || IsMoveNotStand(row, col - 1)) && IsMoveNotSuper(row, col - 1))
                    {
                        possibleStackMove += ";" + row + "," + (col - 1);
                    }
                }
            }
            else
            {
                PlacePion(clickedButton);
            }

            if (selectedCount == 0)
            {
                if (isPlayerFirst && !isWhiteTurn) AIMove();
                else if (!isPlayerFirst && isWhiteTurn) AIMove();
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            CheckMove(clickedButton);
        }

        private void PionWhite_Click(object sender, EventArgs e)
        {
            if (isPion)
            {
                standWhite.Visible = !standWhite.Visible;
            }
            isPion = true;
        }

        private void PionBlack_Click(object sender, EventArgs e)
        {
            if (isPion)
            {
                standBlack.Visible = !standBlack.Visible;
            }
            isPion = true;
        }

        private void SuperPion_Click(object sender, EventArgs e)
        {
            isPion = false;
        }
    
        private void AIMove()
        {
            // Char encode from board
            // WHITE -> W = Pion; X = Super; T = Stand
            // BLACK -> B = Pion; Y = Super; U = Stand

            List<char>[,] convertedBoard = new List<char>[rowSize, colSize];

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < colSize; j++)
                {
                    convertedBoard[i, j] = new List<char>();
                }
            }

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < colSize; j++)
                {
                    for (int k = 0; k < board[i, j].Count; k++)
                    {
                        Button b = board[i, j][k];
                        if (board[i, j].Count > 1)
                        {
                            if (b.BackColor != Color.White)
                            {
                                convertedBoard[i, j].Add(
                                    b.BackColor == Color.NavajoWhite && b.Width == 75 ? 'W' :
                                    b.BackColor == Color.LightGray && b.Width == 75 ? 'B' :
                                    b.BackColor == Color.NavajoWhite && b.Width == 65 ? 'X' :
                                    b.BackColor == Color.LightGray && b.Width == 65 ? 'Y' :
                                    b.BackColor == Color.NavajoWhite && b.Width == 37 ? 'T' : 'U'
                                );
                            }
                        }
                        else
                        {
                            convertedBoard[i, j].Add(' ');
                        }
                    }
                }
            }

            int pionLeft = isPlayerFirst ? Convert.ToInt32(pionBlack.Text) : Convert.ToInt32(pionWhite.Text);
            int superLeft = isPlayerFirst ? Convert.ToInt32(superBlack.Text) : Convert.ToInt32(superWhite.Text);

            if (counterFirstTurn < 3)
            {
                Tree.isFirstTurn = true;
                Tree.MiniMax(convertedBoard, 3, false, int.MinValue, int.MaxValue, pionLeft, superLeft);
            }
            else
            {
                Tree.isFirstTurn = false;
                Tree.MiniMax(convertedBoard, 3, true, int.MinValue, int.MaxValue, pionLeft, superLeft);
            }

            // moveAI.Push(Tree.result);
            // Random rand = new Random();
            // while (true)
            // {
            //    int xx = rand.Next(colSize);
            //    int yy = rand.Next(rowSize);
            //    if (board[xx, yy][board[xx, yy].Count - 1].BackColor == Color.White)
            //    {
            //        moveAI.Push(xx + "," + yy);
            //        break;
            //    }
            // }

            Stack<string> moveAI = Tree.GetAIMove(convertedBoard);

            while (moveAI.Count > 0)
            {
                string move = moveAI.Pop();
                Console.WriteLine("AI Moving : " + move);
                int moveX = Convert.ToInt32(move.Split(',')[0]);
                int moveY = Convert.ToInt32(move.Split(',')[1]);

                if (move.Split(',').Length > 2)
                {
                    char piece = Convert.ToChar(move.Split(',')[2]);
                    if (piece == 'X' || piece == 'Y') isPion = false;
                    else if (piece == 'T') standWhite.Visible = true;
                    else if (piece == 'U') standBlack.Visible = true;
                }

                CheckMove(board[moveX, moveY][board[moveX, moveY].Count - 1]);
            }
            Console.WriteLine(Tree.previewResult);
        }
    }
}