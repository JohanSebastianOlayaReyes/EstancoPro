using Entity.Dto;

namespace Entity.Model
{
    public class UserDto : BaseDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public int PersonId { get; set; }
    }
}
