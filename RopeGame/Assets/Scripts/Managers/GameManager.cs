using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private ReferenceHolder referenceHolder;

    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioClip levelStartedEffect;

    private int currentLevel;
    private bool levelInitiated = false;

    private bool gameStarted;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        GameEventManager.Instance.AddListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.AddListener<PlayerHitSpikeEvent>(OnPlayerHitSpike);
        GameEventManager.Instance.AddListener<KeyHitSpikeEvent>(OnKeyHitSpike);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameEventManager.Instance.RemoveListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.RemoveListener<PlayerHitSpikeEvent>(OnPlayerHitSpike);
        GameEventManager.Instance.RemoveListener<KeyHitSpikeEvent>(OnKeyHitSpike);
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !levelInitiated)
            {
                referenceHolder.spaceText.SetActive(false);
                levelInitiated = true;

                GameEventManager.Instance.TriggerSyncEvent(new LevelInitiatedEvent());
            }

            if (levelInitiated && Input.GetKeyDown(KeyCode.R))
            {
                PlayFadeOut();
                Invoke("LoadSceneWithDelay", 0.8f);

                GameEventManager.Instance.TriggerSyncEvent(new ResetLevelEvent());
            }
        }
    }

    private void OnPlayerCompletedLevel(PlayerReachedEndEvent e)
    {
        currentLevel++;
        PlayFadeOut();
        Invoke("LoadSceneWithDelay", 0.8f);
    }

    private void OnPlayerHitSpike(PlayerHitSpikeEvent e)
    {
        Invoke("PlayFadeOutWithDelay", 0.2f);
    }

    private void PlayFadeOutWithDelay()
    {
        PlayFadeOut();
        Invoke("LoadSceneWithDelay", 0.8f);
    }

    private void OnKeyHitSpike(KeyHitSpikeEvent e)
    {
        PlayFadeOut();
        Invoke("LoadSceneWithDelay", 0.8f);
    }

    void PlayFadeIn()
    {
        GameObject player = GameObject.FindObjectOfType<Player>().gameObject;

        effectsAudioSource.PlayOneShot(levelStartedEffect);

        referenceHolder.transitionAnim.GetComponent<RectTransform>().anchoredPosition = (Camera.main.WorldToScreenPoint(player.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2)) / referenceHolder.transitionAnim.GetComponentInParent<Canvas>().scaleFactor;
        referenceHolder.transitionAnim.Play("FadeIn");
    }

    void PlayFadeOut()
    {
        GameObject player = GameObject.FindObjectOfType<Player>().gameObject;

        referenceHolder.transitionAnim.GetComponent<RectTransform>().anchoredPosition = (Camera.main.WorldToScreenPoint(player.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2)) / referenceHolder.transitionAnim.GetComponentInParent<Canvas>().scaleFactor;
        referenceHolder.transitionAnim.Play("FadeOut");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if(scene.buildIndex > 0)
        {
            referenceHolder = GameObject.FindObjectOfType<ReferenceHolder>();
            PlayFadeIn();

            GameObject.FindObjectOfType<TestGameManager>().gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        currentLevel = 1;
        SceneManager.LoadScene(currentLevel);

        gameStarted = true;
    }

    void LoadSceneWithDelay()
    {
        SceneManager.LoadScene(currentLevel);
        levelInitiated = false;
    }
}
