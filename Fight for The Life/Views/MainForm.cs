using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Domain.Enemies;
using Fight_for_The_Life.Domain.GameObjects;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private readonly Timer timer = new Timer(){ Interval = 50 };
        private Game game;
        private KeyEventHandler onKeyDown;
        private PaintEventHandler gameDrawing;
        private GameImages images;
        public MainForm()
        {
            Size = new Size(1920, 1080);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.GameIcon;
            WindowState = FormWindowState.Maximized;
            Text = "Fight for The Life: Natural Selection";
            Controls.Add(layoutTable);
            BackgroundImageLayout = ImageLayout.Stretch;

            layoutTable.ColumnCount = 3;
            layoutTable.RowCount = 6;
            layoutTable.Dock = DockStyle.Fill;
            for (var i = 0; i < layoutTable.ColumnCount; i++)
                layoutTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            for (var i = 0; i < layoutTable.RowCount; i++)
                layoutTable.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));

            timer.Tick += (sender, args) =>
            {
                game.IncreaseGameTimeInSeconds(50 / 1000d);
                game.UpdateGame();
                if (game.IsGameOver)
                    GameOver();
                Invalidate();
            };

            CheckSave();
            MainMenuInitialization();
        }

        private void CheckSave()
        {
            var saveData = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "save.dat"));
            var saveInfo = string.Join("", new string(Encoding.Unicode.GetChars(saveData)));
            var infoArray = saveInfo.Split();
            var highestScore = 0;
            var dnaAmount = 0;

            if (infoArray.Length >= 4)
                dnaAmount = int.Parse(infoArray[3]);
                
            if (saveInfo.Split().Length >= 2)
                highestScore = int.Parse(infoArray[1]);

            game = new Game(dnaAmount, highestScore);
        }

        private void MainMenuInitialization()
        {
            images = new GameImages(Width, Height);
            var font = new Font("Segoe Print", (int)(70 * Width / 1920d),
                FontStyle.Bold, GraphicsUnit.World);
            Paint -= gameDrawing;
            gameDrawing = (sender, args) =>
            {
                args.Graphics.DrawImage(images.Dna, new Point(20, 20));
                args.Graphics.DrawString(game.DnaAmount.ToString(), font, Brushes.White, 40 + dnaImage.Width, 10);
            };
            Paint += gameDrawing;
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = Resources.MainMenuBackground;

            AddButton("- Выход -", 24, Color.White, 1, 5, 
                AnchorStyles.None, (sender, args) => Application.Exit());
            AddButton("Играть", 54, Color.Black, 0, 4, 
                AnchorStyles.Right, (sender, args) => ControlMenuInitialization());
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());

            Invalidate();
        }

        private void EnemiesFirstPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.FirstEnemies;

            AddButton("- Выход -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());
            AddButton("Следующая страница ->", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => EnemiesSecondPageInitialization());
        }

        private void EnemiesSecondPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.EnemiesSecond;

            AddButton("- Выход -", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => MainMenuInitialization());
            AddButton("<- Предыдущая страница", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }
        private void ControlMenuInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Properties.Resources.Control;

            AddButton("- К игре -", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) => StartGame());
        }

        private void StartGame()
        {
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = null;
            var font = new Font("Segoe Print", (int)(70 * Width / 1920d),
                FontStyle.Bold, GraphicsUnit.World);

            CheckSave();
            timer.Start();



            gameDrawing = (sender, args) =>
            {
                var scoreText = "Score: " + game.GetScore();
                var highestText = "Highest Score: " + game.HighestScore;
                var indent = (int) (Game.FieldHeight * 0.26993006993);
                var coreModel = game.Sperm.Core.GetModel();

                args.Graphics.DrawImage(images.Background, 0, 0);
                args.Graphics.DrawString(scoreText, font, new SolidBrush(Color.White), new PointF(0, 0));
                args.Graphics.DrawString(highestText, font, Brushes.White, new PointF(1050 * Width / 1920f, 0));
                args.Graphics.DrawImage(images.Core, new Point(coreModel.X + coreModel.Width - images.Core.Width,
                    coreModel.Location.Y - (images.Core.Height - coreModel.Height) / 2 + indent));
                args.Graphics.DrawImage(images.Sperm, new Point(game.Sperm.Model.Width - images.Sperm.Width,
                    game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent));

                DrawGameObjects(args, indent);
            };
            Paint += gameDrawing;

            onKeyDown = (sender, args) => ControlInGame(args);
            KeyDown += onKeyDown;
        }

        private void DrawGameObjects(PaintEventArgs args, int indent)
        {
            foreach (var gameObject in game.GameObjects)
            {
                var gameObjectModel = gameObject.GetModel();
                if (gameObject is Blood)
                    args.Graphics.DrawImage(images.Blood, new Point(
                        gameObjectModel.X + gameObjectModel.Width - images.Blood.Width,
                        gameObjectModel.Location.Y - (images.Blood.Height - gameObjectModel.Height) / 2 + indent));

                if (gameObject is IntrauterineDevice)
                    args.Graphics.DrawImage(images.IntrauterineDevice, new Point(
                        gameObjectModel.X,
                        gameObjectModel.Location.Y - (images.IntrauterineDevice.Height - gameObjectModel.Height) / 2 + indent));

                if (gameObject is BirthControl)
                    args.Graphics.DrawImage(images.BirthControl, new Point(
                        gameObjectModel.X,
                        gameObjectModel.Location.Y - (images.BirthControl.Height - gameObjectModel.Height) / 2 + indent));

                if (gameObject is OtherSperm)
                    args.Graphics.DrawImage(images.OtherSperm, new Point(
                        gameObjectModel.X,
                        gameObjectModel.Location.Y - (images.OtherSperm.Height - gameObjectModel.Height) / 2 + indent));

                if (gameObject is Dna)
                    args.Graphics.DrawImage(images.Dna, new Point(
                        gameObjectModel.X,
                        gameObjectModel.Location.Y - (images.Dna.Height - gameObjectModel.Height) / 2 + indent));
            }
        }

        private void GameOver()
        {
            timer.Stop();
            KeyDown -= onKeyDown;
            layoutTable.BackColor = Color.FromArgb(128, 0, 0, 0);

            File.WriteAllBytes(
                Path.Combine(Directory.GetCurrentDirectory(), "save.dat"), 
                Encoding.Unicode.GetBytes("HighestScore " + game.HighestScore + "\nDnaAmount " + game.DnaAmount));

            AddButton("Esc - выход в меню", 24, Color.White, 0, 5, 
                AnchorStyles.Left, (sender, args) =>
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    MainMenuInitialization();
                });

            AddButton("Enter - новая игра", 24, Color.White, 2, 5, 
                AnchorStyles.Right, (sender, args) =>
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    StartGame();
                });

            AddButton("Game Over", 75, Color.White, 1, 1, AnchorStyles.None);

            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    MainMenuInitialization();
                }

                if (args.KeyCode == Keys.Enter)
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    StartGame();
                }
            };
            KeyDown += onKeyDown;
        }

        private void PauseGame()
        {
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    ResumeGame();
            };
            KeyDown += onKeyDown;
            layoutTable.BackColor = Color.FromArgb(128, 0, 0, 0);

            AddButton("- В меню -", 60, Color.White, 1, 2, 
                AnchorStyles.None, (sender, args) =>
                {
                    Paint -= gameDrawing;
                    KeyDown -= onKeyDown;
                    MainMenuInitialization();
                });

            AddButton("- Выход -", 60, Color.White, 1, 3, 
                AnchorStyles.None, (sender, args) => Application.Exit());



            timer.Stop();
        }

        private void ResumeGame()
        {
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) => ControlInGame(args);
            KeyDown += onKeyDown;
            timer.Start();
        }

        private void AddButton(string text, int size, Color color, 
            int column, int row, AnchorStyles anchor, EventHandler actionOnClick=null)
        {
            var button = new Label
            {
                Anchor = anchor,
                Text = text,
                BackColor = Color.Transparent,
                AutoSize = true,
                Cursor = Cursors.Hand,
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Font = new Font("Segoe Print",(int) (size * Width / 1920d), FontStyle.Bold, GraphicsUnit.World)
            };
            button.Click += actionOnClick;
            layoutTable.Controls.Add(button, column, row);
        }

        private void ControlInGame(KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Up)
            {
                game.Sperm.MoveUp();
            }

            if (args.KeyCode == Keys.Down)
            {
                game.Sperm.MoveDown();
            }

            if (args.KeyCode == Keys.Space)
            {
                game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond());
            }

            if (args.KeyCode == Keys.Escape)
            {
                PauseGame();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }
    }
}
