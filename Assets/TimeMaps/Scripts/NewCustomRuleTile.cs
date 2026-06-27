using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.TileMaps
{
    [CreateAssetMenu]
    public class NewCustomRuleTile : RuleTile<NewCustomRuleTile.Neighbor>
    {
        public bool connectToBasicTiles;
        public Object[] connectedWith;

        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            //public const int Null = 3;
            //public const int NotNull = 4;
        }

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This:
                    if (other == null)
                        return false;

                    if (other == this)
                    {
                        return true;
                    }
                    else
                    {
                        if (other is NewCustomRuleTile r)
                        {
                            if (IsConnected(r))
                                return true;
                            else
                                return false;
                        }

                        return connectToBasicTiles;
                    }
                case TilingRuleOutput.Neighbor.NotThis:
                    if (other == null)
                        return true;

                    if (other == this)
                    {
                        return false;
                    }
                    else
                    {
                        if (other is NewCustomRuleTile r)
                        {
                            if (IsConnected(r))
                                return false;
                            else
                                return true;
                        }

                        return !connectToBasicTiles;
                    }
            }


            // switch (neighbor)
            // {
            //     case TilingRuleOutput.Neighbor.This:
            //         if (other == this)
            //             return true;
            //         else
            //             if (other is NewCustomRuleTile rule)
            //                 return string.IsNullOrEmpty(rule.ruleGroup);
            //             else
            //                 return other != null;
            //         
            //     case TilingRuleOutput.Neighbor.NotThis: 
            //         if (other is NewCustomRuleTile rule2)
            //             return rule2.ruleGroup != ruleGroup;
            //         return other == null;
            // }

            return true;
        }


        bool IsConnected(TileBase tile)
        {
            if (connectedWith == null)
                return false;

            foreach (var t in connectedWith)
            {
                if (t == tile)
                    return true;
            }

            return false;
        }
    }
}