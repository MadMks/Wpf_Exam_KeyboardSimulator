using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Task_KeyboardSimulator
{
    public class ButtonAndKey: Button
    {
        public static readonly DependencyProperty IsKeyPressedProperty;

        public bool IsKeyPressed
        {
            get { return (bool)GetValue(IsKeyPressedProperty); }
            set { SetValue(IsKeyPressedProperty, value); }
        }

        static ButtonAndKey()
        {
            IsKeyPressedProperty = DependencyProperty.Register("IsKeyPressed", typeof(bool), typeof(ButtonAndKey));
        }
    }
}
