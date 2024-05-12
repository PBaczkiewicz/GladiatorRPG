using GladiatorRPG;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GladiatorRPG
{

    //Dodawanie złota, zakładanie/zdejmowanie przedmiotów
    public partial class MainTabPage
    {
        //Dodawanie złota do gracza
        void AddGold(int amount)
        {
            player.Gold += amount;
        }
        //Utworzenie listy do UI ekwipunku gracza
        public void GridSquares()
        {
            squareList = new List<Button>
            {
                eq00, eq10, eq20, eq30,
                eq01, eq11, eq21, eq31,
                eq02, eq12, eq22, eq32
            };
            imageList = new List<Image>
            {
                eq00Image, eq10Image, eq20Image, eq30Image,
                eq01Image, eq11Image, eq21Image, eq31Image,
                eq02Image, eq12Image, eq22Image, eq32Image

            };
            borderList = new List<Image>
            {
                eq00Border, eq10Border, eq20Border, eq30Border,
                eq01Border, eq11Border, eq21Border, eq31Border,
                eq02Border, eq12Border, eq22Border, eq32Border

            };

        }
        //Pokazanie ekwipunku gracza na UI
        public void GridEquipment()
        {
            GridSquares();
            int itemsInInventory = player.items.Count;
            for (int i = 0; i < squareList.Count; i++)
            {
                squareList[i].Text = "";
                imageList[i].Source = "emptySlot";
                borderList[i].Source = "borderWhite";
            }
            for (int i = 0; i < itemsInInventory; i++)
            {

                if (player.items[i].itemImage != null) { imageList[i].Source = player.items[i].itemImage; }
                else { imageList[i].Source = "itemTemplate"; }

                squareList[i].Text = "\n\n\n\n\n\n\n\n" + localizedStrings[player.items[i].itemName];
                squareList[i].TextColor = player.items[i].color;
                if (player.items[i].color.ToArgb() == System.Drawing.Color.White.ToArgb()) { borderList[i].Source = "borderWhite"; }
                else if (player.items[i].color.ToArgb() == System.Drawing.Color.LightGreen.ToArgb()) { borderList[i].Source = "borderGreen"; }
                else if (player.items[i].color.ToArgb() == System.Drawing.Color.DodgerBlue.ToArgb()) { borderList[i].Source = "borderBlue"; }
                else if (player.items[i].color.ToArgb() == System.Drawing.Color.DarkViolet.ToArgb()) { borderList[i].Source = "borderViolet"; }
                else if (player.items[i].color.ToArgb() == System.Drawing.Color.Orange.ToArgb()) { borderList[i].Source = "borderOrange"; }
                else if (player.items[i].color.ToArgb() == System.Drawing.Color.Red.ToArgb()) { borderList[i].Source = "borderRed"; }
                else { borderList[i].Source = null; }

                //player.name = System.Drawing.Color.DarkViolet.ToArgb().ToString();
                //player.name = player.name + "\n" +player.items[i].color.ToArgb().ToString();



            }
            UpdateImages();
        }
        void ItemBorder(Item item, Image image)
        {
            if (item.color.ToArgb() == System.Drawing.Color.White.ToArgb()) { image.Source = "borderWhite"; }
            else if (item.color.ToArgb() == System.Drawing.Color.LightGreen.ToArgb()) { image.Source = "borderGreen"; }
            else if (item.color.ToArgb() == System.Drawing.Color.DodgerBlue.ToArgb()) { image.Source = "borderBlue"; }
            else if (item.color.ToArgb() == System.Drawing.Color.DarkViolet.ToArgb()) { image.Source = "borderViolet"; }
            else if (item.color.ToArgb() == System.Drawing.Color.Orange.ToArgb()) { image.Source = "borderOrange"; }
            else if (item.color.ToArgb() == System.Drawing.Color.Red.ToArgb()) { image.Source = "borderRed"; }
            else { image.Source = "borderWhite"; }
        }
        void UpdateImages()
        {
            if (player.head != null) { if (player.head.itemImage != null) { helmetImage.Source = player.head.itemImage; ItemBorder(player.head, helmetBorder); } } else { helmetImage.Source = null; helmetBorder.Source = "borderWhite"; }
            if (player.torso != null)
            {
                if (player.torso.itemImage != null)
                {
                    torsoImage.Source = player.torso.itemImage;
                    ItemBorder(player.torso, torsoBorder);

                }
            }
            else
            {
                torsoImage.Source = null;
                torsoBorder.Source = "borderWhite";
            }
            if (player.boots != null) { if (player.boots.itemImage != null) { bootsImage.Source = player.boots.itemImage; ItemBorder(player.boots, bootsBorder); } } else { bootsImage.Source = null; bootsBorder.Source = "borderWhite"; }
            if (player.gloves != null) { if (player.gloves.itemImage != null) { glovesImage.Source = player.gloves.itemImage; ItemBorder(player.gloves, glovesBorder); } } else { glovesImage.Source = null; glovesBorder.Source = "borderWhite"; }
            if (player.weapon != null) { if (player.weapon.itemImage != null) { weaponImage.Source = player.weapon.itemImage; ItemBorder(player.weapon, weaponBorder); } } else { weaponImage.Source = null; weaponBorder.Source = "borderWhite"; }
            if (player.shield != null) { if (player.shield.itemImage != null) { shieldImage.Source = player.shield.itemImage; ItemBorder(player.shield, shieldBorder); } } else { shieldImage.Source = null; shieldBorder.Source = "borderWhite"; }
            if (player.necklace != null) { if (player.necklace.itemImage != null) { necklaceImage.Source = player.necklace.itemImage; ItemBorder(player.necklace, necklaceBorder); } } else { necklaceImage.Source = null; necklaceBorder.Source = "borderWhite"; }
            if (player.belt != null) { if (player.belt.itemImage != null) { beltImage.Source = player.belt.itemImage; ItemBorder(player.belt, beltBorder); } } else { beltImage.Source = null; beltBorder.Source = "borderWhite"; }
            if (player.ring != null) { if (player.ring.itemImage != null) { ringImage.Source = player.ring.itemImage; ItemBorder(player.ring, ringBorder); } } else { ringImage.Source = null; ringBorder.Source = "borderWhite"; }



        }

        //Zerowanie bonusowych statystyk (bez tego statystyki gracza będą ciągle rosnąć po zmianie ekwipunku
        public async void eq_Clicked(object sender, EventArgs e)
        {

            if (activeDialogBox == false)
            {
                string eqItemText = "\n_______________\n" + localizedStrings["yourItemText"] + "\n";
                Item equippedItem = null;
                activeDialogBox = true;
                int itemNumber=-1;
                if (sender is Button button)
                {
                    itemNumber = int.Parse(button.AutomationId);
                    if(itemNumber>=player.items.Count)
                    {
                        activeDialogBox = false;
                        return;
                    }
                }
                else if (itemNumber > player.items.Count || itemNumber==-1) //NIE WIEM DLACZEGO ALE TO NAPRAWIA CRASH
                {
                    activeDialogBox = false;
                    return;
                }
                else return;
                if (player.items[itemNumber].itemClass == ItemClass.Weapon && player.weapon != null) { equippedItem = player.weapon; }
                if (player.items[itemNumber].itemClass == ItemClass.Helmet && player.head != null) { equippedItem = player.head; }
                if (player.items[itemNumber].itemClass == ItemClass.Torso && player.torso != null) { equippedItem = player.torso; }
                if (player.items[itemNumber].itemClass == ItemClass.Shield && player.shield != null) { equippedItem = player.shield; }
                if (player.items[itemNumber].itemClass == ItemClass.Boots && player.boots != null) { equippedItem = player.boots; }
                if (player.items[itemNumber].itemClass == ItemClass.Gloves && player.gloves != null) { equippedItem = player.gloves; }
                if (player.items[itemNumber].itemClass == ItemClass.Necklace && player.necklace != null) { equippedItem = player.necklace; }
                if (player.items[itemNumber].itemClass == ItemClass.Belt && player.belt != null) { equippedItem = player.belt; }
                if (player.items[itemNumber].itemClass == ItemClass.Ring && player.ring != null) { equippedItem = player.ring; }
                if (equippedItem != null) { equippedItem.UpdateDescription(localizedStrings); eqItemText += equippedItem.itemDescription; } else eqItemText = "";
                if (itemNumber + 1 > player.items.Count) { activeDialogBox = false; return; }
                
                else
                {
                    //player.items[itemNumber].UpdateDescription(localizedStrings);//tests
                    var action = await Application.Current.MainPage.DisplayActionSheet(
                        actionItemText,
                        actionEquipText,
                        actionSellText,
                        null,
                        player.items[itemNumber].itemDescription + eqItemText
                    );
                    if (action == actionEquipText)
                    {
                        if (clicked) return;
                        clicked = true;
                        if (player.items[itemNumber].levelReq>player.level)
                        {
                            TEST(localizedStrings["levelTooLow"]);
                            clicked = false;
                            activeDialogBox = false;
                            return;
                        }
                        switch (player.items[itemNumber].itemClass)
                        {
                            case ItemClass.Weapon:
                                if (player.weapon == null)
                                {
                                    player.weapon = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.weapon);
                                    player.weapon = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Helmet:
                                if (player.head == null)
                                {
                                    player.head = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.head);
                                    player.head = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);

                                }
                                break;
                            case ItemClass.Torso:
                                if (player.torso == null)
                                {
                                    player.torso = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.torso);
                                    player.torso = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Boots:
                                if (player.boots == null)
                                {
                                    player.boots = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.boots);
                                    player.boots = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Gloves:
                                if (player.gloves == null)
                                {
                                    player.gloves = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.gloves);
                                    player.gloves = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Shield:
                                if (player.shield == null)
                                {
                                    player.shield = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.shield);
                                    player.shield = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Ring:
                                if (player.ring == null)
                                {
                                    player.ring = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.ring);
                                    player.ring = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Belt:
                                if (player.belt == null)
                                {
                                    player.belt = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.belt);
                                    player.belt = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                            case ItemClass.Necklace:
                                if (player.necklace == null)
                                {
                                    player.necklace = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                else
                                {
                                    player.items.Add(player.necklace);
                                    player.necklace = player.items[itemNumber];
                                    player.items.RemoveAt(itemNumber);
                                }
                                break;
                        }
                    }
                    else if (action == actionSellText)
                    {
                        if (clicked) return;
                        clicked = true;
                        AddGold(player.items[itemNumber].sellValue);
                        player.items.RemoveAt(itemNumber);
                    }
                }
                GridEquipment();
                ProcessEquippedItems();
                //UpdateImages();
                clicked = false;
                activeDialogBox = false;
                if(player.tutorial==2)
                {
                    player.Tutorial++;
                }
            }

        }
        //Kliknięcie przedmioty wyekwipowanego
        bool clicked = false;
        public async void equipped_Clicked(object sender, EventArgs e)
        {
            if (!activeDialogBox)
            {
                activeDialogBox = true;
                Item item = null;
                string itemClicked;
                if (sender is Button button)
                {
                    itemClicked = button.AutomationId.ToString();



                    switch (itemClicked)
                    {
                        case "equippedHelmet": if (player.head == null) { activeDialogBox = false; return; } else { item = player.head; } break;
                        case "equippedTorso": if (player.torso == null) { activeDialogBox = false; return; } else { item = player.torso; } break;
                        case "equippedWeapon": if (player.weapon == null) { activeDialogBox = false; return; } else { item = player.weapon; } break;
                        case "equippedGloves": if (player.gloves == null) { activeDialogBox = false; return; } else { item = player.gloves; } break;
                        case "equippedBoots": if (player.boots == null) { activeDialogBox = false; return; } else { item = player.boots; } break;
                        case "equippedShield": if (player.shield == null) { activeDialogBox = false; return; } else { item = player.shield; } break;
                        case "equippedRing": if (player.ring == null) { activeDialogBox = false; return; } else { item = player.ring; } break;
                        case "equippedBelt": if (player.belt == null) { activeDialogBox = false; return; } else { item = player.belt; } break;
                        case "equippedNecklace": if (player.necklace == null) { activeDialogBox = false; return; } else { item = player.necklace; } break;

                    }
                    item.UpdateDescription(localizedStrings);
                    var action = await Application.Current.MainPage.DisplayActionSheet(actionItemText, actionUnequipText, localizedStrings["cancelText"] /*było actionSellText*/,  null, item.itemDescription);
                    if (action == actionUnequipText)
                    {
                        if (clicked) return;
                        clicked = true;
                        if (player.items.Count >= 12)
                        {
                            await DisplayAlert(cautionText, noSpaceInInventoryText, okText);
                            activeDialogBox = false;
                            return;
                        }
                        else
                        {
                            player.items.Add(item);
                            if (itemClicked == "equippedHelmet") { player.head = null; helmetImage.Source = null; helmetBorder.Source = null; }
                            else if (itemClicked == "equippedTorso") { player.torso = null; torsoImage.Source = null; torsoBorder.Source = null; }
                            else if (itemClicked == "equippedWeapon") { player.weapon = null; weaponImage.Source = null; weaponBorder.Source = null; }
                            else if (itemClicked == "equippedGloves") { player.gloves = null; glovesImage.Source = null; glovesBorder.Source = null; }
                            else if (itemClicked == "equippedBoots") { player.boots = null; bootsImage.Source = null; bootsBorder.Source = null; }
                            else if (itemClicked == "equippedShield") { player.shield = null; shieldImage.Source = null; shieldBorder.Source = null; }
                            else if (itemClicked == "equippedRing") { player.ring = null; ringImage.Source = null; ringBorder.Source = null; }
                            else if (itemClicked == "equippedBelt") { player.belt = null; beltImage.Source = null; beltBorder.Source = null; }
                            else if (itemClicked == "equippedNecklace") { player.necklace = null; necklaceImage.Source = null; necklaceBorder.Source = null; }
                        }
                    }
                    //sprzedawanie założonego itemka
                    //else if (action == actionSellText)
                    //{
                    //    if (clicked) return;
                    //    clicked = true;
                    //    AddGold(item.sellValue);
                    //    if (itemClicked == "equippedHelmet") { player.head = null; helmetImage.Source = null; helmetBorder.Source = null; }
                    //    else if (itemClicked == "equippedTorso") { player.torso = null; torsoImage.Source = null; torsoBorder.Source = null; }
                    //    else if (itemClicked == "equippedWeapon") { player.weapon = null; weaponImage.Source = null; weaponBorder.Source = null; }
                    //    else if (itemClicked == "equippedGloves") { player.gloves = null; glovesImage.Source = null; glovesBorder.Source = null; }
                    //    else if (itemClicked == "equippedBoots") { player.boots = null; bootsImage.Source = null; bootsBorder.Source = null; }
                    //    else if (itemClicked == "equippedShield") { player.shield = null; shieldImage.Source = null; shieldBorder.Source = null; }
                    //    else if (itemClicked == "equippedRing") { player.ring = null; ringImage.Source = null; ringBorder.Source = null; }
                    //    else if (itemClicked == "equippedBelt") { player.belt = null; beltImage.Source = null; beltBorder.Source = null; }
                    //    else if (itemClicked == "equippedNecklace") { player.necklace = null; necklaceImage.Source = null; necklaceBorder.Source = null; }
                    //}

                    GridEquipment();
                    ProcessEquippedItems();
                    //UpdateImages();
                    activeDialogBox = false;
                    clicked = false;
                }
                else { activeDialogBox = false; return; }

            }
        }

    }
}
