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
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using static Android.Resource;
using Android.App;

namespace GladiatorRPG
{
    //Strona wypraw
    public partial class MainTabPage
    {
        #region Expedition screen
        public bool result;
        List<Button> expeditionButtons;
        //Wczytuje wszystkie ekspedycje
        public void LoadExpeditions()
        {
            expeditionInfo.Text = localizedStrings["expeditionText"]; expeditionDurationText.Text = localizedStrings["expeditionDurationText"];

            granaryInfo.Text = localizedStrings["granaryInfo"]; granaryTime.Text = localizedStrings["granaryTime"];
            granaryButton.Text = localizedStrings["embarkText"];

            forestInfo.Text = localizedStrings["forestInfo"]; forestTime.Text = localizedStrings["forestTime"];
            forestButton.Text = localizedStrings["embarkText"];

            mountainsInfo.Text = localizedStrings["mountainsInfo"]; mountainsTime.Text = localizedStrings["mountainsTime"];
            mountainsButton.Text = localizedStrings["embarkText"];

            banditCampInfo.Text = localizedStrings["banditCampInfo"]; banditCampTime.Text = localizedStrings["banditCampTime"];
            banditCampButton.Text = localizedStrings["embarkText"];

            desertInfo.Text = localizedStrings["desertInfo"]; desertTime.Text = localizedStrings["desertTime"];
            desertButton.Text = localizedStrings["embarkText"];

            ShowExpeditions();

            if (player.expeditionInProgress && (player.expeditionName == "work1" || player.expeditionName == "work2" || player.expeditionName == "work3" || player.expeditionName == "work4"))
            {
                quickenExpedition.IsVisible = false;
                if (expeditionButton != null) expeditionButton.Text = remainingTimeFormatted.ToString();
                foreach (Button x in expeditionButtons) x.IsEnabled = false;




            }
            else if (player.expeditionInProgress)
            {
                foreach (Button x in expeditionButtons)
                {
                    x.IsEnabled = false;
                    if (x == expeditionButton)
                    {
                        expeditionButton.IsEnabled = true;
                        quickenExpedition.IsVisible = true;
                    }
                }

            }
            else
            {
                quickenExpedition.IsVisible = false;
                foreach (Button x in expeditionButtons) x.IsEnabled = true;

            }
        }

        void ShowExpeditions()
        {
            Label label;
            Button button;
            Stack<Label> labels = new Stack<Label>();
            Stack<Button> buttons = new Stack<Button>();
            expeditionButtons = new List<Button>();
            int rowPosition = 0;
            RowDefinition newRow = new RowDefinition { Height = GridLength.Star };
            //Od pierwszego poziomu daje dostęp do wyprawy
            if (player.level >= 1) { rowPosition++; labels.Push(granaryInfo); labels.Push(granaryTime); buttons.Push(granaryButton); }
            //Od piątego poziomu daje dostęp do wyprawy
            if (player.level >= 5) { rowPosition++; labels.Push(forestInfo); labels.Push(forestTime); buttons.Push(forestButton); }
            if (player.level >= 10) { rowPosition++; labels.Push(mountainsInfo); labels.Push(mountainsTime); buttons.Push(mountainsButton); }
            if (player.level >= 20) { rowPosition++; labels.Push(banditCampInfo); labels.Push(banditCampTime); buttons.Push(banditCampButton); }
            if (player.level >= 30) { rowPosition++; labels.Push(desertInfo); labels.Push(desertTime); buttons.Push(desertButton); }

            for (int i = 1; i <= rowPosition; i++)
            {
                expeditionButtons.Add(buttons.Peek());
                expeditionGrid.RowDefinitions.Add(newRow);
                label = labels.Pop(); label.IsVisible = true;
                int currentRow = Grid.GetRow(label);
                Grid.SetRow(label, i);
                label = labels.Pop(); label.IsVisible = true;
                //currentRow = Grid.GetRow(label);
                Grid.SetRow(label, i);
                button = buttons.Pop(); button.IsVisible = true;
                Grid.SetRow(button, i);

            }

        }

        #region Expeditions
        async void quickenExpedition_Clicked(object sender, EventArgs e)
        {
            int cost = 1;

            TimeSpan timeLeft = player.expeditionEnd - DateTime.Now;

            cost = (timeLeft.Hours * 60) + timeLeft.Minutes + 1;
            bool result = await DisplayAlert(cautionText, localizedStrings["expeditionSpeedup"] + cost + " " + localizedStrings["stamText"] + "\n" + localizedStrings["currentStaminaText"] + player.CurrentStamina + "\n" + localizedStrings["stamText"], yesText, noText);
            if (result)
            {
                if (timeLeft == TimeSpan.Zero) { return; }
                if (player.CurrentStamina < cost)
                {

                    await DisplayAlert(cautionText, localizedStrings["insufficientStaminaMessage"], okText);
                    return;
                }
                else
                {
                    player.CurrentStamina -= cost;
                    player.expeditionEnd = DateTime.Now;
                    DeleteNotifications(2);
                }
            }
            else
            {
                return;
            }

        }
        async void ExpeditionStart(Button button, int minutes = 0, int hours = 0, int seconds = 0)
        {
            //IF dający możliwość anulowania wyprawy
            if (player.expeditionInProgress)
            {
                bool result = await DisplayAlert(cautionText, expeditionCancel, yesText, noText);

                if (result)
                {
                    if (player.expeditionInProgress == false) { return; }
                    quickenExpedition.IsVisible = false;
                    player.expeditionInProgress = false;
                    slider.IsEnabled = true;
                    button.Text = localizedStrings["embarkText"];
                    //Enable'uje pozostałe przyciski
                    foreach (Button x in buttonList) { if (x == expeditionButton) { continue; }; x.IsEnabled = true; }
                    return;
                }
                else
                {
                    return;
                }

            }
            quickenExpedition.IsVisible = true;
            player.expeditionEnd = DateTime.Now;
            slider.IsEnabled = false;
            //TESTY
            if (hours > 0) { player.expeditionEnd = player.expeditionEnd.AddHours(hours); }//argument do zamiany na hours
            player.expeditionEnd = player.expeditionEnd.AddMinutes(minutes);//argument do zamiany na minutes

            player.expeditionEnd = player.expeditionEnd.AddSeconds(seconds);

            expeditionButton = button;
            player.expeditionInProgress = true;
            if (player.expeditionNotification) SendNotification(localizedStrings["notificationExpeditionFinished"], localizedStrings["notificationExpeditionFinished1"], 2, player.expeditionEnd);
            //Disable'uje wszystkie przyciski oprócz wciśniętego
            foreach (Button x in buttonList) { if (x == expeditionButton) { continue; }; x.IsEnabled = false; }

        }
        void ExpeditionResume()
        {
            switch (player.expeditionName)
            {
                #region work buttons
                case "work1":
                    expeditionButton = workButton1;
                    break;
                case "work2":
                    expeditionButton = workButton2;
                    break;
                case "work3":
                    expeditionButton = workButton3;
                    break;
                case "work4":
                    expeditionButton = workButton4;
                    break;
                #endregion

                case "granary":
                    expeditionButton = granaryButton;
                    break;
                case "forest":
                    expeditionButton = forestButton;
                    break;
                case "mountains":
                    expeditionButton = mountainsButton;
                    break;
                case "banditCamp":
                    expeditionButton = banditCampButton;
                    break;
                case "desert":
                    expeditionButton = desertButton;
                    break;


            }
            foreach (Button x in buttonList) { if (x == expeditionButton) { continue; }; x.IsEnabled = false; }

        }
        void ExpeditionArrive()
        {
            if (player.expeditionName == "granary") { GranaryExpedition(); }
            if (player.expeditionName == "forest") { ForestExpedition(); }
            if (player.expeditionName == "mountains") { MountainsExpedition(); }
            if (player.expeditionName == "banditCamp") { BanditCampExpedition(); }
            if (player.expeditionName == "desert") { DesertExpedition(); }

        }
        void GenerateEnemy(string name, string source, int lv)
        {
            int items = (int)Math.Ceiling((float)lv / 4);
            if (items > 9) items = 9;
            switch (name)
            {
                case "":
                    return;

                case "enemyNameRat"://1 exp
                    enemy = EnemyGeneration(name, source, lv, items, 40, 35, 35, 45, 35, 25, 20, 15); //~250%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameSpider"://1 exp
                    enemy = EnemyGeneration(name, source, lv, items, 50, 45, 40, 50, 45, 25, 20, 15); //~290%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameHungrySlave"://1 exp
                    enemy = EnemyGeneration(name, source, lv, items, 50, 45, 45, 35, 50, 45, 40, 30); //~340%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameLynx": //2 exp
                    enemy = EnemyGeneration(name, source, lv, items, 35, 65, 45, 70, 60, 25, 35, 20); //~355%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameWolf"://2 exp
                    enemy = EnemyGeneration(name, source, lv, items, 40, 70, 60, 80, 45, 50, 35, 20); //~400%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameBoar"://2 exp
                    enemy = EnemyGeneration(name, source, lv, items, 90, 45, 45, 50, 75, 65, 30, 25); //~425%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameBear"://3 exp
                    enemy = EnemyGeneration(name, source, lv, items, 90, 60, 50, 20, 110, 50, 15, 15); //~410%
                    break;//                                         str per dex agi  vit end cha int
                case "enemyNameBandit"://3, 4 exp
                    enemy = EnemyGeneration(name, source, lv, items, 70, 80, 75, 60, 70, 40, 60, 45); //~500%
                    break;//                                         str per dex agi vit end cha int
                case "enemyNameBattleDog"://4 exp
                    enemy = EnemyGeneration(name, source, lv, items, 55, 125, 85, 115, 60, 35, 55, 15); //~545%
                    break;//                                         str per  dex agi  vit end cha int
                case "enemyNameBanditCaptain"://4 exp
                    enemy = EnemyGeneration(name, source, lv, items, 70, 100, 80, 70, 90, 60, 100, 70); //~640%
                    break;//                                         str per  dex agi vit end cha int
                case "enemyNameScorpion"://5 exp
                    enemy = EnemyGeneration(name, source, lv, items, 55, 105, 85, 100, 75, 130, 110, 55); //~715%
                    break;//                                         str per  dex agi  vit  end  cha int
                case "enemyNameDesertBandit"://5 exp
                    enemy = EnemyGeneration(name, source, lv, items, 105, 115, 90, 110, 70, 75, 80, 80); //~725%
                    break;//                                         str  per  dex agi  vit end cha int
                case "enemyNameDesertChief"://5 exp
                    enemy = EnemyGeneration(name, source, lv, items, 100, 105, 110, 90, 75, 80, 110, 110); //~780%
                    break;//                                         str  per  dex  agi vit end cha  int
                default:
                    TEST("ERROR! ENEMY NOT FOUND");
                    break;

            }
        }

        #region Granary expedition
        void Granary_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "granary";
                ExpeditionStart(button, 5);
            }

        }
        void GranaryExpedition()
        {


            int enemylvl, roll;
            string enemyName = "", enemySource = "";
            if (player.level < 3) { roll = random.Next(1, 3); }
            else roll = random.Next(1, 4);
            if (roll == 1) { enemyName = "enemyNameRat"; enemySource = "enemy_rat"; }
            else if (roll == 2) { enemyName = "enemyNameSpider"; enemySource = "enemy_spider"; }
            else if (roll == 3) { enemyName = "enemyNameHungrySlave"; enemySource = "enemy_slave"; }
            enemy = new Entity();
            if (player.level == 1)
            {
                enemylvl = player.level;
            }
            else if (player.level < 5)
            {
                enemylvl = random.Next(player.level - 1, player.level + 1);
                if(player.winStreak>10) enemylvl = random.Next(player.level, player.level + 2);
            }
            else
            {
                enemylvl = 5;
            }
            GenerateEnemy(enemyName, enemySource, enemylvl);
            int gold = random.Next(enemylvl * 15, enemylvl * 30);
            int exp = 2 + (int)Math.Ceiling((float)enemylvl / 10f); //to balance out
            int itemReward = 1;
            int itemChance = 60;
            Fight(player, enemy, gold, exp, itemChance, itemReward);



        }
        #endregion
        #region Forest expedition
        void Forest_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "forest";
                ExpeditionStart(button, 10);
            }
        }
        void ForestExpedition()
        {
            int enemylvl, roll;
            string enemyName = "", enemySource = "";
            if (player.level < 7) { roll = random.Next(1, 3); }
            else roll = random.Next(1, 4);

            if (roll == 1) { enemyName = "enemyNameLynx"; enemySource = "enemy_lynx"; }
            else if (roll == 2) { enemyName = "enemyNameWolf"; enemySource = "enemy_wolf"; }
            else if (roll == 3) { enemyName = "enemyNameBoar"; enemySource = "enemy_boar"; }
            enemy = new Entity();
            if (player.level < 10)
            {
                enemylvl = random.Next(player.level - 1, player.level + 1);
                if (player.winStreak > 10) enemylvl = random.Next(player.level, player.level + 2);
            }
            else enemylvl = 10;
            GenerateEnemy(enemyName, enemySource, enemylvl);

            int gold = random.Next(enemylvl * 20, enemylvl * 40);
            int exp = 3 + (int)Math.Ceiling((float)enemylvl / 5f); //to balance out
            int itemReward = 1;
            int itemChance = 60;
            Fight(player, enemy, gold, exp, itemChance, itemReward);

        }




        #endregion
        #region Mountains expedition
        void Mountains_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "mountains";
                ExpeditionStart(button, 30);
            }
        }
        void MountainsExpedition()
        {
            int enemylvl, roll;
            string enemyName = "", enemySource = "";
            if (player.level < 12) { roll = random.Next(1, 3); }
            else roll = random.Next(1, 4);
            if (roll == 1) { enemyName = "enemyNameWolf"; enemySource = "enemy_wolf"; }
            else if (roll == 2) { enemyName = "enemyNameBandit"; enemySource = "enemy_bandit"; }
            else if (roll == 3) { enemyName = "enemyNameBear"; enemySource = "enemy_bear"; }
            enemy = new Entity();
            if (player.level < 20)
            {
                enemylvl = random.Next(player.level - 1, player.level + 2);
            }
            else enemylvl = 20;
            GenerateEnemy(enemyName, enemySource, enemylvl);

            int gold = random.Next(enemylvl * 30, enemylvl * 50) + 200;
            int exp = 5 + (int)Math.Ceiling((float)enemylvl / 3f); //to balance out
            int itemReward = 1;
            int itemChance = 80;
            Fight(player, enemy, gold, exp, itemChance, itemReward);
        }
        #endregion
        #region Bandit camp expedition
        void BanditCamp_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "banditCamp"; //DOPISAĆ DO ExpeditionResume() ALBO NIE BĘDZIE DZIAŁAĆ!!!!!
                ExpeditionStart(button, 45);
            }
        }
        void BanditCampExpedition()
        {
            int enemylvl, roll;
            string enemyName = "", enemySource = "";
            if (player.level < 22) { roll = random.Next(1, 3); }
            else roll = random.Next(1, 4);
            if (roll == 1) { enemyName = "enemyNameBandit"; enemySource = "enemy_bandit"; }
            else if (roll == 2) { enemyName = "enemyNameBattleDog"; enemySource = "enemy_banditDog"; }
            else if (roll == 3) { enemyName = "enemyNameBanditCaptain"; enemySource = "enemy_banditCaptain"; }
            enemy = new Entity();
            if (player.level < 30)
            {
                enemylvl = random.Next(player.level - 1, player.level + 2);
            }
            else enemylvl = 30;
            GenerateEnemy(enemyName, enemySource, enemylvl);

            int gold = random.Next(enemylvl * 50, enemylvl * 80) + 500;
            int exp = (int)Math.Ceiling((float)enemylvl / 2.8f); //to balance out
            int itemReward = 2;
            int itemChance = 75;
            Fight(player, enemy, gold, exp, itemChance, itemReward);
        }
        #endregion
        #region Desert expedition
        void Desert_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "desert"; //DOPISAĆ DO ExpeditionResume() ALBO NIE BĘDZIE DZIAŁAĆ!!!!!
                ExpeditionStart(button, 60);
            }
        }

        void DesertExpedition()
        {
            int enemylvl, roll;
            string enemyName = "", enemySource = "";
            roll = random.Next(1, 4);
            if (roll == 1) { enemyName = "enemyNameScorpion"; enemySource = "enemy_scorpion"; }
            else if (roll == 2) { enemyName = "enemyNameDesertBandit"; enemySource = "enemy_desertBandit"; }
            else if (roll == 3) { enemyName = "enemyNameDesertChief"; enemySource = "enemy_desertChieftain"; }
            enemy = new Entity();
            enemylvl = random.Next(player.level-1, player.level + 2);

            GenerateEnemy(enemyName, enemySource, enemylvl);

            int gold = random.Next(enemylvl * 60, enemylvl * 100) + 600;
            int exp = (int)Math.Ceiling((float)enemylvl / 2.8f); //to balance out
            int itemReward = 1;
            int itemChance = 90;
            Fight(player, enemy, gold, exp, itemChance, itemReward);
        }
        #endregion


        #endregion
        #endregion

        private void expeditionInfo_Clicked(object sender, EventArgs e)
        {
            showExpeditionInfo();
        }
        async void showExpeditionInfo()
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["infoText"], localizedStrings["informationsExpeditions"], localizedStrings["okText"]);

                clicked = false;
            }
        }
    }
}
