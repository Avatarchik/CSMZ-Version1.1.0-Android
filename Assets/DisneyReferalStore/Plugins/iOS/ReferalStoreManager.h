//
//  ReferalStoreManager.h
//  ReferalStore
//
//  Created by Jose Antonio Victoria on 30/04/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DMNReferralStoreViewController.h"

@interface ReferalStoreManager : NSObject<DMNReferralStoreViewControllerDelegate>

+ (ReferalStoreManager *) sharedInstance;

- (void)setCallbackObjectName:(NSString*)name;
- (void)unsetCallbackObjectName:(NSString*)name;

- (void)showReferalView;
- (void)showReferalInViewController:(UIViewController *)vc delegate:(id<DMNReferralStoreViewControllerDelegate>)delegate;
- (void)setAppId:(NSString*)appID;

@end
