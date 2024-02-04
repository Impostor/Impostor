﻿using System;
using Impostor.Api;

namespace Impostor.Server.Plugins;

public class PluginLoaderException : ImpostorException
{
    public PluginLoaderException()
    {
    }

    public PluginLoaderException(string? message) : base(message)
    {
    }

    public PluginLoaderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
