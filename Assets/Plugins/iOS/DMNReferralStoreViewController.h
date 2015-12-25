//
//  DMNReferralStoreViewController.h
//  DMNReferralStore
//  libReferralStore.a
//
//  Copyright (c) 2012 Disney. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "DMOAnalytics.h"

@class DMNReferralStoreConnectionManager;
@protocol DMNReferralStoreViewControllerDelegate;


#pragma mark -
#pragma mark DMNReferralStoreConnectionDelegate PROTOCOL
/*!
 @abstract   Conform to DMNReferralStoreConnectionDelegate protocol for handling url response.
 @discussion You can use this delegate pattern to get the response once the url connection is
 successfully completed or an error message if the connection fails. This protocol is internal
 to the functioning of the DMNReferralStoreConnectionDelegate, and not required (nor recommended)
 for clients of this library to implement.
 */

@protocol DMNReferralStoreConnectionDelegate<NSObject>
@optional

/*!
 @abstract Tells the delegate that the connection was successful.
 @param connection DMNReferralStoreConnectionManager object
 @param response Response received
 */
- (void)storeConnection:(DMNReferralStoreConnectionManager*)connection didSucceedWithData:(NSData*)response;

/*!
 @abstract   Tells the delegate that the connection failed.
 @param      connection DMNReferralStoreConnectionManager object
 @param      error Error message
 */
- (void)storeConnection:(DMNReferralStoreConnectionManager*)connection didFailWithError:(NSError*)error;
@end


/*!
   Disney Referral Store Library
 */

/*!
   @abstract DMNReferralStoreViewController is your Referral Store Controller.
 */
@interface DMNReferralStoreViewController : UIViewController<UIWebViewDelegate, DMNReferralStoreConnectionDelegate, UIScrollViewDelegate> {
}

/*!
 @property   _configJson
 @abstract   Config JSON
 */
@property (nonatomic) NSString* configJson;

/*!
 @property   _webViewLoaded
 @abstract   Webview finished loading
 */
@property (nonatomic) BOOL webViewLoaded;

/*!
 @property   _bootStrapUrl
 @abstract   Bootstrap URL
 */
@property (nonatomic) NSString* bootStrapUrl;

/*!
 @property   _gcsUrl
 @abstract   GCS URL
 */
@property (nonatomic) NSString* gcsUrl;

/*!
 @property   _disneyId
 @abstract   DisneyID
 */
@property (nonatomic) NSString* disneyId;

/*!
 @property   _dmoId
 @abstract   DMOID
 */
@property (nonatomic) NSString* dmoId;

/*!
 @property   referralStoreStatusBarHidden
 @abstract   Unsupported: By default this the status bar is hidden on iOS7 devices. If you want to display the status
                            bar you can set referralStoreStatusBarHidden=NO and the status bar will be visible.
 */
@property (nonatomic, assign) BOOL referralStoreStatusBarHidden;

@property (nonatomic,strong) NSString* appId;

/*!
   @property   delegate
   @abstract   Set the delegate object for the Referral Store View Controller. 
               The delegate will receive delegate messages when the player dismisses 
               the Referral Store user interface.
 */
@property (nonatomic, unsafe_unretained) id<DMNReferralStoreViewControllerDelegate> delegate;


/*!
   @method     initWithValue:
   @abstract   Referral store view controller initializer.
   @param      customDataDictionary key-value pair data keys are @"boot_url", @"gcs_url", @"user_id", @"device_id", 
               @"dmo_user_id", @"disney_id"
   @return     Returns an initialized DMNReferralStoreViewController object.
 */
- (id)initWithValue:(NSDictionary*)customDataDictionary;
+ (NSString *) version;
@end


#pragma mark -
#pragma mark DMNReferralStoreViewControllerDelegate PROTOCOL
/*!
   @abstract   Called when the user is done interacting with the view controller’s content
   @discussion The DMNReferralStoreViewControllerDelegate protocol is implemented by 
               delegates of the DMNReferralStoreViewController class. The delegate is called 
               when the user dismisses the Referral Store interface.
 */

@protocol DMNReferralStoreViewControllerDelegate<NSObject>
@required
/*!
   @abstract   Called when the player dismisses the Referral Store user interface.
   @discussion Delegate should dismiss the Referral Store view controller.
   @param      referralStoreViewController View controller the user finished interacting with.
 */
- (void)referralStoreViewControllerDidFinish:(DMNReferralStoreViewController *)referralStoreViewController;
@end

