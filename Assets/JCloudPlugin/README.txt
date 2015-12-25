------------------------------------------
iCloud for Unity
v2.4
Codename: JCloudPlugin
Product Page: http://www.jemast.com/unity/icloud-for-unity/
Copyright (c) 2011-2013 jemast software.
------------------------------------------


iCloud for Unity is a Unity plugin for iOS and Mac OS deployment targets. It allows you to store documents and data (in other words, files and preferences) in the cloud for easy and reliable sharing between your iOS and Mac OS devices. iCloud for Unity provides you with a simple, intuitive and straight-forward application programming interface to manipulate files and folders in the cloud as well as key-value storage in the cloud. Because it implements fall backs for unsupported platforms, you can use this plugin for all your deployment targets and it will detect that iCloud is either unsupported or unavailable and will rely on local storage to continue to provide its services.

We are providing you with two interfaces to implement iCloud storage. On the one hand, JCloudDocument is used to manipulate files and folders in the cloud with methods that matches closely File.IO and Directory.IO using straightforward function names. On the other hand, JCloudData is used to store user/player preferences in the same manner as PlayerPrefs.

System Requirements: To use iCloud for Unity for iOS, you need a Basic or Pro license for Unity iOS. To use iCloud for Unity for Mac OS, you are required to have a Unity Pro license. This plugin has not been tested on versions of Unity earlier than 3.4 and therefore it is recommended that you update your project to at least this version before you proceed with using it.

Target Requirements: iCloud for Unity will use cloud storage on devices using iOS 5.0+ or Mac 10.7.2+ with an active iCloud account. For all other platforms or operating system versions, it will fall back to local storage.

Important Note: You should be extra careful with what you decide to store in the cloud. You should only store documents and data that your users will benefit from being shared between devices. Any file or content that can be reproduced between devices without sharing should not be put in the cloud and should remain local. Reckless usage of iCloud storage could prevent you from getting your app on the iOS App Store or Mac App Store. Moreover, your users are more likely to delete your application if they notice it uses an unnecessarily large amount of their limited iCloud storage space.



------------------------------------------
Plugin Documentation, Reference & Examples
------------------------------------------


For product documentation, please browse to: http://www.jemast.com/unity/icloud-for-unity/documentation/

For product reference, please browse to: http://www.jemast.com/unity/icloud-for-unity/reference/

For code examples, please browse to: http://www.jemast.com/unity/icloud-for-unity/code-examples/

For migration guide from v1.x to x2.x, please browse to : http://www.jemast.com/unity/icloud-for-unity/support/



------------------------------------------
About Tutorial Project
------------------------------------------


Included in this project are two demonstration scenes.

Tutorial Scene is a quick first-person demo where you can move around a plane and interact with 3 colored cubes by pushing them around the place. It uses JCloudDocument to implement a basic load/save game mechanism that will save your position and velocity as well as the cubes' positions and velocities. It also uses JCloudDictionary to track the last device that has been used to play the game, the last time the game was played and the count of games played.

Testbed Scene is a technical scene we used to test each and every of our functions and methods with an unpleasant but exhaustive GUI. It doesn't really serve demonstration purposes but we are leaving it there as some may find this useful when testing iCloud synchronization.


------------------------------------------
About Plugin Xcode Source
------------------------------------------


If you want to modify the plugin Objective-C source itself, the Xcode Project is included in an archive named "Xcode Project.zip" in the Source subfolder of JCloudPlugin folder. Please note that if you modify the source and rebuild it, you need to build both for Mac OS and iOS targets as this is not automatic. You must copy the newly generated library to your Unity project under the Plugins directory (and specifically its iOS subfolder for the iOS library).


------------------------------------------
Help & Support
------------------------------------------


If you need any additional help, please review our product page at http://www.jemast.com/unity/icloud-for-unity/

Feel free to contact us at contact@jemast.com for personal support


-------------------------------------------
Release Notes
-------------------------------------------

-------------------
v2.4 (10/08/2013)
-------------------

- NEW: File changes monitoring (preview)
- FIX: Better compatibility with Unity 4.2 new platforms
- FIX: Removed all instances of foreach
- FIX: Fixed various potential pitfalls

-------------------
v2.3.2 (07/24/2013)
-------------------

- FIX: Fixed directory copy & move not working and returning a native error

-------------------
v2.3 (07/11/2013)
-------------------

- NEW: iCloud for Unity is now available in separate iOS and OS X editions. Full edition remains unchanged in terms of functionalities and still provides access to full source code.
- FIX: Fixed directory listing causing exceptions leading to crashes

-------------------
v2.2 (06/13/2013)
-------------------

- NEW: Updated documentation, examples, reference, tutorials with latest APIs
- FIX: Fixed file conflict resolution methods incorrectly returning "InvalidPlatform" instead of "CloudUnavailable" on platforms where iCloud is unavailable
- FIX: Fixed automatic document moving to iCloud so that it correctly creates intermediate directories
- FIX: Fixed fallback JCloudData not being pushed to iCloud when user enables iCloud on iOS 5.0 targets (iOS 5.1+ is not affected by this issue)

-------------------
v2.1.1 (04/01/2013)
-------------------

- FIX: Picking the current version of a file to resolve a conflict does not raise an exception anymore

-------------------
v2.1 (04/01/2013)
-------------------

- NEW: New APIs for conflict resolution for files (check for conflicts, read versions and pick a version)
- NEW: Operations will now return a detailed error code allowing code simplification and better retries
- FIX: Better handling of NSMetadataQuery reducing the chances of query failure under certain conditions
- FIX: Writing to files does not require them to be downloaded (WARNING: writing to a non-downloaded but "known" file version does not create a conflict ; "known" means the device had a chance to download file metadata from iCloud)
- FIX: Checking for existence and getting modification date will trigger a file download (though not necessary, this will allow early downloads even if you don't attempt to read)
- FIX: JCloudData SetString will now delete the key-value pair if value is null (to match PlayerPrefs behavior - thanks to Alexandre for reporting this)
- FIX: JCloudData now uses lazy initialization and change persistence so you can watch for changes on app launch (make sure registering for changes is your first call to JCloudData prior to anything else)
- FIX: Renamed internal Objective-C class to avoid name collision on OS X causing crashes

-------------------
v2.0.2 (03/11/2013)
------------------

- Fixed a potential issue when trying to write to a file that was not yet downloaded from iCloud.

-------------------
v2.0 (02/27/2013)
-------------------

- Rewrote at least 50% of the plugin, numerous BREAKING CHANGES. Please read changes and migration guide.
- Removed synchronous JCloudDocument API, renamed JCloudDocumentAsync to JCloudDocument as this is now the default and only API for file access.
- Removed JCloudDictionary API as this was obsolete (Apple was rejecting apps using this API). If JCloudData doesn't fit, use file storage.
- Removed JCloudRoutine API. You still need coroutines but just need to yield until async operations are finished. See code examples and migration guide.
- Renamed JCloudDocumentAsyncOperation to JCloudDocumentOperation and changed its structure. See migration guide.
- Fixed late downloading issues on iOS 6.0+ which could cause read operations to erroneously return empty files if they were not downloaded yet.
- Implemented conflict resolution in JCloudData.
- The native plugin now uses NSMetadataQuery which incurs a little global slowdown (barely noticeable but still there).
- Fixed JCloudData changes monitoring watchdog gameobject not marked as DontDestroyOnLoad (plus it now uses JCloudManager object).
- Proper handling of iCloud enabling/disabling during pause/background.
- Unity 3.5.7 is now a base requirement (may work on older versions...).
- Please read the migration guide!

-------------------
v1.4 (11/27/2012)
-------------------

- Unity 3.5.6 is now a base requirement as builds made with Unity 3.4.2 won't run anymore with Xcode 4.5
- iOS 4.3 is now a base requirement as Xcode 4.5 won't compile for any previous OS version (thanks to Apple...)
- armv7 is now a base requirement as Xcode 4.5 won't compile for armv6 chips (thanks to Apple again...)
- Added jailbreaking detection for iOS and a switch to turn iCloud off on those devices due to reports with issues (you have to enable it on each JCloudData, JCloudDictionary, JCloudDocument and JCloudDocumentAsync classes, e.g. 'JCloudData.AcceptJailbrokenDevices = false')
- Fixed script error with WebPlayer build
- Fixed some warnings on Unity 4
- Fixed some memory leaks
- Fixed linking issues when building the plugin from source for iOS

-------------------
v1.36 (06/15/2012)
-------------------

- Fixed an issue with JCloudData.GetString and JCloudDictionary.GetString which could sometimes return incorrect results
- Future-proofed some memory management methods

-------------------
v1.35 (06/08/2012)
-------------------

- Improved reliability when accessing the same file multiple times concurrently to prevent a race condition where the file would lock
- Internal rework for better handling of file access synchronization

-------------------
v1.3 (04/30/2012)
-------------------

- New: Added JCloudDocumentAsync for asynchronous manipulation of files and directories using co-routines in a similar manner to LoadLevelAsync.
- New: Added JCloudCoroutine (based on Mike Talbot's "Extended Unity Coroutines" code) as a tool to emulate YieldInstruction for async operations.
- Examples scenes have been updated with the new async methods. Async methods are now the preferred implementation.
- Many under-the-hood enhancements and reworks for better reliability and simplicity.

-------------------
v1.21 (04/26/2012)
-------------------

- JCloudDocument now uses a unique GameObject manager to communicate with the plugin. This object is instantiated automatically and is set as DontDestroyOnLoad. Just make sure you don't mess with the JCloudManager object.
- Fixed synchronization issues between the plugin managed code and unmanaged code which could result in errors when using threading.
- Internal rework for cleaner code, better performance and future improvements (asynchronous methods using coroutines).

-------------------
v1.2 (04/24/2012)
-------------------

- Added FileCopy and DirectoryCopy methods to JCloudDocument in order to easily make copies of files and directories.
- Added FileMove and DirectoryMove methods to JCloudDocument in order to easily move or rename files and directories.
- Fixed potential crashes with certain Android devices not liking the mere existence of the plugin's DllImport despite the fact that their functions never actually get called.
- Fixed memory leaks when using JCloudPlugin methods in Mono threads.

-------------------
v1.19 (04/03/2012)
-------------------

- Fixed a rare condition where JCloudDictionary could cause crashes when simultaneously setting a key and saving.

-------------------
v1.18 (03/25/2012)
-------------------

- Fixed WebPlayer support (using PlayerPrefs).
- Improved reliability of JCloudDictionary.
- Fixed an edge case with OS X deployment using JCloudDictionary without sandboxing.

-------------------
v1.16 (02/17/2012)
-------------------

- Removed AccountSettings directory that was not supposed to be published in the package.

-------------------
v1.15 (02/17/2012)
-------------------

- New directory structure so that Plugins folder is at the top-level for proper plugin bundling. Please delete your previous JCloudPlugin directory before importing this new version.
- Fixed declaration errors for JCloudData.SetFloat and JCloudDictionary.SetFloat.

-------------------
v1.1 (02/13/2012)
-------------------

- Callbacks for JCloudData and JCloudDictionary notifying updates from iCloud.
- Methods for getting modification date of JCloudDocument files and directories.
- Under-the-hood enhancements paving the way for asynchronous API.

-------------------
v1.0 (12/28/2011)
-------------------

- Initial release.



jemast software


