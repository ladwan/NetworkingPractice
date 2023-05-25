using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDo : MonoBehaviour
{
    /*
        TODO: Start implementing the new click and drag movement system
            
            a) look over drag movement code to reduce bugs
                    make sure when turn ends from timing out drag mover is reset and connections auras are turned off ***
                        also make sure this works when you hit the back button ***
                    make it so you can only click the drag mover once you click move and the camera anmation is done ***
                           also make sure to turn the drag move off when ending a turn (test the edge case of what happens when you are still dragging when the turn ends) ***
                           also make sure to re-disable it when you hit the back or confirm buttons ***
                    make sure drag mover is fully functional on player 2 
                    removed unused ui buttons and old code for movement system


    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~      Things that need refactored      ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    continue working on spawning in both players using same ui
    fix weird ui bugs when ending turn
    add visual indicator of how many AP you have to background panel
    drag mover for player 2 is using player 1 values, meaning it wont overlap the player 2 sq because it thinks its occupied by an opponent , fix this 
    turn not ending on time run out (try to recreate this on the movement screen)
    debug player 2 drag mover its not the tryhightlight

    make smooth camera follow script to go along with the lookAt code ***

    Drag mover is bugged, not showing connections properly  ***

    Test to fix any bugs

    reduce die anim time ***

    do git stuff 
    network both players rotation

                    
    */
}
