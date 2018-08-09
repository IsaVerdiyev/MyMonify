﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monify.Models
{
    class Language
    {

        static int iterator = 0;
        int id;

        string fullName;
        string code;

        public Language()
        {
            id = iterator++;
        }

        public int Id { get => id; private set => id = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string Code { get => code; set => code = value; }
    }
}