using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepSounds; // Array to hold your 4 footstep sounds
    public AudioSource audioSource;    // Reference to the AudioSource component

    private Vector3 previousPosition;  // To store the previous position of the player
    private float stepInterval = 0.5f; // Time between footsteps
    private float timer = 0f;          // Timer to control the interval between steps

    void Start()
    {
        // Initialize the previous position
        previousPosition = transform.position;
    }

    void Update()
    {
        // Get the current position of the player
        Vector3 currentPosition = transform.position;

        // Calculate the distance moved since the last frame
        float distanceMoved = Vector3.Distance(previousPosition, currentPosition);

        // Increment the timer
        timer += Time.deltaTime;

        // If the player has moved a certain distance and enough time has passed, play a footstep sound
        if (distanceMoved > 0.1f && timer >= stepInterval)
        {
            PlayRandomFootstepSound();
            timer = 0f; // Reset the timer
        }

        // Update the previous position
        previousPosition = currentPosition;
    }

    void PlayRandomFootstepSound()
    {
        // Select a random footstep sound from the array
        int randomIndex = Random.Range(0, footstepSounds.Length);
        AudioClip randomFootstep = footstepSounds[randomIndex];

        // Play the selected footstep sound
        audioSource.PlayOneShot(randomFootstep);
    }
}