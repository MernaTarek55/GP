﻿using UnityEngine;

public class ScrollingUVs_Layers : MonoBehaviour
{
    //public int materialIndex = 0;
    public Vector2 uvAnimationRate = new(1.0f, 0.0f);
    public string textureName = "_MainTex";

    private Vector2 uvOffset = Vector2.zero;

    private void LateUpdate()
    {
        uvOffset += uvAnimationRate * Time.deltaTime;
        if (GetComponent<Renderer>().enabled)
        {
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset(textureName, uvOffset);
        }
    }
}