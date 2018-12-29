﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[RequireComponent (typeof(MeshFilter),typeof(MeshRenderer))]
public class InstantPlane2Cube : MonoBehaviour {

    Mesh omesh;
	Mesh newMesh;
	public float initExtrusion=0.1f;

    void Update(){

        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            || (Input.GetMouseButton(0))))
        {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit))
            {
                omesh = GetComponent<MeshFilter>().mesh;
                //omesh = hit.transform.GetComponent<QuadPolygon>().GetComponent<MeshFilter>().mesh;
                newMesh = new Mesh();
                convertSolid();
            }
        }
       
    }

    void Start () {
        //omesh=PlaneData.createMesh(omesh);
		//print("Press A to convert plane into solid");
		//convertSolid();
	}

	void convertSolid(){
		copyData();
		updateMesh();
	}
	void copyData(){
		Debug.Log("Begin Generating Solid Mesh");
		newMesh.vertices= initVertices(omesh);
		newMesh.triangles= initTriangles(omesh);
		//printVertices(newMesh);
	}
	
	public Vector3[] initVertices(Mesh mesh){ //Generate vertices for mesh
		Vector3[] a=new Vector3[mesh.vertexCount*2+mesh.vertexCount*4];
        for(int i=0;i<mesh.vertexCount;i++){
			a[i]=mesh.vertices[i]+new Vector3(0,initExtrusion,0);  //top face vertices
			a[i+mesh.vertexCount]=mesh.vertices[i];  //bottom face vertices
		}
		
		int j=2*mesh.vertexCount;
		for(int i=0;i<mesh.vertexCount;i++){  //0374 1045 2156 3267 for quad
			a[j]=a[i];                         
			if(i==0){
				a[j+1]=a[mesh.vertexCount-1];
				a[j+2]=a[2*mesh.vertexCount-1];
			} 
			else {
				a[j+1]=a[i-1];
				a[j+2]=a[i-1+mesh.vertexCount];
			}
			a[j+3]=a[i+mesh.vertexCount];
			j=j+4;
		}
		return a;
	}

	public int[] initTriangles(Mesh mesh){
		int topTriCount=mesh.vertexCount-2; //number of triangles on top face
		int triCount=4*mesh.vertexCount-4;  //(mesh.vertexCount-2)*2+mesh.vertexCount*2; number of total triangles

		int[] a=new int[triCount*3];
        for(int i=0;i<topTriCount*3;i++){
			a[i]=mesh.triangles[i];  //completes top face
			a[i+topTriCount*3]=mesh.triangles[topTriCount*3-1-i]+4;  //completes bottom face "inverted normal"
		}
		int k=topTriCount*6;
		int l=mesh.vertexCount*2;
		for(int j=0;j<mesh.vertexCount;j++){
            /*
            a[k]=l;           //a
			a[k+1]=l+1;       //b
			a[k+2]=l+3;       //c
			a[k+3]=l+3;       //c
			a[k+4]=l+1;       //b
			a[k+5]=l+2;       //d
            */
            a[k] = l;           //a
            a[k + 1] = l + 3;       //b
            a[k + 2] = l + 1;       //c
            a[k + 3] = l + 3;       //c
            a[k + 4] = l + 2;       //b
            a[k + 5] = l + 1;       //d
            print("completed "+j+" face");
			k=k+6;
			l=l+4;
		}
		return a;
	}

	void updateMesh () {
		omesh.Clear();
		omesh.vertices = newMesh.vertices;
		omesh.triangles = newMesh.triangles;
		omesh.RecalculateNormals();

	}

	public void printVertices(Mesh mesh){ //prints vertex information for testing
		if(mesh.vertices.Length==0) print("No vertex detected!");
        for(int i=0;i<mesh.vertexCount;i++) print("Vertex["+i+"]="+mesh.vertices[i]);
	} 
}
