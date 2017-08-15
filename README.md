# Producer

iOS: [![Build status](https://build.mobile.azure.com/v0.1/apps/507c64e8-f770-454e-b82e-88f53592d117/branches/master/badge)](https://mobile.azure.com)    
Android: [![Build status](https://build.mobile.azure.com/v0.1/apps/8721f631-cf9b-4cc3-8d66-0d6ec10166bd/branches/master/badge)](https://mobile.azure.com)

# Project Setup

Steps to get the app running locally are explained below.  These steps only apply while the project is in development and will be much simpler by the time we complete the project.

## Visual Studio Mac

If you're going to run the `Producer.Functions` project locally on a mac, you'll have to have the addin installed.  Currently, this only works in the **alpha channel**.  Once on the alpha channel, you can find/install the **Azure Functions development** addin by going to _Visual Studio >> Extensions... >> Gallery >> IDE Extensions_.

## Keys & Connection Strings

This is a public repo, so we need to **make sure we don't commit any private keys** and refresh any keys that we accidentally commit.


### iOS

All the keys, urls, etc. needed to run the app are in the app's settings _(in Settings.app under Producer)_.  This is a sample app, so this will be the way we handle all keys, urls, connection strings, etc. -- **Do not hard-code keys in the app**.    Ping me (Colby) and I'll get you what you need.


### Android

Android isn't finished yet, but it'll be handled in the same way as iOS.  Most of the settings code is shared.


## Functions

All keys, connection strings, etc. are in `local.settings.json`.  This file is ignored by git - please make sure it doesn't get committed.  Ping me (Colby) for the values needed in your local copy.


## Azure


# Project Workflow

From a high level, we'll follow [GitHub Flow][0] - a lightweight, branch-based workflow.


## Contributing

- **Every contribution to the codebase/docs/creative should be associated with an Issue and done in a separate branch**
- **Before starting work/creating a branch, assign yourself the Issue and move the associated Project item to `In Progress`**
- **Branches should be branched from `master` and be named something that clearly corresponds to the Issue**
- **When a branch/issue is complete, move the associated Project item to `Needs Review` and submit a Pull Request**
- **DO NOT MERGE YOUR OWN PULL REQUESTS - they must be reviewed and approved before merging into `master`**



## 🔨 Build & 🎨 Paint

All UI contributions (new screens, etc.) will be created in two separate steps; **_Build_** and **_Paint_** _(described in more detail below)_

### 1. 🔨 - Build
In the **_Build_** phase of a new screen or other UI element, you're just getting the required data on the screen and implementing that screen/UI element's functionality - **without making it "pretty"** _(formatting, colors, fonts, animations, etc.)_.  This allows us to **a)** make updates/tweaks/changes to the data/functionality before those changes could effect the layout, animations, etc., and **b)** benchmark the performance of data/functionality to ensure we don't degrade the performance during the **_Paint_** phase.

### 2. 🎨 - Paint
Formatting, colors, fonts, animations, etc. are added during the **_Paint_** phase of the screen/element. This will be a **separate branch, pull request, etc. than the screen/element's **_Build_** phase.



# Documentation

Docs outlining the data, architecture, etc. are at the top of the todo list.



[0]:https://guides.github.com/introduction/flow/
