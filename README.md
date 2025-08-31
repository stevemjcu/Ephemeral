# Ephemeral

This is an educational project similar to tools like [Onetime Secret](https://onetimesecret.com/) which allows you to create a temporary single-use link to a secret which can be shared without exposing the secret directly in the communication channel, such that it cannot be logged and leaked later.

This was created as an exercise to learn web development using a full .NET stack via the ASP.NET Core and Blazor web frameworks.

<img width="868" height="542" alt="image" src="https://github.com/user-attachments/assets/c864219b-504c-44d2-b9a0-ac2df57729eb" /><br />

Three projects:
- Ephemeral.Api: The web API server which hosts the REST API for secret management
- Ephemeral.App: The web server which hosts the web client
- Ephemeral.App.Client: The web client which handles user input and cryptography

The Blazor web app uses the Interactive WebAssembly render mode with prerendering disabled, so the browser must download 
the .NET runtime and app bundle upfront, resulting in a short initial load time.
