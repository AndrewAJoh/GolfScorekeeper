namespace GolfScorekeeper.lib
{
    public class GolfCourse
    {
        private string Name;
        private int[] Scorecard;
        public GolfCourse(string name, int[] scorecard)
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
        //Plus or minus score for all non-zero holes so far
        public int CalculateCurrentScoreRelativeToPar(int[] scorecard) //1 is hole 1
        {
            int score = 0;

            for (int i = 1; i <= Scorecard.Length; i++)
            {
                if (scorecard[i - 1] != 0)  //Player has not entered a score yet for this hole
                {
                    score += (scorecard[i - 1] - GetHolePar(i));
                }
            }

            return score;
        }
    }
}
