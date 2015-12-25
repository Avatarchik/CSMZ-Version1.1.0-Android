#encoding: utf-8
#!/usr/bin/python
from mod_pbxproj import XcodeProject, PBXVariantGroup
import sys
import os
import shutil

def log(message):
  print message


def _get_languages(localizations_path):
  result = []
  for root, dirs, files in os.walk(localizations_path):
    result = dirs
    break
  return result

def _get_files(localizations_path):
  en_path = os.path.join(localizations_path, 'en')
  result = []
  for root, dirs, files in os.walk(en_path):
    result = [f for f in files if not f.endswith('.meta')]
    break
  return result


def main(project_path, unity_path):

    script_path = os.path.dirname(os.path.realpath(__file__))
    pbxproj_path = os.path.join(project_path, 'Unity-iPhone.xcodeproj', 'project.pbxproj')
    project = XcodeProject.Load(pbxproj_path)

    #files_path = '/Users/jesus/Desktop/localizations/{language}/{file_name}'
    localizations_path = os.path.join(script_path, 'ios')
    files_path = os.path.join(localizations_path, '{language}/{file_name}')

    languages = _get_languages(localizations_path)
    files = _get_files(localizations_path)

    ### HELP METHODS ###
    def _find_variants_groups(project):
      result = {}
      for k in project.data.get('objects'):
        item = project.data.get('objects')[k]
        if (item.get('isa') == 'PBXVariantGroup'):
          result[k] = item
      return result

    def _find_project_groups(project):
      result = {}
      for k in project.data.get('objects'):
        item = project.data.get('objects')[k]
        if (item.get('isa') == 'PBXProject'):
          result[k] = item
      return result


    def _get_file_with_id(project, file_id):
      return project.objects[file_id]


    def _get_id_of_file(project, project_file):
      result = None
      for k in project.objects:
        f = project.objects[k]
        if f.get('path') == project_file.get('path') and f.get('sourceTree') == project_file.get('sourceTree') and f.get('name') == project_file.get('name'):
          result = k
          break
      return result


    def _add_variants_of_file(project, file_group, file_id, project_file, variants_ids):

      group_id = _get_id_of_file(project, file_group)

      variant_group = PBXVariantGroup.Create()
      variant_group_id = PBXVariantGroup.GenerateId()
      variant_group['isa'] = 'PBXVariantGroup'
      variant_group['children'] = [file_id] + variants_ids
      variant_group['name'] = project_file['name']
      variant_group['sourceTree'] = project_file['sourceTree']

      # Change in the file group the reference
      group = project.objects[group_id]
      group['children'].remove(file_id)
      group['children'].append(variant_group_id)

      # Add the variant to the project
      project.objects[variant_group_id] = variant_group

      for variant_id in variants_ids:
        file_group.remove_child(variant_id)

    ### HELP METHODS END ###


    # Fix knownRegions
    log("Remove known regions")
    projects_groups = _find_project_groups(project)
    for k in projects_groups:
      pg = projects_groups[k]
      pg['knownRegions'] = languages
      project.data.get('objects')[k] = pg


    # Add new languages files
    for file_name in files:
      file_variants = {}
      files_dst_path = '{language}.lproj/{file_name}'
      ## English language is added directly to the project
      files_group = project.root_group

      ## Frist, remove all ocurrences of the file in the project
      for project_file in project.get_files_by_name(file_name):
        project_file_path = os.path.join(project_path, project_file['path'])
        project_file_id = _get_id_of_file(project, project_file)
        if os.path.exists(project_file_path):
          os.remove(project_file_path)
          project.remove_file(project_file_id)

      ## Add new files
      en_file_ref = None
      variants_files_refs = []

      for l in languages:
        src_path = files_path.format(language = l, file_name = file_name)
        dst_path = os.path.join(project_path, files_dst_path.format(language = l, file_name = file_name))

        if not os.path.exists(os.path.dirname(dst_path)):
          os.makedirs(os.path.dirname(dst_path))
        shutil.copyfile(src_path, dst_path)
        log("Copying file: {0} > {1}".format(src_path, dst_path))
        f = project.add_file(dst_path, parent = files_group, create_build_files = True)

        if l == 'en':
          file_id = _get_id_of_file(project, f[0])
          en_file_ref = file_id
        else:
          file_id = _get_id_of_file(project, f[0])
          variants_files_refs.append(file_id)
      file_variants[en_file_ref] = variants_files_refs


      # Remove all variant groups (just to be sure)
      for k in _find_variants_groups(project):
        del project.objects[k]
      # Create variants of new files
      for k in file_variants:
        f = _get_file_with_id(project, k)
        variants = file_variants[k]
        _add_variants_of_file(project, files_group, k, f, variants)

    project.backup()
    project.save()
    log("Project localization completed")


if __name__ == '__main__':
    script, xcode_project_path, unity_path = sys.argv
    main(xcode_project_path, unity_path)
