# Deploy-Vorlage für eine MovieScreeningApp-Instanz (Docker Compose)

Zieht die von der GitHub-Action gebauten Images aus der Registry und betreibt sie hinter Traefik
(Netz `traefik_reverse_proxy`, Entrypoint `websecure`). Frontend und Backend laufen auf demselben
Host; Traefik leitet `/api/*` ans Backend, alles andere an nginx.

## Vor dem ersten Start

1. Ordner auf den Server kopieren, z. B. nach `/opt/movie-screening-app`.
2. **`.env`** ausfüllen: `MARIADB_*`, `ADMIN_TOKEN`, `HOSTNAME`, `INSTANCE`, `REGISTRY_HOST`, `VERSION`.
   `INSTANCE` ist ein kurzer Name, der Traefik-Router und DB-Hostname eindeutig macht, falls mehrere
   Instanzen auf demselben Host laufen.
3. Verzeichnisse anlegen und Rechte setzen — das Backend-Image läuft als Benutzer `app` (UID 1654):
   ```
   mkdir -p data/filestore data/db
   chown -R 1654:1654 data/filestore
   chmod 600 .env
   ```
4. Bei der Registry anmelden (`docker login <REGISTRY_HOST>`) und starten:
   ```
   docker compose up -d
   ```
   Das Backend wartet auf die Datenbank und legt die Tabellen beim Start selbst an.
5. Einmalig den API-Key der Streaming Availability API hinterlegen:
   ```
   curl -X PUT https://<HOSTNAME>/api/ApiKeys/StreamingAvailabilityApi \
     -H "X-Admin-Token: <ADMIN_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{ "value": "<API-KEY>" }'
   ```
   Der Key wird verschlüsselt in der Datenbank abgelegt. Der zugehörige Data-Protection-Key-Ring
   liegt in `data/filestore` — dieses Verzeichnis muss erhalten bleiben, sonst ist der Key nach
   einem Neustart nicht mehr lesbar und muss erneut gesetzt werden.

## Update auf eine neue Version

`VERSION` in `.env` auf den neuen CalVer-Tag setzen, dann:
```
docker compose pull
docker compose up -d
```
