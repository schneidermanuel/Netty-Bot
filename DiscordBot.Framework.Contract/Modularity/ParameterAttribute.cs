using System;
using Discord;

namespace DiscordBot.Framework.Contract.Modularity;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ParameterAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsOptional { get; set; }
    public ApplicationCommandOptionType ParameterType { get; set; }
}