using CelltechFinal.Data.Connection;
using CelltechFinal.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace CelltechFinal.Services
{
    public class MetodoPagoService
    {
        private readonly ConnectionSQL _connection;

        public MetodoPagoService()
        {
            _connection = new ConnectionSQL();
        }

        public List<MetodoPago> GetAllMetodosPago()
        {
            var metodosPago = new List<MetodoPago>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT MetodoPagoID, Nombre, Descripcion 
            FROM MetodosPago
            ORDER BY Nombre", connection)) // Eliminamos la cláusula WHERE Estado = 1
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metodosPago.Add(new MetodoPago
                            {
                                MetodoPagoID = reader.GetInt32(reader.GetOrdinal("MetodoPagoID")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion"))
                                              ? null
                                              : reader.GetString(reader.GetOrdinal("Descripcion"))
                            });
                        }
                    }
                }
            }

            return metodosPago;
        }

    }
}