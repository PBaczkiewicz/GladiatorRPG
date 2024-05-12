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
using static Android.Content.ClipData;

namespace GladiatorRPG
{
    public partial class MainTabPage
    {
        bool calculatingEXP = false;
        void EventSubscription()
        {
            player.CurrentHealthChanged += Player_CurrentHealthChanged;
            player.CurrentStaminaChanged += Player_CurrentStaminaChanged;
            player.GoldChanged += Player_GoldChanged;
            player.ExpChanged += Player_ExpChanged;
            player.FameChanged += Player_FameChanged;
            player.TutorialChanged += Player_TutorialChanged;
            //CurrentPageChanged += OnCurrentPageChanged;
        }

        #region Events
        //public void OnCurrentPageChanged(object sender, EventArgs e)
        //{
        //    //issue with invoking every "if" no matter the current page
        //    //Works in this state 09.07.23 -> not anymore 01.02.24
        //    activeDialogBox = false;
        //    TEST("PAGE CHANGED");

        //    //if (CurrentPage is TabbedPage characterTab)
        //    //{
        //    //    if (!charCheck)
        //    //    {
        //    //        TEST("charTab");
        //    //        charCheck = true;
        //    //        UpdateTrainingCosts();
        //    //        UpdateItemsToRecieve();
        //    //    }
        //    //    else charCheck = false;
        //    //}
        //    //if (CurrentPage is TabbedPage expeditionTab)
        //    //{
        //    //    if (!expeditionCheck)
        //    //    {
        //    //        TEST("expeditionTab");
        //    //        LoadExpeditions();
        //    //        LoadDungeons();
        //    //    }
        //    //    else expeditionCheck = false;
        //    //}
        //    //if (CurrentPage is TabbedPage cityTab)
        //    //{
        //    //    if (!expeditionCheck)
        //    //    {
        //    //        TEST("expeditionTab");
        //    //        LoadArena();
        //    //        LoadShop();
        //    //        LoadWorkPage();
        //    //        LoadDungeons();
        //    //    }
        //    //    else expeditionCheck = false;
        //    //}

        //    //if (CurrentPage is ContentPage overallScreen)
        //    //{
        //    //    if (!charCheck)
        //    //    {
        //    //        TEST("characterTab");
        //    //        charCheck = true;
        //    //        UpdateTrainingCosts();
        //    //        UpdateItemsToRecieve();

        //    //    }
        //    //    else { charCheck = false; }
        //    //}
        //    //if (CurrentPage is ContentPage trainingScreen)
        //    //{
        //    //    if (!trainingCheck)
        //    //    {
        //    //        TEST("trainingScreen");
        //    //        trainingCheck = true;
        //    //        UpdateTrainingCosts();
        //    //        UpdateItemsToRecieve();

        //    //    }
        //    //    else { trainingCheck = false; }
        //    //}

        //    //if (CurrentPage is ContentPage expeditionScreen)
        //    //{
        //    //    if (!expeditionCheck)
        //    //    {
        //    //        TEST("expeditionScreen");
        //    //        expeditionCheck = true;
        //    //        LoadExpeditions();


        //    //    }
        //    //    else { expeditionCheck = false; }
        //    //}
        //    //if (CurrentPage is ContentPage dungeonScreen)
        //    //{
        //    //    if (!dungeonCheck)
        //    //    {
        //    //        TEST("dungeonScreen");
        //    //        expeditionCheck = true;
        //    //        LoadDungeons();


        //    //    }
        //    //    else { dungeonCheck = false; }
        //    //}
        //    //if (CurrentPage is ContentPage workTab)
        //    //{
        //    //    if(!workCheck)
        //    //    {
        //    //        TEST("workTab");
        //    //        workCheck = true;
        //    //        LoadWorkPage();

        //    //    }
        //    //    else {  workCheck = false; }
        //    //}
        //    //if (CurrentPage is ContentPage shopTab)
        //    //{
        //    //    if (!shopCheck)
        //    //    {
        //    //        TEST("shopTab");
        //    //        shopCheck = true;
        //    //        LoadShop();

        //    //    }
        //    //    else { shopCheck = false; }
        //    //}
        //    //if(CurrentPage is ContentPage arenaTab)
        //    //{
        //    //    if(!arenaCheck)
        //    //    {
        //    //        TEST("arenaTab");
        //    //        arenaCheck = true;
        //    //        LoadArena();
        //    //        LoadShop();//testować czy czegoś nie zepsuje, powinno update'ować nagrodę z areny
        //    //    }
        //    //    else { arenaCheck = false; }
        //    //}

        //}


        //Zdarzenie wywołujące się gdy zmienia się player.CurrentHealth; !nie player.CurrentHealth;!
        private void Player_CurrentHealthChanged(object sender, EventArgs e)
        {
            if (player.CurrentHealth < 0) { player.CurrentHealth = 0; }
            else if (player.CurrentHealth > player.maxHealth) { player.CurrentHealth = player.maxHealth; }
            lifeCounter.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth.ToString();
            if (player.CurrentHealth > 0 && player.maxHealth > 0)
            {
                lifeBar.Progress = (float)player.CurrentHealth / (float)player.maxHealth;
            }
            else
            {
                lifeBar.Progress = 0;
            }
            if (player.CurrentHealth <= 0)
            {
                Die();
            }

            SaveProgress();

        }
        private void Player_CurrentStaminaChanged(object sender, EventArgs e)
        {
            if (player.currentStamina < 0) { player.currentStamina = 0; }
            else if (player.currentStamina > player.maxStamina) { player.currentStamina = player.maxStamina; }
            stamCounter.Text = player.currentStamina.ToString() + " / " + player.maxStamina.ToString();
            stamCounter2.Text = stamText + " " + player.currentStamina.ToString() + " / " + player.maxStamina.ToString();
            workStaminaLabel.Text = stamText + " " + player.currentStamina.ToString() + " / " + player.maxStamina.ToString();
            if (player.currentStamina >= 0 && player.maxStamina > 0)
            {
                stamBar.Progress = (float)player.currentStamina / (float)player.maxStamina;
                stamBar2.Progress = (float)player.currentStamina / (float)player.maxStamina;
                workStamina.Progress = (float)player.currentStamina / (float)player.maxStamina;

            }
            else { stamBar.Progress = 0; stamBar2.Progress = 0; workStamina.Progress = 0; }

            SaveProgress();
        }

        //Zdarzenie aktualizujące złoto gracza zmieniając Gold
        private void Player_GoldChanged(object sender, EventArgs e)
        {
            gold.Text = goldText + " : " + player.Gold.ToString() + localizedStrings["itemGoldText"];
            gold2.Text = goldText + " : " + player.Gold.ToString() + localizedStrings["itemGoldText"];
            gold3.Text = goldText + " : " + player.Gold.ToString() + localizedStrings["itemGoldText"];
            SaveProgress();
        }
        private void Player_ExpChanged(object sender, EventArgs e)
        {

            if (calculatingEXP != true)
            {
                calculatingEXP = true;
                while (player.Exp >= player.expNeededToLvl)
                {
                    LevelUp();

                }
                calculatingEXP = false;
            }
            UpdateExperience();


            SaveProgress();
        }
        void UpdateExperience()
        {
            levelProgress.Progress = (float)player.Exp / (float)player.expNeededToLvl;
            experience.Text = player.Exp.ToString() + "/" + player.expNeededToLvl.ToString();
            SaveProgress();
        }
        async void LevelUp()
        {
            player.Exp = player.Exp - player.expNeededToLvl;
            player.level++;
            player.expNeededToLvl = CalculateExpNeeded(player.level);

            EverythingStats();
            UpdateTrainingCosts();
            player.CurrentHealth = player.maxHealth;
            player.CurrentStamina = player.maxStamina;
            int reward = player.level * 150;
            string infoText = "";
            if (player.level == 5) infoText = "\n" + localizedStrings["levelUp5Info"];
            else if (player.level == 10) infoText = "\n" + localizedStrings["levelUp10Info"];
            else if (player.level == 15) infoText = "\n" + localizedStrings["levelUp15Info"];
            else if (player.level == 20) infoText = "\n" + localizedStrings["levelUp20Info"];
            else if (player.level == 25) infoText = "\n" + localizedStrings["levelUp25Info"];
            await DisplayAlert(localizedStrings["levelUpText"], localizedStrings["levelUpText1"] + player.level + "!\n" + localizedStrings["levelUpText2"] + reward.ToString() + " " + localizedStrings["itemGoldText"] + infoText, localizedStrings["okText"]);
            player.Gold += reward;
            player.winStreak = 0;
            SaveProgress();
            LoadEverything();
        }
        int CalculateExpNeeded(int level)
        {
            int exp = 10;
            for (int i = level; i > 1; i--)
            {
                if (i >= 35) exp += i * 2;
                exp += i;
            }


            return exp;
        }

        private void Player_FameChanged(object sender, EventArgs e)
        {
            fame.Text = fameText + " : " + player.Fame.ToString();
            fame2.Text = fameText + " : " + player.Fame.ToString();
            SaveProgress();
        }
        async public Task TUTORIAL(string text, int delay=0)
        {

            try
            {
                await Task.Delay(delay);
                string skipText = localizedStrings["skipText"];
                var action = await Application.Current.MainPage.DisplayActionSheet(localizedStrings["tutorialText"], okText, skipText, null, text);
                if (action == skipText) { player.tutorial = 0; player.shopDate = DateTime.Now.AddMinutes(5); }
            }
            catch
            {
                await DisplayAlert(localizedStrings["tutorialText"], text, okText);
            }
        }
        private async void Player_TutorialChanged(object sender, EventArgs e)
        {
            if (player.tutorial == 0) return;
            if (player.tutorial == 1)
            {
                await TUTORIAL(localizedStrings["tutorial1"]);
                //trainingScreen.IsVisible = false;
                //expeditionScreen.IsVisible = false;
                //dungeonScreen.IsVisible = false;
                //arenaTab.IsVisible = false;
                //workTab.IsVisible = false;
            }
            if (player.tutorial == 2)
            {
                await TUTORIAL(localizedStrings["tutorial2"]);

            }
            if (player.tutorial == 3)
            {
                await TUTORIAL(localizedStrings["tutorial3"]);
                //arenaTab.IsVisible = true;

            }
            if (player.tutorial == 4)
            {
                await TUTORIAL(localizedStrings["tutorial4"]);
                //trainingScreen.IsVisible = true;
                //expeditionScreen.IsVisible = true;
                //dungeonScreen.IsVisible = true;
                //arenaTab.IsVisible = true;
                //workTab.IsVisible = true;
                player.tutorial = 0;
                player.shopDate = DateTime.Now;

            }
        }
        public async void TUTORIAL1()
        {
            await TUTORIAL(localizedStrings["tutorial1"]);
        }
        #endregion
    }
}
