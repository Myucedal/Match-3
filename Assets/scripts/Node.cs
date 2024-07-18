using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    // to determine wheter the space can be filled with potions or not
    public bool isUsable;
    public GameObject potion;

    public Node(bool _isUsable, GameObject _potion)
    {
        isUsable = _isUsable;
        potion = _potion;
    }
}
