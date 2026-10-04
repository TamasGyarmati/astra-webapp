using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Students.App.Data;
using Students.App.Models;

namespace Students.App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private const int SaltSize = 16;
    private const int Iterations = 100_000;

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    [HttpGet]
    [ActionName(nameof(GetUsers))]
    public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
    {
        var users = await db.Users.AsNoTracking().ToListAsync();
        return Ok(users.Select(ToUserInfo));
    }

    [HttpPut("register")]
    [ActionName(nameof(RegisterUser))]
    public async Task<ActionResult<UserInfo>> RegisterUser(Register body)
    {
        var email = body.Email.Trim();
        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            return Conflict(new { message = "This email address is already registered." });
        }

        var (hash, salt) = HashPassword(body.Password);

        var user = new User
        {
            UserName = email.Split('@')[0],
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            FirstName = body.FirstName.Trim(),
            LastName = body.LastName.Trim(),
            Roles = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToUserInfo(user));
    }

    [HttpPost("login")]
    [ActionName(nameof(LoginUser))]
    public async Task<ActionResult<LoginResponse>> LoginUser(Login body)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == body.Email.Trim());
        if (user is null || !VerifyPassword(body.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(CreateToken(user));
    }

    private static UserInfo ToUserInfo(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Roles = SplitRoles(user.Roles)
    };

    private static string[] SplitRoles(string roles) =>
        roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private LoginResponse CreateToken(User user)
    {
        var jwt = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var expiration = DateTime.UtcNow.Add(TokenLifetime);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(SplitRoles(user.Roles).Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }

    private static (string Hash, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }

    private static bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromHexString(salt);
        var expectedHash = Convert.FromHexString(hash);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password, saltBytes, Iterations, HashAlgorithmName.SHA256, expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
