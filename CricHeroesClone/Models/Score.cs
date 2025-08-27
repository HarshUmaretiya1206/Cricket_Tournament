namespace CricHeroesClone.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }

        public int Runs { get; set; }
        public int Wickets { get; set; }
        public float Overs { get; set; }

        // Navigation / Display
        public string TeamName { get; set; } = string.Empty;
    }
}
