using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CelltechFinal.Data.Connection;
using CelltechFinal.Models;

namespace CelltechFinal.Services
{
    public class ProductoService
    {
        private readonly ConnectionSQL _connection;

        public ProductoService()
        {
            _connection = new ConnectionSQL();
        }

        public List<Producto> GetAllProductos()
        {
            var productos = new List<Producto>();

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    SELECT p.ProductoID, p.Codigo, p.Nombre, p.Descripcion, 
                           p.CategoriaID, p.MarcaID, p.Modelo, p.PrecioVenta,
                           p.Stock, p.StockMinimo, p.Estado,
                           m.Nombre as Marca, c.Nombre as Categoria
                    FROM Productos p
                    LEFT JOIN Marcas m ON p.MarcaID = m.MarcaID
                    LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
                    ORDER BY p.Nombre", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                            null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                CategoriaID = reader.GetInt32(reader.GetOrdinal("CategoriaID")),
                                MarcaID = reader.GetInt32(reader.GetOrdinal("MarcaID")),
                                Modelo = reader.IsDBNull(reader.GetOrdinal("Modelo")) ?
                                        null : reader.GetString(reader.GetOrdinal("Modelo")),
                                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                Categoria = reader.GetString(reader.GetOrdinal("Categoria"))
                            });
                        }
                    }
                }
            }
            return productos;
        }

        public void CrearProducto(Producto producto)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    INSERT INTO Productos (Codigo, Nombre, Descripcion, CategoriaID, 
                                         MarcaID, Modelo, PrecioVenta, Stock, 
                                         StockMinimo, Estado) 
                    VALUES (@Codigo, @Nombre, @Descripcion, @CategoriaID, 
                            @MarcaID, @Modelo, @PrecioVenta, @Stock, 
                            @StockMinimo, @Estado)", connection))
                {
                    command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)producto.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CategoriaID", producto.CategoriaID);
                    command.Parameters.AddWithValue("@MarcaID", producto.MarcaID);
                    command.Parameters.AddWithValue("@Modelo",
                        (object)producto.Modelo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                    command.Parameters.AddWithValue("@Estado", producto.Estado);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarProducto(Producto producto)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    UPDATE Productos 
                    SET Codigo = @Codigo,
                        Nombre = @Nombre, 
                        Descripcion = @Descripcion,
                        CategoriaID = @CategoriaID,
                        MarcaID = @MarcaID,
                        Modelo = @Modelo,
                        PrecioVenta = @PrecioVenta,
                        Stock = @Stock,
                        StockMinimo = @StockMinimo,
                        Estado = @Estado
                    WHERE ProductoID = @ProductoID", connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", producto.ProductoID);
                    command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion",
                        (object)producto.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CategoriaID", producto.CategoriaID);
                    command.Parameters.AddWithValue("@MarcaID", producto.MarcaID);
                    command.Parameters.AddWithValue("@Modelo",
                        (object)producto.Modelo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                    command.Parameters.AddWithValue("@Estado", producto.Estado);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarProducto(int productoId)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    DELETE FROM Productos 
                    WHERE ProductoID = @ProductoID", connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", productoId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Producto> BuscarProductos(string busqueda)
        {
            var productos = new List<Producto>();
            busqueda = busqueda?.Trim().ToLower() ?? "";

            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT p.ProductoID, p.Codigo, p.Nombre, p.Descripcion, 
                   p.CategoriaID, p.MarcaID, p.Modelo, p.PrecioVenta,
                   p.Stock, p.StockMinimo, p.Estado,
                   m.Nombre as Marca, c.Nombre as Categoria
            FROM dbo.Productos p
            INNER JOIN dbo.Marcas m ON p.MarcaID = m.MarcaID
            INNER JOIN dbo.Categorias c ON p.CategoriaID = c.CategoriaID
            WHERE p.Codigo LIKE @busqueda + '%' 
               OR p.Nombre LIKE '%' + @busqueda + '%'
               OR p.Descripcion LIKE '%' + @busqueda + '%'
               OR m.Nombre LIKE '%' + @busqueda + '%'
               OR c.Nombre LIKE '%' + @busqueda + '%'", connection))
                {
                    command.Parameters.AddWithValue("@busqueda", busqueda);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                            null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                CategoriaID = reader.GetInt32(reader.GetOrdinal("CategoriaID")),
                                MarcaID = reader.GetInt32(reader.GetOrdinal("MarcaID")),
                                Modelo = reader.IsDBNull(reader.GetOrdinal("Modelo")) ?
                                        null : reader.GetString(reader.GetOrdinal("Modelo")),
                                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                Categoria = reader.GetString(reader.GetOrdinal("Categoria"))
                            });
                        }
                    }
                }
            }
            return productos;
        }
        // Agregar este método a tu ProductoService existente
        public Producto ObtenerPorCodigo(string codigo)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT p.ProductoID, p.Codigo, p.Nombre, p.Descripcion, 
                   p.CategoriaID, p.MarcaID, p.Modelo, p.PrecioVenta,
                   p.Stock, p.StockMinimo, p.Estado,
                   m.Nombre as Marca, c.Nombre as Categoria
            FROM Productos p
            LEFT JOIN Marcas m ON p.MarcaID = m.MarcaID
            LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
            WHERE p.Codigo = @Codigo AND p.Estado = 1", connection))
                {
                    command.Parameters.AddWithValue("@Codigo", codigo);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                            null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                CategoriaID = reader.GetInt32(reader.GetOrdinal("CategoriaID")),
                                MarcaID = reader.GetInt32(reader.GetOrdinal("MarcaID")),
                                Modelo = reader.IsDBNull(reader.GetOrdinal("Modelo")) ?
                                        null : reader.GetString(reader.GetOrdinal("Modelo")),
                                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                Categoria = reader.GetString(reader.GetOrdinal("Categoria"))
                            };
                        }
                        return null;
                    }
                }
            }
        }
    }
}