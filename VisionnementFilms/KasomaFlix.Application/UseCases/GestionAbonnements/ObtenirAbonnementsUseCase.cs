using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAbonnements
{
    /// <summary>
    /// Cas d'utilisation : Obtenir les abonnements d'un membre
    /// </summary>
    public class ObtenirAbonnementsUseCase
    {
        private readonly IAbonnementRepository _abonnementRepository;

        public ObtenirAbonnementsUseCase(IAbonnementRepository abonnementRepository)
        {
            _abonnementRepository = abonnementRepository;
        }

        public async Task<IEnumerable<AbonnementDTO>> ExecuteAsync(int membreId)
        {
            var abonnements = await _abonnementRepository.GetByMembreIdAsync(membreId);
            var maintenant = DateTime.Now;

            return abonnements.Select(a => new AbonnementDTO
            {
                Id = a.Id,
                TypeAbonnement = a.TypeAbonnement,
                DateDebut = a.DateDebut,
                DateFin = a.DateFin,
                Prix = a.Prix,
                // Un abonnement est actif si le flag EstActif est true ET la date de fin n'est pas passée
                EstActif = a.EstActif && a.DateFin >= maintenant
            }).OrderByDescending(a => a.DateDebut);
        }
    }
}
