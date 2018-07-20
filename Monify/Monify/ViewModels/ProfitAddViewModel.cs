﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monify.Models;

namespace Monify.ViewModels
{
    class ProfitAddViewModel : AbstractOperationAddViewModel, IGetSpecifiedGategories
    {
        public ObservableCollection<OperationCategory> GetSpecifiedCategories => new ObservableCollection<OperationCategory>(Storage.OperationCategories.Where(category => category.Index == Storage.OperationTypes.FirstOrDefault(t => t.Name == "Profit").Index));
    }
}
