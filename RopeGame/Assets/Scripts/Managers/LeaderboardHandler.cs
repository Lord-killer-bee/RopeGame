using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject endpoint;
    [SerializeField]private List<LeaderboardItem> leaderboardList = new List<LeaderboardItem>();

    private List<PlayerStandingData> standingData = new List<PlayerStandingData>();

    private void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            standingData.Add(new PlayerStandingData() { distanceFromEnd = 0, competitorData = players[i].GetComponent<Competitor>() });
        }
    }

    private void Update()
    {
        UpdateStandings();
        UpdateUI();
    }

    private void UpdateStandings()
    {
        for (int i = 0; i < standingData.Count; i++)
        {
            standingData[i].distanceFromEnd = endpoint.transform.position.y - standingData[i].competitorData.transform.position.y;
        }

        standingData.Sort((x, y) => x.distanceFromEnd.CompareTo(y.distanceFromEnd));
    }

    private void UpdateUI()
    {
        for (int i = 0; i < standingData.Count; i++)
        {
            leaderboardList[i].nameText.text = standingData[i].competitorData.name;
        }
    }
}

public class PlayerStandingData
{
    public float distanceFromEnd;
    public Competitor competitorData;
}