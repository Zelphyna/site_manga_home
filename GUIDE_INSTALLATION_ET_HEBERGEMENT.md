# Guide installation, modification et hebergement

Ce document explique tout ce qui est necessaire pour reprendre le projet sur un autre ordinateur, le modifier, puis le publier en ligne.

## 1. Prerequis sur un autre ordinateur

- Git
- .NET SDK 10
- (Optionnel) Docker Desktop pour les tests conteneurises
- Un compte GitHub
- Un compte Render (pour l'hebergement gratuit propose)

## 2. Recuperer le projet

```powershell
git clone https://github.com/Zelphyna/site_manga_home.git
cd site_manga_home
```

## 3. Ouvrir et modifier le projet

1. Ouvrir le dossier dans VS Code.
2. Restaurer/Compiler:

```powershell
dotnet build
```

3. Lancer en local:

```powershell
dotnet run --project site_manga_home.csproj
```

4. Ouvrir dans le navigateur l'URL affichee (souvent http://localhost:5056/).

## 4. Architecture utile pour developper

- Application/Back: cas d'usage et interfaces administration
- Application/Front: cas d'usage et interfaces consultation
- Domain: modeles metier
- Infrastructure: implementation de repository
- Areas/Front/Pages: UI publique
- Areas/Back/Pages: UI administration

## 5. Fichiers de deploiement deja prepares

- Dockerfile: image de production de l'application
- .dockerignore: exclusion des fichiers inutiles au build Docker
- render.yaml: definition du service web sur Render
- Program.cs: configuration des headers proxy pour hebergement derriere reverse proxy

## 6. Tester en local avec Docker (optionnel)

```powershell
docker build -t site-manga-home .
docker run --rm -p 8080:8080 site-manga-home
```

Puis ouvrir http://localhost:8080/

## 7. Heberger gratuitement avec GitHub + Render

1. Pousser le code sur GitHub (branche principale).
2. Se connecter a Render.
3. Creer un nouveau service avec Blueprint depuis le repository GitHub.
4. Render lit automatiquement render.yaml.
5. Attendre le premier deploy puis utiliser l'URL publique fournie.

## 8. Mettre a jour le site apres une modification

```powershell
git add .
git commit -m "Message clair"
git push
```

Avec autoDeploy active, Render redeploie automatiquement apres chaque push.

## 9. Limites de la version gratuite Render

- Le service peut se mettre en veille apres inactivite.
- Le reveil prend quelques secondes.
- Ressources limitees (CPU/RAM/temps de build).

## 10. Process release (GoRoCo)

- MAJOR: rupture de compatibilite
- MINOR: ajout retrocompatible
- PATCH: correction retrocompatible

Procedure:

1. Incrementer la version dans site_manga_home.csproj.
2. Commit avec message clair.
3. Creer un tag de release vX.Y.Z.
4. Pousser commit et tag sur GitHub.
