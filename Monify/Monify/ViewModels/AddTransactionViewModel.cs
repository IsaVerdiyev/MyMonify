﻿using Monify.Models;
using Monify.Services;
using Monify.Services.CalculatorService;
using Monify.Tools;
using Monify.ViewModels.AbstractClassesAndInterfaces;
using Monify.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Monify.ViewModels
{
    class AddTransactionViewModel: AbstractCalculatorViewModel, IViewModel
    {
        public IStorage Storage { get; }
        public string HeaderText { get => "Add Transaction"; }

        UserControl currentControl;
        DateTime selectedDate;
        private Account sourceAccount;
        private Account destinationAccount;


        public AddTransactionViewModel()
        {
            Storage = StorageGetter.Storage;
            PerformOperationButtonName = "Add transaction";
            ResetToInitialState();
        }


        public override string PerformOperationButtonName { get;  }

        public UserControl CurrentControl {
            get => currentControl;
            set {
                SetProperty(ref currentControl, value);
                CurrentControl.DataContext = this;
            }
        }

        public string CurrencyCode { get => Storage.Currencies.FirstOrDefault(c => c.Index == SourceAccount?.CurrencyIndex)?.Code; set => OnPropertyChanged(); }

        public DateTime SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value); }

        public Account SourceAccount {
            get => sourceAccount;
            set
            {
                SetProperty(ref sourceAccount, value);
                CurrencyCode = CurrencyCode;
            }
        }
        public Account DestinationAccount { get => destinationAccount; set => SetProperty(ref destinationAccount, value); }

        RelayCommand returnToMainViewCommand;

        public RelayCommand ReturnToMainViewCommand
        {
            get
            {
                return returnToMainViewCommand ??
                    (returnToMainViewCommand = new RelayCommand(obj =>
                    {
                        ((WindowViewModel)(ViewModelsStorage.GetViewModel(typeof(WindowViewModel).Name))).CurrentControl = new MainView();
                    }));
            }
        }

        private RelayCommand switchToCalculatorCommand;

        public RelayCommand SwitchToCalculatorCommand
        {
            get {
                return switchToCalculatorCommand ??
                    (switchToCalculatorCommand = new RelayCommand(obj =>
                    {
                        CurrentControl = new CalculatorSubView();
                    },
                    obj => SelectedDate != null && SourceAccount != null && DestinationAccount != null && SourceAccount != DestinationAccount));
            }
        }


        private RelayCommand performOperationButtonCommand;

        public override RelayCommand PerformOperationButtonCommand
        {
            get
            {
                return performOperationButtonCommand ??
                    (performOperationButtonCommand = new RelayCommand(obj =>
                    {
                        double transactionAmountInSourceCurrency = Double.Parse(TextBoxNumber);
                        double transactionAmountInDestinationCurrency = CurrencyConverter.Convert(Storage.Currencies.FirstOrDefault(c => c.Index == SourceAccount.CurrencyIndex), Storage.Currencies.FirstOrDefault(c => c.Index == DestinationAccount.CurrencyIndex), transactionAmountInSourceCurrency);
                        Operation sourceOperation = new Operation
                        {
                            AccountIndex = SourceAccount.Index,
                            Amount = transactionAmountInSourceCurrency,
                            Date = SelectedDate,
                            OperationCategoryIndex = Storage.OperationCategories.FirstOrDefault(cat => cat.Name == CategoryEnum.Transaction.ToString() && cat.OperationTypeIndex == Storage.OperationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index).Index
                        };
                        SourceAccount.Balance -= transactionAmountInSourceCurrency;

                        Storage.Operations.Add(sourceOperation);

                        Operation destinationOperation = new Operation
                        {
                            AccountIndex = DestinationAccount.Index,
                            Amount = transactionAmountInDestinationCurrency,
                            Date = SelectedDate,
                            OperationCategoryIndex = Storage.OperationCategories.FirstOrDefault(cat => cat.Name == CategoryEnum.Transaction.ToString() && cat.OperationTypeIndex == Storage.OperationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Profit.ToString()).Index).Index
                        };
                        DestinationAccount.Balance += transactionAmountInDestinationCurrency;

                        Storage.Operations.Add(destinationOperation);

                        ReturnToMainViewCommand.Execute(obj);
                    }));
            }
        }


        public override IViewModel ResetToInitialState()
        {
            CurrentControl = new AddTransactionDownerPartAccountChooseSubView();
            SelectedDate = DateTime.Now;
            CalculatorState = new InitialCalculatorState(this);
            TextBoxNumber = "";
            SelectedDate = DateTime.Now;
            SourceAccount = null;
            DestinationAccount = null;

            return this;
        }
    }
}