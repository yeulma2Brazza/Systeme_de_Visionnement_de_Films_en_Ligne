namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour les critères de recherche de films
    /// </summary>
    public class RechercheFilmsDTO
    {
        public string? Titre { get; set; }
        public string? Categorie { get; set; }
        public int? Annee { get; set; }
    }
}

