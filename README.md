# CmdTools

Algunas herramientas útiles para trabajar con proyectos .NET, especialmente para generar diagramas de relaciones entre proyectos y actualizar versiones.

Para compilar esta solución, es necesario tener clonado el repositorio actual [CmdTools](https://github.com/topelab/CmdTools.git) así como el repositorio de [Core.Resolver](https://github.com/topelab/Core.Resolver.git), ambos en la misma carpeta raíz.

```powershell
Set-Location C:\work\topelab
git clone https://github.com/topelab/Core.Resolver.git
git clone https://github.com/topelab/CmdTools.git
Set-Location CmdTools
dotnet build
```

## UpdateVersion

A partir de una ruta de solución de Visual Studio, intenta aumentar las versiones de los proyectos. Si no se utilizan argumentos, *UpdateVersion* intentará localizar un archivo `version.txt` con la información necesaria para actualizar la versión.

Soporta proyectos .Net MAUI, que utilizan `ApplicationDisplayVersion` en lugar de `Version`.

## RunCustomTool

Herramienta para crear una clase fuertemente tipada a partir de un archivo RESX. A partir de un archivo de recursos, genera una clase que permite acceder a los recursos de manera tipada, evitando el uso de cadenas mágicas.

## CreateRelationsDiagram

Herramienta para generar un diagrama de relaciones entre proyectos de una solución .NET. Esta herramienta permite visualizar las dependencias entre proyectos, facilitando la comprensión de la estructura de la solución.

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


### Opciones

**Verbos disponibles para `CreateRelationsDiagram`:**

- projects    (Default Verb) Manage project options for CreateRelationsDiagram.
- classes     Manage class options for CreateRelationsDiagram.
- help        Display more information on a specific command.
- version     Display version information.

**Opciones disponibles para el verbo `projects`:**

- -s, --root             Set root path
- -f, --filter           Filter by project (if not set, all projects in the solution will be processed)
- -p, --depends-on       Get projects that depends on project
- -w, --with-packages    (Default: false) Nuget packages will be collected

**Opciones disponibles para el verbo `classes`:**

- -a, --assembly     Assembly (full path to dll) where classes will be processed
- -n, --namespace    NameSpace to process (if not set, all name spaces in the solution will be processed)
- -c, --class        Class name to process (if not set, all classes in the assembly will be processed)

**Opciones comunes para todos los verbos:**

- -o, --output       Output file name (default: output to console)
- -e, --exclude      Exclude specific elements from processing (regular expression)
- -r, --reverse      (Default: false) Reverse the direction of the relations in the diagram
- -d, --direction    (Default: TopToDown) Direction of the diagram (TopToDown or TD, LefToRight or LR, RightToLeft or RL, BottomToTop or BT)
- -t, --theme        (Default: NeoDark) Theme of the diagram (Default, Base, MermaidChart, Neo, NeoDark, Forest, Dark, Neutral)
- -l, --layout       (Default: Adaptive) Layout of the diagram (Hierarchical, Adaptative)
- --help             Display this help screen.
- --version          Display version information.
