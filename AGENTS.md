# AGENTS - site_manga_home

## Objectif
Application ASP.NET Core Razor Pages pour gerer et consulter une collection de mangas.

## Stack
- .NET 10
- ASP.NET Core Razor Pages (Areas Front/Back)
- Injection de dependances via Program.cs

## Organisation
- `Application/Back` : use cases et interfaces administration
- `Application/Front` : use cases et interfaces lecture
- `Infrastructure/` : implementation repository (`MangaRepository`)
- `Areas/Back/Pages` : UI administration
- `Areas/Front/Pages` : UI consultation

## Commandes utiles
- Build : `dotnet build`
- Run (depuis la racine du projet) : `dotnet run`
- Run (depuis le workspace) : `dotnet run --project projects/site_manga_home/site_manga_home.csproj`

## Conventions de code
- Conserver une architecture simple type clean architecture (Application/Domain/Infrastructure).
- Preferer des changements minimaux et cibles.
- Eviter de renommer des use cases/interfaces sans mise a jour complete du DI.
- Verifier `dotnet build` apres toute modification de `Program.cs` ou des PageModels.

## Regles release (GoRoCo)
- MAJOR : rupture de compatibilite.
- MINOR : ajout retrocompatible.
- PATCH : correction retrocompatible.
- A chaque release : incrementer `<Version>` dans le `.csproj`, creer un commit clair, puis tag `vX.Y.Z`.
- Avant un commit ou une release, si le site est lance et bloque la sauvegarde des fichiers, fermer le site (arreter le process `dotnet run`) puis reprendre l'operation.

## Git
- Message de commit bref, explicite, en francais si possible.
- Ne pas inclure de fichiers `bin/` et `obj/` dans les commits.
- Si l'application en cours d'execution bloque la sauvegarde de fichiers, l'arreter avant de committer.
