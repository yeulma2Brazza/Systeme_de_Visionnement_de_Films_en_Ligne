using BCrypt.Net;

namespace KasomaFlix.Infrastructure.Services
{
    /// <summary>
    /// Service pour le hashage des mots de passe
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Hash un mot de passe
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Vérifie si un mot de passe correspond au hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}

