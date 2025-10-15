using UnityEngine;

public class AudioChooser : MonoBehaviour
{
    public static AudioChooser instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource hit_audioSource;
    [SerializeField] private AudioSource walk_audioSource;
    [SerializeField] private AudioSource movement_audioSource;

    [Header("Combat")]
    [SerializeField] private AudioClip hit_sfx1;
    [SerializeField] private AudioClip hit_sfx2;
    [SerializeField] private AudioClip hit_sfx3;

    [SerializeField] private AudioClip swingSFX;
    [SerializeField] private AudioClip parrySFX;
    [SerializeField] private AudioClip parryArrowSFX;

    [Header("Ground")]
    [SerializeField] private AudioClip ground_grassSFX1;
    [SerializeField] private AudioClip ground_grassSFX2;
    [SerializeField] private AudioClip ground_grassSFX3;
    [SerializeField] private AudioClip ground_grassSFX4;
    [SerializeField] private AudioClip ground_grassSFX5;
    [SerializeField] private AudioClip ground_grassSFX6;

    [SerializeField] private AudioClip ground_gravelSFX1;
    [SerializeField] private AudioClip ground_gravelSFX2;
    [SerializeField] private AudioClip ground_gravelSFX3;
    [SerializeField] private AudioClip ground_gravelSFX4;
    [SerializeField] private AudioClip ground_gravelSFX5;

    [Header("Jump/Slide")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip slideSFX;
    [SerializeField] private AudioClip landingSFX;

    /*
     * Needed Sounds
     * Dash
     * Climb
     * Ladder
     * Burn
     * Arrow - Fire, Hit
     * Teleport
     */

    //[SerializeField] private AudioClip groundedSFX;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void PlayRandomHitSFX()
    {
        int random = Random.Range(0, 3);
        switch(random)
        {
            case 0:
                hit_audioSource.clip = hit_sfx1;
                break;
            case 1:
                hit_audioSource.clip = hit_sfx2;
                break;
            case 2:
                hit_audioSource.clip = hit_sfx3;
                break;
        }
        hit_audioSource.Play();
    }

    public void PlaySwingSFX()
    {
        audioSource.clip = swingSFX;
        audioSource.Play();
    }

    public void PlayParrySFX()
    {
        audioSource.clip = parrySFX; 
        audioSource.Play();
    }

    public void PlayArrowParrySFX()
    {
        audioSource.clip = parryArrowSFX;
        audioSource.Play();
    }

    public void PlayGroundSFX(string groundType)
    {
        switch(groundType)
        {
            case "Grass":
                int random = Random.Range(0, 6);
                switch (random)
                {
                    case 0:
                        walk_audioSource.clip = ground_grassSFX1;
                        break;
                    case 1:
                        walk_audioSource.clip = ground_grassSFX2;
                        break;
                    case 2:
                        walk_audioSource.clip = ground_grassSFX3;
                        break;
                    case 3:
                        walk_audioSource.clip = ground_grassSFX4;
                        break;
                    case 4:
                        walk_audioSource.clip = ground_grassSFX5;
                        break;
                    case 5:
                        walk_audioSource.clip = ground_grassSFX6;
                        break;
                }
                walk_audioSource.Play();
                break;
            case "Gravel":
                int random2 = Random.Range(0, 5);
                switch (random2)
                {
                    case 0:
                        walk_audioSource.clip = ground_gravelSFX1;
                        break;
                    case 1:
                        walk_audioSource.clip = ground_gravelSFX2;
                        break;
                    case 2:
                        walk_audioSource.clip = ground_gravelSFX3;
                        break;
                    case 3:
                        walk_audioSource.clip = ground_gravelSFX4;
                        break;
                    case 4:
                        walk_audioSource.clip = ground_gravelSFX5;
                        break;
                }
                walk_audioSource.Play();
                break;
        }
    }

    public void PlayJumpSFX()
    {
        movement_audioSource.clip = jumpSFX;
        movement_audioSource.Play();
    }

    public void PlaySlideSFX()
    {
        movement_audioSource.clip = slideSFX;
        movement_audioSource.Play();
    }

    public void PlayLandingSFX()
    {
        movement_audioSource.clip = landingSFX;
        movement_audioSource.Play();
    }
}
