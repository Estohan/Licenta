using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public LevelGenerator _LevelGenerator;
    private LevelGenerator levelGenerator;

    private Level currentLevel;

    // TEST
    public int sizeZ;
    public int sizeX;
    public int outerPaddingDiv;
    public int innerPaddingDiv;
        

    private void Awake() {
        if(instance != null && instance != this) {
            Debug.LogError("Duplicate instance of GameManager.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator = Instantiate(_LevelGenerator, new Vector3(0, 0, 0), Quaternion.identity);
        levelGenerator.transform.position.Set(0f, 0f, 0f);
        currentLevel = levelGenerator.GenerateLevel(sizeZ, sizeX, outerPaddingDiv, innerPaddingDiv);
        levelGenerator.InstantiateLevel();
    }

    public Level getCurrentLevel() {
        return currentLevel;
    }
}
