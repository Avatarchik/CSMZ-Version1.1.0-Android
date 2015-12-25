#!/usr/bin/python
import os
import sys
import json

def main(project_path):

	xcassets_path = os.path.join(project_path, 
		'Unity-iPhone', 
		'Images.xcassets', 
		'AppIcon.appiconset',
		'Contents.json')

	s = ""
	with open(xcassets_path, "r") as f:
		s = json.loads(f.read())
		s["properties"] = {
			"pre-rendered" : True
		}

	with open(xcassets_path, "w") as f:
		f.write(json.dumps(s))

	print "pre-rendered flag applied"


if __name__ == '__main__':
	script, xcode_project_path, unity_path = sys.argv
	main(xcode_project_path)