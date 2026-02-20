using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    public interface IInteractable
    {
        public void Clicked() { }

        private void Unclicked() { }
    }
}
