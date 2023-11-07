using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    
    
    public partial class MainWindow : Window
    {
        #region constants
        private const int NEGATIVEONE = -1;
        private const int ZERO = 0;
        private const int ONE = 1;
        private const int LENGTH_QA = 5;
        #endregion

        #region Data Member

        StringBuilder result = new StringBuilder();

        List<Data> _QuestionData = new List<Data>();
        List<Data> _ChoicesData = new List<Data>();
        List<Results> _Results = new List<Results>();

        private int question_Num;
        private int _points;
        private string[] txtLines;
        private int lengthQuiz;
        private string _loadLocation;
        private string _saveLocation;

        

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            lbQuestion.ItemsSource = _QuestionData;
            lbLevels.ItemsSource = _QuestionData;
            lbChoices.ItemsSource = _ChoicesData;
        }

        #region Buttons

        /* informations about the maker of the quiz */
        private void btn_AboutUs(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Jiahao Yu 2134609 | Yensan Nguyen 1970670", "About us", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /* More instructions about the qquiz explaining how it works */
        private void btn_Instructions(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Start the quiz and answer all the questions, All the quiz are in the quiz folder ", "Instructions", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /*When pressing The start button, it will open up a window for the user to choose the quiz they wish to play 
         *Once done, it will load the first question
         *The user can only re-click the start button once the quiz is finished, and will reset everything and start anew */
        private void btn_Start(object sender, RoutedEventArgs e)
        {
            if (_QuestionData.Count == ZERO)
            {
                const int FIRST_QUIZ_LAUNCH = ZERO;
                

                Reset_Data_Member();

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Txt Files|*.txt"; // quiz are only in txt format
                if (openFileDialog.ShowDialog() == true)
                {
                    _loadLocation = openFileDialog.FileName;
                    txtLines = File.ReadAllLines(_loadLocation);
                    lengthQuiz = txtLines.Length / LENGTH_QA;  //set the lenght quiz for later functions
                    Load_QuestionData(FIRST_QUIZ_LAUNCH);                   
                }
            }
            else
            {
                MessageBox.Show("Finish the quiz first", "Start", MessageBoxButton.OK,
                MessageBoxImage.Information);
            }
        }

        /* next button will only work when quiz has started and one of the choices has been selected
         * everytime the user click the next button, it will load the next question and save the result in a string and records points 
         * once the user answer all the question, a new window will pop up and that display the result and points */
        private void btn_Next(object sender, RoutedEventArgs e)
        {
            if (!(_QuestionData.Count == ZERO))
            {
                if (!(lbChoices.SelectedIndex == NEGATIVEONE))
                {
                    if (question_Num < lengthQuiz)
                    {
                        result.AppendLine(GetResult(lbChoices.SelectedIndex));
                        Load_QuestionData(question_Num);                    
                    }
                    else
                    {
                        result.AppendLine(GetResult(lbChoices.SelectedIndex));
                        _Results.Add(new Results(_points, result.ToString()));

                        User_Result infoWindow = new User_Result(_Results);
                        infoWindow.Show();

                        Clear_Lists();
                        Refresh_Lists();
                    }

                    question_Num++;
                }
                else
                {
                    MessageBox.Show("Need to select an answer", "Next", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You need to Start the quiz by clicking the 'Start quiz' buttton to go to the next question", "Next", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        /* finish button will only work when quiz has started
         * Depending where the user is at in the quiz, it will continue from that point and assumed everything is wrong
         * then it will automatically display the result box*/
        private void btn_Finish(object sender, RoutedEventArgs e)
        {
            if (!(_QuestionData.Count == ZERO))
            {
                for (int i = question_Num; i < lengthQuiz; i++)
                {
                    result.AppendLine(GetResult(NEGATIVEONE));
                    Load_QuestionData(i);
                     
                    question_Num++;
                }
                
                result.AppendLine(GetResult(NEGATIVEONE));
                _Results.Add(new Results(_points, result.ToString())); 

                User_Result infoWindow = new User_Result(_Results);
                infoWindow.Show();

                question_Num++; // so it can save

                Clear_Lists();
                Refresh_Lists();
            }
            else
            {
                MessageBox.Show("You need to Start the quiz first by clicking the 'Start quiz' buttton to finish it", "Finish", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        /* Save button was decided to be put in the main window since the user will only be able to save their latest result
         * it will be uncessary to put the save button in a new window where they won't be able to save if they startede a new quiz
         * Save button will only work when quiz has ended
         * if the user finished the quiz, then they will be allowed to save their result to a text file to wherever directory they like  */
        private void btn_Save(object sender, RoutedEventArgs e)
        {
            if (question_Num > lengthQuiz)
            {
                if (string.IsNullOrEmpty(_saveLocation))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();               
                    saveFileDialog.Filter = "Txt Files|*.txt";
                    if (saveFileDialog.ShowDialog() == true)
                        _saveLocation = saveFileDialog.FileName;
                    else
                        return;
                }

                WriteRecordInfoToFile();
            }
            else
            {
                MessageBox.Show("Finish the quiz first before saving it", "Save", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        #endregion

        #region Methods

        /* The point of this function is to get the questions and answer from the quiz data and output it to the ui
         * it first take the string text, take a specfic section depending on the queston number the user is at, then determine what is a questions and correct answer
         * then story to the class*/
        private void Load_QuestionData(int questionNumber)
        {
            Clear_Lists();

            const int DIFF_START_END = 4;
            int questionStart = questionNumber * LENGTH_QA;
            int questionEnd = questionStart + DIFF_START_END;
            string[] txtLineFormated;

            try
            {
                for (int i = questionStart; i < txtLines.Length; i++)
                {
                    txtLineFormated = txtLines[i].Split(':');
                    if (txtLineFormated[ZERO] == "Q")
                    {
                        _QuestionData.Add(new Data() { Question = txtLineFormated[ONE], NumberofQuestion = $"{questionNumber + ONE} / {lengthQuiz}" });
                    }
                    else if (txtLineFormated[ZERO] == "A") 
                    { 
                        _ChoicesData.Add(new Data() { Correct = txtLineFormated[ONE], Answer = txtLineFormated[ONE] });
                    }
                    else
                    {
                        _ChoicesData.Add(new Data() { Answer = txtLineFormated[ONE] });
                    }

                    if (i == questionEnd)
                    {
                        break;
                    }
                }

                Refresh_Lists();
            }
            catch (Exception ex)
            {
                throw new Exception("All Data Property value not valid " + ex.Message);
            }

        }

        /* if the user choice was nothing, then the user hasn't answered,
         * it search the correct answer, store it along with the user choice
         * also adds point if the user gets the write answer and records it
         * it then return a string in the end telling what the user answer and the correct answer
         * also show whether the user anwser was correct or not*/
        private string GetResult(int user_choice)
        {
            //check for user's answer
            string user_answer;
            if (user_choice == NEGATIVEONE)
            {
                user_answer = "Not Anwsered";
            }
            else
            {
                user_answer = _ChoicesData[user_choice].Answer;
            }

            //check for correct anwser
            string correct_choice = "";
            for (int i = ZERO; i < _ChoicesData.Count; i++)
            {
                if (!(string.IsNullOrEmpty(_ChoicesData[i].Correct)))
                {
                     correct_choice = _ChoicesData[i].Correct;
                }
            }
            
            //show user if they are correct, and if so, give them a point
            if (!(user_choice == NEGATIVEONE))
            {
                if (user_answer == correct_choice)
                {

                    _points += ONE;
                    MessageBox.Show("Correct", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    MessageBox.Show("Wrong", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            string total = $"Question {question_Num}\n Your choice: {user_answer} | The answer : {correct_choice} ";
            return total;
        }
              
        private void WriteRecordInfoToFile()
        {
            try
            {
                File.WriteAllText(_saveLocation, $"{result.ToString()}Points: {_points}");
            }
            catch (Exception)
            {
                throw new Exception("Error while writing Record data to file");
            }
        }

        private void Clear_Lists()
        {
            _QuestionData.Clear();
            _ChoicesData.Clear();
        }

        private void Refresh_Lists()
        {
            lbLevels.Items.Refresh();
            lbQuestion.Items.Refresh();
            lbChoices.Items.Refresh();
        }

        private void Reset_Data_Member()
        {
            _Results.Clear();
            result.Clear();
            question_Num = ONE;
            _points = ZERO;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}

