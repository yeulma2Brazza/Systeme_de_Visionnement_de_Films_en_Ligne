namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour l'inscription d'un nouveau membre
    /// </summary>
    public class InscriptionDTO
    {
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Résultat de l'inscription
    /// </summary>
    public class ResultatInscriptionDTO
    {
        public bool Succes { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? MembreId { get; set; }
    }
}

