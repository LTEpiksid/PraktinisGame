using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorridorFirstMapGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

	[SerializeField]
    private GameObject slimePrefab; // Reference to the 'slime' prefab
    
    [SerializeField]
private GameObject goblinPrefab; // Reference to the 'goblin' prefab

[SerializeField]
private GameObject golemPrefab; // Reference to the 'golem' prefab

private List<GameObject> goblinInstances = new List<GameObject>(); // List to store the spawned 'goblin' instances

private List<GameObject> golemInstances = new List<GameObject>(); // List to store the spawned 'golem' instances

    
	private List<GameObject> slimeInstances = new List<GameObject>(); // List to store the spawned 'slime' instances
	
[SerializeField]
private float slimeSpawnRadius = 5f; // Adjust the spawn radius for slime enemies

[SerializeField]
private float slimeAvoidRadius = 10f; // Adjust the avoid spawn radius for slime enemies

[SerializeField]
private float goblinSpawnRadius = 5f; // Adjust the spawn radius for goblin enemies

[SerializeField]
private float goblinAvoidRadius = 10f; // Adjust the avoid spawn radius for goblin enemies

[SerializeField]
private float golemSpawnRadius = 5f; // Adjust the spawn radius for golem enemies

[SerializeField]
private float golemAvoidRadius = 10f; // Adjust the avoid spawn radius for golem enemies
    
    protected override void RunProceduralGeneration()
{
    DeleteSlimeInstances(); // Delete previously generated 'slime' instances
    
    DeleteGoblinInstances(); // Delete previously generated 'slime' instances
        
    DeleteGolemInstances(); // Delete previously generated 'slime' instances

    HashSet<Vector2Int> floorPositions = CorridorFirstGeneration();

    SpawnSlimes(floorPositions);
    
    SpawnGoblins(floorPositions);
        
    SpawnGolems(floorPositions);
}

    
private void DeleteSlimeInstances()
{
    foreach (var instance in slimeInstances)
    {
        DestroyImmediate(instance);
    }
    slimeInstances.Clear();
}

private void DeleteGoblinInstances()
{
    foreach (var instance in goblinInstances)
    {
        DestroyImmediate(instance);
    }
    goblinInstances.Clear();
}

private void DeleteGolemInstances()
{
    foreach (var instance in golemInstances)
    {
        DestroyImmediate(instance);
    }
    golemInstances.Clear();
}


    private HashSet<Vector2Int> CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        return floorPositions;
    }
    
private void SpawnSlimes(HashSet<Vector2Int> floorPositions)
{
    Vector2Int playerPosition = new Vector2Int(0, 0); // Change this to the actual player position

    float slimeAvoidRadiusSquared = slimeAvoidRadius * slimeAvoidRadius; // Square the avoid radius for comparison
    float slimeSpawnProbability = 0.1f; // Adjust the probability of spawning a slime
    
    foreach (var position in floorPositions)
    {
        // Check if the position is suitable for spawning a 'slime' prefab
        // You can use additional conditions if needed
        if (UnityEngine.Random.value < slimeSpawnProbability)
        {
            // Check if the position is within the slime spawn radius
            if ((position - playerPosition).magnitude <= slimeSpawnRadius)
            {
                // Check if the position is outside the avoid radius around the player
                if ((position - playerPosition).sqrMagnitude > slimeAvoidRadiusSquared)
                {
                    // Check if there is no existing enemy at the spawn position
                    if (!IsEnemyAtPosition(position))
                    {
                        // Convert Vector2Int position to Vector3
                        Vector3 spawnPosition = new Vector3(position.x, position.y, 0);
                        // Instantiate the 'slime' prefab at the position
                        GameObject slimeInstance = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
                        slimeInstances.Add(slimeInstance); // Add the spawned 'slime' instance to the list
                    }
                }
            }
        }
    }
}

private void SpawnGoblins(HashSet<Vector2Int> floorPositions)
{
    Vector2Int playerPosition = new Vector2Int(0, 0); // Change this to the actual player position

    float goblinAvoidRadiusSquared = goblinAvoidRadius * goblinAvoidRadius; // Square the avoid radius for comparison
    float goblinSpawnProbability = 0.1f; // Adjust the probability of spawning a goblin


    foreach (var position in floorPositions)
    {
        // Check if the position is suitable for spawning a 'goblin' prefab
        // You can use additional conditions if needed
        if (UnityEngine.Random.value < goblinSpawnProbability)
        {
            // Check if the position is within the goblin spawn radius
            if ((position - playerPosition).magnitude <= goblinSpawnRadius)
            {
                // Check if the position is outside the avoid radius around the player
                if ((position - playerPosition).sqrMagnitude > goblinAvoidRadiusSquared)
                {
                    // Check if there is no existing enemy at the spawn position
                    if (!IsEnemyAtPosition(position))
                    {
                        // Convert Vector2Int position to Vector3
                        Vector3 spawnPosition = new Vector3(position.x, position.y, 0);
                        // Instantiate the 'goblin' prefab at the position
                        GameObject goblinInstance = Instantiate(goblinPrefab, spawnPosition, Quaternion.identity);
                        goblinInstances.Add(goblinInstance); // Add the spawned 'goblin' instance to the list
                    }
                }
            }
        }
    }
}

private void SpawnGolems(HashSet<Vector2Int> floorPositions)
{
    Vector2Int playerPosition = new Vector2Int(0, 0); // Change this to the actual player position

    float golemAvoidRadiusSquared = golemAvoidRadius * golemAvoidRadius; // Square the avoid radius for comparison
    float golemSpawnProbability = 0.1f; // Adjust the probability of spawning a golem

    foreach (var position in floorPositions)
    {
        // Check if the position is suitable for spawning a 'golem' prefab
        // You can use additional conditions if needed
        if (UnityEngine.Random.value < golemSpawnProbability)
        {
            // Check if the position is within the golem spawn radius
            if ((position - playerPosition).magnitude <= golemSpawnRadius)
            {
                // Check if the position is outside the avoid radius around the player
                if ((position - playerPosition).sqrMagnitude > golemAvoidRadiusSquared)
                {
                    // Check if there is no existing enemy at the spawn position
                    if (!IsEnemyAtPosition(position))
                    {
                        // Convert Vector2Int position to Vector3
                        Vector3 spawnPosition = new Vector3(position.x, position.y, 0);
                        // Instantiate the 'golem' prefab at the position
                        GameObject golemInstance = Instantiate(golemPrefab, spawnPosition, Quaternion.identity);
                        golemInstances.Add(golemInstance); // Add the spawned 'golem' instance to the list
                    }
                }
            }
        }
    }
}

private bool IsEnemyAtPosition(Vector2Int position)
{
    foreach (var slimeInstance in slimeInstances)
    {
        if (slimeInstance.transform.position.x == position.x && slimeInstance.transform.position.y == position.y)
        {
            return true;
        }
    }

    foreach (var goblinInstance in goblinInstances)
    {
        if (goblinInstance.transform.position.x == position.x && goblinInstance.transform.position.y == position.y)
        {
            return true;
        }
    }

    foreach (var golemInstance in golemInstances)
    {
        if (golemInstance.transform.position.x == position.x && golemInstance.transform.position.y == position.y)
        {
            return true;
        }
    }

    return false;
}



    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (!roomFloors.Contains(position))
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
                        currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }
    public List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
    	List<Vector2Int> newCorridor = new List<Vector2Int>();
    	for (int i = 1; i < corridor.Count; i++)
    	{
    		for (int x = -1; x < 2; x++)
    		{
    			for (int y = -1; y < 2; y++)
    			{
    				newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
    			}
    		}
    	}
    	return newCorridor;
    }
    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return Vector2Int.right;
        if (direction == Vector2Int.right)
            return Vector2Int.down;
        if (direction == Vector2Int.down)
            return Vector2Int.left;
        if (direction == Vector2Int.left)
            return Vector2Int.up;
        return Vector2Int.zero;
    }
}
