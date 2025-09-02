namespace CricHeroesClone.Models
{
    public class BallByBall
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int Innings { get; set; }
        public int OverNumber { get; set; }
        public int BallNumber { get; set; }
        
        // Ball details
        public int Runs { get; set; }
        public bool IsWicket { get; set; }
        public bool IsWide { get; set; }
        public bool IsNoBall { get; set; }
        public bool IsBye { get; set; }
        public bool IsLegBye { get; set; }
        
        // Player details
        public int? BatsmanId { get; set; }
        public string BatsmanName { get; set; } = string.Empty;
        public int? BowlerId { get; set; }
        public string BowlerName { get; set; } = string.Empty;
        public int? WicketTakerId { get; set; }
        public string WicketTakerName { get; set; } = string.Empty;
        
        // Wicket details
        public string? WicketType { get; set; } // Bowled, Caught, LBW, Run Out, etc.
        public string? WicketDescription { get; set; }
        
        // Extras
        public int ExtraRuns { get; set; }
        public string ExtraType { get; set; } = string.Empty; // Wide, No Ball, Bye, Leg Bye
        
        // Commentary
        public string Commentary { get; set; } = string.Empty;
        
        // Timestamp
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        // Calculated properties
        public string BallSummary => $"{OverNumber}.{BallNumber} - {Runs} runs";
        public bool IsExtra => IsWide || IsNoBall || IsBye || IsLegBye;
    }
}
