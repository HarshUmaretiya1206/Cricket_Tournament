namespace CricHeroesClone.Models
{
    public class MatchScoreDto
    {
        public int MatchId { get; set; }
        public string TeamAName { get; set; } = string.Empty;
        public string TeamBName { get; set; } = string.Empty;
        public string ScoreA { get; set; } = "0/0 in 0 overs";
        public string ScoreB { get; set; } = "0/0 in 0 overs";
    }
}
