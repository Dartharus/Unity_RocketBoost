using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float loadDelay = 1f;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] AudioClip finishSFX;
    [SerializeField] ParticleSystem finishParticles;
    [SerializeField] ParticleSystem crashParticles;
    AudioSource audioSource;
    bool isControllable = true;
    bool isCollidable = true;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        RespondToDebugKeys();
    }
    void OnCollisionEnter(Collision other)
    {
        if(!isControllable || !isCollidable)
            return;
        switch(other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Hi");
                break;
            case "Fuel":
                Debug.Log("touched fuel");
                break;
            case "Finish":
                StartCollisionSequence("Finish");
                break;
            default:
                StartCollisionSequence("Crash");
                break;
        }
    }
    void StartCollisionSequence(string sequence)
    {
        isControllable = false;
        audioSource.Stop();
        GetComponent<Movement>().enabled = false;
        switch (sequence)
        {
            case "Crash":
                audioSource.PlayOneShot(crashSFX);
                crashParticles.Play();
                StartCoroutine(ReloadLevel());
                break;
            case "Finish":
                audioSource.PlayOneShot(finishSFX);
                finishParticles.Play();
                StartCoroutine(LoadNextLevel());
                break;
        }
    }
    IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
            nextSceneIndex = 0;
        SceneManager.LoadScene(nextSceneIndex);
    }
    void RespondToDebugKeys()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            StartCoroutine(LoadNextLevel());
        }
        else if(Keyboard.current.xKey.wasPressedThisFrame)
        {
            StartCoroutine(ReloadLevel());
        }
        else if(Keyboard.current.cKey.wasPressedThisFrame)
        {
            isCollidable = !isCollidable;
            Debug.Log("Collidable: " + isCollidable);
        }
    }
}
