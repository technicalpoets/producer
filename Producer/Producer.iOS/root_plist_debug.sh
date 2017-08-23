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
	AddNewPreferenceGroup	-t "Azure Functions Url"
		AddNewStringNode 	-e "FooterText" 	-v "Example Url: producer.azurewebsites.net (do not include https:// or a trailing /)"
		AddNewTextFieldPreference		-k "RemoteFunctionsUrl"		-d "" 		-t ""


	# Azure Document DB
	AddNewPreferenceGroup	-t "Azure DocumentDB Url"
		AddNewStringNode 	-e "FooterText" 	-v "Example Url: producer.documents.azure.com (do not include https:// or a trailing /)"
		AddNewTextFieldPreference		-k "RemoteDocumentDbUrl"	-d "" 		-t ""


	# Azure Notification Hubs
	AddNewPreferenceGroup	-t "Azure Notification Hub"
		AddNewStringNode 	-e "FooterText" 	-v "Your Notification Hub Connection String"
		AddNewTextFieldPreference 		-k "NotificationsName"				-d ""	-t "Hub Name:"
		AddNewTextFieldPreference 		-k "NotificationsConnectionString"	-d ""	-t ""


	# Embedded Social
	AddNewPreferenceGroup 	-t "Embedded Social Key"
		# AddNewStringNode 	-e "ServerFooterText" 	-v "The base Url to your Azure Functions."
		AddNewTextFieldPreference		-k "EmbeddedSocialKey"		-d "" 		-t ""


	AddNewPreferenceGroup 	-t "Diagnostics Key"
		AddNewStringNode 	-e "FooterText" 	-v "$copyright"


	AddNewTitleValuePreference  -k "UserReferenceKey" 	-d "anonymous"  	-t ""
