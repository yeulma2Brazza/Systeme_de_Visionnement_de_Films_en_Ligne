using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionTransactions
{
    /// <summary>
    /// Cas d'utilisation : Obtenir les transactions d'un membre
    /// </summary>
    public class ObtenirTransactionsUseCase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFilmRepository _filmRepository;

        public ObtenirTransactionsUseCase(
            ITransactionRepository transactionRepository,
            IFilmRepository filmRepository)
        {
            _transactionRepository = transactionRepository;
            _filmRepository = filmRepository;
        }

        public async Task<IEnumerable<TransactionDTO>> ExecuteAsync(int membreId)
        {
            var transactions = await _transactionRepository.GetByMembreIdAsync(membreId);

            var result = new List<TransactionDTO>();
            foreach (var transaction in transactions)
            {
                string? filmTitre = null;
                if (transaction.FilmId.HasValue)
                {
                    var film = await _filmRepository.GetByIdAsync(transaction.FilmId.Value);
                    filmTitre = film?.Titre;
                }

                result.Add(new TransactionDTO
                {
                    Id = transaction.Id,
                    TypeTransaction = transaction.TypeTransaction,
                    Montant = transaction.Montant,
                    DateTransaction = transaction.DateTransaction,
                    FilmTitre = filmTitre
                });
            }

            return result.OrderByDescending(t => t.DateTransaction);
        }
    }
}
