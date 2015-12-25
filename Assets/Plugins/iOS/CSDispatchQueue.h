//
//  CSQueue.h
//  comScore

#import <Foundation/Foundation.h>
#import "CSAggregateMeasurement.h"
#import "CSDispatchProperties.h"

#define DAY_CHECK_OFFSET @"q_dcf"
#define DAY_CHECK_COUNTER @"q_dcc"
#define MILLIS_PER_SECOND 1000
#define MILLIS_PER_DAY 1000L * 60L * 60L * 24L

@interface CSDispatchQueue : NSObject {
    
    dispatch_queue_t dispatchQueue_;
    CSAggregateMeasurement *aggregateData_;
    int eventCounter_;
    CSCore *core;
    BOOL gdcEnabled_;
    CSDispatchProperties *dispatchProperties_;
}

@property double secondEventCheckOffset;
@property int secondEventCheckCounter;
@property double dayEventCheckOffset;
@property int dayEventCheckCounter;
@property (nonatomic, retain) CSDispatchProperties *dispatchProperties;

- (id) initWithCore:(CSCore *)aCore;
- (void) loadEventData;
- (BOOL) checkCounters;
- (void) enqueueMeasurement:(CSMeasurement *)measurement;
- (void) enqueueCacheFlush;
- (void) enqueueAutomaticCacheFlush;
- (void) processAggregateDataIn:(CSMeasurement *)measurement;
- (void) processEventCounter:(CSMeasurement *)measurement;
- (void) setGDCEnabled:(BOOL) enabled;
- (BOOL) gdcEnabled;
- (void) enqueueSettingChange:(NSDictionary*)settingChange;
- (void) enqueueBlock:(dispatch_block_t) block;

@end
