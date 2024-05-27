namespace CarManagement.Models
{
    public class Modelo
    {
        public int Id { get; set; }
        public int MarcaId { get; set; }
        public string Nome { get; set; }
        public bool Status { get; set; }

        // Propriedade de navegação
        public Marca Marca { get; set; }
    }

}
