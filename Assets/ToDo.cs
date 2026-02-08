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
        Create a enviornment for the chars to fight in
        fix the names for players in the combat scene
        fix the game loop
      
    

    
    BACK LOG:
    
    1) only enable confirm button on move screen if the users move at least 1 sq

    2) Drag mover can be "sticky" and hard to click/manipulate , fix this!

    3) Die roll should be automatic at the start of turns

    4) You raised the floor plane up, and now its clipping the partilce effects. Fix this

    5) Game loop is broken, data from old sessions persist after win state. It is not a clean re-fresh

    6) Ui needs to standout from the background
        
    7) If both players dont select a character, kick them back to the home screen

    investigate parrelsync
    --------------------------------------------------------------------------

    Maybe dont need toggle because ui blockers can clean themselves up
    Make class that holds and displays ability descriptions


    Read from packet to make sure the data is as expected // Do this both server side and client side
    */
}