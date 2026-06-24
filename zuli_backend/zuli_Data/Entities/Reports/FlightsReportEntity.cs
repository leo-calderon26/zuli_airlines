using System;

namespace zuli_Data.Entities.Reports
{
    public class FlightsReportEntity
    {
        public DateTime Fecha { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public int NumeroVuelo { get; set; }
        public int PasajerosPrimera { get; set; }
        public int PasajerosEconomica { get; set; }
        public string Aerolinea { get; set; } = string.Empty;
        public decimal VentaPasajeros { get; set; }
        public decimal VentaEquipaje { get; set; }
        public decimal TotalVenta { get; set; }
    }
}