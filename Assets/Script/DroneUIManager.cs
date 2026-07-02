using UnityEngine;
using TMPro;

public class DroneUIManager : MonoBehaviour
{
    [Header("Drone Reference")]
    public DroneController drone;

    [Header("Cameras")]
    public GameObject thirdPersonCameraObj;
    public GameObject firstPersonCameraObj;

    [Header("HUD Text (TMP)")]
    public TMP_Text speedText;
    public TMP_Text altitudeText;
    public TMP_Text statusText;
    public TMP_Text cameraModeText;
    public TMP_Text compassText;

    private bool isFirstPerson = false;

    void Start()
    {

        SetThirdPerson();
    }

    void Update()
    {
        if (drone == null) return;

        if (speedText != null)
            speedText.text = $"SPEED: {drone.CurrentSpeed:F1} m/s";

        if (altitudeText != null)
            altitudeText.text = $"ALTITUDE: {drone.Altitude:F1} m";

        if (statusText != null)
        {
            statusText.text = drone.IsArmed ? "STATUS: ARMED" : "STATUS: DISARMED";
            statusText.color = drone.IsArmed ? Color.green : Color.red;
        }

        if (cameraModeText != null)   
            cameraModeText.text = isFirstPerson ? "CAMERA: FPV" : "CAMERA: THIRD PERSON";

        if (compassText != null)
            compassText.text = GetCompassLabel(drone.transform.eulerAngles.y);
    }



    public void OnArmButton()
    {
        if (drone != null) drone.Arm();
    }

    public void OnDisarmButton()
    {
        if (drone != null) drone.Disarm();
    }

    public void OnFPVButton()
    {
        SetFirstPerson();
    }

    public void OnThirdPersonButton()
    {
        SetThirdPerson();
    }



    private void SetFirstPerson()
    {
        isFirstPerson = true;
        if (firstPersonCameraObj != null) firstPersonCameraObj.SetActive(true);
        if (thirdPersonCameraObj != null) thirdPersonCameraObj.SetActive(false);
    }

    private void SetThirdPerson()
    {
        isFirstPerson = false;
        if (thirdPersonCameraObj != null) thirdPersonCameraObj.SetActive(true);
        if (firstPersonCameraObj != null) firstPersonCameraObj.SetActive(false);
    }


    private string GetCompassLabel(float yawDegrees)
    {
        string[] dirs = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        int index = Mathf.RoundToInt(yawDegrees / 45f);
        return $"{dirs[index]} ({yawDegrees:F0}°)";
    }
}