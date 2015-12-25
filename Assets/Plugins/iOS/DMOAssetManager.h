 //
//  DMOAssetManager.h
//  AMPS iOS Client Library
//
//
//  Created by Mike Meyer on 4/4/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//
// Version History:
//    28 Oct 2011           1.0.3      Added size checking to downloader, version string
//    09 Dec 2011           1.0.4      Added assetQuality filter to listassets URL query, also raise assertion if attempting download without specifying a destination file name
//    13 Dec 2011           1.0.5      Fixed assetQuality param in URL
//    15 Dec 2011           1.0.6 (beta)   addresses autorelease error in download op
//    11 Jan 2012           1.0.7      Deal with download requests for null assets, and asset updates
//    26 Jan 2012           1.0.8      Update to correct metadata issues when updating
//    10 Feb 2012           1.0.9      Revamped logging
//    15 Feb 2012           1.0.10     Unzip now uses temp directory
//    12 Mar 2012           1.0.11     Fixed issues with temp directory file handling, reachability error
//    19 Mar 2012           1.0.12     changed download metadata handling
//    01 May 2012           1.0.13     pageSize for JSON is now 1000 files.
//    18 Jun 2012           1.0.14     fixed isInstall error in save file restore
//    17 Sep 2012           1.0.15     [jonoble] Recompiled with XCode 4.5 (armv7/armv7s)
//    20 Sep 2012           1.0.16     [jonoble] Combined i386 (simulator) binary with device binary
//    20 May 2013           1.0.17     [jonoble] Allow two assets to be live at the same time (with different version numbers)
//    24 Jun 2013           1.0.18     [jonoble] Parse the "isArchive" JSON data value
//    03 Jul 2013           1.0.19     [jonoble] Parse the "extractToSubdirectory" JSON data value
//    10 Jul 2013           1.0.20     [jonoble] "extractToSubdirectory" fix
//    26 Jul 2013           1.0.21     [jonoble] Removed "isDirectory" function arguments


#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>
#import "DMOAssetDownloadOperation.h"
#import "DMONetworkReachability.h"
#import "DMOAsset.h"

#define kAssetDefaultPlistName  @"asset_defaults"
#define kDefaultNetworkReachabilityMode DMONetworkReachabilityWiFiOnly
#define kDefaultMinFileSpace (10 * 1024 * 1024)

// Version information
#define kDMOAMPSVersion @"AMPS iOS Client Library v1.0.21 (20130726)"

#define kDMOAssetManagerSaveStateFileError  701
#define kDMOAssetManagerRestoreStateFileError 702

// AMPS Test Servers
//#define kAMPSProductionServerURLPreamble @"http://10.198.215.167:8080/amps/api/"
//#define kAMPSProductionServerURLPreamble @"https://amps-temp.tapulous.com/amps/api/"
//#define kAMPSStagingServerURLPreamble @"http://cmsdev.tapulous.com:8080/amps/api/"

#define kAMPSProductionServerURLPreamble @"https://amps.tapulous.com/amps/api/"
#define kAMPSStagingServerURLPreamble @"http://n7vcs1dmttramp01.compliant.disney.private:8080/amps/api/"

#define kServerAPIVersion @"2"

// This flag is deprecated in favor of runtime/client-driven switching
// It shouldn't be used any more in the library build
// Remove here and in AMPSlib-Prefix once tested
#ifdef  USE_STAGING_SERVER
#define kAMPSURLPreamble kAMPSStagingServerURLPreamble
#else
#define kAMPSURLPreamble kAMPSProductionServerURLPreamble
#endif


@protocol DMOAssetManagerDelegateProtocol <NSObject>

@optional
// This delegate method is called when the listasset call is
// completed and the list of assets parsed.
-(void)assetManagerServerListDidFinishLoading;
// this delegate method is called if the listasset call 
// fails to get data, in case caller wants to retry or
// otherwise recover
-(void)assetManagerServerListDidFailWithError:(NSError *)error;

@end

@interface DMOAssetManager : NSObject {
    
    NSString *ampsVersionString;
    NSString *docsDirectory; // Path to directory storing docs
    
    //The following items will be used as arguments for the calls
    // to the web service
    NSString *appName;
    NSString *locale;
    NSString *quality;
    NSString *appVersion;
    NSString *deviceString;
    NSString *sizeString;

  
    
    // THe following NSMutableDictionary contains DMOAsset objects stored by 
    // AssetName as the Key -- this dictionary can be built by either
    // programmatically adding the assets, or by fetching the assets from 
    // the server -- note that these DMAssets are descriptions, the physical
    // files will still need to be fetched
    
    // Once added, an individual asset's properties can be accessed as follows:
    //  DMOAsset *myAsset = [currentLocalAssetCatalog objectForKey:assetNameString];

    NSMutableDictionary *currentLocalAssetCatalog;
        
    // This array contains all keys and values received from the getAssets
    // server call.  Each asset is stored as an DMOAsset object, and there may
    // be multiple versions of each asset
    NSMutableArray *serverAssetCatalog;
    
    DMOAssetDownloader *downloader;
    
    NSInteger maxParallelDownloads;
    
    // Network reachability mode to enforce -- Wifi only or Any
    DMONetworkReachabilityMode networkReachabilityMode;
    
    NSUInteger minFileSpace; // min space left in file system 
  
    BOOL assetCatalogDownloaded;  //We've received the asset catalog from server
@protected
    CGFloat screenWidth;
    CGFloat screenHeight;
    NSOperationQueue *downloadQueue;
    NSMutableData *serverData;
    // internal copy of delegate fetching assetlist so we can report
    // when finished; separate from the delegate we pass to the NSURLConnection
    id listDelegate; 
    // This dictionary contains latest version strings indexed by AssetName
    // e.g.   versionStr = [myAssetMgr.serverAssetVersionCatalog valueForKey:assetName]
    NSMutableDictionary *serverAssetVersionCatalog;
    BOOL buildAssetCatalogAfterParsing;
    NSString *serverURLPreamble; // Path to the AMPS service
}

@property (nonatomic, readonly, retain) NSString *ampsVersionString;
@property (nonatomic, readonly, retain) NSString *docsDirectory;
@property (nonatomic, readonly, retain) NSString *appName;
@property (nonatomic, readwrite, retain) NSString *locale;
@property (nonatomic, readwrite, retain) NSString *quality;
@property (nonatomic, readonly, retain) NSString *appVersion;
@property (nonatomic, readonly, retain) NSString *deviceString;
@property (nonatomic, readonly, retain) NSString *sizeString;
@property (nonatomic, readwrite, assign) CGFloat screenWidth;
@property (nonatomic, readwrite, assign) CGFloat screenHeight;

@property (nonatomic, readwrite, retain) NSMutableDictionary *currentLocalAssetCatalog;
@property (nonatomic, readwrite, retain) NSMutableArray *serverAssetCatalog;

@property (nonatomic, readwrite, retain) DMOAssetDownloader *downloader;
@property (nonatomic, readwrite, assign) NSInteger maxParallelDownloads;
@property (nonatomic, readwrite, assign) DMONetworkReachabilityMode networkReachabilityMode;
@property (nonatomic, readwrite, assign) NSUInteger minFileSpace;

@property (nonatomic, readonly, assign) BOOL assetCatalogDownloaded;


//@property (nonatomic, readwrite, retain) NSMutableData *serverData;
//@property (nonatomic, readwrite, retain) NSOperationQueue *downloadQueue;
//@property (nonatomic, readwrite, retain) NSMutableDictionary *serverAssetVersionCatalog;
//@property (nonatomic, readonly, assign) BOOL buildAssetListAfterParsing;
// internal copy of delegate fetching assetlist so we can report
// when finished
//@property (nonatomic, assign) id<DMAssetManagerDelegateProtocol> listDelegate;



// This version of the init routine autodetects device type and size
-(DMOAssetManager *) initWithHomePath:(NSString *)docsDirPath 
                              appName:(NSString *)myAppname 
                               locale:(NSString *)myLocale 
                              quality:(NSString *)myQuality  
                              version:(NSString *)myAppVersion; 

-(DMOAssetManager *) initWithHomePath:(NSString *)docsDirPath 
                             appName:(NSString *)myAppname
                              locale:(NSString *)myLocale
                             quality:(NSString *)myQuality
                             version:(NSString *)myAppVersion 
                              device:(NSString *)myDevice
                          sizeString:(NSString *)mySizeString;

#pragma mark -
#pragma mark Asset management methods

// Assets can be added to the asset list either automatically via a server
// fetch, or can be explicitly added by the following call:
-(void)addAssetNamed:(NSString *)assetName 
        withFileName:(NSString *)fileName 
              ofType:(DMOAssetDeploymentType)type
         //isDirectory:(BOOL)isDirectory
;

-(DMOAsset *)assetNamed:(NSString *)assetName;


// This returns a DMOAsset object for the named version of the named asset.
// This DMOAsset contains the metadata for the asset on the server.
// NOTE:  this function requires that the server asset list has been
// downloaded, and will return nil if not.
-(DMOAsset *)version:(NSString *)versionString ofAssetNamed:(NSString *)assetName ;

-(void) markAssetNamed:(NSString *)assetName isDownloaded:(BOOL)isDownloaded;

// Removes the named asset from the catalog
// This also removes the asset file if 'removeFile' set to YES
-(void)removeCatalogAssetNamed: (NSString *)assetName removeFile:(BOOL)remove;


// removes the asset file from the file system
// (use this only if not using the asset catalog to track assets)
-(void)removeDownloadedAssetFileNamed: (NSString*)assetFileName;

// Returns whether the asset has been registered in the asset catalog
-(BOOL)isAssetListedInCatalogNamed:(NSString *)assetName;

// Returns whether the asset file is present on the file system
// and is larger than zero bytes (e.g. opened but not written)
// This is always checked on the file system itself
// NOTE:  this assumes that calling app does not move files after
// they are downloaded.
-(BOOL)isAssetFileDownloadedNamed:(NSString *)assetFileName;

// Returns YES if the named file matches the uncompressed file size for 
// the named asset in the catalog.  Also sets isVerified = YES for the asset.
// Will always return no if either the asset record size or file size is 0
// 
// It's the user's responsibility to make sure that the asset record for the
// named asset reflects the version of the asset being downloaded. 
-(BOOL)isAssetFileNamed:(NSString *)assetFileName correctSizeForAssetNamed:(NSString *)assetName;

// Returns YES if file system contains enough free space to fit the uncompressed
// asset file, plus the minimum file space parameter (e.g. at least 10MB of file space)
// If DMOAsset file size parameters are undefined (set to zero), will still compare 
// minimum file storage setting.  Tip:  If you are generating DMOAssets manually and not 
// setting filesizes, override the minimum file storage setting to the size of your 
// largest asset plus desired headroom.
//
// Generally, user should stop downloading assets when this test returns NO.  It is
// independent of the iOS system warning for low disk space.
-(BOOL)isSufficientFileSpaceForAssetNamed:(NSString *)assetName;

// The following selectors are used to save and restore asset catalog status


// Saves current asset catalog state -- this should generally be called 
// whenever applicationWillTerminate, or when application receives memory warnings
-(BOOL)saveLocalAssetCatalogStateToFileAtPath:(NSString *)filePath  withError:(out NSError **)outError;

// After initializing asset manager, check for presence of backup file at filePath
// If the file exists, call this routine:
-(BOOL)restoreLocalAssetCatalogStateFromFileAtPath:(NSString *)filePath  withError:(out NSError **)outError;
// If the asset catalog backup doesn't exist (first launch), then you have the 
// option of loading from a default asset plist.
-(void)loadAssetCatalogDefaultPlist;


#pragma mark - 
#pragma mark Server Interaction methods

// Server Interaction methods

// Retreives assetlist from the AMPS server, building the serverAssetCatalog
// and the serverAssetVersionCatalog used to supply latest version info
// If buildAssetList is set to YES, the list is also copied to 
// the assets dictionary, using the latest version of each asset
//
// If checking for updates, or if you've populated the asset catalog 
// programatically or from backup/default file,  then set buildAssetCatalog to NO.
//
// This is the method for automatically generating a list of downloadable
// assets rather than manually adding assets to the list (see addAssetNamed:)
// This routine returns an NSError if the asset list query can't be sent.

-(BOOL)downloadAssetCatalogFromServerBuildAssetCatalog:(BOOL)buildAssetCatalog delegate:(id)delegate error:(NSError **)error;

// The following routines are used to check whether an asset requires updating
//  This returns a string with the newest version on the server for a given asset.
//  Returns nil if asset not present, or if asset list hasn't been
/// fetched from server.
-(NSString *)latestVersionForAssetNamed:(NSString *)assetName;

// This convenience method adds any new named assets found on the 
// server to the local asset catalog.  Only the highest versions of the new
// assets are added.    Cleanest way to execute a check for updates is to
// call this method after downloading the asset catalog from server
// for an update check.
// (e.g., downloadAssetCatalogFromServerBuildAssetCatalog:NO )
// Then iterate over the updated localAssetCatalog to
// download any asset not downloaded or with
// serverHasNewerVersionForAssetNamed: returning TRUE.

-(void)appendNewServerAssetsToLocalAssetCatalog;



// This compares the current version for the named asset with the 
// latest version available on the server.  Assumes that named asset
// version has been determined/written to the asset record.
//
// NOTE ABOUT ASSET PERSISTENCE:
// It is the caller's responsibility to save and restore a list of the
// asset names, filenames and versions to determine which assets are 
// still missing and which are requiring update upon startup.   Best way to
// do this is probably to write the currentLocalAssetCatalog 
// out to a plist, and restore it on any restarts.
-(BOOL)serverHasNewerVersionForAssetNamed:(NSString *)assetName;
//
// By default, AMPSlib uses the production server
// The user can use this to force use of staging server instead
- (void)useStagingServer:(BOOL)useStagingServer;


#pragma mark - 
#pragma mark Download Management methods

// Downloads are queued, and download status from each download is asynchronously delivered via the DMAssetDownloader notifications.  See DMAssetDownloader.h

// convenience method if you have complete asset data in catalog already
-(void)downloadAssetNamed:(NSString *)assetName 
                 userInfo:(NSDictionary *)info;

// Queue the download for the asset file, unzipping if it is a zip file
-(void)downloadAssetNamed:(NSString *)assetName 
                  Version:(NSString *)version 
              //isDirectory:(BOOL)directory
                 userInfo:(NSDictionary *)info;

// The following routines can pause and restore the queue of file downloads
// Note that these follow NSOperationQueue rules -- operations that are in
// the queue or added to the queue are prevented from starting until Queue
// is resumed, but any operations already running will continue until finished.
-(void) suspendDownloadQueue;

-(void)resumeDownloadQueue;

// Returns the number of pending download operations in the Queue.
// Use with caution, probably not precisely accurate except when it returns 0.
// See NSOperationQueue operationCount property for details,  this is not a
// precise value, as it may change before you use it.

// 3.1.3 Note:   operationCount not supported before iOS 4.0.  This routine
// returns -1 if the operationCount selector is not recognized.
-(NSInteger)pendingDownloads;

// Cancels all downloads in the queue
-(void)stopAllDownloads;

// Default number of concurrent downloads is 2, which can be accommodated
// across EDGE, 3G, or WiFi devices; WiFi only operation can support up to
// 3 downloads.  Set to one if you need downloads to arrive strictly in the
// order in which you have enqueued them.

// If you want to let the OS launch as many concurrent operations as it can,
// pass the system constant  NSOperationQueueDefaultMaxConcurrentOperationCount
-(void)setMaxConcurrentDownloads:(NSInteger)maxDownloads;






@end
