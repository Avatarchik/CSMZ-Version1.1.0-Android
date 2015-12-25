//
//  ReferalStoreManager-Wrapper.h
//  ReferalStore
//
//  Created by Jose Antonio Victoria on 30/04/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//


#ifndef __ReferalStore__ReferalStoreManagerWrapper__
#define __ReferalStore__ReferalStoreManagerWrapper__

extern "C"
{
    void ReferalStoreManager_showReferalView();
    void ReferalStoreManager_setAppID(const char *appId);
    void ReferalStoreManager_setCallbackObjectName(const char *name);
    void ReferalStoreManager_unsetCallbackObjectName(const char *name);
}

#endif