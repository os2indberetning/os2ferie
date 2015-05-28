# OS2Indberetning

## Opsætning

Tilpasning og opsætning af løsning gøres igennem konfigurationsfiler som ligger i projekternes rodmapper. Herunder gennemgåes de forskellige projekter og de relevante konfigurationsfiler, hvor der skal foretages ændringer.

### Presentation.Web

#### CustomSettings.config

##### Mailopsætning
Nedenfor beskrives de oplysninger der skal indtastes til opsætning af SMTP-server.
Serveren bruges til at sende mails til ledere, der har afventende indrapporteringer til godkendelse.

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
Udfyldes med emnet på mails om afventende indrapporteringer til ledere

###### PROTECTED_MAIL_BODY
Udfyldes med indholdet af mails om afventende indrapporteringer

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

###### ReadReportCommentHelp
Udfyldes med hjælpetekst der beskriver kommentar til aflæst tjenestekørselsindberetning.
Hjælpeteksten vises under Indberet tjenestekørsel, når man har valgt aflæst indberetning.

##### Ruteberegning

###### PROTECTED_SEPTIMA_API_KEY
Udfyldes med api-nøgle til Septimas ruteberegner.


