namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant un abonnement d'un membre
    /// </summary>
    public class Abonnement
    {
        public int Id { get; set; }
        public int MembreId { get; set; }
        public string TypeAbonnement { get; set; } = string.Empty; // "Mensuel", "Annuel", etc.
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal Prix { get; set; }
        public bool RenouvellementAutomatique { get; set; } = true;
        public bool EstActif { get; set; } = true;

        // Navigation property
        public virtual Membre Membre { get; set; } = null!;
    }
}

