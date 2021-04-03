using SQLite;

namespace GolfScorekeeper
{
    public class Course
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string ParList { get; set; }
    }
}