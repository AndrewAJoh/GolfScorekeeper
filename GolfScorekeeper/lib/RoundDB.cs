using SQLite;
using System;

namespace GolfScorekeeper.lib
{
    public class RoundDB
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Scorecard { get; set; }
        public int CurrentHole { get; set; }
        public int FurthestHole { get; set; }
        public int Strokes { get; set; }
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
            FurthestHole = 0;
            Strokes = round.GetStrokes();
        }
        public string GetCourseName() { return CourseName; }
        public string GetScorecard() { return Scorecard; }
        public int GetScore(int hole) { return Convert.ToInt32(Scorecard[hole - 1].ToString()); } //1 is hole 1
        public int GetCurrentHole() { return CurrentHole; }
        public int GetStrokes() { return Strokes; }
    }
}
    