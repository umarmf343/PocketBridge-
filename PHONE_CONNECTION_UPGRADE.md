# PocketBridge 1.1 — phone connection and consent-based folder browsing

## What was actually wrong

The laptop was already connected to the Galaxy S20 FE hotspot (10.223.249.65/24), but no listener existed on TCP 48321 when inspected. The original elevation flow launched an admin window without actually starting sharing; a second click was needed. The old "Open local page" button only opened a browser on the laptop. That neither paired the phone nor granted access to its storage. Windows-only loopback tests could not establish that a real phone connection had occurred.

## Changes

- Enable & connect now starts the server automatically after UAC, preserves the adapter selection/shared paths, and generates an offline QR code for the phone to open.
- The former browser button is labelled **Laptop preview only**, warns explicitly, and a paired laptop browser shows a preview warning. A laptop preview cannot publish a phone file selection.
- The server reports whether a remote device has reached it. **PHONE SELECTION ONLINE** means a phone browser has published and is polling its selection; "remote device reached" alone does not imply successful pairing or file permission.
- **Rescan addresses** reports time, discovered interface count, and preserves the selected adapter. It enumerates local IPs rather than searching for Wi-Fi networks or changing Wi-Fi/hotspot settings.
- On the PHONE page, select files or use the optional folder picker, then **Share selection with laptop**. Folder selection support varies by Android browser/version. If unsupported, use multiple file selection. No unselected phone storage is exposed.
- The desktop **Browse shared PHONE selection** tab can navigate folders within that published selection, check files/folders, and request copies. Requests are scoped to the publishing browser's authenticated session; the phone page must stay open and awake. Copies preserve the selected relative folder structure in the laptop Inbox without overwriting existing files. A phone user can revoke the selection or disconnect. Only queued copies can be canceled on the laptop; cancel an active upload on the phone.
- QR rendering uses QRCoder 1.6.0, MIT licensed, embedded in the executable. It makes no QR generation web-service requests; no mobile data is needed. License is included in the installation.
- Installation is versioned in `%LOCALAPPDATA%/Programs/PocketBridge/versions/1.1.0`; desktop/Start-menu shortcuts point there. Older versions and the Inbox are retained.

## Important remaining boundary

This is permission-based browser sharing, not unrestricted phone-file-system access. Full ongoing folder access on Android requires a separate Android companion and Android-granted storage permissions (for example ACTION_OPEN_DOCUMENT_TREE). Private app data and some protected directories remain restricted even then. An .exe alone cannot grant Android storage permission. No Android companion APK is included in this release.

If the phone still cannot open the QR's HTTP link: confirm this laptop is on the phone's hotspot, keep mobile data off, avoid HTTPS-only rewriting of the local HTTP URL, review VPN settings and explicit DataGate/other firewall blocks, and check for hotspot client isolation. PocketBridge does not disable security controls, reset firewalls or turn on cellular data.

## Validation boundaries

Regression tests cover safe filenames, relative traversal rejection, ownership-scoped phone manifests/jobs, queue state, folder-path preservation, embedded QR generation and dashboard construction. Elevated local integration tests exercise pairing, anonymous rejection, upload, duplicate preservation, downloads and CSRF handling. These do not substitute for pairing the actual phone: that requires scanning the QR and granting selection permissions on the device.

References:
- Android folder permissions: https://developer.android.com/training/data-storage/shared/documents-files
- Browser folder selection: https://developer.chrome.com/docs/capabilities/browser-fs-access
- QRCoder: https://www.nuget.org/packages/QRCoder/1.6.0
