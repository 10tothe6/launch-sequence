using System.Collections.Generic;
using Riptide;
using UnityEngine;
using UnityEngine.Events;

public enum e_possibleentitystates
{
    Independent, // a normal, updated entity
    Localized, // left to local physics
    Sleeping,
    Influenced,
    Controlled,
}

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

        allEntities = new List<e_genericentity>();
    }

    public GameObject p_debugText;

    // the master lists for all entities
    // THIS IS A CLIENT OR SERVER BASED THING, THEY SHARE
    // why? because I don't want two entity manager classes
    public List<e_genericentity> allEntities;
    // sandbox and main-world entities are shared between these lists
    // sandbox entities just have a negative index
    // world entities have positive indices

    public GameObject[] p_entities;

    public UnityEvent onSpawnEntity;
    public UnityEvent onDestroyEntity;

    public Transform t_sandboxEntityContainer;
    public Transform t_entityContainer;

    #region PINGS

    // signals that can be emitted by antenna parts, and picked up by the player's scanner
    
    // noting here that I could have an entirely different part do the ping-emitting, 
    // but why not just build it into the crft_antenna script? I don't want 5 million tiny scripts anyways, and it makes sense logically





    // NOTE - all of these functions could just return lists to avoid the array conversion cuz it don't really matter
    // TODO: this ^^ ?

    


    // wrapper for the below function
    public static crft_antenna[] GetSignalEmittersForFrequency(float frequency)
    {
        crft_antenna[] all_emitters = GetAllSignalEmitters();

        List<crft_antenna> emitters_with_correct_frequency = new List<crft_antenna>();

        for (int i = 0; i < all_emitters.Length; i++)
        {
            if (all_emitters[i].ping_frequency == frequency)
            {
                emitters_with_correct_frequency.Add(all_emitters[i]);
            }
        }

        return emitters_with_correct_frequency.ToArray();
    }
    // this function, unlike the others that build off of it,
    // actually needs to check to make sure the antennas ARE emitting a ping in the first place
    // (they may not be)
    // they may also not be powered
    public static crft_antenna[] GetAllSignalEmitters()
    {
        List<crft_antenna> emitterList = new List<crft_antenna>();


        // our job is easy because of the cached arrays of different part types
        // no need to look through every part and check the components
        for (int i = 0; i < Instance.allEntities.Count; i++)
        {
            e_craft comp = Instance.allEntities[i].GetComponent<e_craft>();

            if (comp != null)
            {
                for (int j = 0; j < comp.antennas.Count; j++)
                {
                    if (comp.antennas[j].IsEmittingPing())
                    {
                        emitterList.Add(comp.antennas[j]);
                    }
                }
            }
        }

        return emitterList.ToArray();
    }

    public static crft_antenna[] GetSignalEmittersForFrequencyWithinRange(float frequency, num_precisevector3 checkPosition)
    {
        List<crft_antenna> valid_emitters = new List<crft_antenna>();

        // i guess technically we're doing double the work here?
        // could make some sort of struct to contain the data together, but that seems like too much effort
        crft_antenna[] emitters = GetSignalEmittersForFrequency(frequency);
        num_precisevector3[] emitter_positions = GetSignalEmitterPositionsForFrequency(frequency);

        for (int i = 0; i < emitters.Length; i++)
        {
            if (emitter_positions[i].Sub(checkPosition).Mag().AsDouble() < emitters[i].ping_range)
            {
                valid_emitters.Add(emitters[i]);
            }
        }

        return valid_emitters.ToArray();
    }


    public static num_precisevector3[] GetSignalEmitterPositionsForFrequency(float frequency)
    {
        crft_antenna[] emitters = GetSignalEmittersForFrequency(frequency);

        List<num_precisevector3> emitterPositions = new List<num_precisevector3>();

        for (int i = 0; i < emitters.Length; i++)
        {
            emitterPositions.Add(emitters[i].GetComponent<crft_genericpart>().eComp.GetComponent<e_genericentity>().data.GetPosition());
        }

        return emitterPositions.ToArray();
    }
    public static num_precisevector3[] GetAllSignalEmitterPositions()
    {
        crft_antenna[] emitters = GetAllSignalEmitters();

        List<num_precisevector3> emitterPositions = new List<num_precisevector3>();

        for (int i = 0; i < emitters.Length; i++)
        {
            emitterPositions.Add(emitters[i].GetComponent<crft_genericpart>().eComp.GetComponent<e_genericentity>().data.GetPosition());
        }

        return emitterPositions.ToArray();
    }

    // holy hell of a function name
    public static num_precisevector3[] GetSignalEmitterPositionsForFrequencyWithinRange(float frequency, num_precisevector3 checkPosition)
    {
        List<num_precisevector3> valid_positions = new List<num_precisevector3>();

        // i guess technically we're doing double the work here?
        // could make some sort of struct to contain the data together, but that seems like too much effort
        crft_antenna[] emitters = GetSignalEmittersForFrequency(frequency);
        num_precisevector3[] emitter_positions = GetSignalEmitterPositionsForFrequency(frequency);

        for (int i = 0; i < emitters.Length; i++)
        {
            if (emitter_positions[i].Sub(checkPosition).Mag().AsDouble() < emitters[i].ping_range)
            {
                valid_positions.Add(emitter_positions[i]);
            }
        }

        return valid_positions.ToArray();
    }

    #endregion

    public void ClearAllEntityData()
    {
        DestroyAllEntities();
        allEntities.Clear();
    }


    public void DestroyAllEntities()
    {
        for (int i = allEntities.Count - 1; i>=0; i--)
        {
            Destroy(allEntities[i].gameObject);
        }
    }



    // rides on top of the below function
    public static void SpawnNewSinglePartSpaceCraft(string partName, num_precisevector3 spawnPosition)
    {
        crft_genericpartdata data = new crft_genericpartdata();

        data.partName = partName;
        data.position = Vector3.zero;

        SpawnCraftFromData(new crft_craftdata("craft", new crft_genericpartdata[]{data}), spawnPosition);
    }
    public static void SpawnCraftFromData(crft_craftdata data, num_precisevector3 spawnPosition)
    {
        // this will handle everything on the multiplayer side as well
        GameObject g_newSpaceCraft = Instance.SpawnNewEntity("craft", spawnPosition);

        e_craft comp = g_newSpaceCraft.GetComponent<e_craft>();
        comp.Initialize(data);
    }




    // probably one of the most important functions in this script
    // it will update any entity logic on the entities, including potentially:
    // changing position (not for physics entities)
    // resource transfers
    // etc.
    public void UpdateAllEntities()
    {
        for (int i = 0; i < allEntities.Count; i++)
        {
            allEntities[i].UpdateEntity();
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



    public void PutClientInRobot(ushort clientId)
    {
        // first, we have to spawn a new freecam entity

        GameObject g_newRobot = SpawnNewEntity("robot", "robot_" + ServerNetworkManager.GetClient(clientId).username, num_precisevector3.Zero());
        // ^^ THIS WILL AUTOMATICALLY NOTIFY ALL CLIETNS OF THE NEW ENTITY

        // then, we have to set the client to control that freecam
        ServerNetworkManager.Instance.SetControllingEntity(clientId, g_newRobot.GetComponent<e_genericentity>());
    }

    // seems like the most logical place to put this function
    public e_entityupdatepackage PrepareEntityUpdatePackage()
    {
        e_entityupdatepackage result = new e_entityupdatepackage();

        List<string> independentUpdates = new List<string>();
        List<int> independedIndices = new List<int>();
        

        for (int i = 0; i < allEntities.Count; i++)
        {
            if (allEntities[i].data.HasUpdatedValues())
            {
                // so we gotta decide where to put this entity's data
                // for entity system V1 its all independent
                
                independentUpdates.Add(allEntities[i].data.GetUpdatedData());
                independedIndices.Add(allEntities[i].data.index);

                allEntities[i].data.ClearUpdatedData();
            }
        }
        
        result.independentIndices = independedIndices.ToArray();
        result.independentData = independentUpdates.ToArray();

        return result;
    }


    // this function ONLY runs on the server
    public void RemoveEntity(int entityIndex)
    {
        e_genericentity toRemove = GetEntityFromIndex(entityIndex);

        if (toRemove.data.entityPrefabIndex == 0) {return;} // no killing freecam entities!!!

        // removing from the main list
        allEntities.Remove(toRemove);


        // last but not least, we kill the game object
        Destroy(toRemove.gameObject);


        // updating all the other clients of the murder
        ServerSenders.Instance.SendKillEntity(entityIndex);
    }

    // TODO: function for deleting all entities for when you leave a server


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
        GameObject g_newEntity = SpawnNewEntity(p_entity, num_precisevector3.Zero());

        g_newEntity.GetComponent<e_genericentity>().data.UpdateData(data);

        return g_newEntity;
    }

    public GameObject SpawnNewEntity(int entityIndex, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = p_entities[entityIndex];
        return SpawnNewEntity(p_entity, spawnPosition);
    }

    // okay so
    // * the client tells the server it's spawning a new entity
    // * if the server agrees, it runs this function on its end
    // * all clients then run this on their end, except for the host which did it already
    public GameObject SpawnNewEntity(string entityName, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        return SpawnNewEntity(p_entity, spawnPosition);
    }

    public GameObject SpawnNewEntity(string entityName, string nameToApply, num_precisevector3 spawnPosition)
    {
        GameObject p_entity = GetEntityPrefabFromName(entityName);

        GameObject g_newEntity = SpawnNewEntity(p_entity, spawnPosition);
        g_newEntity.name = "e_" + nameToApply;
        g_newEntity.GetComponent<e_genericentity>().data.SetDataEntry("name", nameToApply);

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

        // better just to have the logic automatically here instead of making a whole separate function
        if (ServerNetworkManager.Instance.isServerActive)
        {
            // since we're on a server, we need to tell everyone BUT the local clients
            ServerSenders.Instance.SendNewEntity(g_newEntity);
        }

        return g_newEntity;
    }

    public GameObject SpawnNewEntity(GameObject p_entity, num_precisevector3 spawnPosition)
    {
        GameObject g_newEntity = Instantiate(p_entity, t_entityContainer);

        e_genericentity genericComp = g_newEntity.GetComponent<e_genericentity>();
        genericComp.data.entityPrefabIndex = (ushort)System.Array.IndexOf(p_entities, p_entity);
        allEntities.Add(genericComp);
        genericComp.data.index = allEntities.Count;
        genericComp.data.SetPosition(spawnPosition);
        genericComp.transform.position = Coord.GetUnityPosition(genericComp);

        // better just to have the logic automatically here instead of making a whole separate function
        if (ServerNetworkManager.Instance.isServerActive)
        {
            // since we're on a server, we need to tell everyone BUT the local clients
            ServerSenders.Instance.SendNewEntity(g_newEntity);
        }

        onSpawnEntity.Invoke();

        return g_newEntity;
    }

    public e_genericentity GetEntityFromName(string name)
    {
        for (int i = 0; i < allEntities.Count; i++)
        {
            if (allEntities[i].data.GetDataEntry("name") == name)
            {
                return allEntities[i];
            }
        }

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
