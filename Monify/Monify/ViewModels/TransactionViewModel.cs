﻿using Monify.Services;
using Monify.Tools;
using Monify.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Monify.ViewModels
{
    class TransactionViewModel: ObservableObject, IViewModel
    {
        public IStorage Storage { get; }

        UserControl currentControl;

        public UserControl CurrentControl { get => currentControl; set => SetProperty(ref currentControl, value); }

        public TransactionViewModel()
        {
            Storage = StorageGetter.Storage;
        }

        RelayCommand returnToMainViewCommand;

        public RelayCommand ReturnToMainViewCommand
        {
            get
            {
                return returnToMainViewCommand ??
                    (returnToMainViewCommand = new RelayCommand(obj =>
                    {
                        ((WindowViewModel)(ViewModelsStorage.ViewModels[VM.WindowViewModel])).CurrentControl = new MainView();
                    }));
            }
        }

        public IViewModel ResetToInitialState()
        {
            CurrentControl = new TransactionAccountChooseSubView();
            ((CalculatorViewModel)ViewModelsStorage.ViewModels[VM.CalculatorViewModel]).OperationButtonName = "Add Transaction";

            return this;
        }
    }
}
