# CmdTools

Some useful tools to use from command line

## UpdateVersion

From a Visual Studio solution path, try to increase project versions. If no arguments are used, *UpdateVersion* will try to locate a `version.txt` with information needed to do update version.

Supports .Net MAUI projects, which use `ApplicationDisplayVersion` instead of `Version` 

## CreateRelationsDiagram

- `-a "C:\arc\src\github\topelab\l2data2code\src\L2Data2Code\L2Data2Code.Avalonia\SampleData\Northwind.ERP.Domain.dll" -o C:\Users\detos\Downloads\classes.md`
- `-a C:\arc\src\tmp\3.5-postgresql\Topelab.Calendar.postgresql.UI\CalendarCreator\bin\Debug\net9.0\Topelab.Calendar.Domain.dll -o C:\Users\detos\Downloads\classes.md -r -c Calendario`
- `-a C:\arc\src\tmp\3.5-postgresql\Topelab.Calendar.postgresql.UI\CalendarCreator\bin\Debug\net9.0\Topelab.Calendar.Domain.dll -o C:\Users\detos\Downloads\classes.md -c Calendario`
- `-o C:\Users\detos\Downloads\relations.md -s C:\arc\src\github\topelab\l2data2code\ -r -p schema`
- `-o C:\Users\detos\Downloads\relations.md -s C:\arc\src\github\topelab\l2data2code\ -p schema`