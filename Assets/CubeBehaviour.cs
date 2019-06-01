using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CubeBehaviour : MonoBehaviour
{
    Vector3 oldPos;
    Vector3 destPos;
    Transform destCube;
    int value = 2;
    public static float moveSpeed;
    bool spawning = true;
    public bool merging { get; set; }
    bool animationMerging = false;
    public static float spawnSpeed;
    public bool selfDestruct { get; set; }
    public Texture[] m_MainTexture;
    public Texture[] m_Textures;

    void Start ()
    {
        destPos = transform.position;
        oldPos = transform.position;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        selfDestruct = false;
        merging = false;
    }

    public void SetValue(int val)
    {
        float size = 0.05f;
        if (val >= 1024)
            size = 0.03f;
        else if (val >= 128)
            size = 0.04f;
        Component[] children = gameObject.GetComponentsInChildren(typeof(TextMesh));
        Color col = new Color();
        ColorUtility.TryParseHtmlString("#edeae6", out col);
        foreach (Component child in children)
        {
            TextMesh text = (TextMesh)child;
            text.text = val.ToString();
            text.characterSize = size;
            text.GetComponent<Renderer>().material.color = col;
        }

        value = val;

        int textureIndex = (int)Math.Log(val, 2) - 1;
        GetComponent<Renderer>().material.mainTexture = m_MainTexture[textureIndex];
    }

    public void SetTarget(Vector3 dest, Transform replace = null)
    {
        destPos = dest;
        destCube = replace;
        spawning = false;
        if (replace != null)
            merging = true;
    }
	
	void Update ()
    {
        if (destPos != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime * Mathf.Pow(2, GameBehaviour.moves.Count));
            if ((transform.position - oldPos).magnitude < (transform.position - destPos).magnitude)
            {
                float ratio = (destPos - transform.position).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                float ratio = (transform.position - oldPos).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
        }
        else if (oldPos != transform.position)
        {
            oldPos = transform.position;
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            GameBehaviour.movingCount--;
            if (destCube != null)
            {
                SetValue(2 * value);
                destCube.GetComponent<CubeBehaviour>().selfDestruct = true;
                destCube = null;
                merging = false;
                animationMerging = true;
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                if (value == 2048 && !GameBehaviour.won)
                    GameObject.Find("Game Object").GetComponent<GameBehaviour>().WinGame();
            }
        }
        if (spawning || animationMerging)
        {
            float coeff = (animationMerging ? -0.5f : 1);
            Vector3 newScale = transform.localScale + new Vector3(1, 1, 1) * Time.deltaTime * spawnSpeed * coeff;
            if (coeff * newScale.x <= coeff * 1.3)
                transform.localScale = newScale;
            else
            {
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                spawning = false;
                animationMerging = false;
            }
        }
    }

    void LateUpdate()
    {
        if (selfDestruct)
            Destroy(this.gameObject);
    }
}
