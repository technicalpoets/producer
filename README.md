iOS: [![Build status][ios-build]][mobile-center]   
Android: [![Build status][droid-build]][mobile-center]   

[![Deploy to Azure][azure-deploy-button]][azure-deploy]

# Producer
 
Producer is a mobile content management system that allows audio and video artist to create and share content with fans.

#### iOS
![ios-login](https://github.com/technicalpoets/producer/raw/docs-1/images/ios-login.png?raw=true "ios-login") | ![ios-content](https://github.com/technicalpoets/producer/raw/docs-1/images/ios-content-list.png "ios-content")
:-------------------------:|:-------------------------:

#### Android
Coming Soon!


# Getting Started

## iOS Provisioning & Certificates

### Apple Push Notification Certificates

Once you're Azure services are configured, you'll need to perform some additional configuration to enable push notifications in the Apple Developer portal.

A brief outline of the steps needed to do this are:

- [Create a Certificate Signing Request (CSR) for a push certificate][ios-push-cert-csr]
- [Register your app for push notifications][ios-push-cert]
	- Creation/configuration of an App ID to enable Push Notifications
	- Creation of a development push notification certificate for the App ID using the CSR
	- Download and installation of the new push certificate on your development machine
- [Create a provisioning profile for the app][ios-push-cert-provisioning]

After that, you'll need to navigate to the Notification Hub in Azure and upload the push certifcate.  The One Click deploy will have created a Notification Hub in Azure - navigate there in the Azure portal.  Under the notification service settings for the Notification Hub, you'll need to upload the Apple push notification certificate you create in the steps above.  This will roughly track with the [steps and screen shots shown in this guide][ios-push-cert-upload] (step 7 specifically; however, these are shown in the legacy azure portal).





## Firebase

### Google Auth

### Firebase Cloud Messaging


## Azure

Once you're Azure services are configured you should be able to see on your subscription the following resources:

- Website
- DocumentDB
- Microsoft.Media
- Microsoft.Storage
- Microsoft.NotificationHubs

All resources should be grouped by the Resource group, e.g. Producer.

It's time to open Producer App and configure the settings properly.

![ios-login](https://github.com/technicalpoets/producer/raw/docs-1/images/ios-dialog-settings-producer.png?raw=true "ios-login") | ![ios-content](https://github.com/technicalpoets/producer/raw/docs-1/images/ios-configure-settings-producer.png?raw=true "ios-content")
:-------------------------:|:-------------------------:

### **Mobile Center**

Mobile Center application key is required to enable the crash and analytics features, to configure Mobile Center is necessary go to: mobile.azure.com and sign up with any of the following credentials:

- GitHub
- Microsoft Account
- Facebook
- Google
- Mobile Center Account

For more information about how to [create an App on Mobile Center][create-an-app-on-mobile-center].

Once the iOS project has been created on Mobile Center, need to go to the project settings and get the App Secret Key, then configure it in our mobile app settings in MOBILE CENTER APP SECRET setting.

### **Functions**

TODO..

### **DocumentDB** (Cosmos DB)

In your Azure subscription you should be able to see a Cosmos DB resource under our resource group.

Select the Cosmos DB resource and press Keys settings to get the service URI.

Once we get the URI, then configure it in our mobile app settings in in AZURE DOCUMENTDB URL setting.

### **Notification Hub**

### **Authentication**








[ios-build]:https://build.mobile.azure.com/v0.1/apps/507c64e8-f770-454e-b82e-88f53592d117/branches/master/badge
[droid-build]:https://build.mobile.azure.com/v0.1/apps/8721f631-cf9b-4cc3-8d66-0d6ec10166bd/branches/master/badge
[mobile-center]:https://mobile.azure.com

[azure-deploy]:https://azuredeploy.net
[azure-deploy-button]:https://azuredeploy.net/deploybutton.svg

[ios-push-cert-csr]:https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-apple-push-notification-apns-get-started#generate-the-certificate-signing-request-file
[ios-push-cert]:https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-apple-push-notification-apns-get-started#register-your-app-for-push-notifications
[ios-push-cert-provisioning]:https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-apple-push-notification-apns-get-started#create-a-provisioning-profile-for-the-app
[ios-push-cert-upload]:https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-ios-push-notification-apns-get-started#configure-your-notification-hub

[create-an-app-on-mobile-center]:https://docs.microsoft.com/en-us/mobile-center/sdk/getting-started/xamarin