using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDo : MonoBehaviour
{
    /*
     * 2 Ap = 5 points of damage
     * 
        If status effect 1 ends before status effect 2, slide that status effect back to the first open slot
        Occasionally the charather selection screen will throw a null ref
        Green mat for availible targeting looked a bit odd, maybe update this to look better 

        Keep learning shader stuff!

     Named variables : Study this

    ---------------------------------------------------------------------------

    DO TODAY:
    make it so the move to random grid points only happens when you move 4 or more grid points, while ulted !
    seems like updateAP is called and is re-showing the passive AP lights when it shouldnt , look into this!
    change the way combat Ui states are set, remove them from happening on object enable to something more tracable


    only enable confirm button on move screen if the users move at least 1 sq
    only do long speedster anim if you move at least 3 sqs

    ---------------------------------------------------------------------------

    Maybe dont need toggle because ui blockers can clean themselves up
    Make class that holds and displays ability descriptions


    Read from packet to make sure the data is as expected // Do this both server side and client side
    */
}