using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string type;
    public int origin; //-1 = el server // 0 = player A  // 1 = player B
    public int data;
}
