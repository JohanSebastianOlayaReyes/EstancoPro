using Entity.Dto;

namespace Entity.Model
{
    public class RolFormPermissionDto : BaseDto
    {
        public int RolId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }
    }
}
