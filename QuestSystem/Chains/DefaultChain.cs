public class DefaultChain : QuestChainBase
{
    public override void StartQuestChain()
    {
        base.StartQuestChain();

        foreach (var q in _quests)
        {
            q.StartQuest();
        }
    }

    public override void UpdateChain()
    {
        if (_isDone)
            return;
        foreach (var q in _quests)
        {
            if (q is not DynamicQuest dq) continue;

            var isDone = dq.CheckQuest();
            if (!isDone)
                dq.StartQuest();
            else
                dq.EndQuest();
        }

        var @break = false;
        //check for all quests are done
        foreach (var q in _quests)
        {
            if (!q.IsDone)
            {
                @break = true;
                break;
            }
        }

        if (@break)
            return;

        EndQuestChain();
    }
}