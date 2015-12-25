//
//  DMOAnalytics.h
//  Disney Mobile Platform - Analytics Library
//
//  Created by Michael VanLandingham on 9/10/10.
//  Copyright 2013 Tapulous, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
 
/*!
    @header DMOAnalytics
    @abstract   Simple to use analytics logging library.  
    @discussion DMOAnalytics is a static library ("libtapalytics.a") for iOS which can be easily dropped into your application in order to log events to the Tapulous / Disney Mobile analytics servers, for analytics tracking purposes.  The library is designed to be as simple as possible to use.  At minimum, you can link it into your application, initialize it, and it will automatically log the following events: application start, application background, application foreground, application quit (the automatic logging can be turned off with a single switch as well).  Additionally, you can log custom events -- simple events or compound events based on key-value pairs (passed to the API in an NSDictionary). Network activity can be suspended (e.g. during performance-critical gameplay), during which any analytics events will be queued for later posting.   
		
	Be aware that you must have a pair of valid keys (UUIDs) for appKey and secret. 
 
*/


/*!
    @class
    @abstract    DMOAnalytics is your one-stop analytics object.
    @discussion  Initialize with one of the two designated initializers using the appropriate UUID's.  Use the singleton accessor class method "sharedAnalyticsManager" once you've initialized the object using initWithAppKey:secret: etc..  DMOAnalytics automatically logs the following events (based on NSNotifications): application start ("app_start"), application background ("app_background"), application foreground ("app_foreground"), application quit ("app_quit"). This automatic logging of the "standard events" can be turned off with the useNotificationsForStandardEvents switch. Furthermore, network access can be suspended, for instance in-game, where network activity may affect performance.  This can be done directly, using the canUseNetwork property, or alternatively, implement the DMOAnalyticsDelegate protocol (see below). Individual events can be tracked using logAnalyticsEvent: and logAnalyticsEvent:withContext: (see below for more on these). The queue of unsent analytics events can be explictly flushed to the network, using flushAnalyticsQueue.  This happens implictly when setting canUseNetwork = YES and there are queued events.
*/
@interface DMOAnalytics : NSObject {
@private
	id backend;
}

/*!
	@property	debugLogging
	@abstract	turn on to log to the console
 */
@property (nonatomic, readwrite) BOOL debugLogging;


/*!
    @property   delegate
    @abstract   Optional. _TOTALLY OPTIONAL_ :) Used for DMOAnalyticsDelegate protocol.
    @discussion Optionally set delegate & implement the delegate protocol (see DMOAnalyticsDelegate below).
*/
@property (readwrite, assign) id delegate;


/*!
 @property   canUseNetwork
 @abstract   Mutable property, defaults to YES. Suspend/resume any network access by analytics.   
 @discussion Suspend/resume any network access by the analytics library (e.g. during performance critical gameplay, etc). Events are queued up for later transmission.
 */
@property (nonatomic, assign, readwrite) BOOL canUseNetwork;

/*!
    @property	useNotificationsForStandardEvents
    @abstract    Mutable property, defaults to YES. Log events on notifications like UIApplicationDidFinishLaunching, etc.
    @discussion  DMOAnalytics automatically logs 4 events (triggered by NSNotifications): app_start, background, foreground, app_end. This automatic logging of "standard events" can be turned off with this switch.
 */
@property (nonatomic, assign, readwrite) BOOL useNotificationsForStandardEvents;

/*!
 @property   restrictedTracking
 @abstract   Mutable property, defaults to NO. Restrict analytics tracking to core events only. All others are dropped.   
 @discussion Restrict analytics tracking to core "standard events":  (app_start, stop, foreground, backround).  This can be used as a control flag based on a backend response (e.g., servers are overloaded, turn down the volume).
 */
@property (nonatomic, assign) BOOL restrictedTracking;


/*!
 @property   DIIDA
 @abstract   Optional. Used for ad tracking.
 @discussion When the app is serving advertisements, DMOAnalytics can use iOS advertising Id to track ads. For code examples, please refer to the library documentation.
 */
@property (nonatomic, retain) NSString *DIIDA;


/*!
    @method     sharedAnalyticsManager
    @abstract   A singleton accessor for the DMOAnalytics class.
    @discussion NOTE-->>  You MUST use one of the designated initializers below before calling the sharedAnalyticsManager, otherwise you get nil.
*/
+ (id) sharedAnalyticsManager;

/*!
 @method     initWithAppKey:secret:
 @abstract   One of 2 designated Analytics initializers.
 @discussion Notification-based analytics events enabled by default.
 @param key A valid UUID which identifies this application (valid == previously registered with the backend server) 
 @param secret Another UUID used to sign & validate the posted data.
 @result Returns an initialized DMOAnalytics object.
 */
- (DMOAnalytics*)initWithAppKey:(NSString*)key secret:(NSString*)secret;

/*!
 @method     initWithAppKey:secret:useNotifications:
 @abstract   One of 2 designated Analytics initializers.
 @discussion Notification-based analytics events enabled or disabled by the last parameter.
 @param key A valid UUID which identifies this application (valid == previously registered with the backend server) 
 @param secret Another UUID used to sign & validate the posted data.
 @param yn Turn on or off notification-based analytics events from the start.
 @result Returns an initialized DMOAnalytics object.
 */
- (DMOAnalytics*)initWithAppKey:(NSString*)key secret:(NSString*)secret useNotifications:(BOOL)yn;

/*!
    @method     initWithURL:appKey:secret:
    @abstract   DEPRECATED Version 1.0 initializer (one of 2) for use with endpoint "alog.analytics.tapulous.com".
    @discussion Notification-based analytics events enabled by default.
	@param url A valid URL to which analytics events will be posted.
	@param key A valid UUID which identifies this application (valid == previously registered with the backend server) 
	@param secret Another UUID used to sign & validate the posted data.
	@result Returns an initialized DMOAnalytics object.
*/
- (DMOAnalytics*)initWithURL:(NSURL*)url appKey:(NSString*)key secret:(NSString*)secret;


/*!
 @method     initWithURL:appKey:secret:useNotifications:
 @abstract   DEPRECATED Version 1.0 initializer (one of 2)for use with endpoint "alog.analytics.tapulous.com".
 @discussion Notification-based analytics events enabled or disabled by the last parameter.
 @param url A valid URL to which analytics events will be posted.
 @param key A valid UUID which identifies this application (valid == previously registered with the backend server) 
 @param secret Another UUID used to sign & validate the posted data.
 @param yn Turn on or off notification-based analytics events from the start.
 @result Returns an initialized DMOAnalytics object.
 */
- (DMOAnalytics*)initWithURL:(NSURL*)url appKey:(NSString*)key secret:(NSString*)secret useNotifications:(BOOL)yn;


/*!
 @method     sendDeviceToken:withKey:secret
 @abstract   Send device token from push notification registration
 @discussion Use this to send the device token received from app delegate to user acquisition server, along with its appropriate key and secret
 */
- (void)sendDeviceToken:(NSData *)deviceToken withKey:(NSString *)key secret:(NSString*)secret;


/*!
    @method     logAnalyticsEvent:
    @abstract   An all-purpose analytics logging call
    @discussion Use this for arbitrary events. 
*/
- (void)logAnalyticsEvent:(NSString*)eventDescription;


/*!
 @method     logAnalyticsEvent:withContext:
 @abstract   Log an event which has a top-level scope and some finer level detail in a dictionary
 @discussion Use this for arbitrary events which require more complex description of context.  The scope string should be the top-level event scope for the rest of the data.  An example would be "store".  The the 'context' dictionary might look something like { 'purchase': {'itemname': 'fingle', 'price': 200, 'page': 3 }}.  In most cases, this should conform to whatever schema or contract you have for your application's tracking data.
 */
- (void)logAnalyticsEvent:(NSString *)scope withContext:(NSDictionary*)details;


/*!
 @method     logAnalyticsEvent:withContext:withFields:
 @abstract   Log an event which has a top-level scope and some finer level detail in a dictionary
 @discussion Use this to additionally send necessary fields outside of the lumped together context data. See documentation or ask your BI contact for specifics as to which fields are expected.
 */
- (void)logAnalyticsEvent:(NSString *)scope withContext:(NSDictionary*)details withFields:(NSDictionary *)fields;

/*!
 @method     logAppStart
 @abstract   Log the app_start event manually
 @discussion Use this function to send app start event data. Some "drop-in" implementations of DMOAnalytics won't be able to initialize the app_start event in the applicationDidFinishLaunching, so expose this API call to serve that purpose.
 */
- (void)logAppStart;

/*!
    @method     flushAnalyticsQueue
    @abstract   Attempt to post any queued analytic events to the network.
    @discussion Won't do anything if canUseNetwork is set to NO. Otherwise writes the queued up analytics events to the server.
*/
- (void)flushAnalyticsQueue;


@end

 
#pragma mark -	
#pragma mark DELEGATE PROTOCOL 
/*!
    @protocol
    @abstract    Conform to DMOAnalyticsDelegate protocol to gatekeep network activity
    @discussion  You can optionally use a delegate pattern to let analytics know if it should do any network activity. This overrides the "canUseNetwork" property in the DMOAnalytics class. To use, set yourself as the delegate & implement the analyticsCanUseNetwork method. Return NO during any performance critical code or in places where you don't want the Analytics library to use the network.  Note that analytics events are still collected, just not sent immediately.  The next time canUseNetwork is enabled, events are flushed/sent. 
*/

@protocol DMOAnalyticsDelegate
@optional
// Implement in delegate if we should check before doing any network posting of analytics data
- (BOOL)analyticsCanUseNetwork;

@end


//
// Strings for common events.
//

// These are used for notification-based events:
extern NSString * kAnalyticsEvent_ApplicationStart;
extern NSString * kAnalyticsEvent_ApplicationQuit;	
extern NSString * kAnalyticsEvent_ApplicationForeground;
extern NSString * kAnalyticsEvent_ApplicationBackground;

// These are defined for use by implementors:
extern NSString * kAnalyticsEvent_ApplicationError;
extern NSString * kAnalyticsEvent_GameplayStart;	
extern NSString * kAnalyticsEvent_GameplayStop;		
