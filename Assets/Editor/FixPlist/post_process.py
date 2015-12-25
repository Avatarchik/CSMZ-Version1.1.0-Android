#!/usr/bin/python
import sys
import os
import shutil
import plistlib

def _fix_plist(xcode_project_path):

	plist_path = os.path.join(xcode_project_path, 'Info.plist')
	try:
		f = open(plist_path, 'r')
		plistlib.readPlist(f)
	except Exception as e:
		s_out = []
		with open(plist_path, 'r') as f:
			map(lambda x: s_out.append(x), [x for x in f.readlines() if x.strip() != '</string>']);
		
		with open(plist_path, 'w') as f:
			f.write(''.join(s_out))


def main(xcode_project_path, base_path):

	_fix_plist(xcode_project_path)


main(sys.argv[1], sys.argv[2])