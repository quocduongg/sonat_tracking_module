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

        private static readonly Dictionary<string,string>  mediationDict = new Dictionary<string, string>()
        {
            {"googleadmanager","googleadmanager"},
            {"admob","admob"},
            {"applovin","applovinmax"},
            {"max","applovinmax"},
            {"fyber","fyber"},
            {"appodeal","appodeal"},
            {"inmobi","inmobi"},
            {"vungle","vungle"},
            {"admost","admost"},
            {"topon","topon"},
            {"tradplus","tradplus"},
            {"chartboost","chartboost"},
            {"facebook","facebook"},
            {"meta","facebook"},
            {"mintegral","mintegral"},
            {"mtg","mintegral"},
            {"ironsource","ironsource"},
            {"unity","unity"},
            {"pangle","pangle"},
            {"bytedance","bytedance"},
            {"bidmachine","bidmachine"},
            {"liftoff","liftoff"},
            {"mytarget","mytarget"},
            {"smaato","smaato"},
            {"tapjoy","tapjoy"},
            {"verve","verve"},
            {"yahoo","yahoo"},
            {"yandex","yandex"},
            {"google","admob"},
        };

        public static string FindNetworkName(string splitLower)
        {
            foreach (var keyValuePair in mediationDict)
                if (splitLower.Contains(keyValuePair.Key))
                    return keyValuePair.Value;

            return null;
        }


        public static string GetNetworkName(string fullNetworkName, AdsPlatform platform)
        {
            if (string.IsNullOrEmpty(fullNetworkName))
                return GetDefault(platform);

            var split = fullNetworkName.Split('.');
            var lower = split[split.Length-1].ToLower();
            return FindNetworkName(lower) ?? GetDefault(platform);
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
            AdTypeLog adType,string fb_instance_id,string placement, string currencyCode = "USD")
        {
            LogFirebaseRevenue(platform,adapter,revenue,precision,adType.ToString(),fb_instance_id,placement,currencyCode);
            LogAppsFlyerAdRevenue(platform,adapter,revenue,adType.ToString(),fb_instance_id,placement,currencyCode);
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
            string adType, string fb_instance_id,string placement, string currencyCode = "USD")
        {
            if (!FirebaseReady) return;

            Parameter[] parameters =
            {
                new Parameter("valuemicros", revenue * 1000000f),
                new Parameter("value", (float) revenue),
                // These values below won’t be used in ROAS recipe.
                // But log for purposes of debugging and future reference.
                new Parameter("currency", currencyCode),
                new Parameter("precision", precision),
                new Parameter(ParameterEnum.ad_format.ToString(), adType),
                new Parameter(ParameterEnum.fb_instance_id.ToString(), fb_instance_id),
                new Parameter(ParameterEnum.ad_placement .ToString(), placement),
                new Parameter(ParameterEnum.ad_source.ToString(), SonatTrackingHelper.GetNetworkName(adapter, platform)),
                new Parameter(ParameterEnum.ad_platform.ToString(), platform.ToString()),
            };
            FirebaseAnalytics.LogEvent(EventNameEnum.paid_ad_impression.ToString(), parameters);
        }

        public static void LogAppsFlyerAdRevenue(AdsPlatform platform,string adapter, double revenue, string adType,string firebase_instance_id,string placement, 
            string currencyCode = "USD")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("af_quantity", "1");
            dic.Add("ad_type", adType);
            dic.Add("ad_unit", adType);
            dic.Add("placement", placement);
            dic.Add("segment", placement);

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
        

        public BaseSonatAnalyticLog SetExtraParameter(LogParameter[] extra)
        {
            _extra = extra;
            return this;
        }

        public void Post(bool logAf = false)
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

            if (logAf)
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