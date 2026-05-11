using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionPaiements
{
    /// <summary>
    /// Cas d'utilisation : Obtenir les cartes de crédit d'un membre
    /// </summary>
    public class ObtenirCartesCreditUseCase
    {
        private readonly ICarteCreditRepository _carteCreditRepository;

        public ObtenirCartesCreditUseCase(ICarteCreditRepository carteCreditRepository)
        {
            _carteCreditRepository = carteCreditRepository;
        }

        public async Task<IEnumerable<CarteCreditDTO>> ExecuteAsync(int membreId)
        {
            var cartes = await _carteCreditRepository.GetByMembreIdAsync(membreId);

            return cartes.Select(c => new CarteCreditDTO
            {
                Id = c.Id,
                TypeCarte = c.TypeCarte,
                NumeroCarteMasque = c.NumeroCarte, // Déjà masqué dans la base
                NomTitulaire = c.NomTitulaire,
                DateExpiration = c.DateExpiration,
                EmailPayPal = c.EmailPayPal,
                EstParDefaut = c.EstParDefaut,
                DateAjout = c.DateAjout
            });
        }
    }
}
