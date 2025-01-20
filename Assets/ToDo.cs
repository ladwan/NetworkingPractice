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

    ON DECK:

    1) Need to re-incorparate lookAt for players

    2) Pause timer and hide ui during grid lerp movement



    how many sq's do you move with your current move speed across a given span of time

    DO LATER:

    1) Drag movers position is begin reset after confirm movement. This can be a bit jarring. Lerp back, rather than snap

    1) only enable confirm button on move screen if the users move at least 1 sq

    2) Game loop is broken, data from old sessions persist after win state. It is not a clean re-fresh

    3) If both players dont select a charather, kick them back to the home screen

    4) Disconnet players form server if BOTH players do not select a character in time
    ---------------------------------------------------------------------------

    Maybe dont need toggle because ui blockers can clean themselves up
    Make class that holds and displays ability descriptions


    Read from packet to make sure the data is as expected // Do this both server side and client side
    */
}