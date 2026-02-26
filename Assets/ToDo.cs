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

    1) If both players dont select a character, kick them back to the home screen
    Do this is one player disconnects too!

    Ride exsiting rails of disconnection logic to tell unity to go to lobby, then disconnect remaining client


    --------------------------------------------------------------------------



    BACK LOG:

    1) Ui needs to standout from the background *polish, on hold for now*
    2) Camera sometimes gets stuck in the air *Couldnt replicate, may have been fixed when die became automatic*



    --------------------------------------------------------------------------
    
    SUGGESTIONS:
    
    animated ability buttons
    track which characther people pick in a db
    break idea of mouse/ touch interactions into single respo classes

    Maybe dont need toggle because ui blockers can clean themselves up
    Make class that holds and displays ability descriptions


    Read from packet to make sure the data is as expected // Do this both server side and client side


    likley interface based
    needs to support touch interaction
    hover interaction
    click interaction
    drag/hold interactions
    raycast through overlapped collisions

    */
}