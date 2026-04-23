using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Buisiness.DTO
{
    public class AircraftDTO
    {
        public string model { get; set; }
        public decimal weight { get; set; }
        public short numberEconomyClassRows { get; set; }
        public short numberSeatingRowsEconomy { get; set; }
        public short numberFirstClassRows { get; set; }
        public short numberSeatingRowsFirst { get; set; }
    }
}
