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

    private bool isRevealed;

    private int layerMask;
    private RaycastHit hitInfo;
    private CellConcealer hitOccluder;
    private bool hit;


    private void Start() {
        isRevealed = false;
        occluderObjectRenderer = occluderObject.GetComponent<MeshRenderer>();

        layerMask = (1 << LayerMask.NameToLayer("CellConcealers"));
    }

    public void RevealNeighbours() {

        RevealCell();
        foreach (MazeDirection direction in Enum.GetValues(typeof(MazeDirection))) {
            // Cast a ray of length 'cellSize' in each direction
            hit = Physics.Raycast(this.transform.position, direction.ToVector3(), out hitInfo, Constants.cellSize, layerMask);
            if (hit) {
                //Debug.Log("cast ray in " + direction + "(" + hitInfo.transform.gameObject.name + ")...");
                hitOccluder = hitInfo.transform.gameObject.GetComponent<CellConcealer>();
                // If an occluder is hit, check if the cell is already revealed
                if (!hitOccluder.isRevealed) {
                    // Automatically reveal padding cells
                    if (hitOccluder.mazeCellObject.data.type == CellType.InnerPadding ||
                        hitOccluder.mazeCellObject.data.type == CellType.OuterPadding) {
                        //Debug.Log("\t ...and hit padding");
                        hitOccluder.RevealCell();
                    } else {
                        // Reveal common cells only if there are no walls in between
                        if (!hitOccluder.mazeCellObject.data.walls[(int)direction.GetOppositeDirection()]) {
                            //Debug.Log("\t ...and revealed a new cell");
                            hitOccluder.RevealCell();
                        } else {
                            //Debug.Log("\t ...but cell had wall in " + direction.GetOppositeDirection());
                        }
                    }
                }
            }
        }
    }

    public void RevealCell() {
        occluderObjectRenderer.enabled = false;
        isRevealed = true;
    }

    public bool IsRevealed() {
        return isRevealed;
    }

}
