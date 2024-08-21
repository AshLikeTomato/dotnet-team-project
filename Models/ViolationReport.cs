using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject2025.Models
{
    public class ViolationReport
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ReportDetails { get; set; }
        public DateTime ReportDate { get; set; }
    }
}