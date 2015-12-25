//
//  DMOAnalitics.h
//  Unity-iPhone
//
//  Created by Juan Cucurella on 10/18/13.
//
//

#import "UnityAppController.h"

#define DMO_BUNDLEID @"com.disney.frozensaga"
#define DMO_APPKEY @"7D17F2F9-4312-476B-9DB7-EEA8BF9A093F"
#define DMO_SECRETKEY @"B3C47EF3-81B1-4085-9070-DC11A4E8D80E"
#define DMO_ACRONIM @"frozenfreefall_ip"


// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]


@interface ExtendedUnityAppController : UnityAppController


@end
