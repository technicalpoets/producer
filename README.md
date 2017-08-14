# Producer

iOS: [![Build status](https://build.mobile.azure.com/v0.1/apps/507c64e8-f770-454e-b82e-88f53592d117/branches/master/badge)](https://mobile.azure.com)    
Android: [![Build status](https://build.mobile.azure.com/v0.1/apps/8721f631-cf9b-4cc3-8d66-0d6ec10166bd/branches/master/badge)](https://mobile.azure.com)

## Project Setup

Steps to get the app running locally are explained below.  These steps only apply while the project is in development and will be much simpler by the time we complete the project.

### Visual Studio Mac

If you're going to run the `Producer.Functions` project locally on a mac, you'll have to have the addin installed.  Currently, this only works in the **alpha channel**.  Once on the alpha channel, you can find/install the **Azure Functions development** addin by going to _Visual Studio >> Extensions... >> Gallery >> IDE Extensions_.

### Keys & Connection Strings

This is a public repo, so we need to **make sure we don't commit any private keys** and refresh any keys that we accidentally commit.


#### iOS

All the keys, urls, etc. needed to run the app are in the app's settings _(in Settings.app under Producer)_.  This is a sample app, so this will be the way we handle all keys, urls, connection strings, etc. -- **Do not hard-code keys in the app**.    Ping me (Colby) and I'll get you what you need.


#### Android

Android isn't finished yet, but it'll be handled in the same way as iOS.  Most of the settings code is shared.


#### Functions

All keys, connection strings, etc. are in `local.settings.json`.  This file is ignored by git - please make sure it doesn't get commited.  Ping me (Colby) for the values needed in your local copy.



## Documentation

Docs outlining the data, architecture, etc. are at the top of the todo list.



## Project Structure



## Azure