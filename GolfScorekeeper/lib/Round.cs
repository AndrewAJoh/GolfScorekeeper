using System;
using System.Linq;

namespace GolfScorekeeper.lib
{
    public class Round
    {
        private Course Course;
        private int[] Scorecard;
        private int CurrentHole;    //Current hole you have navigated to [range 1->[furthesthole + 1]]
        private int FurthestHole;   //Furthest hole you have completed
        private int Strokes;
        public Round(Course course)
        {
            Course = course;
            if (Course.GetLength() == 9)
            {
                Scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                Scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            CurrentHole = 1;
            FurthestHole = 0;
            Strokes = 0;
        }

        public Round(Course course, int[] scorecard, int currentHole, int furthestHole, int strokes)
        {
            Course = course;
            Scorecard = scorecard;
            CurrentHole = currentHole;
            FurthestHole = furthestHole;
            Strokes = strokes;
        }

        public string GetCourseName() { return Course.GetCourseName(); }    //TODO: Either keep this in or get rid of it so it's consistent (return just the course or add all getter methods)
        public int[] GetScorecard() { return Scorecard; }
        public int GetScore(int hole) { return Scorecard[hole - 1]; } //1 is hole 1
        public int GetCurrentHole() { return CurrentHole; }
        public int GetFurthestHole() { return FurthestHole; }
        public int GetCurrentCourseScore() { return Scorecard.Sum(); }
        public Course GetCourse() { return Course; }
        public int GetCurrentCourseScoreRelativeToPar()
        {
            if (FurthestHole == 0)
            {
                return 0;
            }
            else
            {
                return Course.CalculateCurrentScore(FurthestHole, Scorecard.Sum());
            }
        }
        public int GetStrokes() { return Strokes; }
        public void SetScorecard(int[] inputScorecard) { Scorecard = inputScorecard; }
        public void SetScore(int hole, int score) { Scorecard[hole-1] = score; }    //1 is hole 1
        public void SetCurrentHole(int inputCurrentHole) { CurrentHole = inputCurrentHole; }
        public void SetFurthestHole(int inputFurthestHole) { FurthestHole = inputFurthestHole; }
        public void SetStrokes(int inputStrokes) { Strokes = inputStrokes; }
    }
}
