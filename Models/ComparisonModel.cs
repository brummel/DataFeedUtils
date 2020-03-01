using System;
using System.Collections.Generic;

namespace dotnetcore_test.Models
{
    public class ComparisonModel
    {
        public List<Check> Checks { get; set; }
    }

    public class Check
    {
        public bool HasPassed { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
    }
}
