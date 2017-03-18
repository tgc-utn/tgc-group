# tgc-group
[![Build status](https://ci.appveyor.com/api/projects/status/uvyboubq91uhwf3v?svg=true)](https://ci.appveyor.com/project/rejurime/tgc-group)
[![GitHub issues](https://img.shields.io/github/issues/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/issues)
[![GitHub stars](https://img.shields.io/github/stars/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/network)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/tgc-utn/tgc-group/master/LICENSE)
[![GitHub release](https://img.shields.io/github/release/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/releases)

Proyecto plantilla para los trabajos prácticos de la asignatura electiva [Técnicas de Gráficos por Computadora](http://tgc-utn.github.io/) (TGC) en la carrera de Ingeniería en Sistemas de Información. Universidad Tecnológica Nacional, Facultad Regional Buenos Aires (UTN-FRBA).

## Características
* [.NET Framework](https://www.microsoft.com/net)
* [Managed DirectX 9](https://en.wikipedia.org/wiki/Managed_DirectX)
* [Detección de colisiones](https://en.wikipedia.org/wiki/Collision_detection)
* [Skeletal animation](https://en.wikipedia.org/wiki/Skeletal_animation)
* [HLSL shaders](https://msdn.microsoft.com/en-us/library/windows/desktop/bb509561%28v=vs.85%29.aspx)
* [TGC tools](https://github.com/tgc-utn/tgc-tools)
* Level editor
* Exportación para los modelos de [SketchUp](https://www.sketchup.com) (beta)
* Plugins para la exportación de modelos desde [3Ds MAX](http://www.autodesk.com/education/free-software/3ds-max) (outdate)

## Requisitos
* API gráfica
    * [DirectX 9 SDK](http://www.microsoft.com/en-us/download/details.aspx?displaylang=en&id=6812)
* IDEs compatibles
    * [Visual Studio Community Edition](https://www.visualstudio.com/vs/community)
    * [Xamarin Studio Community](https://www.xamarin.com/studio) (Instalar las siguientes dependencias en orden)
        1. [.NET Framework Developer Pack](https://www.microsoft.com/net/targeting)
        2. [GTK# for .NET](http://www.mono-project.com/download/#download-win)
        3. [Microsoft Build Tools 2013](https://www.microsoft.com/es-ar/download/details.aspx?id=40760)
        4. [Microsoft Build Tools 2015](https://www.microsoft.com/es-ar/download/details.aspx?id=48159)
        5. [Xamarin Studio Community](https://dl.xamarin.com/MonoDevelop/Windows/XamarinStudio-6.2.0.1829.msi)
* Dependencia NuGet
    * [TGC.Core](https://www.nuget.org/packages/TGC.Core/) (se baja automáticamente al hacer build por primera vez)
