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
        public string Scorecard { get; set; }
        public DateTime Date { get; set; }
        public ScoreDB() { }
        public ScoreDB(DateTime date, string courseName, string scorecard)
        {
            Date = date;
            CourseName = courseName;
            Scorecard = scorecard;
        }
    }
}
