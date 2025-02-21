﻿namespace Mvp.Selections.Api.Model.Send;

public class Personalization
{
    public IList<Recipient> To { get; set; } = [];

    public Dictionary<string, string> Substitutions { get; set; } = [];
}