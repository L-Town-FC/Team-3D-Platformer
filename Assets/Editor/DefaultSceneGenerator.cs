using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class DefaultSceneGenerator: Editor
{
    //String names of all prefabs that will be loaded
    static string playerString = "Player";
    static string camerRigString = "CameraRig";
    static string levelRootString = "LevelRoot";
    static string deathPlaneString = "DeathPlane";
    static string groundString = "Ground";

    static string defaultSceneName = "DefaultScene";

    [MenuItem("Assets/Generate Default Scene")]
    private static void GenerateScene()
    {
        //Generates new scene with light and camera

        Scene defaultScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        defaultScene.name = "DefaultScene";

        //destroy camera so camera rig can be added
        GameObject camera = GameObject.Find("Main Camera");
        if(camera != null)
        {
            DestroyImmediate(camera);
        }

        //loads all prefabs into scene
        GameObject levelRoot = LoadPrefab(levelRootString);
        GameObject cameraRig = LoadPrefab(camerRigString);
        GameObject player = LoadPrefab(playerString);
        GameObject deathPlane = LoadPrefab(deathPlaneString);
        GameObject ground = LoadPrefab(groundString);

        //sets up camera rig/player interaction
        if(player.TryGetComponent<PlayerCamera>(out PlayerCamera playerCamera))
        {
            playerCamera.cameraTarget = cameraRig.transform.Find("CameraTarget");
        }

        //sets directional light to face directly downward
        GameObject directionLight = GameObject.Find("Directional Light");
        directionLight.transform.eulerAngles = new Vector3(90, 0, 0);

        //saves the scene
        EditorSceneManager.SaveScene(defaultScene, "Assets/Scenes/" + defaultSceneName + ".unity");
    }

    static GameObject LoadPrefab(string _prefabName)
    {
        //finds object in the prefab folder with the specified name and load it into the scene
        Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + _prefabName + ".prefab", typeof(GameObject));
        if(prefab != null)
        {
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
        return null;
    }
}
