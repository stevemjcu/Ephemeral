# Ephemeral

This is an educational project similar to tools like [Onetime Secret](https://onetimesecret.com/).

Three projects:
- Ephemeral.Api: The web server which hosts the REST API for secret management
- Ephemeral.App: The web server which hosts the Blazor WASM web client
- Ephemeral.App.Client: The web client which handles user input and cryptography

The Blazor web app uses the Interactive WebAssembly render mode with prerendering disabled, so the browser must download 
the .NET runtime and app bundle upfront, resulting in a short initial load time.

You can refresh the OpenAPI specification via 'Connected Services' > 'Refresh'.