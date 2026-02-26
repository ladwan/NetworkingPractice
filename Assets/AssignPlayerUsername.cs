using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssignPlayerUsername : MonoBehaviour
{
    [SerializeField] private TMP_Text localUsername = null;
    [SerializeField] private TMP_Text otherUsername = null;

    // Start is called before the first frame update
    void Start()
    {
        AssignUsername();
    }
     private void AssignUsername()
    {
        localUsername.text = ClientInfo.username;
        otherUsername.text = ClientInfo.otherUsername;
    }
}
