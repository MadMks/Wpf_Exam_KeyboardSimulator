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


// TODO #1 задержка визуального нажатия Шифта (скорее из-за поиска кнопки в большем кол-ве циклов).
// TODO #2 добавить 3-ю клавиатуру для Caps Lock (нажатие Caps отличается от нажатия Shift).


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

            //this.buttons = new List<Button>();
            //this.buttons.AddRange(this.firstRowOfButtons.Children as IEnumerable<Button>);
            //this.buttons.Add(this.firstRowBackspace);


            this.PreviewTextInput += MainWindow_PreviewTextInput;
            

            this.textUserTyped.TextChanged += TextUserTyped_TextChanged;

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;

            this.KeyBindingToThePressingCommand();

            // test
            this.KeyUp += MainWindow_KeyUp;
            this.KeyDown += MainWindow_KeyDown;

            //btnTests.Click += BtnTests_Click;
            //btnTests.KeyDown += BtnTests_KeyDown;
            //btnTests.KeyUp += BtnTests_KeyUp;


            // Узнаем состояние CapsLock. Запоминаем.
            if (Keyboard.IsKeyToggled(Key.Capital) == true)
            {
                Console.WriteLine("cap true");
                this.IsCapsLockIsOn = true;
            }
            else if (Keyboard.IsKeyToggled(Key.Capital) == false)
            {
                Console.WriteLine("cap false");
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

        }

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

            Console.WriteLine("MainWindow_PreviewKeyUp ");
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


            Console.WriteLine("PreviewKeyDown ");
            //this.textUserTyped.Text += e.Key;
            this.lastKeyPressed = e.Key;
            if (this.stackPanelButtons.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.lastKeyPressed);
            }
            else if (this.stackPanelButtonsWithShift.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtonsWithShift(this.lastKeyPressed);
            }
            
            //currentPressedButton.Focus();
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
            //CommandBindingPressBtn_Executed(sender, new RoutedEventArgs(e.RoutedEvent) as ExecutedRoutedEventArgs);

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
                    // TODO разукрашивание ошибок

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
                // Тут ошибка с выходом за рамки массива. верхний текстбокс.
                // Вроде ошибку убрал.
                if (this.textNeedToType.Text.Length > 0)
                {
                    this.textTyped.Text += this.textNeedToType.Text[0];
                    this.textNeedToType.Text = this.textNeedToType.Text.Substring(1);
                }

                // TODO for RichTextBox
                //this.textTyped.AppendText(this.textNeedToType.Text[0].ToString());
                //this.textNeedToType.Text = this.textNeedToType.Text.Substring(1);

                isAddingSymbols = true;
            }
            // иначе если Backspace
            else if ((int)e.Key == 2)
            {
                if (this.textTyped.Text.Length > 0)
                {
                    // Изменения верхнего ряда (текстБоксов).
                    this.textNeedToType.Text = String.Concat(this.textTyped.Text[textTyped.Text.Length - 1], this.textNeedToType.Text);
                    this.textTyped.Text = textTyped.Text.Substring(0, textTyped.Text.Length - 1);

                    isAddingSymbols = false;
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

                SwitchingVisualButtonsToStateWithoutPressing();

                MessageBox.Show("Конец тренировки: результаты");

                // TODO убрать фокус с текстБокса
                // например на кнопку старт
                this.btnStart.Focus();
            }
            else if (this.textUserTyped.Text.Length > this.textTyped.Text.Length)
            {
                //this.btnStop.IsEnabled = false;

                //this.timer.Stop();

                //MessageBox.Show("Чак Норрис");
            }
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
            //Console.WriteLine(e.Text);

            if (this.btnStart.IsEnabled == false)
            {
                //this.textUserTyped.Text += e.Text;
                
            }
            //this.textUserTyped.Focus();

            if (this.btnStart.IsEnabled == false)
            {

            }

            Console.WriteLine("MainWindow_PreviewTextInput ");
            //Console.WriteLine(e.Text);
            //Console.WriteLine(sender.ToString());
        }



        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.ZeroingData();

            this.btnStart.IsEnabled = false;

            this.textNeedToType.Text = "Lorem ipsum";
            //this.textNeedToType.Text = this.StringGeneration(); // TODO param StringGeneration(param)

            this.textUserTyped.Focus();

            this.timer.Start();
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
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.btnStop.IsEnabled = false;

            this.timer.Stop();
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
    }
}
