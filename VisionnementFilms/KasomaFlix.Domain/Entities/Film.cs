namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant un film dans le catalogue
    /// </summary>
    public class Film
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public int Duree { get; set; } // en minutes
        public int Annee { get; set; }
        public decimal NoteMoyenne { get; set; }
        public int NombreVotes { get; set; }
        public string Realisateur { get; set; } = string.Empty;
        public string Acteurs { get; set; } = string.Empty; // Liste séparée par des virgules
        public decimal PrixAchat { get; set; }
        public decimal PrixLocation { get; set; }
        public string CheminAffiche { get; set; } = string.Empty;
        public bool EstDisponible { get; set; } = true;
        public DateTime DateAjout { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
        public virtual ICollection<Cote> Cotes { get; set; } = new List<Cote>();
    }
}

