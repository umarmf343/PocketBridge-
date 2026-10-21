# PocketBridge 1.0 — local phone ↔ laptop transfers

PocketBridge is a Windows desktop application for two-way file transfers between a phone browser and a laptop over a trusted local Wi-Fi network or phone hotspot. It uses pairing codes and a browser-based phone portal without a cloud relay or a companion phone app.

Native C# Windows Forms app for Windows x64 with .NET Framework 4.8. No Node.js, Python, phone app, cloud relay, telemetry, third-party website, CDN, internet download or mobile-data connection is used at runtime. Phone interface assets are embedded in the executable.

## Use without internet

1. Turn off **Mobile data** on the phone. Turn on its password-protected hotspot, and connect the laptop to that hotspot. If the laptop warns there is no internet, stay connected. Some phones/carriers have different offline hotspot behavior; if yours refuses to enable without mobile data, use another offline Wi-Fi network instead. PocketBridge itself cannot control the phone's cellular settings or unrelated applications' internet usage.
2. Open PocketBridge. Choose the hotspot-connected IPv4 adapter; click **Enable & start (Admin)** and approve UAC. The elevated window preserves your selected shared-file paths; click **Start local sharing** there. It creates only a temporary inbound rule for the chosen local IP, TCP 48321 and LocalSubnet, covering public/private/domain profiles because phone hotspots often appear Public. HTTP.sys handles the listener, so the inbound rule is port/IP scoped, not incorrectly filtered to the UI process.
3. Type the displayed `http://<laptop-IP>:48321/` address into your phone browser's address bar. Do not enter it into a search engine. Enter the six-digit pairing code shown on the laptop. The code rotates when sharing stops/restarts.
4. **Laptop → phone:** Add or drag individual files into the laptop's shared-files list. Refresh the phone page and tap Download. Android's browser saves them in its Downloads location; protected storage destinations are governed by Android/browser permissions. Unshare removes access from the list without deleting the original laptop file. Folders must be zipped first.
5. **Phone → laptop:** Select files in the phone page using Android's file picker, then Send. Files stream to `%USERPROFILE%/Downloads/PocketBridge Inbox`. Open received files on the laptop. Original phone files are not deleted. Existing filenames are preserved by generating numbered filenames. Partial/interrupted uploads aren't published, and normal cancellation discards the partial file. An abrupt laptop power loss may leave a `.pb-*.partial` file for later manual removal.
6. Stop sharing or close the app when finished. Pairing sessions are revoked and the session's firewall rule is removed. If the phone page says the session expired, pair again; after laptop IP changes, restart sharing using the new IP.

## Boundaries and safety

This is **not remote browsing of the whole phone**. Without a companion app/Android-granted storage access, a laptop cannot enumerate arbitrary phone files. Android deliberately requires the user to select what to send. The browser portal provides two-way, consent-based file transfer instead of claiming otherwise.

The local HTTP connection is **not TLS encrypted**. Use only a trusted private hotspot; anyone who knows its password and observes local traffic could compromise pairing/files. Pairing is rate limited, uses random session cookies, checks exact Host/Origin and CSRF headers, sets no-store/nosniff/frame restrictions, and exposes only deliberately shared file IDs. There is no arbitrary filesystem endpoint, execution of received files, inbound WAN forwarding, UPnP, or outbound internet connection. Do not use a public/untrusted network or forward this port on a router.

Up to four concurrent HTTP requests are processed. Files up to 8 GiB are accepted if enough disk space is available, streamed through 64 KiB buffers. This limit is not a claim of an 8 GiB benchmark. No resumable transfers or folder synchronization are implemented. Completed files remain after cancellation or stopping. Browser download progress is managed by the browser; upload progress and cancellation are shown on the phone portal.

DataGate Focus or other explicit firewall blocks can prevent the local listener/phone browser from working. PocketBridge does **not** reset, remove, or override those restrictions. Review your blocking rules if pairing is unreachable. If a phone hotspot isolates its own connected clients from the host browser, another trusted offline Wi-Fi network may be required.

Normal closing removes the app's temporary firewall rule. A crash/forced process termination can leave a `PocketBridge.Local.*` rule in Windows Firewall; the HTTP endpoint still stops with the process, and the stale rule can be removed manually. The app does not register a persistent Windows service, scheduled task, startup job or URL reservation.

## Build and validation

Run `build.ps1`, then `release/PocketBridge.exe --self-test`. Elevated `--server-test` performs a local integration test: listener, anonymous denial, pairing, CSRF rejection, upload, duplicate preservation, byte-identical download and firewall-rule cleanup. It does not fetch anything from the internet or inspect user files. Its small generated test files are retained in a uniquely named Temp directory.

`PocketBridge-Setup.exe` installs the one-file application under `%LOCALAPPDATA%/Programs/PocketBridge`, adds Desktop and Start-menu shortcuts, and opens it. `--install` performs the same install quietly without starting a server. It is a simple per-user installer, not an MSI, and is unsigned. Stop sharing before uninstalling by deleting its app folder and shortcuts. This never deletes the Inbox or user originals.
