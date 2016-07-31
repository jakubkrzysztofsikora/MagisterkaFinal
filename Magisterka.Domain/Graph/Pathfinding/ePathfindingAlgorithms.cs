using System.ComponentModel;
using Magisterka.Domain.Converters;

namespace Magisterka.Domain.Graph.Pathfinding
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ePathfindingAlgorithms
    {
        [Description("Djikstra Algorithm")]
        Djikstra,
        [Description("Bellman-Ford Algorithm")]
        BellmanFord,
        [Description("A* Algorithm")]
        AStar,
        [Description("Floyd-Warshall Algorithm")]
        FloydWarshall,
        [Description("Johnson Algorithm")]
        Johnson
    }
}