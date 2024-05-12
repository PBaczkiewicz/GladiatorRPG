using Java.Lang.Annotation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using static Android.Resource;
using static GladiatorRPG.Option;
using Button = Xamarin.Forms.Button;
using Color = Xamarin.Forms.Color;

namespace GladiatorRPG
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DungeonPage : ContentPage
    {
        //Ustawienia strony i informacje dla gracza
        #region Settings & info
        async void apInfo_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert(localizedStrings["cautionText"], localizedStrings["apInfo"], localizedStrings["okText"]);
        }

        //Wyłączenie przycisku cofnięcia
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        #endregion
        //Zmienne strony
        #region Variables
        public Dictionary<string, string> localizedStrings;
        public int dungeon;
        public int roll;
        public MainTabPage mainPage;
        public Label text;
        Player player;
        public Random random;
        public Expedition expedition;
        List<Button> buttonList;
        public GoldChange gc;
        public ExpChange ec;
        public ItemReward ir;
        public HpChange hc;
        public ApChange ac;
        public EventFight ef;
        public Entity enemy;
        public bool forceExit = false;
        //public Position playerPosition;
        public Label[,] map;
        List<Label> layout;
        bool dungeonEnter = false;
        float testDifficulty = 3; //soft cap of stat is a 2.75

        #endregion
        public DungeonPage(Dictionary<string, string> _localizedStrings, Player _player, int _dungeon, MainTabPage _mainPage)
        {

            InitializeComponent();
            localizedStrings = _localizedStrings;
            NavigationPage.SetHasNavigationBar(this, false);
            #region InitiateVariables
            player = _player;

            buttonList = new List<Button>();
            layout = new List<Label>();
            buttonList.Add(option1);
            buttonList.Add(option2);
            buttonList.Add(option3);
            buttonList.Add(option4);
            foreach (Button x in buttonList) { x.IsVisible = false; }
            health.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth.ToString();
            actions.Text = player.CurrentStamina.ToString();
            text = infoText;
            mainPage = _mainPage;
            dungeon = _dungeon;
            random = new Random();
            GenerateMap();

            #endregion
            if (player.inDungeon == true)
            {
                expedition = new Expedition();
                expedition = player.dungeonExpedition;
                LoadExpedition();
            }
            else
            {
                expedition = new Expedition();
                StartNewExpedition();
                ShowEventInterface();
                if (player.dungeonNotification) mainPage.SendNotification(localizedStrings["notificationDungeonCooldownFinished"], localizedStrings["notificationDungeonCooldownFinished1"], 4, player.dungeonCooldown);

            }

            health.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth;
            actions.Text = player.CurrentStamina.ToString();

            option4.IsVisible = true;
            option4.Text = localizedStrings["dungeonLeave"];
            continueOption.Text = localizedStrings["continueText"];
            forceExit = false;
            player.inDungeon = true;
            player.dungeonExpedition = expedition;
            map[expedition.playerPosition.x, expedition.playerPosition.y].BackgroundColor = Color.White;


        }

        void GenerateMap()
        {
            map = new Label[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string labelName = "map" + i.ToString("") + j.ToString("");
                    map[i, j] = (Label)FindByName(labelName);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    //map[i, j].Text = i.ToString() + j.ToString();
                    map[i, j].BackgroundColor = Color.Transparent;
                    map[i, j].TextColor = Color.White;
                }
            }
        }


        #region Movement
        private void moveUp_Clicked(object sender, EventArgs e)
        {
            Movement(1);
        }
        private void moveDown_Clicked(object sender, EventArgs e)
        {
            Movement(2);
        }
        private void moveLeft_Clicked(object sender, EventArgs e)
        {
            Movement(3);
        }
        private void moveRight_Clicked(object sender, EventArgs e)
        {
            Movement(4);
        }
        private void moveRest_Clicked(object sender, EventArgs e)
        {
            Movement(0);
        }
        private void moveLeave_Clicked(object sender, EventArgs e)
        {
            LeaveDungeon();
        }
        bool moving = false;
        void Movement(int direction)
        {
            if (!moving)
            {
                moving = true;
                if (player.CurrentHealth <= 0)
                {
                    NoHpLeft();
                    return;
                }
                map[expedition.playerPosition.x, expedition.playerPosition.y].BackgroundColor = Color.Gray;
                map[expedition.playerPosition.x, expedition.playerPosition.y].Text = "";
                switch (direction)
                {
                    case 0:
                        player.CurrentHealth += player.regenerationHP;
                        health.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth;
                        break;
                    case 1:
                        expedition.playerPosition.y--;
                        break;
                    case 2:
                        expedition.playerPosition.y++;
                        break;
                    case 3:
                        expedition.playerPosition.x--;
                        break;
                    case 4:
                        expedition.playerPosition.x++;
                        break;
                }
                player.CurrentStamina -= expedition.movementCost;
                actions.Text = player.CurrentStamina.ToString();

                map[expedition.playerPosition.x, expedition.playerPosition.y].Text = "X";
                map[expedition.playerPosition.x, expedition.playerPosition.y].TextColor = Color.Black;
                CheckMovement();
                map[expedition.playerPosition.x, expedition.playerPosition.y].BackgroundColor = Color.White;

                foreach (Event x in expedition.eventList)
                {
                    if (expedition.playerPosition.x == x.pos.x && expedition.playerPosition.y == x.pos.y)
                    {
                        ShowEvent(x);
                    }
                }
                moving = false;
            }

        }
        void CheckMovement()
        {
            if (expedition.playerPosition.x == 0 || !(layout.Contains(map[expedition.playerPosition.x - 1, expedition.playerPosition.y]))) moveLeft.IsEnabled = false; else moveLeft.IsEnabled = true;
            if (expedition.playerPosition.x == 9 || !(layout.Contains(map[expedition.playerPosition.x + 1, expedition.playerPosition.y]))) moveRight.IsEnabled = false; else moveRight.IsEnabled = true;
            if (expedition.playerPosition.y == 0 || !(layout.Contains(map[expedition.playerPosition.x, expedition.playerPosition.y - 1]))) moveUp.IsEnabled = false; else moveUp.IsEnabled = true;
            if (expedition.playerPosition.y == 9 || !(layout.Contains(map[expedition.playerPosition.x, expedition.playerPosition.y + 1]))) moveDown.IsEnabled = false; else moveDown.IsEnabled = true;
            CheckSurroundings();
        }
        void DisableMovement()
        {
            moveUp.IsEnabled = false;
            moveDown.IsEnabled = false;
            moveLeft.IsEnabled = false;
            moveRight.IsEnabled = false;
            moveRest.IsEnabled = false;
            forceExit = true;
            mapLogs.IsVisible = true;
            mapLogs.Text = localizedStrings["dungeonNoStaminaLeft"];
        }
        //Checks if event is nearby in 1 square radius
        void CheckSurroundings()
        {
            if (player.CurrentStamina <= 0) { DisableMovement(); }
            int x = expedition.playerPosition.x;
            int y = expedition.playerPosition.y;

            foreach (Event z in expedition.eventList)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (x + i == z.pos.x && y + j == z.pos.y)
                        {
                            Color color = Color.Gray;
                            string icon = "";
                            switch (z.eventCategory)
                            {
                                case EventCategory.None:
                                    color = Color.Gray;
                                    break;
                                case EventCategory.Fight:
                                    color = Color.Red;
                                    icon = localizedStrings["fightIcon"];
                                    break;
                                case EventCategory.MiniBoss:
                                    color = Color.DarkRed;
                                    icon = localizedStrings["miniBossIcon"];
                                    break;
                                case EventCategory.Boss:
                                    color = Color.DarkRed;
                                    icon = localizedStrings["bossIcon"];
                                    break;
                                case EventCategory.RandomEvent:
                                    color = Color.Cyan;
                                    icon = localizedStrings["randomEventIcon"];
                                    break;
                                case EventCategory.Treasure:
                                    color = Color.Gold;
                                    icon = localizedStrings["goldIcon"];
                                    break;
                                case EventCategory.Trap:
                                    color = Color.DarkViolet;
                                    icon = localizedStrings["trapIcon"];
                                    break;
                            }
                            map[z.pos.x, z.pos.y].BackgroundColor = color;
                            if (icon != "") map[z.pos.x, z.pos.y].Text = icon;
                            map[x, y].BackgroundColor = Color.White;
                        }

                    }
                }
            }
        }

        #endregion



        //Rozpoczęcie/wczytanie dungeonu
        #region Starting/loading expedition
        void Begin_clicked(object sender, EventArgs e)
        {
            begin.IsVisible = false;
            dungeonEnter = true;
            ShowDungeonMap();


            //RollEvent();
            //NextEvent();
        }

        //DO PRZEROBIENIA
        void LoadExpedition()
        {

            dungeonTitle.Text = expedition.dungeonTitle;
            if (expedition.currentEvent != null)
            {
                infoText.Text = expedition.currentEvent.eventText;
                //NextEvent();
            }
        }
        void StartNewExpedition()
        {
            begin.IsVisible = true;
            player.dungeonExpedition = new Expedition();
            expedition = player.dungeonExpedition;
            switch (dungeon)
            {
                case 1:
                    GranaryDungeon();
                    break;
                case 2:
                    ForestDungeon();
                    break;
                case 3:
                    MountainsDungeon();
                    break;
                case 4:
                    BanditCampDungeon();
                    break;
                case 5:
                    DesertDungeon();
                    break;

            }
            CheckMovement();
            CheckSurroundings();
            RowDefinitionCollection rowDefinitions = TextGrid.RowDefinitions;
            rowDefinitions[0].Height = GridLength.Auto;
            begin.Text = localizedStrings["embarkText"];
            Grid.SetRow(option4, 1);
            option4.IsVisible = true;
            option4.Text = localizedStrings["dungeonLeave"];
        }
        public async void Fight(Option o, Player player, Entity enemy, int gold, int exp, int itemChance = 0, int items = 0, int itemRarity = 0, int rounds = 30)
        //Wymagane player, enemy. Reszta opcjonalna gold, exp -> złoto i exp dla gracza,
        //itemChance-> szansa na wylosowanie przedmiotu
        //items -> ilość losowań
        //itemRarity -> rzadkość przedmiotu, 0=losowa, 2-6=określona, inne=biały
        //rounds -> maksymalna liczba rund
        {
            FightPage fightPage = new FightPage(localizedStrings, mainPage, player, enemy, exp, gold, itemChance, items, itemRarity, rounds);
            await Navigation.PushAsync(fightPage);
            await fightPage.WaitForDismissal();
            if (player.fightWon) { continueText.Text = localizedStrings[o.fightWonText]; if (o.fightItemReward != null) player.itemsToRecieve.Enqueue(o.fightItemReward); }
            else { continueText.Text = localizedStrings[o.fightLostText]; continueOption.IsVisible = false; option4.IsVisible = true; forceExit = true; Grid.SetRow(option4, Grid.GetRow(continueOption)); }
            mainPage.UpdateItemsToRecieve();
            continueOption.IsEnabled = true;
        }



        #endregion
        //Losowanie zdarzeń
        #region Event setting
        void ShowEvent(Event ev)
        {
            expedition.currentEvent = ev;
            ShowEventInterface();
            LoadOptions();
        }

        void ShowEventInterface()
        {
            MapGrid.IsVisible = false;
            TextGrid.IsVisible = true;
        }
        void ShowDungeonMap()
        {
            TextGrid.IsVisible = false;
            MapGrid.IsVisible = true;
        }
        //Not used after rework
        void NextEvent()
        {
            continueText.IsVisible = false;
            //if (expedition.eventList.Count() < 1) { LeaveDungeon(); return; }

            eventText.IsVisible = false;
            rewardText.IsVisible = false;
            continueOption.IsVisible = false;
            infoText.Text = localizedStrings[expedition.currentEvent.eventText];
            option1.Text = localizedStrings[expedition.currentEvent.option1.buttonText];
            if (expedition.currentEvent.option2 != null) option2.Text = localizedStrings[expedition.currentEvent.option2.buttonText];
            if (expedition.currentEvent.option3 != null) option3.Text = localizedStrings[expedition.currentEvent.option3.buttonText];
            LoadOptions();

        }
        void LoadOptions()
        {
            health.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth;
            actions.Text = player.CurrentStamina.ToString();
            infoText.Text = localizedStrings[expedition.currentEvent.eventText];
            RowDefinitionCollection rowDefinitions = TextGrid.RowDefinitions;
            rowDefinitions[0].Height = GridLength.Auto;
            ShowButtons();
            if (expedition.currentEvent.option1.buttonText != null)
            {
                Grid.SetRow(option1, 0);
                option1.IsVisible = true; if (player.CurrentStamina >= expedition.currentEvent.option1.apCost) { option1.IsEnabled = true; } else { option1.IsEnabled = false; }
                option1.Text = localizedStrings[expedition.currentEvent.option1.buttonText];
                if (expedition.currentEvent.option1.IsTest && expedition.currentEvent.option1.Attribute != StatTest.nothing) option1.Text += " [" + CalculateMaxStat(expedition.currentEvent.option1.Attribute).ToString() + "%]";
                if (expedition.currentEvent.option1.apCost > 0)
                {
                    option1.Text += "\n" + expedition.currentEvent.option1.apCost.ToString() + " " + localizedStrings["dungeonApText"];
                    if (expedition.currentEvent.option1.apCost > player.CurrentStamina)
                    {
                        option1.IsEnabled = false;
                    }
                }
            }
            else { option1.IsVisible = false; }

            if (expedition.currentEvent.option2 != null)
            {
                Grid.SetRow(option2, 1);
                option2.IsVisible = true; if (player.CurrentStamina >= expedition.currentEvent.option2.apCost) { option2.IsEnabled = true; } else { option2.IsEnabled = false; }
                option2.Text = localizedStrings[expedition.currentEvent.option2.buttonText];
                if (expedition.currentEvent.option2.IsTest && expedition.currentEvent.option2.Attribute != StatTest.nothing) option2.Text += " [" + CalculateMaxStat(expedition.currentEvent.option2.Attribute).ToString() + "%]";

                if (expedition.currentEvent.option2.apCost > 0)
                {
                    option2.Text += "\n" + expedition.currentEvent.option2.apCost.ToString() + " " + localizedStrings["dungeonApText"];
                    if (expedition.currentEvent.option2.apCost > player.CurrentStamina)
                    {
                        option2.IsEnabled = false;
                    }
                }

            }
            else { option2.IsVisible = false; }

            if (expedition.currentEvent.option3 != null)
            {
                Grid.SetRow(option3, 2);
                option3.IsVisible = true; if (player.CurrentStamina >= expedition.currentEvent.option3.apCost) { option3.IsEnabled = true; } else { option3.IsEnabled = false; }
                option3.Text = localizedStrings[expedition.currentEvent.option3.buttonText];
                if (expedition.currentEvent.option3.IsTest && expedition.currentEvent.option3.Attribute != StatTest.nothing) option3.Text += " [" + CalculateMaxStat(expedition.currentEvent.option3.Attribute).ToString() + "%]";

                if (expedition.currentEvent.option3.apCost > 0)
                {
                    option3.Text += "\n" + expedition.currentEvent.option3.apCost.ToString() + " " + localizedStrings["dungeonApText"];
                    if (expedition.currentEvent.option3.apCost > player.CurrentStamina)
                    {
                        option3.IsEnabled = false;
                    }
                }

            }
            else { option3.IsVisible = false; }

            if (option2.IsVisible == false) { Grid.SetRow(option4, 1); }
            else if (option3.IsVisible == false) { Grid.SetRow(option4, 2); }
            else { Grid.SetRow(option4, 3); }
            option4.IsVisible = true;//DODANE 28.12.2023 (NIE POKAZYWAŁO SIĘ W DUNGEONIE <SOFT LOCK BEZ AKCJI>)


        }

        void ResolveEvent(Option o, Button button, int minValue = 0)
        {
            player.CurrentStamina -= o.apCost;
            eventText.Text = "";
            rewardText.Text = "";
            int statValue;
            if (o.IsTest)
            {

                switch (o.Attribute)
                {
                    case StatTest.strength:
                        statValue = player.baseStrength;
                        break;
                    case StatTest.perception:
                        statValue = player.basePerception;
                        break;
                    case StatTest.dexterity:
                        statValue = player.baseDexterity;
                        break;
                    case StatTest.agility:
                        statValue = player.baseAgility;
                        break;
                    case StatTest.vitality:
                        statValue = player.baseVitality;
                        break;
                    case StatTest.endurance:
                        statValue = player.baseEndurance;
                        break;
                    case StatTest.charisma:
                        statValue = player.baseCharisma;
                        break;
                    case StatTest.intelligence:
                        statValue = player.baseIntelligence;
                        break;
                    case StatTest.nothing:
                        statValue = o.nothingChance;
                        break;
                    default:
                        return;
                }

                roll = random.Next(0, (int)Math.Floor(((DungeonModifier()) * testDifficulty) + 8));
                if (roll < statValue)
                {
                    eventText.Text = localizedStrings[o.successText];
                    continueText.Text = localizedStrings[o.afterSuccessText];
                    foreach (Operation x in o.successList)
                    {
                        if (x.type == opType.EventFight)//(x is EventFight)
                        {
                            continueOption.IsEnabled = false;
                            player.fightWon = false;
                            Fight(o, player, RollGenericEnemy(x), x.gold + (int)Math.Floor(x.modifier * DungeonModifier()), x.exp + (int)Math.Floor(x.expModifier * DungeonModifier()), 100, x.fightItemReward);
                            continue;
                        }
                        if (x.type == opType.GoldChange)//(x is GoldChange)
                        {
                            rewardText.Text += "\n+ " + ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier))).ToString() + " " + localizedStrings["goldText"];
                        }
                        if (x.type == opType.ExpChange)//(x is ExpChange)
                        {
                            int value;
                            if (DungeonModifier() < player.level)
                            {
                                float expNormalization = 1;
                                for (int i = player.level - DungeonModifier(); i > 0; i--)
                                {
                                    expNormalization -= 0.2f;
                                    if (expNormalization <= 0)
                                    {
                                        expNormalization = 0;
                                        break;
                                    }
                                }
                                value = (int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier) * expNormalization);
                            }
                            else value = ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier)));
                            rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["expText"];
                        }
                        if (x.type == opType.ItemReward)//(x is ItemReward)
                        {
                            rewardText.Text += "\n+ " + localizedStrings[x.item.itemName];
                        }
                        if (x.type == opType.HpChange)//(x is HpChange)
                        {
                            int value = ((int)Math.Ceiling(((float)x.baseValue / 100) * player.maxHealth));
                            if (value > 0) rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["lifeText"];
                            if (value < 0) rewardText.Text += "\n" + value.ToString() + " " + localizedStrings["lifeText"];

                        }
                        if (x.type == opType.ApChange)
                        {
                            if (x.baseValue > 0) rewardText.Text += "\n+" + x.baseValue + " " + localizedStrings["stamText"];
                            else if (x.baseValue < 0) rewardText.Text += "\n" + x.baseValue + " " + localizedStrings["stamText"];
                        }
                        x.Execute(player, DungeonModifier());
                    }
                }
                else
                {
                    eventText.Text = localizedStrings[o.failText];
                    continueText.Text = localizedStrings[o.afterFailText];
                    foreach (Operation x in o.failList)
                    {
                        if (x.type == opType.EventFight)//(x is EventFight)
                        {
                            continueOption.IsEnabled = false;
                            player.fightWon = false;
                            Fight(o, player, RollGenericEnemy(x), x.gold + (int)Math.Floor(x.modifier * DungeonModifier()), x.exp + (int)Math.Floor(x.expModifier * DungeonModifier()), 100, x.fightItemReward);
                            continue;
                        }
                        if (x.type == opType.GoldChange)//(x is GoldChange)
                        {
                            rewardText.Text += "\n+ " + ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier))).ToString() + " " + localizedStrings["goldText"];
                        }
                        if (x.type == opType.ExpChange)//(x is ExpChange)
                        {
                            int value;
                            if (DungeonModifier() < player.level)
                            {
                                float expNormalization = 1;
                                for (int i = player.level - DungeonModifier(); i > 0; i--)
                                {
                                    expNormalization -= 0.2f;
                                    if (expNormalization <= 0)
                                    {
                                        expNormalization = 0;
                                        break;
                                    }
                                }
                                value = (int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier) * expNormalization);
                            }
                            else value = ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier)));
                            value = ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier)));
                            rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["expText"];
                        }
                        if (x.type == opType.ItemReward)//(x is ItemReward)
                        {
                            rewardText.Text += "\n+ " + localizedStrings[x.item.itemName];
                        }
                        if (x.type == opType.HpChange)//(x is HpChange)
                        {
                            int value = ((int)Math.Ceiling(((float)x.baseValue / 100) * player.maxHealth));
                            if (value > 0) rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["lifeText"];
                            if (value < 0) rewardText.Text += "\n" + value.ToString() + " " + localizedStrings["lifeText"];

                        }
                        if (x.type == opType.ApChange)
                        {
                            if (x.baseValue > 0) rewardText.Text += "\n+" + x.baseValue + " " + localizedStrings["stamText"];
                            else if (x.baseValue < 0) rewardText.Text += "\n" + x.baseValue + " " + localizedStrings["stamText"];
                        }
                        x.Execute(player, DungeonModifier());
                    }
                }

            }
            else
            {
                eventText.Text = localizedStrings[o.successText];
                continueText.Text = localizedStrings[o.afterSuccessText];
                foreach (Operation x in o.successList)
                {
                    if (x.type == opType.EventFight)//(x is EventFight)
                    {
                        try
                        {
                            if (expedition.currentEvent == expedition.boss)
                            {
                                player.winStreak = 0;
                            }
                        }
                        catch { }
                        continueOption.IsEnabled = false;
                        player.fightWon = false;
                        Fight(o, player, RollGenericEnemy(x), x.gold + (int)Math.Floor(x.modifier * DungeonModifier()), x.exp + (int)Math.Floor(x.expModifier * DungeonModifier()), 100, x.fightItemReward);
                        continue;
                    }
                    if (x.type == opType.GoldChange)//(x is GoldChange)
                    {
                        rewardText.Text += "\n+ " + ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier))).ToString() + " " + localizedStrings["goldText"];
                    }
                    if (x.type == opType.ExpChange)//(x is ExpChange)
                    {
                        int value;
                        if (DungeonModifier() < player.level)
                        {
                            float expNormalization = 1;
                            for (int i = player.level - DungeonModifier(); i > 0; i--)
                            {
                                expNormalization -= 0.2f;
                                if (expNormalization <= 0)
                                {
                                    expNormalization = 0;
                                    break;
                                }
                            }
                            value = (int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier) * expNormalization);
                        }
                        else value = ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier)));
                        value = ((int)Math.Floor(x.baseValue + (DungeonModifier() * x.modifier)));
                        rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["expText"];
                    }
                    if (x.type == opType.ItemReward)//(x is ItemReward)
                    {
                        rewardText.Text += "\n+ " + localizedStrings[x.item.itemName];
                    }
                    if (x.type == opType.HpChange)//(x is HpChange)
                    {
                        int value = ((int)Math.Ceiling(((float)x.baseValue / 100) * player.maxHealth));
                        if (value > 0) rewardText.Text += "\n+ " + value.ToString() + " " + localizedStrings["lifeText"];
                        if (value < 0) rewardText.Text += "\n" + value.ToString() + " " + localizedStrings["lifeText"];

                    }
                    if (x.type == opType.ApChange)
                    {
                        if (x.baseValue > 0) rewardText.Text += "\n+" + x.baseValue + " " + localizedStrings["stamText"];
                        else if (x.baseValue < 0) rewardText.Text += "\n" + x.baseValue + " " + localizedStrings["stamText"];
                    }
                    x.Execute(player, DungeonModifier());
                }
            }
            //expedition.currentRoom = buffor;
            continueText.IsVisible = true;
            Grid.SetRow(continueText, Grid.GetRow(button) + 3);
            continueOption.IsVisible = true;
            Grid.SetRow(continueOption, Grid.GetRow(button) + 2);
            if (eventText.Text != "")
            {
                eventText.IsVisible = true;
                Grid.SetRow(eventText, Grid.GetRow(button) + 1);
            }
            if (rewardText.Text != "")
            {
                rewardText.IsVisible = true;
                Grid.SetRow(rewardText, Grid.GetRow(button) + 4);
            }
            //RollEvent();
            DeleteEvent(o);


            mainPage.SaveProgress();

        }

        async void NoHpLeft()
        {
            await DisplayAlert(localizedStrings["dungeonNoHpLeftTitle"], localizedStrings["dungeonNoHpLeftText"], localizedStrings["okText"]);
            forceExit = true;
            LeaveDungeon();
        }
        void DeleteEvent(Option o)
        {
            if (!o.JustContinue) expedition.eventList.Remove(expedition.currentEvent);
            //expedition.currentEvent.pos = null;
            //expedition.currentEvent = null;
            CheckSurroundings();
            CheckMovement();
        }
        #endregion
        //Przyciski opcji
        #region Option buttons
        void HideButtons()
        {
            for (int i = 1; i <= 3; i++)
            {
                buttonGrid.RowDefinitions[i].Height = 0;
            }


        }
        void ShowButtons()
        {
            for (int i = 0; i <= 3; i++)
            {
                buttonGrid.RowDefinitions[i].Height = GridLength.Auto;
            }
        }
        void option1_clicked(object sender, EventArgs e)
        {
            Option o = expedition.currentEvent.option1;
            foreach (Button x in buttonList)
            {
                x.IsVisible = false;
            }
            Grid.SetRow(option1, 0);
            option1.IsVisible = true;
            option1.IsEnabled = false;

            ResolveEvent(o, option1);
        }
        void option2_clicked(object sender, EventArgs e)
        {
            Option o = expedition.currentEvent.option2;
            foreach (Button x in buttonList)
            {
                x.IsVisible = false;
            }
            Grid.SetRow(option2, 0);

            option2.IsVisible = true;
            option2.IsEnabled = false;
            ResolveEvent(o, option2);
        }
        void option3_clicked(object sender, EventArgs e)
        {
            Option o = expedition.currentEvent.option3;
            foreach (Button x in buttonList)
            {
                x.IsVisible = false;
            }
            Grid.SetRow(option3, 0);

            option3.IsVisible = true;
            option3.IsEnabled = false;
            ResolveEvent(o, option3);
        }
        void ContinueOption_clicked(object sender, EventArgs e)
        {
            health.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth;
            actions.Text = player.CurrentStamina.ToString();
            if (expedition.currentEvent == expedition.boss)
            {
                //forceExit = true;
                if (player.dungeonBossProgress < dungeon + 1)
                {
                    player.dungeonBossProgress = dungeon + 1;
                }
                DungeonReward();


            }
            else ShowDungeonMap();
            continueText.IsVisible = false;
            continueOption.IsVisible = false;
            eventText.IsVisible = false;
            rewardText.IsVisible = false;
        }

        void option4_clicked(object sender, EventArgs e)
        {
            LeaveDungeon();
        }
        #endregion
        //Wyjście z dungeonu, obliczanie max. statystyk, losowanie statystyk i przeciwników
        #region Other
        async void LeaveDungeon()
        {
            mainPage.ShowDungeons();
            mainPage.UpdateItemsToRecieve();
            bool result = false;
            if (!forceExit)
            {
                result = await DisplayAlert(localizedStrings["cautionText"], localizedStrings["dungeonLeaveMessage"], localizedStrings["yesText"], localizedStrings["noText"]);
            }
            if (result || forceExit)
            {

                player.dungeonExpedition = null;
                player.inDungeon = false;
                if (dungeonEnter == false)
                {
                    player.dungeonCooldown = DateTime.Now;
                    mainPage.DeleteNotifications(4);
                }
                await Navigation.PopAsync();
            }
            else
            {

                return;
            }
        }
        public int DungeonModifier()
        {
            int x;
            if (player.level < expedition.maxLevel)
            {
                x = player.level;
            }
            else
            {
                x = expedition.maxLevel;
            }


            return x;
        }
        public int CalculateMaxStat(StatTest stat)
        {
            int value = 0;
            switch (stat)
            {
                case StatTest.strength:
                    value = player.baseStrength;
                    break;
                case StatTest.perception:
                    value = player.basePerception;
                    break;
                case StatTest.dexterity:
                    value = player.baseDexterity;
                    break;
                case StatTest.agility:
                    value = player.baseAgility;
                    break;
                case StatTest.vitality:
                    value = player.baseVitality;
                    break;
                case StatTest.endurance:
                    value = player.baseEndurance;
                    break;
                case StatTest.charisma:
                    value = player.baseCharisma;
                    break;
                case StatTest.intelligence:
                    value = player.baseIntelligence;
                    break;
            }
            if (100 < (int)Math.Floor((value / (((DungeonModifier()) * testDifficulty) + 8)) * 100))
                return 100;
            else
            {
                return (int)Math.Floor((value / (((DungeonModifier()) * testDifficulty) + 8)) * 100);
            }
        }
        #endregion
        #region Enemy generation
        //Generuje przeciwnika z procentowymi statystykami zawartymi w argumentach
        public Entity RollGenericEnemy(Operation o)
        {
            Entity rolledEnemy;
            if (o.enemyStats.Count > 0)
            {
                rolledEnemy = mainPage.EnemyGeneration(o.enemyName, o.enemySource, DungeonModifier() + o.levelModifier, o.enemyItems, o.enemyStats.Dequeue(),
                    o.enemyStats.Dequeue(), o.enemyStats.Dequeue(), o.enemyStats.Dequeue(), o.enemyStats.Dequeue(), o.enemyStats.Dequeue(),
                    o.enemyStats.Dequeue(), o.enemyStats.Dequeue());
                return rolledEnemy;
            }
            else
            {
                rolledEnemy = mainPage.EnemyGeneration(o.enemyName, o.enemySource, DungeonModifier() + o.levelModifier, o.enemyItems,
                   random.Next(o.baseValue, o.maxValue), random.Next(o.baseValue, o.maxValue), random.Next(o.baseValue, o.maxValue),
                   random.Next(o.baseValue, o.maxValue), random.Next(o.baseValue, o.maxValue), random.Next(o.baseValue, o.maxValue),
                   random.Next(o.baseValue, o.maxValue), random.Next(o.baseValue, o.maxValue));
                //rolledEnemy.name = o.enemyName; rolledEnemy.source = o.enemySource;
                return rolledEnemy;
            }
        }

        #endregion
        //Inicjowanie zdarzeń dungeonów
        #region Dungeons
        void AddHpChange(int hp, List<Operation> list)
        {
            list.Add(new HpChange(hp));
        }
        void AddGoldReward(int gold, int modifier, List<Operation> list)
        {
            list.Add(new GoldChange(gold, modifier));
        }
        void AddExpReward(int exp, float modifier, List<Operation> list)
        {
            list.Add(new ExpChange(exp, modifier));
        }
        void AddApReward(int ap, List<Operation> list)
        {
            list.Add(new ApChange(ap));
        }
        void AddItemReward(int minLevel, int maxLevel, List<Operation> list)

        {
            list.Add(new ItemReward(mainPage.ItemGenerator(minLevel, maxLevel)));
        }
        void AddFightItemReward(Item item, Option o)
        {
            o.fightItemReward = item;
        }
        /// <summary>
        /// Creates a fight with random stats between minStat and maxStat
        /// </summary>
        void AddEventFight(int minStat, int maxStat, int items, string enemyName, string enemySource, int gold, int exp, List<Operation> list, float goldModifier = 0, float expModifier = 0, int itemRewards = 0, int levelModifier = 0)
        {
            list.Add(new EventFight(minStat, maxStat, items, enemyName, enemySource, gold, exp, goldModifier, expModifier, itemRewards, levelModifier));
        }
        /// <summary>
        /// Creates a fight with set stats using all of the arguments
        /// </summary>
        void AddSpecificFight(int str, int per, int dex, int agi, int end, int vit, int cha, int wis,
            int items, string enemyName, string enemySource, int gold, int exp, List<Operation> list, float goldModifier = 0,
            float expModifier = 0, int itemRewards = 0, int levelModifier = 0)
        {
            list.Add(new EventFight(str, per, dex, agi, end, vit, cha, wis, items, enemyName, enemySource, gold, exp, goldModifier, expModifier, itemRewards, levelModifier));
        }
        void PopulateEvents(List<Label> miniBoss, List<Label> mainBoss)
        {
            List<Event> normalEvents = new List<Event>();
            List<Label> emptyGrids = new List<Label>();
            foreach (Label lb in layout) { emptyGrids.Add(lb); }
            foreach (Event ev in expedition.eventList)
            {
                normalEvents.Add(ev);
            }
            foreach (Label label in layout)
            {
                //label.Text="X";
                //label.BackgroundColor = Color.Black;
                label.BackgroundColor = Color.FromHex("#FF000000");
            }
            foreach (Label label in miniBoss)
            {
                label.Scale = 1;
            }
            foreach (Label label in mainBoss)
            {
                label.Scale = 1.25;
            }
            int randomSquare = random.Next(0, mainBoss.Count());
            expedition.boss.SetPosition(Grid.GetColumn(mainBoss[randomSquare]), Grid.GetRow(mainBoss[randomSquare]));
            emptyGrids.Remove(mainBoss[randomSquare]);
            if (miniBoss != null)
            {
                expedition.miniBoss.SetPosition(Grid.GetColumn(miniBoss[randomSquare]), Grid.GetRow(miniBoss[randomSquare]));
                emptyGrids.Remove(miniBoss[randomSquare]);
            }
            expedition.eventList.Add(expedition.boss);
            if (miniBoss != null) expedition.eventList.Add(expedition.miniBoss);

            foreach (Event ev in normalEvents)
            {
                if (emptyGrids.Count < 1) break;
                randomSquare = random.Next(0, emptyGrids.Count);
                ev.pos.SetPosition(Grid.GetColumn(emptyGrids[randomSquare]), Grid.GetRow(emptyGrids[randomSquare]));
                emptyGrids.Remove(emptyGrids[randomSquare]);

            }
        }
        Item item1;
        Item item2;
        Item item3;
        //private void reward1_Clicked(object sender, EventArgs e)
        //{
        //    player.itemsToRecieve.Enqueue(item1);
        //    forceExit = true;
        //    LeaveDungeon();
        //}

        //private void reward2_Clicked(object sender, EventArgs e)
        //{
        //    player.itemsToRecieve.Enqueue(item2);
        //    forceExit = true;
        //    LeaveDungeon();

        //}

        //private void reward3_Clicked(object sender, EventArgs e)
        //{
        //    player.itemsToRecieve.Enqueue(item3);
        //    forceExit = true;
        //    LeaveDungeon();

        //}

        bool activeDialogBox = false;
        bool clicked = false;
        async void reward_Clicked(object sender, EventArgs e)
        {
            if (activeDialogBox == false)
            {
                activeDialogBox = true;
                int itemNumber;

                if (sender is Button button)
                {
                    itemNumber = int.Parse(button.AutomationId);
                    Item currentItem = null;
                    Item equippedItem = null;
                    switch (itemNumber)
                    {
                        case 1:
                            currentItem = item1;
                            break;
                        case 2:
                            currentItem = item2;
                            break;
                        case 3:
                            currentItem = item3;
                            break;
                    }
                    if (currentItem.itemClass == ItemClass.Weapon && player.weapon != null) { equippedItem = player.weapon; }
                    if (currentItem.itemClass == ItemClass.Helmet && player.head != null) { equippedItem = player.head; }
                    if (currentItem.itemClass == ItemClass.Torso && player.torso != null) { equippedItem = player.torso; }
                    if (currentItem.itemClass == ItemClass.Shield && player.shield != null) { equippedItem = player.shield; }
                    if (currentItem.itemClass == ItemClass.Boots && player.boots != null) { equippedItem = player.boots; }
                    if (currentItem.itemClass == ItemClass.Gloves && player.gloves != null) { equippedItem = player.gloves; }
                    if (currentItem.itemClass == ItemClass.Necklace && player.necklace != null) { equippedItem = player.necklace; }
                    if (currentItem.itemClass == ItemClass.Belt && player.belt != null) { equippedItem = player.belt; }
                    if (currentItem.itemClass == ItemClass.Ring && player.ring != null) { equippedItem = player.ring; }
                    string itemDescription = SetDescription(currentItem);

                    if (equippedItem != null)
                    {
                        var action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet(
                                    localizedStrings["actionItemText"],
                                    localizedStrings["actionTakeText"],
                                    localizedStrings["cancelText"],
                                    null,
                                    itemDescription + "\n_______________\n" + localizedStrings["yourItemText"] + "\n" + equippedItem.itemDescription
                                );
                        if (action == localizedStrings["actionTakeText"])
                        {
                            if (clicked) return;
                            if (player.items.Count > 11) { await DisplayAlert(mainPage.cautionText, localizedStrings["noSpaceInInventoryText"], mainPage.okText); activeDialogBox = false; clicked = false; return; }
                            player.items.Add(currentItem);
                            clicked = true;
                            RewardGrid.IsVisible = false;
                            ShowDungeonMap();
                        }
                        else if (action == localizedStrings["cancelText"])
                        {
                            if (clicked) return;
                            clicked = true;
                        }
                    }
                    else
                    {
                        var action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet(
                                    localizedStrings["actionItemText"],
                                    localizedStrings["actionTakeText"],
                                    localizedStrings["cancelText"],
                                    null,
                                    itemDescription
                                );
                        if (action == localizedStrings["actionTakeText"])
                        {
                            if (clicked) return;
                            if (player.items.Count > 11) { await DisplayAlert(mainPage.cautionText, localizedStrings["noSpaceInInventoryText"], mainPage.okText); activeDialogBox = false; clicked = false; return; }
                            player.items.Add(currentItem);
                            clicked = true;
                            RewardGrid.IsVisible = false;
                            ShowDungeonMap();
                        }
                        else if (action == localizedStrings["cancelText"])
                        {
                            if (clicked) return;
                            clicked = true;
                        }
                    }
                }
                clicked = false;
                activeDialogBox = false;
                mainPage.GridEquipment();
                mainPage.ProcessEquippedItems();
            }
        }

        string SetDescription(Item item)
        {
            string itemDescription = localizedStrings[item.itemName];
            itemDescription += "\n" + localizedStrings["itemLevel"] + " " + item.levelReq;
            if (item.itemClass == ItemClass.Weapon) { itemDescription += "\n" + localizedStrings["itemWeapon"]; }
            else if (item.itemClass == ItemClass.Helmet) { itemDescription += "\n" + localizedStrings["itemHelmet"]; }
            else if (item.itemClass == ItemClass.Torso) { itemDescription += "\n" + localizedStrings["itemTorso"]; }
            else if (item.itemClass == ItemClass.Gloves) { itemDescription += "\n" + localizedStrings["itemGloves"]; }
            else if (item.itemClass == ItemClass.Boots) { itemDescription += "\n" + localizedStrings["itemBoots"]; }
            else if (item.itemClass == ItemClass.Shield) { itemDescription += "\n" + localizedStrings["itemShield"]; }
            else if (item.itemClass == ItemClass.Ring) { itemDescription += "\n" + localizedStrings["itemRing"]; }
            else if (item.itemClass == ItemClass.Belt) { itemDescription += "\n" + localizedStrings["itemBelt"]; }
            else if (item.itemClass == ItemClass.Necklace) { itemDescription += "\n" + localizedStrings["itemNecklace"]; }
            if (item.armorValue > 0) { itemDescription += "\n" + localizedStrings["itemArmor"] + " " + item.armorValue.ToString(); }

            if (item.minDamage > 0 || item.maxDamage > 0) { itemDescription += "\n" + localizedStrings["damText"] + " " + item.minDamage.ToString() + "-" + item.maxDamage.ToString(); }


            itemDescription += "\n???";



            return itemDescription;
        }
        void DungeonReward()
        {
            MapGrid.IsVisible = false;
            TextGrid.IsVisible = false;
            item1 = mainPage.ItemGenerator(DungeonModifier() - 1, DungeonModifier() + 2, -1);
            item2 = mainPage.ItemGenerator(DungeonModifier() - 1, DungeonModifier() + 2, -1);
            item3 = mainPage.ItemGenerator(DungeonModifier() - 1, DungeonModifier() + 2, -1);

            reward1.Text = "\n\n\n\n\n\n" + localizedStrings["itemLevel"] + " " + item1.levelReq;
            reward1.TextColor = item1.color;
            reward1Image.Source = item1.itemImage;
            ItemBorder(item1, reward1Border);

            reward2.Text = "\n\n\n\n\n\n" + localizedStrings["itemLevel"] + " " + item2.levelReq;
            reward2.TextColor = item2.color;
            reward2Image.Source = item2.itemImage;
            ItemBorder(item2, reward2Border);

            reward3.Text = "\n\n\n\n\n\n" + localizedStrings["itemLevel"] + " " + item3.levelReq;
            reward3.TextColor = item3.color;
            reward3Image.Source = item3.itemImage;
            ItemBorder(item3, reward3Border);

            RewardGrid.IsVisible = true;

        }

        void ItemBorder(Item item, Image image)
        {
            if (item.color.ToArgb() == System.Drawing.Color.White.ToArgb()) { image.Source = "borderWhite"; }
            else if (item.color.ToArgb() == System.Drawing.Color.LightGreen.ToArgb()) { image.Source = "borderGreen"; }
            else if (item.color.ToArgb() == System.Drawing.Color.DodgerBlue.ToArgb()) { image.Source = "borderBlue"; }
            else if (item.color.ToArgb() == System.Drawing.Color.DarkViolet.ToArgb()) { image.Source = "borderViolet"; }
            else if (item.color.ToArgb() == System.Drawing.Color.Orange.ToArgb()) { image.Source = "borderOrange"; }
            else if (item.color.ToArgb() == System.Drawing.Color.Red.ToArgb()) { image.Source = "borderRed"; }
            else { image.Source = "borderRed"; }
        }
        #endregion

        #region Granary dungeon
        void GranaryDungeon()
        {
            expedition.dungeonTitle = localizedStrings["granaryDungeonTitle"];
            expedition.movementCost = 1;
            dungeonTitle.Text = expedition.dungeonTitle;
            infoText.Text = localizedStrings["granaryDungeonEntryText"];
            GranaryEvents();
            player.dungeonCooldown = DateTime.Now.AddHours(2);
            mainPage.LoadDungeons();

        }
        void GranaryEvents()
        {
            expedition.maxLevel = 15;
            //4
            #region Treasures
            //otwórz(dex)3ap /otwórz(str)5ap /zostaw(3% na otwarcie)0ap
            #region Granary Closed Chest
            Event ev = new Event("granaryClosedChest");

            ev.option1 = new Option("granaryClosedChest1", 1);
            ev.option1.SetTest(StatTest.dexterity);
            AddGoldReward(50, 10, ev.option1.successList);
            AddExpReward(1, 0.1f, ev.option1.successList);
            roll = random.Next(0, 101);
            if (roll <= 20)
            {

                AddItemReward(player.level, player.level + 1, ev.option1.successList);
            }

            ev.option2 = new Option("granaryClosedChest2", 1);
            ev.option2.SetTest(StatTest.strength);
            AddGoldReward(50, 10, ev.option2.successList);
            AddExpReward(1, 0.1f, ev.option2.successList);
            roll = random.Next(0, 101);
            if (roll <= 20)
            {

                AddItemReward(player.level, player.level + 1, ev.option2.successList);
            }
            AddHpChange(-10, ev.option2.failList);

            ev.option3 = new Option("granaryClosedChest3", 0);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //gold 1ap /hp 1ap
            #region Damaged Crates
            ev = new Event("granaryDamagedCrates");

            ev.option1 = new Option("granaryDamagedCrates1", 1);
            AddGoldReward(50, 10, ev.option1.successList);

            ev.option2 = new Option("granaryDamagedCrates2", 1);
            AddHpChange(20, ev.option2.successList);

            ev.option3 = new Option("granaryDamagedCrates3", 0);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //szukaj aktualnych dokumentów(int)++gold /zrób ognisko z dokumentów +hp /spróbuj się czegoś dowiedzieć +exp
            #region Old documents
            ev = new Event("granaryOldDocuments");
            #region op1
            ev.option1 = new Option("granaryOldDocuments1", 1);
            ev.option1.SetTest(StatTest.intelligence);
            AddGoldReward(100, 20, ev.option1.successList);
            #endregion
            #region op2
            ev.option2 = new Option("granaryOldDocuments2", 1);
            AddHpChange(20, ev.option2.successList);
            #endregion
            #region op3
            ev.option3 = new Option("granaryOldDocuments3", 0);
            ev.option3.JustContinue = true;
            #endregion
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //wypij wywar +-hp (vit) 2ap / 2sprzedaj +gold
            #region Old brew
            ev = new Event("granaryOldBrew");

            ev.option1 = new Option("granaryOldBrew1", 1);
            ev.option1.SetTest(StatTest.vitality);
            AddHpChange(30, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.option2 = new Option("granaryOldBrew2", 1);
            AddGoldReward(60, 12, ev.option2.successList);

            ev.option3 = new Option("granaryOldBrew3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #endregion
            //5
            #region Enemies
            //walcz 1ap /odstrasz(dex) 3ap /omiń(agi) 3ap
            #region Rat pack
            ev = new Event("granaryRatPack");

            ev.option1 = new Option("granaryRatPack1", 0);
            AddEventFight(40, 55, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option1.successList, 5, 0.1f);

            ev.option2 = new Option("granaryRatPack2", 1);
            ev.option2.SetTest(StatTest.dexterity);
            AddExpReward(1, 0.1f, ev.option2.successList);
            AddEventFight(40, 55, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option2.failList, 5, 0.1f);

            ev.option3 = new Option("granaryRatPack3", 1);
            ev.option3.SetTest(StatTest.agility);
            AddExpReward(1, 0.1f, ev.option3.successList);
            AddEventFight(40, 55, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option3.failList, 5, 0.1f);

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //walka ze szczurem x3
            #region Rat Blockade
            {
                int repeat = 3;
                while (repeat > 0)
                {
                    ev = new Event("granaryRatBlockade");
                    ev.option1 = new Option("granaryRatBlockade1", 0);
                    AddEventFight(40, 60, 2, "enemyNameBigRat", "enemy_rat", 70, 4, ev.option1.successList, 12, 0.1f);

                    ev.eventCategory = EventCategory.Fight;

                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //walka 2ap / przechytrzenie 4ap / nic
            #region Escaped slave
            ev = new Event("granaryEscapedSlave");

            ev.option1 = new Option("granaryEscapedSlave1", 0);
            AddEventFight(40, 65, 2, "enemyNameEscapedSlave", "enemy_slave", 0, 3, ev.option1.successList, 0, 0.45f);

            ev.option2 = new Option("granaryEscapedSlave2", 1);
            ev.option2.SetTest(StatTest.charisma);
            AddExpReward(2, 0.1f, ev.option2.successList);
            AddHpChange(-20, ev.option2.failList);

            ev.option3 = new Option("granaryEscapedSlave3", 2); ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion

            #endregion
            //1
            #region Traps

            //szukaj mechanizmu uruchomienia pułapek (per)+exp /przedostań się uważając na pułapki (end) +exp
            #region Trapped corridor
            ev = new Event("granaryTrappedCorridor");

            ev.option1 = new Option("granaryTrappedCorridor1", 2);
            ev.option1.SetTest(StatTest.perception);
            AddExpReward(3, 0.1f, ev.option1.successList);
            AddHpChange(-30, ev.option1.failList);

            ev.option2 = new Option("granaryTrappedCorridor2", 1);
            ev.option2.SetTest(StatTest.endurance);
            AddExpReward(2, 0.1f, ev.option2.successList);
            AddHpChange(-5, ev.option2.successList);
            AddHpChange(-15, ev.option2.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #endregion

            #region Mini boss
            expedition.miniBoss = new Event("granaryHugeSpider");

            expedition.miniBoss.option1 = new Option("granaryHugeSpider1", 1);
            AddSpecificFight(random.Next(20, 40), random.Next(80, 120), random.Next(30, 40), random.Next(50, 70), random.Next(30, 40), random.Next(70, 80), random.Next(50, 60)
                , random.Next(60, 70), 9, "enemyNameSpiderBoss", "enemy_spiderboss", 200, 8, expedition.miniBoss.option1.successList, 20, 0.1f);
            AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);

            expedition.miniBoss.option2 = new Option("granaryHugeSpider2", 2); expedition.miniBoss.option2.JustContinue = true;

            expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
            #endregion

            #region Boss
            expedition.boss = new Event("granaryBoss");
            expedition.boss.option1 = new Option("granaryBoss1", 0);
            //AddEventFight(80, 120, 6, "enemyNameRatKing", "enemy_ratboss", 500, 10, expedition.boss.option1.successList, 0, 0, 0, 3);
            AddSpecificFight(random.Next(80, 120), random.Next(50, 70), random.Next(30, 50), random.Next(40, 60), random.Next(60, 90), random.Next(80, 100), random.Next(30, 50)
                , random.Next(30, 50), 9, "enemyNameRatKing", "enemy_ratboss", 500, 10, expedition.boss.option1.successList, 0, 0, 0);
            expedition.boss.eventCategory = EventCategory.Boss;
            #endregion
            GranaryMap();


        }

        void GranaryMap()
        {
            layout = new List<Label> { map11, map12, map13, map14, map24, map34, map44, map43, map53, map63, map64, map65, map55, map45, map52, map51, map41, map74, map84, map56, map57, map58, map67, map77, map15 };
            List<Label> miniBoss = new List<Label> { map15, map43, map63, map65, map45 };
            List<Label> mainBoss = new List<Label> { map58, map77, map84, map41 };
            expedition.playerPosition = new Position(1, 0);
            CheckMovement();
            PopulateEvents(miniBoss, mainBoss);

        }
        #endregion
        #region Forest dungeon
        void ForestDungeon()
        {
            expedition.dungeonTitle = localizedStrings["forestDungeonTitle"];
            dungeonTitle.Text = expedition.dungeonTitle;
            infoText.Text = localizedStrings["forestDungeonEntryText"];
            ForestEvents();
            expedition.movementCost = 1;
            player.dungeonCooldown = DateTime.Now.AddHours(4);

            mainPage.LoadDungeons();

        }
        void ForestEvents()
        {
            expedition.maxLevel = 25;
            Event ev;
            //3
            #region Treasure
            //perception test or rest
            #region Old ruins
            ev = new Event("forestRuins");
            ev.option1 = new Option("forestRuins1", 2);
            ev.option1.SetTest(StatTest.perception);
            AddGoldReward(0, 40, ev.option1.successList);
            int x = random.Next(0, 11);
            if (x < 3)
            {
                AddItemReward(DungeonModifier(), DungeonModifier() + 1, ev.option1.successList);
            }
            ev.option2 = new Option("forestRuins2", 0);
            AddHpChange(20, ev.option2.successList);
            AddApReward(4, ev.option2.successList);

            ev.option3 = new Option("forestRuins3", 0);
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //dex for health or loot
            #region Camp
            ev = new Event("forestCamp");
            ev.option1 = new Option("forestCamp1", 3);
            ev.option1.SetTest(StatTest.dexterity);
            AddHpChange(30, ev.option1.successList);
            AddHpChange(5, ev.option1.failList);
            ev.option2 = new Option("forestCamp2", 1);
            AddHpChange(10, ev.option2.successList);
            AddGoldReward(100, 30, ev.option2.successList);
            ev.option3 = new Option("forestCamp3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //str test
            #region Stagecoach
            ev = new Event("forestStagecoach");
            ev.option1 = new Option("forestStagecoach1", 4);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(50, 40, ev.option1.successList);
            ev.option2 = new Option("forestStagecoach2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //3
            #region Random events
            //endurance or intelligence test
            #region Cliffside
            ev = new Event("forestCliffside");
            ev.option1 = new Option("forestCliffside1", 3);
            ev.option1.SetTest(StatTest.endurance);
            AddHpChange(-25, ev.option1.failList);
            ev.option2 = new Option("forestCliffside2", 2);
            ev.option2.SetTest(StatTest.intelligence);
            AddHpChange(-25, ev.option2.failList);
            ev.option3 = new Option("forestCliffside3", 4);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vitality test
            #region River crossing
            ev = new Event("forestRiver");
            ev.option1 = new Option("forestRiver1", 2);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(1, 0.1f, ev.option1.successList);
            AddHpChange(-25, ev.option1.failList);
            ev.option2 = new Option("forestRiver2", 4);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #region Deer
            ev = new Event("forestDeer");
            ev.option1 = new Option("forestDeer1", 3);
            AddHpChange(20, ev.option1.successList);
            ev.option2 = new Option("forestDeer2", 2);
            AddExpReward(1, 0.15f, ev.option2.successList);
            ev.option3 = new Option("forestDeer3", 0);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //6
            #region Fights
            //Fight x2
            #region Bear
            {
                int repeat = 2;
                while (repeat > 0)
                {
                    ev = new Event("forestBear");
                    ev.option1 = new Option("forestBear1", 0);
                    AddSpecificFight(random.Next(70, 90), random.Next(40, 60), random.Next(50, 70), random.Next(20, 40), random.Next(40, 60), random.Next(90, 100),
                        random.Next(20, 30), random.Next(10, 15), 6, "enemyNameBear", "enemy_bear", 300, 5, ev.option1.successList, 15, 0.1f, 1);
                    ev.eventCategory = EventCategory.Fight;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }

            #endregion
            //Fight x3
            #region Wolf ambush
            {
                int repeat = 3;
                while (repeat > 0)
                {
                    ev = new Event("forestWolf");
                    ev.option1 = new Option("forestWolf1", 0);
                    AddSpecificFight(random.Next(30, 60), random.Next(50, 70), random.Next(60, 80), random.Next(70, 80), random.Next(30, 40), random.Next(30, 50),
                                        random.Next(30, 40), random.Next(50, 60), 6, "enemyNameWolf", "enemy_wolf", 250, 5, ev.option1.successList, 10, 0.08f);
                    ev.eventCategory = EventCategory.Fight;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //cha test or fight
            #region Bandit
            ev = new Event("forestBandit");
            ev.option1 = new Option("forestBandit1", 3);
            ev.option1.SetTest(StatTest.charisma);
            AddExpReward(3, 0.1f, ev.option1.successList);
            AddEventFight(40, 90, 9, "enemyNameBandit", "enemy_bandit", 200, 5, ev.option1.failList, 15, 0.75f);
            ev.option2 = new Option("forestBandit2", 0);
            AddEventFight(40, 90, 9, "enemyNameBandit", "enemy_bandit", 200, 5, ev.option2.successList, 15, 0.75f);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //2
            #region Traps
            #region Mutated boar
            {
                int repeat = 2;
                while (repeat > 0)
                {
                    ev = new Event("forestMutatedBoar");

                    ev.option1 = new Option("forestMutatedBoar1", 2);
                    AddEventFight(50, 70, 7, "enemyNameMutatedBoar", "enemy_boar", 200, 1, ev.option1.successList, 10, 0.25f);

                    ev.option2 = new Option("forestMutatedBoar2", 3);
                    ev.option2.SetTest(StatTest.agility);
                    AddExpReward(2, 0.1f, ev.option2.successList);
                    AddHpChange(-30, ev.option2.failList);
                    ev.eventCategory = EventCategory.Trap;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            #endregion
            #region Mini boss
            expedition.miniBoss = new Event("forestMiniBoss");
            expedition.miniBoss.option1 = new Option("forestMiniBoss1", 2);
            AddSpecificFight(random.Next(50, 70), random.Next(100, 110), random.Next(120, 130), random.Next(90, 100), random.Next(50, 70), random.Next(50, 80),
                random.Next(30, 60), random.Next(50, 70), 9, "enemyNameAlphaWolf", "enemy_wolfboss", 1000, 10, expedition.miniBoss.option1.successList, 20, 0.2f, 0, 0);
            AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
            expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
            #endregion
            #region Boss
            expedition.boss = new Event("forestBoss");
            expedition.boss.option1 = new Option("forestBoss1", 0);
            AddSpecificFight(70, 90, 110, 130, 40, 100, 50, 40, 9, "enemyNameGryphon", "enemy_gryphon", 1500, 15, expedition.boss.option1.successList, 0, 0, 0, 0);
            expedition.boss.eventCategory = EventCategory.Boss;
            #endregion
            ForestMap();
        }
        void ForestMap()
        {
            expedition.playerPosition = new Position(7, 9);
            layout = new List<Label> { map78, map77, map67, map87, map66, map86, map56, map96, map65, map75, map85, map64, map84, map54, map94, map63, map73, map83
            , map72, map71, map70, map61, map51, map41, map52, map42, map32, map22, map21, map20, map23, map24, map12, map02};
            List<Label> miniBoss = new List<Label> { map56, map96, map54, map94, map70 };
            List<Label> mainBoss = new List<Label> { map20, map02, map24 };
            CheckMovement();
            PopulateEvents(miniBoss, mainBoss);
        }
        #endregion
        #region Mountains dungeon
        void MountainsMap()
        {
            expedition.playerPosition = new Position(0, 8);
            layout = new List<Label> { map18, map28, map38, map48, map58, map57, map56, map66, map76, map86, map96, map95, map94, map93, map83, map73, map17, map16
                , map15, map14, map13, map12, map22, map32, map31, map41, map51, map61, map71, map72, map63, map53, map54, map44, map02, map40, map70, map92, map97
                , map74, map46, map34};
            List<Label> miniBoss = new List<Label> { map02, map40, map70, map92, map74, map97, map46 };
            List<Label> mainBoss = new List<Label> { map34 };
            CheckMovement();
            PopulateEvents(miniBoss, mainBoss);
        }
        void MountainsDungeon()
        {
            expedition.dungeonTitle = localizedStrings["mountainsDungeonTitle"];
            dungeonTitle.Text = expedition.dungeonTitle;
            infoText.Text = localizedStrings["mountainsDungeonEntryText"];
            MountainsEvents();
            expedition.movementCost = 1;
            player.dungeonCooldown = DateTime.Now.AddHours(6);

            mainPage.LoadDungeons();
        }
        void MountainsEvents()
        {
            expedition.maxLevel = 30;
            Event ev;
            #region Treasure
            //x2
            #region Dead adventurer
            {
                int repeat = 2;
                while (repeat > 0)
                {
                    ev = new Event("mountainsDeadAdventurer");
                    ev.option1 = new Option("mountainsDeadAdventurer1", 4);
                    AddGoldReward(100, 50, ev.option1.successList);

                    ev.option2 = new Option("mountainsDeadAdventurer2", 0);
                    ev.option2.JustContinue = true;

                    ev.eventCategory = EventCategory.Treasure;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //str
            #region Gold vein
            ev = new Event("mountainsGoldVein");
            ev.option1 = new Option("mountainsGoldVein1", 5);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(500, 60, ev.option1.successList);

            ev.option2 = new Option("mountainsGoldVein2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion

            #endregion

            #region Random events
            //cha, int
            #region Lost adventurer
            ev = new Event("mountainsLostAdventurer");
            ev.option1 = new Option("mountainsLostAdventurer1", 5);
            ev.option1.SetTest(StatTest.charisma);
            AddExpReward(1, 0.2f, ev.option1.successList);
            AddHpChange(-10, ev.option1.failList);
            AddEventFight(40, 90, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option1.failList, 5, 0.1f, 1);

            ev.option2 = new Option("mountainsLostAdventurer2", 5);
            ev.option2.SetTest(StatTest.intelligence);
            AddExpReward(1, 0.2f, ev.option2.successList);
            AddHpChange(-20, ev.option2.failList);
            AddEventFight(40, 90, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option2.failList, 5, 0.1f, 1);

            ev.option3 = new Option("mountainsLostAdventurer3", 2);
            AddEventFight(40, 90, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option3.successList, 5, 0.1f, 1);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            //dex
            #region Resting place 
            ev = new Event("mountainsRestingPlace");
            ev.option1 = new Option("mountainsRestingPlace1", 3);
            ev.option1.SetTest(StatTest.dexterity);
            AddHpChange(40, ev.option1.successList);
            AddHpChange(20, ev.option1.failList);
            AddApReward(10, ev.option1.successList);

            ev.option2 = new Option("mountainsRestingPlace2", 0);
            AddHpChange(30, ev.option2.successList);
            AddApReward(5, ev.option2.successList);

            ev.option3 = new Option("mountainsRestingPlace3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vit
            #region Webbed passage
            ev = new Event("mountainsWebbedPassage");
            ev.option1 = new Option("mountainsWebbedPassage1", 3);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(1, 0.2f, ev.option1.successList);
            AddHpChange(-20, ev.option1.failList);
            AddSpecificFight(random.Next(50, 70), random.Next(90, 140), random.Next(90, 150), random.Next(50, 70), random.Next(40, 80)
                , random.Next(70, 100), random.Next(80, 120), random.Next(60, 80), 9, "enemyNameBigSpider", "enemy_spiderboss", 200, 3, ev.option1.failList,
                50, 0.2f, 1, 0);
            ev.option2 = new Option("mountainsWebbedPassage2", 8);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);


            #endregion

            #endregion

            #region Fights

            #region Roaming spider
            ev = new Event("mountainsSpider");
            ev.option1 = new Option("mountainsSpider1", 1);
            AddSpecificFight(random.Next(50, 70), random.Next(90, 140), random.Next(90, 150), random.Next(50, 70), random.Next(40, 80)
                , random.Next(70, 100), random.Next(80, 120), random.Next(60, 80), 9, "enemyNameBigSpider", "enemy_spiderboss", 200, 3, ev.option1.successList,
                50, 0.2f, 1, 0);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //agi
            #region Sleeping bear
            ev = new Event("mountainsSleepingBear");
            ev.option1 = new Option("mountainsSleepingBear1", 2);
            AddSpecificFight(random.Next(80, 120), random.Next(60, 80), random.Next(50, 80), random.Next(40, 70), random.Next(90, 120),
                random.Next(70, 110), random.Next(50, 70), random.Next(40, 60), 9, "enemyNameBear", "enemy_bear", 75, 2, ev.option1.successList, 100, 0.5f);
            ev.option2 = new Option("mountainsSleepingBear2", 5);
            ev.option2.SetTest(StatTest.agility);
            AddExpReward(1, 0.1f, ev.option2.successList);

            AddSpecificFight(random.Next(80, 120), random.Next(60, 80), random.Next(50, 80), random.Next(40, 70), random.Next(90, 120),
                random.Next(90, 130), random.Next(50, 70), random.Next(40, 60), 9, "enemyNameBear", "enemy_bear", 75, 2, ev.option2.failList, 100, 0.5f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //x3
            #region Bandit
            {
                int repeat = 3;
                while (repeat > 0)
                {
                    ev = new Event("mountainsBandit");
                    ev.option1 = new Option("mountainsBandit1", 1);
                    AddEventFight(60, 90, 9, "enemyNameBandit", "enemy_bandit", 100, 1, ev.option1.successList, 100, 0.5f);
                    ev.eventCategory = EventCategory.Fight;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion


            #endregion

            //per,endurance
            #region Traps
            ev = new Event("mountainsSpearTrap");
            ev.option1 = new Option("mountainsSpearTrap1", 5);
            ev.option1.SetTest(StatTest.perception);
            AddExpReward(1, 0.1f, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.option2 = new Option("mountainsSpearTrap2", 2);
            ev.option2.SetTest(StatTest.endurance);
            AddExpReward(1, 0.1f, ev.option2.successList);
            AddHpChange(-50, ev.option2.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #region Mini boss
            expedition.miniBoss = new Event("mountainsMiniBoss");
            expedition.miniBoss.option1 = new Option("mountainsMiniBoss1", 5);
            AddSpecificFight(random.Next(100, 120), random.Next(60, 70), random.Next(50, 70), random.Next(20, 50), random.Next(50, 70), random.Next(100, 120),
                random.Next(30, 60), random.Next(30, 60), 9, "enemyNameGiantBear", "enemy_bearBoss", 1000, 10, expedition.miniBoss.option1.successList, 20, 0.2f, 0, 0);
            AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
            expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
            #endregion

            #region Boss

            expedition.boss = new Event("mountainsBoss");
            expedition.boss.option1 = new Option("mountainsBoss1", 0);
            AddSpecificFight(200, 100, 60, 30, 120, 200, 30, 20, 9, "enemyNameMonster", "enemy_monsterBoss", 2500, 25, expedition.boss.option1.successList, 0, 0, 0, 0);
            expedition.boss.eventCategory = EventCategory.Boss;

            #endregion
            MountainsMap();
        }

        #endregion
        #region Bandit camp dungeon

        void BanditCampDungeon()
        {
            expedition.dungeonTitle = localizedStrings["banditCampDungeonTitle"];
            dungeonTitle.Text = expedition.dungeonTitle;
            infoText.Text = localizedStrings["banditCampDungeonEntryText"];
            BanditCampEvents();
            expedition.movementCost = 1;
            player.dungeonCooldown = DateTime.Now.AddHours(8);

            mainPage.LoadDungeons();
        }
        void BanditCampEvents()
        {
            expedition.maxLevel = 35;
            Event ev;
            #region Treasure
            //Dexterity
            #region Locked chest
            ev = new Event("banditCampLockedChest");
            ev.option1 = new Option("banditCampLockedChest1", 7);
            ev.option1.SetTest(StatTest.dexterity);
            AddGoldReward(700, 70, ev.option1.successList);

            ev.option2 = new Option("banditCampLockedChest2", 0);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //x2
            #region Left valuables
            {
                int repeat = 2;
                while (repeat > 0)
                {
                    ev = new Event("banditCampLeftValuables");
                    ev.option1 = new Option("banditCampLeftValuables1", 4);
                    AddGoldReward(300, 50, ev.option1.successList);

                    ev.option2 = new Option("banditCampLeftValuables2", 0);
                    AddHpChange(20, ev.option2.successList);
                    AddApReward(20, ev.option2.successList);

                    ev.eventCategory = EventCategory.Treasure;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //strength
            #region Camp vault
            ev = new Event("banditCampVault");
            ev.option1 = new Option("banditCampVault1", 7);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(1000, 80, ev.option1.successList);
            AddHpChange(-20, ev.option1.failList);

            ev.option2 = new Option("banditCampVault2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            #region Random events
            //intelligence
            #region Nursing Tent
            ev = new Event("banditCampNursingTent");
            ev.option1 = new Option("banditCampNursingTent1", 5);
            AddHpChange(25, ev.option1.successList);

            ev.option2 = new Option("banditCampNursingTent2", 7);
            ev.option2.SetTest(StatTest.intelligence);
            AddGoldReward(1000, 20, ev.option2.successList);

            ev.option3 = new Option("banditCampNursingTent3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #region Resting camp
            ev = new Event("banditCampRestingCamp");
            ev.option1 = new Option("banditCampRestingCamp1", 0);
            AddHpChange(20, ev.option1.successList);
            AddApReward(15, ev.option1.successList);

            ev.option2 = new Option("banditCampRestingCamp2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vitality
            #region Bandit group
            ev = new Event("banditCampBanditGroup");
            ev.option1 = new Option("banditCampBanditGroup1", 6);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(5, 0.33f, ev.option1.successList);
            AddEventFight(70, 100, 9, "enemyNameYoungBandit", "enemy_slave", 0, 1, ev.option1.failList, 30, 0.25f);


            ev.option2 = new Option("banditCampBanditGroup2", 3);
            AddEventFight(60, 90, 9, "enemyNameYoungBandit", "enemy_slave", 0, 1, ev.option2.successList, 30, 0.25f);


            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //charisma
            #region Hired mercenary
            ev = new Event("banditCampHiredMercenary");
            ev.option1 = new Option("banditCampHiredMercenary1", 3);

            ev.option2 = new Option("banditCampHiredMercenary2", 6);
            ev.option2.SetTest(StatTest.charisma);
            AddExpReward(3, 0.33f, ev.option2.successList);
            {
                string mercenaryImage = mainPage.arenaImageList[random.Next(0, mainPage.arenaImageList.Count)];
                AddEventFight(100, 130, 9, "enemyNameHiredMercenary", mercenaryImage, 1000, 10, ev.option1.successList, 50, 0.7f);
                AddEventFight(100, 130, 9, "enemyNameHiredMercenary", mercenaryImage, 1000, 10, ev.option2.failList, 50, 0.7f);

            }
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            #region Fights
            //endurance
            #region Bowman
            ev = new Event("banditCampBowman");
            ev.option1 = new Option("banditCampBowman1", 2);
            AddHpChange(-20, ev.option1.successList);
            AddEventFight(60, 120, 9, "enemyNameBowman", "enemy_bandit", 100, 3, ev.option1.successList, 40, 0.4f);

            ev.option2 = new Option("banditCampBowman2", 6);
            ev.option2.SetTest(StatTest.endurance);
            AddExpReward(5, 0.4f, ev.option2.successList);
            AddHpChange(-25, ev.option2.failList);
            AddEventFight(60, 120, 9, "enemyNameBowman", "enemy_bandit", 100, 3, ev.option1.successList, 40, 0.4f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //x3
            #region Bandit
            {
                int repeat = 3;
                while (repeat > 0)
                {
                    ev = new Event("banditCampBandit");
                    ev.option1 = new Option("banditCampBandit1", 2);
                    AddEventFight(70, 120, 9, "enemyNameBandit", "enemy_bandit", 500, 5, ev.option1.successList, 50, 0.5f);

                    ev.eventCategory = EventCategory.Fight;
                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //perception
            #region War dog
            ev = new Event("banditCampUnleashedDog");
            ev.option1 = new Option("banditCampUnleashedDog1", 2);
            AddSpecificFight(60, 100, 90, 120, 70, 90, 70, 50, 9, "enemyNameWarDog", "enemy_banditDog", 0, 7, ev.option1.successList, 0, 0.5f);

            ev.option2 = new Option("banditCampUnleashedDog2", 5);
            ev.option2.SetTest(StatTest.perception);
            AddHpChange(-10, ev.option2.failList);
            AddSpecificFight(60, 100, 90, 120, 70, 90, 70, 50, 9, "enemyNameWarDog", "enemy_banditDog", 0, 7, ev.option2.failList, 0, 0.5f);
            AddExpReward(7, 0.2f, ev.option2.successList);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            #endregion

            #region Traps
            //agility
            #region Pendulum
            ev = new Event("banditCampPendulum");
            ev.option1 = new Option("banditCampPendulum1", 3);
            ev.option1.SetTest(StatTest.agility);
            AddExpReward(5, 0.3f, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #region Ambush
            ev = new Event("banditCampAmbush");
            ev.option1 = new Option("banditCampAmbush1", 2);
            AddEventFight(70, 130, 9, "enemyNameBandit", "enemy_bandit", 500, 4, ev.option1.successList, 30, 0.2f);

            ev.option2 = new Option("banditCampAmbush2", 12);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);

            #endregion

            #endregion

            #region Mini boss
            expedition.miniBoss = new Event("banditCampMiniBoss");
            expedition.miniBoss.option1 = new Option("banditCampMiniBoss1", 7);
            AddSpecificFight(200, 90, 60, 90, 150, 160, 70, 60, 9, "enemyNameGiantBandit", "enemy_banditMiniBoss", 2500, 15, expedition.miniBoss.option1.successList, 25, 0.5f, 1);
            AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
            expedition.miniBoss.eventCategory = EventCategory.MiniBoss;


            #endregion

            #region Boss

            expedition.boss = new Event("banditCampBoss");
            expedition.boss.option1 = new Option("banditCampMiniBoss1", 0);
            AddSpecificFight(120, 150, 140, 130, 90, 140, 120, 130, 9, "enemyNameBanditChieftain", "enemy_banditCaptain", 5000, 30, expedition.boss.option1.successList);
            expedition.boss.eventCategory = EventCategory.Boss;
            #endregion
            BanditCampMap();

        }
        void BanditCampMap()
        {
            expedition.playerPosition = new Position(9, 9);
            layout = new List<Label> { map98, map97, map96, map95, map94, map93, map92, map91, map90, map80, map70, map60, map50
            ,map40,map30,map20,map10,map11,map12,map14,map24,map34,map33,map44,map54,map53,map85,map75,map71,map73,map76,map77
            ,map67,map57,map56,map47,map48,map37,map35,map27,map17,map16,map72,map18,map15,map13,map32,map52,map74};
            List<Label> miniBoss = new List<Label> { map72, map18, map15, map13, map77 };
            List<Label> mainBoss = new List<Label> { map32, map52 };
            CheckMovement();
            PopulateEvents(miniBoss, mainBoss);
        }
        #endregion

        #region Desert dungeon

        void DesertDungeon()
        {
            expedition.dungeonTitle = localizedStrings["desertDungeonTitle"];//NIE MA W APPRESOURCES.RESX
            dungeonTitle.Text = expedition.dungeonTitle;
            infoText.Text = localizedStrings["desertDungeonEntryText"];//NIE MA W APPRESOURCES.RESX
            DesertEvents();
            expedition.movementCost = 1;
            player.dungeonCooldown = DateTime.Now.AddHours(8);

            mainPage.LoadDungeons();

        }
        void DesertMap()
        {
            expedition.playerPosition = new Position(9, 0);
            layout = new List<Label> { map80, map70, map80, map70, map60, map50, map40, map30, map20, map10, map00, map01, map02, map03
            , map04, map05, map06, map07, map08, map09, map19, map29, map39, map49, map59, map69, map79, map89, map88, map87, map86
            , map85, map84, map83, map82, map72, map62, map52, map42, map32, map22, map23, map24, map25, map26, map27, map37, map47, map57, map67
            , map66, map65, map64, map54, map44, map45};
            List<Label> miniBoss = new List<Label> { map64 };
            List<Label> mainBoss = new List<Label> { map45 };
            CheckMovement();
            PopulateEvents(miniBoss, mainBoss);
        }
        void DesertEvents()
        {
            expedition.maxLevel = 999;
            Event ev;

            #region granary events
            //4
            #region Treasures
            //otwórz(dex)3ap /otwórz(str)5ap /zostaw(3% na otwarcie)0ap
            #region Granary Closed Chest
            ev = new Event("granaryClosedChest");

            ev.option1 = new Option("granaryClosedChest1", 1);
            ev.option1.SetTest(StatTest.dexterity);
            AddGoldReward(500, 75, ev.option1.successList);
            AddExpReward(1, 0.2f, ev.option1.successList);
            roll = random.Next(0, 101);
            if (roll <= 20)
            {

                AddItemReward(player.level, player.level + 1, ev.option1.successList);
            }

            ev.option2 = new Option("granaryClosedChest2", 1);
            ev.option2.SetTest(StatTest.strength);
            AddGoldReward(500, 75, ev.option2.successList);
            AddExpReward(1, 0.2f, ev.option2.successList);
            roll = random.Next(0, 101);
            if (roll <= 20)
            {

                AddItemReward(player.level, player.level + 1, ev.option2.successList);
            }
            AddHpChange(-10, ev.option2.failList);

            ev.option3 = new Option("granaryClosedChest3", 0);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //gold 1ap /hp 1ap
            #region Damaged Crates
            ev = new Event("granaryDamagedCrates");

            ev.option1 = new Option("granaryDamagedCrates1", 1);
            AddGoldReward(50, 75, ev.option1.successList);

            ev.option2 = new Option("granaryDamagedCrates2", 1);
            AddHpChange(20, ev.option2.successList);

            ev.option3 = new Option("granaryDamagedCrates3", 0);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //szukaj aktualnych dokumentów(int)++gold /zrób ognisko z dokumentów +hp /spróbuj się czegoś dowiedzieć +exp
            #region Old documents
            ev = new Event("granaryOldDocuments");
            #region op1
            ev.option1 = new Option("granaryOldDocuments1", 1);
            ev.option1.SetTest(StatTest.intelligence);
            AddGoldReward(100, 80, ev.option1.successList);
            #endregion
            #region op2
            ev.option2 = new Option("granaryOldDocuments2", 1);
            AddHpChange(20, ev.option2.successList);
            #endregion
            #region op3
            ev.option3 = new Option("granaryOldDocuments3", 0);
            ev.option3.JustContinue = true;
            #endregion
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //wypij wywar +-hp (vit) 2ap / 2sprzedaj +gold
            #region Old brew
            ev = new Event("granaryOldBrew");

            ev.option1 = new Option("granaryOldBrew1", 2);
            ev.option1.SetTest(StatTest.vitality);
            AddHpChange(30, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.option2 = new Option("granaryOldBrew2", 2);
            AddGoldReward(60, 70, ev.option2.successList);

            ev.option3 = new Option("granaryOldBrew3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #endregion
            //5
            #region Enemies
            //walcz 1ap /odstrasz(dex) 3ap /omiń(agi) 3ap
            #region Rat pack
            ev = new Event("granaryRatPack");

            ev.option1 = new Option("granaryRatPack1", 0);
            AddEventFight(90, 120, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option1.successList, 50, 0.2f);

            ev.option2 = new Option("granaryRatPack2", 1);
            ev.option2.SetTest(StatTest.dexterity);
            AddExpReward(1, 0.1f, ev.option2.successList);
            AddEventFight(90, 120, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option2.failList, 50, 0.2f);

            ev.option3 = new Option("granaryRatPack3", 1);
            ev.option3.SetTest(StatTest.agility);
            AddExpReward(1, 0.1f, ev.option3.successList);
            AddEventFight(90, 120, 1, "enemyNameRatPack", "enemy_ratpack", 10, 2, ev.option3.failList, 50, 0.2f);

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //walka ze szczurem x3
            #region Rat Blockade
            {
                int repeat = 3;
                while (repeat > 0)
                {
                    ev = new Event("granaryRatBlockade");
                    ev.option1 = new Option("granaryRatBlockade1", 2);
                    AddEventFight(40, 60, 2, "enemyNameBigRat", "enemy_rat", 70, 4, ev.option1.successList, 55, 0.22f);

                    ev.eventCategory = EventCategory.Fight;

                    expedition.eventList.Add(ev);
                    repeat--;
                }
            }
            #endregion
            //walka 2ap / przechytrzenie 4ap / nic
            #region Escaped slave
            ev = new Event("granaryEscapedSlave");

            ev.option1 = new Option("granaryEscapedSlave1", 0);
            AddEventFight(40, 65, 2, "enemyNameEscapedSlave", "enemy_slave", 0, 3, ev.option1.successList, 0, 0.35f);

            ev.option2 = new Option("granaryEscapedSlave2", 2);
            ev.option2.SetTest(StatTest.charisma);
            AddExpReward(2, 0.2f, ev.option2.successList);
            AddHpChange(-20, ev.option2.failList);

            ev.option3 = new Option("granaryEscapedSlave3", 3); ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion

            #endregion
            //1
            #region Traps

            //szukaj mechanizmu uruchomienia pułapek (per)+exp /przedostań się uważając na pułapki (end) +exp
            #region Trapped corridor
            ev = new Event("granaryTrappedCorridor");

            ev.option1 = new Option("granaryTrappedCorridor1", 4);
            ev.option1.SetTest(StatTest.perception);
            AddExpReward(3, 0.2f, ev.option1.successList);
            AddHpChange(-30, ev.option1.failList);

            ev.option2 = new Option("granaryTrappedCorridor2", 1);
            ev.option2.SetTest(StatTest.endurance);
            AddExpReward(2, 0.2f, ev.option2.successList);
            AddHpChange(-5, ev.option2.successList);
            AddHpChange(-15, ev.option2.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #endregion

            #endregion

            #region forest events
            //3
            #region Treasure
            //perception test or rest
            #region Old ruins
            ev = new Event("forestRuins");
            ev.option1 = new Option("forestRuins1", 2);
            ev.option1.SetTest(StatTest.perception);
            AddGoldReward(0, 75, ev.option1.successList);
            int x = random.Next(0, 11);
            if (x < 3)
            {
                AddItemReward(DungeonModifier(), DungeonModifier() + 1, ev.option1.successList);
            }
            ev.option2 = new Option("forestRuins2", 0);
            AddHpChange(20, ev.option2.successList);
            AddApReward(4, ev.option2.successList);

            ev.option3 = new Option("forestRuins3", 0);
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //dex for health or loot
            #region Camp
            ev = new Event("forestCamp");
            ev.option1 = new Option("forestCamp1", 3);
            ev.option1.SetTest(StatTest.dexterity);
            AddHpChange(30, ev.option1.successList);
            AddHpChange(5, ev.option1.failList);
            ev.option2 = new Option("forestCamp2", 1);
            AddHpChange(10, ev.option2.successList);
            AddGoldReward(100, 50, ev.option2.successList);
            ev.option3 = new Option("forestCamp3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //str test
            #region Stagecoach
            ev = new Event("forestStagecoach");
            ev.option1 = new Option("forestStagecoach1", 5);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(50, 80, ev.option1.successList);
            ev.option2 = new Option("forestStagecoach2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //3
            #region Random events
            //endurance or intelligence test
            #region Cliffside
            ev = new Event("forestCliffside");
            ev.option1 = new Option("forestCliffside1", 4);
            ev.option1.SetTest(StatTest.endurance);
            AddHpChange(-25, ev.option1.failList);
            ev.option2 = new Option("forestCliffside2", 2);
            ev.option2.SetTest(StatTest.intelligence);
            AddHpChange(-25, ev.option2.failList);
            ev.option3 = new Option("forestCliffside3", 6);
            ev.option3.JustContinue = true;
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vitality test
            #region River crossing
            ev = new Event("forestRiver");
            ev.option1 = new Option("forestRiver1", 2);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(1, 0.2f, ev.option1.successList);
            AddHpChange(-25, ev.option1.failList);
            ev.option2 = new Option("forestRiver2", 6);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #region Deer
            ev = new Event("forestDeer");
            ev.option1 = new Option("forestDeer1", 5);
            AddHpChange(20, ev.option1.successList);
            ev.option2 = new Option("forestDeer2", 3);
            AddExpReward(1, 0.15f, ev.option2.successList);
            ev.option3 = new Option("forestDeer3", 0);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //6
            #region Fights
            //Fight x2
            #region Bear

            ev = new Event("forestBear");
            ev.option1 = new Option("forestBear1", 3);
            AddSpecificFight(150, 100, 100, 70, 150, 150, 50, 20, 9, "enemyNameBear", "enemy_bear", 300, 5, ev.option1.successList, 15, 0.2f);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);

            #endregion
            //Fight x3
            #region Wolf ambush
            ev = new Event("forestWolf");
            ev.option1 = new Option("forestWolf1", 3);
            AddSpecificFight(90, 130, 130, 100, 80, 80, 120, 80, 9, "enemyNameWolf", "enemy_wolf", 250, 5, ev.option1.successList, 10, 0.2f);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //cha test or fight
            #region Bandit
            ev = new Event("forestBandit");
            ev.option1 = new Option("forestBandit1", 5);
            ev.option1.SetTest(StatTest.charisma);
            AddExpReward(3, 0.1f, ev.option1.successList);
            AddEventFight(80, 120, 9, "enemyNameBandit", "enemy_bandit", 200, 5, ev.option1.failList, 15, 0.3f);
            ev.option2 = new Option("forestBandit2", 3);
            AddEventFight(80, 120, 9, "enemyNameBandit", "enemy_bandit", 200, 5, ev.option2.successList, 15, 0.3f);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            //2
            #region Traps
            #region Mutated boar

            ev = new Event("forestMutatedBoar");

            ev.option1 = new Option("forestMutatedBoar1", 5);
            AddEventFight(80, 120, 7, "enemyNameMutatedBoar", "enemy_boar", 200, 1, ev.option1.successList, 10, 0.25f);

            ev.option2 = new Option("forestMutatedBoar2", 4);
            ev.option2.SetTest(StatTest.agility);
            AddExpReward(2, 0.15f, ev.option2.successList);
            AddHpChange(-30, ev.option2.failList);
            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            #endregion

            #region mountain events
            #region Treasure
            //x2
            #region Dead adventurer
            ev = new Event("mountainsDeadAdventurer");
            ev.option1 = new Option("mountainsDeadAdventurer1", 4);
            AddGoldReward(100, 65, ev.option1.successList);

            ev.option2 = new Option("mountainsDeadAdventurer2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //str
            #region Gold vein
            ev = new Event("mountainsGoldVein");
            ev.option1 = new Option("mountainsGoldVein1", 5);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(500, 100, ev.option1.successList);

            ev.option2 = new Option("mountainsGoldVein2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion

            #endregion

            #region Random events
            //cha, int
            #region Lost adventurer
            ev = new Event("mountainsLostAdventurer");
            ev.option1 = new Option("mountainsLostAdventurer1", 5);
            ev.option1.SetTest(StatTest.charisma);
            AddExpReward(1, 0.2f, ev.option1.successList);
            AddHpChange(-10, ev.option1.failList);
            AddEventFight(80, 120, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option1.failList, 5, 0.2f, 1);

            ev.option2 = new Option("mountainsLostAdventurer2", 5);
            ev.option2.SetTest(StatTest.intelligence);
            AddExpReward(1, 0.2f, ev.option2.successList);
            AddHpChange(-20, ev.option2.failList);
            AddEventFight(80, 120, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option2.failList, 5, 0.2f, 1);

            ev.option3 = new Option("mountainsLostAdventurer3", 2);
            AddEventFight(80, 120, 9, "enemyNameLostAdventurer", "enemy_slave", 300, 5, ev.option3.successList, 5, 0.2f, 1);
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            //dex
            #region Resting place 
            ev = new Event("mountainsRestingPlace");
            ev.option1 = new Option("mountainsRestingPlace1", 3);
            ev.option1.SetTest(StatTest.dexterity);
            AddHpChange(40, ev.option1.successList);
            AddHpChange(20, ev.option1.failList);
            AddApReward(10, ev.option1.successList);

            ev.option2 = new Option("mountainsRestingPlace2", 0);
            AddHpChange(30, ev.option2.successList);
            AddApReward(5, ev.option2.successList);

            ev.option3 = new Option("mountainsRestingPlace3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vit
            #region Webbed passage
            ev = new Event("mountainsWebbedPassage");
            ev.option1 = new Option("mountainsWebbedPassage1", 3);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(1, 0.2f, ev.option1.successList);
            AddHpChange(-20, ev.option1.failList);
            AddSpecificFight(60, 150, 150, 70, 60, 100, 150, 30, 9, "enemyNameBigSpider", "enemy_spiderboss", 200, 3, ev.option1.failList,
                50, 0.2f, 1, 0);
            ev.option2 = new Option("mountainsWebbedPassage2", 8);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);


            #endregion

            #endregion

            #region Fights

            #region Roaming spider
            ev = new Event("mountainsSpider");
            ev.option1 = new Option("mountainsSpider1", 1);
            AddSpecificFight(60, 120, 120, 70, 60, 80, 120, 30, 9, "enemyNameBigSpider", "enemy_spiderboss", 200, 3, ev.option1.successList,
                50, 0.2f, 1, 0);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //agi
            #region Sleeping bear
            ev = new Event("mountainsSleepingBear");
            ev.option1 = new Option("mountainsSleepingBear1", 2);
            AddSpecificFight(150, 100, 100, 70, 150, 150, 50, 20, 9, "enemyNameBear", "enemy_bear", 75, 2, ev.option1.successList, 100, 0.5f);
            ev.option2 = new Option("mountainsSleepingBear2", 5);
            ev.option2.SetTest(StatTest.agility);
            AddExpReward(1, 0.2f, ev.option2.successList);

            AddSpecificFight(150, 100, 100, 70, 150, 150, 50, 20, 9, "enemyNameBear", "enemy_bear", 75, 2, ev.option1.successList, 100, 0.5f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //x3
            #region Bandit
            ev = new Event("mountainsBandit");
            ev.option1 = new Option("mountainsBandit1", 1);
            AddEventFight(80, 120, 9, "enemyNameBandit", "enemy_bandit", 100, 1, ev.option1.successList, 100, 0.2f);
            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion


            #endregion

            //per,endurance
            #region Traps
            ev = new Event("mountainsSpearTrap");
            ev.option1 = new Option("mountainsSpearTrap1", 5);
            ev.option1.SetTest(StatTest.perception);
            AddExpReward(1, 0.15f, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.option2 = new Option("mountainsSpearTrap2", 2);
            ev.option1.SetTest(StatTest.endurance);
            AddExpReward(1, 0.15f, ev.option2.successList);
            AddHpChange(-50, ev.option2.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #endregion

            #region bandit camp events
            #region Treasure
            //Dexterity
            #region Locked chest
            ev = new Event("banditCampLockedChest");
            ev.option1 = new Option("banditCampLockedChest1", 7);
            ev.option1.SetTest(StatTest.dexterity);
            AddGoldReward(700, 100, ev.option1.successList);

            ev.option2 = new Option("banditCampLockedChest2", 0);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //x2
            #region Left valuables
            ev = new Event("banditCampLeftValuables");
            ev.option1 = new Option("banditCampLeftValuables1", 4);
            AddGoldReward(300, 75, ev.option1.successList);

            ev.option2 = new Option("banditCampLeftValuables2", 0);
            AddHpChange(20, ev.option2.successList);
            AddApReward(20, ev.option2.successList);

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            //strength
            #region Camp vault
            ev = new Event("banditCampVault");
            ev.option1 = new Option("banditCampVault1", 7);
            ev.option1.SetTest(StatTest.strength);
            AddGoldReward(1000, 110, ev.option1.successList);
            AddHpChange(-20, ev.option1.failList);

            ev.option2 = new Option("banditCampVault2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.Treasure;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            #region Random events
            //intelligence
            #region Nursing Tent
            ev = new Event("banditCampNursingTent");
            ev.option1 = new Option("banditCampNursingTent1", 5);
            AddHpChange(25, ev.option1.successList);

            ev.option2 = new Option("banditCampNursingTent2", 7);
            ev.option2.SetTest(StatTest.intelligence);
            AddGoldReward(1000, 50, ev.option2.successList);

            ev.option3 = new Option("banditCampNursingTent3", 0);
            ev.option3.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion

            #region Resting camp
            ev = new Event("banditCampRestingCamp");
            ev.option1 = new Option("banditCampRestingCamp1", 0);
            AddHpChange(20, ev.option1.successList);
            AddApReward(15, ev.option1.successList);

            ev.option2 = new Option("banditCampRestingCamp2", 0);
            ev.option2.JustContinue = true;

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //vitality
            #region Bandit group
            ev = new Event("banditCampBanditGroup");
            ev.option1 = new Option("banditCampBanditGroup1", 6);
            ev.option1.SetTest(StatTest.vitality);
            AddExpReward(5, 0.33f, ev.option1.successList);
            AddEventFight(80, 120, 9, "enemyNameYoungBandit", "enemy_slave", 0, 1, ev.option1.failList, 30, 0.25f);

            ev.option2 = new Option("banditCampBanditGroup2", 3);
            AddEventFight(80, 120, 9, "enemyNameYoungBandit", "enemy_slave", 0, 1, ev.option1.failList, 30, 0.25f);

            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            //charisma
            #region Hired mercenary
            ev = new Event("banditCampHiredMercenary");
            ev.option1 = new Option("banditCampHiredMercenary1", 3);

            ev.option2 = new Option("banditCampHiredMercenary2", 6);
            ev.option2.SetTest(StatTest.charisma);
            AddExpReward(3, 0.33f, ev.option2.successList);
            {
                string mercenaryImage = mainPage.arenaImageList[random.Next(0, mainPage.arenaImageList.Count)];
                AddEventFight(100, 150, 9, "enemyNameHiredMercenary", mercenaryImage, 1000, 10, ev.option1.successList, 50, 0.7f);
                AddEventFight(100, 150, 9, "enemyNameHiredMercenary", mercenaryImage, 1000, 10, ev.option2.failList, 50, 0.7f);

            }
            ev.eventCategory = EventCategory.RandomEvent;
            expedition.eventList.Add(ev);
            #endregion
            #endregion
            #region Fights
            //endurance
            #region Bowman
            ev = new Event("banditCampBowman");
            ev.option1 = new Option("banditCampBowman1", 2);
            AddHpChange(-20, ev.option1.successList);
            AddEventFight(80, 140, 9, "enemyNameBowman", "enemy_bandit", 100, 3, ev.option1.successList, 40, 0.4f);

            ev.option2 = new Option("banditCampBowman2", 6);
            ev.option2.SetTest(StatTest.endurance);
            AddExpReward(5, 0.4f, ev.option2.successList);
            AddHpChange(-25, ev.option2.failList);
            AddEventFight(80, 140, 9, "enemyNameBowman", "enemy_bandit", 100, 3, ev.option1.successList, 40, 0.4f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //x3
            #region Bandit
            ev = new Event("banditCampBandit");
            ev.option1 = new Option("banditCampBandit1", 2);
            AddEventFight(70, 120, 9, "enemyNameBandit", "enemy_bandit", 500, 5, ev.option1.successList, 50, 0.5f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            //perception
            #region War dog
            ev = new Event("banditCampUnleashedDog");
            ev.option1 = new Option("banditCampUnleashedDog1", 2);
            AddSpecificFight(60, 100, 90, 120, 70, 90, 70, 50, 9, "enemyNameWarDog", "enemy_banditDog", 0, 7, ev.option1.successList, 0, 0.5f);

            ev.option2 = new Option("banditCampUnleashedDog2", 5);
            ev.option2.SetTest(StatTest.perception);
            AddHpChange(-10, ev.option2.failList);
            AddExpReward(2, 0.5f, ev.option2.successList);
            AddSpecificFight(60, 100, 90, 120, 70, 90, 70, 50, 9, "enemyNameWarDog", "enemy_banditDog", 0, 7, ev.option2.failList, 0, 0.5f);

            ev.eventCategory = EventCategory.Fight;
            expedition.eventList.Add(ev);
            #endregion
            #endregion

            #region Traps
            //agility
            #region Pendulum
            ev = new Event("banditCampPendulum");
            ev.option1 = new Option("banditCampPendulum1", 3);
            ev.option1.SetTest(StatTest.agility);
            AddExpReward(5, 0.3f, ev.option1.successList);
            AddHpChange(-40, ev.option1.failList);

            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);
            #endregion

            #region Ambush
            ev = new Event("banditCampAmbush");
            ev.option1 = new Option("banditCampAmbush1", 2);
            AddEventFight(70, 130, 9, "enemyNameBandit", "enemy_bandit", 500, 4, ev.option1.successList, 30, 0.2f);

            ev.option2 = new Option("banditCampAmbush2", 12);
            ev.option2.JustContinue = true;
            ev.eventCategory = EventCategory.Trap;
            expedition.eventList.Add(ev);

            #endregion

            #endregion
            #endregion

            int randomMiniBoss = random.Next(1, 5);
            switch (randomMiniBoss)
            {
                case 1:
                    expedition.miniBoss = new Event("granaryHugeSpider");

                    expedition.miniBoss.option1 = new Option("granaryHugeSpider1", 1);
                    AddSpecificFight(50,170,170,100,60,120,60,60, 9, "enemyNameSpiderBoss", "enemy_spiderboss", 200, 8, expedition.miniBoss.option1.successList, 20, 0.1f);
                    AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);

                    expedition.miniBoss.option2 = new Option("granaryHugeSpider2", 2); expedition.miniBoss.option2.JustContinue = true;

                    expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
                    break;
                case 2:
                    expedition.miniBoss = new Event("forestMiniBoss");
                    expedition.miniBoss.option1 = new Option("forestMiniBoss1", 2);
                    AddSpecificFight(100,120,170,120,90,120,80,80, 9, "enemyNameAlphaWolf", "enemy_wolfboss", 1000, 10, expedition.miniBoss.option1.successList, 20, 0.2f, 0, 0);
                    AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
                    expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
                    break;
                case 3:
                    expedition.miniBoss = new Event("mountainsMiniBoss");
                    expedition.miniBoss.option1 = new Option("mountainsMiniBoss1", 5);
                    AddSpecificFight(200,100,70,70,150,150,50,50, 9, "enemyNameGiantBear", "enemy_bearBoss", 1000, 10, expedition.miniBoss.option1.successList, 20, 0.2f, 0, 0);
                    AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
                    expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
                    break;
                case 4:
                    expedition.miniBoss = new Event("banditCampMiniBoss");
                    expedition.miniBoss.option1 = new Option("banditCampMiniBoss1", 7);
                    AddSpecificFight(200, 90, 60, 90, 150, 160, 70, 60, 9, "enemyNameGiantBandit", "enemy_banditMiniBoss", 2500, 15, expedition.miniBoss.option1.successList, 25, 0.5f, 1);
                    AddFightItemReward(mainPage.ItemGenerator(DungeonModifier() + 2, DungeonModifier() + 2, -1), expedition.miniBoss.option1);
                    expedition.miniBoss.eventCategory = EventCategory.MiniBoss;
                    break;
            }

            while (expedition.eventList.Count > 20)
            {
                expedition.eventList.RemoveAt(random.Next(0, expedition.eventList.Count));
            }

            expedition.boss = new Event("desertBoss");
            expedition.boss.option1 = new Option("desertBoss1", 0);
            AddSpecificFight(120, 130, 150, 110, 90, 220, 120, 120, 9, "enemyNameGiantMummy", "enemy_mummyBoss", 15000, 50, expedition.boss.option1.successList);
            expedition.boss.eventCategory = EventCategory.Boss;

            DesertMap();
        }
        #endregion
    }
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public Position() { }
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    #region Main classes
    [Serializable]
    public class Expedition
    {
        public Position playerPosition { get; set; } = new Position();
        public string dungeonTitle { get; set; }
        public int maxLevel { get; set; }
        //public int dungeonLength { get; set; }
        public Event currentEvent { get; set; }
        public List<Event> eventList { get; set; } = new List<Event>();
        public Event boss { get; set; }
        public Event miniBoss { get; set; }
        public int movementCost { get; set; } = 0;

    }
    public class Event
    {
        public Position pos { get; set; } = new Position();
        public EventCategory eventCategory { get; set; } = EventCategory.None;
        public string eventText { get; set; }
        public Option option1 { get; set; }
        public Option option2 { get; set; }
        public Option option3 { get; set; }
        public Event() { }
        public Event(string eventText)
        {
            this.eventText = eventText;
            pos = new Position(-2, -2);
        }
        public Event(string eventText, int x, int y)
        {
            this.eventText = eventText;
            pos = new Position(x, y);
        }
        public void SetPosition(int x, int y)
        {
            pos = new Position(x, y);
        }
    }
    public class Option
    {
        #region basic variables
        //public List<Operation> startList { get; set; } = new List<Operation>();
        public List<Operation> successList { get; set; } = new List<Operation>();
        public List<Operation> failList { get; set; } = new List<Operation>();
        //public Queue<int> enemyStats { get; set; } = new Queue<int>();
        public int apCost { get; set; } = 0;
        public string buttonText { get; set; }
        public string successText { get; set; }
        public string failText { get; set; }
        public string afterSuccessText { get; set; }
        public string afterFailText { get; set; }
        public string fightWonText { get; set; }
        public string fightLostText { get; set; }
        public bool IsTest { get; set; } = false;
        public StatTest Attribute { get; set; } = StatTest.nothing;
        public int nothingChance { get; set; } = 50;
        public bool JustContinue { get; set; } = false;
        public bool OptionDone { get; set; } = false;
        public int rounds { get; set; } = 30;
        public Item fightItemReward { get; set; } = null;
        public bool success { get; set; }
        #endregion
        public Option()
        {
            JustContinue = true;
        }
        public Option(string buttonText, int apCost = 0)
        {
            this.buttonText = buttonText;
            SetText(buttonText);
            this.apCost = apCost;
            ApChange cost = new ApChange(0 - apCost);
            //startList.Add(cost);


        }
        public void SetText(string baseText)
        {
            this.successText = baseText + "_1";
            this.failText = baseText + "_2";
            this.afterSuccessText = baseText + "_3";
            this.afterFailText = baseText + "_4";
            this.fightWonText = baseText + "_5";
            this.fightLostText = baseText + "_6";
        }
        public void SetTest(StatTest statTest = StatTest.nothing)
        {
            IsTest = true;
            Attribute = statTest;

        }
        public Option(string text, int difficulty, int room)
        {
            buttonText = text;


        }
    }

    public class Operation
    {
        public opType type;
        public string enemyName { get; set; }
        public string enemySource { get; set; }
        public int baseValue { get; set; }
        public int maxValue { get; set; }
        public int levelModifier { get; set; }
        public float modifier { get; set; }
        //public Entity enemy { get; set; }
        public Queue<int> enemyStats { get; set; } = new Queue<int>();
        public int fightItemReward { get; set; }
        public int gold { get; set; }
        //public float goldModifier { get; set; }
        public int exp { get; set; }
        public float expModifier { get; set; }
        public Item item { get; set; }
        public string text { get; set; }

        public int enemyItems { get; set; }
        public virtual void Execute(Player player, int currentRoom)
        {
            if (this.type == opType.ApChange)
            {
                player.CurrentStamina += baseValue;

            }
            else if (this.type == opType.EventFight)
            {
                //chyba nic
            }
            else if (this.type == opType.ExpChange)
            {
                player.Exp += (int)Math.Floor(baseValue + (currentRoom * modifier));

            }
            else if (this.type == opType.GoldChange)
            {
                player.Gold += (int)Math.Floor(baseValue + (currentRoom * modifier));

            }
            else if (this.type == opType.HpChange)
            {
                player.CurrentHealth += (int)Math.Ceiling(((float)baseValue / 100) * player.maxHealth);

            }
            else if (this.type == opType.ItemReward)
            {
                player.itemsToRecieve.Enqueue(item);

            }
        }
    }
    #endregion
    #region SubClasses
    public class ApChange : Operation
    {
        public ApChange(int baseValue, int modifier = 0) { this.type = opType.ApChange; this.baseValue = baseValue; this.modifier = modifier; }
        public override void Execute(Player player, int currentRoom)
        {
            player.CurrentStamina += baseValue;
        }
    }
    public class HpChange : Operation
    {
        /// <summary>
        /// baseValue -> percentage health gain or loss
        /// modifier -> currently does nothing
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="modifier"></param>
        public HpChange(int baseValue, float modifier = 0) { this.type = opType.HpChange; this.baseValue = baseValue; this.modifier = modifier; }
        public override void Execute(Player player, int currentRoom)
        {

            player.CurrentHealth += (int)Math.Ceiling(((float)baseValue / 100) * player.maxHealth);
        }
    }
    public class GoldChange : Operation
    {
        /// <summary>
        /// value -> base value of gold gained
        /// modifier -> multiplicative bonus of gold gained multiplied by a current room
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifier"></param>
        public GoldChange(int value, float modifier = 0) { this.type = opType.GoldChange; this.baseValue = value; this.modifier = modifier; }
        public override void Execute(Player player, int currentRoom)
        {

            player.Gold += (int)Math.Floor(baseValue + (currentRoom * modifier));
        }
    }
    public class ExpChange : Operation
    {
        /// <summary>
        /// value -> base vale of exp gained
        /// modifier -> multiplicative bonus of exp gained multiplied by a current room
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifier"></param>
        public ExpChange(int value, float modifier = 0) { this.type = opType.ExpChange; this.baseValue = value; this.modifier = modifier; }
        public override void Execute(Player player, int currentRoom)
        {

            player.Exp += (int)Math.Floor(baseValue + (currentRoom * modifier));
        }
    }
    public class ItemReward : Operation
    {
        /// <summary>
        /// item -> item awarded to player
        /// modifier -> currently does nothing
        /// </summary>
        public ItemReward(Item item, float modifier = 0)
        {
            this.type = opType.ItemReward;
            this.modifier = modifier;
            this.item = item;
        }
        public override void Execute(Player player, int currentRoom)
        {

            player.itemsToRecieve.Enqueue(item);
        }
    }
    public class EventFight : Operation
    {
        /// <summary>
        /// enemy -> enemy to fight
        /// gold -> gold awarded for victory
        /// exp -> exp awarded for victory
        /// modifier -> multiplicative bonus of gold gained multiplied by a current room
        /// </summary>
        /// 
        public EventFight()
        {
            this.type = opType.EventFight;
        }

        public EventFight(int minValue, int maxValue, int enemyItems, string name, string enemySource, int gold, int exp, float modifier = 0, float expModifier = 0, int itemsReward = 0, int levelModifier = 0)
        {
            this.enemyName = name;
            this.enemySource = enemySource;
            this.type = opType.EventFight;
            this.text = name;
            this.enemyItems = enemyItems;
            this.baseValue = minValue;
            this.maxValue = maxValue;
            this.expModifier = expModifier;
            this.gold = gold;
            this.exp = exp;
            this.modifier = modifier;
            this.fightItemReward = itemsReward;
            this.levelModifier = levelModifier;
            this.enemyStats.Clear();
        }
        public EventFight(int str, int per, int dex, int agi, int end, int vit, int cha, int wis,
            int enemyItems, string name, string enemySource, int gold, int exp, float modifier = 0, float expModifier = 0, int itemsReward = 0, int levelModifier = 0)
        {
            this.enemyName = name;
            this.enemySource = enemySource;
            this.type = opType.EventFight;
            this.text = name;
            this.enemyItems = enemyItems;
            this.expModifier = expModifier;
            this.gold = gold;
            this.exp = exp;
            this.modifier = modifier;
            this.fightItemReward = itemsReward;
            this.levelModifier = levelModifier;
            this.enemyStats.Clear();
            this.enemyStats.Enqueue(str);
            this.enemyStats.Enqueue(per);
            this.enemyStats.Enqueue(dex);
            this.enemyStats.Enqueue(agi);
            this.enemyStats.Enqueue(end);
            this.enemyStats.Enqueue(vit);
            this.enemyStats.Enqueue(cha);
            this.enemyStats.Enqueue(wis);

        }
    }
    public enum opType
    {
        ApChange,
        HpChange,
        GoldChange,
        ExpChange,
        ItemReward,
        EventFight,
        Fight
    }
    public enum EventCategory
    {
        Boss,
        MiniBoss,
        Treasure,
        RandomEvent,
        Fight,
        Trap,
        None
    }
    public enum StatTest
    {
        strength,
        perception,
        dexterity,
        agility,
        vitality,
        endurance,
        charisma,
        intelligence,
        nothing
    }
    #endregion
}