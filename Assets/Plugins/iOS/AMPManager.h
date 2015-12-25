//
//  AMPManager.h
//  TestAMPS
//
//  Created by Juan Cucurella on 10/22/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DMOAssetDownloader.h"
#import "DMOAssetManager.h"


/**
 * Stores the reference to each DMOAssetManager and its info required to correctly send
 * events for this asset manager back to unity. That's why this class also stores the name
 * of the unity game object that will contain the AMPS listener for a DMOAssetManager instance.
 */
@interface AssetManagerInfo: NSObject<DMOAssetManagerDelegateProtocol, DMOAssetDownloaderNotifyProtocol>

- (id)initWithUnityListener:(NSString *)_unityListenerName assetManager:(DMOAssetManager *)_assetManager;

@property (atomic, retain) DMOAssetManager *assetManager;
@property (atomic, retain) NSString *unityListenerName;

@end


@interface AMPManager : NSObject

+ (AMPManager*)sharedAMPManager;

- (void)initDMOAssetManagerWithPath:(NSString *)assetDownloadPath unityListenerName:(NSString *)unityListenerName forApp:(NSString *)appName language:(NSString *)language quality:(NSString *)quality version:(NSString *)version;

- (void)downloadAssetWithName:(NSString *)assetName assetDownloadPath:(NSString *)assetDownloadPath;

- (NSString *)versionOfAsset:(NSString *)assetName assetDownloadPath:(NSString *)assetDownloadPath;

- (BOOL) isCatalogDownloaded:(NSString *)assetDownloadPath;

- (void)getServerListForAsset:(AssetManagerInfo *)assetMgrInfo;

+ (void) sendMessage:(NSString *)messageName toObject:(NSString *)objectName withParameter:(NSString *)parameterString;

@end
