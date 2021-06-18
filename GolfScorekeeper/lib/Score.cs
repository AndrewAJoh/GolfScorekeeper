using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfScorekeeper.lib
{
    public class Score
    {
        private DateTime Date { get; set; }
        private string CourseName { get; set; }
        private int[] Scorecard { get; set; }
        public Score(DateTime date, string courseName, int[] scorecard)
        {
            Date = date;
            CourseName = courseName;
            Scorecard = scorecard;
        }
        public Score(ScoreDB scoreDB)
        {
            Date = scoreDB.Date;

            if (scoreDB.Size == 9)
            {
                Scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                Scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            Scorecard[0] = scoreDB.ScoreOne;
            Scorecard[1] = scoreDB.ScoreTwo;
            Scorecard[2] = scoreDB.ScoreThree;
            Scorecard[3] = scoreDB.ScoreFour;
            Scorecard[4] = scoreDB.ScoreFive;
            Scorecard[5] = scoreDB.ScoreSix;
            Scorecard[6] = scoreDB.ScoreSeven;
            Scorecard[7] = scoreDB.ScoreEight;
            Scorecard[8] = scoreDB.ScoreNine;

            if (scoreDB.Size == 18)
            {
                Scorecard[9]  = scoreDB.ScoreTen;
                Scorecard[10] = scoreDB.ScoreEleven;
                Scorecard[11] = scoreDB.ScoreTwelve;
                Scorecard[12] = scoreDB.ScoreThirteen;
                Scorecard[13] = scoreDB.ScoreFourteen;
                Scorecard[14] = scoreDB.ScoreFifteen;
                Scorecard[15] = scoreDB.ScoreSixteen;
                Scorecard[16] = scoreDB.ScoreSeventeen;
                Scorecard[17] = scoreDB.ScoreEighteen;

            }

            CourseName = scoreDB.CourseName;
        }
        public DateTime GetDate() { return Date; }
        public string GetCourseName() { return CourseName; }
        public int[] GetScorecard() { return Scorecard; }
    }
}
