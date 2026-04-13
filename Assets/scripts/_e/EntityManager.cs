using System.Collections.Generic;
using Riptide;
using UnityEngine;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{
    private static EntityManager _instance;

    public static EntityManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        floatingEntities = new List<e_floatingentity>();
        fixedEntities = new List<e_fixedentity>();
        mimicEntities = new List<e_mimicentity>();
    }

    void Start()
    {
        ServerNetworkManager.Instance.onJoinServer.AddListener(InitializeLocalPlayer);
    }

    // the master lists for all entities
    // THIS IS A CLIENT OR SERVER BASED THING, THEY SHARE
    // why? because I don't want two entity manager classes
    public List<e_floatingentity> floatingEntities;
    public List<e_fixedentity> fixedEntities;
    public List<e_mimicentity> mimicEntities;

    public GameObject[] p_entities;

    public UnityEvent onSpawnEntity;
    public UnityEvent onDestroyEntity;

    public Transform t_sandboxPlayerContainer;
    public Transform t_playerContainer;


    // spawns both the sandbox AND world copies
    public void InitializeLocalPlayer()
    {
        Debug.Log("spawning local player");
    }


    // okay so
    // * the client tells the server it's spawning a new entity
    // * if the server agrees, it runs this function on its end
    // * all clients then run this on their end, except for the host which did it already
    public void SpawnNewEntity(string entityName, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        GameObject g_newEntity = Instantiate(p_entity, null);

        // depending on what type of entity we're dealing with
        if (g_newEntity.GetComponent<e_floatingentity>() != null)
        {
            e_floatingentity comp = g_newEntity.GetComponent<e_floatingentity>();
            
            comp.data.localPosition = spawnPosition;
            floatingEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_fixedentity>() != null)
        {
            e_fixedentity comp = g_newEntity.GetComponent<e_fixedentity>();

            comp.data.localPosition = spawnPosition;
            fixedEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_mimicentity>() != null)
        {
            // TODO: mimics
        } else
        {
            cmd.Log("There was an issue with the entity prefab '" + entityName + "'. It has no entity component!");
        }
    }

    public GameObject GetEntityPrefabFromName(string name)
    {
        for (int i = 0; i < p_entities.Length; i++)
        {
            if  (p_entities[i].name == name)
            {
                return p_entities[i];
            }
        }

        // maybe I could return some sort of 'error' entity, like an untextured block from Minecraft?
        return null;
    }
}
