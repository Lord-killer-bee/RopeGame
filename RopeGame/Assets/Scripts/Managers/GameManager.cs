using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private ReferenceHolder referenceHolder;

    private int currentLevel;
    private bool levelInitiated = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        GameEventManager.Instance.AddListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.AddListener<StartGameEvent>(OnGameStarted);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameEventManager.Instance.RemoveListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.RemoveListener<StartGameEvent>(OnGameStarted);
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !levelInitiated)
        {
            referenceHolder.spaceText.SetActive(false);
            levelInitiated = true;
        }
    }

    private void OnPlayerCompletedLevel(PlayerReachedEndEvent e)
    {
        currentLevel++;
        referenceHolder.transitionAnim.Play("FadeOut");
        Invoke("LoadSceneWithDelay", 0.8f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if(scene.buildIndex > 0)
        {
            referenceHolder = GameObject.Find("ReferenceHolder").GetComponent<ReferenceHolder>();
            referenceHolder.transitionAnim.Play("FadeIn");
        }
    }

    public void StartGame()
    {
        currentLevel = 1;
        SceneManager.LoadScene(currentLevel);
    }

    private void OnGameStarted(StartGameEvent e)
    {
        currentLevel = 1;
        SceneManager.LoadScene(currentLevel);
    }

    void LoadSceneWithDelay()
    {
        SceneManager.LoadScene(currentLevel);
        levelInitiated = false;
    }
}
