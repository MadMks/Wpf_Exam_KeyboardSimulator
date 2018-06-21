using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Task_KeyboardSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            this.PreviewTextInput += MainWindow_PreviewTextInput;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.btnStart.IsEnabled == false)
            {
                if ((int)e.Key > 34 
                    && (int)e.Key < 115
                    || (int)e.Key > 117
                    && (int)e.Key < 151
                    || (int)e.Key == 18)
                {
                    this.textTyped.Text += this.textNeedToType.Text[0];
                    this.textNeedToType.Text = this.textNeedToType.Text.Substring(1);
                }
                else if ((int)e.Key == 2)
                {
                    this.textNeedToType.Text = String.Concat(this.textTyped.Text[textTyped.Text.Length - 1], this.textNeedToType.Text);
                    this.textTyped.Text = textTyped.Text.Substring(0, textTyped.Text.Length - 1);
                }
            }
        }

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Console.WriteLine(e.Text);

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

            //        typeof(Button).GetMethod("OnClick",
            //            System.Reflection.BindingFlags.Instance
            //            | System.Reflection.BindingFlags.NonPublic)
            //            .Invoke(item as Button, new object[0]);
            //    }
            //}

            //(sender as Button).
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key > Key.D0 && e.Key < Key.D9)
            //{
            //    Console.WriteLine(e.Key.ToString());
            //}

            Console.WriteLine((int)e.Key);
            //KeyConverter keyConverter = new KeyConverter();
            //string key = keyConverter.ConvertToString(;
            //Console.WriteLine((sender as Button).Content);

            //this.test.Children

            //(sender as Button).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //typeof(Button).GetMethod("OnClick",
            //    System.Reflection.BindingFlags.Instance
            //    | System.Reflection.BindingFlags.NonPublic)
            //    .Invoke(sender as Button, new object[0]);


            if (this.btnStart.IsEnabled == false)
            {
                //if (e.Key == Key.Back)
                //{
                //    string temp = textUserTyped.Text.Substring(0, textUserTyped.Text.Length - 1);
                //}
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.btnStart.IsEnabled = false;

            this.textUserTyped.Focus();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.btnStop.IsEnabled = false;
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
            this.textUserTyped.Focus();
        }
    }
}
