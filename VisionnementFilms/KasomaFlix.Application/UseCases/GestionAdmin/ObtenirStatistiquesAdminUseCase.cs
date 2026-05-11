using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Obtenir les statistiques pour le tableau de bord admin
    /// </summary>
    public class ObtenirStatistiquesAdminUseCase
    {
        private readonly IMembreRepository _membreRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ITransactionRepository _transactionRepository;

        public ObtenirStatistiquesAdminUseCase(
            IMembreRepository membreRepository,
            IFilmRepository filmRepository,
            ITransactionRepository transactionRepository)
        {
            _membreRepository = membreRepository;
            _filmRepository = filmRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<StatistiquesAdminDTO> ExecuteAsync()
        {
            var membres = await _membreRepository.GetAllAsync();
            var films = await _filmRepository.GetAllAsync();
            var toutesTransactions = await _transactionRepository.GetAllAsync();

            // Calculer les revenus du mois en cours
            var debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);
            
            var revenusMois = toutesTransactions
                .Where(t => t.DateTransaction >= debutMois && 
                           t.DateTransaction <= finMois && 
                           t.Statut == "Complétée" &&
                           t.TypeTransaction != "AjoutSolde") // Exclure les ajouts de solde
                .Sum(t => t.Montant);

            return new StatistiquesAdminDTO
            {
                NombreMembres = membres.Count(),
                NombreFilms = films.Count(),
                NombreTransactions = toutesTransactions.Count(),
                RevenusMois = revenusMois
            };
        }
    }
}
