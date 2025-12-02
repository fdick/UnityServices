using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicQuest : QuestBase
{
    /// <summary>
    /// If quest is done returns true.
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckQuest();
}
