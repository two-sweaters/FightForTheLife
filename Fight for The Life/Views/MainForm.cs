﻿using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private readonly Timer timer = new Timer { Interval = 50 };
        private Game game;
        private KeyEventHandler onKeyDown;
        private PaintEventHandler gameDrawing;
        private GameImages images;
        private double widthCoefficient;
        private double heightCoefficient;

        private string SaveData => "HighestScore " + game.HighestScore +
                                   "\nDnaAmount " + game.DnaAmount +
                                   "\nScoreCoefficient " + game.ScoreCoefficient +
                                   "\nShieldMaxTime " + game.ShieldMaxTimeInSeconds +
                                   "\nMagnetMaxTime " + game.MagnetMaxTimeInSeconds;

        public MainForm()
        {
            Size = new Size(1920, 1080);
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
            var shieldMaxTime = 0;
            var magnetMaxTime = 0;
            var scoreCoefficient = 1.0;

            if (infoArray.Length >= 2)
                highestScore = int.Parse(infoArray[1]);

            if (infoArray.Length >= 4)
                dnaAmount = int.Parse(infoArray[3]);

            if (infoArray.Length >= 6)
                scoreCoefficient = double.Parse(infoArray[5]);

            if (infoArray.Length >= 8)
                shieldMaxTime = int.Parse(infoArray[7]);

            if (infoArray.Length >= 10)
                magnetMaxTime = int.Parse(infoArray[9]);

            game = new Game(dnaAmount, highestScore, scoreCoefficient, shieldMaxTime, magnetMaxTime);
        }

        private void MainMenuInitialization()
        {
            widthCoefficient = Width / 1920d;
            heightCoefficient = Height / 1080d;
            images = new GameImages(widthCoefficient, heightCoefficient);
            var font = new Font("Segoe Print", (int)(70 * Width / 1920d),
                FontStyle.Bold, GraphicsUnit.World);

            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = Resources.MainMenuBackground;

            Paint -= gameDrawing;
            gameDrawing = (sender, args) =>
            {
                args.Graphics.DrawImage(images.Dna, new Point(20, 20));
                args.Graphics.DrawString(game.DnaAmount.ToString(), font, Brushes.White, 20 + images.Dna.Width, 5);
            };
            Paint += gameDrawing;

            AddButton("- Выход -", 24, Color.White, 1, 5, 
                AnchorStyles.None, (sender, args) => Application.Exit());
            AddButton("Играть", 54, Color.Black, 0, 4, 
                AnchorStyles.Right, (sender, args) => ControlMenuInitialization());
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
            AddButton("Магазин", 40, Color.White, 2, 0, 
                AnchorStyles.Right, (sender, args) => ShopInitialization());

            Invalidate();
        }

        private void ShopInitialization()
        {
            UpdateShopButtons();
            BackgroundImage = Resources.Shop;
            var font = new Font("Segoe Print", (int)(40 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);

            gameDrawing = (sender, args) =>
            {
                args.Graphics.DrawImage(images.Dna, Width / 3 * 2, Height / 6 * 1 + Height / 24);
                args.Graphics.DrawString(game.MagnetMaxTimeCost.ToString(), font, Brushes.Black,
                    Width / 3 * 2 + images.Dna.Width, Height / 6 * 1 + Height / 24);

                args.Graphics.DrawImage(images.Dna, Width / 3 * 2, Height / 6 * 2 + Height / 24);
                args.Graphics.DrawString(game.ShieldMaxTimeCost.ToString(), font, Brushes.Black, 
                    Width / 3 * 2 + images.Dna.Width, Height / 6 * 2 + Height / 24);

                args.Graphics.DrawImage(images.Dna, Width / 3 * 2, Height / 6 * 3 + Height / 24);
                args.Graphics.DrawString(game.ScoreCoefficientCost.ToString(), font, Brushes.Black,
                    Width / 3 * 2 + images.Dna.Width, Height / 6 * 3 + Height / 24);
            };
            Paint += gameDrawing;
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
            BackgroundImage = Resources.Control;

            AddButton("- К игре -", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) => StartGame());
        }

        private void StartGame()
        {
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = null;
            var font = new Font("Segoe Print", (int)(60 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);

            CheckSave();
            timer.Start();

            Paint -= gameDrawing;
            gameDrawing = (sender, args) =>
            {
                var scoreText = "Score: " + game.GetScore();
                var highestText = "Highest Score: " + game.HighestScore;
                var indent = (int) (Game.FieldHeight * 0.26993006993);

                args.Graphics.DrawImage(images.Background, 0, 0);
                args.Graphics.DrawString(scoreText, font, new SolidBrush(Color.White), 0, 0);
                args.Graphics.DrawString(highestText, font, Brushes.White, (float) (1050 * widthCoefficient), 0);

                DrawSpermAndCore(args, indent);
                DrawGameObjects(args, indent);
            };
            Paint += gameDrawing;

            onKeyDown = (sender, args) => ControlInGame(args);
            KeyDown += onKeyDown;
        }

        private void DrawSpermAndCore(PaintEventArgs args, int indent)
        {
            var coreModel = game.Sperm.Core.GetModel();

            args.Graphics.DrawImage(images.Core,
                coreModel.X + coreModel.Width - images.Core.Width,
                coreModel.Location.Y - (images.Core.Height - coreModel.Height) / 2 + indent);

            if (game.Sperm.IsShieldActivated && game.Sperm.IsMagnetActivated)
            {
                args.Graphics.DrawImage(images.SpermWithShieldAndMagnet, 
                    game.Sperm.Model.Width - images.Sperm.Width,
                    game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
            }
            else if (game.Sperm.IsMagnetActivated)
            {
                args.Graphics.DrawImage(images.SpermWithMagnet,
                    game.Sperm.Model.Width - images.Sperm.Width,
                    game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
            }
            else if (game.Sperm.IsShieldActivated)
            {
                args.Graphics.DrawImage(images.SpermWithShield,
                    game.Sperm.Model.Width - images.Sperm.Width,
                    game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
            }
            else
            {
                args.Graphics.DrawImage(images.Sperm, 
                    game.Sperm.Model.Width - images.Sperm.Width,
                    game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
            }
        }

        private void DrawGameObjects(PaintEventArgs args, int indent)
        {
            foreach (var gameObject in game.GameObjects)
            {
                var gameObjectModel = gameObject.GetModel();
                var image = images.GameObjectsImages[gameObject.GetType()];
                args.Graphics.DrawImage(image, 
                    gameObjectModel.X,
                    gameObjectModel.Y - (image.Height - gameObjectModel.Height) / 2 + indent);
            }
        }

        private void GameOver()
        {
            timer.Stop();
            KeyDown -= onKeyDown;
            layoutTable.BackColor = Color.FromArgb(128, 0, 0, 0);

            File.WriteAllBytes(
                Path.Combine(Directory.GetCurrentDirectory(), "save.dat"),
                Encoding.Unicode.GetBytes(SaveData));

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

            AddButton("Game Over", 60, Color.White, 1, 1, AnchorStyles.None);

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
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.None,
                Font = new Font("Segoe Print",(int) (size * widthCoefficient), FontStyle.Bold, GraphicsUnit.World)
            };
            button.Click += actionOnClick;
            layoutTable.Controls.Add(button, column, row);
        }


        private void TryBuy(int cost, ItemToBuy item)
        {
            if (cost <= game.DnaAmount)
            {
                game.DnaAmount -= cost;
                if (item == ItemToBuy.MagnetMaxTime)
                    game.MagnetMaxTimeInSeconds += 5;
                else if (item == ItemToBuy.ShieldMaxTime)
                    game.ShieldMaxTimeInSeconds += 5;
                else
                    game.ScoreCoefficient += 0.5;

                File.WriteAllBytes(
                    Path.Combine(Directory.GetCurrentDirectory(), "save.dat"),
                    Encoding.Unicode.GetBytes(SaveData));

                UpdateShopButtons();
                Invalidate();
            }
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

        private void UpdateShopButtons()
        {
            layoutTable.Controls.Clear();

            AddButton("- Выход -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());

            AddButton("Магнит + 5с\n(тек. " + game.MagnetMaxTimeInSeconds + ")",
                40, Color.Black, 1, 1, AnchorStyles.Left,
                (sender, args) => TryBuy(game.MagnetMaxTimeCost, ItemToBuy.MagnetMaxTime));

            AddButton("Щит +5с\n(тек. " + game.ShieldMaxTimeInSeconds + ")",
                40, Color.Black, 1, 2, AnchorStyles.Left,
                (sender, args) => TryBuy(game.ShieldMaxTimeCost, ItemToBuy.ShieldMaxTime));

            AddButton("Множитель очков +0.5\n(тек. x" + game.ScoreCoefficient + ")",
                40, Color.Black, 1, 3, AnchorStyles.Left,
                (sender, args) => TryBuy(game.ScoreCoefficientCost, ItemToBuy.ScoreCoefficient));
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
