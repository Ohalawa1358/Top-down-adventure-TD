﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    // Variables //
    public int chunkCountX = 4, chunkCountZ = 3;
    int cellCountX, cellCountZ;
    public HexCell[] cells;
    public Color defaultColor = Color.white;
    HexMesh hexMesh;
    public HexCell cellPrefab;
    public HexGridChunk chunkPrefab;
    HexGridChunk[] chunks;


    // Unity Functions
    private void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateChunks();
        CreateCells();        
    }

    void Start () {
        hexMesh.Triangulate(cells);
    }


    // Public Functions //
    public void Refresh () {
		hexMesh.Triangulate(cells);
	}

    public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    // Private Functions //
    void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}

    void CreateCells() {
        cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z);
			}
		}
    }


    void CreateCell(int x, int z)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z *  (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[x + z * cellCountX] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x,z);
        cell.color = defaultColor;

        //Connect cells to their Left/Right neighbors
        if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[(x + z * cellCountX) - 1]);
		}
        //Connect cells to their lower right/left neighbors
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[(x + z * cellCountX) - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[(x + z * cellCountX) - cellCountX - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[(x + z * cellCountX) - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[(x + z * cellCountX) - cellCountX + 1]);
				}
			}
		}
    }
}
