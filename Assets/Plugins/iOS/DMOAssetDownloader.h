
//
//  DMOAssetDownloader.h
//  AMPS iOS Client Library
//
//
//  Created by Mike Meyer on 3/22/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DMOAssetDownloadItem.h"

#import "EasyURLCopy.h"

#define kAMPSErrorDomain @"DMO_AMPS_Error"

#define kDMOAssetDownloaderFileOpenError 666
#define kDMOAssetDownloaderFileWrongSizeError 667

typedef enum {
    DownloadWaiting = 0,//not yet started
    DownloadInProgress, // downloading
    DownloadFinished,   // file has been downloaded
    DownloadUnpacking,  // file is unzipping (archive only)
    DownloadSuccessful, // file is downloaded, unzipped if needed, and
                        // matches expected file size
    DownloadCancelled,  // download was cancelled
    DownloadFailed      // download failed, see NSError for details
} DMODownloadStatus;

// Download is done asychronously.  Two methods are provided for 
// communicating download progress and/or status to the calling program.
// This is done via either the delegate methods defined below in 
// DMDownloaderNotifyProtocol (for a single delegate),  or for 
// those cases needing multiple listeners, a set of NSNotifications 
// on the default NSNotification center.
//

// NSNotification names for download notifications

// The underlying code monitors system reachability and returns a failure if network becomes 
// unreachable.  It is the caller's responsibility to determine whether the current reachability
// is appropriate, (e.g. initiating a large download over WWLAN, requiring a WiFi connection, etc.)
// before initiating a download.

extern NSString *DownloadSucceededNotification;
extern NSString *DownloadFailedNotification;
extern NSString *DownloadCancelledNotification;
extern NSString *DownloadProgressUpdateNotification;
extern NSString *DownloadStartedNotification;
// For all notifications, the object passed is the downloadItem.
// For Failed notification, user info dictionary contains a key ErrorMsgKey with
// the NSError object.
// For progress update notification,  user info dictionary contains a key
// PercentKey, with the object an NSString with float value between 0.0 and 1.0


//Delegate methods
@protocol DMOAssetDownloaderNotifyProtocol<NSObject>
@optional
-(void)downloadDidStart:(DMOAssetDownloadItem *)download;
-(void)downloadDidFinishLoading:(DMOAssetDownloadItem *)download;
-(void)download:(DMOAssetDownloadItem *)download didFailWithError:(NSError *)error;
-(void)downloadDidCancel:(DMOAssetDownloadItem *)download;
-(void)download:(DMOAssetDownloadItem *)download didCompletePercentage:(float)percent;
-(void)download:(DMOAssetDownloadItem *)download didReceiveResponse:(NSURLResponse *)response;

@end



@interface DMOAssetDownloader : NSObject <EasyURLCopyNotifyProtocol>{
    
    id <DMOAssetDownloaderNotifyProtocol>delegate;   // receives the delegate methods
    DMOAssetDownloadItem *item;  // Object containing to/from info about download
                                 // as well as info for how to handle file when
                                 // it arrives.
    DMODownloadStatus  currentStatus; 
    float percentageDownloaded;      // Value between 0.0f and 1.0f
     
}

@property (nonatomic, readwrite, assign) id<DMOAssetDownloaderNotifyProtocol> delegate;
@property (nonatomic, readwrite, retain) DMOAssetDownloadItem *item;
@property (nonatomic, readonly) DMODownloadStatus   currentStatus;
@property (nonatomic, readonly) float     percentageDownloaded;



// Init method

-(id) initWithItem: (DMOAssetDownloadItem *)downloadItem delegate:(id)theDelegate;




// download controls
-(BOOL)startDownload:(out NSError **)error;

-(void)cancelDownload;   // Stops a download in progress; incomplete file is NOT removed



@end
