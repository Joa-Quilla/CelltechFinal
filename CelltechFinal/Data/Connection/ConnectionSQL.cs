using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CelltechFinal.Data.Connection
{
    public class ConnectionSQL
    {
        private readonly string connectionString;

        public ConnectionSQL()
        {
            // Actualiza estos datos según tu configuración
            connectionString = @"Server=MSI;Database=CellTech3;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}