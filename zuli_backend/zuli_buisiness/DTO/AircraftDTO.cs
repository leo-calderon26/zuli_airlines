using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Business.DTO
{
        public class AircraftDTO
        {
                public Guid aircraftId { get; set; }
                public Guid AdminId { get; set; }
                public string? businessId { get; set; }
                public string? model { get; set; }
                public decimal weight { get; set; }
                public short numberEconomyClassRows { get; set; }
                public short numberSeatingRowsEconomy { get; set; }
                public short numberFirstClassRows { get; set; }
                public short numberSeatingRowsFirst { get; set; }
                public decimal baggageCapacity { get; set; }
        }
}
