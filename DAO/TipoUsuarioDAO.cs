using CarManagement.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class TipoUsuarioDAO
{
    private readonly string connectionString;

    public TipoUsuarioDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<TipoUsuario> GetAll()
    {
        List<TipoUsuario> tipoUsuarios = new List<TipoUsuario>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM TipoUsuario";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TipoUsuario tipoUsuario = new TipoUsuario
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString(),
                    Status = (bool)reader["Status"]
                };
                tipoUsuarios.Add(tipoUsuario);
            }
        }
        return tipoUsuarios;
    }
}
