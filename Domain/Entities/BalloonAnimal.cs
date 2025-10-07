namespace TodoApp.Domain.Entities
{
    public class BalloonAnimal
    {
        public int Id { get; set; }
        public string AnimalType { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}