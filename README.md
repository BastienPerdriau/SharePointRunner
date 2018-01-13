# SharePointRunner
Tool to run processes accros a SharePoint Online structure, from tenant to list items

## Purpose

After a few missions for customers, I had to do several scripts to audit SharePoint elements (structure of content) and did every time the same thing : a PowerShell which run accross the SHarePoint Object Model, site collection by site collection, sub-site by sub-site, list by list, etc, et finally export the informations to (mostly) a CSV file.

Because I don't like to do the same thing many times, I started a little projet for SharePoint Online, C# CSOM-based, to automatically run into SharePoint.
It let me do only thing when a new mission / need comes : Get the informations for each element type (site, list, ...) or write the operation I need to do for each element type

## Usage
The first step is to create a class which inherit from the `Receiver` class, from `SharePointRunner.SDK`.
This class can override these methods :

- `OnStart()` : Executed at the very start of the process, can be used to setup a file
- `OnTenantRunningStart(Tenant tenant)` : Executed at the start of the process for the tenant and expose the `Tenant` object
- `OnSiteCollectionRunningStart(Site site, Web rootSite)` : Executed at the start of the process for a site collection and expose the `Site` object and the `Web` for the root site
- `OnSiteRunningStart(Web web)` : Executed at the start of the process for a site and expose the `Web` object
- `OnListRunningStart(List list)` : Executed at the start of the process for a list and expose the `List` object
- `OnFolderRunningStart(Folder folder)` : Executed at the start of the process for a folder and expose the `Folder` object
- `OnListItemRunningStart(ListItem listItem)` : Executed at the start of the process for a list item and expose the `ListItem` object
- `OnFileRunningStart(File file)` : Executed at the start of the process for a file and expose the `File` object
- `OnListRunningEnd(List list)` : Executed at the end of the process for a list and expose the `List` object
- `OnSiteRunningEnd(Web web)` : Executed at the end of the process for a site (after the lists) and expose the `Web` object
- `OnSiteRunningEndAfterSubSites(Web web)` : Executed at the end of the process for a site (after the sub sites) and expose the `Web` object
- `OnSiteCollectionRunningEnd(Site site, Web rootSite)` : Executed at the end of the process for a site collection and expose the `Site` object and the `Web` for the root site
- `OnTenantRunningEnd(Tenant tenant)` : Executed at the end of the process for the tenant and expose the `Tenant` object
- `OnEnd()` : Executed at the very end of the process, can be used to export a file of make an external call

At the moment, the process to use the process is the one which is in the Program.cs from the [example](Examples/V1/SharePointRunner.LauncherV1).

In the future, I want to have one or many DLLs with contain the receiver and declared in a standard configuration file (XML nor JSON) and call the process with the configuration file (and the DLLs), without any program (but keep the possibility fo doing that way).
In addition, I would create PowerShell CmdLets to call the process from a script.

## Examples
I wrote a few examples from past experiences, availables in examples [here](Examples)

- `GroupsReceiver` : A receiver which export to CSV file the users of groups which contains one of the value (from groupNames) in their name
- `ManagedMetadataReceiver` : A receiver which export all the managed metadata from the SharePoint term store (unfinished)
- `PermissionsReceiver` : A receiver which crawl every site collection, sub sites and lists (and possibly folders and items) to know if they have inheritance broke and get the users and the permission level granted
- `WebPartsReceiver` : A receiver which crawl every site and sub sites to get all pages (from Site Pages or Pages) and the number of web parts on each one
- `WebUsage` : A receiver which get the item the most recently item from each site and sub site

## Close-future features / improvments
These features are mandatory to have a real usable tool for a large panel of use cases

- Finish the example "ManagedMetadataReceiver"
- Create NuGet packages (one with the SDK, one with the process)
- Add logs, there is none at the moment (Log4Net, to console, file, trace, all configurable)
- Add unit tests
- Several tehcnical improvments

## Future features / improvents

- Maybe manage Managed Metadata
- Separate receivers from the program which call the process to DLL
- Create PowerShell CmdLets to call the process with a standard configuration file (XML nor JSON)
- Others technical improvnents