using CelltechFinal.Data.Connection;
using CelltechFinal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CelltechFinal.Services
{
    public class VentaService
    {
        private readonly ConnectionSQL _connection;

        public VentaService()
        {
            _connection = new ConnectionSQL();
        }

        public List<Venta> GetVentasPorFecha(DateTime fecha)
        {
            var ventas = new List<Venta>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    SELECT v.*, m.Nombre as MetodoPago
                    FROM Ventas v
                    LEFT JOIN MetodosPago m ON v.MetodoPagoID = m.MetodoPagoID
                    WHERE CAST(v.Fecha AS DATE) = @Fecha
                    ORDER BY v.VentaID DESC", connection))
                {
                    command.Parameters.AddWithValue("@Fecha", fecha.Date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                VentaID = reader.GetInt32(reader.GetOrdinal("VentaID")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                NumeroComprobante = reader.GetString(reader.GetOrdinal("NumeroComprobante")),
                                MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago"))
                            });
                        }
                    }
                }
            }

            return ventas;
        }

        public void GuardarVenta(Venta venta)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar la venta
                        string sqlVenta = @"
                            INSERT INTO Ventas (
                                Fecha, MetodoPagoID, Subtotal, 
                                Total, Estado, NumeroComprobante)
                            VALUES (
                                @Fecha, @MetodoPagoID, @Subtotal,
                                @Total, @Estado, @NumeroComprobante);
                            SELECT SCOPE_IDENTITY();";

                        int ventaId;
                        using (var command = new SqlCommand(sqlVenta, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Fecha", DateTime.Now);
                            command.Parameters.AddWithValue("@MetodoPagoID", venta.MetodoPagoID);
                            command.Parameters.AddWithValue("@Subtotal", venta.Subtotal);
                            command.Parameters.AddWithValue("@Total", venta.Total);
                            command.Parameters.AddWithValue("@Estado", "Completada");
                            command.Parameters.AddWithValue("@NumeroComprobante",
                                GenerarNumeroComprobante(connection, transaction));

                            ventaId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Insertar los detalles
                        foreach (var detalle in venta.Detalles)
                        {
                            string sqlDetalle = @"
                                INSERT INTO DetalleVentas (
                                    VentaID, ProductoID, Cantidad,
                                    PrecioUnitario, Subtotal)
                                VALUES (
                                    @VentaID, @ProductoID, @Cantidad,
                                    @PrecioUnitario, @Subtotal)";

                            using (var command = new SqlCommand(sqlDetalle, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@VentaID", ventaId);
                                command.Parameters.AddWithValue("@ProductoID", detalle.ProductoID);
                                command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                command.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                                command.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                                command.ExecuteNonQuery();
                            }
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

        private string GenerarNumeroComprobante(SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"
                SELECT ISNULL(MAX(CAST(
                    SUBSTRING(NumeroComprobante, 2, LEN(NumeroComprobante)-1) AS INT)), 0) + 1
                FROM Ventas", connection, transaction))
            {
                int numero = Convert.ToInt32(command.ExecuteScalar());
                return $"V{numero:D8}";
            }
        }
    }
}