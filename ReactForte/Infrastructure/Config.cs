using System;
using System.Collections.Generic;

namespace ReactForte.Infrastructure;

internal class Config
{
    public List<string> ScriptUrls { get; set; } = new();
    public bool IsServerSideDisabled { get; set; } = false;
    public Version ReactVersion { get; set; } = null!;
}