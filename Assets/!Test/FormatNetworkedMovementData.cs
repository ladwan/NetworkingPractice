using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public static class FormatNetworkedMovementData
    {
        private static List<Vector3> remotePlayersHoveredOverGPs = new List<Vector3>();

        public static Action<List<Vector3>> movementComplete;


        public static List<Vector3> RemotePlayersHoveredOverGPs { get => remotePlayersHoveredOverGPs; set => remotePlayersHoveredOverGPs = value; }


        public static void Format(int x, int y, int hoveredOverGPsCount)
        {
            var v = new Vector3(x, 0.0f, y);

            remotePlayersHoveredOverGPs.Add(v);

            if (remotePlayersHoveredOverGPs.Count == hoveredOverGPsCount)
            {
                //run movement logic

                movementComplete?.Invoke(remotePlayersHoveredOverGPs);
                //remotePlayersHoveredOverGPs.Clear();
            }
        }
    }
}
