namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour créer une transaction
    /// </summary>
    public class CreerTransactionDTO
    {
        public int MembreId { get; set; }
        public int? FilmId { get; set; }
        public string TypeTransaction { get; set; } = string.Empty; // "Achat", "Location", "Abonnement", "AjoutSolde"
        public decimal Montant { get; set; }
    }

    /// <summary>
    /// DTO pour afficher une transaction
    /// </summary>
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string TypeTransaction { get; set; } = string.Empty;
        public decimal Montant { get; set; }
        public DateTime DateTransaction { get; set; }
        public string? FilmTitre { get; set; }
    }

    /// <summary>
    /// Résultat de la création d'une transaction
    /// </summary>
    public class ResultatTransactionDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? TransactionId { get; set; }
    }
}
