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
using static System.Net.Mime.MediaTypeNames;

namespace GladiatorRPG
{

    //Wszystko związane z aktualizowaniem statystyk
    public partial class MainTabPage
    {
        #region Player statistics

        async void Stats_Clicked(object sender, EventArgs e)
        {
            if (!clicked)
            {
                string text = "";
                text += localizedStrings["strText"];
                if (player.bonusStrength > 0) { text += " : +" + player.bonusStrength.ToString(); } else text += " : " + player.bonusStrength.ToString();
                if (player.multipliedStrength > 0) { text += " | +" + player.multipliedStrength.ToString(); } else text += " | " + player.multipliedStrength.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseStrength * ((float)player.multipliedStrength / 100f)) + ") ";
                text += "\n" + localizedStrings["perText"];
                if (player.bonusPerception > 0) { text += " : +" + player.bonusPerception.ToString(); } else text += " : " + player.bonusPerception.ToString();
                if (player.multipliedPerception > 0) { text += " | +" + player.multipliedPerception.ToString(); } else text += " | " + player.multipliedPerception.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.basePerception * ((float)player.multipliedPerception / 100f)) + ") ";
                text += "\n" + localizedStrings["dexText"];
                if (player.bonusDexterity > 0) { text += " : +" + player.bonusDexterity.ToString(); } else text += " : " + player.bonusDexterity.ToString();
                if (player.multipliedDexterity > 0) { text += " | +" + player.multipliedDexterity.ToString(); } else text += " | " + player.multipliedDexterity.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseDexterity * ((float)player.multipliedDexterity / 100f)) + ") ";
                text += "\n" + localizedStrings["agiText"];
                if (player.bonusAgility > 0) { text += " : +" + player.bonusAgility.ToString(); } else text += " : " + player.bonusAgility.ToString();
                if (player.multipliedAgility > 0) { text += " | +" + player.multipliedAgility.ToString(); } else text += " | " + player.multipliedAgility.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseAgility * ((float)player.multipliedAgility / 100f)) + ") ";
                text += "\n" + localizedStrings["vitText"];
                if (player.bonusVitality > 0) { text += " : +" + player.bonusVitality.ToString(); } else text += " : " + player.bonusVitality.ToString();
                if (player.multipliedVitality > 0) { text += " | +" + player.multipliedVitality.ToString(); } else text += " | " + player.multipliedVitality.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseVitality * ((float)player.multipliedVitality / 100f)) + ") ";
                text += "\n" + localizedStrings["endText"];
                if (player.bonusEndurance > 0) { text += " : +" + player.bonusEndurance.ToString(); } else text += " : " + player.bonusEndurance.ToString();
                if (player.multipliedEndurance > 0) { text += " | +" + player.multipliedEndurance.ToString(); } else text += " | " + player.multipliedEndurance.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseEndurance * ((float)player.multipliedEndurance / 100f)) + ") ";
                text += "\n" + localizedStrings["chaText"];
                if (player.bonusCharisma > 0) { text += " : +" + player.bonusCharisma.ToString(); } else text += " : " + player.bonusCharisma.ToString();
                if (player.multipliedCharisma > 0) { text += " | +" + player.multipliedCharisma.ToString(); } else text += " | " + player.multipliedCharisma.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseCharisma * ((float)player.multipliedCharisma / 100f)) + ") ";
                text += "\n" + localizedStrings["intText"];
                if (player.bonusIntelligence > 0) { text += " : +" + player.bonusIntelligence.ToString(); } else text += " : " + player.bonusIntelligence.ToString();
                if (player.multipliedIntelligence > 0) { text += " | +" + player.multipliedIntelligence.ToString(); } else text += " | " + player.multipliedIntelligence.ToString();
                text+= "% ("+ (int)Math.Floor((float)player.baseIntelligence * ((float)player.multipliedIntelligence / 100f)) + ") ";
                text += "\n" + localizedStrings["lifeText"];
                if (player.bonusHealth > 0) { text += " : +" + player.bonusHealth.ToString(); } else text += " : " + player.bonusHealth.ToString();
                text += "\n" + localizedStrings["armorText"];
                if (player.bonusArmor > 0) { text += " : +" + player.bonusArmor.ToString(); } else text += " : " + player.bonusArmor.ToString();

                text += "\n\n" + localizedStrings["substatsRegeneration"] + player.regenerationHP.ToString() + "\n" +
                      localizedStrings["substatsCritDamage"] + player.critDamage.ToString() + "%\n" +
                      localizedStrings["substatsStaminaRegeneration"] + player.regenerationStamina.ToString() + "\n";

                clicked = true;
                AfterTraining();
                await DisplayAlert(localizedStrings["substatsText"]
                    , text
                    , okText);
                clicked = false;
            }
        }
        public void ZeroingBonusStats()
        {
            player.bonusHealth = 0;
            player.baseArmor = 0;
            player.bonusArmor = 0;
            player.bonusDamage = 0;
            player.bonusStrength = 0; player.multipliedStrength = 0;
            player.bonusPerception = 0; player.multipliedPerception = 0;
            player.bonusDexterity = 0; player.multipliedDexterity = 0;
            player.bonusAgility = 0; player.multipliedAgility = 0;
            player.bonusVitality = 0; player.multipliedVitality = 0;
            player.bonusEndurance = 0; player.multipliedEndurance = 0;
            player.bonusIntelligence = 0; player.multipliedIntelligence = 0;
            player.bonusCharisma = 0; player.multipliedCharisma = 0;
        }
        //Dodawanie bonusowych statystyk do podstawowych
        public void ProcessBonusStats()
        {
            player.strength = player.baseStrength + player.bonusStrength + (int)Math.Floor((float)player.baseStrength * ((float)player.multipliedStrength / 100f));
            player.perception = player.basePerception + player.bonusPerception + (int)Math.Floor((float)player.basePerception * ((float)player.multipliedPerception / 100f));
            player.dexterity = player.baseDexterity + player.bonusDexterity + (int)Math.Floor((float)player.baseDexterity * ((float)player.multipliedDexterity / 100f));
            player.agility = player.baseAgility + player.bonusAgility + (int)Math.Floor((float)player.baseAgility * ((float)player.multipliedAgility / 100f));
            player.vitality = player.baseVitality + player.bonusVitality + (int)Math.Floor((float)player.baseVitality * ((float)player.multipliedVitality / 100f));
            player.endurance = player.baseEndurance + player.bonusEndurance + (int)Math.Floor((float)player.baseEndurance * ((float)player.multipliedEndurance / 100f));
            player.charisma = player.baseCharisma + player.bonusCharisma + (int)Math.Floor((float)player.baseCharisma * ((float)player.multipliedCharisma / 100f));
            player.intelligence = player.baseIntelligence + player.bonusIntelligence + (int)Math.Floor((float)player.baseIntelligence * ((float)player.multipliedIntelligence / 100f));

            if (player.strength < 1) player.strength = 1; if (player.perception < 1) player.perception = 1; if (player.dexterity < 1) player.dexterity = 1; if (player.agility < 1) player.agility = 1;
            if (player.vitality < 1) player.vitality = 1; if (player.endurance < 1) player.endurance = 1; if (player.charisma < 1) player.charisma = 1; if (player.intelligence < 1) player.intelligence = 1;


            ProcessStatistics();
            player.maxHealth = player.baseHealth + player.bonusHealth; if (player.maxHealth < 1) player.maxHealth = 1;
            player.minDamage += player.bonusDamage; if (player.minDamage < 0) player.minDamage = 0;
            player.maxDamage += player.bonusDamage; if (player.maxDamage < 1) player.maxDamage = 1;
            player.armor = player.baseArmor + player.bonusArmor; if (player.armor < 0) player.armor = 0;
            UpdateStatsCounter();
            UpdateDamage();
        }
        //Przetwarzanie surowców gracza (hp, stamina, regeneracjaHP)
        public void ProcessStatistics()
        {
            player.regenerationHP = player.endurance + 1;
            player.maxStamina = (int)Math.Floor(player.level * player.staminaMultiplier) + 8;
            player.baseHealth = (int)Math.Floor((10 * player.vitality) * (1 + (player.level * 0.03)));
            player.maxHealth = player.baseHealth + player.bonusHealth;
            player.OnCurrentHealthChanged();
            player.OnCurrentStaminaChanged();
        }
        //Pokazanie statystyk i surowców gracza w UI
        public void UpdateStatsCounter()
        {
            name.Text = player.name;
            level.Text = localizedStrings["lvlText"] + " : " + player.level.ToString();
            str.Text = player.strength.ToString();
            if (player.strength > player.baseStrength) { str.TextColor = Color.Green; }
            else if (player.strength < player.baseStrength) { str.TextColor = Color.Red; }
            else { str.TextColor = Color.White; }

            per.Text = player.perception.ToString();
            if (player.perception > player.basePerception) { per.TextColor = Color.Green; }
            else if (player.perception < player.basePerception) { per.TextColor = Color.Red; }
            else { per.TextColor = Color.White; }

            dex.Text = player.dexterity.ToString();
            if (player.dexterity > player.baseDexterity) { dex.TextColor = Color.Green; }
            else if (player.dexterity < player.baseDexterity) { dex.TextColor = Color.Red; }
            else { dex.TextColor = Color.White; }

            agi.Text = player.agility.ToString();
            if (player.agility > player.baseAgility) { agi.TextColor = Color.Green; }
            else if (player.agility < player.baseAgility) { agi.TextColor = Color.Red; }
            else { agi.TextColor = Color.White; }

            vit.Text = player.vitality.ToString();
            if (player.vitality > player.baseVitality) { vit.TextColor = Color.Green; }
            else if (player.vitality < player.baseVitality) { vit.TextColor = Color.Red; }
            else { vit.TextColor = Color.White; }

            end.Text = player.endurance.ToString();
            if (player.endurance > player.baseEndurance) { end.TextColor = Color.Green; }
            else if (player.endurance < player.baseEndurance) { end.TextColor = Color.Red; }
            else { end.TextColor = Color.White; }

            cha.Text = player.charisma.ToString();
            if (player.charisma > player.baseCharisma) { cha.TextColor = Color.Green; }
            else if (player.charisma < player.baseCharisma) { cha.TextColor = Color.Red; }
            else { cha.TextColor = Color.White; }

            wis.Text = player.intelligence.ToString();
            if (player.intelligence > player.baseIntelligence) { wis.TextColor = Color.Green; }
            else if (player.intelligence < player.baseIntelligence) { wis.TextColor = Color.Red; }
            else { wis.TextColor = Color.White; }

            player.HealthUpdate();

            armorCounter.Text = armorText + " : " + player.armor.ToString();

        }
        public void UpdateDamage()
        {
            player.minDamage = 1 + (int)Math.Floor((float)player.strength / 10f); if (player.weapon != null) { player.minDamage += player.weapon.minDamage; }
            player.maxDamage = 1 + (int)Math.Floor((float)player.strength / 5f); if (player.weapon != null) { player.maxDamage += player.weapon.maxDamage; }
            if (player.bonusDamage > 0) { player.minDamage += player.bonusDamage; player.maxDamage += player.bonusDamage; }
            damage.Text = player.minDamage.ToString() + " - " + player.maxDamage.ToString();
        }


        //Przetwarzanie dodatkowych statystyk z ekwipunku założonego
        public void ProcessEquippedItems()
        {
            string spaces = "\n\n\n\n\n\n\n\n";
            List<Item> equippedItems = new List<Item>();
            if (player.head != null)
            {
                equippedItems.Add(player.head); helmet.Text = spaces + localizedStrings[player.head.itemName]; helmet.TextColor = player.head.color;
                if (player.head.itemImage != null) { helmetImage.Source = player.head.itemImage; }
                else { helmetImage.Source = "emptySlot"; }//zamienić potem na defaultHelmet
            }
            else { helmet.Text = spaces + headButtonText; helmet.TextColor = Color.Default; helmetImage.Source = "emptySlot"; }

            if (player.torso != null)
            {
                equippedItems.Add(player.torso); torso.Text = spaces + localizedStrings[player.torso.itemName]; torso.TextColor = player.torso.color;
                if (player.torso.itemImage != null) { torsoImage.Source = player.torso.itemImage; }
                else { torsoImage.Source = "emptySlot"; }//zamienić potem na defaultTorso

            }
            else { torso.Text = spaces + torsoButtonText; torso.TextColor = Color.Default; torsoImage.Source = "emptySlot"; }

            if (player.boots != null)
            {
                equippedItems.Add(player.boots); boots.Text = spaces + localizedStrings[player.boots.itemName]; boots.TextColor = player.boots.color;
                if (player.boots.itemImage != null) { bootsImage.Source = player.boots.itemImage; }
                else { bootsImage.Source = "emptySlot"; }//zamienić potem na defaultBoots

            }
            else { boots.Text = spaces + legsButtonText; boots.TextColor = Color.Default; bootsImage.Source = "emptySlot"; }

            if (player.gloves != null)
            {
                equippedItems.Add(player.gloves); gloves.Text = spaces + localizedStrings[player.gloves.itemName]; gloves.TextColor = player.gloves.color;
                if (player.gloves.itemImage != null) { glovesImage.Source = player.gloves.itemImage; }
                else { glovesImage.Source = "emptySlot"; }//zamienić potem na defaultGloves

            }
            else { gloves.Text = spaces + handsButtonText; gloves.TextColor = Color.Default; glovesImage.Source = "emptySlot"; }

            if (player.weapon != null)
            {
                equippedItems.Add(player.weapon); weapon.Text = spaces + localizedStrings[player.weapon.itemName]; weapon.TextColor = player.weapon.color;
                if (player.weapon.itemImage != null) { weaponImage.Source = player.weapon.itemImage; }
                else { weaponImage.Source = "emptySlot"; }//zamienić potem na defaultWeapon

            }
            else { weapon.Text = spaces + weaponButtonText; weapon.TextColor = Color.Default; weaponImage.Source = "emptySlot"; }

            if (player.shield != null)
            {
                equippedItems.Add(player.shield); shield.Text = spaces + localizedStrings[player.shield.itemName]; shield.TextColor = player.shield.color;
                if (player.shield.itemImage != null) { shieldImage.Source = player.shield.itemImage; }
                else { shieldImage.Source = "emptySlot"; }//zamienić potem na defaultShield

            }
            else { shield.Text = spaces + shieldButtonText; shield.TextColor = Color.Default; shieldImage.Source = "emptySlot"; }

            if (player.ring != null)
            {
                equippedItems.Add(player.ring); ring.Text = spaces + localizedStrings[player.ring.itemName]; ring.TextColor = player.ring.color;
                if (player.ring.itemImage != null) { ringImage.Source = player.ring.itemImage; }
                else { ringImage.Source = "emptySlot"; }//zamienić potem na defaultRing

            }
            else { ring.Text = spaces + ringButtonText; ring.TextColor = Color.Default; ringImage.Source = "emptySlot"; }

            if (player.necklace != null)
            {
                equippedItems.Add(player.necklace); necklace.Text = spaces + localizedStrings[player.necklace.itemName]; necklace.TextColor = player.necklace.color;
                if (player.necklace.itemImage != null) { necklaceImage.Source = player.necklace.itemImage; }
                else { necklaceImage.Source = "emptySlot"; }//zamienić potem na defaultNecklace

            }
            else { necklace.Text = spaces + necklaceButtonText; necklace.TextColor = Color.Default; necklaceImage.Source = "emptySlot"; }

            if (player.belt != null)
            {
                equippedItems.Add(player.belt); belt.Text = spaces + localizedStrings[player.belt.itemName]; belt.TextColor = player.belt.color;
                if (player.belt.itemImage != null) { beltImage.Source = player.belt.itemImage; }
                else { beltImage.Source = "emptySlot"; }//zamienić potem na defaultBelt

            }
            else { belt.Text = spaces + beltButtonText; belt.TextColor = Color.Default; beltImage.Source = "emptySlot"; }
            ZeroingBonusStats();
            if (equippedItems.Count > 0)
            {


                foreach (Item item in equippedItems)
                {
                    if (item.healthBonus != 0) { player.bonusHealth += item.healthBonus; }
                    if (item.armorValue != 0) { player.baseArmor += item.armorValue; }
                    if (item.armorBonus != 0) { player.bonusArmor += item.armorBonus; }
                    if (item.damageBonus != 0) { player.bonusDamage += item.damageBonus; }


                    if (item.strengthBonus != 0) { player.bonusStrength += item.strengthBonus; }
                    if (item.perceptionBonus != 0) { player.bonusPerception += item.perceptionBonus; }
                    if (item.dexterityBonus != 0) { player.bonusDexterity += item.dexterityBonus; }
                    if (item.agilityBonus != 0) { player.bonusAgility += item.agilityBonus; }
                    if (item.vitalityBonus != 0) { player.bonusVitality += item.vitalityBonus; }
                    if (item.enduranceBonus != 0) { player.bonusEndurance += item.enduranceBonus; }
                    if (item.charismaBonus != 0) { player.bonusCharisma += item.charismaBonus; }
                    if (item.intelligenceBonus != 0) { player.bonusIntelligence += item.intelligenceBonus; }

                    if (item.strengthMultiplier != 0) { player.multipliedStrength += item.strengthMultiplier; }
                    if (item.perceptionMultiplier != 0) { player.multipliedPerception += item.perceptionMultiplier; }
                    if (item.dexterityMultiplier != 0) { player.multipliedDexterity += item.dexterityMultiplier; }
                    if (item.agilityMultiplier != 0) { player.multipliedAgility += item.agilityMultiplier; }
                    if (item.vitalityMultiplier != 0) { player.multipliedVitality += item.vitalityMultiplier; }
                    if (item.enduranceMultiplier != 0) { player.multipliedEndurance += item.enduranceMultiplier; }
                    if (item.charismaMultiplier != 0) { player.multipliedCharisma += item.charismaMultiplier; }
                    if (item.intelligenceMultiplier != 0) { player.multipliedIntelligence += item.intelligenceMultiplier; }
                }

            }
            UpdateDamage();
            ProcessBonusStats();
        }

        public void EverythingStats()
        {
            ProcessBonusStats();

            ProcessStatistics();

            ProcessEquippedItems();

            UpdateStatsCounter();

            UpdateDamage();
        }
        #endregion
    }
}
