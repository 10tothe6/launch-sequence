using UnityEngine;

public class valid_console : MonoBehaviour
{
    void Awake()
    {
        GetComponent<valid_dispatcher>().onFullTest.AddListener(RunValidation);
    }


    public void RunValidation()
    {
        bool testFailed = false;



        string testCommand = "@a";
        string[] result = cmd_console.Instance.ProcessMessage(testCommand, 1);

        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] != ServerNetworkManager.Instance.GetUsernameFromIndex(
                ServerNetworkManager.Instance.connectedClients[i].client_index
            ))
            {
                testFailed = true;
                break;
            }
        }

        testCommand = "@s";
        result = cmd_console.Instance.ProcessMessage(testCommand, 1);

        if (result[0] != ServerNetworkManager.GetClient(1).username)
        {
            testFailed = true;
        }

        // can't really test random, other than by checking to see if the result is A username
        testCommand = "@r";
        result = cmd_console.Instance.ProcessMessage(testCommand, 1);

        bool foundUsername = false;
        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (result[0] == ServerNetworkManager.Instance.connectedClients[i].username)
            {
                foundUsername = true;
                break;
            }
        }

        if (!foundUsername)
        {
            testFailed = false;
        }


        // TODO: do something if the test succeeds or fails
    }
}
