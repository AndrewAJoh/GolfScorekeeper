using SQLite;

namespace GolfScorekeeper.lib
{
    class CourseDB
    {
        [PrimaryKey]
        private string Name { get; set; }
        private string Scorecard { get; set; }
        public CourseDB() { }
        public CourseDB(Course course)
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
        public int GetHolePar(int holeNumber) { return Scorecard[holeNumber - 1]; } //1 is hole 1
    }
}
