// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure",
    Justification = "Lightspeed is a global namespace and LightspeedShared is meant to contain globally shared items.",
    Scope = "namespace",
    Target = "~N:Lightspeed")]

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure",
    Justification = "Lightspeed is a global namespace and LightspeedShared is meant to contain globally shared items.",
    Scope = "namespace",
    Target = "~N:Lightspeed.ViewModels")]

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure",
    Justification = "Lightspeed is a global namespace and LightspeedShared is meant to contain globally shared items.",
    Scope = "namespace",
    Target = "~N:Lightspeed.Network")]

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure",
    Justification = "Lightspeed is a global namespace and LightspeedShared is meant to contain globally shared items.",
    Scope = "namespace",
    Target = "~N:Lightspeed.Messages")]

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure",
    Justification = "Lightspeed is a global namespace and LightspeedShared is meant to contain globally shared items.",
    Scope = "namespace",
    Target = "~N:Lightspeed.Services")]