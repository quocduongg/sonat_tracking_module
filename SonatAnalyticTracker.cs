using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using Firebase.Analytics;
using UnityEngine;

// ReSharper disable InconsistentNaming
public enum AdType
{
    undefined = 0,
    banner = 1,
    interstital  = 2,
    rewarded_video = 3,
    app_open = 4,
}

public enum network_connect_type
{
    none = 0,
    wifi = 1,
    mobile  = 2,
    other = 3,
}


namespace Sonat
{
    public static class SonatAnalyticTracker
    {
        public static string RewardedLogName;
        public static string InterstitialLogName;
        public static bool FirebaseReady { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"> admob, max app lovin or iron source</param>
        /// <param name="adNetwork"></param>
        /// <param name="revenue">revenue in USD not microUSD</param>
        /// <param name="precision">float string</param>
        /// <param name="adType">banner, inter, video</param>
        /// <param name="currencyCode">usd maybe</param>
        public static void LogFirebaseRevenue(string platform, string adNetwork, double revenue, string precision,
            string adType, string currencyCode = "USD")
        {
            if(!FirebaseReady) return;
            
            Parameter[] LTVParameters =
            {
                new Parameter("valuemicros", revenue * 1000000f),
                new Parameter("value", (float) revenue),
                // These values below won’t be used in ROAS recipe.
                // But log for purposes of debugging and future reference.
                new Parameter("currency", currencyCode),
                new Parameter("precision", precision),
                new Parameter("ad_format", adType),
                new Parameter("ad_source", adNetwork),
                new Parameter("ad_platform", platform),
                //new Parameter("adunitid", adUnitId),
                //new Parameter("network", this.rewardedAd.MediationAdapterClassName())
            };
            FirebaseAnalytics.LogEvent("paid_ad_impression", LTVParameters);
        }
        
        public static void LogAppsFlyerAdRevenue(string networkName, double revenue, string adType,string currencyCode= "USD", AppsFlyerAdRevenueMediationNetworkType networkType = AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("af_quantity", "1");
            dic.Add("ad_type", adType);
            dic.Add("ad_unit", adType);
            if (AdType.rewarded_video.ToString() == adType)
            {
                dic.Add("placement", RewardedLogName);
                dic.Add("segment", InterstitialLogName);
            }
            if (AdType.interstital.ToString() == adType)
            {
                dic.Add("placement",RewardedLogName);
                dic.Add("segment", InterstitialLogName);
            }
            Debug.Log($"duong logAdRevenue networkName:{networkName},networkType:{networkType},revenue{revenue}");
            AppsFlyerAdRevenue.logAdRevenue(networkName, networkType, revenue, currencyCode, dic);
        }
    }
    
    public abstract class BaseSonatAnalyticLog
    {
        protected abstract List<Parameter> GetParameters();
        public abstract string EventName { get; }


        private Parameter[] _extra;
        public void SetExtraParameter(Parameter[] extra)
        {
            _extra = extra;
        }
        
        public void Post()
        {
            if (SonatAnalyticTracker.FirebaseReady)
            {
                var listParameters = GetParameters();
                listParameters.Add(new Parameter(nameof(network_connect_type),GetConnectionType().ToString()));
                if (_extra != null)
                {
                    listParameters.AddRange(GetParameters());
                    FirebaseAnalytics.LogEvent(EventName, listParameters.ToArray());
                }
                else
                    FirebaseAnalytics.LogEvent(EventName, listParameters.ToArray());
            }
            else
                Debug.Log("Firebase not ready : SonatAnalyticTracker.FirebaseReady");
        }

        private network_connect_type GetConnectionType()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    return network_connect_type.none;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return network_connect_type.mobile;
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return network_connect_type.wifi;
                    break;
                default:
                    return network_connect_type.other;
            }
        }
        
        public static bool IsInternetConnection()
        {
        
#if UNITY_EDITOR

            return true;
#endif
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                   Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
    
    public abstract class BaseSonatAnalyticLogAppflyer
    {
        protected abstract Dictionary<string, string> GetParameters();
        public abstract string EventName { get; }

        public void Post()
        {
            AppsFlyer.sendEvent(EventName, GetParameters());
        }
    }
}
// ReSharper disable InconsistentNaming

public enum EventNameEnum
{
    app_loading,
    game_scene_level,
    buy_iap,
    show_video,
    video_rewarded,
    show_interstitial,
    level_start,
    remove_ads_success,
    ctr_impression,
    ctr_click,
    level_end,
    level_up,
    use_booster,
    post_score,
    app_spent_time,
    tutorial_begin,
    tutorial_complete,
    screen_view,
    earn_virtual_currency,
    spend_virtual_currency,
    show_rate,
    click_icon_shortcut,
    af_purchase
}

public enum EventNameEnumForAf
{
    af_ad_view,
    video_rewarded,
    show_interstitial,
    show_video,
}

public enum ParameterEnumForAf
{
    af_adrev_ad_type,
    af_revenue,
    af_quantity,
    af_content_type,
    af_content_id,
    af_order_id,
    af_receipt_id,
    af_currency
}

public enum UserPropertyName
{
    last_screen,
    level
}
public enum ParameterEnum
{
    placement,
    af_adrev_ad_type, // for af
    log_level,
    value,
    name,
    level,
    product_click_buy,
    in_app_key,
    store_product_id,
    price,
    format,
    ad_value,
    placement_name,
    fb_instance_id,
    mode,
    use_booster_count,
    play_time,
    move_count,
    score,
    highest_score,
    success,
    lose_cause,
    is_first_play,
    character,
    time_msec,
    screen_name,
    screen_class,
    item_type,
    item_id,
    open_by,
    action,
    shortcut,
    virtual_currency_name
}