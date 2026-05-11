namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant une session de visionnement d'un film
    /// </summary>
    public class Session
    {
        public int Id { get; set; }
        public int MembreId { get; set; }
        public int FilmId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int TempsVisionne { get; set; } // en secondes
        public bool EstTerminee { get; set; } = false;

        // Navigation properties
        public virtual Membre Membre { get; set; } = null!;
        public virtual Film Film { get; set; } = null!;
    }
}

