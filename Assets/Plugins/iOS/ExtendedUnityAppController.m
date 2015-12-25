//
//  DMOAnalitics.m
//  Unity-iPhone
//
//  Created by Juan Cucurella on 10/18/13.
//
//
//#import "UAirship.h"
//#import "UAConfig.h"
//#import "UAPush.h"

#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import "ExtendedUnityAppController.h"
#import "CSComScore.h"


#define IMPL_APP_CONTROLLER_SUBCLASS(ExtendedUnityAppController)
@interface ExtendedUnityAppController(OverrideAppDelegate)
{
}
+(void)load;
@end
@implementation ExtendedUnityAppController(OverrideAppDelegate)
+(void)load
{
    extern const char* AppControllerClassName;
    AppControllerClassName = "ExtendedUnityAppController";
}
@end


@implementation ExtendedUnityAppController

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    application.applicationIconBadgeNumber = 0;

    // ---- DMO analitics setup ----- FROZEN DEPRECATED
    /*
    [[AnalyticsManager sharedAnalyticsManger] startAnalyticsWithAppID:DMO_APPKEY appSecret:DMO_SECRETKEY];
    [[AnalyticsManager sharedAnalyticsManger] logEventUserInfoWithDeviceId:nil userId:nil userProfile:nil];
    [[AnalyticsManager sharedAnalyticsManger] logEventPlayerInfoWithDeviceId:nil playerId:nil];
    [[AnalyticsManager sharedAnalyticsManger] startCrittercismWithAppID:CRITTERCISM_APPKEY];
    */
    //[[AnalyticsManager sharedAnalyticsManger] logEventWithName:@"app_start"];

    // ---- ComScore setup -----
    [CSComScore setAppContext];
    [CSComScore setCustomerC2:@"6035140"];
    [CSComScore setPublisherSecret:@"bacd860dcd22dd180bdcb7c680f64060"];
    [CSComScore setAppName:@"Maleficent Free Fall"];
    
    //	Initialize Crittercism so we can see unity startup crashes
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}


- (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification
{
    application.applicationIconBadgeNumber = 0;
}

- (void)applicationWillEnterForeground:(UIApplication *)application 
{
    [super applicationWillEnterForeground:application];
}

- (void)applicationDidEnterBackground:(UIApplication *)application 
{
    application.applicationIconBadgeNumber = 0; 
    [super applicationDidEnterBackground:application];
}


- (void)applicationWillTerminate:(UIApplication *)application 
{
    [super applicationWillTerminate:application];
}


@end

