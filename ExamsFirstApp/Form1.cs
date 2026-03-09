using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ExamsFirstApp
{

    public partial class Form1 : Form
    {

        string[] greek = { "Α", "Β", "Γ", "Δ", "Ε", "Ζ", "Η", "Θ", "Ι", "Κ" };
        private Random random;
        private PictureBox[,] Myships = new PictureBox[10, 10];
        private PictureBox[,] EnemyShips = new PictureBox[10, 10];
        private List<Ship> EnemyListShips;
        private List<Ship> PlayerLiistShips;
        private int seconds = 0;
        private int minutes = 0;
        private Timer timer;
        private int wins = 0;
        private int loses = 0;
        private int count = 0;
        private Database db;

        public Form1()
        {
            InitializeComponent();
        }
        private void button_Click(Object sender, EventArgs e)
        {
            this.Close();
        }
        private void button1_Click(Object sender, EventArgs e)
        {
            count++;
            Restart();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            if(seconds == 60)
            {
                minutes++;
                seconds = 0;
            }
        }

        private void Game() // Αρχη παιχνιδιου / σχεδιαση παιχνιδιου / τοποθετηση πλοιων
        {
            this.BackgroundImage = Properties.Resources.battleship;
            this.Controls.Clear();
            timer = new Timer
            {
                Interval = 1000,
            };
            timer.Tick += timer_Tick;

            timer.Start();
            this.Size = new Size(1600, 800);
            this.BackColor = Color.White;
            TableLayoutPanel table = new TableLayoutPanel
            {
                RowCount = 11,
                BackColor = Color.LightGray,
                ColumnCount = 11,
                Location = new Point(50, 50),
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            TableLayoutPanel EnemyTable = new TableLayoutPanel
            {
                RowCount = 11,
                BackColor = Color.LightGray,
                ColumnCount = 11,
                Location = new Point(1000, 50),
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            Label label1 = new Label
            {
                Text = "ΤΑ ΠΛΟΙΑ ΜΟΥ",
                Font = new Font("Arial", 14),
                Size = new Size(518, 30),
                Location = new Point(50, 20),
                BackColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label1);

            Label label2 = new Label
            {
                Text = "ΤΑ ΠΛΟΙΑ ΤΟΥ ΑΝΤΙΠΑΛΟΥ",
                Font = new Font("Arial", 14),
                Size = new Size(518, 30),
                Location = new Point(1000, 20),
                BackColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label2);

            Label corner = new Label
            {
                Size = new Size(40, 45),
                BackColor = Color.LightGray
            };
            table.Controls.Add(corner, 0, 0);

            Label corner1 = new Label
            {
                Size = new Size(40, 45),
                BackColor = Color.LightGray
            };
            EnemyTable.Controls.Add(corner1, 0, 0);

            for (int row = 0; row < 10; row++)
            {
                Label RowLabel = new Label
                {
                    Text = greek[row],
                    Size = new Size(40, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray
                };
                table.Controls.Add(RowLabel, 0, row + 1);

                Label RowLabel1 = new Label
                {
                    Text = greek[row],
                    Size = new Size(40, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray
                };
                EnemyTable.Controls.Add(RowLabel1, 0, row + 1);
            }
            for (int col = 0; col < 10; col++)
            {
                Label ColumnLabel = new Label
                {
                    Text = (col + 1).ToString(),
                    Size = new Size(40, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray
                };
                table.Controls.Add(ColumnLabel, col + 1, 0);

                Label ColumnLabel1 = new Label
                {
                    Text = (col + 1).ToString(),
                    Size = new Size(40, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray
                };
                EnemyTable.Controls.Add(ColumnLabel1, col + 1, 0);

            }
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Myships[row, col] = new PictureBox
                    {
                        Size = new Size(40, 40),
                        Tag = new Point(row, col),
                        BackColor = Color.LightBlue,
                        BorderStyle = BorderStyle.FixedSingle,
                    };
                    table.Controls.Add(Myships[row, col], col + 1, row + 1);

                    EnemyShips[row, col] = new PictureBox
                    {
                        Size = new Size(40, 40),
                        Tag = new Point(row, col),
                        BackColor = Color.LightBlue,
                        BorderStyle = BorderStyle.FixedSingle,
                        Cursor = Cursors.Hand
                    };
                    EnemyShips[row, col].MouseEnter += PictureBox_MouseEnter;
                    EnemyShips[row, col].MouseLeave += PictureBox_MouseLeave;

                    EnemyShips[row, col].Click += PictureBox_Click;

                    EnemyTable.Controls.Add(EnemyShips[row, col], col + 1, row + 1);
                }
            }
            random = new Random();
            EnemyListShips = new List<Ship>();
            PlayerLiistShips = new List<Ship>();

            ShipPlacement(Myships, 2);
            ShipPlacement(Myships, 3);
            ShipPlacement(Myships, 4);
            ShipPlacement(Myships, 5);

            EnemyShipPlacement(EnemyShips, 2);
            EnemyShipPlacement(EnemyShips, 3);
            EnemyShipPlacement(EnemyShips, 4);
            EnemyShipPlacement(EnemyShips, 5);


            this.Controls.Add(table);
            this.Controls.Add(EnemyTable);

        }
        private void ShipPlacement(PictureBox[,] s, int size) // τοποθετηση πλοιων παικτη
        {
            bool p = false;
            while (!p)
            {
                bool horizontal = random.Next(0, 2) == 0;
                int row = random.Next(0, 10);
                int col = random.Next(0, 10);
                if (Placed(s, row, col, size, horizontal))
                {
                    Ship ship = new Ship(size, new Point(row, col), horizontal);

                    for (int i = 0; i < size; i++)
                    {
                        PictureBox c;
                        if (horizontal)
                        {
                            c = s[row, col + i];
                        }
                        else
                        {
                            c = s[row + i, col];
                        }

                        c.BackColor = Color.Red;
                        c.Tag = "Ship";
                        ship.PlayerBoxes.Add(c);
                    }
                    PlayerLiistShips.Add(ship);
                    p = true;
                }
            }
        }
        private void EnemyShipPlacement(PictureBox[,] s, int size) // ειναι η ιδια με την αλλη συναρτηση απλως το χρωμα των picturebox ειναι το ιδιο με τον πινακα για να μην φαινεται που τοποθετησε τα πλοια του ο αντιπαλος
        {
            bool p = false;
            while (!p)
            {
                bool horizontal = random.Next(0, 2) == 0; // ελεγχει εαν η horizontal = 0 τοτε ειναι true
                int row = random.Next(0, 10);
                int col = random.Next(0, 10);
                if (Placed(s, row, col, size, horizontal))
                {
                    Ship ship = new Ship(size, new Point(row, col), horizontal);

                    for (int i = 0; i < size; i++)
                    {
                        PictureBox c;
                        if (horizontal)
                        {
                            c = s[row, col + i];
                        }
                        else
                        {
                            c = s[row + i, col];
                        }
                        c.Tag = "Ship";
                        ship.EnemyBoxes.Add(c);
                        c.BackColor = Color.LightBlue;
                    }
                    EnemyListShips.Add(ship);
                    p = true;
                }
            }
        }

        private bool Placed(PictureBox[,] s, int row, int col, int size, bool horizontal) // ελεγχος για να μην ειναι κολλημενα τα πλοια οταν τοποθετουνται
        {
            if (horizontal && (col + size) > 10)
                return false;
            if (!horizontal && (row + size) > 10)
                return false;

            for (int i = -1; i <= size; i++) // ελεγχει πριν το μεγεθος του πλοιου και μετα
            {
                for (int j = -1; j <= 1; j++)
                { // ελεγχει αριστερα και δεξια απο το πλοίο

                    int checkR = horizontal ? row + j : row + i; // αν ειναι οριζοντιο τοτε ελεγχει την γραμμη απο πανω του και απο κατω του αλλιως αν ειναι καθετο τις ελεγχει ολες + την μια απο πανω και κατω
                    int checkC = horizontal ? col + i : col + j; // αν ειναι οριζοτνιο ελεγχει ολες τις στηλες με βαση το μεγεθος του πλοιου + την μια πριν και μετα αλλιως αν ειναι καθετο ελεγχει τις στηλες πριν και μετα 

                    if (checkR >= 0 && checkR < 10 && checkC >= 0 && checkC < 10)
                    {
                        if (s[checkR, checkC].Tag.ToString() == "Ship")
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        Ship select = null;
        private int n = 0;
        private void PictureBox_Click(object sender, EventArgs e)
        {
            n++;
            PictureBox clicked = (PictureBox)sender;
            clicked.SizeMode = PictureBoxSizeMode.StretchImage;
            clicked.BringToFront();

            if (clicked.Tag.ToString() == "Ship")
            {
                clicked.Image = Properties.Resources.Red_X;

                foreach (Ship ship in EnemyListShips)
                {
                    if (ship.EnemyBoxes.Contains(clicked))
                    {
                        select = ship;
                    }
                }
                EnemySunk(select, clicked);
            }
            else
            {
                clicked.Image = Properties.Resources.Green_Dash;
            }

            Sunk(Myships);
            clicked.Click -= PictureBox_Click;

            if (EnemyListShips.Count == 0 ){
                wins++;
                End();
            }else if(PlayerLiistShips.Count == 0)
            {
                loses++;
                End();
            }
        }
        
        private void EnemySunk(Ship select,PictureBox clicked) // βυθιση αντιπαλου πλοιου
        {

            if (select.Size == 2)
            {
                select.EnemyBoxes.Remove(clicked);

                if (select.EnemyBoxes.Count == 0)
                {
                    EnemyListShips.Remove(select);
                    MessageBox.Show("Βυθίστηκε το Υποβρύχιο του αντιπάλου!");
                }
            }
            if (select.Size == 3)
            {
                select.EnemyBoxes.Remove(clicked);

                if (select.EnemyBoxes.Count == 0)
                {
                    EnemyListShips.Remove(select);
                    MessageBox.Show("Βυθίστηκε το Πολεμικό του αντιπάλου!"); 
                }
            }
            if (select.Size == 4)
            {
                select.EnemyBoxes.Remove(clicked);

                if (select.EnemyBoxes.Count == 0)
                {
                    EnemyListShips.Remove(select);
                    MessageBox.Show("Βυθίστηκε το Αντιτορπιλικό του αντιπάλου!");
                }
            }
            if (select.Size == 5)
            {
                select.EnemyBoxes.Remove(clicked);

                if (select.EnemyBoxes.Count == 0)
                {
                    EnemyListShips.Remove(select);
                    MessageBox.Show("Βυθίστηκε το Αεροπλανοφόρο του αντιπάλου!");
                }
            }
        }

        private List<(int, int)> used = new List<(int, int)>();
        private void Sunk(PictureBox[,] s) // βυθιση του πλοιου του παικτη 
        {
            bool p = false;
            while (!p)
            {
                int row = random.Next(0, 10);
                int col = random.Next(0, 10);
                s[row,col].SizeMode = PictureBoxSizeMode.StretchImage;

                if (used.Contains((row, col)))
                    continue;
                
                if (s[row, col].Tag.ToString() == "Ship")
                {
                    used.Add((row, col));
                    s[row, col].Image = Properties.Resources.Red_X;
                    
                    foreach(Ship ship in PlayerLiistShips)
                    {
                        if (ship.PlayerBoxes.Contains(s[row, col]))
                        {
                            
                            ship.PlayerBoxes.Remove(s[row, col]);
                            

                            if (ship.PlayerBoxes.Count == 0)
                            {
                                PlayerLiistShips.Remove(ship);
                                PlayerSunk(ship);
                            }
                            break;
                        }
                    }
                    p = true;
                }
                else
                {
                    used.Add((row, col));
                    s[row, col].Image = Properties.Resources.Green_Dash;
                    p = true;
                }
            }
        }

        private void PlayerSunk(Ship select) // εμφανιση μηνυματων για τα πλοια του παικτη
        {
            switch (select.Size)
            {
                case 2:
                    MessageBox.Show("Βυθίστηκε το Υποβρύχιο μου");
                    break;
                case 3:
                    MessageBox.Show("Βυθίστηκε το πολεμικό μου");
                    break;
                case 4:
                    MessageBox.Show("Βυθίστηκε το Αντιτορπιλικό μου");
                    break;
                case 5:
                    MessageBox.Show("Βυθίστηκε το Αεροπλανοφόρο μου");
                    break;
                default:
                    MessageBox.Show(" ");
                    break;
            }
        }
        private void End() // εμφανιση οταν καποιος νικησει 
        {
            timer.Stop();

            this.Controls.Clear();
            this.BackgroundImage = null;
            this.Size = new Size(650, 400);
            this.BackColor = Color.DarkBlue;

            string playername = textBox1.Text;
            db.DB(playername, n, wins, loses, minutes, seconds);

            if (count >= 1)
            {
                Label count = new Label
                {
                    Text = $"Έχεις {wins} Νίκες και {loses} Ήττες",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Size = new Size(530, 30),
                    Location = new Point(50, 190),
                    BackColor = Color.DarkBlue,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                this.Controls.Add(count);
            }
            Label label1 = new Label
            {
                Text = $"Έκανες {n} προσπάθειες.",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(530, 30),
                Location = new Point(50, 70),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label1);
            Label labeltimer = new Label
            {
                Text = $"Χρειάστηκε {minutes} λεπτά και {seconds} δευτερόλεπτα για να ολοκληρωθεί το παιχνίδι.",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(530, 60),
                Location = new Point(50, 115),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(labeltimer);

            Button button = new Button
            {
                Text = "Κλείσιμο Παιχνιδιού",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(150, 60),
                Location = new Point(50, 280),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            button.Click += button_Click;
            this.Controls.Add(button);

            Button button1 = new Button
            {
                Text = "Νέα Προσπάθεια",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(150, 60),
                Location = new Point(420, 280),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            button1.Click += button1_Click;
            this.Controls.Add(button1);
            if (EnemyListShips.Count == 0)
            {
                Label label = new Label
                {
                    Text = "Συγχαρητήρια, Κέρδισες!",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Size = new Size(530, 30),
                    Location = new Point(50, 20),
                    BackColor = Color.DarkBlue,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                this.Controls.Add(label);
            }

            if (PlayerLiistShips.Count == 0)
            {
                loses++;
                Label label = new Label
                {
                    Text = "Δυστυχώς, Έχασες. Ξαναπροσπάθησε!",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Size = new Size(520, 30),
                    Location = new Point(50, 20),
                    BackColor = Color.DarkBlue,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                this.Controls.Add(label);
            }
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            PictureBox Select = (PictureBox)sender;

            Select.BackColor = Color.Black;
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            PictureBox Select = (PictureBox)sender;

            Select.BackColor = Color.LightBlue;
        }


        private void Restart()
        {
            seconds = 0;
            minutes = 0;

            this.Controls.Clear();

            EnemyListShips.Clear();
            PlayerLiistShips.Clear();
            used.Clear();
            n = 0;
            select = null;

            Game();

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartButton.FlatAppearance.BorderSize = 3;
            db = new Database();
            db.CreateDatabase();
            Game();
            MessageBox.Show (
                "Κανόνες Παιχνιδιού:\n\n" +
                "1. Πάτησε σε ένα τετράγωνο του πίνακα του αντιπάλου για να επιτεθείς.\n" +
                "2. Αν πετύχεις πλοίο εμφανίζεται κόκκινο Χ.\n" +
                "3. Αν αστοχήσεις εμφανίζεται πράσινη γραμμή.\n" +
                "4. Μετά από κάθε επίθεση ο αντίπαλος επιτίθεται τυχαία.\n" +
                "5. Νικητής είναι όποιος βυθίσει πρώτος όλα τα πλοία του αντιπάλου.\n\n" +
                "Καλή επιτυχία!",
                "Οδηγίες Παιχνιδιού",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartButton.Cursor = Cursors.Hand;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() == "")
            {
                StartButton.Enabled = false;
            }else
                StartButton.Enabled = true;
        }
    }
}
