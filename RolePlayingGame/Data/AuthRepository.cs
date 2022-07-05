using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RolePlayingGame.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RolePlayingGame.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext dataContext, IConfiguration configuration)
        {
            _dbContext = dataContext;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                response.Success = false;
                response.Message = "Username does not exist.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                response.Data = CreateJwtToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(string username, string password)
        {
            var response = new ServiceResponse<int>();
            if (await UserExists(username))
            {
                response.Success = false;
                response.Message = "Username already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _dbContext.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    
        private string CreateJwtToken(User user)
        {
            // Step 1: Create a list of Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            // Step 2: Create security key using the jwt secret from configs
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value)
            );
            // Step 3: Create signing creds using the security key from Step 2.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // Step 4: Create a token descriptor using claims from Step 1 &
            //         signing creds from Step 3
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            // Step 5: create a new token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            // Step 6: create the token using token handler from Step 5 &,
            //         token descriptor from Step 4
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Step 7: get and return the token as string using token handler
            return tokenHandler.WriteToken(token);
        }
    }
}
