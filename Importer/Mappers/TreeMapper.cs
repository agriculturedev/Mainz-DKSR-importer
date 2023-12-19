using DKSRDomain;
using FrostApi.ResponseModels.Thing;
using FrostApi.ThingImplementations;

namespace Importer.Mappers;

public static class TreeMapper
{
    public static List<Tree> MapDksrResponseToTrees(TreesenseResponse response)
    {
        return response.SensorData.Select(sensorData =>
        {
            var tree = new Tree();
            tree.Name = sensorData.Name;
            tree.Properties.Id = int.Parse(sensorData.Id);
            return tree;
        }).ToList();
    }

    public static Tree MapDksrResponseToTree(TreeSenseSensorData treeData)
    {
        return new Tree
        {
            Name = treeData.Name,
            Properties = new TreeProps { Id = int.Parse(treeData.Id) }
        };
    }

    public static List<Tree> MapFrostResponseToTrees(GetThingsResponse response)
    {
        return response.Value.Select(thing =>
        {
            var tree = new Tree();
            tree.Id = thing.Id;
            tree.Name = thing.Name;
            tree.Properties.Id = int.Parse(thing.Properties["Id"]);
            return tree;
        }).ToList();
    }
}