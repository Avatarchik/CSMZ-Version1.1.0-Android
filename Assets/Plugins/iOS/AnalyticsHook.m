//
//  AnalyticsBinding.m
//  Unity-iPhone
//
//  Created by Juan Cucurella on 10/16/13.
//
//

#import "AnalyticsManager.h"

void Analytics_Init(const char* appID, const char* appSecret)
{
    NSLog(@"Analytics Binding Init");
    [[AnalyticsManager sharedAnalyticsManger] startAnalyticsWithAppID:GetStringParam(appID) appSecret:GetStringParam(appSecret)];
}

void Analytics_LogEventUserInfo()
{
    //Note: Will internally use the device id for the "userId" also.
    [[AnalyticsManager sharedAnalyticsManger] logEventUserInfoWithDeviceId:nil
                                                                    userId:nil
                                                               userProfile:nil];
}

void Analytics_LogEventPlayerInfo()
{
    //Note: Will internally use the device id for the "playerId" also.
    [[AnalyticsManager sharedAnalyticsManger] logEventPlayerInfoWithDeviceId:nil
                                                                    playerId:nil];
}

void Analytics_LogEventGameAction(const char* context, const char* action, const char* type, const char* message, int level, const char* subtype)
{
    [[AnalyticsManager sharedAnalyticsManger] logEventGameActionWithContext:GetStringParam(context)
                                                                     action:GetStringParam(action)
                                                                       type:GetStringParam(type)
                                                                    message:GetStringParam(message)
                                                                      level:level
                                                                    subtype:GetStringParam(subtype)];
}

void Analytics_LogEventNavigationAction(const char* button_pressed, const char* from_location, const char* to_location, const char* target_url)
{
    [[AnalyticsManager sharedAnalyticsManger] logEventNavigationActionWithButton:GetStringParam(button_pressed)
                                                                    fromLocation:GetStringParam(from_location)
                                                                      toLocation:GetStringParam(to_location)
                                                                       targetUrl:GetStringParam(target_url)];
}

void Analytics_LogEventTimingAction(const char* location, float elapsed_time)
{
    [[AnalyticsManager sharedAnalyticsManger] logEventTimingActionWithLocation:GetStringParam(location) elapsedTime:elapsed_time];
}

void Analytics_LogEventPageView(const char* location, const char* pageUrl, const char* message)
{
    [[AnalyticsManager sharedAnalyticsManger] logEventPageViewWithLocation:GetStringParam(location)
                                                                   pageUrl:GetStringParam(pageUrl)
                                                                   message:GetStringParam(message)];
}

void Analytics_LogEventPaymentAction(const char* currency, const char* locale, float amountPaid, const char* itemId, int itemCount,
                                     const char* type, const char* subtype, const char *context, int level)
{
    NSDictionary *itemInfo = @{@"item_id": GetStringParam(itemId),
                               @"item_count": [NSNumber numberWithInt:itemCount],
                               };
    
    [[AnalyticsManager sharedAnalyticsManger] logEventPaymentActionWithCurrency:GetStringParam(currency)
                                                                         locale:GetStringParam(locale)
                                                                     amountPaid:amountPaid
                                                                           item:itemInfo
                                                                           type:GetStringParam(type)
                                                                        subtype:GetStringParam(subtype)
                                                                        context:GetStringParam(context)
                                                                          level:level];
}


void Analytics_LogEvent(const char* eventName)
{
    NSLog(@"Analytics Log Event");
    [[AnalyticsManager sharedAnalyticsManger] logEventWithName:GetStringParam(eventName)];
}

//void Analytics_LogPlayerInformation(const char* playerID, int currentLevel, int powerUp, bool isLogged)
//{
//    NSLog(@"Analytics Log Player Info");
//    [[AnalyticsManager sharedAnalyticsManger] logPlayerInformationWithID:GetStringParam(playerID) currentLevel:currentLevel powerup:powerUp logged:isLogged];
//}
//
//void Analytics_LogInAppPurchase(const char* playerID, const char* currencyType, float amount, const char* localizationContext)
//{
//    NSLog(@"Analytics Log InApp Purchase");
//    [[AnalyticsManager sharedAnalyticsManger] logInAppPurchaseForUserID:GetStringParam(playerID) currencyType:GetStringParam(currencyType) amount:amount purchaseLocalizacion:GetStringParam(localizationContext)];
//}
//
//void Analytics_LogPowerUpUsage(const char* playerID, int level, int movesRemainingInLevel)
//{
//    NSLog(@"Analytics Log PowerUp Usage");
//    [[AnalyticsManager sharedAnalyticsManger] logPowerUpUsageForUserID:GetStringParam(playerID) inLevel:level withMovesRemainingInLevel:movesRemainingInLevel];
//}
//
//void Analytics_LogGameStartedStatistics(const char* playerID, int level, const char* typeOfLevel, const char* parametersOfLevel, const char* characterUsed, bool isLevelRepeated)
//{
//    NSLog(@"Analytics Log Game Started Statistics");
//    [[AnalyticsManager sharedAnalyticsManger] logGameStartedStatisticsForUserID:GetStringParam(playerID) inLevel:level typeOfLevel:GetStringParam(typeOfLevel) parametersOfLevel:GetStringParam(parametersOfLevel) withCharacter:GetStringParam(characterUsed) isLevelRepeated:isLevelRepeated];
//}
//
//void Analytics_LogGameEndedStatistics(const char* playerID, int level, bool isLevelCompleted, const char* typeOfLevel, const char* parametersOfLevelRemaining, const char* characterUsed, bool wasLevelCompletedOnFirstAttemp)
//{
//    NSLog(@"Analytics Log Game Ended Statistics");
//    [[AnalyticsManager sharedAnalyticsManger] logGameEndedStatisticsForUserID:GetStringParam(playerID) inLevel:level isLevelCompleted:isLevelCompleted typeOfLevel:GetStringParam(typeOfLevel) parametersOfLevelRemaining:GetStringParam(parametersOfLevelRemaining) withCharacter:GetStringParam(characterUsed) wasCompletedOnFirstAttempt:wasLevelCompletedOnFirstAttemp];
//}
//
//void Analytics_LogPageView(const char*playerID, const char* screenName)
//{
//    NSLog(@"Analytics Log Page View");
//    [[AnalyticsManager sharedAnalyticsManger] logPageView:GetStringParam(screenName) forUserID:GetStringParam(playerID)];
//}



