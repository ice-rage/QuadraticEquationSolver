﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using QuadraticEquationSolver.Infrastructure.Attributes;
using QuadraticEquationSolver.Infrastructure.Commands;
using QuadraticEquationSolver.Models;
using QuadraticEquationSolver.ViewModels.Base;
using QuadraticEquationSolver.Views;

namespace QuadraticEquationSolver.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly QuadraticEquation _quadraticEquation = new();

        #region StringValue : string - Некоторое строковое значение

        /// <summary>
        /// Некоторое строковое значение.
        /// </summary>
        private string _stringValue;

        /// <summary>
        /// Некоторое строковое значение.
        /// </summary>
        public string StringValue
        {
            get => _stringValue;

            set => SetValue(ref _stringValue, value)
                .Then(v => ChildWindowViewModel?.UpdateBaseViewModelValue(v));
        }

        #endregion

        #region Title : string - Заголовок окна

        /// <summary>
        /// Заголовок окна.
        /// </summary>
        private string _title = "Finding the roots of a quadratic equation";

        /// <summary>
        /// Заголовок окна.
        /// </summary>
        public string Title
        {
            get => _title;

            set => SetValue(ref _title, value)
                //.Then(v => OnPropertyChanged(nameof(TitleLength)))
                .UpdateProperty(nameof(TitleLength))
                .Then(v => Debug.WriteLine("The window title is set to \"{0}\"", value))
                .ThenIf(v => !string.IsNullOrWhiteSpace(value), v => 
                    Debug.WriteLine("Non-empty window title value"));
        }

        #endregion

        [DependencyOn(nameof(Title))]
        public int TitleLength => Title.Length;

        public string UserName
        {
            get => Get<string>();
            set => Set(value);
        }

        private double _a;

        public double A 
        { 
            get => _a;

            //set
            //{
            //    if (!Set(ref _a, value, "The value must be greater than or equal to zero", 
            //            a => a >= 0))
            //    {
            //        return;
            //    }

            //    _quadraticEquation.A = value;
            //    OnPropertyChanged(nameof(FirstRoot));
            //    OnPropertyChanged(nameof(SecondRoot));
            //}

            set => SetValue(ref _a, value)
                .ThenIf(
                    (oldValue, newValue) => Math.Abs(oldValue - newValue) > 0.001,
                    v =>
                    {
                        OnPropertyChanged(nameof(FirstRoot));
                        OnPropertyChanged(nameof(SecondRoot));
                    });
        }

        public double B
        {
            get => _quadraticEquation.B;

            set
            {
                if (Equals(_quadraticEquation.B, value))
                {
                    return;
                }

                _quadraticEquation.B = value;
                OnPropertyChanged(nameof(FirstRoot));
                OnPropertyChanged(nameof(SecondRoot));
            }
        }

        public double C
        {
            get => _quadraticEquation.C;

            set
            {
                if (!Set(value, _quadraticEquation.C, v => _quadraticEquation.C = v))
                {
                    return;
                }

                OnPropertyChanged(nameof(FirstRoot));
                OnPropertyChanged(nameof(SecondRoot));
            }
        }

        public double FirstRoot => _quadraticEquation.FirstRoot;

        public double SecondRoot => _quadraticEquation.SecondRoot;

        //private string _strValue;

        ////private SetValueResult<string> _value; - запрещено

        //private object _value;

        //public string StrValue
        //{
        //    get => _strValue;

        //    set
        //    {
        //        SetValueResult<string> setValue = SetValue(ref _strValue, value);
        //        setValue.UpdateProperty(nameof(Title));

        //        //_value = setValue; - запрещено
        //    }
        //}

        private ChildWindow _childWindow;

        protected ChildWindowViewModel ChildWindowViewModel;

        #region Command ShowChildWindowCommand - Команда для отображения дочернего окна

        /// <summary>
        /// Команда для отображения дочернего окна.
        /// </summary>
        private ICommand _showChildWindowCommand;

        /// <summary>
        /// Команда для отображения дочернего окна.
        /// </summary>
        public ICommand ShowChildWindowCommand => _showChildWindowCommand ??=
            new RelayCommand(OnShowChildWindowCommandExecuted, CanShowChildWindowCommandExecute);

        /// <summary>
        /// Проверка возможности выполнения - Команда для отображения дочернего окна.
        /// </summary>
        private bool CanShowChildWindowCommandExecute(object p) => ChildWindowViewModel is null;

        /// <summary>
        /// Логика выполнения - Команда для отображения дочернего окна.
        /// </summary>
        private void OnShowChildWindowCommandExecuted(object p)
        {
            var childWindowViewModel = new ChildWindowViewModel(this);

            var childWindow = new ChildWindow
            {
                DataContext = childWindowViewModel,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            childWindow.Closed += (_, _) =>
            {
                ChildWindowViewModel = null;
                _childWindow = null;
            };

            ChildWindowViewModel = childWindowViewModel;
            _childWindow = childWindow;

            childWindow.Show();
        }

        #endregion
    }
}
