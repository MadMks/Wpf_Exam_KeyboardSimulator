using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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


            this.PreviewTextInput += MainWindow_PreviewTextInput;
            

            this.textUserTyped.TextChanged += TextUserTyped_TextChanged;

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;


            this.KeyBindingToThePressingCommand();
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

            keyBinding = new KeyBinding();
            keyBinding.Key = Key.D1;
            keyBinding.Command = WindowCommands.BtnPressRoutedCommand;
            this.InputBindings.Add(keyBinding);
        }

        /// <summary>
        /// Создание команды "визуального нажатия на кнопку".
        /// </summary>
        private void CreationOfACommandOfVisualPressing()
        {
            CommandBinding commandBindingPressBtn = new CommandBinding(WindowCommands.BtnPressRoutedCommand);
            commandBindingPressBtn.Executed += CommandBindingPressBtn_Executed;
            this.CommandBindings.Add(commandBindingPressBtn);
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

            typeof(Button).GetMethod(
                "set_IsPressed",
                BindingFlags.Instance | BindingFlags.NonPublic
                ).Invoke(btnTests, new object[] { true });

            //typeof(Button).GetMethod(
            //    "set_IsPressed",
            //    BindingFlags.Instance | BindingFlags.NonPublic
            //    ).Invoke(SearchingVisualButtons((sender as Button).Content), new object[] { true });

            Console.WriteLine("working");
            Console.WriteLine(sender.GetType().Name);
            //Console.WriteLine((int)((Key)sender));
            Console.WriteLine(e.Source.GetType().Name);
            //Console.WriteLine((e.Source as Button).Content);
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                WorkWithSymbolsInBoxes(e);  // уже переместил букву в верхнем текстБоксе.

                //ErrorChecking();    // проверка на ошибку (при этом в нижнем еще нет буквы!!!!!)
            }

            //Console.WriteLine((int)e.Key);
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

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Console.WriteLine(e.Text);

            if (this.btnStart.IsEnabled == false)
            {
                //this.textUserTyped.Text += e.Text;
            }


            //foreach (Button item in this.test.Children)
            //{

            //    //Console.WriteLine(item.Content.ToString());

            //    if (e.Text == item.Content.ToString())
            //    {
            //        //Console.WriteLine(item.Content.ToString());

            //        //item.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            //        //item.Focus();

            //        //typeof(Button).GetMethod("OnClick",
            //        //    System.Reflection.BindingFlags.Instance
            //        //    | System.Reflection.BindingFlags.NonPublic)
            //        //    .Invoke(item as Button, new object[0]);

            //        //item.IsPressed = true;
            //    }
            //}

            //(sender as Button).

            if (this.btnStart.IsEnabled == false)
            {

            }
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
