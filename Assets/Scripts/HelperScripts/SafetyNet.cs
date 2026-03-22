using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public static class SafetyNet
    {
        public static bool IsValid<T>(T obj, string context = "") where T : class
        {
            // Unity's special null check handles destroyed objects
            if (obj is UnityEngine.Object unityObj)
            {
                if (unityObj == null)
                {
                    string errorMsg = string.IsNullOrEmpty(context)
                        ? $"{typeof(T).Name} was null!"
                        : $"[{context}] {typeof(T).Name} was null!";
                    Debug.LogWarning(errorMsg);
                    return false;
                }
            }
            // Regular C# null check
            else if (obj == null)
            {
                string errorMsg = string.IsNullOrEmpty(context)
                    ? $"{typeof(T).Name} was null!"
                    : $"[{context}] {typeof(T).Name} was null!";
                Debug.LogWarning(errorMsg);
                return false;
            }

            return true;
        }
    }
}