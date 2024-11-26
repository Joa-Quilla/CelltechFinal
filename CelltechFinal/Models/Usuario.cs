using System;

namespace CelltechFinal.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public int PersonaID { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public int RolID { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimoAcceso { get; set; }

        // Propiedades de navegación
        public string NombreCompleto { get; set; }
        public string NombreRol { get; set; }
    }
}