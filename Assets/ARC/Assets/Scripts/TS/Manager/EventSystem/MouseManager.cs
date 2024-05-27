using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class MouseManager : MonoBehaviour
    {
        public static MouseManager instance = null;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            //-> If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
        }

        public void CusrorVisibility(bool state)
        {
            Cursor.visible = state;
        }
    }
}

