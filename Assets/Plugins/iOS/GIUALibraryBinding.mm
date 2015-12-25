#import "GIUALibrary.h"

extern "C" {
	
void _requestAppRegistration()
{
	[[GIUALibrary shared] requestAppRegistration];
}

}