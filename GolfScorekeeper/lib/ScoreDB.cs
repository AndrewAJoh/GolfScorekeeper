using SQLite;
using System;

namespace GolfScorekeeper.lib
{
    public class ScoreDB
    {
        public string CourseName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public ScoreDB() { }
        public ScoreDB(DateTime date, string courseName, int score)
        {
            Date = date;
            CourseName = courseName;
            Score = score;
        }
    }
}
