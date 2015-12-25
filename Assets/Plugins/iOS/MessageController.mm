//
//
//  Created by Catalin Marcu on 11/11/13.
//
//

#import "MessageController.h"

#import "ExtendedUnityAppController.h"

@implementation MessageController

UIAlertView* currentAlert = nil;

- (id) init
{
	self = [super init];
    
    currentAlert = nil;
    
	return self;
}

-(void) dealloc
{
    currentAlert = nil;
    
    [super dealloc];
}

- (void) sendEvent: (NSString* ) event listener:(NSString*) listener parameter:(NSString*) param
{
    UnitySendMessage( MakeStringCopy(listener), MakeStringCopy(event), MakeStringCopy(param));
}

- (void)showMessage:(NSString *)message title:(NSString*)title button1:(NSString*)button1 button2:(NSString*)button2 button3:(NSString*)button3
{
    if (currentAlert != nil) {
        [currentAlert dismissWithClickedButtonIndex:0 animated:NO];
    }
    
	UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title message:message
                                                   delegate:self cancelButtonTitle:button1 otherButtonTitles:nil];
    if (button2 != nil) {
        [alert addButtonWithTitle:button2];
    }
    
    if (button3 != nil) {
        [alert addButtonWithTitle:button3];
    }
    
    [alert show];
    [alert release];
    
    currentAlert = alert;
}

- (void)alertView:(UIAlertView *) alertView clickedButtonAtIndex:(NSInteger) buttonIndex
{
    currentAlert = nil;
    
    [self sendEvent:@"ButtonPressed" listener:@"NativeMessagesSystem" parameter:[NSString stringWithFormat:@"%d", buttonIndex]];
}


- (void)scheduleNotification:(NSString *)message title:(NSString*)title showTime:(long)seconds
{
    NSLog(@"Show notification: %@        |       %@      |       %ld", message, title, seconds);
    NSArray *localNotifications = [[UIApplication sharedApplication] scheduledLocalNotifications];
    
    //cancel all notifications of the same type
    for (int i = 0; i < [localNotifications count]; ++i)
    {
        UILocalNotification *localNotification = [localNotifications objectAtIndex:i];
        
        if ([localNotification.alertBody isEqualToString:message]) {
            [[UIApplication sharedApplication] cancelLocalNotification:localNotification];
        }
    }
    
    UILocalNotification* localNotification = [[UILocalNotification alloc] init];
    localNotification.fireDate = [NSDate dateWithTimeIntervalSinceNow:seconds];
    localNotification.alertBody = message;
    localNotification.alertAction = title;
    localNotification.timeZone = [NSTimeZone defaultTimeZone];
    localNotification.applicationIconBadgeNumber = [[UIApplication sharedApplication] applicationIconBadgeNumber] + 1;
    
    [[UIApplication sharedApplication] scheduleLocalNotification:localNotification];
}

- (void)cancelNotifications:(NSString *)message
{
    NSArray *localNotifications = [[UIApplication sharedApplication] scheduledLocalNotifications];
    
    //cancel all notifications of the same type
    for (int i = 0; i < [localNotifications count]; ++i)
    {
        UILocalNotification *localNotification = [localNotifications objectAtIndex:i];
        
        if ([localNotification.alertBody isEqualToString:message]) {
            [[UIApplication sharedApplication] cancelLocalNotification:localNotification];
        }
    }
}


static MessageController* messController = NULL;

extern "C" {
    
    void Native_ShowMessage(char* title, char* message, char* button1, char* button2, char* button3)
    {
        NSLog(@"Show message");
        
        if (messController == NULL) {
            messController = [[MessageController alloc] init];
        }
        
        NSString* buttonText1 = GetStringParam(button1);
        if ([buttonText1 isEqualToString:@""]) {
            buttonText1 = nil;
        }
        
        NSString* buttonText2 = GetStringParam(button2);
        if ([buttonText2 isEqualToString:@""]) {
            buttonText2 = nil;
        }
        
        NSString* buttonText3 = GetStringParam(button3);
        if ([buttonText3 isEqualToString:@""]) {
            buttonText3 = nil;
        }
        
        [messController showMessage:GetStringParam(message)
                              title:GetStringParam(title)
                            button1:buttonText1
                            button2:buttonText2
                            button3:buttonText3];
    }
    
    void Native_ScheduleNotification(char* title, char* message, long seconds)
    {
        NSLog(@"Schedule notification");
        
        if (messController == NULL) {
            messController = [[MessageController alloc] init];
        }
        
        [messController scheduleNotification:GetStringParam(message)
                                       title:GetStringParam(title)
                                    showTime:seconds];
    }
    
    void Native_CancelNotifications(char* message)
    {
        NSLog(@"Cancel notifications" );
        
        if (messController == NULL) {
            messController = [[MessageController alloc] init];
        }
        
        [messController cancelNotifications:GetStringParam(message)];
    }
    
    char* Native_GetRateLink(char* appId)
    {
        NSString *reviewURL = @"";
        
        // iOS 7 needs a different template
        if ([[[UIDevice currentDevice] systemVersion] floatValue] >= 7.0) {
            reviewURL = @"itms-apps://itunes.apple.com/app/idAPP_ID";
        }
        else {
            reviewURL = @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";
        }

        return MakeStringCopy([reviewURL stringByReplacingOccurrencesOfString:@"APP_ID" withString:GetStringParam(appId)]);
    }
    
    char* Native_GetLocaleCountry()
    {
        return MakeStringCopy([[NSLocale currentLocale] objectForKey:NSLocaleCountryCode]);
    }
    
    float Native_GetScreenWidth()
    {
        return [UIScreen mainScreen].bounds.size.width;
    }
    
    float Native_GetScreenHeight()
    {
        return [UIScreen mainScreen].bounds.size.height;
    }
}

@end
