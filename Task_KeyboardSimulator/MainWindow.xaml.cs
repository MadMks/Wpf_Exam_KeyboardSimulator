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
            this.PreviewTextInput += MainWindow_PreviewTextInput;
        }

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Console.WriteLine(e.Text);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key > Key.D0 && e.Key < Key.D9)
            //{
            //    Console.WriteLine(e.Key.ToString());
            //}

            //Console.WriteLine((char)((int)e.Key + 10));
            //KeyConverter keyConverter = new KeyConverter();
            //string key = keyConverter.ConvertToString(;
            //Console.WriteLine(key);
        }
    }
}
