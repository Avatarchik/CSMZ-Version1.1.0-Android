//
//  BurstlyManager-Wrapper.h
//  BurstlyTest
//
//  Created by Jesús on 05/04/14.
//  Copyright (c) 2014 Jesús. All rights reserved.
//

#ifndef __Burstly__BurstlyManagerWrapper__
#define __Burstly__BurstlyManagerWrapper__

extern "C" {
    
    void BurstlyManagerWrapper_configureBanner(const char *appId, const char *zoneId, float originX, float originY, float width, float height, int anchor);
    void BurstlyManagerWrapper_showBanner(const char *zoneId);
    void BurstlyManagerWrapper_hideBanner(const char *zoneId);
    
    void BurstlyManagerWrapper_configureInterstitial(const char *appId, const char *zoneId, BOOL autocache);
    void BurstlyManagerWrapper_showInterstitial(const char *zoneId);
}


#endif
