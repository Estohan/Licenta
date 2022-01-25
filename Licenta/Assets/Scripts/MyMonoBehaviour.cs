using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyMonoBehaviour : MonoBehaviour {
    private bool _started = false;

    protected virtual void Start() {
        _started = true;
        this.SafeOnEnable();
    }

    protected virtual void OnEnable() {
        if(_started) {
            this.SafeOnEnable();
        }
    }

    protected virtual void SafeOnEnable() {

    }
}
