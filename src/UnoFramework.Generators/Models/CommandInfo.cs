// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace UnoFramework.Generators.Models;

/// <summary>
/// A model representing the information for a generated command.
/// </summary>
/// <param name="MethodName">The name of the target method.</param>
/// <param name="CommandName">The name of the generated command property.</param>
/// <param name="FieldName">The name of the backing field for the command.</param>
/// <param name="IsAsync">Whether the command is async.</param>
/// <param name="SupportsCancellation">Whether the async command supports cancellation.</param>
/// <param name="IncludeCancelCommand">Whether to include a cancel command.</param>
/// <param name="CancelCommandName">The name of the cancel command property.</param>
/// <param name="CancelCommandFieldName">The name of the backing field for the cancel command.</param>
/// <param name="ParameterType">The parameter type of the command (null if parameterless).</param>
/// <param name="ContainingTypeName">The name of the containing type.</param>
/// <param name="ContainingTypeNamespace">The namespace of the containing type.</param>
/// <param name="ContainingTypeAccessibility">The accessibility of the containing type.</param>
/// <param name="Busy">The busy scope for the command.</param>
/// <param name="BusyMessage">The optional busy message to display during execution.</param>
internal sealed record CommandInfo(
    string MethodName,
    string CommandName,
    string FieldName,
    bool IsAsync,
    bool SupportsCancellation,
    bool IncludeCancelCommand,
    string? CancelCommandName,
    string? CancelCommandFieldName,
    string? ParameterType,
    string ContainingTypeName,
    string ContainingTypeNamespace,
    string ContainingTypeAccessibility,
    int Busy,
    string? BusyMessage);
