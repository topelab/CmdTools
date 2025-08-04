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

