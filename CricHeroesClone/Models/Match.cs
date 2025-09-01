namespace CricHeroesClone.Models
{
    public class Match
    {
        public int Id { get; set; }

        // Foreign key
        public int TournamentId { get; set; }

        public int TeamAId { get; set; }
        public int TeamBId { get; set; }

        public DateTime? MatchDate { get; set; }
        public string? Venue { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, Live, Completed, Cancelled
        public int? TossWinnerTeamId { get; set; }
        public string? Result { get; set; }

        // Navigation / Display only
        public string TournamentName { get; set; } = string.Empty;
        public string TeamA { get; set; } = string.Empty;
        public string TeamB { get; set; } = string.Empty;

        public string TeamAName { get; set; } = string.Empty;
        public string TeamBName { get; set; } = string.Empty;
        public int ScoreTeamA { get; set; }
        public int ScoreTeamB { get; set; }
    }
}
