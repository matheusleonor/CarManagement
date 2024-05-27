using CarManagement.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class UsuarioDAO
{
    private readonly string connectionString;

    public UsuarioDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public Usuario GetUsuarioByEmailAndSenha(string email, string senha)
    {
        Usuario usuario = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Usuario WHERE Email = @Email AND Senha = @Senha";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Senha", senha);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    Id = (int)reader["Id"],
                    Email = reader["Email"].ToString(),
                    Senha = reader["Senha"].ToString(),
                    TipoUsuarioId = (int)reader["TipoUsuarioId"]
                };
            }
        }
        return usuario;
    }

    public bool EmailExists(string email)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Usuario WHERE Email = @Email";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }

    public void AddUsuario(Usuario usuario)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Usuario (Email, Senha, TipoUsuarioId) VALUES (@Email, @Senha, @TipoUsuarioId)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
            cmd.Parameters.AddWithValue("@TipoUsuarioId", usuario.TipoUsuarioId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
