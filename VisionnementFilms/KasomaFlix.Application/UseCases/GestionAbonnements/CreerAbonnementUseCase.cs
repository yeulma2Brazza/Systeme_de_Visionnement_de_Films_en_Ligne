using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAbonnements
{
    /// <summary>
    /// Cas d'utilisation : Créer/Activer un abonnement
    /// </summary>
    public class CreerAbonnementUseCase
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly IMembreRepository _membreRepository;
        private readonly ITransactionRepository _transactionRepository;

        public CreerAbonnementUseCase(
            IAbonnementRepository abonnementRepository,
            IMembreRepository membreRepository,
            ITransactionRepository transactionRepository)
        {
            _abonnementRepository = abonnementRepository;
            _membreRepository = membreRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<ResultatAbonnementDTO> ExecuteAsync(CreerAbonnementDTO dto)
        {
            // Vérifier que le membre existe
            var membre = await _membreRepository.GetByIdAsync(dto.MembreId);
            if (membre == null)
            {
                return new ResultatAbonnementDTO
                {
                    Succes = false,
                    Message = "Membre introuvable."
                };
            }

            // Vérifier que le solde est suffisant
            if (membre.Solde < dto.Prix)
            {
                return new ResultatAbonnementDTO
                {
                    Succes = false,
                    Message = $"Solde insuffisant. Solde actuel : {membre.Solde:F2} $. Montant requis : {dto.Prix:F2} $."
                };
            }

            // Désactiver les anciens abonnements actifs
            var abonnementsExistants = await _abonnementRepository.GetByMembreIdAsync(dto.MembreId);
            var maintenant = DateTime.Now;
            
            foreach (var abonnementExistant in abonnementsExistants.Where(a => a.EstActif && a.DateFin >= maintenant))
            {
                abonnementExistant.EstActif = false;
                await _abonnementRepository.UpdateAsync(abonnementExistant);
            }

            // Calculer les dates de début et fin
            var dateDebut = DateTime.Now;
            DateTime dateFin;
            
            if (dto.TypeAbonnement.Contains("Mensuel", StringComparison.OrdinalIgnoreCase))
            {
                dateFin = dateDebut.AddMonths(1);
            }
            else if (dto.TypeAbonnement.Contains("Annuel", StringComparison.OrdinalIgnoreCase))
            {
                dateFin = dateDebut.AddYears(1);
            }
            else
            {
                // Par défaut, mensuel
                dateFin = dateDebut.AddMonths(1);
            }

            // Créer la transaction pour l'abonnement (montant négatif pour indiquer une sortie)
            var transaction = new Transaction
            {
                MembreId = dto.MembreId,
                FilmId = null,
                TypeTransaction = "Abonnement",
                Montant = -dto.Prix, // Montant négatif pour indiquer une sortie
                DateTransaction = DateTime.Now
            };

            await _transactionRepository.AddAsync(transaction);

            // Mettre à jour le solde du membre (soustraire)
            membre.Solde -= dto.Prix;
            await _membreRepository.UpdateAsync(membre);

            // Créer l'abonnement
            var abonnement = new Abonnement
            {
                MembreId = dto.MembreId,
                TypeAbonnement = dto.TypeAbonnement,
                DateDebut = dateDebut,
                DateFin = dateFin,
                Prix = dto.Prix,
                RenouvellementAutomatique = dto.RenouvellementAutomatique,
                EstActif = true
            };

            var abonnementCree = await _abonnementRepository.AddAsync(abonnement);

            return new ResultatAbonnementDTO
            {
                Succes = true,
                Message = $"Abonnement {dto.TypeAbonnement} activé avec succès jusqu'au {dateFin:dd/MM/yyyy}.",
                AbonnementId = abonnementCree.Id
            };
        }
    }
}
