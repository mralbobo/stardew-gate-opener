using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Newtonsoft.Json;
using System.IO;

namespace GateOpener
{
    public class GateOpenerMainClass : Mod
    {
        public override void Entry(params object[] objects)
        {
            //StardewModdingAPI.Events.GameEvents.UpdateTick += Event_Update;
            StardewModdingAPI.Events.GameEvents.FourthUpdateTick += Event_Update;
            
        }

        static void myLog(String theString) { 
            #if DEBUG
            Log.Info(theString);
            #endif
        }

        static Dictionary<Vector2, StardewValley.Fence> openGates = new Dictionary<Vector2, Fence>();


        static void debugThing(object theObject, string descriptor = "")
        {
            String thing = JsonConvert.SerializeObject(theObject, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText("debug.json", thing);
            Console.WriteLine(descriptor + "\n"+ thing);
        }

        static StardewValley.Fence getGate(Vector2 pos)
        {
            StardewValley.Locations.BuildableGameLocation loc = StardewValley.Game1.currentLocation as StardewValley.Locations.BuildableGameLocation;
            StardewValley.Object obj;
            loc.objects.TryGetValue(pos, out obj);
            if (obj != null && obj is StardewValley.Fence)
            {
                StardewValley.Fence gate = obj as StardewValley.Fence;
                if (gate.isGate && !openGates.ContainsKey(pos))
                {
                    openGates[pos] = gate;
                    return obj as StardewValley.Fence;
                }
            }
            return null;
        }

        static StardewValley.Fence lookAround(List<Vector2> list)
        {
            foreach (Vector2 pos in list)
            {
                StardewValley.Fence gate = getGate(pos);
                if (gate != null) { return gate; }
            }
            return null;
        }

        static void Event_Update(object sender, EventArgs e)
        {
            if(StardewValley.Game1.currentLocation is StardewValley.Locations.BuildableGameLocation)
            {
                List<Vector2> adj = StardewValley.Game1.player.getAdjacentTiles();
                StardewValley.Fence gate = lookAround(adj);
                if (gate != null)
                {
                    //myLog(gate.ToString());
                    gate.gatePosition = 88;
                }

                //need to close it now...
                foreach (KeyValuePair<Vector2, StardewValley.Fence> gateObj in openGates)
                {
                    if (StardewValley.Game1.player.getTileLocation() != gateObj.Key && !adj.Contains(gateObj.Key) )
                    {
                        gateObj.Value.gatePosition = 0;
                        openGates.Remove(gateObj.Key);
                        break;
                    }
                }
            }
        }
        
    }
}
