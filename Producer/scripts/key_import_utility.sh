#!/bin/bash

# MOBILECENTER_SOURCE_DIRECTORY="/Users/colbywilliams/GitHub/producer"

if [[ MOBILECENTER_SOURCE_DIRECTORY ]];
	then

queryUrl="https://producer.azurewebsites.net/api/settings"

curl $queryUrl -o "$MOBILECENTER_SOURCE_DIRECTORY/Producer/Producer.iOS/ProducerSettings.json"

fi