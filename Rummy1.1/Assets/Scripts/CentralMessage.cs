using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CentralMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _centralMessageTmp;

    private void Awake()
    {
        _centralMessageTmp = GetComponent<TextMeshProUGUI>();
        _centralMessageTmp.rectTransform.localPosition = Vector3.left*3000f;
    }

    public void Message(string message)
    {
        _centralMessageTmp.text = message;
        StartCoroutine(Dissapear());
    }
    
    IEnumerator Dissapear()
    {
        _centralMessageTmp.rectTransform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(1.5f);
        _centralMessageTmp.rectTransform.localPosition = Vector3.left*3000f;
        
    }
}
