﻿using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Controls.Primitives;
using FitnessApp.SQLdatabase;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using System.Collections.Generic;
using LiveCharts.Helpers;
using System.Windows.Media;

namespace FitnessApp.UserMainWindowPages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        SQLqueries SQLqueriesObject = new SQLqueries();

        public HomePage()
        {
            InitializeComponent();
            UserMainWindow.HomePageObject = this;

            FoodComboBox.ItemsSource     = SQLqueriesObject.GetAllFood();
            WorkoutsComboBox.ItemsSource = SQLqueriesObject.GetAllWorkouts();

            LoadWeightChart(UserMainWindow.signedInUser.ID);
            LoadTotalWeightLostCard(UserMainWindow.signedInUser.ID);
            LoadAverageWeightLostCard(UserMainWindow.signedInUser.ID);
            LoadJoinedPlanCard(UserMainWindow.signedInUser.ID);
            LoadMotivationalQuoteCard();

            LoadJoinedChallengesCards();
        }


        // Weight Chart Properties

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }




        ////////// All Weight Cards Functions/Event Handlers //////////

        private void LoadWeightChart(int userID)
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Weight",
                    Values = SQLqueriesObject.GetWeightValues(userID).AsChartValues()
                }
            };

            Labels = SQLqueriesObject.GetWeightDateValues(userID);
            YFormatter = value => value.ToString();
            // Setting Data context for WeightChart
            WeightChart.DataContext = this;
        }

        private void DecimalNumbersOnlyFieldValidation(object sender, TextCompositionEventArgs e)
        {
            var s = sender as TextBox;
            var text = s.Text.Insert(s.SelectionStart, e.Text);
            e.Handled = !double.TryParse(text, out double d);
        }

        private void SaveWeightButton_Click(object sender, RoutedEventArgs e)
        {
            SQLqueriesObject.AddNewWeight(double.Parse(TodaysWeightTextBox.Text), UserMainWindow.signedInUser.ID);
            WeightChart.Series[0].Values.Add(double.Parse(TodaysWeightTextBox.Text));

            // Refresh Weight-Related Cards
            LoadTotalWeightLostCard(UserMainWindow.signedInUser.ID);
            LoadAverageWeightLostCard(UserMainWindow.signedInUser.ID);
        }

        private void LoadTotalWeightLostCard(int userID)
        {

            double WeightLostPerWeek = SQLqueriesObject.GetTotalWeightLostPerWeek(userID);
            double WeightLostPerMonth = SQLqueriesObject.GetTotalWeightLostPerMonth(userID);
            double WeightLostPerYear = SQLqueriesObject.GetTotalWeightLostPerYear(userID);


            // Set Colours
            if (WeightLostPerWeek < 0)
                WeightLostPerWeekTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                WeightLostPerWeekTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];

            if (WeightLostPerWeek < 0)
                WeightLostPerMonthTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                WeightLostPerMonthTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];

            if (WeightLostPerWeek < 0)
                WeightLostPerYearTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                WeightLostPerYearTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];


            // Assign Values to TextBoxes
            WeightLostPerWeekTextBlock.Text = WeightLostPerWeek.ToString();
            WeightLostPerMonthTextBlock.Text = WeightLostPerMonth.ToString();
            WeightLostPerYearTextBlock.Text = WeightLostPerYear.ToString();

        }

        private void LoadAverageWeightLostCard(int userID)
        {

            double AverageWeightLostPerWeek = SQLqueriesObject.GetAverageWeightLostPerWeek(userID);
            double AverageWeightLostPerMonth = SQLqueriesObject.GetAverageWeightLostPerMonth(userID);
            double AverageWeightLostPerYear = SQLqueriesObject.GetAverageWeightLostPerYear(userID);


            // Set Colours
            if (AverageWeightLostPerWeek < 0)
                AverageWeightLostPerWeekTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                AverageWeightLostPerWeekTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];

            if (AverageWeightLostPerMonth < 0)
                AverageWeightLostPerMonthTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                AverageWeightLostPerMonthTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];

            if (AverageWeightLostPerYear < 0)
                AverageWeightLostPerYearTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            else
                AverageWeightLostPerYearTextBlock.Foreground = (Brush)Application.Current.Resources["PrimaryHueMidBrush"];


            // Assign Values to TextBoxes
            AverageWeightLostPerWeekTextBlock.Text = AverageWeightLostPerWeek.ToString();
            AverageWeightLostPerMonthTextBlock.Text = AverageWeightLostPerMonth.ToString();
            AverageWeightLostPerYearTextBlock.Text = AverageWeightLostPerYear.ToString();
        }




        ////////// Joined Challenges Cards Functions/Event Handlers //////////


        // Setting Data context for JoinedChallengesListBox
        private void LoadJoinedChallengesCards()
        {
            ChallengesViewModel joinedChallengesDataContext = new ChallengesViewModel();
            joinedChallengesDataContext.JoinedChallengesViewModel(UserMainWindow.signedInUser.ID);
            UncompletedJoinedChallengesListBox.DataContext = joinedChallengesDataContext;
            CompletedJoinedChallengesListBox.DataContext = joinedChallengesDataContext;
            ControlNoChallengesCard(joinedChallengesDataContext);
        }

        private void JoinChallengeButton_Click(object sender, RoutedEventArgs e)
        {
            UserMainWindow.UserMainWindowObject.UserMainWindowPagesListBox.SelectedIndex = 1;
        }

        private void JoinChallengeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            int selectedChallengeIndex = UncompletedJoinedChallengesListBox.Items.IndexOf(toggleButton.DataContext);

            ChallengeModel currentChallenge = (ChallengeModel)UncompletedJoinedChallengesListBox.Items[selectedChallengeIndex];

            SQLqueriesObject.UnjoinChallenge(UserMainWindow.signedInUser.ID, currentChallenge.ID);

            // Reloading Data context for JoinedChallengesListBox
            ChallengesViewModel joinedChallengesDataContext = new ChallengesViewModel();
            joinedChallengesDataContext.JoinedChallengesViewModel(UserMainWindow.signedInUser.ID);
            UncompletedJoinedChallengesListBox.DataContext = joinedChallengesDataContext;
        }

        private void ControlNoChallengesCard(ChallengesViewModel challengesViewModel)
        {
            if (challengesViewModel.UncompletedJoinedChallengeModels.Count > 0)
                NoChallengesCard.Visibility = Visibility.Collapsed;
            else
                NoChallengesCard.Visibility = Visibility.Visible;
        }

        private void CompletedChallengeButton_Click(object sender, RoutedEventArgs e)
        {

        }



        ////////// Joined Plan Card Functions/Event Handlers //////////


        private void LoadJoinedPlanCard(int userID)
        {
            bool checkJoinedInPlan = SQLqueriesObject.IsInPlan(userID);

            NoPlanCard.Visibility = Visibility.Visible;
            JoinedPlanCard.Visibility = Visibility.Visible;
            PlanCompletedCard.Visibility = Visibility.Visible;

            if (checkJoinedInPlan)
            {
                NoPlanCard.Visibility = Visibility.Collapsed;

                int planDayNum = SQLqueriesObject.GetJoinedPlanDayNumber(userID);

                if (planDayNum > 30)
                    JoinedPlanCard.Visibility = Visibility.Collapsed;
                else
                {
                    PlanCompletedCard.Visibility = Visibility.Collapsed;

                    // Load Header
                    string planName = SQLqueriesObject.GetJoinedPlanName(userID).ToString();
                    PlanHeaderTextBlock.Text = planName + " | Day #" + planDayNum;
                    SQLqueriesObject.UpdatePlanDayNumber(userID, planDayNum);

                    // Load CheckBoxes
                    BreakfastCheckBox.IsChecked = SQLqueriesObject.GetDayBreakfastStatus(userID);
                    LunchCheckBox.IsChecked = SQLqueriesObject.GetDayLunchStatus(userID);
                    DinnerCheckBox.IsChecked = SQLqueriesObject.GetDayDinnerStatus(userID);
                    WorkoutsCheckBox.IsChecked = SQLqueriesObject.GetDayWorkoutStatus(userID);

                    // Load Descriptions
                    BreakfastDescriptionTextBlock.Text = SQLqueriesObject.GetDayBreakfastDescription(userID);
                    LunchDescriptionTextBlock.Text = SQLqueriesObject.GetDayLucnchDescription(userID);
                    DinnerDescriptionTextBlock.Text = SQLqueriesObject.GetDayDinnerDescription(userID);
                    WorkoutsDescriptionTextBlock.Text = SQLqueriesObject.GetDayWorkoutDescription(userID);

                    // Load Progress Bar
                    PlanProgressBar.Value = planDayNum;
                }
            }
            else
            {
                JoinedPlanCard.Visibility = Visibility.Collapsed;
                PlanCompletedCard.Visibility = Visibility.Collapsed;
            }
        }

        private void DayItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;

            switch (currentCheckBox.Name)
            {
                case "BreakfastCheckBox":
                    SQLqueriesObject.UpdateDayBreakfastStatus(true, UserMainWindow.signedInUser.ID);
                    break;

                case "LunchCheckBox":
                    SQLqueriesObject.UpdateDayLunchStatus(true, UserMainWindow.signedInUser.ID);
                    break;

                case "DinnerCheckBox":
                    SQLqueriesObject.UpdateDayDinnerStatus(true, UserMainWindow.signedInUser.ID);
                    break;

                case "WorkoutsCheckBox":
                    SQLqueriesObject.UpdateDayWorkoutStatus(true, UserMainWindow.signedInUser.ID);
                    break;
            }
        }

        private void DayItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;

            switch (currentCheckBox.Name)
            {
                case "BreakfastCheckBox":
                    SQLqueriesObject.UpdateDayBreakfastStatus(false, UserMainWindow.signedInUser.ID);
                    break;

                case "LunchCheckBox":
                    SQLqueriesObject.UpdateDayLunchStatus(false, UserMainWindow.signedInUser.ID);
                    break;

                case "DinnerCheckBox":
                    SQLqueriesObject.UpdateDayDinnerStatus(false, UserMainWindow.signedInUser.ID);
                    break;

                case "WorkoutsCheckBox":
                    SQLqueriesObject.UpdateDayWorkoutStatus(false, UserMainWindow.signedInUser.ID);
                    break;
            }
        }

        private void JoinPlanButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserMainWindow.UserMainWindowObject.UserMainWindowPagesListBox.SelectedIndex = 2;
        }

        private void DismissPlanButton_Click(object sender, RoutedEventArgs e)
        {
            SQLqueriesObject.UnjoinPlan(UserMainWindow.signedInUser.ID);
            LoadJoinedPlanCard(UserMainWindow.signedInUser.ID);
            UserMainWindow.PlansPageObject.PlansListBox.DataContext = new PlansViewModel(UserMainWindow.signedInUser.ID);
        }


        //////////////////////////////////////////////////////////////




        ////////// Motivational Quotes Card Functions/Event Handlers //////////

        private void LoadMotivationalQuoteCard()
        {
            MotiationalQuoteTextBlock.Text = SQLqueriesObject.GetMotivationalQuote();
        }

        //////////////////////////////////////////////////////////////////////




        ////////// Calories Card Functions/Event Handlers //////////


        ///////////////////////////////////////////////////////////




        ////////// PopUpBox Functions/Event Handlers //////////

        private void AddFoodButton_Click(object sender, RoutedEventArgs e)
        {
            DialogBox.IsOpen = true;
            AddFoodDialogBox.Visibility = Visibility.Visible;
        }

        private void AddWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            DialogBox.IsOpen = true;
            AddWorkoutDialogBox.Visibility = Visibility.Visible;
        }

        //////////////////////////////////////////////////////




        ////////// DialogBoxes Functions/Event Handlers //////////

        private void DialogBoxAddFoodButton_Click(object sender, RoutedEventArgs e)
        {

            if (FoodComboBox.SelectedIndex == -1)
                UserMainWindow.UserMainWindowObject.MessagesSnackbar.MessageQueue.Enqueue("Please choose Food!");

            else if (FoodQuantityTextBox.Text == "")
                UserMainWindow.UserMainWindowObject.MessagesSnackbar.MessageQueue.Enqueue("Please enter Food Quantity!");

            else
            {
                SQLqueriesObject.AddFood(FoodComboBox.Text, double.Parse(FoodQuantityTextBox.Text), UserMainWindow.signedInUser.ID);
                AddFoodDialogBox.Visibility = Visibility.Collapsed;
                DialogBox.IsOpen = false;
            }

            FoodComboBox.SelectedIndex = -1;
            FoodQuantityTextBox.Text = "";
        }

        private void DialogBoxAddWorkoutButton_Click(object sender, RoutedEventArgs e)
        {

            if (WorkoutsComboBox.SelectedIndex == -1)
                UserMainWindow.UserMainWindowObject.MessagesSnackbar.MessageQueue.Enqueue("Please choose Workout!");

            else if (WorkoutsDurationTextBox.Text == "")
                UserMainWindow.UserMainWindowObject.MessagesSnackbar.MessageQueue.Enqueue("Please enter Workout Duration!");

            else
            {
                SQLqueriesObject.AddWorkout(WorkoutsComboBox.Text, double.Parse(WorkoutsDurationTextBox.Text), UserMainWindow.signedInUser);
                AddWorkoutDialogBox.Visibility = Visibility.Collapsed;
                DialogBox.IsOpen = false;
            }

            WorkoutsComboBox.SelectedIndex = -1;
            WorkoutsDurationTextBox.Text = "";
            LoadJoinedChallengesCards();
        }

        private void DialogBoxCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddFoodDialogBox.Visibility = Visibility.Collapsed;
            AddWorkoutDialogBox.Visibility = Visibility.Collapsed;
            DialogBox.IsOpen = false;

            WorkoutsComboBox.SelectedIndex = -1;
            WorkoutsDurationTextBox.Text = "";

            FoodComboBox.SelectedIndex = -1;
            FoodQuantityTextBox.Text = "";
        }

        ///////////////////////////////////////////////////////////


    }
}
