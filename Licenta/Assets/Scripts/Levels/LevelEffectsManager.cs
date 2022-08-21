using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelEffectsManager {
    // public delegate void LevelEffect(LevelEffects levelEffect);

    public static void ExecuteEffect(LevelEffects levelEffect) {
        switch(levelEffect) {
            case LevelEffects.MapReveal:
                RevealMap();
                break;
            case LevelEffects.DestinationReveal:
                RevealDestination();
                break;
            default:
                break;
        }
    }

    private static void RevealMap() {
        Level currentLevel = GameManager.instance.getCurrentLevel();
        List<MazeCoords> hiddenCells = new List<MazeCoords>();
        List<MazeCoords> candidates = new List<MazeCoords>();
        List<MazeCoords> revealedCells;
        int sizeZ = currentLevel.sizeZ;
        int sizeX = currentLevel.sizeX;
        int cellsCount = sizeZ * sizeX;
        int revealedPreviouslyCount = 0;
        int revealedNowCount = 0;
        int toRevealCount;
        int randIndex;
        int seedsCount = 3; // [ Debug ] Hardcoded value
        float percentageToReveal = 0.1f; // [ Debug ] Hardcoded value

        /*// this is used to prevent the case where all seeds are chosen inside 'holes'
        // so they cannot expand. The solution would be to start another seed elsewhere
        // but I do not currently have the time to implement that so I'll just exit
        // the function if that case is detected
        // Update: the function will exit when there are no candidates left.
        int _emptyIterations = 0;*/


        // Gather info cells visibility
        for (int z = 0; z < sizeZ; z ++) {
            for (int x = 0; x < sizeX; x ++) {
                if (currentLevel.cellsObjects[z, x].revealed) {
                    revealedPreviouslyCount++;
                } else {
                    hiddenCells.Add(new MazeCoords(z, x));
                }
            }
        }

        // Set number of cells to reveal (10%)
        toRevealCount = (int)(cellsCount * percentageToReveal);

        // Choose seed cells as starting points for the zones that will be revealed
        if (cellsCount - revealedPreviouslyCount < seedsCount) {
            seedsCount = cellsCount - revealedPreviouslyCount;
        }

        for (int i = 0; i < seedsCount; i ++) {
            randIndex = UnityEngine.Random.Range(0, hiddenCells.Count);
            candidates.Add(hiddenCells[randIndex]);
            hiddenCells.RemoveAt(randIndex);
        }
        Debug.Log(cellsCount + " " + revealedPreviouslyCount + " " + revealedNowCount + " " + toRevealCount + " " + seedsCount);
        // Reveal seed cells
        for (int i = 0; i < seedsCount; i ++) {
            revealedCells = currentLevel.cellsObjects[candidates[i].z, candidates[i].x].GetOccluder().RevealNeighbours();
            Debug.Log("Revealed seed at " + candidates[i] + " and " + revealedCells.Count + " neighbours.");
            revealedNowCount += revealedCells.Count;
            candidates.AddRange(revealedCells);
        }

        // Remove seed cells from candidates list
        candidates.RemoveRange(0, seedsCount);
        Debug.Log("Candidates count: " + candidates.Count);
        Debug.Log("=>" + cellsCount + " " + revealedPreviouslyCount + " " + revealedNowCount + " " + toRevealCount + " " + seedsCount);

        // Reveal map zones
        while (revealedNowCount < toRevealCount && revealedPreviouslyCount < cellsCount && candidates.Count > 0) {
            randIndex = UnityEngine.Random.Range(0, candidates.Count);
            revealedCells = currentLevel.cellsObjects[candidates[randIndex].z, candidates[randIndex].x]
                                .GetOccluder().RevealNeighbours();
            revealedNowCount += revealedCells.Count;
            candidates.RemoveAt(randIndex);
            candidates.AddRange(revealedCells);

            Debug.Log("->" + cellsCount + " " + revealedPreviouslyCount + " " + revealedNowCount + " " + toRevealCount + " " + seedsCount);
        }
    }

    public static void RevealDestination() {
        MazeCoords finishCell = GameManager.instance.getCurrentLevel().stats.finishCell;
        GameManager.instance.getCurrentLevel().cellsObjects[finishCell.z, finishCell.x].GetOccluder().RevealCell();
    }

    public enum LevelEffects {
        MapReveal,
        DestinationReveal
    }
}
