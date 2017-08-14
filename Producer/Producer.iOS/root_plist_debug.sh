#!/bin/bash

# c0lby:

## Create New Root.plist file ##

PreparePreferenceFile

	AddNewTitleValuePreference  -k "VersionNumber" 	-d "$versionNumber ($buildNumber)" 	-t "Version"

		# AddNewTitleValuePreference  -k "GitCommitHash" 	-d "$gitCommitHash" -t "Git Hash"

	AddNewPreferenceGroup 	-t "Test Settings"

		AddNewToggleSwitchPreference -k "TestProducer" 	-d true 	-t "Producer"


	AddNewPreferenceGroup	-t "Azure Functions Urls"
		AddNewStringNode 	-e "FunctionsFooterText" 	-v "Example: producer.azurewebsites.net"
		AddNewToggleSwitchPreference 	-k "UseLocalFunctions" 		-d true 	-t "Use Local Functions"
		AddNewTextFieldPreference		-k "LocalFunctionsUrl"		-d "" 		-t "Local"
		AddNewTextFieldPreference		-k "RemoteFunctionsUrl"		-d "" 		-t "Remote"


	AddNewPreferenceGroup	-t "Azure DocumentDB Urls"
		AddNewStringNode 	-e "DocumentDbFooterText" 	-v "Example: producer.documents.azure.com"
		AddNewToggleSwitchPreference 	-k "UseLocalDocumentDb" 	-d true 	-t "Use Local DocumentDB"
		AddNewTextFieldPreference		-k "LocalDocumentDbUrl"		-d "" 		-t "Local"
		AddNewTextFieldPreference		-k "RemoteDocumentDbUrl"	-d "" 		-t "Remote"


	AddNewPreferenceGroup 	-t "Embedded Social Key"
		# AddNewStringNode 	-e "ServerFooterText" 	-v "The base Url to your Azure Functions."
		AddNewTextFieldPreference		-k "EmbeddedSocialKey"		-d "" 		-t "Key"


	AddNewPreferenceGroup 	-t "Diagnostics Key"
		AddNewStringNode 	-e "FooterText" 	-v "$copyright"


	AddNewTitleValuePreference  -k "UserReferenceKey" 	-d "anonymous"  	-t ""
