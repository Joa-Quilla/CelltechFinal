namespace CelltechFinal.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CategoriaID { get; set; }
        public int MarcaID { get; set; }
        public string Modelo { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public bool Estado { get; set; }

        // Propiedades de navegación
        public string Categoria { get; set; }
        public string Marca { get; set; }
    }
}