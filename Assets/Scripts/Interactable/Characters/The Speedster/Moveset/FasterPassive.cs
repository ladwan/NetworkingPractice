using System;
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
    private MeshRenderer meshRendererREF = null;
    private List<MeshRenderer> passiveHighlightedMeshRenderers = new List<MeshRenderer>();
    private bool isChangeColorSubscribed = false;


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

    public int MaxPassiveAp { get => maxPassiveAp; set => maxPassiveAp = value; }

    #endregion


    protected void OnEnable()
    {
        BeginCoroutine();
        PassiveAbilityName = "Faster";
        PassiveAbilityDescription = "You can move 3 extra squares each turn";
        ActionPointsManager.Instance.SpeedsterPassiveApLists = referenceLists;
        CombatUiStatesManager.Instance.OnCombatUiStateChange += ApplyPassive;
        PlayerTurnManager.Instance.OnTurnEnd += ResetPassiveAp;
    }

    protected void OnDisable()
    {
        CombatUiStatesManager.Instance.OnCombatUiStateChange -= ApplyPassive;
        PlayerTurnManager.Instance.OnTurnEnd -= ResetPassiveAp;
    }


    public void ApplyPassive()
    {
        if (CombatUiStatesManager.Instance.CurrentCombatUiState == CombatUiStatesManager.CombatUiState.movement)
        {
            if (passiveAp > 0)
            {
                if (!isChangeColorSubscribed)
                {
                    FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated += ChangeColorOfHighlight;
                    isChangeColorSubscribed = true;
                }

                ActionPointsManager.Instance.UpdateAP(referenceLists, 0);
                UpdateApBackgrounds();
                ChangeColorOfHighlight();
            }
            else
            {
                if (isChangeColorSubscribed)
                {
                    FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated -= ChangeColorOfHighlight;
                    isChangeColorSubscribed = false;
                }

                ResetPropertyBlocks();
                ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, 0);
            }
        }
        else
        {
            ToggleApBackgrounds(false);
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

    public void UpdateMaxApValue(int value)
    {
        maxPassiveAp = value;
    }

    public void SetMaxPassiveApPool(int value)
    {
        var spentAp = maxPassiveAp - passiveAp;
        maxPassiveAp = value;
        var difference = (maxPassiveAp - passiveAp) - spentAp;
        ActionPointsManager.Instance.UpdateAP(referenceLists, difference);
    }


    private void ChangeColorOfHighlight()
    {
        if (FloorGrid.Instance.GridDictionary.TryGetValue(FloorGrid.Instance.DragMoverREF.CurrentLocationOfDragMover, out GridPoint changeHightlightColorGp))
        {
            meshRendererREF = changeHightlightColorGp.Highlight.GetComponent<MeshRenderer>();
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", passiveHightlightColor);
            meshRendererREF.SetPropertyBlock(propertyBlock);
            passiveHighlightedMeshRenderers.Add(meshRendererREF);
        }
    }

    private void ResetPropertyBlocks()
    {
        if (passiveHighlightedMeshRenderers.Count > 0)
        {
            foreach (MeshRenderer meshRenderer in passiveHighlightedMeshRenderers)
            {
                meshRenderer.SetPropertyBlock(null);
            }
            passiveHighlightedMeshRenderers.Clear();
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
        }
    }
}
