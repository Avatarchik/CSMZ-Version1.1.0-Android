#!/usr/bin/python
import os
import sys
import json
import shutil

def main(xcode_project_path, unity_path):

	splash_images_path = [ 
	'_Maleficent',
	'Icons',
	'Splash'
	]

	splash_images_path = os.path.join(unity_path, *splash_images_path)

	print splash_images_path

	files_to_copy = []
	for root, dirs, files in os.walk(splash_images_path):
		files_to_copy = filter(lambda x: os.path.splitext(x)[1] == '.png', files)		
		break

	for f in files_to_copy:
		src_path = os.path.join(splash_images_path, f)
		dst_path = os.path.join(xcode_project_path, f)
		shutil.copyfile(src_path, dst_path)

	print "Copied: {0} files".format(len(files_to_copy))


if __name__ == '__main__':
	script, xcode_project_path, unity_path = sys.argv
	main(xcode_project_path, unity_path)