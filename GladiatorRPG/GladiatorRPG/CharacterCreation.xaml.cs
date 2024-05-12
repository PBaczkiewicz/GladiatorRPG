using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GladiatorRPG
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterCreation : ContentPage
    {
        Player player;
        int str = 0, per = 0, dex = 0, agi = 0, vit = 0, end = 0, cha = 0, intel = 0;
        Dictionary<string, string> localizedStrings;
        MainTabPage mp;
        string imageString;

        List<string> pickerOptions;
        public CharacterCreation(Dictionary<string, string> _localizedStrings, Player _player, MainTabPage _mp)
        {
            InitializeComponent();
            
            NavigationPage.SetHasNavigationBar(this, false);
            mp = _mp;
            player = _player;
            localizedStrings = _localizedStrings;
            pickerOptions = new List<string>();
            player.source = "player_male_1";
            imageString = "player_male_1";
            BackgroundsList();
            startGame.IsEnabled = false;
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        void BackgroundsList()
        {
            pickerOptions.Add("Farmer");
            pickerOptions.Add("Blacksmith");
            pickerOptions.Add("Deserter");
            pickerOptions.Add("Builder");
            pickerOptions.Add("Slaver");
            pickerOptions.Add("Thief");


            pickerOptions.Sort();
            backgroundPicker.ItemsSource = pickerOptions;
        }
        void backgroundPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            string option = backgroundPicker.SelectedItem.ToString();
            string description = "";
            //characterName.Text = option;
            switch (option)
            {
                case "Farmer":
                    description = localizedStrings["backgroundFarmer"];
                    str = 7; per = 3; dex = 4; agi = 3; vit = 5; end = 7; cha = 3; intel = 3;
                    break;
                case "Blacksmith":
                    description = localizedStrings["backgroundBlacksmith"];
                    str = 8; per = 5; dex = 3; agi = 1; vit = 7; end = 4; cha = 4; intel = 3;
                    break;
                case "Deserter":
                    description = localizedStrings["backgroundDeserter"];
                    str = 5; per = 6; dex = 7; agi = 5; vit = 5; end = 4; cha = 1; intel = 2;
                    break;
                case "Builder":
                    description = localizedStrings["backgroundBuilder"];
                    str = 7; per = 3; dex = 2; agi = 3; vit = 8; end = 8; cha = 1; intel = 3;
                    break;
                case "Slaver":
                    description = localizedStrings["backgroundSlaver"];
                    str = 2; per = 5; dex = 3; agi = 2; vit = 5; end = 3; cha = 8; intel = 7;
                    break;
                case "Thief":
                    description = localizedStrings["backgroundThief"];
                    str = 2; per = 5; dex = 8; agi = 8; vit = 4; end = 2; cha = 4; intel = 2;
                    break;
            }
            description += "\n\n" + localizedStrings["startingStatsText"] + "\n" +
                      localizedStrings["strText"] + " : " + str + "\n" +
                      localizedStrings["perText"] + " : " + per + "\n" +
                      localizedStrings["dexText"] + " : " + dex + "\n" +
                      localizedStrings["agiText"] + " : " + agi + "\n" +
                      localizedStrings["vitText"] + " : " + vit + "\n" +
                      localizedStrings["endText"] + " : " + end + "\n" +
                      localizedStrings["chaText"] + " : " + cha + "\n" +
                      localizedStrings["intText"] + " : " + intel + "\n";
            backgroundDescription.Text = description;
            startGame.IsEnabled = true;

        }
        void startGame_Clicked(object sender, EventArgs e)
        {
            CreateNewCharacter();
            mp.MainModules();
            mp.LoadExpeditions();
            mp.UpdateTrainingCosts();
        }

        async void CreateNewCharacter()
        {
            //player = new Player(characterName.Text, 1, str, per, dex, agi, vit, end, cha, intel);
            player.name = characterName.Text;
            player.source = imageString;
            player.level = 1;
            player.baseStrength = str;
            player.basePerception = per;
            player.baseDexterity = dex;
            player.baseAgility = agi;
            player.baseVitality= vit;
            player.baseEndurance = end;
            player.baseCharisma = cha;
            player.baseIntelligence = intel;
            player.items.Clear();
            player.itemsToRecieve.Clear();
            player.head = null;
            player.torso = null;
            player.weapon = null;
            player.shield = null;
            player.gloves = null;
            player.boots = null;
            player.necklace = null;
            player.belt=null;
            player.ring = null;
            player.Exp = 0;
            player.CurrentHealth = player.maxHealth;
            player.expNeededToLvl = 10;
            player.exp = 0;
            player.gold = 0;
            player.fame = 0;
            player.regenerationHP = 2;
            player.regenerationStamina = 1;
            player.staminaMultiplier = 2.75f;
            player.maxStamina = 0;
            player.currentStamina = 0;
            player.expeditionName = "";
            player.trainingCostReduction = 1;
            player.lastRecordedTime = DateTime.Now;
            player.expeditionEnd = DateTime.Now;
            player.arenaCooldown = DateTime.Now;
            player.dungeonCooldown = DateTime.Now;
            player.expeditionNotification = true;
            player.workNotification = true;
            player.dungeonNotification = true;
            player.arenaNotification = false;
            player.shopNotification = false;
            player.Gold += 100;
            player.winStreak = 0;
            player.dungeonBossProgress = 1;
            mp.EverythingStats();
            player.CurrentHealth=player.maxHealth;
            player.CurrentStamina = player.maxStamina;
            player.tutorial = 1;
            await Navigation.PopAsync();
            

        }
        void NextImage_Clicked(object sender, EventArgs e)
        {
            switch (player.source)
            {
                case ("player_male_1"):
                    player.source = "player_male_2";
                    break;
                case ("player_male_2"):
                    player.source = "player_male_3";
                    break;
                case ("player_male_3"):
                    player.source = "player_male_4";
                    break;
                case ("player_male_4"):
                    player.source = "player_female_1";
                    break;
                case ("player_female_1"):
                    player.source = "player_female_2";
                    break;
                case ("player_female_2"):
                    player.source = "player_female_3";
                    break;
                case ("player_female_3"):
                    player.source = "player_female_4";
                    break;
                case ("player_female_4"):
                    player.source = "player_male_1";
                    break;
                default:
                    player.source = "player_male_1";
                    break;
            }
            imageString = player.source;
            playerImage.Source = player.source;
        }
        void PreviousImage_Clicked(object sender, EventArgs e)
        {
            switch (player.source)
            {
                case ("player_male_1"):
                    player.source = "player_female_4";
                    break;
                case ("player_male_2"):
                    player.source = "player_male_1";
                    break;
                case ("player_male_3"):
                    player.source = "player_male_2";
                    break;
                case ("player_male_4"):
                    player.source = "player_male_3";
                    break;
                case ("player_female_1"):
                    player.source = "player_male_4";
                    break;
                case ("player_female_2"):
                    player.source = "player_female_1";
                    break;
                case ("player_female_3"):
                    player.source = "player_female_2";
                    break;
                case ("player_female_4"):
                    player.source = "player_female_3";
                    break;
                default:
                    player.source = "player_male_1";
                    break;
            }
            imageString = player.source;
            playerImage.Source = player.source;
        }
        protected override void OnDisappearing()
        {
            if (player.level != 1) mp.CharacterCreationPage();
            else
            {
                // Call your method or execute code when the page is disappearing
                mp.TUTORIAL1();

                base.OnDisappearing();
            }
        }
    }
}