//
//  CSOfflineMeasurementFileCache.h
//  comScore


#import <Foundation/Foundation.h>
#import "CSApplicationMeasurement.h"
#import "CSMeasurement.h"
#import "CSDispatchProperties.h"

@interface CSOfflineMeasurementsCache : NSObject {
    
    NSURLConnection *connection_;
    
    NSMutableArray *arrayOfCacheFiles_;
    
    NSURLRequest *request_;
    
    int flushesInARow_;
    NSTimeInterval lastFlushDate_;
    NSTimeInterval lastFailDate_;
    
    // Common labels to be aggrupped in the header of the xml
    NSString *c12_;
    NSString *c1_;
    NSString *ns_ap_an_;
    NSString *ns_ap_pn_;
    NSString *ns_ap_device_;
    NSString *ns_ak_;
    NSMutableString *concatedProcessedEvents_;
    
    CSCore *core;
}

-(id) initWithCore:(CSCore *)aCore;
- (int) count;
- (BOOL) isEmpty;
- (NSArray *)newestEventBatch;
- (BOOL) removeAllEvents;
- (BOOL) flush:(CSDispatchProperties*)dispatchProperties;
- (BOOL) isAutomaticFlushAllowed;
- (BOOL) automaticFlush:(CSDispatchProperties *)dispatchProperties;

- (void) saveEvent:(NSString *)event dispatchProperties:(CSDispatchProperties*)dispatchProperties;
- (void) saveMeasurement:(CSMeasurement *)m dispatchProperties:dispatchProperties;
- (void) saveMeasurementWithApplicationType:(CSApplicationEventType) type labels:(NSDictionary *)labels dispatchProperties:(CSDispatchProperties*)dispatchProperties;

- (void) saveEvent:(NSString *)event dispatchProperties:(CSDispatchProperties*)dispatchProperties inBackground:(BOOL)inBackground;
- (void) saveMeasurement:(CSMeasurement *)m dispatchProperties:dispatchProperties inBackground:(BOOL)inBackground;
- (void) saveMeasurementWithApplicationType:(CSApplicationEventType) type labels:(NSDictionary *)labels dispatchProperties:(CSDispatchProperties*)dispatchProperties inBackground:(BOOL)inBackground;
- (void) saveApplicationMeasurement:(CSApplicationEventType)type andLabels:(NSDictionary *)labels
                 dispatchProperties:(CSDispatchProperties*)dispatchProperties;
- (void) saveApplicationMeasurement:(CSApplicationEventType)type andLabels:(NSDictionary *)labels
                 dispatchProperties:(CSDispatchProperties*)dispatchProperties inBackground:(BOOL)inBackground;


@property (nonatomic, assign) int maxSize;
@property (nonatomic, assign) int maxBatchSize;
@property (nonatomic, assign) int maxFlushesInARow;
@property (nonatomic, assign) int minutesToRetry;
@property (nonatomic, assign) int expiryInDays;

@end
