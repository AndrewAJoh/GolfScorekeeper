using System;
using System.Linq;

namespace GolfScorekeeper.lib
{
    public class Round
    {
        private GolfCourse Course;
        private int[] Scorecard;
        private int CurrentHole;    //Current hole you are on (1 is hole 1)
        private int Strokes;
        public Round(GolfCourse course)
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
            Strokes = 0;
        }

        public Round(GolfCourse course, int[] scorecard, int currentHole, int strokes)
        {
            Course = course;
            Scorecard = scorecard;
            CurrentHole = currentHole;
            Strokes = strokes;
        }

        public string GetCourseName() { return Course.GetCourseName(); }    //TODO: Either keep this in or get rid of it so it's consistent (return just the course or add all getter methods)
        public int[] GetScorecard() { return Scorecard; }
        public int GetScore(int hole) { return Scorecard[hole - 1]; } //1 is hole 1
        public int GetCurrentHole() { return CurrentHole; }
        public int GetCurrentCourseScore() { return Scorecard.Sum(); }
        public GolfCourse GetCourse() { return Course; }
        public int GetCurrentCourseScoreRelativeToPar() { return Course.CalculateCurrentScoreRelativeToPar(Scorecard); }
        public int GetStrokes() { return Strokes; }
        public void SetScorecard(int[] inputScorecard) { Scorecard = inputScorecard; }
        public void SetScore(int hole, int score) { Scorecard[hole-1] = score; }    //1 is hole 1
        public void SetCurrentHole(int inputCurrentHole) { CurrentHole = inputCurrentHole; }
        public void SetStrokes(int inputStrokes) { Strokes = inputStrokes; }
        public bool CheckForZeros()
        {
            bool containsZero = false;

            for (int i = 0; i < Course.GetLength(); i++)
            {
                if (Scorecard[i] == 0)
                {
                    containsZero = true;
                }
            }

            return containsZero;
        }
    }
}
