using FoodOrder_API.Models.Dto;
using FoodOrder_API.Repository.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using System.IdentityModel.Tokens.Jwt;

namespace FoodOrder_API.Controllers
{
    [EnableCors("Cors")]
    [Route("api/auth")]
    [ApiController]
    public class UsersController: Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var LoginResponse = await _userRepository.Login(loginRequestDTO);
            if(LoginResponse.User == null || string.IsNullOrEmpty(LoginResponse.Token))
            {
                return BadRequest();
            }
            return LoginResponse;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            //bool userNameIsUnique = _userRepository.IsUniqueUser(registrationRequestDTO.UserName);
            //if(!userNameIsUnique)
            //{
            //    return BadRequest();
            //}

            var user = await _userRepository.Register(registrationRequestDTO);
            if(user == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("check")]
        public async Task<ActionResult<AnyType>> Check()
        {
            try
            {

            var tokenHandler = new JwtSecurityTokenHandler();
            var bearerToken = HttpContext.Request.Headers.Authorization.ToString();
            bearerToken = bearerToken.Substring(bearerToken.LastIndexOf(" ") + 1);
            var jwt = new JwtSecurityToken(bearerToken);
            var claimlist = jwt.Claims.ToList();
            var nameid = jwt.Claims.FirstOrDefault(c => c.Type == "nameid").Value;
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role").Value;

                var resp = new
                {
                    role = role,
                    claimlist = claimlist,
                };

                return Ok(resp);

            } catch (Exception ex)
            {
                return Unauthorized();
            }

        }
    }
}
