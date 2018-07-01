using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


// TODO #1 задержка визуального нажатия Шифта. Из-за визуального нажатия на клавиатуре нижнего регистра,
        // которая в данный момент Visibility.Collapsed (в данный момент не видно).
        // Визуальное нажатие появляется после сработки зажатия Шифта (RepeatButton).
// TODO #2 добавить 3-ю клавиатуру для Caps Lock (нажатие Caps отличается от нажатия Shift).
// TODO #3 Добавить все Button в одну коллекцию (одна коллекция для каждой клавиатуры).
        // Искать кнопки в коллекции (будет меньше кода).


namespace Task_KeyboardSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        private int numberOfMistakes;
        private int numberOfSeconds;
        /// <summary>
        /// Добавление символов 
        /// (для проверки символов,
        /// чтоб не реагировать на Backspace).
        /// </summary>
        private bool isAddingSymbols;
        private Key lastKeyPressed;
        private Button currentPressedButton;
        //private List<Button> buttons;
        private bool IsCapsLockIsOn;

        private List<int> listOfErrorIndicesForUserTyped;
        private List<int> listOfErrorIndicesForTyped;

        //private List<char> characterList;
        /// <summary>
        /// Список символов нижнего регистра.
        /// </summary>
        private string characterListLower;
        /// <summary>
        /// Список символов верхнего регистра.
        /// </summary>
        private string characterListUpper;
        /// <summary>
        /// Список символов, цифры и символы.
        /// </summary>
        //private string numberAndCharacterList;

        // TODO const
        private const int NUMBER_OF_CHARACTERS_IN_STRING = 15;

        private const int NUMBER_OF_SPACES_IN_LINE = 6;

        private bool isGenerateCaseSensitiveString;



        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            numberOfMistakes = 0;
            numberOfSeconds = 0;

            this.listOfErrorIndicesForUserTyped = new List<int>();
            this.listOfErrorIndicesForTyped = new List<int>();


            this.characterListLower = "fjdksla;ghrueiwoqpvmc,x.z/bn4738291056[]'\\`-=";
            this.characterListUpper = "FJDKSLA:GHRUEIWOQPVMC<X>Z?BN$&#*@(!)%^{}\"|~_+";

            

            //this.isGenerateCaseSensitiveString = 



            this.PreviewTextInput += MainWindow_PreviewTextInput;
           

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;

            this.KeyBindingToThePressingCommand();

            
            this.KeyUp += MainWindow_KeyUp;
            this.KeyDown += MainWindow_KeyDown;



            // Узнаем состояние CapsLock. Запоминаем.
            if (Keyboard.IsKeyToggled(Key.Capital) == true)
            {
                //Console.WriteLine("cap true");
                this.IsCapsLockIsOn = true;
            }
            else if (Keyboard.IsKeyToggled(Key.Capital) == false)
            {
                //Console.WriteLine("cap false");
                this.IsCapsLockIsOn = false;
            }

            if (IsCapsLockIsOn)
            {
                this.stackPanelButtonsWithShift.Visibility = Visibility.Visible;
                this.stackPanelButtons.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.stackPanelButtonsWithShift.Visibility = Visibility.Collapsed;
                this.stackPanelButtons.Visibility = Visibility.Visible;
            }


            // test
            //this.sliderDifficulty.ValueChanged += SliderDifficulty_ValueChanged;

        }


        // test #1
        //private void SliderDifficulty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    //Console.WriteLine(sliderDifficulty.TickFrequency);
        //    Console.WriteLine(Convert.ToInt32(e.NewValue));


        //    //Console.WriteLine(this.checkBoxCaseSensitive.IsChecked);
        //}

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
       {
            // Переключение на символы с помощью Shift. (нажатие).
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                if (this.IsCapsLockIsOn)
                {
                    // Нажимаем шифт.
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Collapsed;
                    this.stackPanelButtons.Visibility = Visibility.Visible;
                }
                else if (!this.IsCapsLockIsOn)
                {
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Visible;
                    this.stackPanelButtons.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Переключение на буквы верхнего регистра с помощью Caps Lock.
            if (e.Key == Key.CapsLock)
            {
                if (IsCapsLockIsOn == true)
                {
                    IsCapsLockIsOn = false;
                }
                else if (IsCapsLockIsOn == false)
                {
                    IsCapsLockIsOn = true;
                }

                if (IsCapsLockIsOn)
                {
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Visible;
                    this.stackPanelButtons.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Collapsed;
                    this.stackPanelButtons.Visibility = Visibility.Visible;
                }
            }

            ButtonPressUp(this.currentPressedButton);

            SwitchingVisualButtonsToStateWithoutPressing();
        }

        private void ButtonPressUp(Button button)
        {
            if (button != null)
            {
                typeof(Button).GetMethod(
                    "set_IsPressed",
                    BindingFlags.Instance | BindingFlags.NonPublic
                    ).Invoke(button, new object[] { false });
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Переключение на символы с помощью Shift. (отпускание).
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                if (this.IsCapsLockIsOn)
                {
                    // Отпускаем шифт.
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Visible;
                    this.stackPanelButtons.Visibility = Visibility.Collapsed;
                }
                else if (!this.IsCapsLockIsOn)
                {
                    this.stackPanelButtonsWithShift.Visibility = Visibility.Collapsed;
                    this.stackPanelButtons.Visibility = Visibility.Visible;
                }
            }
        }

        

        private Button SearchingVisualButtons(Key lastKey)
        {
            Console.WriteLine("                  " + lastKey.ToString());

            // HACK много одинакового кода #1. -> заменить на: поместить все кнопки в одну коллекцию.
            #region ButtonSearch
            // 1 ряд
            foreach (Button button in this.firstRowOfButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.firstRowBackspaceButton.Tag.ToString() == lastKey.ToString())
            {
                return this.firstRowBackspaceButton;
            }
            // 2 ряд
            else if (this.secondRowTabButton.Tag.ToString() == lastKey.ToString())
            {
                return this.secondRowTabButton;
            }
            foreach (Button button in this.secondRowOfButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.secondRowBackslashButton.Tag.ToString() == lastKey.ToString())
            {
                return secondRowBackslashButton;
            }
            // 3 ряд
            if (this.thirdRowCapitalButton.Tag.ToString() == lastKey.ToString())
            {
                return this.thirdRowCapitalButton;
            }
            foreach (Button button in this.thirdRowOfButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.thirdRowReturnButton.Tag.ToString() == lastKey.ToString())
            {
                return this.thirdRowReturnButton;
            }
            // 4 ряд
            if (this.fourthRowLShiftButton.Tag.ToString() == lastKey.ToString())
            {
                return this.fourthRowLShiftButton;
            }
            foreach (Button button in this.fourthRowOfButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.fourthRowRShiftButton.Tag.ToString() == lastKey.ToString())
            {
                return this.fourthRowRShiftButton;
            }
            // 5 ряд
            foreach (Button button in this.fifthRowLeftButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.fifthRowSpaceButton.Tag.ToString() == lastKey.ToString())
            {
                return this.fifthRowSpaceButton;
            }
            foreach (Button button in this.fifthRowRightButtons.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            #endregion


            return null;
        }

        private Button SearchingVisualButtonsWithShift(Key lastKey)
        {
            Console.WriteLine("                  " + lastKey.ToString());

            // HACK много одинакового кода #3. -> заменить на: поместить все кнопки в одну коллекцию.
            #region ButtonSearchSecondKeyboard
            // 1 ряд
            foreach (Button button in this.firstRowOfButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.firstRowBackspaceButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.firstRowBackspaceButtonWithShift;
            }
            // 2 ряд
            else if (this.secondRowTabButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.secondRowTabButtonWithShift;
            }
            foreach (Button button in this.secondRowOfButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.secondRowBackslashButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return secondRowBackslashButtonWithShift;
            }
            // 3 ряд
            if (this.thirdRowCapitalButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.thirdRowCapitalButtonWithShift;
            }
            foreach (Button button in this.thirdRowOfButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.thirdRowReturnButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.thirdRowReturnButtonWithShift;
            }
            // 4 ряд
            if (this.fourthRowLShiftButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.fourthRowLShiftButtonWithShift;
            }
            foreach (Button button in this.fourthRowOfButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.fourthRowRShiftButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.fourthRowRShiftButtonWithShift;
            }
            // 5 ряд
            foreach (Button button in this.fifthRowLeftButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            if (this.fifthRowSpaceButtonWithShift.Tag.ToString() == lastKey.ToString())
            {
                return this.fifthRowSpaceButtonWithShift;
            }
            foreach (Button button in this.fifthRowRightButtonsWithShift.Children)
            {
                if (button.Tag != null)
                {
                    if (button.Tag.ToString() == lastKey.ToString())
                    {
                        return button;
                    }
                }
            }
            #endregion


            return null;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                WorkWithSymbolsInBoxes(e);  // уже переместил букву в верхнем текстБоксе.

                //ErrorChecking();    // проверка на ошибку (при этом в нижнем еще нет буквы!!!!!)
            }


            this.lastKeyPressed = e.Key;
            if (this.stackPanelButtons.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.lastKeyPressed);
            }
            else if (this.stackPanelButtonsWithShift.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtonsWithShift(this.lastKeyPressed);
            }
            
            ButtonPressDown();
        }

        private void ButtonPressDown()
        {
            if (currentPressedButton != null)
            {
                typeof(Button).GetMethod(
                    "set_IsPressed",
                    BindingFlags.Instance | BindingFlags.NonPublic
                    ).Invoke(currentPressedButton, new object[] { true });
            }
        }

        private void TextUserTyped_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                ErrorChecking();

                CheckTypingRequiredNumberOfCharacters();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            numberOfSeconds++;

            this.answerNumberOfCharsMin.Text = ComputeNumberOfCharsMin();
        }

        /// <summary>
        /// Вычисление кол-ва набора символов в минуту.
        /// </summary>
        /// <returns></returns>
        private string ComputeNumberOfCharsMin()
        {
            int numberOfCharsMin = (60 / this.numberOfSeconds) * (this.textUserTyped.Text.Length - this.numberOfMistakes);

            return numberOfCharsMin.ToString();
        }



        /// <summary>
        /// Проверка ошибок.
        /// </summary>
        private void ErrorChecking()
        {
            if (textUserTyped.Text.Length > 0 
                && isAddingSymbols)
            {
                if (this.textUserTyped.Text[textUserTyped.Text.Length - 1] != this.textTyped.Text[textTyped.Text.Length - 1])
                {
                    Run run = new Run(this.textUserTyped.Text.Substring(0, this.textUserTyped.Text.Length - 1));
                    run.TextDecorations = this.textUserTyped.TextDecorations;
                    string letterError = this.textUserTyped.Text.Substring(this.textUserTyped.Text.Length - 1);
                    this.textUserTyped.Text = null;
                    this.textUserTyped.Inlines.Add(run);
                    this.textUserTyped.Inlines.Add(new Run(letterError) { Background = Brushes.Red });

                    this.answerNumberOfMistakes.Text = (++numberOfMistakes).ToString();
                }
            }

        }

        /// <summary>
        /// Работа с символами в текстБоксах
        /// </summary>
        /// <param name="e">Параметры события нажатия кнопки.</param>
        private void WorkWithSymbolsInBoxes(KeyEventArgs e)
        {
            if ((int)e.Key > 34
                    && (int)e.Key < 115
                    || (int)e.Key > 117
                    && (int)e.Key < 151
                    || (int)e.Key == 18)
            {
                if (this.textNeedToType.Text.Length > 0)
                {
                    this.textTyped.Inlines.Add(new Run(this.textNeedToType.Text[0].ToString()));
                    string tempStrNeedToType = this.textNeedToType.Text.Substring(1);
                    this.textNeedToType.Inlines.Clear();
                    this.textNeedToType.Inlines.Add(tempStrNeedToType);
                }

                isAddingSymbols = true;
            }
            // иначе если Backspace.
            else if ((int)e.Key == 2)
            {
                // Изменение нижнего ряда (TextBlock).
                if (this.textUserTyped.Text.Length > 0)
                {
                    // При исправлении ошибки - убираем пометку (красный фон, для нижнего textBlock).
                    if (listOfErrorIndicesForUserTyped.Exists(x => x == this.textUserTyped.Text.Length - 1))
                    {
                        this.listOfErrorIndicesForUserTyped.RemoveAt(this.listOfErrorIndicesForUserTyped.FindIndex(x => x == this.textUserTyped.Text.Length - 1));
                    }

                    this.RepaintingAllStoredErrorsWithoutLastLetter(this.textUserTyped, this.listOfErrorIndicesForUserTyped, Brushes.Red);
                }

                // Изменения верхнего ряда (текстБоксов).
                if (this.textTyped.Text.Length > 0)
                {
                    // Удаление последнего символа. // TODO method
                    this.textNeedToType.Text = String.Concat(this.textTyped.Text[textTyped.Text.Length - 1], this.textNeedToType.Text);

                    this.RepaintingAllStoredErrorsWithoutLastLetter(this.textTyped, this.listOfErrorIndicesForTyped, Brushes.Orange);

                    isAddingSymbols = false;
                }
            }
        }

        /// <summary>
        /// Перекрашивание всех сохраненных ошибок без последней буквы (для одного TextBlock).
        /// </summary>
        /// <param name="textBlock">TextBlock в котором нужно перекрасить ошибки.</param>
        /// <param name="errorIndices">Список с индексами ошибок для textBlock</param>
        /// <param name="bgColor">Цвет для закрашивания фона ошибки.</param>
        private void RepaintingAllStoredErrorsWithoutLastLetter(TextBlock textBlock, List<int> errorIndices, Brush bgColor)
        {
            string tempStr = textBlock.Text.Substring(0, textBlock.Text.Length - 1);
            textBlock.Inlines.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                if (errorIndices.Exists(j => i == j))
                {
                    this.AddingLetterAsAnError(textBlock, tempStr[i].ToString(), bgColor);
                }
                else
                {
                    this.AddingLetter(textBlock, tempStr[i].ToString());
                }
            }
        }

        /// <summary>
        /// Проверка равно ли кол-во символов в строках
        /// </summary>
        private void CheckTypingRequiredNumberOfCharacters()
        {
            if (this.textTyped.Text.Length == this.textUserTyped.Text.Length
                && this.textNeedToType.Text.Length == 0)
            {
                this.btnStop.IsEnabled = false;

                this.timer.Stop();

                this.SwitchingVisualButtonsToStateWithoutPressing();

                this.ShowMessageWithResults("completed");

                // TODO убрать фокус с текстБокса
                // например на кнопку старт
                this.btnStart.Focus();
            }
        }

        private void ShowMessageWithResults(string trainingState)
        {
            MessageBox.Show($"Speed: {this.answerNumberOfCharsMin.Text} chars/min\n"
                + $"Fails: {this.answerNumberOfMistakes.Text}",
                $"Training {trainingState}");
        }

        /// <summary>
        /// Переключение визуальных кнопок в состояние без нажатия.
        /// </summary>
        private void SwitchingVisualButtonsToStateWithoutPressing()
        {
            // HACK много одинакового кода #2. -> заменить на: поместить все кнопки в одну коллекцию.
            #region ButtonSearchAndPressUp
            // 1 ряд
            foreach (Button button in this.firstRowOfButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(firstRowBackspaceButton);
            // 2 ряд
            this.ButtonPressUp(secondRowTabButton);
            foreach (Button button in this.secondRowOfButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(secondRowBackslashButton);
            // 3 ряд
            this.ButtonPressUp(thirdRowCapitalButton);
            foreach (Button button in this.thirdRowOfButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(thirdRowReturnButton);
            // 4 ряд
            this.ButtonPressUp(fourthRowLShiftButton);
            foreach (Button button in this.fourthRowOfButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(fourthRowRShiftButton);
            // 5 ряд
            foreach (Button button in this.fifthRowLeftButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(fifthRowSpaceButton);
            foreach (Button button in this.fifthRowRightButtons.Children)
            {
                this.ButtonPressUp(button);
            }
            #endregion

            // HACK много одинакового кода #4. -> заменить на: поместить все кнопки в одну коллекцию.
            #region ButtonSearchAndPressUpSecondKeyboard
            // 1 ряд
            foreach (Button button in this.firstRowOfButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(firstRowBackspaceButtonWithShift);
            // 2 ряд
            this.ButtonPressUp(secondRowTabButtonWithShift);
            foreach (Button button in this.secondRowOfButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(secondRowBackslashButtonWithShift);
            // 3 ряд
            this.ButtonPressUp(thirdRowCapitalButtonWithShift);
            foreach (Button button in this.thirdRowOfButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(thirdRowReturnButtonWithShift);
            // 4 ряд
            this.ButtonPressUp(fourthRowLShiftButtonWithShift);
            foreach (Button button in this.fourthRowOfButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(fourthRowRShiftButtonWithShift);
            // 5 ряд
            foreach (Button button in this.fifthRowLeftButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            this.ButtonPressUp(fifthRowSpaceButtonWithShift);
            foreach (Button button in this.fifthRowRightButtonsWithShift.Children)
            {
                this.ButtonPressUp(button);
            }
            #endregion
        }

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                //if (e.Text == "\u001b")
                //{
                //    //Console.WriteLine(">>>>>>>>         " + e.Text + " -" + Char.IsControl(e.Text[0]));
                //}

                // Если нажали не Backspace, Enter или Esc
                if (e.Text != "\b" && e.Text != "\r" && e.Text != "\u001b")
                {
                    this.AddingLetterToTextUserTyped(e.Text);
                }

                this.RepaintingAllSavedErrors(this.textTyped, this.listOfErrorIndicesForTyped, Brushes.Orange);

                this.CheckTypingRequiredNumberOfCharacters();
            }
        }

        /// <summary>
        /// Перекрашивание всех сохраненных ошибок (для одного TextBlock).
        /// </summary>
        /// <param name="textBlock">TextBlock в котором нужно перекрасить ошибки.</param>
        /// <param name="errorIndices">Список с индексами ошибок для textBlock</param>
        /// <param name="bgColor">Цвет для закрашивания фона ошибки.</param>
        private void RepaintingAllSavedErrors(TextBlock textBlock, List<int> errorIndices, Brush bgColor)
        {
            string tempStr = textBlock.Text;
            textBlock.Inlines.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                if (errorIndices.Exists(j => i == j))
                {
                    this.AddingLetterAsAnError(textBlock, tempStr[i].ToString(), bgColor);
                }
                else
                {
                    this.AddingLetter(textBlock, tempStr[i].ToString());
                }
            }
        }

        private void AddingLetterToTextUserTyped(string letter)
        {
            if (IstheEnteredLetterIsCorrect(letter))
            {
                AddingLetter(this.textUserTyped, letter);
            }
            else
            {
                AddingLetterAsAnError(this.textUserTyped, letter, Brushes.Red);
            }
        }

        private bool IstheEnteredLetterIsCorrect(string letter)
        {
            if (letter[0] == this.textTyped.Text[textTyped.Text.Length - 1])
            {
                return true;
            }

            // TODO method  увеличиваем число допущенных ошибок.
            this.answerNumberOfMistakes.Text = (++numberOfMistakes).ToString();

            // TODO method  addErr(index)
            this.listOfErrorIndicesForUserTyped.Add(this.textUserTyped.Text.Length);
            this.listOfErrorIndicesForTyped.Add(this.textUserTyped.Text.Length);
            return false;
        }

        private void AddingLetter(TextBlock textBlock, string letter)
        {
            textBlock.Inlines.Add(new Run(letter));
        }

        private void AddingLetterAsAnError(TextBlock textBlock, string letter, Brush brush)
        {
            textBlock.Inlines.Add(new Run(letter) { Background = brush });
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.ZeroingData();

            this.btnStart.IsEnabled = false;

            //this.textNeedToType.Text = "Lorem ipsum";
            //this.textNeedToType.Text = ("Lorem ipsum123").ToUpper();
            // TODO param StringGeneration(numberOfLetters, stringWithCaseSensitive)
            this.textNeedToType.Text
                = this.StringGeneration(
                    Convert.ToInt32(this.sliderDifficulty.Value),
                    this.isGenerateCaseSensitiveString);

            this.textUserTyped.Focus();

            this.timer.Start();
        }

        private string StringGeneration(int number, bool IsCaseSensitiveUpper)
        {
            // TODO StringBuilder!?
            // string str = Rand
            Random rand = new Random();
            //Random randomCase = new Random();
            string randomString = null;

            // TODO список индексов пробелов
            List<int> listOfIndicesOfSpace = new List<int>();
            // сгенерировать новые индексы для пробелов
            this.GenerateIndicesForSpaces(listOfIndicesOfSpace);

            if (IsCaseSensitiveUpper)
            {
                int tempRandNumber;

                for (int i = 0; i < NUMBER_OF_CHARACTERS_IN_STRING; i++)
                {
                    tempRandNumber = rand.Next(number);

                    switch (rand.Next(2))   // 2 режима (верхний и нижний регистр)
                    {
                        case 0:
                            // нижний регистр.
                            randomString += this.characterListLower[rand.Next(number)].ToString();
                            break;
                        case 1:
                            // верхний регистр.
                            randomString += this.characterListUpper[rand.Next(number)].ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < NUMBER_OF_CHARACTERS_IN_STRING; i++)
                {
                    if (listOfIndicesOfSpace.Exists(x => x == i))
                    {
                        randomString += " ";
                    }
                    else
                    {
                        randomString += this.characterListLower[rand.Next(number)].ToString();
                    }
                }
            }

            

            return randomString;
        }

        private void GenerateIndicesForSpaces(List<int> listOfIndices)
        {
            Random random = new Random();
            Console.WriteLine("list ind spaces");
            for (int i = 0; i < NUMBER_OF_SPACES_IN_LINE; i++)
            {
                // Ставим рандомные пробелы, кроме первого и последнего символа.
                listOfIndices.Add(random.Next(1, NUMBER_OF_CHARACTERS_IN_STRING - 1));
                Console.WriteLine(listOfIndices[i]);
            }
        }

        /// <summary>
        /// Обнуление данных.
        /// </summary>
        private void ZeroingData()
        {
            this.numberOfMistakes = 0;
            this.numberOfSeconds = 0;

            this.answerNumberOfCharsMin.Text = "0";
            this.answerNumberOfMistakes.Text = "0";

            this.textTyped.Text = "";
            this.textUserTyped.Text = "";

            this.listOfErrorIndicesForUserTyped.Clear();
            this.listOfErrorIndicesForTyped.Clear();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.btnStop.IsEnabled = false;

            this.timer.Stop();

            this.ShowMessageWithResults("is stopped");
        }

        private void btnStop_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.btnStop.IsEnabled == false)
            {
                this.btnStart.IsEnabled = true;

                //this.timer.Start();
            }
        }

        private void btnStart_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.btnStart.IsEnabled == false)
            {
                this.btnStop.IsEnabled = true;

                //this.timer.Stop();
            }
        }

        private void gridTypingBlocks_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                this.textUserTyped.Focus();
            }
        }

        /// <summary>
        /// Началось обучение.
        /// </summary>
        /// <returns>true если началось обучение (если нажата кнопка Start)</returns>
        private bool IsTrainingStarted()
        {
            if (this.btnStart.IsEnabled == false)
            {
                return true;
            }

            return false;
        }

        private void checkBoxCaseSensitive_Checked(object sender, RoutedEventArgs e)
        {
            this.isGenerateCaseSensitiveString = true;
            //Console.WriteLine(this.checkBoxCaseSensitive.IsChecked);
        }

        private void checkBoxCaseSensitive_Unchecked(object sender, RoutedEventArgs e)
        {
            this.isGenerateCaseSensitiveString = false;
        }

        /// <summary>
        /// Обработчик изменения значения слайдера 
        /// (кол-ва используемых символов для вывода строки).
        /// </summary>
        private void sliderDifficulty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // HACK поменял на привязку
            //this.answerDifficulty.Text = Convert.ToInt32(this.sliderDifficulty.Value).ToString();
        }





        // -------------

        /// <summary>
        /// Привязка кнопок клавиатуры к команде "визуального нажатия".
        /// </summary>
        private void KeyBindingToThePressingCommand()
        {
            this.CreationOfACommandOfVisualPressing();

            this.BindingEachButtonToTheCommand();
        }

        private void BindingEachButtonToTheCommand()
        {
            KeyBinding keyBinding;

            //for (int i = 34; i < 35; i++)  // TODO define!
            //{
            //    keyBinding = new KeyBinding();
            //    keyBinding.Key = (Key)i;
            //    keyBinding.Command = WindowCommands.BtnPressRoutedCommand;
            //    this.InputBindings.Add(keyBinding);
            //}

            // Физическая привязка.
            //keyBinding = new KeyBinding();
            //keyBinding.Key = Key.D1;
            //keyBinding.Command = WindowCommands.BtnPressRoutedCommand;
            //this.InputBindings.Add(keyBinding);

            //keyBinding = new KeyBinding();
            //keyBinding.Key = Key.D2;
            //keyBinding.Command = WindowCommands.BtnPressRoutedCommand;
            //this.InputBindings.Add(keyBinding);
        }

        /// <summary>
        /// Создание команды "визуального нажатия на кнопку".
        /// </summary>
        private void CreationOfACommandOfVisualPressing()
        {
            //CommandBinding commandBindingPressBtn = new CommandBinding(WindowCommands.BtnPressRoutedCommand);
            //commandBindingPressBtn.Executed += CommandBindingPressBtn_Executed;
            //this.CommandBindings.Add(commandBindingPressBtn);
        }

        /// <summary>
        /// Метод обработки команды "визуального нажатия на кнопку".
        /// </summary>
        private void CommandBindingPressBtn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //typeof(Button).GetMethod(
            //    "set_IsPressed",
            //    BindingFlags.Instance | BindingFlags.NonPublic
            //    ).Invoke((e.Source as Button), new object[] { true });
            this.currentPressedButton = SearchingVisualButtons(this.lastKeyPressed);
            currentPressedButton.Focus();



            // рабочий вариант с тестовой кнопкой - "2"
            //typeof(Button).GetMethod(
            //    "set_IsPressed",
            //    BindingFlags.Instance | BindingFlags.NonPublic
            //    ).Invoke(btnTests, new object[] { true });

            typeof(Button).GetMethod(
                "set_IsPressed",
                BindingFlags.Instance | BindingFlags.NonPublic
                ).Invoke(currentPressedButton, new object[] { true });


            Console.WriteLine("working" + currentPressedButton.Name);
        }

        
    }
}
