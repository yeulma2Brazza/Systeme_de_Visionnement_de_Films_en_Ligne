namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant une carte de crédit ou un mode de paiement
    /// </summary>
    public class CarteCredit
    {
        public int Id { get; set; }
        public int MembreId { get; set; }
        public string TypeCarte { get; set; } = string.Empty; // "Visa", "Mastercard", "PayPal", etc.
        public string NumeroCarte { get; set; } = string.Empty; // Stocké de manière sécurisée (derniers 4 chiffres seulement)
        public string NomTitulaire { get; set; } = string.Empty;
        public DateTime DateExpiration { get; set; }
        public string? EmailPayPal { get; set; } // Pour PayPal uniquement
        public bool EstParDefaut { get; set; } = false;
        public DateTime DateAjout { get; set; } = DateTime.Now;

        // Navigation property
        public Membre? Membre { get; set; }
    }
}
