namespace MagicVilla_VillaApi.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }

        //public string Role { get; set; } will included in the token 
    }
}
