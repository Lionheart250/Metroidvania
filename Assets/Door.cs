using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorClosedPosition;
    public Transform doorOpenPosition;
    public float doorOpenSpeed = 1.0f;
    private float openStartTime;
    public string doorID;

    private bool doorOpen; // Use a private variable to track door state

    private void Start()
    {
        SaveData.Instance.LoadEnvironmentData();
        //SaveData.Instance.GetDoorState(doorID);
        // Check if the door should be open based on saved state
        if (SaveData.Instance.GetDoorState(doorID))
        {
            doorOpen = true;
            transform.position = doorOpenPosition.position;
        }
        else
        {
            doorOpen = false;
            transform.position = doorClosedPosition.position;
        }
    }

    public void OpenDoor()
    {
        if (!doorOpen)
        {
            // Calculate the time fraction for the lerp
            float t = Mathf.Clamp01((Time.time - openStartTime) / doorOpenSpeed);

            // Perform the lerp to open position
            transform.position = Vector3.Lerp(doorClosedPosition.position, doorOpenPosition.position, t);

            // Check if the door has reached the open position
            if (t >= 1.0f)
            {
                SaveData.Instance.SetDoorState(doorID, true); // Set door state to open in save data
                SaveData.Instance.SaveEnvironmentData();
                doorOpen = true; // Set door state to open
                openStartTime = Time.time; // Reset the open start time for future reference
            }
        }
    }

    public void CloseDoor()
    {
        if (doorOpen)
        {
            // Calculate the time fraction for the lerp
            float t = Mathf.Clamp01((Time.time - openStartTime) / doorOpenSpeed);

            // Perform the lerp to close position
            transform.position = Vector3.Lerp(doorOpenPosition.position, doorClosedPosition.position, t);

            // Check if the door has reached the closed position
            if (t >= 1.0f)
            {
                SaveData.Instance.SetDoorState(doorID, false); // Set door state to closed in save data
                doorOpen = false; // Set door state to closed
                openStartTime = Time.time; // Reset the open start time for future reference
            }
        }
    }
}
