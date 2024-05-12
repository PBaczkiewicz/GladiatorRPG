using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static Android.Renderscripts.Sampler;
using static Android.Resource;
using Button = Xamarin.Forms.Button;
using Slider = Xamarin.Forms.Slider;

namespace GladiatorRPG
{
    public partial class MainTabPage
    {
        int basePay = 20;
        int workHours, work1Pay, work2Pay, work3Pay, work4Pay;
        List<Button> workList;

        //Wczytuje stronę aktualizując wszystkie wartości
        void LoadWorkPage()
        {

            workHours = 1;
            workButtonInfo.Text = localizedStrings["workTextInfo"];
            payLabel.Text = localizedStrings["payText"];
            quickenWork.Text = localizedStrings["buttonSpeedUp"];
            workList = new List<Button> { workButton1, workButton2, workButton3, workButton4 };
            QuickWork.Text = localizedStrings["workText"];
            QuickTrain.Text = localizedStrings["trainText"];
            foreach (Button button in workList) { button.Text = localizedStrings["workText"]; }
            WorkHourModifying();
            if (!player.expeditionInProgress)
            {
                foreach (Button x in workList)
                {
                    x.IsEnabled = true;
                    x.Text = localizedStrings["workText"];
                }
                quickenWork.IsVisible = false;
                slider.IsEnabled = true; slider.Value = 0;

            }
            else
            {
                slider.IsEnabled = false;
                if (new[] { "work1", "work2", "work3", "work4" }.Contains(player.expeditionName)) quickenWork.IsVisible = true;
                if (player.expeditionName == "work1") { expeditionButton = workButton1; }
                else if (player.expeditionName == "work2") { expeditionButton = workButton2; }
                else if (player.expeditionName == "work3") { expeditionButton = workButton3; }
                else if (player.expeditionName == "work4") { expeditionButton = workButton4; }
                foreach (Button x in workList)
                {
                    if (x == expeditionButton) x.IsEnabled = true;
                    else x.IsEnabled = false;
                    x.Text = localizedStrings["workText"];
                }
            }
            UpdatePay();
            UpdateWorkHourly();
            UpdateLabels();
            UpdateQuickButtons();

        }
        //Aktualizuje labelki
        void UpdateLabels()
        {
            pay1.Text = work1Pay.ToString();
            pay2.Text = work2Pay.ToString();
            pay3.Text = work3Pay.ToString();
            pay4.Text = work4Pay.ToString();
        }
        //Aktualizacja godzin pracy
        void WorkHourModifying()
        {
            if (workHours == 0) workHours = 1;
            TimeSet.Text = localizedStrings["hoursText"] + " : " + workHours;
            foreach (Button x in workList)
            {
                x.IsEnabled = true;
            }
            UpdatePay();
            if (workHours > 0) UpdateWorkHourly();
            UpdateLabels();
        }
        //Aktualizacja wysokości zapłaty
        #region Pay
        void UpdatePay()
        {
            work1Pay = (int)Math.Floor((float)player.level * (float)basePay * (1 + ((float)player.baseStrength + (float)player.baseVitality) / 100));
            work2Pay = (int)Math.Floor((float)player.level * (float)basePay * (1 + ((float)player.basePerception + (float)player.baseIntelligence) / 100));
            work3Pay = (int)Math.Floor((float)player.level * (float)basePay * (1 + ((float)player.baseAgility + (float)player.baseCharisma) / 100));
            work4Pay = (int)Math.Floor((float)player.level * (float)basePay * (1 + ((float)player.baseDexterity + (float)player.baseEndurance) / 100));
        }
        void UpdateWorkHourly()
        {
            work1Pay = work1Pay * workHours;
            work2Pay = work2Pay * workHours;
            work3Pay = work3Pay * workHours;
            work4Pay = work4Pay * workHours;
        }
        #endregion
        //Cała logika związana z przyciskami na tej stronie
        #region Buttons

        void workButton1_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "work1";
                Work(button, workHours);
            }
        }
        void workButton2_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "work2";
                Work(button, workHours);
            }
        }
        void workButton3_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "work3";
                Work(button, workHours);
            }
        }
        void workButton4_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                expeditionButton = button;
                player.expeditionName = "work4";
                Work(button, workHours);
            }
        }

        async void Work(Button button, int hours = 0)
        {
            if (player.expeditionInProgress)
            {
                bool result = await DisplayAlert(cautionText, expeditionCancel, yesText, noText);

                if (result)
                {
                    if (player.expeditionInProgress == false) { return; }
                    quickenWork.IsVisible = false;
                    player.expeditionInProgress = false;
                    slider.IsEnabled = true;
                    expeditionButton.Text = localizedStrings["workText"];
                    slider.IsEnabled = true;
                    //Enable'uje pozostałe przyciski
                    foreach (Button x in buttonList) { if (x == expeditionButton) { continue; }; x.IsEnabled = true; }
                    return;
                }
                else
                {
                    return;
                }

            }
            slider.IsEnabled = false;
            quickenWork.IsVisible = true;
            player.workHours = hours;
            if (hours > 0) { player.expeditionEnd = DateTime.Now.AddHours(hours); }
            if(player.workNotification)SendNotification(localizedStrings["notificationWorkFinished"], localizedStrings["notificationWorkFinished1"], 1, player.expeditionEnd);
            expeditionButton = button;
            player.expeditionInProgress = true;
            
            //Disable'uje wszystkie przyciski oprócz wciśniętego
            foreach (Button x in buttonList) { if (x == expeditionButton) { continue; }; x.IsEnabled = false; }
            SaveProgress();
        }
        void EndWork(int type)
        {
            UpdatePay();
            if (player.expeditionInProgress)
            {
                player.expeditionInProgress = false;
                if (type == 1)
                {
                    player.Gold += work1Pay * player.workHours;
                    ShowPayout(work1Pay * player.workHours);
                }
                if (type == 2)
                {
                    player.Gold += work2Pay * player.workHours;
                    ShowPayout(work2Pay * player.workHours);
                }
                if (type == 3)
                {
                    player.Gold += work3Pay * player.workHours;
                    ShowPayout(work3Pay * player.workHours);
                }
                if (type == 4)
                {
                    player.Gold += work4Pay * player.workHours;
                    ShowPayout(work4Pay * player.workHours);
                }
                player.workHours = 0;
            }
        }
        async void ShowPayout(int payOut)
        {
            await DisplayAlert(localizedStrings["cautionText"], localizedStrings["workPayoutText"] + payOut.ToString() + " " + localizedStrings["itemGoldText"], localizedStrings["okText"]);
        }
        void CheckWork()
        {
            if (player.expeditionName == "work1") { EndWork(1); }
            if (player.expeditionName == "work2") { EndWork(2); }
            if (player.expeditionName == "work3") { EndWork(3); }
            if (player.expeditionName == "work4") { EndWork(4); }
        }
        #endregion

        //Suwak
        void Slider_ValueChanged(object sender, EventArgs e)
        {
            workHours = (int)Math.Ceiling(slider.Value)+1;
            if(!player.expeditionInProgress) WorkHourModifying();

        }

        //Przycisk przyspieszenia
        async void quickenWork_Clicked(object sender, EventArgs e)
        {
            int cost = 1;

            TimeSpan timeLeft = player.expeditionEnd - DateTime.Now;

            cost = (timeLeft.Hours * 60) + timeLeft.Minutes + 1;
            bool result = await DisplayAlert(cautionText, localizedStrings["workSpeedup"] + cost + " " + localizedStrings["stamText"] + "\n" + localizedStrings["currentStaminaText"] + player.CurrentStamina + "\n" + localizedStrings["stamText"], yesText, noText);
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
                    player.expeditionEnd = DateTime.Now;
                    DeleteNotifications(1);
                }
            }
            else
            {
                return;
            }
        }
        void UpdateQuickButtons()
        {
            quickWorkCost = player.level; quickWorkPay = player.level * 20;
            if (quickWorkCost < 5) quickWorkCost = 5;
            quickTrainCost = (int)Math.Ceiling((float)player.maxStamina / 2f);
            quickTrainExp = (int)Math.Ceiling((float)player.level / 2f);

            quickWorkLabel.Text = localizedStrings["quickWorkText"] + "\n+" + quickWorkPay.ToString() + " " + localizedStrings["goldText"];
            quickWorkCostLabel.Text = localizedStrings["quickStaminaCost"] + quickWorkCost.ToString();

            quickTrainLabel.Text = localizedStrings["quickTrainText"] + "\n+" + quickTrainExp.ToString() + " " + localizedStrings["expText"];
            quickTrainCostLabel.Text = localizedStrings["quickStaminaCost"] + quickTrainCost.ToString();

        }
        int quickWorkCost = 0, quickWorkPay = 0;
        int quickTrainCost = 0, quickTrainExp = 0;
        async void QuickWork_Clicked(Object sender, EventArgs e)
        {

            if (!clicked)
            {
                UpdateQuickButtons();
                clicked = true;
                if (player.CurrentStamina < quickWorkCost) await DisplayAlert(cautionText, localizedStrings["insufficientStaminaMessage"], okText);
                else
                {
                    player.CurrentStamina -= quickWorkCost;
                    player.Gold += quickWorkPay;
                }


                clicked = false;
            }
        }

        async void QuickTrain_Clicked(Object sender, EventArgs e)
        {
            if (!clicked)
            {
                UpdateQuickButtons();
                clicked = true;
                if (player.CurrentStamina < quickTrainCost) await DisplayAlert(cautionText, localizedStrings["insufficientStaminaMessage"], okText);
                else
                {
                    player.CurrentStamina -= quickTrainCost;
                    player.Exp += quickTrainExp;
                }


                clicked = false;
            }
        }


        private void workButtonInfo_Clicked(object sender, EventArgs e)
        {
            workInfo();
        }
        async void workInfo()
        {
            if (!clicked)
            {
                clicked = true;
                await DisplayAlert(localizedStrings["infoText"], localizedStrings["informationsWork"], localizedStrings["okText"]);
                clicked = false;

            }
        }
    }
    public class SnappingSlider : Slider
    {
        // Define the set points or intervals
        private double[] setPoints = { 0,1,2,3,4,5,6,7 };

        public SnappingSlider()
        {
            // Subscribe to the ValueChanged event
            ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Snap the slider's value to the nearest set point
            double snappedValue = GetNearestSetPoint(e.NewValue);
            Value = snappedValue;
        }

        private double GetNearestSetPoint(double value)
        {
            // Find the nearest set point
            double nearestSetPoint = setPoints[0];
            double minDifference = double.MaxValue;

            foreach (var point in setPoints)
            {
                double difference = Math.Abs(point - value);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearestSetPoint = point;
                }
            }

            return nearestSetPoint;
        }
    }

}
