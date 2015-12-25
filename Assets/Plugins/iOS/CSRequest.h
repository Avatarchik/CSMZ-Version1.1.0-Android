//
//  CSRequest.h
//  comScore
//

// Copyright 2011 comScore, Inc. All right reserved.
//

#import <Foundation/Foundation.h>
#import "CSMeasurement.h"
#import "CSDispatchProperties.h"


@interface CSRequest : NSObject {
	NSURLRequest *req;
	NSURLConnection *conn;
	NSMutableData *receivedData;
    CSCore *core;
    CSDispatchProperties *dispatchProperties_;
}

-(id) initWithCore:(CSCore *)aCore andMeasurement:(CSMeasurement *) m dispatchProperties:(CSDispatchProperties*)dispatchProperties;
-(NSURL *) process: (NSString *) basePixelURL;
-(NSURL *) process;
-(BOOL) send;
-(BOOL) availableConnection;

@property (nonatomic, retain) CSMeasurement *measurement;

@end
