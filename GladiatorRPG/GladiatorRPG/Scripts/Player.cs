using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Xamarin.Forms;

namespace GladiatorRPG
{
    //!!!
    //WSZYSTKIE ZMIENNE TU UTWORZONE MUSZĄ MIEĆ WARTOŚĆ { get; set; } INACZEJ NIE BĘDZIE W STANIE WCZYTAĆ ZAPISU
    //!!!

    [Serializable]
    public class Entity
    {
        public string name { get; set; }
        public string source { get; set; }
        public int level { get; set; }

        public int baseHealth { get; set; }
        public int bonusHealth { get; set; }
        public int maxHealth { get; set; }
        public int currentHealth { get; set; }

        public int minDamage { get; set; }

        public int maxDamage { get; set; }
        public int critDamage { get; set; }//procentowe zwiększenie obrażeń dla krytyków


        public int bonusDamage { get; set; }
        public int baseArmor { get; set; }
        public int armor { get; set; }
        public int bonusArmor { get; set; }

        public int baseStrength { get; set; }
        public int strength { get; set; } //obrażenia i dodatkowe obrażenia krytyczne(+1%/10)?
        public int bonusStrength { get; set; }
        public int multipliedStrength { get; set; }

        public int basePerception { get; set; }
        public int perception { get; set; } //szansa na trafienie
        public int bonusPerception { get; set; }
        public int multipliedPerception { get; set; }

        public int baseDexterity { get; set; }
        public int dexterity { get; set; } //szansa na kryta
        public int bonusDexterity { get; set; }
        public int multipliedDexterity { get; set; }

        public int baseAgility { get; set; }
        public int agility { get; set; } //zmniejszenie szansy na trafienie przeciwnika
        public int bonusAgility { get; set; }
        public int multipliedAgility { get; set; }

        public int baseVitality { get; set; }
        public int vitality { get; set; } //zdrowie
        public int bonusVitality { get; set; }
        public int multipliedVitality { get; set; }

        public int baseEndurance { get; set; }
        public int endurance { get; set; } //odporność na ciosy krytyczne
        public int bonusEndurance { get; set; }
        public int multipliedEndurance { get; set; }

        public int baseCharisma { get; set; }
        public int charisma { get; set; } //podwójny atak
        public int bonusCharisma { get; set; }
        public int multipliedCharisma { get; set; }

        public int baseIntelligence { get; set; }
        public int intelligence { get; set; } //odporność na podwójny atak
        public int bonusIntelligence { get; set; }
        public int multipliedIntelligence { get; set; }

        public Item head { get; set; }
        public Item torso { get; set; }
        public Item gloves { get; set; }
        public Item boots { get; set; }
        public Item weapon { get; set; }
        public Item shield { get; set; }
        public Item ring { get; set; }
        public Item belt { get; set; }
        public Item necklace { get; set; }
        public List<Item> items { get; set; } = new List<Item>();
        public Entity()
        {


        }
        public Entity(string _name, int _level, int _strength, int _perception, int _dexterity, int _agility, int _vitality, int _endurance, int _charisma, int _intelligence)
        {
            name = _name;
            level = _level;
            baseStrength = _strength;
            basePerception = _perception;
            baseDexterity = _dexterity;
            baseAgility = _agility;
            baseVitality = _vitality;
            baseEndurance = _endurance;
            baseCharisma = _charisma;
            baseIntelligence = _intelligence;

            #region entity
            level = 1;
            critDamage = 50;
            #endregion


            strength = baseStrength;
            perception = basePerception;
            dexterity = baseDexterity;
            agility = baseAgility;
            vitality = baseVitality;
            endurance = baseEndurance;
            charisma = baseCharisma;
            intelligence = baseIntelligence;
            maxHealth = 10 + 15 * vitality;
            currentHealth = maxHealth;
            minDamage = 1;
            maxDamage = 1;
        }

    }


    [Serializable]
    public class Player : Entity
    {
        

        #region variables
        public int winStreak { get; set; } = 0;
        public int dungeonBossProgress { get; set; } = 1;
        public int expNeededToLvl { get; set; }
        public int exp { get; set; }
        public int gold { get; set; }
        public int fame { get; set; }
        public bool defeatEasy { get; set; } = false;
        public bool defeatMedium { get; set; } = false;
        public bool defeatHard { get; set;} = false;
        public bool defeatColliseum { get; set;} = false;
        //public int ap { get; set; }
        public Queue<Item> itemsToRecieve { get; set; } = new Queue<Item>();//ZAPAMIĘTAĆ, BEZ TEGO NIE DZIAŁA
        public int regenerationHP { get; set; }
        public int regenerationStamina { get; set; }
        public float staminaMultiplier { get; set; }
        public int maxStamina { get; set; }
        public int currentStamina { get; set; }
        public bool expeditionInProgress { get; set; }
        public DateTime expeditionEnd { get; set; }
        public DateTime arenaCooldown { get; set; }
        public DateTime dungeonCooldown { get; set; }
        public string expeditionName { get; set; }
        public int workHours { get; set; }
        //public Button expeditionButton { get; set; }
        public bool inDungeon { get; set; }
        public bool fightWon { get; set; }
        public bool eqDisplay { get; set; } = true;
        public bool inventoryDisplay { get; set; } = true;
        [JsonIgnore] public Expedition dungeonExpedition { get; set; }
        public bool expeditionNotification { get; set; } = true;
        public bool workNotification { get; set; } = true;
        public bool dungeonNotification { get; set; } = true;
        public bool arenaNotification { get; set; } = false;
        public bool shopNotification {  get; set; } = false;


        #endregion
        public Player(string _name, int _level, int _strength, int _perception, int _dexterity, int _agility, int _vitality, int _endurance, int _charisma, int _intelligence)
        {
            name = _name;
            level = _level;
            baseStrength = _strength;
            basePerception = _perception;
            baseDexterity = _dexterity;
            baseAgility = _agility;
            baseVitality = _vitality;
            baseEndurance = _endurance;
            baseCharisma = _charisma;
            baseIntelligence = _intelligence;
            CurrentHealth = maxHealth;
            expNeededToLvl = 10;
            exp = 0;
            gold = 0;
            fame = 0;
            regenerationHP = 2;
            regenerationStamina = 1;
            staminaMultiplier = 2.75f;
            maxStamina = 0;
            currentStamina = 0;
            expeditionName = "";
            trainingCostReduction = 1;
            lastRecordedTime = DateTime.Now;
        }
        public Player() { }

        public event EventHandler TutorialChanged;
        public int Tutorial
        {
            get { return tutorial; }
            set
            {
                if(tutorial !=value)
                {
                    tutorial = value;
                    OnTutorialChanged();
                }
            }
        }
        public virtual void OnTutorialChanged()
        {

            TutorialChanged?.Invoke(this, EventArgs.Empty);

        }
        #region Events
        #region HealthEvent
        //Zdarzenie pilnujące aktualnego zdrowia
        public event EventHandler CurrentHealthChanged;

        public int CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                if (currentHealth != value)
                {
                    currentHealth = value;
                    OnCurrentHealthChanged();
                }
            }
        }
        public virtual void OnCurrentHealthChanged()
        {
            CurrentHealthChanged?.Invoke(this, EventArgs.Empty);
        }
        public void HealthUpdate()
        {
            baseHealth = (int)Math.Floor((10 * vitality) * (1 + (level * 0.03)));
            maxHealth = baseHealth + bonusHealth;
            OnCurrentHealthChanged();
        }
        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) { currentHealth = maxHealth; }

        }
        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }

        }
        #endregion
        #region Stamina event
        public event EventHandler CurrentStaminaChanged;
        public int CurrentStamina
        {
            get { return currentStamina; }
            set
            {
                if (currentStamina != value)
                {
                    currentStamina = value;
                    OnCurrentStaminaChanged();
                }
            }
        }
        public virtual void OnCurrentStaminaChanged()
        {
            CurrentStaminaChanged?.Invoke(this, EventArgs.Empty);
        }


        #endregion
        #region Gold event
        //Zdarzenie aktualizujące aktualne złoto
        public event EventHandler GoldChanged;
        public int Gold
        {
            get { return gold; }
            set
            {
                if (gold != value)
                {
                    gold = value;
                    OnGoldChanged();
                }
            }
        }
        public virtual void OnGoldChanged()
        {
            GoldChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region Fame event
        public event EventHandler FameChanged;
        public int Fame
        {
            get
            {
                return fame;
            }
            set
            {
                if (fame != value)
                {
                    if (value >= 0) fame = value;
                    else fame = 0;
                    OnFameChanged();
                }
            }
        }
        public virtual void OnFameChanged()
        {
            FameChanged?.Invoke(this, EventArgs.Empty);

        }
        #endregion

        //public event EventHandler ExpStrength;
        //public event EventHandler ExpPerception;
        //public event EventHandler ExpDexterity;
        //public event EventHandler ExpAgility;
        //public event EventHandler ExpVitality;
        //public event EventHandler ExpEndurance;
        //public event EventHandler ExpCharisma;
        //public event EventHandler ExpIntelligence;

        public event EventHandler ExpChanged;
        public int Exp
        {
            get { return exp; }
            set { if (exp != value) { exp = value; OnExpChanged(); } }
        }
        public virtual void OnExpChanged()
        {
            ExpChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region Other
        public DateTime lastRecordedTime { get; set; }
        public float trainingCostReduction { get; set; }
        public DateTime shopDate { get; set; }
        public List<Item> shopInventory { get; set; } = new List<Item>();
        public Item arenaReward { get; set; }
        public List<string> fightHistory { get; set; } = new List<string>();
        public int tutorial { get; set; } = 0;
        #endregion
    }
    public enum ItemClass
    {
        Helmet,
        Torso,
        Gloves,
        Boots,
        Weapon,
        Shield,
        Ring,
        Belt,
        Necklace,
        nothing

    }
    [Serializable]
    public class Item
    {
        public string itemName { get; set; }
        public string preffixName { get; set; } = "";
        public string affixName { get; set; } = "";
        public ItemClass itemClass { get; set; }
        public string itemImage { get; set; }
        public System.Drawing.Color color { get; set; } //White, LightGreen, DodgerBlue, DarkViolet, Orange, Red?
        public string itemDescription { get; set; }
        public int randomizedValue { get; set; } = 0;
        public int sellValue { get; set; }
        public int affixValue { get; set; } = 0;
        public int prefixValue { get; set; } = 0;
        public int buyValue { get; set; }
        public int levelReq { get; set; }
        public int armorValue { get; set; }
        public int minDamage { get; set; }
        public int maxDamage { get; set; }
        public int damageBonus { get; set; }
        public int strengthBonus { get; set; }
        public int strengthMultiplier { get; set; }
        public int perceptionBonus { get; set; }
        public int perceptionMultiplier { get; set; }
        public int dexterityBonus { get; set; }
        public int dexterityMultiplier { get; set; }
        public int agilityBonus { get; set; }
        public int agilityMultiplier { get; set; }
        public int vitalityBonus { get; set; }
        public int vitalityMultiplier { get; set; }
        public int enduranceBonus { get; set; }
        public int enduranceMultiplier { get; set; }
        public int charismaBonus { get; set; }
        public int charismaMultiplier { get; set; }
        public int intelligenceBonus { get; set; }
        public int intelligenceMultiplier { get; set; }
        public int healthBonus { get; set; }
        public int armorBonus { get; set; }

        //REMOVE UNNECCECARY REFERENCES!!!
        public void UpdateDescription(Dictionary<string, string> localizedStrings)
        {
            itemDescription = "";
            if (this.preffixName != "") this.itemDescription += localizedStrings[this.preffixName];
            this.itemDescription += localizedStrings[this.itemName];
            if (this.affixName != "") this.itemDescription += localizedStrings[this.affixName];
            this.itemDescription += "\n" + localizedStrings["itemLevel"] + " " + levelReq.ToString();
            if (itemClass == ItemClass.Weapon) { itemDescription += "\n" + localizedStrings["itemWeapon"]; }
            else if (itemClass == ItemClass.Helmet) { itemDescription += "\n" + localizedStrings["itemHelmet"]; }
            else if (itemClass == ItemClass.Torso) { itemDescription += "\n" + localizedStrings["itemTorso"]; }
            else if (itemClass == ItemClass.Gloves) { itemDescription += "\n" + localizedStrings["itemGloves"]; }
            else if (itemClass == ItemClass.Boots) { itemDescription += "\n" + localizedStrings["itemBoots"]; }
            else if (itemClass == ItemClass.Shield) { itemDescription += "\n" + localizedStrings["itemShield"]; }
            else if (itemClass == ItemClass.Ring) { itemDescription += "\n" + localizedStrings["itemRing"]; }
            else if (itemClass == ItemClass.Belt) { itemDescription += "\n" + localizedStrings["itemBelt"]; }
            else if (itemClass == ItemClass.Necklace) { itemDescription += "\n" + localizedStrings["itemNecklace"]; }

            if (armorValue > 0) { itemDescription += "\n" + localizedStrings["itemArmor"] + " " + armorValue.ToString(); }

            if (minDamage > 0 || maxDamage > 0) { itemDescription += "\n" + localizedStrings["damText"] + " " + minDamage.ToString() + "-" + maxDamage.ToString(); }

            if (damageBonus > 0) { itemDescription += "\n" + localizedStrings["damText"] + " +" + damageBonus.ToString(); }
            else if (damageBonus < 0) { itemDescription += "\n" + localizedStrings["damText"] + " " + damageBonus.ToString(); }

            if (strengthBonus > 0) { itemDescription += "\n" + localizedStrings["strText"] + " +" + strengthBonus.ToString(); }
            else if (strengthBonus < 0) { itemDescription += "\n" + localizedStrings["strText"] + " " + strengthBonus.ToString(); }
            if (strengthMultiplier > 0) { itemDescription += "\n" + localizedStrings["strText"] + " +" + strengthMultiplier.ToString() + "%"; }
            else if (strengthMultiplier < 0) { itemDescription += "\n" + localizedStrings["strText"] + " " + strengthMultiplier.ToString() + "%"; }

            if (perceptionBonus > 0) { itemDescription += "\n" + localizedStrings["perText"] + " +" + perceptionBonus.ToString(); }
            else if (perceptionBonus < 0) { itemDescription += "\n" + localizedStrings["perText"] + " " + perceptionBonus.ToString(); }
            if (perceptionMultiplier > 0) { itemDescription += "\n" + localizedStrings["perText"] + " +" + perceptionMultiplier.ToString() + "%"; }
            else if (perceptionMultiplier < 0) { itemDescription += "\n" + localizedStrings["perText"] + " " + perceptionMultiplier.ToString() + "%"; }

            if (dexterityBonus > 0) { itemDescription += "\n" + localizedStrings["dexText"] + " +" + dexterityBonus.ToString(); }
            else if (dexterityBonus < 0) { itemDescription += "\n" + localizedStrings["dexText"] + " " + dexterityBonus.ToString(); }
            if (dexterityMultiplier > 0) { itemDescription += "\n" + localizedStrings["dexText"] + " +" + dexterityMultiplier.ToString() + "%"; }
            else if (dexterityMultiplier < 0) { itemDescription += "\n" + localizedStrings["dexText"] + " " + dexterityMultiplier.ToString() + "%"; }

            if (agilityBonus > 0) { itemDescription += "\n" + localizedStrings["agiText"] + " +" + agilityBonus.ToString(); }
            else if (agilityBonus < 0) { itemDescription += "\n" + localizedStrings["agiText"] + " " + agilityBonus.ToString(); }
            if (agilityMultiplier > 0) { itemDescription += "\n" + localizedStrings["agiText"] + " +" + agilityMultiplier.ToString() + "%"; }
            else if (agilityMultiplier < 0) { itemDescription += "\n" + localizedStrings["agiText"] + " " + agilityMultiplier.ToString() + "%"; }

            if (vitalityBonus > 0) { itemDescription += "\n" + localizedStrings["vitText"] + " +" + vitalityBonus.ToString(); }
            else if (vitalityBonus < 0) { itemDescription += "\n" + localizedStrings["vitText"] + " " + vitalityBonus.ToString(); }
            if (vitalityMultiplier > 0) { itemDescription += "\n" + localizedStrings["vitText"] + " +" + vitalityMultiplier.ToString() + "%"; }
            else if (vitalityMultiplier < 0) { itemDescription += "\n" + localizedStrings["vitText"] + " " + vitalityMultiplier.ToString() + "%"; }

            if (enduranceBonus > 0) { itemDescription += "\n" + localizedStrings["endText"] + " +" + enduranceBonus.ToString(); }
            else if (enduranceBonus < 0) { itemDescription += "\n" + localizedStrings["endText"] + " " + enduranceBonus.ToString(); }
            if (enduranceMultiplier > 0) { itemDescription += "\n" + localizedStrings["endText"] + " +" + enduranceMultiplier.ToString() + "%"; }
            else if (enduranceMultiplier < 0) { itemDescription += "\n" + localizedStrings["endText"] + " " + enduranceMultiplier.ToString() + "%"; }

            if (charismaBonus > 0) { itemDescription += "\n" + localizedStrings["chaText"] + " +" + charismaBonus.ToString(); }
            else if (charismaBonus < 0) { itemDescription += "\n" + localizedStrings["chaText"] + " " + charismaBonus.ToString(); }
            if (charismaMultiplier > 0) { itemDescription += "\n" + localizedStrings["chaText"] + " +" + charismaMultiplier.ToString() + "%"; }
            else if (charismaMultiplier < 0) { itemDescription += "\n" + localizedStrings["chaText"] + " " + charismaMultiplier.ToString() + "%"; }


            if (intelligenceBonus > 0) { itemDescription += "\n" + localizedStrings["intText"] + " +" + intelligenceBonus.ToString(); }
            else if (intelligenceBonus < 0) { itemDescription += "\n" + localizedStrings["intText"] + " " + intelligenceBonus.ToString(); }
            if (intelligenceMultiplier > 0) { itemDescription += "\n" + localizedStrings["intText"] + " +" + intelligenceMultiplier.ToString() + "%"; }
            else if (intelligenceMultiplier < 0) { itemDescription += "\n" + localizedStrings["intText"] + " " + intelligenceMultiplier.ToString() + "%"; }

            if (healthBonus > 0) { itemDescription += "\n" + localizedStrings["itemHealth"] + " +" + healthBonus.ToString(); }
            else if (healthBonus < 0) { itemDescription += "\n" + localizedStrings["itemHealth"] + " " + healthBonus.ToString(); }

            if (armorBonus > 0) { itemDescription += "\n" + localizedStrings["itemArmor"] + " +" + armorBonus.ToString(); }
            else if (armorBonus < 0) { itemDescription += "\n" + localizedStrings["itemArmor"] + " " + armorBonus.ToString(); }

            SetSellValue();
            itemDescription += "\n\n" + localizedStrings["itemSellValue"] + " : " + sellValue + " " + localizedStrings["itemGoldText"];

        }
        public void SetSellValue()
        {
            Random random = new Random();
            //float rarityModifier = 1;
            //if (color == System.Drawing.Color.White) { rarityModifier = 1f; }
            //else if (color == System.Drawing.Color.LightGreen) { rarityModifier = 1.5f; }
            //else if (color == System.Drawing.Color.DodgerBlue) { rarityModifier = 2.5f; }
            //else if (color == System.Drawing.Color.DarkViolet) { rarityModifier = 4f; }
            //else if (color == System.Drawing.Color.Orange) { rarityModifier = 7f; }
            //else if (color == System.Drawing.Color.Red) { rarityModifier = 12f; }
            //this.sellValue = (int)Math.Floor(rarityModifier * ((float)this.levelReq * 10f + StatValue()));
            if (randomizedValue == 0) randomizedValue = random.Next(90, 111);
            this.sellValue = (int)Math.Floor((float)randomizedValue/100*
                ((float)levelReq*20f+
                (float)this.levelReq * (float)this.affixValue));
        }
        float StatValue()
        {
            float statValue = levelReq;
            float multiValue = levelReq*2;
            float value = 0;
            if (healthBonus > 0) value += healthBonus;
            if (armorBonus > 0) value += armorBonus;
            if (damageBonus > 0) value += damageBonus * statValue*10;
            if (strengthBonus > 0) value += strengthBonus*statValue;
            if (strengthBonus > 0) value += strengthBonus * multiValue;
            if (perceptionBonus > 0) value += perceptionBonus*statValue;
            if (perceptionMultiplier > 0) value += perceptionMultiplier * multiValue;
            if (dexterityBonus > 0) value += dexterityBonus*statValue;
            if (dexterityMultiplier > 0) value += dexterityMultiplier * multiValue;
            if (agilityBonus > 0) value += agilityBonus*statValue;
            if (agilityMultiplier > 0) value += agilityMultiplier * multiValue;
            if (enduranceBonus > 0) value += enduranceBonus*statValue;
            if (enduranceMultiplier > 0) value += enduranceMultiplier * multiValue;
            if (vitalityBonus > 0) value += vitalityBonus*statValue;
            if (vitalityMultiplier > 0) value += vitalityMultiplier * multiValue;
            if (charismaBonus > 0) value += charismaBonus*statValue;
            if (charismaMultiplier > 0) value += charismaMultiplier * multiValue;
            if (intelligenceBonus > 0) value += intelligenceBonus*statValue;
            if (intelligenceMultiplier > 0) value += intelligenceMultiplier * multiValue;

            return value;
        }
        public Item()
        {
        }
        public Item(string itemName, ItemClass itemClass, System.Drawing.Color color, int levelReq, int armorValue, int minDamage, int maxDamage, int damageBonus, int strengthBonus, int perceptionBonus, int dexterityBonus, int agilityBonus, int vitalityBonus, int enduranceBonus, int charismaBonus, int intelligenceBonus, int healthBonus, int armorBonus, Dictionary<string, string> localizedStrings, int strengthMultiplier = 0, int perceptionMultiplier = 0, int dexterityMultiplier = 0, int agilityMultiplier = 0, int vitalityMultiplier = 0, int enduranceMultiplier = 0, int charismaMultiplier = 0, int intelligenceMultiplier = 0)
        {

            this.itemName = itemName;
            this.itemDescription = "";
            this.itemClass = itemClass;
            this.levelReq = levelReq;
            this.color = color;
            this.armorValue = armorValue;
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
            this.damageBonus = damageBonus;
            this.strengthBonus = strengthBonus;
            this.strengthMultiplier = strengthMultiplier;
            this.perceptionBonus = perceptionBonus;
            this.perceptionMultiplier = perceptionMultiplier;
            this.dexterityBonus = dexterityBonus;
            this.dexterityMultiplier = dexterityMultiplier;
            this.agilityBonus = agilityBonus;
            this.agilityMultiplier = agilityMultiplier;
            this.vitalityBonus = vitalityBonus;
            this.vitalityMultiplier = vitalityMultiplier;
            this.enduranceBonus = enduranceBonus;
            this.enduranceMultiplier = enduranceMultiplier;
            this.charismaBonus = charismaBonus;
            this.charismaMultiplier = charismaMultiplier;
            this.intelligenceBonus = intelligenceBonus;
            this.intelligenceMultiplier = intelligenceMultiplier;
            this.healthBonus = healthBonus;
            this.armorBonus = armorBonus;
            UpdateDescription(localizedStrings);





        }

        public Item(Dictionary<string, object> dict)
        {
            itemName = dict["itemName"].ToString();
            int itemType = Convert.ToInt32(dict["itemClass"]);
            switch (itemType)
            {
                case 0:
                    itemClass = ItemClass.Weapon;
                    break;
                case 1:
                    itemClass = ItemClass.Gloves;
                    break;
                case 2:
                    itemClass = ItemClass.Helmet;
                    break;
                case 3:
                    itemClass = ItemClass.Torso;
                    break;
                case 4:
                    itemClass = ItemClass.Boots;
                    break;
                case 5:
                    itemClass = ItemClass.Shield;
                    break;
                case 6:
                    itemClass = ItemClass.Necklace;
                    break;
                case 7:
                    itemClass = ItemClass.Belt;
                    break;
                case 8:
                    itemClass = ItemClass.Ring;
                    break;

            }

            itemImage = dict["itemImage"].ToString();
            color = (System.Drawing.Color)dict["color"];
            itemDescription = dict["itemDescription"].ToString();
            sellValue = Convert.ToInt32(dict["sellValue"]);
            levelReq = Convert.ToInt32(dict["levelReq"]);
            armorValue = Convert.ToInt32(dict["armorValue"]);
            minDamage = Convert.ToInt32(dict["minDamage"]);
            maxDamage = Convert.ToInt32(dict["maxDamage"]);
            damageBonus = Convert.ToInt32(dict["damageBonus"]);
            strengthBonus = Convert.ToInt32(dict["strengthBonus"]);
            strengthMultiplier = Convert.ToInt32(dict["strengthMultiplier"]);
            perceptionBonus = Convert.ToInt32(dict["perceptionBonus"]);
            perceptionMultiplier = Convert.ToInt32(dict["perceptionMultiplier"]);
            dexterityBonus = Convert.ToInt32(dict["dexterityBonus"]);
            dexterityMultiplier = Convert.ToInt32(dict["dexterityMultiplier"]);
            agilityBonus = Convert.ToInt32(dict["agilityBonus"]);
            agilityMultiplier = Convert.ToInt32(dict["agilityMultiplier"]);
            vitalityBonus = Convert.ToInt32(dict["vitalityBonus"]);
            vitalityMultiplier = Convert.ToInt32(dict["vitalityMultiplier"]);
            enduranceBonus = Convert.ToInt32(dict["enduranceBonus"]);
            enduranceMultiplier = Convert.ToInt32(dict["enduranceMultiplier"]);
            charismaBonus = Convert.ToInt32(dict["charismaBonus"]);
            charismaMultiplier = Convert.ToInt32(dict["charismaMultiplier"]);
            intelligenceBonus = Convert.ToInt32(dict["intelligenceBonus"]);
            intelligenceMultiplier = Convert.ToInt32(dict["intelligenceMultiplier"]);
            healthBonus = Convert.ToInt32(dict["healthBonus"]);
            armorBonus = Convert.ToInt32(dict["armorBonus"]);
        }


    }

    public class ItemBuff
    {
        public string buffName = "";
        public int value { get; set; }
        public int strengthBonus { get; set; }
        public int strengthMultiplier { get; set; }
        public int perceptionBonus { get; set; }
        public int perceptionMultiplier { get; set; }
        public int dexterityBonus { get; set; }
        public int dexterityMultiplier { get; set; }
        public int agilityBonus { get; set; }
        public int agilityMultiplier { get; set; }
        public int vitalityBonus { get; set; }
        public int vitalityMultiplier { get; set; }
        public int enduranceBonus { get; set; }
        public int enduranceMultiplier { get; set; }
        public int charismaBonus { get; set; }
        public int charismaMultiplier { get; set; }
        public int intelligenceBonus { get; set; }
        public int intelligenceMultiplier { get; set; }
        public int damageBonus { get; set; }
        public int healthBonus { get; set; }
        public int armorBonus { get; set; }
    }

}
