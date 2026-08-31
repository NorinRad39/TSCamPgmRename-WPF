; =============================================================================
;  Configuration propre au projet.
;
;  C'est le SEUL fichier a adapter. Copiez-le en "deploy.config.iss" a cote de
;  deploy.iss, puis renseignez les valeurs ci-dessous.
;
;  Le nom de l'application n'est ecrit qu'une fois : tout ce qui en decoule est
;  derive par concatenation. Une copie d'un projet a l'autre oublie toujours une
;  occurrence quand chaque ligne repete le nom.
;
;  Tous les chemins relatifs partent du dossier qui contient deploy.iss.
; =============================================================================


; --- Le seul nom a changer ----------------------------------------------------

; Ce que l'utilisateur lit : menu Demarrer, raccourci, Panneau de configuration.
; Les espaces y sont les bienvenus.
#define AppName "TSCamPgmRename"


; --- Ce qui en decoule --------------------------------------------------------

; Nom technique : dossier d'installation (%LOCALAPPDATA%\<AppSlug>), nom du
; fichier setup, identifiant de type de fichier. Ni espace ni caractere exotique
; -- c'est un chemin, et il finira dans des lignes de commande.
;
; Quand AppName n'a ni espace ni tiret, "#define AppSlug AppName" suffit. Sinon,
; ecrivez-le en toutes lettres : "MonApplication".
#define AppSlug AppName

#define ExeName AppSlug + ".exe"

; Dossier reseau ou atterrissent le setup et update.xml, et que AutoUpdater
; interroge a chaque lancement de l'application.
;
; Chemin UNC, jamais une lettre de lecteur : un mappage est propre a la session,
; et sur un poste ou il manque les mises a jour cesseraient sans que personne ne
; s'en apercoive.
#define UpdateFolder "\\jbtec-be\meca$\topsolid\TSCamPgmRename" + AppSlug

#define AppPublisher "Florent FABBRI"


; --- Propre a ce projet, non derivable ----------------------------------------

; Identifiant unique et DEFINITIF de l'application. Inno s'en sert pour
; reconnaitre une installation existante et la mettre a jour au lieu d'en
; empiler une seconde.
;
; A generer une fois pour toutes, puis a ne plus jamais changer :
;     powershell -c "'{{' + [guid]::NewGuid().ToString().ToUpper() + '}'"
;
; Deux projets qui partagent cet identifiant, et Inno prend l'un pour une mise a
; jour de l'autre : installer le second ecrase le premier sur tous les postes.
;
; Les doubles accolades ne sont pas une faute : Inno lit "{{" comme une accolade
; litterale.
#define AppId "{{00000000-0000-0000-0000-000000000000}"

; Dossier de sortie de la compilation Release. Tout son contenu part dans le
; setup, sous-dossiers compris.
;
; Attention a la profondeur : "..\<Projet>\bin\Release" quand le .csproj est dans
; un sous-dossier, "..\bin\Release" quand il est a la racine du depot.
#define SourceDir "..\MonApplication\bin\Release"

; Icone du programme d'installation.
#define IconFile "..\MonApplication\app.ico"


; --- Options (laissez la ligne en commentaire si la fonction ne sert pas) ------

; Extensions a associer a l'application, separees par des virgules.
;
; Attention a ce que cela fait reellement sous Windows 10 et 11 : pour une
; extension inconnue du systeme, l'application devient bien celle qui l'ouvre.
; Pour une extension deja prise en charge (.pdf, .txt...), l'ecriture en HKCU
; ajoute seulement l'application au menu "Ouvrir avec" -- le choix par defaut
; est protege et seul l'utilisateur peut le changer depuis les Parametres.
;#define FileExtensions ".mon,.autre"

; Certificat a deposer dans le magasin TrustedPublisher de l'utilisateur, et cle
; privee servant a signer le setup. La cle n'est pas versionnee : sur un poste
; qui ne l'a pas, la signature est sautee et la publication aboutit quand meme.
;#define CertFile "cert\" + AppSlug + ".cer"
;#define CertPfx  "cert\" + AppSlug + ".pfx"

; Nom de famille d'un paquet MSIX a desinstaller avant d'installer, pour migrer
; un poste depuis un ancien deploiement MSIX. Sans cela les deux versions
; cohabitent et se disputent les associations de fichiers.
;
; A ne renseigner que si CETTE application a bien ete empaquetee en MSIX : y
; laisser l'identifiant d'une autre desinstallerait cette autre application.
;#define MsixPackageName "00000000-0000-0000-0000-000000000000"
