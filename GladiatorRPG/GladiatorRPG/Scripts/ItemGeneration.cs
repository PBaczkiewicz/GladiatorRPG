using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GladiatorRPG
{
    //Generacja itemków i affixów do nich
    public partial class MainTabPage
    {
        

        //Generator itemów
        public Item ItemGenerator(int minItemLevel, int maxItemLevel, int itemRarity = 0, ItemClass iclass = ItemClass.nothing, bool enemyItem = false)
        {
            if (minItemLevel <= 0) { minItemLevel = 1; }
            if(maxItemLevel<=0) { maxItemLevel = 1; }
            Item item;
            //float rarityModifier = 1f;
            bool proper = false;
            do
            {
                float roll;
                float dropChanceGreen7To14 = baseDropChanceGreen7To14;//dodać modyfikator gracza
                float dropChanceBlue14To21 = baseDropChanceBlue14To21;
                float dropChanceViolet21To28 = baseDropChanceViolet21To28;
                float dropChanceBlue21To28 = baseDropChanceBlue21To28;
                float dropChanceOrange28To35 = baseDropChanceOrange28To35;
                float dropChanceViolet28To35 = baseDropChanceViolet28To35;
                float dropChanceBlue28To35 = baseDropChanceBlue28To35;
                float dropChanceRed35Up = baseDropChanceRed35Up;
                float dropChanceOrange35Up = baseDropChanceOrange35Up;
                float dropChanceViolet35Up = baseDropChanceViolet35Up;
                float dropChanceBlue35Up = baseDropChanceBlue35Up;


                item = new Item("Error", ItemClass.nothing, Color.White, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, localizedStrings);
                int itemType;

                //TEST
                item.itemImage = "itemTemplate";

                Color color = Color.White;
                roll = ((float)random.Next(0, 10001) / 100f);
                item.levelReq = random.Next(minItemLevel, maxItemLevel + 1);
                if (iclass == ItemClass.nothing)
                {
                    if (item.levelReq >= 7) itemType = random.Next(0, 9);//PAMIĘTAĆ POTEM TO ZMIENIĆ, JAK WYLOSUJE INNĄ LICZBĘ TO ZOSTAJE ERROR!!!!!!!!!!
                    else itemType = random.Next(0, 6);
                    switch (itemType)
                    {
                        case 0:
                            item.itemClass = ItemClass.Weapon;
                            break;
                        case 1:
                            item.itemClass = ItemClass.Gloves;
                            break;
                        case 2:
                            item.itemClass = ItemClass.Helmet;
                            break;
                        case 3:
                            item.itemClass = ItemClass.Torso;
                            break;
                        case 4:
                            item.itemClass = ItemClass.Boots;
                            break;
                        case 5:
                            item.itemClass = ItemClass.Shield;
                            break;
                        case 6:
                            item.itemClass = ItemClass.Necklace;
                            break;
                        case 7:
                            item.itemClass = ItemClass.Belt;
                            break;
                        case 8:
                            item.itemClass = ItemClass.Ring;
                            break;

                    }
                }
                else { item.itemClass = iclass; }
                //MethodToModifyDropChances();
                #region Level checking for item color

                if (itemRarity == 0)
                {
                    if (item.levelReq < 7) { color = Color.White; }

                    else if (item.levelReq >= 7 && item.levelReq < 14)
                    {
                        if (roll <= dropChanceGreen7To14) { color = Color.LightGreen; }
                        else { color = Color.White; }
                    }

                    else if (item.levelReq >= 14 && item.levelReq < 21)
                    {
                        if (roll <= dropChanceBlue14To21) { color = Color.DodgerBlue; }
                        else { color = Color.LightGreen; }
                    }

                    else if (item.levelReq >= 21 && item.levelReq < 28)
                    {
                        if (roll <= dropChanceViolet21To28) { color = Color.DarkViolet; }
                        else if (roll <= (dropChanceViolet21To28 + dropChanceBlue21To28)) { color = Color.DodgerBlue; }
                        else { color = Color.LightGreen; }

                    }

                    else if (item.levelReq >= 28 && item.levelReq < 35)
                    {
                        if (roll <= dropChanceOrange28To35) { color = Color.Orange; }
                        else if (roll <= dropChanceOrange28To35 + dropChanceViolet28To35) { color = Color.DarkViolet; }
                        else if (roll <= dropChanceOrange28To35 + dropChanceViolet28To35 + dropChanceBlue28To35) { color = Color.DodgerBlue; }
                        else { color = Color.LightGreen; }
                    }

                    else if (item.levelReq >= 35)
                    {
                        if (roll <= dropChanceRed35Up) { color = Color.Red; }
                        else if (roll <= dropChanceRed35Up + dropChanceOrange35Up) { color = Color.Orange; }
                        else if (roll <= dropChanceRed35Up + dropChanceOrange35Up + dropChanceViolet35Up) { color = Color.DarkViolet; }
                        else if (roll <= dropChanceRed35Up + dropChanceOrange35Up + dropChanceViolet35Up + dropChanceBlue35Up) { color = Color.DodgerBlue; }
                        else { color = Color.LightGreen; }
                    }
                }
                else
                {
                    if (itemRarity == -1) { itemRarity = (int)Math.Floor((float)item.levelReq / 7f) + 1; }
                    if (itemRarity == 2) { color = Color.LightGreen; }
                    else if (itemRarity == 3) { color = Color.DodgerBlue; }
                    else if (itemRarity == 4) { color = Color.DarkViolet; }
                    else if (itemRarity == 5) { color = Color.Orange; }
                    else if (itemRarity == 6) { color = Color.Red; }
                    else { color = Color.White; }

                }
                #endregion

                if (item.itemClass == ItemClass.Weapon) { WeaponRoll(item, color); }
                else if (item.itemClass == ItemClass.Torso) { TorsoRoll(item, color); }
                else if (item.itemClass == ItemClass.Helmet) { HelmetRoll(item, color); }
                else if (item.itemClass == ItemClass.Gloves) { GlovesRoll(item, color); }
                else if (item.itemClass == ItemClass.Boots) { BootsRoll(item, color); }
                else if (item.itemClass == ItemClass.Shield) { ShieldRoll(item, color); }
                else if (item.itemClass == ItemClass.Necklace) { NecklaceRoll(item); }
                else if (item.itemClass == ItemClass.Belt) { BeltRoll(item); }
                else if (item.itemClass == ItemClass.Ring) { RingRoll(item); }

                if (color != Color.White)
                {
                    RollAffix(item, color);

                }

                //if (color == Color.White) { rarityModifier = 1f; }
                //else if (color == Color.LightGreen) { rarityModifier = 1.5f; }
                //else if (color == Color.DodgerBlue) { rarityModifier = 2.5f; }
                //else if (color == Color.DarkViolet) { rarityModifier = 4f; }
                //else if (color == Color.Orange) { rarityModifier = 7f; }
                //else if (color == Color.Red) { rarityModifier = 12f; }

                //item.sellValue = (int)Math.Floor(rarityModifier * (float)((float)item.levelReq * 10f + StatValue(item)));



                item.color = color;
                item.UpdateDescription(localizedStrings);
                if (item.itemName == "Error") { proper = false; }
                else { proper = true; }
            } while (proper == false);



            //if (enemyItem) 
            //{
            //    if (item.itemClass == ItemClass.Weapon) { enemy.weapon = item; }
            //    else if (item.itemClass == ItemClass.Torso) { enemy.torso = item; }
            //    else if (item.itemClass == ItemClass.Helmet) { enemy.head = item; }
            //    else if (item.itemClass == ItemClass.Gloves) { enemy.gloves = item; }
            //    else if (item.itemClass == ItemClass.Boots) { enemy.boots = item; }
            //    else if (item.itemClass == ItemClass.Shield) { enemy.shield = item; }
            //    else if (item.itemClass == ItemClass.Necklace) { enemy.necklace = item; }
            //    else if (item.itemClass == ItemClass.Belt) { enemy.belt = item; }
            //    else if (item.itemClass == ItemClass.Ring) { enemy.ring = item; }
            //    item = null;
            //    return item;
            //}
            //player.items.Add(item);

            GridEquipment();
            return item;
        }
        float StatValue(Item i)
        {
            float value = 0;
            if (i.healthBonus > 0) value += i.healthBonus;
            if (i.armorBonus > 0) value += i.armorBonus;
            if (i.damageBonus > 0) value += i.damageBonus * 10;
            if (i.strengthBonus > 0) value += i.strengthBonus;
            if (i.strengthBonus > 0) value += i.strengthBonus * 2;
            if (i.perceptionBonus > 0) value += i.perceptionBonus;
            if (i.perceptionMultiplier > 0) value += i.perceptionMultiplier * 2;
            if (i.dexterityBonus > 0) value += i.dexterityBonus;
            if (i.dexterityMultiplier > 0) value += i.dexterityMultiplier * 2;
            if (i.agilityBonus > 0) value += i.agilityBonus;
            if (i.agilityMultiplier > 0) value += i.agilityMultiplier * 2;
            if (i.enduranceBonus > 0) value += i.enduranceBonus;
            if (i.enduranceMultiplier > 0) value += i.enduranceMultiplier * 2;
            if (i.vitalityBonus > 0) value += i.vitalityBonus;
            if (i.vitalityMultiplier > 0) value += i.vitalityMultiplier * 2;
            if (i.charismaBonus > 0) value += i.charismaBonus;
            if (i.charismaMultiplier > 0) value += i.charismaMultiplier * 2;
            if (i.intelligenceBonus > 0) value += i.intelligenceBonus;
            if (i.intelligenceMultiplier > 0) value += i.intelligenceMultiplier * 2;

            return value;
        }
        void WeaponRoll(Item item, Color color)
        {

            int x;
            float min = 1, max = 1, rarityModifier = 1f;
            int itemChoice = item.levelReq;
            item.itemImage = "weapon_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }

            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponDagger"; item.minDamage = 1; item.maxDamage = 1; min = 0.8f; max = 1f; }
                    else if (x == 1) { item.itemName = "weaponClub"; item.minDamage = 0; item.maxDamage = 2; min = 0.6f; max = 1.2f; }
                    else return;
                    break;
                case 2:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponShortsword"; item.minDamage = 1; item.maxDamage = 2; min = 0.64f; max = 1.2f; }
                    else if (x == 1) { item.itemName = "weaponSpear"; item.minDamage = 0; item.maxDamage = 3; min = 0.52f; max = 1.32f; }
                    else return;
                    break;
                case 3:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponCudgel"; item.minDamage = 2; item.maxDamage = 3; min = 0.48f; max = 1.4f; }
                    else if (x == 1) { item.itemName = "weaponSickle"; item.minDamage = 1; item.maxDamage = 4; min = 0.75f; max = 1.13f; }
                    else return;
                    break;
                case 4:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponSica"; item.minDamage = 3; item.maxDamage = 5; min = 0.8f; max = 1.12f; }
                    else if (x == 1) { item.itemName = "weaponMachaira"; item.minDamage = 1; item.maxDamage = 7; min = 0.5f; max = 1.42f; }
                    else return;
                    break;
                case 5:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponHatchet"; item.minDamage = 3; item.maxDamage = 6; min = 0.7f; max = 1.26f; }
                    else if (x == 1) { item.itemName = "weaponAkinaes"; item.minDamage = 4; item.maxDamage = 5; min = 0.8f; max = 1.16f; }
                    else return;
                    break;
                case 6:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponSeaks"; item.minDamage = 5; item.maxDamage = 5; min = 0.9f; max = 1.1f; }
                    else if (x == 1) { item.itemName = "weaponPugio"; item.minDamage = 2; item.maxDamage = 8; min = 0.82f; max = 1.18f; }
                    else return;
                    break;
                case 7:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponXiphos"; item.minDamage = 5; item.maxDamage = 7; min = 0.89f; max = 1.15f; }
                    else if (x == 1) { item.itemName = "weaponAngon"; item.minDamage = 3; item.maxDamage = 9; min = 0.65f; max = 1.39f; }
                    else return;
                    break;
                case 8:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponBattleChain"; item.minDamage = 0; item.maxDamage = 12; min = 0.25f; max = 1.83f; }
                    else if (x == 1) { item.itemName = "weaponKopis"; item.minDamage = 4; item.maxDamage = 8; min = 0.8f; max = 1.28f; }
                    else return;
                    break;
                case 9:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponLongsword"; item.minDamage = 7; item.maxDamage = 7; min = 0.9f; max = 1.22f; }
                    else if (x == 1) { item.itemName = "weaponHasta"; item.minDamage = 3; item.maxDamage = 11; min = 0.42f; max = 1.7f; }
                    else return;
                    break;
                case 10:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponFranciska"; item.minDamage = 5; item.maxDamage = 10; min = 0.75f; max = 1.41f; }
                    else if (x == 1) { item.itemName = "weaponFalcata"; item.minDamage = 6; item.maxDamage = 9; min = 0.86f; max = 1.3f; }
                    else return;
                    break;
                case 11:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponTrident"; item.minDamage = 4; item.maxDamage = 12; min = 0.7f; max = 1.5f; }
                    else if (x == 1) { item.itemName = "weaponHammer"; item.minDamage = 0; item.maxDamage = 16; min = 0.2f; max = 2f; }
                    else return;
                    break;
                case 12:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponGladius"; item.minDamage = 7; item.maxDamage = 10; min = 0.9f; max = 1.34f; }
                    else if (x == 1) { item.itemName = "weaponPilum"; item.minDamage = 3; item.maxDamage = 14; min = 0.59f; max = 1.65f; }
                    else return;
                    break;
                case 13:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponKhopesh"; item.minDamage = 6; item.maxDamage = 12; min = 0.78f; max = 1.5f; }
                    else if (x == 1) { item.itemName = "weaponSpatha"; item.minDamage = 8; item.maxDamage = 10; min = 1f; max = 1.28f; }
                    else return;
                    break;
                case 14:
                    x = random.Next(0, 2);
                    if (x == 0) { item.itemName = "weaponHugeSword"; item.minDamage = 10; item.maxDamage = 10; min = 0.85f; max = 1.47f; }
                    else if (x == 1) { item.itemName = "weaponHugeAxe"; item.minDamage = 5; item.maxDamage = 15; min = 0.5f; max = 1.82f; }
                    else return;
                    break;
            }
            if (item.levelReq > 14)
            {
                item.minDamage = (int)Math.Floor((1f * (float)item.levelReq - 10f) * (2.75f + (((float)item.levelReq - 10f) * 0.01f)) * (float)min * (float)rarityModifier);
                item.maxDamage = (int)Math.Ceiling((1f * (float)item.levelReq - 10f) * (2.75f + (((float)item.levelReq - 10f) * 0.01f)) * (float)max * (float)rarityModifier);

            }
            return;
        }
        void TorsoRoll(Item item, Color color)
        {
            float modifier = 1f, rarityModifier = 1f; ;
            int itemChoice = item.levelReq;
            item.itemImage = "armor_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }

            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "torsoRags"; item.armorValue = 6; modifier = 1f;
                    break;
                case 2:
                    item.itemName = "torsoChiton"; item.armorValue = 12; modifier = 1.05f;
                    break;
                case 3:
                    item.itemName = "torsoCaftan"; item.armorValue = 16; modifier = 1.1f;
                    break;
                case 4:
                    item.itemName = "torsoLeatherJacket"; item.armorValue = 20; modifier = 1.15f;
                    break;
                case 5:
                    item.itemName = "torsoLeatherArmor"; item.armorValue = 30; modifier = 1.2f;
                    break;
                case 6:
                    item.itemName = "torsoReinforcedHide"; item.armorValue = 50; modifier = 1.25f;
                    break;
                case 7:
                    item.itemName = "torsoGalerus"; item.armorValue = 70; modifier = 1.3f;
                    break;
                case 8:
                    item.itemName = "torsoHauberk"; item.armorValue = 80; modifier = 1.35f;
                    break;
                case 9:
                    item.itemName = "torsoMetalPlatedArmor"; item.armorValue = 100; modifier = 1.4f;
                    break;
                case 10:
                    item.itemName = "torsoHeavyHauberk"; item.armorValue = 120; modifier = 1.45f;
                    break;
                case 11:
                    item.itemName = "torsoSegmentedArmor"; item.armorValue = 140; modifier = 1.5f;
                    break;
                case 12:
                    item.itemName = "torsoBronzeArmor"; item.armorValue = 160; modifier = 1.55f;
                    break;
                case 13:
                    item.itemName = "torsoScaleArmor"; item.armorValue = 180; modifier = 1.6f;
                    break;
                case 14:
                    item.itemName = "torsoLamellarArmor"; item.armorValue = 200; modifier = 1.65f;
                    break;
            }
            if (item.levelReq > 14) item.armorValue = (int)Math.Floor(200 * (float)modifier * (((float)item.levelReq / 20f) * (float)rarityModifier));
            return;

        }
        void HelmetRoll(Item item, Color color)
        {
            float modifier = 1f, rarityModifier = 1f; ;
            int itemChoice = item.levelReq;
            item.itemImage = "helmet_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }


            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "helmetCap"; item.armorValue = 3; modifier = 1f;
                    break;
                case 2:
                    item.itemName = "helmetLeatherCap"; item.armorValue = 7; modifier = 1.05f;
                    break;
                case 3:
                    item.itemName = "helmetLeatherHelmet"; item.armorValue = 9; modifier = 1.1f;
                    break;
                case 4:
                    item.itemName = "helmetBronzeHelmet"; item.armorValue = 12; modifier = 1.15f;
                    break;
                case 5:
                    item.itemName = "helmetReinforcedHelmet"; item.armorValue = 18; modifier = 1.2f;
                    break;
                case 6:
                    item.itemName = "helmetGalea"; item.armorValue = 30; modifier = 1.25f;
                    break;
                case 7:
                    item.itemName = "helmetVisorHelmet"; item.armorValue = 42; modifier = 1.3f;
                    break;
                case 8:
                    item.itemName = "helmetMailHood"; item.armorValue = 48; modifier = 1.35f;
                    break;
                case 9:
                    item.itemName = "helmetCapalin"; item.armorValue = 60; modifier = 1.4f;
                    break;
                case 10:
                    item.itemName = "helmetNasal"; item.armorValue = 72; modifier = 1.45f;
                    break;
                case 11:
                    item.itemName = "helmetHeavyHelmet"; item.armorValue = 84; modifier = 1.5f;
                    break;
                case 12:
                    item.itemName = "helmetDecoratedHelmet"; item.armorValue = 96; modifier = 1.55f;
                    break;
                case 13:
                    item.itemName = "helmetCassis"; item.armorValue = 108; modifier = 1.6f;
                    break;
                case 14:
                    item.itemName = "helmetLamellarHelmet"; item.armorValue = 120; modifier = 1.65f;
                    break;
            }
            if (item.levelReq > 14) item.armorValue = (int)Math.Floor(120 * (float)modifier * (((float)item.levelReq / 20f) * (float)rarityModifier));
            return;

        }
        void GlovesRoll(Item item, Color color)
        {
            float modifier = 1f, rarityModifier = 1f; ;
            int itemChoice = item.levelReq;
            item.itemImage = "gloves_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }

            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "glovesClothBinding"; item.armorValue = 2; modifier = 1f;
                    break;
                case 2:
                    item.itemName = "glovesClothGloves"; item.armorValue = 4; modifier = 1.05f;
                    break;
                case 3:
                    item.itemName = "glovesLeatherSleeve"; item.armorValue = 6; modifier = 1.1f;
                    break;
                case 4:
                    item.itemName = "glovesLeatherGloves"; item.armorValue = 8; modifier = 1.15f;
                    break;
                case 5:
                    item.itemName = "glovesHardLeatherMittens"; item.armorValue = 12; modifier = 1.2f;
                    break;
                case 6:
                    item.itemName = "glovesChainSleeve"; item.armorValue = 20; modifier = 1.25f;
                    break;
                case 7:
                    item.itemName = "glovesBronzeGloves"; item.armorValue = 28; modifier = 1.3f;
                    break;
                case 8:
                    item.itemName = "glovesLayeredGloves"; item.armorValue = 32; modifier = 1.35f;
                    break;
                case 9:
                    item.itemName = "glovesIronSleeve"; item.armorValue = 40; modifier = 1.4f;
                    break;
                case 10:
                    item.itemName = "glovesIronGauntlets"; item.armorValue = 48; modifier = 1.45f;
                    break;
                case 11:
                    item.itemName = "glovesReinforcedIronGauntlets"; item.armorValue = 56; modifier = 1.5f;
                    break;
                case 12:
                    item.itemName = "glovesPlateGloves"; item.armorValue = 64; modifier = 1.55f;
                    break;
                case 13:
                    item.itemName = "glovesScaleGloves"; item.armorValue = 72; modifier = 1.6f;
                    break;
                case 14:
                    item.itemName = "glovesLamellarGloves"; item.armorValue = 80; modifier = 1.65f;
                    break;
            }
            if (item.levelReq > 14) item.armorValue = (int)Math.Floor(80 * (float)modifier * (((float)item.levelReq / 20f) * (float)rarityModifier));
            return;

        }
        void BootsRoll(Item item, Color color)
        {
            float modifier = 1f, rarityModifier = 1f; ;
            int itemChoice = item.levelReq;
            item.itemImage = "boots_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }

            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "bootsFootBindings"; item.armorValue = 3; modifier = 1f;
                    break;
                case 2:
                    item.itemName = "bootsSandals"; item.armorValue = 6; modifier = 1.05f;
                    break;
                case 3:
                    item.itemName = "bootsWorkingShoes"; item.armorValue = 8; modifier = 1.1f;
                    break;
                case 4:
                    item.itemName = "bootsHideBoots"; item.armorValue = 10; modifier = 1.15f;
                    break;
                case 5:
                    item.itemName = "bootsHideGreaves"; item.armorValue = 15; modifier = 1.2f;
                    break;
                case 6:
                    item.itemName = "bootsReinforcedHideBoots"; item.armorValue = 25; modifier = 1.25f;
                    break;
                case 7:
                    item.itemName = "bootsChainGreaves"; item.armorValue = 35; modifier = 1.3f;
                    break;
                case 8:
                    item.itemName = "bootsBronzeBoots"; item.armorValue = 40; modifier = 1.35f;
                    break;
                case 9:
                    item.itemName = "bootsReinforcedGreaves"; item.armorValue = 50; modifier = 1.4f;
                    break;
                case 10:
                    item.itemName = "bootsCaligae"; item.armorValue = 60; modifier = 1.45f;
                    break;
                case 11:
                    item.itemName = "bootsIronBoots"; item.armorValue = 70; modifier = 1.5f;
                    break;
                case 12:
                    item.itemName = "bootsIronGreaves"; item.armorValue = 80; modifier = 1.55f;
                    break;
                case 13:
                    item.itemName = "bootsScaleGreaves"; item.armorValue = 90; modifier = 1.6f;
                    break;
                case 14:
                    item.itemName = "bootsLamellarGreaves"; item.armorValue = 100; modifier = 1.65f;
                    break;
            }
            if (item.levelReq > 14) item.armorValue = (int)Math.Floor(100 * (float)modifier * (((float)item.levelReq / 20f) * (float)rarityModifier));
            return;

        }
        void ShieldRoll(Item item, Color color)
        {
            float modifier = 1f, rarityModifier = 1f; ;
            int itemChoice = item.levelReq;
            item.itemImage = "shield_default";
            if (color == Color.White) { rarityModifier = 1f; }
            else if (color == Color.LightGreen) { rarityModifier = 1.1f; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.3f; }
            else if (color == Color.Orange) { rarityModifier = 1.4f; }
            else if (color == Color.Red) { rarityModifier = 1.5f; }

            if (itemChoice > 14 || itemChoice < 0) { itemChoice = random.Next(1, 15); }
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "shieldBoards"; item.armorValue = 9; modifier = 1f;
                    break;
                case 2:
                    item.itemName = "shieldWoodenShield"; item.armorValue = 18; modifier = 1.05f;
                    break;
                case 3:
                    item.itemName = "shieldHideShield"; item.armorValue = 24; modifier = 1.1f;
                    break;
                case 4:
                    item.itemName = "shieldReinforcedShield"; item.armorValue = 30; modifier = 1.15f;
                    break;
                case 5:
                    item.itemName = "shieldBronzeShield"; item.armorValue = 45; modifier = 1.2f;
                    break;
                case 6:
                    item.itemName = "shieldParryingShield"; item.armorValue = 75; modifier = 1.25f;
                    break;
                case 7:
                    item.itemName = "shieldIronShield"; item.armorValue = 105; modifier = 1.3f;
                    break;
                case 8:
                    item.itemName = "shieldSpikedShield"; item.armorValue = 120; modifier = 1.35f;
                    break;
                case 9:
                    item.itemName = "shieldUmbo"; item.armorValue = 150; modifier = 1.4f;
                    break;
                case 10:
                    item.itemName = "shieldAncile"; item.armorValue = 180; modifier = 1.45f;
                    break;
                case 11:
                    item.itemName = "shieldHoplon"; item.armorValue = 210; modifier = 1.5f;
                    break;
                case 12:
                    item.itemName = "shieldScutum"; item.armorValue = 240; modifier = 1.55f;
                    break;
                case 13:
                    item.itemName = "shieldTowerShield"; item.armorValue = 270; modifier = 1.6f;
                    break;
                case 14:
                    item.itemName = "shieldReinforcedTowerShield"; item.armorValue = 300; modifier = 1.65f;
                    break;
            }
            if (item.levelReq > 14) item.armorValue = (int)Math.Floor(300 * (float)modifier * (((float)item.levelReq / 20f) * (float)rarityModifier));
            return;

        }
        void NecklaceRoll(Item item)
        {
            item.itemImage = "necklace_default";
            int itemChoice = random.Next(1, 5);
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "necklaceGoldenAmulet";
                    break;
                case 2:
                    item.itemName = "necklaceAmethystCharm";
                    break;
                case 3:
                    item.itemName = "necklaceSapphireIdol";
                    break;
                case 4:
                    item.itemName = "necklaceRubyPendant";
                    break;
            }
            return;
        }
        void BeltRoll(Item item)
        {
            item.itemImage = "belt_default";
            int itemChoice = random.Next(1, 5);
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "beltLeatherSash";
                    break;
                case 2:
                    item.itemName = "beltUrartian";
                    break;
                case 3:
                    item.itemName = "beltIronBuckle";
                    break;
                case 4:
                    item.itemName = "beltBalteus";
                    break;
            }
            return;
        }
        void RingRoll(Item item)
        {
            item.itemImage = "ring_default";
            int itemChoice = random.Next(1, 5);
            switch (itemChoice)
            {
                case 1:
                    item.itemName = "ringGoldenSignet";
                    break;
                case 2:
                    item.itemName = "ringAmethystRing";
                    break;
                case 3:
                    item.itemName = "ringSapphireBand";
                    break;
                case 4:
                    item.itemName = "ringRubyToken";
                    break;
            }
            return;
        }



        ///<summary>
        ///Calculates 10% of max stamina for item level
        ///</summary>
        float LevelModifier(Item item)
        {

            return 0.1f * (8f + (float)item.levelReq * 2.75f);
        }

        ///<summary>+- 10% to stats</summary>
        float AffixValueRandomizer()
        {
            return (float)random.Next(90, 111) / 100f;
        }

        /// <summary>
        /// Combines 10% of max stamina for item level and +-10% modifier to a stat
        /// multiplier multiplies after estimate stat value (+-10% of value)
        /// [so multiplier=2 doubles to 20% of stamina] 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        float StandardStatValue(Item item, float multiplier = 1)
        {
            return LevelModifier(item) * AffixValueRandomizer() * multiplier;
        }
        //Generator affixów do itemów
        void RollAffix(Item item, Color color)
        {
            List<ItemBuff> list = new List<ItemBuff>();
            float rarityModifier = 1f;
            int affixTier = 0;
            if (color == Color.LightGreen) { rarityModifier = 1f; affixTier = 1; }
            else if (color == Color.DodgerBlue) { rarityModifier = 1.2f; affixTier = 2; }
            else if (color == Color.DarkViolet) { rarityModifier = 1.4f; affixTier = 3; }
            else if (color == Color.Orange) { rarityModifier = 2f; affixTier = 4; }
            else if (color == Color.Red) { rarityModifier = 4f; affixTier = 5; }



            //float basicStatBonus = 0.1f;//10% bonus do statystyk wzięty z maksymalnych możliwych statystyk na poziom

            ItemBuff buff = new ItemBuff();

            int buffs = 0, debuffs = 0;
            int x = random.Next(0, 101);
            switch (affixTier)
            {
                case 1:

                    if (x < 75) buffs = 1; else buffs = 2;
                    item.affixValue = 0; if (buffs == 2) item.affixValue = 15;
                    //buffs += (int)Math.Floor((float)item.levelReq / 30f);
                    debuffs = 0;
                    item.affixValue += greenAffixValue+item.levelReq*2;
                    break;
                case 2:
                    if (x < 70) buffs = 2; else buffs = 3;
                    item.affixValue = 0; if (buffs > 3) item.affixValue = 25;
                    //buffs += (int)Math.Ceiling((float)item.levelReq / 30f);
                    if (buffs < 3) debuffs = 0; else debuffs = 1;
                    item.affixValue += blueAffixValue+item.levelReq*3;
                    break;
                case 3:
                    if (x < 70) buffs = 3;
                    else if (x < 90) buffs = 4;
                    else buffs = 5;
                    item.affixValue = 0;
                    if (buffs > 3) item.affixValue = 30 * (buffs - 3);
                    //buffs += (int)Math.Ceiling((float)item.levelReq / 30f);
                    if (buffs < 5) debuffs = 1; else debuffs = 2;
                    item.affixValue += violetAffixValue+item.levelReq*4;
                    break;
                case 4:
                    if (x < 75) buffs = 4; else if (x < 95) buffs = 5; else buffs = 6;
                    item.affixValue = 0; if (buffs > 4) item.affixValue = 33 * (buffs - 4);
                    //buffs += (int)Math.Ceiling((float)item.levelReq / 30f);
                    if (buffs < 6) debuffs = 2; else debuffs = 3;
                    item.affixValue += orangeAffixValue+item.levelReq*5;
                    break;
                case 5:
                    if (x < 80) buffs = 6; else if (x < 90) buffs = 7; else if (x < 97) buffs = 8; else buffs = 9;
                    if (buffs > 6) item.affixValue = 40 * (buffs - 6);
                   // buffs += (int)Math.Ceiling((float)item.levelReq / 30f);
                    if (buffs < 8) debuffs = 2; else debuffs = 3;
                    item.affixValue += redAffixValue+item.levelReq*6;
                    break;
            }
            if(item.levelReq>=42) { buffs += (int)Math.Floor(((float)item.levelReq - 35f) / 7f); }
            while (buffs > 0)
            {
                AddRandomBonus(item, rarityModifier);
                buffs--;
            }
            while (debuffs > 0)
            {
                AddRandomBonus(item, -(rarityModifier / 2), false);
                debuffs--;
            }

            #region Old affix
            //switch (affixTier)
            //{
            //    #region Green buffs
            //    case 1:
            //        buff = new ItemBuff(); buff.buffName = "affixOfStrength"; buff.strengthBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfPerception"; buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfDexterity"; buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfAgility"; buff.agilityBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVitality"; buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfEndurance"; buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfCharisma"; buff.charismaBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfIntelligence"; buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item)); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfDamage"; buff.damageBonus = (int)Math.Ceiling(StandardStatValue(item) / 2); list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfArmor"; buff.armorBonus = (int)Math.Floor(150 * ((float)item.levelReq / 20f)); list.Add(buff);
            //        foreach (ItemBuff x in list)
            //        {
            //            x.value = greenAffixValue;
            //        }
            //        break;
            //    #endregion
            //    #region Blue buffs
            //    case 2:
            //        buff = new ItemBuff(); buff.buffName = "affixOfHoplite";
            //        buff.damageBonus = (int)Math.Ceiling(StandardStatValue(item, rarityModifier) / 2);
            //        buff.armorBonus = (int)Math.Floor(150 * ((float)item.levelReq / 20f));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfBrute";
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfArcher";
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfQuickness";
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMerchant";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfColossus";
            //        buff.strengthMultiplier = random.Next(10, 16);
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfEagleEye";
            //        buff.perceptionMultiplier = random.Next(10, 16);
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfAssasin";
            //        buff.dexterityMultiplier = random.Next(10, 16);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfRunaway";
            //        buff.agilityMultiplier = random.Next(10, 16);
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfToughness";
            //        buff.vitalityMultiplier = random.Next(10, 16);
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMarathon";
            //        buff.enduranceMultiplier = random.Next(10, 16);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfSilverTongue";
            //        buff.charismaMultiplier = random.Next(10, 16);
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfWisdom";
            //        buff.intelligenceMultiplier = random.Next(10, 16);
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        foreach (ItemBuff x in list)
            //        {
            //            x.value = blueAffixValue;
            //        }
            //        break;
            //    #endregion
            //    #region Violet buffs
            //    case 3:
            //        buff = new ItemBuff(); buff.buffName = "affixOfVersatile";
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfBerserk";
            //        buff.strengthMultiplier = random.Next(15, 21);
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.intelligenceMultiplier = -random.Next(15, 21);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfBastion";
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.healthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) * 15);
            //        buff.armorBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier) * 20);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfAccurate";
            //        buff.strengthBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionMultiplier = random.Next(15, 21);
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfCoward";
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityMultiplier = random.Next(15, 21);
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMighty";
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfDeadly";
            //        buff.dexterityMultiplier = random.Next(15, 21);
            //        buff.perceptionMultiplier = random.Next(15, 21);
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthBonus = -(int)Math.Floor(StandardStatValue(item));
            //        buff.enduranceBonus = -(int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfCunning";
            //        buff.charismaMultiplier = random.Next(15, 21);
            //        buff.intelligenceMultiplier = random.Next(15, 21);
            //        buff.perceptionBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        buff.enduranceBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfCurse";
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item));
            //        buff.strengthBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.perceptionBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.agilityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.enduranceBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfRisk";
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, 2));
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, 2));
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, 2));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.enduranceBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.agilityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        foreach (ItemBuff x in list)
            //        {
            //            x.value = violetAffixValue;
            //        }
            //        break;
            //    #endregion
            //    #region Yellow buffs
            //    case 4:
            //        buff = new ItemBuff(); buff.buffName = "affixOfJupiter";
            //        buff.damageBonus = item.levelReq;
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthMultiplier = random.Next(20, 26);
            //        buff.armorBonus = (int)Math.Floor(StandardStatValue(item, 20));
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfJuno";
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceMultiplier = random.Next(20, 26);
            //        buff.strengthBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfNeptune";
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item));
            //        buff.agilityBonus = -(int)Math.Floor(StandardStatValue(item));
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityMultiplier = random.Next(20, 26);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMinerva";
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityMultiplier = random.Next(20, 26);
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaBonus -= (int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMars";
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityMultiplier = random.Next(20, 26);
            //        buff.perceptionBonus -= (int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVenus";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        buff.enduranceMultiplier = random.Next(20, 26);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfApollo";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaMultiplier = random.Next(20, 26);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfDiana";
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionMultiplier = random.Next(20, 26);
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVulcan";
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item));
            //        buff.armorBonus = (int)Math.Floor(StandardStatValue(item, 25));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthMultiplier = random.Next(20, 26);
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfMercury";
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityMultiplier = random.Next(20, 26);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfCeres";
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityMultiplier = random.Next(20, 26);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVesta";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaMultiplier = random.Next(20, 26);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionBonus = -(int)Math.Floor(StandardStatValue(item) / 2);
            //        list.Add(buff);
            //        foreach (ItemBuff x in list)
            //        {
            //            x.value = orangeAffixValue;
            //        }
            //        break;
            //    #endregion
            //    #region Red buffs
            //    case 5:
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedJupiter";
            //        buff.damageBonus = item.levelReq;
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthMultiplier = random.Next(25, 31);
            //        buff.armorBonus = (int)Math.Floor(StandardStatValue(item, 20));
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedJuno";
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceMultiplier = random.Next(25, 31);
            //        buff.strengthBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedNeptune";
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item));
            //        buff.agilityBonus = -(int)Math.Floor(StandardStatValue(item));
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityMultiplier = random.Next(25, 31);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedMinerva";
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.dexterityMultiplier = random.Next(25, 31);
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaBonus -= (int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedMars";
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityMultiplier = random.Next(25, 31);
            //        buff.perceptionBonus -= (int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedVenus";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item));
            //        buff.enduranceMultiplier = random.Next(25, 31);
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedApollo";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaMultiplier = random.Next(25, 31);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedDiana";
            //        buff.perceptionBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionMultiplier = random.Next(25, 31);
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedVulcan";
            //        buff.damageBonus = (int)Math.Floor(StandardStatValue(item));
            //        buff.armorBonus = (int)Math.Floor(StandardStatValue(item, 25));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthMultiplier = random.Next(25, 31);
            //        buff.charismaBonus = -(int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedMercury";
            //        buff.agilityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.agilityMultiplier = random.Next(25, 31);
            //        buff.dexterityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityBonus = -(int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedCeres";
            //        buff.vitalityBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.vitalityMultiplier = random.Next(25, 31);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.intelligenceBonus = -(int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        list.Add(buff);
            //        buff = new ItemBuff(); buff.buffName = "affixOfVeneratedVesta";
            //        buff.charismaBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.charismaMultiplier = random.Next(25, 31);
            //        buff.enduranceBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.strengthBonus = (int)Math.Floor(StandardStatValue(item, rarityModifier));
            //        buff.perceptionBonus = -(int)Math.Floor(StandardStatValue(item));
            //        list.Add(buff);

            //        foreach (ItemBuff x in list)
            //        {
            //            x.value = redAffixValue;
            //        }
            //        break;

            //        #endregion
            //}
            #endregion






            //ItemBuff rolledBuff = list[random.Next(0, list.Count)];
            //ItemBuff rolledBuff = buff;
            ////item.itemName = item.itemName + " " + rolledBuff.buffName;
            //item.affixName = rolledBuff.buffName;
            //item.strengthBonus = rolledBuff.strengthBonus; item.strengthMultiplier = rolledBuff.strengthMultiplier;
            //item.perceptionBonus = rolledBuff.perceptionBonus; item.perceptionMultiplier = rolledBuff.perceptionMultiplier;
            //item.dexterityBonus = rolledBuff.dexterityBonus; item.dexterityMultiplier = rolledBuff.dexterityMultiplier;
            //item.agilityBonus = rolledBuff.agilityBonus; item.agilityMultiplier = rolledBuff.agilityMultiplier;
            //item.vitalityBonus = rolledBuff.vitalityBonus; item.vitalityMultiplier = rolledBuff.vitalityMultiplier;
            //item.enduranceBonus = rolledBuff.enduranceBonus; item.enduranceMultiplier = rolledBuff.enduranceMultiplier;
            //item.charismaBonus = rolledBuff.charismaBonus; item.charismaMultiplier = rolledBuff.charismaMultiplier;
            //item.intelligenceBonus = rolledBuff.intelligenceBonus; item.intelligenceMultiplier = rolledBuff.intelligenceMultiplier;
            //item.damageBonus = rolledBuff.damageBonus;
            //item.healthBonus = rolledBuff.healthBonus;
            //item.armorBonus = rolledBuff.armorBonus;
            //item.affixValue = rolledBuff.value;
            return;
        }
        void AddRandomBonus(Item item, float modifier = 1, bool buff = true)
        {
            int roll = -1;
            if (buff) roll = random.Next(0, 27);
            else roll = random.Next(0, 28);
            switch (roll)
            {
                case 0:
                    item.damageBonus += (int)Math.Floor(StandardStatValue(item, modifier * 0.5f));
                    break;
                case 1:
                    item.strengthBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 2:
                    item.strengthBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 3:
                    item.perceptionBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 4:
                    item.perceptionBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 5:
                    item.dexterityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 6:
                    item.dexterityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 7:
                    item.agilityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 8:
                    item.agilityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 9:
                    item.vitalityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 10:
                    item.vitalityBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 11:
                    item.enduranceBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 12:
                    item.enduranceBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 13:
                    item.charismaBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 14:
                    item.charismaBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 15:
                    item.intelligenceBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 16:
                    item.intelligenceBonus += (int)Math.Floor(StandardStatValue(item, modifier));
                    break;
                case 17:
                    item.healthBonus += (int)Math.Floor(StandardStatValue(item, modifier * 10));
                    break;
                case 18:
                    item.armorBonus += (int)Math.Floor(StandardStatValue(item, modifier * 8));
                    break;
                case 19:
                    item.strengthMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 20:
                    item.perceptionMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 21:
                    item.dexterityMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 22:
                    item.agilityMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 23:
                    item.vitalityMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 24:
                    item.enduranceMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 25:
                    item.charismaMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 26:
                    item.intelligenceMultiplier += (int)Math.Floor((float)random.Next(5, 11) * modifier);
                    break;
                case 27:
                    break;
                default:

                    break;
            }
        }
    }
}