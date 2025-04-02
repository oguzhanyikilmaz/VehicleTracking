using System;
using System.Security.Cryptography;
using System.Text;

namespace VehicleTracking.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;
        
        public string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password, 
                SaltSize, 
                Iterations, 
                HashAlgorithmName.SHA256);
            
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Iterations}.{salt}.{key}";
        }
        
        public bool VerifyPassword(string hash, string password)
        {
            var parts = hash.Split('.', 3);
            
            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. Should be {iterations}.{salt}.{hash}");
            }
            
            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);
            
            using var algorithm = new Rfc2898DeriveBytes(
                password, 
                salt, 
                iterations, 
                HashAlgorithmName.SHA256);
            
            var keyToCheck = algorithm.GetBytes(KeySize);
            
            var verified = keyToCheck.Length == key.Length;
            
            for (var i = 0; i < keyToCheck.Length && i < key.Length; i++)
            {
                verified &= keyToCheck[i] == key[i];
            }
            
            return verified;
        }
    }
    
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hash, string password);
    }
} 