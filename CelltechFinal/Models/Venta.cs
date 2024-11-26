using System;
using System.Collections.Generic;

namespace CelltechFinal.Models
{
    public class Venta
    {
        public int VentaID { get; set; }
        public DateTime Fecha { get; set; }
        public int MetodoPagoID { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string NumeroComprobante { get; set; }

        // Propiedades de navegación
        public string MetodoPago { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}