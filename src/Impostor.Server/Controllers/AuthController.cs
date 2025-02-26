using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Impostor.Api.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Impostor.Server.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class AuthController(IOptions<ExtensionServerConfig> serverConfig, JwtSecurityTokenHandler tokenHandler) : ControllerBase
{
    private readonly ExtensionServerConfig _config = serverConfig.Value;
    private readonly byte[] _uuidBytes = Encoding.UTF8.GetBytes(Program.CurrentUuid.ToString());
    private const string Sha256 = SecurityAlgorithms.HmacSha256;

    [HttpPost]
    public IActionResult Login([FromBody] string validaToken)
    {
        if (_config.Token != validaToken)
            return BadRequest("Invalid token");
        
        var expires = DateTime.UtcNow.AddMinutes(30);
        var credential = new SigningCredentials(new SymmetricSecurityKey(_uuidBytes), Sha256);
        var token = new JwtSecurityToken(expires: expires, signingCredentials: credential);
        var tokenString = tokenHandler.WriteToken(token);
        return Ok(new
        {
            Token = tokenString,
        });
    }
}
