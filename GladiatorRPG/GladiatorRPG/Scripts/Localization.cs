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
using GladiatorRPG.ResourceFiles.ResourceFilesPre13;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;

namespace GladiatorRPG
{
    //Tłumaczenie
    public partial class MainTabPage
    {
        #region Localization

        public string headButtonText, torsoButtonText, legsButtonText, handsButtonText, weaponButtonText, shieldButtonText, lvlText, damText, lifeText, fameText, goldText, armorText, eqText, stamText,
            actionItemText, actionEquipText, actionSellText, actionUnequipText, lvlFightText, insufficientGoldMessage, okText,
            cautionText, noSpaceInInventoryText, insufficientStaminaMessage, insufficientMaxStaminaMessage, expeditionCancel, yesText, noText,
            ringButtonText, beltButtonText, necklaceButtonText;
        //Pobiera wartości z AppResources.resx do dictionary
        public static Dictionary<string, string> GetAllLocalizedStrings()
        {
            var localizedStrings = new Dictionary<string, string>();

            string wersja = DeviceInfo.Version.ToString();
            float version = float.Parse(wersja, CultureInfo.InvariantCulture);
            var resourceManager = new ResourceManager(typeof(AppResources));
            if (version < 13f) resourceManager = new ResourceManager(typeof(AppResourcesPre13));

            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSet != null)
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    string key = entry.Key.ToString();
                    string value = entry.Value.ToString();

                    localizedStrings[key] = value;
                }
            }
            //do przerobienia, ma decydować co zrobić gdy nie istnieje dane tłumaczenie z lokalizacji
            //if (string.IsNullOrEmpty(value))
            //{
            //    var defaultResourceManager = new ResourceManager(typeof(AppResources));
            //    value = defaultResourceManager.GetString(key, culture);
            //}
            return localizedStrings;
        }

        //Aktualizuje elementy informacyjne UI
        void Localization(Dictionary<string, string> localizedStrings)
        {
            try
            {


                lvlText = localizedStrings["lvlText"];
                headButtonText = localizedStrings["headButtonText"];
                torsoButtonText = localizedStrings["torsoButtonText"];
                legsButtonText = localizedStrings["legsButtonText"];
                handsButtonText = localizedStrings["handsButtonText"];
                weaponButtonText = localizedStrings["weaponButtonText"];
                shieldButtonText = localizedStrings["shieldButtonText"];
                ringButtonText = localizedStrings["ringButtonText"];
                beltButtonText = localizedStrings["beltButtonText"];
                necklaceButtonText = localizedStrings["necklaceButtonText"];
                strLabel.Text = localizedStrings["strText"]; strLabel1.Text = localizedStrings["strText"];
                perLabel.Text = localizedStrings["perText"]; perLabel1.Text = localizedStrings["perText"];
                dexLabel.Text = localizedStrings["dexText"]; dexLabel1.Text = localizedStrings["dexText"];
                agiLabel.Text = localizedStrings["agiText"]; agiLabel1.Text = localizedStrings["agiText"];
                vitLabel.Text = localizedStrings["vitText"]; vitLabel1.Text = localizedStrings["vitText"];
                endLabel.Text = localizedStrings["endText"]; endLabel1.Text = localizedStrings["endText"];
                chaLabel.Text = localizedStrings["chaText"]; chaLabel1.Text = localizedStrings["chaText"];
                intLabel.Text = localizedStrings["intText"]; intLabel1.Text = localizedStrings["intText"];
                damText = localizedStrings["damText"];
                lifeText = localizedStrings["lifeText"];
                fameText = localizedStrings["fameText"];
                goldText = localizedStrings["goldText"];
                armorText = localizedStrings["armorText"];
                eqText = localizedStrings["eqText"];
                stamText = localizedStrings["stamText"];
                actionItemText = localizedStrings["actionItemText"];
                actionEquipText = localizedStrings["actionEquipText"];
                actionSellText = localizedStrings["actionSellText"];
                actionUnequipText = localizedStrings["actionUnequipText"];
                overallScreen.Title = localizedStrings["overallScreen"];
                trainingScreen.Title = localizedStrings["trainingScreen"];
                expeditionScreen.Title = localizedStrings["expeditionScreen"];
                dungeonScreen.Title = localizedStrings["dungeonScreen"];
                trainingInfo.Text = localizedStrings["trainingInfo"];
                insufficientGoldMessage = localizedStrings["insufficientGoldMessage"];
                insufficientStaminaMessage = localizedStrings["insufficientStaminaMessage"];
                insufficientMaxStaminaMessage = localizedStrings["insufficientMaxStaminaMessage"];
                okText = localizedStrings["okText"];
                cautionText = localizedStrings["cautionText"];
                noSpaceInInventoryText = localizedStrings["noSpaceInInventoryText"];
                expeditionCancel = localizedStrings["expeditionCancel"];
                yesText = localizedStrings["yesText"]; noText = localizedStrings["noText"];
                characterTab.Title = localizedStrings["characterTab"];
                expeditionTab.Title = localizedStrings["expeditionTab"];
                cityTab.Title = localizedStrings["cityTab"];
                arenaFameReward.Text = localizedStrings["arenaFameRewardText"];
                optionsButton.Text = localizedStrings["optionText"];
                trainingInfoButton.Text = localizedStrings["trainingInfoButton"];
                shopButton.Text = localizedStrings["shopButton"];
                strTrainButton.Text = localizedStrings["trainText"];
                perTrainButton.Text = localizedStrings["trainText"];
                dexTrainButton.Text = localizedStrings["trainText"];
                agiTrainButton.Text = localizedStrings["trainText"];
                vitTrainButton.Text = localizedStrings["trainText"];
                endTrainButton.Text = localizedStrings["trainText"];
                chaTrainButton.Text = localizedStrings["trainText"];
                intTrainButton.Text = localizedStrings["trainText"];
                goalsButton.Text = localizedStrings["goalsText"];
                //manualSave.Text = localizedStrings["manualSave"];
                
            }
            catch
            {
                TEST("TEXT NOT FOUND");
            }
        }
        #endregion

    }
}
