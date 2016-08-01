# tgc-group
[![Build status](https://ci.appveyor.com/api/projects/status/uvyboubq91uhwf3v?svg=true)](https://ci.appveyor.com/project/rejurime/tgc-group)
[![GitHub issues](https://img.shields.io/github/issues/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/issues)
[![GitHub stars](https://img.shields.io/github/stars/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/network)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/tgc-utn/tgc-group/master/LICENSE)
[![GitHub release](https://img.shields.io/github/release/tgc-utn/tgc-group.svg)](https://github.com/tgc-utn/tgc-group/releases)

Proyecto para realizar el trabajo práctico cuatrimestral de [Técnicas de Gráficos por Computadora](http://tgc-utn.github.io/)

## Características
* [Lenguaje C#](https://msdn.microsoft.com/es-ar/library/kx37x362.aspx)
* [Managed DirectX 9](https://en.wikipedia.org/wiki/Managed_DirectX)
* [Skeletal animation](https://en.wikipedia.org/wiki/Skeletal_animation)
* [HLSL shaders](https://msdn.microsoft.com/en-us/library/windows/desktop/bb509561%28v=vs.85%29.aspx)

## Requisitos
* API gráfica
    * [DirectX 9 SDK](http://www.microsoft.com/en-us/download/details.aspx?displaylang=en&id=6812)
* IDEs compatibles
    * [Visual Studio Community Edition](https://www.visualstudio.com/es-ar/products/visual-studio-community-vs)
    * [MonoDevelop](http://www.monodevelop.com) (instalar despúes de las siguientes dependencias)
        1. [Microsoft .NET Framework 4.5.2 Developer Pack](https://www.microsoft.com/es-ar/download/details.aspx?id=42637) (instalar primero)
        2. [GTK# for .NET](http://download.xamarin.com/GTKforWindows/Windows/gtk-sharp-2.12.30.msi) (instalar segundo)
        3. [Microsoft Build Tools 2015](https://www.microsoft.com/es-ar/download/details.aspx?id=48159) (instalar para poder generar la solución de tgc-group)
* Dependencia NuGet
    * [TGC.Core](https://www.nuget.org/packages/TGC.Core/) (se baja automáticamente al hacer build por primera vez)
