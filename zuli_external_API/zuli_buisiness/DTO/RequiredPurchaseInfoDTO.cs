using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Business.DTO
{
    public class RequiredPurchaseInfoDTO
    {
        public SummarizedFlightRouteDTO SummarizedFlightRoute { get; set; } = new();
        public ReservationRequestDTO ReservationInfo { get; set; } = new();
    }
}
