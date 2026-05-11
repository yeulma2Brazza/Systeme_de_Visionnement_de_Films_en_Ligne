namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour les informations de profil d'un membre
    /// </summary>
    public class ProfilDTO
    {
        public int Id { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public decimal Solde { get; set; }
    }

    /// <summary>
    /// DTO pour modifier le profil
    /// </summary>
    public class ModifierProfilDTO
    {
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string? NouveauMotDePasse { get; set; }
    }

    /// <summary>
    /// Résultat de la modification du profil
    /// </summary>
    public class ResultatModificationProfilDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
