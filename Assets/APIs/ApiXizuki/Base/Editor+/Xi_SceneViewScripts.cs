using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XizukiMethods
{
    // USELESS 4 NOW

    /*
       namespace Scene
        {
            public static class Xi_Scene
            {
           

            
            public static void GetRayHitInScene(SceneView scene)
            {
                Event e = Event.current;

                if (e.type == EventType.MouseDown && e.button == 2)
                {
                    Vector3 mousePos = e.mousePosition;
                    float ppp = EditorGUIUtility.pixelsPerPoint;
                    mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
                    mousePos.x *= ppp;

                    Ray ray = scene.camera.ScreenPointToRay(mousePos);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                    }
                    e.Use();
                }

            }

            public static void OnScene(SceneView scene)
            {
                Event e = Event.current;

                if (e.type == EventType.MouseDown && e.button == 2)
                {
                    Vector3 mousePos = e.mousePosition;
                    float ppp = EditorGUIUtility.pixelsPerPoint;
                    mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
                    mousePos.x *= ppp;

                    Ray ray = scene.camera.ScreenPointToRay(mousePos);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        //Do something, ---Example---
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.transform.position = hit.point;
                        Debug.Log("Instantiated Primitive " + hit.point);
                    }
                    e.Use();
                }
            }
        }
        
    }
    */
}