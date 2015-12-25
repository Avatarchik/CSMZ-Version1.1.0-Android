//
//  AMPManager.m
//  TestAMPS
//
//  Created by Juan Cucurella on 10/22/13.
//  Copyright (c) 2013 Juan Cucurella. All rights reserved.
//

#import "AMPManager.h"
#import "DMOAssetManager.h"
#import "DMOAssetDownloader.h"

@implementation AMPManager {
    //    DMOAssetManager *assetManager;
    // The key is a NSString representing the asset download path.
	// The value for the each key is a AssetManagerInfo object - the asset manager info corresponding to the asset download path.
    NSMutableDictionary *assetManagers;
}

#pragma mark -
#pragma mark Singleton

+ (AMPManager*)sharedAMPManager
{
    static AMPManager *sharedInstance;
    
    if (!sharedInstance)
    {
        sharedInstance = [[AMPManager alloc] init];
    }
    
    return sharedInstance;
}


- (void)initDMOAssetManagerWithPath:(NSString *)assetDownloadPath unityListenerName:(NSString *)unityListenerName forApp:(NSString *)appName language:(NSString *)language quality:(NSString *)quality version:(NSString *)version
{
    NSLog(@"Init DMO AssetManager");
    
    if (assetManagers == nil) {
        assetManagers = [NSMutableDictionary new];
    }
    
    NSLog(@"[AMPS]: Initializing Asset Manager with download path ");
    NSLog(@"[AMPS]: Unity events listener: %@", unityListenerName);
    NSLog(@"[AMPS]: Path %@", assetDownloadPath);
    NSLog(@"[AMPS]: AppName %@", appName);
    NSLog(@"[AMPS]: Language %@", language);
    NSLog(@"[AMPS]: Quality %@", quality);
    NSLog(@"[AMPS]: Version %@", version);
    NSLog(@"[AMPS]: **************************************************");
    
    // Check if the specified asset download path is already registered to an asset manager. If not, register it here.
    AssetManagerInfo *assetMgrInfo = [assetManagers valueForKey:assetDownloadPath];
    if (assetMgrInfo == nil)
    {
        DMOAssetManager* assetManager = [[DMOAssetManager alloc] initWithHomePath:assetDownloadPath appName:appName locale:language quality:quality version:version device:@"iphone" sizeString:@"0640x1136"];
        
        [assetManager setNetworkReachabilityMode:DMONetworkReachabilityAny];
        
        assetMgrInfo = [[AssetManagerInfo alloc] initWithUnityListener:unityListenerName assetManager:assetManager];
        
        [assetManagers setValue:assetMgrInfo forKey:assetDownloadPath];
    }
    
    [self getServerListForAsset:assetMgrInfo];
}



- (void)getServerListForAsset:(AssetManagerInfo *)assetMgrInfo
{
    NSError *error;
    
    if ([DMONetworkReachability isNetworkReachable:DMONetworkReachabilityAny])
    {
        if (![assetMgrInfo.assetManager downloadAssetCatalogFromServerBuildAssetCatalog:YES delegate:assetMgrInfo error:&error]) {
            [AMPManager sendMessage:@"ErrorInitAMPManager" toObject:assetMgrInfo.unityListenerName withParameter:@"Error downloading asset catalogs"];
            
        }
    }
    else {
        [AMPManager sendMessage:@"ErrorInitAMPManager" toObject:assetMgrInfo.unityListenerName withParameter:@"Network connection error"];
    }
}

- (void)downloadAssetWithName:(NSString *)assetName assetDownloadPath:(NSString *)assetDownloadPath
{
    NSLog(@"Downloading asset %@", assetName);
    AssetManagerInfo *assetMgrInfo = [assetManagers valueForKey:assetDownloadPath];
    
    if (assetMgrInfo.assetManager == nil || !assetMgrInfo.assetManager.assetCatalogDownloaded)
    {
        [AMPManager sendMessage:@"ErrorDownloadingAsset" toObject:assetMgrInfo.unityListenerName withParameter:@"AMPManager not initialized. You should initialized before download any files"];
        return;
    }
    
    id asset = [assetMgrInfo.assetManager.currentLocalAssetCatalog objectForKey:[assetName stringByDeletingPathExtension]];
    if (asset != nil)
    {
        NSDictionary *userInfo = @{@"name": assetName};
        [assetMgrInfo.assetManager downloadAssetNamed:[assetName stringByDeletingPathExtension] Version:[assetMgrInfo.assetManager latestVersionForAssetNamed:[assetName stringByDeletingPathExtension]] userInfo:userInfo];
    }
    else
    {
        [AMPManager sendMessage:@"ErrorDownloadingAsset" toObject:assetMgrInfo.unityListenerName withParameter:[NSString stringWithFormat:@"File %@ not exists in server", assetName]];
    }
    
}

- (NSString *)versionOfAsset:(NSString *)assetName assetDownloadPath:(NSString *)assetDownloadPath
{
    AssetManagerInfo *assetMgrInfo = [assetManagers valueForKey:assetDownloadPath];
    
    if (assetMgrInfo.assetManager.assetCatalogDownloaded)
        return [assetMgrInfo.assetManager latestVersionForAssetNamed:[assetName stringByDeletingPathExtension]];
    else
        return @" ";
}

- (BOOL) isCatalogDownloaded:(NSString *)assetDownloadPath
{
    AssetManagerInfo *assetMgrInfo = [assetManagers valueForKey:assetDownloadPath];
    
    return (assetMgrInfo.assetManager != nil && assetMgrInfo.assetManager.assetCatalogDownloaded);
}

+ (void) sendMessage:(NSString *)messageName toObject:(NSString *)objectName withParameter:(NSString *)parameterString
{
    NSLog(@"AMPManager: Sending Message %@", parameterString);
    
    UnitySendMessage([objectName cStringUsingEncoding:NSUTF8StringEncoding], [messageName cStringUsingEncoding:NSUTF8StringEncoding], [parameterString cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void)dealloc
{
    if (assetManagers != nil)
    {
        [assetManagers removeAllObjects];
    }
    
    [super dealloc];
}

@end


#pragma mark AssetManagerInfo class

@implementation AssetManagerInfo

@synthesize assetManager;
@synthesize unityListenerName;

- (id)initWithUnityListener:(NSString *)_unityListenerName assetManager:(DMOAssetManager *)_assetManager
{
    if ( (self = [super init]) )
    {
        self.unityListenerName = _unityListenerName;
        self.assetManager = _assetManager;
        
        [self.assetManager.downloader setDelegate:self];
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(downloadFileDidFinish:) name:DownloadSucceededNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(downloadFileDidFail:) name:DownloadFailedNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(downloadFileDidCancel:) name:DownloadCancelledNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(downloadFileStarted:) name:DownloadStartedNotification object:nil];
    }
    
    return self;
}

#pragma mark DMOAssetDownloader Delegate Methods

- (void)downloadFileStarted:(NSNotification *)notification
{
    DMOAssetDownloadItem *item = (DMOAssetDownloadItem *)notification.object;
    NSString* fileName = [item.userInfo objectForKey:@"name"];
    NSLog(@"AMPManager: Start downloading %@", fileName);
    
    [AMPManager sendMessage:@"DownloadFileStarted" toObject:self.unityListenerName withParameter:fileName];
}

- (void)downloadFileDidCancel:(NSNotification *)notification
{
    DMOAssetDownloadItem *item = (DMOAssetDownloadItem *)notification.object;
    NSString* fileName = [item.userInfo objectForKey:@"name"];
    NSLog(@"AMPManager: Cancel Download %@", fileName);
    [AMPManager sendMessage:@"DownloadFileDidCancel" toObject:self.unityListenerName withParameter:fileName];
    
}

- (void)downloadFileDidFail:(NSNotification *)notification
{
    DMOAssetDownloadItem *item = (DMOAssetDownloadItem *)notification.object;
    NSString* fileName = [item.userInfo objectForKey:@"name"];
    NSLog(@"AMPManager: Fail Download %@", fileName);
    [AMPManager sendMessage:@"DownloadFileDidFail" toObject:self.unityListenerName withParameter:fileName];
}

- (void)downloadFileDidFinish:(NSNotification *)notification
{
    DMOAssetDownloadItem *item = (DMOAssetDownloadItem *)notification.object;
    NSString* fileName = [item.userInfo objectForKey:@"name"];
    NSLog(@"AMPManager: Finish %@", fileName);
    [AMPManager sendMessage:@"DownloadFileDidFinish" toObject:self.unityListenerName withParameter:fileName];
}

-(void)assetManagerServerListDidFinishLoading
{
    NSLog(@"Catalog downloaded");
    [AMPManager sendMessage:@"AMPManagerDidInit" toObject:self.unityListenerName withParameter:@"AMPSManager is initialized"];
}

- (void)dealloc
{
    self.unityListenerName = nil;
    [self.assetManager.downloader setDelegate:nil];
    self.assetManager = nil;
    
    [[NSNotificationCenter defaultCenter] removeObserver:DownloadSucceededNotification];
    [[NSNotificationCenter defaultCenter] removeObserver:DownloadFailedNotification];
    [[NSNotificationCenter defaultCenter] removeObserver:DownloadCancelledNotification];
    [[NSNotificationCenter defaultCenter] removeObserver:DownloadStartedNotification];
    
    [super dealloc];
}



@end
