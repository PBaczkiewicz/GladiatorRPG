using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace GladiatorRPG
{
    public partial class MainTabPage
    {

        public void LoadDungeons(bool forceUpdate = false)
        {
            TimeSpan dungeonReset = player.dungeonCooldown - DateTime.Now;
            if (dungeonReset < TimeSpan.Zero || forceUpdate)
            {
                noDungeonAvailable.Text = localizedStrings["noDungeonText"];
                dungeonGrid.IsVisible = true;
                dungeonTimer.IsVisible = false;

                dungeonText.Text = localizedStrings["dungeonText"];

                granaryDungeonInfo.Text = localizedStrings["granaryDungeonInfo"];
                granaryDungeonButton.Text = localizedStrings["embarkText"];
                granaryDungeonDifficulty.Text = localizedStrings["granaryDungeonDifficulty"];

                forestDungeonInfo.Text = localizedStrings["forestDungeonInfo"];
                forestDungeonButton.Text = localizedStrings["embarkText"];
                forestDungeonDifficulty.Text = localizedStrings["forestDungeonDifficulty"];

                mountainsDungeonInfo.Text = localizedStrings["mountainsDungeonInfo"];
                mountainsDungeonButton.Text = localizedStrings["embarkText"];
                mountainsDungeonDifficulty.Text = localizedStrings["mountainsDungeonDifficulty"];

                banditCampDungeonInfo.Text = localizedStrings["banditCampDungeonInfo"];
                banditCampDungeonButton.Text = localizedStrings["embarkText"];
                banditCampDungeonDifficulty.Text = localizedStrings["banditCampDungeonDifficulty"];

                desertDungeonInfo.Text = localizedStrings["desertDungeonInfo"];
                desertDungeonButton.Text = localizedStrings["embarkText"];
                desertDungeonDifficulty.Text = localizedStrings["desertDungeonDifficulty"];

                ShowDungeons();
            }
            else
            {
                dungeonTimer.IsVisible = true;
                dungeonGrid.IsVisible = false;

            }
        }

        public void ShowDungeons()
        {
            Label label;
            Button button;
            List<Button> dungeonButtons = new List<Button>();
            Stack<Label> labels = new Stack<Label>();
            Stack<Button> buttons = new Stack<Button>();
            int rowPosition = 0;
            RowDefinition newRow = new RowDefinition { Height = GridLength.Star };

            //Od pierwszego poziomu daje dostęp do wyprawy
            if (player.level >= 5) { rowPosition++; labels.Push(granaryDungeonInfo); labels.Push(granaryDungeonDifficulty); buttons.Push(granaryDungeonButton); }

            //Od piątego poziomu daje dostęp do wyprawy
            if (player.level >= 15) { rowPosition++; labels.Push(forestDungeonInfo); labels.Push(forestDungeonDifficulty); buttons.Push(forestDungeonButton); }


            if (player.level >= 25) { rowPosition++; labels.Push(mountainsDungeonInfo); labels.Push(mountainsDungeonDifficulty); buttons.Push(mountainsDungeonButton); }

            if(player.level>=30) { rowPosition++; labels.Push(banditCampDungeonInfo); labels.Push(banditCampDungeonDifficulty); buttons.Push(banditCampDungeonButton); }

            if(player.level>=35) { rowPosition++; labels.Push(desertDungeonInfo); labels.Push(desertDungeonDifficulty); buttons.Push(desertDungeonButton); }

            if (rowPosition == 0)
            {
                dungeonText.IsVisible = false;
                dungeonDifficultyText.IsVisible = false;
                noDungeonAvailable.IsVisible = true;
            }
            else
            {
                dungeonText.IsVisible = true;
                dungeonDifficultyText.IsVisible = true;
                noDungeonAvailable.IsVisible = false;
            }

            for (int i = 1; i <= rowPosition; i++)
            {
                dungeonButtons.Add(buttons.Peek());
                dungeonGrid.RowDefinitions.Add(newRow);
                label = labels.Pop(); label.IsVisible = true;
                //int currentRow = Grid.GetRow(label);
                Grid.SetRow(label, i);
                label = labels.Pop(); label.IsVisible = true;
                //currentRow = Grid.GetRow(label);
                Grid.SetRow(label, i);
                button = buttons.Pop(); button.IsVisible = true; //button.IsEnabled = false;
                Grid.SetRow(button, i);


            }

            if (player.expeditionInProgress)
            {
                foreach (Button x in dungeonButtons) x.IsEnabled = false;
            }
            else
                foreach (Button x in dungeonButtons) x.IsEnabled = true;



            if (player.dungeonBossProgress<2 &&player.level>=15)
            {
                Grid.SetColumnSpan(forestDungeonInfo, 2);
                forestDungeonInfo.Text = localizedStrings["defeatPreviousBossText"];
                forestDungeonDifficulty.IsVisible = false;
                forestDungeonButton.IsEnabled = false;
            }
            if (player.dungeonBossProgress < 3&&player.level>=25)
            {
                Grid.SetColumnSpan(mountainsDungeonInfo, 2);
                mountainsDungeonInfo.Text = localizedStrings["defeatPreviousBossText"];
                mountainsDungeonDifficulty.IsVisible = false;
                mountainsDungeonButton.IsEnabled = false;
            }
            if (player.dungeonBossProgress < 4&&player.level>=30)
            {
                Grid.SetColumnSpan(banditCampDungeonInfo, 2);
                banditCampDungeonInfo.Text = localizedStrings["defeatPreviousBossText"];
                banditCampDungeonDifficulty.IsVisible = false;
                banditCampDungeonButton.IsEnabled = false;
            }
            if (player.dungeonBossProgress < 5&&player.level>=35)
            {
                Grid.SetColumnSpan(desertDungeonInfo, 2);
                desertDungeonInfo.Text = localizedStrings["defeatPreviousBossText"];
                desertDungeonDifficulty.IsVisible = false;
                desertDungeonButton.IsEnabled = false;
            }


            


        }

        void GranaryDungeon_Clicked(object sender, EventArgs e)
        {

            if (!player.inDungeon) DungeonStart(1);


        }
        void ForestDungeon_Clicked(object sender, EventArgs e)
        {
            if (!player.inDungeon) DungeonStart(2);
        }

        void MountainsDungeon_Clicked(object sender, EventArgs e)
        {
            if (!player.inDungeon) DungeonStart(3);
        }
        void BanditCampDungeon_Clicked(object sender, EventArgs e)
        {
            if (!player.inDungeon) DungeonStart(4);
        }
        void DesertDungeon_Clicked(object sender, EventArgs e)
        {
            if (!player.inDungeon) DungeonStart(5);
        }

        async void DungeonStart(int dungeon = 0)
        {
            if (player.inDungeon)
            {
                DungeonPage dungeonPage = new DungeonPage(localizedStrings, player, 0, this);
                await Navigation.PushAsync(dungeonPage);
            }
            if (dungeon == 0) { return; }
            else
            {
                DungeonPage dungeonPage = new DungeonPage(localizedStrings, player, dungeon, this);
                //player.inDungeon = true;

                await Navigation.PushAsync(dungeonPage);
            }



        }


        private void dungeonText_Clicked(object sender, EventArgs e)
        {
            DungeonInfo();
        }
        async void DungeonInfo()
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["infoText"], localizedStrings["informationsDungeons"], localizedStrings["okText"]);
                clicked = false;
            }
        }

    }


}
