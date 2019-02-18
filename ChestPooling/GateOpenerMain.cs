using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace GateOpener
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class GateOpenerMainClass : Mod
    {
        /*********
        ** Fields
        *********/
        private readonly Dictionary<Vector2, Fence> OpenGates = new Dictionary<Vector2, Fence>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        private Fence GetGate(BuildableGameLocation location, Vector2 pos)
        {
            if (!location.objects.TryGetValue(pos, out StardewValley.Object obj))
                return null;

            if (obj is Fence fence && fence.isGate.Value && !this.OpenGates.ContainsKey(pos))
            {
                this.OpenGates[pos] = fence;
                return fence;
            }
            return null;
        }

        private Fence LookAround(BuildableGameLocation location, List<Vector2> list)
        {
            foreach (Vector2 pos in list)
            {
                Fence gate = this.GetGate(location, pos);
                if (gate != null)
                    return gate;
            }
            return null;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(4) && Game1.currentLocation is BuildableGameLocation location)
            {
                List<Vector2> adj = Utility.getAdjacentTileLocations(Game1.player.getTileLocation());
                Fence gate = this.LookAround(location, adj);
                if (gate != null)
                {
                    gate.gatePosition.Set(88);
                    Game1.playSound("doorClose");
                }

                //need to close it now...
                foreach (KeyValuePair<Vector2, Fence> gateObj in OpenGates)
                {
                    if (Game1.player.getTileLocation() != gateObj.Key && !adj.Contains(gateObj.Key))
                    {
                        gateObj.Value.gatePosition.Set(0);
                        Game1.playSound("doorClose");
                        OpenGates.Remove(gateObj.Key);
                        break;
                    }
                }
            }
        }
    }
}
