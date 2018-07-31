﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Monify.Models;
using Monify.Tools;

namespace Monify.Services
{
    class FileDataStorage :ObservableObject, IStorage
    {
        static FileDataStorage storage;

        public IFileSaveLoader FileSaveLoader { get; set; }

        ObservableCollection<OperationCategory> operationCategories;
        ObservableCollection<OperationType> operationTypes;
        ObservableCollection<Operation> operations;
        ObservableCollection<Account> accounts;
        ObservableCollection<Currency> currencies;

        private FileDataStorage()
        {
            FileSaveLoader = new JsonFileSaveLoader(this);
            try
            {
                Load();
            }
            catch(FileNotFoundException ex)
            {
                Initialize();
            }
        }

        public static FileDataStorage Storage { get => storage ?? (storage = new FileDataStorage()); }
       
        public ObservableCollection<Account> Accounts { get => accounts; set => SetProperty(ref accounts,value); }
        public ObservableCollection<OperationType> OperationTypes { get => operationTypes; set => SetProperty(ref operationTypes, value); }
        public ObservableCollection<OperationCategory> OperationCategories { get => operationCategories; set => SetProperty(ref operationCategories, value); }
        public ObservableCollection<Operation> Operations { get => operations; set => SetProperty(ref operations, value); }
        public ObservableCollection<Currency> Currencies { get => currencies; set => SetProperty(ref currencies, value); }

        public void Initialize()
        {
            OperationTypes = new ObservableCollection<OperationType>
            {
                new OperationType{Name = OperationTypesEnum.Profit.ToString()} ,
                new OperationType{Name = OperationTypesEnum.Expense.ToString()}
            };

            OperationCategories = new ObservableCollection<OperationCategory>
            {
                new OperationCategory{
                    Name = "Hygiene",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory
                {
                    Name = "Food",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Accommodation",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },

                new OperationCategory{
                    Name = "Health",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Cafe",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Car",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Clothes",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Pets",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Presents",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Entertainments",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Communication",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Sports",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Taxi",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Transport",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory
                {
                    Name = CategoryEnum.Transaction.ToString(),
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Expense.ToString()).Index
                },
                new OperationCategory{
                    Name = "Deposits",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Profit.ToString()).Index
                },
                new OperationCategory{
                    Name = "Salary",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Profit.ToString()).Index
                },
                new OperationCategory{
                    Name = "Saving",
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Profit.ToString()).Index
                },
                new OperationCategory
                {
                    Name = CategoryEnum.Transaction.ToString(),
                    OperationTypeIndex = operationTypes.FirstOrDefault(t => t.Name == OperationTypesEnum.Profit.ToString()).Index
                }
            };

            Accounts = new ObservableCollection<Account>();

            Operations = new ObservableCollection<Operation>();

            Currencies = new ObservableCollection<Currency>();
            SetCurrencies();


        }


        string GetXmlOfCurrencies()
        {
            using (WebClient web = new WebClient())
            {
                string day = String.Format("{0,2}", DateTime.Now.Day.ToString()).Replace(' ', '0');
                string month = String.Format("{0,2}", DateTime.Now.Month.ToString()).Replace(' ', '0');
                string date = day + "." + month + "." + DateTime.Now.Year;
                string unformattedUrl = "http://www.cbar.az/currencies/" + date + ".xml";
                string url = string.Format(unformattedUrl);
                string data = web.DownloadString(url);
                return data;

            }
        }

        public void SetCurrencies()
        {
            string xmlData = GetXmlOfCurrencies();

            var doc = new XmlDocument();
            doc.LoadXml(xmlData);

            

            var nodes = doc.SelectNodes("/ValCurs/ValType[@Type='Xarici valyutalar']/Valute");
            foreach (XmlNode item in nodes)
            {
                Currency currency = new Currency
                {
                    Code = item.Attributes["Code"].InnerText.ToUpper(),
                    Value = Double.Parse(item["Value"].InnerText)
                };


                Currency searchedCurrency = Currencies.FirstOrDefault(c => c.Code == currency.Code);
                if (searchedCurrency != null)
                {
                    searchedCurrency.Value = currency.Value;
                }
                else
                {
                    Currencies.Add(currency);
                }
            }

            if(Currencies.FirstOrDefault(c => c.Code == "AZN") == null)
            {
                Currency aznCurrency = new Currency
                {
                    Code = "AZN",
                    Value = 1
                };

                Currencies.Add(aznCurrency);
            }
        }

        public void Save()
        {
            try
            {
                FileSaveLoader.Save();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Load()
        {
            try
            {
                FileSaveLoader.Load();
            }catch(FileNotFoundException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
