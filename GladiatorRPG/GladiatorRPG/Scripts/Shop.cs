using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using static Android.Resource;
using static System.Net.Mime.MediaTypeNames;
using Image = Xamarin.Forms.Image;

namespace GladiatorRPG
{
    public partial class MainTabPage
    {
        List<Button> shopList;
        List<Image> shopImages;
        List<Image> shopBorders;
        List<Label> shopLabels;


        void LoadShop(bool forceUpdate = false)
        {
            shopButton.Text = localizedStrings["shopButtonInfo"];

            shopList = new List<Button>
            {
                shopButton1, shopButton2, shopButton3, shopButton4, shopButton5, shopButton6,
                shopButton7, shopButton8, shopButton9, shopButton10, shopButton11, shopButton12
            };
            shopImages = new List<Image>
            {
                shopImage1, shopImage2, shopImage3, shopImage4, shopImage5, shopImage6,
                shopImage7, shopImage8, shopImage9, shopImage10, shopImage11, shopImage12
            };
            shopBorders = new List<Image>
            {
                shopBorder1, shopBorder2, shopBorder3, shopBorder4, shopBorder5, shopBorder6,
                shopBorder7, shopBorder8, shopBorder9, shopBorder10, shopBorder11, shopBorder12
            };
            shopLabels = new List<Label>
            {
                shopLabel1, shopLabel2, shopLabel3, shopLabel4, shopLabel5, shopLabel6,
                shopLabel7, shopLabel8, shopLabel9, shopLabel10, shopLabel11, shopLabel12
            };
            UpdateShop();
            //TimeSpan nextRefresh = player.shopDate - DateTime.Now;
            //Prawdopodobnie do zmodyfikowania żeby sklep co godzine się resetował
            if (player.shopDate == null || player.shopDate < DateTime.Now ||forceUpdate)//player.shopDate.DayOfYear!=DateTime.Now.DayOfYear||player.shopDate.Year!=DateTime.Now.Year)
            {
                player.shopDate = DateTime.Now.AddHours(1);
                if(player.shopNotification) SendNotification(localizedStrings["notificationShopResetFinished"], localizedStrings["notificationShopResetFinished1"], 5, player.shopDate);
                RollShopItems();

            }

            if(player.tutorial==1)
            {
                ShopTutorial();
            }


        }
        void ShopTutorial()
        {
            if (player.shopInventory != null) player.shopInventory.Clear();
            else player.shopInventory = new List<Item>();
            for (int i = 0; i < 12; i++)
            {
                player.shopInventory.Add(ItemGenerator(1,1,0,ItemClass.Weapon));
                player.shopInventory[i].buyValue = 0;
                player.shopInventory[i].sellValue = 0;
            }
            UpdateShop();
            RollArenaReward();
        }
        void RollShopItems()
        {
            if (player.shopInventory != null) player.shopInventory.Clear();
            else player.shopInventory = new List<Item>();
            for (int i = 0; i < 12; i++)
            {
                int iLvl=player.level;
                if (i < 2) iLvl = player.level + 1;

                player.shopInventory.Add(ItemGenerator(iLvl,iLvl));
                player.shopInventory[i].buyValue = (int)Math.Ceiling(player.shopInventory[i].sellValue * buyValueMultiplier);
            }
            player.shopInventory.Sort((x, y) => y.buyValue.CompareTo(x.buyValue));
            UpdateShop();
            RollArenaReward();
        }
        void UpdateShop()
        {
            for (int i = 0; i < shopList.Count; i++)
            {
                shopList[i].Text = "";
                shopImages[i].Source = "emptySlot";
                shopBorders[i].Source = "borderWhite";
                shopLabels[i].Text = "";
            }
            for (int i = 0; i < player.shopInventory.Count; i++)
            {
                if (player.shopInventory[i].itemImage != null) { shopImages[i].Source = player.shopInventory[i].itemImage; }
                else { shopImages[i].Source = ""; }

                shopList[i].Text = "\n\n\n\n\n\n\n" + localizedStrings[player.shopInventory[i].itemName];
                shopList[i].TextColor = player.shopInventory[i].color;
                if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.White.ToArgb()) { shopBorders[i].Source = "borderWhite"; }
                else if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.LightGreen.ToArgb()) { shopBorders[i].Source = "borderGreen"; }
                else if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.DodgerBlue.ToArgb()) { shopBorders[i].Source = "borderBlue"; }
                else if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.DarkViolet.ToArgb()) { shopBorders[i].Source = "borderViolet"; }
                else if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.Orange.ToArgb()) { shopBorders[i].Source = "borderOrange"; }
                else if (player.shopInventory[i].color.ToArgb() == System.Drawing.Color.Red.ToArgb()) { shopBorders[i].Source = "borderRed"; }
                else { shopBorders[i].Source = null; }
                //player.shopInventory[i].UpdateDescription(localizedStrings);//tests
                shopLabels[i].Text = SetDescription(player.shopInventory[i]);
                int newlines = shopLabels[i].Text.Count(c => c == '\n');
                shopLabels[i].FontSize = 10;
                while (newlines >= 8 && shopLabels[i].FontSize!=3)
                {
                    shopLabels[i].FontSize -= 1;
                    newlines--;
                }


            }
        }
        string SetDescription(Item i)
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

            description += "\n" + localizedStrings["itemBuyValue"] + " : " + i.buyValue;


            return description;
        }

        //void ShopBuying_Clicked(object sender, EventArgs e)
        //{
        //    if (sender is Button button)
        //    { 
        //        TEST(button.AutomationId);

        //    }
        //}
        float buyValueMultiplier=3;
        public async void ShopBuying_Clicked(object sender, EventArgs e)
        {
            if (activeDialogBox == false)
            {
                Item equippedItem = null;
                activeDialogBox = true;
                int itemNumber;
                if (sender is Button button) { itemNumber = int.Parse(button.AutomationId); }
                else return;
                if (itemNumber + 1 > player.shopInventory.Count) { activeDialogBox = false; return; }
                else
                {
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Weapon && player.weapon != null) { equippedItem = player.weapon; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Helmet && player.head != null) { equippedItem = player.head; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Torso && player.torso != null) { equippedItem = player.torso; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Shield && player.shield != null) { equippedItem = player.shield; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Boots && player.boots != null) { equippedItem = player.boots; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Gloves && player.gloves != null) { equippedItem = player.gloves; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Necklace && player.necklace != null) { equippedItem = player.necklace; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Belt && player.belt != null) { equippedItem = player.belt; }
                    if (player.shopInventory[itemNumber].itemClass == ItemClass.Ring && player.ring != null) { equippedItem = player.ring; }
                    if (equippedItem != null)
                    {
                        player.shopInventory[itemNumber].UpdateDescription(localizedStrings);
                        player.shopInventory[itemNumber].buyValue = (int)Math.Ceiling((float)player.shopInventory[itemNumber].sellValue * buyValueMultiplier);
                        var action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet(
                            localizedStrings["actionItemText"],
                            localizedStrings["actionBuyText"],
                            localizedStrings["cancelText"],
                            null,
                            player.shopInventory[itemNumber].itemDescription + "\n" + localizedStrings["costText"] + player.shopInventory[itemNumber].buyValue +" "+ localizedStrings["itemGoldText"]+ "\n_______________\n" + localizedStrings["yourItemText"] + "\n" + equippedItem.itemDescription
                        );
                        if (action == localizedStrings["actionBuyText"])
                        {
                            if (clicked) return;
                            if (player.items.Count > 11) { await DisplayAlert(cautionText, localizedStrings["noSpaceInInventoryText"], okText); activeDialogBox = false; clicked = false; return; }
                            if (player.Gold < player.shopInventory[itemNumber].buyValue) { await DisplayAlert(cautionText, localizedStrings["insufficientGoldMessage"], okText); activeDialogBox = false; clicked = false; return; }
                            player.Gold -= player.shopInventory[itemNumber].buyValue;
                            player.items.Add(player.shopInventory[itemNumber]);
                            player.shopInventory.RemoveAt(itemNumber);
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
                        player.shopInventory[itemNumber].itemDescription + "\n" + localizedStrings["costText"] + player.shopInventory[itemNumber].buyValue +" "+ localizedStrings["itemGoldText"]
                        );
                        if (action == localizedStrings["actionBuyText"])
                        {
                            if (clicked) return;
                            if (player.items.Count > 11) { await DisplayAlert(cautionText, localizedStrings["noSpaceInInventoryText"], okText); activeDialogBox = false; clicked = false; return; }
                            if (player.Gold < player.shopInventory[itemNumber].buyValue) { await DisplayAlert(cautionText, localizedStrings["insufficientGoldMessage"], okText); activeDialogBox = false; clicked = false; return; }
                            player.Gold -= player.shopInventory[itemNumber].buyValue;
                            player.items.Add(player.shopInventory[itemNumber]);
                            player.shopInventory.RemoveAt(itemNumber);
                            clicked = true;
                            if (player.tutorial == 1)
                            {
                                player.Tutorial++;
                                player.shopDate = DateTime.Now.AddMinutes(5);
                            }
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


        private void shopButton_Clicked(object sender, EventArgs e)
        {
            shopButtonInfo();
        }

        async void shopButtonInfo()
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["infoText"], localizedStrings["informationsShop"], localizedStrings["okText"]);
                clicked = false;
            }
        }
    }
}
