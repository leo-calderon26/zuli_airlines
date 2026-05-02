using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class AircraftEntity
    {
        public Guid AdminId { get; set; }
        public string model {  get; set; }
        public decimal weight { get; set; }
        public short numberEconomyClassRows { get; set; }
        public short numberSeatingRowsEconomy { get; set; }
        public short numberFirstClassRows { get; set; }
        public short numberSeatingRowsFirst { get; set; }
        public decimal baggageCapacity { get; set; }
    }
}
