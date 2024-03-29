﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Logger extension methods are the preferred approach for the team.")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "LINQ to SQL")]
