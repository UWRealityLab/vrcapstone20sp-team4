using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadingAnimation : MonoBehaviour
{
    private HashSet<string> currentObjects = new HashSet<string>();
    private GameObject onboardingInterface;
    private GameObject onboardingPreview;

    private void Awake()
    {
        onboardingPreview = GameObject.Find("OnboardingPreview");
        onboardingInterface = GameObject.Find("OnboardingInterface");
    }

    // Start is called before the first frame update

    private void Update()
    {
        /*
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(FadeOut(obj));
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(FadeIn(obj));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {

        }*/
    }

    /*
    private IEnumerator playParticles(Vector3 location)
    {
        ParticleSystem particles = Instantiate(particleSystemObj, location, Quaternion.identity);
        particles.Play();
        yield return new WaitForSeconds(1.0f);
        Destroy(particles);
    }
    */

    public void turnOnPreview()
    {
        StartCoroutine(FadeOutFadeIn(onboardingInterface, onboardingPreview));

    }

    public void turnOffPreview()
    {
        StartCoroutine(FadeOutFadeIn(onboardingPreview, onboardingInterface));
    }

    public IEnumerator FadeOutFadeIn(GameObject obj1, GameObject obj2)
    {
        yield return StartCoroutine(FadeOut(obj1));
        yield return StartCoroutine(FadeIn(obj2));
    }

    public IEnumerator FadeOut(GameObject obj)
    {
        Debug.Log(obj == null);
        Debug.Log(currentObjects == null);
        if (obj.activeSelf && !currentObjects.Contains(obj.name))
        {
            currentObjects.Add(obj.name);
            float fadeTimer = 1f;
            Vector3 originalScale = obj.transform.localScale;
            while (fadeTimer > 0f)
            {
                fadeTimer -= (Time.deltaTime * 1.6f);
                float scale = fadeTimer;
                if (scale < 0f) scale = 0f;
                Vector3 newScale = scale * originalScale;
                obj.transform.localScale = newScale;
                yield return new WaitForEndOfFrame();
            }
            currentObjects.Remove(obj.name);
            obj.SetActive(false);
            obj.transform.localScale = originalScale;
        } else
        {
            //yield break;
        }
    }

    public IEnumerator FadeIn(GameObject obj)
    {
        if (!obj.activeSelf && !currentObjects.Contains(obj.name))
        {
            currentObjects.Add(obj.name);
            if (obj.name == "Preview")
            {
                Debug.Log("Preview in FadeIn");
            }
            Vector3 originalScale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(0, 0, 0);
            obj.SetActive(true);
            float fadeTimer = 1.0f;
            while (fadeTimer > 0f)
            {
                fadeTimer -= (Time.deltaTime * 1.6f);
                float scale = 1 - fadeTimer;
                if (scale < 0f) scale = 0f;
                Vector3 newScale = scale * originalScale;
                obj.transform.localScale = newScale;
                yield return new WaitForEndOfFrame();
            }
            currentObjects.Remove(obj.name);
            obj.transform.localScale = originalScale;
        } else
        {
            //yield break;
        }
    }
}
