:: move this file to the folder where CrmWebResourceManager.exe is located
@echo off
CrmWebResourceDeployer.exe -o:"https://my_crm_server/my_organization_name/XRMServices/2011/Organization.svc" -x:"new" -w:"path_to_folder_where_scripts_located" -i:"FormEvents.xml|WebResources.xml" -p:"False" -e:"False" -s:"False"