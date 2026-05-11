namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour un abonnement
    /// </summary>
    public class AbonnementDTO
    {
        public int Id { get; set; }
        public string TypeAbonnement { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal Prix { get; set; }
        public bool EstActif { get; set; }
    }
}
