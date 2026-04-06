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

    ON DECK

    gameplay is broken, fix the nulls
    speedster quick punch is not disabling ult
    turn off debug Die roll
    speedsters distortion cube on mobile

    --------------------------------------------------------------------------



    BACK LOG:

    1) Ui needs to standout from the background *polish, on hold for now*
    2) Camera sometimes gets stuck in the air *Couldnt replicate, may have been fixed when die became automatic*
    3) Move die roll anim to right side of screen and update anims 
    4) Speedster ult seems to persist after attack, fix this


    lock game to landscape views




    --------------------------------------------------------------------------
    
    SUGGESTIONS:
    
    Do we want the confirm buttons? or would the gameplay be smoother without them?
    Make turn times user configureable. Maybe they can play a match where one user has a handicap
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