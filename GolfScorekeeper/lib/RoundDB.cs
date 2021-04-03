using SQLite;

namespace GolfScorekeeper.lib
{
    public class RoundDB
    {
        [PrimaryKey]
        private int Id { get; set; }
        private string CourseName { get; set; }
        private string Scorecard { get; set; }
        private int CurrentHole { get; set; }
        private int FurthestHole { get; set; }
        private int Strokes { get; set; }
        public RoundDB(){} //Empty constructor is needed for SQLite
        public RoundDB(Round round)
        {
            Id = 1;
            CourseName = round.GetCourseName();

            int[] inputScorecard = round.GetScorecard();
            string scorecard = "";
            
            for (int i=0; i < inputScorecard.Length; i++)
            {
                scorecard += inputScorecard[i].ToString();
            }

            Scorecard = scorecard;
            CurrentHole = round.GetCurrentHole();
            FurthestHole = round.GetFurthestHole();
            Strokes = round.GetStrokes();
        }
        public string GetCourseName() { return CourseName; }
        public string GetScorecard() { return Scorecard; }
        public int GetScore(int hole) { return Scorecard[hole - 1]; } //1 is hole 1
        public int GetCurrentHole() { return CurrentHole; }
        public int GetFurthestHole() { return FurthestHole; }
        public int GetStrokes() { return Strokes; }
    }
}
    