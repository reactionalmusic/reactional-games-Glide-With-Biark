using System.Collections.Generic;
using NUnit.Framework;
using Reactional.Experimental;
using UnityEngine;
/// <summary>
/// Scriptable Object Container for Reactional Offline Data Asset
/// </summary>
[CreateAssetMenu(fileName = "DeepAnalysisAssetList", menuName = "Scriptable Objects/DeepAnalysisAssetList")]
public class DeepAnalysisAssetList : ScriptableObject
{
    public List<OfflineMusicDataAsset> songs = new List<OfflineMusicDataAsset>();
}
