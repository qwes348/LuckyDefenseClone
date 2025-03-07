using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypointsContainer : MonoBehaviour
{
    [SerializeField, DrawHandle]
    private List<Vector3> playerSideWaypoints;
    [SerializeField, DrawHandle]
    private List<Vector3> opponentSideWaypoints;
    
    #region Properties
    public IReadOnlyList<Vector3> PlayerSideWaypoints => playerSideWaypoints;
    public IReadOnlyList<Vector3> OpponentSideWaypoints => opponentSideWaypoints;
    #endregion

    private void Start()
    {
        InGameManagers.FieldMgr.enemyWaypointsContainer = this;
    }
}
