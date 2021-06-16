using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using System.Text.RegularExpressions;
using SQLite;
using SQLitePCL;
using System.IO;
using GolfScorekeeper.lib;

namespace GolfScorekeeper
{
    public class App : Application
    {
        private SQLiteConnection dbConnection;
        private Button scoreTrackerButton;
        private Button moreButton;
        private Button greenButton;
        private Button teeBoxButton;
        private Button sandAstheticButton1;
        private Button sandAstheticButton2;
        private Button sandAstheticButton3;
        private Button waterAstheticButton1;
        private Button waterAstheticButton2;
        private Label hole;
        private Label redTeeBoxMarker1;
        private Label redTeeBoxMarker2;
        private Label whiteTeeBoxMarker1;
        private Label whiteTeeBoxMarker2;
        private Label areYouSureLabel;
        private Label areYouReallySureLabel;
        private Label customCoursePrompt;
        private Entry customCourseEntry;
        private Button customCourseNextButton;
        private Button customNineButton;
        private Button customEighteenButton;
        private CirclePage mp;
        private CirclePage sp;
        private CirclePage ssp;
        private CirclePage cffp;
        private CirclePage hp;
        private CirclePage chp;
        private CirclePage fp;
        private CirclePage qp;
        private CirclePage qqp;
        private CirclePage ep;
        private CirclePage mop;
        private CirclePage clp;
        private CirclePage cdp;
        private CirclePage scp;
        private CirclePage dp;
        private AbsoluteLayout homePageLayout;
        private StackLayout morePageStackLayout;
        private AbsoluteLayout enterPageLayout;
        private AbsoluteLayout courseDetailLayout;
        private CircleScrollView morePage;
        private CircleScrollView historyScrollView;
        private AbsoluteLayout cFFinalPageLayout;
        AbsoluteLayout questionPageLayout;
        private CircleScrollView morePageScrollView;
        private CircleScrollView finalScreenLayout;
        private Button roundInfoButton;
        private Button overallButton;
        private Button strokeButton;
        private Button enhanceButton;
        private Button nextHoleButton;
        private string courseNameText;
        private List<GolfCourse> courseList;
        private Round currentRound;
        private string newCourseScorecard; //Needed for course creation
        private int newCourseLength; //Needed for course creation
        private string newCourseName;
        private bool midRound = false;
        private bool isEnhanced = false;
        private string historyCourse; //The course you have navigated to in the game history
        GolfCourse courseToBeAdded;

        private readonly Color greenColor = Color.FromRgb(10, 80, 22);
        private readonly Color puttingGreenColor = Color.FromRgb(84, 161, 88);
        private readonly Color grayColor = Color.FromRgb(70, 70, 70);
        private readonly Color darkGreenColor = Color.FromRgb(0, 35, 0);
        private readonly Color sandColor = Color.FromRgb(218, 189, 129);
        private readonly Color waterColor = Color.FromRgb(0, 64, 98);
        public App()
        {
            courseList = new List<GolfCourse>();

            //Get info from database
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider(true);

            string dataPath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;
            string courseDatabaseFileName = "courses2.db3";
            string courseDatabasePath = Path.Combine(dataPath, courseDatabaseFileName);

            dbConnection = new SQLiteConnection(courseDatabasePath);

            dbConnection.CreateTable<RoundDB>();
            dbConnection.CreateTable<Course>();
            dbConnection.CreateTable<GolfCourseDB>();
            dbConnection.DropTable<ScoreDB>();
            dbConnection.CreateTable<ScoreDB>();

            //Import old Course data
            var courseDBReturn = dbConnection.Query<Course>("select * from Course");
            List<int> courseScorecard = new List<int>();

            foreach (var courseDB in courseDBReturn)
            {
                courseScorecard.Clear();
                for (int i = 0; i < courseDB.ParList.Length; i++)
                {
                    courseScorecard.Add(Convert.ToInt32(courseDB.ParList[i].ToString()));
                }

                int[] courseScorecardIntArray = courseScorecard.ToArray();
                GolfCourseDB c = new GolfCourseDB();
                c.Scorecard = courseDB.ParList;
                c.Name = courseDB.Name;
                dbConnection.InsertOrReplace(c);
                dbConnection.DropTable<Course>();
            }
            //Import list of courses from the database
            var courseDBList = dbConnection.Table<GolfCourseDB>();
            
            foreach (var courseDB in courseDBList)
            {
                courseScorecard.Clear();
                for (int i = 0; i < courseDB.GetLength(); i++)
                {
                    courseScorecard.Add(courseDB.GetHolePar(i+1));
                }

                int[] courseScorecardIntArray = courseScorecard.ToArray();
                courseList.Add(new GolfCourse(courseDB.GetCourseName(), courseScorecardIntArray));
            }

            var roundDBList = dbConnection.Table<RoundDB>();

            var currentGame = 0;
            foreach (var item in roundDBList)
            {
                currentGame++;
            }

            if (currentGame == 1)
            {
                foreach (var roundDB in roundDBList)        //This is fine to keep for now as we will eventually be pulling previous games in from the DB
                {
                    midRound = true;
                    GolfCourse currentCourse = courseList.First(c => c.GetCourseName().Equals(roundDB.GetCourseName()));
                    //Importing roundDB record into currentRound, must convert scorecard first
                    int[] scorecard;
                    if (currentCourse.GetLength() == 18)
                    {
                        scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    else
                    {
                        scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    for (int i = 0; i < currentCourse.GetLength(); i++)
                    {
                        scorecard[i] = roundDB.GetScore(i+1);
                    }
                    currentRound = new Round(currentCourse, scorecard, roundDB.GetCurrentHole(), roundDB.GetStrokes());
                }
            }
            //Home page objects
            scoreTrackerButton = new Button() { Text = "Score Tracker", BackgroundColor = greenColor };
            moreButton = new Button() { Text = "More", BackgroundColor = greenColor };
            greenButton = new Button() { BackgroundColor = puttingGreenColor };
            teeBoxButton = new Button() { BackgroundColor = greenColor };
            sandAstheticButton1 = new Button() { Text = "", BackgroundColor = sandColor };
            sandAstheticButton2 = new Button() { Text = "", BackgroundColor = sandColor };
            sandAstheticButton3 = new Button() { Text = "", BackgroundColor = sandColor };
            waterAstheticButton1 = new Button() { Text = "", BackgroundColor = waterColor };
            waterAstheticButton2 = new Button() { Text = "", BackgroundColor = waterColor };
            hole = new Label() { Text = ".", TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Center };
            redTeeBoxMarker1 = new Label() { Text = ".", TextColor = Color.Red };
            redTeeBoxMarker2 = new Label() { Text = ".", TextColor = Color.Red };
            whiteTeeBoxMarker1 = new Label() { Text = ".", TextColor = Color.White };
            whiteTeeBoxMarker2 = new Label() { Text = ".", TextColor = Color.White };

            roundInfoButton = new Button() { Text = "", BackgroundColor = grayColor };
            overallButton = new Button() { Text = "", BackgroundColor = grayColor };
            Button addStrokeButton = new Button() { Text = "+1", FontSize = 20, BackgroundColor = greenColor };
            strokeButton = new Button() { Text = "0", BackgroundColor = greenColor };
            Button subtractStrokeButton = new Button() { Text = "-1", BackgroundColor = greenColor };
            nextHoleButton = new Button() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            Button previousHoleButton = new Button() { Text = "Prev\nHole", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            
            Label messageLabel = new Label() { Text = "All current round\ndata will be lost.\nAre you sure?" };
            Button yesConfirmButton = new Button() { Text = "Yes"};
            Button noConfirmButton = new Button() { Text = "No"};

            areYouSureLabel = new Label() { };
            areYouReallySureLabel = new Label() { FontSize = 8 };
            Button yesButton = new Button() { Text = "Yes"};
            Button noButton = new Button() { Text = "No"};

            Label finishRoundText = new Label(){ Text = "Finish Round?" };
            Button finishRoundYesButton = new Button() { Text = "Yes" };
            finishRoundYesButton.Clicked += FinishRound;
            Button finishRoundNoButton = new Button() { Text = "No" };
            finishRoundNoButton.Clicked += OnNoFinishRoundButtonClicked;

            AbsoluteLayout.SetLayoutBounds(finishRoundText, new Rectangle(0.5, .25, 250, 80));
            AbsoluteLayout.SetLayoutFlags(finishRoundText, AbsoluteLayoutFlags.PositionProportional);


            addStrokeButton.Clicked += OnAddStrokeButtonClicked;
            strokeButton.Clicked += OnStrokeButtonClicked;
            subtractStrokeButton.Clicked += OnSubtractStrokeButtonClicked;
            nextHoleButton.Clicked += OnNextHoleButtonClicked;
            previousHoleButton.Clicked += OnPreviousHoleButtonClicked;
            waterAstheticButton1.Clicked += OnWaterAstheticButtonClicked;
            waterAstheticButton2.Clicked += OnWaterAstheticButtonClicked;
            roundInfoButton.Clicked += OnRoundInfoButtonClicked;
            
            AbsoluteLayout deleteCourseLayout = new AbsoluteLayout
            {
                Children =
                {
                    areYouSureLabel,
                    yesButton,
                    noButton,
                    areYouReallySureLabel
                }
            };

            AbsoluteLayout.SetLayoutBounds(areYouSureLabel, new Rectangle(0.5, .25, 250, 80));
            AbsoluteLayout.SetLayoutFlags(areYouSureLabel, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(areYouReallySureLabel, new Rectangle(0.5, .50, 250, 80));
            AbsoluteLayout.SetLayoutFlags(areYouReallySureLabel, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(yesButton, new Rectangle(0.2, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(yesButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(noButton, new Rectangle(0.8, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(noButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(finishRoundYesButton, new Rectangle(0.2, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(finishRoundYesButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(finishRoundNoButton, new Rectangle(0.8, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(finishRoundNoButton, AbsoluteLayoutFlags.PositionProportional);

            yesButton.Clicked += OnYesDeleteButtonClicked;
            noButton.Clicked += OnNoDeleteButtonClicked;
            
            yesConfirmButton.Clicked += OnYesConfirmButtonClicked;
            noConfirmButton.Clicked += OnNoConfirmButtonClicked;

            questionPageLayout = new AbsoluteLayout { };

            AbsoluteLayout questionConfirmCircleStackLayout = new AbsoluteLayout
            {
                Children =
                {
                    messageLabel,
                    yesConfirmButton,
                    noConfirmButton
                }
            };

            

            AbsoluteLayout.SetLayoutBounds(messageLabel, new Rectangle(0.5, .35, 225, 200));
            AbsoluteLayout.SetLayoutFlags(messageLabel, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(yesConfirmButton, new Rectangle(0.2, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(yesConfirmButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(noConfirmButton, new Rectangle(0.8, .7, 100, 60));
            AbsoluteLayout.SetLayoutFlags(noConfirmButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(roundInfoButton, new Rectangle(0.5, 0, 155, 50));
            AbsoluteLayout.SetLayoutFlags(roundInfoButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(overallButton, new Rectangle(0.5, 0.17, 155, 50));
            AbsoluteLayout.SetLayoutFlags(overallButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(addStrokeButton, new Rectangle(0.5, 0.75, 155, 120));
            AbsoluteLayout.SetLayoutFlags(addStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(strokeButton, new Rectangle(0.5, 0.367, 80, 60));
            AbsoluteLayout.SetLayoutFlags(strokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(subtractStrokeButton, new Rectangle(0.5, 1, 155, 58));
            AbsoluteLayout.SetLayoutFlags(subtractStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(nextHoleButton, new Rectangle(1, 0, 110, 360));
            AbsoluteLayout.SetLayoutFlags(nextHoleButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(previousHoleButton, new Rectangle(0, 0, 110, 360));
            AbsoluteLayout.SetLayoutFlags(previousHoleButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(scoreTrackerButton, new Rectangle(0.1, 0.35, 200, 80));
            AbsoluteLayout.SetLayoutFlags(scoreTrackerButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(moreButton, new Rectangle(0.1, 0.7, 200, 80));
            AbsoluteLayout.SetLayoutFlags(moreButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(greenButton, new Rectangle(0.88, 0.35, 75, 75));
            AbsoluteLayout.SetLayoutFlags(greenButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(teeBoxButton, new Rectangle(0.81, 0.69, 60, 50));
            AbsoluteLayout.SetLayoutFlags(teeBoxButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton1, new Rectangle(0.65, 0.5, 25, 25));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton2, new Rectangle(0.1, 0.92, 60, 40));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton3, new Rectangle(0.65, 0.28, 25, 25));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton3, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton1, new Rectangle(0.3, 0, 250, 80));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton2, new Rectangle(0.8, 1, 200, 70));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(hole, new Rectangle(0.85, .4, 5, 30));
            AbsoluteLayout.SetLayoutFlags(hole, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(redTeeBoxMarker1, new Rectangle(0.78, .62, 30, 30));
            AbsoluteLayout.SetLayoutFlags(redTeeBoxMarker1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(redTeeBoxMarker2, new Rectangle(0.78, .67, 30, 30));
            AbsoluteLayout.SetLayoutFlags(redTeeBoxMarker2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(whiteTeeBoxMarker1, new Rectangle(0.86, .62, 30, 30));
            AbsoluteLayout.SetLayoutFlags(whiteTeeBoxMarker1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(whiteTeeBoxMarker2, new Rectangle(0.86, .67, 30, 30));
            AbsoluteLayout.SetLayoutFlags(whiteTeeBoxMarker2, AbsoluteLayoutFlags.PositionProportional);

            homePageLayout = new AbsoluteLayout
            {
                Children =
                {
                    scoreTrackerButton,
                    moreButton,
                    greenButton,
                    teeBoxButton,
                    sandAstheticButton1,
                    sandAstheticButton2,
                    sandAstheticButton3,
                    waterAstheticButton1,
                    waterAstheticButton2,
                    hole,
                    redTeeBoxMarker1,
                    redTeeBoxMarker2,
                    whiteTeeBoxMarker1,
                    whiteTeeBoxMarker2
                }
            };

            CircleScrollView questionLayout = new CircleScrollView
            {
                Content = questionPageLayout
            };

            CircleScrollView questionConfirmLayout = new CircleScrollView
            {
                Content = questionConfirmCircleStackLayout
            };

            AbsoluteLayout parTrackerLayout = new AbsoluteLayout()
            {
                Children =
                {
                    roundInfoButton,
                    overallButton,
                    addStrokeButton,
                    strokeButton,
                    subtractStrokeButton,
                    nextHoleButton,
                    previousHoleButton
                }
            };

            //MainPage
            mp = new CirclePage() {
                Content = homePageLayout,
                BackgroundColor = darkGreenColor
            };

            //SubPage
            sp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //SubSubPage
            ssp = new CirclePage()
            {
                Content = parTrackerLayout,
                BackgroundColor = darkGreenColor
            };

            //QuestionPage
            qp = new CirclePage()
            {
                Content = questionLayout,
                BackgroundColor = darkGreenColor
            };

            qqp = new CirclePage()
            {
                Content = questionConfirmLayout,
                BackgroundColor = darkGreenColor
            };

            //EnterPage
            ep = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            mop = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //CourseListPage
            clp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            hp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            chp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //CourseDetailPage
            cdp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //ScoreDetailPage
            scp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //DeletePage
            dp = new CirclePage()
            {
                Content = deleteCourseLayout,
                BackgroundColor = darkGreenColor
            };

            //Checkfor Final Page
            cFFinalPageLayout = new AbsoluteLayout
            {
                Children =
                {
                    finishRoundYesButton,
                    finishRoundNoButton,
                    finishRoundText
                }
            };

            //FinalPage (results screen)
            cffp = new CirclePage()
            {
                Content = cFFinalPageLayout,
                BackgroundColor = darkGreenColor
            };

            //FinalPage (results screen)
            fp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            NavigationPage np = new NavigationPage(mp);
            NavigationPage.SetHasNavigationBar(mp, false);
            NavigationPage.SetHasNavigationBar(sp, false);
            NavigationPage.SetHasNavigationBar(ssp, false);
            NavigationPage.SetHasNavigationBar(qp, false);
            NavigationPage.SetHasNavigationBar(qqp, false);
            NavigationPage.SetHasNavigationBar(ep, false);
            NavigationPage.SetHasNavigationBar(cffp, false);
            NavigationPage.SetHasNavigationBar(hp, false);
            NavigationPage.SetHasNavigationBar(chp, false);
            NavigationPage.SetHasNavigationBar(fp, false);
            NavigationPage.SetHasNavigationBar(mop, false);
            NavigationPage.SetHasNavigationBar(clp, false);
            NavigationPage.SetHasNavigationBar(cdp, false);
            NavigationPage.SetHasNavigationBar(scp, false);
            NavigationPage.SetHasNavigationBar(dp, false);

            MainPage = np;
            scoreTrackerButton.Clicked += DetermineNewOrResumeGame;
            moreButton.Clicked += OnMoreButtonClicked;
        }
        //Ask for course name. Happens before GoToParPrompt
        protected void GoToNamePrompt(object sender, System.EventArgs e)
        {
            //Reset 'new course' variables
            newCourseLength = Convert.ToInt32((sender as Button).Text);
            newCourseScorecard = "";
            newCourseName = "";

            customCoursePrompt.Text = "Enter course name:";

            customCourseEntry.Keyboard = Keyboard.Default;
            customCourseEntry.TextColor = Color.Black;
            customCourseEntry.BackgroundColor = Color.White;
            customCourseEntry.Opacity = 10;
            customCourseEntry.MaxLength = 25;

            enterPageLayout.Children.Remove(customNineButton);
            enterPageLayout.Children.Remove(customEighteenButton);
            enterPageLayout.Children.Add(customCourseNextButton);
            enterPageLayout.Children.Add(customCourseEntry);
        }
        //Ask for course pars. Rejects if course name is invalid.
        protected void GoToParPrompt(object sender, System.EventArgs e)
        {
            if (customCourseNextButton.Text == "Next" && (newCourseLength == 9 || (newCourseLength == 18 && newCourseName.Equals("")))) //TODO: Evaluate this logic, can it be simplified?
            {
                //Validate course name entered by player
                var courseNameRegex = new Regex(@"^[a-zA-Z\s]*$");

                if (!courseNameRegex.IsMatch(customCourseEntry.Text))
                {
                    Toast.DisplayText("Only letters and <br>spaces are allowed.");
                    return;
                }
                if (customCourseEntry.Text.Length == 0)
                {
                    Toast.DisplayText("Course name must be <br>at least one character long.");
                    return;
                }

                newCourseName = customCourseEntry.Text;

                customCourseEntry.Text = "";
                customCourseEntry.Keyboard = Keyboard.Numeric;
                customCourseEntry.MaxLength = 9;

                customCoursePrompt.Text = "Enter front 9 pars in order (Example: 443545344)";

                if (newCourseLength == 9)
                {
                    customCourseNextButton.Text = "Done";
                }
            }
            else if (customCourseNextButton.Text == "Next" && (newCourseLength == 18 && !newCourseName.Equals("")))
            {
                //Validate hole pars entered by player
                string pars = customCourseEntry.Text;
                if (pars.Length != 9)
                {
                    Toast.DisplayText("You must have 9 pars <br>in the entry. Follow the<br> example for formatting.");
                    return;
                }

                var parRegex = new Regex("^[1-9]*$");

                if (!parRegex.IsMatch(pars))
                {
                    Toast.DisplayText("0s and symbols <br>are not allowed.");
                    return;
                }

                newCourseScorecard += pars;

                customCourseNextButton.Text = "Done";
                customCoursePrompt.Text = "Enter back 9 pars in order";
                customCourseEntry.Text = "";
            }
            else if (customCourseNextButton.Text == "Done")
            {
                //Validate hole pars entered by player
                string pars = customCourseEntry.Text;
                if (pars.Length != 9)
                {
                    Toast.DisplayText("You must have 9 pars <br>in the entry. Follow the<br> example for formatting.");
                    return;
                }

                var parRegex = new Regex("^[1-9]*$");

                if (!parRegex.IsMatch(pars))
                {
                    Toast.DisplayText("0s and symbols <br>are not allowed.");
                    return;
                }

                newCourseScorecard += pars;

                //Add course to list of courses
                List<int> newCourseScorecardList = new List<int>();    //Todo: eliminate this variable
                for (int i = 0; i < newCourseLength; i++)
                {
                    newCourseScorecardList.Add(Convert.ToInt32(newCourseScorecard[i].ToString()));
                }

                int[] newCourseScorecardIntArray = newCourseScorecardList.ToArray();

                courseToBeAdded = new GolfCourse(newCourseName, newCourseScorecardIntArray);

                int resultDuplicate = CheckCourseDuplicate(courseToBeAdded);  //Change this to check if course to be overwritten is the one currently playing
                int resultCurrentCourse = CheckCurrentCourse(courseToBeAdded);
                if (resultDuplicate == 1 && resultCurrentCourse == 1)
                {
                    Button yes = new Button
                    {
                        Text = "Yes"
                    };
                    Button no = new Button
                    {
                        Text = "No"
                    };
                    Label prompt = new Label
                    {
                        Text = "Current round will be\nreset if course is overwritten.\nContinue?"
                    };

                    AbsoluteLayout.SetLayoutBounds(yes, new Rectangle(0.2, .7, 100, 60));
                    AbsoluteLayout.SetLayoutFlags(yes, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(no, new Rectangle(0.8, .7, 100, 60));
                    AbsoluteLayout.SetLayoutFlags(no, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(prompt, new Rectangle(0.5, .25, 250, 80));
                    AbsoluteLayout.SetLayoutFlags(prompt, AbsoluteLayoutFlags.PositionProportional);

                    yes.Clicked += OnYesOverwriteCourseClicked;
                    no.Clicked += OnNoOverwriteCourseClicked;

                    questionPageLayout.Children.Clear();
                    questionPageLayout.Children.Add(yes);
                    questionPageLayout.Children.Add(no);
                    questionPageLayout.Children.Add(prompt);

                    MainPage.Navigation.PushAsync(qp);
                    
                }
                else
                {
                    courseList.Remove(courseList.FirstOrDefault(c => c.GetCourseName().Equals(courseToBeAdded.GetCourseName())));
                    courseList.Add(courseToBeAdded);
                    GolfCourseDB golfCourse = new GolfCourseDB(courseToBeAdded);
                    dbConnection.InsertOrReplace(golfCourse);
                }

                GenerateCourseList(false, true, false);
                GenerateCourseList(true, false, false);

                MainPage.Navigation.RemovePage(ep);
            }
        }

        protected void DetermineNewOrResumeGame(object sender, System.EventArgs e)
        {
            if (midRound)
            {
                //Ask player whether they want to resume or start new

                Button resumeGameQuestionButton = new Button() { Text = "Resume Game", BackgroundColor = greenColor };
                Button newGameQuestionButton = new Button() { Text = "New Round", BackgroundColor = Color.DarkRed };
                resumeGameQuestionButton.Clicked += OnResumeGameQuestionButtonClicked;
                newGameQuestionButton.Clicked += OnNewGameQuestionButtonClicked;
                AbsoluteLayout.SetLayoutBounds(resumeGameQuestionButton, new Rectangle(0.5, .25, 250, 80));
                AbsoluteLayout.SetLayoutFlags(resumeGameQuestionButton, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(newGameQuestionButton, new Rectangle(0.5, .75, 250, 80));
                AbsoluteLayout.SetLayoutFlags(newGameQuestionButton, AbsoluteLayoutFlags.PositionProportional);

                questionPageLayout.Children.Clear();
                questionPageLayout.Children.Add(resumeGameQuestionButton);
                questionPageLayout.Children.Add(newGameQuestionButton);

                MainPage.Navigation.PushAsync(qp);                  //Question page
            }

            else
            {
                //Bring up course selection - it is a new game
                GenerateCourseList(true, false, false);
                MainPage.Navigation.PushAsync(sp);
            }
        }
        protected void PlayCourse(object sender, System.EventArgs e)
        {
            string courseName = (sender as Button).Text;
            if (courseName == "Add Course")
            {
                AddNewCourse();
            }
            else
            {
                NewGame(courseName);
            }
        }

        protected async void AddNewCourse()
        {
            customCoursePrompt = new Label 
            { 
                Text = "9 or 18 holes:",
                HorizontalOptions = LayoutOptions.Center
            };
            customCourseEntry = new Entry() { };
            customCourseNextButton = new Button() { Text = "Next" };
            customNineButton = new Button() { Text = "9" };
            customEighteenButton = new Button() { Text = "18" };

            enterPageLayout = new AbsoluteLayout()
            {
                Children =
                {
                    customCoursePrompt,
                    customNineButton,
                    customEighteenButton
                }
            };

            AbsoluteLayout.SetLayoutBounds(customCoursePrompt, new Rectangle(0.5, 0.5, 300, 200));
            AbsoluteLayout.SetLayoutFlags(customCoursePrompt, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customCourseEntry, new Rectangle(0.5, 0.7, 300, 60));
            AbsoluteLayout.SetLayoutFlags(customCourseEntry, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customCourseNextButton, new Rectangle(0.5, 0.95, 110, 60));
            AbsoluteLayout.SetLayoutFlags(customCourseNextButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customNineButton, new Rectangle(0.2, 0.6, 100, 60));
            AbsoluteLayout.SetLayoutFlags(customNineButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customEighteenButton, new Rectangle(0.8, 0.6, 100, 60));
            AbsoluteLayout.SetLayoutFlags(customEighteenButton, AbsoluteLayoutFlags.PositionProportional);

            customCourseNextButton.Clicked += GoToParPrompt;
            customNineButton.Clicked += GoToNamePrompt;
            customEighteenButton.Clicked += GoToNamePrompt;

            ep.Content = enterPageLayout;

            await MainPage.Navigation.PushAsync(ep);
        }

        protected async void NewGame(string courseName)
        {
            midRound = true;
            GolfCourse course = courseList.First(c => c.GetCourseName().Equals(courseName));
            int[] scorecard;
            if (course.GetLength() == 18)
            {
                scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            currentRound = new Round(course, scorecard, 1, 0);

            roundInfoButton.Text = "H1" + " P" + Convert.ToString(course.GetHolePar(1));
            overallButton.Text = "ovr: 0";
            strokeButton.Text = "0";
            nextHoleButton.Text = "Next\nHole";

            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(sp);
        }
        protected async void OnResumeGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            //Keep values
            int currentHole = currentRound.GetCurrentHole();
            roundInfoButton.Text = "H" + Convert.ToString(currentHole) + " P" + Convert.ToString(currentRound.GetCourse().GetHolePar(currentHole));
            nextHoleButton.Text = "Next\nHole";

            int currentCourseScoreRelativeToPar = currentRound.GetCurrentCourseScoreRelativeToPar();
            if (currentCourseScoreRelativeToPar > 0)
            {
                overallButton.Text = "ovr: +" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                overallButton.Text = "ovr: " + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            strokeButton.Text = Convert.ToString(currentRound.GetStrokes());
            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(qp);
        }

        protected void OnYesConfirmButtonClicked(object sender, System.EventArgs e)
        {
            //Ensure new course is loaded
            GenerateCourseList(true, false, false);
            MainPage.Navigation.PushAsync(sp);
            MainPage.Navigation.RemovePage(qqp);
            midRound = false;

            //Reset CurrentGame table
            var currentGameQueryResult = dbConnection.Query<RoundDB>("select * from RoundDB").FirstOrDefault(); //TODO: will need to find a way so that old users course info is not overwritten (table name change)
            if (currentGameQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(currentGameQueryResult);
                });
            }

            MainPage.Navigation.RemovePage(qp);
        }

        protected void OnAddStrokeButtonClicked(object sender, System.EventArgs e)
        {
            if (currentRound.GetStrokes() >= 20)
            {
                return;
            }
            currentRound.SetStrokes(currentRound.GetStrokes() + 1);
            strokeButton.Text = Convert.ToString(currentRound.GetStrokes());
        }
        protected void OnStrokeButtonClicked(object sender, System.EventArgs e)
        {
            //For now do nothing, possibly add a prompt to tell the user to select amount
        }
        protected void OnSubtractStrokeButtonClicked(object sender, System.EventArgs e)
        {
            if (currentRound.GetStrokes() == 0)
            {
                return;
            }
            currentRound.SetStrokes(currentRound.GetStrokes() - 1); //TODO: If this is too slow you can change the logic a bit to make it faster
            strokeButton.Text = Convert.ToString(currentRound.GetStrokes());
        }

        protected void OnMoreButtonClicked(object sender, System.EventArgs e)
        {
            Button courseLookupButton = new Button() { Text = "Course List", FontSize = 8, BackgroundColor = greenColor };
            courseLookupButton.Clicked += OnCourseListButtonClicked;
            Button roundHistoryButton = new Button() { Text = "Round History", FontSize = 8, BackgroundColor = greenColor };
            roundHistoryButton.Clicked += OnRoundHistoryButtonClicked;
            Button aboutButton = new Button() { Text = "About", FontSize = 8, BackgroundColor = greenColor };
            aboutButton.Clicked += OnAboutButtonClicked;

            morePageStackLayout = new CircleStackLayout
            {
                Children =
                {
                    courseLookupButton,
                    roundHistoryButton,
                    aboutButton
                }
            };
            morePageScrollView = new CircleScrollView
            {
                Content = morePageStackLayout
            };

            mop.Content = morePageScrollView;

            MainPage.Navigation.PushAsync(mop);
        }

        protected void OnCourseListButtonClicked(object sender, System.EventArgs e)
        {
            GenerateCourseList(false, true, false);
            MainPage.Navigation.PushAsync(clp);
        }

        protected void GenerateCourseList(bool courseSelectPage, bool courseLookupPage, bool roundHistoryPage)
        {

            CircleScrollView resultView;
            if (courseLookupPage)
            {
                resultView = GenerateList(false, true, false);
                clp.Content = resultView;
            }
            if (courseSelectPage)
            {
                resultView = GenerateList(true, false, false);
                sp.Content = resultView;
            }
            if (roundHistoryPage)
            {
                resultView = GenerateList(false, false, true);
                hp.Content = resultView;
            }

        }

        protected CircleScrollView GenerateList(bool courseSelectPage, bool courseLookupPage, bool roundHistoryPage)    //TODO: Rewrite this, try to get rid of the parameter
        {
            CircleStackLayout coursesLayout = new CircleStackLayout { };
            CircleScrollView courseSelectionScrollView = new CircleScrollView
            {
                Content = coursesLayout
            };

            if (!roundHistoryPage)
            {
                Button addNewCourseButton = new Button
                {
                    Text = "Add Course",
                    BackgroundColor = grayColor,
                    FontSize = 8
                };

                addNewCourseButton.Clicked += PlayCourse;

                coursesLayout.Children.Add(addNewCourseButton);
            }

            //Add the list of courses in the database
            foreach(GolfCourse course in courseList)
            {
                Button courseNameButton = new Button()
                {
                    Text = course.GetCourseName(),
                    BackgroundColor = greenColor,
                    FontSize = 8
                };
                if (courseSelectPage)
                {
                    courseNameButton.Clicked += PlayCourse;
                }
                else if (courseLookupPage)
                {
                    courseNameButton.Clicked += ListCourseDetails;
                }
                else if (roundHistoryPage)
                {
                    courseNameButton.Clicked += GenerateRoundHistoryList;
                }
                coursesLayout.Children.Add(courseNameButton);
            }

            return courseSelectionScrollView;
        }

        protected void OnNextHoleButtonClicked(object sender, System.EventArgs e)
        {
            int currentHole = currentRound.GetCurrentHole();
            int courseLength = currentRound.GetCourse().GetLength();
            int strokes = currentRound.GetStrokes();


            currentRound.SetScore(currentHole, strokes); //TODO: Can this be simplified? Automatically set score based on current hole and strokes 


            if (currentHole == courseLength) 
            {
                if (currentRound.CheckForZeros())
                {
                    Toast.DisplayText("There is at least one <br> hole without a score.");
                    return;
                }
                else
                {
                    CheckForFinishRound();
                    return;
                }
            }
             
            currentRound.SetCurrentHole(currentHole + 1);

            UpdateButtonsNext();
        }

        protected void OnPreviousHoleButtonClicked(object sender, System.EventArgs e)
        {
            int currentHole = currentRound.GetCurrentHole();
            int strokes = currentRound.GetStrokes();

            if (currentHole == 1)
            {
                return;
            }

            currentRound.SetScore(currentHole, strokes);
            currentRound.SetCurrentHole(currentHole - 1);

            UpdateButtonsPrevious();
        }
        //Updates text on the game screen once the Next Button is pressed (Parent: OnNextHoleButtonClicked)
        protected void UpdateButtonsNext()
        {
            int currentCourseScoreRelativeToPar = currentRound.GetCurrentCourseScoreRelativeToPar();
            string relativeCourseScoreString;
            if (currentCourseScoreRelativeToPar > 0)
            {
                relativeCourseScoreString = "+" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                relativeCourseScoreString = Convert.ToString(currentCourseScoreRelativeToPar);
            }

            int currentHole = currentRound.GetCurrentHole();

            if (currentHole == currentRound.GetCourse().GetLength())
            {
                nextHoleButton.Text = "Done";
            }

            roundInfoButton.Text = "H" + currentHole + " P" + Convert.ToString(currentRound.GetCourse().GetHolePar(currentHole));
            overallButton.Text = "ovr: " + relativeCourseScoreString;
            currentRound.SetStrokes(currentRound.GetScore(currentHole));
            strokeButton.Text = Convert.ToString(currentRound.GetStrokes());
        }
        //Updates text on the game screen once the Prev Button is pressed (Parent: OnPreviousHoleButtonClicked)
        protected void UpdateButtonsPrevious()
        {
            int currentCourseScoreRelativeToPar = currentRound.GetCurrentCourseScoreRelativeToPar();
            string relativeCourseScoreString;
            if (currentCourseScoreRelativeToPar > 0)
            {
                relativeCourseScoreString = "+" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                relativeCourseScoreString = Convert.ToString(currentCourseScoreRelativeToPar);
            }
           
            if (nextHoleButton.Text.Equals("Done")){
                nextHoleButton.Text = "Next\nHole";
            }

            int currentHole = currentRound.GetCurrentHole();
            roundInfoButton.Text = "H" + currentHole + " P" + Convert.ToString(currentRound.GetCourse().GetHolePar(currentHole));
            overallButton.Text = "ovr: " + relativeCourseScoreString;
            currentRound.SetStrokes(currentRound.GetScore(currentHole));
            strokeButton.Text = Convert.ToString(currentRound.GetStrokes());
        }

        public void CheckForFinishRound()
        {
            MainPage.Navigation.PushAsync(cffp);
        }

        public async void FinishRound(object sender, System.EventArgs e)
        {
            StackLayout finalLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                    {
                        new Label
                        {
                            Text = ""
                        }
                    }
            };
            finalScreenLayout = new CircleScrollView
            {
                Content = finalLayout
            };

            finalLayout.Children.Add(new Label
            {
                Text = currentRound.GetCourseName(),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            finalLayout.Children.Add(new Label
            {
                Text = DateTime.Now.ToString("MM/dd"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            int currentCourseScoreRelativeToPar = currentRound.GetCurrentCourseScoreRelativeToPar();

            Grid g = new Grid
            {
                RowDefinitions =
                {
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 }
                }
            };

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, 0);

            g.Children.Add(new Label
            {
                Text = "Hole",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, 0);

            g.Children.Add(new Label
            {
                Text = "Par",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, 0);

            g.Children.Add(new Label
            {
                Text = "Score",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, 0);

            int courseLength = currentRound.GetCourse().GetLength();

            for (int i = 0; i < courseLength; i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, 0, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 0, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, 1, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetCourse().GetHolePar(i+1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 1, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = greenColor
                }, 2, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetScore(i+1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 2, i + 1);
            }

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = "Total",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(currentRound.GetCourse().GetCoursePar()),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(currentRound.GetCurrentCourseScore()),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, courseLength + 2);

            g.Children.Add(new Label
            {
                Text = "Ovr",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, courseLength + 2);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, courseLength + 2);

            g.Children.Add(new Label
            {
                Text = currentCourseScoreRelativeToPar > 0 ? "+" + Convert.ToString(currentCourseScoreRelativeToPar) : Convert.ToString(currentCourseScoreRelativeToPar),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, courseLength + 2);

            finalLayout.Children.Add(g);
            finalLayout.Children.Add(new Label { });

            fp.Content = finalScreenLayout;

            await MainPage.Navigation.PushAsync(fp);
            MainPage.Navigation.RemovePage(ssp);
            MainPage.Navigation.RemovePage(cffp);
            midRound = false;

            //Reset all round counters
            var currentGameQueryResult = dbConnection.Query<RoundDB>("select * from RoundDB").FirstOrDefault();
            if (currentGameQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(currentGameQueryResult);
                });
            }

            //Add round record to ScoreDB 
            //Convert scorecard
            string scorecard = "";
            int[] scorecardInt = currentRound.GetScorecard();

            for (int i = 0; i < scorecardInt.Length; i++)
            {
                scorecard += scorecardInt[i].ToString();
            }

            ScoreDB roundScore = new ScoreDB(DateTime.Now, currentRound.GetCourseName(), scorecard);
            dbConnection.Insert(roundScore);

        }

        protected async void ListCourseDetails(object sender, System.EventArgs e)
        {
            Button removeButton = new Button() {
                Text = "Remove course",
                FontSize = 8
            };

            courseNameText = (sender as Button).Text;

            Label courseName = new Label()
            {
                Text = courseNameText,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 8
            };

            Label holeParLabel = new Label()
            {
                Text = "Hole/Par",
                FontSize = 6
            };

            Grid g = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{ },
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ },
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },

                }
            };

            GolfCourse course = courseList.First(c => c.GetCourseName().Equals(courseNameText));
            int courseLength = course.GetLength();

            for (int i = 0; i < courseLength; i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, i % 9, (2 * (i / 9)));

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (2 * (i / 9)));

                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, i % 9, (2 * (i / 9)) + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(course.GetHolePar(i+1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (2 * (i / 9)) + 1);
            }

            courseDetailLayout = new AbsoluteLayout
            {
                Children =
                {
                    removeButton,
                    courseName,
                    holeParLabel,
                    g
                }
            };

            AbsoluteLayout.SetLayoutBounds(courseName, new Rectangle(0.5, 0.2, 250, 120));
            AbsoluteLayout.SetLayoutFlags(courseName, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(removeButton, new Rectangle(0.5, .9, 150, 60));
            AbsoluteLayout.SetLayoutFlags(removeButton, AbsoluteLayoutFlags.PositionProportional);
            if (courseLength == 9)
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.55, 350, 100));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(holeParLabel, new Rectangle(0.1, 0.65, 140, 60));
                AbsoluteLayout.SetLayoutFlags(holeParLabel, AbsoluteLayoutFlags.PositionProportional);
            }
            else
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.5, 350, 100));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(holeParLabel, new Rectangle(0.1, 0.8, 140, 60));
                AbsoluteLayout.SetLayoutFlags(holeParLabel, AbsoluteLayoutFlags.PositionProportional);
            }

            removeButton.Clicked += DisplayRemoveCourseScreen;

            cdp.Content = courseDetailLayout;
            await MainPage.Navigation.PushAsync(cdp);
        }
        

        protected async void DisplayRemoveCourseScreen(object sender, System.EventArgs e)
        {
            areYouSureLabel.Text = "Delete all info for " + courseNameText + "?";   //TODO: Consider not using courseNameText as a global, is there a workaround?
            if ((midRound == true) && (courseNameText.Equals(currentRound.GetCourseName())))
            {
                areYouReallySureLabel.Text = "You will lose all data for your current round";
            }
            else
            {
                areYouReallySureLabel.Text = "";
            }
            await MainPage.Navigation.PushAsync(dp);
        }
        protected void RemoveCourse(string courseName)
        {
            //Remove course from database as well as global list (courseList)
            courseList.Remove(courseList.First(c => c.GetCourseName().Equals(courseName)));

            GenerateCourseList(false, true, false);

            if (midRound)
            {
                if (currentRound.GetCourseName().Equals(courseName))
                { 
                    midRound = false;
                    var currentGameQueryResult = dbConnection.Query<RoundDB>("select * from RoundDB").FirstOrDefault();
                    if (currentGameQueryResult != null)
                    {
                        dbConnection.RunInTransaction(() =>
                        {
                            dbConnection.Delete(currentGameQueryResult);
                        });
                    }
                }
            }

            var courseQueryResult = dbConnection.Query<GolfCourseDB>("select * from GolfCourseDB where Name = '" + courseName + "'").FirstOrDefault();
            if (courseQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(courseQueryResult);
                });
            }

            MainPage.Navigation.RemovePage(cdp);
        }

        protected void OnRoundHistoryButtonClicked(object sender, System.EventArgs e)
        {
            GenerateCourseList(false, false, true);
            MainPage.Navigation.PushAsync(hp);
        }

        protected void OnAboutButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Developed by<br>Andrew Johnson<br>Version 1.0");
        }

        protected void OnWaterAstheticButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Splash!");
        }

        protected void OnYesDeleteButtonClicked(object sender, System.EventArgs e)
        {
            RemoveCourse(courseNameText);
            MainPage.Navigation.RemovePage(dp);
        }

        protected void OnNoDeleteButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.PopAsync();
        }
        protected void OnNoFinishRoundButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.RemovePage(ep);
            MainPage.Navigation.PopAsync();
        }
        protected void OnYesOverwriteCourseClicked(object sender, System.EventArgs e)
        {
            midRound = false;
            courseList.Remove(courseList.FirstOrDefault(c => c.GetCourseName().Equals(courseToBeAdded.GetCourseName())));
            courseList.Add(courseToBeAdded);
            GolfCourseDB golfCourse = new GolfCourseDB(courseToBeAdded);
            dbConnection.InsertOrReplace(golfCourse);
            MainPage.Navigation.PopAsync();
            Toast.DisplayText("Course information for <br>" + newCourseName + "<br>has been overwritten.");
        }

        protected void OnNoOverwriteCourseClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.PopAsync();
        }

        protected void OnNewGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.PushAsync(qqp);
        }

        protected void OnNoConfirmButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.RemovePage(qqp);
        }
        protected async void OnRoundInfoButtonClicked(object sender, System.EventArgs e)
        {
            Label courseName = new Label()
            {
                Text = Convert.ToString(currentRound.GetCourseName()),
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 8
            };

            Label scoreCardLabel = new Label()
            {
                Text = "Scorecard",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 6
            };

            Label holeParLabel = new Label()
            {
                Text = "Hole/Par/Score",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 6
            };

            Grid g = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(2, GridUnitType.Star) },

                }
            };

            int courseLength = currentRound.GetCourse().GetLength();

            for (int i = 0; i < courseLength; i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, i % 9, (3 * (i / 9)));

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (3 * (i / 9)));

                g.Children.Add(new BoxView
                {
                    Color = puttingGreenColor
                }, i % 9, (3 * (i / 9)) + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetCourse().GetHolePar(i+1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (3 * (i / 9)) + 1);

                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, i % 9, (3 * (i / 9)) + 2);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetScore(i+1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (3 * (i / 9)) + 2);
            }

            enhanceButton = new Button() { Text = "Zoom In" };
            enhanceButton.Clicked += OnEnhanceButtonClicked;

            AbsoluteLayout currentScorecardLayout = new AbsoluteLayout
            {
                Children =
                {
                    courseName,
                    scoreCardLabel,
                    holeParLabel,
                    g,
                    enhanceButton
                }
            };
            
            AbsoluteLayout.SetLayoutBounds(courseName, new Rectangle(0.5, 0.1, 250, 120));
            AbsoluteLayout.SetLayoutFlags(courseName, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(scoreCardLabel, new Rectangle(0.5, 0.2, 300, 60));
            AbsoluteLayout.SetLayoutFlags(scoreCardLabel, AbsoluteLayoutFlags.PositionProportional);
            if (courseLength == 9)
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.55, 330, 140));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(holeParLabel, new Rectangle(0.5, 0.65, 300, 60));
                AbsoluteLayout.SetLayoutFlags(holeParLabel, AbsoluteLayoutFlags.PositionProportional);
            }
            else
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.5, 330, 140));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(holeParLabel, new Rectangle(0.5, 0.9, 300, 60));
                AbsoluteLayout.SetLayoutFlags(holeParLabel, AbsoluteLayoutFlags.PositionProportional);
            }

            AbsoluteLayout.SetLayoutBounds(enhanceButton, new Rectangle(0.5, 0.9, 150, 60));
            AbsoluteLayout.SetLayoutFlags(enhanceButton, AbsoluteLayoutFlags.PositionProportional);

            scp.Content = currentScorecardLayout;
            
            await MainPage.Navigation.PushAsync(scp);
        }

        protected void OnEnhanceButtonClicked(object sender, System.EventArgs e)
        {
            StackLayout scoreCardEnhancedLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 10
            };

            //left padding
            scoreCardEnhancedLayout.Children.Add(new Label
            {
                Text = "",
                Margin = 15
            });

            Grid g;

            g = new Grid
            {
                RowDefinitions =
                    {
                        new RowDefinition{ Height = 50 },
                        new RowDefinition{ Height = 50 },
                        new RowDefinition{ Height = 50 }
                    },
                ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = 100},
                    }
            };

            g.Children.Add(new BoxView
            {
                Color = darkGreenColor
            }, 0, 0);

            g.Children.Add(new Label
            {
                Text = "Hole",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 10
            }, 0, 0);

            g.Children.Add(new BoxView
            {
                Color = darkGreenColor
            }, 0, 1);

            g.Children.Add(new Label
            {
                Text = "Par",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 10
            }, 0, 1);

            g.Children.Add(new BoxView
            {
                Color = darkGreenColor
            }, 0, 2);

            g.Children.Add(new Label
            {
                Text = "Score",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 10
            }, 0, 2);

            scoreCardEnhancedLayout.Children.Add(g);

            for (int i = 0; i < currentRound.GetCourse().GetLength(); i++)
            {
                g = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition{ Height = 50 },
                        new RowDefinition{ Height = 50 },
                        new RowDefinition{ Height = 50 }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = 100},
                    }
                };


                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, 0, 0);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15
                }, 0, 0);

                g.Children.Add(new BoxView
                {
                    Color = puttingGreenColor
                }, 0, 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetCourse().GetHolePar(i + 1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15
                }, 0, 1);

                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, 0, 2);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(currentRound.GetScore(i + 1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15
                }, 0, 2);

                scoreCardEnhancedLayout.Children.Add(g);

            }

            //right padding
            scoreCardEnhancedLayout.Children.Add(new Label
            {
                Text = "",
                Margin = 30
            });

            CircleScrollView enhancedScreenLayout = new CircleScrollView
            {
                Content = scoreCardEnhancedLayout
                ,Orientation = ScrollOrientation.Horizontal
                ,VerticalOptions = LayoutOptions.Center
            };

            scp.Content = enhancedScreenLayout;

        }

        public void GenerateRoundHistoryList(object sender, System.EventArgs e)
        {
            var courseName = (sender as Button).Text;
            historyCourse = courseName;
            StackLayout historyLayout = new CircleStackLayout { };
            CircleScrollView historyScrollView = new CircleScrollView
            {
                Content = historyLayout
            };

            var scoreDBList = dbConnection.Query<ScoreDB>("select * from ScoreDB where CourseName = '" + courseName + "' order by Date desc;");

            var countRecords = 0;
            foreach (var scoreDB in scoreDBList)
            {
                countRecords++;

                Button historicalRoundButton = new Button
                {
                    Text = "#" + scoreDB.ID.ToString() + " " + scoreDB.Date.ToString("MM/dd/yy"),
                    BackgroundColor = grayColor,
                    FontSize = 8
                };

                historicalRoundButton.Clicked += GenerateRoundHistory;

                historyLayout.Children.Add(historicalRoundButton);
            }
            if (countRecords == 0)
            {
                Toast.DisplayText("No records to show");
            }
            else
            {
                chp.Content = historyScrollView;
                MainPage.Navigation.PushAsync(chp);
            }
        }

        public void GenerateRoundHistory(object sender, System.EventArgs e)
        {
            var roundDetails = (sender as Button).Text;
            string roundID = Convert.ToString(roundDetails[roundDetails.IndexOf("#") + 1]);
            ScoreDB round = dbConnection.Query<ScoreDB>("select * from ScoreDB where ID = '" + roundID + "';").FirstOrDefault();
            GolfCourse course = courseList.FirstOrDefault(c => c.GetCourseName().Equals(round.CourseName));
            
            //Convert scorecard to int[]
            int[] scorecard;
            if (course.GetLength() == 18)
            {
                scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                scorecard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            for (int i = 0; i < course.GetLength(); i++)
            {
                scorecard[i] = Convert.ToInt32(Convert.ToString(round.Scorecard[i]));
            }

            int currentCourseScoreRelativeToPar = course.CalculateCurrentScoreRelativeToPar(scorecard);


            StackLayout finalLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                    {
                        new Label
                        {
                            Text = ""
                        }
                    }
            };
            finalScreenLayout = new CircleScrollView
            {
                Content = finalLayout
            };

            finalLayout.Children.Add(new Label
            {
                Text = round.CourseName,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            finalLayout.Children.Add(new Label
            {
                Text = DateTime.Now.ToString("MM/dd"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            

            Grid g = new Grid
            {
                RowDefinitions =
                {
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 }
                }
            };

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, 0);

            g.Children.Add(new Label
            {
                Text = "Hole",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, 0);

            g.Children.Add(new Label
            {
                Text = "Par",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, 0);

            g.Children.Add(new Label
            {
                Text = "Score",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, 0);

            int courseLength = round.Scorecard.Length;

            for (int i = 0; i < courseLength; i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, 0, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 0, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, 1, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(course.GetHolePar(i + 1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 1, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = greenColor
                }, 2, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(round.Scorecard[i]),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 2, i + 1);
            }

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = "Total",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(course.GetCoursePar()),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, courseLength + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(scorecard.Sum()),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, courseLength + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, courseLength + 2);

            g.Children.Add(new Label
            {
                Text = "Ovr",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, courseLength + 2);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, courseLength + 2);

            g.Children.Add(new Label
            {
                Text = currentCourseScoreRelativeToPar > 0 ? "+" + Convert.ToString(currentCourseScoreRelativeToPar) : Convert.ToString(currentCourseScoreRelativeToPar),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, courseLength + 2);

            finalLayout.Children.Add(g);
            finalLayout.Children.Add(new Label { });

            fp.Content = finalScreenLayout;

            MainPage.Navigation.PushAsync(fp);

        }

        public int CheckCourseDuplicate(GolfCourse course)  //return 1 if course name already exists, else 0
        {
            if (courseList.FirstOrDefault(c => c.GetCourseName().Equals(course.GetCourseName())) != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public int CheckCurrentCourse(GolfCourse course)  //return 1 if course is currently being played, else 0
        {
            if (!midRound)
            {
                return 0;
            }
            else
            {
                if (currentRound.GetCourseName().Equals(course.GetCourseName()) && (midRound))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            if (midRound)
            {
                //Save the score for the hole the player was on when they left
                currentRound.SetScore(currentRound.GetCurrentHole(), currentRound.GetStrokes());
                
                RoundDB gameRecord = new RoundDB(currentRound);
                dbConnection.InsertOrReplace(gameRecord);
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
