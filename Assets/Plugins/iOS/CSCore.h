//
//  CScomScore.h
//  comScore
//

// Copyright 2011 comScore, Inc. All right reserved.
//

#ifndef CSCOMSCORE_H
#define CSCOMSCORE_H

#import <Foundation/Foundation.h>
#import "CSEventType.h"
#import "CSReachability.h"
#import "CSTransmissionMode.h"
#import "CSCacheFlusher.h"
#import "CSUncaughtExceptionHandler.h"
#include <libkern/OSAtomic.h>
#include <execinfo.h>
#include "CSApplicationState.h"
#import "CSSessionState.h"
#import "CSDispatchProperties.h"

@class CSCensus, CSNotificationsObserver, CSOfflineMeasurementsCache, CSDispatchQueue, CSStorage, CSKeepAlive;

/**
 Nedstat ComScore analytics interface
 */
@interface CSCore : NSObject {
    
	NSString *visitorID;
	NSString *salt;
	NSString *appName;
	NSString *devModel;
    NSMutableDictionary *labels_;
    CSReachability *internetReachable_;
    BOOL keepAliveEnabled_;
    NSTimeInterval prevInactiveTime_;
    long cacheFlushingInterval;
    BOOL sessionRestored_;
    NSString *crossPublisherId_;
    BOOL isCrossPublisherIdBasedOnIFDA_;
    BOOL errorHandlingEnabled_;
    NSUncaughtExceptionHandler *defaultUncaughtExceptionHandler_;
    BOOL autoStartEnabled_;
    
    CSStorage *storage;
    CSDispatchQueue *dispatchQueue;
    CSDispatchProperties *dispatchProperties_;
    CSOfflineMeasurementsCache *offlineCache;
    CSNotificationsObserver *observer;
    CSKeepAlive *keepAlive;
    CSCacheFlusher *cacheFlusher;
    NSMutableSet *ssids_;
    
    // Common state machine fields
    long long autoUpdateInterval_;
    BOOL autoUpdateInForegroundOnly_;
    BOOL delayStateTransition_;
    BOOL coldStartDispatched_;
    int runsCount_;
    long long coldStartId_;
    int coldStartCount_;
    long long installId_;
    long long firstInstallId_;
    NSString *currentVersion_;
    NSString *previousVersion_;
    
    // Application State Machine
    #define kApplicationStateTransitionDelay 300
    CSApplicationState currentApplicationState_;
    int foregroundComponentsCount_;
    int activeUxComponentsCount_;
    int foregroundTransitionsCount_;
    long long totalForegroundTime_;
    long long accumulatedBackgroundTime_;
    long long accumulatedForegroundTime_;
    long long accumulatedInactiveTime_;
    long long genesis_;
    long long previousGenesis_;
    long long lastApplicationAccumulationTimestamp_;
    long long totalBackgroundTime_;
    long long totalInactiveTime_;
    
    // Session State Machine
    CSSessionState currentSessionState_;
    long long accumulatedApplicationSessionTime_;
    long long accumulatedUserSessionTime_;
    long long accumulatedActiveUserSessionTime_;
    int applicationSessionTimestamp_;
    int userSessionCount_;
    int activeUserSessionCount_;
    long long lastApplicationSessionTimestamp_;
    long long lastUserSessionTimestamp_;
    long long lastActiveUserSessionTimestamp_;
    int userInteractionCount_;
    long long lastUserInteractionTimestamp_;
    long long lastSessionAccumulationTimestamp_;
    int applicationSessionCount_;
    NSTimer *userInteractionTimer_;
    NSTimer *autoUpdateTimer_;
}

/**
 PixelURL setter.
 
 Parameters:
 
 - value: A NSString that contains the PixelURL.
 */
-(NSString *) setPixelURL: (NSString *) value;

/**
 Notify Application event (Start / Close / Aggregate) with custom labels
 
 Parameters:
 
 - type: A CSApplicationEventType enum that value must be Start, Close or Aggregate.
 - labels: A NSDictionary that contains a set of custom labels with key-value pairs.
 */
-(void) notifyWithApplicationEventType: (CSApplicationEventType) type labels: (NSDictionary *) labels;

-(id) init;
-(CSDispatchQueue *) queue;
-(CSStorage *) storage;
-(CSOfflineMeasurementsCache *) offlineCache;
-(CSNotificationsObserver *) observer;
-(CSKeepAlive *) keepAlive;
-(CSCacheFlusher *) cacheFlusher;
-(void) setVisitorId: (NSString *) value;
-(void) resetVisitorID;
- (void) restoreVisitorId;
-(NSString *) visitorId;
-(void) setSalt: (NSString *) value;
-(NSString *) salt;
-(void) appName: (NSString *) value;
-(NSString *) appName;
-(NSString *) devModel;
- (NSString *) generateVisitorId;
-(long long) genesis;
-(long long) previousGenesis;
- (NSString *) macaddress;
-(void) setLabel:(NSString *)name value:(NSString *)value;
-(void) setLabels:(NSDictionary *)labels;
-(NSMutableDictionary *)labels;
- (NSString *) label:(NSString *)labelName;
- (BOOL) isKeepAliveEnabled;
- (void) setKeepAliveEnabled:(BOOL)enabled;
- (void) setCustomerC2:(NSString *)c2;
- (NSString *)customerC2;
- (void) setSecure:(BOOL)secure;
- (BOOL) isSecure;
- (NSString *) crossPublisherId;
- (void) flushOfflineMeasurementsCache;
- (NSArray *)measurementLabelOrder;
- (void) disableAutoUpdate;
- (void) enableAutoUpdate:(int) intervalInSeconds foregroundOnly:(BOOL)foregroundOnly;
- (int) coldStartCount;
- (long long) coldStartId;
- (NSString *) currentVersion;
- (long long) firstInstallId;
- (long long) installId;
- (NSString *) previousVersion;
- (int) runsCount;
- (bool) handleColdStart;
- (BOOL) isAutoUpdateEnabled;
- (void) onEnterForeground;
- (void) onExitForeground;
- (void) onUserInteraction;
- (void) onUxActive;
- (void) onUxInactive;
- (void) setMeasurementLabelOrder:(NSArray *)ordering;
- (void) update;
-(NSString *) version;
- (BOOL) isJailBroken;
- (long long) totalBackgroundTime:(bool)reset;
- (long long) totalInactiveTime:(bool)reset;
- (CSReachability *) internetReachable;


/** 
 Enables or disables live events (GETs) dispatched one by one when connectivity is available
 */
- (void) allowLiveTransmission:(CSTransmissionMode) mode;

/**
 Enables or disables automatic offline cache flushes (POSTS). The cache can always be manually 
 flushed using the public api comScore.FlushOfflineCache()
 */
- (void) allowOfflineTransmission:(CSTransmissionMode) mode;


/**
 Returns the live transmission mode
 */
- (CSTransmissionMode) liveTransmissionMode;

/**
 Returns the offline transmission mode
 */
- (CSTransmissionMode) offlineTransmissionMode;
- (long) cacheFlushingInterval;
- (void) setCacheFlushingInterval:(long) seconds;

@property (nonatomic, assign, getter = isErrorHandlingEnabled) BOOL errorHandlingEnabled;
@property (readonly, nonatomic, assign) NSString *pixelURL;
@property (readonly, nonatomic, assign) BOOL isSessionRestored;
@property (readwrite,assign) BOOL enabled;
@property (nonatomic, getter = isAutoStartEnabled ,assign) BOOL autoStartEnabled;
@property (nonatomic, retain) CSDispatchProperties *dispatchProperties;

@end

#endif // ifndef CSCOMSCORE_H