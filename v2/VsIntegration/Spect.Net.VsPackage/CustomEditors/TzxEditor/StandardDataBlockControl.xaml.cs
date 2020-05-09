﻿using System.Windows.Input;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for StandardDataBlockControl.xaml
    /// </summary>
    public partial class StandardDataBlockControl
    {
        public TzxStandardSpeedBlockViewModel Vm { get; }

        public StandardDataBlockControl()
        {
            InitializeComponent();
        }

        public StandardDataBlockControl(TzxStandardSpeedBlockViewModel vm) : this()
        {
            DataContext = Vm = vm;
            RomEditor.BasicList = vm?.ProgramList;
        }

        /// <summary>
        /// Switch between Data and Program view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataLabelClicked(object sender, MouseButtonEventArgs e)
        {
            if (Vm.IsProgramDataBlock)
            {
                Vm.ShowProgram = !Vm.ShowProgram;
            }
        }
    }
}
