using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAbonnements
{
    /// <summary>
    /// Cas d'utilisation : Annuler le renouvellement automatique d'un abonnement
    /// </summary>
    public class AnnulerRenouvellementUseCase
    {
        private readonly IAbonnementRepository _abonnementRepository;

        public AnnulerRenouvellementUseCase(IAbonnementRepository abonnementRepository)
        {
            _abonnementRepository = abonnementRepository;
        }

        public async Task<bool> ExecuteAsync(int abonnementId)
        {
            var abonnement = await _abonnementRepository.GetByIdAsync(abonnementId);
            if (abonnement == null)
            {
                return false;
            }

            abonnement.RenouvellementAutomatique = false;
            await _abonnementRepository.UpdateAsync(abonnement);
            
            return true;
        }
    }
}
