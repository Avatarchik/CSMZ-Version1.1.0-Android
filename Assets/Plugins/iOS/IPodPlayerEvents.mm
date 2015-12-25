#import <QuartzCore/QuartzCore.h>
#import <MediaPlayer/MediaPlayer.h>

#import "IPodPlayerEvents.h"

//#include <stdlib.h>
#include <sys/types.h>
#include <sys/sysctl.h>
#include "AudioToolbox/AudioServices.h"

//=================================================================================================================
#pragma mark Unity iPodMusicNotifier

extern "C"
{
    float iPodPlayerEvents_HardwareVolume()
    {
        float vol = 0;
        UInt32 size = sizeof(float);
        AudioSessionGetProperty(kAudioSessionProperty_CurrentHardwareOutputVolume, &size, (void*)&vol);
        return vol;
    }

	bool iPodPlayerEvents_IsIPodPlaying()
	{
		return [[MPMusicPlayerController iPodMusicPlayer] playbackState] == MPMusicPlaybackStatePlaying;
	}
}
//=================================================================================================================
