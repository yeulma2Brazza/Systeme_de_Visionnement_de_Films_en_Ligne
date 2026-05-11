using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionProfil
{
    /// <summary>
    /// Cas d'utilisation : Obtenir le profil d'un membre
    /// </summary>
    public class ObtenirProfilUseCase
    {
        private readonly IMembreRepository _membreRepository;

        public ObtenirProfilUseCase(IMembreRepository membreRepository)
        {
            _membreRepository = membreRepository;
        }

        public async Task<ProfilDTO?> ExecuteAsync(int membreId)
        {
            var membre = await _membreRepository.GetByIdAsync(membreId);
            if (membre == null)
            {
                return null;
            }

            return new ProfilDTO
            {
                Id = membre.Id,
                Prenom = membre.Prenom,
                Nom = membre.Nom,
                Courriel = membre.Courriel,
                Adresse = membre.Adresse,
                Telephone = membre.Telephone,
                Solde = membre.Solde
            };
        }
    }
}
