using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Business.DTO
{
    public class AirportDTO
    {
        public string airportCode { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public Guid adminId { get; set; }
    }
}
