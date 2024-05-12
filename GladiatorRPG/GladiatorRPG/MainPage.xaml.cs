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
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using Xamarin.Forms.PlatformConfiguration;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using static Android.Resource;

namespace GladiatorRPG
{


    public partial class MainTabPage : TabbedPage
    {
        #region Game Balancing Options

        #region Szansa na przedmioty
        float baseDropChanceGreen7To14 = 75f;

        float baseDropChanceBlue14To21 = 25f;

        float baseDropChanceViolet21To28 = 25f;
        float baseDropChanceBlue21To28 = 35f;

        float baseDropChanceOrange28To35 = 2f;
        float baseDropChanceViolet28To35 = 40f;
        float baseDropChanceBlue28To35 = 45f;

        float baseDropChanceRed35Up = 1f;
        float baseDropChanceOrange35Up = 5f;
        float baseDropChanceViolet35Up = 45f;
        float baseDropChanceBlue35Up = 35f;
        #endregion

        #region Koszt przedmiotów o określonej rzadkości
        //na 10 lvlu cena zielonych powinna być około 2k -> jest ok 2.5k
        //na ~35 lvlu cena fioletowych powinna być około 25k -> jest ok 30k
        int greenAffixValue = 30; //było 100, teraz mnożnik od poziomu razy 2 (od 7 lvla min+14
        int blueAffixValue = 70; //było 125, teraz mnożnik od poziomu razy 3 (od 14 lvla min+42
        int violetAffixValue = 90;//było 150, teraz mnożnik od poziomu razy 4 (od 21 lvla min+84
        int orangeAffixValue = 180;//było 175, teraz mnożnik od poziomu razy 5 (od 28 lvla min+140
        int redAffixValue = 240;//było 200, teraz mnożnik od poziomu razy 6 (od 35 lvla min+210
        #endregion

        //Czas pomiędzy turami w ms
        public int fightSleepTime = 750;
        #endregion
        //Przycisk przejścia do testowej walki
        public void Test_Clicked(object sender, EventArgs e)
        {
            string wersja = DeviceInfo.Version.ToString();
            float version = float.Parse(wersja, CultureInfo.InvariantCulture);
            TEST(version.ToString());

        }
        async public void TEST(string tekst)
        {
            await DisplayAlert(cautionText, tekst, okText);

        }


        public void SetImage()
        {
            playerAvatar.Source = player.source;
        }

        void optionButton_Clicked(object sender, EventArgs e)
        {
            OptionsScreen();
        }

        async void OptionsScreen()
        {
            if (!clicked)
            {
                clicked = true;
                Settings settings = new Settings(player, localizedStrings, this);

                await Navigation.PushAsync(settings);
                clicked = false;
            }
        }

        public void CheckPlayerOptions()
        {
            if (player.eqDisplay) { playerEquippedDisplay.IsVisible = true; playerEquippedText.Text = localizedStrings["optionHideEq"]; } else { playerEquippedDisplay.IsVisible = false; playerEquippedText.Text = localizedStrings["optionShowEq"]; }
            if (player.inventoryDisplay) { playerInventoryDisplay.IsVisible = true; eqLabel.Text = localizedStrings["optionHideInventory"]; } else { playerInventoryDisplay.IsVisible = false; eqLabel.Text = localizedStrings["optionShowInventory"]; }
        }


        //Zmiana awatara gracza, prawdopodobnie do wywalenia w późniejszej fazie produkcji
        void playerAvatar_Clicked(object sender, EventArgs e)
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
            playerAvatar.Source = player.source;
        }
        //Zmienne
        #region Essential variables
        Random random = new Random();
        bool activeDialogBox = false;
        TimeSpan duration;
        Button expeditionButton;
        List<Button> squareList;
        List<Image> imageList;
        List<Image> borderList;
        List<Button> buttonList = new List<Button>();
        List<Item> shop = new List<Item>();
        int regenOccurence = 60;//in seconds
        public string remainingTimeFormatted = "--:--";
        TimeSpan remainingTime;
        [JsonProperty]
        public Player player;
        Entity enemy { get; set; }
        private bool isTimerRunning;
        public int canceledDungeonTicks;
        public bool inDungeon = false, minimized = false;
        //private ContentPage lastVisitedPage;
        public Dictionary<string, string> localizedStrings;
        int strCost, perCost, dexCost, agiCost, vitCost, endCost, chaCost, intCost;
        #endregion
        //Uruchomienie programu
        #region Startup
        //Wywołanie programu
        public MainTabPage()
        {
            InitializeComponent();
            player = new Player();
            notificationManager = DependencyService.Get<INotificationManager>();
            localizedStrings = GetAllLocalizedStrings();
            Localization(localizedStrings);
            NavigationPage.SetHasNavigationBar(this, false);
            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");
            if (!File.Exists(fileName)) { CharacterCreationPage(); }
            else
            {
                LoadProgress();
                if (player == null || player.baseStrength == 0) { player = new Player(); CharacterCreationPage(); }
                playerAvatar.Source = player.source;
                try
                {
                    if (player.inDungeon && player.dungeonExpedition.currentEvent != null)
                    {
                        //duration = DateTime.Now - player.lastRecordedTime;
                        //canceledDungeonTicks = (int)Math.Floor((float)duration.TotalSeconds / 10f);
                        //ContinueDungeon();
                        player.inDungeon = false;//DO USUNIĘCIA PO NAPRAWIE DUNGEONÓW
                    }
                    else
                    {
                        player.inDungeon = false;
                    }
                }
                catch
                {
                    player.inDungeon = false;
                    player.dungeonExpedition = null;
                    //TEST("ERROR OCCURED WHILE TRYING TO LOAD DUNGEON");
                    //LOADING DUNGEON IS NOT WORKING AT THE MOMENT AND PROBABLY WON'T BE EVER (BECAUSE IDC)
                }
            }
            OnAppResume();
            player.lastRecordedTime = DateTime.Now;
            MainModules();
            LoadExpeditions();
            UpdateTrainingCosts();
            CheckTimers();
            CheckTutorial(2000);


        }
        async void CheckTutorial(int delay)
        {
            if (player.tutorial == 1)
            {
                await TUTORIAL(localizedStrings["tutorial1"], delay);
            }
            else if (player.tutorial == 2)
            {
                await TUTORIAL(localizedStrings["tutorial2"], delay);

            }
            else if (player.tutorial == 3)
            {
                await TUTORIAL(localizedStrings["tutorial3"], delay);

            }
            else if (player.tutorial == 4)
            {
                player.tutorial = 0;
                //player.shopDate = DateTime.Now;

            }
        }
        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
            });
        }
        INotificationManager notificationManager;

        async void LEVELUPTEST()
        {

            int level = 5;
            for (int i = 0; i < 8; i++)
            {
                string infoText = "";
                if (level == 5) infoText = "\n" + localizedStrings["levelUp5Info"];
                else if (level == 10) infoText = "\n" + localizedStrings["levelUp10Info"];
                else if (level == 15) infoText = "\n" + localizedStrings["levelUp15Info"];
                else if (level == 20) infoText = "\n" + localizedStrings["levelUp20Info"];
                else if (level == 25) infoText = "\n" + localizedStrings["levelUp25Info"];
                else if (level == 30) infoText = "\n" + localizedStrings["levelUp30Info"];
                else if (level == 35) infoText = "\n" + localizedStrings["levelUp35Info"];
                await DisplayAlert(localizedStrings["levelUpText"], localizedStrings["levelUpText1"] + level + "!\n" + localizedStrings["levelUpText2"] + "REWARD" + " " + localizedStrings["itemGoldText"] + infoText, localizedStrings["okText"]);
                level += 5;
            }
        }
        public void CheckTimers()
        {
            currentTime = DateTime.Now;
            if (player.arenaCooldown <= currentTime)
            {
                LoadArena();
                EnableArenaButtons();
                arenaNextDuelTimer.IsVisible = false;
                arenaSpeedup.IsVisible = false;
            }
            else
            {

                DisableArenaButtons();
                arenaNextDuelTimer.IsVisible = true;
                arenaSpeedup.IsVisible = true;
            }
            if (player.shopDate < currentTime) { LoadShop(true); }
            if (player.dungeonCooldown < currentTime) { LoadDungeons(true); }

        }

        async void ContinueDungeon()
        {
            bool result = await DisplayAlert(localizedStrings["cautionText"], localizedStrings["dungeonUnfinishedMessage"], localizedStrings["yesText"], localizedStrings["noText"]);

            if (result)
            {
                DungeonStart();
                canceledDungeonTicks = 0;
            }
            else
            {
                player.inDungeon = false;
                EachTick(canceledDungeonTicks);
                return;
            }
        }


        //Funkcje wywołujące się przy uruchomieniu aplikacji
        public void MainModules()
        {
            if (expeditionButton == null)
            {
                ExpeditionResume();
            }

            //Subskrybcja zdarzeń
            EventSubscription();
            UpdateItemsToRecieve();
            UpdateExperience();
            fame.Text = fameText + " : " + player.Fame; dmgLabel.Text = damText; staminaLabel.Text = stamText;
            gold.Text = goldText + " : " + player.Gold + localizedStrings["itemGoldText"]; hpLabel.Text = lifeText; eqLabel.Text = eqText;
            gold2.Text = goldText + " : " + player.Gold + localizedStrings["itemGoldText"]; fame2.Text = fameText + " : " + player.Fame;
            gold3.Text = goldText + " : " + player.Gold + localizedStrings["itemGoldText"];

            GridEquipment();
            EverythingStats();
            ButtonListComposing();
            CheckPlayerOptions();
            LoadEverything();
        }

        void LoadEverything()
        {
            UpdateTrainingCosts();
            UpdateItemsToRecieve();
            LoadExpeditions();
            LoadDungeons();
            LoadWorkPage();
            LoadShop();
            LoadArena();
        }
        #endregion
        //Zachowanie przycisków
        #region Buttons
        void ButtonListComposing()
        {
            buttonList.Clear();
            buttonList.Add(granaryButton);
            buttonList.Add(forestButton);
            buttonList.Add(mountainsButton);
            buttonList.Add(banditCampButton);

            buttonList.Add(granaryDungeonButton);
            buttonList.Add(forestDungeonButton);

            buttonList.Add(workButton1);
            buttonList.Add(workButton2);
            buttonList.Add(workButton3);
            buttonList.Add(workButton4);
        }

        void ShowEquippedItems(object sender, EventArgs e)
        {
            if (playerEquippedDisplay.IsVisible)
            {
                playerEquippedDisplay.IsVisible = false;
                player.eqDisplay = false;
            }
            else
            {
                playerEquippedDisplay.IsVisible = true;
                player.eqDisplay = true;
            }
            if (player.eqDisplay) { playerEquippedDisplay.IsVisible = true; playerEquippedText.Text = localizedStrings["optionHideEq"]; } else { playerEquippedDisplay.IsVisible = false; playerEquippedText.Text = localizedStrings["optionShowEq"]; }

        }

        void ShowInventory(object sender, EventArgs e)
        {
            if (playerInventoryDisplay.IsVisible)
            {
                playerInventoryDisplay.IsVisible = false;
                player.inventoryDisplay = false;
            }
            else
            {
                playerInventoryDisplay.IsVisible = true;
                player.inventoryDisplay = true;
            }
            if (player.inventoryDisplay) { playerInventoryDisplay.IsVisible = true; eqLabel.Text = localizedStrings["optionHideInventory"]; } else { playerInventoryDisplay.IsVisible = false; eqLabel.Text = localizedStrings["optionShowInventory"]; }

        }


        #endregion
        //Operowanie programem
        #region Application manipulation
        //Funkcja odbywająca się przy zmaksymalizowaniu aplikacji
        protected override void OnAppearing()
        {
            //base.OnAppearing();
            if (minimized) OnAppResume();
            StartTimer();
            minimized = false;
            CheckTimers();
            App.AppActive = true;
            //ClearNotifications();
        }

        protected override void OnDisappearing()
        {

            player.lastRecordedTime = DateTime.Now;
            SaveProgress();
            minimized = true;
            App.AppActive = false;
            //isTimerRunning = false;

        }
        //Wywołuje się przy przejściu na ekran treningu, aktualizuje koszt szkolenia statystyk

        #endregion
        #region Navigation/Fights

        //funkcja testowa wysyłająca do walki z losowym przeciwnikiem

        public async void Fight(Player player, Entity enemy, int gold, int exp, int itemChance = 0, int items = 0, int itemRarity = 0, int rounds = 30, bool isArena = false)
        //Wymagane player, enemy. Reszta opcjonalna gold, exp -> złoto i exp dla gracza,
        //itemChance-> szansa na wylosowanie przedmiotu
        //items -> ilość losowań
        //itemRarity -> rzadkość przedmiotu, 0=losowa, 2-6=określona, inne=biały
        //rounds -> maksymalna liczba rund
        {
            duration = DateTime.Now - player.lastRecordedTime;
            player.lastRecordedTime = DateTime.Now;
            int ticks = (int)Math.Floor((float)duration.TotalSeconds / 60f);
            if (ticks > 0)
            {
                duration = TimeSpan.Zero;
                if(!player.inDungeon) EachTick(ticks);
            }
            FightPage fightPage = new FightPage(localizedStrings, this, player, enemy, exp, gold, itemChance, items, itemRarity, rounds, isArena);

            await Navigation.PushAsync(fightPage);
            UpdateItemsToRecieve();
        }
        #endregion
        //Timer tykający co sekundę, co minutę wywołuje funkcje
        #region Timers
        //Kliknięcie przedmiotu w ekwipunku#region Timers & methods for them
        //Timer tykający co sekundę(zapisuje postęp), sprawdza czas urządzenia i co pełną minutę odbywa funkcję

        public async void CharacterCreationPage()
        {
            CharacterCreation creationPage = new CharacterCreation(localizedStrings, player, this);
            playerAvatar.Source = player.source;
            await Navigation.PushAsync(creationPage);
        }
        bool regenStopper = false;



        string shopResetFormatted, arenaTimerFormatted, dungeonResetFormatted;



        DateTime currentTime;
        //TimeSpan shopReset, dungeonReset, arenaTimer;
        TimeSpan timeRemaining;
        private void StartTimer()
        {
            if (!isTimerRunning)
            {
                isTimerRunning = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {


                    currentTime = DateTime.Now;
                    timerLabel.Text = currentTime.ToString("HH:mm:ss");

                    if (player.shopDate.AddSeconds(1) >= currentTime)
                    {
                        timeRemaining = player.shopDate - currentTime;
                        shopResetFormatted = $"{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}";
                        shopTimer.Text = localizedStrings["timeToShopReset"] + shopResetFormatted;
                        if (player.shopDate <= currentTime) { LoadShop(true); }
                    }


                    if (player.dungeonCooldown.AddSeconds(1) >= currentTime)
                    {
                        timeRemaining = player.dungeonCooldown - currentTime;
                        dungeonResetFormatted = $"{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}";
                        dungeonTimer.Text = "\n\n\n" + localizedStrings["dungeonOpensIn"] + dungeonResetFormatted;
                        if (player.dungeonCooldown <= currentTime)
                        {

                            LoadDungeons(true);
                        }
                    }

                    if (player.arenaCooldown.AddSeconds(1) >= currentTime)
                    {
                        timeRemaining = player.arenaCooldown - currentTime;
                        arenaTimerFormatted = $"{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}";
                        arenaNextDuelTimer.Text = localizedStrings["arenaNextFight"] + arenaTimerFormatted;
                        if (player.arenaCooldown <= currentTime)
                        {
                            arenaNextDuelTimer.IsVisible = false;
                            arenaSpeedup.IsVisible = false;
                            EnableArenaButtons();
                        }

                    }

                    if (DateTime.Now.Second % regenOccurence == 0 && !regenStopper) //co minutę wywołuje EachTick()
                    {

                        regenStopper = true;
                        EachTick();
                    }
                    else
                    {
                        regenStopper = false;
                    }
                    if (player.expeditionInProgress)
                    {

                        remainingTime = player.expeditionEnd - DateTime.Now;
                        remainingTimeFormatted = $"{remainingTime.Hours:00}:{remainingTime.Minutes:00}:{remainingTime.Seconds:00}";
                        if (expeditionButton == null) { ExpeditionResume(); }
                        expeditionButton.Text = remainingTimeFormatted.ToString();
                        if (remainingTime <= TimeSpan.Zero)
                        {
                            expeditionButton.Text = "--:--";
                            ExpeditionArrive();
                            CheckWork();
                            expeditionButton.Text = localizedStrings["embarkText"];
                            quickenExpedition.IsVisible = false;
                            player.expeditionInProgress = false;
                            LoadWorkPage();
                            foreach (Button x in buttonList) { x.IsEnabled = true; }
                        }
                    }
                    SaveProgress();
                    return isTimerRunning;
                });
                isTimerRunning = true;
                player.lastRecordedTime = DateTime.Now;
            }
        }


        //Po wznowieniu aplikacji sprawdza ile czasu minęło od zablokowania/zminimalizowania i zależnie od ilości minut odnawia surowce itd.
        public void OnAppResume()
        {

            //liczy upłynięty czas
            duration = DateTime.Now - player.lastRecordedTime;
            player.lastRecordedTime = DateTime.Now;
            int ticks = (int)Math.Floor((float)duration.TotalSeconds / 60f);
            duration = TimeSpan.Zero;
            EachTick(ticks);

        }






        //funkcja odbywająca się co minutę
        public void EachTick(int ticks = 1)
        {
            if (player.inDungeon == false)
            {
                player.CurrentHealth += ticks * player.regenerationHP;
                player.CurrentStamina += ticks * player.regenerationStamina;
            }
        }

        #endregion
        //Jeszcze nie wiem jakie mają być konsekwencje śmierci
        void Die()
        {
            player.CurrentHealth = 1;
        }
        //Zapis i odczyt
        #region Save&Load
        public async void OldSaveProgress()
        {
            player.lastRecordedTime = DateTime.Now;
            string json = JsonConvert.SerializeObject(player);
            Application.Current.Properties["Player"] = json;
            await Application.Current.SavePropertiesAsync();
            //SaveToFile();

        }
        public void OLDLoadProgress()
        {
            if (Application.Current.Properties.ContainsKey("Player"))
            {
                string playerJson = (string)Application.Current.Properties["Player"];

                player = JsonConvert.DeserializeObject<Player>(playerJson);
            }
            else
            {
                TEST("Save file not found!");
            }
            //if (File.Exists("player_data.bin"))
            //{
            //    LoadFromFile();
            //}
        }
        public void DeleteProgress()
        {
            if (Application.Current.Properties.ContainsKey("Player"))
            {
                Application.Current.Properties.Remove("Player");
            }
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
        #region Hard save
        string fileName = "";
        public void SaveProgress()
        {
            try
            {
                if (player == null) return;
                string json = JsonConvert.SerializeObject(player);
                File.WriteAllText(fileName, json);

            }
            catch { return; }

        }

        public void manualSave_Clicked(object sender, EventArgs e)
        {
            SaveProgress();
        }

        void SaveExternal(object sender, EventArgs e)
        {
            //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");
            //string json = JsonConvert.SerializeObject(player);
            ////Clipboard.SetTextAsync(json);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, player);

                // Convert the serialized data to a base64 string
                string binaryString = Convert.ToBase64String(memoryStream.ToArray());

                Clipboard.SetTextAsync(binaryString);
            }

        }


        public bool LoadProgress()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");

            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);

                player = JsonConvert.DeserializeObject<Player>(json);
                //TEST("Loaded from file");
                return true;
            }
            else
            {
                //TEST("Loaded from properties");
                return false;
            }
        }
        #endregion
        #endregion
        async void RecieveItemButton_Clicked(object sender, EventArgs e)
        {
            if (player.itemsToRecieve.Count > 0)
            {
                //ItemClass itemClass = ItemClass.nothing;
                //if (player.itemClass!=null) { itemClass = player.itemClass.Dequeue(); }
                //player.items.Add(ItemGenerator(player.itemLevel.First(), player.itemLevel.Dequeue(), player.itemRarity.Dequeue(), itemClass));
                //player.itemsToRecieve--;
                if (player.items.Count < 12)
                {
                    player.items.Add(player.itemsToRecieve.Dequeue());
                    UpdateItemsToRecieve();
                    GridEquipment();
                }
                else
                {
                    await DisplayAlert(cautionText, noSpaceInInventoryText, okText);

                }
            }
        }
        public void UpdateItemsToRecieve()
        {
            if (player.itemsToRecieve != null && player.itemsToRecieve.Count > 0)
            {
                recieveItemButton.IsEnabled = true;
                recieveItemLabel.Text = player.itemsToRecieve.Count.ToString() + localizedStrings["itemsToRecieve"];
            }
            else
            {
                recieveItemButton.IsEnabled = false;
                recieveItemLabel.Text = localizedStrings["noItemsToRecieve"];
            }
        }
        //NARESZCIE
        private void testButton_Clicked(object sender, EventArgs e)
        {
            //SendNotification("Gladiator RPG", "Testowe powiadomienie", 0);
            //SendNotification("Gladiator RPG", "Pierwsze powiadomienia po czasie", 0, DateTime.Now.AddSeconds(3));
            //LEVELUPTEST();
        }
        private void testButton1_Clicked(object sender, EventArgs e)
        {
            //SendNotification("Gladiator RPG", "Testowe powiadomienie", 0);
            //SendNotification("Gladiator RPG", "Powiadomienie nr " + xd.ToString(), 9, DateTime.Now.AddSeconds(3));
        }
        private void testButton2_Clicked(object sender, EventArgs e)
        {
            //SendNotification("Gladiator RPG", "Testowe powiadomienie", 0);
            DeleteNotifications(9);

        }

        public void SendNotification(string title, string message, int id, DateTime date)
        {
            //notificationManager.DeleteNotification(id);
            notificationManager.SendNotification(title, message, id, date);
        }
        void ClearNotifications()
        {
            DeleteNotifications(1);
            DeleteNotifications(2);
            DeleteNotifications(3);
            DeleteNotifications(4);
            DeleteNotifications(5);
        }
        public void DeleteNotifications(int id)
        {
            notificationManager.DeleteNotification(id);
        }
        private void goalsButton_Clicked(object sender, EventArgs e)
        {
            Goals();

        }
        async void Goals()
        {
            string done = localizedStrings["goalsDone"];
            string todo = localizedStrings["goalsTodo"];
            string txt = "";
            int counter = 0;
            if (player.defeatEasy) { txt += done; counter++; } else txt += todo;
            txt += localizedStrings["goalsArenaEasy"] + "\n\n";
            if (player.defeatMedium) { txt += done; counter++; } else txt += todo;
            txt += localizedStrings["goalsArenaMedium"] + "\n\n";
            if (player.defeatHard) { txt += done; counter++; } else txt += todo;
            txt += localizedStrings["goalsArenaHard"] + "\n\n";
            if (player.defeatColliseum) { txt += done; counter++; } else txt += todo;
            txt += localizedStrings["goalsArenaColliseum"] + "\n\n";

            if (player.level < 5) txt += todo; else { txt += done; counter++; }
            txt += localizedStrings["goals5"] + "\n\n";

            if (player.level < 7) txt += todo; else { txt += done; counter++; }
            if (player.level < 5) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals7"];
            txt += "\n\n";

            if (player.level < 10) txt += todo; else { txt += done; counter++; }
            if (player.level < 7) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals10"];
            txt += "\n\n";

            if (player.level < 14) txt += todo; else { txt += done; counter++; }
            if (player.level < 10) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals14"];
            txt += "\n\n";

            if (player.level < 15 || player.dungeonBossProgress < 2) txt += todo; else { txt += done; counter++; }
            if (player.level < 15) txt += localizedStrings["goalsUnknown"]; 
            else txt += localizedStrings["goals15"];
            txt += "\n\n";

            if (player.level < 20) txt += todo; else { txt += done; counter++; }
            if (player.level < 15) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals20"];
            txt += "\n\n";

            if (player.level < 21) txt += todo; else { txt += done; counter++; }
            if (player.level < 20) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals21"];
            txt += "\n\n";

            if (player.level < 25 || player.dungeonBossProgress < 3) txt += todo; else { txt += done; counter++; }
            if (player.level < 25) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals25"];
            txt += "\n\n";

            if (player.level < 28) txt += todo; else { txt += done; counter++; }
            if (player.level < 25) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals28"];
            txt += "\n\n";

            if (player.level < 30) txt += todo; else { txt += done; counter++; }
            if (player.level < 28) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals30"];
            txt += "\n\n";

            if (player.level < 30 || player.dungeonBossProgress < 4) txt += todo; else { txt += done; counter++; }
            if (player.level < 30) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals30_1"];
            txt += "\n\n";

            if (player.level < 35) txt += todo; else { txt += done; counter++; }
            if (player.level < 30) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals35_1"];
            txt += "\n\n";

            if (player.level < 35 || player.dungeonBossProgress < 5) txt += todo; else { txt += done; counter++; }
            if (player.level < 35) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goals35"];
            txt += "\n\n";

            if (player.level < 35 || player.dungeonBossProgress < 6) txt += todo; else { txt += done; counter++; }
            if (player.level < 35) txt += localizedStrings["goalsUnknown"]; else txt += localizedStrings["goalsLast"];
            txt += "\n\n";

            //18 celi
            if (counter == 18)
            {
                txt += done;
                txt += localizedStrings["goalsAll"] + localizedStrings["goalsAllReward"] + "\n\n\n";
                txt += localizedStrings["goalsAllDone"];
            }
            else
            {
                txt += todo;
                txt += localizedStrings["goalsAll"] + localizedStrings["goalsUnknown"];
                txt += "\n" + counter + "/18";
                txt += "\n\n";
            }


            await DisplayAlert(localizedStrings["goalsText"], txt, okText);
        }
    }

    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }


}