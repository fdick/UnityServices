using System;

public interface IItem
{
    public string UniqueName { get; }
    public bool IsCountable { get; }
    public int MaxCounts { get; }
}