//
//  EasyURLCopy.h
//  WData
//
//  Created by bdoig on 9/15/10.
//  Copyright (c) 2010 Disney Mobile. All rights reserved.
//

#import <Foundation/Foundation.h>

#pragma mark -
#pragma mark EasyURLCopyNotifyProtocol

@protocol EasyURLCopyNotifyProtocol<NSObject>
@optional
-(void) downloadForURL:(NSString*)srcURL toURL:(NSString*)dstURL completeWithObject:(id)object tag:(NSInteger)tag;
-(void) downloadForURL:(NSString*)srcURL toURL:(NSString*)dstURL failedWithError:(NSError*)error tag:(NSInteger)tag;
-(void) downloadForURL:(NSString*)srcURL toURL:(NSString*)dstURL hasCompletedPercent:(float)percent tag:(NSInteger)tag;
-(void) downloadForURL:(NSString*)srcURL toURL:(NSString*)dstURL startedWithResponse:(NSURLResponse*)response tag:(NSInteger)tag;
@end

#pragma mark -
#pragma mark EasyURLCopyNotifyDelegate

@interface EasyURLCopyNotifyDelegate : NSObject {
	id<EasyURLCopyNotifyProtocol> delegate;
	NSFileHandle *file;
	NSString* srcURLString;
	NSString* dstURLString;
	long long expectedSize;
	unsigned int statusCode;
	NSInteger tag;
}

@property(readonly, assign) id<EasyURLCopyNotifyProtocol> delegate;
@property(readwrite, assign) NSInteger tag;


-(id) initWithDelegate:(id<EasyURLCopyNotifyProtocol>)d srcURL:(NSString*)src dstURL:(NSString*)dst;
-(void) appendData:(NSData*)data;
-(NSData*) data;
-(void) notifyRequestComplete:(NSURLRequest*)req;
-(void) notifyError:(NSError*)error;
-(void) notifyResponse:(NSURLResponse*)response;
@end

#pragma mark -
#pragma mark EasyURLCopyOperation

typedef enum EnumEasyURLCopyOperationCacheBehavior {
	kEasyURLCopyOperationNormalCache = 0,
	kEasyURLCopyOperationNoCache = 1,
	kEasyURLCopyOperationOnlyCache = 2
} EasyURLCopyOperationCacheBehavior;


@interface EasyURLCopyOperation : NSOperation {
	EasyURLCopyNotifyDelegate *recipient;
	NSDictionary * headers;
	NSMutableURLRequest *request;
	NSString* srcURL;
	NSString* dstURL;
	BOOL finished;
	BOOL running;
	BOOL cancelled;
	EasyURLCopyOperationCacheBehavior cacheBehavior;
	NSURLResponse *responseToCache;
}

@property(readonly, retain) EasyURLCopyNotifyDelegate *recipient;
@property(readwrite, retain) NSDictionary * headers;

-(id) initWithDownload:(NSString*)url destination:(NSString*)d andRecipient:(EasyURLCopyNotifyDelegate*)r cacheBehavior:(EasyURLCopyOperationCacheBehavior)nc;
-(void) connection:(NSURLConnection*)connection didFailWithError:(NSError*) error;
- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response;
-(void) connectionDidFinishLoading:(NSURLConnection*)connection;
-(void) connection:(NSURLConnection*)connection didReceiveData:(NSData*) data;
-(NSCachedURLResponse*) connection:(NSURLConnection*)connection willCacheResponse:(NSCachedURLResponse*)response;
-(void) setFinished;


@end

#pragma mark -
#pragma mark EasyURLCopy

@interface EasyURLCopy : NSObject {
	unsigned int maxConnections;
	NSOperationQueue *queue;
	BOOL isOperating;
	unsigned int currentlyDownloading;
	NSString * userAgent;
	NSString * applicationName;
}

@property(readwrite, assign) unsigned int maxConnections;
@property(readonly, assign) BOOL isOperating;
@property(readonly, assign) unsigned int currentlyDownloading;
@property(readwrite, retain) NSString * userAgent;
@property(readwrite, retain) NSString * applicationName;

+(EasyURLCopy*) sharedCopier;
+(void)cancelDownloadsFor:(id<EasyURLCopyNotifyProtocol>)delegate;

-(id) init;

-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate;
-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate noCache:(BOOL)nc;
-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate noCache:(BOOL)nc highPriority:(BOOL)b;

-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate tag:(NSInteger)tag;
-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate noCache:(BOOL)nc tag:(NSInteger)tag;
-(void) startDownloading:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate noCache:(BOOL)nc highPriority:(BOOL)b tag:(NSInteger)tag;

-(void) startDownloading:(NSString *)url destination:(NSString*)dst withDelegate:(id <EasyURLCopyNotifyProtocol>)delegate andHeaders:(NSDictionary *)headers tag:(NSInteger)tag;;
-(void) startDownloading:(NSString *)url destination:(NSString*)dst withDelegate:(id <EasyURLCopyNotifyProtocol>)delegate andHeaders:(NSDictionary *)headers noCache:(BOOL)nc tag:(NSInteger)tag;
-(void) startDownloading:(NSString *)url destination:(NSString*)dst withDelegate:(id <EasyURLCopyNotifyProtocol>)delegate andHeaders:(NSDictionary *)headers noCache:(BOOL)nc highPriority:(BOOL)b tag:(NSInteger)tag;

-(void) startDownloadingOnlyFromCache:(NSString*)url destination:(NSString*)dst withDelegate:(id<EasyURLCopyNotifyProtocol>)delegate;

-(void) observeValueForKeyPath:(NSString*)keypath ofObject:(id)object change:(NSDictionary*)change context:(void*)context;
-(void) cancelAllPendingDownloads;
-(void) cancelAllPendingDownloadsForDelegate:(id<EasyURLCopyNotifyProtocol>)delegate;
@end




