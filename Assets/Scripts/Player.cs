
// CLIENT CODE
using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public enum Team : byte
{
    none,
    green,
    orange
}

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public bool IsAlive => health > 0f;
    public WeaponManager WeaponManager => WeaponManager;

    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject model;
    [SerializeField] private MeshRenderer headband;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private Transform camTransform;

    [Header("Team Colours")]
    [SerializeField] private Material none;
    [SerializeField] private Material green;
    [SerializeField] private Material orange;

    private string username;
    private float health;

    private void OnValidate()
    {
        if (weaponManager == null)
            weaponManager = GetComponent<WeaponManager>();
        if (animationManager == null)
            animationManager = GetComponent<PlayerAnimationManager>();
    }

    private void Start()
    {
        health = maxHealth;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    private void Move(Vector3 newPosition, Vector3 forward)
    {
        transform.position = newPosition;

        if (!IsLocal)
            camTransform.forward = forward;

        animationManager.AnimateBasedOnSpeed();
    }

    public void SetHealth(float amount)
    {
        health = Mathf.Clamp(amount, 0f, maxHealth);
        UIManager.Singleton.HealthUpdated(health, maxHealth, true);
    }

    public void Died(Vector3 position)
    {

    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.username = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]

    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]

    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector3(), message.GetVector3());
    }
}

