using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CelltechFinal.Models;
using CelltechFinal.Data.Connection;

namespace CelltechFinal.Services
{
    public class DetalleVentaService
    {
        private readonly ConnectionSQL _connection;

        public DetalleVentaService()
        {
            _connection = new ConnectionSQL();
        }

        public List<DetalleVenta> GetDetallesPorVenta(int ventaId)
        {
            List<DetalleVenta> detalles = new List<DetalleVenta>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT d.DetalleVentaID, d.VentaID, d.ProductoID, 
                           d.Cantidad, d.PrecioUnitario,
                           p.Codigo, p.Nombre
                    FROM DetallesVenta d
                    INNER JOIN Productos p ON d.ProductoID = p.ProductoID
                    WHERE d.VentaID = @VentaID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VentaID", ventaId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detalles.Add(new DetalleVenta
                            {
                                DetalleVentaID = reader.GetInt32(0),
                                VentaID = reader.GetInt32(1),
                                ProductoID = reader.GetInt32(2),
                                Cantidad = reader.GetInt32(3),
                                PrecioUnitario = reader.GetDecimal(4),
                                Codigo = reader.GetString(5),
                                Nombre = reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return detalles;
        }

        public void CrearDetalleVenta(DetalleVenta detalle)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            INSERT INTO DetallesVenta (VentaID, ProductoID, Cantidad, PrecioUnitario)
                            VALUES (@VentaID, @ProductoID, @Cantidad, @PrecioUnitario);

                            UPDATE Productos 
                            SET Stock = Stock - @Cantidad 
                            WHERE ProductoID = @ProductoID";

                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@VentaID", detalle.VentaID);
                            command.Parameters.AddWithValue("@ProductoID", detalle.ProductoID);
                            command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                            command.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void EliminarDetalleVenta(int detalleVentaId)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero obtenemos la cantidad y el producto para restaurar el stock
                        string queryGet = @"
                            SELECT ProductoID, Cantidad 
                            FROM DetallesVenta 
                            WHERE DetalleVentaID = @DetalleVentaID";

                        int productoId = 0;
                        int cantidad = 0;

                        using (var command = new SqlCommand(queryGet, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DetalleVentaID", detalleVentaId);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    productoId = reader.GetInt32(0);
                                    cantidad = reader.GetInt32(1);
                                }
                            }
                        }

                        // Restauramos el stock
                        string queryStock = @"
                            UPDATE Productos 
                            SET Stock = Stock + @Cantidad 
                            WHERE ProductoID = @ProductoID";

                        using (var command = new SqlCommand(queryStock, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ProductoID", productoId);
                            command.Parameters.AddWithValue("@Cantidad", cantidad);
                            command.ExecuteNonQuery();
                        }

                        // Eliminamos el detalle
                        string queryDelete = "DELETE FROM DetallesVenta WHERE DetalleVentaID = @DetalleVentaID";

                        using (var command = new SqlCommand(queryDelete, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DetalleVentaID", detalleVentaId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ActualizarCantidad(int detalleVentaId, int nuevaCantidad)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero obtenemos la cantidad actual para calcular la diferencia
                        string queryGet = @"
                            SELECT ProductoID, Cantidad 
                            FROM DetallesVenta 
                            WHERE DetalleVentaID = @DetalleVentaID";

                        int productoId = 0;
                        int cantidadActual = 0;

                        using (var command = new SqlCommand(queryGet, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DetalleVentaID", detalleVentaId);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    productoId = reader.GetInt32(0);
                                    cantidadActual = reader.GetInt32(1);
                                }
                            }
                        }

                        int diferencia = nuevaCantidad - cantidadActual;

                        // Actualizamos el stock
                        string queryStock = @"
                            UPDATE Productos 
                            SET Stock = Stock - @Diferencia 
                            WHERE ProductoID = @ProductoID";

                        using (var command = new SqlCommand(queryStock, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ProductoID", productoId);
                            command.Parameters.AddWithValue("@Diferencia", diferencia);
                            command.ExecuteNonQuery();
                        }

                        // Actualizamos la cantidad en el detalle
                        string queryUpdate = @"
                            UPDATE DetallesVenta 
                            SET Cantidad = @NuevaCantidad 
                            WHERE DetalleVentaID = @DetalleVentaID";

                        using (var command = new SqlCommand(queryUpdate, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DetalleVentaID", detalleVentaId);
                            command.Parameters.AddWithValue("@NuevaCantidad", nuevaCantidad);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}