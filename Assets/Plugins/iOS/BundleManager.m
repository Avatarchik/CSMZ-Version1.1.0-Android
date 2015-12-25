//
//  BundleManager.m
//  BundleManager
//
//  Created by Jose Antonio Victoria on 03/06/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import "BundleManager.h"

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}


@implementation BundleManager

+ (BundleManager *)sharedInstance
{
    static  BundleManager *instance = nil;
    @synchronized(self)
    {
        if (!instance) {
            instance = [BundleManager new];
        }
    }
    return instance;
}

- (id)init
{
    self = [super init];
    if (self) {
        
    }
    return self;
}

- (const char *)getBundleVersion
{
    NSString *bundleVersion = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
    return cStringCopy([bundleVersion UTF8String]);
}

- (const char*)getBundleID
{
    NSString *bundleID = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleIdentifier"];
    return cStringCopy([bundleID UTF8String]);
}

@end
