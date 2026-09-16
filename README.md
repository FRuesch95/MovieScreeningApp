# Movie Screening App

Search movies via the [Streaming Availability API](https://docs.movieofthenight.com/), filter by title, year and genre, sort and paginate the results, and keep a list of favourites.

## Stack

- **Frontend:** Angular 22, Bootstrap 5
- **Backend:** ASP.NET Core 10 Web API, EF Core, MariaDB

## Run locally

Requirements: .NET 10 SDK, Node 24, a MariaDB instance (connection string in `MovieScreeningApp.Api/appsettings.Development.json`).

```
cd MovieScreeningApp.Api
dotnet run                     # migrates the database on startup, listens on http://localhost:5262

cd MovieScreeningApp.Frontend
npm ci
npm start                      # http://localhost:4200, proxies /api to the backend
```

Set the Streaming Availability API key once (stored encrypted in the database):

```
curl -X PUT http://localhost:5262/api/ApiKeys/StreamingAvailabilityApi \
  -H "X-Admin-Token: dev-admin-token-change-me" \
  -H "Content-Type: application/json" \
  -d '{ "value": "<API-KEY>" }'
```

## Tests

```
dotnet test                                    # backend
cd MovieScreeningApp.Frontend && npm test      # frontend
```

## Notes

- The API free plan allows 1,000 requests per month. All calls go through the backend; genres are cached, favourites are stored locally so viewing them costs no requests.
- Search is limited to movies in `country=de` with English titles.
