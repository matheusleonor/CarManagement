using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CarManagement.Models;
using System.Data.SqlClient;

public class MarcaDAO
{
    private readonly string connectionString;

    public MarcaDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<Marca> GetAll()
    {
        List<Marca> marcas = new List<Marca>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Marca";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Marca marca = new Marca
                {
                    Id = (int)reader["Id"],
                    Nome = reader["Nome"].ToString(),
                    Status = (bool)reader["Status"]
                };
                marcas.Add(marca);
            }
        }
        return marcas;
    }

    public void Add(Marca marca)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Marca (Nome, Status) VALUES (@Nome, @Status)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nome", marca.Nome);
            cmd.Parameters.AddWithValue("@Status", marca.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Marca GetById(int id)
    {
        Marca marca = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Marca WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                marca = new Marca
                {
                    Id = (int)reader["Id"],
                    Nome = reader["Nome"].ToString(),
                    Status = (bool)reader["Status"]
                };
            }
        }
        return marca;
    }

    public void Update(Marca marca)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Marca SET Nome = @Nome, Status = @Status WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nome", marca.Nome);
            cmd.Parameters.AddWithValue("@Status", marca.Status);
            cmd.Parameters.AddWithValue("@Id", marca.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Marca WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public bool NomeExists(string nome, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Marca WHERE Nome = @Nome AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
