using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;
using GladiatorRPG.ResourceFiles;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;

namespace GladiatorRPG
{
    //Strona trenowania
    public partial class MainTabPage
    {
        #region Training screen
        //uaktualnia koszt trenowania statystyk i pokazuje postęp do maksymalnego poziomu statystyk
        float healthPercentage;
        float upCostPercentage = 5f;
        public void UpdateTrainingCosts()
        {
            float modifier = 0.5f; //połowa ceny jak na razie

            //TESTY
            //modifier = 0f;
            //TESTY

            #region Progressbars&values
            if (player.baseStrength > player.maxStamina + 1)
            {
                strProgress.Progress = ((float)player.baseStrength - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                strProgress.ProgressColor = Color.Gold;
            }
            else
            {
                strProgress.Progress = ((float)player.baseStrength / ((float)player.maxStamina + 1));
                strProgress.ProgressColor = Color.White;
            }
            str1.Text = player.baseStrength.ToString();
            if (player.basePerception > player.maxStamina + 1)
            {
                perProgress.Progress = ((float)player.basePerception - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                perProgress.ProgressColor = Color.Gold;
            }
            else
            {
                perProgress.Progress = ((float)player.basePerception / ((float)player.maxStamina + 1));
                perProgress.ProgressColor = Color.White;
            }
            per1.Text = player.basePerception.ToString();
            if (player.baseDexterity > player.maxStamina + 1)
            {
                dexProgress.Progress = ((float)player.baseDexterity - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                dexProgress.ProgressColor = Color.Gold;
            }
            else
            {
                dexProgress.Progress = ((float)player.baseDexterity / ((float)player.maxStamina + 1));
                dexProgress.ProgressColor = Color.White;
            }
            dex1.Text = player.baseDexterity.ToString();
            if (player.baseAgility > player.maxStamina + 1)
            {
                agiProgress.Progress = ((float)player.baseAgility - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                agiProgress.ProgressColor = Color.Gold;
            }
            else
            {
                agiProgress.Progress = ((float)player.baseAgility / ((float)player.maxStamina + 1));
                agiProgress.ProgressColor = Color.White;
            }
            agi1.Text = player.baseAgility.ToString();
            if (player.baseVitality > player.maxStamina + 1)
            {
                vitProgress.Progress = ((float)player.baseVitality - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                vitProgress.ProgressColor = Color.Gold;
            }
            else
            {
                vitProgress.Progress = ((float)player.baseVitality / ((float)player.maxStamina + 1));
                vitProgress.ProgressColor = Color.White;
            }
            vit1.Text = player.baseVitality.ToString();
            if (player.baseEndurance > player.maxStamina + 1)
            {
                endProgress.Progress = ((float)player.baseEndurance - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                endProgress.ProgressColor = Color.Gold;
            }
            else
            {
                endProgress.Progress = ((float)player.baseEndurance / ((float)player.maxStamina + 1));
                endProgress.ProgressColor = Color.White;
            }
            end1.Text = player.baseEndurance.ToString();
            if (player.baseCharisma > player.maxStamina + 1)
            {
                chaProgress.Progress = ((float)player.baseCharisma - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                chaProgress.ProgressColor = Color.Gold;
            }
            else
            {
                chaProgress.Progress = ((float)player.baseCharisma / ((float)player.maxStamina + 1));
                chaProgress.ProgressColor = Color.White;
            }
            cha1.Text = player.baseCharisma.ToString();
            if (player.baseIntelligence > player.maxStamina + 1)
            {
                intProgress.Progress = ((float)player.baseIntelligence - (float)player.maxStamina + 1) / ((float)player.maxStamina + 1);
                intProgress.ProgressColor = Color.Gold;
            }
            else
            {
                intProgress.Progress = ((float)player.baseIntelligence / ((float)player.maxStamina + 1)); int1.Text = player.baseIntelligence.ToString();
                intProgress.ProgressColor = Color.White;
            }
            int1.Text = player.baseIntelligence.ToString();
            #endregion


            if (player.baseStrength <= player.maxStamina)
            {
                strCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseStrength, 2)) * modifier) * player.trainingCostReduction); strCostLabel.Text = strCost.ToString(); strCostLabel.TextColor = Color.White;
            }
            else
            {
                strCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseStrength, 2)) * modifier) * CalculateHigherStatCost(player.baseStrength) * player.trainingCostReduction); strCostLabel.Text = strCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseStrength)-1) *100).ToString("0") + "%)"; strCostLabel.TextColor = Color.Gold;

            }
            if (player.basePerception <= player.maxStamina)
            {
                perCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.basePerception, 2)) * modifier) * player.trainingCostReduction); perCostLabel.Text = perCost.ToString(); perCostLabel.TextColor = Color.White;
            }
            else
            {
                perCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.basePerception, 2)) * modifier) * CalculateHigherStatCost(player.basePerception) * player.trainingCostReduction); perCostLabel.Text = perCost.ToString() + "(+" + ((CalculateHigherStatCost(player.basePerception)-1) *100).ToString("0") + "%)"; perCostLabel.TextColor = Color.Gold;

            }
            if (player.baseDexterity <= player.maxStamina)
            {
                dexCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseDexterity, 2)) * modifier) * player.trainingCostReduction); dexCostLabel.Text = dexCost.ToString(); dexCostLabel.TextColor = Color.White;
            }
            else
            {
                dexCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseDexterity, 2)) * modifier) * CalculateHigherStatCost(player.baseDexterity) * player.trainingCostReduction); dexCostLabel.Text = dexCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseDexterity)-1) *100).ToString("0") + "%)"; dexCostLabel.TextColor = Color.Gold;

            }
            if (player.baseAgility <= player.maxStamina)
            {
                agiCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseAgility, 2)) * modifier) * player.trainingCostReduction); agiCostLabel.Text = agiCost.ToString(); agiCostLabel.TextColor = Color.White;
            }
            else
            {
                agiCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseAgility, 2)) * modifier) * CalculateHigherStatCost(player.baseAgility) * player.trainingCostReduction); agiCostLabel.Text = agiCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseAgility)-1) *100).ToString("0") + "%)"; agiCostLabel.TextColor = Color.Gold;

            }
            if (player.baseVitality <= player.maxStamina)
            {
                vitCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseVitality, 2)) * modifier) * player.trainingCostReduction); vitCostLabel.Text = vitCost.ToString(); vitCostLabel.TextColor = Color.White;
            }
            else
            {
                vitCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseVitality, 2)) * modifier) * CalculateHigherStatCost(player.baseVitality) * player.trainingCostReduction); vitCostLabel.Text = vitCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseVitality)-1) *100).ToString("0") + "%)"; vitCostLabel.TextColor = Color.Gold;

            }
            if (player.baseEndurance <= player.maxStamina)
            {
                endCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseEndurance, 2)) * modifier) * player.trainingCostReduction); endCostLabel.Text = endCost.ToString(); endCostLabel.TextColor = Color.White;
            }
            else
            {
                endCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseEndurance, 2)) * modifier) * CalculateHigherStatCost(player.baseEndurance) * player.trainingCostReduction); endCostLabel.Text = endCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseEndurance)-1) *100).ToString("0") + "%)"; endCostLabel.TextColor = Color.Gold;

            }
            if (player.baseCharisma <= player.maxStamina)
            {
                chaCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseCharisma, 2)) * modifier) * player.trainingCostReduction); chaCostLabel.Text = chaCost.ToString(); chaCostLabel.TextColor = Color.White;
            }
            else
            {
                chaCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseCharisma, 2)) * modifier) * CalculateHigherStatCost(player.baseCharisma) * player.trainingCostReduction); chaCostLabel.Text = chaCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseCharisma)-1) *100).ToString("0") + "%)"; chaCostLabel.TextColor = Color.Gold;

            }
            if (player.baseIntelligence <= player.maxStamina)
            {
                intCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseIntelligence, 2)) * modifier) * player.trainingCostReduction); intCostLabel.Text = intCost.ToString(); intCostLabel.TextColor = Color.White;
            }
            else
            {
                intCost = (int)Math.Ceiling((int)Math.Ceiling(((int)Math.Pow(player.baseIntelligence, 2)) * modifier) * CalculateHigherStatCost(player.baseIntelligence) * player.trainingCostReduction); intCostLabel.Text = intCost.ToString() + "(+" + ((CalculateHigherStatCost(player.baseIntelligence)-1) *100).ToString("0") + "%)"; intCostLabel.TextColor = Color.Gold;

            }
        }

        float CalculateHigherStatCost(int stat)
        {
            return (float)Math.Round((((float)stat / (float)player.maxStamina - 1) * upCostPercentage)+1, 2);
        }
        void AfterTraining()
        {
            UpdateSubStats();
            UpdateTrainingCosts();
            EverythingStats();
        }

        async void TrainingInfo_Clicked(object sender, EventArgs e)
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["primaryStatsText"]
                    , localizedStrings["trainingExplanation"] + "\n\n" +
                      localizedStrings["strText"] + "\n" + localizedStrings["statStrInfo"] + "\n\n" +
                      localizedStrings["perText"] + "\n" + localizedStrings["statPerInfo"] + "\n\n" +
                      localizedStrings["dexText"] + "\n" + localizedStrings["statDexInfo"] + "\n\n" +
                      localizedStrings["agiText"] + "\n" + localizedStrings["statAgiInfo"] + "\n\n" +
                      localizedStrings["vitText"] + "\n" + localizedStrings["statVitInfo"] + "\n\n" +
                      localizedStrings["endText"] + "\n" + localizedStrings["statEndInfo"] + "\n\n" +
                      localizedStrings["chaText"] + "\n" + localizedStrings["statChaInfo"] + "\n\n" +
                      localizedStrings["intText"] + "\n" + localizedStrings["statIntInfo"]
                    , okText);
                clicked = false;
            }
        }
        void UpdateSubStats()
        {
            player.critDamage = 50 + (int)Math.Floor((float)player.strength / 5f);//strength
            player.regenerationHP = 1 + player.endurance; //endurance
            //player.trainingCostReduction = (((float)player.baseCharisma/((float)player.baseCharisma*2)));
        }

        #region Training buttons
        async void TrainStrength(object sender, EventArgs e)
        {
            if (player.Gold >= strCost && player.maxStamina * 2 >= player.baseStrength)
            {
                player.Gold -= strCost;
                //player.CurrentStamina -= player.baseStrength;
                player.baseStrength++;
                AfterTraining();
            }
            else if (player.Gold < strCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainPerception(object sender, EventArgs e)
        {
            if (player.Gold >= perCost && player.maxStamina * 2 >= player.basePerception)
            {
                player.Gold -= perCost;
                //player.CurrentStamina -= player.basePerception;
                player.basePerception++;
                AfterTraining();
            }
            else if (player.Gold < perCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainDexterity(object sender, EventArgs e)
        {
            if (player.Gold >= dexCost && player.maxStamina * 2 >= player.baseDexterity)
            {
                player.Gold -= dexCost;
                //player.CurrentStamina -= player.baseDexterity;
                player.baseDexterity++;
                AfterTraining();
            }
            else if (player.Gold < dexCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainAgility(object sender, EventArgs e)
        {
            if (player.Gold >= agiCost && player.maxStamina * 2 >= player.baseAgility)
            {
                player.Gold -= agiCost;
                player.baseAgility++;
                //player.CurrentStamina -= player.baseAgility;
                AfterTraining();
            }
            else if (player.Gold < agiCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainVitality(object sender, EventArgs e)
        {
            if (player.Gold >= vitCost && player.maxStamina * 2 >= player.baseVitality)
            {
                healthPercentage = (float)player.CurrentHealth / (float)player.maxHealth;
                player.Gold -= vitCost;
                //player.CurrentStamina -= player.baseVitality;
                player.baseVitality++;

                AfterTraining();

                player.CurrentHealth = (int)Math.Floor((float)player.maxHealth * healthPercentage);
            }
            else if (player.Gold < vitCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainEndurance(object sender, EventArgs e)
        {
            if (player.Gold >= endCost && player.maxStamina * 2 >= player.baseEndurance)
            {
                player.Gold -= endCost;
                //player.CurrentStamina -= player.baseEndurance;
                player.baseEndurance++;
                AfterTraining();
            }
            else if (player.Gold < endCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainCharisma(object sender, EventArgs e)
        {
            if (player.Gold >= chaCost && player.maxStamina * 2 >= player.baseCharisma)
            {
                player.Gold -= chaCost;
                //player.CurrentStamina -= player.baseCharisma;
                player.baseCharisma++;
                AfterTraining();
            }
            else if (player.Gold < chaCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        async void TrainIntelligence(object sender, EventArgs e)
        {
            if (player.Gold >= intCost && player.maxStamina * 2 >= player.baseIntelligence)
            {
                player.Gold -= intCost;
                //player.CurrentStamina -= player.baseIntelligence;
                player.baseIntelligence++;
                AfterTraining();
            }
            else if (player.Gold < intCost)
            {
                await DisplayAlert(cautionText, insufficientGoldMessage, okText);
            }
            else
            {
                await DisplayAlert(cautionText, insufficientMaxStaminaMessage, okText);
            }
        }
        #endregion
        #endregion

    }
}
