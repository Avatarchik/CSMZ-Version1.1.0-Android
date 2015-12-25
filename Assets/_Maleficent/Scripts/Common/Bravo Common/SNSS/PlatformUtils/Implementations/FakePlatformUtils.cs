using UnityEngine;
using System.Collections;

public class FakePlatformUtils : IPlatformUtils {

	public override string BundleVersion() {
		return "[FakeVersion]";
	}
	
	public override string BundleID() {
		return "[FakeBundleId]";
	}
}
