using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Task_KeyboardSimulator
{
    public class WindowCommands
    {
        public static RoutedUICommand BtnPressRoutedCommand { get; set; }

        static WindowCommands()
        {
            //BtnPressRoutedCommand = new RoutedCommand("PressBtn", typeof(MainWindow));
            BtnPressRoutedCommand = new RoutedUICommand("PressBtn", "PressBtn", typeof(MainWindow));
        }
    }
}
