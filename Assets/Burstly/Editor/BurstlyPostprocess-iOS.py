#encoding: utf-8
#!/usr/bin/python
from mod_pbxproj import XcodeProject
import sys
import os

FRAMEWORKS_TO_ADD = '''
    AddressBook.framework
    AddressBookUI.framework
    AdSupport.framework
    AudioToolbox.framework
    AVFoundation.framework
    CFNetwork.framework
    CoreGraphics.framework
    CoreLocation.framework
    CoreMotion.framework
    CoreTelephony.framework
    EventKit.framework
    EventKitUI.framework
    Foundation.framework
    iAd.framework
    libsqlite3.dylib
    libxml2.dylib
    libz.dylib
    MapKit.framework
    MediaPlayer.framework
    MessageUI.framework
    MobileCoreServices.framework
    OpenAL.framework
    OpenGLES.framework
    PassKit.framework
    QuartzCore.framework
    Security.framework
    StoreKit.framework
    SystemConfiguration.framework
    UIKit.framework
    '''

OTHER_LDFLAGS = '''
    -ObjC
    -weak_framework AdSupport
    -weak_framework PassKit
    -weak_framework CoreTelephony
    '''

def main(project_path):
    project = XcodeProject.Load(project_path)
    frameworks = FRAMEWORKS_TO_ADD.strip().split('\n')
    framework_group = project.get_or_create_group('Frameworks')
    
    for framework in frameworks:
        framework = framework.strip()
        if framework.endswith('.framework'):
            framework_path = 'System/Library/Frameworks/{0}'.format(framework)
        elif framework.endswith('.dylib'):
            framework_path = 'usr/lib/{0}'.format(framework)
        project.add_file_if_doesnt_exist(framework_path, parent = framework_group, tree = 'SDKROOT')
    
    project.add_other_ldflags([x.strip() for x in OTHER_LDFLAGS.strip().split('\n')])
    
    project.backup()
    project.save()


if __name__ == '__main__':
    script, path = sys.argv
    path = os.path.join(path, 'Unity-iphone.xcodeproj', 'project.pbxproj')
    main(path)
