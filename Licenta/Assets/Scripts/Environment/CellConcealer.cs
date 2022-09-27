using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Controls the fog particle systems that initially conceal the maze.
 *  It reveals connected cells and padding cells when the player gets
 *  close to them and also triggers the activation of their associated
 *  map icons.
 */
public class CellConcealer : MonoBehaviour {

    [SerializeField]
    private MazeCellObject mazeCellObject;
    [SerializeField]
    private ParticleSystem fogParticles;

    private int layerMask;
    private RaycastHit hitInfo;
    private CellConcealer hitOccluder;
    private bool hit;


    private void Start() {
        layerMask = (1 << LayerMask.NameToLayer("CellConcealers"));
    }

    public List<MazeCoords> RevealNeighbours() {
        List<MazeCoords> revealedCells = new List<MazeCoords>();

        if (!mazeCellObject.revealed) {
            RevealCell();
            revealedCells.Add(new MazeCoords(mazeCellObject.data.coordinates));
        }
        foreach (MazeDirection direction in Enum.GetValues(typeof(MazeDirection))) {
            // Cast a ray of length 'cellSize' in each direction
            hit = Physics.Raycast(this.transform.position, direction.ToVector3(), out hitInfo, Constants.cellSize, layerMask);
            if (hit) {
                hitOccluder = hitInfo.transform.gameObject.GetComponent<CellConcealer>();
                // If an occluder is hit, check if the cell is already revealed
                if (!hitOccluder.mazeCellObject.revealed) {
                    // Automatically reveal padding cells
                    if (hitOccluder.mazeCellObject.data.type == CellType.InnerPadding ||
                        hitOccluder.mazeCellObject.data.type == CellType.OuterPadding) {
                        hitOccluder.RevealCell();
                        revealedCells.Add(new MazeCoords(hitOccluder.mazeCellObject.data.coordinates));
                    } else {
                        // Reveal common cells only if there are no walls in between
                        if (!hitOccluder.mazeCellObject.data.walls[(int)direction.GetOppositeDirection()]) {
                            hitOccluder.RevealCell();
                            revealedCells.Add(new MazeCoords(hitOccluder.mazeCellObject.data.coordinates));
                        } 
                    }
                }
            }
        }

        return revealedCells;
    }

    public void RevealCell() {
        fogParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        mazeCellObject.revealed = true;
        mazeCellObject.MapIconsVisibility(true);
    }
}
