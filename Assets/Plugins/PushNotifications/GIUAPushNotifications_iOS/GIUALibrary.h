//
//  GIUALibrary.h
//  testUALibrary
//
//  Created by Ale Moreno on 10/02/14.
//  Copyright (c) 2014 Ale Moreno. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface GIUALibrary : NSObject

+ (id)shared;

- (void) setAppKey:(NSString *)appKey andAppSecret:(NSString*)appSecret;
- (void) requestAppRegistration;
- (void) registerTokenInUrbanAirship:(NSData*)stringToken;

@end

