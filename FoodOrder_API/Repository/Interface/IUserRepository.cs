using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;

namespace FoodOrder_API.Repository.Interface
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<User> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
