using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CarManagement.Models;
using System.Data.SqlClient;

public class ModeloDAO
{
    private readonly string connectionString;

    public ModeloDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<Modelo> GetAll()
    {
        List<Modelo> modelos = new List<Modelo>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Modelo";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Modelo modelo = new Modelo
                {
                    Id = (int)reader["Id"],
                    MarcaId = (int)reader["MarcaId"],
                    Nome = reader["Nome"].ToString(),
                    Status = (bool)reader["Status"]
                };
                modelos.Add(modelo);
            }
        }
        return modelos;
    }

    public void Add(Modelo modelo)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Modelo (MarcaId, Nome, Status) VALUES (@MarcaId, @Nome, @Status)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MarcaId", modelo.MarcaId);
            cmd.Parameters.AddWithValue("@Nome", modelo.Nome);
            cmd.Parameters.AddWithValue("@Status", modelo.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Modelo GetById(int id)
    {
        Modelo modelo = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Modelo WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                modelo = new Modelo
                {
                    Id = (int)reader["Id"],
                    MarcaId = (int)reader["MarcaId"],
                    Nome = reader["Nome"].ToString(),
                    Status = (bool)reader["Status"]
                };
            }
        }
        return modelo;
    }

    public void Update(Modelo modelo)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Modelo SET MarcaId = @MarcaId, Nome = @Nome, Status = @Status WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MarcaId", modelo.MarcaId);
            cmd.Parameters.AddWithValue("@Nome", modelo.Nome);
            cmd.Parameters.AddWithValue("@Status", modelo.Status);
            cmd.Parameters.AddWithValue("@Id", modelo.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Modelo WHERE Id = @Id";
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
            string query = "SELECT COUNT(*) FROM Modelo WHERE Nome = @Nome AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
