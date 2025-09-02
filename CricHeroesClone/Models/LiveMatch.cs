namespace CricHeroesClone.Models
{
    public class LiveMatch
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public string TeamAName { get; set; } = string.Empty;
        public string TeamBName { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string Status { get; set; } = "Live"; // Live, Completed, Cancelled
        
        // Current innings
        public int CurrentInnings { get; set; } = 1; // 1 or 2
        public int BattingTeamId { get; set; }
        public string BattingTeamName { get; set; } = string.Empty;
        public int BowlingTeamId { get; set; }
        public string BowlingTeamName { get; set; } = string.Empty;
        
        // Current score
        public int CurrentRuns { get; set; }
        public int CurrentWickets { get; set; }
        public float CurrentOvers { get; set; }
        public int TargetRuns { get; set; }
        
        // Match result
        public string Result { get; set; } = string.Empty;
        public int? WinningTeamId { get; set; }
        public string WinningTeamName { get; set; } = string.Empty;
        
        // Toss information
        public int? TossWinnerTeamId { get; set; }
        public string TossDecision { get; set; } = string.Empty; // Bat or Bowl
        
        // Last updated
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
