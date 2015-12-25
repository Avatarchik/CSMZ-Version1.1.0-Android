//
//  GIUALibrary.m
//  testUALibrary
//
//  Created by Ale Moreno on 10/02/14.
//  Copyright (c) 2014 Ale Moreno. All rights reserved.
//

#import "GIUALibrary.h"

#define GIPushAccepted @"GIPushAccepted"

@interface GIUALibrary ()

@property(nonatomic, strong) NSString *appKey;
@property(nonatomic, strong) NSString *appSecret;

@end

@implementation GIUALibrary

+ (id)shared {
    static GIUALibrary *shared = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        shared = [[self alloc] init];
    });
    return shared;
}

- (id)init {
    if (self = [super init]) {
        
    }
    return self;
}

- (void)dealloc {
    // Should never be called, but just here for clarity really.
}


// methods

-(void) setAppKey:(NSString *)appKey andAppSecret:(NSString*)appSecret
{
    [self setAppKey:appKey];
    [self setAppSecret:appSecret];
    
    if ([[NSUserDefaults standardUserDefaults] boolForKey:GIPushAccepted]) {
        [self requestAppRegistration];
    }
}


- (void) requestAppRegistration
{
    [[NSUserDefaults standardUserDefaults] setBool:YES forKey:GIPushAccepted];
    
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes:UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeSound | UIRemoteNotificationTypeBadge];
}

- (void)registerTokenInUrbanAirship:(NSData*)dataToken {
    
    NSMutableString *deviceToken = [NSMutableString stringWithCapacity:([dataToken length] * 2)];
    const unsigned char *bytes = (const unsigned char *)[dataToken bytes];
    
    for (NSUInteger i = 0; i < [dataToken length]; i++) {
        [deviceToken appendFormat:@"%02X", bytes[i]];
    }
    
    NSString *stringToken = [deviceToken lowercaseString];
    
    NSURL *url = [NSURL URLWithString: [NSString stringWithFormat:@"https://%@:%@@go.urbanairship.com/api/device_tokens/%@/", _appKey, _appSecret, stringToken]];
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    
    [request setURL:url];
    [request setHTTPMethod:@"PUT"];
    [request setValue:@"application/json" forHTTPHeaderField:@"Content-Type"];
    //[request setValue:@"application/vnd.urbanairship+json; version=3" forHTTPHeaderField:@"Accept"];

    [NSURLConnection sendAsynchronousRequest:request queue:[NSOperationQueue mainQueue] completionHandler:^(NSURLResponse *response, NSData *data, NSError *connectionError) {
        NSString *myString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        NSLog(@"Data Response %@ Error %@",myString,connectionError);
    }];
    
}

@end
