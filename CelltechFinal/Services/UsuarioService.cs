using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CelltechFinal.Data.Connection;
using CelltechFinal.Models;

namespace CelltechFinal.Services
{
    public class UsuarioService
    {
        private readonly ConnectionSQL _connection;

        public UsuarioService()
        {
            _connection = new ConnectionSQL();
        }

        public Usuario ValidateUser(string username, string password)
        {
            using (var connection = _connection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                    SELECT u.*, p.Nombres + ' ' + p.Apellidos as NombreCompleto, r.Nombre as NombreRol
                    FROM Usuarios u
                    INNER JOIN Personas p ON u.PersonaID = p.PersonaID
                    INNER JOIN Roles r ON u.RolID = r.RolID
                    WHERE u.NombreUsuario = @Usuario AND u.Contrasena = @Password AND u.Estado = 1", connection))
                {
                    command.Parameters.AddWithValue("@Usuario", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                                PersonaID = reader.GetInt32(reader.GetOrdinal("PersonaID")),
                                NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                                Contrasena = reader.GetString(reader.GetOrdinal("Contrasena")),
                                RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                NombreRol = reader.GetString(reader.GetOrdinal("NombreRol"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}