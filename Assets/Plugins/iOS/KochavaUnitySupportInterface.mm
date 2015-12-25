//  Created by Aubrey Falconer on 12/15/12
//  Copyright (c) 2012 PlayXpert. All rights reserved.

#include "KochavaUnitySupport.h"
#import "JSONKit.h"

char* AutonomousStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

extern "C" {
    char* GetExternalKochavaInfo(bool suppressIDFV)
    {
        NSDictionary *kochavaInfo = [[KochavaUnitySupport sharedManager] returnKochavaInfo:suppressIDFV];
        
        //Works great for iOS 5
        //NSError *error;
        //NSData *jsonData = [NSJSONSerialization dataWithJSONObject:kochavaInfo options:0 error:&error];
        //NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

        NSString *jsonString = [kochavaInfo JSONString];
        
        const char* jsonChar = AutonomousStringCopy([jsonString UTF8String]);
        return AutonomousStringCopy(jsonChar);
    }
}