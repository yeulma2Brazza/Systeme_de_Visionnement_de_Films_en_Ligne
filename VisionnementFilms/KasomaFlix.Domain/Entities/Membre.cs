namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant un membre du système
    /// </summary>
    public class Membre
    {
        public int Id { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public string MotDePasseHash { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public decimal Solde { get; set; } = 0;
        public DateTime DateInscription { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
        public virtual ICollection<Cote> Cotes { get; set; } = new List<Cote>();
        public virtual ICollection<CarteCredit> CartesCredit { get; set; } = new List<CarteCredit>();
    }
}

