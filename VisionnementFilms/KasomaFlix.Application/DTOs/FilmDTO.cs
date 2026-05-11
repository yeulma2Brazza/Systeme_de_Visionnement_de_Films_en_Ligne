namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour un film
    /// </summary>
    public class FilmDTO
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public int Duree { get; set; }
        public int Annee { get; set; }
        public decimal NoteMoyenne { get; set; }
        public int NombreVotes { get; set; }
        public string Realisateur { get; set; } = string.Empty;
        public string Acteurs { get; set; } = string.Empty;
        public decimal PrixAchat { get; set; }
        public decimal PrixLocation { get; set; }
        public string CheminAffiche { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer ou modifier un film
    /// </summary>
    public class CreateFilmDTO
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public int Duree { get; set; }
        public int Annee { get; set; }
        public string Realisateur { get; set; } = string.Empty;
        public string Acteurs { get; set; } = string.Empty;
        public decimal PrixAchat { get; set; }
        public decimal PrixLocation { get; set; }
        public string CheminAffiche { get; set; } = string.Empty;
    }
}

