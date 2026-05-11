using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;
using KasomaFlix.Infrastructure.Services;

namespace KasomaFlix.Application.UseCases.Connexion
{
    /// <summary>
    /// Cas d'utilisation : Connexion d'un utilisateur (membre ou administrateur)
    /// </summary>
    public class ConnexionUseCase
    {
        private readonly IMembreRepository _membreRepository;
        private readonly IAdministrateurRepository _administrateurRepository;

        public ConnexionUseCase(
            IMembreRepository membreRepository,
            IAdministrateurRepository administrateurRepository)
        {
            _membreRepository = membreRepository;
            _administrateurRepository = administrateurRepository;
        }

        public async Task<ResultatConnexionDTO> ExecuteAsync(ConnexionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Identifiant) || string.IsNullOrWhiteSpace(dto.MotDePasse))
            {
                return new ResultatConnexionDTO
                {
                    Succes = false,
                    Message = "L'identifiant et le mot de passe sont requis."
                };
            }

            // Essayer d'abord comme administrateur
            var admin = await _administrateurRepository.GetByNomUtilisateurAsync(dto.Identifiant);
            if (admin != null)
            {
                // Pour l'admin : accepter le mot de passe en clair OU vérifier le hash BCrypt
                // Cela permet de tester avec un mot de passe en clair dans la base de données
                bool motDePasseValide = false;
                
                // Vérifier d'abord si c'est un hash BCrypt (commence par $2a$, $2b$ ou $2y$)
                if (admin.MotDePasseHash.StartsWith("$2a$") || 
                    admin.MotDePasseHash.StartsWith("$2b$") || 
                    admin.MotDePasseHash.StartsWith("$2y$"))
                {
                    // C'est un hash BCrypt, utiliser la vérification normale
                    motDePasseValide = PasswordHasher.VerifyPassword(dto.MotDePasse, admin.MotDePasseHash);
                }
                else
                {
                    // C'est un mot de passe en clair, comparer directement
                    motDePasseValide = admin.MotDePasseHash == dto.MotDePasse;
                }
                
                if (motDePasseValide)
                {
                    return new ResultatConnexionDTO
                    {
                        Succes = true,
                        Message = "Connexion administrateur réussie.",
                        TypeUtilisateur = "Administrateur",
                        UtilisateurId = admin.Id,
                        NomUtilisateur = admin.NomUtilisateur
                    };
                }
                else
                {
                    // Admin trouvé mais mot de passe incorrect
                    return new ResultatConnexionDTO
                    {
                        Succes = false,
                        Message = "Mot de passe incorrect pour l'administrateur."
                    };
                }
            }

            // Essayer comme membre (par courriel)
            var membre = await _membreRepository.GetByCourrielAsync(dto.Identifiant);
            if (membre != null)
            {
                // Vérifier le mot de passe
                if (PasswordHasher.VerifyPassword(dto.MotDePasse, membre.MotDePasseHash))
                {
                    return new ResultatConnexionDTO
                    {
                        Succes = true,
                        Message = "Connexion membre réussie.",
                        TypeUtilisateur = "Membre",
                        UtilisateurId = membre.Id,
                        NomUtilisateur = $"{membre.Prenom} {membre.Nom}"
                    };
                }
                else
                {
                    // Membre trouvé mais mot de passe incorrect
                    return new ResultatConnexionDTO
                    {
                        Succes = false,
                        Message = "Mot de passe incorrect."
                    };
                }
            }

            // Aucun utilisateur trouvé
            return new ResultatConnexionDTO
            {
                Succes = false,
                Message = "Identifiants invalides. Vérifiez votre nom d'utilisateur ou courriel."
            };
        }
    }
}

