using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionTransactions
{
    /// <summary>
    /// Cas d'utilisation : Créer une transaction
    /// </summary>
    public class CreerTransactionUseCase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMembreRepository _membreRepository;

        public CreerTransactionUseCase(
            ITransactionRepository transactionRepository,
            IMembreRepository membreRepository)
        {
            _transactionRepository = transactionRepository;
            _membreRepository = membreRepository;
        }

        public async Task<ResultatTransactionDTO> ExecuteAsync(CreerTransactionDTO dto)
        {
            var membre = await _membreRepository.GetByIdAsync(dto.MembreId);
            if (membre == null)
            {
                return new ResultatTransactionDTO
                {
                    Succes = false,
                    Message = "Membre introuvable."
                };
            }

            // Pour les transactions autres que "AjoutSolde", vérifier le solde
            if (dto.TypeTransaction != "AjoutSolde" && dto.Montant > 0)
            {
                // Vérifier que le solde est suffisant
                if (membre.Solde < dto.Montant)
                {
                    return new ResultatTransactionDTO
                    {
                        Succes = false,
                        Message = $"Solde insuffisant. Solde actuel : {membre.Solde:F2} $. Montant requis : {dto.Montant:F2} $."
                    };
                }
            }

            // Créer la transaction
            // Pour "AjoutSolde", on enregistre le montant positif
            // Pour les autres types, on enregistre le montant négatif pour indiquer une sortie
            decimal montantTransaction = dto.TypeTransaction == "AjoutSolde" ? dto.Montant : -dto.Montant;
            
            var transaction = new Transaction
            {
                MembreId = dto.MembreId,
                FilmId = dto.FilmId,
                TypeTransaction = dto.TypeTransaction,
                Montant = montantTransaction,
                DateTransaction = DateTime.Now
            };

            await _transactionRepository.AddAsync(transaction);

            // Mettre à jour le solde du membre
            if (dto.TypeTransaction == "AjoutSolde")
            {
                membre.Solde += dto.Montant;
            }
            else
            {
                membre.Solde -= dto.Montant;
            }
            
            await _membreRepository.UpdateAsync(membre);

            return new ResultatTransactionDTO
            {
                Succes = true,
                Message = "Transaction créée avec succès.",
                TransactionId = transaction.Id
            };
        }
    }
}
