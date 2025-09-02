namespace CricHeroesClone.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? BattingStyle { get; set; }
        public string? BowlingStyle { get; set; }

        // Foreign key
        public int TeamId { get; set; }

        // Navigation / Display
        public string TeamName { get; set; } = string.Empty;
    }
}
