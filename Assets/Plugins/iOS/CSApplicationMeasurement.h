//
//  CSApplicationMeasurement.h
//  comScore
//

// Copyright 2011 comScore, Inc. All right reserved.
//

#import <Foundation/Foundation.h>
#import "CSMeasurement.h"
#import "CSEventType.h"


@interface CSApplicationMeasurement : CSMeasurement {

}

-(id) initWithCore:(CSCore *)core eventType:(CSApplicationEventType)type pixelURL:(NSString *)pixelURL;

-(id) initWithCore:(CSCore *)core eventType:(CSApplicationEventType)type pixelURL:(NSString *) pixelURL coldStart:(bool)coldBoot;

+ (CSApplicationMeasurement *) newApplicationMeasurement:(CSCore *)core eventType:(CSApplicationEventType) type labels:(NSDictionary *)details pixelURL:(NSString *) pixelURL;

@end
