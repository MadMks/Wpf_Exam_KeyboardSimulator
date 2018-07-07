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


namespace Task_KeyboardSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Кол-во символов в строке.
        /// </summary>
        private const int NUMBER_OF_CHARACTERS_IN_STRING = 15;
        /// <summary>
        /// Кол-во пробелов в строке.
        /// </summary>
        private const int NUMBER_OF_SPACES_IN_LINE = 4;

        private DispatcherTimer timer;
        private Random rand;

        /// <summary>
        /// Кол-во ошибок.
        /// </summary>
        private int numberOfMistakes;
        /// <summary>
        /// Кол-во секунд.
        /// </summary>
        private int numberOfSeconds;
        /// <summary>
        /// Добавление символов 
        /// (для проверки символов,
        /// чтоб не реагировать на Backspace).
        /// </summary>
        private bool isAddingSymbols;
        /// <summary>
        /// Текущая нажатая кнопка.
        /// </summary>
        private ButtonAndKey currentPressedButton;
        private bool IsCapsLockIsOn;
        /// <summary>
        /// Список индексов ошибок для UserTyped.
        /// </summary>
        private List<int> listOfErrorIndicesForUserTyped;
        /// <summary>
        /// Список индексов ошибок для Typed.
        /// </summary>
        private List<int> listOfErrorIndicesForTyped;
        /// <summary>
        /// Список символов нижнего регистра.
        /// </summary>
        private string characterListLower;
        /// <summary>
        /// Список символов верхнего регистра.
        /// </summary>
        private string characterListUpper;
        /// <summary>
        /// Генерировать строку учитывая символы верхнего регистра.
        /// </summary>
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

            rand = new Random();


            this.PreviewTextInput += MainWindow_PreviewTextInput;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;
            this.KeyUp += MainWindow_KeyUp;
            this.KeyDown += MainWindow_KeyDown;


            this.ComputeAndRememberStateOfCapsLock();
        }

        private void ComputeAndRememberStateOfCapsLock()
        {
            if (Keyboard.IsKeyToggled(Key.Capital) == true)
            {
                this.IsCapsLockIsOn = true;
            }
            else if (Keyboard.IsKeyToggled(Key.Capital) == false)
            {
                this.IsCapsLockIsOn = false;
            }

            this.SwitchingToCapsLockButtonsOrStandardButtons();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
       {
            // Переключение на символы с помощью Shift. (нажатие).
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                if (this.IsCapsLockIsOn)
                {
                    // Нажимаем шифт.
                    this.ShowBlockOfStandardButtonsWhenYouPressCapsLockWithShift();
                }
                else if (!this.IsCapsLockIsOn)
                {
                    this.ShowABlockOfButtonsWhenYouPressTheShift();
                }
            }

        }

        /// <summary>
        /// Показать блок кнопок с нажатым Caps Lock и зажатым Shift.
        /// </summary>
        private void ShowBlockOfStandardButtonsWhenYouPressCapsLockWithShift()
        {
            this.blockButtons.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLock.Visibility = Visibility.Collapsed;
            this.blockButtonsWithShift.Visibility = Visibility.Collapsed;

            this.blockButtonsWithCapsLockWithShift.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Показать блок кнопок с нажатым Caps Lock.
        /// </summary>
        private void ShowBlockOfStandardButtonsWhenYouPressTheCapsLock()
        {
            this.blockButtonsWithCapsLock.Visibility = Visibility.Visible;

            this.blockButtons.Visibility = Visibility.Collapsed;
            this.blockButtonsWithShift.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLockWithShift.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Показать стандартный блок кнопок (без нажатого Shift или Caps Lock).
        /// </summary>
        private void ShowBlockOfStandardButtons()
        {
            this.blockButtons.Visibility = Visibility.Visible;

            this.blockButtonsWithShift.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLock.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLockWithShift.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Показать блок кнопок с нажатым Shift.
        /// </summary>
        private void ShowABlockOfButtonsWhenYouPressTheShift()
        {
            this.blockButtonsWithShift.Visibility = Visibility.Visible;

            this.blockButtons.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLock.Visibility = Visibility.Collapsed;
            this.blockButtonsWithCapsLockWithShift.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Переключение на буквы верхнего регистра с помощью Caps Lock.
            if (e.Key == Key.CapsLock)
            {
                this.ChangeCapslockValue();

                this.SwitchingToCapsLockButtonsOrStandardButtons();
            }

            this.ButtonPressUp(this.currentPressedButton);

            this.SwitchingVisualButtonsToStateWithoutPressing();
        }

        /// <summary>
        /// Переключение или на клавиатуру "Caps Lock" или на стандартную клавиатуру.
        /// </summary>
        private void SwitchingToCapsLockButtonsOrStandardButtons()
        {
            if (IsCapsLockIsOn)
            {
                this.ShowBlockOfStandardButtonsWhenYouPressTheCapsLock();
            }
            else
            {
                this.ShowBlockOfStandardButtons();
            }
        }

        /// <summary>
        /// Меняем значение Caps Lock на противоположное.
        /// </summary>
        private void ChangeCapslockValue()
        {
            if (IsCapsLockIsOn == true)
            {
                IsCapsLockIsOn = false;
            }
            else if (IsCapsLockIsOn == false)
            {
                IsCapsLockIsOn = true;
            }
        }

        private void ButtonPressUp(ButtonAndKey button)
        {
            if (button != null)
            {
                button.IsKeyPressed = false;
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Переключение на символы с помощью Shift. (отпускание).
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.SwitchingToCapsLockButtonsOrStandardButtons();
            }
        }

        


        private ButtonAndKey SearchingVisualButtons(UniformGrid block, Key lastKey)
        {
            foreach (Grid grid in block.Children)
            {
                foreach (var item in grid.Children)
                {
                    if (item is UniformGrid)
                    {
                        foreach (ButtonAndKey button in (item as UniformGrid).Children)
                        {
                            if (button.Tag.ToString() == lastKey.ToString())
                            {
                                return button;
                            }
                        }
                    }
                    else if (item is ButtonAndKey)
                    {
                        if ((item as ButtonAndKey).Tag.ToString() == lastKey.ToString())
                        {
                            return (item as ButtonAndKey);
                        }
                    }
                }
            }


            return null;
        }

        /// <summary>
        /// Переключение визуальных кнопок в состояние без нажатия.
        /// </summary>
        private void SwitchingVisualButtonsToStateWithoutPressing()
        {
            this.SwitchingToStateWithoutPressing(this.blockButtons);

            this.SwitchingToStateWithoutPressing(this.blockButtonsWithShift);

            this.SwitchingToStateWithoutPressing(this.blockButtonsWithCapsLock);

            this.SwitchingToStateWithoutPressing(this.blockButtonsWithCapsLockWithShift);
        }

        private void SwitchingToStateWithoutPressing(UniformGrid block)
        {
            foreach (Grid grid in block.Children)
            {
                foreach (var item in grid.Children)
                {
                    if (item is UniformGrid)
                    {
                        foreach (ButtonAndKey button in (item as UniformGrid).Children)
                        {
                            this.ButtonPressUp(button);
                        }
                    }
                    else if (item is ButtonAndKey)
                    {
                        this.ButtonPressUp(item as ButtonAndKey);
                    }
                }
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                WorkWithSymbolsInBoxes(e);  // уже переместил букву в верхнем текстБоксе.
            }

            this.SearchForAPressedVisualButton(e.Key);

            this.ButtonPressDown();
        }

        /// <summary>
        /// Поиск нажатой визуальной кнопки (соответствующей физической клавише).
        /// </summary>
        /// <param name="lastKeyPressed">Последняя нажатая физическая кнопка.</param>
        private void SearchForAPressedVisualButton(Key lastKeyPressed)
        {
            if (this.blockButtons.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.blockButtons, lastKeyPressed);
            }
            else if (this.blockButtonsWithShift.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.blockButtonsWithShift, lastKeyPressed);
            }
            else if (this.blockButtonsWithCapsLock.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.blockButtonsWithCapsLock, lastKeyPressed);
            }
            else if (this.blockButtonsWithCapsLockWithShift.Visibility == Visibility.Visible)
            {
                this.currentPressedButton = SearchingVisualButtons(this.blockButtonsWithCapsLockWithShift, lastKeyPressed);
            }
        }

        private void ButtonPressDown()
        {
            if (currentPressedButton != null)
            {
                currentPressedButton.IsKeyPressed = true;
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
        /// Работа с символами в текстБлоках.
        /// </summary>
        /// <param name="e">Параметры события нажатия кнопки.</param>
        private void WorkWithSymbolsInBoxes(KeyEventArgs e)
        {
            if (IsAValidInputKey(e.Key))
            {
                this.MoveOneCharacterInTheTopLine();

                isAddingSymbols = true;
            }
            else if (e.Key == Key.Back)
            {
                DeleteTheLastCharacterInTheBottomLine();

                DeleteTheLastCharacterInTheTopLine();

                isAddingSymbols = false;
            }
            // Иначе запретим нажатие любых других клавиш (ввод недопустимых символов).
            else
            {
                isAddingSymbols = false;
            }
        }

        /// <summary>
        /// Удаление последнего символа в верхней строке.
        /// </summary>
        private void DeleteTheLastCharacterInTheTopLine()
        {
            if (this.textTyped.Text.Length > 0)
            {
                this.textNeedToType.Text = String.Concat(this.textTyped.Text[textTyped.Text.Length - 1], this.textNeedToType.Text);

                this.RepaintingAllStoredErrorsWithoutLastLetter(this.textTyped, this.listOfErrorIndicesForTyped, Brushes.Orange);
            }
        }

        /// <summary>
        /// Удаление последнего символа в нижней строке.
        /// </summary>
        private void DeleteTheLastCharacterInTheBottomLine()
        {
            if (this.textUserTyped.Text.Length > 0)
            {
                // При исправлении ошибки - убираем пометку (красный фон, для нижнего textBlock).
                if (listOfErrorIndicesForUserTyped.Exists(x => x == this.textUserTyped.Text.Length - 1))
                {
                    this.listOfErrorIndicesForUserTyped.RemoveAt(this.listOfErrorIndicesForUserTyped.FindIndex(x => x == this.textUserTyped.Text.Length - 1));
                }

                this.RepaintingAllStoredErrorsWithoutLastLetter(this.textUserTyped, this.listOfErrorIndicesForUserTyped, Brushes.Red);
            }
        }

        /// <summary>
        /// Перемещение одного символа в верхней строке.
        /// </summary>
        private void MoveOneCharacterInTheTopLine()
        {
            if (this.textNeedToType.Text.Length > 0)
            {
                this.textTyped.Inlines.Add(new Run(this.textNeedToType.Text[0].ToString()));
                string tempStrNeedToType = this.textNeedToType.Text.Substring(1);
                this.textNeedToType.Inlines.Clear();
                this.textNeedToType.Inlines.Add(tempStrNeedToType);
            }
        }

        /// <summary>
        /// Допустимая клавиша для ввода.
        /// </summary>
        /// <param name="key">Нажатая клавиша на физической клавиатуре.</param>
        /// <returns>true если клавиша допустимая для ввода.</returns>
        private bool IsAValidInputKey(Key key)
        {
            if (key >= Key.D0 && key <= Key.Z
                || key >= Key.Oem1 && key <= Key.Oem3
                || key >= Key.OemOpenBrackets && key <= Key.OemQuotes
                || key == Key.Space)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    return false;
                }

                return true;
            }

            return false;
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

                this.SwitchKeyboardCaseInCorrectPositionAtTheEndOfWorkout();

                this.ShowMessageWithResults("completed");

                this.btnStart.Focus();
            }
        }

        /// <summary>
        /// Переключение регистра визуальной клавиатуры 
        /// в правильное (визуальное) положение,
        /// при завершении тренировки.
        /// </summary>
        private void SwitchKeyboardCaseInCorrectPositionAtTheEndOfWorkout()
        {
            if (IsCapsLockIsOn)
            {
                if (Keyboard.IsKeyUp(Key.LeftShift) || Keyboard.IsKeyUp(Key.RightShift))
                {
                    this.ShowBlockOfStandardButtonsWhenYouPressTheCapsLock();
                }
            }
            else if (!IsCapsLockIsOn)
            {
                if (Keyboard.IsKeyUp(Key.LeftShift) || Keyboard.IsKeyUp(Key.RightShift))
                {
                    this.ShowBlockOfStandardButtons();
                }
            }
        }

        private void ShowMessageWithResults(string trainingState)
        {
            MessageBox.Show($"Speed: {this.answerNumberOfCharsMin.Text} chars/min\n"
                + $"Fails: {this.answerNumberOfMistakes.Text}",
                $"Training {trainingState}");
        }

        

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (this.IsTrainingStarted())
            {
                if (isAddingSymbols)
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
        /// <param name="errorIndices">Список с индексами ошибок для textBlock.</param>
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

            this.IncreaseInTheNumberOfMistakesMade();

            this.RememberTheIndexErrorLocations();
            
            return false;
        }

        private void RememberTheIndexErrorLocations()
        {
            this.listOfErrorIndicesForUserTyped.Add(this.textUserTyped.Text.Length);
            this.listOfErrorIndicesForTyped.Add(this.textUserTyped.Text.Length);
        }

        /// <summary>
        /// Увеличение числа допущенных ошибок.
        /// </summary>
        private void IncreaseInTheNumberOfMistakesMade()
        {
            this.answerNumberOfMistakes.Text = (++numberOfMistakes).ToString();
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

            this.textNeedToType.Text
                = this.StringGeneration(
                    Convert.ToInt32(this.sliderDifficulty.Value),
                    this.isGenerateCaseSensitiveString);

            this.textUserTyped.Focus();

            this.timer.Start();
        }

        /// <summary>
        /// Генерация строки рандомных символов.
        /// </summary>
        /// <param name="quantitySymbol">Кол-во используемых символов.</param>
        /// <param name="IsCaseSensitiveUpper">Использовать верхний регистр.</param>
        /// <returns>Строка рандомных символов.</returns>
        private string StringGeneration(int quantitySymbol, bool IsCaseSensitiveUpper)
        {
            string randomString = null;
            List<int> listOfIndicesOfSpace = new List<int>();

            this.GenerateIndicesForSpaces(listOfIndicesOfSpace);

            if (IsCaseSensitiveUpper)
            {
                randomString = StringGenerationInBothRegisters(listOfIndicesOfSpace, quantitySymbol);
            }
            else
            {
                randomString = StringGenerationOnlyInLowercase(listOfIndicesOfSpace, quantitySymbol);  
            }

            return randomString;
        }

        /// <summary>
        /// Генерация строки рандомных символов в обоих регистрах.
        /// </summary>
        /// <param name="listOfIndicesOfSpace">Список индексов пробелов.</param>
        /// <param name="quantitySymbol">Кол-во используемых символов.</param>
        /// <returns>Строку рандомных символов в верхнем и нижнем регистрах.</returns>
        private string StringGenerationInBothRegisters(List<int> listOfIndicesOfSpace, int quantitySymbol)
        {
            string characterString = "";

            for (int i = 0; i < NUMBER_OF_CHARACTERS_IN_STRING; i++)
            {
                if (listOfIndicesOfSpace.Exists(x => x == i))
                {
                    characterString += " ";
                }
                else
                {
                    characterString += GeneratingRandomCharacterInUpperOrLowercase(quantitySymbol);
                }
            }

            return characterString;
        }

        private string GeneratingRandomCharacterInUpperOrLowercase(int quantitySymbol)
        {
            // 2 режима (верхний и нижний регистр).
            if (rand.Next(2) == 0)
            {
                // нижний регистр.
                return this.characterListLower[rand.Next(quantitySymbol)].ToString();
            }
            else
            {
                // верхний регистр.
                return this.characterListUpper[rand.Next(quantitySymbol)].ToString();
            }
        }

        /// <summary>
        /// Генерация строки рандомных символов только в нижнем регистре.
        /// </summary>
        /// <param name="listOfIndicesOfSpace">Список индексов пробелов.</param>
        /// <param name="quantitySymbol">кол-во используемых символов.</param>
        /// <returns>Строку рандомных символов в нижнем регистре.</returns>
        private string StringGenerationOnlyInLowercase(List<int> listOfIndicesOfSpace, int quantitySymbol)
        {
            string characterString = "";

            for (int i = 0; i < NUMBER_OF_CHARACTERS_IN_STRING; i++)
            {
                if (listOfIndicesOfSpace.Exists(x => x == i))
                {
                    characterString += " ";
                }
                else
                {
                    characterString += this.characterListLower[rand.Next(quantitySymbol)].ToString();
                }
            }

            return characterString;
        }

        private void GenerateIndicesForSpaces(List<int> listOfIndices)
        {
            Random random = new Random();
            int randIndex;

            for (int i = 0; i < NUMBER_OF_SPACES_IN_LINE; i++)
            {
                // Получаем рандомные пробелы 
                // # (доступные индексы кроме первого и последнего символа).
                // # Индексы пробелов могут повторятся - чтоб кол-во пробелов было разное.
                // # Пробелы не могут быть рядом.
                do
                {
                    randIndex = random.Next(1, NUMBER_OF_CHARACTERS_IN_STRING - 1);
                } while (IsPrevOrNextCellThereIsSpace(listOfIndices, randIndex));

                listOfIndices.Add(randIndex);
            }
        }

        /// <summary>
        /// В предыдущей или следующей ячейке есть пробел.
        /// </summary>
        /// <param name="listOfIndices">Список индексов пробелов.</param>
        /// <param name="randIndex">Новый рандомный индекс.</param>
        /// <returns>true если в соседней ячейке (слева или справа) есть пробел.</returns>
        private bool IsPrevOrNextCellThereIsSpace(List<int> listOfIndices, int randIndex)
        {
            if (listOfIndices.Exists(x => x == randIndex - 1) || listOfIndices.Exists(x => x == randIndex + 1))
            {
                return true;
            }

            return false;
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

            this.btnStart.Focus();
        }

        private void btnStop_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.btnStop.IsEnabled == false)
            {
                this.btnStart.IsEnabled = true;
            }
        }

        private void btnStart_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.btnStart.IsEnabled == false)
            {
                this.btnStop.IsEnabled = true;
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
        }

        private void checkBoxCaseSensitive_Unchecked(object sender, RoutedEventArgs e)
        {
            this.isGenerateCaseSensitiveString = false;
        }

    }
}