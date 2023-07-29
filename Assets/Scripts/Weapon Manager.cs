using UnityEngine;

public enum WeaponType : byte
{
    none,
    pistol,
    teleporter,
    laser
}

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject weaponModels;

    [SerializeField] private GameObject pistolModel;
    [SerializeField] private AudioSource pistolAudioSource;
    [SerializeField] private Animator pistolMuzzleFlash;

    [SerializeField] private GameObject teleporterModel;
    [SerializeField] private AudioSource teleporterAudioSource;
    [SerializeField] private Animator teleporterMuzzleFlash;

    [SerializeField] private GameObject laserModel;
    [SerializeField] private AudioSource laserAudioSource;
    [SerializeField] private Animator laserMuzzleFlash;

    private void OnValidate()
    {
        if (pistolModel != null)
        {
            pistolAudioSource = pistolModel.transform.parent.GetComponent<AudioSource>();
        }
    }

}
