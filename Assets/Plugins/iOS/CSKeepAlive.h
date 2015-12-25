//
//  CSKeepAlive.h
//  comScore

#import <Foundation/Foundation.h>

@class CSCore;

@interface CSKeepAlive : NSObject {
    
    NSTimer *timer_;
    CSCore *core;
    long long  timeout_;
    long long nextKeepAliveTime_;
    long long currentTimeout_;
    bool foreground_;
    bool timerSet_;
}

-(id) initWithCore:(CSCore *)aCore timeout:(long long)timeout;
- (void) sendKeepAlive;
- (void) reset;
- (void) reset:(long long)timeout;
- (void) processKeepAlive:(BOOL)saveInCache;
- (void) start:(long long)millis;
- (void) stop;


@end
