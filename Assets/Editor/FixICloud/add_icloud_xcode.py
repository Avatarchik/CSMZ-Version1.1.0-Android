#encoding: utf-8
#!/usr/bin/python
from mod_pbxproj import XcodeProject, PBXVariantGroup, PBXDict
import sys
import os
import shutil

def log(message):
  print message

def main(project_path, unity_path):

    script_path = os.path.dirname(os.path.realpath(__file__))
    pbxproj_path = os.path.join(project_path, 'Unity-iPhone.xcodeproj', 'project.pbxproj')
    entitlements_file_name = 'icloud.entitlements'
    entitlements_path = os.path.join(script_path, 'ios', entitlements_file_name)

    project = XcodeProject.Load(pbxproj_path)

    ### HELPERS ##
    def _find_groups_by_isa(isa):
      result = {}
      for k in project.data.get('objects'):
        item = project.data.get('objects')[k]
        if (item.get('isa') == isa):
          result[k] = item
      return result

    def _find_build_configuration_groups(project):
      return _find_groups_by_isa('XCBuildConfiguration')

    def _find_project_groups(project):
      return _find_groups_by_isa('PBXProject')

    def _get_targets_groups(project):
      return _find_groups_by_isa('PBXNativeTarget')


    ### END HELPERS ###

    # Copy entitlements file to project
    src_path = entitlements_path
    dst_path = os.path.join(project_path, entitlements_file_name)
    shutil.copyfile(src_path, dst_path)
    log("Entitlements file copied")

    # Add entitlements file to project
    project.add_file_if_doesnt_exist(dst_path, create_build_files = False)
    log("Entitlements file added to project")

    # Add iCloud config in target attributes
    project_groups = _find_project_groups(project)
    project_group = project_groups.values()[0]
    targets = _get_targets_groups(project)
    target_attributes = project_group['attributes'].get('TargetAttributes', PBXDict())
    for k in targets:
      attributes = PBXDict()
      attributes['SystemCapabilities'] = PBXDict()
      attributes['SystemCapabilities']['com.apple.iCloud'] = PBXDict()
      attributes['SystemCapabilities']['com.apple.iCloud']['enabled'] = '1'
      target_attributes[k] = attributes
    project_group['attributes']['TargetAttributes'] = target_attributes
    log("iCloud config added to project")

    # Add build settings
    build_configuration_groups = _find_build_configuration_groups(project)
    for k in build_configuration_groups:
      bc_group = build_configuration_groups[k]
      bc_group['buildSettings']['CODE_SIGN_ENTITLEMENTS'] = entitlements_file_name
    log("CODE_SIGN_ENTITLEMENTS flag added")

    project.backup()
    project.save()
    log("iCloud added correctly")


if __name__ == '__main__':
    script, xcode_project_path, unity_path = sys.argv
    main(xcode_project_path, unity_path)
