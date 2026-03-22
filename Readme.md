# CoachConnect - Bokningsapplikation
CoachConnect är en ASP.NET MVC-applikation för hantering av bokningar med coacher. Systemet låter administratörer skapa coacher, hantera bokningar och tider. det låter coacher hantera sina egna tider och bokningar, samt ger användare möjlighet att boka tider med coacher och hantera sina egna bokningar. Projektet använder Identity för autentisering och roller, och SQLite som databas.

## Funktioner
- Användarroller: Admin, Coach och User.
- Autentisering: ASP.NET Identity med e-post och lösenord.
- Coach-hantering: Admin kan skapa, redigera och ta bort coacher, samt ladda upp profilbilder. De kan också hantera bokningar och tider.
- Bokningar: Användare kan boka tider hos coacher, avboka och se status på sina bokningar.
- TimeSlots: Varje coach har bokningsbara tider.
- Seed-data: Vid uppstart skapas standardroller och ett admin-konto automatiskt.

## Teknisk stack
- ASP.NET MVC 
- C#
- Entity Framework Core med SQLite
- ASP.NET Identity för autentisering och roller
- Razor Views för frontend
- Bootstrap (grundläggande styling)

## Projektstruktur
CoachBookingApp/
├─ Controllers/     # Hanterar HTTP-förfrågningar
├─ Models/          # Datamodeller: Coach, Booking, TimeSlot
├─ Views/           # Razor Views för frontend
├─ wwwroot/         # Statisk innehåll, t.ex. bilder
├─ Data/            # ApplicationDbContext
├─ Program.cs       # Startup och konfiguration

## Rollsystem
- Admin: Kan skapa och redigera coacher, se och hantera alla tider och bokningar.
- Coach: Kan hantera sina tider och bokningar.
- User: Kan hantera sina egna bokningar.

Alla användare som registreras utan roll får automatiskt rollen User.


