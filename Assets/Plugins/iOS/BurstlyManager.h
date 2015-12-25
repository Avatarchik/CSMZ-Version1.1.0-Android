//
//  BurstlyManager.h
//  BurstlyTest
//
//  Created by Jesús on 05/04/14.
//  Copyright (c) 2014 Jesús. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "BurstlyBannerAdView.h"

@interface BurstlyManager : NSObject

+ (BurstlyManager*)sharedManager;

- (void)configureBannerWithAppId:(NSString*)appId zoneId:(NSString*)zoneId frame:(CGRect)bannerFrame anchor:(BurstlyAnchor)anchor;

- (void)showBannerwithZoneId:(NSString*)zoneId;
- (void)hideBannerWithZoneId:(NSString*)zoneId;

- (void)configureInterstitialWithAppId:(NSString*)appId zoneId:(NSString*)zoneId autoCache:(BOOL)autocache;
- (void)showInterstitialWithZoneId:(NSString*)zoneId;

@end
