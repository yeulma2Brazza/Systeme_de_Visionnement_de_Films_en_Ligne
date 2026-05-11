using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Obtenir la liste de tous les membres pour l'administration
    /// </summary>
    public class ObtenirTousMembresUseCase
    {
        private readonly IMembreRepository _membreRepository;
        private readonly ITransactionRepository _transactionRepository;

        public ObtenirTousMembresUseCase(
            IMembreRepository membreRepository,
            ITransactionRepository transactionRepository)
        {
            _membreRepository = membreRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<MembreDTO>> ExecuteAsync()
        {
            var membres = await _membreRepository.GetAllAsync();
            var resultat = new List<MembreDTO>();

            foreach (var membre in membres)
            {
                var transactions = await _transactionRepository.GetByMembreIdAsync(membre.Id);
                
                resultat.Add(new MembreDTO
                {
                    Id = membre.Id,
                    Prenom = membre.Prenom,
                    Nom = membre.Nom,
                    Courriel = membre.Courriel,
                    Adresse = membre.Adresse ?? string.Empty,
                    Telephone = membre.Telephone ?? string.Empty,
                    Solde = membre.Solde,
                    DateInscription = membre.DateInscription,
                    NombreAbonnements = membre.Abonnements?.Count(a => a.EstActif) ?? 0,
                    NombreTransactions = transactions.Count()
                });
            }

            return resultat.OrderByDescending(m => m.DateInscription);
        }
    }
}
