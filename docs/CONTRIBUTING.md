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
