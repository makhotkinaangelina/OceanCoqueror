using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OceanConqueror
{
    public partial class Form1 : Form
    {
        private const int START_PLAYER_LIVES = 3;

        private Player player;
        private List<Opponent> opponents;

        private Thread opponentsThread;
        private Thread projectilesThread;
        private Thread playerThread;

        private CancellationTokenSource opponentsCancellation;
        private CancellationTokenSource projectilesCancellation;
        private CancellationTokenSource playerCancellation;

        private int gameTimeInSeconds = 0;
        private int score = 0;
        private int playerLives = START_PLAYER_LIVES;

        public static Image playerImage;
        public static Image opponentImage;

        private Random random = new Random();

        private const int blinkInterval = 200;
        private const int blinkCount = 6;

        bool isRunning = true;

        public Form1()
        {
            InitializeComponent();

            UpdateLivesLabel(playerLives);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            InitializeGame();
        }

        private void InitializeGame()
        {
            playerImage = Image.FromFile("planePlayer.png");
            opponentImage = Image.FromFile("planeOpponent.png");

            player = new Player();
            opponents = new List<Opponent>();

            Controls.Add(player.PictureBox);
            player.PictureBox.Location = new Point((this.ClientSize.Width - player.PictureBox.Width)/2, this.ClientSize.Height - player.PictureBox.Height - 20); ;
            player.PictureBox.BringToFront();

            player.AutoShoot();

            CreateOpponents();

            UpdateLivesLabel(playerLives); 

            timer.Start();

            opponentsCancellation = new CancellationTokenSource();
            projectilesCancellation = new CancellationTokenSource();
            playerCancellation = new CancellationTokenSource();

            try
            {
                opponentsThread = new Thread(() => UpdateOpponents(opponentsCancellation.Token));
                opponentsThread.Start();

                projectilesThread = new Thread(() => UpdateProjectiles(projectilesCancellation.Token));
                projectilesThread.Start();

                playerThread = new Thread(() => UpdatePlayer(playerCancellation.Token));
                playerThread.Start();
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Недостаточно памяти для запуска приложения.", "Ошибка памяти!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла неожиданная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CreateOpponents()
        {
            opponents.Clear();
            GC.Collect(0);

            this.BeginInvoke(new Action(() =>
            {
                int opponentCount = random.Next(2, 4); 
                int fieldParts = opponentCount; 

                for (int i = 0; i < opponentCount; i++)
                {
                    Opponent opponent = new Opponent();
                    opponent.DirectionChangeTimer.Start();
                    int partWidth = (
                    this.ClientSize.Width - 150) / fieldParts;
                    int left = random.Next(i * partWidth, (i + 1) * partWidth - opponent.PictureBox.Width);
                    int top = random.Next(-opponent.PictureBox.Height * 2, -opponent.PictureBox.Height);
                    opponent.PictureBox.Location = new Point(left, top);
                    opponent.Speed = GetRandomSpeed();

                    opponents.Add(opponent);
                    Controls.Add(opponent.PictureBox);
                    opponent.PictureBox.BringToFront();
                    opponent.AutoShoot();
                }
            }));
        }

        private int GetRandomSpeed()
        {
            int randomNumber = random.Next(1, 101); 

            if (randomNumber <= 40) // 40% вероятность
            {
                return 7;
            }
            else if (randomNumber <= 70) // 30% вероятность
            {
                return 10;
            }
            else if (randomNumber <= 90) // 20% вероятность
            {
                return 5;
            }
            else // 10% вероятность
            {
                return 15;
            }
        }

        private void UpdateOpponents(CancellationToken cancellationToken)
        {
            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                this.BeginInvoke(new Action(() =>
                {
                    var opponentsCopy = opponents.ToList();
                    foreach (var opponent in opponentsCopy)
                    {
                        int speed = opponent.Speed;

                        if (opponent.PictureBox.Top <= this.ClientSize.Height)
                        {
                            int verticalSpeed = speed; // Vertical movement speed
                            int horizontalSpeed = 2 * speed; // Horizontal movement speed

                            int margin = 20;
                            int right_margin = 150;

                            if (opponent.PictureBox.Left <= margin && opponent.isMovingLeft)
                            {
                                opponent.isMovingLeft = false;
                            }
                            else if (opponent.PictureBox.Left >= this.ClientSize.Width - right_margin - opponent.PictureBox.Width && !opponent.isMovingLeft)
                            {
                                opponent.isMovingLeft = true;
                            }

                            if (opponent.isMovingLeft)
                            {
                                horizontalSpeed = -speed; // Negative speed for left movement
                            }

                            // Update opponent's position
                            opponent.PictureBox.Left += horizontalSpeed;
                            opponent.PictureBox.Top += verticalSpeed;

                            if (player.CheckCollision(opponent.PictureBox))
                            {
                                // Handle collision with player
                                playerLives--;
                                score = Math.Max(score - 10, 0);

                                UpdateScoreLabel();
                                UpdateLivesLabel(playerLives);

                                opponent.PictureBox.Visible = false;
                                AnimateBlinkOpponent(opponent);

                                opponents.Remove(opponent);
                                Controls.Remove(opponent.PictureBox);
                                opponent.Dispose();
                                break; 
                            }
                            else
                            {
                                foreach (var otherOpponent in opponentsCopy)
                                {
                                    if (otherOpponent != opponent && opponent.CheckCollision(otherOpponent.PictureBox))
                                    {
                                        // Handle collision between opponents
                                        opponents.Remove(opponent);
                                        opponents.Remove(otherOpponent);

                                        Controls.Remove(opponent.PictureBox);
                                        Controls.Remove(otherOpponent.PictureBox);

                                        opponent.Dispose();
                                        otherOpponent.Dispose();
                                        break; 
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Remove opponent when it goes beyond the form's height
                            opponents.Remove(opponent);
                            Controls.Remove(opponent.PictureBox);
                            opponent.Dispose();
                        }
                    }
                    
                    opponentsCopy.Clear();
                    if (opponents.Count == 0)
                    {
                        CreateOpponents();
                    }

                    
                }));

                Thread.Sleep(50);
            }
        }

        private async void AnimateBlinkOpponent(Opponent opponent)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                if (gameTimeInSeconds < 1) break;
                opponent.PictureBox.Visible = !opponent.PictureBox.Visible;
                await Task.Delay(blinkInterval);
            }

            opponents.Remove(opponent);
            Controls.Remove(opponent.PictureBox);
            opponent.Dispose();
        }

        private async void AnimateBlinkPlayer(Player player)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                if (gameTimeInSeconds < 1) break;
                player.PictureBox.Visible = !player.PictureBox.Visible;
                await Task.Delay(blinkInterval);
            }

        }

        private void UpdateProjectiles(CancellationToken cancellationToken)
        {
            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                List<PictureBox> projectilesCopy = new List<PictureBox>();
                List<PictureBox> opponentsProjectilesCopy = new List<PictureBox>();
                List<Opponent> opponentsCopy = new List<Opponent>();

                this.BeginInvoke(new Action(() =>
                {
                    if (!isRunning) return;
                    projectilesCopy.AddRange(player.projectiles);
                    opponentsCopy.AddRange(opponents);

                    foreach (Opponent opponent in opponentsCopy)
                    {
                        opponentsProjectilesCopy.AddRange(opponent.projectiles);
                    }
                }));

                if (!isRunning) break;

                Thread.Sleep(20);

                foreach (PictureBox projectile in opponentsProjectilesCopy)
                {
                    if (projectile.Bounds.IntersectsWith(player.PictureBox.Bounds))
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            AnimateBlinkPlayer(player);
                            playerLives--;
                            UpdateLivesLabel(playerLives);

                            if (playerLives <= 0)
                            {
                                GameOver();
                                return;
                            }

                            foreach (Opponent opponent in opponentsCopy)
                            {
                                if ((projectile.Top <= 0) || (projectile.Top >= this.ClientSize.Height))
                                {
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        if (opponent.projectiles.Contains(projectile))
                                        {
                                            opponent.projectiles.Remove(projectile);
                                            Controls.Remove(projectile);
                                            projectile.Dispose();
                                        }
                                    }));
                                }

                                if (opponent.projectiles.Contains(projectile))
                                {
                                    opponent.projectiles.Remove(projectile);
                                    Controls.Remove(projectile);
                                    projectile.Dispose();
                                    break; // Добавлен break здесь
                                }
                            }
                        }));
                    }

                    foreach (var playerProjectile in projectilesCopy)
                    {
                        if (playerProjectile.Bounds.IntersectsWith(projectile.Bounds))
                        {
                            score += 3;
                            this.BeginInvoke(new Action(() =>
                            {
                                foreach (Opponent opponent in opponentsCopy)
                                {
                                    if (opponent.projectiles.Contains(projectile))
                                    {
                                        opponent.projectiles.Remove(projectile);
                                        Controls.Remove(projectile);
                                        projectile.Dispose();
                                        break; // Добавлен break здесь
                                    }
                                }

                                if (player.projectiles.Contains(playerProjectile))
                                {
                                    player.projectiles.Remove(playerProjectile);
                                    Controls.Remove(playerProjectile);
                                    playerProjectile.Dispose();
                                }
                            }));
                        }
                    }
                }

                foreach (PictureBox projectile in projectilesCopy)
                {
                    foreach (Opponent opponent in opponentsCopy)
                    {
                        if (projectile.Bounds.IntersectsWith(opponent.PictureBox.Bounds))
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (opponents.Contains(opponent))
                                {
                                    score += 5;
                                    AnimateBlinkOpponent(opponent);
                                    UpdateScoreLabel();
                                    //opponents.Remove(opponent);
                                }
                            }));

                            this.BeginInvoke(new Action(() =>
                            {
                                if (player.projectiles.Contains(projectile))
                                {
                                    player.projectiles.Remove(projectile);
                                    Controls.Remove(projectile);
                                    projectile.Dispose();
                                }
                            }));

                            break;
                        }
                    }

                    if ((projectile.Top <= 0) || (projectile.Top >= this.ClientSize.Height))
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (player.projectiles.Contains(projectile))
                            {
                                player.projectiles.Remove(projectile);
                                Controls.Remove(projectile);
                                projectile.Dispose();
                            }
                        }));
                    }
                }

                this.BeginInvoke(new Action(() => Invalidate()));
                Thread.Sleep(20);

                projectilesCopy.Clear();
                opponentsProjectilesCopy.Clear();
                opponentsCopy.Clear();
            }
        }
        private void UpdatePlayer(CancellationToken cancellationToken)
        {
            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                BeginInvoke(new Action(() => Invalidate()));
                Thread.Sleep(50);
            }
        }

        private void Pause()
        {
            isRunning = false;
            timer.Stop();

            foreach (var opponent in opponents)
            {
                opponent.StopProjectileTimer();
                opponent.StopAutoShootTimer();
            }
            player.StopProjectileTimer();
            player.StopAutoShootTimer();

            DialogResult result = MessageBox.Show($"Your Score: {score}\nPlay Time: {gameTimeInSeconds}s\nDo you want to continue?", "Pause...", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                isRunning = true;
                timer.Start();

                foreach (var opponent in opponents)
                {
                    opponent.StartProjectileTimer();
                    opponent.StartAutoShootTimer();
                }
                player.StartProjectileTimer();
                player.StartAutoShootTimer();
            }
            else
            {
                opponents.Clear();
                player.Dispose();
                Controls.Remove(player.PictureBox);
                Close();
            }
        }

        private void GameOver()
        {
            opponentsCancellation?.Cancel();
            projectilesCancellation?.Cancel();
            playerCancellation?.Cancel();

            opponentsCancellation.Dispose();
            projectilesCancellation.Dispose();
            playerCancellation.Dispose();

            opponentsThread.Join();
            playerThread.Join();
            projectilesThread.Join();

            opponentsThread = null;
            playerThread = null;
            projectilesThread = null;

            foreach (Opponent opponent in opponents)
            {
                Controls.Remove(opponent.PictureBox);
                opponent.Dispose();
            }
            opponents.Clear();

            Controls.Remove(player.PictureBox);
            player.Dispose();


            timer.Stop();

            DialogResult result = MessageBox.Show($"Your Score: {score}\nPlay Time: {gameTimeInSeconds}s\nDo you want to try again?", "Game Over!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                ResetGame();
            }
            else
            {
                Close(); 
            }
        }

        private void ResetGame()
        {
            gameTimeInSeconds = 0;
            score = 0;

            playerLives = START_PLAYER_LIVES;
            UpdateLivesLabel(playerLives);

            InitializeGame();
        }

        private void UpdateLivesLabel(int curLives)
        {
            switch (curLives)
            {
                case 0:
                    live.Visible = false;
                    live2.Visible = false;
                    live3.Visible = false;
                    break;
                case 1:
                    live.Visible = false;
                    live2.Visible = false;
                    live3.Visible = true;
                    break;
                case 2:
                    live.Visible = false;
                    live2.Visible = true;
                    live3.Visible = true;
                    break;
                case 3:
                    live.Visible = true;
                    live2.Visible = true;
                    live3.Visible = true;
                    break;

            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int margin = 20;
            int right_margin = 150;

            if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && player.PictureBox.Left >= margin)
                player.PictureBox.Left -= player.Speed;
            else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && player.PictureBox.Left <= this.ClientSize.Width - right_margin - player.PictureBox.Width)
                player.PictureBox.Left += player.Speed;
            else if ((e.KeyCode == Keys.Space) && player.projectiles.Count <= 10)
                player.Shoot();
            else if ((e.KeyCode == Keys.Up))
                player.Speed += 3;
            else if ((e.KeyCode == Keys.Down))
            {
                if (player.Speed > 3)
                    player.Speed -= 3;
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape) Pause();
        }
     
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (opponentsCancellation != null && !opponentsCancellation.IsCancellationRequested)
            {
                opponentsCancellation?.Cancel();
                opponentsCancellation.Dispose();
            }

            if (projectilesCancellation != null && !projectilesCancellation.IsCancellationRequested)
            {
                projectilesCancellation.Cancel();
                projectilesCancellation.Dispose();
            }

            if (playerCancellation != null && !playerCancellation.IsCancellationRequested)
            {
                playerCancellation?.Cancel();
                playerCancellation.Dispose();
            }

            if (opponentsThread != null)
            {
                opponentsThread.Join();
                opponentsThread = null;
            }

            if (playerThread != null)
            {
                playerThread.Join();
                playerThread = null;
            }

            if (projectilesThread != null)
            {
                projectilesThread.Join();
                projectilesThread = null;
            }
         
            foreach (var opponent in opponents)
            {
                opponent.PictureBox.Dispose();
                opponent.Dispose();
            }

            player.PictureBox.Dispose();
        }

        private void UpdateScoreLabel()
        {
            scoreLabel.Text = $"SCORE: {score}";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            gameTimeInSeconds++;
            timeLabel.Text = $"TIME: {gameTimeInSeconds} s";
            UpdateScoreLabel();

            if (gameTimeInSeconds % 3 == 0) score++;
        }
       
    }


    public class GameObject
    {
        public PictureBox PictureBox { get; private set; }
        public int Speed { get; set; } = 25;
        public List<PictureBox> projectiles;
        public Color ProjectileColor { get; private set; }

        System.Windows.Forms.Timer autoShootTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer projectileTimer;

        public GameObject(ref Image image, Size size, PictureBoxSizeMode sizeMode, Color backColor, Color projectileColor)
        {
            PictureBox = new PictureBox();
            PictureBox.Size = size;
            PictureBox.SizeMode = sizeMode;
            PictureBox.Image = image;
            PictureBox.BackColor = backColor;
            ProjectileColor = projectileColor;

            projectiles = new List<PictureBox>();
        }

        public bool CheckCollision(PictureBox opponent)
        {
            return PictureBox.Bounds.IntersectsWith(opponent.Bounds);
        }

        public virtual void Shoot()
        {
            PictureBox projectile = new PictureBox();
            projectile.Size = new Size(15, 15);
            projectile.BackColor = ProjectileColor;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, projectile.Width, projectile.Height);
            projectile.Region = new Region(path);

            int projectileTopPosition = PictureBox.Top;

            if (this is Player)
            {
                projectileTopPosition = PictureBox.Top - projectile.Height;
            }
            else if (this is Opponent)
            {
                projectileTopPosition = PictureBox.Bottom;
            }

            projectile.Location = new Point(PictureBox.Left + PictureBox.Width / 2, projectileTopPosition);

            projectiles.Add(projectile);

            if (PictureBox.Parent != null)
            {
                PictureBox.Parent.Controls.Add(projectile);

                if (projectileTimer == null)
                {
                    projectileTimer = new System.Windows.Forms.Timer();
                    projectileTimer.Interval = 20;
                    projectileTimer.Tick += ProjectileTimer_Tick;
                    projectileTimer.Start();
                }
            }
        }

        private void ProjectileTimer_Tick(object sender, EventArgs e)
        {
            if (PictureBox.Parent != null)
            {
                var projectilesCopy = new List<PictureBox>(projectiles);

                foreach (PictureBox projectile in projectilesCopy)
                {
                    int direction = (projectile.Top < PictureBox.Top) ? -1 : 1;
                    int speed = Speed;
                    if (this is Player) speed = 5;
                    projectile.Top += direction * speed;

                    if (projectile.Top < 0 || projectile.Top > PictureBox.Parent.Height)
                    {
                        projectiles.Remove(projectile);
                        projectile.Dispose();
                        PictureBox.Parent.Controls.Remove(projectile);
                    }
                }

                projectilesCopy.Clear();
            }
        }

        public void AutoShoot()
        {
            autoShootTimer.Interval = 2000; 
            autoShootTimer.Tick += (sender, e) =>
            {
                Shoot();
            };
            autoShootTimer.Start();
        }

        public void Dispose()
        {
            StopProjectileTimer();
            ClearProjectiles();
            PictureBox.Dispose();

            GC.Collect(0);
        }

        private void ClearProjectiles()
        {
            foreach (PictureBox projectile in projectiles)
            {
                projectile.Dispose();
            }

            projectiles.Clear();
        }

        public void StopProjectileTimer()
        {
            if (projectileTimer != null)
            {
                projectileTimer.Stop();
                projectileTimer.Dispose();
            }
        }

        public void StopAutoShootTimer()
        {
            if (autoShootTimer != null)
            {
                autoShootTimer.Stop();
            }
        }

        public void StartProjectileTimer()
        {
            if (projectileTimer != null)
            {
                projectileTimer.Start();
            }
        }

        public void StartAutoShootTimer()
        {
            if (autoShootTimer != null)
            {
                autoShootTimer.Start();
            }
        }

    }

    public class Player : GameObject
    {
        public Player()
            : base(ref Form1.playerImage, new Size(140, 140), PictureBoxSizeMode.StretchImage, Color.Transparent, Color.DarkGreen)
        {
        }
    }

    public class Opponent : GameObject
    {
        public System.Windows.Forms.Timer DirectionChangeTimer { get; private set; }
        public bool isMovingLeft = false;
        Random random = new Random();

        public Opponent()
            : base(ref Form1.opponentImage, new Size(140, 140), PictureBoxSizeMode.StretchImage, Color.Transparent, Color.Red)
        {
            DirectionChangeTimer = new System.Windows.Forms.Timer();
            DirectionChangeTimer.Interval = 500; 
            DirectionChangeTimer.Tick += (sender, e) =>
            {
                isMovingLeft = random.Next(2) == 0; 
            };
        }
    }


}
