//
//
//  Created by Catalin Marcu on 11/11/13.
//
//

#import <Foundation/Foundation.h>

@interface MessageController : NSObject<UIAlertViewDelegate>

- (void)showMessage:(NSString *)message title:(NSString*)title button1:(NSString*)button1 button2:(NSString*)button2 button3:(NSString*)button3;

- (void)scheduleNotification:(NSString *)message title:(NSString*)title showTime:(long)seconds;
- (void)cancelNotifications:(NSString *)message;

@end
