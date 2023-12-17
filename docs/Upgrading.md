# Upgrading Impostor

Sometimes we make incompatible changes to existing code. This document lists these changes and which changes you should make as a server administrator.

## Impostor 1.9.0

Previously we recommended using the Impostor.Http plugin for HTTP matchmaking. Because Among Us now relies on HTTP matchmaking, this plugin is now part of the default installation. As a result, you should change your server as follows:

- If you have Impostor.Http installed, you should remove that plugin. If you have changed the default settings, you need to move these changes to the [HttpServer section in config.json](Server-configuration.md#HttpServer).
- If you have plugins that required Impostor.Http's API (like Reactor.Impostor.Http), you should check that plugin for updates
- It is no longer necessary to add the ASP.NET Core folder to the PluginLoader's LibraryPaths

