//
//  StoreKitUtilsManager
//
//  Created by Juan Cucurella on 10/22/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import "StoreKitUtilsManager.h"
#import <StoreKit/StoreKit.h>

@implementation StoreKitUtilsManager

#pragma mark -
#pragma mark Singleton

+ (StoreKitUtilsManager*)sharedInstance
{
    static StoreKitUtilsManager *_sharedInstance;
    
    if (!_sharedInstance)
    {
        _sharedInstance = [[StoreKitUtilsManager alloc] init];
    }
    
    return _sharedInstance;
}

#pragma mark -
#pragma mark Methods

// Request iTunes Store info for product with a valid identifier (com.company.app.productID)
- (void)requestProductsDataWithIdentifier:(NSString *)identifier
{
    NSSet *productIdentifiers = [NSSet setWithObject:identifier];
    SKProductsRequest* productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    productsRequest.delegate = self;
    [productsRequest start];
    [productsRequest release];
}

#pragma mark -
#pragma mark SK Methods Delegate

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    
    NSArray *products = response.products;
    
    if (products && products.count != 0) {
        SKProduct *product = [products objectAtIndex:0];
        NSString *locale = [product.priceLocale objectForKey:NSLocaleCountryCode];
        
        UnitySendMessage([@"StoreKitUtilsBinding" cStringUsingEncoding:NSUTF8StringEncoding], [@"ProductLocaleReceived" cStringUsingEncoding:NSUTF8StringEncoding], [locale cStringUsingEncoding:NSUTF8StringEncoding]);
    }
    else {
        UnitySendMessage([@"StoreKitUtilsBinding" cStringUsingEncoding:NSUTF8StringEncoding], [@"ProductLocaleReceived" cStringUsingEncoding:NSUTF8StringEncoding], [@"error" cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

@end
