﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVSAssistant.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string haystack, string needle)
        {
            return haystack.ToLower().Contains(needle.ToLower());
        }
    }
}