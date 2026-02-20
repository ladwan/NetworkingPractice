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

    1) Die roll should be automatic at the start of turns






    BACK LOG:

    1) You raised the floor plane up, and now its clipping the partilce effects. Fix this

    2) If both players dont select a character, kick them back to the home screen

    3) Game loop is broken, data from old sessions persist after win state. It is not a clean re-fresh

    4) Fix the names for players in the combat scene

    5) Ui needs to standout from the background

    6) Camera sometimes gets stuck in the air



    animated ability buttons
    track which characther people pick in a db
    break idea of mouse/ touch interactions into single respo classes
    --------------------------------------------------------------------------

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