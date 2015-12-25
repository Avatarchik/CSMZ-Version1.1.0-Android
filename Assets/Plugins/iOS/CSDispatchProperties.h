//
//  CSDispatchProperties.h
//  comScore
//

#import <Foundation/Foundation.h>
#import "CSTransmissionMode.h"

#define kLiveTransmissionModeSetting 0
#define kOfflineTransmisionModeSetting 1
#define kSecureModeSetting 2
#define kMeasurementLabelOrderSetting 3

#define kCSSettingChangeType @"CSSettingChangeType"
#define kCSSettingChangeValue @"CSSettingChangeValue"

@interface CSDispatchProperties : NSObject {
    
    CSTransmissionMode liveTransmissionMode_;
    CSTransmissionMode offlineTransmissionMode_;
    bool secure_;
    NSArray *measurementLabelOrder_;
}

- (void) reset;
- (void) copyFrom:(CSDispatchProperties *)props;
- (void) updateSetting:(NSDictionary*)settingChange;

@property (nonatomic, setter = allowLiveTransmission:, assign) CSTransmissionMode liveTransmissionMode;
@property (nonatomic, setter = allowOfflineTransmission:, assign) CSTransmissionMode offlineTransmissionMode;
@property (nonatomic, getter = isSecure, assign) bool secure;
@property (nonatomic, retain) NSArray *measurementLabelOrder;

@end
