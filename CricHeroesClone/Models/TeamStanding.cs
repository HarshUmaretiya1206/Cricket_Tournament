namespace CricHeroesClone.Models
{
    public class TeamStanding
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesTied { get; set; }
        public int Points { get; set; }
        public float NetRunRate { get; set; }
        
        // Navigation properties
        public string TeamName { get; set; } = string.Empty;
        public string TournamentName { get; set; } = string.Empty;
        public string TeamLogo { get; set; } = string.Empty;
    }
}
