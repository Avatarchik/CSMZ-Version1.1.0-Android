//
//  DMOAssetDownloadOperation.h
//  AMPS iOS Client Library
//
//  Created by Mike Meyer on 4/13/11.
//  Copyright 2011 Disney Mobile. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "DMOAssetDownloader.h"
#import "DMONetworkReachability.h"


@interface DMOAssetDownloadOperation : NSOperation <DMOAssetDownloaderNotifyProtocol> {
    
    DMOAssetDownloadItem *downloadItem;
    DMONetworkReachabilityMode networkReachabiltyMode;
@protected
    BOOL isDone;
    BOOL sendErrNotify;
   
    
}


@property (nonatomic, readonly) DMOAssetDownloadItem *downloadItem;
@property (nonatomic, readonly) DMONetworkReachabilityMode networkReachabilityMode;
@property (nonatomic, readonly) BOOL isDone;
@property (nonatomic, readwrite, assign) BOOL sendErrNotify;



-(id)initWithDownloadItem:(DMOAssetDownloadItem *)item networkReachabilityMode:(DMONetworkReachabilityMode)mode;


@end
