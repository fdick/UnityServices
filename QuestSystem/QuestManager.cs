using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestChainBase[] _chains;

    public QuestChainBase[] Chains => _chains;

    public void Start()
    {
        StartChains();
    }

    public void Update()
    {
        foreach (var c in _chains)
        {
            c.UpdateChain();
        }
    }

    private void StartChains()
    {
        foreach (var chain in _chains)
        {
            chain.StartQuestChain();
        }
    }

}
