//
//  KochavaUnitySupport.h
//  KochavaUnitySupport
//
//  Created by David Bardwick on 12/3/12.
//  Copyright (c) 2012 PlayXpert. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface KochavaUnitySupport : NSObject

+(KochavaUnitySupport *) sharedManager;

- (NSDictionary*) returnKochavaInfo:(bool)suppressIDFV;

@end
