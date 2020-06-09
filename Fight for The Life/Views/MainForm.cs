using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private readonly SoundPlayer player = new SoundPlayer(
                Path.Combine(Directory.GetCurrentDirectory(), "Soundtrack.wav"));
        private readonly Timer timer = new Timer { Interval = 33 };
        private int animationTimeCount;
        private readonly Timer animationTimer = new Timer { Interval = 125, Enabled = true };
        private Game game;
        private KeyEventHandler onKeyDown;
        private PaintEventHandler gameDrawing;
        private PaintEventHandler interfaceDrawing;
        private GameImages images;
        private double widthCoefficient;
        private double heightCoefficient;
        private bool isMusicPlaying = true;
        private bool isGamePlaying;

        private string SaveData => "HighestScore " + game.HighestScore +
                                   "\nDnaAmount " + game.DnaAmount +
                                   "\nScoreCoefficient " + game.ScoreCoefficient +
                                   "\nShieldMaxTime " + game.ShieldMaxTimeInSeconds +
                                   "\nMagnetMaxTime " + game.MagnetMaxTimeInSeconds +
                                   "\nExtraLifeAmount " + game.ExtraLifeAmount;

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
                game.IncreaseGameTimeInSeconds(timer.Interval / 1000d);
                game.UpdateGame();
                if (game.IsGameOver)
                    GameOver();
                if (isGamePlaying)
                    HandleControlInGame();
                Invalidate();
            };
            animationTimer.Tick += (sender, args) => animationTimeCount += animationTimer.Interval;

            KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Up || args.KeyCode == Keys.Down)
                    pressedKeys.Add(args.KeyCode);
            };
            KeyUp += (sender, args) => pressedKeys.Remove(args.KeyCode);

            player.PlayLooping();

            CheckSave();
            MainMenuInitialization();
        }

        private void CheckSave()
        {
            byte[] saveData;
            try
            {
                saveData = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "save.dat"));
            }
            catch (Exception)
            {
                File.Create(Path.Combine(Directory.GetCurrentDirectory(), "save.dat"));
                saveData = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "save.dat"));
            }

            var saveInfo = string.Join("", new string(Encoding.Unicode.GetChars(saveData)));
            var infoArray = saveInfo.Split();
            var highestScore = 0;
            var dnaAmount = 0;
            var shieldMaxTime = 0;
            var magnetMaxTime = 0;
            var extraLifeAmount = 0;
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

            if (infoArray.Length >= 12)
                extraLifeAmount = int.Parse(infoArray[11]);

            game = new Game(dnaAmount, highestScore, scoreCoefficient, shieldMaxTime, magnetMaxTime, extraLifeAmount);
        }

        private void MainMenuInitialization()
        {
            widthCoefficient = Width / 1920d;
            heightCoefficient = Height / 1080d;
            images = new GameImages(widthCoefficient, heightCoefficient);
            var font = new Font("Segoe Print", (int)(70 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);


            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = Resources.MainMenuBackground;

            Paint -= interfaceDrawing;
            Paint -= gameDrawing;
            interfaceDrawing = (sender, args) =>
            {
                args.Graphics.DrawImage(images.Dna, new Point(20, 20));
                args.Graphics.DrawString(game.DnaAmount.ToString(), font, Brushes.White, 20 + images.Dna.Width, 5);
            };
            Paint += interfaceDrawing;

            AddButton("- Выход -", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) =>
                {
                    Save();
                    Application.Exit();
                });
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
            AddButton("Магазин", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => ShopInitialization());
            AddButton("Играть", 54, Color.Black, 0, 4,
                AnchorStyles.Right, (sender, args) =>
                {
                    if (game.HighestScore == 0)
                        TutorialInitialization();
                    else
                        ControlMenuInitialization();
                });
            AddMusicButton(24);

            Invalidate();
        }

        private void ManageMusic(object sender)
        {
            var musicButton = sender as Label;
            if (isMusicPlaying)
            {
                musicButton.Text = "Музыка: выкл.";
                player.Stop();
                isMusicPlaying = false;
            }
            else
            {
                musicButton.Text = "Музыка: вкл.";
                player.PlayLooping();
                isMusicPlaying = true;
            }
        }

        private void ShopInitialization()
        {
            UpdateShopButtons();
            BackgroundImage = Resources.Shop;
            var font = new Font("Segoe Print", (int)(40 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);
            var dnaFont = new Font("Segoe Print", (int)(70 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);

            Paint -= interfaceDrawing;
            interfaceDrawing = (sender, args) =>
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

                args.Graphics.DrawImage(images.Dna, Width / 3 * 2, Height / 6 * 4 + Height / 24);
                args.Graphics.DrawString(Game.ExtraLifeCost.ToString(), font, Brushes.Black,
                    Width / 3 * 2 + images.Dna.Width, Height / 6 * 4 + Height / 24);

                args.Graphics.DrawImage(images.Dna, new Point(20, 20));
                args.Graphics.DrawString(game.DnaAmount.ToString(), dnaFont, Brushes.White, 20 + images.Dna.Width, 5);
            };
            Paint += interfaceDrawing;
        }

        private void EnemiesFirstPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.FirstEnemies;

            AddButton("- В меню -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());
            AddButton("Следующая страница ->", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => EnemiesSecondPageInitialization());
        }

        private void EnemiesSecondPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.EnemiesSecond;

            AddButton("- В меню -", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => MainMenuInitialization());
            AddButton("<- Предыдущая страница", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }

        private void ControlMenuInitialization()
        {
            widthCoefficient = Width / 1920d;
            heightCoefficient = Height / 1080d;
            images = new GameImages(widthCoefficient, heightCoefficient);
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.Control;

            AddButton("Нажмите на любую кнопку для начала игры", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) => StartGame());

            onKeyDown = (sender, args) => StartGame();
            KeyDown += onKeyDown;
        }

        private void TutorialInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.Tutorial;

            AddButton("- Управление -", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) => ControlMenuInitialization());
        }

        private void StartGame()
        {
            isGamePlaying = true;
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = null;
            var font = new Font("Segoe Print", (int)(60 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);
            var dnaFont = new Font("Segoe Print", (int)(70 * widthCoefficient),
                FontStyle.Bold, GraphicsUnit.World);

            CheckSave();
            timer.Start();

            Paint -= interfaceDrawing;
            gameDrawing = (sender, args) =>
            {
                var scoreText = "Score: " + game.GetScore();
                var highestText = "Highest Score: " + game.HighestScore;
                var indent = (int)(Game.FieldHeight * 0.26993006993);

                args.Graphics.DrawImage(images.Background, 0, 0);
                args.Graphics.DrawString(scoreText, font, new SolidBrush(Color.White), 0, 0);
                args.Graphics.DrawString(highestText, font, Brushes.White, (float)(1050 * widthCoefficient), 0);
                args.Graphics.DrawImage(images.Dna, 20, Height - 20 - images.Dna.Height);
                args.Graphics.DrawString(game.DnaAmount.ToString(), dnaFont, Brushes.White, 20 + images.Dna.Width,
                    Height - 30 - images.Dna.Height);
                if (game.ExtraLifeAmount == 1)
                    args.Graphics.DrawImage(images.OneLife, Width * 5 / 6 - images.OneLife.Width / 2,
                        Height - images.OneLife.Height * 2);
                if (game.ExtraLifeAmount == 2)
                    args.Graphics.DrawImage(images.TwoLives, Width * 5 / 6 - images.OneLife.Width / 2,
                        Height - images.OneLife.Height * 2);
                if (game.ExtraLifeAmount == 3)
                    args.Graphics.DrawImage(images.ThreeLives, Width * 5 / 6 - images.OneLife.Width / 2,
                        Height - images.OneLife.Height * 2);
                if (game.ExtraLifeAmount == 4)
                    args.Graphics.DrawImage(images.FourLives, Width * 5 / 6 - images.OneLife.Width / 2,
                        Height - images.OneLife.Height * 2);
                if (game.ExtraLifeAmount == 5)
                    args.Graphics.DrawImage(images.FiveLives, Width * 5 / 6 - images.OneLife.Width / 2,
                        Height - images.OneLife.Height * 2);

                DrawSpermAndCore(args, indent);
                DrawGameObjects(args, indent);
            };
            Paint += gameDrawing;

            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    PauseGame();
                if (args.KeyCode == Keys.Space)
                    game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond());
            };
            KeyDown += onKeyDown;
        }

        private void DrawSpermAndCore(PaintEventArgs args, int indent)
        {
            var coreModel = game.Sperm.Core.GetModel();

            if (!game.Sperm.IsInvulnerable || animationTimeCount % 250 == 0)
            {
                args.Graphics.DrawImage(images.Core,
                    coreModel.X + coreModel.Width - images.Core.Width,
                    coreModel.Location.Y - (images.Core.Height - coreModel.Height) / 2 + indent);
                if (game.Sperm.IsShieldActivated && game.Sperm.IsMagnetActivated)
                {
                    if (game.ShieldMaxTimeInSeconds - game.ShieldTimeInSeconds > 1 || animationTimeCount % 250 == 0)
                        args.Graphics.DrawImage(images.SpermWithShieldAndMagnet,
                            game.Sperm.Model.Width - images.Sperm.Width,
                            game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
                    else
                        args.Graphics.DrawImage(images.SpermWithMagnet,
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
                    if (game.ShieldMaxTimeInSeconds - game.ShieldTimeInSeconds > 1 || animationTimeCount % 250 == 0)
                        args.Graphics.DrawImage(images.SpermWithShield,
                            game.Sperm.Model.Width - images.Sperm.Width,
                            game.Sperm.Location.Y - (images.Sperm.Height - game.Sperm.Model.Height) / 2 + indent);
                    else
                        args.Graphics.DrawImage(images.Sperm,
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
            isGamePlaying = false;
            timer.Stop();
            KeyDown -= onKeyDown;
            layoutTable.BackColor = Color.FromArgb(170, 0, 0, 0);

            Save();

            if (game.MagnetMaxTimeInSeconds == 0 && game.ShieldMaxTimeInSeconds == 0
                                                 && game.DnaAmount >= game.ShieldMaxTimeCost)
            {
                FirstBuyInitialization();
                return;
            }

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

        private void FirstBuyInitialization()
        {
            layoutTable.BackColor = Color.Transparent;
            layoutTable.Controls.Clear();

            interfaceDrawing = (sender, args) =>
            {
                args.Graphics.FillRectangle(
                    new SolidBrush(Color.FromArgb(170, 0, 0, 0)), 0, 0, Width, Height);
                args.Graphics.DrawImage(Resources.FirstBuy, (Width - Resources.FirstBuy.Width) / 2,
                    (Height - Resources.FirstBuy.Height) / 2, Resources.FirstBuy.Width, Resources.FirstBuy.Height);
            };
            Paint += interfaceDrawing;

            AddButton("В магазин!", 60, Color.White, 1, 3,
                AnchorStyles.None, (sender, args) =>
                {
                    Paint -= gameDrawing;
                    Paint -= interfaceDrawing;
                    ShopInitialization();
                });
        }

        private void PauseGame()
        {
            isGamePlaying = false;
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    ResumeGame();
            };
            KeyDown += onKeyDown;
            layoutTable.BackColor = Color.FromArgb(170, 0, 0, 0);


            AddButton("- В меню -", 60, Color.White, 1, 3,
                AnchorStyles.None, (sender, args) =>
                {
                    Paint -= gameDrawing;
                    KeyDown -= onKeyDown;
                    MainMenuInitialization();
                });

            AddButton("- К игре -", 60, Color.White, 1, 2,
                AnchorStyles.None, (sender, args) => ResumeGame());

            AddMusicButton(40);

            timer.Stop();
        }

        private void ResumeGame()
        {
            isGamePlaying = true;
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    PauseGame();
                if (args.KeyCode == Keys.Space)
                    game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond());
            };
            KeyDown += onKeyDown;
            timer.Start();
        }

        private void AddButton(string text, int size, Color color,
            int column, int row, AnchorStyles anchor, EventHandler actionOnClick = null, Bitmap background = null)
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
                Font = new Font("Segoe Print", (int)(size * widthCoefficient),
                    FontStyle.Bold, GraphicsUnit.World),
                BackgroundImage = background
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
                else if (item == ItemToBuy.ScoreCoefficient)
                    game.ScoreCoefficient += 0.5;
                else
                    game.ExtraLifeAmount++;

                Save();
                UpdateShopButtons();
                Invalidate();
            }
        }

        private void HandleControlInGame()
        {
            if (pressedKeys.Contains(Keys.Up))
            {
                game.Sperm.MoveUp();
            }

            if (pressedKeys.Contains(Keys.Down))
            {
                game.Sperm.MoveDown();
            }

            if (pressedKeys.Contains(Keys.Space))
            {
                game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond());
            }
        }

        private void UpdateShopButtons()
        {
            layoutTable.Controls.Clear();

            AddButton("- В меню -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());

            AddButton("Магнит + 5с\n(тек. " + game.MagnetMaxTimeInSeconds + ")",
                40, Color.Black, 1, 1, AnchorStyles.Left,
                (sender, args) => TryBuy(game.MagnetMaxTimeCost, ItemToBuy.MagnetMaxTime));

            AddButton("Щит + 5с\n(тек. " + game.ShieldMaxTimeInSeconds + ")",
                40, Color.Black, 1, 2, AnchorStyles.Left,
                (sender, args) => TryBuy(game.ShieldMaxTimeCost, ItemToBuy.ShieldMaxTime));

            AddButton("Множитель очков + 0.5\n(тек. x" + game.ScoreCoefficient + ")",
                40, Color.Black, 1, 3, AnchorStyles.Left,
                (sender, args) => TryBuy(game.ScoreCoefficientCost, ItemToBuy.ScoreCoefficient));

            AddButton("Дополнительные жизни + 1\n(тек. " + game.ExtraLifeAmount + ", макс. 5)",
                40, Color.Black, 1, 4, AnchorStyles.Left,
                (sender, args) => TryBuy(Game.ExtraLifeCost, ItemToBuy.ExtraLife));
        }

        private void AddMusicButton(int size)
        {
            string musicButtonText;
            if (isMusicPlaying)
                musicButtonText = "Музыка: вкл.";
            else
                musicButtonText = "Музыка: выкл.";
            AddButton(musicButtonText, size, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => ManageMusic(sender));
        }

        private void Save()
        {
            File.WriteAllBytes(
                Path.Combine(Directory.GetCurrentDirectory(), "save.dat"),
                Encoding.Unicode.GetBytes(SaveData));
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
