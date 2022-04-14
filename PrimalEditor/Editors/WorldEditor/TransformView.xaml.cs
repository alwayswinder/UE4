﻿using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// TransformView.xaml 的交互逻辑
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChange = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChange = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector,
            Action<(Transform transfrom, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChange = false;
                return null;
            }
            var selection = vm.SelectedComponents.Select(x => selector(x)).ToList();
            return new Action(() =>
            {
                selection.ForEach(x => forEachAction(x));
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }
        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transfrom.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transfrom.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transfrom.Scale = x.Item2);

        private void RecordAction(Action redoAction, string name)
        {
            if (_propertyChange)
            {
                Debug.Assert(_undoAction != null);
                _propertyChange = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }
        private void OnPosition_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetPositionAction();
        }

        private void OnPosition_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position changed");
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnPosition_VectorBox_Mouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetRotationAction();
        }

        private void OnRotation_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation changed");
        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnRotation_VectorBox_Mouse_LBU(sender, null);
            }
        }

        private void OnScale_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetScaleAction();
        }

        private void OnScale_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale changed");

        }

        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnScale_VectorBox_Mouse_LBU(sender, null);
            }
        }


    }
}
