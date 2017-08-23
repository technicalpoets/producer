#!/bin/bash

# c0lby:

## Create New Root.plist file ##

PreparePreferenceFile

		AddNewTitleValuePreference  -k "VersionNumber" 	-d "$versionNumber ($buildNumber)" 	-t "Version"
		# AddNewTitleValuePreference  -k "GitCommitHash" 	-d "$gitCommitHash" -t "Git Hash"


	AddNewPreferenceGroup 	-t "Test Settings"
		AddNewToggleSwitchPreference -k "TestProducer" 	-d true 	-t "Producer"


	# Mobile Center
	AddNewPreferenceGroup	-t "Mobile Center App Secret"
		AddNewTextFieldPreference		-k "MobileCenterKey"		-d "" 		-t ""


	# Azure Functions
	AddNewPreferenceGroup	-t "Azure Functions"
		AddNewStringNode 	-e "FooterText" 	-v "Example Url: producer.azurewebsites.net (do not include https:// or a trailing /)"
		AddNewToggleSwitchPreference 	-k "UseLocalFunctions" 		-d false 	-t "Use Local Functions"
		AddNewTextFieldPreference		-k "LocalFunctionsUrl"		-d "" 		-t "Local Url:"
		AddNewTextFieldPreference		-k "RemoteFunctionsUrl"		-d "" 		-t "Remote Url:"


	# Azure Document DB
	AddNewPreferenceGroup	-t "Azure DocumentDB"
		AddNewStringNode 	-e "FooterText" 	-v "Example Url: producer.documents.azure.com (do not include https:// or a trailing /)"
		AddNewToggleSwitchPreference 	-k "UseLocalDocumentDb" 	-d false 	-t "Use Local DocumentDB"
		AddNewTextFieldPreference		-k "LocalDocumentDbUrl"		-d "" 		-t "Local Url:"
		AddNewTextFieldPreference		-k "LocalDocumentDbKey"		-d "" 		-t "Local Key:"
		AddNewTextFieldPreference		-k "RemoteDocumentDbUrl"	-d "" 		-t "Remote Url:"
		AddNewTextFieldPreference		-k "RemoteDocumentDbKey"	-d "" 		-t "Remote Key:"


	# Azure Notification Hubs
	AddNewPreferenceGroup	-t "Azure Notification Hub"
		AddNewStringNode 	-e "FooterText" 	-v "Your Notification Hub Connection String"
		AddNewTextFieldPreference 		-k "NotificationsName"				-d ""	-t "Hub Name:"
		AddNewTextFieldPreference 		-k "NotificationsConnectionString"	-d ""	-t ""
		#NotificationsKey
		#NotificationsUrl


	# Embedded Social
	AddNewPreferenceGroup 	-t "Embedded Social Key"
		# AddNewStringNode 	-e "ServerFooterText" 	-v "The base Url to your Azure Functions."
		AddNewTextFieldPreference		-k "EmbeddedSocialKey"		-d "" 		-t ""


	AddNewPreferenceGroup 	-t "Diagnostics Key"
		AddNewStringNode 	-e "FooterText" 	-v "$copyright"


	AddNewTitleValuePreference  -k "UserReferenceKey" 	-d "anonymous"  	-t ""
