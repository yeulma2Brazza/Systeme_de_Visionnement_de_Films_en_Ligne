using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Obtenir la liste de toutes les transactions pour l'administration
    /// </summary>
    public class ObtenirToutesTransactionsUseCase
    {
        private readonly ITransactionRepository _transactionRepository;

        public ObtenirToutesTransactionsUseCase(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionDTO>> ExecuteAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            
            return transactions.Select(t => new TransactionDTO
            {
                Id = t.Id,
                TypeTransaction = t.TypeTransaction,
                Montant = t.Montant,
                DateTransaction = t.DateTransaction,
                FilmTitre = t.Film?.Titre
            });
        }
    }
}
