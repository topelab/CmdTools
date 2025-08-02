# CmdTools

Some useful tools to use from command line

## UpdateVersion

From a Visual Studio solution path, try to increase project versions. If no arguments are used, *UpdateVersion* will try to locate a `version.txt` with information needed to do update version.

Supports .Net MAUI projects, which use `ApplicationDisplayVersion` instead of `Version` 

## CreateRelationsDiagram

### Funcionamiento

Imaginemos que tenemos una solución con la siguiente estructura:


    Proyecto1 (Proyecto2, Proyecto3, Proyecto4)
    Proyecto2 (Proyecto5, Proyecto3, Proyecto4)
    Proyecto3 (Proyecto4, Proyecto5)
    Proyecto4 (Proyecto7, Proyecto8, Proyecto9)
    Proyecto5 (Proyecto6, Proyecto7)

Si queremos generar un diagrama de relaciones entre proyectos, podemos ejecutar el comando:

```powershell
CreateRelationsDiagram -s "C:\ruta\del\proyecto\raiz" -o "C:\ruta\de\salida\sample.mmd' -d LeftToRight -w
```

Esto generará un archivo [sample.mmd](./sample.mmd) con las relaciones entre los proyectos de la solución, donde cada proyecto es un nodo y las relaciones entre ellos son flechas que indican la dirección de la dependencia. Al indicar `-w`, se incluirán las dependencias de los paquetes nuget, creando un diagrama más completo.

El parámetro `-d`, nos permite elegir la dirección del diagrama, ya sea `LeftToRight`, `TopToBottom`, `BottomToTop` o `RightToLeft`. Por defecto, se utiliza `LeftToRight`.

El parámetro `-r`, realizará un diagrama [inverso.mmd](./inverso.mmd), donde las relaciones se muestran en sentido contrario, es decir, desde el proyecto dependiente hacia el proyecto del que depende.

El parámetro '-p', permite fijar los proyectos por uno específico, como por ejemplo `avalonia`, para incluir solo aquellos proyectos que en sus referencias (y referencias de sus referencias, etc) lleguen a usar el proyecto o nupkg `avalonia`.

### Ejemplos

- `-a "C:\arc\src\github\topelab\l2data2code\src\L2Data2Code\L2Data2Code.Avalonia\SampleData\Northwind.ERP.Domain.dll" -o C:\Users\detos\Downloads\classes.md`
- `-a C:\arc\src\tmp\3.5-postgresql\Topelab.Calendar.postgresql.UI\CalendarCreator\bin\Debug\net9.0\Topelab.Calendar.Domain.dll -o C:\Users\detos\Downloads\classes.md -r -c Calendario`
- `-a C:\arc\src\tmp\3.5-postgresql\Topelab.Calendar.postgresql.UI\CalendarCreator\bin\Debug\net9.0\Topelab.Calendar.Domain.dll -o C:\Users\detos\Downloads\classes.md -c Calendario`
- `-o C:\Users\detos\Downloads\relations.md -s C:\arc\src\github\topelab\l2data2code\ -r -f schema`
- `-o C:\Users\detos\Downloads\relations.md -s C:\arc\src\github\topelab\l2data2code\ -f schema`
- `-o C:\Users\detos\Downloads\relations.mermaid -s C:\arc\src\github\topelab\l2data2code\ -d BottomToTop`
- `-a C:\arc\output\CalendarCreator\Topelab.Calendar.Domain.dll -o C:\Users\detos\Downloads\classes.mmd -d LeftToRight`
- `-s C:\arc\src\github\topelab\Topelab.Calendar\ -o C:\Users\detos\Downloads\calendars-projects.mermaid -d LeftToRight -w`
- `-s C:\arc\src\github\topelab\Topelab.Calendar\ -o C:\Users\detos\Downloads\calendars-projects.mermaid -d LeftToRight -w -p avalonia`

