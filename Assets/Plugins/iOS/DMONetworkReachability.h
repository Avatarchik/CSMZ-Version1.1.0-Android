//
//  DMONetworkReachability.h
//  Downloader
//
//  Created by Mike Meyer on 4/20/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//

#import <Foundation/Foundation.h>

#define kDMONetworkNotReachable 400

typedef enum {
    DMONetworkReachabilityAny = 0,
    DMONetworkReachabilityWiFiOnly
}DMONetworkReachabilityMode;

@interface DMONetworkReachability : NSObject {
    
}


// Checks for reachable network, setting useWifiOnly to YES causes
// this to return YES only if the network is using wifi

+(BOOL)isNetworkReachable:(DMONetworkReachabilityMode)mode;

@end
