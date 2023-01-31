
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

public enum ParameterValue
{
    IntersAds,
    RewardedAds,
    ad_break,
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
    virtual_currency_name,
}

public enum AdsPlatform
{
    applovinmax,
    googleadmob,
    ironsource
}

public enum AdTypeLog
{
    banner,
    interstitial,
    native,
    video,
    rewarded_video,
    rewarded,
    mraid,
    mrec,
    offer_wall,
    playable,
    more_apps,
    video_interstitial,
    medium,
    custom,
    banner_interstitial,
    app_open,
    other,
    native_banner
}

public enum network_connect_type
{
    none = 0,
    wifi = 1,
    mobile = 2,
    other = 3,
}
