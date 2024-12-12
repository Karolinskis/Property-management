# Nekilnomojo turto valdymo sistema

## Uždavinio aprašymas

Nekilnojamojo turto valdymo sistema skirta padėti butų savininkams efektyviai valdyti savo turtą bei leisti nuomininkams patogiai rezervuoti ir nuomotis nekilnojamąjį turtą. Ši sistema leidžia savininkams talpinti butų nuomos pasiūlymus, sekti rezervacijas, o nuomininkams lengvai rasti butus, rezervuoti juos ir pateikti atsiliepimus apie nuomojamus objektus.

Sistemos paskirtis:

- Pateikti visą reikiamą informaciją apie laisvus nuomojamus butus.
- Leisti nuomininkams rasti butus pagal poreikius, pateikti rezervacijas, rašyti atsiliepimus.
- Suteikti administratoriui įrankius valdyti platformos turinį, vartotojus.

## Funkciniai reikalavimai

1. **Nuomininko funkcijos:**
   - Peržiūrėti butus pagal pasirinktus kriterijus (plotas, aukštas, miegamųjų skaičius, kaina ir pan.).
   - Rezervuoti butą pasirinktam laikotarpiui.
   - Stebėti nuomos būseną (laukia patvirtinimo, patvirtinta, baigta, atšaukta).
   - Palikti atsiliepimą apie nuomotą butą (įvertinimas, komentaras).
   - Peržiūrėti nuomų istoriją.
2. **Savininko funkcijos:**
   - Patalpinti informaciją apie nuomojamą butą (adresas, plotas, miegamųjų skaičius, kaina, ir t.t.).
   - Peržiūrėti visų rezervacijų sąrašą ir jų būseną.
   - Patvirtinti arba atmesti buto rezervacijas.
   - Peržiūrėti nuomininkų atsiliepimus apie butus.
   - Redaguoti arba atnaujinti buto informaciją.
3. **Administratoriaus funkcijos:**
   - Valdyti vartotojų paskyras (nuomininkų ir savininkų).
   - Redaguoti ir moderuoti butų informaciją bei atsiliepimus.
   - Turėti prieigą prie visų sistemos duomenų ir įrankių, reikalingų tvarkyti platformą.

## Pasirinktų technologijų aprašymas:

1. Backend technologijos:
   - Programavimo kalba: C#
   - Duomenų bazė: PostgreSQL
   - Autentifikacija: JWT
2. Frontend technologijos:
   - React.js
   - TailwindCSS

## Objektai:

1. **Butas**

- Kambarių skaičius: bute esančių kambarių skaičius.
- Kvdaratiniai metrai: buto plotas kvadratiniais metrais.
- Aukštas: buto aukštas pastate
- Adresas: buto adresas (gatvė, miestas, šalis).
- Nuomos kaina: mėnesinė nuomos kaina (eurais).

2. **Rezervacija / Nuoma**

- Data: rezervacijos ar nuomos sukūrimo data.
- Statusas: rezervacijos statusas (laukia patvirtinimo, patvirtinta, baigta, atšauktą).
- Pradžia: nuomos laikotarpio pradžia.
- Pabaiga: nuomos laikotarpio pabaiga.

3. **Atsiliepimas**

- Ivertinimas: nuomos įvertinimas (1-5 žvaigždutės).
- Komentaras: vieta nuomininkui pateikti savo įspūdžius apie butą ir/ar nuomos patirtį.

## Hierarchiniai ryšiai:

- **Butas -> Rezervacija**: Vienas butas gali turėti kelias rezervacijas skirtingiems laikotarpiams.
- **Rezervacija -> Atsiliepimas**: Kiekviena užbaigta rezervacija gali turėti vieną atsiliepimą nuo nuomininko.

```mermaid
classDiagram
flowchart-elk
direction LR
class Place {
    int roomsNum
    int size
    int floor
    string adress
    float price
    string userId
}
class Reservation {
    dateTime date
    status status
    dateTime start
    dateTime end
    string userId
}
Place "1" --> "0..*" Reservation
class Review {
    int stars
    string description
    string userId
}
Reservation "1" --> "0..1" Review
class Session {
   int sessionId
   string device
   string lastRefreshToken
   dateTime initiatedAt
   dateTime expiresAt
   bool isRevoked
   string userId
}
class User {
   string userId
   string email
}
Session "1" --> "1" User
User "1" --> "0..*" Place
User "1" --> "0..*" Reservation
User "1" --> "0..*" Review
class UserRole {
   string userRoleId
   string name
}
User "1" --> "0..*" UserRole
class status {
    <<enumeration>>
    waiting
    confirmed
    finished
    canceled
}
```

## Rolės:

- **Nuomininkas**: gali ieškoti butų, rezervuoti juos, palikti atsiliepimus.
- **Savininkas**: gali skelbti butus, tvarkyti rezervacijas, peržiūrėti atsiliepimus.
- **Administratorius**: turi visas valdymo funkcijas, įskaitant vartotojų administravimą, butų rezervacijų ir atsiliepimų priežiūrą.

## Paleidimo instrukcija

Norėdami paleisti šį projektą, atlikite šiuos žingsnius:

### Backend

1. **Įdiekite reikalingas priklausomybes**: Atidarykite teminalą ir eikite į `RentalManagement` katalogą. Paleiskite komandą:
   ```sh
   dotnet restore
   ```
2. **Konfigūruokite duomenų bazę**: Įsitikinkite, kad `appsettings.json` ir `appsettings.Development.json` failuose yra teisingi duomenų bazės prisijungimo duomenys.
3. **Paleiskite duomenų bazės migracijas**: Paleiskite komandą:
   ```sh
   dotnet ef database update
   ```
4. **Paleiskite backend serverį**: Paleiskite šią komandą:
   ```sh
   dotnet run
   ```

### Frontend

1. **Įdiekite reikalingas priklausomybes**: Atidarykite teminalą ir eikite į `rental-management-frontend` katalogą. Paleiskite šią komandą:
   ```sh
   npm install
   ```
2. **Paleiskite frontend serverį**: Paleiskite šią komandą:
   ```sh
   npm start
   ```

### Docker

Jei norite paleisti projektą naudojant Docker, naudokite šią komandą:

```sh
docker-compose up
```

### Naudotojo sąsajos wireframe

<table>
   <tr>
      <td><img src="docs/images/wireframe/Register.png" alt="Register" width="500" height="400"></td>
      <td><img src="docs/images/wireframe/Login.png" alt="Login" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/wireframe/Home.png" alt="Home" width="500" height="400"></td>
      <td><img src="docs/images/wireframe/Create a place.png" alt="Create a place" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/wireframe/Place.png" alt="Place" width="500" height="400"></td>
      <td><img src="docs/images/wireframe/My places and reservations.png" alt="My places and reservations" width="500" height="400"></td>
   </tr>
</table>

### Naudotojo sąsajos dizainas

<table>
   <tr>
      <td><img src="docs/images/actual/Register.png" alt="Register" width="500" height="400"></td>
      <td><img src="docs/images/actual/Login.png" alt="Login" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/actual/Home.png" alt="Home" width="500" height="400"></td>
      <td><img src="docs/images/actual/Create a place.png" alt="Create a place" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/actual/Place.png" alt="Place" width="500" height="400"></td>
      <td><img src="docs/images/actual/My places and reservations.png" alt="My places and reservations" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/actual/My Reservations.png" alt="My Reservations" width="500" height="400"></td>
   </tr>
</table>

Modalai

<table>
   <tr>
      <td><img src="docs/images/actual/Modal Leave a Review.png" alt="Modal Leave a Review" width="500" height="400"></td>
      <td><img src="docs/images/actual/Modal Reservation cancelation confirmation.png" alt="Modal Reservation cancelation confirmation" width="500" height="400"></td>
   </tr>
   <tr>
      <td><img src="docs/images/actual/Modal Reserve a place.png" alt="Modal Reserve a place" width="500" height="400"></td>
   </tr>
</table>

### UML "Deployment" diagrama

![Deployment diagram](docs/images/Deployment.jpg)

### OpenAPI specifikacija

Buvo realizuotos vietų, rezervacijų ir atsiliepimų CRUD operacijos. Taip pat buvo realizuota vartotojo autentifikacija ir autorizacija.

#### Autentifikacija

- **Registracija** `(POST /api/Authentication/Register)`: Leidžia vartotojams registruotis sistemoje pateikiant vartotojo duomenis.

  ```json
  {
    "userName": "JohnDoe",
    "email": "JohnDoe@email.com",
    "roles": ["Tennant"],
    "password": "password123"
  }
  ```

- **Prisijungimas** `(POST /api/Authentication/Login)`: Leidžia vartotojams prisijungti prie sistemos pateikiant vartotojo vardą ir slaptažodį.

  ```json
  {
    "userName": "JohnDoe",
    "password": "password123"
  }
  ```

- **Atnaujinti prisijungimo žetoną** `(POST /api/Authentication/RefreshToken)`: Leidžia atnaujinti prisijungimo žetoną.
  ```json
  {
    "userName": "JohnDoe",
    "password": "password123"
  }
  ```
- **Atsijungimas** `(POST /api/Authentication/Logout)`: Leidžia vartotojams atsijungti nuo sistemos.

#### Vietos

- **Gauti visas vietas** `(GET /api/Places)`: Grąžina visų vietų sąrašą.
- Sukurti naują vietą `(POST /api/Places)`: Leidžia sukurti naują vietą pateikiant vietos duomenis.

  ```json
  {
    "roomsCount": 3,
    "size": 100,
    "address": "1234 Main St, Springfield, IL 62701",
    "description": "Beautiful place with a view of the park",
    "price": 1000
  }
  ```

- **Gauti vietą pagal ID** `(GET /api/Places/{placeId})`: Grąžina vietos informaciją pagal pateiktą ID.
- **Atnaujinti vietą pagal ID** `(PUT /api/Places/{placeId})`: Leidžia atnaujinti vietos informaciją pagal pateiktą ID.
  ```json
  {
    "roomsCount": 3,
    "size": 100,
    "address": "1234 Main St, Springfield, IL 62701",
    "description": "Beautiful place with a view of the park",
    "price": 1000
  }
  ```
- **Ištrinti vietą pagal ID** `(DELETE /api/Places/{placeId})`: Leidžia ištrinti vietą pagal pateiktą ID.

#### Rezervacijos

- **Gauti visas rezervacijas vietai** `(GET /api/Places/{placeId}/Reservations)`: Grąžina visų rezervacijų sąrašą konkrečiai vietai.
- **Sukurti naują rezervaciją** `(POST /api/Places/{placeId}/Reservations)`: Leidžia sukurti naują rezervaciją konkrečiai vietai.
  ```json
  {
    "startDate": "2021-09-01T00:00:00Z",
    "endDate": "2021-09-10T00:00:00Z",
    "price": 100
  }
  ```
- **Gauti rezervaciją pagal ID** `(GET /api/Places/{placeId}/Reservations/{reservationId})`: Grąžina rezervacijos informaciją pagal pateiktą ID.
- **Atnaujinti rezervaciją pagal ID** `(PUT /api/Places/{placeId}/Reservations/{reservationId})`: Leidžia atnaujinti rezervacijos informaciją pagal pateiktą ID.
  ```json
  {
    "startDate": "2021-09-01T00:00:00Z",
    "endDate": "2021-09-10T00:00:00Z",
    "price": 100
  }
  ```
- **Ištrinti rezervaciją pagal ID** `(DELETE /api/Places/{placeId}/Reservations/{reservationId})`: Leidžia ištrinti rezervaciją pagal pateiktą ID.

#### Atsiliepimai

- **Gauti visus atsiliepimus vietai** `(GET /api/Places/{placeId}/Reviews)`: Grąžina visų atsiliepimų sąrašą konkrečiai vietai.
- **Gauti atsiliepimą pagal rezervacijos ID** `(GET /api/Places/{placeId}/Reservations/{reservationId}/Reviews)`: Grąžina atsiliepimą pagal rezervacijos ID.
- **Sukurti naują atsiliepimą** `(POST /api/Places/{placeId}/Reservations/{reservationId}/Reviews)`: Leidžia sukurti naują atsiliepimą konkrečiai rezervacijai.
  ```json
  {
    "rating": 5,
    "comment": "Great place!"
  }
  ```
- **Atnaujinti atsiliepimą pagal ID** `(PUT /api/Places/{placeId}/Reservations/{reservationId}/Reviews/{reviewId})`: Leidžia atnaujinti atsiliepimą pagal pateiktą ID.
- **Ištrinti atsiliepimą pagal ID** `(DELETE /api/Places/{placeId}/Reservations/{reservationId}/Reviews/{reviewId})`: Leidžia ištrinti atsiliepimą pagal pateiktą ID.

#### API Schemos

- RegisterUserDTO: Aprašo vartotojo registracijos duomenis.
- LoginUserDTO: Aprašo vartotojo prisijungimo duomenis.
- CreatePlaceDTO: Aprašo vietos kūrimo duomenis.
- PlaceDTO: Aprašo vietos duomenis.
- CreateReservationDTO: Aprašo rezervacijos kūrimo duomenis.
- ReservationDTO: Aprašo rezervacijos duomenis.
- CreateReviewDTO: Aprašo atsiliepimo kūrimo duomenis.
- ReviewDTO: Aprašo atsiliepimo duomenis.
- UpdatePlaceDTO: Aprašo vietos atnaujinimo duomenis.
- UpdateReservationDTO: Aprašo rezervacijos atnaujinimo duomenis.
- UpdateReviewDTO: Aprašo atsiliepimo atnaujinimo duomenis.
- UserDTO: Aprašo vartotojo duomenis.
- ValidationProblemDetails: Aprašo validacijos klaidų duomenis.

```

```
