using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities.External
{
    public class RequestedFlightEntity
    {
        [Key] public string origin { get; set; }
        [Key] public string destination { get; set; }
        public DateTime earliestDeparture { get; set; }
        public DateTime latestDeparture { get; set; }
        public int passengersQuantity { get; set; }
    }
}
