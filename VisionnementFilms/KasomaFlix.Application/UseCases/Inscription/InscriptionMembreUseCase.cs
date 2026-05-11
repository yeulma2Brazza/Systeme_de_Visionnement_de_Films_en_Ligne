using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;
using KasomaFlix.Infrastructure.Services;

namespace KasomaFlix.Application.UseCases.Inscription
{
    /// <summary>
    /// Cas d'utilisation : Inscription d'un nouveau membre
    /// </summary>
    public class InscriptionMembreUseCase
    {
        private readonly IMembreRepository _membreRepository;

        public InscriptionMembreUseCase(IMembreRepository membreRepository)
        {
            _membreRepository = membreRepository;
        }

        public async Task<ResultatInscriptionDTO> ExecuteAsync(InscriptionDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Courriel))
            {
                return new ResultatInscriptionDTO
                {
                    Succes = false,
                    Message = "Le courriel est requis."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.MotDePasse) || dto.MotDePasse.Length < 6)
            {
                return new ResultatInscriptionDTO
                {
                    Succes = false,
                    Message = "Le mot de passe doit contenir au moins 6 caractères."
                };
            }

            // Vérifier si le courriel existe déjà
            var existe = await _membreRepository.ExistsByCourrielAsync(dto.Courriel);
            if (existe)
            {
                return new ResultatInscriptionDTO
                {
                    Succes = false,
                    Message = "Un compte avec ce courriel existe déjà."
                };
            }

            // Créer le nouveau membre
            var membre = new Membre
            {
                Prenom = dto.Prenom,
                Nom = dto.Nom,
                Courriel = dto.Courriel,
                MotDePasseHash = PasswordHasher.HashPassword(dto.MotDePasse),
                Adresse = dto.Adresse,
                Telephone = dto.Telephone,
                DateInscription = DateTime.Now
            };

            var membreCree = await _membreRepository.AddAsync(membre);

            return new ResultatInscriptionDTO
            {
                Succes = true,
                Message = "Inscription réussie !",
                MembreId = membreCree.Id
            };
        }
    }
}

