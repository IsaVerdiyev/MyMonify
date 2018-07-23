﻿using Monify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monify.Tools
{
    class AccountFullInfoClass
    {
        public AbstractAccount Account { get; set; }
        public string Code { get; set; }


        public override string ToString()
        {
            return $"{Account.Name}\n {Code}";
        }
    }
}