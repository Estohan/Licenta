using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public delegate void ExecuteOnAllChildrenDelegate(GameObject gameObject);

    public static void ExecuteOnAllChildren(GameObject parent, 
                                               ExecuteOnAllChildrenDelegate delegateFunction,
                                               int depth,
                                               bool includeParent = true) {
        if (includeParent) {
            delegateFunction(parent);
        }
        // Debug.Log("Entered recursion: depth = " + depth + ", [ " + parent.name + " ]");
        if (depth <= 0) {
            return;
        } else {
            IterativeExecuteOnAllChildren(parent, delegateFunction, depth - 1);
        }
    }

    private static void IterativeExecuteOnAllChildren(GameObject parent,
                                                        ExecuteOnAllChildrenDelegate delegateFunction,
                                                        int depth) {
        // Debug.Log("\t depth = " + depth + ", [ " + parent.name + " ]");
        foreach (Transform child in parent.transform) {
            delegateFunction(child.gameObject);
            if (depth == 0) {
                continue;
            } else {
                IterativeExecuteOnAllChildren(child.gameObject, delegateFunction, depth - 1);
            }
        }
    }

}
