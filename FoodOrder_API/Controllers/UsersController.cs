using FoodOrder_API.Models.Dto;
using FoodOrder_API.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder_API.Controllers
{
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
    }
}
