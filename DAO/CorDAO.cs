using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CarManagement.Models;
using System.Data.SqlClient;

[SessionCheck]
public class CorDAO
{
    private readonly string connectionString;

    public CorDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<Cor> GetAll()
    {
        List<Cor> cores = new List<Cor>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Cor";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cor cor = new Cor
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString(),
                    Status = (bool)reader["Status"]
                };
                cores.Add(cor);
            }
        }
        return cores;
    }

    public void Add(Cor cor)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Cor (Descricao, Status) VALUES (@Descricao, @Status)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", cor.Descricao);
            cmd.Parameters.AddWithValue("@Status", cor.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Cor GetById(int id)
    {
        Cor cor = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Cor WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                cor = new Cor
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString(),
                    Status = (bool)reader["Status"]
                };
            }
        }
        return cor;
    }

    public void Update(Cor cor)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Cor SET Descricao = @Descricao, Status = @Status WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", cor.Descricao);
            cmd.Parameters.AddWithValue("@Status", cor.Status);
            cmd.Parameters.AddWithValue("@Id", cor.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Cor WHERE Id = @Id";
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
            string query = "SELECT COUNT(*) FROM Cor WHERE Descricao = @Descricao AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Descricao", descricao);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
