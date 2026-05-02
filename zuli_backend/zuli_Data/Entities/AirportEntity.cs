using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class AirportEntity
    {
        [Key] public string AirportCode { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public Guid AdminId { get; set; }
    }
}
