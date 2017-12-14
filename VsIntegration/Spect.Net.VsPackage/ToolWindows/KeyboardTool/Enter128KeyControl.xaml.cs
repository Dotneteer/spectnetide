﻿using System.Windows.Input;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for Single128KeyControl.xaml
    /// </summary>
    public partial class Enter128KeyControl
    {
        /// <summary>
        /// Responds to the event when the main key is clicked
        /// </summary>
        public event MouseButtonEventHandler MainKeyClicked;

        public Enter128KeyControl()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack2.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack3.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack4.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack5.Fill = Single128KeyControl.MouseOverButtonBack;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack2.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack3.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack4.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack5.Fill = Single128KeyControl.NormalButtonBack;
        }
    }
}