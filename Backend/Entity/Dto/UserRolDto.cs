using Entity.Dto;

namespace Entity.Model

{
    public class UserRolDto : BaseDto
    {
        public int UserId { set; get; }
        public int RolId { set; get; }
    }
}