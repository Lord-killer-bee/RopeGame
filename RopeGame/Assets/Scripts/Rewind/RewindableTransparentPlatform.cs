using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableTransparentPlatform : RewindableEntity
{
    [SerializeField] private bool willStartFullyOpaque;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float speedMultiplier;

    private SpriteRenderer sprite;
    private Collider2D collider;
    private float currentOpacity;
    private bool isPlay;
    private bool isForward;
    private bool isSpedUp;

    #region Base methods

    public override void OnLevelInitiated()
    {
        base.OnLevelInitiated();

        PlayEntity();

        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        currentOpacity = sprite.color.a;
    }

    public override void PlayEntity()
    {
        base.PlayEntity();

        isPlay = true;
        isForward = true;
        isSpedUp = false;
    }

    public override void PauseEntity()
    {
        base.PauseEntity();

        isPlay = false;
    }

    public override void RewindEntity()
    {
        base.RewindEntity();

        isPlay = true;
        isForward = false;
    }

    public override void StopEntity()
    {
        base.StopEntity();

        if (willStartFullyOpaque)
        {
            currentOpacity = 1;
        }
        else
        {
            currentOpacity = 0;
        }

        SetOpacity(currentOpacity);

        isPlay = false;
        isSpedUp = false;
    }

    public override void FastForwardEntity()
    {
        base.FastForwardEntity();

        isPlay = true;
        isSpedUp = true;
    }

    #endregion

    private void Update()
    {
        if (!levelInitiated)
            return;

        if (isPlay)
        {
            if (isForward)
            {
                if (willStartFullyOpaque)
                {
                    currentOpacity -= disappearSpeed * (isSpedUp ? speedMultiplier : 1) * Time.deltaTime;
                }
                else
                {
                    currentOpacity += disappearSpeed * (isSpedUp ? speedMultiplier : 1) * Time.deltaTime;
                }
            }
            else
            {
                if (willStartFullyOpaque)
                {
                    currentOpacity += disappearSpeed * (isSpedUp ? speedMultiplier : 1) * Time.deltaTime;
                }
                else
                {
                    currentOpacity -= disappearSpeed * (isSpedUp ? speedMultiplier : 1) * Time.deltaTime;
                }
            }

            currentOpacity = Mathf.Clamp01(currentOpacity);

            SetOpacity(currentOpacity);

            collider.enabled = (currentOpacity != 0);
        }
    }

    void SetOpacity(float value)
    {
        Color temp = sprite.color;
        temp.a = value;

        sprite.color = temp;
    }
}
