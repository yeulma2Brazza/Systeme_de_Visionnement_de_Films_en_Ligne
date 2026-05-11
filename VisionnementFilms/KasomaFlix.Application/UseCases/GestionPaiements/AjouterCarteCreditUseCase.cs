using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionPaiements
{
    /// <summary>
    /// Cas d'utilisation : Ajouter une carte de crédit ou un mode de paiement
    /// </summary>
    public class AjouterCarteCreditUseCase
    {
        private readonly ICarteCreditRepository _carteCreditRepository;
        private readonly IMembreRepository _membreRepository;

        public AjouterCarteCreditUseCase(
            ICarteCreditRepository carteCreditRepository,
            IMembreRepository membreRepository)
        {
            _carteCreditRepository = carteCreditRepository;
            _membreRepository = membreRepository;
        }

        public async Task<ResultatCarteCreditDTO> ExecuteAsync(CreerCarteCreditDTO dto)
        {
            // Vérifier que le membre existe
            var membre = await _membreRepository.GetByIdAsync(dto.MembreId);
            if (membre == null)
            {
                return new ResultatCarteCreditDTO
                {
                    Succes = false,
                    Message = "Membre introuvable."
                };
            }

            // Valider les données
            if (string.IsNullOrWhiteSpace(dto.TypeCarte))
            {
                return new ResultatCarteCreditDTO
                {
                    Succes = false,
                    Message = "Le type de carte est requis."
                };
            }

            // Pour les cartes de crédit, valider le numéro et la date d'expiration
            if (dto.TypeCarte != "PayPal")
            {
                if (string.IsNullOrWhiteSpace(dto.NumeroCarte) || dto.NumeroCarte.Length < 13)
                {
                    return new ResultatCarteCreditDTO
                    {
                        Succes = false,
                        Message = "Le numéro de carte est invalide."
                    };
                }

                if (dto.DateExpiration < DateTime.Now)
                {
                    return new ResultatCarteCreditDTO
                    {
                        Succes = false,
                        Message = "La date d'expiration est invalide."
                    };
                }
            }
            else
            {
                // Pour PayPal, valider l'email
                if (string.IsNullOrWhiteSpace(dto.EmailPayPal))
                {
                    return new ResultatCarteCreditDTO
                    {
                        Succes = false,
                        Message = "L'adresse email PayPal est requise."
                    };
                }
            }

            // Si cette carte est définie comme par défaut, désactiver les autres
            if (dto.EstParDefaut)
            {
                var cartesExistantes = await _carteCreditRepository.GetByMembreIdAsync(dto.MembreId);
                foreach (var carte in cartesExistantes.Where(c => c.EstParDefaut))
                {
                    carte.EstParDefaut = false;
                    await _carteCreditRepository.UpdateAsync(carte);
                }
            }

            // Masquer le numéro de carte (garder seulement les 4 derniers chiffres)
            string numeroMasque = string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.NumeroCarte) && dto.NumeroCarte.Length >= 4)
            {
                numeroMasque = dto.NumeroCarte.Substring(dto.NumeroCarte.Length - 4);
            }

            // Créer la carte de crédit
            var carteCredit = new CarteCredit
            {
                MembreId = dto.MembreId,
                TypeCarte = dto.TypeCarte,
                NumeroCarte = numeroMasque, // Stocker seulement les 4 derniers chiffres
                NomTitulaire = dto.NomTitulaire,
                DateExpiration = dto.DateExpiration,
                EmailPayPal = dto.EmailPayPal,
                EstParDefaut = dto.EstParDefaut,
                DateAjout = DateTime.Now
            };

            var carteCree = await _carteCreditRepository.AddAsync(carteCredit);

            return new ResultatCarteCreditDTO
            {
                Succes = true,
                Message = $"Mode de paiement {dto.TypeCarte} ajouté avec succès.",
                CarteCreditId = carteCree.Id
            };
        }
    }
}
