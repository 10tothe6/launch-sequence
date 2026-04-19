using System.Collections.Generic;
using UnityEngine;

public class ui_chat : MonoBehaviour
{
    private static ui_chat _instance;

    public static ui_chat Instance
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
    }

    public ui_list messageList;

    public List<net_chatmessage> chatMessages;


    // for now, chat messages won't have their own timestamps - they'll be internal
    public void AddChatMessage(string msg)
    {
        net_chatmessage newMessage = new net_chatmessage();
        newMessage.msg = msg;
        newMessage.timestamp = Time.time;
        newMessage.col = Color.white;

        chatMessages.Add(newMessage);

        Refresh();
    }

    public void AddChatMessage(string msg, Color col)
    {
        net_chatmessage newMessage = new net_chatmessage();
        newMessage.msg = msg;
        newMessage.timestamp = Time.time;
        newMessage.col = col;

        chatMessages.Add(newMessage);

        Refresh();
    }

    public void Refresh()
    {
        int l = Mathf.Min(chatMessages.Count, 5);

        string[] messages = new string[l];
        for (int i = 0; i < l; i++)
        {
            messages[i] = chatMessages[chatMessages.Count - i - 1].msg;
        }

        messageList.SetItems(messages);

        for (int i = 0; i < l; i++)
        {
            messageList.t_listContainer.GetChild(i).GetComponent<ui_stringdisplay>().SetColor(chatMessages[chatMessages.Count - i - 1].col);
        }
    }
}
