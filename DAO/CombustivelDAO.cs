using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CarManagement.Models;
using System.Data.SqlClient;

[SessionCheck]
public class CombustivelDAO
{
    private readonly string connectionString;

    public CombustivelDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<Combustivel> GetAll()
    {
        List<Combustivel> combustiveis = new List<Combustivel>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Combustivel";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Combustivel combustivel = new Combustivel
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString(),
                    Status = (bool)reader["Status"]
                };
                combustiveis.Add(combustivel);
            }
        }
        return combustiveis;
    }

    public void Add(Combustivel combustivel)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Combustivel (Descricao, Status) VALUES (@Descricao, @Status)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", combustivel.Descricao);
            cmd.Parameters.AddWithValue("@Status", combustivel.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Combustivel GetById(int id)
    {
        Combustivel combustivel = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Combustivel WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                combustivel = new Combustivel
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString(),
                    Status = (bool)reader["Status"]
                };
            }
        }
        return combustivel;
    }

    public void Update(Combustivel combustivel)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Combustivel SET Descricao = @Descricao, Status = @Status WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", combustivel.Descricao);
            cmd.Parameters.AddWithValue("@Status", combustivel.Status);
            cmd.Parameters.AddWithValue("@Id", combustivel.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Combustivel WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public bool NomeExists(string descricao, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Combustivel WHERE Descricao = @Descricao AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", descricao);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
