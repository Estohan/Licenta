using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewBuilder : MonoBehaviour {
    public GameObject _root;
    public GameObject _floorWhite;
    public GameObject _floorGrey;
    public GameObject _corner;
    [Space]
    public bool originToGeometry;
    public bool includeCorners;
    [Space]
    public int mazeSizeX;
    public int mazeSizeZ;

    GameObject currentFloor;
    GameObject currentRoot;
    float cellSize;


    public void GenerateBoard() {
        currentRoot = GameObject.Find("_LevelRoot(Clone)");
        if (currentRoot != null) {
            DestroyImmediate(currentRoot);
        }

        Transform rootParent = Instantiate(_root).transform;

        for(int z = 0; z < mazeSizeZ; z++) {
            for(int x = 0; x < mazeSizeX; x++) {
                if((z+x) % 2 == 0) {
                    currentFloor = _floorWhite;
                } else {
                    currentFloor = _floorGrey;
                }

                // Floor instantiation
                GameObject newFloor = Instantiate(currentFloor, rootParent);
                newFloor.name += (z + "-" + x);
                // Move floor to correct position
                cellSize = newFloor.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                transform.parent = transform;
                newFloor.transform.localPosition =
                            new Vector3(x * cellSize - cellSize / 2,
                                        0f,
                                       -z * cellSize + cellSize / 2);

                // Corner instantiation
                if (includeCorners) {
                    GameObject currentCorner;
                    Vector3 position = Vector3.zero;
                    float offset = _corner.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.z / 2;
                    if (originToGeometry) {
                        position.y = _corner.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * 2;
                    } else {
                        position.y = 0f;
                    }

                    for (int i = 0; i < 4; i++) {
                        switch (i) {
                            case 0:
                                position.z = (cellSize / 2) - offset;
                                position.x = -(cellSize / 2) + offset;
                                break;
                            case 1:
                                position.z = (cellSize / 2) - offset;
                                position.x = (cellSize / 2) - offset;
                                break;
                            case 2:
                                position.z = -(cellSize / 2) + offset;
                                position.x = (cellSize / 2) - offset;
                                break;
                            default:
                                position.z = -(cellSize / 2) + offset;
                                position.x = -(cellSize / 2) + offset;
                                break;
                        }

                        currentCorner = Instantiate(_corner, newFloor.transform);
                        currentCorner.name = "corner " + i;
                        currentCorner.transform.localPosition = position;
                    }
                }
            }
        }
    }

    public void DeleteBoard() {
        currentRoot = GameObject.Find("_LevelRoot(Clone)");
        if (currentRoot != null) {
            DestroyImmediate(currentRoot);
        }
    }
}
