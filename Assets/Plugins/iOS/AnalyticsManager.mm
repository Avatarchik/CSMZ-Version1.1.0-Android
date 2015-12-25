//
//  DMOAnalyticsIOS.cpp
//  Unity-iPhone
//
//  Created by Juan Cucurella on 10/16/13.
//
//

#import "AnalyticsManager.h"
#import "DMOAnalytics.h"

//extern "C" void Crittercism_EnableWithAppID(const char* appID);

@implementation AnalyticsManager
@synthesize cachedDeviceId;

#pragma mark -
#pragma mark Singleton

+ (AnalyticsManager*)sharedAnalyticsManger
{
    static AnalyticsManager *sharedInstance;
    
    if ( !sharedInstance )
    {
        sharedInstance = [[AnalyticsManager alloc] init];
        //        [[DMOAnalytics sharedAnalyticsManager] setDelegate:self];
    }
    
    return sharedInstance;
}


#pragma mark -
#pragma mark Public Methods

- (id) init
{
	self = [super init];
    
	if (self)
	{
        cachedDeviceId = [AnalyticsManager getDeviceId];
        NSLog(@"Initializing cachedDeviceId = %@", cachedDeviceId);
    }
    
    return self;
}

- (void)startCrittercismWithAppID:(const char*)appID
{
//    Crittercism_EnableWithAppID(appID);
    NSLog(@"Init Crittercism");
}

- (void)startAnalyticsWithAppID:(NSString *)appKey appSecret:(NSString *)appSecret
{
    NSLog(@"Called AnalyticsManager->startAnalyticsWithAppID with appKey %@ and secret %@", appKey, appSecret);
    DMOAnalytics* analytics = [[DMOAnalytics alloc] initWithAppKey:appKey secret:appSecret];
    analytics.debugLogging = YES;
    
    if (analytics == nil) {
        NSLog(@"FAILED creating analytics DMOAnalytics.");
    }
    else {
        //        NSString* configuration = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"Configuration"];
        //        analyticsController.debugLogging = !([configuration isEqualToString:@"Distribution"]);
        analytics.restrictedTracking = NO;
        NSLog(@"Initialized DMOAnalytics succesfully!");
    }
}

+ (NSString *) getDeviceId
{
    NSString* deviceId = [[NSUserDefaults standardUserDefaults] stringForKey:@"AppUUID"];
    
    if ( !deviceId ) {
        deviceId = @"deviceId";
    }
    
    if ([deviceId isEqualToString:@"deviceId"]) {
        CFUUIDRef uuid;
        CFStringRef uuidStr;
        
        uuid = CFUUIDCreate(NULL);
        if (uuid != NULL) {
            uuidStr = CFUUIDCreateString(NULL, uuid);
            if (uuidStr != NULL) {
                deviceId = [NSString stringWithFormat:@"%@", uuidStr];
                CFRelease(uuidStr);
            }
            CFRelease(uuid);
        }
        
        [[NSUserDefaults standardUserDefaults] setValue:deviceId forKey:@"AppUUID"];
    }
    
    return deviceId;
}

- (void)logEventUserInfoWithDeviceId:(NSString *)deviceId userId:(NSString *)userId userProfile:(NSDictionary *)userProfile
{
    if (deviceId == nil || deviceId.length == 0) {
        deviceId = self.cachedDeviceId;
    }
    if (userId == nil || userId.length == 0) {
        userId = self.cachedDeviceId;
    }
    
    // Create an empty dictionary if this param is nil.
    if (userProfile == nil) {
        userProfile = [NSDictionary dictionary];
    }
    
    NSDictionary *dict = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:userId, @"Disney", nil]
                                                     forKeys:[NSArray arrayWithObjects:@"user_id", @"user_id_domain", nil]];
    //NSDictionary *dict = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:deviceId, userId, @"Disney", userProfile, nil]
    //                                                 forKeys:[NSArray arrayWithObjects:@"device_id", @"user_id", @"user_id_domain", @"user_profile", nil]];
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"user_info" withContext:dict];
}

- (void)logEventPlayerInfoWithDeviceId:(NSString *)deviceId playerId:(NSString *)playerId
{
    if (deviceId == nil || deviceId.length == 0) {
        deviceId = self.cachedDeviceId;
    }
    if (playerId == nil || playerId.length == 0) {
        playerId = self.cachedDeviceId;
    }
    
    NSDictionary *dict = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:playerId, @"Disney", nil]
                                                     forKeys:[NSArray arrayWithObjects:@"player_id", @"user_id_domain", nil]];
    //NSDictionary *dict = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:deviceId, playerId, @"Disney", nil]
    //                                                 forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"user_id_domain", nil]];
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"player_info" withContext:dict];
}

- (void)logEventWithName:(NSString *)eventName {
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:eventName];
}

- (void)logEventGameActionWithContext:(NSString *)context action:(NSString *)action type:(NSString *) type message:(NSString *)message level:(int)level subtype:(NSString *)subtype
{
    NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
                                 [NSArray arrayWithObjects:self.cachedDeviceId, context, action, nil]
                                                                   forKeys:[NSArray arrayWithObjects:@"player_id", @"context", @"action", nil]];
    //NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
    //                             [NSArray arrayWithObjects:self.cachedDeviceId, self.cachedDeviceId, context, action, nil]
    //                                                               forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"context", @"action", nil]];
    if (type != nil && type.length > 0) {
        [dict setValue:type forKey:@"type"];
    }
    if (message != nil && message.length > 0) {
        [dict setValue:message forKey:@"message"];
    }
    if (level > -1) {
        [dict setValue:[NSNumber numberWithInt:level] forKey:@"level"];
    }
    if (subtype != nil && subtype.length > 0) {
        [dict setValue:subtype forKey:@"subtype"];
    }
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"game_action" withContext:dict];
}

- (void)logEventNavigationActionWithButton:(NSString *)button_pressed fromLocation:(NSString *)from_location toLocation:(NSString *)toLocation
                                 targetUrl:(NSString *)target_url
{
    NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
                                 [NSArray arrayWithObjects:self.cachedDeviceId, button_pressed, from_location, target_url, nil]
                                                                   forKeys:[NSArray arrayWithObjects:@"player_id", @"button_pressed", @"from_location", @"target_url", nil]];
    //NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
    //                             [NSArray arrayWithObjects:self.cachedDeviceId, self.cachedDeviceId, button_pressed, from_location, target_url, nil]
    //                                                               forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"button_pressed", @"from_location", @"target_url", nil]];
    
    if (toLocation != nil && toLocation.length > 0) {
        [dict setValue:toLocation forKey:@"to_location"];
    }
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"navigation_action" withContext:dict];
}

- (void)logEventTimingActionWithLocation:(NSString *)location elapsedTime:(float)elapsed_time
{
    NSDictionary *dict = [NSDictionary dictionaryWithObjects:
                          [NSArray arrayWithObjects:self.cachedDeviceId, location, [NSNumber numberWithFloat:elapsed_time], nil]
                                                     forKeys:[NSArray arrayWithObjects:@"player_id", @"location", @"elapsed_time", nil]];
    
    //NSDictionary *dict = [NSDictionary dictionaryWithObjects:
    //                      [NSArray arrayWithObjects:self.cachedDeviceId, self.cachedDeviceId, location, [NSNumber numberWithFloat:elapsed_time], nil]
    //                                                 forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"location", @"elapsed_time", nil]];
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"timing" withContext:dict];
}


- (void)logEventPageViewWithLocation:(NSString *)location pageUrl:(NSString *)pageUrl message:(NSString *)message
{
    NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
                                 [NSArray arrayWithObjects:self.cachedDeviceId, location, nil]
                                                                   forKeys:[NSArray arrayWithObjects:@"player_id", @"location", nil]];
    //NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:
    //                             [NSArray arrayWithObjects:self.cachedDeviceId, self.cachedDeviceId, location, nil]
    //                                                               forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"location", nil]];
    
    if (pageUrl != nil && pageUrl.length > 0) {
        [dict setValue:pageUrl forKey:@"page_url"];
    }
    if (message != nil && message.length > 0) {
        [dict setValue:message forKey:@"message"];
    }
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"page_view" withContext:dict];
}

- (void)logEventPaymentActionWithCurrency:(NSString *)currency locale:(NSString *)locale amountPaid:(float)amountPaid
                                     item:(NSDictionary *)item type:(NSString *)type subtype:(NSString *)subtype context:(NSString *)context
                                    level:(int)level
{
    NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:[NSArray arrayWithObjects:self.cachedDeviceId, currency,
                                                                            locale, [NSNumber numberWithFloat:amountPaid], item, type, nil]
                                                                   forKeys:[NSArray arrayWithObjects:@"player_id", @"currency", @"locale",
                                                                            @"amount_paid", @"item", @"type", nil]];
    //NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjects:[NSArray arrayWithObjects:self.cachedDeviceId, self.cachedDeviceId, currency,
    //                                                                        locale, [NSNumber numberWithFloat:amountPaid], item, type, nil]
    //                                                               forKeys:[NSArray arrayWithObjects:@"device_id", @"player_id", @"currency", @"locale",
    //                                                                        @"amount_paid", @"item", @"type", nil]];
    
    if (subtype != nil && subtype.length > 0) {
        [dict setValue:subtype forKey:@"subtype"];
    }
    if (context != nil && context.length > 0) {
        [dict setValue:context forKey:@"context"];
    }
    if (level >= 0) {
        [dict setValue:[NSNumber numberWithInt:level] forKey:@"level"];
    }
    
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"payment_action" withContext:dict];
}

//--------------------------
- (void)logPlayerInformationWithID:(NSString *)playerID currentLevel:(int)level powerup:(int)powerup logged:(BOOL)isLogged {
    NSDictionary *infoDict = @{@"player_id": playerID,
                               @"current_level": [NSNumber numberWithInt:level],
                               @"current_power_balance": [NSNumber numberWithInt:powerup],
                               @"isLogged":[NSNumber numberWithBool:isLogged]
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"player_info" withContext:infoDict];
}

- (void)logInAppPurchaseForUserID:(NSString *)playerID currencyType:(NSString *)currency amount:(float)amount purchaseLocalizacion:(NSString *)context {
    NSDictionary *infoDict = @{@"player_id": playerID,
                               @"currency_type": currency,
                               @"amount": [NSNumber numberWithFloat:amount],
                               @"purchase_localization": context
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"payment_action" withContext:infoDict];
}

- (void)logPowerUpUsageForUserID:(NSString *)playerID inLevel:(int)level withMovesRemainingInLevel:(int)movesRemaining {
    NSDictionary *infoDict = @{@"player_id": playerID,
                               @"action": @"power-up usage",
                               @"level": [NSNumber numberWithInt:level],
                               @"moves_remaining": [NSNumber numberWithInt:movesRemaining]
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"game_action" withContext:infoDict];
}

- (void)logGameStartedStatisticsForUserID:(NSString *)playerID inLevel:(int)level typeOfLevel:(NSString *)type parametersOfLevel:(NSString *)parameters withCharacter:(NSString *)characterName isLevelRepeated:(bool)isRepeated {
    NSDictionary *infoDict = @{@"player_id": playerID,
                               @"action": @"game started statistics",
                               @"level": [NSNumber numberWithInt:level],
                               @"level_type": type,
                               @"level_parameters": parameters,
                               @"characted_used": characterName,
                               @"is_repeated": [NSNumber numberWithBool:isRepeated]
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"game_action" withContext:infoDict];
}

- (void)logGameEndedStatisticsForUserID:(NSString *)playerID inLevel:(int)level isLevelCompleted:(bool)completed typeOfLevel:(NSString *)type parametersOfLevelRemaining:(NSString *)parameters withCharacter:(NSString *)characterName wasCompletedOnFirstAttempt:(bool)wasCompleted {
    NSDictionary *infoDict = @{@"player_id": playerID,
                               @"action": @"game ended statistics",
                               @"level": [NSNumber numberWithInt:level],
                               @"isCompleted": [NSNumber numberWithBool:completed],
                               @"level_type": type,
                               @"level_parameters_remaining": parameters,
                               @"characted_used": characterName,
                               @"was_completed_on_first_attempt": [NSNumber numberWithBool:wasCompleted]
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"game_action" withContext:infoDict];
}

- (void)logPageView:(NSString *)screenName forUserID:(NSString *)playerID {
    NSDictionary *infoDict = @{@"screen_name": screenName,
                               @"player_id": playerID
                               };
    [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:@"page_view" withContext:infoDict];
}

@end


