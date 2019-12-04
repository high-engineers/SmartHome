using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SmartHome.BusinessLogic.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string GetHashedString(this string obj)
        {
            using (var hash = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(obj);
                var hashed = hash.ComputeHash(bytes);
                return string.Join(
                    string.Empty,
                    hashed.Select(x => x.ToString("x2")));
            }
        }
    }
}
