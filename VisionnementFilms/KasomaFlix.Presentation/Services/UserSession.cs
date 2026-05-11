using KasomaFlix.Application.DTOs;

namespace KasomaFlix.Presentation.Services
{
    /// <summary>
    /// Service pour gérer la session de l'utilisateur connecté
    /// </summary>
    public static class UserSession
    {
        private static ResultatConnexionDTO? _currentUser;

        public static void SetCurrentUser(ResultatConnexionDTO user)
        {
            _currentUser = user;
        }

        public static ResultatConnexionDTO? GetCurrentUser()
        {
            return _currentUser;
        }

        public static bool IsLoggedIn()
        {
            return _currentUser != null && _currentUser.Succes;
        }

        public static bool IsAdmin()
        {
            return _currentUser?.TypeUtilisateur == "Administrateur";
        }

        public static bool IsMembre()
        {
            return _currentUser?.TypeUtilisateur == "Membre";
        }

        public static int? GetUserId()
        {
            return _currentUser?.UtilisateurId;
        }

        public static void Logout()
        {
            _currentUser = null;
        }
    }
}
