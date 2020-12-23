using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Helpers
{
    public static class StringExtensions
    {
        public static string EmptyIfNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }
    }
}
