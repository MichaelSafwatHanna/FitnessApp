﻿using FitnessApp.Models;
using System;
using FitnessApp.SQLdatabase;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FitnessApp.ViewModels
{
    class ChallengesViewModel
    {
        static SQLqueries SQLqueriesObject = new SQLqueries();
        private List<ChallengeModel> allChallengeModels;
        private List<ChallengeModel> joinedChallengeModels;

        public ChallengesViewModel() { }

        public void AllChallengesViewModel(int accountID)
        {
            allChallengeModels = SQLqueriesObject.LoadAllChallenges(accountID);
        }

        public void JoinedChallengesViewModel(int accountID)
        {
            joinedChallengeModels = SQLqueriesObject.LoadJoinedChallenges(accountID);

            foreach (var item in joinedChallengeModels)
            {
                string joiningDate = SQLqueriesObject.GetChallengeJoiningDate(accountID, item.ID);

                int tempProgress = SQLqueriesObject.GetChallengeProgress
                                    (accountID, joiningDate, item.DueDate, item.WorkoutType);

                if (tempProgress > -1)
                {
                    item.Progress = tempProgress;
                }
            }

        }

        public List<ChallengeModel> AllChallengeModels
        {
            get => allChallengeModels;
            set { }
        }

        public List<ChallengeModel> JoinedChallengeModels
        {
            get => joinedChallengeModels;
            set { }
        }
    }
}
