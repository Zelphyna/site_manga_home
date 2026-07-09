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

## Guide complet

Consulter `GUIDE_INSTALLATION_ET_HEBERGEMENT.md` pour:

- installer et modifier le projet sur un autre ordinateur
- tester en local avec Docker
- deployer gratuitement avec GitHub + Render
- appliquer le process de release (version + tag)

## Hebergement gratuit (GitHub + Render)

Le projet est maintenant prepare pour un deploiement gratuit via Render avec un build Docker.

### Fichiers de deploiement ajoutes

- Dockerfile
- .dockerignore
- render.yaml

### Etapes a faire (une seule fois)

1. Pousser le code sur GitHub (branche principale).
2. Creer un compte sur Render: https://render.com
3. Dans Render, creer un nouveau service via Blueprint et selectionner le repository GitHub.
4. Render detecte `render.yaml` et cree automatiquement le service web.
5. Attendre la fin du premier deploy puis ouvrir l'URL fournie par Render.

### Deploy automatique

`autoDeploy: true` est active dans `render.yaml`, donc chaque push GitHub redeploie le site automatiquement.

### Test local avec Docker

```powershell
docker build -t site-manga-home .
docker run --rm -p 8080:8080 site-manga-home
```

Puis ouvrir: http://localhost:8080/

### Limites de l'offre gratuite

- Le service peut se mettre en veille apres inactivite.
- Le reveil peut prendre quelques secondes.
