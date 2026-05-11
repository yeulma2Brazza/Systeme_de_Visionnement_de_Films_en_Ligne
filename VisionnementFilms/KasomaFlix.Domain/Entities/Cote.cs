namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant une note et commentaire d'un membre sur un film
    /// </summary>
    public class Cote
    {
        public int Id { get; set; }
        public int MembreId { get; set; }
        public int FilmId { get; set; }
        public int Note { get; set; } // De 1 à 5
        public string? Commentaire { get; set; }
        public DateTime DateCote { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Membre Membre { get; set; } = null!;
        public virtual Film Film { get; set; } = null!;
    }
}

