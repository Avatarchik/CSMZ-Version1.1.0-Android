
#import "DMOAnalytics.h"
// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil
extern "C" {
    void _dmoAnalyticsInit( const char * appId, const char * appKey )
    {
        [[DMOAnalytics alloc] initWithAppKey:GetStringParam(appId) secret:GetStringParam(appKey)];
    }
    
    void _dmoAnalyticsLogEvent (const char *eventString)
    {
        [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:GetStringParam(eventString)];
    }
    
    void _dmoAnalyticsLogAppStart()
    {
        [[DMOAnalytics sharedAnalyticsManager] logAppStart];
    }
    
    void _dmoAnalyticsflushAnalyticsQueue()
    {
        [[DMOAnalytics sharedAnalyticsManager] flushAnalyticsQueue];
    }
    
    
    void _dmoAnalyticsLogEventWithContext (const char *eventString, const char *parameters )
    {
        NSData *eventData = [GetStringParam(parameters) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictData = [NSJSONSerialization JSONObjectWithData:eventData options:kNilOptions error:nil];
        [[DMOAnalytics sharedAnalyticsManager] logAnalyticsEvent:GetStringParam(eventString) withContext:dictData];
    }
    
    void _dmoAnalyticsSetDebugLogging (BOOL debugLogging)
    {
        [[DMOAnalytics sharedAnalyticsManager] setDebugLogging:debugLogging];
    }
    
    void _dmoAnalyticsSetCanUseNetwork (BOOL canUseNetwork)
    {
        [[DMOAnalytics sharedAnalyticsManager] setCanUseNetwork: canUseNetwork];
    }
    
    void _dmoAnalyticsSetRestrictedTracking (BOOL restrictedTracking)
    {
        [[DMOAnalytics sharedAnalyticsManager] setRestrictedTracking: restrictedTracking];
    }
    
    void _dmoAnalyticsSetDIIDA (const char *advertisingId)
    {
        [[DMOAnalytics sharedAnalyticsManager] setDIIDA:GetStringParam(advertisingId)];
    }
    
}