using System;
using UnityEngine;

[Serializable]
public abstract class QuestChainBase : MonoBehaviour
{
    [SerializeField] private string _nameChain = "Chain";
    [SerializeField] private string _commentChain = "Comment";

    [SerializeField] protected QuestBase[] _quests;
    protected bool _isDone = false;
    public QuestBase[] Quests => _quests;
    public bool IsDone => _isDone;

    public Action OnStart { get; set; }
    public Action OnEnd { get; set; }


    public virtual void StartQuestChain()
    {
        OnStart?.Invoke();
    }

    public virtual void EndQuestChain()
    {
        OnEnd?.Invoke();
        _isDone = true;
    }

    public abstract void UpdateChain();
}