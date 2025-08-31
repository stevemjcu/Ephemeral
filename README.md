# Ephemeral

This is an educational project similar to tools like [Onetime Secret](https://onetimesecret.com/) which allow you to create a temporary link to a secret which can be shared without exposing the secret directly in a communication channel, such that it cannot be logged and leaked later.

This was created as an exercise to learn web development using a full .NET stack via ASP.NET Core and Blazor.

<img width="868" height="542" alt="image" src="https://github.com/user-attachments/assets/c864219b-504c-44d2-b9a0-ac2df57729eb" /><br />

Three projects:
- Ephemeral.Api: The web server which hosts the REST API for secret management
- Ephemeral.App: The web server which hosts the web client
- Ephemeral.App.Client: The web client which handles user input and cryptography

The web app uses Blazor's Interactive WebAssembly (WASM) render mode with prerendering disabled, so the browser will download the .NET runtime and app bundle upfront on your first visit.
