//
//  DMOAssetDownloadItem.h
//  Downloader
//
//  Created by Mike Meyer on 4/18/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface DMOAssetDownloadItem : NSObject {
    
    NSURL*          sourceURL;      //create with URLWithString
    NSURL*          destinationURL; //create with FileURLWithPath
    //  especially if an archive, is it single file or directory?
  //  BOOL            isDirectory;
    BOOL            isArchive;
    BOOL            checkFileSize;
    
    // This is the expected file size from the download on completion
    // For zips, this is the size before unzipping (fileSizeCompressed)
    // It's really important to set the right value for this when building
    // the download item, as of this writing the JSON field doesn't set
    // both fields, Asset Manager does it for you.
    NSUInteger      expectedFileSize;
    
    NSDictionary*   userInfo;
    
}

-(id)initWithSourceURL:(NSURL *)srcURL //full-on URLWithString
        destinationURL:(NSURL *)destURL //created using FileURLWithPath
          // isDirectory:(BOOL)isDir
isArchive:(BOOL)archive
doExtractToSubdirectory:(BOOL)extractToSubdirectory
              userInfo:(NSDictionary *)context;

// This variant of the init uses the expected file size information
// to validate a file before unzipping -- this helps us recover from
// JSON error messages or empty zip archives

-(id)initWithSourceURL:(NSURL *)srcURL 
        destinationURL:(NSURL *)destURL 
          // isDirectory:(BOOL)isDir
isArchive:(BOOL)archive
doExtractToSubdirectory:(BOOL)extractToSubdirectory
         checkFileSize:(BOOL)checkSize
      expectedFileSize:(NSUInteger)expectedSize
              userInfo:(NSDictionary *)context;

-(NSString *)description;

@property (nonatomic, readonly) NSURL *sourceURL;
@property (nonatomic, readonly) NSURL *destinationURL;
//@property (nonatomic, readonly) BOOL isDirectory;
@property (nonatomic, readonly) BOOL isArchive;
@property (nonatomic, readonly) NSDictionary *userInfo;
@property (nonatomic, readonly) BOOL checkFileSize;
@property (nonatomic, readonly) NSUInteger expectedFileSize;
@property (nonatomic, readonly) BOOL doExtractToSubdirectory;

@end
