using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Spect.Net.Wpf.Behaviors
{
    /// <summary>
    /// This behavior automatically changes the visual state as an associated
    /// property of the data context changes
    /// </summary>
    public class VsmChangerBehavior : Behavior<DependencyObject>
    {
        private Dictionary<string, ViewModelStateGroupInfo> _vsGroups;

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(VsmChangerBehavior),
                new PropertyMetadata(null, ViewModelPropertyChangedCallback));

        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// If set to true, the initial state transitions are played
        /// </summary>
        public bool PlayInitialTransitions { get; set; }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedFrameworkElement.DataContextChanged += (s, e) =>
            {
                ViewModel = AssociatedFrameworkElement.DataContext;
            };
        }

        /// <summary>
        /// Handle the change of the ViewModel property
        /// </summary>
        private static void ViewModelPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((VsmChangerBehavior)dependencyObject).OnViewModelChanged(dependencyPropertyChangedEventArgs);
        }

        /// <summary>
        /// Handle the change of the ViewModel property
        /// </summary>
        private void OnViewModelChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue) return;

            if (args.OldValue is INotifyPropertyChanged inpcvm)
            {
                inpcvm.PropertyChanged -= ViewModelPropertyChanged;
            }

            inpcvm = args.NewValue as INotifyPropertyChanged;

            if (inpcvm == null) return;

            inpcvm.PropertyChanged += ViewModelPropertyChanged;
            _vsGroups = new Dictionary<string, ViewModelStateGroupInfo>();
            if (AssociatedFrameworkElement == null) return;

            var viewVStates = VisualStateManager.GetVisualStateGroups(AssociatedFrameworkElement);
            if (viewVStates == null) return;

            foreach (VisualStateGroup group in viewVStates)
            {
                var pi = inpcvm.GetType().GetRuntimeProperty(group.Name);
                var callbackName = group.Name + "TransitionComplete";
                var mi = inpcvm.GetType().GetRuntimeMethod(callbackName, new Type[] { });
                if (pi == null && mi == null)
                    continue;
                _vsGroups.Add(group.Name, new ViewModelStateGroupInfo
                {
                    PropertyInfo = pi,
                    CallbackMethodInfo = mi,
                    VisualStateGroup = group
                });
            }

            //Set up initial View States according to default values of the ViewModel enums
            AssociatedFrameworkElement.Loaded += (s, a) => InitializeStates();
        }

        private void InitializeStates()
        {
            foreach (var groupName in _vsGroups.Keys)
            {
                PerformTransitionForGroup(groupName, PlayInitialTransitions);
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var groupName = args.PropertyName;
            if (!_vsGroups.ContainsKey(groupName)) return;
            PerformTransitionForGroup(groupName, true);
        }

        private void PerformTransitionForGroup(string groupName, bool useTransitions)
        {
            var property = _vsGroups[groupName].PropertyInfo;
            var name = property.GetValue(ViewModel, null).ToString();

            var stateName = CalculateStateName(property.Name, name);


            var stateGroup = _vsGroups[groupName].VisualStateGroup;

            //check if callback function exists and invoke
            if (_vsGroups[groupName].CallbackMethodInfo != null)
                SetupCallback(stateGroup);

            var controlParent = GetVisualAncestor<Control>(AssociatedFrameworkElement);
            if (controlParent != null)
                VisualStateManager.GoToState(controlParent, stateName, useTransitions);
        }

        private void SetupCallback(VisualStateGroup stateGroup)
        {
            if (_vsGroups[stateGroup.Name].CallBackWiredUp)
                return;

            stateGroup.CurrentStateChanged += StateGroupOnCurrentStateChanged;
            _vsGroups[stateGroup.Name].VisualStateGroup = stateGroup;
            _vsGroups[stateGroup.Name].CallBackWiredUp = true;
        }

        private void StateGroupOnCurrentStateChanged(object sender, VisualStateChangedEventArgs args)
        {
            var stateGroupName = args.NewState.Name.Split('_')[0];
            var vsg = _vsGroups[stateGroupName].VisualStateGroup;
            _vsGroups[vsg.Name].CallbackMethodInfo?.Invoke(ViewModel, null); 
        }

        private static string CalculateStateName(string stateGroup, string state)
        {
            return stateGroup + "_" + state;
        }

        private FrameworkElement AssociatedFrameworkElement => (FrameworkElement)AssociatedObject;

        /// <summary>
        /// Gets the visual ancestor of the specified dependency object
        /// </summary>
        /// <typeparam name="T">Type of visual ancestor to get</typeparam>
        /// <param name="dependencyObject">Object to get the ancestor of</param>
        /// <returns>Ancestor, if found; otherwise, null</returns>
        private static T GetVisualAncestor<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = dependencyObject;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent is T ancestor) return ancestor;
            }
            return null;
        }
    }
}