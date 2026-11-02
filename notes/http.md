# HTTP, briefly

A client (browser, `curl`, this API's future consumers) asks a server for a resource. DNS turns `pokeapi.co` into an IP address. TCP opens a connection. TLS wraps it for HTTPS.

A request is:

- **method** — `GET` reads, `POST` creates, `PUT` replaces, `DELETE` removes
- **path** — `/api/v2/pokemon/25`
- **headers** — metadata (`Accept`, `Content-Type`, `Host`)
- **body** — optional payload (JSON for POST/PUT)

A response is a **status code** plus headers and an optional body:

- `200` OK, `201` created (look at `Location`), `204` success with no body
- `400` bad input, `404` missing, `409` unique conflict
- `5xx` the server failed

Try: `curl -v https://pokeapi.co/api/v2/pokemon/25` and read every line: DNS, TLS handshake, the request line, response headers, then JSON.
