using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTest : GrandParent
{
    [Serializable]
    public struct GroupingOfTypes
    {
        public int testInt;
        public GameObject testGameObject;
        public string testString;
    }

    public void TestStuff(GroupingOfTypes myGroup)
    {
        Debug.Log($"My int : {myGroup.testInt} , My GameObject : {myGroup.testGameObject.name} , My string : {myGroup.testString}");
    }
}
