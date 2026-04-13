using System.Collections.Generic;
using Riptide;
using UnityEngine;
using UnityEngine.Events;

// here's a quick note on player entity handling specifically,
// derived from a thinking session during a night-walk that I did

// there are 2 schools of thought when it comes to player entities:
// 1. the 'transient' system
// the idea here is that clients don't have their own entities

//  ^^ THIS IS THE ONE I'M ACTUALLY GOING WITH FOR THIS PROJECT, MIND YOU

// 2. the 'soul' system
// each client has its own entity (nicknamed the 'soul') that is just slaved to whatever robot that its controlling

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
        
    }

    // the master lists for all entities
    // THIS IS A CLIENT OR SERVER BASED THING, THEY SHARE
    // why? because I don't want two entity manager classes
    public List<e_floatingentity> floatingEntities;
    public List<e_fixedentity> fixedEntities;
    public List<e_mimicentity> mimicEntities;
    public Dictionary<int,e_generic> entities; // TODO: this
    // sandbox and main-world entities are shared between these lists
    // sandbox entities just have a negative index
    // world entities have positive indices

    public GameObject[] p_entities;

    public UnityEvent onSpawnEntity;
    public UnityEvent onDestroyEntity;

    public Transform t_sandboxPlayerContainer;
    public Transform t_playerContainer;


    public net_packagedentitydata[] PackageAllEntityData()
    {
        List<net_packagedentitydata> result = new List<net_packagedentitydata>();

        for (int i = 0; i < floatingEntities.Count; i++)
        {
            result.Add(floatingEntities[i].data.Package());
        }
        for (int i = 0; i < fixedEntities.Count; i++)
        {
            result.Add(fixedEntities[i].data.Package());
        }
        for (int i = 0; i < mimicEntities.Count; i++)
        {
            result.Add(mimicEntities[i].data.Package());
        }

        return result.ToArray();
    }

    // used mainly when spawning entities delivered by the server
    public void SpawnNewEntity(int entityIndex, string data)
    {
        GameObject p_entity = p_entities[entityIndex];

        GameObject g_newEntity = SpawnNewEntity(p_entity, num_precisevector3.Zero());

        g_newEntity.GetComponent<e_generic>().SetData(data);
    }

    public void SpawnNewEntity(int entityIndex, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = p_entities[entityIndex];
        SpawnNewEntity(p_entity, spawnPosition);
    }

    // okay so
    // * the client tells the server it's spawning a new entity
    // * if the server agrees, it runs this function on its end
    // * all clients then run this on their end, except for the host which did it already
    public void SpawnNewEntity(string entityName, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        SpawnNewEntity(p_entity, spawnPosition);
    }

    public GameObject SpawnNewEntity(GameObject p_entity, num_precisevector3 spawnPosition)
    {
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
            cmd.Log("There was an issue with the entity prefab '" + p_entity.name + "'. It has no entity component!");
        }

        return g_newEntity;
    }

    public GameObject GetEntityPrefabFromName(string name)
    {
        for (int i = 0; i < p_entities.Length; i++)
        {
            if  (p_entities[i].name == "e_" + name)
            {
                return p_entities[i];
            }
        }

        // maybe I could return some sort of 'error' entity, like an untextured block from Minecraft?
        return null;
    }
}
