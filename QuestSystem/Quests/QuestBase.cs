using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class QuestBase : MonoBehaviour
{
    [SerializeField] private string _description;
    public bool IsStart { get; private set; } = false;
    public Action OnStart { get; set; }
    public Action OnEnd { get; set; }

    [SerializeField] private UnityEvent _onStartQuest;
    [SerializeField] private UnityEvent _onEndQuest;

    private bool _isDone = false;

    public bool IsDone
    {
        get => _isDone;
        set => IsDone = value;
    }

    private void Awake()
    {
        DeactivateQuest();
    }

    public virtual void StartQuest()
    {
        if (IsStart)
            return;
        OnStart?.Invoke();
        _onStartQuest?.Invoke();
        IsStart = true;
        _isDone = false;
        gameObject.SetActive(true);
    }

    public virtual void EndQuest()
    {
        if (!IsStart)
            return;
        OnEnd?.Invoke();
        _onEndQuest?.Invoke();
        _isDone = true;
        IsStart = false;
        gameObject.SetActive(false);
    }

    public void DeactivateQuest()
    {
        IsStart = false;
        _isDone = false;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnStart = null;
        OnEnd = null;
    }
}