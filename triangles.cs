//display triangular shapes

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//to display the mesh: the game object needs a renderer and a mesh filter to pass the mesh info to the renderer 
//create necessary components automatically
[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]  

//call the update method whenever there're some changes in the scene
[ExecuteInEditMode] 

public class Triangles : MonoBehaviour{
  
  [SerializeField] private Material material;   
  private List<Vector3> vertices;
  private List<int> triangles;
  private MeshFilter meshFilter; 
  private MeshRenderer meshRenderer;
  private Mesh mesh; //store the mesh info - vertices, triangles, normals
  
  void Update(){
  //mesh generating code - gets updated whenever a value is changed in the inspector, gets references 
    meshFilter = GetComponent<MeshFilter>(); 
    meshRenderer = GetComponent<MeshRenderer>(); 
    meshRenderer.material = material; //assign created material to material property and display the model
    
    //init
    vertices = new List<Vector3>();
    triangles = new List<int>();
  }
  private void CreateMesh(){
  
  }
  
}
