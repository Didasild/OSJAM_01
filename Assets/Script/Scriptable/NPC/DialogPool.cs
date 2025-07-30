using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Script.Scriptable.NPC
{
    [CreateAssetMenu(fileName = "DialogPool", menuName = "MineCrawler/DialogPool")]
    public class DialogPool : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private int totalWeight;

        [System.Serializable]
        public struct DialogWeightedSequence
        {
            public List<string> sentences;
            public int weight;
        }
        public List<DialogWeightedSequence> dialogPull;

        public List<string> GetSentences()
        {
            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (DialogWeightedSequence seq in dialogPull)
            {
                cumulative += seq.weight;
                if (randomValue < cumulative)
                {
                    return seq.sentences;
                }
            }

            //Debug.Log("No sentences found");
            return dialogPull[0].sentences;
        }
    
        #region EDITOR
        private void OnValidate()
        {
            UpdateTotalWeight();
        }

        private void UpdateTotalWeight()
        {
            totalWeight = 0;
            foreach (DialogWeightedSequence sequence in dialogPull)
            {
                totalWeight += sequence.weight;
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        #endregion EDITOR

    }
}
