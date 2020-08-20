using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGameManager : MonoBehaviour
{
    private ReferenceHolder referenceHolder;

    private int currentLevel;
    private bool levelInitiated = false;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.AddListener<PlayerHitSpikeEvent>(OnPlayerHitSpike);
        GameEventManager.Instance.AddListener<KeyHitSpikeEvent>(OnKeyHitSpike);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PlayerReachedEndEvent>(OnPlayerCompletedLevel);
        GameEventManager.Instance.RemoveListener<PlayerHitSpikeEvent>(OnPlayerHitSpike);
        GameEventManager.Instance.RemoveListener<KeyHitSpikeEvent>(OnKeyHitSpike);
    }

    private void Start()
    {
        referenceHolder = GameObject.FindObjectOfType<ReferenceHolder>();
        PlayFadeIn();
    }

    private void Update()
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
        }
    }

    private void OnPlayerCompletedLevel(PlayerReachedEndEvent e)
    {
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

        referenceHolder.transitionAnim.GetComponent<RectTransform>().anchoredPosition = (Camera.main.WorldToScreenPoint(player.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2)) / referenceHolder.transitionAnim.GetComponentInParent<Canvas>().scaleFactor;
        referenceHolder.transitionAnim.Play("FadeIn");
    }

    void PlayFadeOut()
    {
        GameObject player = GameObject.FindObjectOfType<Player>().gameObject;

        referenceHolder.transitionAnim.GetComponent<RectTransform>().anchoredPosition = (Camera.main.WorldToScreenPoint(player.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2)) / referenceHolder.transitionAnim.GetComponentInParent<Canvas>().scaleFactor;
        referenceHolder.transitionAnim.Play("FadeOut");
    }

    void LoadSceneWithDelay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        levelInitiated = false;
    }
}
