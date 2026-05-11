namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant une transaction (achat, location, abonnement)
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }
        public int MembreId { get; set; }
        public int? FilmId { get; set; } // Nullable car peut être un abonnement
        public string TypeTransaction { get; set; } = string.Empty; // "Achat", "Location", "Abonnement"
        public decimal Montant { get; set; }
        public DateTime DateTransaction { get; set; } = DateTime.Now;
        public string Statut { get; set; } = "Complétée"; // "Complétée", "Annulée", "En attente"

        // Navigation properties
        public virtual Membre Membre { get; set; } = null!;
        public virtual Film? Film { get; set; }
    }
}

