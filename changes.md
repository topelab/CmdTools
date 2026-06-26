## Versions 1.1.5 (AvaloniaProjectRelations), 1.1.36 (Contracts, CreateRelationsDiagram, RunCustomTool, Shared, UpdateVersion), 1.2.5 (ProjectRelations2026)

- Updating Avalonia.Controls.DataGrid to 12.0.1
- Updating Avalonia.Desktop to 12.0.5
- Updating Topelab.Core.Adapters to 1.12.14
- Updating Topelab.Core.Avalonia to 1.2.12

## Version 1.1.3 (AvaloniaProjectRelations), 1.1.34 (Contracts), 1.1.34 (CreateRelationsDiagram), 1.2.3 (ProjectRelations2026), 1.1.34 (RunCustomTool), 1.1.34 (Shared), 1.1.34 (UpdateVersion)

- Updating Microsoft.Web.WebView2 to 1.0.4022.49
- Updating AngleSharp to 1.5.1
- Updating System.CodeDom to 10.0.9
- Updating Topelab.Core.Adapters to 1.12.12
- Updating Topelab.Core.Avalonia to 1.2.11

## 1.1.33 (Tools)

- Fix `UpdateVersion` command to correctly update version numbers in `Directory.Build.props` files.

## 1.1.32 (Tools)

- Updated `UpdateVersion` command to support Directory.Build.props files for version updates.

## 1.2.1 (ProjectRelations2026), 1.1.31 (Tools), 1.1.2 (AvaloniaProjectRelations)

- Upgrade Avalonia.Desktop to 12.0.4
- Upgrade Topelab.Core.Adapters to 1.12.9
- Upgrade Topelab.Core.Avalonia to 1.2.10

## 1.2.0 (ProjectRelations2026), 1.1.30 (Tools), 1.1.0 (AvaloniaProjectRelations)

- Optimized recovery of projects by implementing a more efficient search mechanism.

## 1.1.5 (ProjectRelations2026), 1.1.27 (Tools), 1.0.2 (AvaloniaProjectRelations)

- Upgrade ReactiveUI.Avalonia to 12.0.1
- Upgrade Microsoft.Web.WebView2 to 1.0.3967.48
- Upgrade NLog.Extensions.Logging to 6.1.3
- Upgrade Avalonia.Desktop to 12.0.3
- Upgrade System.CodeDom to 10.0.8
- Upgrade Topelab.Core.Resolver to 2.0.7
- Upgrade Avalonia.Controls.WebView to 12.0.1
- Upgrade Topelab.Core.Adapters to 1.12.8
- Upgrade Topelab.Core.Avalonia to 1.2.9


## 1.1.25

- Updating Microsoft.Web.WebView2 to 1.0.3800.47
- Updating System.CodeDom to 10.0.3
- Updating Topelab.Core.Resolver to 2.0.3

## 1.1.24

- Updating Topelab.Core.Resolver to 2.0.2
- Updating Microsoft.Web.WebView2 to 1.0.3719.77
- Updating System.CodeDom to 10.0.2


## 1.1.23

- Updating System.CodeDom to 10.0.1
- Updating Microsoft.Web.WebView2 to 1.0.3650.58

## 1.1.22

- Added `--open` option to automatically open the generated diagram after creation.

## 1.1.21

- Updating System.CodeDom to 9.0.9

## 1.1.20

- Changed solution type to `slnx`
- Replaced nuget package `Topelab.Core.Resolver.Microsoft` with `Topelab.Core.Resolver.Microsoft.csproj`

## 1.1.19

- Updating System.CodeDom to 9.0.8
- Updating Topelab.Core.Resolver.Microsoft to 1.9.1

## CreateRelationsDiagram - 1.1.15

- Small refactor in `ProjectReferences` and `ProjectFinder` to improve `GetFilteredReferences` method.

## CreateRelationsDiagram - 1.1.14

- Updated version numbers in project files.
- Added shorthand values to `Direction` enum.
- Improved `FindPinnedElement` logic in `ElementFinderBase`.
- Enhanced help text for options in `Options.cs`.
- Refactored `GetFilteredReferences` in `ProjectFinder`.
- Introduced `SetInverseReferences` and `GetInverseReferences` methods.
- Modified `FinderType` logic in `ProjectOptions` to consider `PinnedProject`.
- Updated `changes.md` and `version.txt` for new versioning.


## CreateRelationsDiagram - 1.1.13

- Options class refactoring

## CreateRelationsDiagram - 1.1.12

- Fix pinning logic in `ProjectFinder` to ensure correct handling of pinned elements.

## CreateRelationsDiagram - 1.1.11

Refactor ClassesFinder and update project version

- Updated `Run` method in `ClassesFinder` to remove default `nameSpaceToClean` assignment and added `excludeClasses` parameter for regex filtering.
- Modified `GetClasses` and `GetProperties` methods to utilize the new `excludeClasses` parameter for more flexible class and property retrieval.
- Removed `ClassesFinderReverse`.
- Bumped project version to `1.1.11` in `CreateRelationsDiagram.csproj` and `Shared.csproj`.
- Documented changes in `changes.md` and updated versioning in `version.txt`.

## CreateRelationsDiagram - 1.1.10

Update output handling and versioning in project files

- Modified `Run` methods in `ClassesFinder` and `ClassesFinderReverse` to remove default output file assignment for flexibility.
- Updated project version from `1.1.9` to `1.1.10` in `CreateRelationsDiagram.csproj` and `Shared.csproj`.
- Enhanced `Finalize` method in `ElementFinderBase` to handle null or empty output file cases, allowing console output.
- Renamed `GetPinnedElemet` to `FindPinnedElement` and improved its logic in `ElementFinderBase`.
- Clarified help text for `PinnedProject` option in `Options.cs`.
- Removed default value for `OutputFile` in `Options.cs`, changing behavior to console output if unspecified.
- Updated `Run` method in `ProjectFinder` to align with new output handling.
- Removed `ProjectFinderReverse.cs` file, indicating potential restructuring.
- Added `ExtractVersion` method in `ProjectReferences` to streamline version extraction from package references.
- Updated version information in `version.txt` to reflect new version numbers.


## 1.1.18

- Updating Topelab.Core.Resolver.Microsoft to 1.9.0

## 1.1.17

- Updating Topelab.Core.Resolver.Microsoft to 1.8.12

## 1.1.16

- Updating Topelab.Core.Resolver.Microsoft to 1.8.11

## 1.1.15

- Updating System.CodeDom to 9.0.7
- Updating Topelab.Core.Resolver.Microsoft to 1.8.9

