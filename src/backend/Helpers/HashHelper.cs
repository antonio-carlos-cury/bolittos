using System.Security.Cryptography;
using System.Text;

	public (string hashedPassword, string salt) HashPassword(string password)
	{
		int saltSize = 16;
		int iterations = 10000;
		int hashSize = 32;
		
		using (var deriveBytes = new Rfc2898DeriveBytes(password, saltSize, iterations, HashAlgorithmName.SHA256))
		{
			byte[] salt = deriveBytes.Salt;
			byte[] hash = deriveBytes.GetBytes(hashSize);
			
			return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
		}
	}
	
	public bool VerifyPassword(string password, string storedHashedPassword, string storedSalt)
	{
		byte[] saltBytes = Convert.FromBase64String(storedSalt);
		byte[] storedHashBytes = Convert.FromBase64String(storedHashedPassword);
		
		int iterations = 10000;
		int hashSize = 32;
		
		using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
		{
			byte[] computedHash = deriveBytes.GetBytes(hashSize);
			return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
		}
	}