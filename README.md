# OS2Indberetning

OS2Indberetning was programmed by IT Minds ApS (http://it-minds.dk) for OS2 - Offentligt digitaliseringsfællesskab (http://os2.eu).

Copyright (c) 2016, OS2 - Offentligt digitaliseringsfællesskab.

OS2Indberetning is free software; you may use, study, modify and distribute it under the terms of version 2.0 of the Mozilla Public License. See the LICENSE file for details. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.

All source code in this and the underlying directories is subject to the terms of the Mozilla Public License, v. 2.0.

## Opsætning

Tilpasning og opsætning af løsning gøres igennem konfigurationsfiler som ligger i projekternes rodmapper. Herunder gennemgåes de forskellige projekter og de relevante konfigurationsfiler, hvor der skal foretages ændringer.

### Presentation.Web

#### CustomSettings.config

##### Mailopsætning
Nedenfor beskrives de oplysninger der skal indtastes til opsætning af SMTP-server.
Serveren bruges til at sende mails til brugere, når en af deres indberetninger bliver afvist.

###### PROTECTED_SMTP_PASSWORD
Udfyldes med password til SMTP-server

###### PROTECTED_SMTP_HOST
udfyldes med adresse til SMTP-server

###### PROTECTED_SMTP_USER
udfyldes med brugernavn til SMTP-server

###### PROTECTED_SMTP_HOST_PORT
Udfyldes med port til SMTP-server

###### PROTECTED_MAIL_FROM_ADDRESS
Udfyldes med den mail der skal stå som afsender

###### PROTECTED_MAIL_SUBJECT
Udfyldes med emnet på mails om afventende indberetninger til ledere

###### PROTECTED_MAIL_BODY
Udfyldes med indholdet af mails om afventende indberetninger

##### KMD-opsætning
Nedenfor beskrives de oplysninger der skal indtastes i forbindelse med generering af fil til KMD ind01 snitflade.

###### PROTECTED_KMDFilePath
Udfyldes med den sti hvor filen til KMD skal gemmes, når en administrator vælger at generere den

###### PROTECTED_KMDFileName
Navn på den genererede fil til KMD.

###### PROTECTED_KMDHeader
Første linje i fil til KMD

###### PROTECTED_KMDStaticNr
Udfyldes med statisk KMD-nummer

###### PROTECTED_CommuneNr
Udfyldes med kommunenummer til KMD

###### PROTECTED_KMDReservedNr
Udfyldes med reserveret KMD-nummer

##### Startadresse til kort
Nedenfor beskrives de oplysninger der skal indtastes for at vælge den adresse kortet skal starte med at vise i kørselsindberetning.

###### MapStartStreetName
Udfyldes med vejnavn for startadresse.

###### MapStartStreetNumber
Udfyldes med vejnummer for startadresse.

###### MapStartZipCode
Udfyldes med postnummer for startadresse.

###### MapStartTown
Udfyldes med by for startadresse.

##### AD-domæne opsætning

###### PROTECTED_AD_DOMAIN
Udfyldes med AD-domænenavn.

##### Hjælpetekster
Nedenfor beskrives de felter der skal udfyldes for diverse hjælpetekster i OS2Indberetning. Hvis et felt efterlades tomt
vil der ikke blive vist et hjælpeikon på hjemmesiden.

###### InformationHelpLink
Udfyldes med link til den side der linkes til under Information og vejledning øverst til højre på hjemmesiden.

###### TableSortHelp
Udfyldes med den hjælpetekst der vises over tabeller med indberetninger.
Hjælpeteksten vises på Mine indberetninger/Godkend indberetninger og Admin->Indberetninger

###### FourKmRuleHelpText
Udfyldes med hjælpetekst der beskriver 4 km-reglen.
Hjælpeteksten vises under Indberet Tjenestekørsel

###### MobileTokenHelpText
Udfyldes med hjælpetekst der beskriver tokens til mobil-app.
Hjælpeteksten vises under Personlige indstillinger.

###### AlternativeWorkAddressHelpText
Udfyldes med hjælpetekst der beskriver afvigende arbejdsadresse.
Hjælpeteksten vises under Personlige indstillinger.

###### PrimaryLicensePlateHelpText
Udfyldes med hjælpetekst der beskriver primær nummerplade.
Hjælpeteksten vises under Personlige indstillinger.

###### PersonalApproverHelpText
Udfyldes med hjælpetekst der beskriver personlig godkender.
Hjælpeteksten vises under Godkend indberetninger->Stedfortrædere/Godkendere og Admin->Stedfortrædere og godkendere

###### EmailHelpText
Udfyldes med hjælpetekst der beskriver email-notifikationer.
Hjælpeteksten vises under Admin->Emailadviseringer

###### PurposeHelpText
Udfyldes med hjælpetekst der beskriver formål med tjenestekørselsindberetning.
Hjælpeteksten vises under Indberet tjenestekørsel.

###### NoLicensePlateHelpText
Udfyles med hjælpetekst der fortæller brugeren han ingen nummerplader har.
Hjælpeteksten vises under Indberet tjenestekørsel, når man ingen nummerplader har.

###### ReadReportCommentHelp
Udfyldes med hjælpetekst der beskriver kommentar til aflæst tjenestekørselsindberetning.
Hjælpeteksten vises under Indberet tjenestekørsel, når man har valgt aflæst indberetning.

##### Ruteberegning

###### PROTECTED_SEPTIMA_API_KEY
Udfyldes med api-nøgle til Septimas ruteberegner.

#### connections.config
Udfyldes med oplysninger om OS2Indberetnings databasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen

#### favicon.ico
Erstattes med kommunelogo til visning på fane i browser.

#### logo.png
Erstattes med kommunelogo til visning på hjemmesiden øverst til højre.

### DBUpdater
Dette projekt bruges til at migrere oplysninger om medarbejdere og organisationer fra kommunen til OS2Indberetning.

#### connections.config

##### DefaultConnection
Bruges til at oprette forbindelse til OS2Indberetningsdatabasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen

##### DBUpdaterConnection
Bruges til at oprette forbindelse til kommunedatabasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen

### Mail
Dette projekt bruges til at sende planlagte mailadviseringer til ledere, der har afventende indberetninger til godkendelse.

#### connections.config
Udfyldes med oplysninger om OS2Indberetnings databasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen

#### CustomSettings.config
Nedenfor beskrives de oplysninger der skal bruges til at sende mails til ledere, der har afventende indberetninger til godkendelse.

###### PROTECTED_SMTP_PASSWORD
Udfyldes med password til SMTP-server

###### PROTECTED_SMTP_HOST
udfyldes med adresse til SMTP-server

###### PROTECTED_SMTP_USER
udfyldes med brugernavn til SMTP-server

###### PROTECTED_SMTP_HOST_PORT
Udfyldes med port til SMTP-server

###### PROTECTED_MAIL_FROM_ADDRESS
Udfyldes med den mail der skal stå som afsender

###### PROTECTED_MAIL_SUBJECT
Udfyldes med emnet på mails om afventende indberetninger til ledere

###### PROTECTED_MAIL_BODY
Udfyldes med indholdet af mails om afventende indberetninger

### Infrastructure.DmzSync
Dette projekt bruges til at synkronisere indberetninger fra mobilapp til OS2Indberetnings databasen.

#### connections.config

##### DefaultConnection
Bruges til at oprette forbindelse til OS2Indberetningsdatabasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen

##### DMZConnection
Bruges til at oprette forbindelse til DMZ-databasen.

###### Data Source
Udfyldes med adressen på databaseserveren.

###### Initial Catalog
Udfyldes med navnet på databasen.

###### uid
Udfyldes med brugernavn til login på databasen.

###### pwd
Udfyldes med adgangskode til databasen
