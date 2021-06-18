using SQLite;
using System;

namespace GolfScorekeeper.lib
{
    public class ScoreDB
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }
        public string CourseName { get; set; }
        public int Size { get; set; }
        public int ScoreOne { get; set; }
        public int ScoreTwo { get; set; }
        public int ScoreThree { get; set; }
        public int ScoreFour { get; set; }
        public int ScoreFive { get; set; }
        public int ScoreSix { get; set; }
        public int ScoreSeven { get; set; }
        public int ScoreEight { get; set; }
        public int ScoreNine { get; set; }
        public int ScoreTen { get; set; }
        public int ScoreEleven { get; set; }
        public int ScoreTwelve { get; set; }
        public int ScoreThirteen { get; set; }
        public int ScoreFourteen { get; set; }
        public int ScoreFifteen { get; set; }
        public int ScoreSixteen { get; set; }
        public int ScoreSeventeen { get; set; }
        public int ScoreEighteen { get; set; }
        public DateTime Date { get; set; }
        public ScoreDB() { }
        public ScoreDB(Score score)
        {
            Date = score.GetDate();

            int[] scorecard = score.GetScorecard();

            ScoreOne   = scorecard[0];
            ScoreTwo   = scorecard[1];
            ScoreThree = scorecard[2];
            ScoreFour  = scorecard[3];
            ScoreFive  = scorecard[4];
            ScoreSix   = scorecard[5];
            ScoreSeven = scorecard[6];
            ScoreEight = scorecard[7];
            ScoreNine  = scorecard[8];
            
            if (scorecard.Length == 18)
            {
                Size = 18;
                ScoreTen       = scorecard[9];
                ScoreEleven    = scorecard[10];
                ScoreTwelve    = scorecard[11];
                ScoreThirteen  = scorecard[12];
                ScoreFourteen  = scorecard[13];
                ScoreFifteen   = scorecard[14];
                ScoreSixteen   = scorecard[15];
                ScoreSeventeen = scorecard[16];
                ScoreEighteen  = scorecard[17];
            }
            else
            {
                Size = 9;
                ScoreTen       = 0;
                ScoreEleven    = 0;
                ScoreTwelve    = 0;
                ScoreThirteen  = 0;
                ScoreFourteen  = 0;
                ScoreFifteen   = 0;
                ScoreSixteen   = 0;
                ScoreSeventeen = 0;
                ScoreEighteen  = 0;
            }

            CourseName = score.GetCourseName();
        }
    }
}
