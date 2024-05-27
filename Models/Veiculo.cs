namespace CarManagement.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Renavam { get; set; }
        public string NumeroChassi { get; set; }
        public string NumeroMotor { get; set; }
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int CombustivelId { get; set; }
        public int CorId { get; set; }
        public int AnoFabricacao { get; set; }
        public bool Status { get; set; }
        public List<string> Fotos { get; set; } = new List<string>();

        public Marca Marca { get; set; }
        public Modelo Modelo { get; set; }
        public Combustivel Combustivel { get; set; }
        public Cor Cor { get; set; }
    }

}
