using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;
using System;

public class LevelManager : MonoBehaviour
{
    private List<RewindableEntity> entitiesList = new List<RewindableEntity>();

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<LevelInitiatedEvent>(OnLevelInitiated);
        GameEventManager.Instance.AddListener<ActivateButtonEffect>(OnButtonActivated);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<LevelInitiatedEvent>(OnLevelInitiated);
        GameEventManager.Instance.RemoveListener<ActivateButtonEffect>(OnButtonActivated);
    }

    private void OnLevelInitiated(LevelInitiatedEvent e)
    {
        entitiesList = FindObjectsOfType<RewindableEntity>().ToList();

        for (int i = 0; i < entitiesList.Count; i++)
        {
            entitiesList[i].OnLevelInitiated();
        }
    }

    private void OnButtonActivated(ActivateButtonEffect e)
    {
        for (int i = 0; i < entitiesList.Count; i++)
        {
            if(entitiesList[i].GetGroupID() == e.groupID)
            {
                entitiesList[i].ActivateButtonEffect(e.recorderButtonType);
            }
        }
    }

}
