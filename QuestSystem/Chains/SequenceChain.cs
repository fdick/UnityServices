using System;

[Serializable]
public class SequenceChain : QuestChainBase
{
    private int _currentQuestIndex = -1;
    public override void StartQuestChain()
    {
        base.StartQuestChain();
        StartNextQuest();
    }

    public override void UpdateChain()
    {
        if (_isDone)
            return;

        //check done dynamic quests
        for (var i = 0; i < _quests.Length; i++)
        {
            var q = _quests[i];
            if (q is not DynamicQuest d)
                continue;
            if (!d.IsDone && d != _quests[_currentQuestIndex])
                continue;

            var isDone = d.CheckQuest();
            if (!isDone)
            {
                d.StartQuest();
                StopQuestsAfterIndex(i + 1);
                _currentQuestIndex = i;
            }
            else
            {
                d.EndQuest();

                //start next quest if this quest is current
                if(d == _quests[_currentQuestIndex])
                    StartNextQuest();

            }
        }
    }

    public QuestBase GetCurrentQuest()
    {
        if (_isDone)
            return null;

        return _quests[_currentQuestIndex];
    }


    private void StartNextQuest()
    {
        _currentQuestIndex++;
        if ( _currentQuestIndex >= _quests.Length)
        {
            EndQuestChain();
            return;
        }

        //statics quests
        if (_quests[_currentQuestIndex] is not DynamicQuest)
            _quests[_currentQuestIndex].OnEnd += StartNextQuest;
        _quests[_currentQuestIndex].StartQuest();
    }

    private void StopQuestsAfterIndex(int startIndex)
    {
        if (startIndex >= _quests.Length)
            return;

        for (var i = startIndex; i < _quests.Length; i++)
        {
            var q = _quests[i];
            if (!q.IsDone && !q.IsStart)
                continue;
            q.DeactivateQuest();
            if (q is not DynamicQuest)
                q.OnEnd -= StartNextQuest;
        }
    }
}