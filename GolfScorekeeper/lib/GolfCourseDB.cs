using SQLite;
using System;

namespace GolfScorekeeper.lib
{
    public class GolfCourseDB
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Scorecard { get; set; }
        public GolfCourseDB() { }
        public GolfCourseDB(GolfCourse course)
        {
            Name = course.GetCourseName();

            int[] inputScorecard = course.GetScorecard();
            string scorecard = "";

            for (int i = 0; i < inputScorecard.Length; i++)
            {
                scorecard += inputScorecard[i].ToString();
            }

            Scorecard = scorecard;
        }
        public string GetCourseName() { return Name; }
        public string GetScorecard() { return Scorecard; }
        public int GetLength() { return Scorecard.Length; }
        public int GetHolePar(int holeNumber) { return Convert.ToInt32(Scorecard[holeNumber - 1].ToString()); } //1 is hole 1
    }
}
