using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    public GameObject goLogo;
    public GameObject goName;

    public AudioSource compAudio;
    public AudioClip sndSplash;

    public void Start()
    {
        //Cursor.visible = false;
        StartCoroutine(CRTExecuteLogoSequence());

        /* AnalyticsWrapper.Initialize();
		AnalyticsWrapper.RegisterSessionStart(); */
    }

    private IEnumerator CRTExecuteLogoSequence()
    {
        goLogo.GetComponent<SpriteRenderer>().enabled = false;
        goName.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.7f);

        float soundsVolume = GetSoundsVolume();

        goLogo.GetComponent<SpriteRenderer>().enabled = true;
        compAudio.PlayOneShot(sndSplash, soundsVolume);

        yield return new WaitForSeconds(0.3f);

        goName.GetComponent<SpriteRenderer>().enabled = true;
        compAudio.PlayOneShot(sndSplash, soundsVolume);

        yield return new WaitForSeconds(1.5f);

        goLogo.GetComponent<SpriteRenderer>().enabled = false;
        goName.GetComponent<SpriteRenderer>().enabled = false;
        compAudio.PlayOneShot(sndSplash, soundsVolume);
    }

    private float GetSoundsVolume()
    {
        /* float sfxVol = Options.GetSFXVolume() * 0.1f;
        float masterVol = Options.GetMasterVolume() * 0.1f;

        return sfxVol * masterVol; */

        return 1;
    }
}
