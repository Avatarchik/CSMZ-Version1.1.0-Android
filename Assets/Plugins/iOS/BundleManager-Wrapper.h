//
//  BundleManager-Wrapper.h
//  BundleManager
//
//  Created by Jose Antonio Victoria on 03/06/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#ifndef __BundleManager__BundleManagerWrapper__
#define __BundleManager__BundleManagerWrapper__

extern "C"
{
    const char * BundleManager_getBundleVersion();
    const char * BundleManager_getBundleID();
}

#endif