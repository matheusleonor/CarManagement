using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CarManagement.Models;
using System.Data.SqlClient;
using System.Text.Json;

public class VeiculoDAO
{
    private readonly string connectionString;

    public VeiculoDAO(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CarManagementConnection");
    }

    public List<Veiculo> GetAll()
    {
        List<Veiculo> veiculos = new List<Veiculo>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT v.Id, v.Placa, v.Renavam, v.NumeroChassi, v.NumeroMotor, v.AnoFabricacao, v.Status, v.Fotos,
                   m.Nome AS MarcaNome, mo.Nome AS ModeloNome, c.Descricao AS CorDescricao, cb.Descricao AS CombustivelDescricao
            FROM Veiculo v
            INNER JOIN Marca m ON v.MarcaId = m.Id
            INNER JOIN Modelo mo ON v.ModeloId = mo.Id
            INNER JOIN Cor c ON v.CorId = c.Id
            INNER JOIN Combustivel cb ON v.CombustivelId = cb.Id";

            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Veiculo veiculo = new Veiculo
                {
                    Id = (int)reader["Id"],
                    Placa = reader["Placa"].ToString(),
                    Renavam = reader["Renavam"].ToString(),
                    NumeroChassi = reader["NumeroChassi"].ToString(),
                    NumeroMotor = reader["NumeroMotor"].ToString(),
                    AnoFabricacao = (int)reader["AnoFabricacao"],
                    Status = (bool)reader["Status"],
                    Fotos = reader["Fotos"] != DBNull.Value ? reader["Fotos"].ToString().Split(';').ToList() : new List<string>(),
                    Marca = new Marca { Nome = reader["MarcaNome"].ToString() },
                    Modelo = new Modelo { Nome = reader["ModeloNome"].ToString() },
                    Cor = new Cor { Descricao = reader["CorDescricao"].ToString() },
                    Combustivel = new Combustivel { Descricao = reader["CombustivelDescricao"].ToString() }
                };
                veiculos.Add(veiculo);
            }
        }
        return veiculos;
    }

    public void Add(Veiculo veiculo)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string fotosConcatenadas = veiculo.Fotos != null && veiculo.Fotos.Any()
                ? string.Join(";", veiculo.Fotos)
                : null;

            string query = "INSERT INTO Veiculo (Placa, Renavam, NumeroChassi, NumeroMotor, AnoFabricacao, Status, MarcaId, ModeloId, CombustivelId, CorId, Fotos) " +
                "VALUES (@Placa, @Renavam, @NumeroChassi, @NumeroMotor, @AnoFabricacao, @Status, @MarcaId, @ModeloId, @CombustivelId, @CorId, @Fotos)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Placa", veiculo.Placa);
            cmd.Parameters.AddWithValue("@Renavam", veiculo.Renavam);
            cmd.Parameters.AddWithValue("@NumeroChassi", veiculo.NumeroChassi);
            cmd.Parameters.AddWithValue("@NumeroMotor", veiculo.NumeroMotor);
            cmd.Parameters.AddWithValue("@AnoFabricacao", veiculo.AnoFabricacao);
            cmd.Parameters.AddWithValue("@Status", veiculo.Status);
            cmd.Parameters.AddWithValue("@MarcaId", veiculo.MarcaId);
            cmd.Parameters.AddWithValue("@ModeloId", veiculo.ModeloId);
            cmd.Parameters.AddWithValue("@CombustivelId", veiculo.CombustivelId);
            cmd.Parameters.AddWithValue("@CorId", veiculo.CorId);
            cmd.Parameters.AddWithValue("@Fotos", (object)fotosConcatenadas ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Veiculo GetById(int id)
    {
        Veiculo veiculo = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT v.Id, v.Placa, v.Renavam, v.NumeroChassi, v.NumeroMotor, v.AnoFabricacao, v.Status, v.Fotos,
                   v.MarcaId, m.Nome AS MarcaNome,
                   v.ModeloId, mo.Nome AS ModeloNome,
                   v.CorId, c.Descricao AS CorDescricao,
                   v.CombustivelId, cb.Descricao AS CombustivelDescricao
            FROM Veiculo v
            INNER JOIN Marca m ON v.MarcaId = m.Id
            INNER JOIN Modelo mo ON v.ModeloId = mo.Id
            INNER JOIN Cor c ON v.CorId = c.Id
            INNER JOIN Combustivel cb ON v.CombustivelId = cb.Id
            WHERE v.Id = @Id";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                veiculo = new Veiculo
                {
                    Id = (int)reader["Id"],
                    Placa = reader["Placa"].ToString(),
                    Renavam = reader["Renavam"].ToString(),
                    NumeroChassi = reader["NumeroChassi"].ToString(),
                    NumeroMotor = reader["NumeroMotor"].ToString(),
                    AnoFabricacao = (int)reader["AnoFabricacao"],
                    Status = (bool)reader["Status"],
                    Fotos = reader["Fotos"] != DBNull.Value ? reader["Fotos"].ToString().Split(';').ToList() : new List<string>(),
                    MarcaId = (int)reader["MarcaId"],
                    Marca = new Marca { Nome = reader["MarcaNome"].ToString() },
                    ModeloId = (int)reader["ModeloId"],
                    Modelo = new Modelo { Nome = reader["ModeloNome"].ToString() },
                    CorId = (int)reader["CorId"],
                    Cor = new Cor { Descricao = reader["CorDescricao"].ToString() },
                    CombustivelId = (int)reader["CombustivelId"],
                    Combustivel = new Combustivel { Descricao = reader["CombustivelDescricao"].ToString() }
                };
            }
        }
        return veiculo;
    }

    public void Update(Veiculo veiculo)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string fotosConcatenadas = veiculo.Fotos != null && veiculo.Fotos.Any()
                ? string.Join(";", veiculo.Fotos)
                : null;

            string query = @"
                UPDATE Veiculo
                SET Placa = @Placa, Renavam = @Renavam, NumeroChassi = @NumeroChassi, NumeroMotor = @NumeroMotor, AnoFabricacao = @AnoFabricacao, 
                    Status = @Status, MarcaId = @MarcaId, ModeloId = @ModeloId, CombustivelId = @CombustivelId, CorId = @CorId, Fotos = @Fotos
                WHERE Id = @Id";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Placa", veiculo.Placa);
            cmd.Parameters.AddWithValue("@Renavam", veiculo.Renavam);
            cmd.Parameters.AddWithValue("@NumeroChassi", veiculo.NumeroChassi);
            cmd.Parameters.AddWithValue("@NumeroMotor", veiculo.NumeroMotor);
            cmd.Parameters.AddWithValue("@AnoFabricacao", veiculo.AnoFabricacao);
            cmd.Parameters.AddWithValue("@Status", veiculo.Status);
            cmd.Parameters.AddWithValue("@MarcaId", veiculo.MarcaId);
            cmd.Parameters.AddWithValue("@ModeloId", veiculo.ModeloId);
            cmd.Parameters.AddWithValue("@CombustivelId", veiculo.CombustivelId);
            cmd.Parameters.AddWithValue("@CorId", veiculo.CorId);
            cmd.Parameters.AddWithValue("@Fotos", (object)fotosConcatenadas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", veiculo.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Veiculo WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public bool PlacaExists(string placa, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Veiculo WHERE Placa = @Placa AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Placa", placa);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }

    public bool RenavamExists(string renavam, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Veiculo WHERE Renavam = @Renavam AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Renavam", renavam);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }

    public bool NumeroChassiExists(string numeroChassi, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Veiculo WHERE NumeroChassi = @NumeroChassi AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@NumeroChassi", numeroChassi);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }

    public bool NumeroMotorExists(string numeroMotor, int id = 0)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM Veiculo WHERE NumeroMotor = @NumeroMotor AND Id <> @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@NumeroMotor", numeroMotor);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
