using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Builds Assets/Scenes/Lab04_PlayerLevel.unity from the imported Level_Warehouse scene:
/// adds the Player (CharacterController + PlayerController), makes the Main Camera follow it, and adds a goal trigger.
/// Menu: Lab04/Build Player Scene
/// </summary>
public static class Lab04Setup
{
    const string Source = "Assets/Scenes/Lab03/Level_Warehouse.unity";   // imported from the exported package
    const string Target = "Assets/Scenes/Lab04_PlayerLevel.unity";

    [MenuItem("Lab04/Build Player Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(Source, OpenSceneMode.Single);

        WidenDoorway();

        // where the level says the player starts
        Vector3 spawn = new Vector3(-7.5f, 0.1f, -33f);
        var pad = GameObject.Find("Spawn");
        if (pad != null) { var b = pad.GetComponentInChildren<Renderer>().bounds; spawn = new Vector3(b.center.x, b.max.y, b.center.z); }

        // ---- the player: a 2 m tall capsule with a small "nose" so you can see which way it faces
        var player = new GameObject("Player");
        player.transform.position = spawn + Vector3.up * 0.05f;
        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f; cc.radius = 0.4f; cc.center = new Vector3(0, 1, 0);
        cc.stepOffset = 0.35f; cc.slopeLimit = 50f; cc.skinWidth = 0.05f;

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body"; Object.DestroyImmediate(body.GetComponent<Collider>());
        body.transform.SetParent(player.transform, false);
        body.transform.localPosition = new Vector3(0, 1, 0); body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        body.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grey_Light.mat");

        var nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nose.name = "Nose"; Object.DestroyImmediate(nose.GetComponent<Collider>());
        nose.transform.SetParent(player.transform, false);
        nose.transform.localPosition = new Vector3(0, 1.5f, 0.4f); nose.transform.localScale = new Vector3(0.25f, 0.2f, 0.5f);
        nose.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grey_Ground.mat");

        var controller = player.AddComponent<PlayerController>();
        player.AddComponent<LevelHud>();

        // the same default route the level marks on the floor (used only for the recorded walkthrough)
        var auto = player.AddComponent<RouteAutopilot>();
        auto.waypoints = new[] {
            new Vector3(-7.5f, 0.1f, -33), new Vector3(-7.5f, 0.1f, -20), new Vector3(-1, 0.1f, -20), new Vector3(-1, 0.1f, -11),
            new Vector3(-7.5f, 0.1f, -11), new Vector3(-7.5f, 0.1f, -8.5f), new Vector3(-7.5f, 3.1f, -1.4f),
            new Vector3(-7.5f, 3.1f, 3), new Vector3(7.5f, 3.1f, 3) };

        // ---- the camera follows the player
        var cam = Camera.main;
        var follow = cam.gameObject.AddComponent<ThirdPersonCamera>();
        follow.target = player.transform;
        cam.transform.position = spawn + new Vector3(0, 3.5f, -6f);
        controller.cameraTransform = cam.transform;

        // ---- reaching the goal
        var goalPad = GameObject.Find("GoalPad");
        if (goalPad != null)
        {
            var b = goalPad.GetComponentInChildren<Renderer>().bounds;
            var trig = new GameObject("GoalTrigger");
            trig.transform.position = new Vector3(b.center.x, b.max.y + 1.5f, b.center.z);
            var box = trig.AddComponent<BoxCollider>(); box.size = new Vector3(3, 3, 3); box.isTrigger = true;
            trig.AddComponent<GoalTrigger>();
        }

        EditorSceneManager.SaveScene(scene, Target);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Target, true) };
        Debug.Log("[Lab04Setup] saved " + Target);
    }

    // The pack's doorway is only about 1 m wide, too narrow for the player. Replace it with a 1.5 m x 2.5 m opening
    // built from the pack's own wall piece: a piece on each side plus a lintel across the top.
    static void WidenDoorway()
    {
        const string wallPath = "Assets/Synty/PolygonPrototype/Prefabs/Buildings/Simple/SM_Buildings_Wall_5x3_01.prefab";
        var wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(wallPath);
        if (wallPrefab == null) { Debug.LogWarning("[Lab04Setup] wall prefab not found: " + wallPath); return; }

        GameObject door = null, plainWall = null;
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude))
        {
            if (t.name.Contains("WallDoor")) door = t.gameObject;
            else if (t.name.Contains("SM_Buildings_Wall_5x3") && plainWall == null) plainWall = t.gameObject;
        }
        if (door == null) { Debug.LogWarning("[Lab04Setup] no doorway piece found"); return; }

        var db = door.GetComponentInChildren<Renderer>().bounds;       // the 5 m module the doorway sits in
        var parent = door.transform.parent;
        var mats = plainWall != null ? plainWall.GetComponentInChildren<Renderer>().sharedMaterials : door.GetComponentInChildren<Renderer>().sharedMaterials;
        float x0 = db.min.x, zc = db.center.z, mid = db.center.x;
        Object.DestroyImmediate(door);

        Piece(wallPrefab, parent, mats, x0, 0f, zc, 1.75f / 5f, 1f);                       // left of the opening
        Piece(wallPrefab, parent, mats, mid + 0.75f, 0f, zc, 1.75f / 5f, 1f);             // right of the opening
        Piece(wallPrefab, parent, mats, mid - 0.75f, 2.5f, zc, 1.5f / 5f, 0.5f / 3f);     // lintel: opening is 2.5 m high
    }

    static void Piece(GameObject prefab, Transform parent, Material[] mats, float minX, float minY, float centerZ, float scaleX, float scaleY)
    {
        var g = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        g.transform.SetParent(parent, false);
        g.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        g.transform.position = Vector3.zero;
        var r = g.GetComponentsInChildren<Renderer>(); var b = r[0].bounds; foreach (var x in r) b.Encapsulate(x.bounds);
        g.transform.position = new Vector3(minX - b.min.x, minY - b.min.y, centerZ - b.center.z);
        foreach (var x in r) { var m = new Material[x.sharedMaterials.Length]; for (int i = 0; i < m.Length; i++) m[i] = mats[Mathf.Min(i, mats.Length - 1)]; x.sharedMaterials = m; }
        g.name = "Doorway_Piece";
    }
}
