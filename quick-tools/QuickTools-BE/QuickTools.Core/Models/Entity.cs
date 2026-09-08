namespace QuickTools.Core.Models
{
    public class Entity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}