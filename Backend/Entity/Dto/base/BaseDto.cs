namespace Entity.Dto
{
    public class BaseDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
