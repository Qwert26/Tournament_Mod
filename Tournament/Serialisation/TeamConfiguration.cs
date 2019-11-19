using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using System;
using UnityEngine;

namespace Tournament.Serialisation {
    /// <summary>
    /// Enthält die Team-Konfiguration.
    /// </summary>
    [Serializable]
    public class TeamConfiguration {
        public TeamConfiguration() {
            FleetColors.Add(new Color(0, 0, 0, 0));
            FleetColors.Add(new Color(0, 0, 0, 0));
            FleetColors.Add(new Color(0, 0, 0, 0));
            FleetColors.Add(new Color(0, 0, 0, 0));
        }
        #region Spawn-Einstellungen
        public Var<int> SpawngapLeftRight { get; set; } = new VarIntClamp(100, -1000, 1000);
        public Var<int> SpawngapForwardBackward { get; set; } = new VarIntClamp(100, -1000, 1000);
        public Var<int> FormationIndex { get; set; } = new VarIntClamp(0, 0, TournamentFormation.tournamentFormations.Length - 1);
        #endregion
        #region Grenzen
        /// <summary>
        /// Laut dem Planeten-Editor sind -1000 und 100000 absolute Grenzen.
        /// </summary>
        public VarVector2PairClamp AltitudeLimits { get; set; } = new VarVector2PairClamp(new Vector2(-50, 500), 0, -1000, 100000);
        public Var<int> DistanceLimit { get; set; } = new VarIntClamp(1500, 0, 10000);
        public Var<bool> ProjectedDistance { get; set; } = new VarBool(false);
        public Var<bool> SoftLimits { get; set; } = new VarBool(true);
        public Var<int> MaximumBufferTime { get; set; } = new VarIntClamp(3, 0, 360);
        public Var<int> DistanceReversing { get; set; } = new VarIntClamp(3, -500, 500);
        public Var<int> AltitudeReversing { get; set; } = new VarIntClamp(-3, -500, 500);
        #endregion
        #region Materialien
        public Var<bool> InfinteMaterials { get; set; } = new VarBool(false);
        public Var<bool> DistributeMaterials { get; set; } = new VarBool(true);
        public Var<int> MaterialAllowance { get; set; } = new VarIntClamp(10000, 0, 1000000);
        #endregion
        #region Augenschmaus
        /// <summary>
        /// Reihenfolge ist Main, Secondary, Trim, Detail.
        /// </summary>
        public VarList<Color> FleetColors { get; set; } = new ColorList();
        #endregion
        public bool IsCurrentlyBreakingTeamRules(MainConstruct teamMember, MainConstruct nearestEnemy) {
            #region Prüfe Distanz
            Vector3 deltaPos = nearestEnemy.CentreOfMass - teamMember.CentreOfMass;
            if (ProjectedDistance) {
                deltaPos.y = 0;
            }
            float currentDistance = deltaPos.magnitude;
            if (currentDistance > DistanceLimit) {
                deltaPos += teamMember.Velocity;
                if (ProjectedDistance)
                {
                    deltaPos.y = 0;
                }
                if (deltaPos.magnitude - currentDistance > DistanceReversing)
                {
                    return true;
                }
            }
            #endregion
            #region Prüfe Höhen
            if (AltitudeLimits.Lower > teamMember.AltitudeOfComAboveMeanSeaLevel) {
                if (teamMember.Velocity.y < AltitudeReversing) {
                    return true;
                }
            }
            if (AltitudeLimits.Upper < teamMember.AltitudeOfComAboveMeanSeaLevel) {
                if (teamMember.Velocity.y > AltitudeReversing) {
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}