using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForverFight.Ui;
using ForverFight.Networking;
using ForverFight.FlowControl;
using ForverFight.Interactable;

public class FasterPassive : MonoBehaviour, IPassiveAbility
{
    [SerializeField]
    private ApReferenceLists referenceLists = null;
    [SerializeField]
    private string passiveAbilityName = null;
    [SerializeField]
    private string passiveAbilityDescription = null;
    [SerializeField]
    private List<Image> passiveApBackgrounds = new List<Image>();
    [SerializeField]
    private Color passiveHightlightColor = Color.black;


    private int passiveAp = 3;
    private int maxPassiveAp = 3;
    private Coroutine coroutineREF = null;

    #region Public Properties
    public int PassiveAp
    {
        get => passiveAp;
        set
        {
            if (value >= 0)
            {
                passiveAp = value;
            }
            else
            {
                passiveAp = 0;
                Debug.Log("Invalid passive AP amount");
            }
        }
    }

    public string PassiveAbilityName
    {
        get => passiveAbilityName;
        set
        {
            if (value != "" && value != null)
            {
                passiveAbilityName = value;
            }
            else
            {
                Debug.Log("Invalid passive ability name");
            }
        }
    }
    public string PassiveAbilityDescription
    {
        get => passiveAbilityDescription;
        set
        {
            if (value != "" && value != null)
            {
                passiveAbilityDescription = value;
            }
            else
            {
                Debug.Log("Invalid passive ability description");
            }
        }
    }

    public List<Image> PassiveApBackgrounds => passiveApBackgrounds;

    public Color PassiveHightlightColor { get => passiveHightlightColor; set => passiveHightlightColor = value; }
    #endregion


    protected void OnEnable()
    {
        BeginCoroutine();
        PassiveAbilityName = "Faster";
        PassiveAbilityDescription = "You can move 3 extra squares each turn";
        ActionPointsManager.Instance.SpeedsterPassiveApLists = referenceLists;
        CombatUiStatesManager.Instance.onCombatUiStateChange += ApplyPassive;
        PlayerTurnManager.Instance.OnTurnEnd += ResetPassiveAp;
        FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated += ChangeColorOfHighlight;
    }

    protected void OnDisable()
    {
        CombatUiStatesManager.Instance.onCombatUiStateChange -= ApplyPassive;
        PlayerTurnManager.Instance.OnTurnEnd -= ResetPassiveAp;
        FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated -= ChangeColorOfHighlight;
    }


    public void ApplyPassive()
    {
        if (CombatUiStatesManager.Instance.CurrentCombatUiState == CombatUiStatesManager.combatUiState.movement)
        {
            if (passiveAp > 0)
            {
                ActionPointsManager.Instance.UpdateAP(referenceLists, 0);
                UpdateApBackgrounds();
                ChangeColorOfHighlight();
            }
            else
            {
                ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, 0);
            }
        }
        else
        {
            ToggleApBackgrounds(false);
            referenceLists.EmptyAllAP();
            //make visuals go away
            //this will probably keep running when it shouldnt , fix this !
        }

    }

    public void ToggleApBackgrounds(bool toggle)
    {
        for (int i = 0; i < PassiveApBackgrounds.Count; i++)
        {
            PassiveApBackgrounds[i].enabled = toggle;
        }
    }

    public void UpdateApBackgrounds()
    {
        ToggleApBackgrounds(false);
        for (int i = 0; i < maxPassiveAp - (maxPassiveAp - passiveAp); i++)
        {
            PassiveApBackgrounds[i].enabled = true;
        }
    }


    private void ChangeColorOfHighlight()
    {
        if (FloorGrid.Instance.GridDictionary.TryGetValue(FloorGrid.Instance.DragMoverREF.CurrentLocationOfDragMover, out GridPoint changeHightlightColorGp))
        {
            var meshRendererREF = changeHightlightColorGp.Highlight.GetComponent<MeshRenderer>();
            var propertyBlock = new MaterialPropertyBlock();
            meshRendererREF.SetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", passiveHightlightColor);
            Debug.Log("Yoo I  Ran!");
        }
    }

    private void ResetPassiveAp()
    {
        passiveAp = maxPassiveAp;
    }


    private void BeginCoroutine()
    {
        EndCoroutine();
        coroutineREF = StartCoroutine(OnEnableEnum());
    }

    private void EndCoroutine()
    {
        if (coroutineREF != null)
        {
            StopCoroutine(coroutineREF);
            coroutineREF = null;
        }
    }

    private IEnumerator OnEnableEnum()
    {
        yield return new WaitUntil(() => LocalStoredNetworkData.GetLocalCharacter());

        var tempChar = LocalStoredNetworkData.GetLocalCharacter();
        if (tempChar.CharIdentity != Character.Identity.Speedster)
        {
            this.enabled = false;
            //  Destroy(this);
        }
    }

    //Passive : Faster  - 3 *one use per turn* movement AP
    //if youre not in the movement ui, disable passive ap

    //need something to toggle on passive visual as a whole
    //need to be able to only display ap slot that are full

    //handle keeping track of what the current passive ap value is then turning on the visuals for the background. then turn on the ap light

    //we want to ApplyPassive(); when the movement menu is clicked, and make it disappear when the menu goes down
    //make sure to keep track of the passive ap amount , display the correct amount if they use any.
    //rest to 3 when turn ends
}
