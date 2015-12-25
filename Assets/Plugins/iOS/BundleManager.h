//
//  BundleManager.h
//  BundleManager
//
//  Created by Jose Antonio Victoria on 03/06/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface BundleManager : NSObject

+ (BundleManager *)sharedInstance;

- (const char *)getBundleVersion;
- (const char*)getBundleID;

@end
