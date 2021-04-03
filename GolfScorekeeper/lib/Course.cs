namespace GolfScorekeeper.lib
{
    public class Course
    {
        private string Name;
        private int[] Scorecard;
        public Course(string name, int[] scorecard)
        {
            Name = name;
            Scorecard = scorecard;
        }
        public string GetCourseName() { return Name; }
        public int[] GetScorecard() { return Scorecard; }
        public int GetLength() { return Scorecard.Length; }
        public int GetHolePar(int holeNumber) { return Scorecard[holeNumber - 1]; } //1 is hole 1
        public int GetCoursePar()
        {
            int par = 0;
            for (int i = 0; i < Scorecard.Length; i++)
            {
                par += Scorecard[i];
            }
            return par;
        }
        //What score (+/-) do you have on this course after X holes?
        public int CalculateCurrentScore(int holeNumber, int currentScore) //1 is hole 1
        {
            int totalPar = 0;
            for (int i = 1; i <= holeNumber; i++)
            {
                totalPar += GetHolePar(i);
            }

            return currentScore - totalPar;
        }
    }
}
