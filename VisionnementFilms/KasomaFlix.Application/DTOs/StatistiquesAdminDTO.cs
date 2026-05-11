namespace KasomaFlix.Application.DTOs
{
    /// <summary>
    /// DTO pour les statistiques du tableau de bord admin
    /// </summary>
    public class StatistiquesAdminDTO
    {
        public int NombreMembres { get; set; }
        public int NombreFilms { get; set; }
        public int NombreTransactions { get; set; }
        public decimal RevenusMois { get; set; }
    }
}
