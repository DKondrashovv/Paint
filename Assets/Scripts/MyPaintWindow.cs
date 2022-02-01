using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MyPaintWindow : EditorWindow
{
    private int _leftBoard;
    private int _topBoard;
    public GameObject _object;
    private Color _paintColor;
    private Color _eraseColor;
    private const int _size = 4;

    private Color[,] Matrix = new Color[_size , _size];
    private Texture2D _texture;
    [MenuItem("Tools/Paint")]
    
    private static void OpenToolsWindow()
    {
        GetWindow<MyPaintWindow>();
    }
    

    private void OnEnable()
    {
        _object = null;
        _leftBoard = 10;
        _topBoard = 100;
        for (var i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                Matrix[i,j] = new Color(Random.value, Random.value, Random.value, 1);
            }
        }

        _paintColor = new Color(Random.value, Random.value, Random.value, 1);
        _eraseColor= new Color(Random.value, Random.value, Random.value, 1);

        _texture = new Texture2D(_size,_size);
    }

    private void OnGUI()
    {
        Event evnt = Event.current;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Toolbar");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Paint Color");
        _paintColor = EditorGUILayout.ColorField(_paintColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Erase Color");
        _eraseColor = EditorGUILayout.ColorField(_eraseColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fill All"))
        {
            FillAllMatrix();
        }
       
#pragma warning disable 618
        _object = (GameObject) EditorGUILayout.ObjectField(_object, typeof(GameObject));
#pragma warning restore 618
        if (_object)
        {
            Selection.objects=EditorUtility.CollectDependencies(new Object[] {_object});
        }
        
        if (GUILayout.Button("Save"))
        {
            if (_object != null)
            {
                SaveGameObject();
            }
            else
            {
                return;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                Rect rect = new Rect(_leftBoard+i*150,_topBoard+j*150, 100, 100);
                GUI.color = Matrix[i,j];
                GUI.DrawTexture(rect, _texture);
                if (evnt.type == EventType.MouseDown)
                {
                    if (evnt.button == 0 && rect.Contains(evnt.mousePosition))
                    {
                        Matrix[i,j] = _paintColor;
                        evnt.Use();
                    }
                    if(evnt.button == 1 && rect.Contains(evnt.mousePosition))
                    {
                        Matrix[i,j] = _eraseColor;
                        evnt.Use();
                    }
                }

            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
    private void SaveGameObject()
    {
        Texture2D _newTexture = new Texture2D(_size, _size);
        _object.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _newTexture;
        for (int i = 0; i < Matrix.GetLength(0);i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
              _newTexture.SetPixel(i,j,Matrix[i,j]);
            }
        }
        _newTexture.Apply();
    }
    
    private void FillAllMatrix()
    {
        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                Matrix[i, j] = _paintColor;
            }
        }
    }
}

