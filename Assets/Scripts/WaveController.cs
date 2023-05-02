using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class WaveController : MonoBehaviour
{
    [SerializeField] private List<Sprite> numbersList;
    [SerializeField] private GameObject goTimer;
    [SerializeField] private GameObject goTimerContainer;

    private const int DELAY_BETWEEN_WAVES = 3;
    private const float DELAY_BETWEEN_WARNINGS = 0.5f;

    public void StartFirstWave()
    {
        StartCoroutine(CRTWaitForNewWaveStart(true));
    }

    private IEnumerator CRTWaitForNewWaveStart(bool withInitialDelay)
    {
        goTimerContainer.SetActive(false);

        if (withInitialDelay)
        {
            yield return new WaitForSeconds(1);
        }

        goTimerContainer.SetActive(true);

        for (int i = DELAY_BETWEEN_WAVES; i >= 0; i--)
        {
            goTimer.GetComponent<SpriteRenderer>().sprite = numbersList[i];
            goTimer.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 10, 0.2f);
            yield return new WaitForSeconds(1);
        }

        goTimerContainer.SetActive(false);
    }
}
