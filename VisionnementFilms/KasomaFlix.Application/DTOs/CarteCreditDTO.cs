namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour créer une carte de crédit
    /// </summary>
    public class CreerCarteCreditDTO
    {
        public int MembreId { get; set; }
        public string TypeCarte { get; set; } = string.Empty; // "Visa", "Mastercard", "PayPal", etc.
        public string NumeroCarte { get; set; } = string.Empty; // Numéro complet (sera masqué lors de l'affichage)
        public string NomTitulaire { get; set; } = string.Empty;
        public DateTime DateExpiration { get; set; }
        public string? EmailPayPal { get; set; } // Pour PayPal uniquement
        public bool EstParDefaut { get; set; } = false;
    }

    /// <summary>
    /// DTO pour afficher une carte de crédit (sans numéro complet)
    /// </summary>
    public class CarteCreditDTO
    {
        public int Id { get; set; }
        public string TypeCarte { get; set; } = string.Empty;
        public string NumeroCarteMasque { get; set; } = string.Empty; // Derniers 4 chiffres seulement
        public string NomTitulaire { get; set; } = string.Empty;
        public DateTime DateExpiration { get; set; }
        public string? EmailPayPal { get; set; }
        public bool EstParDefaut { get; set; }
        public DateTime DateAjout { get; set; }
    }

    /// <summary>
    /// Résultat de la création d'une carte de crédit
    /// </summary>
    public class ResultatCarteCreditDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CarteCreditId { get; set; }
    }
}
