//
//  CSCacheFlusher.h
//  comScore

#import <Foundation/Foundation.h>

@class CSCore;

@interface CSCacheFlusher : NSObject {
    CSCore *core;
    NSTimer *timer;
    bool isForeground;
}

-(id) initWithCore:(CSCore *)aCore;
-(void) start;
-(void) stop;
-(void) reset;
@end