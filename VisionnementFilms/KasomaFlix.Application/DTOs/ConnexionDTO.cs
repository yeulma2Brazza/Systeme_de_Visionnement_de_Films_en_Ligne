namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour la connexion
    /// </summary>
    public class ConnexionDTO
    {
        public string Identifiant { get; set; } = string.Empty; // Courriel ou NomUtilisateur
        public string MotDePasse { get; set; } = string.Empty;
    }

    /// <summary>
    /// Résultat de la connexion
    /// </summary>
    public class ResultatConnexionDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TypeUtilisateur { get; set; } = string.Empty; // "Membre" ou "Administrateur"
        public int? UtilisateurId { get; set; }
        public string? NomUtilisateur { get; set; }
    }
}

