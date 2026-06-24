using System;

namespace zuli_Business.DTO.Reports
{
    public class FlightsReportDTO
    {
        public DateTime Fecha { get; set; }
        public string? Origen { get; set; }
        public string? Destino { get; set; }
        public int? NumeroVuelo { get; set; }
        public int PasajerosPrimera { get; set; }
        public int PasajerosEconomica { get; set; }
        public string? Aerolinea { get; set; }
        public decimal VentaPasajeros { get; set; }
        public decimal VentaEquipaje { get; set; }
        public decimal TotalVenta { get; set; }
    }
}