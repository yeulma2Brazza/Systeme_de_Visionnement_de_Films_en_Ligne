namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour créer un abonnement
    /// </summary>
    public class CreerAbonnementDTO
    {
        public int MembreId { get; set; }
        public string TypeAbonnement { get; set; } = string.Empty; // "Mensuel", "Annuel"
        public decimal Prix { get; set; }
        public bool RenouvellementAutomatique { get; set; } = true;
    }

    /// <summary>
    /// Résultat de la création d'un abonnement
    /// </summary>
    public class ResultatAbonnementDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? AbonnementId { get; set; }
    }
}
