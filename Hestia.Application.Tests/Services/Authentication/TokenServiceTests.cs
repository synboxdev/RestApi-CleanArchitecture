using Hestia.Access.Requests.Authentication.Commands.CreateTokenLog;
using Hestia.Application.Models.Shared;
using Hestia.Application.Services.Authentication;
using Hestia.Domain.Enumerations;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Layers;
using Hestia.Mediator.Infrastructure.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hestia.Application.Tests.Services.Authentication;

public class TokenServiceTests
{
    private readonly Mock<IAccessLayer> _mockAccessLayer;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly IOptions<Jwt> _jwtOptions;
    private readonly TokenService _tokenService;
    private readonly MemoryCache _memoryCache;

    public TokenServiceTests()
    {
        _mockAccessLayer = new Mock<IAccessLayer>();
        _mockUserManager = MockUserManager<ApplicationUser>();

        _jwtOptions = Options.Create(new Jwt
        {
            Secret = "ThisIsA32ByteLongSecretKeyForJwtAuth",
            TokenExpiryInHours = 1,
            ValidAudience = "testAudience",
            ValidIssuer = "testIssuer"
        });

        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _tokenService = new TokenService(
            _mockAccessLayer.Object,
            _memoryCache,
            _mockUserManager.Object,
            _jwtOptions
        );
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@example.com",
            Role = Role.User
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateOrUpdateTokenLogCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        // Act
        string token = await _tokenService.GenerateTokenAsync(user);

        // Assert
        Assert.NotNull(token);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(user.Id, jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.UserName, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.Role.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);

        // Verify that the token is stored in the cache
        Assert.True(_memoryCache.TryGetValue(token, out object? cachedUserId));
        Assert.Equal(user.Id, cachedUserId);
    }

    [Fact]
    public async Task GetIdentityUserIdByToken_ShouldReturnUserId_WhenTokenIsValid()
    {
        // Arrange
        string token = "validToken";
        string userId = "1";
        _memoryCache.Set(token, userId);
        _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(new ApplicationUser { Id = userId });

        // Act
        string? result = await _tokenService.GetIdentityUserIdByToken(token);

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public async Task GetIdentityUserIdByToken_ShouldReturnNull_WhenTokenIsInvalid()
    {
        // Arrange
        string token = "invalidToken";

        // Act
        string? result = await _tokenService.GetIdentityUserIdByToken(token);

        // Assert
        Assert.Null(result);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }
}