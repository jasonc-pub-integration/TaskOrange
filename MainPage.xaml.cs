using InMobi.Ads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaskOrange
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IMBanner banner;
        private IMInterstitial interstitial;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e){
            adPanel?.UpdateLayout();    // Update layout on page load
        }


        #region Utility 

        private String getPlacementFromInput(){
            if (plcTextbox.Text == null){
                return "";
            } else {
                return plcTextbox.Text.Trim();
            }
        }


        #endregion

        #region Banner

        public void LoadAndShowBanner(object sender, RoutedEventArgs e)
        {
            if (banner != null){
                RemoveBannerFromParent();
                ClearBannerRef();
            }

            String bannerPLC = getPlacementFromInput();

            if (bannerPLC == "")
            {
                Debug.WriteLine($"No PLC provided");
                return;
            }

            banner = new IMBanner(bannerPLC, IMAdSize.Banner320x50);
            banner.Margin = new Thickness(0, 40, 0, 0);
            banner.OnAdLoadSucceeded += Banner_OnAdLoadSucceeded;
            banner.OnAdLoadFailed += Banner_OnAdLoadFailed;
            banner.OnAdDisplayed += Banner_OnAdDisplayed;
            banner.OnAdDismissed += Banner_OnAdDismissed;
            banner.OnAdInteraction += Banner_OnAdInteraction;
            banner.OnAdWillLeaveApplication += Banner_OnAdWillLeaveApplication;
            banner.RefreshInterval = 30;
            banner.LoadAd();

            Debug.WriteLine("banner.LoadAd called");

            LoadingIndicator.Visibility = Visibility.Visible;
            LoadingIndicator.IsActive = true;
        }

        private void Banner_OnAdWillLeaveApplication(object sender, string e)
        {
            Debug.WriteLine("Banner_OnAdWillLeaveApplication");
        }

        private void Banner_OnAdInteraction(object sender, string e)
        {
            Debug.WriteLine("Banner_OnAdInteraction");
        }

        private void Banner_OnAdDismissed(object sender, string e)
        {
            Debug.WriteLine("Banner_OnAdDismissed");
        }

        private void Banner_OnAdDisplayed(object sender, string e)
        {
            Debug.WriteLine("Banner_OnAdDisplayed");
        }

        private void Banner_OnAdLoadFailed(object sender, IMError e)
        {
            Debug.WriteLine("Banner_OnAdLoadFailed");
            LoadingIndicator.Visibility = Visibility.Collapsed;
            LoadingIndicator.IsActive = false;
        }

        private void Banner_OnAdLoadSucceeded(object sender, string e){

            LoadingIndicator.Visibility = Visibility.Collapsed;
            LoadingIndicator.IsActive = false;

            if (banner != null){
                adSpacePanel?.Children.Add(banner);
                adSpacePanel?.UpdateLayout();
                Debug.WriteLine("Banner_OnAdLoadSucceeded - banner added to view");
            }
        }

        private void RemoveBannerFromParent()
        {
            var parent = banner?.Parent;
            if (parent != null && parent is Panel)
            {
                var panelParent = parent as Panel;
                panelParent.Children.Remove(banner);
                Debug.WriteLine("RemoveBannerFromParent - banner removed from view");
            }
        }


        public void RemoveBanner(object sender, RoutedEventArgs e)
        {
            RemoveBannerFromParent();
            ClearBannerRef();
        }

        public void ClearBannerRef()
        {
            if (banner != null)
            {
                banner.OnAdLoadSucceeded -= Banner_OnAdLoadSucceeded;
                banner.OnAdLoadFailed -= Banner_OnAdLoadFailed;
                banner.OnAdDisplayed -= Banner_OnAdDisplayed;
                banner.OnAdDismissed -= Banner_OnAdDismissed;
                banner.OnAdInteraction -= Banner_OnAdInteraction;
                banner.OnAdWillLeaveApplication -= Banner_OnAdWillLeaveApplication;
                banner.Dispose();
                banner = null;
                Debug.WriteLine("ClearBannerRef - banner listeners cleaned up and disposed");

            }
        }

        #endregion


        #region Interstitial

        private void ClearInterstitialRef()
        {
            if (interstitial != null)
            {
                interstitial.OnAdLoadSucceeded -= Interstitial_OnAdLoaded;
                interstitial.OnAdLoadFailed -= Interstitial_OnAdFailed;
                interstitial.OnAdWillDisplay -= Interstitial_OnAdWillDisplay;
                interstitial.OnAdDisplayed -= Interstitial_OnAdDisplayed;
                interstitial.OnAdDismissed -= Interstitial_OnAdDismissed;
                interstitial.OnAdDisplayFailed -= Interstitial_OnAdDisplayFailed;
                interstitial.OnAdInteraction -= Interstitial_OnAdInteraction;
                interstitial.OnAdWillLeaveApplication -= Interstitial_OnAdWillLeaveApplication;
                interstitial.OnRewardsUnlocked -= Interstitial_OnRewardsUnlocked;
                interstitial.OnVideoCompleted -= Interstitial_OnVideoCompleted;
                interstitial.OnVideoCompleted -= Interstitial_OnVideoCompleted;
                interstitial.Dispose();
                interstitial = null;
            }
        }


        private void AttachInterstitialListeners()
        {
            interstitial.OnAdLoadSucceeded += Interstitial_OnAdLoaded;
            interstitial.OnAdLoadFailed += Interstitial_OnAdFailed;
            interstitial.OnAdWillDisplay += Interstitial_OnAdWillDisplay;
            interstitial.OnAdDisplayed += Interstitial_OnAdDisplayed;
            interstitial.OnAdDismissed += Interstitial_OnAdDismissed;
            interstitial.OnAdDisplayFailed += Interstitial_OnAdDisplayFailed;
            interstitial.OnAdInteraction += Interstitial_OnAdInteraction;
            interstitial.OnAdWillLeaveApplication += Interstitial_OnAdWillLeaveApplication;
            interstitial.OnRewardsUnlocked += Interstitial_OnRewardsUnlocked;
            interstitial.OnVideoCompleted += Interstitial_OnVideoCompleted;
        }

        public void LoadAndShowInterstitial(object sender, RoutedEventArgs e)
        {
            // TODO
            Debug.WriteLine("LoadAndShowInterstitial");
            ClearInterstitialRef();


            String intPLC = getPlacementFromInput();

            if (intPLC == "")
            {
                return;
            }

            interstitial = new IMInterstitial(intPLC);
            AttachInterstitialListeners();
            interstitial.LoadAd();
        }


        private void Interstitial_OnVideoCompleted(object sender, bool e)
        {
            String completeMsg = e ? "video completed" : "video incomplete";
            Debug.WriteLine($"Interstitial_OnVideoCompleted - {completeMsg}");
        }

        private void Interstitial_OnAdFailed(object sender, IMError e)
        {
            Debug.WriteLine($"Interstitial_OnAdFailed - {e.Code}: {e.Reason}");
        }

        private void Interstitial_OnAdLoaded(object sender, string e)
        {
            Debug.WriteLine("Interstitial_OnAdLoaded - showing ad!");
            interstitial?.ShowAd();

            /*
            var handler = new DispatchedHandler(() => {
                Debug.WriteLine("Interstitial_OnAdLoaded - showing ad!");
                interstitial?.ShowAd();
            });

            Task show = Task.Run(async () => {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
            });
            */

        }

        private void Interstitial_OnAdWillDisplay(object sender, string e)
        {
            Debug.WriteLine($"OnAdWillPresent - {e}");
        }

        private void Interstitial_OnAdDisplayed(object sender, string e)
        {
            Debug.WriteLine("Interstitial_OnAdDisplayed - ad displayed");
        }

        private void Interstitial_OnAdDismissed(object sender, string e)
        {
            Debug.WriteLine("Interstitial_OnAdDismissed - ad dismissed");
            ClearInterstitialRef();
        }

        private void Interstitial_OnAdDisplayFailed(object sender, IMError e)
        {
            Debug.WriteLine($"OnAdDisplayFailed - {e.Code} : {e.Reason}");
        }

        private void Interstitial_OnAdInteraction(object sender, string e)
        {
            Debug.WriteLine($"OnAdInteraction - {e}");
        }

        private void Interstitial_OnAdWillLeaveApplication(object sender, string e)
        {
            Debug.WriteLine($"OnAdWillLeaveApplication - {e}");
        }

        private void Interstitial_OnRewardsUnlocked(object sender, IMAdRewards e)
        {
            string rewardString = "";
            foreach (KeyValuePair<string, string> kvp in e.Rewards)
            {
                if (rewardString.Length != 0)
                {
                    rewardString += ", ";
                }
                rewardString += string.Format("{0} : {1}", kvp.Key, kvp.Value);
            }
            Debug.WriteLine($"OnRewardsUnlocked - {rewardString}");
        }

        #endregion
    }
}
