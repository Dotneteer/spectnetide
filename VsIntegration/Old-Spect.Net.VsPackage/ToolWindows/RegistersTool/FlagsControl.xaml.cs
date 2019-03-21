﻿using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.RegistersTool
{
    /// <summary>
    /// Interaction logic for FlagsControl.xaml
    /// </summary>
    public partial class FlagsControl
    {
        public static readonly DependencyProperty RegNameProperty = DependencyProperty.Register(
            "RegName", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string RegName
        {
            get => (string)GetValue(RegNameProperty);
            set => SetValue(RegNameProperty, value);
        }

        public static readonly DependencyProperty RegValueProperty = DependencyProperty.Register(
            "RegValue", typeof(byte), typeof(FlagsControl), new PropertyMetadata(default(byte), OnRegValueChanged));

        public byte RegValue
        {
            get => (byte)GetValue(RegValueProperty);
            set => SetValue(RegValueProperty, value);
        }

        public static readonly DependencyProperty Bit0Property = DependencyProperty.Register(
            "Bit0", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit0
        {
            get => (string)GetValue(Bit0Property);
            set => SetValue(Bit0Property, value);
        }

        public static readonly DependencyProperty Bit1Property = DependencyProperty.Register(
            "Bit1", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit1
        {
            get => (string)GetValue(Bit1Property);
            set => SetValue(Bit1Property, value);
        }

        public static readonly DependencyProperty Bit2Property = DependencyProperty.Register(
            "Bit2", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit2
        {
            get => (string)GetValue(Bit2Property);
            set => SetValue(Bit2Property, value);
        }

        public static readonly DependencyProperty Bit3Property = DependencyProperty.Register(
            "Bit3", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit3
        {
            get => (string)GetValue(Bit3Property);
            set => SetValue(Bit3Property, value);
        }

        public static readonly DependencyProperty Bit4Property = DependencyProperty.Register(
            "Bit4", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit4
        {
            get => (string)GetValue(Bit4Property);
            set => SetValue(Bit4Property, value);
        }

        public static readonly DependencyProperty Bit5Property = DependencyProperty.Register(
            "Bit5", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit5
        {
            get => (string)GetValue(Bit5Property);
            set => SetValue(Bit5Property, value);
        }

        public static readonly DependencyProperty Bit6Property = DependencyProperty.Register(
            "Bit6", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit6
        {
            get => (string)GetValue(Bit6Property);
            set => SetValue(Bit6Property, value);
        }

        public static readonly DependencyProperty Bit7Property = DependencyProperty.Register(
            "Bit7", typeof(string), typeof(FlagsControl), new PropertyMetadata(default(string)));

        public string Bit7
        {
            get => (string)GetValue(Bit7Property);
            set => SetValue(Bit7Property, value);
        }

        public FlagsControl()
        {
            InitializeComponent();
            Update(0);
        }

        private void Update(byte value)
        {
            Bit0 = Bit(value, 0);
            Bit1 = Bit(value, 1);
            Bit2 = Bit(value, 2);
            Bit3 = Bit(value, 3);
            Bit4 = Bit(value, 4);
            Bit5 = Bit(value, 5);
            Bit6 = Bit(value, 6);
            Bit7 = Bit(value, 7);
        }

        private static void OnRegValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reg = d as FlagsControl;
            reg?.Update((byte)e.NewValue);
        }

        private static string Bit(byte value, byte bitNo)
        {
            return (value & (1 << bitNo)) != 0 ? "1" : "0";
        }
    }
}
