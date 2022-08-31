using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunSingleton<T> : MonoBehaviourPun where T : MonoBehaviourPun
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }
}
