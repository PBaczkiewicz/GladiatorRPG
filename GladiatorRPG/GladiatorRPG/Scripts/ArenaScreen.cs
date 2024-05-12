using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using static Android.Resource;

namespace GladiatorRPG
{
    public partial class MainTabPage
    {
        List<string> arenaNameList;
        public List<string> arenaImageList;
        public int arenaDifficulty;
        void LoadArena()
        {
            UpdateArena();
            ShowReward();

        }
        void RollArenaReward()
        {
            if (player.Fame > (player.level + 1) * 10)
            {
                int rewardCost = player.fame - player.level * 10;
                player.arenaReward = ItemGenerator(player.level + (((int)Math.Floor((float)rewardCost / 10) - 2)), player.level + (((int)Math.Floor((float)rewardCost / 10) - 2)), -1);
                player.arenaReward.buyValue = rewardCost;
                ShowReward();
            }
            else
            {
                player.arenaReward = null;
                return;
            }
        }
        void ShowReward()
        {
            if (player.arenaReward != null)
            {
                arenaRewardGrid.IsVisible = true;
                recieveArenaReward.Text = "\n\n\n\n\n\n\n" + localizedStrings[player.arenaReward.itemName];
                arenaImage.Source = player.arenaReward.itemImage;
                if (player.arenaReward.color.ToArgb() == System.Drawing.Color.White.ToArgb()) { arenaBorder.Source = "borderWhite"; }
                else if (player.arenaReward.color.ToArgb() == System.Drawing.Color.LightGreen.ToArgb()) { arenaBorder.Source = "borderGreen"; }
                else if (player.arenaReward.color.ToArgb() == System.Drawing.Color.DodgerBlue.ToArgb()) { arenaBorder.Source = "borderBlue"; }
                else if (player.arenaReward.color.ToArgb() == System.Drawing.Color.DarkViolet.ToArgb()) { arenaBorder.Source = "borderViolet"; }
                else if (player.arenaReward.color.ToArgb() == System.Drawing.Color.Orange.ToArgb()) { arenaBorder.Source = "borderOrange"; }
                else if (player.arenaReward.color.ToArgb() == System.Drawing.Color.Red.ToArgb()) { arenaBorder.Source = "borderRed"; }
                else { arenaBorder.Source = null; }
                arenaReward.Text = ArenaRewardDescription(player.arenaReward);
                int newlines = arenaReward.Text.Count(c => c == '\n');
                arenaReward.FontSize = 10;
                while (newlines >= 8 && arenaReward.FontSize != 3)
                {
                    arenaReward.FontSize -= 1;
                    newlines--;
                }

            }
            else arenaRewardGrid.IsVisible = false;
        }
        string ArenaRewardDescription(Item i)
        {
            string description = "";
            //i.UpdateDescription(localizedStrings);//tests
            description += "\n" + localizedStrings["itemLevel"] + " " + i.levelReq.ToString();
            if (i.itemClass == ItemClass.Weapon) { description += " " + localizedStrings["itemWeapon"]; }//if (player.weapon != null) { equippedItem = player.weapon; } }
            else if (i.itemClass == ItemClass.Helmet) { description += " " + localizedStrings["itemHelmet"]; }//if (player.head!= null) { equippedItem = player.head; } }
            else if (i.itemClass == ItemClass.Torso) { description += " " + localizedStrings["itemTorso"]; }//if (player.torso!= null) { equippedItem = player.torso; } }
            else if (i.itemClass == ItemClass.Gloves) { description += " " + localizedStrings["itemGloves"]; }//if (player.gloves!= null) { equippedItem = player.gloves; } }
            else if (i.itemClass == ItemClass.Boots) { description += " " + localizedStrings["itemBoots"]; }// if (player.boots!= null) { equippedItem = player.boots; } }
            else if (i.itemClass == ItemClass.Shield) { description += " " + localizedStrings["itemShield"]; }//if (player.shield!= null) { equippedItem = player.shield; } }
            else if (i.itemClass == ItemClass.Ring) { description += " " + localizedStrings["itemRing"]; }//if (player.ring!= null) { equippedItem = player.ring; } }
            else if (i.itemClass == ItemClass.Belt) { description += " " + localizedStrings["itemBelt"]; }//if (player.belt!= null) { equippedItem = player.belt; } }
            else if (i.itemClass == ItemClass.Necklace) { description += " " + localizedStrings["itemNecklace"]; }// if (player.necklace != null) { equippedItem = player.necklace; } }

            if (i.armorValue > 0) { description += "\n" + localizedStrings["itemArmor"] + " " + i.armorValue.ToString(); }
            //if (equippedItem.armorValue > 0) { description += " -> " + localizedStrings["itemArmor"] + " " + equippedItem.armorValue.ToString(); }

            if (i.minDamage > 0 || i.maxDamage > 0) { description += "\n" + localizedStrings["damText"] + " " + i.minDamage.ToString() + "-" + i.maxDamage.ToString(); }
            //if (equippedItem.minDamage > 0 || equippedItem.maxDamage > 0) { description += " -> " + equippedItem.minDamage.ToString() + "-" + equippedItem.maxDamage.ToString(); }

            if (i.damageBonus > 0) { description += "\n" + localizedStrings["damText"] + " +" + i.damageBonus.ToString(); }
            else if (i.damageBonus < 0) { description += "\n" + localizedStrings["damText"] + " " + i.damageBonus.ToString(); }

            if (i.strengthBonus > 0) { description += "\n" + localizedStrings["strText"] + " +" + i.strengthBonus.ToString(); }
            else if (i.strengthBonus < 0) { description += "\n" + localizedStrings["strText"] + " " + i.strengthBonus.ToString(); }
            if (i.strengthMultiplier > 0) { description += "\n" + localizedStrings["strText"] + " +" + i.strengthMultiplier.ToString() + "%"; }
            else if (i.strengthMultiplier < 0) { description += "\n" + localizedStrings["strText"] + " " + i.strengthMultiplier.ToString() + "%"; }

            if (i.perceptionBonus > 0) { description += "\n" + localizedStrings["perText"] + " +" + i.perceptionBonus.ToString(); }
            else if (i.perceptionBonus < 0) { description += "\n" + localizedStrings["perText"] + " " + i.perceptionBonus.ToString(); }
            if (i.perceptionMultiplier > 0) { description += "\n" + localizedStrings["perText"] + " +" + i.perceptionMultiplier.ToString() + "%"; }
            else if (i.perceptionMultiplier < 0) { description += "\n" + localizedStrings["perText"] + " " + i.perceptionMultiplier.ToString() + "%"; }

            if (i.dexterityBonus > 0) { description += "\n" + localizedStrings["dexText"] + " +" + i.dexterityBonus.ToString(); }
            else if (i.dexterityBonus < 0) { description += "\n" + localizedStrings["dexText"] + " " + i.dexterityBonus.ToString(); }
            if (i.dexterityMultiplier > 0) { description += "\n" + localizedStrings["dexText"] + " +" + i.dexterityMultiplier.ToString() + "%"; }
            else if (i.dexterityMultiplier < 0) { description += "\n" + localizedStrings["dexText"] + " " + i.dexterityMultiplier.ToString() + "%"; }

            if (i.agilityBonus > 0) { description += "\n" + localizedStrings["agiText"] + " +" + i.agilityBonus.ToString(); }
            else if (i.agilityBonus < 0) { description += "\n" + localizedStrings["agiText"] + " " + i.agilityBonus.ToString(); }
            if (i.agilityMultiplier > 0) { description += "\n" + localizedStrings["agiText"] + " +" + i.agilityMultiplier.ToString() + "%"; }
            else if (i.agilityMultiplier < 0) { description += "\n" + localizedStrings["agiText"] + " " + i.agilityMultiplier.ToString() + "%"; }

            if (i.vitalityBonus > 0) { description += "\n" + localizedStrings["vitText"] + " +" + i.vitalityBonus.ToString(); }
            else if (i.vitalityBonus < 0) { description += "\n" + localizedStrings["vitText"] + " " + i.vitalityBonus.ToString(); }
            if (i.vitalityMultiplier > 0) { description += "\n" + localizedStrings["vitText"] + " +" + i.vitalityMultiplier.ToString() + "%"; }
            else if (i.vitalityMultiplier < 0) { description += "\n" + localizedStrings["vitText"] + " " + i.vitalityMultiplier.ToString() + "%"; }

            if (i.enduranceBonus > 0) { description += "\n" + localizedStrings["endText"] + " +" + i.enduranceBonus.ToString(); }
            else if (i.enduranceBonus < 0) { description += "\n" + localizedStrings["endText"] + " " + i.enduranceBonus.ToString(); }
            if (i.enduranceMultiplier > 0) { description += "\n" + localizedStrings["endText"] + " +" + i.enduranceMultiplier.ToString() + "%"; }
            else if (i.enduranceMultiplier < 0) { description += "\n" + localizedStrings["endText"] + " " + i.enduranceMultiplier.ToString() + "%"; }

            if (i.charismaBonus > 0) { description += "\n" + localizedStrings["chaText"] + " +" + i.charismaBonus.ToString(); }
            else if (i.charismaBonus < 0) { description += "\n" + localizedStrings["chaText"] + " " + i.charismaBonus.ToString(); }
            if (i.charismaMultiplier > 0) { description += "\n" + localizedStrings["chaText"] + " +" + i.charismaMultiplier.ToString() + "%"; }
            else if (i.charismaMultiplier < 0) { description += "\n" + localizedStrings["chaText"] + " " + i.charismaMultiplier.ToString() + "%"; }

            if (i.intelligenceBonus > 0) { description += "\n" + localizedStrings["intText"] + " +" + i.intelligenceBonus.ToString(); }
            else if (i.intelligenceBonus < 0) { description += "\n" + localizedStrings["intText"] + " " + i.intelligenceBonus.ToString(); }
            if (i.intelligenceMultiplier > 0) { description += "\n" + localizedStrings["intText"] + " +" + i.intelligenceMultiplier.ToString() + "%"; }
            else if (i.intelligenceMultiplier < 0) { description += "\n" + localizedStrings["intText"] + " " + i.intelligenceMultiplier.ToString() + "%"; }

            if (i.healthBonus > 0) { description += "\n" + localizedStrings["itemHealth"] + " +" + i.healthBonus.ToString(); }
            else if (i.healthBonus < 0) { description += "\n" + localizedStrings["itemHealth"] + " " + i.healthBonus.ToString(); }
            if (i.armorBonus > 0) { description += "\n" + localizedStrings["itemArmor"] + " +" + i.armorBonus.ToString(); }
            else if (i.armorBonus < 0) { description += "\n" + localizedStrings["itemArmor"] + " " + i.armorBonus.ToString(); }

            description += "\n" + localizedStrings["itemBuyValue"] + " : " + player.arenaReward.buyValue + localizedStrings["fameCostText"];


            return description;
        }
        void UpdateArena()
        {
            arenaButton.Text = localizedStrings["arenaTextInfo"];
            arenaEasyFightText.Text = localizedStrings["arenaEasyFightText"]; arenaEasyFight.Text = localizedStrings["arenaFight"];
            arenaMediumFightText.Text = localizedStrings["arenaMediumFightText"]; arenaMediumFight.Text = localizedStrings["arenaFight"];
            arenaHardFightText.Text = localizedStrings["arenaHardFightText"]; arenaHardFight.Text = localizedStrings["arenaFight"];
            arenaBossFightText.Text = localizedStrings["arenaBossFightText"]; arenaBossFight.Text = localizedStrings["arenaFight"];
            //arenaText.Text = localizedStrings["arenaEntryText"] + "\n\n"; 
            arenaSpeedup.Text = localizedStrings["buttonSpeedUp"];
            arenaNameList = new List<string>();
            arenaImageList = new List<string>();
            for (int i = 1; i <= 30; i++)
            {
                arenaNameList.Add("gladiatorName" + i.ToString());
            }
            for (int i = 1; i <= 12; i++)
            {
                arenaImageList.Add("enemy_gladiator" + i.ToString());
            }
        }
        public bool arenaClicked = false;//, arenaReady;
        public int baseEasyReward = 50;
        public int baseMediumReward = 65;
        public int baseHardReward = 75;
        public int baseBossReward = 90;
        void ArenaFight(int difficulty)
        {
            if (!arenaClicked)
            {
                //arenaReady = false;
                arenaClicked = true;
                DisableArenaButtons();
                arenaDifficulty = difficulty;
                string gladiatorName, gladiatorImage;
                int items, arenaGoldReward = 0, arenaExpReward = 0;
                gladiatorName = arenaNameList[random.Next(0, arenaNameList.Count)];
                gladiatorImage = arenaImageList[random.Next(0, arenaImageList.Count)];
                switch (difficulty)
                {
                    case 1:
                        arenaGoldReward = baseEasyReward * fameEnemyLevel();
                        arenaExpReward = 1 + (int)Math.Ceiling((float)fameEnemyLevel() / 5f);
                        items = (int)Math.Ceiling((float)player.level / 4f);
                        if (items > 9) items = 9;
                        GenerateArenaEnemy(1, gladiatorName, gladiatorImage, items);
                        //EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), items, random.Next(30, 60), random.Next(30, 60),
                        //random.Next(30, 60), random.Next(30, 60), random.Next(30, 60), random.Next(30, 60), random.Next(30, 60), random.Next(30, 60));
                        if (player.tutorial == 3) EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), 0, random.Next(10, 30), random.Next(10, 30),
                            random.Next(10, 30), random.Next(10, 30), random.Next(10, 30), random.Next(10, 30), random.Next(10, 30), random.Next(10, 30));
                        break;
                    case 2:
                        arenaGoldReward = baseMediumReward * fameEnemyLevel();
                        arenaExpReward = 3 + (int)Math.Floor((float)fameEnemyLevel() / 4f);
                        items = (int)Math.Ceiling((float)fameEnemyLevel() / 3f);
                        if (items > 9) items = 9;
                        GenerateArenaEnemy(2, gladiatorName, gladiatorImage, items);
                        //EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), items, random.Next(50, 80), random.Next(50, 80), random.Next(50, 80),
                        //    random.Next(50, 80), random.Next(50, 80), random.Next(50, 80), random.Next(50, 80), random.Next(50, 80));
                        break;
                    case 3:
                        arenaGoldReward = baseHardReward * fameEnemyLevel();
                        arenaExpReward = 5 + (int)Math.Floor((float)fameEnemyLevel() / 3f);
                        items = (int)Math.Ceiling((float)fameEnemyLevel() / 2f);
                        if (items > 9) items = 9;
                        GenerateArenaEnemy(3, gladiatorName, gladiatorImage, items);
                        //EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), items, random.Next(70, 100), random.Next(70, 100), random.Next(70, 100),
                        //    random.Next(70, 100), random.Next(70, 100), random.Next(70, 100), random.Next(70, 100), random.Next(70, 100));
                        break;
                    case 4:
                        arenaGoldReward = baseBossReward * fameEnemyLevel();
                        arenaExpReward = 7 + (int)Math.Ceiling((float)fameEnemyLevel() / 2);
                        items = 9;
                        GenerateArenaEnemy(4, gladiatorName, gladiatorImage, items);
                        //EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), items, random.Next(90, 120), random.Next(90, 120), random.Next(90, 120),
                        //random.Next(90, 120), random.Next(90, 120), random.Next(90, 120), random.Next(90, 120), random.Next(90, 120));
                        break;

                }
                Fight(player, enemy, arenaGoldReward, arenaExpReward, 0, 0, 0, 30, true);
                //ProcessArenaFight(arenaGoldReward);
                arenaClicked = false;
                player.arenaCooldown = DateTime.Now.AddMinutes(30);
                if (player.arenaNotification) SendNotification(localizedStrings["notificationArenaCooldownFinished"], localizedStrings["notificationArenaCooldownFinished1"], 3, player.arenaCooldown);


            }
        }
        void GenerateArenaEnemy(int difficulty, string gladiatorName, string gladiatorImage, int items)
        {
            List<int> testerLevel = new List<int>() { 3, 6, 7, 9, 11,12, 18, 21, 23, 27, 30 };
            int x, level;
            level = fameEnemyLevel();
            if (testerLevel.Contains(level) && random.Next(0, 11) <= 3)
            {
                if (level == 3 && difficulty >= 1)
                {
                    EnemyGeneration("playerLemoniados", "player_male_1", 3, 1);
                    enemy.strength = 17; enemy.perception = 17; enemy.dexterity = 17; enemy.agility = 17;
                    enemy.vitality = 17; enemy.endurance = 17; enemy.charisma = 17; enemy.intelligence = 17;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 6 && difficulty >= 1)
                {
                    EnemyGeneration("playerBiorn", "player_male_3", 6, 7);
                    enemy.strength = 28; enemy.perception = 25; enemy.dexterity = 23; enemy.agility = 19;
                    enemy.vitality = 23; enemy.endurance = 19; enemy.charisma = 26; enemy.intelligence = 17;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 7 && difficulty >= 1)
                {
                    EnemyGeneration("playerBlankKill", "player_male_4", 7, 6);
                    enemy.strength = 25; enemy.perception = 28; enemy.dexterity = 26; enemy.agility = 28;
                    enemy.vitality = 25; enemy.endurance = 24; enemy.charisma = 23; enemy.intelligence = 23;
                    UpdateArenaPresetStats();
                    return;

                }
                if (level == 9 && difficulty >= 1)
                {
                    EnemyGeneration("playerNati", "player_female_2", 9, 9);
                    enemy.strength = 30; enemy.perception = 27; enemy.dexterity = 22; enemy.agility = 21;
                    enemy.vitality = 26; enemy.endurance = 21; enemy.charisma = 25; enemy.intelligence = 25;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 11 && difficulty >= 1)
                {
                    EnemyGeneration("playerRoberto", "player_male_3", 11, 9);
                    enemy.strength = 37; enemy.perception = 36; enemy.dexterity = 29; enemy.agility = 29;
                    enemy.vitality = 28; enemy.endurance = 26; enemy.charisma = 25; enemy.intelligence = 27;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 12 && difficulty >= 2)
                {
                    EnemyGeneration("playerIvu", "player_female_3", 12, 9);
                    enemy.strength = 45; enemy.perception = 42; enemy.dexterity = 45; enemy.agility = 61;
                    enemy.vitality = 42; enemy.endurance = 42; enemy.charisma = 46; enemy.intelligence = 40;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 18 && difficulty >= 2)
                {
                    EnemyGeneration("playerTytus", "player_male_2", 18, 9);
                    enemy.strength = 68; enemy.perception = 81; enemy.dexterity = 62; enemy.agility = 74;
                    enemy.vitality = 56; enemy.endurance = 53; enemy.charisma = 53; enemy.intelligence = 83;
                    UpdateArenaPresetStats();
                    return;
                }
                if (level == 21 && difficulty >= 2)
                {
                    EnemyGeneration("playerVaxacus", "player_male_1", 21, 9);
                    enemy.strength = 86; enemy.perception = 127; enemy.dexterity = 66; enemy.agility = 105;
                    enemy.vitality = 89; enemy.endurance = 66; enemy.charisma = 62; enemy.intelligence = 62;
                    UpdateArenaPresetStats();
                    return;
                }
                else if (level == 23 && difficulty >= 2)
                {
                    EnemyGeneration("playerMadionLedaney", "player_female_2", 23, 9);
                    enemy.strength = 88; enemy.perception = 118; enemy.dexterity = 80; enemy.agility = 108;
                    enemy.vitality = 72; enemy.endurance = 72; enemy.charisma = 102; enemy.intelligence = 74;
                    UpdateArenaPresetStats();
                    return;
                }
                else if (level == 27 && difficulty >= 2)
                {
                    EnemyGeneration("playerRyner", "player_male_2", 27, 9);
                    enemy.strength = 136; enemy.perception = 116; enemy.dexterity = 109; enemy.agility = 86;
                    enemy.vitality = 101; enemy.endurance = 121; enemy.charisma = 156; enemy.intelligence = 90;
                    UpdateArenaPresetStats();
                    return;
                }
                else if (level <= 30 && difficulty >= 3)
                {
                    EnemyGeneration("playerAmalDestroyer", "player_male_1", 30, 9);
                    enemy.strength = 105; enemy.perception = 157; enemy.dexterity = 126; enemy.agility = 158;
                    enemy.vitality = 151; enemy.endurance = 116; enemy.charisma = 214; enemy.intelligence = 106;
                    UpdateArenaPresetStats();
                    return;
                }

            }

            List<int> percentages = new List<int>();
            switch (difficulty)
            {
                case 1:
                    percentages = new List<int>
                    {
                        50,60,70,80,
                        90,60,70,60
                    };
                    break;
                case 2:
                    percentages = new List<int>
                    {
                        65,60,70,80,
                        100,100,80,90
                    };
                    break;

                case 3:
                    percentages = new List<int>
                    {
                        80,85,90,90,
                        120,110,95,90
                    };
                    break;
                case 4:
                    percentages = new List<int>
                    {
                        150,120,110,100,
                        140,90,85,80
                    };
                    break;

            }
            //for(int i=0; i < percentages.Count;i++)
            //{
            //    percentages[i]+=level-30;
            //}
            Queue<int> z = new Queue<int>();
            while (percentages.Count > 0)
            {
                x = random.Next(0, percentages.Count);
                z.Enqueue(random.Next(percentages[x] - 10, percentages[x] + 11));
                percentages.RemoveAt(x);
            }
            EnemyGeneration(gladiatorName, gladiatorImage, fameEnemyLevel(), items, z.Dequeue(), z.Dequeue(), z.Dequeue(), z.Dequeue(), z.Dequeue(), z.Dequeue(), z.Dequeue(), z.Dequeue());

        }
        int fameEnemyLevel()
        {
            int x = (int)Math.Ceiling((float)player.fame / 10);
            if (x <= 0) x = 1;
            return x;
        }
        public void ProcessArenaFight(int goldReward, bool win)
        {
            if (win)
            {

                if (player.level > fameEnemyLevel())
                {
                    player.Fame += arenaDifficulty * 2 + (player.level - fameEnemyLevel());
                }
                else if (player.level <= fameEnemyLevel())//fame/10 po prostu
                {
                    player.Fame += arenaDifficulty * 2;
                }
                if (fameEnemyLevel() >= player.level)
                    switch (arenaDifficulty)
                    {
                        case 1:
                            if (player.defeatEasy == false) player.defeatEasy = true;
                            break;
                        case 2:
                            if (player.defeatMedium == false) player.defeatMedium = true;
                            break;
                        case 3:
                            if (player.defeatHard == false) player.defeatHard = true;
                            break;
                        case 4:
                            if (player.defeatColliseum == false) player.defeatColliseum = true;
                            break;
                    }

            }
            else
            {
                if (player.level >= fameEnemyLevel())
                {
                    player.Fame -= arenaDifficulty * 2;
                }
                else if (player.level < fameEnemyLevel())
                {
                    player.Fame -= arenaDifficulty * 2 + (fameEnemyLevel() - player.level);
                }
                if (player.Gold - goldReward / 3 < 0) { player.Gold = 0; }
                else { goldReward /= 3; player.Gold -= goldReward; }
                TEST(localizedStrings["arenaLostGold"] + goldReward.ToString() + " " + localizedStrings["goldText"]);

            }
            if (player.Fame <= (player.level + 1) * 10) { player.arenaReward = null; }
            else if (player.Fame > (player.level + 1) * 10) RollArenaReward();

            if (player.tutorial == 3)
            {
                player.Tutorial++;
            }

        }
        void EasyFight_Clicked(object sender, EventArgs e)
        {
            ArenaFight(1);
        }
        void MediumFight_Clicked(object sender, EventArgs e)
        {
            ArenaFight(2);
        }
        void HardFight_Clicked(object sender, EventArgs e)
        {
            ArenaFight(3);

        }
        void BossFight_Clicked(object sender, EventArgs e)
        {
            ArenaFight(4);
        }

        async void ArenaSpeedup_Clicked(object sender, EventArgs e)
        {

            int cost = 1;

            TimeSpan timeLeft = player.arenaCooldown - DateTime.Now;

            cost = timeLeft.Minutes + 1;
            bool result = await DisplayAlert(cautionText, localizedStrings["arenaSpeedup"] + cost + " " + localizedStrings["stamText"] + "\n" + localizedStrings["currentStaminaText"] + player.CurrentStamina + "\n" + localizedStrings["stamText"], yesText, noText);
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
                    player.arenaCooldown = DateTime.Now;
                }
            }
            else
            {
                return;
            }
        }

        void DisableArenaButtons()
        {
            arenaEasyFight.IsEnabled = false;
            arenaMediumFight.IsEnabled = false;
            arenaHardFight.IsEnabled = false;
            arenaBossFight.IsEnabled = false;
            arenaSpeedup.IsVisible = true;
            arenaNextDuelTimer.IsVisible = true;
        }
        void EnableArenaButtons()
        {
            arenaSpeedup.IsVisible = false;
            arenaNextDuelTimer.IsVisible = false;
            DisableArenaButtons();
            arenaEasyFight.IsEnabled = true;
            if (player.fame >= 50) arenaMediumFight.IsEnabled = true;
            if (player.fame >= 100) arenaHardFight.IsEnabled = true;
            if (player.fame >= 200) arenaBossFight.IsEnabled = true;
        }

        async void recieveArenaReward_Clicked(object sender, EventArgs e)
        {
            if (player.arenaReward != null)
            {
                if (activeDialogBox == false)
                {
                    Item equippedItem = null;
                    activeDialogBox = true;
                    if (arenaReward == null) { activeDialogBox = false; return; }
                    else
                    {
                        if (player.arenaReward.itemClass == ItemClass.Weapon && player.weapon != null) { equippedItem = player.weapon; }
                        if (player.arenaReward.itemClass == ItemClass.Helmet && player.head != null) { equippedItem = player.head; }
                        if (player.arenaReward.itemClass == ItemClass.Torso && player.torso != null) { equippedItem = player.torso; }
                        if (player.arenaReward.itemClass == ItemClass.Shield && player.shield != null) { equippedItem = player.shield; }
                        if (player.arenaReward.itemClass == ItemClass.Boots && player.boots != null) { equippedItem = player.boots; }
                        if (player.arenaReward.itemClass == ItemClass.Gloves && player.gloves != null) { equippedItem = player.gloves; }
                        if (player.arenaReward.itemClass == ItemClass.Necklace && player.necklace != null) { equippedItem = player.necklace; }
                        if (player.arenaReward.itemClass == ItemClass.Belt && player.belt != null) { equippedItem = player.belt; }
                        if (player.arenaReward.itemClass == ItemClass.Ring && player.ring != null) { equippedItem = player.ring; }
                        if (equippedItem != null)
                        {
                            player.arenaReward.UpdateDescription(localizedStrings);
                            var action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet(
                                localizedStrings["actionItemText"],
                                localizedStrings["actionBuyText"],
                                localizedStrings["cancelText"],
                                null,
                                player.arenaReward.itemDescription + "\n" + localizedStrings["costText"] + player.arenaReward.buyValue + " " + localizedStrings["fameCostText"] + "\n_______________\n" + localizedStrings["yourItemText"] + "\n" + equippedItem.itemDescription
                            );
                            if (action == localizedStrings["actionBuyText"])
                            {
                                if (clicked) return;
                                if (player.items.Count > 11) { await DisplayAlert(cautionText, localizedStrings["noSpaceInInventoryText"], okText); activeDialogBox = false; clicked = false; return; }
                                if (player.Fame < player.arenaReward.buyValue) { await DisplayAlert(cautionText, localizedStrings["insufficientFameMessage"], okText); activeDialogBox = false; clicked = false; return; }
                                player.Fame -= player.arenaReward.buyValue;
                                player.items.Add(player.arenaReward);
                                player.arenaReward = null;
                                clicked = true;
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
                                localizedStrings["actionBuyText"],
                                localizedStrings["cancelText"],
                                null,
                                player.arenaReward.itemDescription + "\n" + localizedStrings["costText"] + player.arenaReward.buyValue + " " + localizedStrings["fameCostText"]
                                );
                            if (action == localizedStrings["actionBuyText"])
                            {
                                if (clicked) return;
                                if (player.items.Count > 11) { await DisplayAlert(cautionText, localizedStrings["noSpaceInInventoryText"], okText); activeDialogBox = false; clicked = false; return; }
                                if (player.Fame < player.arenaReward.buyValue) { await DisplayAlert(cautionText, localizedStrings["insufficientFameMessage"], okText); activeDialogBox = false; clicked = false; return; }
                                player.Fame -= player.arenaReward.buyValue;
                                player.items.Add(player.arenaReward);
                                player.arenaReward = null;
                                clicked = true;
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
                    UpdateShop();
                    GridEquipment();
                    ProcessEquippedItems();
                }

            }
            ShowReward(); //testuj, ale powinno działać (po zakupie itemka zupdate'uje UI i nie będzie go widać)

        }


        private void arenaButton_Clicked(object sender, EventArgs e)
        {
            ArenaInfo();
        }

        async void ArenaInfo()
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["infoText"], localizedStrings["arenaEntryText"], localizedStrings["okText"]);
                clicked = false;
            }
        }
    }
}