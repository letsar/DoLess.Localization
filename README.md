![Logo](./nuget/doless.png)

**DoLess.Localization** is a simple way to share localization files (resx) from cross-platform lib to iOS and Android projects.

## Why?
Share localization strings between viewmodels and views is difficult in Xamarin.
**DoLess.Localization** attemps to make it easy !

## Install

Available on NuGet:

[![NuGet](https://img.shields.io/badge/Nuget-1.0.0-green.svg)](https://www.nuget.org/packages/DoLess.Localization/)

Installs the nuget package on your project containing the resx files.
All resx files are scanned, if you don't want a resx file to be scanned, create another library to put the resx in.
The languages used are automatically get from the name of the resx files.

When the package is installed on a project, a file called **doless.localization.json** is inserted into your project.
The file is used by **DoLess.Localization** as a configuration file, you can set the folders containing the Android and the iOS projects. 

## Configuration

The default json file comes with this:
```json
{
  "android-project-folder-path": "",
  "ios-project-folder-path": "",
  "overwrite": false
}
```

You can set here the path to your Android and iOS project folders using the **android-project-folder-path** and the **ios-project-folder-path** fields.
The **overwrite**  fields is used to tell whether the files must be rewritten completly from the resx (`true`) or if specific platform strings should be kept (`false`).

## Configuration sample

```json
{
  "android-project-folder-path": "../DoLess.Localization.Sample.Droid",
  "ios-project-folder-path": "../DoLess.Localization.Sample.iOS",
  "overwrite": false
}
```

## Usage

Simply build the project containing your resx files !

If the destination files don't exist, the tool create them but **it does not** include them in the target project. **You must** do it yourself for the first time.
