#!/bin/bash

# c0lby:

## Create New Root.plist file ##

PreparePreferenceFile

		AddNewTitleValuePreference  -k "VersionNumber" 	-d "$versionNumber ($buildNumber)" 	-t "Version"

		# AddNewTitleValuePreference  -k "GitCommitHash" 	-d "$gitCommitHash" -t "Git Hash"

	AddNewPreferenceGroup 	-t "Test Settings"

		AddNewToggleSwitchPreference -k "TestProducer" 	-d true 	-t "Producer"


	# Azure Functions
	AddNewPreferenceGroup	-t "Azure Functions"
		AddNewToggleSwitchPreference 	-k "UseLocalFunctions" 		-d true 	-t "Use Local Functions"

	AddNewPreferenceGroup	-t "Local Azure Functions"
		AddNewTextFieldPreference		-k "LocalFunctionsUrl"		-d "" 		-t "Local"

	AddNewPreferenceGroup	-t "Local Azure Functions"
		AddNewTextFieldPreference		-k "RemoteFunctionsUrl"		-d "" 		-t "Remote"

	# Azure Document DB
	AddNewPreferenceGroup	-t "Azure DocumentDB"
		AddNewToggleSwitchPreference 	-k "UseLocalDocumentDb" 	-d true 	-t "Use Local DocumentDB"
		
	AddNewPreferenceGroup	-t "Local Azure DocumentDB"
		AddNewTextFieldPreference		-k "LocalDocumentDbUrl"		-d "" 		-t "https://"
		AddNewTextFieldPreference		-k "LocalDocumentDbKey"		-d "" 		-t "Key"

	AddNewPreferenceGroup	-t "Remote Azure DocumentDB"
		AddNewTextFieldPreference		-k "RemoteDocumentDbUrl"	-d "" 		-t "https://"
		AddNewTextFieldPreference		-k "RemoteDocumentDbKey"	-d "" 		-t "Key"


	AddNewPreferenceGroup 	-t "Embedded Social Key"
		# AddNewStringNode 	-e "ServerFooterText" 	-v "The base Url to your Azure Functions."
		AddNewTextFieldPreference		-k "EmbeddedSocialKey"		-d "" 		-t "Key"


	AddNewPreferenceGroup 	-t "Diagnostics Key"
		AddNewStringNode 	-e "FooterText" 	-v "$copyright"


	AddNewTitleValuePreference  -k "UserReferenceKey" 	-d "anonymous"  	-t ""
