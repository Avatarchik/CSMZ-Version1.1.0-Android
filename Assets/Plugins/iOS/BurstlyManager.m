//
//  BurstlyManager.m
//  BurstlyTest
//
//  Created by Jesús on 05/04/14.
//  Copyright (c) 2014 Jesús. All rights reserved.
//

#import "UnityAppController.h"
#import "BurstlyManager.h"
#import "BurstlyInterstitial.h"

@interface BurstlyManager () <BurstlyInterstitialDelegate>

@property (nonatomic, strong) NSMutableDictionary *shownBanners;
@property (nonatomic, strong) NSMutableDictionary *shownInterstitials;

@end

@implementation BurstlyManager

+ (BurstlyManager*)sharedManager
{
    static dispatch_once_t onceToken;
    static BurstlyManager *sharedInstance;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[BurstlyManager alloc] init];
    });
    
    return sharedInstance;
}


- (id)init
{
    self = [super init];
    if (self) {
        self.shownBanners = [NSMutableDictionary dictionary];
        self.shownInterstitials = [NSMutableDictionary dictionary];
    }
    return self;
}


#pragma mark Public

- (void)configureBannerWithAppId:(NSString*)appId zoneId:(NSString*)zoneId frame:(CGRect)bannerFrame anchor:(BurstlyAnchor)anchor
{
    BurstlyBannerAdView *vBanner = [_shownBanners objectForKey:zoneId];
    
    if (!vBanner) {
        vBanner = [[BurstlyBannerAdView alloc] initWithAppId:appId
                                                      zoneId:zoneId
                                                       frame:bannerFrame
                                                      anchor:anchor
                                          rootViewController:[self _viewControllerForBannerPresentation]
                                                    delegate:nil];
        [_shownBanners setObject:vBanner forKey:zoneId];
        
        [vBanner.layer setBorderColor:[UIColor redColor].CGColor];
        [vBanner.layer setBorderWidth:2.f];
        
    } else {
        vBanner.frame = bannerFrame;
        vBanner.anchor = anchor;
    }
    
    [vBanner cacheAd];
}


- (void)showBannerwithZoneId:(NSString *)zoneId
{
    BurstlyBannerAdView *vBanner = [_shownBanners objectForKey:zoneId];
    if (vBanner && !vBanner.superview) {
        [[self _viewControllerForBannerPresentation].view addSubview:vBanner];
        [vBanner showAd];
    }
}

- (void)hideBannerWithZoneId:(NSString *)zoneId
{
    BurstlyBannerAdView *vBanner = [_shownBanners objectForKey:zoneId];
    if (vBanner && vBanner.superview) {
        [vBanner removeFromSuperview];
        [vBanner cacheAd];
    }
}


- (void)configureInterstitialWithAppId:(NSString *)appId zoneId:(NSString *)zoneId autoCache:(BOOL)autocache
{
    BurstlyInterstitial *vInterstitial = [_shownInterstitials objectForKey:zoneId];
    
    if (!vInterstitial) {
        vInterstitial = [[BurstlyInterstitial alloc] initAppId:appId
                                                        zoneId:zoneId
                                                      delegate:self
                                           useAutomaticCaching:autocache];
        [_shownInterstitials setObject:vInterstitial forKey:zoneId];
    } else {
        vInterstitial.useAutomaticCaching = autocache;
    }
}


- (void)showInterstitialWithZoneId:(NSString *)zoneId
{
    BurstlyInterstitial *vInterstitial = [_shownInterstitials objectForKey:zoneId];
    [vInterstitial showAd];
}

#pragma mark Private

- (UIViewController*)_viewControllerForBannerPresentation
{
    return ((UnityAppController*)[UIApplication sharedApplication].delegate).rootViewController;
}

#pragma mark BurstlyInterstitialDelegate

- (UIViewController *)viewControllerForModalPresentation:(BurstlyInterstitial *)interstitial
{
    return [self _viewControllerForBannerPresentation];
}


@end
