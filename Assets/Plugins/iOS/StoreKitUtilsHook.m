//
//  StoreKitUtilsHook.m
//
//  Created by Juan Cucurella on 10/24/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import "StoreKitUtilsManager.h"

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]



void RequestProductsDataWithIdentifier(const char * identifier)
{
    [[StoreKitUtilsManager sharedInstance] requestProductsDataWithIdentifier:GetStringParam(identifier)];
}

