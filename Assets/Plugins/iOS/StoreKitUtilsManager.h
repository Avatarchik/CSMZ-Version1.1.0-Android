//
//  StoreKitUtilsManager
//
//  Created by Juan Cucurella on 10/22/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface StoreKitUtilsManager : NSObject<SKProductsRequestDelegate>

+ (StoreKitUtilsManager*)sharedInstance;

- (void)requestProductsDataWithIdentifier:(NSString *)identifier;

@end
