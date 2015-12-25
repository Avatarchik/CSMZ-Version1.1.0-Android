//
//  BurstlyManager-Wrapper.m
//  BurstlyTest
//
//  Created by Jesús on 05/04/14.
//  Copyright (c) 2014 Jesús. All rights reserved.
//

#import "BurstlyManager-Wrapper.h"
#import "BurstlyManager.h"

extern "C" {
    
    NSString *CreateNSString(const char* string) {
        return [NSString stringWithUTF8String:(string ? string : "")];
    }
    
    
    void BurstlyManagerWrapper_configureBanner(const char *appId, const char *zoneId, float originX, float originY, float width, float height, int anchor) {
        
        CGRect frame = CGRectMake(originX, originY, width, height);
        [[BurstlyManager sharedManager] configureBannerWithAppId:CreateNSString(appId)
                                                          zoneId:CreateNSString(zoneId)
                                                           frame:frame
                                                          anchor:(BurstlyAnchor)anchor];
        
    }
    
    
    void BurstlyManagerWrapper_showBanner(const char *zoneId)
    {
        [[BurstlyManager sharedManager] showBannerwithZoneId:CreateNSString(zoneId)];
    }
    
    
    void BurstlyManagerWrapper_hideBanner(const char *zoneId)
    {
        [[BurstlyManager sharedManager] hideBannerWithZoneId:CreateNSString(zoneId)];
    }
    
    
    void BurstlyManagerWrapper_configureInterstitial(const char *appId, const char *zoneId, BOOL autocache)
    {
        [[BurstlyManager sharedManager] configureInterstitialWithAppId:CreateNSString(appId)
                                                                zoneId:CreateNSString(zoneId)
                                                             autoCache:autocache];
    }
    
    
    void BurstlyManagerWrapper_showInterstitial(const char *zoneId)
    {
        [[BurstlyManager sharedManager] showInterstitialWithZoneId:CreateNSString(zoneId)];
    }
}