using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiatorRPG;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GladiatorRPG
{


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FightPage : ContentPage
    {
        public Task WaitForDismissal()
        {
            return dismissalTaskCompletionSource.Task;
        }
        private TaskCompletionSource<bool> dismissalTaskCompletionSource = new TaskCompletionSource<bool>();
        int maxRounds;
        bool conditionMet, isArena;
        Player player;
        Entity enemy;
        Dictionary<string, string> localizedStrings;
        Random random = new Random();
        int expReward, goldReward, itemChance, items, itemRarity;
        public MainTabPage mainPage;
        int playerDamageDealt = 0, enemyDamageDealt = 0;
        public string recap;
        public FightPage(Dictionary<string, string> _localizedStrings, MainTabPage _mainPage, Player _player, Entity _enemy, int _expReward = 0, int _goldReward = 0, int _itemChance = 0, int _items = 0, int _itemRarity = 0, int rounds = 30, bool isArena = false)
        {

            mainPage = _mainPage;
            expReward = _expReward;
            goldReward = _goldReward;
            maxRounds = rounds;
            this.isArena = isArena;
            InitializeComponent();
            localizedStrings = _localizedStrings;
            Localization(localizedStrings);

            player = _player;
            enemy = _enemy;
            itemChance = _itemChance; items = _items; itemRarity = _itemRarity;
            NavigationPage.SetHasBackButton(this, false);
            if (enemy.source != "" || player.source != null) enemyAvatar.Source = enemy.source;
            if (player.source != "" || enemy.source != null) playerAvatar.Source = player.source;
            player.fightWon = false;
            conditionMet = false;//gdy false to zabrania cofnięcia do głównej strony aplikacji

            ShowStats(player, enemy);
            StartFight();
        }
        void Localization(Dictionary<string, string> localizedStrings)
        {
            lvlLabel.Text = localizedStrings["lvlText"]; lvlLabel2.Text = localizedStrings["lvlText"];
            strLabel.Text = localizedStrings["strText"]; strLabel2.Text = localizedStrings["strText"];
            perLabel.Text = localizedStrings["perText"]; perLabel2.Text = localizedStrings["perText"];
            dexLabel.Text = localizedStrings["dexText"]; dexLabel2.Text = localizedStrings["dexText"];
            agiLabel.Text = localizedStrings["agiText"]; agiLabel2.Text = localizedStrings["agiText"];
            vitLabel.Text = localizedStrings["vitText"]; vitLabel2.Text = localizedStrings["vitText"];
            endLabel.Text = localizedStrings["endText"]; endLabel2.Text = localizedStrings["endText"];
            chaLabel.Text = localizedStrings["chaText"]; charLabel2.Text = localizedStrings["chaText"];
            intLabel.Text = localizedStrings["intText"]; intLabel2.Text = localizedStrings["intText"];
            roundLabel.Text = localizedStrings["roundText"];
        }
        float playerHitChance, playerCritChance, playerMultiHitChance, playerCritEvade, playerMultiHitEvade,
                enemyHitChance, enemyCritChance, enemyMultiHitChance, enemyCritEvade, enemyMultiHitEvade,
                playerArmorDamage, enemyArmorDamage, lvlDiff, enemylvlDiff, currentPlayerMultiHitChance
            , currentEnemyMultiHitChance;

        bool playerHits = false, playerCrits = false, playerMultiHits = true, playerAlive = true, enemyAlive = true, enemyHits = false, enemyCrits = false, enemyMultiHits = true, victory
            , crit, enemyArmorPresent;

        int playerArmor, enemyArmor, roundCounter, damage, damageDealt;
        string message;
        //Automatyczna walka turowa, żadnego wkładu gracza
        void StartFight()
        {
            #region Error prevention
            if (player.strength < 1) player.strength = 1; if (enemy.strength < 1) enemy.strength = 1;
            if (player.perception < 1) player.perception = 1; if (enemy.perception < 1) enemy.perception = 1;
            if (player.dexterity < 1) player.dexterity = 1; if (enemy.dexterity < 1) enemy.dexterity = 1;
            if (player.agility < 1) player.agility = 1; if (enemy.agility < 1) enemy.agility = 1;
            if (player.endurance < 1) player.endurance = 1; if (enemy.endurance < 1) enemy.endurance = 1;
            if (player.vitality < 1) player.vitality = 1; if (enemy.endurance < 1) enemy.endurance = 1;
            if (player.charisma < 1) player.charisma = 1; if (enemy.charisma < 1) enemy.charisma = 1;
            if (player.intelligence < 1) player.intelligence = 1; if (enemy.intelligence < 1) enemy.intelligence = 1;

            #endregion

            //Obliczanie szans na trafienie, kryta itd.
            #region Counting chances
            // int critAndmultiModifier = 5;
            lvlDiff = player.level - enemy.level;
            enemylvlDiff = enemy.level - player.level;
            if (lvlDiff < 0) lvlDiff = 0; if (enemylvlDiff < 0) enemylvlDiff = 0;


            //playerHitChance = (((float)player.perception / ((float)player.perception + (float)enemy.agility + enemylvlDiff)) * 100);

            if (player.perception > enemy.agility) playerHitChance = 100f - 50f * ((float)enemy.agility / (float)player.perception);
            else if (player.perception < enemy.agility) playerHitChance = 50 * ((float)player.perception / (float)enemy.agility);
            else playerHitChance = 50;
            if (playerHitChance < 10) playerHitChance = 10; else if (playerHitChance > 95) playerHitChance = 95;

            //enemyHitChance = (((float)enemy.perception / ((float)enemy.perception + (float)player.agility + lvlDiff)) * 100);

            if (enemy.perception > player.agility) enemyHitChance = 100f - 50f * ((float)player.agility / (float)enemy.perception);
            else if (enemy.perception < player.agility) enemyHitChance = 50 * ((float)enemy.perception / (float)player.agility);
            else enemyHitChance = 50;
            if (enemyHitChance < 10) enemyHitChance = 10; else if (enemyHitChance > 95) enemyHitChance = 95;

            //playerCritChance = (((float)player.dexterity / (((float)player.dexterity + (float)enemy.endurance + enemylvlDiff) * critAndmultiModifier)) * 100);
            enemyCritEvade = 1 - (((float)enemy.endurance * 15 / (float)player.level) / 100);
            if (enemyCritEvade > 1) enemyCritEvade = 1; else if (enemyCritEvade < 0) enemyCritEvade = 0;
            playerCritChance = ((float)player.dexterity * 15) / ((float)enemy.level) * enemyCritEvade;
            if (playerCritChance > 50) playerCritChance = (playerCritChance - 50) / 2 + 50;
            if (playerCritChance > 65) playerCritChance = (playerCritChance - 65) / 4 + 65;
            if (playerCritChance > 75) playerCritChance = (playerCritChance - 75) / 8 + 75;
            if (playerCritChance > 90) playerCritChance = 90;
            if (playerCritChance < 0) playerCritChance = 0;

            //enemyCritChance = (((float)enemy.dexterity / (((float)enemy.dexterity + (float)player.endurance + lvlDiff) * critAndmultiModifier)) * 100);
            playerCritEvade = 1 - (((float)player.endurance * 12 / (float)enemy.level) / 100);
            if (playerCritEvade > 1) playerCritEvade = 1; else if (playerCritEvade < 0) playerCritEvade = 0;
            enemyCritChance = ((float)enemy.dexterity * 15) / ((float)player.level) * playerCritEvade;
            if (enemyCritChance > 50) enemyCritChance = (enemyCritChance - 50) / 2 + 50;
            if (enemyCritChance > 65) enemyCritChance = (enemyCritChance - 65) / 4 + 65;
            if (enemyCritChance > 75) enemyCritChance = (enemyCritChance - 75) / 8 + 75;
            if (enemyCritChance > 90) enemyCritChance = 90;
            if (enemyCritChance < 0) enemyCritChance = 0;

            //playerMultiHitChance = (((float)player.charisma / (((float)player.charisma + (float)enemy.intelligence + enemylvlDiff) * critAndmultiModifier)) * 100);
            enemyMultiHitEvade = 1 - (((float)enemy.intelligence * 12) / (float)player.level) / 100;
            if (enemyMultiHitEvade > 1) enemyMultiHitEvade = 1; else if (enemyMultiHitEvade < 0) enemyMultiHitEvade = 0;
            playerMultiHitChance = ((float)player.charisma * 15) / ((float)enemy.level) * enemyMultiHitEvade;
            if (playerMultiHitChance > 50) playerMultiHitChance = (playerMultiHitChance - 50) / 2 + 50;
            if (playerMultiHitChance > 65) playerMultiHitChance = (playerMultiHitChance - 65) / 4 + 65;
            if (playerMultiHitChance > 75) playerMultiHitChance = (playerMultiHitChance - 75) / 8 + 75;
            if (playerMultiHitChance > 90) playerMultiHitChance = 90;
            else if (playerMultiHitChance < 0) playerMultiHitChance = 0;


            //enemyMultiHitChance = (((float)enemy.charisma / (((float)enemy.charisma + (float)player.intelligence + lvlDiff) * critAndmultiModifier)) * 100);
            playerMultiHitEvade = 1 - (((float)player.intelligence * 12) / (float)enemy.level) / 100;
            if (playerMultiHitEvade > 1) playerMultiHitEvade = 1; else if (playerMultiHitEvade < 0) playerMultiHitEvade = 0;
            enemyMultiHitChance = ((float)enemy.charisma * 15) / ((float)player.level) * playerMultiHitEvade;
            if (enemyMultiHitChance > 50) enemyMultiHitChance = (enemyMultiHitChance - 50) / 2 + 50;
            if (enemyMultiHitChance > 65) enemyMultiHitChance = (enemyMultiHitChance - 65) / 4 + 65;
            if (enemyMultiHitChance > 75) enemyMultiHitChance = (enemyMultiHitChance - 75) / 8 + 75;
            if (enemyMultiHitChance > 90) enemyMultiHitChance = 90;
            else if (enemyMultiHitChance < 0) enemyMultiHitChance = 0;

            enemy.critDamage = 50 + (int)Math.Floor((enemy.strength / 5f));

            playerArmorDamage = (float)player.strength / (float)(enemy.level * 3);
            playerArmorDamageLabel.Text = localizedStrings["armorDamageText"] + "x" + playerArmorDamage.ToString("0.00");
            enemyArmorDamage = (float)enemy.strength / (float)(player.level * 3);
            enemyArmorDamageLabel.Text = localizedStrings["armorDamageText"] + "x" + enemyArmorDamage.ToString("0.00");



            playerHitChanceLabel.Text = localizedStrings["hitChanceText"] + " : " + playerHitChance.ToString("0.00") + "%";
            playerCritChanceLabel.Text = localizedStrings["critChanceText"] + " : " + playerCritChance.ToString("0.00") + "%";
            playerMultiHitChanceLabel.Text = localizedStrings["multiHitChanceText"] + " : " + playerMultiHitChance.ToString("0.00") + "%";
            enemyHitChanceLabel.Text = localizedStrings["hitChanceText"] + " : " + enemyHitChance.ToString("0.00") + "%";
            enemyCritChanceLabel.Text = localizedStrings["critChanceText"] + " : " + enemyCritChance.ToString("0.00") + "%";
            enemyMultiHitChanceLabel.Text = localizedStrings["multiHitChanceText"] + " : " + enemyMultiHitChance.ToString("0.00") + "%";
            #endregion
            playerArmor = player.armor;
            enemyArmor = enemy.armor;
            roundCounter = 0;
            playerDamageDealt = 0; enemyDamageDealt = 0;
            ProcessFight();

        }
        async void ProcessFight()
        {
            string enemyStatus = "";
            string playerStatus = "";
            //mainPage.fightSleepTime = 750;
            fightSleepTimeBuffor = mainPage.fightSleepTime;
            await Task.Delay(mainPage.fightSleepTime);
            while (roundCounter < maxRounds)
            {
                playerHits = false; playerCrits = false; playerMultiHits = true; playerAlive = true; enemyAlive = true; enemyHits = false; enemyCrits = false; enemyMultiHits = true;
                roundCounter++;
                roundCounterText.Text = roundCounter.ToString();
                message = "====================\nRound " + roundCounter.ToString();
                currentPlayerMultiHitChance = playerMultiHitChance; currentEnemyMultiHitChance = enemyMultiHitChance;
                damage = 0; damageDealt = 0;
                playerStatus = "";
                enemyStatus = "";
                int armorDestroyed;

                
                //tura gracza
                while (playerMultiHits)
                {

                    //message = "";
                    playerHits = Roll(playerHitChance);
                    if (playerHits)
                    {

                        crit = false;
                        enemyArmorPresent = true;
                        playerCrits = Roll(playerCritChance);

                        damage = DamageRoll(player);
                        if (playerCrits) { damage = (int)Math.Floor(damage * (1f + ((float)player.critDamage / 100))); crit = true; }

                        //Zmniejsza zadawane obrażenia zależnie od armora przeciwnika
                        if (enemyArmor > 0 && damage > 0)
                        {
                            damageDealt = DamageReduction(damage, enemyArmor, player.level);
                            armorDestroyed = (int)Math.Floor((float)DamagePrevented(damage, enemyArmor, player.level) * playerArmorDamage);
                        }
                        else { damageDealt = damage; armorDestroyed = 0; }
                        enemy.currentHealth -= damageDealt;
                        playerDamageDealt += damageDealt;
                        //Zadaje pełne obrażenia pancerzowi
                        if (enemyArmor != 0) { enemyArmor -= armorDestroyed; } else enemyArmorPresent = false;
                        if (enemyArmor < 0) { enemyArmor = 0; }
                        if (crit)
                        {
                            message += "\n" + player.name + localizedStrings["fightCriticalHitText"] + localizedStrings[enemy.name] + localizedStrings["fightForText"] + damageDealt.ToString() + localizedStrings["fightDamageText"];
                            if (enemyStatus != "") enemyStatus += "\n";
                            enemyStatus += "-" + damageDealt + " !";
                        }
                        else
                        {
                            message += "\n" + player.name + localizedStrings["fightHitText"] + localizedStrings[enemy.name] + localizedStrings["fightForText"] + damageDealt.ToString() + localizedStrings["fightDamageText"];
                            if (enemyStatus != "") enemyStatus += "\n";
                            enemyStatus += "-" + damageDealt.ToString();

                        }
                        if (enemyArmorPresent) message += localizedStrings["fightAndDestroysText"] + armorDestroyed.ToString() + localizedStrings["fightArmorPointsText"] + localizedStrings["fightOfEnemysArmorText"];
                        else message += localizedStrings["fightBypassingArmorText"];

                        //fightLogs.Text = message + fightLogs.Text;
                    }

                    else
                    {
                        message += "\n" + player.name + localizedStrings["fightMissAttack"];
                        //SetPlayerStatus(localizedStrings["fightMiss"], Color.White);
                        //SetEnemyStatus("", Color.White);
                    }
                    playerMultiHits = Roll(currentPlayerMultiHitChance);
                    if (playerMultiHits) { currentPlayerMultiHitChance = currentPlayerMultiHitChance / 2; }
                }

                //tura przeciwnika
                while (enemyMultiHits)
                {
                    enemyHits = Roll(enemyHitChance);
                    if (enemyHits)
                    {
                        enemyArmorPresent = true;
                        //message = "";
                        crit = false;
                        enemyCrits = Roll(enemyCritChance);
                        damage = DamageRoll(enemy);
                        if (enemyCrits) { damage = (int)Math.Floor(damage * (1f + ((float)enemy.critDamage / 100))); crit = true; }
                        if (playerArmor > 0) { damageDealt = DamageReduction(damage, playerArmor, enemy.level); armorDestroyed = (int)Math.Floor((float)DamagePrevented(damage, playerArmor, enemy.level) * enemyArmorDamage); }
                        else { damageDealt = damage; armorDestroyed = 0; }
                        player.CurrentHealth -= damageDealt;
                        enemyDamageDealt += damageDealt;

                        if (playerArmor != 0) { playerArmor -= armorDestroyed; } else enemyArmorPresent = false;
                        if (playerArmor < 0) { playerArmor = 0; armorDestroyed = 0; }

                        if (crit)
                        {
                            message += "\n" + localizedStrings[enemy.name] + localizedStrings["fightCriticalHitText"] + player.name + localizedStrings["fightForText"] + damageDealt.ToString() + localizedStrings["fightDamageText"];
                            if (playerStatus != "") playerStatus += "\n";
                            playerStatus += "-" + damageDealt.ToString() + " !";


                        }
                        else
                        {
                            message += "\n" + localizedStrings[enemy.name] + localizedStrings["fightHitText"] + player.name + localizedStrings["fightForText"] + damageDealt.ToString() + localizedStrings["fightDamageText"];
                            if (playerStatus != "") playerStatus += "\n";
                            playerStatus += "-" + damageDealt.ToString();


                        }
                        if (enemyArmorPresent) message += localizedStrings["fightAndDestroysText"] + armorDestroyed.ToString() + localizedStrings["fightArmorPointsText"] + localizedStrings["fightOfEnemysArmorText"];
                        else message += localizedStrings["fightBypassingArmorText"];

                        //fightLogs.Text = message + fightLogs.Text;
                        //fightLogs.Text = "\nEnemy hits player for " + damageDealt.ToString() + "\nEnemy destroyed " + damage.ToString() + " of player's armor." + fightLogs.Text;
                    }
                    else
                    {
                        message += "\n" + localizedStrings[enemy.name] + localizedStrings["fightMissAttack"];
                        //SetEnemyStatus(localizedStrings["fightMiss"], Color.White);
                        //SetPlayerStatus("", Color.White);
                    }
                    enemyMultiHits = Roll(currentEnemyMultiHitChance);
                    if (enemyMultiHits) { currentEnemyMultiHitChance = currentEnemyMultiHitChance / 2; }

                    
                }
                HpBarUpdate();
                UpdateProgressBar();
                

                if (enemyStatus != "") SetEnemyStatus(enemyStatus, Color.Red);
                else SetEnemyStatus(localizedStrings["fightMiss"], Color.White);

                if (playerStatus != "") SetPlayerStatus(playerStatus, Color.Red);
                else SetPlayerStatus(localizedStrings["fightMiss"], Color.White);

                message += "\nPlayer armor = " + playerArmor.ToString() + "\nEnemy armor = " + enemyArmor.ToString() + "\n\n";
                fightLogs.Text = message + fightLogs.Text;
                await Task.Delay(mainPage.fightSleepTime);
                if (player.CurrentHealth <= 1) { playerAlive = false; }
                if (enemy.currentHealth <= 1) { enemyAlive = false; }
                if (!enemyAlive && playerAlive)
                { victory = true; FightEnd(victory); return; }
                else if ((!enemyAlive && !playerAlive) || enemyAlive && !playerAlive)
                { victory = false; FightEnd(false); return; }



                //EndOfFight
            }
            if (((float)enemyDamageDealt / (float)player.maxHealth) < ((float)playerDamageDealt / (float)enemy.maxHealth))
            {
                victory = true; FightEnd(victory);
            }
            else
            { victory = false; FightEnd(victory); }

        }

        void UpdateProgressBar()
        {
            if (playerDamageDealt == 0 && enemyDamageDealt == 0) { fightProgress.Progress = 0.5; fightProgress.ProgressColor = Color.Yellow; fightProgress.Rotation = 0; }
            else if (playerDamageDealt == 0 && enemyDamageDealt > 0) { fightProgress.Progress = 1; fightProgress.ProgressColor = Color.Red; fightProgress.Rotation = 180; }
            else if (playerDamageDealt > 0 && enemyDamageDealt == 0) { fightProgress.Progress = 1; fightProgress.ProgressColor = Color.LightGreen; fightProgress.Rotation = 0; }
            else
            {
                float pdp = (((float)playerDamageDealt / (float)enemy.maxHealth) * 100);
                float edp = (((float)enemyDamageDealt / (float)player.maxHealth) * 100);
                if (pdp > edp)
                {
                    float progress = 0.5f + ((pdp - edp) / 100f);
                    if (progress > 1) progress = 1;
                    fightProgress.Progress = progress;
                    fightProgress.Rotation = 0;
                    fightProgress.ProgressColor = Color.LightGreen;


                }
                else if (edp > pdp)
                {
                    float progress = 0.5f + ((edp - pdp) / 100f);
                    if (progress < 0) progress = 0;
                    fightProgress.Progress = progress;
                    fightProgress.Rotation = 180;
                    fightProgress.ProgressColor = Color.Red;
                }
                else if (pdp == edp)
                {
                    fightProgress.Progress = 0.5f;
                    fightProgress.Rotation = 0;
                    fightProgress.ProgressColor = Color.Yellow;
                }
            }
        }

        string DamageDealtText()
        {
            string text = "\n";
            text += localizedStrings["fightPlayerDamageDealtText"] + playerDamageDealt + " (" + (((float)playerDamageDealt / (float)enemy.maxHealth) * 100).ToString("0.00") + "%)" + localizedStrings["fightDamageText"] + "\n";
            text += localizedStrings["fightEnemyDamageDealtText"] + enemyDamageDealt + " (" + (((float)enemyDamageDealt / (float)player.maxHealth) * 100).ToString("0.00") + "%)" + localizedStrings["fightDamageText"];
            return text;
        }
        void FightEnd(bool victory)
        {
            mainPage.fightSleepTime = fightSleepTimeBuffor;
            fightLogs.Text = "\n\n" + fightLogs.Text;
            if (victory)
            {
                if (player.level - 1 > enemy.level)
                {
                    if (player.winStreak >= 0) player.winStreak++;
                    else if (player.winStreak < 0)
                    {

                        player.winStreak += (int)Math.Ceiling((float)Math.Abs(player.winStreak) / 4f);
                        if (player.winStreak > 0) player.winStreak = 0;

                    }
                }
                string rewards = "";
                if (player.level > enemy.level)
                {
                    float expNormalization = 1;
                    for (int i = player.level - enemy.level; i > 0; i--)
                    {
                        expNormalization -= 0.2f;
                        if (expNormalization <= 0)
                        {
                            expNormalization = 0;
                            break;
                        }
                    }
                    expReward = (int)Math.Floor((float)expReward * expNormalization);
                }
                if (expReward > 0) player.Exp += expReward;
                if (goldReward > 0) player.Gold += goldReward;
                if (expReward > 0) rewards = "\n" + localizedStrings["expText"] + " : " + expReward;
                if (goldReward > 0) rewards = rewards + "\n" + localizedStrings["goldText"] + " : " + goldReward;

                player.fightWon = true;
                while (items > 0)
                {

                    int roll = random.Next(0, 101);
                    if (roll < itemChance)
                    {
                        player.itemsToRecieve.Enqueue(mainPage.ItemGenerator(enemy.level, enemy.level, itemRarity));
                        rewards += "\n" + localizedStrings[player.itemsToRecieve.Last().itemName] + " " + localizedStrings["itemLevel"] + " " + player.itemsToRecieve.Peek().levelReq;
                    }
                    items--;
                }
                postFight.Text = localizedStrings["victoryText"]; postFight.TextColor = Color.LightGreen;
                fightLogs.Text = localizedStrings["victoryText"] + rewards + "\n" + DamageDealtText() + fightLogs.Text;
                playerBorder.Source = "borderGreen";
                enemyBorder.Source = "borderRed";
                recap = localizedStrings["victoryText"] + "\n" + player.name + " vs " + localizedStrings[enemy.name] + "\n" + localizedStrings["dungeonRewardsText"] + rewards;
            }
            else
            {
                if (player.winStreak <= 0) player.winStreak--;
                else if (player.winStreak > 0)
                {
                    player.winStreak -= (int)Math.Ceiling((float)player.winStreak / 4f);
                    if (player.winStreak < 0) player.winStreak = 0;
                }
                fightLogs.Text = localizedStrings["defeatText"] + "\n" + DamageDealtText() + fightLogs.Text;
                playerBorder.Source = "borderRed";
                enemyBorder.Source = "borderGreen";
                player.fightWon = false;
                recap = localizedStrings["defeatText"] + "\n" + player.name + " vs " + localizedStrings[enemy.name];
                postFight.Text = localizedStrings["defeatText"];
                postFight.TextColor = Color.Red;
            }
            if (isArena) { mainPage.ProcessArenaFight(goldReward, victory); }
            conditionMet = true;
            NavigationPage.SetHasBackButton(this, true);
            skipWaiting.IsVisible = false;
            SetPlayerStatus("", Color.White);
            SetEnemyStatus("", Color.White);
            mainPage.UpdateItemsToRecieve();
            dismissalTaskCompletionSource.TrySetResult(true);
            //player.fightHistory.Add(recap);

        }
        void SetPlayerStatus(string text, Color color)
        {
            playerStatus.TextColor = color;
            playerStatus.Text = text;
        }
        void SetEnemyStatus(string text, Color color)
        {
            enemyStatus.TextColor = color;
            enemyStatus.Text = text;
        }
        void HpBarUpdate()
        {
            playerHealth.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth.ToString();
            enemyHealth.Text = enemy.currentHealth.ToString() + "/" + enemy.maxHealth.ToString();
            playerHpBar.Progress = (float)player.CurrentHealth / (float)player.maxHealth;
            enemyHpBar.Progress = (float)enemy.currentHealth / (float)enemy.maxHealth;
        }
        int DamageReduction(int damage, int armor, int attackerlvl)
        {
            float armorLevelScaling = 40;
            int damageReduction = (int)Math.Ceiling((float)damage * (((float)armor / ((float)armor + (float)attackerlvl * armorLevelScaling))));
            damage -= damageReduction;
            return damage;
        }
        int DamagePrevented(int damage, int armor, int attackerlvl)
        {

            float armorLevelScaling = 40;
            return (int)Math.Ceiling((float)damage * (((float)armor / ((float)armor + (float)attackerlvl * armorLevelScaling))));

        }
        int DamageRoll(Entity entity)
        {
            int damage = random.Next(entity.minDamage, entity.maxDamage + 1);
            //fightLogs.Text = damage.ToString() + fightLogs.Text;
            return damage;
        }

        bool Roll(float hitChance)
        {
            double roll = Math.Round(random.NextDouble() * 100, 2);
            //fightLogs.Text = fightLogs.Text + roll.ToString();
            if (roll < hitChance) { return true; }
            else { return false; }
        }
        void ShowStats(Player player, Entity enemy)
        {

            playerName.Text = player.name;
            if (localizedStrings.ContainsKey(enemy.name)) enemyName.Text = localizedStrings[enemy.name];
            else enemyName.Text = enemy.name;
            playerLevel.Text = player.level.ToString(); enemyLevel.Text = enemy.level.ToString();
            playerStrength.Text = player.strength.ToString(); enemyStrength.Text = enemy.strength.ToString();
            playerPerception.Text = player.perception.ToString(); enemyPerception.Text = enemy.perception.ToString();
            playerDexterity.Text = player.dexterity.ToString(); enemyDexterity.Text = enemy.dexterity.ToString();
            playerAgility.Text = player.agility.ToString(); enemyAgility.Text = enemy.agility.ToString();
            playerVitality.Text = player.vitality.ToString(); enemyVitality.Text = enemy.vitality.ToString();
            playerEndurance.Text = player.endurance.ToString(); enemyEndurance.Text = enemy.endurance.ToString();
            playerCharisma.Text = player.charisma.ToString(); enemyCharisma.Text = enemy.charisma.ToString();
            playerIntelligence.Text = player.intelligence.ToString(); enemyIntelligence.Text = enemy.intelligence.ToString();
            playerDamage.Text = localizedStrings["damText"] + " : " + player.minDamage.ToString() + " - " + player.maxDamage.ToString();
            enemyDamage.Text = localizedStrings["damText"] + " : " + enemy.minDamage.ToString() + " - " + enemy.maxDamage.ToString();
            playerHealth.Text = player.CurrentHealth.ToString() + "/" + player.maxHealth.ToString();
            enemyHealth.Text = enemy.currentHealth.ToString() + "/" + enemy.maxHealth.ToString();
            playerHpBar.Progress = (float)player.CurrentHealth / (float)player.maxHealth;
            enemyHpBar.Progress = (float)enemy.currentHealth / (float)enemy.maxHealth;
            playerArmorLabel.Text = localizedStrings["armorText"] + " : " + player.armor.ToString();
            enemyArmorLabel.Text = localizedStrings["armorText"] + " : " + enemy.armor.ToString();

            //enemyName.Text = localizedStrings[enemy.name];
            //enemyLevel.Text = enemy.level.ToString();
            //enemyStrength.Text = enemy.strength.ToString();
            //enemyPerception.Text = enemy.perception.ToString();
            //enemyDexterity.Text = enemy.dexterity.ToString();
            //enemyAgility.Text = enemy.agility.ToString();
            //enemyVitality.Text = enemy.vitality.ToString();
            //enemyEndurance.Text = enemy.endurance.ToString();
            //enemyCharisma.Text = enemy.charisma.ToString();
            //enemyIntelligence.Text = enemy.intelligence.ToString();
            //enemyDamage.Text = localizedStrings["damText"] + " : "+ enemy.minDamage.ToString() + " - " + enemy.maxDamage.ToString();
            //enemyHealth.Text = enemy.currentHealth.ToString() + "/" + enemy.maxHealth.ToString();
            //enemyHpBar.Progress = (float)enemy.currentHealth / (float)enemy.maxHealth;
            //enemyArmor.Text = localizedStrings["armorText"] + " : "+ enemyArmor.ToString();
        }




        //Nadpisuje logikę pozwalającej na możliwość powrotu do głównego ekranu
        protected override bool OnBackButtonPressed()
        {
            if (conditionMet)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                return true;
            }
        }
        int fightSleepTimeBuffor = 0;
        private void skipWaiting_Clicked(object sender, EventArgs e)
        {
            
            mainPage.fightSleepTime = 0;
            skipWaiting.IsVisible = false;
        }

    }
}