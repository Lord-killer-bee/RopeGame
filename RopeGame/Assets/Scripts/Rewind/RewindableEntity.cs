using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableEntity : MonoBehaviour
{
    [SerializeField] protected int rewindGroupID;

    protected EntityActionType entityActionType;
    protected bool levelInitiated;
    protected bool isRewinding;

    public virtual void OnLevelInitiated()
    {
        levelInitiated = true;
    }

    public virtual void PlayEntity()
    {
        entityActionType = EntityActionType.Play;
    }

    public virtual void RewindEntity()
    {
        entityActionType = EntityActionType.Rewind;
    }

    public virtual void StopEntity()
    {
        entityActionType = EntityActionType.Stop;
    }

    public virtual void PauseEntity()
    {
        entityActionType = EntityActionType.Pause;
    }

    public virtual void FastForwardEntity()
    {
        entityActionType = EntityActionType.FastForward;
    }

    public virtual void ActivateButtonEffect(RecorderButtonType buttonType)
    {
        switch (buttonType)
        {
            case RecorderButtonType.Play:
                PlayEntity();
                break;
            case RecorderButtonType.Pause:
                PauseEntity();
                break;
            case RecorderButtonType.Stop:
                StopEntity();
                break;
            case RecorderButtonType.Rewind:
                RewindEntity();
                break;
            case RecorderButtonType.FastForward:
                FastForwardEntity();
                break;
            case RecorderButtonType.Next:
                break;
            case RecorderButtonType.Previous:
                break;
        }
    }

    public int GetGroupID()
    {
        return rewindGroupID;
    }

}
