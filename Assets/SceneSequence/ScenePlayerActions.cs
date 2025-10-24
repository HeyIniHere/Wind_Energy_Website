using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScenePlayerActions : MonoBehaviour
{
    [Header("General Settings")]
    public float circleRadius = 10f;
    public float circleSpeed = 20f;
    public float heightOffset = 2f;

    [Header("Scene 2 & 6 Movement Settings")]
    public Vector3 moveTarget = new Vector3(0, 0, 0);
    public float descendAmount = 10f;
    public float moveSpeed = 5f;

    [Header("Scene 3 Settings")]
    public Vector3 scene3StartPos = new Vector3(100, -60, 200);
    public Vector3 scene3EndPos = new Vector3(100, -60, -200);
    public float scene3WaitDuration = 3f;

    [Header("Scene 4 Settings")]
    public Vector3 scene4StartPos = new Vector3(50, 50, 0);
    public Vector3 scene4Target = new Vector3(300, 0, 0);
    public float rotationSpeed = 1f;

    [Header("Scene 6 Prefabs")]
    public List<GameObject> scene6Prefabs; // Drag prefabs (not instances!) here

    private List<GameObject> scene6Instances = new List<GameObject>();

    [Header("Scene Text Display")]
    public TextMeshProUGUI sceneText; 
    public string scene1Text = "Scene 1: Circling the wind turbine...";
    public string scene2Text = "Scene 2: Descending toward target.";
    public string scene3Text = "Scene 3: Side motion across X-axis.";
    public string scene4Text = "Scene 4: Rotating to target.";
    public string scene5Text = "Scene 5: (Reserved)";
    public string scene6Text = "Scene 6: Descending with models visible.";

    private bool isPlaying = false;
    private Coroutine currentRoutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartSequence(PlayScene1(), scene1Text);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartSequence(PlayScene2(false), scene2Text);
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartSequence(PlayScene3(), scene3Text);
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartSequence(PlayScene4(), scene4Text);
        if (Input.GetKeyDown(KeyCode.Alpha5)) StartSequence(PlayScene5(), scene5Text);
        if (Input.GetKeyDown(KeyCode.Alpha6)) StartSequence(PlayScene2(true), scene6Text);
    }

    void StartSequence(IEnumerator routine, string message)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        isPlaying = false;
        ShowSceneText(message);
        currentRoutine = StartCoroutine(routine);
    }

    void ShowSceneText(string message)
    {
        if (sceneText != null)
        {
            sceneText.text = message;
            sceneText.alpha = 1f;
        }
    }

    // Scene 1 — circle around (0,0,0)
    IEnumerator PlayScene1()
    {
        HideScene6Meshes();
        isPlaying = true;
        Vector3 center = Vector3.zero;
        float angle = 0f;
        while (isPlaying)
        {
            angle += circleSpeed * Time.deltaTime;
            if (angle >= 360f) angle -= 360f;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * circleRadius;
            transform.position = center + offset + Vector3.up * heightOffset;
            transform.LookAt(center + Vector3.up * heightOffset);
            yield return null;
        }
    }

    // Scene 2 & 6 — descend movement
    IEnumerator PlayScene2(bool showMeshes)
    {
        if (showMeshes) ShowScene6Meshes();
        else HideScene6Meshes();

        Vector3 center = Vector3.zero;
        Vector3 startPos = new Vector3(moveTarget.x, moveTarget.y + descendAmount, moveTarget.z);
        transform.position = startPos;
        transform.LookAt(center + Vector3.up * heightOffset);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / descendAmount);
            transform.position = Vector3.Lerp(startPos, moveTarget, t);
            transform.LookAt(center + Vector3.up * heightOffset);
            yield return null;
        }

        transform.position = moveTarget;
        transform.LookAt(center + Vector3.up * heightOffset);
    }

    // Scene 3 — move from (100,-60,200) to (100,-60,-200)
    IEnumerator PlayScene3()
    {
        HideScene6Meshes();
        transform.position = scene3StartPos;
        transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up); // face negative x

        yield return new WaitForSeconds(scene3WaitDuration);

        float dist = Vector3.Distance(scene3StartPos, scene3EndPos);
        float duration = dist / moveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(scene3StartPos, scene3EndPos, elapsed / duration);
            yield return null;
        }

        transform.position = scene3EndPos;
    }

    // Scene 4 — go to (50,50,0), rotate to face (300,0,0)
    IEnumerator PlayScene4()
    {
        HideScene6Meshes();
        transform.position = scene4StartPos;
        Quaternion targetRot = Quaternion.LookRotation(scene4Target - scene4StartPos, Vector3.up);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRot;
    }

    // Scene 5 — currently placeholder
    IEnumerator PlayScene5()
    {
        HideScene6Meshes();
        yield return null;
    }

    // Mesh management
    void ShowScene6Meshes()
    {
        if (scene6Instances.Count == 0)
        {
            foreach (var prefab in scene6Prefabs)
            {
                if (prefab)
                {
                    GameObject obj = Instantiate(prefab);
                    scene6Instances.Add(obj);
                }
            }
        }

        foreach (var obj in scene6Instances)
            if (obj) obj.SetActive(true);
    }

    void HideScene6Meshes()
    {
        foreach (var obj in scene6Instances)
            if (obj) obj.SetActive(false);
    }
}
