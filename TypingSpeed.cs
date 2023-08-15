using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Typing_Speed {
    public partial class TypingSpeed : Form {
        // Constants for size/positions
        private readonly int formWidth = 800;
        private readonly int formHeight = 500;
        private readonly int titleLocationX = 85;
        private readonly int titleLocationY = 50;
        private readonly int fontSize = 12;
        private readonly int inputBoxWidth = 200;
        private readonly int inputBoxHeight = 100;
        private readonly int inputBoxLocation = 230;
        private readonly int startGameButtonLocationX = 330;
        private readonly int startGameButtonLocationY = 200;
        private readonly int textWidth = 500;
        private readonly int textHeight = 30;
        private readonly int textLocationX = 130;
        private readonly int textLocationY = 100;
        private readonly int textBoxLabelLocationX = 140;
        private readonly int textBoxLabelLocationY = 233;
        private readonly int scoreLocationX = 520;
        private readonly int scoreLocationY = 200;
        private readonly int timerLocationY = 170;
        private int minuteInSeconds = 60;

        // Font
        private static Font normalFont = new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
        private static Padding margin = new Padding(5, 0, 5, 0);
        private static Padding padding = new Padding(3, 3, 3, 3);
        private static Size minSize = new Size(100, 30);
        private static Color back = Color.Gray;
        private static Color border = Color.Black;
        private static Color fore = Color.White;

        // Attributes
        private string[] words = { "wheat", "confidence", "budget", "transport", "press", "scream", "late", "research", "shake", "brush" };
        private string randomText;
        private TextBox userInput;
        private Button startGameButton;
        private Label title;
        private Label timer;
        private int currentPosition = 0;
        private RichTextBox text;
        private int currentInputTextPosition = 0;
        private int randomTextPosition = 0;
        private bool stopThread = false;
        private Label score;
        private int wordsCounter = 0;
        private int greenLettersCounter = 0;

        // Constructor
        public TypingSpeed() {
            InitializeComponent();
            createRandomText();
            formDisplaySettings();
            displayStartGameWindow();
            createUserInputBox();
            generateStartGameButton();
        }

        // Methods 
        private void createRandomText() {
            int counter = 0;
            Random random = new Random();
            randomText += words[random.Next(0, words.Length)] + " ";
            while (counter < words.Length - 1) {
                int index = random.Next(0, words.Length);
                if (!randomText.Contains(words[index])) {
                    randomText += words[index] + " ";
                    ++counter;
                }
            }
        }

        private void formDisplaySettings() {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximumSize = new Size(formWidth, formHeight);
            this.MinimumSize = new Size(formWidth, formHeight);
            this.WindowState = FormWindowState.Normal;
            this.KeyPreview = true;
        }

        private void displayStartGameWindow() { 
            title = new Label();
            title.Text = "Welcome to the Typing Speed game! Press the 'Start' button in order to begin the challenge.";
            title.Location = new Point(titleLocationX, titleLocationY);
            title.Font = new Font("Calibri", fontSize);
            title.AutoSize = true;
            title.Visible = true;
            title.BorderStyle = BorderStyle.Fixed3D;
            title.ForeColor = Color.Black;
            title.Padding = new Padding(6);
            this.Controls.Add(title);
        }

        private void createUserInputBox() {
            userInput = new TextBox();
            userInput.Name = "UserInput";
            userInput.Width = inputBoxWidth;
            userInput.Height = inputBoxHeight;
            userInput.Font = new Font("Calibri", fontSize);
            userInput.Location = new Point(inputBoxLocation, inputBoxLocation);
            userInput.Visible = false;
            userInput.TextChanged += inputBoxAction;
            userInput.KeyPress += UserInput_KeyPress;
            userInput.KeyDown += UserInput_KeyDown;
            this.Controls.Add(userInput);
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Back) {
                if (currentPosition > 0) {
                    --randomTextPosition;
                    --currentInputTextPosition;
                    --currentPosition;
                    text.Select(currentPosition, 1);
                    if (text.SelectionColor == Color.Green) {
                        --greenLettersCounter;
                    }
                    text.SelectionColor = Color.Black;
                }
            }
        }

        private void UserInput_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == ' ') {
                text.Select(currentPosition - 1, 1);
                if (text.SelectionColor == Color.Green && greenLettersCounter == userInput.Text.Length) {
                    ++wordsCounter;
                    score.Text = "Score: " + wordsCounter;
                    greenLettersCounter = 0;
                    randomTextPosition = 0;
                    currentInputTextPosition = 0;
                    ++currentPosition;
                    userInput.Clear();
                }
                e.Handled = true;
            }
        }

        private void generateStartGameButton() {
            startGameButton = new Button();
            startGameButton.Name = "startGameButton";
            startGameButton.AutoSize = true;
            startGameButton.Location = new Point(startGameButtonLocationX, startGameButtonLocationY);
            startGameButton.Text = "Start Game";
            startGameButton.Font = normalFont;
            startGameButton.BackColor = border;
            startGameButton.ForeColor = fore;
            startGameButton.FlatAppearance.BorderColor = back;
            startGameButton.FlatStyle = FlatStyle.Flat;
            startGameButton.Margin = margin;
            startGameButton.Padding = padding;
            startGameButton.MinimumSize = minSize;
            startGameButton.Click += new EventHandler(startGameButton_Click);
            this.Controls.Add(startGameButton);
        }

        private void startGameButton_Click(object sender, EventArgs e) {
            title.Visible = false;
            startGameButton.Visible = false;
            userInput.Visible = true;
            displayRandomText();
            displayTextBoxLabel();
            displayScore();
            displayTimer();
            Thread timer = new Thread(timerControl);
            timer.Start();
        }

        private void displayRandomText() {
            text = new RichTextBox();
            text.Font = new Font("Calibri", fontSize);
            text.ReadOnly = true;
            text.Size = new Size(textWidth, textHeight);
            text.BorderStyle = BorderStyle.Fixed3D;
            text.ForeColor = Color.Black;
            text.Location = new Point(textLocationX, textLocationY);
            text.Text = randomText;
            text.Visible = true;
            text.Padding = new Padding(6);
            this.Controls.Add(text);
        }

        private void displayTextBoxLabel() {
            Label textBoxLabel = new Label();
            textBoxLabel.Text = "Start typing:";
            textBoxLabel.Location = new Point(textBoxLabelLocationX, textBoxLabelLocationY);
            textBoxLabel.Font = new Font("Calibri", fontSize);
            textBoxLabel.AutoSize = true;
            textBoxLabel.Visible = true;
            textBoxLabel.ForeColor = Color.Black;
            this.Controls.Add(textBoxLabel);
        }

        private void displayScore() {
            score = new Label();
            score.Text = "Score: 0";
            score.Location = new Point(scoreLocationX, scoreLocationY);
            score.Font = new Font("Calibri", fontSize);
            score.AutoSize = true;
            score.Visible = true;
            score.ForeColor = Color.Black;
            this.Controls.Add(score);
        }

        private void displayTimer() {
            timer = new Label();
            timer.Location = new Point(scoreLocationX, timerLocationY);
            timer.Font = new Font("Calibri", fontSize);
            timer.AutoSize = true;
            timer.Visible = true;
            timer.ForeColor = Color.Black;
            this.Controls.Add(timer);
        }

        private void timerControl() {
            int counter = minuteInSeconds;
            while (!stopThread) {
                this.Invoke(new Action(() => {
                    if (counter >= 0) {
                        timer.Text = "Remaining time: " + counter + " seconds";
                        --counter;
                    }
                    else {
                        endGame();
                    }
                }));
                Thread.Sleep(1000);
            }
        }

        private void inputBoxAction(object sender, EventArgs e) {
            text.SelectAll();
            if (currentPosition < randomText.Length) {
                if (userInput.Text.Length > randomTextPosition && userInput.Text[currentInputTextPosition] == randomText[currentPosition]) {
                    text.Select(currentPosition, 1);
                    text.SelectionColor = Color.Green;
                    ++currentPosition;
                    ++currentInputTextPosition;
                    ++randomTextPosition;
                    ++greenLettersCounter;
                } else if (userInput.Text.Length > randomTextPosition && userInput.Text[currentInputTextPosition] != randomText[currentPosition]) {
                    text.Select(currentPosition, 1);
                    text.SelectionColor = Color.Red;
                    ++currentPosition;
                    ++currentInputTextPosition;
                    ++randomTextPosition;
                }
            }
            else {
                endGame();
            }
        }

        private void endGame() {
            stopThread = true;
            MessageBox.Show("Game Over! You typed: " + wordsCounter + " correct words!");
            Application.Exit();
        }
    }
}
