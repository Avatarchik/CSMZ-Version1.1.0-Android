//
//  BundleManager-Wrapper.m
//  BundleManager
//
//  Created by Jose Antonio Victoria on 03/06/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import "BundleManager.h"
#import "BundleManager-Wrapper.h"

extern "C"
{
    const char * BundleManager_getBundleVersion()
    {
        return [[BundleManager sharedInstance] getBundleVersion];
    }
    
    const char * BundleManager_getBundleID()
    {
        return [[BundleManager sharedInstance] getBundleID];
    }
}
