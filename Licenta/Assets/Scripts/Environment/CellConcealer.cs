using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellConcealer : MonoBehaviour {

    [SerializeField]
    private GameObject occluderObject;
    [SerializeField]
    private MazeCellObject mazeCellObject;
    private MeshRenderer occluderObjectRenderer;
    [SerializeField]
    private ParticleSystem fogParticles;

    private int layerMask;
    private RaycastHit hitInfo;
    private CellConcealer hitOccluder;
    private bool hit;


    private void Start() {
        // occluderObjectRenderer = occluderObject.GetComponent<MeshRenderer>();

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
                //Debug.Log("cast ray in " + direction + "(" + hitInfo.transform.gameObject.name + ")...");
                hitOccluder = hitInfo.transform.gameObject.GetComponent<CellConcealer>();
                // If an occluder is hit, check if the cell is already revealed
                if (!hitOccluder.mazeCellObject.revealed) {
                    // Automatically reveal padding cells
                    if (hitOccluder.mazeCellObject.data.type == CellType.InnerPadding ||
                        hitOccluder.mazeCellObject.data.type == CellType.OuterPadding) {
                        //Debug.Log("\t ...and hit padding");
                        hitOccluder.RevealCell();
                        revealedCells.Add(new MazeCoords(hitOccluder.mazeCellObject.data.coordinates));
                    } else {
                        // Reveal common cells only if there are no walls in between
                        if (!hitOccluder.mazeCellObject.data.walls[(int)direction.GetOppositeDirection()]) {
                            //Debug.Log("\t ...and revealed a new cell");
                            hitOccluder.RevealCell();
                            revealedCells.Add(new MazeCoords(hitOccluder.mazeCellObject.data.coordinates));
                        } else {
                            //Debug.Log("\t ...but cell had wall in " + direction.GetOppositeDirection());
                        }
                    }
                }
            }
        }

        return revealedCells;
    }

    public void RevealCell() {
        // occluderObjectRenderer.enabled = false;
        fogParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        // Debug.Log("Revealed cell " + mazeCellObject.name);
        mazeCellObject.revealed = true;
        mazeCellObject.MapIconsVisibility(true);
    }
}
