using System.Collections.Generic;
using UnityEngine;

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

    // the master lists for all entities
    // THIS IS A CLIENT OR SERVER BASED THING, THEY SHARE
    // why? because I don't want two entity manager classes
    public List<e_floatingentity> floatingEntities;
    public List<e_fixedentity> fixedEntities;
    public List<e_mimicentity> mimicEntities;


    // okay so
    // * the client tells the server it's spawning a new entity
    // * if the server agrees, it runs this function on its end
    // * all clients then run this on their end, except for the host which did it already
    public void SpawnNewEntity()
    {
        
    }
}
