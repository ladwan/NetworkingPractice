using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTest : BaseTest
{
    [SerializeField]
    private GroupingOfTypes myGroupingOfTypes = new();

    public GroupingOfTypes MyGroupingOfTypes { get => myGroupingOfTypes; set => myGroupingOfTypes = value; }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunLogic();
        }
    }


    public void RunLogic()
    {
        base.TestStuff(myGroupingOfTypes);
    }
}
