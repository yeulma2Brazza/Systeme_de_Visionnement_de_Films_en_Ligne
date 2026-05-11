using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionProfil
{
    /// <summary>
    /// Cas d'utilisation : Modifier le profil d'un membre
    /// </summary>
    public class ModifierProfilUseCase
    {
        private readonly IMembreRepository _membreRepository;
        private readonly Infrastructure.Services.PasswordHasher _passwordHasher;

        public ModifierProfilUseCase(IMembreRepository membreRepository)
        {
            _membreRepository = membreRepository;
            _passwordHasher = new Infrastructure.Services.PasswordHasher();
        }

        public async Task<ResultatModificationProfilDTO> ExecuteAsync(int membreId, ModifierProfilDTO dto)
        {
            var membre = await _membreRepository.GetByIdAsync(membreId);
            if (membre == null)
            {
                return new ResultatModificationProfilDTO
                {
                    Succes = false,
                    Message = "Membre introuvable."
                };
            }

            // Mettre à jour les informations
            membre.Prenom = dto.Prenom;
            membre.Nom = dto.Nom;
            membre.Adresse = dto.Adresse;
            membre.Telephone = dto.Telephone;

            // Si un nouveau mot de passe est fourni, le hasher
            if (!string.IsNullOrWhiteSpace(dto.NouveauMotDePasse))
            {
                membre.MotDePasseHash = Infrastructure.Services.PasswordHasher.HashPassword(dto.NouveauMotDePasse);
            }

            await _membreRepository.UpdateAsync(membre);

            return new ResultatModificationProfilDTO
            {
                Succes = true,
                Message = "Profil mis à jour avec succès."
            };
        }
    }
}
