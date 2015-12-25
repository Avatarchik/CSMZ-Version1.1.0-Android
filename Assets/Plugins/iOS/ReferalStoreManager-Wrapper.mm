//
//  ReferalStoreManager-Wrapper.m
//  ReferalStore
//
//  Created by Jose Antonio Victoria on 30/04/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import "ReferalStoreManager-Wrapper.h"
#import "ReferalStoreManager.h"

extern "C"
{
    void ReferalStoreManager_showReferalView()
    {
        [[ReferalStoreManager sharedInstance] showReferalView];
    }
    
    void ReferalStoreManager_setAppID(const char *appId)
    {
        NSString *stringAppId = [NSString stringWithUTF8String:appId];
        [[ReferalStoreManager sharedInstance] setAppId:stringAppId];
    }
    
    void ReferalStoreManager_setCallbackObjectName(const char *name)
    {
        NSString *stringName = [NSString stringWithUTF8String:name];
        [[ReferalStoreManager sharedInstance] setCallbackObjectName:stringName];
    }
    
    void ReferalStoreManager_unsetCallbackObjectName(const char *name)
    {
        NSString *stringName = [NSString stringWithUTF8String:name];
        [[ReferalStoreManager sharedInstance] unsetCallbackObjectName:stringName];
    }
}
