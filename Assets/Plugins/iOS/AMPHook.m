//
//  AMPBinding.cpp
//  TestAMPS
//
//  Created by Juan Cucurella on 10/24/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import "AMPManager.h"

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

void InitDMOAssetManagerWithPath(const char* unityListenerName, const char* assetDownloadPath, const char* appName, const char* language, const char* quality, const char* version)
{
    NSLog(@"AMPBinding: Init DMOAssentManager");
    [[AMPManager sharedAMPManager] initDMOAssetManagerWithPath:GetStringParam(assetDownloadPath) unityListenerName:GetStringParam(unityListenerName) forApp:GetStringParam(appName) language:GetStringParam(language) quality:GetStringParam(quality) version:GetStringParam(version)];
}

void DownloadAssetWithName(const char* assetDownloadPath, const char* assetName)
{
    NSLog(@"AMPBinding: Downloading %s", assetName);
    [[AMPManager sharedAMPManager] downloadAssetWithName:GetStringParam(assetName) assetDownloadPath:GetStringParam(assetDownloadPath)];
}

const char * VersionOfFile(const char* assetDownloadPath, const char* assetName)
{
    NSLog(@"AMPBinding: version of %s", assetName);
    return MakeStringCopy([[AMPManager sharedAMPManager] versionOfAsset:GetStringParam(assetName) assetDownloadPath:GetStringParam(assetDownloadPath)]);
}

bool IsCatalogDownloaded(const char* assetDownloadPath)
{
    return [[AMPManager sharedAMPManager] isCatalogDownloaded:GetStringParam(assetDownloadPath)];
}