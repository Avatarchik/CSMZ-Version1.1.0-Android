//
//  DMOAsset.h
//  AMPS iOS Client Library
//
//  Created by Mike Meyer on 4/5/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//

#import <Foundation/Foundation.h>

#define kFileSizeUnknown 0


// This type represents how the asset content gets used
// in the target game
typedef enum {
    AssetTypeEmbedded = 0,  // replacement for bundle asset
    AssetTypeRequired,  // Needed to play game
    AssetTypeOptional,  // optional download (bonus content)
    AssetTypePremium    // Paid content or content requiring unlock
}DMOAssetDeploymentType;

// NOTE:  if adding any instance vars to this object, be sure to
// update the description and copyWithZone methods!!!!

@interface DMOAsset : NSObject <NSCopying>
{
    // This object stores the asset-specific properties;
    // properties such as device type, etc. that don't change
    // across assets are stored in the asset manager
    // TODO:  should have more properties to match whats on server
    NSString *assetName;
    NSString *assetFileName; // filepath relative to user's document root path
    
    DMOAssetDeploymentType deploymentType;
    NSString *userAssetType;  // User-defined asset type, (e.g. "level data")
    
    NSString *assetVersion;
    NSString *assetLocale;
    NSString *assetQuality;
    
    BOOL isArchive;       // is an archive (currently Zip Archive)
    BOOL doExtractToSubdirectory; // AMPS should extract the archive after it is downloaded
    //BOOL isCompoundAsset; //contains other assets, unpack as directory
    BOOL isInstalled;  // File has been downloaded to device;  this
    // may be set as new files come in, or marked
    // as existing files are found.
    BOOL isVerified;   // stored file size verified to match expected size
    
    // The following file size properties are generally read from
    // the server rather than explicitly set by the user,
    // and can be used to determine whether file arrived intact.
    // Set these to kFileSizeUnknown (0) if you are manually building
    // the catalog and don't know the file size.
    // Note that if you don't know the expected file size, the file size
    // checking will still make sure that it isn't a zero byte file.
    //
    // "fileSizeCompressed" would more accurately be "fileSizeOnServer"
    // "fileSizeUncompressed" is DEPRECATED, web service doesn't calculate
    // it.
    NSUInteger fileSizeCompressed;  // size in bytes (up to 4096 MB)
    NSUInteger fileSizeUncompressed;// size in bytes --DEPRECATED
}

@property (nonatomic, readwrite, retain) NSString *assetName;
@property (nonatomic, readwrite, retain) NSString *assetFileName;

@property (nonatomic, readwrite, assign) DMOAssetDeploymentType deploymentType;
@property (nonatomic, readwrite, retain) NSString *userAssetType;

@property (nonatomic, readwrite, retain) NSString *assetVersion;
@property (nonatomic, readwrite, retain) NSString *assetLocale;
@property (nonatomic, readwrite, retain) NSString *assetQuality;

@property (nonatomic, readwrite, assign) BOOL isArchive;
@property (nonatomic, readwrite, assign) BOOL doExtractToSubdirectory;
//@property (nonatomic, readwrite, assign) BOOL isCompoundAsset;
@property (nonatomic, readwrite, assign) BOOL isInstalled;
@property (nonatomic, readwrite, assign) BOOL isVerified;

@property (nonatomic, readwrite, assign) NSUInteger fileSizeCompressed;
// DEPRECATED -- this property is not calculated in server, and isn't
// helpful in the library code.
@property (nonatomic, readwrite, assign) NSUInteger fileSizeUncompressed;

- (NSString *) description;

@end
