namespace KasomaFlix.Domain.Entities
{
    /// <summary>
    /// Entité représentant un administrateur du système
    /// </summary>
    public class Administrateur
    {
        public int Id { get; set; }
        public string NomUtilisateur { get; set; } = string.Empty;
        public string MotDePasseHash { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public bool EstActif { get; set; } = true;
    }
}

