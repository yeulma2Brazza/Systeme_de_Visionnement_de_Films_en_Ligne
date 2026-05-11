namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour afficher les informations d'un membre
    /// </summary>
    public class MembreDTO
    {
        public int Id { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public decimal Solde { get; set; }
        public DateTime DateInscription { get; set; }
        public int NombreAbonnements { get; set; }
        public int NombreTransactions { get; set; }
    }
}
