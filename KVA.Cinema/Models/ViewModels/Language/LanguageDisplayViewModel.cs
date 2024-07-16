﻿using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class LanguageDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
