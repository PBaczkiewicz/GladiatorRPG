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

    public partial class MainTabPage : TabbedPage
    {
        public Entity EnemyGeneration(string name, string imageSource, int enemylvl, int items = 1, float strPer = 0, float perPer = 0, float dexPer = 0, float agiPer = 0, float vitPer = 0, float endPer = 0, float chaPer = 0, float intPer = 0, int preset = 0)
        {
            int bonusStats = items;
            if (bonusStats > 9) bonusStats = 9;
            List<Item> enemyItemList = new List<Item>();
            enemy = new Entity();
            enemy.name = name; enemy.source = imageSource;
            enemy.level = enemylvl;

            //string xz ="PRZED RANDOMIZEREM\n" + strPer.ToString() + "\n" +
            //    perPer.ToString() + "\n" +
            //    dexPer.ToString() + "\n" +
            //    agiPer.ToString() + "\n" +
            //    vitPer.ToString() + "\n" +
            //    endPer.ToString() + "\n" +
            //    chaPer.ToString() + "\n" +
            //    intPer.ToString() + "\n";

            #region slightly randomizing stats
            strPer = (int)Math.Round(strPer*((float)random.Next(90, 110) / 100f));
            perPer = (int)Math.Round(perPer*((float)random.Next(90, 110) / 100f));
            dexPer = (int)Math.Round(dexPer*((float)random.Next(90, 110) / 100f));
            agiPer = (int)Math.Round(agiPer*((float)random.Next(90, 110) / 100f));
            vitPer = (int)Math.Round(vitPer*((float)random.Next(90, 110) / 100f));
            endPer = (int)Math.Round(endPer*((float)random.Next(90, 110) / 100f));
            chaPer = (int)Math.Round(chaPer*((float)random.Next(90, 110) / 100f));
            intPer = (int)Math.Round(intPer*((float)random.Next(90, 110) / 100f));
            #endregion

            //TEST(xz+"=================\n"+"PO RANDOMIZERZE\n" + strPer.ToString() + "\n" +
            //    perPer.ToString() + "\n" +
            //    dexPer.ToString() + "\n" +
            //    agiPer.ToString() + "\n" +
            //    vitPer.ToString() + "\n" +
            //    endPer.ToString() + "\n" +
            //    chaPer.ToString() + "\n" +
            //    intPer.ToString() + "\n");

            if (preset != 0) { PresetGeneration(enemylvl, preset); }
            else
            {
                enemy.baseStrength = (int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + strPer) / 100f));   //2.75f) + 8f) * (strPer / 100f));
                enemy.basePerception = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + perPer) / 100f)));   //2.75f) + 8f) * (perPer / 100f));
                enemy.baseDexterity = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + dexPer) / 100f)));   //2.75f) + 8f) * (dexPer / 100f));
                enemy.baseAgility = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + agiPer) / 100f)));   //2.75f) + 8f) * (agiPer / 100f));
                enemy.baseVitality = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + vitPer) / 100f)));   //2.75f) + 8f) * (vitPer / 100f));
                enemy.baseEndurance = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + endPer) / 100f)));   //2.75f) + 8f) * (endPer / 100f));
                enemy.baseCharisma = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + chaPer) / 100f)));   //2.75f) + 8f) * (chaPer / 100f));
                enemy.baseIntelligence = ((int)Math.Floor((((float)enemy.level * 3f) + 8f) * ((player.winStreak + intPer) / 100f)));   //2.75f) + 8f) * (intPer / 100f));

            }

            if (items > 0) { enemy.weapon = ItemGenerator((int)Math.Ceiling(enemylvl * 0.95f), (int)Math.Ceiling(enemylvl * 1.05f), 0, ItemClass.Weapon); items--; enemyItemList.Add(enemy.weapon); }
            if (items > 0) { enemy.shield = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Shield); items--; enemyItemList.Add(enemy.shield); }
            if (items > 0) { enemy.torso = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Torso); items--; enemyItemList.Add(enemy.torso); }
            if (items > 0) { enemy.head = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Helmet); items--; enemyItemList.Add(enemy.head); }
            if (items > 0) { enemy.gloves = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Gloves); items--; enemyItemList.Add(enemy.gloves); }
            if (items > 0) { enemy.boots = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Boots); items--; enemyItemList.Add(enemy.boots); }
            if (items > 0) { enemy.ring = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Ring); items--; enemyItemList.Add(enemy.ring); }
            if (items > 0) { enemy.belt = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Belt); items--; enemyItemList.Add(enemy.belt); }
            if (items > 0) { enemy.necklace = ItemGenerator((int)Math.Ceiling(enemylvl * ((float)random.Next(80, 100) / 100)), enemylvl, 0, ItemClass.Necklace); items--; enemyItemList.Add(enemy.necklace); }

            foreach (Item x in enemyItemList)
            {
                #region stat adding (disabled
                //if (x.strengthBonus > 0) enemy.bonusStrength += x.strengthBonus;
                //if (x.strengthMultiplier > 0) enemy.multipliedStrength += x.strengthMultiplier;
                //if (x.perceptionBonus > 0) enemy.bonusPerception += x.perceptionBonus;
                //if (x.perceptionMultiplier > 0) enemy.multipliedPerception += x.perceptionMultiplier;
                //if (x.dexterityBonus > 0) enemy.bonusDexterity += x.dexterityBonus;
                //if (x.dexterityMultiplier > 0) enemy.multipliedDexterity += x.dexterityMultiplier;
                //if (x.agilityBonus > 0) enemy.bonusAgility += x.agilityBonus;
                //if (x.agilityMultiplier > 0) enemy.multipliedAgility += x.agilityMultiplier;
                //if (x.vitalityBonus > 0) enemy.bonusVitality += x.vitalityBonus;
                //if (x.vitalityMultiplier > 0) enemy.multipliedVitality += x.vitalityMultiplier;
                //if (x.enduranceBonus > 0) enemy.bonusEndurance += x.enduranceBonus;
                //if (x.enduranceMultiplier > 0) enemy.multipliedEndurance += x.enduranceMultiplier;
                //if (x.charismaBonus > 0) enemy.bonusCharisma += x.charismaBonus;
                //if (x.charismaMultiplier > 0) enemy.multipliedCharisma += x.charismaMultiplier;
                //if (x.intelligenceBonus > 0) enemy.bonusIntelligence += x.intelligenceBonus;
                //if (x.intelligenceMultiplier > 0) enemy.multipliedIntelligence += x.intelligenceMultiplier;
                //if (x.armorBonus > 0) { enemy.bonusArmor += x.armorBonus; }
                #endregion
                if (x.armorValue > 0) { enemy.baseArmor += x.armorValue; }

            }

            if (enemy.baseArmor > player.armor) //DO TESTOWANIA NA TEN MOMENT!!! 16.04.2024
            {
                enemy.baseArmor = player.armor + (int)Math.Floor(((float)enemy.baseArmor - (float)player.armor) / 2f);
            }


            if (enemy.level <= 7)
            {
                bonusStats = 0;
            }
            else if (enemy.level < 14)
            {
                bonusStats = (int)Math.Ceiling((enemy.level - 7 * 0.33333f) + bonusStats * 1.5f);
            }
            else if (enemy.level < 21)
            {
                bonusStats = (int)Math.Ceiling((enemy.level - 14 * 0.33333f) + bonusStats * 2f);

            }
            else if (enemy.level < 28)
            {
                bonusStats = (int)Math.Ceiling((enemy.level - 21 * 0.33333f) + bonusStats * 2.5f);

            }
            else if (enemy.level < 35)
            {
                bonusStats = (int)Math.Ceiling((enemy.level - 28 * 0.33333f) + bonusStats * 3f);

            }
            else
            {
                bonusStats = (int)Math.Ceiling((enemy.level - 35 * 0.33333f) + bonusStats * 4f);

            }

            while (bonusStats > 0)
            {
                int roll = random.Next(0, 11);
                switch (roll)
                {
                    case 0:
                        enemy.bonusDamage += StatBonus(0.5f);
                        break;

                    case 1:
                        enemy.bonusArmor += StatBonus(10);
                        break;
                    case 2:
                        enemy.bonusStrength += StatBonus();
                        break;
                    case 3:
                        enemy.bonusPerception += StatBonus();
                        break;
                    case 4:
                        enemy.bonusDexterity += StatBonus();
                        break;
                    case 5:
                        enemy.bonusAgility += StatBonus();
                        break;
                    case 6:
                        enemy.bonusEndurance += StatBonus();
                        break;
                    case 7:
                        enemy.bonusVitality += StatBonus();
                        break;
                    case 8:
                        enemy.bonusCharisma += StatBonus();
                        break;
                    case 9:
                        enemy.bonusIntelligence += StatBonus();
                        break;
                    case 10:
                        enemy.bonusHealth += StatBonus(8);
                        break;
                }

                bonusStats--;
            }



            if (enemy.weapon == null) enemy.weapon = new Item("weaponClub", ItemClass.Weapon, Color.White, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, localizedStrings);

            enemy.strength = enemy.baseStrength + enemy.bonusStrength; enemy.strength += (int)Math.Floor((float)enemy.multipliedStrength / 100 * (float)enemy.baseStrength);
            enemy.perception = enemy.basePerception + enemy.bonusPerception; enemy.perception += (int)Math.Floor((float)enemy.multipliedPerception / 100 * (float)enemy.basePerception);
            enemy.dexterity = enemy.baseDexterity + enemy.bonusDexterity; enemy.dexterity += (int)Math.Floor((float)enemy.multipliedDexterity / 100 * (float)enemy.baseDexterity);
            enemy.agility = enemy.baseAgility + enemy.bonusAgility; enemy.agility += (int)Math.Floor((float)enemy.multipliedAgility / 100 * (float)enemy.baseDexterity);
            enemy.vitality = enemy.baseVitality + enemy.bonusVitality; enemy.vitality += (int)Math.Floor((float)enemy.multipliedVitality / 100 * (float)enemy.baseVitality);
            enemy.endurance = enemy.baseEndurance + enemy.bonusEndurance; enemy.endurance += (int)Math.Floor((float)enemy.multipliedEndurance / 100 * (float)enemy.baseEndurance);
            enemy.charisma = enemy.baseCharisma + enemy.bonusCharisma; enemy.charisma += (int)Math.Floor((float)enemy.multipliedCharisma / 100 * (float)enemy.baseCharisma);
            enemy.intelligence = enemy.baseIntelligence + enemy.bonusIntelligence; enemy.intelligence += (int)Math.Floor((float)enemy.multipliedIntelligence / 100 * (float)enemy.baseIntelligence);
            enemy.minDamage = enemy.weapon.minDamage + enemy.bonusDamage + (int)Math.Floor((float)enemy.strength / 10f);
            enemy.maxDamage = enemy.weapon.maxDamage + enemy.bonusDamage + (int)Math.Floor((float)enemy.strength / 5f);
            enemy.armor = enemy.baseArmor + enemy.bonusArmor;
            enemy.baseHealth = (int)Math.Floor((10 * enemy.vitality) * (1 + (enemy.level * 0.03)));
            enemy.maxHealth = enemy.baseHealth + enemy.bonusHealth; enemy.currentHealth = enemy.maxHealth;

            return enemy;
        }
        void UpdateArenaPresetStats()
        {
            enemy.minDamage = enemy.weapon.minDamage + enemy.bonusDamage + (int)Math.Floor((float)enemy.strength / 10f);
            enemy.maxDamage = enemy.weapon.maxDamage + enemy.bonusDamage + (int)Math.Floor((float)enemy.strength / 5f);
            enemy.armor = enemy.baseArmor + enemy.bonusArmor;
            enemy.baseHealth = (int)Math.Floor((10 * enemy.vitality) * (1 + (enemy.level * 0.03)));
            enemy.maxHealth = enemy.baseHealth + enemy.bonusHealth; enemy.currentHealth = enemy.maxHealth;
        }
        void PresetGeneration(int enemylvl, int preset)
        {

        }

        int StatBonus(float modifier = 1)
        {
            return (int)Math.Floor((((float)random.Next(90, 111) / 100f) * (0.1f * (8f + (float)enemy.level * 3f))) * modifier); //3F MODIFIED FROM 2.75F
        }
    }
}
