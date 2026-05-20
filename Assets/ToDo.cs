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
    Add back button to how to play scene
    make prefab to typed tutorial content
    
    fix connection icon, its stretched make it square
    add w/l to profile scene
    do something with the settings button
    --------------------------------------------------------------------------



    BACK LOG:

    1) Info button in combat scene doesnt work. Make it appear when its not your turn
    2) Camera sometimes gets stuck in the air *Couldnt replicate, may have been fixed when die became automatic*
    3) Move die roll anim to right side of screen and update anims 
    4) Way for users to report bugs
    5) Write a shader that makes sure player can see through obj's that overlap the camera
    6) Aspect ratio for foldable landscape is 3:4 not 4:3 
    7) Ui needs to standout from the background *polish, on hold for now*
    8) Keep track of disconnections that are likely linked to griefing, If > 3 || 4 in some span of time , time player out
    9) Implement overdrive AP system or some equivent to speed up the game, or make it more action packed
    10) Create a way to visualize how far away the opponent is percisley 
    11) Interactive tutorial walkthrough for all basic mechanics

    lock game to landscape views




    --------------------------------------------------------------------------
    
    SUGGESTIONS:
    
    Areas of interest that benifit the players to draw them into fighting more (cheek ripper char otw)
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


        Passive : 
        ---------------------------

        69 health

            
        Passive bonuses increase based on number of AP at start of turn. More damage for fire, Evasion for wind , Healing for Nat

        Fire: Flame trail
                where you move leaves fire for 2 turns that damages enemies

        Wind: Extra movements per turn like the speedster
                if you run through fire, it becomes "Fanned" and deals enhanced damage
                  (potentially extra evasion?)

        Nature: Healing over time
                    5+ health each turn + amount of AP you have
                    
        
        ---------------------------
        1st Ability :
        ---------------------------
        Fire: Fireball
                    just deals damage, maybe add DoT debufff

        Wind: Torando
                Push Enemies away and enhance all fire in range (make this cost a good amount like an ult)

        Nature: Hibernation (extra healing & AP Regen at the cost of movement)
                    Double healing and gain one AP at the end of turn
        
        ---------------------------    
        2nd Ability :
        ---------------------------

        Stance switch: cost 1

        ---------------------------    
        Ultimate Ability : 
        ---------------------------

        Stance switch: cost 1
        
    */
}