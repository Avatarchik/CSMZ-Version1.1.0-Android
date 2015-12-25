//
//  CSAppStartMeasurement.h
//  comScore
//

// Copyright 2011 comScore, Inc. All right reserved.
//

#import <Foundation/Foundation.h>
#import "CSApplicationMeasurement.h"


@interface CSAppStartMeasurement : CSApplicationMeasurement {

}

-(id) initWithCore:(CSCore *)core
EventType:(CSApplicationEventType)type
     pixelURL:(NSString *) pixelURL
        coldStart:(BOOL)coldStart;

@end
