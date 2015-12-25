//
//  DMOAnalyticsIOS.h
//  Unity-iPhone
//
//  Created by Juan Cucurella on 10/16/13.
//
//

#import <UIKit/UIKit.h>
#import "DMOAnalytics.h"

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

@interface AnalyticsManager : NSObject <DMOAnalyticsDelegate>
{
    NSString* cachedDeviceId;
}
@property (nonatomic, readonly) NSString *cachedDeviceId;

+ (AnalyticsManager*)sharedAnalyticsManger;

- (void)startAnalyticsWithAppID:(NSString *)appKey appSecret:(NSString *)appSecret;

- (void)startCrittercismWithAppID:(const char*)appID;

- (void)logEventUserInfoWithDeviceId:(NSString *)deviceId userId:(NSString *)userId userProfile:(NSDictionary *)userProfile;

- (void)logEventPlayerInfoWithDeviceId:(NSString *)deviceId playerId:(NSString *)playerId;

- (void)logEventWithName:(NSString *)eventName;

- (void)logEventGameActionWithContext:(NSString *)context action:(NSString *)action type:(NSString *) type message:(NSString *)message level:(int)level subtype:(NSString *)subtype;

- (void)logEventNavigationActionWithButton:(NSString *)button_pressed fromLocation:(NSString *)from_location toLocation:(NSString *)toLocation
                                 targetUrl:(NSString *)target_url;

- (void)logEventTimingActionWithLocation:(NSString *)location elapsedTime:(float)elapsed_time;

- (void)logEventPageViewWithLocation:(NSString *)location pageUrl:(NSString *)pageUrl message:(NSString *)message;

- (void)logEventPaymentActionWithCurrency:(NSString *)currency locale:(NSString *)locale amountPaid:(float)amountPaid
                                     item:(NSDictionary *)item type:(NSString *)type subtype:(NSString *)subtype context:(NSString *)context
                                    level:(int)level;


- (void)logPlayerInformationWithID:(NSString *)playerID currentLevel:(int)level powerup:(int)powerup logged:(BOOL)isLogged;

//- (void)logInAppPurchaseForUserID:(NSString *)playerID currencyType:(NSString *)currency amount:(float)amount purchaseLocalizacion:(NSString *)context;
//- (void)logPowerUpUsageForUserID:(NSString *)playerID inLevel:(int)level withMovesRemainingInLevel:(int)movesRemaining;
//- (void)logGameStartedStatisticsForUserID:(NSString *)playerID inLevel:(int)level typeOfLevel:(NSString *)type parametersOfLevel:(NSString *)parameters withCharacter:(NSString *)characterName isLevelRepeated:(bool)isRepeated;
//- (void)logGameEndedStatisticsForUserID:(NSString *)playerID inLevel:(int)level isLevelCompleted:(bool)completed typeOfLevel:(NSString *)type parametersOfLevelRemaining:(NSString *)parameters withCharacter:(NSString *)characterName wasCompletedOnFirstAttempt:(bool)wasCompleted;
//- (void)logPageView:(NSString *)screenName forUserID:(NSString *)playerID;


@end

