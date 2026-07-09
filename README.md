# site_manga_home

Application web ASP.NET Core Razor Pages en .NET 10 avec separation Front et Back.

## Fonctionnalites

- Landing page Front avec titre centre
- Zone Back separee sur /back
- Architecture en couches:
  - Domain
  - Application
  - Infrastructure

## Structure

- Areas/Front: pages de la partie publique
- Areas/Back: pages de la partie administration
- Domain: modeles metier
- Application: cas d'usage et interfaces
- Infrastructure: implementations techniques

## Prerequis

- .NET SDK 10

## Lancer en local

```powershell
dotnet run --project site_manga_home.csproj
```

Puis ouvrir:

- http://localhost:5056/

## Build

```powershell
dotnet build
```

## Repository

https://github.com/Zelphyna/site_manga_home
