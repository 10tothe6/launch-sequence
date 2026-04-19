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

        allEntities = new List<e_genericentity>();
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
    public List<e_genericentity> allEntities;
    // sandbox and main-world entities are shared between these lists
    // sandbox entities just have a negative index
    // world entities have positive indices

    public GameObject[] p_entities;

    public UnityEvent onSpawnEntity;
    public UnityEvent onDestroyEntity;

    public Transform t_sandboxEntityContainer;
    public Transform t_entityContainer;

    public void UpdateAllEntityPositions()
    {
        for (int i = 0; i < allEntities.Count; i++)
        {
            allEntities[i].Refresh();
        }
    }

    public void PutClientInFreecam(ushort clientId)
    {
        // first, we have to spawn a new freecam entity

        GameObject g_newFreecam = SpawnNewEntity("freecam", "freecam_" + ServerNetworkManager.GetClient(clientId).username, num_precisevector3.Zero());
        // ^^ THIS WILL AUTOMATICALLY NOTIFY ALL CLIETNS OF THE NEW ENTITY

        // then, we have to set the client to control that freecam
        ServerNetworkManager.Instance.SetControllingEntity(clientId, g_newFreecam.GetComponent<e_genericentity>());
    }


    public net_packagedentitydata[] PackageAllEntityData()
    {
        List<net_packagedentitydata> result = new List<net_packagedentitydata>();

        for (int i = 0; i < allEntities.Count; i++)
        {
            result.Add(allEntities[i].data.GetPackagedData());
        }

        return result.ToArray();
    }

    // used mainly when spawning entities delivered by the server
    public GameObject SpawnNewEntity(int entityIndex, string data)
    {
        GameObject p_entity = p_entities[entityIndex];

        // name need not be set cuz it'll be overwritten by data
        GameObject g_newEntity = SpawnNewEntity(p_entity, "e", num_precisevector3.Zero());

        g_newEntity.GetComponent<e_genericentity>().data.SetPackagedData(data);

        return g_newEntity;
    }

    public GameObject SpawnNewEntity(int entityIndex, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = p_entities[entityIndex];
        return SpawnNewEntity(p_entity, "e", spawnPosition);
    }

    // okay so
    // * the client tells the server it's spawning a new entity
    // * if the server agrees, it runs this function on its end
    // * all clients then run this on their end, except for the host which did it already
    public GameObject SpawnNewEntity(string entityName, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        return SpawnNewEntity(p_entity, "spawned", spawnPosition);
    }

    public GameObject SpawnNewEntity(string entityName, string nameToApply, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        GameObject g_newEntity = SpawnNewEntity(p_entity, nameToApply, spawnPosition);
        g_newEntity.name = "e_" + nameToApply;

        return g_newEntity;
    }

    public GameObject SpawnNewEntityInSandbox(string entityName, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        return SpawnNewEntityInSandbox(p_entity, spawnPosition);
    }

    // hate how this is just copy-pasted
    public GameObject SpawnNewEntityInSandbox(GameObject p_entity, num_precisevector3 spawnPosition)
    {
        GameObject g_newEntity = Instantiate(p_entity, t_sandboxEntityContainer);

        e_genericentity genericComp = g_newEntity.GetComponent<e_genericentity>();
        allEntities.Add(genericComp);
        genericComp.data.index = allEntities.Count * -1; // negative index because sandbox
        genericComp.data.SetPosition(spawnPosition);

        // depending on what type of entity we're dealing with
        if (g_newEntity.GetComponent<e_floatingentity>() != null)
        {
            e_floatingentity comp = g_newEntity.GetComponent<e_floatingentity>();
            floatingEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_fixedentity>() != null)
        {
            e_fixedentity comp = g_newEntity.GetComponent<e_fixedentity>();
            fixedEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_mimicentity>() != null)
        {
            // TODO: mimics
        } else
        {
            cmd.Log("There was an issue with the entity prefab '" + p_entity.name + "'. It has no entity component!");
        }
        // better just to have the logic automatically here instead of making a whole separate function
        if (ServerNetworkManager.Instance.isServerActive)
        {
            // since we're on a server, we need to tell everyone BUT the local clients
            ServerNetworkManager.Instance.SendNewEntity(g_newEntity);
        }

        return g_newEntity;
    }

    public GameObject SpawnNewEntity(GameObject p_entity, string name, num_precisevector3 spawnPosition)
    {
        GameObject g_newEntity = Instantiate(p_entity, t_entityContainer);

        e_genericentity genericComp = g_newEntity.GetComponent<e_genericentity>();
        genericComp.data.entityName = name;
        genericComp.data.entityPrefabIndex = (ushort)System.Array.IndexOf(p_entities, p_entity);
        allEntities.Add(genericComp);
        genericComp.data.index = allEntities.Count;
        genericComp.data.SetPosition(spawnPosition);

        // depending on what type of entity we're dealing with
        if (g_newEntity.GetComponent<e_floatingentity>() != null)
        {
            e_floatingentity comp = g_newEntity.GetComponent<e_floatingentity>();
            floatingEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_fixedentity>() != null)
        {
            e_fixedentity comp = g_newEntity.GetComponent<e_fixedentity>();
            fixedEntities.Add(comp);
        } else if (g_newEntity.GetComponent<e_mimicentity>() != null)
        {
            // TODO: mimics
        } else
        {
            cmd.Log("There was an issue with the entity prefab '" + p_entity.name + "'. It has no entity component!");
        }
        // better just to have the logic automatically here instead of making a whole separate function
        if (ServerNetworkManager.Instance.isServerActive)
        {
            // since we're on a server, we need to tell everyone BUT the local clients
            ServerNetworkManager.Instance.SendNewEntity(g_newEntity);
        }

        return g_newEntity;
    }

    public e_genericentity GetEntityFromName(string name)
    {
        for (int i = 0; i < allEntities.Count; i++)
        {
            if  (allEntities[i].data.entityName == name)
            {
                return allEntities[i];
            }
        }

        // maybe I could return some sort of 'error' entity, like an untextured block from Minecraft?
        return null;
    }

    public e_genericentity GetEntityFromIndex(int index)
    {
        for (int i = 0; i < allEntities.Count; i++)
        {
            if  (allEntities[i].data.index == index)
            {
                return allEntities[i];
            }
        }

        // maybe I could return some sort of 'error' entity, like an untextured block from Minecraft?
        return null;
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
