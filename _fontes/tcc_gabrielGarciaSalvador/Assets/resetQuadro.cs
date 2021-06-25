using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetQuadro : MonoBehaviour
{
    public NewTexturePainter texturePainter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("coll") || other.gameObject.name.Contains("hands"))
        {
            if (!texturePainter.resettingQuadro) {
                StartCoroutine(texturePainter.resetQuadro());
                }
        }
    }
}
