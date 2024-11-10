using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Android.Content;
using Android.Widget;
using System.Security.Cryptography;

namespace GladiatorRPG
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        Player player;
        Dictionary<string, string> lS;
        MainTabPage mp;
        public Settings(Player player, Dictionary<string, string> localizedStrings, MainTabPage mainPage)
        {
            InitializeComponent();
            lS = localizedStrings;
            this.player = player;
            this.mp = mainPage;
            optionsLabel.Text = localizedStrings["optionText"];
            exportOption.Text = localizedStrings["exportOption"];
            //importOption.Text = localizedStrings["importOption"];
            proceedImport.Text = localizedStrings["importOption"];
            saveImportEntry.Text = localizedStrings["importText"];
            deleteProgress.Text = localizedStrings["deleteProgressOption"];
            otherInformations.Text = localizedStrings["optionOtherText"];
            if (player.workNotification) workNotifications.Text = localizedStrings["notificationsActive"]; else workNotifications.Text = localizedStrings["notificationsInactive"];
            if (player.expeditionNotification) expeditionNotifications.Text = localizedStrings["notificationsActive"]; else expeditionNotifications.Text = localizedStrings["notificationsInactive"];
            if (player.arenaNotification) arenaNotifications.Text = localizedStrings["notificationsActive"]; else arenaNotifications.Text = localizedStrings["notificationsInactive"];
            if (player.dungeonNotification) dungeonNotifications.Text = localizedStrings["notificationsActive"]; else dungeonNotifications.Text = localizedStrings["notificationsInactive"];
            if (player.shopNotification) shopNotifications.Text = localizedStrings["notificationsActive"]; else shopNotifications.Text = localizedStrings["notificationsInactive"];

            LoadTips();
        }

        public async void DeleteProgress()
        {
            bool result = await DisplayAlert(lS["cautionText"], lS["deleteProgressOption1"], lS["yesText"], lS["noText"]);
            if (result)
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");
                //string json = JsonConvert.SerializeObject(player);
                //await Clipboard.SetTextAsync(json);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                await Navigation.PopAsync();
                mp.CharacterCreationPage();

            }
            else
            {

            }

        }
        public async void ImportChar_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert(lS["cautionText"], lS["importOption3"], lS["yesText"], lS["noText"]);
            if (result)
            {
                importOption();
            }
        }

        async void importOption()
        {
            try
            {
                string json = saveImportEntry.Text;
                json = DecryptPlayerData(json);
                mp.player = JsonConvert.DeserializeObject<Player>(json);
                mp.SaveProgress();
                mp.OnAppResume();
                mp.player.lastRecordedTime = DateTime.Now;
                mp.MainModules();
                mp.LoadExpeditions();
                mp.UpdateTrainingCosts();
                mp.CheckTimers();

                TEST(lS["loadedCharacterSuccess"]);
                await Navigation.PopAsync();
            }
            catch
            {
                TEST(lS["loadedCharacterFailed"]);
            }
        }

        private void exportOption_Clicked(object sender, EventArgs e)
        {
            //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "player_data.json");
            string json = JsonConvert.SerializeObject(player);
            json=EncryptPlayerData(json);
            Clipboard.SetTextAsync(json);
            TEST(lS["characterSavedToClipboard"]);
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(memoryStream, player);

            //    // Convert the serialized data to a base64 string
            //    string binaryString = Convert.ToBase64String(memoryStream.ToArray());

            //    Clipboard.SetTextAsync(binaryString);
            //}

        }
        async void TEST(string text)
        {
            await DisplayAlert(lS["cautionText"], text, lS["okText"]);
        }

        private void deleteProgress_Clicked(object sender, EventArgs e)
        {
            DeleteProgress();
        }

        private void saveFileOperations_Clicked(object sender, EventArgs e)
        {
            if (saveFileOperationsGrid.IsVisible == true) saveFileOperationsGrid.IsVisible = false;
            else if (saveFileOperationsGrid.IsVisible == false) saveFileOperationsGrid.IsVisible = true;
        }

        private void notificationOperations_Clicked(object sender, EventArgs e)
        {
            if (notificationOperationsGrid.IsVisible == true) notificationOperationsGrid.IsVisible = false;
            else if (notificationOperationsGrid.IsVisible == false) notificationOperationsGrid.IsVisible = true;
        }

        private void showPhoneNotifOptions_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Intent intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);
                //intent.AddFlags(ActivityFlags.NewTask);
                //intent.AddCategory(Intent.CategoryDefault);
                //intent.SetData(Android.Net.Uri.FromParts("package", Android.App.Application.Context.PackageName, null));
                //Android.App.Application.Context.StartActivity(intent);


                Intent intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);

                intent.AddFlags(ActivityFlags.NewTask);
                intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, Android.App.Application.Context.PackageName);
                intent.PutExtra(Android.Provider.Settings.ExtraChannelId, Android.App.Application.Context.ApplicationInfo.Uid);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddCategory(Intent.CategoryDefault);
                intent.SetData(Android.Net.Uri.FromParts("package", Android.App.Application.Context.PackageName, null));
                Android.App.Application.Context.StartActivity(intent);
            }

        }


        private void workNotif_Clicked(object sender, EventArgs e)
        {
            if (player.workNotification) player.workNotification = false;
            else player.workNotification = true;
            if (player.workNotification) workNotifications.Text = lS["notificationsActive"]; else workNotifications.Text = lS["notificationsInactive"];

            if (!player.workNotification) mp.DeleteNotifications(1);
            else mp.SendNotification(lS["notificationWorkFinished"], lS["notificationWorkFinished1"], 1, player.expeditionEnd);

        }

        private void expNotif_Clicked(object sender, EventArgs e)
        {
            if (player.expeditionNotification) player.expeditionNotification = false;
            else player.expeditionNotification = true;
            if (player.expeditionNotification) expeditionNotifications.Text = lS["notificationsActive"]; else expeditionNotifications.Text = lS["notificationsInactive"];

            if (!player.expeditionNotification) mp.DeleteNotifications(2);
            else mp.SendNotification(lS["notificationExpeditionFinished"], lS["notificationExpeditionFinished1"], 2, player.expeditionEnd);
        }

        private void arenaNotif_Clicked(object sender, EventArgs e)
        {
            if (player.arenaNotification) player.arenaNotification = false;
            else player.arenaNotification = true;
            if (player.arenaNotification) arenaNotifications.Text = lS["notificationsActive"]; else arenaNotifications.Text = lS["notificationsInactive"];

            if (!player.workNotification) mp.DeleteNotifications(3);
            else mp.SendNotification(lS["notificationArenaCooldownFinished"], lS["notificationArenaCooldownFinished1"], 3, player.arenaCooldown);
        }

        private void dungNotif_Clicked(object sender, EventArgs e)
        {
            if (player.dungeonNotification) player.dungeonNotification = false;
            else player.dungeonNotification = true;
            if (player.dungeonNotification) dungeonNotifications.Text = lS["notificationsActive"]; else dungeonNotifications.Text = lS["notificationsInactive"];

            if (!player.workNotification) mp.DeleteNotifications(4);
            else mp.SendNotification(lS["notificationWorkFinished"], lS["notificationWorkFinished1"], 4, player.expeditionEnd);
        }

        private void shopNotif_Clicked(object sender, EventArgs e)
        {
            if (player.shopNotification) player.shopNotification = false;
            else player.shopNotification = true;
            if (player.shopNotification) shopNotifications.Text = lS["notificationsActive"]; else shopNotifications.Text = lS["notificationsInactive"];

            if (!player.workNotification) mp.DeleteNotifications(5);
            else mp.SendNotification(lS["notificationShopResetFinished"], lS["notificationShopResetFinished1"], 5, player.expeditionEnd);
        }

        private void gameTips_Clicked(object sender, EventArgs e)
        {
            if (gameTipsGrid.IsVisible == true) gameTipsGrid.IsVisible = false;
            else if (gameTipsGrid.IsVisible == false) gameTipsGrid.IsVisible = true;
        }
        private void otherOptionButton_Clicked(object sender, EventArgs e)
        {
            if (otherGrid.IsVisible == true) otherGrid.IsVisible = false;
            else if (otherGrid.IsVisible == false) otherGrid.IsVisible = true;
        }
        void LoadTips()
        {
            List<string> list = new List<string> { lS["tipArena"], lS["tipArmor"], lS["tipArmorDamage"], lS["tipBonusStat"], lS["tipCriticalHitChance"],
                lS["tipDamage"], lS["tipDungeonEvents"], lS["tipDungeonMovement"], lS["tipDungeons"], lS["tipExpeditions"], lS["tipExperience"],
                lS["tipFame"], lS["tipFight"], lS["tipFightRounds"], lS["tipGold"], lS["tipHitChance"], lS["tipHitPoints"], lS["tipItems"],
                lS["tipMultiHitChance"], lS["tipMultipliedStat"], lS["tipPlayerLevel"], lS["tipRegeneration"], lS["tipShop"], lS["tipStamina"],
                lS["tipStatistics"], lS["tipWork"], lS["tipArenaReward"]
            };
            list.Sort();
            tip.Text = "";
            FormattedString formattedString = new FormattedString();
            foreach (string s in list)
            {

                string originalText = s;
                string cutText = originalText.Substring(0, originalText.IndexOf("\n"));

                formattedString.Spans.Add(new Span { Text = cutText, FontAttributes = FontAttributes.Bold, FontSize = 16 });

                // Add the rest of the text as a regular span
                formattedString.Spans.Add(new Span { Text = originalText.Substring(originalText.IndexOf("\n")) + "\n\n\n", FontSize = 12 });



            }
            Label label = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FormattedText = formattedString
            };
            gameTipsGrid.Children.Add(label, 0, 0);
        }




        private const string Key = "****";
        private const string IV = "****";


        public string EncryptPlayerData(string jsonData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(jsonData);
                        }
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public string DecryptPlayerData(string encryptedData)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }



            }
        }


    }
}
