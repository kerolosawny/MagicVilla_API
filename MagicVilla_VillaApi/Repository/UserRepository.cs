using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string secretkey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserRepository( ApplicationDbContext context , UserManager<ApplicationUser> userManager
            , IConfiguration configuration, IMapper mapper , RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            secretkey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDTO.Username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user == null || isValid == false)
            { 
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            // if the user was found generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name , user.UserName.ToString()),
                    new Claim(ClaimTypes.Role , roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                //Role = roles.FirstOrDefault()
            };

            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new ()
            {
                UserName = registerationRequestDTO.Username,
                //Password = registerationRequestDTO.Password,
                Name = registerationRequestDTO.Name,
                //Role = registerationRequestDTO.Role,
                Email = registerationRequestDTO.Username,
                NormalizedEmail = registerationRequestDTO.Username.ToUpper(),

            };

            try
            {
                var result = await _userManager.CreateAsync(user , registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user , "admin"); //registerationRequestDTO.Role
                    var userToReutrn = _context.ApplicationUsers.FirstOrDefault(u=> u.UserName == registerationRequestDTO.Username);
                    return _mapper.Map<UserDTO>(userToReutrn);
                }
            }
            catch (Exception ex) 
            {

            }
            // _context.LocalUsers.Add(user); //why not Async
            // await _context.SaveChangesAsync();
            return new UserDTO();
        }
    }
}
