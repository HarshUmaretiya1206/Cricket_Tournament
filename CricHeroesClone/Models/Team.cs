namespace CricHeroesClone.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Foreign key
        public int TournamentId { get; set; }

        // Navigation / Display
        public string TournamentName { get; set; } = string.Empty;
    }
}
