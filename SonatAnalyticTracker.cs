using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if !dummy_log
using AppsFlyerSDK;
using Firebase.Analytics;
// ReSharper disable InconsistentNaming

namespace Sonat
{
  
    
    public static class SonatTrackingHelper
    {
        private static string GetDefault(AdsPlatform platform)
        {
            switch (platform)
            {
                case AdsPlatform.applovinmax:
                    return platform.ToString();
                case AdsPlatform.googleadmob:
                    return "admob";
                case AdsPlatform.ironsource:
                    return platform.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }


        public static string GetNetworkName(string fullNetworkName, AdsPlatform platform)
        {
            if (string.IsNullOrEmpty(fullNetworkName))
                return GetDefault(platform);

            var lower = fullNetworkName.ToLower();
            if (lower.Contains("admob"))
                return "admob";
            if (lower.Contains("max"))
                return "applovinmax";
            if (lower.Contains("fyber"))
                return "fyber";
            if (lower.Contains("appodeal"))
                return "appodeal";
            if (lower.Contains("inmobi"))
                return "inmobi";
            if (lower.Contains("vungle"))
                return "vungle";
            if (lower.Contains("admost"))
                return "admost";
            if (lower.Contains("topon"))
                return "topon";
            if (lower.Contains("tradplus"))
                return "tradplus";
            if (lower.Contains("chartboost"))
                return "chartboost";
            if (lower.Contains("appodeal"))
                return "appodeal";
            if (lower.Contains("google"))
                return "googleadmanager";
            if (lower.Contains("google"))
                return "googlead";
            if (lower.Contains("facebook") || lower.Contains("meta"))
                return "facebook";
            if (lower.Contains("applovin") || lower.Contains("max"))
                return "applovin";
            if (lower.Contains("ironsource"))
                return "ironsource";
            if (lower.Contains("unity"))
                return "unity";
            if (lower.Contains("mintegral") || lower.Contains("mtg"))
                return "mtg";

            return GetDefault(platform);
        }
    }

    public static class SonatAnalyticTracker
    {
        public static string RewardedLogName;
        public static string InterstitialLogName;
        public static bool FirebaseReady { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"> admob, max app lovin or iron source</param>
        /// <param name="adapter"></param>
        /// <param name="revenue">revenue in USD not microUSD</param>
        /// <param name="precision">float string</param>
        /// <param name="adType">banner, inter, video</param>
        /// <param name="currencyCode">usd maybe</param>
        public static void LogRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            AdTypeLog adType, string currencyCode = "USD")
        {
            LogFirebaseRevenue(platform,adapter,revenue,precision,adType.ToString(),currencyCode);
            LogAppsFlyerAdRevenue(platform,adapter,revenue,adType.ToString(),currencyCode);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"> admob, max app lovin or iron source</param>
        /// <param name="adapter"></param>
        /// <param name="revenue">revenue in USD not microUSD</param>
        /// <param name="precision">float string</param>
        /// <param name="adType">banner, inter, video</param>
        /// <param name="currencyCode">usd maybe</param>
        public static void LogFirebaseRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            string adType, string currencyCode = "USD")
        {
            if (!FirebaseReady) return;

            Parameter[] LTVParameters =
            {
                new Parameter("valuemicros", revenue * 1000000f),
                new Parameter("value", (float) revenue),
                // These values below won’t be used in ROAS recipe.
                // But log for purposes of debugging and future reference.
                new Parameter("currency", currencyCode),
                new Parameter("precision", precision),
                new Parameter("ad_format", adType),
                new Parameter("ad_source", SonatTrackingHelper.GetNetworkName(adapter, platform)),
                new Parameter("ad_platform", platform.ToString()),
                //new Parameter("adunitid", adUnitId),
                //new Parameter("network", this.rewardedAd.MediationAdapterClassName())
            };
            FirebaseAnalytics.LogEvent("paid_ad_impression", LTVParameters);
        }

        public static void LogAppsFlyerAdRevenue(AdsPlatform platform,string adapter, double revenue, string adType,
            string currencyCode = "USD")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("af_quantity", "1");
            dic.Add("ad_type", adType);
            dic.Add("ad_unit", adType);
            if (AdTypeLog.rewarded_video.ToString() == adType)
            {
                dic.Add("placement", RewardedLogName);
                dic.Add("segment", RewardedLogName);
            }

            if (AdTypeLog.interstitial.ToString() == adType)
            {
                dic.Add("placement", InterstitialLogName);
                dic.Add("segment", InterstitialLogName);
            }

            Debug.Log($"duong logAdRevenue adapter:{adapter},platform:{platform},revenue{revenue}");
            AppsFlyerAdRevenue.logAdRevenue(SonatTrackingHelper.GetNetworkName(adapter, platform),
                GetNetworkType(platform), revenue, currencyCode, dic);
        }

        private static AppsFlyerAdRevenueMediationNetworkType GetNetworkType(AdsPlatform platform)
        {
            switch (platform)
            {
                case AdsPlatform.applovinmax:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax;
                case AdsPlatform.googleadmob:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob;
                case AdsPlatform.ironsource:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
    }

    public abstract class BaseSonatAnalyticLog
    {
        protected abstract List<LogParameter> GetParameters();
        public abstract string EventName { get; }


        private LogParameter[] _extra;

        private bool _logAf;

        public void SetLogAppflyer()
        {
            _logAf = true;
        }
        

        public BaseSonatAnalyticLog SetExtraParameter(LogParameter[] extra)
        {
            _extra = extra;
            return this;
        }

        public void Post()
        {
            var listParameters = GetParameters();
            if (SonatAnalyticTracker.FirebaseReady)
            {
                listParameters.Add(new LogParameter(nameof(network_connect_type), GetConnectionType().ToString()));
                if (_extra != null)
                {
                    listParameters.AddRange(GetParameters());
                    FirebaseAnalytics.LogEvent(EventName, listParameters.Select(x => x.Param).ToArray());
                }
                else
                    FirebaseAnalytics.LogEvent(EventName, listParameters.Select(x => x.Param).ToArray());
            }
            else
                Debug.Log("Firebase not ready : SonatAnalyticTracker.FirebaseReady");

            if (_logAf)
            {
                var dict = new Dictionary<string, string>();
                foreach (var parameter in listParameters)
                    dict.Add(parameter.stringKey, parameter.stringValue);
                AppsFlyer.sendEvent(EventName, dict);
            }
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
#endif