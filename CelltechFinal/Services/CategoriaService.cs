using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CelltechFinal.Data.Connection;
using CelltechFinal.Models;

namespace CelltechFinal.Services
{
    public class CategoriaService
    {
        private readonly ConnectionSQL _connection;

        public CategoriaService()
        {
            _connection = new ConnectionSQL();
        }

        public List<Categoria> GetAllCategorias()
        {
            var categorias = new List<Categoria>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    SELECT CategoriaID, Nombre, Descripcion 
                    FROM Categorias 
                    ORDER BY Nombre", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                CategoriaID = reader.GetInt32(reader.GetOrdinal("CategoriaID")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                            null : reader.GetString(reader.GetOrdinal("Descripcion"))
                            });
                        }
                    }
                }
            }

            return categorias;
        }

        public void CrearCategoria(Categoria categoria)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    INSERT INTO Categorias (Nombre, Descripcion) 
                    VALUES (@Nombre, @Descripcion)", connection))
                {
                    command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)categoria.Descripcion ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarCategoria(Categoria categoria)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    UPDATE Categorias 
                    SET Nombre = @Nombre, 
                        Descripcion = @Descripcion 
                    WHERE CategoriaID = @CategoriaID", connection))
                {
                    command.Parameters.AddWithValue("@CategoriaID", categoria.CategoriaID);
                    command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)categoria.Descripcion ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarCategoria(int categoriaId)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    DELETE FROM Categorias 
                    WHERE CategoriaID = @CategoriaID", connection))
                {
                    command.Parameters.AddWithValue("@CategoriaID", categoriaId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}