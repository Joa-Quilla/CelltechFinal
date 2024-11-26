using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CelltechFinal.Data.Connection;
using CelltechFinal.Models;

namespace CelltechFinal.Services
{
    public class MarcaService
    {
        private readonly ConnectionSQL _connection;

        public MarcaService()
        {
            _connection = new ConnectionSQL();
        }

        public List<Marca> GetAllMarcas()
        {
            var marcas = new List<Marca>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    SELECT MarcaID, Nombre, Descripcion 
                    FROM Marcas 
                    ORDER BY Nombre", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            marcas.Add(new Marca
                            {
                                MarcaID = reader.GetInt32(reader.GetOrdinal("MarcaID")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                            null : reader.GetString(reader.GetOrdinal("Descripcion"))
                            });
                        }
                    }
                }
            }

            return marcas;
        }

        public void CrearMarca(Marca marca)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    INSERT INTO Marcas (Nombre, Descripcion) 
                    VALUES (@Nombre, @Descripcion)", connection))
                {
                    command.Parameters.AddWithValue("@Nombre", marca.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)marca.Descripcion ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarMarca(Marca marca)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    UPDATE Marcas 
                    SET Nombre = @Nombre, 
                        Descripcion = @Descripcion 
                    WHERE MarcaID = @MarcaID", connection))
                {
                    command.Parameters.AddWithValue("@MarcaID", marca.MarcaID);
                    command.Parameters.AddWithValue("@Nombre", marca.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)marca.Descripcion ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarMarca(int marcaId)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    DELETE FROM Marcas 
                    WHERE MarcaID = @MarcaID", connection))
                {
                    command.Parameters.AddWithValue("@MarcaID", marcaId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}