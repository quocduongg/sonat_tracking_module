using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if !dummy_log

using AppsFlyerSDK;
using Firebase.Analytics;
namespace Sonat
{
    [Serializable]
    public class SonatLogLevelStart : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.level_start.ToString();

        public string level;
        public string mode = "classic";
        public bool setUserProperty = true;

        public static int UserPropertyLevel
        {
            get => PlayerPrefs.GetInt("user_property_level");
            set => PlayerPrefs.SetInt("user_property_level",value);
        }
    
        public static string UserPropertyMode
        {
            get => PlayerPrefs.GetString("user_property_mode");
            set => PlayerPrefs.SetString("user_property_mode","classic");
        }
        
        protected override List<LogParameter> GetParameters()
        {
            if (setUserProperty)
            {
                FirebaseAnalytics.SetUserProperty(UserPropertyName.level.ToString(), level);
                FirebaseAnalytics.SetUserProperty(UserPropertyName.mode.ToString(), mode);
                try
                {
                    UserPropertyLevel = int.Parse(level);
                    UserPropertyMode = mode;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            
            if (mode == "classic")
                foreach (var i in LevelLog)
                    if (level == i.ToString())
                        if (TryLogIaaIap(i))
                        {
                            parameters.Add(new LogParameter(ParameterEnum.sn_ltv_iaa.ToString(), SonatAnalyticTracker.sn_ltv_iaa));
                            parameters.Add(new LogParameter(ParameterEnum.sn_ltv_iap.ToString(), BasePurchaser.sn_ltv_iap));
                            break;
                        }
            
            return parameters;
        }


        public override void Post(bool logAf = false)
        {
            PostAf = logAf;
            base.Post(logAf);
        
        }

        private static readonly int[] LevelLog = {4, 6, 10, 15, 20, 30, 40, 50};
        private bool TryLogIaaIap(int levelLog)
        {
            if (PlayerPrefs.GetInt("log_iaa_iap_level_" + levelLog) == 0)
            {
                PlayerPrefs.SetInt("log_iaa_iap_level_" + levelLog, 1);
                var dict = new Dictionary<string, string>();
                dict.Add("af_revenue", SonatAnalyticTracker.sn_ltv_iaa.ToString());
                AppsFlyer.sendEvent("iaa_start_level_" + levelLog.ToString("D4"), dict);

                var dict2 = new Dictionary<string, string>();
                dict2.Add("af_revenue", BasePurchaser.sn_ltv_iap.ToString());
                AppsFlyer.sendEvent("iap_start_level_" + levelLog.ToString("D4"), dict2);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class SonatLogLevelEnd : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.level_end.ToString();

        public string level;
        public string mode;
        public int use_booster_count;
        public int play_time;
        public int move_count;
        public bool is_first_play;
        public bool success;

        public string lose_cause;

        // optional
        public int score = int.MinValue;
        public int highest_score = int.MinValue;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            parameters.Add(new LogParameter(ParameterEnum.use_booster_count.ToString(), use_booster_count));
            parameters.Add(new LogParameter(ParameterEnum.play_time.ToString(), play_time));
            parameters.Add(new LogParameter(ParameterEnum.move_count.ToString(), move_count));
            if (score > int.MinValue)
                parameters.Add(new LogParameter(ParameterEnum.score.ToString(), score));
            if (highest_score > int.MinValue)
                parameters.Add(new LogParameter(ParameterEnum.highest_score.ToString(), highest_score));
            parameters.Add(new LogParameter(ParameterEnum.success.ToString(), success ? "true" : "false"));
            parameters.Add(new LogParameter(ParameterEnum.is_first_play.ToString(), is_first_play ? "true" : "false"));
            if(success)
                parameters.Add(new LogParameter(ParameterEnum.lose_cause.ToString(), lose_cause));
            return parameters;

//            var dict = new Dictionary<string, string>();
//            dict.Add(ParameterEnum.level.ToString(),level);
//            dict.Add(ParameterEnum.mode.ToString(), mode);
//            dict.Add(ParameterEnum.use_booster_count.ToString(), use_booster_count.ToString());
//            dict.Add(ParameterEnum.play_time.ToString(), play_time.ToString());
//            dict.Add(ParameterEnum.move_count.ToString(), move_count.ToString());
//            dict.Add(ParameterEnum.score.ToString(), score.ToString());
//            dict.Add(ParameterEnum.highest_score.ToString(), highest_score.ToString());
//            dict.Add(ParameterEnum.success.ToString(), success.ToString());
//            dict.Add(ParameterEnum.is_first_play.ToString(), is_first_play.ToString());
//            dict.Add(ParameterEnum.lose_cause.ToString(), lose_cause);
//            AppsFlyer.sendEvent(EventName, dict);
        }
    }

    [Serializable]
    public class SonatLogLevelUp : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.level_up.ToString();

        public string level;
        public string character;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(character))
                parameters.Add(new LogParameter(ParameterEnum.character.ToString(), character));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogUseBooster : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.use_booster.ToString();

        public string name;
        public string level;
        public string mode;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.name.ToString(), name));
            parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogPostScore : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.post_score.ToString();

        public int score;
        public string level;
        public string mode;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.score.ToString(), score));
            parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogSpentTime : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.app_spent_time.ToString();

        public string placement;
        public long time_msec;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            parameters.Add(new LogParameter(ParameterEnum.time_msec.ToString(), time_msec));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogTutorialBegin : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.tutorial_begin.ToString();

        public string placement;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogTutorialComplete : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.tutorial_complete.ToString();

        public string placement;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            return parameters;
        }
    }

    /// <summary>
    /// "Nhằm để thống kê được thời gian user ở từng giao diện ( screen class nếu ko đặt thì dùng luôn giá trị của screen_name )"
    /// </summary>
    [Serializable]
    public class SonatLogScreenView : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.screen_view.ToString();

        /// <summary>
        /// "Tên của screen trong game. Tuân theo quy tắc viết hoa mỗi  chữ cái của mỗi từ"
        /// </summary>
        public string screen_name;

        /// <summary>
        /// "Tên class của Screen  trong code(có thể dùng chính screen_name)"
        /// </summary>
        public string screen_class;

        public bool saveLastScreen = true;

        private string LastScreen
        {
            get => PlayerPrefs.GetString("last_screen");
            set => PlayerPrefs.SetString("last_screen",value);
        }
        private string LastScreenClass
        {
            get => PlayerPrefs.GetString("last_screen_class");
            set => PlayerPrefs.SetString("last_screen_class",value);
        }
        protected override List<LogParameter> GetParameters()
        {
            if (saveLastScreen)
            {
                LastScreen = screen_name;
                LastScreenClass = screen_class;
            }
            FirebaseAnalytics.SetUserProperty(UserPropertyName.last_screen.ToString(), screen_name);
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.screen_name.ToString(), screen_name));
            if (!string.IsNullOrEmpty(screen_class))
                parameters.Add(new LogParameter(ParameterEnum.screen_class.ToString(), screen_class));
            return parameters;
        }
    }
    
    public class SonatLogLastScreenView : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.screen_view.ToString();

        private string LastScreen
        {
            get => PlayerPrefs.GetString("last_screen");
        }
        private string LastScreenClass
        {
            get => PlayerPrefs.GetString("last_screen_class");
        }
        protected override List<LogParameter> GetParameters()
        {
            FirebaseAnalytics.SetUserProperty(UserPropertyName.last_screen.ToString(), LastScreen);
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.screen_name.ToString(), LastScreen));
            if (!string.IsNullOrEmpty(LastScreenClass))
                parameters.Add(new LogParameter(ParameterEnum.screen_class.ToString(), LastScreenClass));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogEarnVirtualCurrency : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.earn_virtual_currency.ToString();

        public string virtual_currency_name;
        public double value;
        public string placement;
        public string item_type;
        public string item_id;
        
        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.virtual_currency_name.ToString(), virtual_currency_name));
            parameters.Add(new LogParameter(ParameterEnum.value.ToString(), (float)value));
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            parameters.Add(new LogParameter(ParameterEnum.item_type.ToString(), item_type));
            parameters.Add(new LogParameter(ParameterEnum.item_id.ToString(), item_id));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogSpendVirtualCurrency : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.spend_virtual_currency.ToString();

        public string virtual_currency_name;
        public double value;
        public string placement;
        public string item_type;
        public string item_id;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.virtual_currency_name.ToString(), virtual_currency_name));
            parameters.Add(new LogParameter(ParameterEnum.value.ToString(), (float)value));
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            parameters.Add(new LogParameter(ParameterEnum.item_type.ToString(), item_type));
            parameters.Add(new LogParameter(ParameterEnum.item_id.ToString(), item_id));
            return parameters;
        }
    }
    public class SonatLogCrossPromotion : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.cross_pro.ToString();

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogShowInterstitial : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.show_interstitial.ToString();

        public string placement;
        public int level = int.MinValue;
        public string mode;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            if (level > int.MinValue)
                parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogVideoRewarded : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.video_rewarded.ToString();

        public string placement;
        public int level;
        public string mode;
        public string item_type;
        public string item_id;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.placement.ToString(), placement));
            if (level > int.MinValue)
                parameters.Add(new LogParameter(ParameterEnum.level.ToString(), level));
            if (!string.IsNullOrEmpty(mode))
                parameters.Add(new LogParameter(ParameterEnum.mode.ToString(), mode));
            if (!string.IsNullOrEmpty(item_type))
                parameters.Add(new LogParameter(ParameterEnum.item_type.ToString(), item_type));
            if (!string.IsNullOrEmpty(item_id))
                parameters.Add(new LogParameter(ParameterEnum.item_id.ToString(), item_id));
            return parameters;
        }
    }

    
    [Serializable]
    public class SonatPaidAdClick : BaseSonatAnalyticLog
    {
        
      
        public override string EventName =>  EventNameEnum.paid_ad_click.ToString();
        public AdTypeLog ad_format;
        public string ad_placement ;
        public string fb_instance_id;
        
        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.ad_format.ToString(), ad_format.ToString()));
            parameters.Add(new LogParameter(ParameterEnum.ad_placement .ToString(), ad_placement ));
            parameters.Add(new LogParameter(ParameterEnum.fb_instance_id.ToString(), fb_instance_id));
            return parameters;
        }
        
    }
    
    [Serializable]
    public class SonatLogClickShop : BaseSonatAnalyticLog
    {
        public override string EventName =>  EventNameEnum.click_shop.ToString();
        public string action;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.action.ToString(), action.ToString()));
            return parameters;
        }
    }


    
    [Serializable]
    public class SonatLogShowRate : BaseSonatAnalyticLog
    {
        public enum ShowRateOpenBy
        {
            user,
            auto,
        }

        public enum ShowRateAction
        {
            open,
            star_0,
            rate_now,
            other_star,
            star_5,
            star_4,
            close
        }

        private static readonly string[] convert =
        {
            "open",
            "0_star",
            "rate_now",
            "other_star",
            "5_star",
            "4_star",
            "close",
        };

        public override string EventName => EventNameEnum.show_rate.ToString();

        public ShowRateOpenBy open_by;
        public ShowRateAction action;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.open_by.ToString(), open_by.ToString()));
            parameters.Add(new LogParameter(ParameterEnum.action.ToString(), convert[(int) action]));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogClickIconShortcut : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.click_icon_shortcut.ToString();
        public string shortcut;

        protected override List<LogParameter> GetParameters()
        {
            List<LogParameter> parameters = new List<LogParameter>();
            parameters.Add(new LogParameter(ParameterEnum.shortcut.ToString(), shortcut.ToString()));
            return parameters;
        }
    }
    
    [Serializable]
    public class SonatLogInAppPurchaseAppflyer : BaseSonatAnalyticLogAppflyer
    {
        public string af_revenue;
        public int af_quantity = 1;
        public string af_content_type;
        public string af_content_id;
        public string af_order_id;
        public string af_receipt_id;
        public string af_currency;
        public override string EventName => EventNameEnum.af_purchase.ToString();

        protected override Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add(ParameterEnumForAf.af_revenue.ToString(), af_revenue);
            parameters.Add(ParameterEnumForAf.af_quantity.ToString(), af_quantity.ToString());
            parameters.Add(ParameterEnumForAf.af_content_type.ToString(), af_content_type);
            parameters.Add(ParameterEnumForAf.af_content_id.ToString(), af_content_id);
            parameters.Add(ParameterEnumForAf.af_order_id.ToString(), af_order_id);
            parameters.Add(ParameterEnumForAf.af_receipt_id.ToString(), af_receipt_id);
            parameters.Add(ParameterEnumForAf.af_currency.ToString(), af_currency);
            return parameters;
        }
    }

}
#endif
// ReSharper disable InconsistentNaming