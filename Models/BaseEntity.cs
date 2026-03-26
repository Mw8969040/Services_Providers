namespace Smart_Platform.Models
{
    public abstract class BaseEntity
    {
        public int Id {  get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
