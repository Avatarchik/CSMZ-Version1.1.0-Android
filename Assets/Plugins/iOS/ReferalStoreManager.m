//
//  ReferalStoreManager.m
//  ReferalStore
//
//  Created by Jose Antonio Victoria on 30/04/14.
//  Copyright (c) 2014 Jose Antonio Victoria. All rights reserved.
//

#import "UnityAppController.h"
#import "ReferalStoreManager.h"



@interface ReferalStoreManager ()

@property (nonatomic, strong) DMNReferralStoreViewController *referalStoreVC;
@property (nonatomic, strong) UINavigationController *nvc;
@property (nonatomic, strong) UIViewController *rootViewController;
@property (nonatomic, copy) NSString *callBackGameObjectName;

@end


@implementation ReferalStoreManager

+ (ReferalStoreManager *) sharedInstance
{
	static  ReferalStoreManager *instance = nil;
	@synchronized(self)
	{
		if (!instance) {
			instance = [ReferalStoreManager new];
		}
	}
	return instance;
}

- (id)init
{
	self = [super init];
	if (self) {
		//_rootViewController = [[[UIApplication sharedApplication].delegate window] rootViewController];
		_rootViewController = ((UnityAppController*)[UIApplication sharedApplication].delegate).rootViewController;
	}
	return self;
}


- (void)showReferalInViewController:(UIViewController *)vc delegate:(id<DMNReferralStoreViewControllerDelegate>)delegate
{
	_referalStoreVC = [[DMNReferralStoreViewController alloc] init];
	_referalStoreVC.delegate = delegate;
	
	NSString *identifier = [[NSBundle mainBundle] bundleIdentifier];
	if (identifier != nil && ![identifier isEqualToString:@""]) {
		_referalStoreVC.appId = identifier;
	}
	
	
	_nvc = [[UINavigationController alloc] initWithRootViewController:_referalStoreVC];
	[_nvc.view setClipsToBounds:YES];
	//[viewC presentViewController:_nvc animated:YES completion:nil];
	
	[vc addChildViewController:_nvc];
	[_nvc.view setAutoresizingMask:(UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight)];
	[vc.view addSubview:_nvc.view];
	
	CGRect fromFrame = CGRectOffset(vc.view.bounds, 0, CGRectGetHeight(vc.view.bounds));
	CGRect toFrame = vc.view.bounds;
	
	[_nvc.view setFrame:fromFrame];
	[UIView animateWithDuration:0.4f animations:^{
		[_nvc.view setFrame:toFrame];
	}];
}

- (void)showReferalView
{
	[self showReferalInViewController:_rootViewController delegate:self];
}

- (void)setAppId:(NSString*)appID
{
	if (appID != nil) {
		//_referalStoreVC.appID = _appID;
	}
}

- (void)setCallbackObjectName:(NSString*)name
{
	self.callBackGameObjectName = name;
}

- (void)unsetCallbackObjectName:(NSString*)name
{
	if ([self.callBackGameObjectName isEqualToString:name]) {
		self.callBackGameObjectName = nil;
	}
}

#pragma mark Private

- (void)_notifyReferalDismissed
{
	if (_callBackGameObjectName) {
		UnitySendMessage([_callBackGameObjectName UTF8String], "ReferallCallback", [@"ReferalDismissed" UTF8String]);
	}
}

- (void)_dismissReferalInViewController:(UIViewController*)viewC
{
	_referalStoreVC.delegate = nil;
	_referalStoreVC = nil;
	
	CGRect toFrame = CGRectOffset(_nvc.view.frame, 0, CGRectGetHeight(_nvc.parentViewController.view.frame));
	[UIView animateWithDuration:0.4f animations:^{
		[_nvc.view setFrame:toFrame];
	} completion:^(BOOL finished) {
		[_nvc.view removeFromSuperview];
		[_nvc removeFromParentViewController];
		_nvc = nil;
		
		[self _notifyReferalDismissed];
	}];
}

#pragma mark DMNReferralStoreViewControllerDelegate

- (void)referralStoreViewControllerDidFinish:(DMNReferralStoreViewController *)referralStoreViewController
{
	[self _dismissReferalInViewController:_rootViewController];
}

@end
